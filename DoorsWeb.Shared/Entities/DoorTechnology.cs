using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Entities;

public partial class DoorTechnology
{
    public int Code { get; set; }

    public string? Description { get; set; }

    public int? TechnologyCode { get; set; }
}

