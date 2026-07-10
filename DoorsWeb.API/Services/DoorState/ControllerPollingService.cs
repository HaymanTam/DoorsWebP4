using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.API.Services.Protocol;

namespace DoorsWeb.API.Services.DoorState
{
    /// <summary>
    /// Background loop that pings every door controller over UDP (protocol command B,1) on the
    /// interval configured in System Settings (<c>ControllerCommunication.PingIntervalSeconds</c>).
    /// Each controller answers with a B,2 reply, which <see cref="DoorStateService"/> decodes into
    /// live relay / alarm state. The list of controllers (address + IP) is cached and refreshed
    /// periodically so adding or editing a door is picked up without a restart.
    /// </summary>
    public sealed class ControllerPollingService : BackgroundService
    {
        private static readonly TimeSpan TargetRefreshInterval = TimeSpan.FromSeconds(60);
        private static readonly TimeSpan LoadRetryDelay = TimeSpan.FromSeconds(5);

        private readonly ILogger<ControllerPollingService> _logger;
        private readonly IUdpProtocolService _udp;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ISystemSettingsService _settings;

        // Swapped wholesale on refresh so the poll loop reads it without locking.
        private volatile IReadOnlyList<PollTarget> _targets = Array.Empty<PollTarget>();

        private readonly record struct PollTarget(uint Address, string Host);

        public ControllerPollingService(
            ILogger<ControllerPollingService> logger,
            IUdpProtocolService udp,
            IServiceScopeFactory scopeFactory,
            ISystemSettingsService settings)
        {
            _logger = logger;
            _udp = udp;
            _scopeFactory = scopeFactory;
            _settings = settings;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Load the controller list, retrying until the database is reachable.
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await RefreshTargetsAsync(stoppingToken);
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Controller poll target load failed; retrying in {Seconds}s.",
                        LoadRetryDelay.TotalSeconds);
                    await Task.Delay(LoadRetryDelay, stoppingToken);
                }
            }

            var lastRefresh = DateTime.UtcNow;
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await PollOnceAsync(stoppingToken);

                    if (DateTime.UtcNow - lastRefresh >= TargetRefreshInterval)
                    {
                        await RefreshTargetsAsync(stoppingToken);
                        lastRefresh = DateTime.UtcNow;
                    }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Controller poll cycle failed.");
                }

                await Task.Delay(PingInterval(), stoppingToken);
            }
        }

        // Sends one B,1 ping to every known controller. Because each ping now waits (up to the
        // response timeout) for its own strict B,2 reply, the controllers are pinged concurrently:
        // the per-IP channel still guarantees one outstanding message per controller, and different
        // controllers are independent, so a slow or offline controller can't hold up the rest.
        private Task PollOnceAsync(CancellationToken ct)
        {
            var targets = _targets;
            if (targets.Count == 0) return Task.CompletedTask;

            var sends = new List<Task>(targets.Count);
            foreach (var target in targets)
            {
                if (ct.IsCancellationRequested) break;
                sends.Add(PingAsync(target, ct));
            }
            return Task.WhenAll(sends);
        }

        // Pings a single controller. A failure (or cancellation) to one door must not stop the rest,
        // so everything is wrapped and logged at debug here.
        private async Task PingAsync(PollTarget target, CancellationToken ct)
        {
            try
            {
                var packet = new ProtocolPacket
                {
                    DestinationAddress = target.Address,
                    SourceAddress = 0, // PC
                    CommandGroup = DoorStatusDecoder.PingGroup,
                    CommandNumber = DoorStatusDecoder.PingRequest,
                    Data = Array.Empty<byte>()
                };
                // Drop (don't queue) the ping if a message is already outstanding to this
                // controller — a ping waiting behind another message is stale by the time it lands.
                // Strict match: only a B,2 reply from this controller's own address frees the
                // channel, so an unrelated inbound packet (e.g. an event-log D,3) can't be
                // mistaken for the ping's answer.
                var address = target.Address;
                var sent = await _udp.TrySendAsync(packet, target.Host,
                    matchReply: p => p.CommandGroup == DoorStatusDecoder.PingGroup
                                  && p.CommandNumber == DoorStatusDecoder.PingReply
                                  && p.SourceAddress == address,
                    cancellationToken: ct);
                if (!sent)
                    _logger.LogDebug("Ping to controller 0x{Address:X8} ({Host}) skipped; a message is already in flight.",
                        target.Address, target.Host);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Ping to controller 0x{Address:X8} ({Host}) failed.",
                    target.Address, target.Host);
            }
        }

        private async Task RefreshTargetsAsync(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DoorsEnterpriseContext>();

            var rows = await db.Doors.AsNoTracking()
                .Where(d => d.DoorIpaddress != null && d.DoorIpaddress != "")
                .Select(d => new { d.ControllerId, d.DoorIpaddress })
                .ToListAsync(ct);

            var targets = new List<PollTarget>(rows.Count);
            foreach (var r in rows)
            {
                var host = r.DoorIpaddress!.Trim();
                if (host.Length == 0) continue;
                uint.TryParse(r.ControllerId, out var address); // 0 if the controller id isn't numeric
                targets.Add(new PollTarget(address, host));
            }
            _targets = targets;
        }

        private TimeSpan PingInterval()
        {
            int seconds = _settings.Get().ControllerCommunication.PingIntervalSeconds;
            return TimeSpan.FromSeconds(Math.Max(seconds, 1));
        }
    }
}
