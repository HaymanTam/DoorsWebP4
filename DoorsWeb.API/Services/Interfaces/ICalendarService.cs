using DoorsWeb.Shared.DTO;
using DoorsWeb.Shared.Entities;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface ICalendarService
    {
        Task<List<Calendar>> GetAll();
        Task<Calendar?> GetById(int id);
        Task<List<Calendar>> Create(Calendar entity);
        Task<List<Calendar>?> Update(int id, Calendar entity);
        Task<List<Calendar>?> Delete(int id);

        /// <summary>Header plus its holiday dates, for seeding the editor.</summary>
        Task<CalendarSaveDto?> GetWithHolidays(int id);

        /// <summary>Creates or updates the header and replaces its holiday rows.</summary>
        Task<Calendar> Save(CalendarSaveDto dto);
    }
}
