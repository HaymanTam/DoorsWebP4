using DoorsWeb.API.Services.DoorState;
using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.API.Services.Protocol;
using DoorsWeb.Shared.DTO;
using DoorsWeb.Shared.Enums;

namespace DoorsWeb.API.Services
{
    /// <summary>
    /// Sends "5,4 Trigger Channel A/B" commands to door controllers over UDP and updates the live
    /// state so the floorplan reflects the action immediately. Channel A is treated as the door lock.
    /// </summary>
    public class DoorCommandService : IDoorCommandService
    {
        // 5,4 Trigger Channel A/B.
        private const byte TriggerGroup = 0x05;
        private const byte TriggerNumber = 0x04;
        private const byte ChannelLockA = 0x01;
        private const byte ModeTimed = 0x00;        // release for D2 seconds then auto-relock
        private const byte ModeOpenForever = 0x01;  // release and hold open
        private const byte ModeClose = 0x02;        // secure / lock
        private const int DefaultReleaseSeconds = 5;

        private readonly DoorsEnterpriseContext _context;
        private readonly IUdpProtocolService _udp;
        private readonly IDoorStateService _doorState;
        private readonly ILogger<DoorCommandService> _logger;

        public DoorCommandService(
            DoorsEnterpriseContext context,
            IUdpProtocolService udp,
            IDoorStateService doorState,
            ILogger<DoorCommandService> logger)
        {
            _context = context;
            _udp = udp;
            _doorState = doorState;
            _logger = logger;
        }

        public async Task<DoorCommandResult> SendAsync(int door, DoorCommandRequest request, CancellationToken ct = default)
        {
            var e = await _context.Doors.AsNoTracking()
                .FirstOrDefaultAsync(d => d.Door == door, ct);
            if (e is null)
                return new DoorCommandResult(DoorCommandOutcome.DoorNotFound, null);

            if (string.IsNullOrWhiteSpace(e.DoorIpaddress))
                return new DoorCommandResult(DoorCommandOutcome.NoIpAddress, null);

            var (mode, seconds) = ResolveMode(request, e.ReleaseTime);
            var packet = BuildTrigger(e.ControllerId, mode, seconds);

            await _udp.SendAsync(packet, e.DoorIpaddress.Trim(), cancellationToken: ct);
            _logger.LogInformation("Sent {Action} to door {Door} ({Ip}).", request.Action, door, e.DoorIpaddress);

            var (status, label) = OptimisticState(request.Action);
            await _doorState.ApplyLocalAsync(door, status, label, ct);

            var state = _doorState.GetSnapshot().FirstOrDefault(s => s.Door == door);
            return new DoorCommandResult(DoorCommandOutcome.Sent, state);
        }

        public async Task<int> LockdownAsync(int? site, CancellationToken ct = default)
        {
            var query = _context.Doors.AsNoTracking()
                .Where(d => d.DoorIpaddress != null && d.DoorIpaddress != "");
            if (site is int s)
                query = query.Where(d => d.Site == s);

            var doors = await query
                .Select(d => new { d.Door, d.ControllerId, d.DoorIpaddress })
                .ToListAsync(ct);

            int commanded = 0;
            foreach (var d in doors)
            {
                try
                {
                    var packet = BuildTrigger(d.ControllerId, ModeClose, 0);
                    await _udp.SendAsync(packet, d.DoorIpaddress!.Trim(), cancellationToken: ct);
                    await _doorState.ApplyLocalAsync(d.Door, DoorLiveStatus.Locked, "Lockdown", ct);
                    commanded++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lockdown: failed to command door {Door}.", d.Door);
                }
            }

            _logger.LogWarning("Lockdown issued for {Count} door(s){Scope}.",
                commanded, site is int site2 ? $" in site {site2}" : " (all sites)");
            return commanded;
        }

        // ---- helpers -----------------------------------------------------------------

        private static (byte mode, byte seconds) ResolveMode(DoorCommandRequest request, int? releaseTime) => request.Action switch
        {
            DoorCommandAction.Unlock => (ModeOpenForever, (byte)0),
            DoorCommandAction.Lock => (ModeClose, (byte)0),
            DoorCommandAction.MomentaryRelease => (ModeTimed, ClampSeconds(request.Seconds ?? releaseTime ?? DefaultReleaseSeconds)),
            _ => (ModeClose, (byte)0)
        };

        private static byte ClampSeconds(int seconds) => (byte)Math.Clamp(seconds, 1, 255);

        private static ProtocolPacket BuildTrigger(string? controllerId, byte mode, byte seconds)
        {
            uint.TryParse(controllerId, out var address);
            return new ProtocolPacket
            {
                DestinationAddress = address,
                SourceAddress = 0, // PC
                CommandGroup = TriggerGroup,
                CommandNumber = TriggerNumber,
                Data = new[] { ChannelLockA, mode, seconds }
            };
        }

        private static (DoorLiveStatus status, string label) OptimisticState(DoorCommandAction action) => action switch
        {
            DoorCommandAction.Unlock => (DoorLiveStatus.Unlocked, "Unlock (command)"),
            DoorCommandAction.Lock => (DoorLiveStatus.Locked, "Lock (command)"),
            DoorCommandAction.MomentaryRelease => (DoorLiveStatus.Unlocked, "Release (command)"),
            _ => (DoorLiveStatus.Locked, "Command")
        };
    }
}
