using DoorsWeb.Shared.Reports;

namespace DoorsWeb.API.Reports
{
    /// <summary>Lookup over the registered <see cref="IReport"/>s by their descriptor key.</summary>
    public interface IReportRegistry
    {
        /// <summary>Descriptors of every registered report (for the hub listing).</summary>
        IReadOnlyList<ReportDescriptor> Descriptors { get; }

        /// <summary>The report with this key, or null if none is registered.</summary>
        IReport? Find(string key);
    }

    /// <summary>
    /// Collects every <see cref="IReport"/> the DI container resolves and indexes them by key. Adding a
    /// report is just registering a new IReport implementation — the registry, controller and hub pick it
    /// up automatically.
    /// </summary>
    public class ReportRegistry : IReportRegistry
    {
        private readonly Dictionary<string, IReport> _byKey;

        public ReportRegistry(IEnumerable<IReport> reports)
        {
            _byKey = reports.ToDictionary(r => r.Descriptor.Key, StringComparer.OrdinalIgnoreCase);
        }

        public IReadOnlyList<ReportDescriptor> Descriptors =>
            _byKey.Values.Select(r => r.Descriptor).OrderBy(d => d.Category).ThenBy(d => d.Name).ToList();

        public IReport? Find(string key) =>
            !string.IsNullOrWhiteSpace(key) && _byKey.TryGetValue(key, out var report) ? report : null;
    }
}
