using DoorsWeb.Shared.Models;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<JwtPair?> SignInAsync(AdminLoginDto dto, CancellationToken ct = default);
        Task<JwtPair?> RefreshAsync(string refreshToken, CancellationToken ct = default);
        Task<JwtPair?> ChangePasswordAsync(string username, string newPassword, CancellationToken ct = default);
        Task<string?> GetDefaultPasswordHintAsync(string username, CancellationToken ct = default);

        /// <summary>
        /// First-run convenience hint: the username + default password of an as-yet-unconfigured
        /// Super account (the seeded "admin" still on its default password). Returns null once that
        /// account's password has been changed, or when no such account exists — so the hint disappears
        /// as soon as the system has been set up.
        /// </summary>
        Task<DefaultLoginHint?> GetFirstRunHintAsync(CancellationToken ct = default);
    }
}
