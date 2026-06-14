using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Legacy.Entities;

public partial class TIocontrollerHeader
{
    public int ControllerId { get; set; }

    public int? ControllerIndex { get; set; }

    public int? Connector { get; set; }

    public string? Name { get; set; }

    public string? Ipaddress { get; set; }

    public string? ControllerDay { get; set; }

    public string? ControllerVersion { get; set; }

    public string? Rtctime { get; set; }

    public string? Rtcdate { get; set; }
}

