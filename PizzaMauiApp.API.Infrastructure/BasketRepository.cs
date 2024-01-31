using System.Text.Json;
using PizzaMauiApp.API.Core.Interfaces;
using PizzaMauiApp.API.Core.Models;
using StackExchange.Redis;

namespace PizzaMauiApp.API.Infrastructure;

public class BasketRepository : IBasketRepository
{
    private readonly IDatabase _database;
    
    public BasketRepository(IConnectionMultiplexer connectionMultiplexer)
    {
        _database = connectionMultiplexer.GetDatabase();
    }
    
    public async Task<CustomerBasket?> GetCustomerBasket(string customerId)
    {
        var data = await _database.StringGetAsync(customerId);
        return data.IsNullOrEmpty ? null : JsonSerializer.Deserialize<CustomerBasket>(data);
    }

    public async Task<CustomerBasket?> UpdateCustomerBasket(CustomerBasket basket)
    {
        var created = await _database.StringSetAsync(basket.CustomerId, 
            JsonSerializer.Serialize(basket), 
            TimeSpan.FromDays(2));
        return !created ? null : await GetCustomerBasket(basket.CustomerId);
    }

    public async Task<bool> DeleteCustomerBasket(string customerId)
    {
        return await _database.KeyDeleteAsync(customerId);
    }
}