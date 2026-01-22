using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Interfaces.Chats
{
    public interface IChatRepository
    {
        Task<Chat> CreateChat(Chat chat);
        Task<Chat?> GetChatAlongWithMessages(int chatId);
        Task<Chat?> GetChatById(int id);
    }
}
