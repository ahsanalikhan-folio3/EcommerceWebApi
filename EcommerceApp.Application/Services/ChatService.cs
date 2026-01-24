using AutoMapper;
using EcommerceApp.Application.Common;
using EcommerceApp.Application.Dtos;
using EcommerceApp.Application.Interfaces;
using EcommerceApp.Application.Interfaces.Chats;
using EcommerceApp.Application.Interfaces.Realtime;
using EcommerceApp.Application.Interfaces.User;
using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Services
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;
        private readonly IUserService user;
        private readonly IRealtimeChatNotifier realtimeChatNotifier;

        public ChatService(IUnitOfWork uow, IMapper mapper, IUserService user, IRealtimeChatNotifier realtimeChatNotifier)
        {
            this.uow = uow;
            this.mapper = mapper;
            this.user = user;
            this.realtimeChatNotifier = realtimeChatNotifier;
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
        public async Task<bool> CloseChat(int id)
        {
            var chat = await uow.Chats.GetChatById(id);
            // Ensure that only the customer who owns the chat can close it.
            if (chat == null || chat.CustomerId != user.GetUserIdInt()) return false;
            chat.IsClosed = true;
            await uow.SaveChangesAsync();
            return true;
        }
        public async Task<bool> SendMessage(int chatId, SendMessageDto sendMessageDto)
        {
            var chat = await uow.Chats.GetChatById(chatId);

            // Ensure that the chat exists and is not closed.
            if (chat == null || chat.IsClosed) return false;

            string userRole = user.Role!;
            int userId = user.GetUserIdInt();

            // Ensure that the user is either the customer or the seller in the chat.
            if (userRole == AppRoles.Customer && chat.CustomerId != userId) return false;
            if (userRole == AppRoles.Seller && chat.SellerId != userId) return false;

            var message = mapper.Map<Message>(sendMessageDto);
            message.ChatId = chatId;
            message.SenderId = userId;
            message.SenderRole = userRole;
            message.MessagedAt = DateTime.UtcNow;
            
            var result = await uow.Messages.AddMessage(message);

            // Update the chat's last messaged timestamp.
            chat.LastMessagedAt = DateTime.UtcNow;
            await uow.SaveChangesAsync();
            var mappedMessage = mapper.Map<MessageDto>(result);

            string receiverId = (userId == chat.CustomerId ? chat.SellerId : chat.CustomerId).ToString();
            await realtimeChatNotifier.SendMessageInRealtime(receiverId, mappedMessage);

            return true;
        }
        public async Task<ChatDto?> GetChatAlongWithMessages(int chatId)
        {
            Chat? chat = await uow.Chats.GetChatAlongWithMessages(chatId);
            if (chat is null) return null;

            return mapper.Map<ChatDto>(chat);
        }

        public async Task<bool> MarkMessagesAsRead(int chatId)
        {
            var chat = await uow.Chats.GetChatById(chatId);
            if (chat is null) return false;

            string senderRole = user.Role!;
            int userId = user.GetUserIdInt();

            // Ensure that the user is either the customer or the seller in the chat.
            if (senderRole == AppRoles.Customer && chat.CustomerId != userId) return false;
            if (senderRole == AppRoles.Seller && chat.SellerId != userId) return false;

            // It will mark all messages in the chat not sent by the requestor as read.
            await uow.Messages.MarkMessagesAsRead(chatId, senderRole);

            await uow.SaveChangesAsync();
            return true;
        }
    }
}
