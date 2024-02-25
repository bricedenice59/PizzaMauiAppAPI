using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PizzaMauiApp.API.Core.EntityFramework;
using PizzaMauiApp.API.Dtos;
using PizzaMauiApp.API.Helpers.API;
using PizzaMauiApp.API.Models;
using PizzaMauiApp.API.Repositories;
using PizzaMauiApp.API.Specifications;

namespace PizzaMauiApp.API.Controllers;

public class PizzaProductsController : BaseApiController
{
    private readonly StoreDbRepository<PizzaProduct> _productRepository; 
    private readonly IMapper _mapper;

    public PizzaProductsController(StoreDbRepository<PizzaProduct> productRepository,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }
    
    [API.Attributes.Authorize]
    [HttpGet]
    public async Task<ApiResponse<IReadOnlyList<PizzaProductDto>>> GetAllProducts()
    {
        var specification = new GetAllProductPizzaSpecification();
        var pizzaProducts = await _productRepository.ListAllAsync(specification);

        var pizzaProductsDto = _mapper.Map<IReadOnlyList<PizzaProduct>, IReadOnlyList<PizzaProductDto>>(pizzaProducts);
        return new ApiResponse<IReadOnlyList<PizzaProductDto>>(StatusCodes.Status200OK, pizzaProductsDto);
    }
}