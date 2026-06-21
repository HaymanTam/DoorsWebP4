using System.Net.Http.Json;
using DoorsWeb.Shared.DTO;
using DoorsWeb.Shared.Enums;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;

namespace DoorsWeb.Client.Services
{
    /// <summary>
    /// App-wide live view of every door's state, used by the navigation problem badge and the Door
    /// Manager's Activity column. It owns its <em>own</em> SignalR connection (deliberately not the
    /// shared <see cref="DoorStateHubClient"/>, which the floorplan disposes on teardown) so the
    /// nav badge keeps updating no matter which page is open.
    ///
    /// Backed by the initial snapshot (GET api/DoorControl/states) plus the "DoorStateChanged" pushes.
    /// </summary>
    public sealed class DoorStatusStore : IAsyncDisposable
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IJSRuntime _js;
        private readonly SemaphoreSlim _startGate = new(1, 1);
        private readonly Dictionary<int, DoorStateDto> _doors = new();
        private IReadOnlyList<PendingCommandDto> _pending = Array.Empty<PendingCommandDto>();

        private HubConnection? _connection;
        private bool _started;

        /// <summary>Raised on the snapshot load and on every pushed change. UI handlers should marshal to the renderer.</summary>
        public event Action? Changed;

        public DoorStatusStore(IHttpClientFactory clientFactory, IJSRuntime js)
        {
            _clientFactory = clientFactory;
            _js = js;
        }

        public bool IsConnected => _connection?.State == HubConnectionState.Connected;

        public IReadOnlyCollection<DoorStateDto> Doors => _doors.Values;

        public DoorStateDto? TryGet(int door) => _doors.TryGetValue(door, out var d) ? d : null;

        /// <summary>Commands that have been sent to controllers but not yet acknowledged (still retrying).</summary>
        public IReadOnlyList<PendingCommandDto> PendingCommands => _pending;

        /// <summary>Total number of outstanding (unacknowledged) commands across all doors.</summary>
        public int PendingCount => _pending.Count;

        /// <summary>The outstanding commands for one door, oldest first.</summary>
        public IReadOnlyList<PendingCommandDto> PendingFor(int door)
            => _pending.Where(p => p.Door == door).ToList();

        /// <summary>Doors not currently in contact with their controller.</summary>
        public int InactiveCount => _doors.Values.Count(d => d.Status == DoorLiveStatus.Offline);

        /// <summary>Doors with an active alarm condition (forced / held / fire / intruder / …).</summary>
        public int AlarmCount => _doors.Values.Count(HasAlarm);

        /// <summary>Doors that need attention — inactive or in alarm (counted once each).</summary>
        public int ProblemCount => _doors.Values.Count(IsProblem);

        /// <summary>Doors needing attention in one site (or all sites when <paramref name="site"/> is null).</summary>
        public int ProblemCountFor(int? site) => DoorsFor(site).Count(IsProblem);

        /// <summary>Doors in alarm in one site (or all sites when <paramref name="site"/> is null).</summary>
        public int AlarmCountFor(int? site) => DoorsFor(site).Count(HasAlarm);

        /// <summary>Offline doors in one site (or all sites when <paramref name="site"/> is null).</summary>
        public int InactiveCountFor(int? site)
            => DoorsFor(site).Count(d => d.Status == DoorLiveStatus.Offline);

        private IEnumerable<DoorStateDto> DoorsFor(int? site)
            => site is int s ? _doors.Values.Where(d => d.Site == s) : _doors.Values;

        /// <summary>True when the door has any active alarm condition.</summary>
        public static bool HasAlarm(DoorStateDto d)
            => d.Status is DoorLiveStatus.ForcedOpen or DoorLiveStatus.HeldOpen
               || (d.Hardware?.Alarms ?? DoorAlarmFlags.None) != DoorAlarmFlags.None;

        /// <summary>True when the door should be flagged as a problem on the nav badge.</summary>
        public static bool IsProblem(DoorStateDto d)
            => d.Status == DoorLiveStatus.Offline || HasAlarm(d);

