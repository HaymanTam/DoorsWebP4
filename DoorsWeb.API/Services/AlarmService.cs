using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;
using Microsoft.AspNetCore.SignalR;

namespace DoorsWeb.API.Services
{
    public class AlarmService : IAlarmService
    {
        private const string AlarmsChanged = "AlarmsChanged";

        private readonly DoorsEnterpriseContext _context;
        private readonly IHubContext<EventHub> _hub;

        public AlarmService(DoorsEnterpriseContext context, IHubContext<EventHub> hub)
        {
            _context = context;
            _hub = hub;
        }

        public async Task<List<AlarmListDto>> GetAll()
        {
            return await _context.Alarms
                .AsNoTracking()
                .OrderByDescending(a => a.AlarmDate)
                .Select(a => new AlarmListDto
                {
                    Code = a.Code,
                    Description = a.AlarmDescription,
                    Location = a.SiteNavigation != null ? a.SiteNavigation.Name : null,
                    Date = a.AlarmDate,
                    Details = a.ActionedText,
                    ActionedDate = a.ActionedDate,
                    ActionedBy = a.ActionedBy,
                    IsActioned = a.ActionedDate != null
                })
                .ToListAsync();
        }

        public async Task<bool> ActionAsync(int code, string actionedBy, string? note)
        {
            var alarm = await _context.Alarms.FirstOrDefaultAsync(a => a.Code == code);
            if (alarm is null) return false;

            // Idempotent: re-actioning an already-actioned alarm is a no-op success.
            if (alarm.ActionedDate is null)
            {
                alarm.ActionedDate = DateTime.Now;
                alarm.ActionedBy = Trim(actionedBy, 20);
                alarm.ActionedText = Trim(note, 255);
                alarm.IsRead = true;
                await _context.SaveChangesAsync();
                await _hub.Clients.All.SendAsync(AlarmsChanged);
            }

            return true;
        }

        // Clamps a value to the column's max length (null/blank passes through unchanged).
        private static string? Trim(string? value, int max)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;
            value = value.Trim();
            return value.Length > max ? value[..max] : value;
        }
    }
}
