namespace PizzaMauiApp.API.Core.Models;

public class OrderItems : BaseModel
{
    public required Guid OrderId { get; set; }
    
    public Guid PizzaId { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
}