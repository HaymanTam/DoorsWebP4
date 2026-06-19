using DoorsWeb.Shared.DTO;
using DoorsWeb.Shared.Entities;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface ITimeSheetService
    {
        Task<List<TimeSheet>> GetAll();
        Task<TimeSheet?> GetById(int id);
        Task<List<TimeSheet>> Create(TimeSheet entity);
        Task<List<TimeSheet>?> Update(int id, TimeSheet entity);
        Task<List<TimeSheet>?> Delete(int id);

        /// <summary>Load a time sheet definition for editing (header + selected zones + all available zones).</summary>
        Task<TimeSheetSaveDto> GetForEdit(int? code);

        /// <summary>Create or update a time sheet definition plus its selected space zones.</summary>
        Task<TimeSheet> Save(TimeSheetSaveDto dto);

        /// <summary>Run the saved settings now and return the hours-worked report rows. Null if the definition is missing.</summary>
        Task<List<TimeSheetReportRowDto>?> RunReport(int code);
    }
}
