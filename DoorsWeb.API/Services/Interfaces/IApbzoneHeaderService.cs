using DoorsWeb.Shared.DTO;
using DoorsWeb.Shared.Entities;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface IApbzoneHeaderService
    {
        Task<List<ApbZone>> GetAll();
        Task<ApbZone?> GetById(int id);
        Task<List<ApbZone>> Create(ApbZone entity);
        Task<List<ApbZone>?> Update(int id, ApbZone entity);
        Task<List<ApbZone>?> Delete(int id);

        /// <summary>The site's full door list, with selection/header seeded when apbnumber is given.</summary>
        Task<ApbZoneSaveDto> GetForEdit(int site, int? apbnumber);

        /// <summary>Creates or updates the header and replaces its door rows.</summary>
        Task<ApbZone> Save(ApbZoneSaveDto dto);
    }
}
