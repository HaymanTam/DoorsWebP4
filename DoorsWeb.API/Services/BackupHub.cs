using Microsoft.AspNetCore.SignalR;

namespace DoorsWeb.API.Services
{
    /// <summary>
    /// SignalR hub used to push live backup/restore progress (percent complete) to the
    /// client that initiated the operation. The client connects, reads its ConnectionId,
    /// and passes that id in the backup/restore request so the server can target it.
    /// No server-callable methods are needed  the server pushes via IHubContext.
    /// </summary>
    public class BackupHub : Hub
    {
    }
}
