using DoorsWeb.Shared.Reports;

namespace DoorsWeb.API.Reports
{
    /// <summary>
    /// One report = one configuration of the engine. A report exposes a self-describing
    /// <see cref="Descriptor"/> (key, name, category, parameters) and knows how to turn a
    /// <see cref="ReportRequest"/> into a uniform <see cref="ReportResult"/>. The controller,
    /// CSV serializer and PDF serializer all work against that result, so new reports never
    /// touch the transport/output layers.
    /// </summary>
    public interface IReport
    {
        ReportDescriptor Descriptor { get; }

        Task<ReportResult> RunAsync(ReportRequest request, CancellationToken ct = default);
    }
}
