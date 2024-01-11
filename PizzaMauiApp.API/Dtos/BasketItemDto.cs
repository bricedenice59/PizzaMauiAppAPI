using System.ComponentModel.DataAnnotations;

namespace PizzaMauiApp.API.Dtos;

public class BasketItemDto
{
    public required Guid Id { get; set; }
    
    public required string ProductName { get; set; }
    
    [Range(0.1, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
    public required decimal Price { get; set; }
    
    [Range(1, double.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public required int Quantity { get; set; }

    public required string PictureUrl { get; set; }
}