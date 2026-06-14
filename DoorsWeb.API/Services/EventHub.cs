using Microsoft.AspNetCore.SignalR;

public class EventHub : Hub
{
    // Example only  not used 
    public async Task PushEvent(string user, string message)
    {
        // Example only  not used 
        await Clients.All.SendAsync("NewEvent", user, message);
    }
}