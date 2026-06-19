using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;
using DoorsWeb.Shared.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DoorsWeb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalendarHeaderController : ControllerBase
    {
        private readonly ICalendarHeaderService _service;

        public CalendarHeaderController(ICalendarHeaderService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<Calendar>>> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Calendar>> GetById(int id)
        {
            var result = await _service.GetById(id);
            if (result is null)
            {
                return Problem(detail: $"Calendar Header <{id}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        [HttpGet("{id}/full")]
        public async Task<ActionResult<CalendarSaveDto>> GetWithHolidays(int id)
        {
            var result = await _service.GetWithHolidays(id);
            if (result is null)
            {
                return Problem(detail: $"Calendar Header <{id}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<List<Calendar>>> Create(Calendar entity)
        {
            return Ok(await _service.Create(entity));
        }

        [HttpPost("save")]
        public async Task<ActionResult<Calendar>> Save(CalendarSaveDto dto)
        {
            return Ok(await _service.Save(dto));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<List<Calendar>?>> Update(int id, Calendar entity)
        {
            var result = await _service.Update(id, entity);
            if (result is null)
            {
                return Problem(detail: $"Update Failed! Calendar Header <{id}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<Calendar>?>> Delete(int id)
        {
            var result = await _service.Delete(id);
            if (result is null)
            {
                return Problem(detail: $"Delete Failed! Calendar Header <{id}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }
    }
}
