using System.Net;
using System.Net.Sockets;

namespace DoorsWeb.API.Services.Protocol
{
    public sealed class UdpProtocolOptions
    {
        public int ListenPort { get; set; } = 10012;
        public int DefaultSendPort { get; set; } = 10012;
    }

    /// <summary>
    /// Background service that binds a single UDP socket to the listen port, continuously receives and
    /// validates inbound <see cref="ProtocolPacket"/>s, and serves as the send handle (<see cref="IUdpProtocolService"/>).
    /// Replies from controllers arrive on the same socket, so listening and sending share one port.
    /// </summary>
    public sealed class UdpProtocolService : BackgroundService, IUdpProtocolService
    {
        private readonly ILogger<UdpProtocolService> _logger;
        private readonly UdpProtocolOptions _options;
        private readonly UdpClient _udp;

        public event Action<ProtocolPacket, IPEndPoint>? PacketReceived;

        public UdpProtocolService(ILogger<UdpProtocolService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _options = configuration.GetSection("UdpProtocol").Get<UdpProtocolOptions>() ?? new UdpProtocolOptions();
            _udp = new UdpClient(_options.ListenPort);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("UDP protocol listener started on port {Port}.", _options.ListenPort);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = await _udp.ReceiveAsync(stoppingToken);

                    if (ProtocolPacket.TryParse(result.Buffer, out var packet, out var error) && packet is not null)
                    {
                        _logger.LogDebug("RX {Endpoint}: {Packet}", result.RemoteEndPoint, packet);
                        try
                        {
                            PacketReceived?.Invoke(packet, result.RemoteEndPoint);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "A PacketReceived handler threw for packet from {Endpoint}.", result.RemoteEndPoint);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Discarded malformed packet from {Endpoint}: {Error}", result.RemoteEndPoint, error);
                    }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while receiving on UDP port {Port}.", _options.ListenPort);
                }
            }

            _logger.LogInformation("UDP protocol listener stopped.");
        }

        public Task SendAsync(ProtocolPacket packet, string host, int? port = null, CancellationToken cancellationToken = default)
            => SendAsync(packet, new IPEndPoint(ResolveAddress(host), port ?? _options.DefaultSendPort), cancellationToken);

        public async Task SendAsync(ProtocolPacket packet, IPEndPoint endpoint, CancellationToken cancellationToken = default)
        {
            var bytes = packet.ToBytes();
            await _udp.Client.SendToAsync(bytes, SocketFlags.None, endpoint, cancellationToken);
            _logger.LogDebug("TX {Endpoint}: {Packet}", endpoint, packet);
        }

        private static IPAddress ResolveAddress(string host)
            => IPAddress.TryParse(host, out var ip) ? ip : Dns.GetHostAddresses(host).First();

        public override void Dispose()
        {
            _udp.Dispose();
            base.Dispose();
        }
    }
}
