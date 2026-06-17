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

        // Composite key: { AccessLevel, Site }
        [HttpGet("{accessLevel}/{site}")]
        public async Task<ActionResult<TAccessLevelHeader>> GetById(int accessLevel, int site)
        {
            var result = await _service.GetById(accessLevel, site);
            if (result is null)
            {
                return Problem(detail: $"Access Level Header <{accessLevel}/{site}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<List<TAccessLevelHeader>>> Create(TAccessLevelHeader entity)
        {
            return Ok(await _service.Create(entity));
        }

        [HttpPut("{accessLevel}/{site}")]
        public async Task<ActionResult<List<TAccessLevelHeader>?>> Update(int accessLevel, int site, TAccessLevelHeader entity)
        {
            var result = await _service.Update(accessLevel, site, entity);
            if (result is null)
            {
                return Problem(detail: $"Update Failed! Access Level Header <{accessLevel}/{site}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        [HttpDelete("{accessLevel}/{site}")]
        public async Task<ActionResult<List<TAccessLevelHeader>?>> Delete(int accessLevel, int site)
        {
            var result = await _service.Delete(accessLevel, site);
            if (result is null)
            {
                return Problem(detail: $"Delete Failed! Access Level Header <{accessLevel}/{site}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }
    }
}
