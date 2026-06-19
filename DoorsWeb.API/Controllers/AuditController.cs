using DoorsWeb.API.Authorization;
using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoorsWeb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = AreaPolicies.UserSettingsRead)]
    public class AuditController : ControllerBase
    {
        private readonly IAuditService _service;

        public AuditController(IAuditService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<AuditLogDto>>> GetAll()
        {
            return Ok(await _service.GetAll());
        }
    }
}
