using PizzaMauiApp.API.Core.Models;

namespace PizzaMauiApp.API.Core.Specifications;

public class GetAllProductPizzaSpecification : BaseSpecification<PizzaProduct>
{
    public GetAllProductPizzaSpecification() : base()
    {
        HasEntityTracking = false;
        AddInclude(x=>x.ProductImages);
    }
}