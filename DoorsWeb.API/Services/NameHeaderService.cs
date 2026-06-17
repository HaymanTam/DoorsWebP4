using DoorsWeb.API.Services.Interfaces;

namespace DoorsWeb.API.Services
{
    public class NameHeaderService : INameHeaderService
    {
        private readonly DoorsEnterpriseContext _context;

        public NameHeaderService(DoorsEnterpriseContext context)
        {
            _context = context;
        }

        public async Task<List<TNameHeader>> GetAll()
        {
            return await _context.TNameHeader.AsNoTracking().ToListAsync();
        }

        public async Task<TNameHeader?> GetById(int id)
        {
            return await _context.TNameHeader.FindAsync(id);
        }

        public async Task<List<TNameHeader>> Create(TNameHeader entity)
        {
            _context.TNameHeader.Add(entity);
            await _context.SaveChangesAsync();
            return await _context.TNameHeader.AsNoTracking().ToListAsync();
        }

        public async Task<List<TNameHeader>?> Update(int id, TNameHeader entity)
        {
            var result = await _context.TNameHeader.FindAsync(id);
            if (result is null) return null;
            entity.CardNumber = id; // keep route and body key aligned
            _context.Entry(result).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return await _context.TNameHeader.AsNoTracking().ToListAsync();
        }

        public async Task<List<TNameHeader>?> Delete(int id)
        {
            var result = await _context.TNameHeader.FindAsync(id);
            if (result is null) return null;
            _context.TNameHeader.Remove(result);
            await _context.SaveChangesAsync();
            return await _context.TNameHeader.AsNoTracking().ToListAsync();
        }
    }
}
