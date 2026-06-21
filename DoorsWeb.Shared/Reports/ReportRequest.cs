using System.Collections.Generic;

namespace DoorsWeb.Shared.Reports
{
    /// <summary>
    /// A request to run/export a report: the raw parameter values keyed by <see cref="ReportParameter.Key"/>.
    /// Values are always strings (the server parses them per the parameter's declared type) so the engine
    /// stays uniform across every report.
    /// </summary>
    public class ReportRequest
    {
        public Dictionary<string, string?> Parameters { get; set; } = new();
    }
}
