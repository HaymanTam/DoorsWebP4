namespace DoorsWeb.Shared.DTO
{
    /// <summary>Bundled save for a space zone: header plus its included doors.</summary>
    public class SpaceZoneSaveDto
    {
        /// <summary>Null when creating; assigned globally on the server (ZoneNumber is not an identity).</summary>
        public int? ZoneNumber { get; set; }
        public int Site { get; set; }
        public string? Name { get; set; }
        /// <summary>"Remove card holder from zone if stay exceeds" → T_SpaceZone_Header.MaxStayOn.</summary>
        public bool RemoveOnExceed { get; set; }
        /// <summary>Max-stay hours → T_SpaceZone_Header.MaxStay.</summary>
        public int MaxStayHours { get; set; }
        public bool FireZone { get; set; }
        /// <summary>"Restrict Zone Access" → T_SpaceZone_Header.RestrictCardholders.</summary>
        public bool RestrictZoneAccess { get; set; } = true;
        public List<SpaceZoneDoorDto> Doors { get; set; } = new();
    }

    /// <summary>One door row in the space-zone door picker (T_SpaceZone_Details).</summary>
    public class SpaceZoneDoorDto
    {
        public int Door { get; set; }
        public string? Name { get; set; }
        public bool Included { get; set; }
        public bool OpenOnFireAlarm { get; set; }
    }
}
