using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Dtos
{
    public class CreateChatDto
    {
        public int SellerId { get; set; }
    }
    public class SendMessageDto
    {
        public required string Content { get; set; }
    }
    public class ChatDto
    {
        public int Id { get; set; }
        public bool IsClosed { get; set; } // Only Customer can close chats with sellers
        public DateTime CreatedAt { get; set; }
        public DateTime LastMessagedAt { get; set; }
        public required ICollection<MessageDto> Messages { get; set; } //  Navigation property to Messages
    }
    public class MessageDto 
    {
        public int Id { get; set; }
        public int SenderId { get; set; } 
        public bool IsRead { get; set; }
        public required string SenderRole { get; set; } 
        public required string Content { get; set; }
        public DateTime MessagedAt { get; set; }
    }
}
