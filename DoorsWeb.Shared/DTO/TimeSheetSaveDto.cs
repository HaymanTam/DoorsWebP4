namespace DoorsWeb.Shared.DTO
{
    /// <summary>
    /// Bundled save/load for a time sheet definition (saved report settings):
    /// header fields plus the selected space zones. Mirrors the legacy frmTimeSheet tabs
    /// (General / Who / When / Where / Print). The "who" custom-field filters are not
    /// surfaced in the new UI, so they are left blank on save.
    /// </summary>
    public class TimeSheetSaveDto
    {
        /// <summary>Null when creating; assigned by the server (Code is not an identity column).</summary>
        public int? Code { get; set; }

        public string Name { get; set; } = "";

        // ---- Who: cardholder filters (blank = no filter) ----
        public string CardId { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";

        // ---- When ----
        /// <summary>1 = single day (DateFrom), 2 = between (DateFrom..DateTo), 3 = in the last N periods.</summary>
        public int DateSearch { get; set; } = 1;
        public int InLastNumber { get; set; } = 1;
        /// <summary>1 = days, 4 = weeks, 5 = months (legacy DATEPART codes).</summary>
        public int InLastPeriod { get; set; } = 1;
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        /// <summary>Time-of-day the working "day" rolls over (e.g. a night shift starting time). Only the time part is used.</summary>
        public DateTime Rollover { get; set; }

        // ---- Print ----
        public bool DatePageBreak { get; set; }
        public bool CardIdPageBreak { get; set; }

        // ---- Where: space zones the report is restricted to (empty = all zones) ----
        public List<TimeSheetZoneDto> Zones { get; set; } = new();
    }

    /// <summary>One space-zone row in the time-sheet zone picker (T_TimeSheet_Zones / T_SpaceZone_Header).</summary>
    public class TimeSheetZoneDto
    {
        public int Zone { get; set; }
        public string? Name { get; set; }
        public bool Included { get; set; }
    }
}
