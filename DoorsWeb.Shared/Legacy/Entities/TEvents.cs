using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Legacy.Entities;

public partial class TEvents
{
    public DateTime EventDate { get; set; }

    public int CardNumber { get; set; }

    public int DoorNumber { get; set; }

    public int EventType { get; set; }

    public int ReaderId { get; set; }

    public int EventId { get; set; }

    public string? ActualCardId { get; set; }

    public int? AlarmId { get; set; }
}

