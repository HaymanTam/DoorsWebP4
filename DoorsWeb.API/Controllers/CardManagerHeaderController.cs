using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DoorsWeb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardManagerHeaderController : ControllerBase
    {
        private readonly ICardManagerHeaderService _service;

        public CardManagerHeaderController(ICardManagerHeaderService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<CardManager>>> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CardManager>> GetById(int id)
        {
            var result = await _service.GetById(id);
            if (result is null)
            {
                return Problem(detail: $"Card Manager Header <{id}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<List<CardManager>>> Create(CardManager entity)
        {
            return Ok(await _service.Create(entity));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<List<CardManager>?>> Update(int id, CardManager entity)
        {
            var result = await _service.Update(id, entity);
            if (result is null)
            {
                return Problem(detail: $"Update Failed! Card Manager Header <{id}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<CardManager>?>> Delete(int id)
        {
            var result = await _service.Delete(id);
            if (result is null)
            {
                return Problem(detail: $"Delete Failed! Card Manager Header <{id}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }
    }
}
