using DoorsWeb.API.Authorization;
using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoorsWeb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = AreaPolicies.SiteSettingsRead)]
    public class SystemSettingsController : ControllerBase
    {
        private readonly ISystemSettingsService _service;

        public SystemSettingsController(ISystemSettingsService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<SystemSettings> Get() => Ok(_service.Get());

        [Authorize(Policy = AreaPolicies.SiteSettingsWrite)]
        [HttpPut]
        public ActionResult<SystemSettings> Update(SystemSettings settings)
        {
            if (settings is null)
            {
                return Problem(detail: "Settings payload is required.", title: "Invalid Settings", statusCode: 400);
            }

            var cc = settings.ControllerCommunication ??= new ControllerCommunicationSettings();
            if (cc.PingIntervalSeconds < 1 || cc.CommandCheckIntervalSeconds < 1 || cc.ForcePingAfterCommands < 1)
            {
                return Problem(
                    detail: "Ping interval, command-check interval and force-ping count must each be at least 1.",
                    title: "Invalid Settings", statusCode: 400);
            }

            return Ok(_service.Save(settings));
        }
    }
}
