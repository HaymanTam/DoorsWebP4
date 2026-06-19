using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services
{
    public class TriggersHeaderService : ITriggersHeaderService
    {
        private const int TriggerTypeSpaceZone = 3;

        private readonly DoorsEnterpriseContext _context;

        public TriggersHeaderService(DoorsEnterpriseContext context)
        {
            _context = context;
        }

        public async Task<List<Trigger>> GetAll()
        {
            return await _context.Trigger.AsNoTracking().ToListAsync();
        }

        public async Task<Trigger?> GetById(int id)
        {
            return await _context.Trigger.FindAsync(id);
        }

        public async Task<List<Trigger>> Create(Trigger entity)
        {
            _context.Trigger.Add(entity);
            await _context.SaveChangesAsync();
            return await _context.Trigger.AsNoTracking().ToListAsync();
        }

        public async Task<List<Trigger>?> Update(int id, Trigger entity)
        {
            var result = await _context.Trigger.FindAsync(id);
            if (result is null) return null;
            entity.Code = id; // keep route and body key aligned
            _context.Entry(result).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return await _context.Trigger.AsNoTracking().ToListAsync();
        }

        public async Task<List<Trigger>?> Delete(int id)
        {
            var result = await _context.Trigger.FindAsync(id);
            if (result is null) return null;

            var controllers = await _context.TriggerController.Where(c => c.Code == id).ToListAsync();
            _context.TriggerController.RemoveRange(controllers);
            var events = await _context.TriggerEvent.Where(e => e.Code == id).ToListAsync();
            _context.TriggerEvent.RemoveRange(events);
            _context.Trigger.Remove(result);
            await _context.SaveChangesAsync();
            return await _context.Trigger.AsNoTracking().ToListAsync();
        }

        public async Task<TriggerSaveDto> GetForEdit(int site, int triggerType, int? code)
        {
            // Sources are doors for door triggers, space zones for space-zone triggers.
            List<(int Code, string? Name)> sources = triggerType == TriggerTypeSpaceZone
                ? (await _context.SpaceZone.AsNoTracking()
                        .Where(z => z.Site == site)
                        .OrderBy(z => z.Name)
                        .Select(z => new { z.ZoneNumber, z.Name })
                        .ToListAsync())
                    .Select(z => (z.ZoneNumber, z.Name)).ToList()
                : (await _context.Doors.AsNoTracking()
                        .Where(d => d.Site == site)
                        .OrderBy(d => d.Name)
                        .Select(d => new { d.Door, d.Name })
                        .ToListAsync())
                    .Select(d => (d.Door, d.Name)).ToList();

            var dto = new TriggerSaveDto { Site = site, TriggerType = triggerType };

            var selected = new HashSet<int>();
            if (code is int c)
            {
                var header = await _context.Trigger.AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Code == c && t.Site == site);
                if (header is not null)
                {
                    dto.Code = header.Code;
                    dto.Name = header.Name;
                    dto.TriggerType = header.TriggerType;
                    dto.ShowAlarm = header.ShowAlarm;
                    dto.AlarmText = header.AlarmText;
                    dto.SuppressDuplicates = header.SuppressDuplicates;
                    dto.TriggerRelayB = header.TriggerRelayB;
                    dto.RelayBdoor = header.RelayBdoor;
                    dto.OpenRelayB = header.OpenRelayB;
                    dto.ResetRelayB = header.ResetRelayB;
                    dto.ResetRelayBperiod = header.ResetRelayBperiod;
                    dto.PopulationDirection = header.PopulationDirection;
                    dto.PopulationValue = header.PopulationValue;
                }
                selected = (await _context.TriggerController.AsNoTracking()
                    .Where(t => t.Code == c)
                    .Select(t => t.ControllerCode)
                    .ToListAsync()).ToHashSet();
            }

            dto.Sources = sources.Select(s => new TriggerSourceDto
            {
                Code = s.Code,
                Name = s.Name,
                Selected = selected.Contains(s.Code),
            }).ToList();

            return dto;
        }

        public async Task<Trigger> Save(TriggerSaveDto dto)
        {
            Trigger header;
            int code;
            if (dto.Code is int c && await _context.Trigger.FindAsync(c) is { } existing)
            {
                ApplyHeader(existing, dto);
                header = existing;
                code = c;

                var oldControllers = await _context.TriggerController.Where(t => t.Code == c).ToListAsync();
                _context.TriggerController.RemoveRange(oldControllers);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Code is a DB identity, so it is assigned on insert.
                header = new Trigger { Site = dto.Site, Name = dto.Name ?? "", AlarmText = "" };
                ApplyHeader(header, dto);
                _context.Trigger.Add(header);
                await _context.SaveChangesAsync();
                code = header.Code;
            }

            // Source code (door/zone) goes in ControllerCode; legacy stores InputIndex 0 for both.
            foreach (var s in dto.Sources.Where(x => x.Selected))
            {
                _context.TriggerController.Add(new TriggerController
                {
                    Code = code,
                    ControllerCode = s.Code,
                    InputIndex = 0,
                });
            }
            await _context.SaveChangesAsync();
            return header;
        }

        private static void ApplyHeader(Trigger header, TriggerSaveDto dto)
        {
            header.Name = dto.Name ?? "";
            header.TriggerType = dto.TriggerType;
            header.ShowAlarm = dto.ShowAlarm;
            header.AlarmText = dto.AlarmText ?? "";
            header.SuppressDuplicates = dto.SuppressDuplicates;
            header.TriggerRelayB = dto.TriggerRelayB;
            header.RelayBdoor = dto.RelayBdoor;
            header.OpenRelayB = dto.OpenRelayB;
            header.ResetRelayB = dto.ResetRelayB;
            header.ResetRelayBperiod = dto.ResetRelayBperiod;
            header.PopulationDirection = dto.PopulationDirection;
            header.PopulationValue = dto.PopulationValue;
        }
    }
}
