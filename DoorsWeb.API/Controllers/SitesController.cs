using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;
using Microsoft.AspNetCore.Mvc;

namespace DoorsWeb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SitesController : ControllerBase
    {
        private readonly ISiteService _service;

        public SitesController(ISiteService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<SiteDto>>> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        // Adds a site. Only Name is used; the id is server-assigned.
        [HttpPost]
        public async Task<ActionResult<SiteDto>> Create(SiteDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return Problem(detail: "Site name is required.", title: "Invalid Site", statusCode: 400);
            }

            var created = await _service.Create(dto.Name);
            return Ok(created);
        }

        [HttpDelete("{site}")]
        public async Task<IActionResult> Delete(int site)
        {
            var removed = await _service.Delete(site);
            if (!removed)
            {
                return Problem(detail: $"Site <{site}> was not found.", title: "Not Found", statusCode: 404);
            }
            return NoContent();
        }
    }
}
