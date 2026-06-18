using DoorsWeb.Shared.DTO;
using DoorsWeb.Shared.Entities;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface ICalendarHeaderService
    {
        Task<List<TCalendarHeader>> GetAll();
        Task<TCalendarHeader?> GetById(int id);
        Task<List<TCalendarHeader>> Create(TCalendarHeader entity);
        Task<List<TCalendarHeader>?> Update(int id, TCalendarHeader entity);
        Task<List<TCalendarHeader>?> Delete(int id);

        /// <summary>Header plus its holiday dates, for seeding the editor.</summary>
        Task<CalendarSaveDto?> GetWithHolidays(int id);

        /// <summary>Creates or updates the header and replaces its holiday rows.</summary>
        Task<TCalendarHeader> Save(CalendarSaveDto dto);
    }
}
