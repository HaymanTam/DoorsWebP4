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
        private readonly ILegacyBackupService _legacyService;

        public BackupController(IBackupService service, ILegacyBackupService legacyService)
        {
            _service = service;
            _legacyService = legacyService;
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

        // Restores a legacy DoorsClient backup (a password-protected ZIP of .sql/.rs table dumps)
        // by bulk-loading the recordset data into the existing tables. The upload can be large
        // (the T_Events recordset alone is ~100 MB uncompressed), so the usual size limits are lifted.
        [HttpPost("restore-legacy")]
        [RequestSizeLimit(2L * 1024 * 1024 * 1024)]
        [RequestFormLimits(MultipartBodyLengthLimit = 2L * 1024 * 1024 * 1024)]
        public async Task<ActionResult<LegacyRestoreResult>> RestoreLegacy(
            [FromForm] IFormFile file,
            [FromForm] string? password,
            [FromForm] string? connectionId,
            CancellationToken cancellationToken)
        {
            if (file is null || file.Length == 0)
            {
                return Problem(detail: "No backup file was uploaded.", title: "Restore Failed", statusCode: 400);
            }

            // Stage the upload to a temp file; SharpZipLib needs a seekable archive.
            var tempZip = Path.Combine(Path.GetTempPath(), $"doorsweb_legacy_{Guid.NewGuid():N}.zip");
            try
            {
                await using (var stream = System.IO.File.Create(tempZip))
                {
                    await file.CopyToAsync(stream, cancellationToken);
                }

                var result = await _legacyService.RestoreLegacyBackup(
                    tempZip, password ?? "", connectionId, cancellationToken);

                if (!result.Success)
                {
                    return Problem(detail: result.Message, title: "Restore Failed", statusCode: 500);
                }
                return Ok(result);
            }
            finally
            {
                try { if (System.IO.File.Exists(tempZip)) System.IO.File.Delete(tempZip); } catch { /* best-effort */ }
            }
        }
    }
}
