using System.Text;
using DoorsWeb.Shared.DTO;

namespace DoorsWeb.Client.Components
{
    /// <summary>
    /// Builds the Time Sheet "hours worked" report as RFC-4180 CSV. Shared by the Time Sheet
    /// settings modal ("Run report") and the Time Sheets list ("Run" row action) so the file
    /// layout stays identical wherever the run is triggered. Row values arrive already
    /// pre-formatted from the server, so building the file is a straight projection.
    /// </summary>
    public static class TimeSheetCsv
    {
        // Columns mirror the legacy results grid.
        public static string Build(IEnumerable<TimeSheetReportRowDto> rows)
        {
            var sb = new StringBuilder();
            sb.Append("Date,Card ID,First name,Last name,In,Out,Hours worked\r\n");
            foreach (var r in rows)
            {
                sb.Append(Field(r.EventDate)).Append(',')
                  .Append(Field(r.CardId)).Append(',')
                  .Append(Field(r.FirstName)).Append(',')
                  .Append(Field(r.LastName)).Append(',')
                  .Append(Field(r.TimeFrom)).Append(',')
                  .Append(Field(r.TimeTo)).Append(',')
                  .Append(Field(r.HoursWorked)).Append("\r\n");
            }
            return sb.ToString();
        }

        // Quotes a field only when it contains a delimiter, quote or newline (doubling embedded quotes).
        private static string Field(string? value)
        {
            value ??= "";
            return value.IndexOfAny(new[] { ',', '"', '\n', '\r' }) >= 0
                ? "\"" + value.Replace("\"", "\"\"") + "\""
                : value;
        }

        // Turns a time-sheet name into a safe file-name fragment (letters/digits kept, the rest hyphenated).
        public static string FileNameSafe(string? name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "report";
            var cleaned = new string(name.Trim().Select(c => char.IsLetterOrDigit(c) ? c : '-').ToArray()).Trim('-');
            return cleaned.Length > 0 ? cleaned : "report";
        }
    }
}
