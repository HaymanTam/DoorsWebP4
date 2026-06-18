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

        public SiteService(DoorsEnterpriseContext context)
        {
            _context = context;
        }

        public async Task<List<SiteDto>> GetAll()
        {
            return await _context.TSites
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
            var nextId = await _context.TSites.AnyAsync()
                ? await _context.TSites.MaxAsync(s => s.Site) + 1
                : 1;

            var entity = new TSites { Site = nextId, Name = name, Inuse = true };
            _context.TSites.Add(entity);
            await _context.SaveChangesAsync();

            return new SiteDto { Site = entity.Site, Name = entity.Name };
        }

        public async Task<bool> Delete(int site)
        {
            var entity = await _context.TSites.FindAsync(site);
            if (entity is null) return false;

            _context.TSites.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
