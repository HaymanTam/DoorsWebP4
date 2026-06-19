namespace DoorsWeb.Shared.Enums
{
    /// <summary>
    /// Per-area permission level for a user. Stored as an int on T_Users and emitted as
    /// JWT claims (perm.cardmanager / perm.sitesettings / perm.usersettings).
    /// A "Super" user (<see cref="Entities.Users.Administrator"/>) implicitly has
    /// <see cref="ReadWrite"/> on every area regardless of the stored column values.
    /// </summary>
    public enum AreaAccess
    {
        /// <summary>No access: the area's navigation/menu items are hidden and the API rejects all calls.</summary>
        None = 0,

        /// <summary>Read-only: the user can view the area but cannot create/update/delete.</summary>
        Read = 1,

        /// <summary>Full access: the user can view and mutate the area.</summary>
        ReadWrite = 2
    }
}
