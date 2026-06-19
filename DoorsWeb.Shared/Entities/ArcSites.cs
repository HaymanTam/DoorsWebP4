using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Entities;

public partial class ArcSites
{
    public int Site { get; set; }

    public string? Name { get; set; }

    public string? Key { get; set; }

    public bool? Inuse { get; set; }

    public string? Status { get; set; }
}

