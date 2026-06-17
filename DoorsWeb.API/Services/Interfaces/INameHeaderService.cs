using DoorsWeb.Shared.Entities;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface INameHeaderService
    {
        Task<List<TNameHeader>> GetAll();
        Task<TNameHeader?> GetById(int id);
        Task<List<TNameHeader>> Create(TNameHeader entity);
        Task<List<TNameHeader>?> Update(int id, TNameHeader entity);
        Task<List<TNameHeader>?> Delete(int id);
    }
}
