using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Entities;

public partial class TAccessLevelDetails
{
    public int Level { get; set; }

    public int Door { get; set; }

    public bool? LevelDefault { get; set; }

    public int? DoorTimeZone { get; set; }
}

