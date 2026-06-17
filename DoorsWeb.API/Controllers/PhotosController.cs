using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;
using Microsoft.AspNetCore.Mvc;

namespace DoorsWeb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IPhotoStorageService _storage;

        public PhotosController(IPhotoStorageService storage)
        {
            _storage = storage;
        }

        // Uploads a user photo (multipart/form-data, field name "file").
        [HttpPost]
        [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB
        public async Task<ActionResult<PhotoUploadResult>> Upload(IFormFile file)
        {
            if (file is null || file.Length == 0)
            {
                return Problem(detail: "No file was uploaded.", title: "Upload Failed", statusCode: 400);
            }

            try
            {
                await using var stream = file.OpenReadStream();
                var result = await _storage.SaveAsync(stream, file.FileName);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Problem(detail: ex.Message, title: "Upload Failed", statusCode: 400);
            }
        }

        // Removes a previously uploaded photo by its stored file name.
        [HttpDelete("{fileName}")]
        public IActionResult Delete(string fileName)
        {
            return _storage.Delete(fileName) ? NoContent() : NotFound();
        }
    }
}
