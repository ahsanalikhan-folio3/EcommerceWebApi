namespace EcommerceApp.Domain.Entities
{
    public class EmailVerificationOtp
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int OtpAttempts { get; set; }
        public required string HashedOtp { get; set; }
        public DateTime ExpiryTime { get; set; }
        public ApplicationUser User { get; set; }
    }
}
