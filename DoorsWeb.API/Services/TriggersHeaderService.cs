using DoorsWeb.API.Services.Interfaces;

namespace DoorsWeb.API.Services
{
    public class TriggersHeaderService : ITriggersHeaderService
    {
        private readonly DoorsEnterpriseContext _context;

        public TriggersHeaderService(DoorsEnterpriseContext context)
        {
            _context = context;
        }

        public async Task<List<TTriggersHeader>> GetAll()
        {
            return await _context.TTriggersHeader.AsNoTracking().ToListAsync();
        }

        public async Task<TTriggersHeader?> GetById(int id)
        {
            return await _context.TTriggersHeader.FindAsync(id);
        }

        public async Task<List<TTriggersHeader>> Create(TTriggersHeader entity)
        {
            _context.TTriggersHeader.Add(entity);
            await _context.SaveChangesAsync();
            return await _context.TTriggersHeader.AsNoTracking().ToListAsync();
        }

        public async Task<List<TTriggersHeader>?> Update(int id, TTriggersHeader entity)
        {
            var result = await _context.TTriggersHeader.FindAsync(id);
            if (result is null) return null;
            entity.Code = id; // keep route and body key aligned
            _context.Entry(result).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return await _context.TTriggersHeader.AsNoTracking().ToListAsync();
        }

        public async Task<List<TTriggersHeader>?> Delete(int id)
        {
            var result = await _context.TTriggersHeader.FindAsync(id);
            if (result is null) return null;
            _context.TTriggersHeader.Remove(result);
            await _context.SaveChangesAsync();
            return await _context.TTriggersHeader.AsNoTracking().ToListAsync();
        }
    }
}
