namespace DoorsWeb.Shared.DTO
{
    /// <summary>Lightweight site row for the System Settings dialog (list / add / remove).</summary>
    public class SiteDto
    {
        /// <summary>Site id. Server-assigned on create (ignored on the create request).</summary>
        public int Site { get; set; }

        /// <summary>Display name. Required on create; max 30 chars (legacy column length).</summary>
        public string? Name { get; set; }
    }
}
