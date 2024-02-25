namespace PizzaMauiApp.API.Models;

public class CustomerBasket
{
    public CustomerBasket()
    {
        
    }
    public CustomerBasket(string id)
    {
        CustomerId = id;
    }

    public string CustomerId { get; set; }
    public List<BasketItem> Items { get; set; } = new();
}