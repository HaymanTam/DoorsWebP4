using DoorsWeb.API.Authorization;
using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoorsWeb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = AreaPolicies.SiteSettingsRead)]
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
        [Authorize(Policy = AreaPolicies.SiteSettingsWrite)]
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

        // Renames a site. Only Name is used; the route id identifies the site.
        [Authorize(Policy = AreaPolicies.SiteSettingsWrite)]
        [HttpPut("{site}")]
        public async Task<ActionResult<SiteDto>> Rename(int site, SiteDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return Problem(detail: "Site name is required.", title: "Invalid Site", statusCode: 400);
            }

            var updated = await _service.Rename(site, dto.Name);
            if (updated is null)
            {
                return Problem(detail: $"Site <{site}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(updated);
        }

        [Authorize(Policy = AreaPolicies.SiteSettingsWrite)]
        [HttpDelete("{site}")]
        public async Task<IActionResult> Delete(int site)
        {
            try
            {
                var removed = await _service.Delete(site);
                if (!removed)
                {
                    return Problem(detail: $"Site <{site}> was not found.", title: "Not Found", statusCode: 404);
                }
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                // Last-site protection: at least one site must always exist.
                return Problem(detail: ex.Message, title: "Cannot delete site", statusCode: 409);
            }
        }
    }
}