        /// <summary>
        /// Loads the snapshot and opens the live connection. Idempotent and safe to call from several
        /// components' OnInitialized at once (guarded by a gate). If the snapshot can't be fetched
        /// (e.g. the user lacks Site Settings read) it simply leaves the store empty.
        /// </summary>
        public async Task EnsureStartedAsync()
        {
            if (_started) return;
            await _startGate.WaitAsync();
            try
            {
                if (_started) return;

                var client = _clientFactory.CreateClient("WebAPI");

                // Snapshot first, so the UI has data even if the live hub can't connect.
                try
                {
                    var snapshot = await client.GetFromJsonAsync<List<DoorStateDto>>("api/DoorControl/states") ?? new();
                    _doors.Clear();
                    foreach (var d in snapshot) _doors[d.Door] = d;
                }
                catch
                {
                    // No snapshot (403 for unprivileged users / transient). Leave empty; a later
                    // EnsureStartedAsync call can retry.
                    return;
                }

                // Pending-command snapshot (best-effort; the live "PendingCommandsChanged" push keeps it current).
                try
                {
                    _pending = await client.GetFromJsonAsync<List<PendingCommandDto>>("api/DoorControl/pending")
                        ?? (IReadOnlyList<PendingCommandDto>)Array.Empty<PendingCommandDto>();
                }
                catch
                {
                    // Leave empty; a push will populate it once the connection is up.
                }

                // Live connection (best-effort). Auto-reconnect handles drops once connected.
                try
                {
                    var baseAddress = client.BaseAddress
                        ?? throw new InvalidOperationException("WebAPI client has no BaseAddress.");
                    var hubUrl = new Uri(baseAddress, "eventHub");

                    var connection = new HubConnectionBuilder()
                        .WithUrl(hubUrl, options =>
                        {
                            options.AccessTokenProvider = async () =>
                                await _js.InvokeAsync<string>("localStorage.getItem", "access_token");
                        })
                        .WithAutomaticReconnect()
                        .Build();

                    connection.On<DoorStateDto>("DoorStateChanged", OnDoorStateChanged);
                    connection.On<List<PendingCommandDto>>("PendingCommandsChanged", OnPendingChanged);
                    await connection.StartAsync();
                    _connection = connection;
                }
                catch
                {
                    // Snapshot is loaded and will be shown; live updates just won't flow.
                }

                _started = true;
            }
            finally
            {
                _startGate.Release();
            }

            Changed?.Invoke();
        }

        private void OnDoorStateChanged(DoorStateDto dto)
        {
            _doors[dto.Door] = dto;
            Changed?.Invoke();
        }

        private void OnPendingChanged(List<PendingCommandDto> pending)
        {
            _pending = pending;
            Changed?.Invoke();
        }

        /// <summary>
        /// Cancels a single pending command so the server stops retrying it. The store updates from
        /// the resulting "PendingCommandsChanged" push, but we also drop it locally for snappy UI.
        /// </summary>
        public async Task ClearCommandAsync(Guid id)
        {
            try
            {
                var client = _clientFactory.CreateClient("WebAPI");
                await client.DeleteAsync($"api/DoorControl/pending/{id}");
            }
            catch
            {
                // Ignore; the list stays as-is and the next push reconciles it.
            }
        }

        /// <summary>Cancels every pending command, or only those for one door when <paramref name="door"/> is supplied.</summary>
        public async Task ClearAllAsync(int? door = null)
        {
            try
            {
                var client = _clientFactory.CreateClient("WebAPI");
                var url = door is int d ? $"api/DoorControl/pending?door={d}" : "api/DoorControl/pending";
                await client.DeleteAsync(url);
            }
            catch
            {
                // Ignore; the next push reconciles it.
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_connection is not null)
            {
                try { await _connection.DisposeAsync(); }
                catch { /* circuit/connection already gone */ }
                _connection = null;
            }
            _startGate.Dispose();
        }
    }
}
