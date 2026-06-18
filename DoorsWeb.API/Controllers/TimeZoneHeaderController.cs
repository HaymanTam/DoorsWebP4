using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;
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

        // Route key order: { Site, TimeZone } (EF composite key is { TimeZone, Site })
        [HttpGet("{site}/{timeZone}")]
        public async Task<ActionResult<TTimeZoneHeader>> GetById(int site, int timeZone)
        {
            var result = await _service.GetById(site, timeZone);
            if (result is null)
            {
                return Problem(detail: $"Time Zone Header <{site}/{timeZone}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        [HttpGet("{site}/{timeZone}/full")]
        public async Task<ActionResult<TimeZoneSaveDto>> GetWithElements(int site, int timeZone)
        {
            var result = await _service.GetWithElements(site, timeZone);
            if (result is null)
            {
                return Problem(detail: $"Time Zone Header <{site}/{timeZone}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<List<TTimeZoneHeader>>> Create(TTimeZoneHeader entity)
        {
            return Ok(await _service.Create(entity));
        }

        [HttpPost("save")]
        public async Task<ActionResult<TTimeZoneHeader>> Save(TimeZoneSaveDto dto)
        {
            return Ok(await _service.Save(dto));
        }

        [HttpPut("{site}/{timeZone}")]
        public async Task<ActionResult<List<TTimeZoneHeader>?>> Update(int site, int timeZone, TTimeZoneHeader entity)
        {
            var result = await _service.Update(site, timeZone, entity);
            if (result is null)
            {
                return Problem(detail: $"Update Failed! Time Zone Header <{site}/{timeZone}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        [HttpDelete("{site}/{timeZone}")]
        public async Task<ActionResult<List<TTimeZoneHeader>?>> Delete(int site, int timeZone)
        {
            var result = await _service.Delete(site, timeZone);
            if (result is null)
            {
                return Problem(detail: $"Delete Failed! Time Zone Header <{site}/{timeZone}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }
    }
}
