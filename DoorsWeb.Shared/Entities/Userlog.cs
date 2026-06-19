using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Entities;

public partial class Userlog
{
    public string? Id { get; set; }

    public string Key { get; set; } = null!;

    public string? User { get; set; }

    public string? LogInPoint { get; set; }

    public string? LogInEvent { get; set; }

    public string? LogInDateTime { get; set; }
}

