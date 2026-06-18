using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface ILegacyBackupService
    {
        /// <summary>
        /// Restores a legacy DoorsClient backup into the current database. The backup is a
        /// password-protected ZIP containing per-table <c>.sql</c>/<c>.rs</c> pairs; the <c>.rs</c>
        /// files (ADO ADTG persisted recordsets) carry the data, which is bulk-loaded into the
        /// matching existing tables (identity values preserved).
        /// </summary>
        /// <param name="zipFilePath">Path to the uploaded ZIP saved on the server.</param>
        /// <param name="password">ZIP password (legacy backups are ZipCrypto-encrypted).</param>
        /// <param name="connectionId">Optional SignalR connection id for live progress.</param>
        Task<LegacyRestoreResult> RestoreLegacyBackup(
            string zipFilePath, string password, string? connectionId, CancellationToken cancellationToken = default);
    }
}
