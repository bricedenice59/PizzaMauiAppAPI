using AutoMapper;
using PizzaMauiApp.API.Core.Models;
using PizzaMauiApp.API.Dtos;

namespace PizzaMauiApp.API.Helpers;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<PizzaProduct, PizzaProductDto>()
            .ForMember(destination=> destination.ProductImages, o
                =>o.MapFrom(s=>s.ProductImages.Select(x=>x.Url)))
            .ForMember(d => d.MainImageUrl, 
                o => o.MapFrom<ProductUrlResolver>())
            .ForMember(d => d.ProductImages, 
                o => o.MapFrom<ProductPicturesUrlResolver>())
            ;
        
        CreateMap<Core.Models.Identity.UserAddress, UserAddressDto>().ReverseMap();
        CreateMap<CustomerBasketDto, CustomerBasket>();
        CreateMap<BasketItemDto, BasketItem>();
    }
}