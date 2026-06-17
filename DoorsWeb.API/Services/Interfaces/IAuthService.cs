using DoorsWeb.Shared.Models;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<JwtPair?> SignInAsync(AdminLoginDto dto, CancellationToken ct = default);
        Task<JwtPair?> RefreshAsync(string refreshToken, CancellationToken ct = default);
        Task<JwtPair?> ChangePasswordAsync(string username, string newPassword, CancellationToken ct = default);
        Task<string?> GetDefaultPasswordHintAsync(string username, CancellationToken ct = default);
    }
}
