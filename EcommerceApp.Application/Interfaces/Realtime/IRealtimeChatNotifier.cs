namespace EcommerceApp.Application.Interfaces.Realtime
{
    public interface IRealtimeChatNotifier
    {
        Task SendMessageInRealtime(string userId, string message);
    }
}
