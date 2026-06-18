using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services
{
    public class CalendarHeaderService : ICalendarHeaderService
    {
        private readonly DoorsEnterpriseContext _context;

        public CalendarHeaderService(DoorsEnterpriseContext context)
        {
            _context = context;
        }

        public async Task<List<TCalendarHeader>> GetAll()
        {
            return await _context.TCalendarHeader.AsNoTracking().ToListAsync();
        }

        public async Task<TCalendarHeader?> GetById(int id)
        {
            return await _context.TCalendarHeader.FindAsync(id);
        }

        public async Task<List<TCalendarHeader>> Create(TCalendarHeader entity)
        {
            _context.TCalendarHeader.Add(entity);
            await _context.SaveChangesAsync();
            return await _context.TCalendarHeader.AsNoTracking().ToListAsync();
        }

        public async Task<List<TCalendarHeader>?> Update(int id, TCalendarHeader entity)
        {
            var result = await _context.TCalendarHeader.FindAsync(id);
            if (result is null) return null;
            entity.Code = id; // keep route and body key aligned
            _context.Entry(result).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return await _context.TCalendarHeader.AsNoTracking().ToListAsync();
        }

        public async Task<List<TCalendarHeader>?> Delete(int id)
        {
            var result = await _context.TCalendarHeader.FindAsync(id);
            if (result is null) return null;

            var details = await _context.TCalendarDetails.Where(d => d.Code == id).ToListAsync();
            _context.TCalendarDetails.RemoveRange(details);
            _context.TCalendarHeader.Remove(result);
            await _context.SaveChangesAsync();
            return await _context.TCalendarHeader.AsNoTracking().ToListAsync();
        }

        public async Task<CalendarSaveDto?> GetWithHolidays(int id)
        {
            var header = await _context.TCalendarHeader.AsNoTracking().FirstOrDefaultAsync(c => c.Code == id);
            if (header is null) return null;

            var holidays = await _context.TCalendarDetails.AsNoTracking()
                .Where(d => d.Code == id)
                .Select(d => d.ExceptionDate)
                .ToListAsync();

            return new CalendarSaveDto
            {
                Code = header.Code,
                Site = header.Site,
                Description = header.Description,
                Holidays = holidays,
            };
        }

        public async Task<TCalendarHeader> Save(CalendarSaveDto dto)
        {
            TCalendarHeader header;
            if (dto.Code is int code && await _context.TCalendarHeader.FindAsync(code) is { } existing)
            {
                existing.Description = dto.Description;
                header = existing;

                // Replace the holiday set: drop the old rows, then re-insert below.
                var old = await _context.TCalendarDetails.Where(d => d.Code == code).ToListAsync();
                _context.TCalendarDetails.RemoveRange(old);
                await _context.SaveChangesAsync();
            }
            else
            {
                header = new TCalendarHeader
                {
                    Site = dto.Site,
                    Description = dto.Description,
                    LocalNumber = await NextLocalNumber(dto.Site),
                };
                _context.TCalendarHeader.Add(header);
                await _context.SaveChangesAsync(); // assigns the identity Code
            }

            foreach (var date in dto.Holidays.Select(d => d.Date).Distinct())
            {
                _context.TCalendarDetails.Add(new TCalendarDetails { Code = header.Code, ExceptionDate = date });
            }
            await _context.SaveChangesAsync();
            return header;
        }

        // Per-site 1-based index legacy callers expect alongside the global identity Code.
        private async Task<int> NextLocalNumber(int site)
        {
            var max = await _context.TCalendarHeader
                .Where(c => c.Site == site)
                .Select(c => (int?)c.LocalNumber)
                .MaxAsync();
            return (max ?? 0) + 1;
        }
    }
}
