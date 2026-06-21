using System.Globalization;
using DoorsWeb.Shared.Reports;

namespace DoorsWeb.API.Reports
{
    /// <summary>
    /// Tier-1 "Access History" report: who went through which door, when. Reads T_Events over an
    /// optional date range, optionally narrowed to a single card, newest first. This is the engine's
    /// vertical slice — the first concrete <see cref="IReport"/> proving descriptor → run → tabular result.
    /// </summary>
    public class AccessHistoryReport : IReport
    {
        // Match EventService's cap. We fetch one extra row to detect (and flag) truncation.
        private const int MaxRows = 10000;

        private readonly DoorsEnterpriseContext _context;

        public AccessHistoryReport(DoorsEnterpriseContext context)
        {
            _context = context;
        }

        public ReportDescriptor Descriptor { get; } = new()
        {
            Key = "access-history",
            Name = "Access History",
            Category = "Access",
            Description = "Door access events over a date range, optionally for a single card. Newest first.",
            Parameters = new()
            {
                new ReportParameter
                {
                    Key = "from",
                    Label = "From",
                    Type = ReportParameterType.Date,
                    Required = false,
                    HelpText = "Start of the date range (inclusive). Leave blank for no lower bound."
                },
                new ReportParameter
                {
                    Key = "to",
                    Label = "To",
                    Type = ReportParameterType.Date,
                    Required = false,
                    HelpText = "End of the date range (inclusive)."
                },
                new ReportParameter
                {
                    Key = "cardNumber",
                    Label = "Card Number",
                    Type = ReportParameterType.Number,
                    Required = false,
                    HelpText = "Limit to a single card. Leave blank for all cards."
                }
            }
        };

        public async Task<ReportResult> RunAsync(ReportRequest request, CancellationToken ct = default)
        {
            var from = request.GetDate("from");
            var to = request.GetEndOfDay("to");
            var cardNumber = request.GetInt("cardNumber");

            var query = _context.Events.AsNoTracking().AsQueryable();
            if (from.HasValue)
                query = query.Where(e => e.EventDate >= from.Value);
            if (to.HasValue)
                query = query.Where(e => e.EventDate <= to.Value);
            if (cardNumber.HasValue)
                query = query.Where(e => e.CardNumber == cardNumber.Value);

            var rows = await query
                .OrderByDescending(e => e.EventDate)
                .Take(MaxRows + 1)
                .Select(e => new
                {
                    e.EventDate,
                    e.CardNumber,
                    CardHolder = e.CardNumberNavigation != null
                        ? ((e.CardNumberNavigation.Forname ?? "") + " " + (e.CardNumberNavigation.Surname ?? "")).Trim()
                        : null,
                    DoorName = e.DoorNavigation != null ? e.DoorNavigation.Name : null,
                    e.DoorNumber,
                    EventName = e.EventTypeNavigation != null ? e.EventTypeNavigation.Description : null
                })
                .ToListAsync(ct);

            var truncated = rows.Count > MaxRows;
            if (truncated)
                rows = rows.Take(MaxRows).ToList();

            var result = new ReportResult
            {
                Title = Descriptor.Name,
                Truncated = truncated,
                Columns = new()
                {
                    new ReportColumn { Key = "date", Label = "Date / Time" },
                    new ReportColumn { Key = "card", Label = "Card #", Align = ColumnAlign.Right },
                    new ReportColumn { Key = "holder", Label = "Cardholder" },
                    new ReportColumn { Key = "door", Label = "Door" },
                    new ReportColumn { Key = "event", Label = "Event" }
                }
            };

            foreach (var r in rows)
            {
                result.Rows.Add(new List<string?>
                {
                    r.EventDate.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                    r.CardNumber.ToString(CultureInfo.InvariantCulture),
                    string.IsNullOrWhiteSpace(r.CardHolder) ? null : r.CardHolder,
                    string.IsNullOrWhiteSpace(r.DoorName) ? $"Door {r.DoorNumber}" : r.DoorName,
                    r.EventName
                });
            }

            if (from.HasValue)
                result.AppliedParameters.Add($"From: {from.Value:yyyy-MM-dd}");
            if (to.HasValue)
                result.AppliedParameters.Add($"To: {to.Value:yyyy-MM-dd}");
            if (cardNumber.HasValue)
                result.AppliedParameters.Add($"Card: {cardNumber.Value}");

            return result;
        }
    }
}
