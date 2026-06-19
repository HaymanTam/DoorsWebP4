using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Entities;

public partial class Display
{
    public int Code { get; set; }

    public int Position { get; set; }

    public int PropertyId { get; set; }

    public int? ColumnWidth { get; set; }
}

