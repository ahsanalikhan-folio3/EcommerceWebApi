using AutoMapper;
using EcommerceApp.Application.Dtos;
using EcommerceApp.Application.Interfaces;
using EcommerceApp.Application.Interfaces.Chats;
using EcommerceApp.Application.Interfaces.User;
using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Services
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;
        private readonly IUserService user;

        public ChatService(IUnitOfWork uow, IMapper mapper, IUserService user)
        {
            this.uow = uow;
            this.mapper = mapper;
            this.user = user;
        }

        public async Task<bool> CreateChat(CreateChatDto createChatDto)
        {
            Chat chat = mapper.Map<Chat>(createChatDto);
            chat.CreatedAt = DateTime.UtcNow;
            chat.LastMessagedAt = DateTime.UtcNow;
            chat.CustomerId = user.GetUserIdInt();

            var result = await uow.Chats.CreateChat(chat);
            await uow.SaveChangesAsync();
            return true;
        }
    }
}
