namespace DoorsWeb.Shared.Auth
{
    /// <summary>
    /// JWT claim types for the 3-area permission model. Defined once in Shared so the API
    /// (token issuing + authorization handler) and the Client (navigation gating) never drift.
    /// The three area claims carry the <see cref="Enums.AreaAccess"/> int value as a string
    /// ("0"/"1"/"2"). A Super user always carries "2" for every area.
    /// </summary>
    public static class PermissionClaims
    {
        /// <summary>"true"/"false" — the user is a Super (manages users, implicit ReadWrite everywhere).</summary>
        public const string Super = "super";

        /// <summary>Card Manager area access level as an int string.</summary>
        public const string CardManager = "perm.cardmanager";

        /// <summary>Site Settings area access level as an int string.</summary>
        public const string SiteSettings = "perm.sitesettings";

        /// <summary>User Settings area access level as an int string.</summary>
        public const string UserSettings = "perm.usersettings";
    }
}
