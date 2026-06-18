using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface IEventService
    {
        Task<List<EventDto>> GetAll();
    }
}
