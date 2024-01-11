namespace PizzaMauiApp.API.Core.Models;

public class CustomerBasket
{
    public CustomerBasket()
    {
        
    }
    public CustomerBasket(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }
    public List<BasketItem> Items { get; set; } = new();
}