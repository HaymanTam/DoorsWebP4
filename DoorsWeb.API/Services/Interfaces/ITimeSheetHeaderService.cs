using DoorsWeb.Shared.Entities;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface ITimeSheetHeaderService
    {
        Task<List<TimeSheet>> GetAll();
        Task<TimeSheet?> GetById(int id);
        Task<List<TimeSheet>> Create(TimeSheet entity);
        Task<List<TimeSheet>?> Update(int id, TimeSheet entity);
        Task<List<TimeSheet>?> Delete(int id);
    }
}
