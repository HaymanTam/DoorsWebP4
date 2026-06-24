using DoorsWeb.API.Authorization;
using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoorsWeb.API.Controllers
{
    // Global custom-field definitions, their combobox option lists and per-card values.
    // Reading needs Card Manager read access (it feeds the card record + the settings screen);
    // any change needs write access. Custom fields are part of card management, so the same
    // 3-area policies as CardholderController apply.
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = AreaPolicies.CardManagerRead)]
    public class CustomFieldController : ControllerBase
    {
        private readonly ICustomFieldService _service;

        public CustomFieldController(ICustomFieldService service)
        {
            _service = service;
        }

        /// <summary>All global field definitions (with combobox options), ordered by slot.</summary>
        [HttpGet("definitions")]
        public async Task<ActionResult<List<CustomFieldDefinitionDto>>> GetDefinitions()
        {
            return Ok(await _service.GetDefinitions());
        }

        /// <summary>The definitions merged with one card's stored values.</summary>
        [HttpGet("card/{cardNumber:int}")]
        public async Task<ActionResult<List<CardCustomFieldDto>>> GetForCard(int cardNumber)
        {
            return Ok(await _service.GetForCard(cardNumber));
        }

        /// <summary>Persists a card's custom-field values (one slot/value pair each).</summary>
        [Authorize(Policy = AreaPolicies.CardManagerWrite)]
        [HttpPut("card/{cardNumber:int}")]
        public async Task<IActionResult> SaveForCard(int cardNumber, List<CardCustomFieldValueDto> values)
        {
            await _service.SaveForCard(cardNumber, values ?? new());
            return NoContent();
        }

        /// <summary>Adds a new selectable option to a combobox field's list.</summary>
        [Authorize(Policy = AreaPolicies.CardManagerWrite)]
        [HttpPost("definitions/{slot:int}/options")]
        public async Task<ActionResult<CustomFieldOptionDto>> AddOption(int slot, [FromBody] AddOptionRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Description))
                return Problem(detail: "Description must be entered.", title: "Invalid Option", statusCode: 400);

            return Ok(await _service.AddOption(slot, request.Description));
        }

        /// <summary>Removes a single option from a combobox field's list.</summary>
        [Authorize(Policy = AreaPolicies.CardManagerWrite)]
        [HttpDelete("definitions/{slot:int}/options/{code:int}")]
        public async Task<IActionResult> DeleteOption(int slot, int code)
        {
            if (!await _service.DeleteOption(slot, code))
                return Problem(detail: $"Option {code} was not found.", title: "Not Found", statusCode: 404);
            return NoContent();
        }

        /// <summary>Creates a new field definition in the next free slot.</summary>
        [Authorize(Policy = AreaPolicies.CardManagerWrite)]
        [HttpPost("definitions")]
        public async Task<ActionResult<CustomFieldDefinitionDto>> CreateDefinition(CustomFieldDefinitionDto dto)
        {
            var result = await _service.CreateDefinition(dto);
            if (result is null)
                return Problem(detail: "All 25 custom-field slots are in use.", title: "No Free Slot", statusCode: 409);
            return Ok(result);
        }

        /// <summary>Renames / re-types an existing field definition.</summary>
        [Authorize(Policy = AreaPolicies.CardManagerWrite)]
        [HttpPut("definitions/{slot:int}")]
        public async Task<ActionResult<CustomFieldDefinitionDto>> UpdateDefinition(int slot, CustomFieldDefinitionDto dto)
        {
            var result = await _service.UpdateDefinition(slot, dto);
            if (result is null)
                return Problem(detail: $"Custom field (slot {slot}) was not found.", title: "Not Found", statusCode: 404);
            return Ok(result);
        }

        /// <summary>Removes a field definition and its combobox options.</summary>
        [Authorize(Policy = AreaPolicies.CardManagerWrite)]
        [HttpDelete("definitions/{slot:int}")]
        public async Task<IActionResult> DeleteDefinition(int slot)
        {
            if (!await _service.DeleteDefinition(slot))
                return Problem(detail: $"Custom field (slot {slot}) was not found.", title: "Not Found", statusCode: 404);
            return NoContent();
        }

        /// <summary>Body for adding a combobox option (a JSON object so it's unambiguous over the wire).</summary>
        public record AddOptionRequest(string Description);
    }
}
