namespace DoorsWeb.Shared.DTO
{
    /// <summary>Bundled save for a time zone: header plus its element rows.</summary>
    public class TimeZoneSaveDto
    {
        /// <summary>Null when creating; assigned globally on the server.</summary>
        public int? TimeZone { get; set; }
        public int Site { get; set; }
        public string? Name { get; set; }
        public int? Calendar { get; set; }
        public List<TimeZoneElementDto> Elements { get; set; } = new();
    }

    /// <summary>One time-zone element row (T_TimeZone_Details). Times are "HH:mm".</summary>
    public class TimeZoneElementDto
    {
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public bool Mon { get; set; }
        public bool Tue { get; set; }
        public bool Wed { get; set; }
        public bool Thu { get; set; }
        public bool Fri { get; set; }
        public bool Sat { get; set; }
        public bool Sun { get; set; }
        public int? Calendar { get; set; }
        public bool DefaultCalendar { get; set; }
    }
}
