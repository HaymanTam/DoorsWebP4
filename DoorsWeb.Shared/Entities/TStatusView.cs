using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Entities;

public partial class TStatusView
{
    public int Code { get; set; }

    public string? Name { get; set; }

    public bool? ShowAllFloors { get; set; }

    public int? Display { get; set; }

    public int? Wait { get; set; }

    public int? Move { get; set; }

    public int? Pause { get; set; }

    public bool? UpdateChangesWhenPanning { get; set; }

    public int? StatusView { get; set; }

    public int? StatusViewX { get; set; }

    public int? StatusViewY { get; set; }

    public int? StatusViewZ { get; set; }
}

