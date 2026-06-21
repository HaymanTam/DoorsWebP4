using System.Collections.Concurrent;
using System.Net;
using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.API.Services.Protocol;
using DoorsWeb.Shared.DTO;
using Microsoft.AspNetCore.SignalR;

namespace DoorsWeb.API.Services.DoorState
{
    /// <summary>
    /// Drains a controller's unread event log whenever a ping reply (B,2) reports unread entries.
    /// The connector pulls each entry with the legacy request/reply/ack handshake (Coms.bas):
    /// send D,1 ("read oldest unread"), receive D,3 (one entry), record + broadcast it, send D,2
    /// (ack — advances the controller's queue), and repeat until the controller answers D,4
    /// ("Event Log Data End"). This service is the single sender of the D,2 ack.
    /// </summary>
    public interface IEventLogService
    {
        /// <summary>
        /// Ensures the unread event log of <paramref name="door"/>'s controller is being drained.
        /// Safe to call on every ping: a drain already in flight for the door is a no-op. The drain
        /// stops when the controller reports no more entries, on a reply timeout, or at a safety cap;
        /// a still-unread count on the next ping simply re-triggers it.
        /// </summary>
        void EnsureDrain(int door, uint controllerAddress, string host);
    }

    public sealed class EventLogService : IEventLogService, IDisposable
    {
        private const string NewEvent = "NewEvent";
        private const byte PcSourceAddress = 1;        // the legacy connector identifies the PC as address 1
        private const int MaxEntriesPerDrain = 500;    // safety cap so a stuck queue can't loop forever
        private static readonly byte[] ReadOldestUnread = { 0x01, 0x00 }; // D,1 data (legacy Read_Log_Event)
        private static readonly TimeSpan ReplyTimeout = TimeSpan.FromSeconds(5);

        private readonly IUdpProtocolService _udp;
        private readonly IHubContext<EventHub> _hub;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<EventLogService> _logger;

        // Doors with a drain in flight (dedup: at most one drain per door at a time).
        private readonly ConcurrentDictionary<int, byte> _draining = new();
        // Controller address -> the drain currently awaiting that controller's next D,3 / D,4 reply.
        private readonly ConcurrentDictionary<uint, TaskCompletionSource<ProtocolPacket>> _waiters = new();

        public EventLogService(
            IUdpProtocolService udp,
            IHubContext<EventHub> hub,
            IServiceScopeFactory scopeFactory,
            ILogger<EventLogService> logger)
        {
            _udp = udp;
            _hub = hub;
            _scopeFactory = scopeFactory;
            _logger = logger;
            _udp.PacketReceived += OnPacketReceived;
        }

        public void EnsureDrain(int door, uint controllerAddress, string host)
        {
            if (string.IsNullOrWhiteSpace(host)) return;
            if (!_draining.TryAdd(door, 0)) return; // a drain for this door is already running

            // Fire-and-forget off the UDP listener thread; always release the per-door claim.
            _ = Task.Run(async () =>
            {
                try
                {
                    await DrainAsync(door, controllerAddress, host.Trim());
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Event-log drain for door {Door} failed.", door);
                }
                finally
                {
                    _draining.TryRemove(door, out _);
                }
            });
        }

        // Fires on the UDP listener thread for every inbound packet; we only care about event-log
        // replies, which complete the drain's pending wait. The lookup + TrySetResult is cheap and
        // (with RunContinuationsAsynchronously) won't run drain work on the listener thread.
        private void OnPacketReceived(ProtocolPacket packet, IPEndPoint endpoint)
        {
            if (packet.CommandGroup != DoorStatusDecoder.EventLogGroup) return;
            if (packet.CommandNumber != DoorStatusDecoder.EventLogReply &&
                packet.CommandNumber != DoorStatusDecoder.EventLogEnd) return;

            if (_waiters.TryRemove(packet.SourceAddress, out var tcs))
                tcs.TrySetResult(packet);
        }

        private async Task DrainAsync(int door, uint controllerAddress, string host)
        {
            for (int i = 0; i < MaxEntriesPerDrain; i++)
            {
                var reply = await RequestNextAsync(controllerAddress, host);
                if (reply is null) return;                                       // timed out — next ping resumes
                if (reply.CommandNumber == DoorStatusDecoder.EventLogEnd) return; // no more unread entries
                if (reply.CommandNumber != DoorStatusDecoder.EventLogReply) return;

                // Record (best-effort) then ack to advance the controller's queue — the legacy
                // connector acks unconditionally, so a failed store must not stall the whole queue.
                await RecordAndBroadcastAsync(door, reply.Data);
                await SendAckAsync(controllerAddress, host);
            }

            _logger.LogInformation("Event-log drain for door {Door} hit the {Cap}-entry cap; will resume on the next ping.",
                door, MaxEntriesPerDrain);
        }

        // Registers a waiter, sends D,1, and waits for the controller's D,3 / D,4 reply (or times out).
        private async Task<ProtocolPacket?> RequestNextAsync(uint controllerAddress, string host)
        {
            var tcs = new TaskCompletionSource<ProtocolPacket>(TaskCreationOptions.RunContinuationsAsynchronously);
            _waiters[controllerAddress] = tcs; // register before sending so a fast reply isn't missed
            try
            {
                await _udp.SendAsync(new ProtocolPacket
                {
                    DestinationAddress = controllerAddress,
                    SourceAddress = PcSourceAddress,
                    CommandGroup = DoorStatusDecoder.EventLogGroup,
                    CommandNumber = DoorStatusDecoder.EventLogRequest,
                    Data = ReadOldestUnread
                }, host);

                return await tcs.Task.WaitAsync(ReplyTimeout);
            }
            catch (TimeoutException)
            {
                return null;
            }
            finally
            {
                _waiters.TryRemove(controllerAddress, out _);
            }
        }

        private Task SendAckAsync(uint controllerAddress, string host)
            => _udp.SendAsync(new ProtocolPacket
            {
                DestinationAddress = controllerAddress,
                SourceAddress = PcSourceAddress,
                CommandGroup = DoorStatusDecoder.EventLogGroup,
                CommandNumber = DoorStatusDecoder.EventLogAck,
                Data = Array.Empty<byte>()
            }, host);

        // Decodes one D,3 entry, inserts it into T_Events, and pushes it to clients as "NewEvent".
        // Failures are logged and swallowed so the caller can still ack and keep draining.
        private async Task RecordAndBroadcastAsync(int door, byte[] data)
        {
            var rec = DoorStatusDecoder.DecodeEvent(data);
            int cardForDb = rec.CardNumber is >= 0 and <= int.MaxValue ? (int)rec.CardNumber : 0;
            var when = rec.TimestampValid ? rec.TimestampLocal : DateTime.Now;

            try
            {
                EventDto? dto;
                using (var scope = _scopeFactory.CreateScope())
                {
                    var events = scope.ServiceProvider.GetRequiredService<IEventService>();
                    dto = await events.RecordAsync(door, when, cardForDb, rec.CardId, rec.ReaderId, rec.EventType);
                }

                if (dto is not null)
                    await _hub.Clients.All.SendAsync(NewEvent, dto);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to record event for door {Door} (type {Type}).", door, rec.EventType);
            }
        }

        public void Dispose() => _udp.PacketReceived -= OnPacketReceived;
    }
}
