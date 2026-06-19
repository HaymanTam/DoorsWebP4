using DoorsWeb.Shared.DTO;
using DoorsWeb.Shared.Entities;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface ISpaceZoneService
    {
        Task<List<SpaceZone>> GetAll();
        Task<SpaceZone?> GetById(int id);
        Task<List<SpaceZone>> Create(SpaceZone entity);
        Task<List<SpaceZone>?> Update(int id, SpaceZone entity);
        Task<List<SpaceZone>?> Delete(int id);

        /// <summary>The site's full door list, with selection/header seeded when zoneNumber is given.</summary>
        Task<SpaceZoneSaveDto> GetForEdit(int site, int? zoneNumber);

        /// <summary>Creates or updates the header and replaces its door rows.</summary>
        Task<SpaceZone> Save(SpaceZoneSaveDto dto);
    }
}
