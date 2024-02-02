using AutoMapper;
using PizzaMauiApp.API.Core.Models;
using PizzaMauiApp.API.Dtos;
using PizzaMauiApp.API.Helpers.EnvironmentConfig;

namespace PizzaMauiApp.API.Helpers;

public class ProductPicturesUrlResolver(DbConnectionConfig config)
    : IValueResolver<PizzaProduct, PizzaProductDto, ICollection<string>?>
{
    public ICollection<string>? Resolve(PizzaProduct source, PizzaProductDto destination, ICollection<string>? destMember, ResolutionContext context)
    {
        return source.ProductImages?
            .Where(x => !string.IsNullOrEmpty(x.Url))
            .Select(s => $"{config.Host}/web_images/{source.MainImageUrl}" + s.Url)
            .ToList();
    }   
}