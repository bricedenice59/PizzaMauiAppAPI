using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PizzaMauiApp.API.Core.Models.Identity;

namespace PizzaMauiApp.API.Infrastructure.DbIdentity;

public class AppIdentityDbContext : IdentityDbContext<User>
{
    public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) 
        : base(options)
    {
    }
}