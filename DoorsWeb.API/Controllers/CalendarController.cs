using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;
using Microsoft.AspNetCore.Mvc;

namespace DoorsWeb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly ICalendarService _calendarService;

        public CalendarController(ICalendarService calendarService)
        {
            _calendarService = calendarService;
        }

        [HttpGet]
        public async Task<ActionResult<List<AccessCalendarDto>>> GetAllCalendars()
        {
            var result = await _calendarService.GetAllCalendars();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AccessCalendarDto>> GetCalendarById(Guid id)
        {
            var result = await _calendarService.GetCalendarById(id);
            if (result is null)
                return Problem(detail: $"Calendar Id <{id}> was not found.", title: "Not Found", statusCode: 404);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<List<AccessCalendarDto>>> CreateCalendar(AccessCalendarDto dto)
        {
            var result = await _calendarService.CreateCalendar(dto);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<List<AccessCalendarDto>>> UpdateCalendar(Guid id, AccessCalendarDto dto)
        {
            var result = await _calendarService.UpdateCalendar(id, dto);
            if (result is null)
                return Problem(detail: $"Update Failed! Calendar Id <{id}> was not found.", title: "Not Found", statusCode: 404);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<AccessCalendarDto>>> DeleteCalendarById(Guid id)
        {
            var result = await _calendarService.DeleteCalendarById(id);
            if (result is null)
                return Problem(detail: $"Delete Failed! Calendar Id <{id}> was not found.", title: "Not Found", statusCode: 404);
            return Ok(result);
        }
    }
}
