using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;
using DoorsWeb.Shared.Entities;

namespace DoorsWeb.API.Services
{
    public class DoorService : IDoorService
    {
        // Legacy sentinel meaning "no time zone / never open" (frmDoors lcNoTimeZone).
        private const int NoTimeZone = 10000;

        private readonly DoorsEnterpriseContext _context;
        private readonly IAuditService _audit;

        public DoorService(DoorsEnterpriseContext context, IAuditService audit)
        {
            _context = context;
            _audit = audit;
        }

        public async Task<List<DoorListDto>> GetAll()
        {
            // Pull raw columns first; ControllerID parsing isn't translatable to SQL.
            var rows = await _context.Doors
                .AsNoTracking()
                .OrderBy(d => d.Name)
                .Select(d => new { d.Door, d.ControllerId, d.Name, d.DoorIpaddress, d.Site, d.Updated })
                .ToListAsync();

            return rows.Select(d => new DoorListDto
            {
                Door = d.Door,
                ControllerId = ParseControllerId(d.ControllerId),
                Name = d.Name ?? string.Empty,
                IPAddressString = d.DoorIpaddress ?? string.Empty,
                Site = d.Site,
                LastUpdated = d.Updated ?? DateTime.MinValue
            }).ToList();
        }

        public async Task<DoorDetailDto?> GetById(int door)
        {
            var e = await _context.Doors.AsNoTracking().FirstOrDefaultAsync(d => d.Door == door);
            return e is null ? null : ToDetail(e);
        }

        public async Task<List<DoorListDto>> Create(DoorDetailDto dto)
        {
            var e = new Doors { Door = dto.Door };
            ApplyToEntity(dto, e);
            _context.Doors.Add(e);
            await _context.SaveChangesAsync();
            await _audit.LogAsync(AuditAction.Create, "Door", e.Door.ToString(), e.Name);
            return await GetAll();
        }

        public async Task<List<DoorListDto>?> Update(int door, DoorDetailDto dto)
        {
            var e = await _context.Doors.FirstOrDefaultAsync(d => d.Door == door);
            if (e is null) return null;
            ApplyToEntity(dto, e);
            await _context.SaveChangesAsync();
            await _audit.LogAsync(AuditAction.Update, "Door", door.ToString(), e.Name);
            return await GetAll();
        }

        public async Task<List<DoorListDto>?> Delete(int door)
        {
            var e = await _context.Doors.FirstOrDefaultAsync(d => d.Door == door);
            if (e is null) return null;
            _context.Doors.Remove(e);
            await _context.SaveChangesAsync();
            await _audit.LogAsync(AuditAction.Delete, "Door", door.ToString(), e.Name);
            return await GetAll();
        }

        // ---- mapping helpers ------------------------------------------------

        private static int ParseControllerId(string? value)
            => int.TryParse(value, out var id) ? id : 0;

        private static DoorDetailDto ToDetail(Doors e)
        {
            var pdo = e.Pdo ?? 0;
            var timeLock = e.TimeLock ?? NoTimeZone;

            return new DoorDetailDto
            {
                Door = e.Door,
                ControllerId = ParseControllerId(e.ControllerId),
                Name = e.Name ?? string.Empty,
                IPAddressString = e.DoorIpaddress ?? string.Empty,

                AutoRelockEnable = e.AutoRelock ?? false,
                RelayA_Delay = ToByte(e.ReleaseDelay),
                RelayA_Time = ToByte(e.ReleaseTime),
                RelayA_TZOverideEnable = timeLock != NoTimeZone,
                RelayA_OpenTimeZone = timeLock == NoTimeZone ? null : timeLock,
                RelayB_Mode = ToByte(e.RelayBmode),
                RelayB_Delay = ToByte(e.AutoDelayVal),
                RelayB_Time = ToByte(e.ReleaseTimeB),
                RelayB_TimeZone = e.RelayBtimeZone,
                PDO_AlarmEnable = pdo > 0,
                PDO_Alarm_Time = ToByte(pdo),
                LockDriveMode = ToByte(e.LockDriveMode),
                ValidFrom = ToTime(e.ValidFromTimeHh, e.ValidFromTimeMm),
                ValidTo = ToTime(e.ValidToTimeHh, e.ValidToTimeMm),
                Notes = e.Notes,
                RandomSearchFrequency = e.RandomSearchFreq ?? 0,
                Controller_FeedbackVolume = ToByte(e.ConFbVolume, 15),
                Controller_AlarmVolume = ToByte(e.ConAlmVolume, 15),

                Keypad_VCardLength = ToByte(e.AccessCodeLen),
                Keypad_AccessCode = ReassembleAccessCode(e),
                Keypad_StarModeEnable = (e.KeypadStarMode ?? 0) != 0,
                Keypad_Name = e.KeyboardName,
                Keypad_TechId = ToByte(e.KeyboardTech),
                MFA_OverrideTimeZone = e.CardandPintimeZone,

                ReaderA_Name = e.ReaderAname ?? "In",
                ReaderA_TechId = ToByte(e.TechnologyA),
                ReaderA_Volume = ToByte(e.RdrVolumeA, 15),
                ReaderA_Brightness = ToByte(e.RdrBrightnessA, 5),
                ReaderA_MFA_Sequence = ToByte(e.IdSequenceA),

                ReaderB_Name = e.ReaderBname ?? "Out",
                ReaderB_TechId = ToByte(e.TechnologyB),
                ReaderB_Volume = ToByte(e.RdrVolumeB, 15),
                ReaderB_Brightness = ToByte(e.RdrBrightnessB, 5),
                ReaderB_MFA_Sequence = ToByte(e.IdSequenceB),

                LastUpdated = e.Updated ?? DateTime.Now
            };
        }

