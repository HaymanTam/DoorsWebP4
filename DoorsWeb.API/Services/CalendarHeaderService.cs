using DoorsWeb.API.Services.Interfaces;

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
            _context.TCalendarHeader.Remove(result);
            await _context.SaveChangesAsync();
            return await _context.TCalendarHeader.AsNoTracking().ToListAsync();
        }
    }
}
