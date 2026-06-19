using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;
using DoorsWeb.Shared.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DoorsWeb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApbzoneHeaderController : ControllerBase
    {
        private readonly IApbzoneHeaderService _service;

        public ApbzoneHeaderController(IApbzoneHeaderService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<ApbZone>>> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApbZone>> GetById(int id)
        {
            var result = await _service.GetById(id);
            if (result is null)
            {
                return Problem(detail: $"APB Zone Header <{id}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        // Door picker for the editor. apb omitted = new (all doors unincluded).
        [HttpGet("{site}/doors")]
        public async Task<ActionResult<ApbZoneSaveDto>> GetForEdit(int site, [FromQuery] int? apb)
        {
            return Ok(await _service.GetForEdit(site, apb));
        }

        [HttpPost]
        public async Task<ActionResult<List<ApbZone>>> Create(ApbZone entity)
        {
            return Ok(await _service.Create(entity));
        }

        [HttpPost("save")]
        public async Task<ActionResult<ApbZone>> Save(ApbZoneSaveDto dto)
        {
            return Ok(await _service.Save(dto));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<List<ApbZone>?>> Update(int id, ApbZone entity)
        {
            var result = await _service.Update(id, entity);
            if (result is null)
            {
                return Problem(detail: $"Update Failed! APB Zone Header <{id}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<ApbZone>?>> Delete(int id)
        {
            var result = await _service.Delete(id);
            if (result is null)
            {
                return Problem(detail: $"Delete Failed! APB Zone Header <{id}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }
    }
}
