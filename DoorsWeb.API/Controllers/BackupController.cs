using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;
using Microsoft.AspNetCore.Mvc;

namespace DoorsWeb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BackupController : ControllerBase
    {
        private readonly IBackupService _service;

        public BackupController(IBackupService service)
        {
            _service = service;
        }

        // Lists existing backup files in the server backup directory.
        [HttpGet]
        public ActionResult<List<BackupFileDto>> GetAll()
        {
            return Ok(_service.GetBackups());
        }

        // Creates a full database backup.
        [HttpPost]
        public async Task<ActionResult<BackupOperationResult>> Create(CreateBackupRequest request)
        {
            var result = await _service.CreateBackup(request);
            if (!result.Success)
            {
                return Problem(detail: result.Message, title: "Backup Failed", statusCode: 500);
            }
            return Ok(result);
        }

        // Restores the database from an existing backup file.
        [HttpPost("restore")]
        public async Task<ActionResult<BackupOperationResult>> Restore(RestoreBackupRequest request)
        {
            var result = await _service.RestoreBackup(request);
            if (!result.Success)
            {
                return Problem(detail: result.Message, title: "Restore Failed", statusCode: 500);
            }
            return Ok(result);
        }
    }
}
