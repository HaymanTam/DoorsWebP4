using DoorsWeb.Shared.Entities;

namespace DoorsWeb.API.Services.Interfaces
{
    /// <summary>
    /// Pushes a door's saved configuration down to its physical controller over UDP, so a settings
    /// change made in the web client (release times, relay-B behaviour, reader volumes, keypad access
    /// code, valid-from/to window, …) is actually programmed into the hardware rather than only stored
    /// in the database. Delivery is handled by the pending-command retry queue: each programming packet
    /// is sent once immediately and resent until the controller acknowledges it.
    /// </summary>
    public interface IDoorConfigSyncService
    {
        /// <summary>
        /// Queues the "engineers pack" (1,6) and "users pack" (1,5) programming packets that mirror
        /// <paramref name="door"/>'s current settings to its controller. A no-op when the door has no
        /// IP address or an unparseable controller id. Never throws: build/send failures are logged
        /// and swallowed so a save is never blocked by programming.
        /// </summary>
        Task SyncDoorAsync(Doors door, CancellationToken ct = default);
    }
}
