using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Entities;

public partial class BioData
{
    public int Slot { get; set; }

    public int? BioChecksumLeft { get; set; }

    public int? BioChecksumRight { get; set; }

    public string? Id { get; set; }

    public string? Status { get; set; }

    public string? FingerLeft { get; set; }

    public string? FingerRight { get; set; }

    public string? BioTemplateLeft { get; set; }

    public string? BioTemplateRight { get; set; }
}

