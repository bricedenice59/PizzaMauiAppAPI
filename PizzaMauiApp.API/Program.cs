using Microsoft.EntityFrameworkCore;
using Npgsql;
using PizzaMauiApp.API.Core.Interfaces;
using PizzaMauiApp.API.Helpers;
using PizzaMauiApp.API.Infrastructure;
using PizzaMauiApp.API.Infrastructure.Data;
using PizzaMauiApp.API.Infrastructure.SeedData;
using PizzaMauiApp.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

//build connection string with stored secrets in my local machine: dotnet user-secrets
//hostname, password from connection string are not stored as environment variables for obvious purposes
//ApiUrl is also not stored.
var conStrBuilder = new NpgsqlConnectionStringBuilder(
    configuration.GetConnectionString("DbDefaultConnection"))
{
    Password = builder.Configuration["DbPassword"],
    Host = builder.Configuration["DbHost"],
};
var connection = conStrBuilder.ConnectionString;

// Add services to the container.
builder.Services.AddScoped(typeof(IGenericRepository<>),(typeof(GenericRepository<>)));
builder.Services.AddAutoMapper(typeof(MappingProfiles));
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Application Db Context options
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connection));

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();

    try
    {
        // var context = services.GetRequiredService<ApplicationDbContext>();
        // context.Database.EnsureCreated();
        // await StoreContextSeed.SeedAsync(context, loggerFactory);
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

app.UseStaticFiles();

app.MapControllers();

app.Run();