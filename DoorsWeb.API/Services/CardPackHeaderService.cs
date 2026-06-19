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

        public async Task<List<CardPack>> GetAll()
        {
            return await _context.CardPack.AsNoTracking().ToListAsync();
        }

        public async Task<CardPack?> GetById(int id)
        {
            return await _context.CardPack.FindAsync(id);
        }

        public async Task<List<CardPack>> Create(CardPack entity)
        {
            _context.CardPack.Add(entity);
            await _context.SaveChangesAsync();
            return await _context.CardPack.AsNoTracking().ToListAsync();
        }

        public async Task<List<CardPack>?> Update(int id, CardPack entity)
        {
            var result = await _context.CardPack.FindAsync(id);
            if (result is null) return null;
            entity.Code = id; // keep route and body key aligned
            _context.Entry(result).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return await _context.CardPack.AsNoTracking().ToListAsync();
        }

        public async Task<List<CardPack>?> Delete(int id)
        {
            var result = await _context.CardPack.FindAsync(id);
            if (result is null) return null;
            _context.CardPack.Remove(result);
            await _context.SaveChangesAsync();
            return await _context.CardPack.AsNoTracking().ToListAsync();
        }
    }
}
