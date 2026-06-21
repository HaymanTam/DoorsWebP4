using DoorsWeb.API.Authorization;
using DoorsWeb.API.Reports;
using DoorsWeb.Shared.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoorsWeb.API.Controllers
{
    // The whole reports area is gated on ReportsRead: running and exporting are the same data, just
    // different renderings, so read access is enough for all of them.
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = AreaPolicies.ReportsRead)]
    public class ReportsController : ControllerBase
    {
        private readonly IReportRegistry _registry;

        public ReportsController(IReportRegistry registry)
        {
            _registry = registry;
        }

        // Lists every available report (key, name, category, parameters) so the hub can group them
        // and build each report's parameter form generically.
        [HttpGet]
        public ActionResult<IReadOnlyList<ReportDescriptor>> List()
        {
            return Ok(_registry.Descriptors);
        }

        // Runs a report and returns the uniform tabular result for in-app grid display.
        [HttpPost("{key}/run")]
        public async Task<ActionResult<ReportResult>> Run(string key, ReportRequest request, CancellationToken ct)
        {
            var report = _registry.Find(key);
            if (report is null)
                return Problem(detail: $"Report '{key}' was not found.", title: "Not Found", statusCode: 404);

            var result = await report.RunAsync(request ?? new ReportRequest(), ct);
            return Ok(result);
        }

        // Runs a report and streams it back as a downloadable CSV or PDF (?format=csv|pdf, default csv).
        [HttpPost("{key}/export")]
        public async Task<IActionResult> Export(string key, ReportRequest request, [FromQuery] string format, CancellationToken ct)
        {
            var report = _registry.Find(key);
            if (report is null)
                return Problem(detail: $"Report '{key}' was not found.", title: "Not Found", statusCode: 404);

            var result = await report.RunAsync(request ?? new ReportRequest(), ct);
            var stamp = DateTime.Now.ToString("yyyyMMdd-HHmm");

            if (string.Equals(format, "pdf", StringComparison.OrdinalIgnoreCase))
            {
                var pdf = ReportPdfSerializer.ToPdf(result);
                return File(pdf, "application/pdf", $"{key}-{stamp}.pdf");
            }

            // Default to CSV. Prepend a UTF-8 BOM so Windows-side importers detect the encoding.
            var csv = ReportCsvSerializer.ToCsv(result);
            var preamble = System.Text.Encoding.UTF8.GetPreamble();
            var body = System.Text.Encoding.UTF8.GetBytes(csv);
            var bytes = new byte[preamble.Length + body.Length];
            Buffer.BlockCopy(preamble, 0, bytes, 0, preamble.Length);
            Buffer.BlockCopy(body, 0, bytes, preamble.Length, body.Length);

            return File(bytes, "text/csv", $"{key}-{stamp}.csv");
        }
    }
}
