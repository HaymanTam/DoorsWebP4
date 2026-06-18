using DoorsWeb.API.Legacy.Adtg;
using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;

namespace DoorsWeb.API.Services
{
    /// <summary>
    /// Restores a legacy DoorsClient backup into the current database.
    ///
    /// A legacy backup is a password-protected (traditional ZipCrypto) ZIP containing one
    /// <c>&lt;Table&gt;.sql</c> + <c>&lt;Table&gt;.rs</c> pair per table. The <c>.sql</c> files are the
    /// original DoorsEnterprise DDL and are ignored — the target tables already exist in this
    /// database. The <c>.rs</c> files are ADO ADTG persisted recordsets holding the row data; each is
    /// parsed with <see cref="AdtgRecordsetReader"/> (a managed, Linux-friendly parser) and streamed
    /// into the matching table via <c>SqlBulkCopy</c>, preserving identity values.
    ///
    /// Foreign-key constraints are disabled for the duration so tables can be cleared and reloaded in
    /// any order, then re-enabled afterwards. Progress is pushed to the initiating client over
    /// <see cref="BackupHub"/> (method <c>LegacyRestoreProgress</c>).
    /// </summary>
    public class LegacyBackupService : ILegacyBackupService
    {
        private readonly string _connectionString;
        private readonly IHubContext<BackupHub> _hub;
        private readonly ILogger<LegacyBackupService> _logger;

        public LegacyBackupService(
            IConfiguration configuration, IHubContext<BackupHub> hub, ILogger<LegacyBackupService> logger)
        {
            _hub = hub;
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public async Task<LegacyRestoreResult> RestoreLegacyBackup(
            string zipFilePath, string password, string? connectionId, CancellationToken cancellationToken = default)
        {
            var result = new LegacyRestoreResult();
            var tempFiles = new List<string>();

            try
            {
                // 1. Read the recordset (.rs) entries out of the encrypted ZIP.
                var entries = new List<(string table, string tempPath, long size)>();
                using (var zip = new ZipFile(zipFilePath) { Password = password })
                {
                    foreach (ZipEntry entry in zip)
                    {
                        if (!entry.IsFile) continue;
                        var fileName = Path.GetFileName(entry.Name);
                        if (!fileName.EndsWith(".rs", StringComparison.OrdinalIgnoreCase)) continue;

                        var table = Path.GetFileNameWithoutExtension(fileName);
                        var tempPath = Path.Combine(
                            Path.GetTempPath(), $"doorsweb_rs_{Guid.NewGuid():N}.rs");
                        tempFiles.Add(tempPath);

                        await using (var input = zip.GetInputStream(entry))
                        await using (var output = File.Create(tempPath))
                        {
                            await input.CopyToAsync(output, cancellationToken);
                        }
                        entries.Add((table, tempPath, entry.Size));
                    }
                }

                if (entries.Count == 0)
                {
                    return new LegacyRestoreResult
                    {
                        Success = false,
                        Message = "No .rs recordset files were found in the archive. Is this a DoorsClient backup?",
                    };
                }

                await using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync(cancellationToken);

                // 2. Match backup tables against tables that actually exist here; skip the rest.
                var existingTables = await GetExistingTables(connection, cancellationToken);
                var toLoad = new List<(string table, string tempPath, long size)>();
                foreach (var e in entries)
                {
                    if (existingTables.Contains(e.table)) toLoad.Add(e);
                    else result.Skipped.Add(e.table);
                }

                if (toLoad.Count == 0)
                {
                    return new LegacyRestoreResult
                    {
                        Success = false,
                        Message = "None of the tables in the archive match tables in the current database.",
                        Skipped = result.Skipped,
                    };
                }

                long totalBytes = Math.Max(1, toLoad.Sum(t => t.size));
                long bytesDone = 0;

                // 3. Disable FK constraints so tables can be cleared/loaded in any order.
                await SetAllConstraints(connection, enable: false, cancellationToken);
                try
                {
                    foreach (var (table, tempPath, size) in toLoad)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        await ClearTable(connection, table, cancellationToken);

                        long baseUnits = bytesDone;
                        long rows = await BulkLoadTable(
                            connection, table, tempPath, connectionId, totalBytes, baseUnits, size, cancellationToken);

                        bytesDone += size;
                        result.Tables.Add(new LegacyTableResult { Table = table, Rows = rows });
                        result.RowsLoaded += rows;
                        Report(connectionId, (int)(bytesDone * 100 / totalBytes));
                    }

                    // Post-load cleanup: the new system is IP-only, so collapse any duplicate UDP
                    // connectors into one and repoint every door at the survivor. Done while FK
                    // constraints are still disabled so the orphan deletes can't trip a reference.
                    result.UdpConnectorsMerged =
                        await ConsolidateUdpConnectors(connection, cancellationToken);
                }
                finally
                {
                    // 4. Re-enable FK constraints (best-effort) regardless of outcome.
                    try { await SetAllConstraints(connection, enable: true, cancellationToken); }
                    catch (Exception ex) { _logger.LogWarning(ex, "Failed to re-enable constraints after legacy restore."); }
                }

                ReportComplete(connectionId);

                result.Success = true;
                result.TablesLoaded = result.Tables.Count;
                var skippedNote = result.Skipped.Count > 0 ? $" Skipped {result.Skipped.Count} unmatched table(s)." : "";
                var mergedNote = result.UdpConnectorsMerged > 0
                    ? $" Merged {result.UdpConnectorsMerged} duplicate UDP connector(s) into one."
                    : "";
                result.Message =
                    $"Restored {result.TablesLoaded} table(s), {result.RowsLoaded:N0} row(s).{skippedNote}{mergedNote}";
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Legacy restore failed.");
                return new LegacyRestoreResult
                {
                    Success = false,
                    Message = $"Legacy restore failed: {DeepMessage(ex)}",
                    Tables = result.Tables,
                    RowsLoaded = result.RowsLoaded,
                    TablesLoaded = result.Tables.Count,
                    Skipped = result.Skipped,
                };
            }
            finally
            {
                foreach (var f in tempFiles)
                {
                    try { if (File.Exists(f)) File.Delete(f); } catch { /* temp cleanup best-effort */ }
                }
            }
        }

