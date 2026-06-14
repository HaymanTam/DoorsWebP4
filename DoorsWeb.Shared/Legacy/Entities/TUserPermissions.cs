using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Legacy.Entities;

public partial class TUserPermissions
{
    public int Code { get; set; }

    public string Area { get; set; } = null!;
}

