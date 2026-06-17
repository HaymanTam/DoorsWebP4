using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Entities;

public partial class TDisplayTypes
{
    public int Code { get; set; }

    public int PropertyId { get; set; }

    public string Description { get; set; } = null!;

    public bool IsTrueFalse { get; set; }

    public string TrueDescription { get; set; } = null!;

    public string FalseDescription { get; set; } = null!;
}

