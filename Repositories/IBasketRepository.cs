using PizzaMauiApp.API.Models;

namespace PizzaMauiApp.API.Repositories;

public interface IBasketRepository
{
    Task<CustomerBasket?> GetCustomerBasket(string customerId);

    Task<CustomerBasket?> UpdateCustomerBasket(CustomerBasket basket);

    Task<bool> DeleteCustomerBasket(string customerId);
}