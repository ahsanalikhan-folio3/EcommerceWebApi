using EcommerceApp.Application.Dtos;
using EcommerceApp.Application.Interfaces.Realtime;
using EcommerceApp.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace EcommerceApp.Infrastructure.Realtime
{
    public class SignalRRealtimeChatNotifier : IRealtimeChatNotifier
    {
        private readonly IHubContext<ChatHub> hubContext;

        public SignalRRealtimeChatNotifier(IHubContext<ChatHub> hubContext)
        {
            this.hubContext = hubContext;
        }
        public async Task SendMessageInRealtime(string userId, MessageDto message)
        {
            // This calls a function called "ReceiveMessage(message)" on the client side from the server.
            await hubContext.Clients.Group(userId).SendAsync("ReceiveMessage", message);
        }
    }
}
