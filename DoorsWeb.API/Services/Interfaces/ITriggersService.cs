using DoorsWeb.Shared.DTO;
using DoorsWeb.Shared.Entities;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface ITriggersService
    {
        Task<List<Trigger>> GetAll();
        Task<Trigger?> GetById(int id);
        Task<List<Trigger>> Create(Trigger entity);
        Task<List<Trigger>?> Update(int id, Trigger entity);
        Task<List<Trigger>?> Delete(int id);

        /// <summary>The site's source doors (triggerType 1) or space zones (triggerType 3),
        /// with selection/header seeded when code is given.</summary>
        Task<TriggerSaveDto> GetForEdit(int site, int triggerType, int? code);

        /// <summary>Creates or updates the header and replaces its source rows.</summary>
        Task<Trigger> Save(TriggerSaveDto dto);
    }
}
