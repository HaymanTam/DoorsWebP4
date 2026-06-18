namespace DoorsWeb.Shared.DTO
{
    /// <summary>A single card-change record for the Audit Log Viewer.</summary>
    public class AuditLogDto
    {
        public int Id { get; set; }
        public string? CardId { get; set; }        // Card ID
        public DateTime SaveDate { get; set; }      // Date Saved
        public string? SavedBy { get; set; }        // Saved By
        public string? Workstation { get; set; }    // captured but not shown in the page
        public string? FirstName { get; set; }      // T_Audit.Forename
        public string? LastName { get; set; }       // T_Audit.Surname
        public bool Enabled { get; set; }
        public string? AccessLevels { get; set; }
    }
}
