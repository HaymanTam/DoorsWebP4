using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Legacy.Entities;

public partial class TEventTypes
{
    public string? Description { get; set; }

    public int EventType { get; set; }

    public string Icon { get; set; } = null!;
}

