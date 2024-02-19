﻿using MassTransit;
using PizzaMauiApp.RabbitMq.Messages;

namespace PizzaMaui.API.Orders.Kitchen.Consumers
{
    public class OrderCreatedConsumer : IConsumer<IOrderMessage>
    {
        // private readonly ILogger<OrderCreatedConsumer> _logger;
        //
        // public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger)
        // {
        //     _logger = logger;
        // }
        
        public async Task Consume(ConsumeContext<IOrderMessage> context)
        {
            var data = context.Message;
            //_logger.LogInformation(context.Message.OrderId.ToString());
            Console.WriteLine(context.Message.OrderId.ToString());
            //Validate the Ticket Data
            //Store to Database
            //Notify the user via Email / SMS
            await Task.CompletedTask;
        }
    }
}