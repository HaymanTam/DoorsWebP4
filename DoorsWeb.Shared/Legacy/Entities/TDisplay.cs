using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Legacy.Entities;

public partial class TDisplay
{
    public int Code { get; set; }

    public int Position { get; set; }

    public int PropertyId { get; set; }

    public int? ColumnWidth { get; set; }
}

