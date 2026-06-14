using DoorsWeb.API.Services;
using DoorsWeb.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System;


namespace DoorsWeb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }
    
        [HttpGet]
        public async Task<ActionResult<List<Admin>>> GetAllAdmins()
        {
            var result = await _adminService.GetAllAdmins();
            return Ok(result); 
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<Admin>>> GetAdminById(Guid id)
        {
            var result = await _adminService.GetAdminById(id);
            if (result is null)
            {
                return Problem(detail: $"Admin Id <{id}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<List<Admin>>> CreateAdmin(Admin admin)
        {
            var result = await _adminService.CreateAdmin(admin);
            return Ok(result);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<List<Admin>?>> UpdateAdmin(Guid id, Admin admin)
        {
            var result = await _adminService.UpdateAdmin(id, admin);
            if (result is null)
            {
                return Problem(detail: $"Update Failed! Admin Id <{id}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<List<Admin>?>> DeleteAdminById(Guid id)
        {
            var result = await _adminService.DeleteAdminById(id);
            if (result is null)
            {
                return Problem(detail: $"Delete Failed! Admin Id <{id}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }
    }
}
