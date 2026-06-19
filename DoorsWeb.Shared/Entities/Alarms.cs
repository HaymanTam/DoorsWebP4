using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Entities;

public partial class Alarms
{
    public int Code { get; set; }

    public int? Site { get; set; }

    public int? AlarmType { get; set; }

    public DateTime? AlarmDate { get; set; }

    public string? AlarmDescription { get; set; }

    public DateTime? ActionedDate { get; set; }

    public string? ActionedBy { get; set; }

    public string? ActionedText { get; set; }

    public bool? IsRead { get; set; }

    public int? ControllerNumber { get; set; }

    public int? InputIndex { get; set; }

    public int? EventType { get; set; }
}

