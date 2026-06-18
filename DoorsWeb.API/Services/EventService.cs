using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services
{
    public class EventService : IEventService
    {
        private readonly DoorsEnterpriseContext _context;

        public EventService(DoorsEnterpriseContext context)
        {
            _context = context;
        }

        public async Task<List<EventDto>> GetAll()
        {
            return await _context.TEvents
                .AsNoTracking()
                .OrderByDescending(e => e.EventDate)
                .Select(e => new EventDto
                {
                    EventId = e.EventId,
                    EventDate = e.EventDate,
                    CardNumber = e.CardNumber,
                    CardHolder = e.CardNumberNavigation != null
                        ? ((e.CardNumberNavigation.Forname ?? "") + " " + (e.CardNumberNavigation.Surname ?? "")).Trim()
                        : null,
                    DoorNumber = e.DoorNumber,
                    DoorName = e.DoorNavigation != null ? e.DoorNavigation.Name : null,
                    ReaderId = e.ReaderId,
                    EventType = e.EventType,
                    EventName = e.EventTypeNavigation != null ? e.EventTypeNavigation.Description : null,
                    ActualCardId = e.ActualCardId
                })
                .ToListAsync();
        }
    }
}
