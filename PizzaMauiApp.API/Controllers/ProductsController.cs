using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PizzaMauiApp.API.Core.Interfaces;
using PizzaMauiApp.API.Core.Models;
using PizzaMauiApp.API.Dtos;
using PizzaMauiApp.API.Helpers.API;

namespace PizzaMauiApp.API.Controllers;

public class PizzaProductsController : BaseApiController
{
    private readonly IGenericRepository<PizzaProduct> _productRepository; 
    private readonly IMapper _mapper;

    public PizzaProductsController(IGenericRepository<PizzaProduct> productRepository,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }
    
    [HttpGet]
    public async Task<ApiResponse<IReadOnlyList<PizzaProductDto>>> GetAllProducts()
    {
        var pizzaProducts = await _productRepository.ListAllAsync();

        var pizzaProductsDto = _mapper.Map<IReadOnlyList<PizzaProduct>, IReadOnlyList<PizzaProductDto>>(pizzaProducts);
        return new ApiResponse<IReadOnlyList<PizzaProductDto>>(200, pizzaProductsDto);
    }
}