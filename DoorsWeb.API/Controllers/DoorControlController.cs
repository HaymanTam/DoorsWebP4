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
        private readonly IPendingCommandService _pending;

        public DoorControlController(IDoorStateService state, IDoorCommandService commands, IPendingCommandService pending)
        {
            _state = state;
            _commands = commands;
            _pending = pending;
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

        /// <summary>Commands that have been sent but not yet acknowledged (still being retried).</summary>
        [HttpGet("pending")]
        public ActionResult<IReadOnlyCollection<PendingCommandDto>> GetPending()
            => Ok(_pending.GetSnapshot());

        /// <summary>Cancels a single pending command so it stops being retried.</summary>
        [Authorize(Policy = AreaPolicies.SiteSettingsWrite)]
        [HttpDelete("pending/{id:guid}")]
        public IActionResult ClearPending(Guid id)
            => _pending.Clear(id) ? NoContent()
                                  : Problem(detail: $"Pending command <{id}> was not found.", title: "Not Found", statusCode: 404);

        /// <summary>Cancels every pending command, or only those for one door when <c>door</c> is supplied.</summary>
        [Authorize(Policy = AreaPolicies.SiteSettingsWrite)]
        [HttpDelete("pending")]
        public ActionResult<int> ClearAllPending([FromQuery] int? door)
            => Ok(_pending.ClearAll(door));
    }
}
