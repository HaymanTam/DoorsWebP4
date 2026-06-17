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
    }
}
