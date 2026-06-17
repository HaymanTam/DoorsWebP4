using DoorsWeb.API.Services.Interfaces;

namespace DoorsWeb.API.Services
{
    public class TimeSheetHeaderService : ITimeSheetHeaderService
    {
        private readonly DoorsEnterpriseContext _context;

        public TimeSheetHeaderService(DoorsEnterpriseContext context)
        {
            _context = context;
        }

        public async Task<List<TTimeSheetHeader>> GetAll()
        {
            return await _context.TTimeSheetHeader.AsNoTracking().ToListAsync();
        }

        public async Task<TTimeSheetHeader?> GetById(int id)
        {
            return await _context.TTimeSheetHeader.FindAsync(id);
        }

        public async Task<List<TTimeSheetHeader>> Create(TTimeSheetHeader entity)
        {
            _context.TTimeSheetHeader.Add(entity);
            await _context.SaveChangesAsync();
            return await _context.TTimeSheetHeader.AsNoTracking().ToListAsync();
        }

        public async Task<List<TTimeSheetHeader>?> Update(int id, TTimeSheetHeader entity)
        {
            var result = await _context.TTimeSheetHeader.FindAsync(id);
            if (result is null) return null;
            entity.Code = id; // keep route and body key aligned
            _context.Entry(result).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return await _context.TTimeSheetHeader.AsNoTracking().ToListAsync();
        }

        public async Task<List<TTimeSheetHeader>?> Delete(int id)
        {
            var result = await _context.TTimeSheetHeader.FindAsync(id);
            if (result is null) return null;
            _context.TTimeSheetHeader.Remove(result);
            await _context.SaveChangesAsync();
            return await _context.TTimeSheetHeader.AsNoTracking().ToListAsync();
        }
    }
}
