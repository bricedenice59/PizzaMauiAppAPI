namespace PizzaMauiApp.API.Core.Models;

public class Order : BaseModel
{
    public string UserId { get; set; } = null!;
    
    public ICollection<OrderItems> OrderItems { get; } = new List<OrderItems>(); 

    public ICollection<OrderStatusUpdate> OrdersStatusHistory { get; } = new List<OrderStatusUpdate>(); 
}