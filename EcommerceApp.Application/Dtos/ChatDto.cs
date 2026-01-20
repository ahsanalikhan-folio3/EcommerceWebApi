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
}
