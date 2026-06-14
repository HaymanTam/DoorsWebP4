using DoorsWeb.API.Services;
using DoorsWeb.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DoorsWeb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetAllUsers()
        {
            var result = await _userService.GetAllUsers();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById(Guid id)
        {
            var result = await _userService.GetUserById(id);
            return result.Map<ActionResult>(
                onSuccess: value => Ok(value),
                onFailure: error => Problem(detail: error.Message, title: error.Type.ToString(), statusCode: error.StatusCode)
            );
        }

        [HttpPost]
        public async Task<ActionResult<List<UserDto>>> CreateUser(UserDto userDto)
        {
            var result = await _userService.CreateUser(userDto);
            return result.Map<ActionResult>(
                onSuccess: value => Ok(value),
                onFailure: error => Problem(detail: error.Message, title: error.Type.ToString(), statusCode: error.StatusCode)
            );
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<List<UserDto>>> UpdateUser(Guid id, UserDto userDto)
        {
            var result = await _userService.UpdateUser(id, userDto);
            return result.Map<ActionResult>(
                onSuccess: value => Ok(value),
                onFailure: error => Problem(detail: error.Message, title: error.Type.ToString(), statusCode: error.StatusCode)
            );
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<UserDto>>> DeleteUserById(Guid id)
        {
            var result = await _userService.DeleteUserById(id);
            return result.Map<ActionResult>(
                onSuccess: value => Ok(value),
                onFailure: error => Problem(detail: error.Message, title: error.Type.ToString(), statusCode: error.StatusCode)
            );
        }
    }
}
