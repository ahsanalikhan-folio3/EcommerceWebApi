namespace EcommerceApp.Domain.Entities
{
    public class AdminProfile
    {
        public int UserId { get; set; } // FK - ApplicationUsers Table
        public required ApplicationUser User { get; set; } // FK - ApplicationUsers Table
        public int CreatedBy { get; set; } // FK - ApplicationUsers Table [Admin that created this User]
    }
}
