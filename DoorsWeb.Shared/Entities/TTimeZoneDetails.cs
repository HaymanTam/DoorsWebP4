using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Entities;

public partial class TTimeZoneDetails
{
    public int TimeZone { get; set; }

    public int Sequence { get; set; }

    public string? StartTime { get; set; }

    public string? EndTime { get; set; }

    public bool? Sun { get; set; }

    public bool? Mon { get; set; }

    public bool? Tue { get; set; }

    public bool? Wed { get; set; }

    public bool? Thu { get; set; }

    public bool? Fri { get; set; }

    public bool? Sat { get; set; }

    public int? Calendar { get; set; }

    public bool? DefaultCalendar { get; set; }
}

