using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;
using DoorsWeb.Shared.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DoorsWeb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TriggersHeaderController : ControllerBase
    {
        private readonly ITriggersHeaderService _service;

        public TriggersHeaderController(ITriggersHeaderService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<TTriggersHeader>>> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TTriggersHeader>> GetById(int id)
        {
            var result = await _service.GetById(id);
            if (result is null)
            {
                return Problem(detail: $"Trigger Header <{id}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        // Source picker for the editor. code omitted = new. triggerType 1 = Door, 3 = Space Zone.
        [HttpGet("edit/{site}/{triggerType}")]
        public async Task<ActionResult<TriggerSaveDto>> GetForEdit(int site, int triggerType, [FromQuery] int? code)
        {
            return Ok(await _service.GetForEdit(site, triggerType, code));
        }

        [HttpPost]
        public async Task<ActionResult<List<TTriggersHeader>>> Create(TTriggersHeader entity)
        {
            return Ok(await _service.Create(entity));
        }

        [HttpPost("save")]
        public async Task<ActionResult<TTriggersHeader>> Save(TriggerSaveDto dto)
        {
            return Ok(await _service.Save(dto));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<List<TTriggersHeader>?>> Update(int id, TTriggersHeader entity)
        {
            var result = await _service.Update(id, entity);
            if (result is null)
            {
                return Problem(detail: $"Update Failed! Trigger Header <{id}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<TTriggersHeader>?>> Delete(int id)
        {
            var result = await _service.Delete(id);
            if (result is null)
            {
                return Problem(detail: $"Delete Failed! Trigger Header <{id}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }
    }
}
