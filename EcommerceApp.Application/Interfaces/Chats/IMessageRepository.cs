using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Interfaces.Chats
{
    public interface IMessageRepository
    {
        Task<Message> GetMessageById(int id);
        Task<IEnumerable<Message>> GetMessagesByChatId(int chatId);
        Task<Message> AddMessage(Message message);
    }
}
