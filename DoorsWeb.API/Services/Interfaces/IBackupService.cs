using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface IBackupService
    {
        /// <summary>Lists the backup (.dump) files currently in the server backup directory.</summary>
        List<BackupFileDto> GetBackups();

        /// <summary>Creates a full database backup via pg_dump (custom format).</summary>
        Task<BackupOperationResult> CreateBackup(CreateBackupRequest request, CancellationToken cancellationToken = default);

        /// <summary>Restores the database from an existing backup via pg_restore (--clean).</summary>
        Task<BackupOperationResult> RestoreBackup(RestoreBackupRequest request, CancellationToken cancellationToken = default);
    }
}
