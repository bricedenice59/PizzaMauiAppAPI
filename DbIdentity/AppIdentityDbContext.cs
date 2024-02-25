using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PizzaMauiApp.API.Models.Identity;

namespace PizzaMauiApp.API.DbIdentity;

public class AppIdentityDbContext : IdentityDbContext<User>
{
    public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) 
        : base(options)
    {
    }
}