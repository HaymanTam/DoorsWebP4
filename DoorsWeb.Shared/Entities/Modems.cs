using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Entities;

public partial class Modems
{
    public int Code { get; set; }

    public int? Comport { get; set; }

    public string? Description { get; set; }
}

