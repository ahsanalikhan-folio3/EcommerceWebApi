namespace EcommerceApp.Domain.Entities
{
    public class Chat
    {
        public int Id { get; set; }
        public int SellerId { get; set; } // FK - ApplicationUser Table For Seller 
        public int CustomerId { get; set; } // FK - ApplicationUser Table For Customer
        public bool IsClosed { get; set; } // Only Customer can close chats with sellers
        public DateTime CreatedAt { get; set; }
        public DateTime LastMessagedAt { get; set; }
        public required ApplicationUser Seller { get; set; } //  Navigation property to ApplicationUser
        public required ApplicationUser Customer { get; set; } //  Navigation property to ApplicationUser
        public required ICollection<Message> Messages { get; set; } //  Navigation property to Messages
    }
}
