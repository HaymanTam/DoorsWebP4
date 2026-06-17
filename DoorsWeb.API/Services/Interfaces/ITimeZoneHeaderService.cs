using DoorsWeb.Shared.Entities;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface ITimeZoneHeaderService
    {
        Task<List<TTimeZoneHeader>> GetAll();
        Task<TTimeZoneHeader?> GetById(int timeZone, int site);
        Task<List<TTimeZoneHeader>> Create(TTimeZoneHeader entity);
        Task<List<TTimeZoneHeader>?> Update(int timeZone, int site, TTimeZoneHeader entity);
        Task<List<TTimeZoneHeader>?> Delete(int timeZone, int site);
    }
}
