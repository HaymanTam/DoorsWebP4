using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace DoorsWeb.API.Services.Protocol
{
    public sealed class UdpProtocolOptions
    {
        public int ListenPort { get; set; } = 10013;
        public int DefaultSendPort { get; set; } = 10012;

        /// <summary>How long to wait for a controller's reply before freeing its channel for the
        /// next message. Bounds how long a silent/offline controller can hold up its own channel.
        /// Applies per controller IP.</summary>
        public int ResponseTimeoutMs { get; set; } = 500;
    }

    /// <summary>
    /// Background service that binds a single UDP socket to the listen port (10013), continuously receives and
    /// validates inbound <see cref="ProtocolPacket"/>s, and serves as the send handle (<see cref="IUdpProtocolService"/>).
    /// Outbound packets target the controllers' port (DefaultSendPort, 10012); their replies come back to this
    /// socket's source port, so the single bound socket handles both receiving and sending.
    /// </summary>
    public sealed class UdpProtocolService : BackgroundService, IUdpProtocolService
    {
        private readonly ILogger<UdpProtocolService> _logger;
        private readonly UdpProtocolOptions _options;
        private readonly UdpClient _udp;

        // One channel per controller IP. Enforces "one message outstanding at a time": a send holds
        // the channel until the controller replies (or ResponseTimeoutMs elapses), so we never send
        // a second message before the previous one is answered. Keyed by IP (not IP:port) so all
        // traffic to a controller shares one timeline; different controllers are independent.
        private readonly ConcurrentDictionary<IPAddress, ControllerChannel> _channels = new();
        private readonly TimeSpan _responseTimeout;

        public event Action<ProtocolPacket, IPEndPoint>? PacketReceived;

        public UdpProtocolService(ILogger<UdpProtocolService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _options = configuration.GetSection("UdpProtocol").Get<UdpProtocolOptions>() ?? new UdpProtocolOptions();
            _responseTimeout = TimeSpan.FromMilliseconds(Math.Max(1, _options.ResponseTimeoutMs));
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

                        // Free the outstanding request's channel when the controller replies. If the
                        // sender asked for strict matching, only a packet its predicate accepts (the
                        // exact command reply, e.g. a B,2 for a B,1 ping) counts — any other packet
                        // from this controller is ignored so the request keeps waiting for its answer.
                        if (_channels.TryGetValue(result.RemoteEndPoint.Address, out var channel))
                        {
                            var pending = channel.Pending;
                            if (pending is not null)
                            {
                                var match = channel.Match;
                                if (match is null || match(packet))
                                    pending.TrySetResult();
                            }
                        }

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

        public Task SendAsync(ProtocolPacket packet, string host, int? port = null, Func<ProtocolPacket, bool>? matchReply = null, CancellationToken cancellationToken = default)
            => SendAsync(packet, new IPEndPoint(ResolveAddress(host), port ?? _options.DefaultSendPort), matchReply, cancellationToken);

        public async Task SendAsync(ProtocolPacket packet, IPEndPoint endpoint, Func<ProtocolPacket, bool>? matchReply = null, CancellationToken cancellationToken = default)
        {
            // Queue behind any message already outstanding to this controller, then send and wait
            // for its reply before the channel is freed for the next message.
            var channel = _channels.GetOrAdd(endpoint.Address, static _ => new ControllerChannel());
            await channel.Gate.WaitAsync(cancellationToken);
            await SendAndAwaitReplyAsync(channel, packet, endpoint, matchReply, cancellationToken);
        }

        public Task<bool> TrySendAsync(ProtocolPacket packet, string host, int? port = null, Func<ProtocolPacket, bool>? matchReply = null, CancellationToken cancellationToken = default)
            => TrySendAsync(packet, new IPEndPoint(ResolveAddress(host), port ?? _options.DefaultSendPort), matchReply, cancellationToken);

        public async Task<bool> TrySendAsync(ProtocolPacket packet, IPEndPoint endpoint, Func<ProtocolPacket, bool>? matchReply = null, CancellationToken cancellationToken = default)
        {
            // Best-effort: if a message is already outstanding to this controller, drop rather than
            // queue (used for pings — a stale ping stuck behind another message is worthless).
            var channel = _channels.GetOrAdd(endpoint.Address, static _ => new ControllerChannel());
            if (!await channel.Gate.WaitAsync(0, cancellationToken))
                return false;
            await SendAndAwaitReplyAsync(channel, packet, endpoint, matchReply, cancellationToken);
            return true;
        }

        // Precondition: the caller holds channel.Gate. Sends the packet, waits for the controller's
        // reply (or the response timeout), then releases the gate for the next message. When
        // matchReply is non-null only a packet it accepts frees the channel (strict command↔reply).
        private async Task SendAndAwaitReplyAsync(ControllerChannel channel, ProtocolPacket packet, IPEndPoint endpoint, Func<ProtocolPacket, bool>? matchReply, CancellationToken cancellationToken)
        {
            try
            {
                // RunContinuationsAsynchronously: the receive loop completes this, and we don't want
                // the waiting send's continuation to run inline on the listener thread.
                var reply = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
                // Publish the matcher before the pending completion: the receive loop reads Pending
                // first (acquire) and only then Match, so a non-null Pending guarantees Match is set.
                channel.Match = matchReply;
                channel.Pending = reply;

                var bytes = packet.ToBytes();
                await _udp.Client.SendToAsync(bytes, SocketFlags.None, endpoint, cancellationToken);
                _logger.LogDebug("TX {Endpoint}: {Packet}", endpoint, packet);

                try
                {
                    await reply.Task.WaitAsync(_responseTimeout, cancellationToken);
                }
                catch (TimeoutException)
                {
                    _logger.LogDebug("No reply from {Endpoint} within {Timeout}ms.",
                        endpoint, _responseTimeout.TotalMilliseconds);
                }
            }
            finally
            {
                channel.Pending = null;
                channel.Match = null;
                channel.Gate.Release();
            }
        }

        private static IPAddress ResolveAddress(string host)
            => IPAddress.TryParse(host, out var ip) ? ip : Dns.GetHostAddresses(host).First();

        public override void Dispose()
        {
            _udp.Dispose();
            base.Dispose();
        }

        // Per-controller-IP channel: a 1-slot gate enforcing a single outstanding message, plus the
        // completion the listener signals when the controller replies.
        private sealed class ControllerChannel
        {
            public readonly SemaphoreSlim Gate = new(1, 1);
            // Written by the sender (under the gate), read by the listener thread; volatile for visibility.
            public volatile TaskCompletionSource? Pending;
            // Optional strict-match predicate for the outstanding request: when set, only an inbound
            // packet it accepts completes Pending. Set before Pending, cleared after; volatile so the
            // listener sees it once it observes a non-null Pending.
            public volatile Func<ProtocolPacket, bool>? Match;
        }
    }
}
