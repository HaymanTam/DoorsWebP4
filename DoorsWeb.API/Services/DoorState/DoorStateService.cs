using System.Collections.Concurrent;
using System.Globalization;
using System.Net;
using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.API.Services.Protocol;
using DoorsWeb.Shared.DTO;
using DoorsWeb.Shared.Enums;
using Microsoft.AspNetCore.SignalR;

namespace DoorsWeb.API.Services.DoorState
{
    /// <summary>
    /// Singleton that owns the live state of every door. It subscribes to the UDP listener,
    /// turns inbound controller packets into <see cref="DoorLiveStatus"/> changes, keeps an
    /// in-memory cache, sweeps quiet doors to Offline, and pushes every change to floorplan
    /// clients over the SignalR <see cref="EventHub"/> ("DoorStateChanged").
    /// </summary>
    public sealed class DoorStateService : BackgroundService, IDoorStateService
    {
        private const string DoorStateChanged = "DoorStateChanged";
        private static readonly TimeSpan SweepInterval = TimeSpan.FromSeconds(5);
        private static readonly TimeSpan MapRefreshInterval = TimeSpan.FromSeconds(60);

        private readonly ILogger<DoorStateService> _logger;
        private readonly IUdpProtocolService _udp;
        private readonly IHubContext<EventHub> _hub;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ISystemSettingsService _settings;
        private readonly IEventLogService _eventLog;

        private readonly object _gate = new();
        private readonly ConcurrentDictionary<int, DoorStateDto> _states = new();
        private readonly ConcurrentDictionary<int, DateTime> _lastSeenUtc = new();
        // Controller address -> door number. Swapped wholesale on refresh so reads are lock-free.
        private volatile IReadOnlyDictionary<uint, int> _addressToDoor =
            new Dictionary<uint, int>();
        // Last status bytes written to the DB per door, so we only persist on an actual change
        // (a ping arrives every couple of seconds — we must not write to T_Doors that often).
        private readonly ConcurrentDictionary<int, (byte Status1, byte Status2)> _lastPersisted = new();

        public DoorStateService(
            ILogger<DoorStateService> logger,
            IUdpProtocolService udp,
            IHubContext<EventHub> hub,
            IServiceScopeFactory scopeFactory,
            ISystemSettingsService settings,
            IEventLogService eventLog)
        {
            _logger = logger;
            _udp = udp;
            _hub = hub;
            _scopeFactory = scopeFactory;
            _settings = settings;
            _eventLog = eventLog;
            _udp.PacketReceived += OnPacketReceived;
        }

        public IReadOnlyCollection<DoorStateDto> GetSnapshot()
        {
            lock (_gate)
            {
                return _states.Values.Select(Clone).ToList();
            }
        }

        public Task ApplyLocalAsync(int door, DoorLiveStatus status, string? eventName, CancellationToken ct = default)
        {
            // The operator just acted on this door, so treat it as reachable for one offline window;
            // a real controller's ping/event will reconcile, an absent one will revert to Offline.
            var changed = Apply(door, status, eventName, DateTime.UtcNow, markSeen: true);
            return BroadcastAsync(changed, ct);
        }

        public async Task RefreshDoorMapAsync(CancellationToken ct = default)
        {
            List<(int Door, string? ControllerId, string? Name, int? Site)> rows;
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DoorsEnterpriseContext>();
                rows = (await db.Doors.AsNoTracking()
                        .Select(d => new { d.Door, d.ControllerId, d.Name, d.Site })
                        .ToListAsync(ct))
                    .Select(d => (d.Door, d.ControllerId, d.Name, d.Site))
                    .ToList();
            }

