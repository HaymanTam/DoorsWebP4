using Microsoft.JSInterop;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace DoorsWeb.Client.Auth
{
    // Attaches the stored JWT access token to outgoing API calls. When the API
    // answers 401 (typically an expired access token), it transparently exchanges
    // the refresh token for a new pair and replays the original request once.
    public class JwtAuthorizationHandler : DelegatingHandler
    {
        // Serializes refresh attempts so a burst of 401s triggers a single refresh.
        private static readonly SemaphoreSlim RefreshLock = new(1, 1);

        private readonly IJSRuntime _js;
        private readonly IHttpClientFactory _clientFactory;

        public JwtAuthorizationHandler(IJSRuntime js, IHttpClientFactory clientFactory)
        {
            _js = js;
            _clientFactory = clientFactory;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Clone before sending: the original request can't be reused once sent.
            var retry = await CloneAsync(request);

            await AttachAccessTokenAsync(request);
            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode != HttpStatusCode.Unauthorized)
                return response;

            if (!await TryRefreshAsync(cancellationToken))
                return response;

            response.Dispose();
            await AttachAccessTokenAsync(retry);
            return await base.SendAsync(retry, cancellationToken);
        }

        private async Task AttachAccessTokenAsync(HttpRequestMessage request)
        {
            var token = await _js.InvokeAsync<string?>("localStorage.getItem", "access_token");
            if (!string.IsNullOrWhiteSpace(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        private async Task<bool> TryRefreshAsync(CancellationToken ct)
        {
            var tokenBeforeWait = await _js.InvokeAsync<string?>("localStorage.getItem", "access_token");
            await RefreshLock.WaitAsync(ct);
            try
            {
                // A concurrent request may have already refreshed while we waited.
                var current = await _js.InvokeAsync<string?>("localStorage.getItem", "access_token");
                if (!string.IsNullOrWhiteSpace(current) && current != tokenBeforeWait)
                    return true;

                var refreshToken = await _js.InvokeAsync<string?>("localStorage.getItem", "refresh_token");
                if (string.IsNullOrWhiteSpace(refreshToken))
                    return false;

                var client = _clientFactory.CreateClient("WebAPI-Refresh");
                using var resp = await client.PostAsJsonAsync("auth/refresh", refreshToken, ct);
                if (!resp.IsSuccessStatusCode)
                {
                    // Refresh token is gone/expired: clear tokens so the app drops to login.
                    await _js.InvokeVoidAsync("localStorage.removeItem", "access_token");
                    await _js.InvokeVoidAsync("localStorage.removeItem", "refresh_token");
                    return false;
                }

                var pair = await resp.Content.ReadFromJsonAsync<RefreshResult>(cancellationToken: ct);
                if (pair?.AccessToken is null)
                    return false;

                await _js.InvokeVoidAsync("localStorage.setItem", "access_token", pair.AccessToken);
                await _js.InvokeVoidAsync("localStorage.setItem", "refresh_token", pair.RefreshToken);
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                RefreshLock.Release();
            }
        }

        private static async Task<HttpRequestMessage> CloneAsync(HttpRequestMessage request)
        {
            var clone = new HttpRequestMessage(request.Method, request.RequestUri)
            {
                Version = request.Version
            };

            if (request.Content is not null)
            {
                var bytes = await request.Content.ReadAsByteArrayAsync();
                var content = new ByteArrayContent(bytes);
                foreach (var header in request.Content.Headers)
                    content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                clone.Content = content;
            }

            foreach (var header in request.Headers)
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

            return clone;
        }

        private record RefreshResult(string AccessToken, string RefreshToken);
    }
}
