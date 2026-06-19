using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services
{
    public class SpaceZoneHeaderService : ISpaceZoneHeaderService
    {
        private readonly DoorsEnterpriseContext _context;

        public SpaceZoneHeaderService(DoorsEnterpriseContext context)
        {
            _context = context;
        }

        public async Task<List<SpaceZone>> GetAll()
        {
            return await _context.SpaceZone.AsNoTracking().ToListAsync();
        }

        public async Task<SpaceZone?> GetById(int id)
        {
            return await _context.SpaceZone.FindAsync(id);
        }

        public async Task<List<SpaceZone>> Create(SpaceZone entity)
        {
            _context.SpaceZone.Add(entity);
            await _context.SaveChangesAsync();
            return await _context.SpaceZone.AsNoTracking().ToListAsync();
        }

        public async Task<List<SpaceZone>?> Update(int id, SpaceZone entity)
        {
            var result = await _context.SpaceZone.FindAsync(id);
            if (result is null) return null;
            entity.ZoneNumber = id; // keep route and body key aligned
            _context.Entry(result).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return await _context.SpaceZone.AsNoTracking().ToListAsync();
        }

        public async Task<List<SpaceZone>?> Delete(int id)
        {
            var result = await _context.SpaceZone.FindAsync(id);
            if (result is null) return null;

            var details = await _context.SpaceZoneDoor.Where(d => d.Zone == id).ToListAsync();
            _context.SpaceZoneDoor.RemoveRange(details);
            _context.SpaceZone.Remove(result);
            await _context.SaveChangesAsync();
            return await _context.SpaceZone.AsNoTracking().ToListAsync();
        }

        public async Task<SpaceZoneSaveDto> GetForEdit(int site, int? zoneNumber)
        {
            var doors = await _context.Doors.AsNoTracking()
                .Where(d => d.Site == site)
                .OrderBy(d => d.Name)
                .Select(d => new { d.Door, d.Name })
                .ToListAsync();

            var dto = new SpaceZoneSaveDto { Site = site };

            var selected = new Dictionary<int, SpaceZoneDoor>();
            if (zoneNumber is int zn)
            {
                var header = await _context.SpaceZone.AsNoTracking()
                    .FirstOrDefaultAsync(z => z.ZoneNumber == zn && z.Site == site);
                if (header is not null)
                {
                    dto.ZoneNumber = header.ZoneNumber;
                    dto.Name = header.Name;
                    dto.RemoveOnExceed = header.MaxStayOn ?? false;
                    dto.MaxStayHours = header.MaxStay ?? 0;
                    dto.FireZone = header.FireZone ?? false;
                    dto.RestrictZoneAccess = header.RestrictCardholders ?? true;
                }
                selected = await _context.SpaceZoneDoor.AsNoTracking()
                    .Where(d => d.Zone == zn)
                    .ToDictionaryAsync(d => d.Door);
            }

            dto.Doors = doors.Select(d =>
            {
                selected.TryGetValue(d.Door, out var det);
                return new SpaceZoneDoorDto
                {
                    Door = d.Door,
                    Name = d.Name,
                    Included = det is not null,
                    OpenOnFireAlarm = det?.OpenOnFireAlarm ?? false,
                };
            }).ToList();

            return dto;
        }

        public async Task<SpaceZone> Save(SpaceZoneSaveDto dto)
        {
            SpaceZone header;
            int zone;
            if (dto.ZoneNumber is int zn && await _context.SpaceZone.FindAsync(zn) is { } existing)
            {
                existing.Name = dto.Name;
                existing.MaxStayOn = dto.RemoveOnExceed;
                existing.MaxStay = dto.MaxStayHours;
                existing.FireZone = dto.FireZone;
                existing.RestrictCardholders = dto.RestrictZoneAccess;
                header = existing;
                zone = zn;

                var old = await _context.SpaceZoneDoor.Where(d => d.Zone == zn).ToListAsync();
                _context.SpaceZoneDoor.RemoveRange(old);
                await _context.SaveChangesAsync();
            }
            else
            {
                zone = await NextZoneNumber();
                header = new SpaceZone
                {
                    ZoneNumber = zone,
                    Site = dto.Site,
                    Name = dto.Name,
                    MaxStayOn = dto.RemoveOnExceed,
                    MaxStay = dto.MaxStayHours,
                    FireZone = dto.FireZone,
                    RestrictCardholders = dto.RestrictZoneAccess,
                    Inuse = true,
                };
                _context.SpaceZone.Add(header);
                await _context.SaveChangesAsync();
            }

            foreach (var d in dto.Doors.Where(x => x.Included))
            {
                _context.SpaceZoneDoor.Add(new SpaceZoneDoor
                {
                    Door = d.Door,
                    Site = dto.Site,
                    Zone = zone,
                    Name = d.Name,
                    OpenOnFireAlarm = d.OpenOnFireAlarm,
                });
            }
            await _context.SaveChangesAsync();
            return header;
        }

        // ZoneNumber is the global key (details key on Zone without Site).
        private async Task<int> NextZoneNumber()
        {
            var max = await _context.SpaceZone.Select(z => (int?)z.ZoneNumber).MaxAsync();
            return (max ?? 0) + 1;
        }
    }
}
