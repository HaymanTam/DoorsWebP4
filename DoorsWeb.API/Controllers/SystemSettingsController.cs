using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;
using Microsoft.AspNetCore.Mvc;

namespace DoorsWeb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemSettingsController : ControllerBase
    {
        private readonly IConnectorSettingsService _connectorSettings;

        public SystemSettingsController(IConnectorSettingsService connectorSettings)
        {
            _connectorSettings = connectorSettings;
        }

        // Global connector polling settings (ping / command / force-ping).
        [HttpGet("connector")]
        public async Task<ActionResult<ConnectorSettingsDto>> GetConnectorSettings()
        {
            return Ok(await _connectorSettings.GetAsync());
        }

        [HttpPut("connector")]
        public async Task<ActionResult<ConnectorSettingsDto>> SaveConnectorSettings(ConnectorSettingsDto settings)
        {
            return Ok(await _connectorSettings.SaveAsync(settings));
        }
    }
}
