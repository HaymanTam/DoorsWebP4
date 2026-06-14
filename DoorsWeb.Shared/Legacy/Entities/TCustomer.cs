using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Legacy.Entities;

public partial class TCustomer
{
    public string CustomerCompany { get; set; } = null!;

    public string CustomerContact { get; set; } = null!;

    public string CustomerTelephone { get; set; } = null!;

    public string CustomerFax { get; set; } = null!;

    public string Customeremail { get; set; } = null!;

    public string CustomerAddress1 { get; set; } = null!;

    public string CustomerAddress2 { get; set; } = null!;

    public string CustomerAddress3 { get; set; } = null!;

    public string CustomerAddress4 { get; set; } = null!;

    public string CustomerCountry { get; set; } = null!;

    public string CustomerPostCode { get; set; } = null!;

    public string InstallerCompany { get; set; } = null!;

    public string InstallerContact { get; set; } = null!;

    public string InstallerTelephone { get; set; } = null!;

    public string InstallerFax { get; set; } = null!;

    public string Installeremail { get; set; } = null!;

    public string InstallerAddress1 { get; set; } = null!;

    public string InstallerAddress2 { get; set; } = null!;

    public string InstallerAddress3 { get; set; } = null!;

    public string InstallerAddress4 { get; set; } = null!;

    public string InstallerCountry { get; set; } = null!;

    public string InstallerPostCode { get; set; } = null!;

    public DateTime InstallationDate { get; set; }

    public DateTime CommissionDate { get; set; }

    public int InstallType { get; set; }

    public string ProductKey { get; set; } = null!;

    public string Comments { get; set; } = null!;
}

