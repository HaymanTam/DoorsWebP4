using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;
using Microsoft.AspNetCore.Mvc;

namespace DoorsWeb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlarmController : ControllerBase
    {
        private readonly IAlarmService _service;

        public AlarmController(IAlarmService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<AlarmListDto>>> GetAll()
        {
            return Ok(await _service.GetAll());
        }
    }
}
