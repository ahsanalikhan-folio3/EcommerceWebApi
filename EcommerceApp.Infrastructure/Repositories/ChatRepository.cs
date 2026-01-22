using EcommerceApp.Application.Interfaces.Chats;
using EcommerceApp.Domain.Entities;
using EcommerceApp.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Infrastructure.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly ApplicationDbContext db;
        public ChatRepository(ApplicationDbContext db)
        {
            this.db = db;
        }
        public async Task<Chat> CreateChat(Chat chat)
        {
            var result = await db.Chats.AddAsync(chat);
            return result.Entity;
        }

        public async Task<Chat?> GetChatAlongWithMessages(int chatId)
        {
            return await db.Chats.Include(m => m.Messages.OrderByDescending(m => m.MessagedAt)).FirstOrDefaultAsync(c => c.Id == chatId);
        }

        public async Task<Chat?> GetChatById(int id)
        {
            return await db.Chats.FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
