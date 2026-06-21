using DoorsWeb.API.Authorization;
using DoorsWeb.API.Licensing;
using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoorsWeb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = AreaPolicies.SiteSettingsRead)]
    public class DoorController : ControllerBase
    {
        private readonly IDoorService _service;

        public DoorController(IDoorService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<DoorListDto>>> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DoorDetailDto>> GetById(int id)
        {
            var result = await _service.GetById(id);
            if (result is null)
            {
                return Problem(detail: $"Door <{id}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        [Authorize(Policy = AreaPolicies.SiteSettingsWrite)]
        [HttpPost]
        public async Task<ActionResult<List<DoorListDto>>> Create(DoorDetailDto dto)
        {
            try
            {
                return Ok(await _service.Create(dto));
            }
            catch (LicenseLimitException ex)
            {
                return Problem(detail: ex.Message, title: "License Limit Reached", statusCode: 409);
            }
        }

        [Authorize(Policy = AreaPolicies.SiteSettingsWrite)]
        [HttpPut("{id}")]
        public async Task<ActionResult<List<DoorListDto>?>> Update(int id, DoorDetailDto dto)
        {
            var result = await _service.Update(id, dto);
            if (result is null)
            {
                return Problem(detail: $"Update Failed! Door <{id}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        [Authorize(Policy = AreaPolicies.SiteSettingsWrite)]
        [HttpDelete("{id}")]
        public async Task<ActionResult<List<DoorListDto>?>> Delete(int id)
        {
            var result = await _service.Delete(id);
            if (result is null)
            {
                return Problem(detail: $"Delete Failed! Door <{id}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }
    }
}
