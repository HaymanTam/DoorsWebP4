using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface IAuditService
    {
        Task<List<AuditLogDto>> GetAll();
    }
}
