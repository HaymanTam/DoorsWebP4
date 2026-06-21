using DoorsWeb.API.Services;
using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DoorsWeb.API.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("signin")]
        public async Task<ActionResult<JwtPair>> SignIn(AdminLoginDto dto, CancellationToken ct)
        {
            var result = await _authService.SignInAsync(dto, ct);
            if (result is null)
                return Problem(detail: "Invalid username or password.", title: "Unauthorized", statusCode: 401);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<ActionResult<JwtPair>> Refresh([FromBody] string refreshToken, CancellationToken ct)
        {
            var result = await _authService.RefreshAsync(refreshToken, ct);
            if (result is null)
                return Problem(detail: "Invalid or expired refresh token.", title: "Unauthorized", statusCode: 401);
            return Ok(result);
        }

        // Requires a valid access token (issued at sign-in); the username comes from
        // the token, not the request body.
        [Authorize]
        [HttpPost("change-password")]
        public async Task<ActionResult<JwtPair>> ChangePassword(ChangePasswordDto dto, CancellationToken ct)
        {
            var username = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (username is null)
                return Problem(detail: "Token does not identify a user.", title: "Unauthorized", statusCode: 401);

            try
            {
                var result = await _authService.ChangePasswordAsync(username, dto.NewPassword, ct);
                if (result is null)
                    return Problem(detail: "User not found.", title: "Not Found", statusCode: 404);
                return Ok(result);
            }
            catch (PasswordPolicyException ex)
            {
                // Password too short or on the breached/common list.
                return Problem(detail: ex.Message, title: "Password not allowed", statusCode: 400);
            }
        }

        // Exposes the default password as a first-run hint while the account still
        // uses it. Returns hint = null once the password has been changed.
        [AllowAnonymous]
        [HttpGet("default-hint/{username}")]
        public async Task<IActionResult> DefaultHint(string username, CancellationToken ct)
        {
            var hint = await _authService.GetDefaultPasswordHintAsync(username, ct);
            return Ok(new { isDefault = hint is not null, hint });
        }

        // First-run hint for the blank login screen: surfaces the seeded Super account's username and
        // default password while it is still unconfigured, so a fresh install can be signed into.
        // available = false once that account's password has been changed (or no such account exists).
        [AllowAnonymous]
        [HttpGet("first-run-hint")]
        public async Task<IActionResult> FirstRunHint(CancellationToken ct)
        {
            var hint = await _authService.GetFirstRunHintAsync(ct);
            return Ok(new { available = hint is not null, username = hint?.Username, password = hint?.Password });
        }
    }
}
