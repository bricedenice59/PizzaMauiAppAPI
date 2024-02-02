using Microsoft.EntityFrameworkCore;
using PizzaMauiApp.API.Core.Interfaces;
using PizzaMauiApp.API.Extensions;
using PizzaMauiApp.API.Helpers;
using PizzaMauiApp.API.Helpers.EnvironmentConfig;
using PizzaMauiApp.API.Infrastructure;
using PizzaMauiApp.API.Infrastructure.Data;
using PizzaMauiApp.API.Infrastructure.EnvironmentConfig;
using PizzaMauiApp.API.Infrastructure.Identity;
using PizzaMauiApp.API.Infrastructure.Services;
using PizzaMauiApp.API.Middlewares;
using StackExchange.Redis;
using DbConnectionConfig = PizzaMauiApp.API.Helpers.EnvironmentConfig.DbConnectionConfig;

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
    throw new ArgumentNullException("Setting is missing: Auth0:Issuer; Add Auth0Issuer key in dotnet user-secrets or in docker secrets for this project");
if (string.IsNullOrEmpty(tokenAuth0Config.Secret))
    throw new ArgumentNullException("Setting is missing: Auth0:Secret; Add Auth0Secret key in dotnet user-secrets or in docker secrets for this project");
if (tokenAuth0Config.TokenExpirationDelay == 0)
    throw new ArgumentNullException("Setting is missing: Auth0:TokenExpirationDelay");
if (tokenAuth0Config.RefreshTokenValidityInDays == 0)
    throw new ArgumentNullException("Setting is missing: Auth0:RefreshTokenValidityInDays");
builder.Services.AddSingleton<TokenAuth0Config>(tokenAuth0Config);

// Add services to the container.
builder.Services.AddScoped(typeof(IGenericRepository<>),typeof(GenericRepository<>));
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

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();

    try
    {
        // var context = services.GetRequiredService<ApplicationDbContext>();
        // context.Database.EnsureCreated();
        // await StoreContextSeed.SeedAsync(context, loggerFactory);
        
        // var identityContext = services.GetRequiredService<AppIdentityDbContext>();
        // identityContext.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogError(ex, "An error occured during database creation");
    }
}

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