using DoorsWeb.API.Legacy.Adtg;
using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.AspNetCore.SignalR;
using Npgsql;
using NpgsqlTypes;

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
    /// into the matching table via PostgreSQL's binary <c>COPY</c> protocol, preserving identity values.
    ///
    /// Referential-integrity triggers are disabled for the duration (<c>session_replication_role =
    /// replica</c>) so tables can be cleared and reloaded in any order, then re-enabled afterwards.
    /// Because explicit identity values are loaded, each table's identity sequence is bumped past its
    /// new maximum at the end. Progress is pushed to the initiating client over
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

                await using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync(cancellationToken);

                // 2. Match backup tables against tables that actually exist here; skip the rest.
                //    PostgreSQL identifiers are case-sensitive, so resolve each backup table to the
                //    real on-disk name (the .rs file name may differ in case).
                var existingTables = await GetExistingTables(connection, cancellationToken);
                var toLoad = new List<(string table, string tempPath, long size)>();
                foreach (var e in entries)
                {
                    if (existingTables.TryGetValue(e.table, out var actual)) toLoad.Add((actual, e.tempPath, e.size));
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

                // 3. Disable FK/trigger enforcement so tables can be cleared/loaded in any order.
                await SetReplicationRole(connection, replica: true, cancellationToken);
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
                    // connectors into one and repoint every door at the survivor. Done while trigger
                    // enforcement is still disabled so the orphan deletes can't trip a reference.
                    result.UdpConnectorsMerged =
                        await ConsolidateUdpConnectors(connection, cancellationToken);

                    // Explicit identity values were just loaded; advance each identity sequence past
                    // its new maximum so future inserts don't collide.
                    await ResetIdentitySequences(connection, cancellationToken);
                }
                finally
                {
                    // 4. Re-enable FK/trigger enforcement (best-effort) regardless of outcome.
                    try { await SetReplicationRole(connection, replica: false, cancellationToken); }
                    catch (Exception ex) { _logger.LogWarning(ex, "Failed to re-enable trigger enforcement after legacy restore."); }
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

        // Maps each base table in the public schema by a case-insensitive key to its real name.
        private static async Task<Dictionary<string, string>> GetExistingTables(NpgsqlConnection connection, CancellationToken ct)
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            await using var cmd = new NpgsqlCommand(
                "SELECT table_name FROM information_schema.tables " +
                "WHERE table_schema = 'public' AND table_type = 'BASE TABLE';", connection);
            await using var reader = await cmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                var name = reader.GetString(0);
                map[name] = name;
            }
            return map;
        }

        // Maps each column of a table by a case-insensitive key to its real name.
        private static async Task<Dictionary<string, string>> GetTableColumns(
            NpgsqlConnection connection, string table, CancellationToken ct)
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            await using var cmd = new NpgsqlCommand(
                "SELECT column_name FROM information_schema.columns " +
                "WHERE table_schema = 'public' AND table_name = @t;", connection);
            cmd.Parameters.AddWithValue("t", table);
            await using var reader = await cmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                var name = reader.GetString(0);
                map[name] = name;
            }
            return map;
        }

        // Toggle session-level referential-integrity enforcement. 'replica' suppresses user and
        // FK triggers for this connection so bulk loads can run in any order; DEFAULT restores them.
        private static async Task SetReplicationRole(NpgsqlConnection connection, bool replica, CancellationToken ct)
        {
            await using var cmd = new NpgsqlCommand(
                replica ? "SET session_replication_role = replica;" : "SET session_replication_role = DEFAULT;",
                connection);
            await cmd.ExecuteNonQueryAsync(ct);
        }

        private static async Task ClearTable(NpgsqlConnection connection, string table, CancellationToken ct)
        {
            var quoted = QualifiedTable(table);
            await using var cmd = new NpgsqlCommand($"TRUNCATE TABLE {quoted};", connection) { CommandTimeout = 0 };
            try
            {
                await cmd.ExecuteNonQueryAsync(ct);
            }
            catch (PostgresException)
            {
                // TRUNCATE is rejected when the table is referenced by a foreign key from a table that
                // isn't also being truncated; fall back to a plain DELETE.
                await using var fallback = new NpgsqlCommand($"DELETE FROM {quoted};", connection) { CommandTimeout = 0 };
                await fallback.ExecuteNonQueryAsync(ct);
            }
        }

        // Collapses every UDP (Lan/UDP) connector into a single survivor and repoints all doors at it.
        // UDP connectors are T_Connectors rows with ConnType = 3 (legacy mdlConnector.bas gcUDP).
        // Returns the number of duplicate connectors removed (0 if there were 0 or 1 to begin with).
        private static async Task<int> ConsolidateUdpConnectors(NpgsqlConnection connection, CancellationToken ct)
        {
            const int udpConnType = 3;

            await using var survivorCmd = new NpgsqlCommand(
                "SELECT MIN(\"Connector\") FROM \"public\".\"T_Connectors\" WHERE \"ConnType\" = @udp;", connection)
            { CommandTimeout = 0 };
            survivorCmd.Parameters.AddWithValue("udp", udpConnType);

            var survivorObj = await survivorCmd.ExecuteScalarAsync(ct);
            if (survivorObj is null || survivorObj is DBNull) return 0;
            int survivor = Convert.ToInt32(survivorObj);

            // Repoint doors that point at a soon-to-be-removed UDP connector.
            await using (var update = new NpgsqlCommand(
                "UPDATE \"public\".\"T_Doors\" SET \"Connector\" = @survivor " +
                "WHERE \"Connector\" IN (SELECT \"Connector\" FROM \"public\".\"T_Connectors\" WHERE \"ConnType\" = @udp) " +
                "AND \"Connector\" <> @survivor;", connection)
            { CommandTimeout = 0 })
            {
                update.Parameters.AddWithValue("survivor", survivor);
                update.Parameters.AddWithValue("udp", udpConnType);
                await update.ExecuteNonQueryAsync(ct);
            }

            // Drop the now-orphaned duplicate UDP connectors; the affected-row count is the merge total.
            await using var delete = new NpgsqlCommand(
                "DELETE FROM \"public\".\"T_Connectors\" WHERE \"ConnType\" = @udp AND \"Connector\" <> @survivor;", connection)
            { CommandTimeout = 0 };
            delete.Parameters.AddWithValue("udp", udpConnType);
            delete.Parameters.AddWithValue("survivor", survivor);
            return await delete.ExecuteNonQueryAsync(ct);
        }

        // Bumps every identity sequence in the public schema past the current max value of its column,
        // so inserts after a KeepIdentity-style load don't collide with loaded values.
        private static async Task ResetIdentitySequences(NpgsqlConnection connection, CancellationToken ct)
        {
            const string sql = @"
DO $$
DECLARE r record; seq text; maxid bigint;
BEGIN
    FOR r IN
        SELECT table_name, column_name
        FROM information_schema.columns
        WHERE table_schema = 'public' AND is_identity = 'YES'
    LOOP
        seq := pg_get_serial_sequence(format('public.%I', r.table_name), r.column_name);
        IF seq IS NOT NULL THEN
            EXECUTE format('SELECT COALESCE(MAX(%I), 0) FROM public.%I', r.column_name, r.table_name) INTO maxid;
            PERFORM setval(seq, GREATEST(maxid, 1), maxid > 0);
        END IF;
    END LOOP;
END $$;";
            await using var cmd = new NpgsqlCommand(sql, connection) { CommandTimeout = 0 };
            await cmd.ExecuteNonQueryAsync(ct);
        }

        private async Task<long> BulkLoadTable(
            NpgsqlConnection connection, string table, string rsPath, string? connectionId,
            long totalBytes, long baseUnits, long tableBytes, CancellationToken ct)
        {
            using var recordset = AdtgRecordsetReader.Open(rsPath);
            using var dataReader = new AdtgDataReader(recordset);

            // Resolve recordset columns to the real (case-correct) destination column names and pick
            // an explicit PostgreSQL type per column so the binary COPY stream is unambiguous.
            var dbColumns = await GetTableColumns(connection, table, ct);
            int count = recordset.Columns.Count;
            var targetNames = new string[count];
            var types = new NpgsqlDbType[count];
            for (int i = 0; i < count; i++)
            {
                var col = recordset.Columns[i];
                if (!dbColumns.TryGetValue(col.Name, out var actual))
                    throw new InvalidOperationException(
                        $"Column '{col.Name}' from the backup does not exist in table '{table}'.");
                targetNames[i] = actual;
                types[i] = NpgsqlTypeOf(col);
            }

            var columnList = string.Join(", ", targetNames.Select(Quote));
            var copyCommand = $"COPY {QualifiedTable(table)} ({columnList}) FROM STDIN (FORMAT BINARY)";

            double baseFraction = (double)baseUnits / totalBytes;
            double slice = (double)tableBytes / totalBytes;
            bool report = !string.IsNullOrEmpty(connectionId);

            await using var importer = await connection.BeginBinaryImportAsync(copyCommand, ct);
            long rows = 0;
            while (dataReader.Read())
            {
                await importer.StartRowAsync(ct);
                for (int i = 0; i < count; i++)
                {
                    if (dataReader.IsDBNull(i))
                        await importer.WriteNullAsync(ct);
                    else
                        await importer.WriteAsync(dataReader.GetValue(i), types[i], ct);
                }

                rows++;
                if (report && rows % 50000 == 0)
                {
                    // No reliable up-front row count, so ease toward (but never reach) the slice end.
                    double within = 1.0 - 1.0 / (1.0 + rows / 100000.0);
                    int pct = (int)((baseFraction + slice * within) * 100);
                    Report(connectionId, Math.Min(99, pct));
                }
            }

            ulong written = await importer.CompleteAsync(ct);
            return (long)written;
        }

        // PostgreSQL maps for the CLR types the ADTG parser produces.
        private static NpgsqlDbType NpgsqlTypeOf(AdtgColumn column)
        {
            var t = AdtgRecordsetReader.ClrTypeOf(column);
            if (t == typeof(int)) return NpgsqlDbType.Integer;
            if (t == typeof(short)) return NpgsqlDbType.Smallint;
            if (t == typeof(bool)) return NpgsqlDbType.Boolean;
            if (t == typeof(float)) return NpgsqlDbType.Real;
            if (t == typeof(DateTime)) return NpgsqlDbType.Timestamp;
            return NpgsqlDbType.Text;
        }

        private static string Quote(string identifier) => "\"" + identifier.Replace("\"", "\"\"") + "\"";

        private static string QualifiedTable(string table) => $"\"public\".{Quote(table)}";

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
