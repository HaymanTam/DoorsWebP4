using DoorsWeb.Shared.Entities;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface ITimeSheetHeaderService
    {
        Task<List<TTimeSheetHeader>> GetAll();
        Task<TTimeSheetHeader?> GetById(int id);
        Task<List<TTimeSheetHeader>> Create(TTimeSheetHeader entity);
        Task<List<TTimeSheetHeader>?> Update(int id, TTimeSheetHeader entity);
        Task<List<TTimeSheetHeader>?> Delete(int id);
    }
}
