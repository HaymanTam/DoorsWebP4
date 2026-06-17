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
    }
}
