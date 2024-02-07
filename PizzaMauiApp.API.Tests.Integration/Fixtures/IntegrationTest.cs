using System.Data.Common;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PizzaMauiApp.API.Helpers.EnvironmentConfig;
using PizzaMauiApp.API.Infrastructure.Data;
using PizzaMauiApp.API.Infrastructure.EnvironmentConfig;
using PizzaMauiApp.API.Infrastructure.Identity;

namespace PizzaMauiApp.API.Tests.Integration.Fixtures;

public class IntegrationTestFixture
{
    public HttpClient Client => _client;
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;
    
    public IntegrationTestFixture()
    {
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var dbStoreContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(DbContextOptions<ApplicationDbContext>));
                var dbIdentityContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(DbContextOptions<AppIdentityDbContext>));
                
                services.Remove(dbStoreContextDescriptor);
                services.Remove(dbIdentityContextDescriptor);
                
                var dbConnectionDescriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(DbConnection));
                
                services.Remove(dbConnectionDescriptor);

                // Create open SqliteConnection so EF won't automatically close it.
                services.AddSingleton<DbConnection>(container =>
                {
                    var connection = new SqliteConnection("DataSource=:memory:");
                    connection.Open();

                    return connection;
                });

                services.AddDbContext<ApplicationDbContext>((container, options) =>
                {
                    var connection = container.GetRequiredService<DbConnection>();
                    options.UseSqlite(connection);
                });
                
                services.AddDbContext<AppIdentityDbContext>((container, options) =>
                {
                    var connection = container.GetRequiredService<DbConnection>();
                    options.UseSqlite(connection);
                });
            });
        });

        _client = _factory.CreateClient();
    }
    
    [CollectionDefinition("integrationTest collection")]
    public class IntegrationTestCollection : ICollectionFixture<IntegrationTestFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}