using PizzaMauiApp.API.Core.EntityFramework;

namespace PizzaMauiApp.API.Models;

public class PizzaProductImage : BaseModel
{
    public required Guid ProductId { get; set; }
    
    public required string Url{ get; set; }
}