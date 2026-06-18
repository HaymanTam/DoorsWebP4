using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services
{
    public class TimeZoneHeaderService : ITimeZoneHeaderService
    {
        private readonly DoorsEnterpriseContext _context;

        public TimeZoneHeaderService(DoorsEnterpriseContext context)
        {
            _context = context;
        }

        public async Task<List<TTimeZoneHeader>> GetAll()
        {
            return await _context.TTimeZoneHeader.AsNoTracking().ToListAsync();
        }

        public async Task<TTimeZoneHeader?> GetById(int site, int timeZone)
        {
            // EF composite key order is { TimeZone, Site } — keep FindAsync args in that order.
            return await _context.TTimeZoneHeader.FindAsync(timeZone, site);
        }

        public async Task<List<TTimeZoneHeader>> Create(TTimeZoneHeader entity)
        {
            _context.TTimeZoneHeader.Add(entity);
            await _context.SaveChangesAsync();
            return await _context.TTimeZoneHeader.AsNoTracking().ToListAsync();
        }

        public async Task<List<TTimeZoneHeader>?> Update(int site, int timeZone, TTimeZoneHeader entity)
        {
            var result = await _context.TTimeZoneHeader.FindAsync(timeZone, site);
            if (result is null) return null;
            entity.TimeZone = timeZone; // keep route and body key aligned
            entity.Site = site;
            _context.Entry(result).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return await _context.TTimeZoneHeader.AsNoTracking().ToListAsync();
        }

        public async Task<List<TTimeZoneHeader>?> Delete(int site, int timeZone)
        {
            var result = await _context.TTimeZoneHeader.FindAsync(timeZone, site);
            if (result is null) return null;

            var details = await _context.TTimeZoneDetails.Where(d => d.TimeZone == timeZone).ToListAsync();
            _context.TTimeZoneDetails.RemoveRange(details);
            _context.TTimeZoneHeader.Remove(result);
            await _context.SaveChangesAsync();
            return await _context.TTimeZoneHeader.AsNoTracking().ToListAsync();
        }

        public async Task<TimeZoneSaveDto?> GetWithElements(int site, int timeZone)
        {
            var header = await _context.TTimeZoneHeader.AsNoTracking()
                .FirstOrDefaultAsync(t => t.TimeZone == timeZone && t.Site == site);
            if (header is null) return null;

            var details = await _context.TTimeZoneDetails.AsNoTracking()
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

        public async Task<TTimeZoneHeader> Save(TimeZoneSaveDto dto)
        {
            TTimeZoneHeader header;
            int timeZone;
            if (dto.TimeZone is int tz && await _context.TTimeZoneHeader.FindAsync(tz, dto.Site) is { } existing)
            {
                existing.Name = dto.Name;
                existing.Calendar = dto.Calendar;
                header = existing;
                timeZone = tz;

                // Replace the element set: drop the old rows, then re-insert below.
                var old = await _context.TTimeZoneDetails.Where(d => d.TimeZone == tz).ToListAsync();
                _context.TTimeZoneDetails.RemoveRange(old);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Details key on TimeZone alone, so the number must be globally unique.
                timeZone = await NextTimeZone();
                header = new TTimeZoneHeader
                {
                    TimeZone = timeZone,
                    Site = dto.Site,
                    Name = dto.Name,
                    Calendar = dto.Calendar,
                    LocalTimeZone = await NextLocalTimeZone(dto.Site),
                };
                _context.TTimeZoneHeader.Add(header);
                await _context.SaveChangesAsync();
            }

            int sequence = 1;
            foreach (var el in dto.Elements)
            {
                _context.TTimeZoneDetails.Add(new TTimeZoneDetails
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
            return header;
        }

        // Globally unique time-zone number (details key on TimeZone without Site).
        private async Task<int> NextTimeZone()
        {
            var max = await _context.TTimeZoneHeader.Select(t => (int?)t.TimeZone).MaxAsync();
            return (max ?? 0) + 1;
        }

        // Per-site 1-based display index the legacy client shows.
        private async Task<int> NextLocalTimeZone(int site)
        {
            var max = await _context.TTimeZoneHeader.Where(t => t.Site == site).Select(t => t.LocalTimeZone).MaxAsync();
            return (max ?? 0) + 1;
        }
    }
}
