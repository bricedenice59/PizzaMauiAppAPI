namespace PizzaMauiApp.RabbitMq.Messages;

public interface IOrderItem
{
    public Guid ItemId { get; set; }
    public int Quantity { get; set; }
}