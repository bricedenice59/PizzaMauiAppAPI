using AutoMapper;
using PizzaMauiApp.API.Core.Models;
using PizzaMauiApp.API.Dtos;

namespace PizzaMauiApp.API.Helpers;

public class ProductUrlResolver(IConfiguration config) : IValueResolver<PizzaProduct, PizzaProductDto, string?>
{
    public string? Resolve(PizzaProduct source, PizzaProductDto destination, string? destMember, ResolutionContext context)
    {
        if (!string.IsNullOrEmpty(source.MainImageUrl)) 
        {
            return config["ApiUrl"] + source.MainImageUrl;
        }
        return null;
    }   
}