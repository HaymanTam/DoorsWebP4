using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DoorsWeb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeSheetHeaderController : ControllerBase
    {
        private readonly ITimeSheetHeaderService _service;

        public TimeSheetHeaderController(ITimeSheetHeaderService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<TimeSheet>>> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TimeSheet>> GetById(int id)
        {
            var result = await _service.GetById(id);
            if (result is null)
            {
                return Problem(detail: $"Time Sheet Header <{id}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<List<TimeSheet>>> Create(TimeSheet entity)
        {
            return Ok(await _service.Create(entity));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<List<TimeSheet>?>> Update(int id, TimeSheet entity)
        {
            var result = await _service.Update(id, entity);
            if (result is null)
            {
                return Problem(detail: $"Update Failed! Time Sheet Header <{id}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<TimeSheet>?>> Delete(int id)
        {
            var result = await _service.Delete(id);
            if (result is null)
            {
                return Problem(detail: $"Delete Failed! Time Sheet Header <{id}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }
    }
}
