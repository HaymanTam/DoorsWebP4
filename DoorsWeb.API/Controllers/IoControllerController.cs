using DoorsWeb.API.Authorization;
using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoorsWeb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = AreaPolicies.SiteSettingsRead)]
    public class IoControllerController : ControllerBase
    {
        private readonly IIoControllerService _service;

        public IoControllerController(IIoControllerService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<IoController>>> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IoController>> GetById(int id)
        {
            var result = await _service.GetById(id);
            if (result is null)
            {
                return Problem(detail: $"IO Controller Header <{id}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        [Authorize(Policy = AreaPolicies.SiteSettingsWrite)]
        [HttpPost]
        public async Task<ActionResult<List<IoController>>> Create(IoController entity)
        {
            return Ok(await _service.Create(entity));
        }

        [Authorize(Policy = AreaPolicies.SiteSettingsWrite)]
        [HttpPut("{id}")]
        public async Task<ActionResult<List<IoController>?>> Update(int id, IoController entity)
        {
            var result = await _service.Update(id, entity);
            if (result is null)
            {
                return Problem(detail: $"Update Failed! IO Controller Header <{id}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        [Authorize(Policy = AreaPolicies.SiteSettingsWrite)]
        [HttpDelete("{id}")]
        public async Task<ActionResult<List<IoController>?>> Delete(int id)
        {
            var result = await _service.Delete(id);
            if (result is null)
            {
                return Problem(detail: $"Delete Failed! IO Controller Header <{id}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }
    }
}
