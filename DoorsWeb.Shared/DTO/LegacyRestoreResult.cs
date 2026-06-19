namespace DoorsWeb.Shared.DTO
{
    /// <summary>
    /// Outcome of restoring a legacy DoorsClient backup (a password-protected ZIP of per-table
    /// <c>.sql</c>/<c>.rs</c> pairs) into the current database.
    /// </summary>
    public class LegacyRestoreResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";

        /// <summary>Number of tables that were cleared and reloaded.</summary>
        public int TablesLoaded { get; set; }

        /// <summary>Total number of rows inserted across all tables.</summary>
        public long RowsLoaded { get; set; }

        /// <summary>Per-table row counts, in load order.</summary>
        public List<LegacyTableResult> Tables { get; set; } = new();

        /// <summary>Tables present in the backup that were skipped (no matching table in the database).</summary>
        public List<string> Skipped { get; set; } = new();

        /// <summary>
        /// Number of duplicate UDP (Lan/UDP) connectors removed during post-restore consolidation.
        /// Legacy data can hold several UDP connectors; the new IP-only system keeps a single one and
        /// repoints every door at it.
        /// </summary>
        public int UdpConnectorsMerged { get; set; }

        /// <summary>
        /// Read-only data-integrity findings from the post-restore scan. Reports rows that violate a
        /// foreign-key constraint (orphans) or have a NULL name/description that crashes the legacy
        /// client. Nothing is changed — these are surfaced for review only. Empty when the import is clean.
        /// </summary>
        public List<LegacyScanFinding> Findings { get; set; } = new();
    }

    public class LegacyTableResult
    {
        public string Table { get; set; } = "";
        public long Rows { get; set; }
    }

    /// <summary>One read-only data-integrity finding from the post-restore scan.</summary>
    public class LegacyScanFinding
    {
        /// <summary>"orphan" = rows whose FK references a missing parent; "blank-name" = NULL name/description.</summary>
        public string Kind { get; set; } = "";
        /// <summary>Table the offending rows live in.</summary>
        public string Table { get; set; } = "";
        /// <summary>For orphans, the referenced (missing) parent table; for blank-name, the NULL column.</summary>
        public string Reference { get; set; } = "";
        /// <summary>Underlying constraint or column name the finding came from.</summary>
        public string Detail { get; set; } = "";
        /// <summary>Number of offending rows.</summary>
        public long Count { get; set; }
    }
}
