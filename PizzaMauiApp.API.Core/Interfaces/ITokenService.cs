using PizzaMauiApp.API.Core.Models.Identity;

namespace PizzaMauiApp.API.Core.Interfaces;

public interface ITokenService
{
    string CreateToken(User user);

    User? ValidateToken(string token);
}