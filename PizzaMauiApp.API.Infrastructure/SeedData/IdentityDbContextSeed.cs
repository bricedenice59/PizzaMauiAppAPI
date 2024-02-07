using Microsoft.AspNetCore.Identity;
using PizzaMauiApp.API.Core.Models.Identity;

namespace PizzaMauiApp.API.Infrastructure.SeedData;

public class IdentityDbContextSeed 
{
    public static async Task SeedAsync(UserManager<User> userManager)
    {
        if (!userManager.Users.Any())
        {
            var user = new User
            {
                DisplayName = "Bob",
                Email = "bob@test.com",
                UserName = "bob@test.com",
                Address = new UserAddress
                {
                    Id = Guid.NewGuid().ToString(),
                    FirstName = "Bob",
                    LastName = "Bobbity",
                    Street = "10 The street",
                    City = "New York",
                    State = "NY",
                    ZipCode = "90210"
                }
            };

            await userManager.CreateAsync(user, "Pa$$w0rd7");
        }
    }
}
