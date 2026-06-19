using DoorsWeb.Shared.Enums;
using Microsoft.AspNetCore.Authorization;

namespace DoorsWeb.API.Authorization
{
    /// <summary>
    /// Authorization requirement satisfied when the caller's permission claim for
    /// <see cref="ClaimType"/> is at least <see cref="MinLevel"/> — or the caller is a Super.
    /// </summary>
    public sealed class AreaAccessRequirement : IAuthorizationRequirement
    {
        /// <summary>The JWT claim type carrying the area's <see cref="AreaAccess"/> value (e.g. "perm.cardmanager").</summary>
        public string ClaimType { get; }

        /// <summary>Minimum access level required (Read or ReadWrite).</summary>
        public AreaAccess MinLevel { get; }

        public AreaAccessRequirement(string claimType, AreaAccess minLevel)
        {
            ClaimType = claimType;
            MinLevel = minLevel;
        }
    }
}
