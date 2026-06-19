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
    public class SpaceZoneController : ControllerBase
    {
        private readonly ISpaceZoneService _service;

        public SpaceZoneController(ISpaceZoneService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<SpaceZone>>> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SpaceZone>> GetById(int id)
        {
            var result = await _service.GetById(id);
            if (result is null)
            {
                return Problem(detail: $"Space Zone Header <{id}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        // Door picker for the editor. zone omitted = new (all doors unincluded).
        [HttpGet("{site}/doors")]
        public async Task<ActionResult<SpaceZoneSaveDto>> GetForEdit(int site, [FromQuery] int? zone)
        {
            return Ok(await _service.GetForEdit(site, zone));
        }

        [Authorize(Policy = AreaPolicies.SiteSettingsWrite)]
        [HttpPost]
        public async Task<ActionResult<List<SpaceZone>>> Create(SpaceZone entity)
        {
            return Ok(await _service.Create(entity));
        }

        [Authorize(Policy = AreaPolicies.SiteSettingsWrite)]
        [HttpPost("save")]
        public async Task<ActionResult<SpaceZone>> Save(SpaceZoneSaveDto dto)
        {
            return Ok(await _service.Save(dto));
        }

        [Authorize(Policy = AreaPolicies.SiteSettingsWrite)]
        [HttpPut("{id}")]
        public async Task<ActionResult<List<SpaceZone>?>> Update(int id, SpaceZone entity)
        {
            var result = await _service.Update(id, entity);
            if (result is null)
            {
                return Problem(detail: $"Update Failed! Space Zone Header <{id}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        [Authorize(Policy = AreaPolicies.SiteSettingsWrite)]
        [HttpDelete("{id}")]
        public async Task<ActionResult<List<SpaceZone>?>> Delete(int id)
        {
            var result = await _service.Delete(id);
            if (result is null)
            {
                return Problem(detail: $"Delete Failed! Space Zone Header <{id}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }
    }
}
