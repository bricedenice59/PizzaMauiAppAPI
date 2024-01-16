using Microsoft.AspNetCore.Identity;

namespace PizzaMauiApp.API.Core.Models.Identity;

public class User : IdentityUser
{
    public string DisplayName { get; set; }
    public UserAddress Address { get; set; }
}