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

        public async Task<List<TCardDesignHeader>> GetAll()
        {
            return await _context.TCardDesignHeader.AsNoTracking().ToListAsync();
        }

        public async Task<TCardDesignHeader?> GetById(int id)
        {
            return await _context.TCardDesignHeader.FindAsync(id);
        }

        public async Task<List<TCardDesignHeader>> Create(TCardDesignHeader entity)
        {
            _context.TCardDesignHeader.Add(entity);
            await _context.SaveChangesAsync();
            return await _context.TCardDesignHeader.AsNoTracking().ToListAsync();
        }

        public async Task<List<TCardDesignHeader>?> Update(int id, TCardDesignHeader entity)
        {
            var result = await _context.TCardDesignHeader.FindAsync(id);
            if (result is null) return null;
            entity.Code = id; // keep route and body key aligned
            _context.Entry(result).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return await _context.TCardDesignHeader.AsNoTracking().ToListAsync();
        }

        public async Task<List<TCardDesignHeader>?> Delete(int id)
        {
            var result = await _context.TCardDesignHeader.FindAsync(id);
            if (result is null) return null;
            _context.TCardDesignHeader.Remove(result);
            await _context.SaveChangesAsync();
            return await _context.TCardDesignHeader.AsNoTracking().ToListAsync();
        }
    }
}
