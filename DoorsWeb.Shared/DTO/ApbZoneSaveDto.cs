namespace DoorsWeb.Shared.DTO
{
    /// <summary>Bundled save for an anti-passback zone: header plus its included doors.</summary>
    public class ApbZoneSaveDto
    {
        /// <summary>Null when creating; assigned globally on the server (Apbnumber is not an identity).</summary>
        public int? Apbnumber { get; set; }
        public int Site { get; set; }
        public string? Name { get; set; }

        /// <summary>"Discovery" | "Active" | "Off" → T_APBZone_Header.APBMode (0 | 1 | 2).</summary>
        public string Mode { get; set; } = "Discovery";

        /// <summary>"LastsFor" | "SetUntil" | "FurtherNotice" → DiscoveryMode (0 | 1 | 2).</summary>
        public string DiscoveryDuration { get; set; } = "FurtherNotice";
        /// <summary>Minutes when DiscoveryDuration == "LastsFor".</summary>
        public int? DiscoveryMinutes { get; set; }
        /// <summary>Expiry date+time when DiscoveryDuration == "SetUntil".</summary>
        public DateTime? DiscoveryExpiry { get; set; }

        public bool FireAlarmReset { get; set; }
        public int FireAlarmDiscoveryMinutes { get; set; }

        public bool LogOutDaily { get; set; }
        /// <summary>Daily logout time, "HH:mm". Stored as the next-occurrence in NextAutoLogout.</summary>
        public string? LogOutTime { get; set; }

        public List<ApbZoneDoorDto> Doors { get; set; } = new();
    }

    /// <summary>One door row in the anti-passback door picker (T_APBZone_Details).</summary>
    public class ApbZoneDoorDto
    {
        public int Door { get; set; }
        public string? Name { get; set; }
        public bool Included { get; set; }
    }
}
