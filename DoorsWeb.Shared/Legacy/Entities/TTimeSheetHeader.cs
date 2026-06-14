using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Legacy.Entities;

public partial class TTimeSheetHeader
{
    public int Code { get; set; }

    public string Name { get; set; } = null!;

    public string CardId { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Custom1 { get; set; } = null!;

    public string Custom2 { get; set; } = null!;

    public string Custom3 { get; set; } = null!;

    public string Custom4 { get; set; } = null!;

    public string Custom5 { get; set; } = null!;

    public string Custom6 { get; set; } = null!;

    public string Custom7 { get; set; } = null!;

    public string Custom8 { get; set; } = null!;

    public string Custom9 { get; set; } = null!;

    public string Custom10 { get; set; } = null!;

    public string Custom11 { get; set; } = null!;

    public string Custom12 { get; set; } = null!;

    public string Custom13 { get; set; } = null!;

    public string Custom14 { get; set; } = null!;

    public string Custom15 { get; set; } = null!;

    public string Custom16 { get; set; } = null!;

    public string Custom17 { get; set; } = null!;

    public string Custom18 { get; set; } = null!;

    public string Custom19 { get; set; } = null!;

    public string Custom20 { get; set; } = null!;

    public string Custom21 { get; set; } = null!;

    public string Custom22 { get; set; } = null!;

    public string Custom23 { get; set; } = null!;

    public string Custom24 { get; set; } = null!;

    public string Custom25 { get; set; } = null!;

    public int DateSearch { get; set; }

    public int InLastNumber { get; set; }

    public int InLastPeriod { get; set; }

    public DateTime DateFrom { get; set; }

    public DateTime DateTo { get; set; }

    public DateTime Rollover { get; set; }

    public bool DatePageBreak { get; set; }

    public bool CardIdpageBreak { get; set; }
}

