namespace DoorsWeb.Shared.DTO
{
    /// <summary>Outcome of a backup or restore operation.</summary>
    public class BackupOperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";

        /// <summary>The backup file involved (for create: the file written; for restore: the file read).</summary>
        public string? FileName { get; set; }
    }
}
