using DoorsWeb.Shared.Entities;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface ICardPackHeaderService
    {
        Task<List<TCardPackHeader>> GetAll();
        Task<TCardPackHeader?> GetById(int id);
        Task<List<TCardPackHeader>> Create(TCardPackHeader entity);
        Task<List<TCardPackHeader>?> Update(int id, TCardPackHeader entity);
        Task<List<TCardPackHeader>?> Delete(int id);
    }
}
