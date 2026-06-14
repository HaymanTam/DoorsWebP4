using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Legacy.Entities;

public partial class TAccessLevelHeader
{
    public int AccessLevel { get; set; }

    public int Site { get; set; }

    public string? Name { get; set; }

    public string? Key { get; set; }

    public int? TimeZone { get; set; }

    public int? LocalLevel { get; set; }
}

