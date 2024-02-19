using MassTransit;
using Microsoft.AspNetCore.Mvc;
using PizzaMauiApp.API.Dtos;
using PizzaMauiApp.API.Dtos.Enums;
using PizzaMauiApp.RabbitMq.Messages;

namespace PizzaMauiApp.API.Orders.Controllers;

public class OrdersController : BaseApiController
{
    private readonly IBus _bus;
    
    public OrdersController(IBus bus)
    {
        _bus = bus;

    }
    
    [HttpPost]
    public async Task<IActionResult> CreateOrder(OrderDto orderDto)
    {
        if (orderDto is not null)
        {
            await _bus.Publish<IOrderMessage>(new
            {
                OrderId = orderDto.Id,
                UserId = orderDto.UserId,
                Items = orderDto.OrderItems,
                Status = OrderStatus.New
            });
            return Ok();
        }
        return BadRequest();
    }
}