using Microsoft.AspNetCore.Identity;

namespace PizzaMauiApp.API.Models.Identity;

public class User : IdentityUser
{
    public string DisplayName { get; set; }
    public UserAddress Address { get; set; }
    
    public string? RefreshToken { get; set; }
    
    public DateTime RefreshTokenExpiryTime { get; set; }
}