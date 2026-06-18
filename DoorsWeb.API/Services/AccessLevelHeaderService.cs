using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services
{
    public class AccessLevelHeaderService : IAccessLevelHeaderService
    {
        private readonly DoorsEnterpriseContext _context;

        public AccessLevelHeaderService(DoorsEnterpriseContext context)
        {
            _context = context;
        }

        public async Task<List<TAccessLevelHeader>> GetAll()
        {
            return await _context.TAccessLevelHeader.AsNoTracking().ToListAsync();
        }

        public async Task<TAccessLevelHeader?> GetById(int site, int accessLevel)
        {
            // EF composite key order is { AccessLevel, Site } — keep FindAsync args in that order.
            return await _context.TAccessLevelHeader.FindAsync(accessLevel, site);
        }

        public async Task<List<TAccessLevelHeader>> Create(TAccessLevelHeader entity)
        {
            _context.TAccessLevelHeader.Add(entity);
            await _context.SaveChangesAsync();
            return await _context.TAccessLevelHeader.AsNoTracking().ToListAsync();
        }

        public async Task<List<TAccessLevelHeader>?> Update(int site, int accessLevel, TAccessLevelHeader entity)
        {
            var result = await _context.TAccessLevelHeader.FindAsync(accessLevel, site);
            if (result is null) return null;
            entity.AccessLevel = accessLevel; // keep route and body key aligned
            entity.Site = site;
            _context.Entry(result).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return await _context.TAccessLevelHeader.AsNoTracking().ToListAsync();
        }

        public async Task<List<TAccessLevelHeader>?> Delete(int site, int accessLevel)
        {
            var result = await _context.TAccessLevelHeader.FindAsync(accessLevel, site);
            if (result is null) return null;

            var details = await _context.TAccessLevelDetails.Where(d => d.Level == accessLevel).ToListAsync();
            _context.TAccessLevelDetails.RemoveRange(details);
            _context.TAccessLevelHeader.Remove(result);
            await _context.SaveChangesAsync();
            return await _context.TAccessLevelHeader.AsNoTracking().ToListAsync();
        }

        public async Task<AccessLevelSaveDto> GetForEdit(int site, int? accessLevel)
        {
            var doors = await _context.TDoors.AsNoTracking()
                .Where(d => d.Site == site)
                .OrderBy(d => d.Name)
                .Select(d => new { d.Door, d.Name })
                .ToListAsync();

            var dto = new AccessLevelSaveDto { Site = site };

            var selected = new Dictionary<int, TAccessLevelDetails>();
            if (accessLevel is int lvl)
            {
                var header = await _context.TAccessLevelHeader.AsNoTracking()
                    .FirstOrDefaultAsync(a => a.AccessLevel == lvl && a.Site == site);
                if (header is not null)
                {
                    dto.AccessLevel = header.AccessLevel;
                    dto.Name = header.Name;
                    dto.TimeZone = header.TimeZone;
                }
                selected = await _context.TAccessLevelDetails.AsNoTracking()
                    .Where(d => d.Level == lvl)
                    .ToDictionaryAsync(d => d.Door);
            }

            dto.Doors = doors.Select(d =>
            {
                selected.TryGetValue(d.Door, out var det);
                return new AccessLevelDoorDto
                {
                    Door = d.Door,
                    Name = d.Name,
                    Selected = det is not null,
                    DoorTimeZone = det?.DoorTimeZone,
                    LevelDefault = det?.LevelDefault ?? true,
                };
            }).ToList();

            return dto;
        }

        public async Task<TAccessLevelHeader> Save(AccessLevelSaveDto dto)
        {
            TAccessLevelHeader header;
            int level;
            if (dto.AccessLevel is int al && await _context.TAccessLevelHeader.FindAsync(al, dto.Site) is { } existing)
            {
                existing.Name = dto.Name;
                existing.TimeZone = dto.TimeZone;
                header = existing;
                level = al;

                // Replace the door set: drop the old rows, then re-insert below.
                var old = await _context.TAccessLevelDetails.Where(d => d.Level == al).ToListAsync();
                _context.TAccessLevelDetails.RemoveRange(old);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Details key on Level alone, so the number must be globally unique.
                level = await NextAccessLevel();
                header = new TAccessLevelHeader
                {
                    AccessLevel = level,
                    Site = dto.Site,
                    Name = dto.Name,
                    TimeZone = dto.TimeZone,
                    LocalLevel = await NextLocalLevel(dto.Site),
                };
                _context.TAccessLevelHeader.Add(header);
                await _context.SaveChangesAsync();
            }

            foreach (var d in dto.Doors.Where(x => x.Selected))
            {
                _context.TAccessLevelDetails.Add(new TAccessLevelDetails
                {
                    Level = level,
                    Door = d.Door,
                    DoorTimeZone = d.DoorTimeZone,
                    LevelDefault = d.LevelDefault,
                });
            }
            await _context.SaveChangesAsync();
            return header;
        }

        // Globally unique level number (details key on Level without Site).
        private async Task<int> NextAccessLevel()
        {
            var max = await _context.TAccessLevelHeader.Select(a => (int?)a.AccessLevel).MaxAsync();
            return (max ?? 0) + 1;
        }

        // Per-site 1-based display index the legacy client shows.
        private async Task<int> NextLocalLevel(int site)
        {
            var max = await _context.TAccessLevelHeader.Where(a => a.Site == site).Select(a => a.LocalLevel).MaxAsync();
            return (max ?? 0) + 1;
        }
    }
}
