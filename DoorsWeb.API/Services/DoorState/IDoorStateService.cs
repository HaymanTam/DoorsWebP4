using DoorsWeb.Shared.DTO;
using DoorsWeb.Shared.Enums;

namespace DoorsWeb.API.Services.DoorState
{
    /// <summary>
    /// Maintains the in-memory live state of every door and broadcasts changes to floorplan
    /// clients over SignalR. Fed by inbound controller packets (UDP) and by command
    /// acknowledgements from the control API.
    /// </summary>
    public interface IDoorStateService
    {
        /// <summary>Current live state of every known door (the initial floorplan snapshot).</summary>
        IReadOnlyCollection<DoorStateDto> GetSnapshot();

        /// <summary>
        /// Applies a locally-known state change (e.g. just after sending an unlock command) so the
        /// UI reflects the action immediately, and broadcasts it. The next controller ping/event
        /// reconciles it with the truth.
        /// </summary>
        Task ApplyLocalAsync(int door, DoorLiveStatus status, string? eventName, CancellationToken ct = default);

        /// <summary>Re-reads the door list (number, controller address, name, site) from the database.</summary>
        Task RefreshDoorMapAsync(CancellationToken ct = default);
    }
}
