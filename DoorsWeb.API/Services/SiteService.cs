using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;
using DoorsWeb.Shared.Entities;

namespace DoorsWeb.API.Services
{
    public class SiteService : ISiteService
    {
        // Legacy T_Sites.Name column length.
        private const int NameMaxLength = 30;

        private readonly DoorsEnterpriseContext _context;
        private readonly IAuditService _audit;

        public SiteService(DoorsEnterpriseContext context, IAuditService audit)
        {
            _context = context;
            _audit = audit;
        }

        public async Task<List<SiteDto>> GetAll()
        {
            return await _context.Sites
                .AsNoTracking()
                .OrderBy(s => s.Site)
                .Select(s => new SiteDto { Site = s.Site, Name = s.Name })
                .ToListAsync();
        }

        public async Task<SiteDto> Create(string name)
        {
            name = (name ?? string.Empty).Trim();
            if (name.Length == 0)
                throw new InvalidOperationException("Site name is required.");
            if (name.Length > NameMaxLength)
                name = name.Substring(0, NameMaxLength);

            // T_Sites.Site is ValueGeneratedNever, so assign the next id ourselves.
            var nextId = await _context.Sites.AnyAsync()
                ? await _context.Sites.MaxAsync(s => s.Site) + 1
                : 1;

            var entity = new Sites { Site = nextId, Name = name, Inuse = true };
            _context.Sites.Add(entity);
            await _context.SaveChangesAsync();
            await _audit.LogAsync(AuditAction.Create, "Site", entity.Site.ToString(), entity.Name);

            return new SiteDto { Site = entity.Site, Name = entity.Name };
        }

        public async Task<SiteDto?> Rename(int site, string name)
        {
            name = (name ?? string.Empty).Trim();
            if (name.Length == 0)
                throw new InvalidOperationException("Site name is required.");
            if (name.Length > NameMaxLength)
                name = name.Substring(0, NameMaxLength);

            var entity = await _context.Sites.FindAsync(site);
            if (entity is null) return null;

            entity.Name = name;
            await _context.SaveChangesAsync();
            await _audit.LogAsync(AuditAction.Update, "Site", entity.Site.ToString(), entity.Name);

            return new SiteDto { Site = entity.Site, Name = entity.Name };
        }

        public async Task<bool> Delete(int site)
        {
            var entity = await _context.Sites.FindAsync(site);
            if (entity is null) return false;

            // At least one site must always exist, so the last one can't be removed.
            if (await _context.Sites.CountAsync() <= 1)
                throw new InvalidOperationException("Cannot delete the last remaining site. At least one site must exist.");

            _context.Sites.Remove(entity);
            await _context.SaveChangesAsync();
            await _audit.LogAsync(AuditAction.Delete, "Site", entity.Site.ToString(), entity.Name);
            return true;
        }
    }
}
