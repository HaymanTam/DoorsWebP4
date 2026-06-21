using System.Net;
using DoorsWeb.API.Services.Protocol;
using DoorsWeb.Shared.DTO;
using Microsoft.AspNetCore.SignalR;

namespace DoorsWeb.API.Services.DoorState
{
    /// <summary>
    /// Tracks operator commands (unlock / lock / momentary release, and lockdown) that have been
    /// sent to a controller but not yet acknowledged, and resends them on a fixed interval until the
    /// controller answers with a "command received" ack (group 0x11, command 2 = 0x01) or an operator
    /// clears them. Pings and event-log traffic are never tracked here — only deliberate commands.
    ///
    /// Acks are correlated to the most recent outstanding command for the controller's address
    /// (FIFO), matching the legacy connector which kept a single Last_command_address per controller
    /// and did not match on the block-sequence number. The current list is pushed to clients over the
    /// EventHub as "PendingCommandsChanged" whenever it changes.
    ///
    /// The store is in-memory only: a process restart drops anything still outstanding.
    /// </summary>
    public interface IPendingCommandService
    {
        /// <summary>
        /// Registers <paramref name="packet"/> as a pending command for <paramref name="door"/>,
        /// sends it once immediately, and keeps resending it until the controller acks or it is
        /// cleared. <paramref name="controllerAddress"/> is the destination/controller address used
        /// to correlate the ack; <paramref name="host"/> is the controller's IP.
        /// </summary>
        Task EnqueueAsync(int door, string? doorName, uint controllerAddress, string host,
            ProtocolPacket packet, DoorCommandAction action, DoorRelay relay, string description,
            CancellationToken ct = default);

        /// <summary>Snapshot of everything still outstanding (oldest first).</summary>
        IReadOnlyList<PendingCommandDto> GetSnapshot();

        /// <summary>Clears a single pending command. Returns true if it was present.</summary>
        bool Clear(Guid id);

        /// <summary>
        /// Clears every pending command, or only those for <paramref name="door"/> when supplied.
        /// Returns the number removed.
        /// </summary>
        int ClearAll(int? door = null);
    }

    public sealed class PendingCommandService : BackgroundService, IPendingCommandService
    {
        private const string PendingChanged = "PendingCommandsChanged";
        private static readonly TimeSpan RetryInterval = TimeSpan.FromSeconds(5);
        private static readonly TimeSpan TickInterval = TimeSpan.FromSeconds(1);

        private readonly IUdpProtocolService _udp;
        private readonly IHubContext<EventHub> _hub;
        private readonly ILogger<PendingCommandService> _logger;

        // Insertion-ordered so the oldest command for an address is the first match (FIFO ack).
        private readonly object _gate = new();
        private readonly List<PendingCommand> _pending = new();

        public PendingCommandService(
            IUdpProtocolService udp,
            IHubContext<EventHub> hub,
            ILogger<PendingCommandService> logger)
        {
            _udp = udp;
            _hub = hub;
            _logger = logger;
            _udp.PacketReceived += OnPacketReceived;
        }

        public async Task EnqueueAsync(int door, string? doorName, uint controllerAddress, string host,
            ProtocolPacket packet, DoorCommandAction action, DoorRelay relay, string description,
            CancellationToken ct = default)
        {
            var cmd = new PendingCommand
            {
                Id = Guid.NewGuid(),
                Door = door,
                DoorName = doorName,
                ControllerAddress = controllerAddress,
                Host = host,
                Packet = packet,
                Action = action,
                Relay = relay,
                Description = description,
                CreatedUtc = DateTime.UtcNow
            };

            // Register before sending so an ack that arrives between the send and the add isn't lost.
            lock (_gate) _pending.Add(cmd);

            await SendAsync(cmd, ct);
            await BroadcastAsync();
        }

        public IReadOnlyList<PendingCommandDto> GetSnapshot()
        {
            lock (_gate) return _pending.Select(c => c.ToDto()).ToList();
        }

