using DoorsWeb.API.Services.Interfaces;

namespace DoorsWeb.API.Services
{
    public class SpaceZoneHeaderService : ISpaceZoneHeaderService
    {
        private readonly DoorsEnterpriseContext _context;

        public SpaceZoneHeaderService(DoorsEnterpriseContext context)
        {
            _context = context;
        }

        public async Task<List<TSpaceZoneHeader>> GetAll()
        {
            return await _context.TSpaceZoneHeader.AsNoTracking().ToListAsync();
        }

        public async Task<TSpaceZoneHeader?> GetById(int id)
        {
            return await _context.TSpaceZoneHeader.FindAsync(id);
        }

        public async Task<List<TSpaceZoneHeader>> Create(TSpaceZoneHeader entity)
        {
            _context.TSpaceZoneHeader.Add(entity);
            await _context.SaveChangesAsync();
            return await _context.TSpaceZoneHeader.AsNoTracking().ToListAsync();
        }

        public async Task<List<TSpaceZoneHeader>?> Update(int id, TSpaceZoneHeader entity)
        {
            var result = await _context.TSpaceZoneHeader.FindAsync(id);
            if (result is null) return null;
            entity.ZoneNumber = id; // keep route and body key aligned
            _context.Entry(result).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return await _context.TSpaceZoneHeader.AsNoTracking().ToListAsync();
        }

        public async Task<List<TSpaceZoneHeader>?> Delete(int id)
        {
            var result = await _context.TSpaceZoneHeader.FindAsync(id);
            if (result is null) return null;
            _context.TSpaceZoneHeader.Remove(result);
            await _context.SaveChangesAsync();
            return await _context.TSpaceZoneHeader.AsNoTracking().ToListAsync();
        }
    }
}
