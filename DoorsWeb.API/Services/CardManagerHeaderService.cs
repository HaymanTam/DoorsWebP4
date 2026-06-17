using DoorsWeb.API.Services.Interfaces;

namespace DoorsWeb.API.Services
{
    public class CardManagerHeaderService : ICardManagerHeaderService
    {
        private readonly DoorsEnterpriseContext _context;

        public CardManagerHeaderService(DoorsEnterpriseContext context)
        {
            _context = context;
        }

        public async Task<List<TCardManagerHeader>> GetAll()
        {
            return await _context.TCardManagerHeader.AsNoTracking().ToListAsync();
        }

        public async Task<TCardManagerHeader?> GetById(int id)
        {
            return await _context.TCardManagerHeader.FindAsync(id);
        }

        public async Task<List<TCardManagerHeader>> Create(TCardManagerHeader entity)
        {
            _context.TCardManagerHeader.Add(entity);
            await _context.SaveChangesAsync();
            return await _context.TCardManagerHeader.AsNoTracking().ToListAsync();
        }

        public async Task<List<TCardManagerHeader>?> Update(int id, TCardManagerHeader entity)
        {
            var result = await _context.TCardManagerHeader.FindAsync(id);
            if (result is null) return null;
            entity.Code = id; // keep route and body key aligned
            _context.Entry(result).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return await _context.TCardManagerHeader.AsNoTracking().ToListAsync();
        }

        public async Task<List<TCardManagerHeader>?> Delete(int id)
        {
            var result = await _context.TCardManagerHeader.FindAsync(id);
            if (result is null) return null;
            _context.TCardManagerHeader.Remove(result);
            await _context.SaveChangesAsync();
            return await _context.TCardManagerHeader.AsNoTracking().ToListAsync();
        }
    }
}
