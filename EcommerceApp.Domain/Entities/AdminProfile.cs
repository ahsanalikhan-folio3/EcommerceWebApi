namespace EcommerceApp.Domain.Entities
{
    public class AdminProfile
    {
        public Guid UserId { get; set; } // FK - ApplicationUsers Table
        public Guid CreatedBy { get; set; } // FK - ApplicationUsers Table [Admin that created this User]
        public DateTime LastLoginDate { get; set; } 
    }
}
