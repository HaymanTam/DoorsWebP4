using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Entities;

public partial class Cardholder
{
    public string? Surname { get; set; }

    public string? Forname { get; set; }

    public int CardNumber { get; set; }

    public string? CardId { get; set; }

    public bool? Flexi { get; set; }

    public bool? InUse { get; set; }

    public bool? Enabled { get; set; }

    public bool? Void { get; set; }

    public DateTime? ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    public int? LastDoor { get; set; }

    public int? LastEvent { get; set; }

    public DateTime? LastDate { get; set; }

    public int? Rollcall { get; set; }

    public DateTime? Modified { get; set; }

    public bool? UpdatePending { get; set; }

    public string? Pin { get; set; }

    public int? PinRequired { get; set; }

    public int? ValidToOverride { get; set; }

    public int? ValidFromOverride { get; set; }

    public string? HotStamp { get; set; }

    public bool? BioAdmin { get; set; }

    public bool? BioOptOut { get; set; }

    public int? CardDesign { get; set; }

    public string? IdcardDesign { get; set; }

    public int? Apbnumber { get; set; }

    public DateTime? Apbdate { get; set; }
}

