using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services
{
    public class AuditService : IAuditService
    {
        private readonly DoorsEnterpriseContext _context;

        public AuditService(DoorsEnterpriseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// All card-change audit records, newest first. Mirrors the legacy
        /// sp_Audit_Read (ORDER BY SaveDate DESC); the audit table is append-only and read here only.
        /// </summary>
        public async Task<List<AuditLogDto>> GetAll()
        {
            return await _context.Audit
                .AsNoTracking()
                .OrderByDescending(a => a.SaveDate)
                .Select(a => new AuditLogDto
                {
                    Id = a.Id,
                    CardId = a.CardId,
                    SaveDate = a.SaveDate,
                    SavedBy = a.SavedBy,
                    Workstation = a.Workstation,
                    FirstName = a.Forename,
                    LastName = a.Surname,
                    Enabled = a.Enabled,
                    AccessLevels = a.AccessLevels
                })
                .ToListAsync();
        }
    }
}
