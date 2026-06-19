using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Entities;

public partial class CardDesign
{
    public int Code { get; set; }

    public string Description { get; set; } = null!;

    public int Orientation { get; set; }
}

