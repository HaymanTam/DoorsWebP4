using DoorsWeb.Shared.DTO;

namespace DoorsWeb.API.Services.Interfaces
{
    /// <summary>Outcome of trying to send a control command to a door.</summary>
    public enum DoorCommandOutcome
    {
        Sent,
        DoorNotFound,
        NoIpAddress
    }

    /// <summary>Result of a single door command: what happened plus the door's new (optimistic) state.</summary>
    public record DoorCommandResult(DoorCommandOutcome Outcome, DoorStateDto? State);

    /// <summary>
    /// Builds and sends controller commands for the floorplan's click-to-act surface:
    /// unlock (open forever), lock (close), momentary release (timed), and site/all lockdown.
    /// Maps to the "5,4 Trigger Channel A/B" protocol command and updates the live state optimistically.
    /// </summary>
    public interface IDoorCommandService
    {
        Task<DoorCommandResult> SendAsync(int door, DoorCommandRequest request, CancellationToken ct = default);

        /// <summary>Locks every door (optionally restricted to one site). Returns the number commanded.</summary>
        Task<int> LockdownAsync(int? site, CancellationToken ct = default);
    }
}
