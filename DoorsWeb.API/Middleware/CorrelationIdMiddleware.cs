using Serilog.Context;

namespace DoorsWeb.API.Middleware
{
    // Gives every request a stable correlation ID and makes it traceable end to end — the backbone
    // of the support-bundle/bug-report trace chain. For each request it:
    //   * honours an inbound X-Correlation-Id header (so an ID the client already minted flows
    //     through unchanged), otherwise generates one;
    //   * overwrites HttpContext.TraceIdentifier with it, so the existing ProblemDetails "requestId"
    //     (see Program.cs) surfaces the same value in every error response body;
    //   * echoes it back in the X-Correlation-Id response header, which the client records so a user
    //     filing a bug report can quote it;
    //   * pushes it into the Serilog LogContext so EVERY log line emitted while handling the request
    //     is tagged with the same ID.
    // Net effect: a user-quoted reference resolves directly to the exact log lines for that request.
    public class CorrelationIdMiddleware
    {
        public const string HeaderName = "X-Correlation-Id";

        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId =
                context.Request.Headers.TryGetValue(HeaderName, out var inbound) &&
                !string.IsNullOrWhiteSpace(inbound)
                    ? inbound.ToString()
                    : Guid.NewGuid().ToString("N");

            context.TraceIdentifier = correlationId;

            // Headers can't be changed once the response has started, so set it just before flush.
            context.Response.OnStarting(() =>
            {
                context.Response.Headers[HeaderName] = correlationId;
                return Task.CompletedTask;
            });

            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                await _next(context);
            }
        }
    }
}
