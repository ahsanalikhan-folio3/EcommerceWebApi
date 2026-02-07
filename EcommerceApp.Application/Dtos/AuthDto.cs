namespace EcommerceApp.Application.Dtos
{
    public class RegisterUserDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
        public required string Role { get; set; }
        public required string PhoneNumber { get; set; }
        public required string FullName { get; set; }
    }
    public class EmailVerificationDto
    {
        public required string Email { get; set; }
        public required string Otp { get; set; }
    }
    public class LoginDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
    public class GetLoginResultDto
    {
        public required string Role { get; set; }
        public required string Token { get; set; }
    }
    public class  UserActivationDto 
    {
        public bool IsActive { get; set; }
    }
    public class ApplicationUserDto
    {
        public int Id { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Role { get; set; }
        public required string FullName { get; set; }
    }

    public class AdminProfileDto : RegisterUserDto 
    {
        public int UserId { get; set; }
        public int CreatedBy { get; set; }
    }
    public class CustomerProfileDto : RegisterUserDto 
    {
        public required string HouseNumber { get; set; }
        public required string StreetNumber { get; set; }
        public required string City { get; set; }
        public required string State { get; set; }
        public required string PostalCode { get; set; }
        public required string Country { get; set; }
        public required string Gender { get; set; }
    }
    public class SellerProfileDto : RegisterUserDto 
    {
        public required string Storename { get; set; }
        public required string City { get; set; }
        public required string State { get; set; }
        public required string PostalCode { get; set; }
        public required string Country { get; set; }
    }
    public class GetUserDto 
    {
        public int Id { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string FullName { get; set; }
        public required string Role { get; set; }
    }
    public class GetAdminProfileDto : GetUserDto
    {

    }
    public class GetCustomerProfileDto : GetUserDto
    {
        public required string HouseNumber { get; set; }
        public required string StreetNumber { get; set; }
        public required string City { get; set; }
        public required string State { get; set; }
        public required string PostalCode { get; set; }
        public required string Country { get; set; }
        public required string Gender { get; set; }
    }
    public class GetSellerProfileDto : GetUserDto
    {
        public required string Storename { get; set; }
        public required string City { get; set; }
        public required string State { get; set; }
        public required string PostalCode { get; set; }
        public required string Country { get; set; }
    }
}
