using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DoorsWeb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccessLevelHeaderController : ControllerBase
    {
        private readonly IAccessLevelHeaderService _service;

        public AccessLevelHeaderController(IAccessLevelHeaderService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<TAccessLevelHeader>>> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        // Route key order: { Site, AccessLevel } (EF composite key is { AccessLevel, Site })
        [HttpGet("{site}/{accessLevel}")]
        public async Task<ActionResult<TAccessLevelHeader>> GetById(int site, int accessLevel)
        {
            var result = await _service.GetById(site, accessLevel);
            if (result is null)
            {
                return Problem(detail: $"Access Level Header <{site}/{accessLevel}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<List<TAccessLevelHeader>>> Create(TAccessLevelHeader entity)
        {
            return Ok(await _service.Create(entity));
        }

        [HttpPut("{site}/{accessLevel}")]
        public async Task<ActionResult<List<TAccessLevelHeader>?>> Update(int site, int accessLevel, TAccessLevelHeader entity)
        {
            var result = await _service.Update(site, accessLevel, entity);
            if (result is null)
            {
                return Problem(detail: $"Update Failed! Access Level Header <{site}/{accessLevel}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        [HttpDelete("{site}/{accessLevel}")]
        public async Task<ActionResult<List<TAccessLevelHeader>?>> Delete(int site, int accessLevel)
        {
            var result = await _service.Delete(site, accessLevel);
            if (result is null)
            {
                return Problem(detail: $"Delete Failed! Access Level Header <{site}/{accessLevel}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }
    }
}
