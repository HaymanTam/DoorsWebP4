namespace DoorsWeb.Shared.DTO
{
    /// <summary>Bundled save for a trigger: header plus its source doors/zones (T_Triggers_Controllers).
    /// Events (T_Triggers_Events) are not written: the web editor lists events by name only and the
    /// legacy EventType codes come from the controller event catalogue, so there is no safe mapping yet.</summary>
    public class TriggerSaveDto
    {
        /// <summary>Null when creating; Code is a DB identity, assigned on insert.</summary>
        public int? Code { get; set; }
        public int Site { get; set; }
        public string? Name { get; set; }
        /// <summary>1 = Door, 3 = Space Zone (legacy TriggerTypeConstants).</summary>
        public int TriggerType { get; set; }

        public bool ShowAlarm { get; set; }
        public string? AlarmText { get; set; }
        public bool SuppressDuplicates { get; set; }

        public bool TriggerRelayB { get; set; }
        public bool ResetRelayB { get; set; }
        public int ResetRelayBperiod { get; set; }

        /// <summary>Space-zone population rule (TriggerType == 3): -1 down, 0 reaches, 1 up.</summary>
        public int PopulationDirection { get; set; }
        public int PopulationValue { get; set; }

        /// <summary>Selected source doors (TriggerType == 1) or space zones (TriggerType == 3).</summary>
        public List<TriggerSourceDto> Sources { get; set; } = new();
    }

    /// <summary>One source row: a door (TriggerType == 1) or a space zone (TriggerType == 3).</summary>
    public class TriggerSourceDto
    {
        /// <summary>Door number or zone number → T_Triggers_Controllers.ControllerCode (InputIndex is 0).</summary>
        public int Code { get; set; }
        public string? Name { get; set; }
        public bool Selected { get; set; }
    }
}
