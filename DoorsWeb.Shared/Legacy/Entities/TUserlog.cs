using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Legacy.Entities;

public partial class TUserlog
{
    public string? Id { get; set; }

    public string Key { get; set; } = null!;

    public string? User { get; set; }

    public string? LogInPoint { get; set; }

    public string? LogInEvent { get; set; }

    public string? LogInDateTime { get; set; }
}

