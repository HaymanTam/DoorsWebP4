using DoorsWeb.API.Services.Interfaces;

namespace DoorsWeb.API.Services
{
    public class CardPackHeaderService : ICardPackHeaderService
    {
        private readonly DoorsEnterpriseContext _context;

        public CardPackHeaderService(DoorsEnterpriseContext context)
        {
            _context = context;
        }

        public async Task<List<TCardPackHeader>> GetAll()
        {
            return await _context.TCardPackHeader.AsNoTracking().ToListAsync();
        }

        public async Task<TCardPackHeader?> GetById(int id)
        {
            return await _context.TCardPackHeader.FindAsync(id);
        }

        public async Task<List<TCardPackHeader>> Create(TCardPackHeader entity)
        {
            _context.TCardPackHeader.Add(entity);
            await _context.SaveChangesAsync();
            return await _context.TCardPackHeader.AsNoTracking().ToListAsync();
        }

        public async Task<List<TCardPackHeader>?> Update(int id, TCardPackHeader entity)
        {
            var result = await _context.TCardPackHeader.FindAsync(id);
            if (result is null) return null;
            entity.Code = id; // keep route and body key aligned
            _context.Entry(result).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return await _context.TCardPackHeader.AsNoTracking().ToListAsync();
        }

        public async Task<List<TCardPackHeader>?> Delete(int id)
        {
            var result = await _context.TCardPackHeader.FindAsync(id);
            if (result is null) return null;
            _context.TCardPackHeader.Remove(result);
            await _context.SaveChangesAsync();
            return await _context.TCardPackHeader.AsNoTracking().ToListAsync();
        }
    }
}
