using EcommerceApp.Application.Dtos;

namespace EcommerceApp.Application.Interfaces.Chats
{
    public interface IChatService
    {
        Task<bool> CreateChat(CreateChatDto createChatDto);
        Task<bool> CloseChat(int id);
        Task<bool> SendMessage(int chatId, SendMessageDto sendMessageDto);
        Task<ChatDto?> GetChatAlongWithMessages(int chatId);
        Task<bool> MarkMessagesAsRead(int chatId);
    }
}
