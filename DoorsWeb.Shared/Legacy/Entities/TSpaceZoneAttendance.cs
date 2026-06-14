using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Legacy.Entities;

public partial class TSpaceZoneAttendance
{
    public int ZoneNumber { get; set; }

    public int CardIndex { get; set; }

    public DateTime DateandTime { get; set; }

    public string? Keyf { get; set; }

    public bool? AlarmFlag { get; set; }
}

