using System.Collections.Generic;

namespace DoorsWeb.Shared.Reports
{
    /// <summary>How a report parameter should be rendered/edited on the client and parsed on the server.</summary>
    public enum ReportParameterType
    {
        Text,
        Number,
        Date,
        DateTime,
        Boolean,
        Select
    }

    /// <summary>One choice for a <see cref="ReportParameterType.Select"/> parameter.</summary>
    public class ReportParameterOption
    {
        public string Value { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
    }

    /// <summary>
    /// Declares a single input a report accepts. The client renders an editor from <see cref="Type"/>
    /// and posts the raw string value back; the server parses it. This is what makes the engine
    /// "one engine, many configs" — reports differ only by their descriptor and run logic.
    /// </summary>
    public class ReportParameter
    {
        public string Key { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public ReportParameterType Type { get; set; }
        public bool Required { get; set; }

        /// <summary>Optional default the client pre-fills (e.g. an ISO date). Sent as a string like every value.</summary>
        public string? DefaultValue { get; set; }

        /// <summary>Optional hint shown under the editor.</summary>
        public string? HelpText { get; set; }

        /// <summary>Choices for a <see cref="ReportParameterType.Select"/> parameter; empty otherwise.</summary>
        public List<ReportParameterOption> Options { get; set; } = new();
    }

    /// <summary>
    /// Self-describing metadata for one report: its stable key, display name, category (for the hub),
    /// and the parameters it accepts. Returned to the client so it can list/group reports and build
    /// the parameter form generically.
    /// </summary>
    public class ReportDescriptor
    {
        /// <summary>Stable url-safe identifier (e.g. "access-history"). Used in run/export routes.</summary>
        public string Key { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        /// <summary>Grouping label for the Reports hub (e.g. "Access", "Cardholders", "System").</summary>
        public string Category { get; set; } = string.Empty;

        public List<ReportParameter> Parameters { get; set; } = new();
    }
}
