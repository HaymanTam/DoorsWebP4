using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Entities;

public partial class SpaceZoneDoor
{
    public int Door { get; set; }

    public int Site { get; set; }

    public int Zone { get; set; }

    public string? Name { get; set; }

    public string? Key { get; set; }

    public bool? Inuse { get; set; }

    public string? Status { get; set; }

    public bool? InReader1 { get; set; }

    public bool? InReader2 { get; set; }

    public bool? InReader3 { get; set; }

    public bool? OutReader1 { get; set; }

    public bool? OutReader2 { get; set; }

    public bool? OutReader3 { get; set; }

    public bool? OpenOnFireAlarm { get; set; }
}

