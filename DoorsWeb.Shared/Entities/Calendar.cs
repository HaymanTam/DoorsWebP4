using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Entities;

public partial class Calendar
{
    public int Code { get; set; }

    public int Site { get; set; }

    public int LocalNumber { get; set; }

    public string? Description { get; set; }
}

