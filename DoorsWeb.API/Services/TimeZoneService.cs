using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services
{
    public class TimeZoneService : ITimeZoneService
    {
        private readonly DoorsEnterpriseContext _context;
        private readonly IAuditService _audit;

        public TimeZoneService(DoorsEnterpriseContext context, IAuditService audit)
        {
            _context = context;
            _audit = audit;
        }

        public async Task<List<TimeZones>> GetAll()
        {
            return await _context.TimeZones.AsNoTracking().ToListAsync();
        }

        public async Task<TimeZones?> GetById(int site, int timeZone)
        {
            // EF composite key order is { TimeZone, Site } — keep FindAsync args in that order.
            return await _context.TimeZones.FindAsync(timeZone, site);
        }

        public async Task<List<TimeZones>> Create(TimeZones entity)
        {
            _context.TimeZones.Add(entity);
            await _context.SaveChangesAsync();
            await _audit.LogAsync(AuditAction.Create, "Time Zone", entity.TimeZone.ToString(), entity.Name);
            return await _context.TimeZones.AsNoTracking().ToListAsync();
        }

        public async Task<List<TimeZones>?> Update(int site, int timeZone, TimeZones entity)
        {
            var result = await _context.TimeZones.FindAsync(timeZone, site);
            if (result is null) return null;
            entity.TimeZone = timeZone; // keep route and body key aligned
            entity.Site = site;
            _context.Entry(result).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            await _audit.LogAsync(AuditAction.Update, "Time Zone", timeZone.ToString(), result.Name);
            return await _context.TimeZones.AsNoTracking().ToListAsync();
        }

        public async Task<List<TimeZones>?> Delete(int site, int timeZone)
        {
            var result = await _context.TimeZones.FindAsync(timeZone, site);
            if (result is null) return null;

            var details = await _context.TimeZoneInterval.Where(d => d.TimeZone == timeZone).ToListAsync();
            _context.TimeZoneInterval.RemoveRange(details);
            _context.TimeZones.Remove(result);
            await _context.SaveChangesAsync();
            await _audit.LogAsync(AuditAction.Delete, "Time Zone", timeZone.ToString(), result.Name);
            return await _context.TimeZones.AsNoTracking().ToListAsync();
        }

        public async Task<TimeZoneSaveDto?> GetWithElements(int site, int timeZone)
        {
            var header = await _context.TimeZones.AsNoTracking()
                .FirstOrDefaultAsync(t => t.TimeZone == timeZone && t.Site == site);
            if (header is null) return null;

            var details = await _context.TimeZoneInterval.AsNoTracking()
                .Where(d => d.TimeZone == timeZone)
                .OrderBy(d => d.Sequence)
                .ToListAsync();

            return new TimeZoneSaveDto
            {
                TimeZone = header.TimeZone,
                Site = header.Site,
                Name = header.Name,
                Calendar = header.Calendar,
                Elements = details.Select(d => new TimeZoneElementDto
                {
                    StartTime = d.StartTime,
                    EndTime = d.EndTime,
                    Mon = d.Mon ?? false,
                    Tue = d.Tue ?? false,
                    Wed = d.Wed ?? false,
                    Thu = d.Thu ?? false,
                    Fri = d.Fri ?? false,
                    Sat = d.Sat ?? false,
                    Sun = d.Sun ?? false,
                    Calendar = d.Calendar,
                    DefaultCalendar = d.DefaultCalendar ?? false,
                }).ToList(),
            };
        }

        public async Task<TimeZones> Save(TimeZoneSaveDto dto)
        {
            TimeZones header;
            int timeZone;
            bool isUpdate;
            if (dto.TimeZone is int tz && await _context.TimeZones.FindAsync(tz, dto.Site) is { } existing)
            {
                isUpdate = true;
                existing.Name = dto.Name;
                existing.Calendar = dto.Calendar;
                header = existing;
                timeZone = tz;

                // Replace the element set: drop the old rows, then re-insert below.
                var old = await _context.TimeZoneInterval.Where(d => d.TimeZone == tz).ToListAsync();
                _context.TimeZoneInterval.RemoveRange(old);
                await _context.SaveChangesAsync();
            }
            else
            {
                isUpdate = false;
                // Details key on TimeZone alone, so the number must be globally unique.
                timeZone = await NextTimeZone();
                header = new TimeZones
                {
                    TimeZone = timeZone,
                    Site = dto.Site,
                    Name = dto.Name,
                    Calendar = dto.Calendar,
                    LocalTimeZone = await NextLocalTimeZone(dto.Site),
                };
                _context.TimeZones.Add(header);
                await _context.SaveChangesAsync();
            }

            int sequence = 1;
            foreach (var el in dto.Elements)
            {
                _context.TimeZoneInterval.Add(new TimeZoneInterval
                {
                    TimeZone = timeZone,
                    Sequence = sequence++,
                    StartTime = el.StartTime,
                    EndTime = el.EndTime,
                    Mon = el.Mon,
                    Tue = el.Tue,
                    Wed = el.Wed,
                    Thu = el.Thu,
                    Fri = el.Fri,
                    Sat = el.Sat,
                    Sun = el.Sun,
                    Calendar = el.Calendar,
                    DefaultCalendar = el.DefaultCalendar,
                });
            }
            await _context.SaveChangesAsync();
            await _audit.LogAsync(isUpdate ? AuditAction.Update : AuditAction.Create, "Time Zone", header.TimeZone.ToString(), header.Name);
            return header;
        }

        // Globally unique time-zone number (details key on TimeZone without Site).
        private async Task<int> NextTimeZone()
        {
            var max = await _context.TimeZones.Select(t => (int?)t.TimeZone).MaxAsync();
            return (max ?? 0) + 1;
        }

        // Per-site 1-based display index the legacy client shows.
        private async Task<int> NextLocalTimeZone(int site)
        {
            var max = await _context.TimeZones.Where(t => t.Site == site).Select(t => t.LocalTimeZone).MaxAsync();
            return (max ?? 0) + 1;
        }
    }
}
