using DoorsWeb.API.Services.Interfaces;

namespace DoorsWeb.API.Services
{
    public class ApbzoneHeaderService : IApbzoneHeaderService
    {
        private readonly DoorsEnterpriseContext _context;

        public ApbzoneHeaderService(DoorsEnterpriseContext context)
        {
            _context = context;
        }

        public async Task<List<TApbzoneHeader>> GetAll()
        {
            return await _context.TApbzoneHeader.AsNoTracking().ToListAsync();
        }

        public async Task<TApbzoneHeader?> GetById(int id)
        {
            return await _context.TApbzoneHeader.FindAsync(id);
        }

        public async Task<List<TApbzoneHeader>> Create(TApbzoneHeader entity)
        {
            _context.TApbzoneHeader.Add(entity);
            await _context.SaveChangesAsync();
            return await _context.TApbzoneHeader.AsNoTracking().ToListAsync();
        }

        public async Task<List<TApbzoneHeader>?> Update(int id, TApbzoneHeader entity)
        {
            var result = await _context.TApbzoneHeader.FindAsync(id);
            if (result is null) return null;
            entity.Apbnumber = id; // keep route and body key aligned
            _context.Entry(result).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return await _context.TApbzoneHeader.AsNoTracking().ToListAsync();
        }

        public async Task<List<TApbzoneHeader>?> Delete(int id)
        {
            var result = await _context.TApbzoneHeader.FindAsync(id);
            if (result is null) return null;
            _context.TApbzoneHeader.Remove(result);
            await _context.SaveChangesAsync();
            return await _context.TApbzoneHeader.AsNoTracking().ToListAsync();
        }
    }
}
