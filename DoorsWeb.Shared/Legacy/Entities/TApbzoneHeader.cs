using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Legacy.Entities;

public partial class TApbzoneHeader
{
    public int Apbnumber { get; set; }

    public int? Site { get; set; }

    public string? Name { get; set; }

    public string? Key { get; set; }

    public int? Apbmode { get; set; }

    public int? DiscoveryMode { get; set; }

    public int? DiscoveryModeDuration { get; set; }

    public DateTime? DiscoveryModeExpiryDate { get; set; }

    public DateTime? DiscoveryModeStart { get; set; }

    public bool? DiscoveryModeOnFireAlarm { get; set; }

    public int? FireInterfaceDoor { get; set; }

    public int? DiscoveryModeOnFireAlarmDuration { get; set; }

    public bool? AutoLogOut { get; set; }

    public DateTime? NextAutoLogout { get; set; }
}

