using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Entities;

public partial class TSpaceZoneHeader
{
    public int? Site { get; set; }

    public int ZoneNumber { get; set; }

    public string? Name { get; set; }

    public string? Key { get; set; }

    public bool? Inuse { get; set; }

    public string? Status { get; set; }

    public bool? FireZone { get; set; }

    public int? FireInterfaceDoor { get; set; }

    public bool? OpenDoorsOnFireAlarm { get; set; }

    public bool? CloseDoorsOnFireReset { get; set; }

    public bool? MaxStayOn { get; set; }

    public int? MaxStay { get; set; }

    public string? Report { get; set; }

    public bool? LocalReport { get; set; }

    public bool? Rented { get; set; }

    public bool? InDispute { get; set; }

    public bool? Reserved { get; set; }

    public bool? RestrictCardholders { get; set; }
}

