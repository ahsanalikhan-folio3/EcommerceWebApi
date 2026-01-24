using EcommerceApp.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace EcommerceApp.Infrastructure.Hubs
{
    [Authorize(Roles = $"{AppRoles.Seller},{AppRoles.Customer}")]
    public class ChatHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"Client connected: {Context.ConnectionId}");

            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrWhiteSpace(userId))
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);

            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"Client disconnected: {Context.ConnectionId} Reason: {exception?.Message}");

            var userId = Context.User?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrWhiteSpace(userId))
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);

            await base.OnDisconnectedAsync(exception);
        }

    }
}
