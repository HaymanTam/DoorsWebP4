using DoorsWeb.Shared.Entities;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface IUsersService
    {
        Task<List<Users>> GetAll();
        Task<Users?> GetById(int id);
        Task<List<Users>> Create(Users entity);
        Task<List<Users>?> Update(int id, Users entity);
        Task<List<Users>?> Delete(int id);
    }
}
