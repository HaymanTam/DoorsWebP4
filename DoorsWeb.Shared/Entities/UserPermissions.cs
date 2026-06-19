using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Entities;

public partial class UserPermissions
{
    public int Code { get; set; }

    public string Area { get; set; } = null!;
}

