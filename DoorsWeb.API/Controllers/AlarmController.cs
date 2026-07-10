using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DoorsWeb.API.Authorization;
using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoorsWeb.API.Controllers
{
    /// <summary>
    /// The alarm log shown on the Alarms page. Reading is open to any authenticated user (the nav
    /// entry is too); actioning an alarm is a site-monitoring write, so it requires Site Settings write.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AlarmController : ControllerBase
    {
        private readonly IAlarmService _service;

        public AlarmController(IAlarmService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<AlarmListDto>>> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        /// <summary>Marks an alarm as actioned, recording the current operator and their optional note.</summary>
        [Authorize(Policy = AreaPolicies.SiteSettingsWrite)]
        [HttpPost("{code:int}/action")]
        public async Task<IActionResult> Action(int code, AlarmActionRequest request)
        {
            var actionedBy = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                ?? "unknown";

            return await _service.ActionAsync(code, actionedBy, request?.Note)
                ? NoContent()
                : Problem(detail: $"Alarm <{code}> was not found.", title: "Not Found", statusCode: 404);
        }
    }
}
