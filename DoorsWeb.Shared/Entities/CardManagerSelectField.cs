using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Entities;

public partial class CardManagerSelectField
{
    public int Code { get; set; }

    public int Position { get; set; }

    public int? FieldType { get; set; }

    public string? TableName { get; set; }

    public string? FieldName { get; set; }

    public int? ColumnWidth { get; set; }
}

