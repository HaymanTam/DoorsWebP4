using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Legacy.Entities;

public partial class TSites
{
    public int Site { get; set; }

    public string? Name { get; set; }

    public string? Key { get; set; }

    public bool? Inuse { get; set; }

    public string? Status { get; set; }
}

