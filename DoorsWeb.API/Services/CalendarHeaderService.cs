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

        public async Task<List<Calendar>> GetAll()
        {
            return await _context.Calendar.AsNoTracking().ToListAsync();
        }

        public async Task<Calendar?> GetById(int id)
        {
            return await _context.Calendar.FindAsync(id);
        }

        public async Task<List<Calendar>> Create(Calendar entity)
        {
            _context.Calendar.Add(entity);
            await _context.SaveChangesAsync();
            return await _context.Calendar.AsNoTracking().ToListAsync();
        }

        public async Task<List<Calendar>?> Update(int id, Calendar entity)
        {
            var result = await _context.Calendar.FindAsync(id);
            if (result is null) return null;
            entity.Code = id; // keep route and body key aligned
            _context.Entry(result).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return await _context.Calendar.AsNoTracking().ToListAsync();
        }

        public async Task<List<Calendar>?> Delete(int id)
        {
            var result = await _context.Calendar.FindAsync(id);
            if (result is null) return null;

            var details = await _context.CalendarException.Where(d => d.Code == id).ToListAsync();
            _context.CalendarException.RemoveRange(details);
            _context.Calendar.Remove(result);
            await _context.SaveChangesAsync();
            return await _context.Calendar.AsNoTracking().ToListAsync();
        }

        public async Task<CalendarSaveDto?> GetWithHolidays(int id)
        {
            var header = await _context.Calendar.AsNoTracking().FirstOrDefaultAsync(c => c.Code == id);
            if (header is null) return null;

            var holidays = await _context.CalendarException.AsNoTracking()
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

        public async Task<Calendar> Save(CalendarSaveDto dto)
        {
            Calendar header;
            if (dto.Code is int code && await _context.Calendar.FindAsync(code) is { } existing)
            {
                existing.Description = dto.Description;
                header = existing;

                // Replace the holiday set: drop the old rows, then re-insert below.
                var old = await _context.CalendarException.Where(d => d.Code == code).ToListAsync();
                _context.CalendarException.RemoveRange(old);
                await _context.SaveChangesAsync();
            }
            else
            {
                header = new Calendar
                {
                    Site = dto.Site,
                    Description = dto.Description,
                    LocalNumber = await NextLocalNumber(dto.Site),
                };
                _context.Calendar.Add(header);
                await _context.SaveChangesAsync(); // assigns the identity Code
            }

            foreach (var date in dto.Holidays.Select(d => d.Date).Distinct())
            {
                _context.CalendarException.Add(new CalendarException { Code = header.Code, ExceptionDate = date });
            }
            await _context.SaveChangesAsync();
            return header;
        }

        // Per-site 1-based index legacy callers expect alongside the global identity Code.
        private async Task<int> NextLocalNumber(int site)
        {
            var max = await _context.Calendar
                .Where(c => c.Site == site)
                .Select(c => (int?)c.LocalNumber)
                .MaxAsync();
            return (max ?? 0) + 1;
        }
    }
}