            var map = new Dictionary<uint, int>(rows.Count);
            lock (_gate)
            {
                var present = new HashSet<int>(rows.Count);
                foreach (var r in rows)
                {
                    present.Add(r.Door);
                    if (!_states.TryGetValue(r.Door, out var dto))
                    {
                        dto = new DoorStateDto { Door = r.Door, Status = DoorLiveStatus.Offline };
                        _states[r.Door] = dto;
                        _lastSeenUtc[r.Door] = DateTime.MinValue;
                    }
                    dto.Name = r.Name ?? string.Empty;
                    dto.Site = r.Site;

                    if (uint.TryParse(r.ControllerId, out var addr))
                        map[addr] = r.Door;
                }

                // Drop doors that no longer exist.
                foreach (var gone in _states.Keys.Where(k => !present.Contains(k)).ToList())
                {
                    _states.TryRemove(gone, out _);
                    _lastSeenUtc.TryRemove(gone, out _);
                    _lastPersisted.TryRemove(gone, out _);
                }
            }
            _addressToDoor = map;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Load the door map, retrying until the database is reachable.
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await RefreshDoorMapAsync(stoppingToken);
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Door-state map load failed; retrying in {Seconds}s.", SweepInterval.TotalSeconds);
                    await Task.Delay(SweepInterval, stoppingToken);
                }
            }

            var lastMapRefresh = DateTime.UtcNow;
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(SweepInterval, stoppingToken);
                    await SweepOfflineAsync(stoppingToken);

                    if (DateTime.UtcNow - lastMapRefresh >= MapRefreshInterval)
                    {
                        await RefreshDoorMapAsync(stoppingToken);
                        lastMapRefresh = DateTime.UtcNow;
                    }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Door-state sweep failed.");
                }
            }
        }

        // ---- inbound packet handling -------------------------------------------------

        private void OnPacketReceived(ProtocolPacket packet, IPEndPoint endpoint)
        {
            // PacketReceived fires on the UDP listener thread; do the work off-thread.
            _ = HandlePacketAsync(packet, endpoint);
        }

        private async Task HandlePacketAsync(ProtocolPacket packet, IPEndPoint endpoint)
        {
            try
            {
                if (!_addressToDoor.TryGetValue(packet.SourceAddress, out var door))
                    return; // packet from an unknown / unmapped controller

                if (packet.CommandGroup == DoorStatusDecoder.EventLogGroup &&
                    packet.CommandNumber == DoorStatusDecoder.EventLogReply &&
                    packet.Data.Length > DoorStatusDecoder.EventTypeIndex)
                {
                    int eventType = packet.Data[DoorStatusDecoder.EventTypeIndex];
                    var status = DoorStatusDecoder.FromEventType(eventType);
                    var changed = Apply(door, status, DoorStatusDecoder.EventName(eventType), DateTime.UtcNow, markSeen: true);
                    await BroadcastAsync(changed, CancellationToken.None);
                }
                else if (packet.CommandGroup == DoorStatusDecoder.PingGroup &&
                         packet.CommandNumber == DoorStatusDecoder.PingReply)
                {
                    // Ping reply (B,2): the status bytes carry the live relay + alarm state.
                    var ping = DoorStatusDecoder.DecodePing(packet.Data, DateTime.UtcNow);
                    var changed = ApplyPing(door, ping);
                    await BroadcastAsync(changed, CancellationToken.None);
                    await PersistPingAsync(door, ping);

                    // The reply also reports how many event-log entries the controller is holding
                    // unread; pull them when there are any (a no-op if a drain is already running).
                    if (ping.Hardware.UnreadLogCount > 0)
                        _eventLog.EnsureDrain(door, packet.SourceAddress, endpoint.Address.ToString());
                }
                else
                {
                    // Any other well-formed packet just proves the door is alive.
                    var changed = Apply(door, status: null, eventName: null, eventUtc: null, markSeen: true);
                    await BroadcastAsync(changed, CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process packet from 0x{Address:X8}.", packet.SourceAddress);
            }
        }

        private async Task SweepOfflineAsync(CancellationToken ct)
        {
            var timeout = OfflineTimeout();
            var now = DateTime.UtcNow;
            var toBroadcast = new List<DoorStateDto>();

            lock (_gate)
            {
                foreach (var (door, dto) in _states)
                {
                    if (dto.Status == DoorLiveStatus.Offline) continue;
                    var lastSeen = _lastSeenUtc.TryGetValue(door, out var t) ? t : DateTime.MinValue;
                    if (now - lastSeen > timeout)
                    {
                        dto.Status = DoorLiveStatus.Offline;
                        toBroadcast.Add(Clone(dto));
                    }
                }
            }

            foreach (var dto in toBroadcast)
                await BroadcastAsync(dto, ct);
        }

        // ---- state mutation ----------------------------------------------------------

        /// <summary>Applies a change under the lock and returns a clone to broadcast, or null if nothing changed.</summary>
        private DoorStateDto? Apply(int door, DoorLiveStatus? status, string? eventName, DateTime? eventUtc, bool markSeen)
        {
            lock (_gate)
            {
                if (!_states.TryGetValue(door, out var dto))
                {
                    dto = new DoorStateDto { Door = door, Status = DoorLiveStatus.Offline };
                    _states[door] = dto;
                }

                if (markSeen) _lastSeenUtc[door] = DateTime.UtcNow;

                bool changed = false;

                var effective = status ?? dto.Status;
                // Coming online from Offline with no explicit reading: assume secured until told otherwise.
                if (markSeen && dto.Status == DoorLiveStatus.Offline && status is null)
                    effective = DoorLiveStatus.Locked;

                if (effective != dto.Status)
                {
                    dto.Status = effective;
                    changed = true;
                }

                if (eventName is not null)
                {
                    dto.LastEventName = eventName;
                    dto.LastEventUtc = eventUtc ?? DateTime.UtcNow;
                    changed = true;
                }

                return changed ? Clone(dto) : null;
            }
        }

        /// <summary>
        /// Applies a decoded ping reply (relay/alarm/firmware/voltage) under the lock. Always caches
        /// the freshest hardware snapshot (voltage, last-polled, logs) but only returns a clone to
        /// broadcast when something a viewer would notice changed — status, a relay, the alarm set,
        /// the firmware string, or the very first reply (hardware was previously unknown).
        /// </summary>
        private DoorStateDto? ApplyPing(int door, DoorStatusDecoder.PingResult ping)
        {
            lock (_gate)
            {
                if (!_states.TryGetValue(door, out var dto))
                {
                    dto = new DoorStateDto { Door = door, Status = DoorLiveStatus.Offline };
                    _states[door] = dto;
                }

                _lastSeenUtc[door] = DateTime.UtcNow;

                var prev = dto.Hardware;
                bool material =
                    dto.Status != ping.Status ||
                    prev is null ||
                    prev.RelayA != ping.Hardware.RelayA ||
                    prev.RelayB != ping.Hardware.RelayB ||
                    prev.Alarms != ping.Hardware.Alarms ||
                    prev.FirmwareVersion != ping.Hardware.FirmwareVersion;

                dto.Status = ping.Status;
                dto.Hardware = ping.Hardware; // always cache the latest reading

                return material ? Clone(dto) : null;
            }
        }

        // Writes the live status bytes (and, faithful to the legacy connector, the controller clock
        // and unread-log count) back to T_Doors — but only when the status bytes actually changed,
        // since a ping lands every couple of seconds. Never touches Updated, which is the door
        // *configuration's* last-edit time shown in the Door Manager, not a live-status timestamp.
        private async Task PersistPingAsync(int door, DoorStatusDecoder.PingResult ping)
        {
            var current = (ping.Hardware.Status1, ping.Hardware.Status2);
            if (_lastPersisted.TryGetValue(door, out var last) && last == current)
                return;

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<DoorsEnterpriseContext>();

                string? rtcDate = ping.ControllerTimeLocal?.ToString("yyyy-MM-d", CultureInfo.InvariantCulture);
                string? rtcTime = ping.ControllerTimeLocal?.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
                int? logCount = ping.Hardware.UnreadLogCount;

                await db.Doors
                    .Where(d => d.Door == door)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(d => d.Status1, ping.Hardware.Status1)
                        .SetProperty(d => d.Status2, ping.Hardware.Status2)
                        .SetProperty(d => d.LogCount, d => logCount ?? d.LogCount)
                        .SetProperty(d => d.Rtcdate, d => rtcDate ?? d.Rtcdate)
                        .SetProperty(d => d.Rtctime, d => rtcTime ?? d.Rtctime));

                _lastPersisted[door] = current;
            }
            catch (Exception ex)
            {
                // A failed write must not stop the live feed; we'll retry on the next status change.
                _logger.LogWarning(ex, "Failed to persist live status for door {Door}.", door);
            }
        }

        private Task BroadcastAsync(DoorStateDto? dto, CancellationToken ct)
            => dto is null ? Task.CompletedTask : _hub.Clients.All.SendAsync(DoorStateChanged, dto, ct);

        private TimeSpan OfflineTimeout()
        {
            int ping = _settings.Get().ControllerCommunication.PingIntervalSeconds;
            if (ping < 1) ping = 1;
            // Tolerate a few missed pings before declaring a door offline.
            return TimeSpan.FromSeconds(Math.Max(ping * 4, 12));
        }

        private static DoorStateDto Clone(DoorStateDto d) => new()
        {
            Door = d.Door,
            Name = d.Name,
            Site = d.Site,
            Status = d.Status,
            LastEventName = d.LastEventName,
            LastEventUtc = d.LastEventUtc,
            // Hardware is replaced wholesale on every ping (never mutated in place), so sharing
            // the reference with the broadcast clone is safe.
            Hardware = d.Hardware
        };

        public override void Dispose()
        {
            _udp.PacketReceived -= OnPacketReceived;
            base.Dispose();
        }
    }
}
