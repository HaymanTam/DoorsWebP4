using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Entities;

public partial class NameAccessLevelsView
{
    public int CardNumber { get; set; }

    public string? CardId { get; set; }

    public int Site { get; set; }

    public int LevelA { get; set; }

    public int LevelB { get; set; }

    public string? ValidFrom { get; set; }

    public string? ValidTo { get; set; }

    public string Pin { get; set; } = null!;

    public long? Id { get; set; }
}

