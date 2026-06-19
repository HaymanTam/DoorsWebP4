namespace DoorsWeb.Shared.DTO
{
    /// <summary>The kind of change recorded in an audit entry.</summary>
    public enum AuditAction
    {
        Create,
        Update,
        Delete
    }

    /// <summary>A single change-audit record for the Audit Log Viewer.</summary>
    public class AuditLogDto
    {
        public int Id { get; set; }

        /// <summary>When the change happened.</summary>
        public DateTime Timestamp { get; set; }

        /// <summary>Login name of the user who made the change.</summary>
        public string? UserName { get; set; }

        /// <summary>IP address the change came from.</summary>
        public string? ClientIp { get; set; }

        /// <summary>"Create", "Update" or "Delete".</summary>
        public string? Action { get; set; }

        /// <summary>Kind of record changed (e.g. "Cardholder", "Access Level").</summary>
        public string? EntityType { get; set; }

        /// <summary>Key of the changed record (card number, or id/code).</summary>
        public string? EntityKey { get; set; }

        /// <summary>Human-readable name/description of the changed record.</summary>
        public string? EntityName { get; set; }
    }
}
