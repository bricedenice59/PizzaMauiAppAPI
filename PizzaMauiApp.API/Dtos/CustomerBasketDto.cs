namespace PizzaMauiApp.API.Dtos;

public class CustomerBasketDto
{
    public required Guid Id { get; set; }
    public List<BasketItemDto> Items { get; set; }
}