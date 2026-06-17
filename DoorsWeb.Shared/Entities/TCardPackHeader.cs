using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Entities;

public partial class TCardPackHeader
{
    public int Code { get; set; }

    public string? Name { get; set; }

    public string? FirstCardId { get; set; }

    public int? Qty { get; set; }
}

