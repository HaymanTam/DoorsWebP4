using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace DoorsWeb.API.Services
{
    /// <summary>
    /// Database backup/restore via SQL Server Management Objects (SMO). SMO issues native
    /// BACKUP/RESTORE statements server-side  the SQL Server engine does the actual work on
    /// its own disks, so this is exactly as fast as running the statement in SSMS  while
    /// exposing PercentComplete events. Those events are forwarded over SignalR (BackupHub)
    /// to the initiating client so the UI can render a live progress bar.
    ///
    /// NOTE: the SQL Server *service account* (not the API process) writes/reads the .bak file,
    /// so <c>Backup:Directory</c> must be a path that account can access. For a local single-box
    /// dev setup this is typically fine.
    /// </summary>
    public class BackupService : IBackupService
    {
        private readonly string _connectionString;
        private readonly string _backupDirectory;
        private readonly string _databaseName;
        private readonly IHubContext<BackupHub> _hub;

        public BackupService(IConfiguration configuration, IHubContext<BackupHub> hub)
        {
            _hub = hub;

            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            _databaseName = new SqlConnectionStringBuilder(_connectionString).InitialCatalog;
            if (string.IsNullOrWhiteSpace(_databaseName))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' has no Initial Catalog / Database.");
            }

            var configured = configuration["Backup:Directory"];
            _backupDirectory = string.IsNullOrWhiteSpace(configured)
                ? Path.Combine(AppContext.BaseDirectory, "Backups")
                : configured;

            Directory.CreateDirectory(_backupDirectory);
        }

        public List<BackupFileDto> GetBackups()
        {
            if (!Directory.Exists(_backupDirectory))
            {
                return new List<BackupFileDto>();
            }

            return Directory.EnumerateFiles(_backupDirectory, "*.bak")
                .Select(path => new FileInfo(path))
                .OrderByDescending(f => f.CreationTimeUtc)
                .Select(f => new BackupFileDto
                {
                    FileName = f.Name,
                    SizeBytes = f.Length,
                    CreatedUtc = f.CreationTimeUtc
                })
                .ToList();
        }

        public async Task<BackupOperationResult> CreateBackup(CreateBackupRequest request)
        {
            var fileName = string.IsNullOrWhiteSpace(request.FileName)
                ? $"{_databaseName}_{DateTime.Now:yyyyMMdd_HHmmss}.bak"
                : EnsureBakExtension(Path.GetFileName(request.FileName));

            var fullPath = Path.Combine(_backupDirectory, fileName);

            try
            {
                await Task.Run(() =>
                {
                    using var sqlConn = new SqlConnection(_connectionString);
                    var serverConn = new ServerConnection(sqlConn) { StatementTimeout = 0 };
                    var server = new Server(serverConn);

                    var backup = new Backup
                    {
                        Action = BackupActionType.Database,
                        Database = _databaseName,
                        Initialize = true,          // WITH FORMAT, INIT  overwrite the device
                        Checksum = true,            // WITH CHECKSUM
                        CompressionOption = request.Compress
                            ? BackupCompressionOptions.On
                            : BackupCompressionOptions.Off,
                        PercentCompleteNotification = 5,
                    };
                    backup.Devices.AddDevice(fullPath, DeviceType.File);

                    if (!string.IsNullOrEmpty(request.ConnectionId))
                    {
                        backup.PercentComplete += (_, e) => Report(request.ConnectionId, "BackupProgress", e.Percent);
                    }

                    backup.SqlBackup(server);
                });

                ReportComplete(request.ConnectionId, "BackupProgress");

                return new BackupOperationResult
                {
                    Success = true,
                    Message = $"Backup completed: {fileName}",
                    FileName = fileName
                };
            }
            catch (Exception ex)
            {
                return new BackupOperationResult
                {
                    Success = false,
                    Message = $"Backup failed: {DeepMessage(ex)}",
                    FileName = fileName
                };
            }
        }

        public async Task<BackupOperationResult> RestoreBackup(RestoreBackupRequest request)
        {
            var fileName = EnsureBakExtension(Path.GetFileName(request.FileName ?? ""));
            var fullPath = Path.Combine(_backupDirectory, fileName);

            if (!File.Exists(fullPath))
            {
                return new BackupOperationResult
                {
                    Success = false,
                    Message = $"Backup file '{fileName}' was not found.",
                    FileName = fileName
                };
            }

            // Restore must run against master and requires exclusive access to the target DB.
            var masterConnectionString = new SqlConnectionStringBuilder(_connectionString)
            {
                InitialCatalog = "master"
            }.ConnectionString;

            var db = QuoteName(_databaseName);

            try
            {
                await Task.Run(() =>
                {
                    using var sqlConn = new SqlConnection(masterConnectionString);
                    var serverConn = new ServerConnection(sqlConn) { StatementTimeout = 0 };
                    var server = new Server(serverConn);

                    // Kick out existing connections so the restore isn't blocked.
                    server.ConnectionContext.ExecuteNonQuery(
                        $"ALTER DATABASE {db} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;");

                    try
                    {
                        var restore = new Restore
                        {
                            Database = _databaseName,
                            ReplaceDatabase = true,     // WITH REPLACE
                            NoRecovery = false,         // WITH RECOVERY  bring DB online after
                            PercentCompleteNotification = 5,
                        };
                        restore.Devices.AddDevice(fullPath, DeviceType.File);

                        if (!string.IsNullOrEmpty(request.ConnectionId))
                        {
                            restore.PercentComplete += (_, e) => Report(request.ConnectionId, "RestoreProgress", e.Percent);
                        }

                        restore.SqlRestore(server);
                    }
                    finally
                    {
                        // Best-effort: return the DB to multi-user even if the restore failed.
                        try { server.ConnectionContext.ExecuteNonQuery($"ALTER DATABASE {db} SET MULTI_USER;"); }
                        catch { /* DB may be mid-restore; ignore */ }
                    }
                });

                ReportComplete(request.ConnectionId, "RestoreProgress");

                return new BackupOperationResult
                {
                    Success = true,
                    Message = $"Restore completed from: {fileName}",
                    FileName = fileName
                };
            }
            catch (Exception ex)
            {
                return new BackupOperationResult
                {
                    Success = false,
                    Message = $"Restore failed: {DeepMessage(ex)}",
                    FileName = fileName
                };
            }
        }

        // Fire-and-forget progress push to the single initiating client.
        private void Report(string? connectionId, string method, int percent)
        {
            if (string.IsNullOrEmpty(connectionId)) return;
            _ = _hub.Clients.Client(connectionId).SendAsync(method, percent);
        }

        private void ReportComplete(string? connectionId, string method) => Report(connectionId, method, 100);

        private static string EnsureBakExtension(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
            }
            return fileName.EndsWith(".bak", StringComparison.OrdinalIgnoreCase) ? fileName : fileName + ".bak";
        }

        // BACKUP/RESTORE can't parameterize the database name, so bracket-quote it safely.
        private static string QuoteName(string identifier) => "[" + identifier.Replace("]", "]]") + "]";

        // SMO wraps the real SQL error in nested exceptions; surface the whole chain.
        private static string DeepMessage(Exception ex)
        {
            var message = ex.Message;
            var inner = ex.InnerException;
            while (inner is not null)
            {
                message += " " + inner.Message;
                inner = inner.InnerException;
            }
            return message;
        }
    }
}
