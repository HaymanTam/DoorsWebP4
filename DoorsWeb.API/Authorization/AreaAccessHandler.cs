using DoorsWeb.Shared.Auth;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace DoorsWeb.API.Authorization
{
    /// <summary>
    /// Evaluates <see cref="AreaAccessRequirement"/>: a Super bypasses every area check; otherwise
    /// the area claim's int value must be at least the required level. Stateless, so registered as
    /// a singleton.
    /// </summary>
    public sealed class AreaAccessHandler : AuthorizationHandler<AreaAccessRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, AreaAccessRequirement requirement)
        {
            // Super (Administrator) has ReadWrite everywhere.
            if (string.Equals(context.User.FindFirstValue(PermissionClaims.Super), "true",
                    StringComparison.OrdinalIgnoreCase))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            var raw = context.User.FindFirstValue(requirement.ClaimType);
            if (int.TryParse(raw, out var level) && level >= (int)requirement.MinLevel)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
