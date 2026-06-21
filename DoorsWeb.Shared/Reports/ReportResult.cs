using System;
using System.Collections.Generic;

namespace DoorsWeb.Shared.Reports
{
    /// <summary>Display alignment hint for a column, honoured by the grid and the PDF renderer.</summary>
    public enum ColumnAlign
    {
        Left,
        Right,
        Center
    }

    /// <summary>One column in a <see cref="ReportResult"/>. Cells are addressed positionally by column index.</summary>
    public class ReportColumn
    {
        public string Key { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public ColumnAlign Align { get; set; } = ColumnAlign.Left;
    }

    /// <summary>
    /// The uniform tabular output every report produces. The grid, CSV serializer and PDF serializer all
    /// consume this same shape — so adding a report never touches the output code. Cells are pre-formatted
    /// strings aligned to <see cref="Columns"/> by index (server-side formatting keeps CSV/PDF/grid identical).
    /// </summary>
    public class ReportResult
    {
        public string Title { get; set; } = string.Empty;

        public DateTime GeneratedAtUtc { get; set; } = DateTime.UtcNow;

        public List<ReportColumn> Columns { get; set; } = new();

        /// <summary>Each inner list is one row, with one cell per column (same order as <see cref="Columns"/>).</summary>
        public List<List<string?>> Rows { get; set; } = new();

        /// <summary>True when a row cap was hit and the result is partial (so the UI can warn the user).</summary>
        public bool Truncated { get; set; }

        /// <summary>Human-readable echo of the parameters that were applied, for the report header/footer.</summary>
        public List<string> AppliedParameters { get; set; } = new();

        public int RowCount => Rows.Count;
    }
}
