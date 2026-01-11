namespace EcommerceApp.Application.Interfaces.User
{
    public interface IUserService
    {
        string? UserId { get; }
        string? Role { get; }
        bool IsInRole(string role);
        int GetUserIdInt();
    }
}
