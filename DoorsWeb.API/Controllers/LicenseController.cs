using DoorsWeb.API.Authorization;
using DoorsWeb.API.Legacy;
using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoorsWeb.API.Controllers
{
    /// <summary>
    /// Exposes the current licensing state (validated server-side from the signed key in system
    /// settings) plus live door/card usage, so the System Settings → License panel can show
    /// "12 / 50 doors" and explain why a key is invalid/expired. Read-only; the key itself is
    /// saved through <see cref="SystemSettingsController"/>.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = AreaPolicies.SiteSettingsRead)]
    public class LicenseController : ControllerBase
    {
        private readonly ILicenseService _license;
        private readonly DoorsEnterpriseContext _context;

        public LicenseController(ILicenseService license, DoorsEnterpriseContext context)
        {
            _license = license;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<LicenseStatusDto>> Get()
        {
            var state = _license.GetState();

            return Ok(new LicenseStatusDto
            {
                HasKey = state.HasKey,
                IsValid = state.IsValid,
                IsExpired = state.IsExpired,
                IsActive = state.IsActive,
                IsLicensed = state.IsLicensed,
                Message = state.Message,
                Customer = state.Customer,
                LicenseId = state.LicenseId,
                MaxDoors = state.MaxDoors,
                MaxCards = state.MaxCards,
                ExpiryUtc = state.ExpiryUtc,
                IssuedUtc = state.IssuedUtc,
                DoorCount = await _context.Doors.CountAsync(),
                CardCount = await _context.Cardholder.CountAsync()
            });
        }
    }
}
