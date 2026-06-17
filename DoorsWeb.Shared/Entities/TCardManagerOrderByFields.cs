using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Entities;

public partial class TCardManagerOrderByFields
{
    public int Code { get; set; }

    public int SortNumber { get; set; }

    public string? TableName { get; set; }

    public string? FieldName { get; set; }

    public int? Descending { get; set; }
}

