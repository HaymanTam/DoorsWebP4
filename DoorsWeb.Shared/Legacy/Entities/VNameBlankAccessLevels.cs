using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Legacy.Entities;

public partial class VNameBlankAccessLevels
{
    public int CardNumber { get; set; }

    public string? CardId { get; set; }

    public int Site { get; set; }

    public int LevelA { get; set; }

    public int LevelB { get; set; }

    public string ValidFrom { get; set; } = null!;

    public string ValidTo { get; set; } = null!;

    public string Pin { get; set; } = null!;

    public long? Id { get; set; }
}

