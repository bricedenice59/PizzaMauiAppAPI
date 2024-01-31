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

        CreateMap<CustomerBasket, CustomerBasketDto>()
            .ForMember(destination => destination.CustomerId, o
                => o.MapFrom(src => src.CustomerId))
            .ForMember(destination => destination.Items, o
                => o.MapFrom(src => src.Items.Select(z => new BasketItemDto()
                {
                    Id = z.Id,
                    Quantity = z.Quantity
                }).ToList()));

        CreateMap<CustomerBasketDto, CustomerBasket>()
            .ForMember(destination => destination.CustomerId, o
                => o.MapFrom(src => src.CustomerId))
            .ForMember(destination => destination.Items, o
                => o.MapFrom(src => src.Items.Select(z => new BasketItem()
                {
                    Id = z.Id,
                    Quantity = z.Quantity
                }).ToList()));
        
        CreateMap<Core.Models.Identity.UserAddress, UserAddressDto>().ReverseMap();

        CreateMap<BasketItemDto, BasketItem>();
    }
}