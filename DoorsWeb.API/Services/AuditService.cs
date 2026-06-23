using System.Security.Claims;
using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;
using DoorsWeb.Shared.Entities;
using Microsoft.IdentityModel.JsonWebTokens;

namespace DoorsWeb.API.Services
{
    public class AuditService : IAuditService
    {
        private readonly DoorsEnterpriseContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuditService> _logger;

        public AuditService(
            DoorsEnterpriseContext context,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AuditService> logger)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<List<AuditLogDto>> GetAll()
        {
            return await _context.Audit
                .AsNoTracking()
                .OrderByDescending(a => a.Timestamp)
                .Select(a => new AuditLogDto
                {
                    Id = a.Id,
                    Timestamp = a.Timestamp,
                    UserName = a.UserName,
                    ClientIp = a.ClientIp,
                    Action = a.Action,
                    EntityType = a.EntityType,
                    EntityKey = a.EntityKey,
                    EntityName = a.EntityName
                })
                .ToListAsync();
        }

        public async Task LogAsync(AuditAction action, string entityType, string? entityKey, string? entityName)
        {
            try
            {
                var entry = new Audit
                {
                    // T_Audit.Timestamp is "timestamp without time zone" (like every legacy date
                    // column). Npgsql rejects a Kind=Utc DateTime against that type, so use local
                    // time — the same convention the rest of the app persists with (e.g. DoorService)
                    // and what the Audit page displays as-is, keeping all UI timestamps consistent.
                    Timestamp = DateTime.Now,
                    UserName = ResolveUserName(),
                    ClientIp = ResolveClientIp(),
                    Action = action.ToString(),
                    EntityType = entityType,
                    EntityKey = Truncate(entityKey, 50),
                    EntityName = Truncate(entityName, 200)
                };

                _context.Audit.Add(entry);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Auditing must never break the operation the user actually performed.
                _logger.LogWarning(ex,
                    "Failed to write audit entry ({Action} {EntityType} {EntityKey}).",
                    action, entityType, entityKey);
            }
        }

        /// <summary>Login name from the JWT, handling default inbound-claim mapping of "sub".</summary>
        private string ResolveUserName()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user is null) return "system";

            return user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value     // unmapped "sub"
                ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value         // mapped "sub"
                ?? user.Identity?.Name
                ?? "unknown";
        }

        private string? ResolveClientIp()
        {
            return _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
        }

        private static string? Truncate(string? value, int maxLength)
            => value is { Length: var len } && len > maxLength ? value[..maxLength] : value;
    }
}
