using AutoMapper;
using PizzaMauiApp.API.Core.Models;
using PizzaMauiApp.API.Dtos;

namespace PizzaMauiApp.API.Helpers;

public class ProductPicturesUrlResolver(IConfiguration config)
    : IValueResolver<PizzaProduct, PizzaProductDto, ICollection<string>?>
{
    public ICollection<string>? Resolve(PizzaProduct source, PizzaProductDto destination, ICollection<string>? destMember, ResolutionContext context)
    {
        return source.ProductImages?
            .Where(x => !string.IsNullOrEmpty(x.Url))
            .Select(s => config["ApiUrl"] + s.Url)
            .ToList();
    }   
}