using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Entities;

public partial class Users
{
    public int Code { get; set; }

    public string Description { get; set; } = null!;

    public string Password { get; set; } = null!;

    /// <summary>
    /// "Super" user. Reuses the legacy administrator flag: a Super implicitly has ReadWrite on
    /// every area, is the only role allowed to manage users, and at least one Super must always
    /// exist (enforced in UsersService).
    /// </summary>
    public bool Administrator { get; set; }

    /// <summary>Card Manager area access level (0=None, 1=Read, 2=ReadWrite). See <see cref="Enums.AreaAccess"/>.</summary>
    public int CardManagerAccess { get; set; }

    /// <summary>Site Settings area access level (0=None, 1=Read, 2=ReadWrite). See <see cref="Enums.AreaAccess"/>.</summary>
    public int SiteSettingsAccess { get; set; }

    /// <summary>User Settings area access level (0=None, 1=Read, 2=ReadWrite). See <see cref="Enums.AreaAccess"/>.</summary>
    public int UserSettingsAccess { get; set; }
}

