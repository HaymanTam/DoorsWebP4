using System.Globalization;
using System.Text;
using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services
{
    public class CardholderService : ICardholderService
    {
        private readonly DoorsEnterpriseContext _context;
        private readonly IAuditService _audit;
        private readonly ILicenseService _license;
        private readonly ICardPhotoService _cardPhoto;

        public CardholderService(DoorsEnterpriseContext context, IAuditService audit, ILicenseService license, ICardPhotoService cardPhoto)
        {
            _context = context;
            _audit = audit;
            _license = license;
            _cardPhoto = cardPhoto;
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

        // Column order/headers chosen to map cleanly onto CardPresso fields. The Photo column holds the
        // bare file name (e.g. "1024.jpg"); point CardPresso's image-from-database mapping at the
        // card-photo folder and it resolves the picture by that name. CRLF line endings and a UTF-8 BOM
        // (added by the controller) keep accented names intact for Windows-side importers.
        public async Task<string> BuildCardPressoCsvAsync()
        {
            // One directory scan up front instead of a filesystem lookup per card.
            var photos = _cardPhoto.GetAllFileNames();

            var sb = new StringBuilder();
            void AppendRow(params string?[] cells) =>
                sb.Append(string.Join(',', cells.Select(EscapeCsv))).Append("\r\n");

            AppendRow(
                "Card Number", "System Id", "First Name", "Last Name", "Full Name",
                "Access Levels", "Enabled", "Void", "Valid From", "Valid To", "Photo");

            await foreach (var c in GetAllCards())
            {
                photos.TryGetValue(c.CardNumber, out var photoFile);

                AppendRow(
                    c.CardId,
                    c.CardNumber.ToString(CultureInfo.InvariantCulture),
                    c.Forname,
                    c.Surname,
                    $"{c.Forname} {c.Surname}".Trim(),
                    string.Join("; ", c.AccessLevels),
                    c.Enabled == true ? "Yes" : "No",
                    c.Void == true ? "Yes" : "No",
                    c.ValidFrom?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    c.ValidTo?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    photoFile);
            }

            return sb.ToString();
        }

        // RFC 4180 escaping: wrap in double quotes (doubling any inner quote) only when the value
        // contains a comma, quote, or line break.
        private static string EscapeCsv(string? value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            if (value.IndexOfAny(new[] { ',', '"', '\n', '\r' }) < 0)
                return value;

            return $"\"{value.Replace("\"", "\"\"")}\"";
        }

        public async Task<Cardholder?> GetById(int id)
        {
            return await _context.Cardholder.FindAsync(id);
        }

        public async Task<List<Cardholder>> Create(Cardholder entity)
        {
            // License gate: refuse to add a cardholder once the licensed (or unlicensed default) limit is hit.
            _license.EnforceCardLimit(await _context.Cardholder.CountAsync());

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
