using DoorsWeb.API.Authorization;
using DoorsWeb.API.Services;
using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoorsWeb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = AreaPolicies.UserSettingsRead)]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _service;

        public UsersController(IUsersService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<Users>>> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Users>> GetById(int id)
        {
            var result = await _service.GetById(id);
            if (result is null)
            {
                return Problem(detail: $"User ({id}) was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        [Authorize(Policy = AreaPolicies.UserSettingsWrite)]
        [HttpPost]
        public async Task<ActionResult<List<Users>>> Create(Users entity)
        {
            try
            {
                return Ok(await _service.Create(entity));
            }
            catch (PasswordPolicyException ex)
            {
                // Password too short or on the breached/common list.
                return Problem(detail: ex.Message, title: "Password not allowed", statusCode: 400);
            }
        }

        [Authorize(Policy = AreaPolicies.UserSettingsWrite)]
        [HttpPut("{id}")]
        public async Task<ActionResult<List<Users>?>> Update(int id, Users entity)
        {
            try
            {
                var result = await _service.Update(id, entity);
                if (result is null)
                {
                    return Problem(detail: $"Update Failed! User ({id}) was not found.", title: "Not Found", statusCode: 404);
                }
                return Ok(result);
            }
            catch (PasswordPolicyException ex)
            {
                // Password too short or on the breached/common list.
                return Problem(detail: ex.Message, title: "Password not allowed", statusCode: 400);
            }
            catch (InvalidOperationException ex)
            {
                // Last-Super protection: at least one Super user must always exist.
                return Problem(detail: ex.Message, title: "Cannot update user", statusCode: 409);
            }
        }

        [Authorize(Policy = AreaPolicies.UserSettingsWrite)]
        [HttpDelete("{id}")]
        public async Task<ActionResult<List<Users>?>> Delete(int id)
        {
            try
            {
                var result = await _service.Delete(id);
                if (result is null)
                {
                    return Problem(detail: $"Delete Failed! User ({id}) was not found.", title: "Not Found", statusCode: 404);
                }
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                // Last-Super protection: at least one Super user must always exist.
                return Problem(detail: ex.Message, title: "Cannot delete user", statusCode: 409);
            }
        }
    }
}
