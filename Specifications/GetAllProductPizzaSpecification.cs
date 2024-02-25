using PizzaMauiApp.API.Core.EntityFramework.Specifications;
using PizzaMauiApp.API.Models;

namespace PizzaMauiApp.API.Specifications;

public class GetAllProductPizzaSpecification : BaseSpecification<PizzaProduct>
{
    public GetAllProductPizzaSpecification() : base()
    {
        HasEntityTracking = false;
        AddInclude(x=>x.ProductImages);
    }
}