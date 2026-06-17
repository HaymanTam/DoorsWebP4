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
    }
}
