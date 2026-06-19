namespace DoorsWeb.Shared.DTO
{
    /// <summary>
    /// One row of the hours-worked report produced by running a time sheet definition.
    /// Mirrors the output columns of the legacy sp_TimeSheet stored procedure.
    /// All time/duration values are pre-formatted strings (the legacy report formatted them server-side).
    /// </summary>
    public class TimeSheetReportRowDto
    {
        public int CardNumber { get; set; }
        /// <summary>The working day this row belongs to (yyyy-MM-dd).</summary>
        public string EventDate { get; set; } = "";
        public string CardId { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        /// <summary>Clock-in time (HH:mm:ss), or blank for an unmatched clock-out.</summary>
        public string TimeFrom { get; set; } = "";
        /// <summary>Clock-out time (HH:mm:ss), or blank for an unmatched clock-in.</summary>
        public string TimeTo { get; set; } = "";
        /// <summary>Worked duration (HH:mm:ss); "00:00:00" when the in/out pair is incomplete.</summary>
        public string HoursWorked { get; set; } = "";
    }
}
