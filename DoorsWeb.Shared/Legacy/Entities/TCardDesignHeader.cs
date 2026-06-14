using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Legacy.Entities;

public partial class TCardDesignHeader
{
    public int Code { get; set; }

    public string Description { get; set; } = null!;

    public int Orientation { get; set; }
}

