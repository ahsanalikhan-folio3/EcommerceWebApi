using EcommerceApp.Application.Interfaces.Chats;
using EcommerceApp.Domain.Entities;
using EcommerceApp.Infrastructure.Database;

namespace EcommerceApp.Infrastructure.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ApplicationDbContext db;
        public MessageRepository(ApplicationDbContext db)
        {
            this.db = db;          
        }
        public async Task<Message> AddMessage(Message message)
        {
            var result = await db.Messages.AddAsync(message);
            return result.Entity;
        }

        public Task<Message> GetMessageById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Message>> GetMessagesByChatId(int chatId)
        {
            throw new NotImplementedException();
        }
    }
}
