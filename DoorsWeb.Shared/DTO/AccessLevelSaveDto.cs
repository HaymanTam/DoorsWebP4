namespace DoorsWeb.Shared.DTO
{
    /// <summary>Bundled save for an access level: header plus its selected doors.</summary>
    public class AccessLevelSaveDto
    {
        /// <summary>Null when creating; assigned globally on the server.</summary>
        public int? AccessLevel { get; set; }
        public int Site { get; set; }
        public string? Name { get; set; }
        /// <summary>Header-level time zone (FK). Carried through unchanged for now.</summary>
        public int? TimeZone { get; set; }
        public List<AccessLevelDoorDto> Doors { get; set; } = new();
    }

    /// <summary>One door row in the access-level door picker (T_AccessLevel_Details).</summary>
    public class AccessLevelDoorDto
    {
        public int Door { get; set; }
        public string? Name { get; set; }
        public bool Selected { get; set; }
        public int? DoorTimeZone { get; set; }
        public bool LevelDefault { get; set; } = true;
    }
}
