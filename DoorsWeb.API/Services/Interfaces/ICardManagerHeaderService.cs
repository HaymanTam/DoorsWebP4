using DoorsWeb.Shared.Entities;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface ICardManagerHeaderService
    {
        Task<List<TCardManagerHeader>> GetAll();
        Task<TCardManagerHeader?> GetById(int id);
        Task<List<TCardManagerHeader>> Create(TCardManagerHeader entity);
        Task<List<TCardManagerHeader>?> Update(int id, TCardManagerHeader entity);
        Task<List<TCardManagerHeader>?> Delete(int id);
    }
}
