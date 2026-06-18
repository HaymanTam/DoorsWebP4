namespace DoorsWeb.Shared.DTO
{
    /// <summary>Bundled save for a calendar: header (Description) plus its holiday dates.</summary>
    public class CalendarSaveDto
    {
        /// <summary>Null when creating; the DB assigns the identity Code.</summary>
        public int? Code { get; set; }
        public int Site { get; set; }
        public string? Description { get; set; }
        public List<DateTime> Holidays { get; set; } = new();
    }
}
