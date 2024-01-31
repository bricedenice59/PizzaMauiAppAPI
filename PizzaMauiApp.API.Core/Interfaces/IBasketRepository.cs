using PizzaMauiApp.API.Core.Models;

namespace PizzaMauiApp.API.Core.Interfaces;

public interface IBasketRepository
{
    Task<CustomerBasket?> GetCustomerBasket(string customerId);

    Task<CustomerBasket?> UpdateCustomerBasket(CustomerBasket basket);

    Task<bool> DeleteCustomerBasket(string customerId);
}