using DoorsWeb.Shared.Entities;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface IUsersService
    {
        Task<List<TUsers>> GetAll();
        Task<TUsers?> GetById(int id);
        Task<List<TUsers>> Create(TUsers entity);
        Task<List<TUsers>?> Update(int id, TUsers entity);
        Task<List<TUsers>?> Delete(int id);
    }
}
