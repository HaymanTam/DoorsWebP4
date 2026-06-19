using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services.Interfaces
{
    public interface IAuditService
    {
        /// <summary>All audit records, newest first.</summary>
        Task<List<AuditLogDto>> GetAll();

        /// <summary>
        /// Write one audit record for a change a user just made. The current user and client IP are
        /// resolved from the active HTTP request. Best-effort: failures are logged and swallowed so a
        /// problem writing the audit row can never roll back or break the change the user actually made.
        /// </summary>
        /// <param name="action">Create, Update or Delete.</param>
        /// <param name="entityType">Kind of record, e.g. "Cardholder", "Access Level", "Time Zone".</param>
        /// <param name="entityKey">Key of the record: card number for cardholders, otherwise id/code.</param>
        /// <param name="entityName">Human-readable name/description of the record.</param>
        Task LogAsync(AuditAction action, string entityType, string? entityKey, string? entityName);
    }
}
