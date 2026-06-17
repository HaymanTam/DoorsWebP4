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

        /// <summary>Sends a packet to <paramref name="host"/>. When <paramref name="port"/> is null the configured default (10012) is used.</summary>
        Task SendAsync(ProtocolPacket packet, string host, int? port = null, CancellationToken cancellationToken = default);

        /// <summary>Sends a packet to an explicit endpoint.</summary>
        Task SendAsync(ProtocolPacket packet, IPEndPoint endpoint, CancellationToken cancellationToken = default);
    }
}
