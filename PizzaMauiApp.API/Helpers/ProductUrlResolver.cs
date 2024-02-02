using AutoMapper;
using PizzaMauiApp.API.Core.Models;
using PizzaMauiApp.API.Dtos;
using PizzaMauiApp.API.Helpers.EnvironmentConfig;

namespace PizzaMauiApp.API.Helpers;

public class ProductUrlResolver(DbConnectionConfig config) : IValueResolver<PizzaProduct, PizzaProductDto, string?>
{
    public string? Resolve(PizzaProduct source, PizzaProductDto destination, string? destMember, ResolutionContext context)
    {
        if (!string.IsNullOrEmpty(source.MainImageUrl))
        {
            return $"{config.Host}/web_images/{source.MainImageUrl}";
        }
        return null;
    }   
}