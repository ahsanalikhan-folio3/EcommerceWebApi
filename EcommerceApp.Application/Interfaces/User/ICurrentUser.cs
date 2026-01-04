namespace EcommerceApp.Application.Interfaces.User
{
    public interface ICurrentUser
    {
        string? UserId { get; }
        string? Role { get; }
        bool IsInRole(string role);
        int GetUserIdInt();
    }
}
