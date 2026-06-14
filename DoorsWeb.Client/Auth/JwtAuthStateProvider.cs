using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Security.Claims;
using System.Text.Json;

namespace DoorsWeb.Client.Auth
{
    public class JwtAuthStateProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime _js;
        private static readonly AuthenticationState Anonymous =
            new(new ClaimsPrincipal(new ClaimsIdentity()));

        public JwtAuthStateProvider(IJSRuntime js) => _js = js;

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _js.InvokeAsync<string?>("localStorage.getItem", "access_token");
            if (string.IsNullOrWhiteSpace(token))
                return Anonymous;

            var claims = ParseClaimsFromJwt(token);

            var expClaim = claims.FirstOrDefault(c => c.Type == "exp");
            if (expClaim != null && long.TryParse(expClaim.Value, out var exp))
            {
                if (DateTimeOffset.FromUnixTimeSeconds(exp) < DateTimeOffset.UtcNow)
                    return Anonymous;
            }

            return new AuthenticationState(
                new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt")));
        }

        public void NotifyAuthStateChanged() =>
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

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
