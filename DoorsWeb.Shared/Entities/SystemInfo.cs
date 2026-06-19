using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Entities;

public partial class SystemInfo
{
    public string? CurrentVersion { get; set; }

    public bool? AutoConfigUpdate { get; set; }

    public bool? EnableFlexi { get; set; }

    public string? Corporate1000Code { get; set; }
}

