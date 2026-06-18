using DoorsWeb.Shared.DTO;
using DoorsWeb.Shared.Entities;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface ITriggersHeaderService
    {
        Task<List<TTriggersHeader>> GetAll();
        Task<TTriggersHeader?> GetById(int id);
        Task<List<TTriggersHeader>> Create(TTriggersHeader entity);
        Task<List<TTriggersHeader>?> Update(int id, TTriggersHeader entity);
        Task<List<TTriggersHeader>?> Delete(int id);

        /// <summary>The site's source doors (triggerType 1) or space zones (triggerType 3),
        /// with selection/header seeded when code is given.</summary>
        Task<TriggerSaveDto> GetForEdit(int site, int triggerType, int? code);

        /// <summary>Creates or updates the header and replaces its source rows.</summary>
        Task<TTriggersHeader> Save(TriggerSaveDto dto);
    }
}
