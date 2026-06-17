using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Entities;

public partial class TCustomFieldTypes
{
    public int CustomField { get; set; }

    public string? Literal { get; set; }

    public int? DataType { get; set; }
}

