using DoorsWeb.API.Services.Interfaces;

namespace DoorsWeb.API.Services
{
    public class IocontrollerHeaderService : IIocontrollerHeaderService
    {
        private readonly DoorsEnterpriseContext _context;

        public IocontrollerHeaderService(DoorsEnterpriseContext context)
        {
            _context = context;
        }

        public async Task<List<TIocontrollerHeader>> GetAll()
        {
            return await _context.TIocontrollerHeader.AsNoTracking().ToListAsync();
        }

        public async Task<TIocontrollerHeader?> GetById(int id)
        {
            return await _context.TIocontrollerHeader.FindAsync(id);
        }

        public async Task<List<TIocontrollerHeader>> Create(TIocontrollerHeader entity)
        {
            _context.TIocontrollerHeader.Add(entity);
            await _context.SaveChangesAsync();
            return await _context.TIocontrollerHeader.AsNoTracking().ToListAsync();
        }

        public async Task<List<TIocontrollerHeader>?> Update(int id, TIocontrollerHeader entity)
        {
            var result = await _context.TIocontrollerHeader.FindAsync(id);
            if (result is null) return null;
            entity.ControllerId = id; // keep route and body key aligned
            _context.Entry(result).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return await _context.TIocontrollerHeader.AsNoTracking().ToListAsync();
        }

        public async Task<List<TIocontrollerHeader>?> Delete(int id)
        {
            var result = await _context.TIocontrollerHeader.FindAsync(id);
            if (result is null) return null;
            _context.TIocontrollerHeader.Remove(result);
            await _context.SaveChangesAsync();
            return await _context.TIocontrollerHeader.AsNoTracking().ToListAsync();
        }
    }
}
