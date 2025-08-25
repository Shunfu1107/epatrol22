using Microsoft.AspNetCore.SignalR;

namespace AdminPortalV8.Hubs
{
    public class NotificationHub : Hub
    {
        // You can add methods here if you want clients to invoke server-side logic
        public async Task SendNotification(string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", message);
        }
    }
}