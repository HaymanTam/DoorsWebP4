using DoorsWeb.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DoorsWeb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccessLevelController : ControllerBase
    {
        private readonly IAccessLevelService _accessLevelService;

        public AccessLevelController(IAccessLevelService accessLevelService)
        {
            _accessLevelService = accessLevelService;
        }

        [HttpGet]
        public async Task<ActionResult<List<AccessLevelDto>>> GetAllAccessLevel()
        {
            var result = await _accessLevelService.GetAllAccessLevel();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<AccessLevelDto>>> GetAccessLevelById(Guid id)
        {
            var result = await _accessLevelService.GetAccessLevelById(id);
            if (result is null)
            {
                return Problem(detail: $"Access Level Id <{id}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<List<AccessLevelDto>>> CreateAccessLevel(AccessLevelDto accessLevelDto)
        {
            var result = await _accessLevelService.CreateAccessLevel(accessLevelDto);
            return Ok(result);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<List<AccessLevelDto>?>> UpdateAccessLevel(Guid id, AccessLevelDto accessLevelDto)
        {
            var result = await _accessLevelService.UpdateAccessLevel(id, accessLevelDto);
            if (result is null)
            {
                return Problem(detail: $"Update Failed! Access Level Id <{id}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<List<AccessLevelDto>?>> DeleteAccessLevelById(Guid id)
        {
            var result = await _accessLevelService.DeleteAccessLevelById(id);
            if (result is null)
            {
                return Problem(detail: $"Delete Failed! Access Level Id <{id}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }
    }
}