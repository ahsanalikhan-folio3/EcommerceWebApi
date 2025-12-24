namespace EcommerceApp.Application.Dtos
{
    public class RegisterDto
    {
        public required string Email { get; set; }
        public required string HashedPassword { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Role { get; set; }
        public required string FullName { get; set; }
    }
    public class LoginDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
