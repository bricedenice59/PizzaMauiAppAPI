using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PizzaMauiApp.API.Core.Models.Identity;

namespace PizzaMauiApp.API.Infrastructure.Identity;

public class AppIdentityDbContext : IdentityDbContext<User>
{
    public AppIdentityDbContext(DbContextOptions options) : base(options)
    {
    }
}