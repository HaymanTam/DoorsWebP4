using DoorsWeb.API.Services.Interfaces;

namespace DoorsWeb.API.Middleware
{
    /// <summary>
    /// Enforces read-only mode once a valid license has passed its expiry date
    /// (<see cref="ILicenseService.IsReadOnly"/>). Every mutating request (POST/PUT/PATCH/DELETE) is
    /// rejected with 403, EXCEPT the auth routes and the System Settings endpoint — so an expired
    /// installation can still log in and paste a renewed license key to recover. Reads (GET/HEAD/
    /// OPTIONS) always pass through. Unlicensed or active licenses are never read-only here; the
    /// door/card count limits are enforced separately at create time.
    /// </summary>
    public class LicenseReadOnlyMiddleware
    {
        private readonly RequestDelegate _next;

        public LicenseReadOnlyMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context, ILicenseService license)
        {
            if (IsWrite(context.Request.Method) && license.IsReadOnly && !IsExempt(context.Request.Path))
            {
                await Results.Problem(
                    detail: license.GetState().Message
                            ?? "The license has expired — the system is read-only until it is renewed.",
                    title: "License expired (read-only)",
                    statusCode: StatusCodes.Status403Forbidden).ExecuteAsync(context);
                return;
            }

            await _next(context);
        }

        private static bool IsWrite(string method) =>
            HttpMethods.IsPost(method) || HttpMethods.IsPut(method) ||
            HttpMethods.IsPatch(method) || HttpMethods.IsDelete(method);

        // Always allow logging in/out and renewing the key, even while read-only.
        private static bool IsExempt(PathString path) =>
            path.StartsWithSegments("/auth") ||
            path.StartsWithSegments("/api/SystemSettings");
    }
}
