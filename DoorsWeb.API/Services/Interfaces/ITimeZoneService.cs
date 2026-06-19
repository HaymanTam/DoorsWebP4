using DoorsWeb.Shared.DTO;
using DoorsWeb.Shared.Entities;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface ITimeZoneService
    {
        Task<List<TimeZones>> GetAll();
        Task<TimeZones?> GetById(int site, int timeZone);
        Task<List<TimeZones>> Create(TimeZones entity);
        Task<List<TimeZones>?> Update(int site, int timeZone, TimeZones entity);
        Task<List<TimeZones>?> Delete(int site, int timeZone);

        /// <summary>Header plus its element rows, for seeding the editor.</summary>
        Task<TimeZoneSaveDto?> GetWithElements(int site, int timeZone);

        /// <summary>Creates or updates the header and replaces its element rows.</summary>
        Task<TimeZones> Save(TimeZoneSaveDto dto);
    }
}
