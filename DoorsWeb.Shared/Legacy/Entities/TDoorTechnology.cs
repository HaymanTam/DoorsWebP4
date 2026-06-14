using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Legacy.Entities;

public partial class TDoorTechnology
{
    public int Code { get; set; }

    public string? Description { get; set; }

    public int? TechnologyCode { get; set; }
}