        private static async Task<HashSet<string>> GetExistingTables(SqlConnection connection, CancellationToken ct)
        {
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';";
            await using var reader = await cmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct)) set.Add(reader.GetString(0));
            return set;
        }

        // Enable/disable every foreign-key constraint in the database.
        private static async Task SetAllConstraints(SqlConnection connection, bool enable, CancellationToken ct)
        {
            var verb = enable ? "WITH CHECK CHECK CONSTRAINT ALL" : "NOCHECK CONSTRAINT ALL";
            await using var cmd = connection.CreateCommand();
            cmd.CommandText =
                "DECLARE @sql NVARCHAR(MAX) = N'';" +
                "SELECT @sql += N'ALTER TABLE ' + QUOTENAME(SCHEMA_NAME(schema_id)) + N'.' + QUOTENAME(name) + " +
                $"N' {verb};' FROM sys.tables WHERE is_ms_shipped = 0;" +
                "EXEC sp_executesql @sql;";
            cmd.CommandTimeout = 0;
            try
            {
                await cmd.ExecuteNonQueryAsync(ct);
            }
            catch when (enable)
            {
                // Re-enabling WITH CHECK can fail if loaded data violates a constraint (e.g. a missing
                // parent in the backup). Fall back to re-enabling enforcement without validating
                // existing rows, so constraints still apply to future writes.
                await using var fallback = connection.CreateCommand();
                fallback.CommandTimeout = 0;
                fallback.CommandText =
                    "DECLARE @sql NVARCHAR(MAX) = N'';" +
                    "SELECT @sql += N'ALTER TABLE ' + QUOTENAME(SCHEMA_NAME(schema_id)) + N'.' + QUOTENAME(name) + " +
                    "N' CHECK CONSTRAINT ALL;' FROM sys.tables WHERE is_ms_shipped = 0;" +
                    "EXEC sp_executesql @sql;";
                await fallback.ExecuteNonQueryAsync(ct);
            }
        }

        private static async Task ClearTable(SqlConnection connection, string table, CancellationToken ct)
        {
            var quoted = $"[dbo].[{table.Replace("]", "]]")}]";
            await using var cmd = connection.CreateCommand();
            cmd.CommandTimeout = 0;
            try
            {
                cmd.CommandText = $"TRUNCATE TABLE {quoted};";
                await cmd.ExecuteNonQueryAsync(ct);
            }
            catch (SqlException)
            {
                // TRUNCATE is rejected when the table is referenced by a foreign key (even a disabled
                // one); fall back to a logged DELETE.
                cmd.CommandText = $"DELETE FROM {quoted};";
                await cmd.ExecuteNonQueryAsync(ct);
            }
        }

        // Collapses every UDP (Lan/UDP) connector into a single survivor and repoints all doors at it.
        // UDP connectors are T_Connectors rows with ConnType = 3 (legacy mdlConnector.bas gcUDP).
        // Returns the number of duplicate connectors removed (0 if there were 0 or 1 to begin with).
        private static async Task<int> ConsolidateUdpConnectors(SqlConnection connection, CancellationToken ct)
        {
            const int udpConnType = 3;
            await using var cmd = connection.CreateCommand();
            cmd.CommandTimeout = 0;
            cmd.CommandText = @"
SET NOCOUNT ON;
DECLARE @merged INT = 0;
DECLARE @survivor INT = (SELECT MIN([Connector]) FROM [dbo].[T_Connectors] WHERE [ConnType] = @udp);
IF @survivor IS NOT NULL
BEGIN
    -- Repoint doors that point at a soon-to-be-removed UDP connector.
    UPDATE [dbo].[T_Doors]
        SET [Connector] = @survivor
    WHERE [Connector] IN (SELECT [Connector] FROM [dbo].[T_Connectors] WHERE [ConnType] = @udp)
      AND [Connector] <> @survivor;

    -- Drop the now-orphaned duplicate UDP connectors.
    DELETE FROM [dbo].[T_Connectors] WHERE [ConnType] = @udp AND [Connector] <> @survivor;
    SET @merged = @@ROWCOUNT;
END
SELECT @merged;";
            var p = cmd.CreateParameter();
            p.ParameterName = "@udp";
            p.Value = udpConnType;
            cmd.Parameters.Add(p);

            var scalar = await cmd.ExecuteScalarAsync(ct);
            return scalar is int n ? n : Convert.ToInt32(scalar);
        }

        private async Task<long> BulkLoadTable(
            SqlConnection connection, string table, string rsPath, string? connectionId,
            long totalBytes, long baseUnits, long tableBytes, CancellationToken ct)
        {
            using var recordset = AdtgRecordsetReader.Open(rsPath);
            using var dataReader = new AdtgDataReader(recordset);

            using var bulk = new SqlBulkCopy(
                connection, SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.KeepNulls, externalTransaction: null)
            {
                DestinationTableName = $"[dbo].[{table.Replace("]", "]]")}]",
                BulkCopyTimeout = 0,
                BatchSize = 10000,
                NotifyAfter = 50000,
                EnableStreaming = true,
            };

            foreach (var column in recordset.Columns)
            {
                bulk.ColumnMappings.Add(column.Name, column.Name);
            }

            if (!string.IsNullOrEmpty(connectionId))
            {
                double baseFraction = (double)baseUnits / totalBytes;
                double slice = (double)tableBytes / totalBytes;
                bulk.SqlRowsCopied += (_, e) =>
                {
                    // No reliable up-front row count, so ease toward (but never reach) the slice end.
                    double within = 1.0 - 1.0 / (1.0 + e.RowsCopied / 100000.0);
                    int pct = (int)((baseFraction + slice * within) * 100);
                    Report(connectionId, Math.Min(99, pct));
                };
            }

            await bulk.WriteToServerAsync(dataReader, ct);
            return bulk.RowsCopied64;
        }

        // Fire-and-forget progress push to the single initiating client.
        private void Report(string? connectionId, int percent)
        {
            if (string.IsNullOrEmpty(connectionId)) return;
            _ = _hub.Clients.Client(connectionId).SendAsync("LegacyRestoreProgress", percent);
        }

        private void ReportComplete(string? connectionId) => Report(connectionId, 100);

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
