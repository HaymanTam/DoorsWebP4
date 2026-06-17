using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Entities;

public partial class TIocontrollerDetails
{
    public int ControllerId { get; set; }

    public short IoinputIndex { get; set; }

    public string? InputName { get; set; }

    public string? OutputName { get; set; }

    public bool? Inverted { get; set; }

    public int? FloorPlan { get; set; }

    public int? FloorPlanX { get; set; }

    public int? FloorPlanY { get; set; }
}

