using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services
{
    public class CardManagerService : ICardManagerService
    {
        private readonly DoorsEnterpriseContext _context;
        private readonly IAuditService _audit;

        public CardManagerService(DoorsEnterpriseContext context, IAuditService audit)
        {
            _context = context;
            _audit = audit;
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
            await _audit.LogAsync(AuditAction.Create, "Card Manager", entity.Code.ToString(), entity.Description);
            return await _context.CardManager.AsNoTracking().ToListAsync();
        }

        public async Task<List<CardManager>?> Update(int id, CardManager entity)
        {
            var result = await _context.CardManager.FindAsync(id);
            if (result is null) return null;
            entity.Code = id; // keep route and body key aligned
            _context.Entry(result).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            await _audit.LogAsync(AuditAction.Update, "Card Manager", id.ToString(), result.Description);
            return await _context.CardManager.AsNoTracking().ToListAsync();
        }

        public async Task<List<CardManager>?> Delete(int id)
        {
            var result = await _context.CardManager.FindAsync(id);
            if (result is null) return null;
            _context.CardManager.Remove(result);
            await _context.SaveChangesAsync();
            await _audit.LogAsync(AuditAction.Delete, "Card Manager", id.ToString(), result.Description);
            return await _context.CardManager.AsNoTracking().ToListAsync();
        }
    }
}
