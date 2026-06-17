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
    }
}
