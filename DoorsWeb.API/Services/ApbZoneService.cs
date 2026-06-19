using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services
{
    public class ApbZoneService : IApbZoneService
    {
        private readonly DoorsEnterpriseContext _context;
        private readonly IAuditService _audit;

        public ApbZoneService(DoorsEnterpriseContext context, IAuditService audit)
        {
            _context = context;
            _audit = audit;
        }

        public async Task<List<ApbZone>> GetAll()
        {
            return await _context.ApbZone.AsNoTracking().ToListAsync();
        }

        public async Task<ApbZone?> GetById(int id)
        {
            return await _context.ApbZone.FindAsync(id);
        }

        public async Task<List<ApbZone>> Create(ApbZone entity)
        {
            _context.ApbZone.Add(entity);
            await _context.SaveChangesAsync();
            await _audit.LogAsync(AuditAction.Create, "Anti-Passback Zone", entity.Apbnumber.ToString(), entity.Name);
            return await _context.ApbZone.AsNoTracking().ToListAsync();
        }

        public async Task<List<ApbZone>?> Update(int id, ApbZone entity)
        {
            var result = await _context.ApbZone.FindAsync(id);
            if (result is null) return null;
            entity.Apbnumber = id; // keep route and body key aligned
            _context.Entry(result).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            await _audit.LogAsync(AuditAction.Update, "Anti-Passback Zone", id.ToString(), result.Name);
            return await _context.ApbZone.AsNoTracking().ToListAsync();
        }

        public async Task<List<ApbZone>?> Delete(int id)
        {
            var result = await _context.ApbZone.FindAsync(id);
            if (result is null) return null;

            var details = await _context.ApbZoneDoor.Where(d => d.Apbnumber == id).ToListAsync();
            _context.ApbZoneDoor.RemoveRange(details);
            _context.ApbZone.Remove(result);
            await _context.SaveChangesAsync();
            await _audit.LogAsync(AuditAction.Delete, "Anti-Passback Zone", id.ToString(), result.Name);
            return await _context.ApbZone.AsNoTracking().ToListAsync();
        }

        public async Task<ApbZoneSaveDto> GetForEdit(int site, int? apbnumber)
        {
            var doors = await _context.Doors.AsNoTracking()
                .Where(d => d.Site == site)
                .OrderBy(d => d.Name)
                .Select(d => new { d.Door, d.Name })
                .ToListAsync();

            var dto = new ApbZoneSaveDto { Site = site };

            var details = new Dictionary<int, ApbZoneDoor>();
            if (apbnumber is int apb)
            {
                var header = await _context.ApbZone.AsNoTracking()
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
                    dto.FireInterfaceDoor = header.FireInterfaceDoor;
                    dto.FireAlarmDiscoveryMinutes = header.DiscoveryModeOnFireAlarmDuration ?? 0;
                    dto.LogOutDaily = header.AutoLogOut ?? false;
                    dto.LogOutTime = header.NextAutoLogout?.ToString("HH:mm");
                }
                details = (await _context.ApbZoneDoor.AsNoTracking()
                    .Where(d => d.Apbnumber == apb)
                    .ToListAsync())
                    .GroupBy(d => d.DoorNumber)
                    .ToDictionary(g => g.Key, g => g.First());
            }

            dto.Doors = doors.Select(d =>
            {
                details.TryGetValue(d.Door, out var det);
                return new ApbZoneDoorDto
                {
                    Door = d.Door,
                    Name = d.Name,
                    Included = det is not null,
                    MemberType = det?.MemberType ?? 0,
                    ReaderA = det?.ReaderA ?? 0,
                    ReaderB = det?.ReaderB ?? 0,
                    EnforceOnEntry = det?.EnforceOnEntry ?? false,
                    EnforceOnExit = det?.EnforceOnExit ?? false,
                };
            }).ToList();

            return dto;
        }

        public async Task<ApbZone> Save(ApbZoneSaveDto dto)
        {
            ApbZone header;
            int apb;
            bool isUpdate;
            if (dto.Apbnumber is int an && await _context.ApbZone.FindAsync(an) is { } existing)
            {
                isUpdate = true;
                ApplyHeader(existing, dto);
                header = existing;
                apb = an;

                var old = await _context.ApbZoneDoor.Where(d => d.Apbnumber == an).ToListAsync();
                _context.ApbZoneDoor.RemoveRange(old);
                await _context.SaveChangesAsync();
            }
            else
            {
                isUpdate = false;
                apb = await NextApbnumber();
                header = new ApbZone { Apbnumber = apb, Site = dto.Site };
                ApplyHeader(header, dto);
                _context.ApbZone.Add(header);
                await _context.SaveChangesAsync();
            }

            // Per-door behaviour (role / reader direction / enforce) is set via the Doors tab's
            // properties popup; an unconfigured but included door defaults to a Border door.
            foreach (var d in dto.Doors.Where(x => x.Included))
            {
                _context.ApbZoneDoor.Add(new ApbZoneDoor
                {
                    Apbnumber = apb,
                    DoorNumber = d.Door,
                    MemberType = d.MemberType,
                    ReaderA = d.ReaderA,
                    ReaderB = d.ReaderB,
                    EnforceOnEntry = d.EnforceOnEntry,
                    EnforceOnExit = d.EnforceOnExit,
                });
            }
            await _context.SaveChangesAsync();
            await _audit.LogAsync(isUpdate ? AuditAction.Update : AuditAction.Create, "Anti-Passback Zone", header.Apbnumber.ToString(), header.Name);
            return header;
        }

        private static void ApplyHeader(ApbZone header, ApbZoneSaveDto dto)
        {
            header.Name = dto.Name;
            header.Apbmode = ModeValue(dto.Mode);
            header.DiscoveryMode = DurationValue(dto.DiscoveryDuration);
            header.DiscoveryModeDuration = dto.DiscoveryMinutes ?? 0;
            header.DiscoveryModeExpiryDate = dto.DiscoveryExpiry;
            header.DiscoveryModeOnFireAlarm = dto.FireAlarmReset;
            header.FireInterfaceDoor = dto.FireInterfaceDoor;
            header.DiscoveryModeOnFireAlarmDuration = dto.FireAlarmDiscoveryMinutes;
            header.AutoLogOut = dto.LogOutDaily;
            header.NextAutoLogout = NextLogout(dto.LogOutDaily, dto.LogOutTime);
        }

        // Apbnumber is the global key (details key on Apbnumber without Site).
        private async Task<int> NextApbnumber()
        {
            var max = await _context.ApbZone.Select(a => (int?)a.Apbnumber).MaxAsync();
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
