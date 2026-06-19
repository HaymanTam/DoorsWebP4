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

        public async Task<List<CardManager>> GetAll()
        {
            return await _context.CardManager.AsNoTracking().ToListAsync();
        }

        public async Task<CardManager?> GetById(int id)
        {
            return await _context.CardManager.FindAsync(id);
        }

        public async Task<List<CardManager>> Create(CardManager entity)
        {
            _context.CardManager.Add(entity);
            await _context.SaveChangesAsync();
            return await _context.CardManager.AsNoTracking().ToListAsync();
        }

        public async Task<List<CardManager>?> Update(int id, CardManager entity)
        {
            var result = await _context.CardManager.FindAsync(id);
            if (result is null) return null;
            entity.Code = id; // keep route and body key aligned
            _context.Entry(result).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return await _context.CardManager.AsNoTracking().ToListAsync();
        }

        public async Task<List<CardManager>?> Delete(int id)
        {
            var result = await _context.CardManager.FindAsync(id);
            if (result is null) return null;
            _context.CardManager.Remove(result);
            await _context.SaveChangesAsync();
            return await _context.CardManager.AsNoTracking().ToListAsync();
        }
    }
}
