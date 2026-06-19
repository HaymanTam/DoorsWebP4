using System.Globalization;
using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services
{
    public class TimeSheetService : ITimeSheetService
    {
        private readonly DoorsEnterpriseContext _context;
        private readonly IAuditService _audit;

        public TimeSheetService(DoorsEnterpriseContext context, IAuditService audit)
        {
            _context = context;
            _audit = audit;
        }

        public async Task<List<TimeSheet>> GetAll()
        {
            return await _context.TimeSheet.AsNoTracking().ToListAsync();
        }

        public async Task<TimeSheet?> GetById(int id)
        {
            return await _context.TimeSheet.FindAsync(id);
        }

        public async Task<List<TimeSheet>> Create(TimeSheet entity)
        {
            _context.TimeSheet.Add(entity);
            await _context.SaveChangesAsync();
            await _audit.LogAsync(AuditAction.Create, "Time Sheet", entity.Code.ToString(), entity.Name);
            return await _context.TimeSheet.AsNoTracking().ToListAsync();
        }

        public async Task<List<TimeSheet>?> Update(int id, TimeSheet entity)
        {
            var result = await _context.TimeSheet.FindAsync(id);
            if (result is null) return null;
            entity.Code = id; // keep route and body key aligned
            _context.Entry(result).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            await _audit.LogAsync(AuditAction.Update, "Time Sheet", id.ToString(), result.Name);
            return await _context.TimeSheet.AsNoTracking().ToListAsync();
        }

        public async Task<List<TimeSheet>?> Delete(int id)
        {
            var result = await _context.TimeSheet.FindAsync(id);
            if (result is null) return null;

            var zones = await _context.TimeSheetZone.Where(z => z.Code == id).ToListAsync();
            _context.TimeSheetZone.RemoveRange(zones);
            _context.TimeSheet.Remove(result);
            await _context.SaveChangesAsync();
            await _audit.LogAsync(AuditAction.Delete, "Time Sheet", id.ToString(), result.Name);
            return await _context.TimeSheet.AsNoTracking().ToListAsync();
        }

        public async Task<TimeSheetSaveDto> GetForEdit(int? code)
        {
            // Every space zone is a candidate filter; mark the ones already attached to this definition.
            var allZones = await _context.SpaceZone.AsNoTracking()
                .OrderBy(z => z.Name)
                .Select(z => new { z.ZoneNumber, z.Name })
                .ToListAsync();

            var dto = new TimeSheetSaveDto();
            var selected = new HashSet<int>();

            if (code is int id)
            {
                var header = await _context.TimeSheet.AsNoTracking().FirstOrDefaultAsync(t => t.Code == id);
                if (header is not null)
                {
                    dto.Code = header.Code;
                    dto.Name = header.Name;
                    dto.CardId = header.CardId;
                    dto.FirstName = header.FirstName;
                    dto.LastName = header.LastName;
                    dto.DateSearch = header.DateSearch;
                    dto.InLastNumber = header.InLastNumber;
                    dto.InLastPeriod = header.InLastPeriod;
                    dto.DateFrom = header.DateFrom;
                    dto.DateTo = header.DateTo;
                    dto.Rollover = header.Rollover;
                    dto.DatePageBreak = header.DatePageBreak;
                    dto.CardIdPageBreak = header.CardIdpageBreak;
                }
                selected = (await _context.TimeSheetZone.AsNoTracking()
                    .Where(z => z.Code == id)
                    .Select(z => z.Zone)
                    .ToListAsync()).ToHashSet();
            }
            else
            {
                // Sensible defaults for a new definition: single day = today, rollover at midnight.
                var today = DateTime.Today;
                dto.DateFrom = today;
                dto.DateTo = today;
                dto.Rollover = today;
            }

            dto.Zones = allZones.Select(z => new TimeSheetZoneDto
            {
                Zone = z.ZoneNumber,
                Name = z.Name,
                Included = selected.Contains(z.ZoneNumber),
            }).ToList();

            return dto;
        }

        public async Task<TimeSheet> Save(TimeSheetSaveDto dto)
        {
            TimeSheet header;
            bool isUpdate;
            int code;

            if (dto.Code is int c && await _context.TimeSheet.FindAsync(c) is { } existing)
            {
                isUpdate = true;
                code = c;
                header = existing;
                ApplyHeader(header, dto);

                // Replace the zone set.
                var old = await _context.TimeSheetZone.Where(z => z.Code == c).ToListAsync();
                _context.TimeSheetZone.RemoveRange(old);
                await _context.SaveChangesAsync();
            }
            else
            {
                isUpdate = false;
                code = await NextCode();
                header = new TimeSheet { Code = code };
                ApplyHeader(header, dto);
                BlankCustomFields(header); // NOT NULL columns; the new UI does not surface custom-field filters.
                _context.TimeSheet.Add(header);
                await _context.SaveChangesAsync();
            }

            foreach (var z in dto.Zones.Where(x => x.Included))
            {
                _context.TimeSheetZone.Add(new TimeSheetZone { Code = code, Zone = z.Zone });
            }
            await _context.SaveChangesAsync();

            await _audit.LogAsync(isUpdate ? AuditAction.Update : AuditAction.Create, "Time Sheet", code.ToString(), header.Name);
            return header;
        }

        private static void ApplyHeader(TimeSheet header, TimeSheetSaveDto dto)
        {
            header.Name = dto.Name?.Trim() ?? "";
            header.CardId = dto.CardId?.Trim() ?? "";
            header.FirstName = dto.FirstName?.Trim() ?? "";
            header.LastName = dto.LastName?.Trim() ?? "";
            header.DateSearch = dto.DateSearch;
            header.InLastNumber = dto.InLastNumber;
            header.InLastPeriod = dto.InLastPeriod;
            header.DateFrom = dto.DateFrom;
            header.DateTo = dto.DateTo;
            header.Rollover = dto.Rollover;
            header.DatePageBreak = dto.DatePageBreak;
            header.CardIdpageBreak = dto.CardIdPageBreak;
        }

        // The Custom1..Custom25 columns are NOT NULL with no default; seed them empty when creating.
        private static void BlankCustomFields(TimeSheet h)
        {
            h.Custom1 = ""; h.Custom2 = ""; h.Custom3 = ""; h.Custom4 = ""; h.Custom5 = "";
            h.Custom6 = ""; h.Custom7 = ""; h.Custom8 = ""; h.Custom9 = ""; h.Custom10 = "";
            h.Custom11 = ""; h.Custom12 = ""; h.Custom13 = ""; h.Custom14 = ""; h.Custom15 = "";
            h.Custom16 = ""; h.Custom17 = ""; h.Custom18 = ""; h.Custom19 = ""; h.Custom20 = "";
            h.Custom21 = ""; h.Custom22 = ""; h.Custom23 = ""; h.Custom24 = ""; h.Custom25 = "";
        }

        // Code is not an identity column; the legacy client allocates the next free number.
        private async Task<int> NextCode()
        {
            var max = await _context.TimeSheet.Select(t => (int?)t.Code).MaxAsync();
            return (max ?? 0) + 1;
        }

        /// <summary>
        /// Executes the saved settings now and returns the hours-worked report rows, faithfully
        /// reproducing the legacy sp_TimeSheet algorithm in memory (date-window computation,
        /// in/out transaction filtering with the reader flags, in/out pairing and unmatched-out append).
        /// </summary>
        public async Task<List<TimeSheetReportRowDto>?> RunReport(int code)
        {
            var header = await _context.TimeSheet.AsNoTracking().FirstOrDefaultAsync(t => t.Code == code);
            if (header is null) return null;

            // @Rollover is used as a time-of-day offset (legacy stores it as a datetime over base 1900-01-01).
            var rollover = header.Rollover.TimeOfDay;
            var (dateFrom, dateTo) = ComputeWindow(header, rollover);

            // The zones the report is restricted to (empty = all zones).
            var zoneFilter = (await _context.TimeSheetZone.AsNoTracking()
                .Where(z => z.Code == code)
                .Select(z => z.Zone)
                .ToListAsync()).ToHashSet();

            // Build the set of doors that count as an "in" / "out" for each reader, honouring the zone filter.
            // Using a set naturally de-duplicates a door that appears in several zones.
            var doorRows = await _context.SpaceZoneDoor.AsNoTracking()
                .Where(d => zoneFilter.Count == 0 || zoneFilter.Contains(d.Zone))
                .Select(d => new { d.Door, d.Zone, d.InReader1, d.InReader2, d.InReader3, d.OutReader1, d.OutReader2, d.OutReader3 })
                .ToListAsync();

            var inDoors = new[] { new HashSet<int>(), new HashSet<int>(), new HashSet<int>(), new HashSet<int>() };  // index = reader 1..3
            var outDoors = new[] { new HashSet<int>(), new HashSet<int>(), new HashSet<int>(), new HashSet<int>() };
            foreach (var d in doorRows)
            {
                if (d.InReader1 == true) inDoors[1].Add(d.Door);
                if (d.InReader2 == true) inDoors[2].Add(d.Door);
                if (d.InReader3 == true) inDoors[3].Add(d.Door);
                if (d.OutReader1 == true) outDoors[1].Add(d.Door);
                if (d.OutReader2 == true) outDoors[2].Add(d.Door);
                if (d.OutReader3 == true) outDoors[3].Add(d.Door);
            }

            // Candidate events: access-granted (EventType 0 or 16) on readers 1..3 inside the window.
            var events = await _context.Events.AsNoTracking()
                .Where(e => (e.EventType == 0 || e.EventType == 16)
                         && (e.ReaderId == 1 || e.ReaderId == 2 || e.ReaderId == 3)
                         && e.EventDate >= dateFrom && e.EventDate <= dateTo)
                .Select(e => new { e.CardNumber, e.DoorNumber, e.ReaderId, e.EventDate })
                .ToListAsync();

            if (events.Count == 0) return new List<TimeSheetReportRowDto>();

            // Cardholders referenced by the events (inner join in the legacy proc).
            var cardNumbers = events.Select(e => e.CardNumber).Distinct().ToList();
            var holders = await _context.Cardholder.AsNoTracking()
                .Where(c => cardNumbers.Contains(c.CardNumber))
                .ToDictionaryAsync(c => c.CardNumber);

            // Custom-field filters only matter for restored legacy definitions (the new UI leaves them blank).
            var customs = HeaderCustoms(header);
            Dictionary<int, CardholderCustomFields> customRows = new();
            if (customs.Any(v => v.Length > 0))
            {
                customRows = await _context.CardholderCustomFields.AsNoTracking()
                    .Where(c => cardNumbers.Contains(c.CardNumber))
                    .ToDictionaryAsync(c => c.CardNumber);
            }

            string cardIdFilter = header.CardId?.Trim() ?? "";
            string firstNameFilter = header.FirstName?.Trim() ?? "";
            string lastNameFilter = header.LastName?.Trim() ?? "";

            bool Passes(int cardNumber)
            {
                if (!holders.TryGetValue(cardNumber, out var c)) return false;
                if (cardIdFilter.Length > 0 && !Like(c.CardId, cardIdFilter)) return false;
                if (firstNameFilter.Length > 0 && !StartsWithCi(c.Forname, firstNameFilter)) return false;
                if (lastNameFilter.Length > 0 && !StartsWithCi(c.Surname, lastNameFilter)) return false;

                if (customRows.Count > 0)
                {
                    customRows.TryGetValue(cardNumber, out var cf);
                    var vals = cf is null ? null : CardCustoms(cf);
                    for (int i = 0; i < customs.Length; i++)
                    {
                        if (customs[i].Length == 0) continue;
                        var v = vals?[i] ?? "";
                        if (!StartsWithCi(v, customs[i])) return false;
                    }
                }
                return true;
            }

            // Internal transactions are stored rollover-adjusted (EventDate - @Rollover), so that a shift
            // crossing midnight is bucketed and paired within the same working day. Rollover is added back
            // only for display.
            var ins = new List<(int Card, DateTime T)>();
            var outs = new List<(int Card, DateTime T)>();
            foreach (var e in events)
            {
                if (!Passes(e.CardNumber)) continue;
                var t = e.EventDate - rollover;
                if (inDoors[e.ReaderId].Contains(e.DoorNumber)) ins.Add((e.CardNumber, t));
                if (outDoors[e.ReaderId].Contains(e.DoorNumber)) outs.Add((e.CardNumber, t));
            }

            var rows = new List<Pairing>();

            // Pair each "in" with the first "out" that follows it on the same working day, unless the
            // cardholder clocks in again first (then the "in" stays open).
            foreach (var inTx in ins)
            {
                DateTime? nextIn = ins
                    .Where(x => x.Card == inTx.Card && x.T > inTx.T && x.T.Date == inTx.T.Date)
                    .Select(x => (DateTime?)x.T).DefaultIfEmpty(null).Min();
                DateTime? nextOut = outs
                    .Where(x => x.Card == inTx.Card && x.T > inTx.T && x.T.Date == inTx.T.Date)
                    .Select(x => (DateTime?)x.T).DefaultIfEmpty(null).Min();

                DateTime? outDate = nextOut is not null && (nextIn is null || nextIn > nextOut) ? nextOut : null;

                rows.Add(new Pairing
                {
                    CardNumber = inTx.Card,
                    Bucket = inTx.T.Date,
                    InDate = inTx.T,
                    OutDate = outDate,
                    SortDate = inTx.T,
                });
            }

            // Append clock-outs that were never matched to an "in", as out-only rows.
            var usedOuts = rows.Where(r => r.OutDate is not null)
                               .Select(r => (r.CardNumber, r.OutDate!.Value))
                               .ToHashSet();
            foreach (var o in outs)
            {
                if (usedOuts.Contains((o.Card, o.T))) continue;
                rows.Add(new Pairing
                {
                    CardNumber = o.Card,
                    Bucket = o.T.Date,
                    InDate = null,
                    OutDate = o.T,
                    SortDate = o.T,
                });
            }

            // Output: ORDER BY Surname, transaction date.
            return rows
                .OrderBy(r => holders.TryGetValue(r.CardNumber, out var h) ? h.Surname : "", StringComparer.OrdinalIgnoreCase)
                .ThenBy(r => r.SortDate)
                .Select(r =>
                {
                    holders.TryGetValue(r.CardNumber, out var h);
                    return new TimeSheetReportRowDto
                    {
                        CardNumber = r.CardNumber,
                        EventDate = r.Bucket.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                        CardId = h?.CardId ?? "",
                        FirstName = h?.Forname ?? "",
                        LastName = h?.Surname ?? "",
                        TimeFrom = r.InDate is DateTime i ? (i + rollover).ToString("HH:mm:ss", CultureInfo.InvariantCulture) : "",
                        TimeTo = r.OutDate is DateTime o ? (o + rollover).ToString("HH:mm:ss", CultureInfo.InvariantCulture) : "",
                        HoursWorked = r.InDate is DateTime fi && r.OutDate is DateTime fo ? FormatDuration(fo - fi) : "00:00:00",
                    };
                })
                .ToList();
        }

        // Replicates the legacy DateFrom/DateTo computation for the three search modes.
        private static (DateTime From, DateTime To) ComputeWindow(TimeSheet h, TimeSpan rollover)
        {
            switch (h.DateSearch)
            {
                case 2: // Between two explicit dates, used as-is.
                    return (h.DateFrom, h.DateTo);

                case 3: // In the last N days/weeks/months, counting back from now.
                    var now = DateTime.Now;
                    var back = AddPeriod(now.Date, h.InLastPeriod, -h.InLastNumber);
                    var from3 = (back + rollover).AddDays(1);
                    return (from3, now + rollover);

                default: // 1: a single working day starting at the rollover time.
                    var from1 = h.DateFrom.Date + rollover;
                    var to1 = from1.AddDays(1).Date;
                    return (from1, to1);
            }
        }

        // Legacy DATEPART codes: 1 = day, 4 = week, 5 = month.
        private static DateTime AddPeriod(DateTime d, int period, int n) => period switch
        {
            4 => d.AddDays(7 * n),
            5 => d.AddMonths(n),
            _ => d.AddDays(n),
        };

        private static string FormatDuration(TimeSpan ts)
        {
            if (ts < TimeSpan.Zero) ts = TimeSpan.Zero;
            return $"{(int)ts.TotalHours:00}:{ts.Minutes:00}:{ts.Seconds:00}";
        }

        private static bool StartsWithCi(string? value, string prefix) =>
            (value ?? "").StartsWith(prefix, StringComparison.OrdinalIgnoreCase);

        // Minimal SQL LIKE: supports % (any run) and _ (single char); otherwise a case-insensitive equals.
        private static bool Like(string? value, string pattern)
        {
            value ??= "";
            if (!pattern.Contains('%') && !pattern.Contains('_'))
                return string.Equals(value, pattern, StringComparison.OrdinalIgnoreCase);

            var regex = "^" + System.Text.RegularExpressions.Regex.Escape(pattern)
                .Replace("%", ".*").Replace("_", ".") + "$";
            return System.Text.RegularExpressions.Regex.IsMatch(value, regex,
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        private static string[] HeaderCustoms(TimeSheet h) => new[]
        {
            h.Custom1, h.Custom2, h.Custom3, h.Custom4, h.Custom5, h.Custom6, h.Custom7, h.Custom8, h.Custom9, h.Custom10,
            h.Custom11, h.Custom12, h.Custom13, h.Custom14, h.Custom15, h.Custom16, h.Custom17, h.Custom18, h.Custom19, h.Custom20,
            h.Custom21, h.Custom22, h.Custom23, h.Custom24, h.Custom25,
        }.Select(v => (v ?? "").Trim()).ToArray();

        private static string[] CardCustoms(CardholderCustomFields c) => new[]
        {
            c.Custom1, c.Custom2, c.Custom3, c.Custom4, c.Custom5, c.Custom6, c.Custom7, c.Custom8, c.Custom9, c.Custom10,
            c.Custom11, c.Custom12, c.Custom13, c.Custom14, c.Custom15, c.Custom16, c.Custom17, c.Custom18, c.Custom19, c.Custom20,
            c.Custom21, c.Custom22, c.Custom23, c.Custom24, c.Custom25,
        }.Select(v => v ?? "").ToArray();

        private sealed class Pairing
        {
            public int CardNumber { get; set; }
            public DateTime Bucket { get; set; }
            public DateTime? InDate { get; set; }
            public DateTime? OutDate { get; set; }
            public DateTime SortDate { get; set; }
        }
    }
}
