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
    }

    public class LegacyTableResult
    {
        public string Table { get; set; } = "";
        public long Rows { get; set; }
    }
}
