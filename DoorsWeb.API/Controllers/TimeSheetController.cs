using DoorsWeb.API.Authorization;
using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;
using DoorsWeb.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoorsWeb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = AreaPolicies.CardManagerRead)]
    public class TimeSheetController : ControllerBase
    {
        private readonly ITimeSheetService _service;

        public TimeSheetController(ITimeSheetService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<TimeSheet>>> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TimeSheet>> GetById(int id)
        {
            var result = await _service.GetById(id);
            if (result is null)
            {
                return Problem(detail: $"Time Sheet Header <{id}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        // Editor payload: header + selected zones + all available zones. code omitted = new definition.
        [HttpGet("edit")]
        public async Task<ActionResult<TimeSheetSaveDto>> GetForEdit([FromQuery] int? code)
        {
            return Ok(await _service.GetForEdit(code));
        }

        // Run the saved settings now and return the hours-worked report.
        [HttpGet("{id}/run")]
        public async Task<ActionResult<List<TimeSheetReportRowDto>>> Run(int id)
        {
            var result = await _service.RunReport(id);
            if (result is null)
            {
                return Problem(detail: $"Time Sheet Header <{id}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        [Authorize(Policy = AreaPolicies.CardManagerWrite)]
        [HttpPost]
        public async Task<ActionResult<List<TimeSheet>>> Create(TimeSheet entity)
        {
            return Ok(await _service.Create(entity));
        }

        [Authorize(Policy = AreaPolicies.CardManagerWrite)]
        [HttpPost("save")]
        public async Task<ActionResult<TimeSheet>> Save(TimeSheetSaveDto dto)
        {
            return Ok(await _service.Save(dto));
        }

        [Authorize(Policy = AreaPolicies.CardManagerWrite)]
        [HttpPut("{id}")]
        public async Task<ActionResult<List<TimeSheet>?>> Update(int id, TimeSheet entity)
        {
            var result = await _service.Update(id, entity);
            if (result is null)
            {
                return Problem(detail: $"Update Failed! Time Sheet Header <{id}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        [Authorize(Policy = AreaPolicies.CardManagerWrite)]
        [HttpDelete("{id}")]
        public async Task<ActionResult<List<TimeSheet>?>> Delete(int id)
        {
            var result = await _service.Delete(id);
            if (result is null)
            {
                return Problem(detail: $"Delete Failed! Time Sheet Header <{id}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }
    }
}
