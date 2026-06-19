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

        public async Task<List<IoController>> GetAll()
        {
            return await _context.IoController.AsNoTracking().ToListAsync();
        }

        public async Task<IoController?> GetById(int id)
        {
            return await _context.IoController.FindAsync(id);
        }

        public async Task<List<IoController>> Create(IoController entity)
        {
            _context.IoController.Add(entity);
            await _context.SaveChangesAsync();
            return await _context.IoController.AsNoTracking().ToListAsync();
        }

        public async Task<List<IoController>?> Update(int id, IoController entity)
        {
            var result = await _context.IoController.FindAsync(id);
            if (result is null) return null;
            entity.ControllerId = id; // keep route and body key aligned
            _context.Entry(result).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return await _context.IoController.AsNoTracking().ToListAsync();
        }

        public async Task<List<IoController>?> Delete(int id)
        {
            var result = await _context.IoController.FindAsync(id);
            if (result is null) return null;
            _context.IoController.Remove(result);
            await _context.SaveChangesAsync();
            return await _context.IoController.AsNoTracking().ToListAsync();
        }
    }
}
