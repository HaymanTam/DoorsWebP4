using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Entities;

public partial class CardDesignField
{
    public int Code { get; set; }

    public int Sequence { get; set; }

    public int Type { get; set; }

    public int Left { get; set; }

    public int Top { get; set; }

    public int Height { get; set; }

    public int Width { get; set; }

    public string Text { get; set; } = null!;

    public string FontName { get; set; } = null!;

    public string FontSize { get; set; } = null!;

    public int Alignment { get; set; }

    public bool Bold { get; set; }

    public bool Italic { get; set; }

    public bool Underline { get; set; }

    public int Colour { get; set; }
}

