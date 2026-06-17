using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DoorsWeb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeZoneHeaderController : ControllerBase
    {
        private readonly ITimeZoneHeaderService _service;

        public TimeZoneHeaderController(ITimeZoneHeaderService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<TTimeZoneHeader>>> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        // Composite key: { TimeZone, Site }
        [HttpGet("{timeZone}/{site}")]
        public async Task<ActionResult<TTimeZoneHeader>> GetById(int timeZone, int site)
        {
            var result = await _service.GetById(timeZone, site);
            if (result is null)
            {
                return Problem(detail: $"Time Zone Header <{timeZone}/{site}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<List<TTimeZoneHeader>>> Create(TTimeZoneHeader entity)
        {
            return Ok(await _service.Create(entity));
        }

        [HttpPut("{timeZone}/{site}")]
        public async Task<ActionResult<List<TTimeZoneHeader>?>> Update(int timeZone, int site, TTimeZoneHeader entity)
        {
            var result = await _service.Update(timeZone, site, entity);
            if (result is null)
            {
                return Problem(detail: $"Update Failed! Time Zone Header <{timeZone}/{site}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        [HttpDelete("{timeZone}/{site}")]
        public async Task<ActionResult<List<TTimeZoneHeader>?>> Delete(int timeZone, int site)
        {
            var result = await _service.Delete(timeZone, site);
            if (result is null)
            {
                return Problem(detail: $"Delete Failed! Time Zone Header <{timeZone}/{site}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }
    }
}
