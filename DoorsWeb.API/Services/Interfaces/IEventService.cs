namespace DoorsWeb.API.Services.Interfaces
{
    public interface IEventService
    {
        Task<List<EventListDto>> GetEvents(DateTime? startDate, DateTime? endDate, int? pageSize, int? pageNumber);
        Task<EventListDto?> GetEventById(Guid id);
        Task<List<EventListDto>> CreateEvent(EventCreateDto EvtDto);
        Task<List<EventListDto>?> DeleteEventById(Guid id);
    }
}
