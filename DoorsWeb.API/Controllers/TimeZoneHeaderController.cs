using DoorsWeb.API.Authorization;
using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;
using DoorsWeb.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoorsWeb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = AreaPolicies.SiteSettingsRead)]
    public class TimeZoneHeaderController : ControllerBase
    {
        private readonly ITimeZoneHeaderService _service;

        public TimeZoneHeaderController(ITimeZoneHeaderService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<TimeZones>>> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        // Route key order: { Site, TimeZone } (EF composite key is { TimeZone, Site })
        [HttpGet("{site}/{timeZone}")]
        public async Task<ActionResult<TimeZones>> GetById(int site, int timeZone)
        {
            var result = await _service.GetById(site, timeZone);
            if (result is null)
            {
                return Problem(detail: $"Time Zone Header <{site}/{timeZone}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        [HttpGet("{site}/{timeZone}/full")]
        public async Task<ActionResult<TimeZoneSaveDto>> GetWithElements(int site, int timeZone)
        {
            var result = await _service.GetWithElements(site, timeZone);
            if (result is null)
            {
                return Problem(detail: $"Time Zone Header <{site}/{timeZone}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        [Authorize(Policy = AreaPolicies.SiteSettingsWrite)]
        [HttpPost]
        public async Task<ActionResult<List<TimeZones>>> Create(TimeZones entity)
        {
            return Ok(await _service.Create(entity));
        }

        [Authorize(Policy = AreaPolicies.SiteSettingsWrite)]
        [HttpPost("save")]
        public async Task<ActionResult<TimeZones>> Save(TimeZoneSaveDto dto)
        {
            return Ok(await _service.Save(dto));
        }

        [Authorize(Policy = AreaPolicies.SiteSettingsWrite)]
        [HttpPut("{site}/{timeZone}")]
        public async Task<ActionResult<List<TimeZones>?>> Update(int site, int timeZone, TimeZones entity)
        {
            var result = await _service.Update(site, timeZone, entity);
            if (result is null)
            {
                return Problem(detail: $"Update Failed! Time Zone Header <{site}/{timeZone}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        [Authorize(Policy = AreaPolicies.SiteSettingsWrite)]
        [HttpDelete("{site}/{timeZone}")]
        public async Task<ActionResult<List<TimeZones>?>> Delete(int site, int timeZone)
        {
            var result = await _service.Delete(site, timeZone);
            if (result is null)
            {
                return Problem(detail: $"Delete Failed! Time Zone Header <{site}/{timeZone}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }
    }
}
