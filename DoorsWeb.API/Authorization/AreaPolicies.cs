namespace DoorsWeb.API.Authorization
{
    /// <summary>
    /// Named authorization policy strings for the 3-area permission model. Each area has a
    /// Read policy (≥ Read) and a Write policy (≥ ReadWrite). Controllers reference these via
    /// <c>[Authorize(Policy = AreaPolicies.XxxRead/Write)]</c>; they are registered in Program.cs.
    /// </summary>
    public static class AreaPolicies
    {
        public const string CardManagerRead = "CardManager.Read";
        public const string CardManagerWrite = "CardManager.Write";

        public const string SiteSettingsRead = "SiteSettings.Read";
        public const string SiteSettingsWrite = "SiteSettings.Write";

        public const string UserSettingsRead = "UserSettings.Read";
        public const string UserSettingsWrite = "UserSettings.Write";

        public const string ReportsRead = "Reports.Read";
        public const string ReportsWrite = "Reports.Write";
    }
}
