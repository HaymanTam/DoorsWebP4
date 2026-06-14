using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Legacy.Entities;

public partial class TNameAccessLevels
{
    public bool? Inuse { get; set; }

    public int Site { get; set; }

    public int Level { get; set; }

    public int CardNumber { get; set; }

    public DateTime? Modified { get; set; }
}

