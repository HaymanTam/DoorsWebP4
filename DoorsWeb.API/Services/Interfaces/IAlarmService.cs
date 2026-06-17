using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface IAlarmService
    {
        Task<List<AlarmListDto>> GetAll();
    }
}
