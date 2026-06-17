namespace DoorsWeb.API.Middleware
{
    // Hard gate for the "force password change on first login" flow: an
    // authenticated user whose access token carries must_change_password=true is
    // blocked from every endpoint except the /auth routes (so they can still hit
    // auth/change-password). The flag clears once they change the password, because
    // ChangePasswordAsync re-issues a token without it.
    public class MustChangePasswordMiddleware
    {
        private readonly RequestDelegate _next;

        public MustChangePasswordMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity?.IsAuthenticated == true &&
                context.User.HasClaim("must_change_password", "true") &&
                !context.Request.Path.StartsWithSegments("/auth"))
            {
                await Results.Problem(
                    detail: "You must change your default password before continuing.",
                    title: "Password change required",
                    statusCode: StatusCodes.Status403Forbidden).ExecuteAsync(context);
                return;
            }

            await _next(context);
        }
    }
}