        private static void ApplyToEntity(DoorDetailDto dto, Doors e)
        {
            e.ControllerId = dto.ControllerId.ToString();
            e.Name = dto.Name;
            e.DoorIpaddress = dto.IPAddressString;
            // Connectors are obsolete in this IP-only system; Doors.Connector is left untouched
            // (null on new doors, preserved on existing ones).

            e.AutoRelock = dto.AutoRelockEnable;
            e.ReleaseDelay = dto.RelayA_Delay;
            e.ReleaseTime = dto.RelayA_Time;
            e.TimeLock = dto.RelayA_TZOverideEnable ? (dto.RelayA_OpenTimeZone ?? NoTimeZone) : NoTimeZone;
            e.RelayBmode = dto.RelayB_Mode;
            e.AutoDelayVal = dto.RelayB_Delay;
            e.ReleaseTimeB = dto.RelayB_Time;
            e.RelayBtimeZone = dto.RelayB_TimeZone;
            e.Pdo = dto.PDO_AlarmEnable ? dto.PDO_Alarm_Time : 0;
            e.LockDriveMode = dto.LockDriveMode;
            e.ValidFromTimeHh = dto.ValidFrom.Hour;
            e.ValidFromTimeMm = dto.ValidFrom.Minute;
            e.ValidToTimeHh = dto.ValidTo.Hour;
            e.ValidToTimeMm = dto.ValidTo.Minute;
            e.Notes = dto.Notes;
            e.RandomSearchFreq = dto.RandomSearchFrequency;
            e.ConFbVolume = dto.Controller_FeedbackVolume;
            e.ConAlmVolume = dto.Controller_AlarmVolume;

            e.AccessCodeLen = dto.Keypad_VCardLength;
            SplitAccessCode(dto.Keypad_AccessCode ?? 0, e);
            e.KeypadStarMode = dto.Keypad_StarModeEnable ? 1 : 0;
            e.KeyboardName = dto.Keypad_Name;
            e.KeyboardTech = dto.Keypad_TechId;
            e.CardandPintimeZone = dto.MFA_OverrideTimeZone;

            e.ReaderAname = dto.ReaderA_Name;
            e.TechnologyA = dto.ReaderA_TechId;
            e.RdrVolumeA = dto.ReaderA_Volume;
            e.RdrBrightnessA = dto.ReaderA_Brightness;
            e.IdSequenceA = dto.ReaderA_MFA_Sequence;

            e.ReaderBname = dto.ReaderB_Name;
            e.TechnologyB = dto.ReaderB_TechId;
            e.RdrVolumeB = dto.ReaderB_Volume;
            e.RdrBrightnessB = dto.ReaderB_Brightness;
            e.IdSequenceB = dto.ReaderB_MFA_Sequence;

            e.Updated = DateTime.Now;
        }

        private static byte ToByte(int? value, byte fallback = 0)
        {
            if (value is null) return fallback;
            if (value < 0) return 0;
            if (value > 255) return 255;
            return (byte)value.Value;
        }

        private static TimeOnly ToTime(int? hh, int? mm)
        {
            var h = Math.Clamp(hh ?? 0, 0, 23);
            var m = Math.Clamp(mm ?? 0, 0, 59);
            return new TimeOnly(h, m);
        }

        // Access code is stored across AccessCode_Dig1..8, least-significant digit first
        // (legacy: Dig{i} = digit at position 9-i of the zero-padded 8-digit code).
        private static int? ReassembleAccessCode(Doors e)
        {
            var digits = new[]
            {
                e.AccessCodeDig1, e.AccessCodeDig2, e.AccessCodeDig3, e.AccessCodeDig4,
                e.AccessCodeDig5, e.AccessCodeDig6, e.AccessCodeDig7, e.AccessCodeDig8
            };

            int code = 0;
            int multiplier = 1;
            foreach (var d in digits)
            {
                code += (d ?? 0) * multiplier;
                multiplier *= 10;
            }
            return code == 0 ? null : code;
        }

        private static void SplitAccessCode(int code, Doors e)
        {
            if (code < 0) code = 0;
            e.AccessCodeDig1 = code / 1 % 10;
            e.AccessCodeDig2 = code / 10 % 10;
            e.AccessCodeDig3 = code / 100 % 10;
            e.AccessCodeDig4 = code / 1000 % 10;
            e.AccessCodeDig5 = code / 10000 % 10;
            e.AccessCodeDig6 = code / 100000 % 10;
            e.AccessCodeDig7 = code / 1000000 % 10;
            e.AccessCodeDig8 = code / 10000000 % 10;
        }
    }
}
