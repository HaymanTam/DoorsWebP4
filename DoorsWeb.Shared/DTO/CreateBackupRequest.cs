namespace DoorsWeb.Shared.DTO
{
    /// <summary>Request to create a full database backup.</summary>
    public class CreateBackupRequest
    {
        /// <summary>Optional file name (without path). If empty, a timestamped name is generated.</summary>
        public string? FileName { get; set; }

        /// <summary>Use SQL Server backup compression (not supported on Express/LocalDB editions).</summary>
        public bool Compress { get; set; } = true;

        /// <summary>Optional SignalR connection id. When set, percent-complete progress is pushed to this client.</summary>
        public string? ConnectionId { get; set; }
    }
}
