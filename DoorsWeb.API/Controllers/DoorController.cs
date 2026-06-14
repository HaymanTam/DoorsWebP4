using DoorsWeb.API.Services;
using DoorsWeb.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DoorsWeb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoorController : ControllerBase
    {
        private readonly IDoorService _doorService;

        public DoorController(IDoorService doorService)
        {
            _doorService = doorService;
        }

        [HttpGet]
        public async Task<ActionResult<List<DoorListDto>>> GetAllDoors()
        {
            var result = await _doorService.GetAllDoors();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DoorDetailDto?>> GetDoorById(Guid id)
        {
            var result = await _doorService.GetDoorById(id);
            if (result is null)
            {
                return NotFound($"Get Failed! Door Id <{id}> was not found.");
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<List<DoorListDto>>> CreateDoor(DoorDetailDto doorCreateRequest)
        {
            var result = await _doorService.CreateDoor(doorCreateRequest);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<List<DoorListDto>?>> UpdateDoor(Guid id, DoorDetailDto doorCreateRequest)
        {
            var result = await _doorService.UpdateDoor(id, doorCreateRequest);
            if (result is null)
            {
                return NotFound($"Update Failed! Door Id <{id}> was not found.");
            }
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<List<DoorListDto>?>> DeleteDoorById(Guid id)
        {
            var result = await _doorService.DeleteDoorById(id);
            if (result is null)
            {
                return NotFound($"Delete Failed! Door Id <{id}> was not found.");
            }
            return Ok(result);
        }
    }
}