        public bool Clear(Guid id)
        {
            bool removed;
            lock (_gate) removed = _pending.RemoveAll(c => c.Id == id) > 0;
            if (removed) _ = BroadcastAsync();
            return removed;
        }

        public int ClearAll(int? door = null)
        {
            int removed;
            lock (_gate)
            {
                if (door is int d)
                {
                    removed = _pending.RemoveAll(c => c.Door == d);
                }
                else
                {
                    removed = _pending.Count;
                    _pending.Clear();
                }
            }
            if (removed > 0) _ = BroadcastAsync();
            return removed;
        }

        // Background retry loop: every tick, resend any command whose retry interval has elapsed.
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try { await Task.Delay(TickInterval, stoppingToken); }
                catch (OperationCanceledException) { break; }

                var now = DateTime.UtcNow;
                List<PendingCommand> due;
                lock (_gate)
                {
                    due = _pending
                        .Where(c => c.LastAttemptUtc is null || now - c.LastAttemptUtc.Value >= RetryInterval)
                        .ToList();
                }

                if (due.Count == 0) continue;

                foreach (var cmd in due)
                    await SendAsync(cmd, stoppingToken);

                await BroadcastAsync();
            }
        }

        // Fires on the UDP listener thread for every inbound packet; we only act on a command ack,
        // which resolves (removes) the oldest outstanding command for that controller's address.
        private void OnPacketReceived(ProtocolPacket packet, IPEndPoint endpoint)
        {
            if (packet.CommandGroup != DoorStatusDecoder.AckGroup) return;
            if (packet.CommandNumber != DoorStatusDecoder.AckReply) return;

            PendingCommand? resolved = null;
            lock (_gate)
            {
                int idx = _pending.FindIndex(c => c.ControllerAddress == packet.SourceAddress);
                if (idx >= 0)
                {
                    resolved = _pending[idx];
                    _pending.RemoveAt(idx);
                }
            }

            if (resolved is not null)
            {
                _logger.LogInformation(
                    "Pending command {Id} ({Description}) for door {Door} acknowledged by controller {Address}.",
                    resolved.Id, resolved.Description, resolved.Door, packet.SourceAddress);
                _ = BroadcastAsync();
            }
        }

        private async Task SendAsync(PendingCommand cmd, CancellationToken ct)
        {
            try
            {
                await _udp.SendAsync(cmd.Packet, cmd.Host, cancellationToken: ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to (re)send pending command {Id} for door {Door}.", cmd.Id, cmd.Door);
                return;
            }

            lock (_gate)
            {
                cmd.Attempts++;
                cmd.LastAttemptUtc = DateTime.UtcNow;
            }
        }

        private Task BroadcastAsync() => _hub.Clients.All.SendAsync(PendingChanged, GetSnapshot());

        public override void Dispose()
        {
            _udp.PacketReceived -= OnPacketReceived;
            base.Dispose();
        }

        // Wraps the client-facing DTO with the wire detail needed to resend and correlate acks.
        private sealed class PendingCommand
        {
            public required Guid Id { get; init; }
            public required int Door { get; init; }
            public string? DoorName { get; init; }
            public required uint ControllerAddress { get; init; }
            public required string Host { get; init; }
            public required ProtocolPacket Packet { get; init; }
            public required DoorCommandAction Action { get; init; }
            public required DoorRelay Relay { get; init; }
            public required string Description { get; init; }
            public required DateTime CreatedUtc { get; init; }
            public int Attempts { get; set; }
            public DateTime? LastAttemptUtc { get; set; }

            public PendingCommandDto ToDto() => new()
            {
                Id = Id,
                Door = Door,
                DoorName = DoorName,
                Action = Action,
                Relay = Relay,
                Description = Description,
                Attempts = Attempts,
                CreatedUtc = CreatedUtc,
                LastAttemptUtc = LastAttemptUtc
            };
        }
    }
}
