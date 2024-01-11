namespace PizzaMauiApp.API.Dtos;

public class PizzaProductDto
{
    public string Id { get; set; }
    
    public string Name { get; set; }
    
    public string Ingredients { get; set; }
    
    public string Description { get; set; }
    
    public double PriceWithExcludedVAT { get; set; }
    
    public string MainImageUrl { get; set; }
    
    public ICollection<string> ProductImages  {get; set; }
}