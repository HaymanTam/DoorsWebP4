using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services
{
    public class EventService : IEventService
    {
        // Cap on how many recent events are loaded/streamed into the client. The client
        // paints the first batch immediately and streams the remainder in the background,
        // so this is bounded by browser memory rather than perceived load time.
        private const int MaxRows = 10000;

        private readonly DoorsEnterpriseContext _context;

        public EventService(DoorsEnterpriseContext context)
        {
            _context = context;
        }

        public IAsyncEnumerable<EventDto> GetAll(DateTime? from = null, DateTime? to = null)
        {
            return Filter(_context.TEvents.AsNoTracking(), from, to)
                .OrderByDescending(e => e.EventDate)
                .Take(MaxRows)
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
                .AsAsyncEnumerable();
        }

        public async Task<int> GetCount(DateTime? from = null, DateTime? to = null)
        {
            return Math.Min(await Filter(_context.TEvents, from, to).CountAsync(), MaxRows);
        }

        // Applies the optional date-range bounds shared by GetAll and GetCount.
        private static IQueryable<TEvents> Filter(IQueryable<TEvents> query, DateTime? from, DateTime? to)
        {
            if (from.HasValue)
                query = query.Where(e => e.EventDate >= from.Value);
            if (to.HasValue)
                query = query.Where(e => e.EventDate <= to.Value);
            return query;
        }
    }
}
