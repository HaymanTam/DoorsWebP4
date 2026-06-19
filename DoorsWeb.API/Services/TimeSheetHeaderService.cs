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

        public async Task<List<TimeSheet>> GetAll()
        {
            return await _context.TimeSheet.AsNoTracking().ToListAsync();
        }

        public async Task<TimeSheet?> GetById(int id)
        {
            return await _context.TimeSheet.FindAsync(id);
        }

        public async Task<List<TimeSheet>> Create(TimeSheet entity)
        {
            _context.TimeSheet.Add(entity);
            await _context.SaveChangesAsync();
            return await _context.TimeSheet.AsNoTracking().ToListAsync();
        }

        public async Task<List<TimeSheet>?> Update(int id, TimeSheet entity)
        {
            var result = await _context.TimeSheet.FindAsync(id);
            if (result is null) return null;
            entity.Code = id; // keep route and body key aligned
            _context.Entry(result).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return await _context.TimeSheet.AsNoTracking().ToListAsync();
        }

        public async Task<List<TimeSheet>?> Delete(int id)
        {
            var result = await _context.TimeSheet.FindAsync(id);
            if (result is null) return null;
            _context.TimeSheet.Remove(result);
            await _context.SaveChangesAsync();
            return await _context.TimeSheet.AsNoTracking().ToListAsync();
        }
    }
}
