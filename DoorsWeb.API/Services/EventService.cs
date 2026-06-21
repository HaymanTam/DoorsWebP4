using System.Linq.Expressions;
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

        // Single source of truth for Events -> EventDto so the streamed list (GetAll) and a freshly
        // recorded event (RecordAsync) are shaped identically, including the navigation lookups.
        private static readonly Expression<Func<Events, EventDto>> ToDto = e => new EventDto
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
        };

        public IAsyncEnumerable<EventDto> GetAll(DateTime? from = null, DateTime? to = null)
        {
            return Filter(_context.Events.AsNoTracking(), from, to)
                .OrderByDescending(e => e.EventDate)
                .Take(MaxRows)
                .Select(ToDto)
                .AsAsyncEnumerable();
        }

        public async Task<EventDto?> RecordAsync(int door, DateTime when, int cardNumber, string? cardId,
            int readerId, int eventType, CancellationToken ct = default)
        {
            // EventId is database-generated (ValueGeneratedOnAdd), so we don't supply it; EF fills it
            // in on the entity after SaveChanges, which we then use to re-read the row with navigations.
            var entity = new Events
            {
                EventDate = when,
                CardNumber = cardNumber,
                DoorNumber = door,
                EventType = eventType,
                ReaderId = readerId,
                ActualCardId = cardId
            };

            _context.Events.Add(entity);
            await _context.SaveChangesAsync(ct);

            return await _context.Events.AsNoTracking()
                .Where(e => e.EventId == entity.EventId)
                .Select(ToDto)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<int> GetCount(DateTime? from = null, DateTime? to = null)
        {
            return Math.Min(await Filter(_context.Events, from, to).CountAsync(), MaxRows);
        }

        // Applies the optional date-range bounds shared by GetAll and GetCount.
        private static IQueryable<Events> Filter(IQueryable<Events> query, DateTime? from, DateTime? to)
        {
            if (from.HasValue)
                query = query.Where(e => e.EventDate >= from.Value);
            if (to.HasValue)
                query = query.Where(e => e.EventDate <= to.Value);
            return query;
        }
    }
}
