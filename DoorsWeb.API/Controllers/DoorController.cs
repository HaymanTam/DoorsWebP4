using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;
using Microsoft.AspNetCore.Mvc;

namespace DoorsWeb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoorController : ControllerBase
    {
        private readonly IDoorService _service;

        public DoorController(IDoorService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<DoorListDto>>> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DoorDetailDto>> GetById(int id)
        {
            var result = await _service.GetById(id);
            if (result is null)
            {
                return Problem(detail: $"Door <{id}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<List<DoorListDto>>> Create(DoorDetailDto dto)
        {
            return Ok(await _service.Create(dto));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<List<DoorListDto>?>> Update(int id, DoorDetailDto dto)
        {
            var result = await _service.Update(id, dto);
            if (result is null)
            {
                return Problem(detail: $"Update Failed! Door <{id}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<DoorListDto>?>> Delete(int id)
        {
            var result = await _service.Delete(id);
            if (result is null)
            {
                return Problem(detail: $"Delete Failed! Door <{id}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }
    }
}
