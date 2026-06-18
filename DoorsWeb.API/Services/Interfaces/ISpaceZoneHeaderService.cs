using DoorsWeb.Shared.DTO;
using DoorsWeb.Shared.Entities;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface ISpaceZoneHeaderService
    {
        Task<List<TSpaceZoneHeader>> GetAll();
        Task<TSpaceZoneHeader?> GetById(int id);
        Task<List<TSpaceZoneHeader>> Create(TSpaceZoneHeader entity);
        Task<List<TSpaceZoneHeader>?> Update(int id, TSpaceZoneHeader entity);
        Task<List<TSpaceZoneHeader>?> Delete(int id);

        /// <summary>The site's full door list, with selection/header seeded when zoneNumber is given.</summary>
        Task<SpaceZoneSaveDto> GetForEdit(int site, int? zoneNumber);

        /// <summary>Creates or updates the header and replaces its door rows.</summary>
        Task<TSpaceZoneHeader> Save(SpaceZoneSaveDto dto);
    }
}
