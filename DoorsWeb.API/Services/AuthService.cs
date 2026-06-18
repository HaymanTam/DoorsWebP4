using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DoorsWeb.API.Services
{
    public record JwtPair(
        string AccessToken,
        DateTimeOffset AccessTokenExpiresAt,
        string RefreshToken,
        DateTimeOffset RefreshTokenExpiresAt,
        bool MustChangePassword);

    public class AuthService : IAuthService
    {
        // Default password seeded for the "admin" account; users on this password
        // are forced to change it on sign-in.
        private const string DefaultPassword = "654321";

        private readonly DoorsEnterpriseContext _context;
        private readonly IConfiguration _configuration;
        private readonly IPwHashService _pwHash;

        public AuthService(DoorsEnterpriseContext context, IConfiguration configuration, IPwHashService pwHash)
        {
            _context = context;
            _configuration = configuration;
            _pwHash = pwHash;
        }

        public async Task<JwtPair?> SignInAsync(AdminLoginDto dto, CancellationToken ct = default)
        {
            var user = await _context.TUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Description == dto.Username, ct);

            if (user is null || !VerifyPassword(dto.Password, user.Password))
                return null;

            return GenerateJwtPair(user);
        }

        public async Task<JwtPair?> RefreshAsync(string refreshToken, CancellationToken ct = default)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!));
            ClaimsPrincipal principal;
            try
            {
                principal = new JwtSecurityTokenHandler().ValidateToken(refreshToken, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out _);
            }
            catch
            {
                return null;
            }

            if (principal.FindFirstValue("type") != "refresh")
                return null;

            var username = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (username is null)
                return null;

            var user = await _context.TUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Description == username, ct);

            return user is null ? null : GenerateJwtPair(user);
        }

        public async Task<JwtPair?> ChangePasswordAsync(string username, string newPassword, CancellationToken ct = default)
        {
            var user = await _context.TUsers
                .FirstOrDefaultAsync(u => u.Description == username, ct);

            if (user is null)
                return null;

            user.Password = _pwHash.Hash(newPassword);
            await _context.SaveChangesAsync(ct);

            return GenerateJwtPair(user);
        }

        // Returns the default password as a login hint while the account still uses
        // it; null once the user has set their own password.
        public async Task<string?> GetDefaultPasswordHintAsync(string username, CancellationToken ct = default)
        {
            var user = await _context.TUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Description == username, ct);

            return user is not null && StoredIsDefault(user.Password) ? DefaultPassword : null;
        }

        // Legacy T_Users may still hold plaintext passwords; bcrypt hashes start
        // with "$2". Verify against whichever format is stored.
        private bool VerifyPassword(string entered, string stored) =>
            IsBcryptHash(stored) ? _pwHash.Verify(entered, stored) : entered == stored;

        private bool StoredIsDefault(string stored) =>
            IsBcryptHash(stored) ? _pwHash.Verify(DefaultPassword, stored) : stored == DefaultPassword;

        private static bool IsBcryptHash(string stored) =>
            stored.StartsWith("$2", StringComparison.Ordinal);

        // Force a password reset on first sign-in when the stored secret is still the seeded
        // default, OR is not a bcrypt hash — i.e. a legacy plaintext password carried over from a
        // restored DoorsClient database. Once the user sets a new password it is bcrypt-hashed, so
        // this returns false on subsequent logins.
        private bool RequiresPasswordReset(string stored) =>
            !IsBcryptHash(stored) || StoredIsDefault(stored);

        private JwtPair GenerateJwtPair(TUsers user)
        {
            var mustChangePassword = RequiresPasswordReset(user.Password);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var accessExpiry = DateTimeOffset.UtcNow.AddMinutes(
                double.Parse(_configuration["Jwt:AccessTokenExpiryMinutes"] ?? "15"));
            var refreshExpiry = DateTimeOffset.UtcNow.AddDays(
                double.Parse(_configuration["Jwt:RefreshTokenExpiryDays"] ?? "7"));

            var accessToken = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims:
                [
                    new Claim(JwtRegisteredClaimNames.Sub, user.Description),
                    new Claim("Administrator", user.Administrator ? "true" : "false"),
                    new Claim(ClaimTypes.Role, user.Administrator ? "Administrator" : "User"),
                    new Claim("must_change_password", mustChangePassword ? "true" : "false"),
                ],
                expires: accessExpiry.UtcDateTime,
                signingCredentials: creds);

            var refreshToken = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims:
                [
                    new Claim(JwtRegisteredClaimNames.Sub, user.Description),
                    new Claim("type", "refresh"),
                ],
                expires: refreshExpiry.UtcDateTime,
                signingCredentials: creds);

            var handler = new JwtSecurityTokenHandler();
            return new JwtPair(
                handler.WriteToken(accessToken),
                accessExpiry,
                handler.WriteToken(refreshToken),
                refreshExpiry,
                mustChangePassword);
        }
    }
}
