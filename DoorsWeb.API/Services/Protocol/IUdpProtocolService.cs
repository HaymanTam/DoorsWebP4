using System.Net;

namespace DoorsWeb.API.Services.Protocol
{
    /// <summary>
    /// Send handle for the controller UDP protocol. Inject this anywhere a packet needs to be sent.
    /// The same underlying socket also runs the background listener; subscribe to
    /// <see cref="PacketReceived"/> to be notified of inbound packets.
    /// </summary>
    public interface IUdpProtocolService
    {
        /// <summary>Raised on the listener thread for every well-formed inbound packet.</summary>
        event Action<ProtocolPacket, IPEndPoint>? PacketReceived;

        /// <summary>Sends a packet to <paramref name="host"/> and waits for the controller's reply
        /// (or the configured response timeout) before returning. Only one message is outstanding to
        /// a given controller at a time; concurrent sends to the same controller queue. When
        /// <paramref name="port"/> is null the configured default (10012) is used.
        /// <para><paramref name="matchReply"/> makes the wait <i>strict</i>: the channel is released
        /// only by an inbound packet the predicate accepts (e.g. the exact command reply for this
        /// request). When null, any well-formed packet from the controller releases the channel.</para></summary>
        Task SendAsync(ProtocolPacket packet, string host, int? port = null, Func<ProtocolPacket, bool>? matchReply = null, CancellationToken cancellationToken = default);

        /// <summary>Sends a packet to an explicit endpoint. See <see cref="SendAsync(ProtocolPacket, string, int?, Func{ProtocolPacket, bool}, CancellationToken)"/> for the wait/queue and strict-match semantics.</summary>
        Task SendAsync(ProtocolPacket packet, IPEndPoint endpoint, Func<ProtocolPacket, bool>? matchReply = null, CancellationToken cancellationToken = default);

        /// <summary>Best-effort send: if a message is already outstanding to this controller, returns
        /// <c>false</c> without sending (used for pings, which are pointless to queue). Otherwise
        /// behaves like <see cref="SendAsync(ProtocolPacket, string, int?, Func{ProtocolPacket, bool}, CancellationToken)"/>
        /// (including the strict <paramref name="matchReply"/> semantics) and returns <c>true</c>.</summary>
        Task<bool> TrySendAsync(ProtocolPacket packet, string host, int? port = null, Func<ProtocolPacket, bool>? matchReply = null, CancellationToken cancellationToken = default);

        /// <summary>Best-effort send to an explicit endpoint. See <see cref="TrySendAsync(ProtocolPacket, string, int?, Func{ProtocolPacket, bool}, CancellationToken)"/>.</summary>
        Task<bool> TrySendAsync(ProtocolPacket packet, IPEndPoint endpoint, Func<ProtocolPacket, bool>? matchReply = null, CancellationToken cancellationToken = default);
    }
}
