using DoorsWeb.Shared.DTO;
using DoorsWeb.Shared.Entities;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface IAccessLevelService
    {
        Task<List<AccessLevels>> GetAll();
        Task<AccessLevels?> GetById(int site, int accessLevel);
        Task<List<AccessLevels>> Create(AccessLevels entity);
        Task<List<AccessLevels>?> Update(int site, int accessLevel, AccessLevels entity);
        Task<List<AccessLevels>?> Delete(int site, int accessLevel);

        /// <summary>The site's full door list, with selection/header seeded when accessLevel is given.</summary>
        Task<AccessLevelSaveDto> GetForEdit(int site, int? accessLevel);

        /// <summary>Creates or updates the header and replaces its door rows.</summary>
        Task<AccessLevels> Save(AccessLevelSaveDto dto);
    }
}
