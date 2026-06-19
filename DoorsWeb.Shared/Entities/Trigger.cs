using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Entities;

public partial class Trigger
{
    public int Code { get; set; }

    public int Site { get; set; }

    public string Name { get; set; } = null!;

    public int TriggerType { get; set; }

    public int PopulationDirection { get; set; }

    public int PopulationValue { get; set; }

    public int PopulationSim { get; set; }

    public int PopulationInput { get; set; }

    public bool PopulationInputOpens { get; set; }

    public bool TriggerOutput { get; set; }

    public bool SuppressDuplicates { get; set; }

    public int OutputSim { get; set; }

    public int OutputIndex { get; set; }

    public bool OpenOutput { get; set; }

    public bool ResetOutput { get; set; }

    public int ResetOutputPeriod { get; set; }

    public bool TriggerRelayB { get; set; }

    public int RelayBdoor { get; set; }

    public bool OpenRelayB { get; set; }

    public bool ResetRelayB { get; set; }

    public int ResetRelayBperiod { get; set; }

    public bool ShowAlarm { get; set; }

    public string AlarmText { get; set; } = null!;
}

