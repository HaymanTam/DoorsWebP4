using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Entities;

public partial class CardManagerWhereField
{
    public int Code { get; set; }

    public int Sequence { get; set; }

    public string? TableName { get; set; }

    public string? FieldName { get; set; }

    public int? FieldType { get; set; }

    public string? Operator { get; set; }

    public string? Value1 { get; set; }

    public string? Value2 { get; set; }
}

