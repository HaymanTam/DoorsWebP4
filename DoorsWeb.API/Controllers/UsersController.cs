using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DoorsWeb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _service;

        public UsersController(IUsersService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<TUsers>>> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TUsers>> GetById(int id)
        {
            var result = await _service.GetById(id);
            if (result is null)
            {
                return Problem(detail: $"User ({id}) was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<List<TUsers>>> Create(TUsers entity)
        {
            return Ok(await _service.Create(entity));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<List<TUsers>?>> Update(int id, TUsers entity)
        {
            var result = await _service.Update(id, entity);
            if (result is null)
            {
                return Problem(detail: $"Update Failed! User ({id}) was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<TUsers>?>> Delete(int id)
        {
            var result = await _service.Delete(id);
            if (result is null)
            {
                return Problem(detail: $"Delete Failed! User ({id}) was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }
    }
}
