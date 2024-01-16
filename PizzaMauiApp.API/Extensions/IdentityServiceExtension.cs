using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PizzaMauiApp.API.Core.Models.Identity;
using PizzaMauiApp.API.Infrastructure.Identity;

namespace PizzaMauiApp.API.Extensions;

public static class IdentityServiceExtensions
{
    public static void AddIdentityServices(this IServiceCollection services,
        IConfiguration config)
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
        var auth0Secret = config["Auth0Secret"];
        var auth0Issuer = config["Auth0Issuer"];
        if (string.IsNullOrEmpty(auth0Secret))
            throw new ArgumentNullException("Setting is missing: Auth0:Issuer; Add Auth0Secret key in dotnet user-secrets for this project");
        if (string.IsNullOrEmpty(auth0Issuer))
            throw new ArgumentNullException("Setting is missing: Auth0:Issuer; Add Auth0Issuer key in dotnet user-secrets for this project");
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(auth0Secret)),
                    ValidIssuer = auth0Issuer,
                    ValidateIssuer = true,
                    ValidateAudience = false,
                };
            });
    }
}