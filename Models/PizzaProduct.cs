using PizzaMauiApp.API.Core.EntityFramework;

namespace PizzaMauiApp.API.Models;

public class PizzaProduct : BaseModel
{
    public required string Name { get; set; }
    public required string MainImageUrl { get; set; }
    public required double PriceWithExcludedVAT { get; set; }
    public required string Description { get; set; }
    public required string Ingredients { get; set; }
    
    public ICollection<PizzaProductImage> ProductImages { get; set; }
}