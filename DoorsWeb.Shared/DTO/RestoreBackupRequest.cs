namespace DoorsWeb.Shared.DTO
{
    /// <summary>Request to restore the database from an existing backup file.</summary>
    public class RestoreBackupRequest
    {
        /// <summary>File name (without path) of a backup in the server's backup directory.</summary>
        public string FileName { get; set; } = null!;

        /// <summary>Optional SignalR connection id. When set, percent-complete progress is pushed to this client.</summary>
        public string? ConnectionId { get; set; }
    }
}
