namespace DoorsWeb.Shared.DTO
{
    /// <summary>A single alarm row for the Alarms page (Active / Actioned tabs).</summary>
    public class AlarmListDto
    {
        public int Code { get; set; }
        public string? Description { get; set; }   // T_Alarms.AlarmDescription
        public string? Location { get; set; }      // T_Sites.Name (via Site FK)
        public DateTime? Date { get; set; }         // T_Alarms.AlarmDate
        public string? Details { get; set; }       // T_Alarms.ActionedText
        public DateTime? ActionedDate { get; set; }
        public string? ActionedBy { get; set; }
        public bool IsActioned { get; set; }

        /// <summary>Elapsed time from the alarm being raised to being actioned.</summary>
        public string Turnaround
        {
            get
            {
                if (Date is null || ActionedDate is null) return "";
                var span = ActionedDate.Value - Date.Value;
                if (span < TimeSpan.Zero) return "";
                if (span.TotalDays >= 1) return $"{(int)span.TotalDays}d {span.Hours}h {span.Minutes}m";
                if (span.TotalHours >= 1) return $"{span.Hours}h {span.Minutes}m";
                return $"{span.Minutes}m";
            }
        }
    }
}
