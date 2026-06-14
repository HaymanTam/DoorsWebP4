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
        DateTimeOffset RefreshTokenExpiresAt);

    public class AuthService
    {
        private readonly IAdminService _adminService;
        private readonly IConfiguration _configuration;

        public AuthService(IAdminService adminService, IConfiguration configuration)
        {
            _adminService = adminService;
            _configuration = configuration;
        }

        public async Task<Result<JwtPair>> SignInAsync(
            AdminLoginDto dto, CancellationToken ct = default)
        {
            var result = await _adminService.LoginAsync(dto, ct);
            if (!result.IsSuccess)
                return Result<JwtPair>.Failure(result.Error!);
            return Result<JwtPair>.Success(GenerateJwtPair(result.Value!));
        }

        public async Task<Result<JwtPair>> RefreshAsync(
            string refreshToken, CancellationToken ct = default)
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
                return Result<JwtPair>.Failure(new Error("Invalid or expired refresh token.", ErrorType.Unauthorized));
            }

            if (principal.FindFirstValue("type") != "refresh")
                return Result<JwtPair>.Failure(new Error("Invalid token type.", ErrorType.Unauthorized));

            var username = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (username is null)
                return Result<JwtPair>.Failure(new Error("Invalid refresh token.", ErrorType.Unauthorized));

            var admin = await _adminService.GetByUsernameAsync(username, ct);
            if (admin is null)
                return Result<JwtPair>.Failure(new Error("Admin not found.", ErrorType.NotFound));

            return Result<JwtPair>.Success(GenerateJwtPair(admin));
        }

        private JwtPair GenerateJwtPair(Admin admin)
        {
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
                    new Claim(JwtRegisteredClaimNames.Sub, admin.Username),
                    new Claim("CanEditUsers", admin.CanEditUsers ? "yes" : "no"),
                    new Claim("CanEditDoors", admin.CanEditDoors ? "yes" : "no"),
                    new Claim("CanEditAdmins", admin.CanEditAdmins ? "yes" : "no"),
                ],
                expires: accessExpiry.UtcDateTime,
                signingCredentials: creds);

            var refreshToken = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims:
                [
                    new Claim(JwtRegisteredClaimNames.Sub, admin.Username),
                    new Claim("type", "refresh"),
                ],
                expires: refreshExpiry.UtcDateTime,
                signingCredentials: creds);

            var handler = new JwtSecurityTokenHandler();
            return new JwtPair(
                handler.WriteToken(accessToken),
                accessExpiry,
                handler.WriteToken(refreshToken),
                refreshExpiry);
        }
    }
}
