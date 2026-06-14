using DoorsWeb.API.Services;
using DoorsWeb.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System;


namespace DoorsWeb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<EventListDto>>> GetEventById(Guid id)
        {
            var result = await _eventService.GetEventById(id);
            if (result is null)
            {
                return Problem(
                    title: "Resource Not Found",
                    detail: $"EventId <{id}> not found.",
                    statusCode: StatusCodes.Status404NotFound
                    );
            }
            return Ok(result);
        }
        [HttpGet]
        public async Task<ActionResult<List<EventListDto>>> GetEvents(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] int? pageSize,
            [FromQuery] int? pageNumber)

        {
            var result = await _eventService.GetEvents(startDate,endDate,pageSize,pageNumber);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<List<EventListDto>>> CreateEvent(EventCreateDto evtDto)
        {
            var result = await _eventService.CreateEvent(evtDto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<EventListDto>?>> DeleteEventById(Guid id)
        {
            var result = await _eventService.DeleteEventById(id);
            if (result is null)
            {
                return NotFound($"Delete Failed! Event Id <{id}> was not found.");
            }
            return Ok(result);
        }
    }
}
