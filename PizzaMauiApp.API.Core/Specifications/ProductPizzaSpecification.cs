using PizzaMauiApp.API.Core.Models;

namespace PizzaMauiApp.API.Core.Specifications;

public class ProductPizzaSpecification : BaseSpecification<PizzaProduct>
{
    public ProductPizzaSpecification(Guid id) : base(x => x.Id == id)
    {
        AddInclude(x=>x.ProductImages);
    }
}