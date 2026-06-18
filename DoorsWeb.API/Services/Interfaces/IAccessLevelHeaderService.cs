using DoorsWeb.Shared.DTO;
using DoorsWeb.Shared.Entities;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface IAccessLevelHeaderService
    {
        Task<List<TAccessLevelHeader>> GetAll();
        Task<TAccessLevelHeader?> GetById(int site, int accessLevel);
        Task<List<TAccessLevelHeader>> Create(TAccessLevelHeader entity);
        Task<List<TAccessLevelHeader>?> Update(int site, int accessLevel, TAccessLevelHeader entity);
        Task<List<TAccessLevelHeader>?> Delete(int site, int accessLevel);

        /// <summary>The site's full door list, with selection/header seeded when accessLevel is given.</summary>
        Task<AccessLevelSaveDto> GetForEdit(int site, int? accessLevel);

        /// <summary>Creates or updates the header and replaces its door rows.</summary>
        Task<TAccessLevelHeader> Save(AccessLevelSaveDto dto);
    }
}
