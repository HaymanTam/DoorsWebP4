using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface ICalendarService
    {
        Task<List<AccessCalendarDto>> GetAllCalendars();
        Task<AccessCalendarDto?> GetCalendarById(Guid id);
        Task<List<AccessCalendarDto>> CreateCalendar(AccessCalendarDto dto);
        Task<List<AccessCalendarDto>?> UpdateCalendar(Guid id, AccessCalendarDto dto);
        Task<List<AccessCalendarDto>?> DeleteCalendarById(Guid id);
    }
}
