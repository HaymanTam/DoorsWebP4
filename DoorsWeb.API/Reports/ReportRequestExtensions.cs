using System.Globalization;
using DoorsWeb.Shared.Reports;

namespace DoorsWeb.API.Reports
{
    /// <summary>
    /// Typed accessors over the raw string parameter bag. Reports parse their inputs through these so
    /// the parsing rules (invariant culture, lenient date handling) stay consistent across every report.
    /// </summary>
    public static class ReportRequestExtensions
    {
        public static string? GetString(this ReportRequest request, string key)
        {
            if (request.Parameters.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value))
                return value.Trim();
            return null;
        }

        public static int? GetInt(this ReportRequest request, string key)
        {
            var raw = request.GetString(key);
            if (raw is not null && int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
                return value;
            return null;
        }

        public static bool GetBool(this ReportRequest request, string key)
        {
            var raw = request.GetString(key);
            return raw is not null && (raw.Equals("true", StringComparison.OrdinalIgnoreCase) || raw == "1");
        }

        /// <summary>Parses a date parameter. Accepts ISO (yyyy-MM-dd / round-trip) and the current culture's formats.</summary>
        public static DateTime? GetDate(this ReportRequest request, string key)
        {
            var raw = request.GetString(key);
            if (raw is null) return null;

            if (DateTime.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.None, out var value) ||
                DateTime.TryParse(raw, CultureInfo.CurrentCulture, DateTimeStyles.None, out value))
                return value;

            return null;
        }

        /// <summary>A "to" date with no time means "end of that day"; nudge it to 23:59:59 so the range is inclusive.</summary>
        public static DateTime? GetEndOfDay(this ReportRequest request, string key)
        {
            var date = request.GetDate(key);
            if (date is null) return null;
            return date.Value.TimeOfDay == TimeSpan.Zero
                ? date.Value.Date.AddDays(1).AddTicks(-1)
                : date.Value;
        }
    }
}
