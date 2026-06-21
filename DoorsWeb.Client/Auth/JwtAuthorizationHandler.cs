using Microsoft.JSInterop;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace DoorsWeb.Client.Auth
{
    // Attaches the stored JWT access token to outgoing API calls. When the API
    // answers 401 (typically an expired access token), it transparently exchanges
    // the refresh token for a new pair and replays the original request once. If that
    // refresh also fails the session is over, so it signs out — which makes the router
    // drop the user to the login page instead of leaving them on a now-broken page.
    public class JwtAuthorizationHandler : DelegatingHandler
    {
        // Serializes refresh attempts so a burst of 401s triggers a single refresh.
        private static readonly SemaphoreSlim RefreshLock = new(1, 1);

        private readonly IJSRuntime _js;
        private readonly IHttpClientFactory _clientFactory;
        private readonly JwtAuthStateProvider _authState;

        public JwtAuthorizationHandler(
            IJSRuntime js, IHttpClientFactory clientFactory, JwtAuthStateProvider authState)
        {
            _js = js;
            _clientFactory = clientFactory;
            _authState = authState;
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

            if (await TryRefreshAsync(cancellationToken))
            {
                response.Dispose();
                await AttachAccessTokenAsync(retry);
                return await base.SendAsync(retry, cancellationToken);
            }

            // Couldn't refresh — the session has expired or been revoked. End it so the
            // app reacts: SignOutAsync clears the tokens and notifies the auth state, which
            // re-runs route authorization and sends the user to login (RedirectToLogin). This
            // is a no-op on the [AllowAnonymous] login page, so a failed sign-in attempt that
            // returns 401 won't bounce the user around — it just surfaces the error there.
            await _authState.SignOutAsync();
            return response;
        }

        private async Task AttachAccessTokenAsync(HttpRequestMessage request)
        {
            var token = await _js.InvokeAsync<string?>("localStorage.getItem", JwtAuthStateProvider.AccessTokenKey);
            if (!string.IsNullOrWhiteSpace(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        private async Task<bool> TryRefreshAsync(CancellationToken ct)
        {
            var tokenBeforeWait = await _js.InvokeAsync<string?>("localStorage.getItem", JwtAuthStateProvider.AccessTokenKey);
            await RefreshLock.WaitAsync(ct);
            try
            {
                // A concurrent request may have already refreshed while we waited.
                var current = await _js.InvokeAsync<string?>("localStorage.getItem", JwtAuthStateProvider.AccessTokenKey);
                if (!string.IsNullOrWhiteSpace(current) && current != tokenBeforeWait)
                    return true;

                var refreshToken = await _js.InvokeAsync<string?>("localStorage.getItem", JwtAuthStateProvider.RefreshTokenKey);
                if (string.IsNullOrWhiteSpace(refreshToken))
                    return false;

                var client = _clientFactory.CreateClient("WebAPI-Refresh");
                using var resp = await client.PostAsJsonAsync("auth/refresh", refreshToken, ct);
                if (!resp.IsSuccessStatusCode)
                    // Refresh token is gone/expired. Returning false makes SendAsync call
                    // SignOutAsync, which clears the tokens and drops the user to login.
                    return false;

                var pair = await resp.Content.ReadFromJsonAsync<RefreshResult>(cancellationToken: ct);
                if (pair?.AccessToken is null)
                    return false;

                await _js.InvokeVoidAsync("localStorage.setItem", JwtAuthStateProvider.AccessTokenKey, pair.AccessToken);
                await _js.InvokeVoidAsync("localStorage.setItem", JwtAuthStateProvider.RefreshTokenKey, pair.RefreshToken);
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
