using DoorsWeb.API.Services.Interfaces;

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

        public async Task<TTimeZoneHeader?> GetById(int timeZone, int site)
        {
            // Composite key order: { TimeZone, Site }
            return await _context.TTimeZoneHeader.FindAsync(timeZone, site);
        }

        public async Task<List<TTimeZoneHeader>> Create(TTimeZoneHeader entity)
        {
            _context.TTimeZoneHeader.Add(entity);
            await _context.SaveChangesAsync();
            return await _context.TTimeZoneHeader.AsNoTracking().ToListAsync();
        }

        public async Task<List<TTimeZoneHeader>?> Update(int timeZone, int site, TTimeZoneHeader entity)
        {
            var result = await _context.TTimeZoneHeader.FindAsync(timeZone, site);
            if (result is null) return null;
            entity.TimeZone = timeZone; // keep route and body key aligned
            entity.Site = site;
            _context.Entry(result).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return await _context.TTimeZoneHeader.AsNoTracking().ToListAsync();
        }

        public async Task<List<TTimeZoneHeader>?> Delete(int timeZone, int site)
        {
            var result = await _context.TTimeZoneHeader.FindAsync(timeZone, site);
            if (result is null) return null;
            _context.TTimeZoneHeader.Remove(result);
            await _context.SaveChangesAsync();
            return await _context.TTimeZoneHeader.AsNoTracking().ToListAsync();
        }
    }
}
