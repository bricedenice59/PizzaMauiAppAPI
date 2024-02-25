using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PizzaMauiApp.API.Core.EntityFramework;
using PizzaMauiApp.API.Core.Environment;
using PizzaMauiApp.API.Core.Services;
using PizzaMauiApp.API.DbIdentity;
using PizzaMauiApp.API.DbStore;
using PizzaMauiApp.API.Extensions;
using PizzaMauiApp.API.Helpers;
using PizzaMauiApp.API.Middlewares;
using PizzaMauiApp.API.Models.Identity;
using PizzaMauiApp.API.Repositories;
using PizzaMauiApp.API.SeedData;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

//decode configuration environment variables;
#if DEBUG
    var dbStoreConnectionConfig = new DbConnectionConfig(configuration, "Store");
    var dbStoreIdentityConfig = new DbConnectionConfig(configuration, "Identity");
    var dbRedisConfig = new DbConnectionConfig(configuration, "Redis");
#else
    var dbStoreConnectionConfig = new DbConnectionConfig("store_db");
    var dbStoreIdentityConfig = new DbConnectionConfig("identity_db");
    var dbRedisConfig = new DbConnectionConfig("redis");
#endif
builder.Services.AddSingleton<DbConnectionConfig>(dbStoreConnectionConfig);

var connectionStore = dbStoreConnectionConfig.ToString();
var connectionIdentity = dbStoreIdentityConfig.ToString();

TokenAuth0Config tokenAuth0Config = new();
configuration.GetSection("Auth0").Bind(tokenAuth0Config);
#if DEBUG
    tokenAuth0Config.Issuer = configuration["Auth0Issuer"];
    tokenAuth0Config.Secret = configuration["Auth0Secret"];
#else
    var tokenManagerSecrets = DockerUtils.GetSecrets<TokenAuth0Config>("auth_token");
    tokenAuth0Config.Issuer = tokenManagerSecrets?.Issuer;
    tokenAuth0Config.Secret = tokenManagerSecrets?.Secret;
#endif

if (string.IsNullOrEmpty(tokenAuth0Config.Issuer))
    throw new ArgumentNullException(nameof(tokenAuth0Config.Issuer),"Setting is missing: Auth0:Issuer; Add Auth0Issuer key in dotnet user-secrets or in docker secrets for this project");
if (string.IsNullOrEmpty(tokenAuth0Config.Secret))
    throw new ArgumentNullException(nameof(tokenAuth0Config.Secret),"Setting is missing: Auth0:Secret; Add Auth0Secret key in dotnet user-secrets or in docker secrets for this project");
if (tokenAuth0Config.TokenExpirationDelay == 0)
    throw new ArgumentNullException(nameof(tokenAuth0Config.TokenExpirationDelay),"Setting is missing: Auth0:TokenExpirationDelay");
if (tokenAuth0Config.RefreshTokenValidityInDays == 0)
    throw new ArgumentNullException(nameof(tokenAuth0Config.RefreshTokenValidityInDays),"Setting is missing: Auth0:RefreshTokenValidityInDays");
builder.Services.AddSingleton<TokenAuth0Config>(tokenAuth0Config);

// Add services to the container.
builder.Services.AddScoped(typeof(StoreDbRepository<>),typeof(StoreDbRepository<>));
builder.Services.AddScoped(typeof(IBasketRepository),(typeof(BasketRepository)));
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddAutoMapper(typeof(MappingProfiles));
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Db Context options
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionStore));
builder.Services.AddDbContext<AppIdentityDbContext>(options =>
    options.UseNpgsql(connectionIdentity));

builder.Services.AddSingleton<IConnectionMultiplexer>(option =>
    ConnectionMultiplexer.Connect(new ConfigurationOptions{
        EndPoints = {$"{dbRedisConfig.Host}:{dbRedisConfig.Port}"},
        AbortOnConnectFail = false,
        Ssl = false,
        Password = dbRedisConfig.Password
    }));

builder.Services.AddIdentityServices(configuration, tokenAuth0Config);

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<JwtTokensMiddleware>();

#region For testing purposes, let this code uncommented

// using (var scope = app.Services.CreateScope())
// {
//     var services = scope.ServiceProvider;
//     var loggerFactory = services.GetRequiredService<ILoggerFactory>();
//
//     try
//     {
//         var context = services.GetRequiredService<ApplicationDbContext>();
//         await context.Database.MigrateAsync();
//         await StoreDbContextSeed.SeedAsync(context, loggerFactory);
//         
//         var userManager = services.GetRequiredService<UserManager<User>>();
//         var identityContext = services.GetRequiredService<AppIdentityDbContext>();
//         await identityContext.Database.MigrateAsync();
//         await IdentityDbContextSeed.SeedAsync(userManager);
//     }
//     catch (Exception ex)
//     {
//         var logger = loggerFactory.CreateLogger<Program>();
//         logger.LogError(ex, "An error occured during database creation");
//     }
// }

#endregion

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStatusCodePagesWithReExecute("/errors/{0}");

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.Run();

// Make the implicit Program class public so test projects can access it
namespace PizzaMauiApp.API
{
    public partial class Program { }
}