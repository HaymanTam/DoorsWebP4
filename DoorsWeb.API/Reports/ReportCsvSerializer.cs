using System.Text;
using DoorsWeb.Shared.Reports;

namespace DoorsWeb.API.Reports
{
    /// <summary>Serializes a <see cref="ReportResult"/> to RFC 4180 CSV (UTF-8 BOM added by the controller).</summary>
    public static class ReportCsvSerializer
    {
        public static string ToCsv(ReportResult result)
        {
            var sb = new StringBuilder();

            void AppendRow(IEnumerable<string?> cells) =>
                sb.Append(string.Join(',', cells.Select(Escape))).Append("\r\n");

            AppendRow(result.Columns.Select(c => c.Label));

            foreach (var row in result.Rows)
                AppendRow(row);

            return sb.ToString();
        }

        // RFC 4180: quote a cell (doubling inner quotes) only when it contains a comma, quote, or line break.
        private static string Escape(string? value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            if (value.IndexOfAny(new[] { ',', '"', '\n', '\r' }) < 0)
                return value;

            return $"\"{value.Replace("\"", "\"\"")}\"";
        }
    }
}
