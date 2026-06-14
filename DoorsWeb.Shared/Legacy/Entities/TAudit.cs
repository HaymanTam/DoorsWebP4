using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Legacy.Entities;

public partial class TAudit
{
    public int Id { get; set; }

    public string CardId { get; set; } = null!;

    public DateTime SaveDate { get; set; }

    public string SavedBy { get; set; } = null!;

    public string Workstation { get; set; } = null!;

    public string Forename { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public bool Enabled { get; set; }

    public string AccessLevels { get; set; } = null!;
}

