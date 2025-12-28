namespace EcommerceApp.Application.Dtos
{
    public class RegisterDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Role { get; set; }
        public required string FullName { get; set; }
    }
    public class LoginDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
    public class ApplicationUserDto
    {
        public Guid Id { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Role { get; set; }
        public required string FullName { get; set; }
    }
}
