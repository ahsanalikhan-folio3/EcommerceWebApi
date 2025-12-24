namespace EcommerceApp.Domain.Entities
{
    public class ApplicationUser
    {
        public Guid Id { get; set; }
        public required string Email { get; set; }
        public required string HashedPassword { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Role { get; set; }
        public required string FullName { get; set; } 
        public bool IsActive { get; set; } // Higher Ups can deactivate the User
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
