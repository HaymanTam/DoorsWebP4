using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;
using Microsoft.AspNetCore.Mvc;

namespace DoorsWeb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _service;

        public EventController(IEventService service)
        {
            _service = service;
        }

        // Streams the most-recent events as a JSON array so the client can report
        // incremental load progress while the response is being read. Optional from/to
        // restrict the result to a date range.
        [HttpGet]
        public IAsyncEnumerable<EventDto> GetAll([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            return _service.GetAll(from, to);
        }

        // Total number of events GetAll will stream for the same filter (capped), so the
        // client can show "X of N".
        [HttpGet("count")]
        public async Task<ActionResult<int>> GetCount([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            return Ok(await _service.GetCount(from, to));
        }
    }
}
