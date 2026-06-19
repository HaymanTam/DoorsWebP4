using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;
using DoorsWeb.Shared.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DoorsWeb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NameHeaderController : ControllerBase
    {
        private readonly INameHeaderService _service;

        public NameHeaderController(INameHeaderService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<Cardholder>>> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        // Streams cards (with their tied access-level names) as a JSON array so the client
        // can paint the first batch immediately and load the rest in the background.
        [HttpGet("cards")]
        public IAsyncEnumerable<CardDto> GetCards()
        {
            return _service.GetAllCards();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Cardholder>> GetById(int id)
        {
            var result = await _service.GetById(id);
            if (result is null)
            {
                return Problem(detail: $"Name Header (card {id}) was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<List<Cardholder>>> Create(Cardholder entity)
        {
            return Ok(await _service.Create(entity));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<List<Cardholder>?>> Update(int id, Cardholder entity)
        {
            var result = await _service.Update(id, entity);
            if (result is null)
            {
                return Problem(detail: $"Update Failed! Name Header (card {id}) was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<Cardholder>?>> Delete(int id)
        {
            var result = await _service.Delete(id);
            if (result is null)
            {
                return Problem(detail: $"Delete Failed! Name Header (card {id}) was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }
    }
}
