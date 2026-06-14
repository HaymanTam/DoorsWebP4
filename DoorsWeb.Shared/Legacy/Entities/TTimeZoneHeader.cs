using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Legacy.Entities;

public partial class TTimeZoneHeader
{
    public int TimeZone { get; set; }

    public int? LocalTimeZone { get; set; }

    public int Site { get; set; }

    public string? Name { get; set; }

    public string? Key { get; set; }

    public int? Calendar { get; set; }
}

