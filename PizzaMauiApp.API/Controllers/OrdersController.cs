using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PizzaMauiApp.API.Core.Interfaces;
using PizzaMauiApp.API.Core.Models;
using PizzaMauiApp.API.Core.Specifications;
using PizzaMauiApp.API.Dtos;
using PizzaMauiApp.API.Helpers.API;

namespace PizzaMauiApp.API.Controllers;

public class OrdersController : BaseApiController
{

    private readonly IMapper _mapper;

    public OrdersController(IMapper mapper)
    {
        _mapper = mapper;
    }
    
}