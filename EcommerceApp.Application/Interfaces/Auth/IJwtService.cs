using EcommerceApp.Domain.Entities;

namespace EcommerceApp.Application.Interfaces.Auth
{
    public interface IJwtService
    {
        string GenerateToken(ApplicationUser user);
    }
}
