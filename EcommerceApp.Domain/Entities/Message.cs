namespace EcommerceApp.Domain.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public int ChatId { get; set; } // FK - Chat Table
        public int SenderId { get; set; } // Fk - ApplicationUser Table
        public bool IsRead { get; set; }
        public required string SenderRole { get; set; } // "Customer" or "Seller"
        public required string Content { get; set; }
        public DateTime MessagedAt { get; set; }
        public required Chat CorrespondingChat { get; set; } // Navigation Property to Chat
        public required ApplicationUser Sender { get; set; } // Navigation Property to ApplicationUser
    }
}
