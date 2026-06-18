using DoorsWeb.Shared.DTO;
using DoorsWeb.Shared.Entities;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface ITimeZoneHeaderService
    {
        Task<List<TTimeZoneHeader>> GetAll();
        Task<TTimeZoneHeader?> GetById(int site, int timeZone);
        Task<List<TTimeZoneHeader>> Create(TTimeZoneHeader entity);
        Task<List<TTimeZoneHeader>?> Update(int site, int timeZone, TTimeZoneHeader entity);
        Task<List<TTimeZoneHeader>?> Delete(int site, int timeZone);

        /// <summary>Header plus its element rows, for seeding the editor.</summary>
        Task<TimeZoneSaveDto?> GetWithElements(int site, int timeZone);

        /// <summary>Creates or updates the header and replaces its element rows.</summary>
        Task<TTimeZoneHeader> Save(TimeZoneSaveDto dto);
    }
}
