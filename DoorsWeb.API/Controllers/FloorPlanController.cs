using DoorsWeb.API.Authorization;
using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoorsWeb.API.Controllers
{
    /// <summary>
    /// The optional spatial layout for a site's floorplan: the background image and the placed
    /// door markers. The live states themselves come from DoorControl/EventHub; this only stores
    /// where each door sits on the picture.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = AreaPolicies.SiteSettingsRead)]
    public class FloorPlanController : ControllerBase
    {
        private readonly IFloorPlanService _service;

        public FloorPlanController(IFloorPlanService service)
        {
            _service = service;
        }

        /// <summary>Gets a site's saved layout (empty when none has been created).</summary>
        [HttpGet("{site:int}")]
        public ActionResult<FloorPlanLayoutDto> Get(int site)
            => Ok(_service.Get(site));

        /// <summary>Saves a site's layout (door placements + image reference).</summary>
        [Authorize(Policy = AreaPolicies.SiteSettingsWrite)]
        [HttpPut("{site:int}")]
        public ActionResult<FloorPlanLayoutDto> Save(int site, FloorPlanLayoutDto layout)
        {
            layout.Site = site;
            return Ok(_service.Save(layout));
        }

        /// <summary>Uploads a background image for a site (multipart/form-data, field "file").</summary>
        [Authorize(Policy = AreaPolicies.SiteSettingsWrite)]
        [HttpPost("{site:int}/image")]
        [RequestSizeLimit(15 * 1024 * 1024)] // 15 MB
        public async Task<ActionResult<FloorPlanLayoutDto>> UploadImage(int site, IFormFile file, CancellationToken ct)
        {
            if (file is null || file.Length == 0)
                return Problem(detail: "No file was uploaded.", title: "Upload Failed", statusCode: 400);

            try
            {
                await using var stream = file.OpenReadStream();
                var layout = await _service.SaveImageAsync(site, stream, file.FileName, ct);
                return Ok(layout);
            }
            catch (InvalidOperationException ex)
            {
                return Problem(detail: ex.Message, title: "Upload Failed", statusCode: 400);
            }
        }
    }
}
