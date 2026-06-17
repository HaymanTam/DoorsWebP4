using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Entities;

public partial class TCardManagerHeader
{
    public int Code { get; set; }

    public string? Description { get; set; }

    public int? ViewType { get; set; }

    public int? Owner { get; set; }

    public int? ListViewType { get; set; }

    public int? Orientation { get; set; }

    public int? LeftMargin { get; set; }

    public int? RightMargin { get; set; }

    public int? TopMargin { get; set; }

    public int? BottomMargin { get; set; }

    public string? Printer { get; set; }
}

