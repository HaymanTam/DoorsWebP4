using DoorsWeb.API.Authorization;
using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoorsWeb.API.Controllers
{
    /// <summary>
    /// Cardholder photos, stored on disk keyed by card number (see <see cref="Services.CardPhotoService"/>).
    /// The Card Record dialog uploads here after the cardholder is saved, and reads the path back to
    /// show the existing photo. The image bytes are served separately as static files at
    /// <c>/media/card-photo</c>.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = AreaPolicies.CardManagerRead)]
    public class CardPhotoController : ControllerBase
    {
        private readonly ICardPhotoService _photos;

        public CardPhotoController(ICardPhotoService photos)
        {
            _photos = photos;
        }

        // Returns the card's photo path, or 204 when it has none.
        [HttpGet("{cardNumber:int}")]
        public ActionResult<PhotoUploadResult> Get(int cardNumber)
        {
            var path = _photos.GetPath(cardNumber);
            if (path is null) return NoContent();
            return Ok(new PhotoUploadResult { FileName = System.IO.Path.GetFileName(path), Path = path });
        }

        // Uploads / replaces the card's photo (multipart/form-data, field name "file").
        [Authorize(Policy = AreaPolicies.CardManagerWrite)]
        [HttpPost("{cardNumber:int}")]
        [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB
        public async Task<ActionResult<PhotoUploadResult>> Upload(int cardNumber, IFormFile file)
        {
            if (file is null || file.Length == 0)
            {
                return Problem(detail: "No file was uploaded.", title: "Upload Failed", statusCode: 400);
            }

            try
            {
                await using var stream = file.OpenReadStream();
                var result = await _photos.SaveAsync(cardNumber, stream, file.FileName);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Problem(detail: ex.Message, title: "Upload Failed", statusCode: 400);
            }
        }

        // Removes the card's photo.
        [Authorize(Policy = AreaPolicies.CardManagerWrite)]
        [HttpDelete("{cardNumber:int}")]
        public IActionResult Delete(int cardNumber)
        {
            return _photos.Delete(cardNumber) ? NoContent() : NotFound();
        }
    }
}
