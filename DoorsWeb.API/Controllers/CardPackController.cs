using DoorsWeb.API.Authorization;
using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoorsWeb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = AreaPolicies.CardManagerRead)]
    public class CardPackController : ControllerBase
    {
        private readonly ICardPackService _service;

        public CardPackController(ICardPackService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<CardPack>>> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CardPack>> GetById(int id)
        {
            var result = await _service.GetById(id);
            if (result is null)
            {
                return Problem(detail: $"Card Pack Header <{id}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        [Authorize(Policy = AreaPolicies.CardManagerWrite)]
        [HttpPost]
        public async Task<ActionResult<List<CardPack>>> Create(CardPack entity)
        {
            return Ok(await _service.Create(entity));
        }

        [Authorize(Policy = AreaPolicies.CardManagerWrite)]
        [HttpPut("{id}")]
        public async Task<ActionResult<List<CardPack>?>> Update(int id, CardPack entity)
        {
            var result = await _service.Update(id, entity);
            if (result is null)
            {
                return Problem(detail: $"Update Failed! Card Pack Header <{id}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }

        [Authorize(Policy = AreaPolicies.CardManagerWrite)]
        [HttpDelete("{id}")]
        public async Task<ActionResult<List<CardPack>?>> Delete(int id)
        {
            var result = await _service.Delete(id);
            if (result is null)
            {
                return Problem(detail: $"Delete Failed! Card Pack Header <{id}> was not found.", title: "Not Found", statusCode: 404);
            }
            return Ok(result);
        }
    }
}
