using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PizzaMauiApp.API.Core.Models.Identity;
using PizzaMauiApp.API.Infrastructure.EnvironmentConfig;
using PizzaMauiApp.API.Infrastructure.Identity;

namespace PizzaMauiApp.API.Extensions;

public static class IdentityServiceExtensions
{
    public static void AddIdentityServices(this IServiceCollection services, 
        ConfigurationManager _,
        TokenAuth0Config config)
    {
        var builder = services.AddIdentityCore<User>();

        builder = new IdentityBuilder(builder.UserType, builder.Services);
        builder.AddEntityFrameworkStores<AppIdentityDbContext>();
        builder.AddSignInManager<SignInManager<User>>();
        
        //Override the configuration 
        services.Configure<IdentityOptions>(opt =>
        {
            // Password settings 
            opt.Password.RequireDigit = false;
            opt.Password.RequiredLength = 10;
 
            // Lockout settings 
            opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(60);
            opt.Lockout.MaxFailedAccessAttempts = 3;
 
            //Signin option
            opt.SignIn.RequireConfirmedEmail = false;
 
            // User settings 
            opt.User.RequireUniqueEmail = true;
        });

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Secret!)),
                    ValidIssuer = config.Issuer,
                    ValidateIssuer = true,
                    ValidateAudience = false,
                };
            });
    }
}