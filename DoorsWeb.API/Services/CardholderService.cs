using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services
{
    public class CardholderService : ICardholderService
    {
        private readonly DoorsEnterpriseContext _context;
        private readonly IAuditService _audit;

        public CardholderService(DoorsEnterpriseContext context, IAuditService audit)
        {
            _context = context;
            _audit = audit;
        }

        public async Task<List<Cardholder>> GetAll()
        {
            return await _context.Cardholder.AsNoTracking().ToListAsync();
        }

        // Streams cards (newest names first) projected to CardDto. Each card's access-level
        // names come from the T_Name_AccessLevels junction joined to T_AccessLevel_Header.
        public IAsyncEnumerable<CardDto> GetAllCards()
        {
            return _context.Cardholder.AsNoTracking()
                // Legacy "blank" cards (card-pack placeholders / unissued) have a CardId but no
                // name; keep them after the real cardholders rather than sorting NULLs to the top.
                .OrderBy(c => string.IsNullOrEmpty(c.Surname) && string.IsNullOrEmpty(c.Forname))
                // Then most-recently-updated first (NULL Modified sorts last under descending).
                .ThenByDescending(c => c.Modified)
                .Select(c => new CardDto
                {
                    CardNumber = c.CardNumber,
                    CardId = c.CardId,
                    Surname = c.Surname,
                    Forname = c.Forname,
                    Enabled = c.Enabled,
                    Void = c.Void,
                    ValidFrom = c.ValidFrom,
                    ValidTo = c.ValidTo,
                    AccessLevels = c.CardholderAccessLevels
                        .Where(nal => nal.AccessLevel != null && nal.AccessLevel.Name != null)
                        .Select(nal => nal.AccessLevel!.Name!)
                        .OrderBy(name => name)
                        .ToList()
                })
                .AsAsyncEnumerable();
        }

        public async Task<Cardholder?> GetById(int id)
        {
            return await _context.Cardholder.FindAsync(id);
        }

        public async Task<List<Cardholder>> Create(Cardholder entity)
        {
            _context.Cardholder.Add(entity);
            await _context.SaveChangesAsync();
            await _audit.LogAsync(AuditAction.Create, "Cardholder", entity.CardId, FullName(entity));
            return await _context.Cardholder.AsNoTracking().ToListAsync();
        }

        public async Task<List<Cardholder>?> Update(int id, Cardholder entity)
        {
            var result = await _context.Cardholder.FindAsync(id);
            if (result is null) return null;
            entity.CardNumber = id; // keep route and body key aligned
            _context.Entry(result).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            await _audit.LogAsync(AuditAction.Update, "Cardholder", result.CardId, FullName(result));
            return await _context.Cardholder.AsNoTracking().ToListAsync();
        }

        public async Task<List<Cardholder>?> Delete(int id)
        {
            var result = await _context.Cardholder.FindAsync(id);
            if (result is null) return null;
            _context.Cardholder.Remove(result);
            await _context.SaveChangesAsync();
            await _audit.LogAsync(AuditAction.Delete, "Cardholder", result.CardId, FullName(result));
            return await _context.Cardholder.AsNoTracking().ToListAsync();
        }

        private static string FullName(Cardholder c) => $"{c.Forname} {c.Surname}".Trim();
    }
}
