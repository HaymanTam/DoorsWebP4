using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;
using DoorsWeb.Shared.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DoorsWeb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpaceZoneHeaderController : ControllerBase
    {
        private readonly ISpaceZoneHeaderService _service;

        public SpaceZoneHeaderController(ISpaceZoneHeaderService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<TSpaceZoneHeader>>> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TSpaceZoneHeader>> GetById(int id)
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

        [HttpPost]
        public async Task<ActionResult<List<TSpaceZoneHeader>>> Create(TSpaceZoneHeader entity)
        {
            return Ok(await _service.Create(entity));
        }

        [HttpPost("save")]
        public async Task<ActionResult<TSpaceZoneHeader>> Save(SpaceZoneSaveDto dto)
        {
            return Ok(await _service.Save(dto));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<List<TSpaceZoneHeader>?>> Update(int id, TSpaceZoneHeader entity)
        {
            var result = await _service.Update(id, entity);
            if (result is null)
            {
                return Problem(detail: $"Update Failed! Space Zone Header <{id}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<TSpaceZoneHeader>?>> Delete(int id)
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
