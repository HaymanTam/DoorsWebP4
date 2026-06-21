using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DoorsWeb.API.Services
{
    /// <summary>
    /// Real-time channel used by the live floorplan. The server pushes door-state changes to all
    /// connected clients via <c>IHubContext&lt;EventHub&gt;</c> (message "DoorStateChanged",
    /// payload <see cref="DoorsWeb.Shared.DTO.DoorStateDto"/>); see <c>DoorStateService</c>.
    /// Clients do not call any server methods on this hub.
    ///
    /// Authorized: the JWT is supplied on the query string (access_token) because browsers can't
    /// set Authorization headers on the WebSocket handshake (wired up in Program.cs OnMessageReceived).
    /// </summary>
    [Authorize]
    public class EventHub : Hub
    {
    }
}
