using DoorsWeb.Shared.DTO;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;

namespace DoorsWeb.Client.Services
{
    /// <summary>
    /// Thin wrapper around the SignalR connection to the API's <c>/eventHub</c>. Carries the live
    /// "NewEvent" pushes to the events page. The JWT is supplied via the access-token provider
    /// (read from localStorage), since the hub requires authentication.
    /// </summary>
    public sealed class EventHubClient : IAsyncDisposable
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IJSRuntime _js;
        private HubConnection? _connection;

        /// <summary>Raised whenever the server pushes a new event.</summary>
        public event Action<EventDto>? NewEvent;

        public EventHubClient(IHttpClientFactory clientFactory, IJSRuntime js)
        {
            _clientFactory = clientFactory;
            _js = js;
        }

        public bool IsConnected => _connection?.State == HubConnectionState.Connected;

        /// <summary>Opens the connection (idempotent). Safe to call from a page's OnInitialized.</summary>
        public async Task StartAsync()
        {
            if (_connection is not null) return;

            // Resolve the API base address from the configured "WebAPI" client.
            var baseAddress = _clientFactory.CreateClient("WebAPI").BaseAddress
                ?? throw new InvalidOperationException("WebAPI client has no BaseAddress.");
            var hubUrl = new Uri(baseAddress, "eventHub");

            _connection = new HubConnectionBuilder()
                .WithUrl(hubUrl, options =>
                {
                    options.AccessTokenProvider = async () =>
                        await _js.InvokeAsync<string>("localStorage.getItem", "access_token");
                })
                .WithAutomaticReconnect()
                .Build();

            _connection.On<EventDto>("NewEvent", dto => NewEvent?.Invoke(dto));

            await _connection.StartAsync();
        }

        public async ValueTask DisposeAsync()
        {
            if (_connection is not null)
            {
                try { await _connection.DisposeAsync(); }
                catch { /* circuit/connection already gone */ }
                _connection = null;
            }
        }
    }
}
