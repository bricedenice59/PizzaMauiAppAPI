using System.Security.Claims;
using PizzaMauiApp.API.Core.Models.Identity;

namespace PizzaMauiApp.API.Core.Interfaces;

public interface ITokenService
{
    string CreateToken(User user);

    string GenerateRefreshToken();
    
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token);

    User? ValidateToken(string token);
}