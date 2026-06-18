using DoorsWeb.Shared.DTO;
using DoorsWeb.Shared.Entities;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface IApbzoneHeaderService
    {
        Task<List<TApbzoneHeader>> GetAll();
        Task<TApbzoneHeader?> GetById(int id);
        Task<List<TApbzoneHeader>> Create(TApbzoneHeader entity);
        Task<List<TApbzoneHeader>?> Update(int id, TApbzoneHeader entity);
        Task<List<TApbzoneHeader>?> Delete(int id);

        /// <summary>The site's full door list, with selection/header seeded when apbnumber is given.</summary>
        Task<ApbZoneSaveDto> GetForEdit(int site, int? apbnumber);

        /// <summary>Creates or updates the header and replaces its door rows.</summary>
        Task<TApbzoneHeader> Save(ApbZoneSaveDto dto);
    }
}
