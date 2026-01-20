using EcommerceApp.Application.Dtos;

namespace EcommerceApp.Application.Interfaces.Chats
{
    public interface IChatService
    {
        Task<bool> CreateChat(CreateChatDto createChatDto);
    }
}
