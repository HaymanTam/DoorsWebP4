using DoorsWeb.API.Services.Interfaces;

namespace DoorsWeb.API.Services
{
    public class AccessLevelHeaderService : IAccessLevelHeaderService
    {
        private readonly DoorsEnterpriseContext _context;

        public AccessLevelHeaderService(DoorsEnterpriseContext context)
        {
            _context = context;
        }

        public async Task<List<TAccessLevelHeader>> GetAll()
        {
            return await _context.TAccessLevelHeader.AsNoTracking().ToListAsync();
        }

        public async Task<TAccessLevelHeader?> GetById(int site, int accessLevel)
        {
            // EF composite key order is { AccessLevel, Site } — keep FindAsync args in that order.
            return await _context.TAccessLevelHeader.FindAsync(accessLevel, site);
        }

        public async Task<List<TAccessLevelHeader>> Create(TAccessLevelHeader entity)
        {
            _context.TAccessLevelHeader.Add(entity);
            await _context.SaveChangesAsync();
            return await _context.TAccessLevelHeader.AsNoTracking().ToListAsync();
        }

        public async Task<List<TAccessLevelHeader>?> Update(int site, int accessLevel, TAccessLevelHeader entity)
        {
            var result = await _context.TAccessLevelHeader.FindAsync(accessLevel, site);
            if (result is null) return null;
            entity.AccessLevel = accessLevel; // keep route and body key aligned
            entity.Site = site;
            _context.Entry(result).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return await _context.TAccessLevelHeader.AsNoTracking().ToListAsync();
        }

        public async Task<List<TAccessLevelHeader>?> Delete(int site, int accessLevel)
        {
            var result = await _context.TAccessLevelHeader.FindAsync(accessLevel, site);
            if (result is null) return null;
            _context.TAccessLevelHeader.Remove(result);
            await _context.SaveChangesAsync();
            return await _context.TAccessLevelHeader.AsNoTracking().ToListAsync();
        }
    }
}
