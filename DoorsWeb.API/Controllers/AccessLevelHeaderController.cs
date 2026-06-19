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
    public class AccessLevelHeaderController : ControllerBase
    {
        private readonly IAccessLevelHeaderService _service;

        public AccessLevelHeaderController(IAccessLevelHeaderService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<AccessLevels>>> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        // Route key order: { Site, AccessLevel } (EF composite key is { AccessLevel, Site })
        [HttpGet("{site}/{accessLevel}")]
        public async Task<ActionResult<AccessLevels>> GetById(int site, int accessLevel)
        {
            var result = await _service.GetById(site, accessLevel);
            if (result is null)
            {
                return Problem(detail: $"Access Level Header <{site}/{accessLevel}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        // Door picker for the editor. accessLevel omitted = new (all doors unselected).
        [HttpGet("{site}/doors")]
        public async Task<ActionResult<AccessLevelSaveDto>> GetForEdit(int site, [FromQuery] int? accessLevel)
        {
            return Ok(await _service.GetForEdit(site, accessLevel));
        }

        [Authorize(Policy = AreaPolicies.SiteSettingsWrite)]
        [HttpPost]
        public async Task<ActionResult<List<AccessLevels>>> Create(AccessLevels entity)
        {
            return Ok(await _service.Create(entity));
        }

        [Authorize(Policy = AreaPolicies.SiteSettingsWrite)]
        [HttpPost("save")]
        public async Task<ActionResult<AccessLevels>> Save(AccessLevelSaveDto dto)
        {
            return Ok(await _service.Save(dto));
        }

        [Authorize(Policy = AreaPolicies.SiteSettingsWrite)]
        [HttpPut("{site}/{accessLevel}")]
        public async Task<ActionResult<List<AccessLevels>?>> Update(int site, int accessLevel, AccessLevels entity)
        {
            var result = await _service.Update(site, accessLevel, entity);
            if (result is null)
            {
                return Problem(detail: $"Update Failed! Access Level Header <{site}/{accessLevel}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        [Authorize(Policy = AreaPolicies.SiteSettingsWrite)]
        [HttpDelete("{site}/{accessLevel}")]
        public async Task<ActionResult<List<AccessLevels>?>> Delete(int site, int accessLevel)
        {
            var result = await _service.Delete(site, accessLevel);
            if (result is null)
            {
                return Problem(detail: $"Delete Failed! Access Level Header <{site}/{accessLevel}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }
    }
}
