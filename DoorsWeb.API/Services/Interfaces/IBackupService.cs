using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface IBackupService
    {
        /// <summary>Lists the backup (.bak) files currently in the server backup directory.</summary>
        List<BackupFileDto> GetBackups();

        /// <summary>Creates a full database backup via native T-SQL BACKUP DATABASE.</summary>
        Task<BackupOperationResult> CreateBackup(CreateBackupRequest request);

        /// <summary>Restores the database from an existing backup via native T-SQL RESTORE DATABASE.</summary>
        Task<BackupOperationResult> RestoreBackup(RestoreBackupRequest request);
    }
}
