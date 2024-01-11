namespace PizzaMauiApp.API.Core.Models;

public class BasketItem
{
    public Guid Id { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}