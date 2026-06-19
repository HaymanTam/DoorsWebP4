using DoorsWeb.Shared.Entities;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface IIocontrollerHeaderService
    {
        Task<List<IoController>> GetAll();
        Task<IoController?> GetById(int id);
        Task<List<IoController>> Create(IoController entity);
        Task<List<IoController>?> Update(int id, IoController entity);
        Task<List<IoController>?> Delete(int id);
    }
}
