using DoorsWeb.API.Authorization;
using DoorsWeb.API.Services.DoorState;
using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoorsWeb.API.Controllers
{
    /// <summary>
    /// Live control surface for the floorplan: the current door-state snapshot, per-door commands
    /// (unlock / lock / momentary release) and site/all lockdown. Real-time changes are pushed
    /// separately over the SignalR EventHub.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = AreaPolicies.SiteSettingsRead)]
    public class DoorControlController : ControllerBase
    {
        private readonly IDoorStateService _state;
        private readonly IDoorCommandService _commands;

        public DoorControlController(IDoorStateService state, IDoorCommandService commands)
        {
            _state = state;
            _commands = commands;
        }

        /// <summary>Current live state of every door (the floorplan's initial snapshot).</summary>
        [HttpGet("states")]
        public ActionResult<IReadOnlyCollection<DoorStateDto>> GetStates()
            => Ok(_state.GetSnapshot());

        /// <summary>Sends a control command (unlock / lock / momentary release) to one door.</summary>
        [Authorize(Policy = AreaPolicies.SiteSettingsWrite)]
        [HttpPost("{door:int}/command")]
        public async Task<ActionResult<DoorStateDto>> SendCommand(int door, DoorCommandRequest request, CancellationToken ct)
        {
            var result = await _commands.SendAsync(door, request, ct);
            return result.Outcome switch
            {
                DoorCommandOutcome.DoorNotFound =>
                    Problem(detail: $"Door <{door}> was not found.", title: "Not Found", statusCode: 404),
                DoorCommandOutcome.NoIpAddress =>
                    Problem(detail: $"Door <{door}> has no IP address configured, so it cannot be commanded.",
                            title: "No IP Address", statusCode: 409),
                _ => Ok(result.State)
            };
        }

        /// <summary>Locks every door (optionally one site). Returns the number of doors commanded.</summary>
        [Authorize(Policy = AreaPolicies.SiteSettingsWrite)]
        [HttpPost("lockdown")]
        public async Task<ActionResult<int>> Lockdown([FromQuery] int? site, CancellationToken ct)
            => Ok(await _commands.LockdownAsync(site, ct));
    }
}
