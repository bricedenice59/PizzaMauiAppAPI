using Microsoft.EntityFrameworkCore;
using Npgsql;
using PizzaMauiApp.API.Core.Interfaces;
using PizzaMauiApp.API.Extensions;
using PizzaMauiApp.API.Helpers;
using PizzaMauiApp.API.Infrastructure;
using PizzaMauiApp.API.Infrastructure.Data;
using PizzaMauiApp.API.Infrastructure.Identity;
using PizzaMauiApp.API.Infrastructure.Services;
using PizzaMauiApp.API.Middlewares;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

//build connection string with stored secrets in my local machine: dotnet user-secrets
//hostname, password from connection strings are not stored as environment variables for obvious purposes
//ApiUrl is also not stored.
var conStoreStrBuilder = new NpgsqlConnectionStringBuilder(
    configuration.GetConnectionString("StoreDbDefaultConnection"))
{
    Password = builder.Configuration["DbStorePassword"],
    Host = builder.Configuration["DbStoreHost"],
};
var connectionStore = conStoreStrBuilder.ConnectionString;

var conIdentityStrBuilder = new NpgsqlConnectionStringBuilder(
    configuration.GetConnectionString("UserDbDefaultConnection"))
{
    Password = builder.Configuration["DbIdentityPassword"],
    Host = builder.Configuration["DbIdentityHost"],
};
var connectionIdentity = conIdentityStrBuilder.ConnectionString;

var redisPassword = builder.Configuration["RedisDbStorePassword"];
var redisHost = builder.Configuration["DbStoreHost"];
var redisPort = builder.Configuration["RedisDbStorePort"];

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

RedisConnection redisConnection = new();
configuration.GetSection("RedisConnection").Bind(redisConnection);

builder.Services.AddSingleton<IConnectionMultiplexer>(option =>
    ConnectionMultiplexer.Connect(new ConfigurationOptions{
        EndPoints = {$"{redisHost}:{redisPort}"},
        AbortOnConnectFail = false,
        Ssl = redisConnection.IsSSL,
        Password = redisPassword
    }));

builder.Services.AddIdentityServices(configuration);

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

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.Run();