using DoorsWeb.Shared.Auth;
using DoorsWeb.Shared.Enums;
using System.Security.Claims;

namespace DoorsWeb.Client.Auth
{
    /// <summary>
    /// Reads the 3-area permission model from a signed-in user's JWT claims. Mirrors the server's
    /// authorization rules so the UI can hide navigation/actions the API would reject anyway.
    /// (The API is the source of truth — these checks are convenience only, never the security boundary.)
    /// </summary>
    public static class ClaimsPermissions
    {
        /// <summary>True when the user is a Super (implicit ReadWrite everywhere, manages users).</summary>
        public static bool IsSuper(this ClaimsPrincipal user) =>
            string.Equals(user.FindFirst(PermissionClaims.Super)?.Value, "true",
                StringComparison.OrdinalIgnoreCase);

        public static AreaAccess CardManager(this ClaimsPrincipal user) => Area(user, PermissionClaims.CardManager);
        public static AreaAccess SiteSettings(this ClaimsPrincipal user) => Area(user, PermissionClaims.SiteSettings);
        public static AreaAccess UserSettings(this ClaimsPrincipal user) => Area(user, PermissionClaims.UserSettings);

        public static bool CanReadCardManager(this ClaimsPrincipal user) => user.CardManager() >= AreaAccess.Read;
        public static bool CanWriteCardManager(this ClaimsPrincipal user) => user.CardManager() >= AreaAccess.ReadWrite;

        public static bool CanReadSiteSettings(this ClaimsPrincipal user) => user.SiteSettings() >= AreaAccess.Read;
        public static bool CanWriteSiteSettings(this ClaimsPrincipal user) => user.SiteSettings() >= AreaAccess.ReadWrite;

        public static bool CanReadUserSettings(this ClaimsPrincipal user) => user.UserSettings() >= AreaAccess.Read;
        public static bool CanWriteUserSettings(this ClaimsPrincipal user) => user.UserSettings() >= AreaAccess.ReadWrite;

        // A Super implicitly has ReadWrite; otherwise read the claim's int value (defaulting to None).
        private static AreaAccess Area(ClaimsPrincipal user, string claimType)
        {
            if (user.IsSuper()) return AreaAccess.ReadWrite;
            var raw = user.FindFirst(claimType)?.Value;
            if (int.TryParse(raw, out var value) && Enum.IsDefined(typeof(AreaAccess), value))
                return (AreaAccess)value;
            return AreaAccess.None;
        }
    }
}
