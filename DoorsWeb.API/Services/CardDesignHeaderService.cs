using DoorsWeb.API.Services.Interfaces;

namespace DoorsWeb.API.Services
{
    public class CardDesignHeaderService : ICardDesignHeaderService
    {
        private readonly DoorsEnterpriseContext _context;

        public CardDesignHeaderService(DoorsEnterpriseContext context)
        {
            _context = context;
        }

        public async Task<List<CardDesign>> GetAll()
        {
            return await _context.CardDesign.AsNoTracking().ToListAsync();
        }

        public async Task<CardDesign?> GetById(int id)
        {
            return await _context.CardDesign.FindAsync(id);
        }

        public async Task<List<CardDesign>> Create(CardDesign entity)
        {
            _context.CardDesign.Add(entity);
            await _context.SaveChangesAsync();
            return await _context.CardDesign.AsNoTracking().ToListAsync();
        }

        public async Task<List<CardDesign>?> Update(int id, CardDesign entity)
        {
            var result = await _context.CardDesign.FindAsync(id);
            if (result is null) return null;
            entity.Code = id; // keep route and body key aligned
            _context.Entry(result).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return await _context.CardDesign.AsNoTracking().ToListAsync();
        }

        public async Task<List<CardDesign>?> Delete(int id)
        {
            var result = await _context.CardDesign.FindAsync(id);
            if (result is null) return null;
            _context.CardDesign.Remove(result);
            await _context.SaveChangesAsync();
            return await _context.CardDesign.AsNoTracking().ToListAsync();
        }
    }
}
