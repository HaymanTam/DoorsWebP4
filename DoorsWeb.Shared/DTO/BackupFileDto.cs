namespace DoorsWeb.Shared.DTO
{
    /// <summary>Describes a single backup (.bak) file available on the server.</summary>
    public class BackupFileDto
    {
        public string FileName { get; set; } = null!;
        public long SizeBytes { get; set; }
        public DateTime CreatedUtc { get; set; }
    }
}
