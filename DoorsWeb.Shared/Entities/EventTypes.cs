using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Entities;

public partial class EventTypes
{
    public string? Description { get; set; }

    public int EventType { get; set; }

    public string Icon { get; set; } = null!;
}

