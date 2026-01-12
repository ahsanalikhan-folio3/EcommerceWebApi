using EcommerceApp.Application.Interfaces.User;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

public class UserService : IUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public UserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public string UserId =>
        _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new UnauthorizedAccessException("UserId not found in JWT");

    public string Role =>
        _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value
        ?? throw new UnauthorizedAccessException("Role not found in JWT");
    public bool IsInRole(string role) =>
        _httpContextAccessor.HttpContext?
            .User
            .IsInRole(role) ?? false;
    public int GetUserIdInt() =>
        int.Parse(this.UserId);
}
