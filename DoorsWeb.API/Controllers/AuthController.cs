using DoorsWeb.API.Services;
using DoorsWeb.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace DoorsWeb.API.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("signin")]
        public async Task<ActionResult<JwtPair>> SignIn(
            AdminLoginDto dto, CancellationToken ct)
        {
            var result = await _authService.SignInAsync(dto, ct);
            return result.Map<ActionResult>(
                onSuccess: value => Ok(value),
                onFailure: error => Problem(detail: error.Message, title: error.Type.ToString(), statusCode: error.StatusCode)
            );
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<JwtPair>> Refresh(
            [FromBody] string refreshToken, CancellationToken ct)
        {
            var result = await _authService.RefreshAsync(refreshToken, ct);
            return result.Map<ActionResult>(
                onSuccess: value => Ok(value),
                onFailure: error => Problem(detail: error.Message, title: error.Type.ToString(), statusCode: error.StatusCode)
            );
        }

        // JWT is stateless — logout is handled client-side by discarding the tokens.
        // This endpoint exists as a convenience hook for future server-side token revocation.
        [HttpPost("signout")]
        public new IActionResult SignOut() => Ok();
    }
}
