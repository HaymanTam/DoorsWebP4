using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services
{
    public class CardPackService : ICardPackService
    {
        private readonly DoorsEnterpriseContext _context;
        private readonly IAuditService _audit;

        public CardPackService(DoorsEnterpriseContext context, IAuditService audit)
        {
            _context = context;
            _audit = audit;
        }

        public async Task<List<CardPack>> GetAll()
        {
            return await _context.CardPack.AsNoTracking().ToListAsync();
        }

        public async Task<CardPack?> GetById(int id)
        {
            return await _context.CardPack.FindAsync(id);
        }

        public async Task<List<CardPack>> Create(CardPack entity)
        {
            _context.CardPack.Add(entity);
            await _context.SaveChangesAsync();
            await _audit.LogAsync(AuditAction.Create, "Card Pack", entity.Code.ToString(), entity.Name);
            return await _context.CardPack.AsNoTracking().ToListAsync();
        }

        public async Task<List<CardPack>?> Update(int id, CardPack entity)
        {
            var result = await _context.CardPack.FindAsync(id);
            if (result is null) return null;
            entity.Code = id; // keep route and body key aligned
            _context.Entry(result).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            await _audit.LogAsync(AuditAction.Update, "Card Pack", id.ToString(), result.Name);
            return await _context.CardPack.AsNoTracking().ToListAsync();
        }

        public async Task<List<CardPack>?> Delete(int id)
        {
            var result = await _context.CardPack.FindAsync(id);
            if (result is null) return null;
            _context.CardPack.Remove(result);
            await _context.SaveChangesAsync();
            await _audit.LogAsync(AuditAction.Delete, "Card Pack", id.ToString(), result.Name);
            return await _context.CardPack.AsNoTracking().ToListAsync();
        }
    }
}
