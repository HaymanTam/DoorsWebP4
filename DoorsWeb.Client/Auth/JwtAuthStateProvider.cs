using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;

namespace DoorsWeb.Client.Auth
{
    public class JwtAuthStateProvider : AuthenticationStateProvider, IDisposable
    {
        public const string AccessTokenKey = "access_token";
        public const string RefreshTokenKey = "refresh_token";

        // Fire the proactive logout a hair before the real expiry so the user is on the login
        // page by the time the token is actually dead (avoids a last in-flight call racing it).
        private static readonly TimeSpan ExpirySkew = TimeSpan.FromSeconds(2);

        private readonly IJSRuntime _js;
        private static readonly AuthenticationState Anonymous =
            new(new ClaimsPrincipal(new ClaimsIdentity()));

        // One-shot timer that signs the user out the moment the access token expires, even if they
        // are idle (no API call / navigation to trip the lazy checks). (Re)armed on every auth-state
        // evaluation — i.e. on login and whenever NotifyAuthStateChanged runs — and cancelled on sign-out.
        private Timer? _expiryTimer;

        public JwtAuthStateProvider(IJSRuntime js) => _js = js;

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _js.InvokeAsync<string?>("localStorage.getItem", AccessTokenKey);
            if (string.IsNullOrWhiteSpace(token))
            {
                CancelExpiryTimer();
                return Anonymous;
            }

            var claims = ParseClaimsFromJwt(token);
            var expiresAt = GetExpiry(claims);

            if (expiresAt is DateTimeOffset exp)
            {
                if (exp <= DateTimeOffset.UtcNow)
                {
                    // Already expired: drop the timer and report anonymous so the router redirects.
                    CancelExpiryTimer();
                    return Anonymous;
                }

                // Still valid: schedule the proactive logout for the instant it expires.
                ScheduleExpiry(exp);
            }
            else
            {
                // No exp claim — nothing to schedule against.
                CancelExpiryTimer();
            }

            return new AuthenticationState(
                new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt")));
        }

        /// <summary>Reads the JWT "exp" (seconds since epoch) as a UTC instant, or null if absent/unparseable.</summary>
        private static DateTimeOffset? GetExpiry(IEnumerable<Claim> claims)
        {
            var expClaim = claims.FirstOrDefault(c => c.Type == "exp");
            return expClaim != null && long.TryParse(expClaim.Value, out var exp)
                ? DateTimeOffset.FromUnixTimeSeconds(exp)
                : null;
        }

        // (Re)arm the one-shot expiry timer. Disposes any previous timer first so repeated auth-state
        // evaluations don't stack callbacks.
        private void ScheduleExpiry(DateTimeOffset expiresAt)
        {
            CancelExpiryTimer();

            var due = expiresAt - DateTimeOffset.UtcNow - ExpirySkew;
            if (due < TimeSpan.Zero) due = TimeSpan.Zero;

            _expiryTimer = new Timer(_ => _ = OnExpiryElapsedAsync(), null, due, Timeout.InfiniteTimeSpan);
        }

        private void CancelExpiryTimer()
        {
            _expiryTimer?.Dispose();
            _expiryTimer = null;
        }

        // Fired by the timer at (token expiry − skew). The user chose "immediately redirect" on
        // timeout, so this signs out rather than attempting a silent refresh. Guarded against a race:
        // if a reactive refresh replaced the token with a later expiry in the meantime, re-arm for the
        // new expiry instead of logging out a session that is actually still valid.
        private async Task OnExpiryElapsedAsync()
        {
            try
            {
                var token = await _js.InvokeAsync<string?>("localStorage.getItem", AccessTokenKey);
                if (string.IsNullOrWhiteSpace(token))
                {
                    CancelExpiryTimer();
                    return;
                }

                if (GetExpiry(ParseClaimsFromJwt(token)) is DateTimeOffset exp
                    && exp - ExpirySkew > DateTimeOffset.UtcNow)
                {
                    ScheduleExpiry(exp);
                    return;
                }

                await SignOutAsync();
            }
            catch
            {
                // Interop can throw if the circuit is tearing down; nothing useful to do here.
            }
        }

        public void NotifyAuthStateChanged() =>
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

        /// <summary>
        /// Ends the current session: clears the stored tokens and notifies the app so the router
        /// re-evaluates authorization and drops the user to the login page (via RedirectToLogin).
        /// Safe to call when already signed out — it's an idempotent "the session is over" signal.
        /// </summary>
        public async Task SignOutAsync()
        {
            CancelExpiryTimer();
            try
            {
                await _js.InvokeVoidAsync("localStorage.removeItem", AccessTokenKey);
                await _js.InvokeVoidAsync("localStorage.removeItem", RefreshTokenKey);
            }
            catch
            {
                // Ignore JS interop failures — we still raise the state-changed notification below.
            }
            NotifyAuthStateChanged();
        }

        public void Dispose() => CancelExpiryTimer();

        private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var padded = (payload.Length % 4) switch
            {
                2 => payload + "==",
                3 => payload + "=",
                _ => payload
            };
            var jsonBytes = Convert.FromBase64String(padded);
            var kvps = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonBytes) ?? [];
            return kvps.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
        }
    }
}
