using PizzaMauiApp.API.Core.Enums;

namespace PizzaMauiApp.API.Core.Models;

public class OrderStatusUpdate : BaseModel
{
    public required Guid OrderId { get; set; }
    public required DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public required OrderStatus Status { get; set; }
}