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
    /// The archive may also contain a <c>Photos/</c> folder holding one cardholder photo per card,
    /// named <c>PH&lt;CardNumber&gt;.jpg</c> (e.g. card 1037 → <c>PH001037.jpg</c>). These are copied
    /// into the card-photo store renamed to the new system's convention (<c>&lt;CardNumber&gt;.jpg</c>),
    /// replacing any existing photos. A backup with no <c>Photos/</c> folder leaves photos untouched.
    ///
    /// It may also contain a <c>Floors/</c> folder holding one floorplan background image per legacy
    /// floorplan, named <c>Floors/&lt;PlanCode&gt;.jpg</c> (matching <c>T_FloorPlans.Code</c>). Because
    /// this system keeps a single floorplan per site, exactly one plan per site (the lowest-coded one)
    /// is imported into the floorplan image store via <see cref="IFloorPlanService"/>. Door placements
    /// are <i>not</i> carried over — the legacy positions use a vector-design coordinate model that does
    /// not map onto this system's resolution-independent percentages — so the restored layout holds just
    /// the image and the operator re-pins doors from the floorplan editor.
    ///
    /// Referential-integrity triggers are disabled for the duration (<c>session_replication_role =
    /// replica</c>) so tables can be cleared and reloaded in any order, then re-enabled afterwards.
    /// Because explicit identity values are loaded, each table's identity sequence is bumped past its
    /// new maximum at the end. Progress is pushed to the initiating client over
    /// <see cref="BackupHub"/> (method <c>LegacyRestoreProgress</c>).
    /// </summary>
    public class LegacyBackupService : ILegacyBackupService
    {
        // The new-system audit log. It has no legacy counterpart, so a legacy backup never carries
        // matching column data; rather than risk a column-mismatch on import, it is never loaded. It
        // is, however, cleared on every legacy restore: the imported data is a different point in time,
        // so the existing audit entries would reference entities that no longer exist. The import UI
        // warns that all previous audit history is dropped.
        private const string AuditTable = "T_Audit";

        // Legacy stores each cardholder photo in a "Photos" folder named PH<CardNumber>.jpg. The new
        // system stores one photo per card named <CardNumber>.<ext> in the card-photo store. These are
        // the image extensions we recognise for the rename-and-copy (mirrors CardPhotoService).
        private static readonly HashSet<string> PhotoExtensions =
            new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        private readonly string _connectionString;
        private readonly IHubContext<BackupHub> _hub;
        private readonly ILogger<LegacyBackupService> _logger;
        private readonly ICardPhotoService _cardPhoto;
        private readonly IFloorPlanService _floorPlan;

        public LegacyBackupService(
            IConfiguration configuration, IHubContext<BackupHub> hub, ILogger<LegacyBackupService> logger,
            ICardPhotoService cardPhoto, IFloorPlanService floorPlan)
        {
            _hub = hub;
            _logger = logger;
            _cardPhoto = cardPhoto;
            _floorPlan = floorPlan;
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
                // 1. Read the recordset (.rs) entries — and any cardholder photos — out of the
                //    encrypted ZIP into temp files. Photos live in a "Photos/" folder named
                //    PH<CardNumber>.jpg and are processed (renamed + copied) after the table load.
                var entries = new List<(string table, string tempPath, long size)>();
                var photos = new List<(int cardNumber, string ext, string tempPath)>();
                var floors = new List<(int planCode, string ext, string tempPath)>();
                using (var zip = new ZipFile(zipFilePath) { Password = password })
                {
                    foreach (ZipEntry entry in zip)
                    {
                        if (!entry.IsFile) continue;
                        var fileName = Path.GetFileName(entry.Name);

                        if (fileName.EndsWith(".rs", StringComparison.OrdinalIgnoreCase))
                        {
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
                        else if (IsCardholderPhoto(entry, out var cardNumber, out var ext))
                        {
                            var tempPath = Path.Combine(
                                Path.GetTempPath(), $"doorsweb_photo_{Guid.NewGuid():N}{ext}");
                            tempFiles.Add(tempPath);

                            await using (var input = zip.GetInputStream(entry))
                            await using (var output = File.Create(tempPath))
                            {
                                await input.CopyToAsync(output, cancellationToken);
                            }
                            photos.Add((cardNumber, ext, tempPath));
                        }
                        else if (IsFloorImage(entry, out var planCode, out var floorExt))
                        {
                            var tempPath = Path.Combine(
                                Path.GetTempPath(), $"doorsweb_floor_{Guid.NewGuid():N}{floorExt}");
                            tempFiles.Add(tempPath);

                            await using (var input = zip.GetInputStream(entry))
                            await using (var output = File.Create(tempPath))
                            {
                                await input.CopyToAsync(output, cancellationToken);
                            }
                            floors.Add((planCode, floorExt, tempPath));
                        }
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
                    if (!existingTables.TryGetValue(e.table, out var actual))
                    {
                        result.Skipped.Add(e.table);
                        continue;
                    }
                    // Never load into the audit table (cleared separately below); a legacy backup has
                    // no matching schema for it, so loading would throw on the column mismatch.
                    if (string.Equals(actual, AuditTable, StringComparison.OrdinalIgnoreCase))
                    {
                        result.Skipped.Add(e.table);
                        continue;
                    }
                    toLoad.Add((actual, e.tempPath, e.size));
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
                    // Drop all previous audit history. The restored data is a different point in time,
                    // so existing audit entries would reference entities that no longer exist. It is
                    // never reloaded from the backup (legacy has no audit table). Warned about in the UI.
                    if (existingTables.TryGetValue(AuditTable, out var auditTable))
                        await ClearTable(connection, auditTable, cancellationToken);

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

                    // Explicit identity values were just loaded; advance each identity sequence past
                    // its new maximum so future inserts don't collide.
                    await ResetIdentitySequences(connection, cancellationToken);

                    // A legacy backup carries no granular permission data, so every imported user would
                    // otherwise land with no access — and there might be no Super left to administer the
                    // system. Promote all imported users to full access (Super + ReadWrite on every area)
                    // so the operator isn't locked out. This is warned about loudly in the import UI.
                    if (existingTables.TryGetValue("T_Users", out var usersTable)
                        && toLoad.Any(t => string.Equals(t.table, usersTable, StringComparison.OrdinalIgnoreCase)))
                    {
                        result.UsersGrantedFullAccess =
                            await GrantFullAccessToAllUsers(connection, usersTable, cancellationToken);
                    }

                    // Restore cardholder photos from the backup's Photos/ folder (filesystem only,
                    // independent of the DB). No-op when the backup carried no photos.
                    result.PhotosRestored = await RestorePhotos(photos, cancellationToken);

                    // Restore floorplan background images from the backup's Floors/ folder. One plan
                    // per site is imported into the floorplan store (this system keeps a single
                    // floorplan per site). Uses the just-loaded T_FloorPlans rows to map each image to
                    // its site. No-op when the backup carried no floor images.
                    result.FloorPlansRestored =
                        await RestoreFloorPlans(connection, existingTables, floors, cancellationToken);

                    // Read-only integrity scan. The bulk COPY ran with FK enforcement off
                    // (session_replication_role = replica), and PostgreSQL does NOT retro-validate
                    // rows loaded that way, so a legacy backup can leave orphans behind even though the
                    // constraints are nominally valid. Report them (and NULL names that crash the legacy
                    // client) for review — this pass changes nothing.
                    result.Findings = await ScanIntegrity(connection, cancellationToken);
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
                var photoNote = result.PhotosRestored > 0 ? $" Restored {result.PhotosRestored:N0} photo(s)." : "";
                var floorNote = result.FloorPlansRestored > 0 ? $" Restored {result.FloorPlansRestored:N0} floorplan(s)." : "";
                result.Message =
                    $"Restored {result.TablesLoaded} table(s), {result.RowsLoaded:N0} row(s).{photoNote}{floorNote}{skippedNote}";
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

        // Promotes every row in the users table to full access: Super (Administrator = true) plus
        // ReadWrite (= 2) on each of the three permission areas. A legacy backup carries no granular
        // permission data, so without this every imported user would land with no access and the
        // operator could be locked out entirely. The SET clause is built only from columns that
        // actually exist, so it is tolerant of older schemas. Returns the number of users updated.
        private static async Task<long> GrantFullAccessToAllUsers(
            NpgsqlConnection connection, string usersTable, CancellationToken ct)
        {
            var columns = await GetTableColumns(connection, usersTable, ct);
            var sets = new List<string>();
            if (columns.TryGetValue("Administrator", out var a)) sets.Add($"{Quote(a)} = true");
            if (columns.TryGetValue("CardManagerAccess", out var cm)) sets.Add($"{Quote(cm)} = 2");
            if (columns.TryGetValue("SiteSettingsAccess", out var ss)) sets.Add($"{Quote(ss)} = 2");
            if (columns.TryGetValue("UserSettingsAccess", out var us)) sets.Add($"{Quote(us)} = 2");
            if (columns.TryGetValue("ReportsAccess", out var rp)) sets.Add($"{Quote(rp)} = 2");
            if (sets.Count == 0) return 0;

            var sql = $"UPDATE {QualifiedTable(usersTable)} SET {string.Join(", ", sets)};";
            await using var cmd = new NpgsqlCommand(sql, connection) { CommandTimeout = 0 };
            return await cmd.ExecuteNonQueryAsync(ct);
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

        // Recognises a cardholder photo entry: an image file in the archive's top-level "Photos"
        // folder named PH<CardNumber>[.ext]. Legacy zero-pads the number to six digits (e.g. card
        // 1037 → PH001037.jpg). Yields the parsed card number and the lower-cased extension. Entries
        // outside Photos/, with an unknown extension, or without a positive card number are rejected.
        private static bool IsCardholderPhoto(ZipEntry entry, out int cardNumber, out string ext)
        {
            cardNumber = 0;
            ext = string.Empty;

            var normalized = entry.Name.Replace('\\', '/');
            if (!normalized.StartsWith("Photos/", StringComparison.OrdinalIgnoreCase)) return false;

            var fileExt = Path.GetExtension(normalized);
            if (string.IsNullOrEmpty(fileExt) || !PhotoExtensions.Contains(fileExt)) return false;

            var name = Path.GetFileNameWithoutExtension(normalized).Trim();
            if (name.StartsWith("PH", StringComparison.OrdinalIgnoreCase))
                name = name.Substring(2);

            if (!int.TryParse(name, out var n) || n <= 0) return false;

            cardNumber = n;
            ext = fileExt.ToLowerInvariant();
            return true;
        }

        // Restores cardholder photos (already extracted to temp files) into the card-photo store,
        // renamed from the legacy PH<CardNumber>.jpg convention to the new system's <CardNumber>.<ext>.
        // The restore replaces the photo set wholesale — mirroring the truncate-then-load of the
        // tables — so every existing card photo is removed first, then the backup's photos written.
        // Called only when the backup actually carried photos, so a photo-less backup leaves the
        // existing photos untouched. Filesystem only; independent of the DB transaction. Returns the
        // number of photos restored.
        private async Task<long> RestorePhotos(
            IReadOnlyList<(int cardNumber, string ext, string tempPath)> photos, CancellationToken ct)
        {
            if (photos.Count == 0) return 0;

            var dir = _cardPhoto.PhysicalDirectory;
            Directory.CreateDirectory(dir);

            // Wholesale replace: drop every existing card photo before importing the backup's set.
            foreach (var existing in Directory.EnumerateFiles(dir))
            {
                if (!PhotoExtensions.Contains(Path.GetExtension(existing))) continue;
                try { File.Delete(existing); }
                catch (Exception ex) { _logger.LogWarning(ex, "Could not delete existing card photo {File}.", existing); }
            }

            long restored = 0;
            foreach (var (cardNumber, ext, tempPath) in photos)
            {
                ct.ThrowIfCancellationRequested();

                var dest = Path.Combine(dir, $"{cardNumber}{ext}");
                await using (var input = File.OpenRead(tempPath))
                await using (var output = File.Create(dest))
                {
                    await input.CopyToAsync(output, ct);
                }
                restored++;
            }

            _logger.LogInformation("Legacy import: restored {Count} cardholder photo(s).", restored);
            return restored;
        }

        // Recognises a floorplan background-image entry: an image file in the archive's top-level
        // "Floors" folder named <PlanCode>[.ext] (e.g. Floors/1.jpg, Floors/2.jpg). The number is the
        // legacy floorplan's Code (T_FloorPlans.Code), which links it to a site. Yields the parsed plan
        // code and the lower-cased extension. Entries outside Floors/, with an unknown extension, or
        // without a positive plan code are rejected.
        private static bool IsFloorImage(ZipEntry entry, out int planCode, out string ext)
        {
            planCode = 0;
            ext = string.Empty;

            var normalized = entry.Name.Replace('\\', '/');
            if (!normalized.StartsWith("Floors/", StringComparison.OrdinalIgnoreCase)) return false;

            var fileExt = Path.GetExtension(normalized);
            if (string.IsNullOrEmpty(fileExt) || !PhotoExtensions.Contains(fileExt)) return false;

            var name = Path.GetFileNameWithoutExtension(normalized).Trim();
            if (!int.TryParse(name, out var n) || n <= 0) return false;

            planCode = n;
            ext = fileExt.ToLowerInvariant();
            return true;
        }

        // Restores floorplan background images (already extracted to temp files) into the floorplan
        // store. Legacy keeps multiple floorplans per site; this system keeps exactly one, so a single
        // plan per site is imported — the lowest-coded one (typically the ground/first floor). Each
        // image is mapped to a site through the just-loaded T_FloorPlans rows (Floors/<Code>.jpg →
        // T_FloorPlans.Code → Site); if a backup has no T_FloorPlans table but the database has exactly
        // one site, a lone floor image is attached to that site. Door placements are NOT imported (the
        // legacy positions use a vector-design coordinate model that doesn't map onto this system's
        // resolution-independent percentages), so the saved layout is image-only and the operator
        // re-pins doors in the floorplan editor. Filesystem only, independent of the DB transaction.
        // Returns the number of floorplan images restored.
        private async Task<long> RestoreFloorPlans(
            NpgsqlConnection connection,
            IReadOnlyDictionary<string, string> existingTables,
            IReadOnlyList<(int planCode, string ext, string tempPath)> floors,
            CancellationToken ct)
        {
            if (floors.Count == 0) return 0;

            // Map each legacy floorplan (by its Code) to the site it belongs to.
            var planToSite = new Dictionary<int, int>();
            if (existingTables.TryGetValue("T_FloorPlans", out var fpTable))
            {
                await using var cmd = new NpgsqlCommand(
                    $"SELECT \"Code\", \"Site\" FROM {QualifiedTable(fpTable)};", connection);
                await using var reader = await cmd.ExecuteReaderAsync(ct);
                while (await reader.ReadAsync(ct))
                {
                    if (reader.IsDBNull(0) || reader.IsDBNull(1)) continue;
                    planToSite[reader.GetInt32(0)] = reader.GetInt32(1);
                }
            }

            // Fallback site for a floor image with no matching T_FloorPlans row — only safe to guess
            // when the database holds exactly one site.
            int? soleSite = null;
            if (existingTables.TryGetValue("T_Sites", out var sitesTable))
            {
                var ids = new List<int>();
                await using var cmd = new NpgsqlCommand(
                    $"SELECT \"Site\" FROM {QualifiedTable(sitesTable)};", connection);
                await using var reader = await cmd.ExecuteReaderAsync(ct);
                while (await reader.ReadAsync(ct))
                    if (!reader.IsDBNull(0)) ids.Add(reader.GetInt32(0));
                if (ids.Count == 1) soleSite = ids[0];
            }

            // Resolve each floor image to a site, then keep one plan per site (one-to-one). Iterating in
            // ascending plan-code order means the lowest code wins when a site has several floors.
            var chosen = new Dictionary<int, (int planCode, string ext, string tempPath)>();
            foreach (var f in floors.OrderBy(f => f.planCode))
            {
                int? site = planToSite.TryGetValue(f.planCode, out var s) ? s : soleSite;
                if (site is null || chosen.ContainsKey(site.Value)) continue;
                chosen[site.Value] = f;
            }

            long restored = 0;
            foreach (var (site, f) in chosen)
            {
                ct.ThrowIfCancellationRequested();

                FloorPlanLayoutDto layout;
                await using (var input = File.OpenRead(f.tempPath))
                {
                    layout = await _floorPlan.SaveImageAsync(site, input, $"floor{f.ext}", ct);
                }

                // The whole dataset was just replaced; any door placements carried over from a previous
                // DoorsWeb layout would reference doors that may no longer exist. Reset to image-only.
                if (layout.Doors.Count > 0)
                {
                    layout.Doors.Clear();
                    _floorPlan.Save(layout);
                }
                restored++;
            }

            if (restored > 0)
                _logger.LogInformation("Legacy import: restored {Count} floorplan image(s).", restored);
            return restored;
        }

        // One foreign key as read from the catalog: ordered child/parent column lists included so
        // composite and Site-scoped keys are handled without hardcoding the relationship map.
        private sealed record ForeignKeyInfo(
            string Name, string ChildTable, string ParentTable,
            IReadOnlyList<string> ChildColumns, IReadOnlyList<string> ParentColumns);

        // Enumerates every foreign key in the public schema directly from pg_constraint, resolving the
        // child/parent table names and the ordered child/parent column lists. This lets the orphan scan
        // cover all live relationships (including composite, Site-scoped keys) without hardcoding them.
        private static async Task<List<ForeignKeyInfo>> GetForeignKeys(NpgsqlConnection connection, CancellationToken ct)
        {
            const string sql = @"
SELECT con.conname,
       child.relname  AS child_table,
       parent.relname AS parent_table,
       (SELECT array_agg(att.attname ORDER BY k.ord)
          FROM unnest(con.conkey) WITH ORDINALITY AS k(attnum, ord)
          JOIN pg_attribute att ON att.attrelid = con.conrelid AND att.attnum = k.attnum) AS child_cols,
       (SELECT array_agg(att.attname ORDER BY k.ord)
          FROM unnest(con.confkey) WITH ORDINALITY AS k(attnum, ord)
          JOIN pg_attribute att ON att.attrelid = con.confrelid AND att.attnum = k.attnum) AS parent_cols
FROM pg_constraint con
JOIN pg_class child  ON child.oid  = con.conrelid
JOIN pg_class parent ON parent.oid = con.confrelid
JOIN pg_namespace ns ON ns.oid = con.connamespace
WHERE con.contype = 'f' AND ns.nspname = 'public'
ORDER BY child.relname, con.conname;";

            var list = new List<ForeignKeyInfo>();
            await using var cmd = new NpgsqlCommand(sql, connection);
            await using var reader = await cmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                list.Add(new ForeignKeyInfo(
                    reader.GetString(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    (string[])reader.GetValue(3),
                    (string[])reader.GetValue(4)));
            }
            return list;
        }

        // Read-only post-restore integrity scan. Returns one finding per problem found and changes
        // nothing — the caller surfaces these for review.
        //
        // Two kinds of findings:
        //   "orphan"     - rows whose FK column(s) reference a parent row that doesn't exist. The bulk
        //                  COPY ran under session_replication_role = replica (enforcement off) and
        //                  PostgreSQL does NOT retro-validate those rows when enforcement is restored,
        //                  so a legacy backup can leave orphans behind even though the constraints are
        //                  nominally valid. A NULL FK column is skipped, so legitimate legacy "none"
        //                  links (loaded as NULL) are not flagged.
        //   "blank-name" - a name/description column that is NULL on a row the legacy VB6 client
        //                  dereferences; a NULL there throws run-time error 91 when the record opens.
        private async Task<List<LegacyScanFinding>> ScanIntegrity(NpgsqlConnection connection, CancellationToken ct)
        {
            var findings = new List<LegacyScanFinding>();

            // 1. Foreign-key orphans, discovered from the catalog so every live FK is covered.
            foreach (var fk in await GetForeignKeys(connection, ct))
            {
                ct.ThrowIfCancellationRequested();

                // Skip rows whose FK is fully NULL — that's a legitimate "no parent" link, not an orphan.
                var notNull = string.Join(" AND ", fk.ChildColumns.Select(c => $"c.{Quote(c)} IS NOT NULL"));
                var join = string.Join(" AND ",
                    fk.ParentColumns.Zip(fk.ChildColumns, (p, c) => $"p.{Quote(p)} = c.{Quote(c)}"));

                var sql =
                    $"SELECT count(*) FROM {QualifiedTable(fk.ChildTable)} c " +
                    $"WHERE {notNull} AND NOT EXISTS (" +
                    $"SELECT 1 FROM {QualifiedTable(fk.ParentTable)} p WHERE {join});";

                await using var cmd = new NpgsqlCommand(sql, connection) { CommandTimeout = 0 };
                var count = Convert.ToInt64(await cmd.ExecuteScalarAsync(ct) ?? 0L);
                if (count > 0)
                {
                    findings.Add(new LegacyScanFinding
                    {
                        Kind = "orphan",
                        Table = fk.ChildTable,
                        Reference = fk.ParentTable,
                        Detail = $"{fk.Name} ({string.Join(", ", fk.ChildColumns)} → {string.Join(", ", fk.ParentColumns)})",
                        Count = count,
                    });
                    _logger.LogWarning(
                        "Legacy import: {Count} orphan row(s) in {Child} violate FK {Constraint} -> {Parent}.",
                        count, fk.ChildTable, fk.Name, fk.ParentTable);
                }
            }

            // 2. NULL name/description on rows the legacy client dereferences. A NULL here makes the
            //    VB6 client throw run-time error 91 when the record is opened.
            var nameChecks = new (string Table, string Column)[]
            {
                ("T_Calendar_Header", "Description"),
                ("T_AccessLevel_Header", "Name"),
                ("T_TimeZone_Header", "Name"),
                ("T_SpaceZone_Header", "Name"),
                ("T_Triggers_Header", "Name"),
                ("T_Doors", "Name"),
            };
            foreach (var (table, column) in nameChecks)
            {
                ct.ThrowIfCancellationRequested();

                var columns = await GetTableColumns(connection, table, ct);
                if (columns.Count == 0 || !columns.TryGetValue(column, out var actualColumn)) continue;

                var sql = $"SELECT count(*) FROM {QualifiedTable(table)} WHERE {Quote(actualColumn)} IS NULL;";
                await using var cmd = new NpgsqlCommand(sql, connection) { CommandTimeout = 0 };
                var count = Convert.ToInt64(await cmd.ExecuteScalarAsync(ct) ?? 0L);
                if (count > 0)
                {
                    findings.Add(new LegacyScanFinding
                    {
                        Kind = "blank-name",
                        Table = table,
                        Reference = actualColumn,
                        Detail = $"{actualColumn} IS NULL",
                        Count = count,
                    });
                    _logger.LogWarning(
                        "Legacy import: {Count} row(s) in {Table} have a NULL {Column} (crashes the legacy client).",
                        count, table, actualColumn);
                }
            }

            return findings;
        }

        private async Task<long> BulkLoadTable(
            NpgsqlConnection connection, string table, string rsPath, string? connectionId,
            long totalBytes, long baseUnits, long tableBytes, CancellationToken ct)
        {
            using var recordset = AdtgRecordsetReader.Open(rsPath);
            using var dataReader = new AdtgDataReader(recordset);

            // Resolve recordset columns to the real (case-correct) destination column names and pick
            // an explicit PostgreSQL type per column so the binary COPY stream is unambiguous.
            //
            // A backup column that no longer exists in the current schema (e.g. the dropped legacy
            // "Connector" FK) is skipped rather than fatal: a legacy backup can legitimately carry
            // columns this IP-only system no longer models, and that data is irrelevant here. Only
            // the matched columns are streamed into the COPY.
            var dbColumns = await GetTableColumns(connection, table, ct);
            int sourceCount = recordset.Columns.Count;
            var sourceIndexes = new List<int>(sourceCount);
            var targetNames = new List<string>(sourceCount);
            var types = new List<NpgsqlDbType>(sourceCount);
            var skippedColumns = new List<string>();
            for (int i = 0; i < sourceCount; i++)
            {
                var col = recordset.Columns[i];
                if (!dbColumns.TryGetValue(col.Name, out var actual))
                {
                    skippedColumns.Add(col.Name);
                    continue;
                }
                sourceIndexes.Add(i);
                targetNames.Add(actual);
                types.Add(NpgsqlTypeOf(col));
            }

            if (skippedColumns.Count > 0)
                _logger.LogInformation(
                    "Legacy import: {Count} backup column(s) not present in {Table} were skipped: {Columns}.",
                    skippedColumns.Count, table, string.Join(", ", skippedColumns));

            // Every backup column was dropped from this table — nothing to load (it was already cleared).
            if (targetNames.Count == 0) return 0;

            int count = targetNames.Count;
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
                    int src = sourceIndexes[i];
                    if (dataReader.IsDBNull(src))
                        await importer.WriteNullAsync(ct);
                    else
                        await importer.WriteAsync(dataReader.GetValue(src), types[i], ct);
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
