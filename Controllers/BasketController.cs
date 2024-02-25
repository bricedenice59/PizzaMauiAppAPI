using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PizzaMauiApp.API.Dtos;
using PizzaMauiApp.API.Helpers.API;
using PizzaMauiApp.API.Models;
using PizzaMauiApp.API.Repositories;

namespace PizzaMauiApp.API.Controllers;

public class BasketController : BaseApiController
{
    private readonly IBasketRepository _basketRepository;
    private readonly IMapper _mapper;

    public BasketController(IBasketRepository basketRepository, IMapper mapper)
    {
        _basketRepository = basketRepository;
        _mapper = mapper;
    }

    [API.Attributes.Authorize]
    [HttpGet]
    public async Task<ApiResponse<CustomerBasketDto>> GetBasketByUserEmail(string customerId)
    {
        var customerBasket = await _basketRepository.GetCustomerBasket(customerId);
        var customBasketDto = _mapper.Map<CustomerBasketDto>(customerBasket);
        return new ApiResponse<CustomerBasketDto>(StatusCodes.Status200OK, customBasketDto ?? new CustomerBasketDto{CustomerId = customerId});
    }

    [API.Attributes.Authorize]
    [HttpPost]
    public async Task<ApiResponse<bool>> UpdateBasket(CustomerBasketDto basketDto)
    {
        var customerBasket = _mapper.Map<CustomerBasket>(basketDto);
        var updatedBasket = await _basketRepository.UpdateCustomerBasket(customerBasket);
        if(updatedBasket is null)
            return new ApiResponse<bool>(400, $"Basket for guid {basketDto.CustomerId} has not been updated");
        
        return new ApiResponse<bool>(StatusCodes.Status200OK, true);
    }

    [API.Attributes.Authorize]
    [HttpDelete]
    public async Task<ApiResponse<bool>> DeleteBasket(string customerId)
    {
        var success = await _basketRepository.DeleteCustomerBasket(customerId);
        return success
            ? new ApiResponse<bool>(StatusCodes.Status200OK, success)
            : new ApiResponse<bool>(StatusCodes.Status400BadRequest, $"Basket for customerId {customerId} could not be deleted");
    }
}
