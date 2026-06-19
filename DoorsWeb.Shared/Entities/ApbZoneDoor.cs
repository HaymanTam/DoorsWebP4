using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Entities;

public partial class ApbZoneDoor
{
    public int Apbnumber { get; set; }

    public int DoorNumber { get; set; }

    public string? Key { get; set; }

    public int? MemberType { get; set; }

    public int? ReaderA { get; set; }

    public int? ReaderB { get; set; }

    public bool? EnforceOnEntry { get; set; }

    public bool? EnforceOnExit { get; set; }
}

