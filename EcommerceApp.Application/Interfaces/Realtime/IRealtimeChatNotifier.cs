using EcommerceApp.Application.Dtos;

namespace EcommerceApp.Application.Interfaces.Realtime
{
    public interface IRealtimeChatNotifier
    {
        Task SendMessageInRealtime(string userId, MessageDto message);
    }
}
