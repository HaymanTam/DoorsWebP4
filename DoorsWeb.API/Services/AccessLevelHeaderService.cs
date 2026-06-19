using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services
{
    public class AccessLevelHeaderService : IAccessLevelHeaderService
    {
        private readonly DoorsEnterpriseContext _context;
        private readonly IAuditService _audit;

        public AccessLevelHeaderService(DoorsEnterpriseContext context, IAuditService audit)
        {
            _context = context;
            _audit = audit;
        }

        public async Task<List<AccessLevels>> GetAll()
        {
            return await _context.AccessLevels.AsNoTracking().ToListAsync();
        }

        public async Task<AccessLevels?> GetById(int site, int accessLevel)
        {
            // EF composite key order is { AccessLevel, Site } — keep FindAsync args in that order.
            return await _context.AccessLevels.FindAsync(accessLevel, site);
        }

        public async Task<List<AccessLevels>> Create(AccessLevels entity)
        {
            _context.AccessLevels.Add(entity);
            await _context.SaveChangesAsync();
            await _audit.LogAsync(AuditAction.Create, "Access Level", entity.AccessLevel.ToString(), entity.Name);
            return await _context.AccessLevels.AsNoTracking().ToListAsync();
        }

        public async Task<List<AccessLevels>?> Update(int site, int accessLevel, AccessLevels entity)
        {
            var result = await _context.AccessLevels.FindAsync(accessLevel, site);
            if (result is null) return null;
            entity.AccessLevel = accessLevel; // keep route and body key aligned
            entity.Site = site;
            _context.Entry(result).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            await _audit.LogAsync(AuditAction.Update, "Access Level", accessLevel.ToString(), result.Name);
            return await _context.AccessLevels.AsNoTracking().ToListAsync();
        }

        public async Task<List<AccessLevels>?> Delete(int site, int accessLevel)
        {
            var result = await _context.AccessLevels.FindAsync(accessLevel, site);
            if (result is null) return null;

            var details = await _context.AccessLevelDoor.Where(d => d.Level == accessLevel).ToListAsync();
            _context.AccessLevelDoor.RemoveRange(details);
            _context.AccessLevels.Remove(result);
            await _context.SaveChangesAsync();
            await _audit.LogAsync(AuditAction.Delete, "Access Level", accessLevel.ToString(), result.Name);
            return await _context.AccessLevels.AsNoTracking().ToListAsync();
        }

        public async Task<AccessLevelSaveDto> GetForEdit(int site, int? accessLevel)
        {
            var doors = await _context.Doors.AsNoTracking()
                .Where(d => d.Site == site)
                .OrderBy(d => d.Name)
                .Select(d => new { d.Door, d.Name })
                .ToListAsync();

            var dto = new AccessLevelSaveDto { Site = site };

            var selected = new Dictionary<int, AccessLevelDoor>();
            if (accessLevel is int lvl)
            {
                var header = await _context.AccessLevels.AsNoTracking()
                    .FirstOrDefaultAsync(a => a.AccessLevel == lvl && a.Site == site);
                if (header is not null)
                {
                    dto.AccessLevel = header.AccessLevel;
                    dto.Name = header.Name;
                    dto.TimeZone = header.TimeZone;
                }
                selected = await _context.AccessLevelDoor.AsNoTracking()
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

        public async Task<AccessLevels> Save(AccessLevelSaveDto dto)
        {
            AccessLevels header;
            int level;
            bool isUpdate;
            if (dto.AccessLevel is int al && await _context.AccessLevels.FindAsync(al, dto.Site) is { } existing)
            {
                isUpdate = true;
                existing.Name = dto.Name;
                existing.TimeZone = dto.TimeZone;
                header = existing;
                level = al;

                // Replace the door set: drop the old rows, then re-insert below.
                var old = await _context.AccessLevelDoor.Where(d => d.Level == al).ToListAsync();
                _context.AccessLevelDoor.RemoveRange(old);
                await _context.SaveChangesAsync();
            }
            else
            {
                isUpdate = false;
                // Details key on Level alone, so the number must be globally unique.
                level = await NextAccessLevel();
                header = new AccessLevels
                {
                    AccessLevel = level,
                    Site = dto.Site,
                    Name = dto.Name,
                    TimeZone = dto.TimeZone,
                    LocalLevel = await NextLocalLevel(dto.Site),
                };
                _context.AccessLevels.Add(header);
                await _context.SaveChangesAsync();
            }

            foreach (var d in dto.Doors.Where(x => x.Selected))
            {
                _context.AccessLevelDoor.Add(new AccessLevelDoor
                {
                    Level = level,
                    Door = d.Door,
                    DoorTimeZone = d.DoorTimeZone,
                    LevelDefault = d.LevelDefault,
                });
            }
            await _context.SaveChangesAsync();
            await _audit.LogAsync(isUpdate ? AuditAction.Update : AuditAction.Create, "Access Level", header.AccessLevel.ToString(), header.Name);
            return header;
        }

        // Globally unique level number (details key on Level without Site).
        private async Task<int> NextAccessLevel()
        {
            var max = await _context.AccessLevels.Select(a => (int?)a.AccessLevel).MaxAsync();
            return (max ?? 0) + 1;
        }

        // Per-site 1-based display index the legacy client shows.
        private async Task<int> NextLocalLevel(int site)
        {
            var max = await _context.AccessLevels.Where(a => a.Site == site).Select(a => a.LocalLevel).MaxAsync();
            return (max ?? 0) + 1;
        }
    }
}
