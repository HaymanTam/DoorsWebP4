using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface IAlarmService
    {
        Task<List<AlarmListDto>> GetAll();

        /// <summary>
        /// Marks an alarm as actioned (stamps the date, operator and optional note). Returns false
        /// when no alarm with that code exists; true otherwise (idempotent if already actioned).
        /// </summary>
        Task<bool> ActionAsync(int code, string actionedBy, string? note);
    }
}
