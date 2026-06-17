using DoorsWeb.Shared.Entities;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface IIocontrollerHeaderService
    {
        Task<List<TIocontrollerHeader>> GetAll();
        Task<TIocontrollerHeader?> GetById(int id);
        Task<List<TIocontrollerHeader>> Create(TIocontrollerHeader entity);
        Task<List<TIocontrollerHeader>?> Update(int id, TIocontrollerHeader entity);
        Task<List<TIocontrollerHeader>?> Delete(int id);
    }
}
