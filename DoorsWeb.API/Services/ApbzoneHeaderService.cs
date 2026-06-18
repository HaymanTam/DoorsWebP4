using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services
{
    public class ApbzoneHeaderService : IApbzoneHeaderService
    {
        private readonly DoorsEnterpriseContext _context;

        public ApbzoneHeaderService(DoorsEnterpriseContext context)
        {
            _context = context;
        }

        public async Task<List<TApbzoneHeader>> GetAll()
        {
            return await _context.TApbzoneHeader.AsNoTracking().ToListAsync();
        }

        public async Task<TApbzoneHeader?> GetById(int id)
        {
            return await _context.TApbzoneHeader.FindAsync(id);
        }

        public async Task<List<TApbzoneHeader>> Create(TApbzoneHeader entity)
        {
            _context.TApbzoneHeader.Add(entity);
            await _context.SaveChangesAsync();
            return await _context.TApbzoneHeader.AsNoTracking().ToListAsync();
        }

        public async Task<List<TApbzoneHeader>?> Update(int id, TApbzoneHeader entity)
        {
            var result = await _context.TApbzoneHeader.FindAsync(id);
            if (result is null) return null;
            entity.Apbnumber = id; // keep route and body key aligned
            _context.Entry(result).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return await _context.TApbzoneHeader.AsNoTracking().ToListAsync();
        }

        public async Task<List<TApbzoneHeader>?> Delete(int id)
        {
            var result = await _context.TApbzoneHeader.FindAsync(id);
            if (result is null) return null;

            var details = await _context.TApbzoneDetails.Where(d => d.Apbnumber == id).ToListAsync();
            _context.TApbzoneDetails.RemoveRange(details);
            _context.TApbzoneHeader.Remove(result);
            await _context.SaveChangesAsync();
            return await _context.TApbzoneHeader.AsNoTracking().ToListAsync();
        }

        public async Task<ApbZoneSaveDto> GetForEdit(int site, int? apbnumber)
        {
            var doors = await _context.TDoors.AsNoTracking()
                .Where(d => d.Site == site)
                .OrderBy(d => d.Name)
                .Select(d => new { d.Door, d.Name })
                .ToListAsync();

            var dto = new ApbZoneSaveDto { Site = site };

            var selected = new HashSet<int>();
            if (apbnumber is int apb)
            {
                var header = await _context.TApbzoneHeader.AsNoTracking()
                    .FirstOrDefaultAsync(a => a.Apbnumber == apb && a.Site == site);
                if (header is not null)
                {
                    dto.Apbnumber = header.Apbnumber;
                    dto.Name = header.Name;
                    dto.Mode = ModeText(header.Apbmode);
                    dto.DiscoveryDuration = DurationText(header.DiscoveryMode);
                    dto.DiscoveryMinutes = header.DiscoveryModeDuration;
                    dto.DiscoveryExpiry = header.DiscoveryModeExpiryDate;
                    dto.FireAlarmReset = header.DiscoveryModeOnFireAlarm ?? false;
                    dto.FireAlarmDiscoveryMinutes = header.DiscoveryModeOnFireAlarmDuration ?? 0;
                    dto.LogOutDaily = header.AutoLogOut ?? false;
                    dto.LogOutTime = header.NextAutoLogout?.ToString("HH:mm");
                }
                selected = (await _context.TApbzoneDetails.AsNoTracking()
                    .Where(d => d.Apbnumber == apb)
                    .Select(d => d.DoorNumber)
                    .ToListAsync()).ToHashSet();
            }

            dto.Doors = doors.Select(d => new ApbZoneDoorDto
            {
                Door = d.Door,
                Name = d.Name,
                Included = selected.Contains(d.Door),
            }).ToList();

            return dto;
        }

        public async Task<TApbzoneHeader> Save(ApbZoneSaveDto dto)
        {
            TApbzoneHeader header;
            int apb;
            if (dto.Apbnumber is int an && await _context.TApbzoneHeader.FindAsync(an) is { } existing)
            {
                ApplyHeader(existing, dto);
                header = existing;
                apb = an;

                var old = await _context.TApbzoneDetails.Where(d => d.Apbnumber == an).ToListAsync();
                _context.TApbzoneDetails.RemoveRange(old);
                await _context.SaveChangesAsync();
            }
            else
            {
                apb = await NextApbnumber();
                header = new TApbzoneHeader { Apbnumber = apb, Site = dto.Site };
                ApplyHeader(header, dto);
                _context.TApbzoneHeader.Add(header);
                await _context.SaveChangesAsync();
            }

            // The web Doors tab only offers an include checkbox; reader/role/enforce columns have no
            // editor yet, so they are left null rather than written with guessed values.
            foreach (var d in dto.Doors.Where(x => x.Included))
            {
                _context.TApbzoneDetails.Add(new TApbzoneDetails
                {
                    Apbnumber = apb,
                    DoorNumber = d.Door,
                });
            }
            await _context.SaveChangesAsync();
            return header;
        }

        private static void ApplyHeader(TApbzoneHeader header, ApbZoneSaveDto dto)
        {
            header.Name = dto.Name;
            header.Apbmode = ModeValue(dto.Mode);
            header.DiscoveryMode = DurationValue(dto.DiscoveryDuration);
            header.DiscoveryModeDuration = dto.DiscoveryMinutes ?? 0;
            header.DiscoveryModeExpiryDate = dto.DiscoveryExpiry;
            header.DiscoveryModeOnFireAlarm = dto.FireAlarmReset;
            header.DiscoveryModeOnFireAlarmDuration = dto.FireAlarmDiscoveryMinutes;
            header.AutoLogOut = dto.LogOutDaily;
            header.NextAutoLogout = NextLogout(dto.LogOutDaily, dto.LogOutTime);
        }

        // Apbnumber is the global key (details key on Apbnumber without Site).
        private async Task<int> NextApbnumber()
        {
            var max = await _context.TApbzoneHeader.Select(a => (int?)a.Apbnumber).MaxAsync();
            return (max ?? 0) + 1;
        }

        // Legacy APB_Modes enum (frmAPBZones.frm): Discovery=0, Active=1, Off=2.
        private static int ModeValue(string? mode) => mode switch
        {
            "Active" => 1,
            "Off" => 2,
            _ => 0,
        };

        private static string ModeText(int? mode) => mode switch
        {
            1 => "Active",
            2 => "Off",
            _ => "Discovery",
        };

        // Legacy APB_DiscoveryModes enum: Minutes(LastsFor)=0, DateTime(SetUntil)=1, NoExpiry(FurtherNotice)=2.
        private static int DurationValue(string? duration) => duration switch
        {
            "LastsFor" => 0,
            "SetUntil" => 1,
            _ => 2,
        };

        private static string DurationText(int? duration) => duration switch
        {
            0 => "LastsFor",
            1 => "SetUntil",
            _ => "FurtherNotice",
        };

        private static DateTime? NextLogout(bool enabled, string? time)
        {
            if (!enabled || !TimeOnly.TryParse(time, out var t)) return null;
            var now = DateTime.Now;
            var candidate = now.Date + t.ToTimeSpan();
            return candidate <= now ? candidate.AddDays(1) : candidate;
        }
    }
}
