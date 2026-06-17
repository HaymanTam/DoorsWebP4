using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services
{
    public class AlarmService : IAlarmService
    {
        private readonly DoorsEnterpriseContext _context;

        public AlarmService(DoorsEnterpriseContext context)
        {
            _context = context;
        }

        public async Task<List<AlarmListDto>> GetAll()
        {
            return await _context.TAlarms
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
    }
}
