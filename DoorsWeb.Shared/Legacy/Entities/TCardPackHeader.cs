using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Legacy.Entities;

public partial class TCardPackHeader
{
    public int Code { get; set; }

    public string? Name { get; set; }

    public string? FirstCardId { get; set; }

    public int? Qty { get; set; }
}

