using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Entities;

public partial class TFloorPlans
{
    public int Code { get; set; }

    public int Site { get; set; }

    public string Name { get; set; } = null!;
}

