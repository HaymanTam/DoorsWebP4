using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services
{
    public class TimeSheetHeaderService : ITimeSheetHeaderService
    {
        private readonly DoorsEnterpriseContext _context;
        private readonly IAuditService _audit;

        public TimeSheetHeaderService(DoorsEnterpriseContext context, IAuditService audit)
        {
            _context = context;
            _audit = audit;
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
            await _audit.LogAsync(AuditAction.Create, "Time Sheet", entity.Code.ToString(), entity.Name);
            return await _context.TimeSheet.AsNoTracking().ToListAsync();
        }

        public async Task<List<TimeSheet>?> Update(int id, TimeSheet entity)
        {
            var result = await _context.TimeSheet.FindAsync(id);
            if (result is null) return null;
            entity.Code = id; // keep route and body key aligned
            _context.Entry(result).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            await _audit.LogAsync(AuditAction.Update, "Time Sheet", id.ToString(), result.Name);
            return await _context.TimeSheet.AsNoTracking().ToListAsync();
        }

        public async Task<List<TimeSheet>?> Delete(int id)
        {
            var result = await _context.TimeSheet.FindAsync(id);
            if (result is null) return null;
            _context.TimeSheet.Remove(result);
            await _context.SaveChangesAsync();
            await _audit.LogAsync(AuditAction.Delete, "Time Sheet", id.ToString(), result.Name);
            return await _context.TimeSheet.AsNoTracking().ToListAsync();
        }
    }
}
