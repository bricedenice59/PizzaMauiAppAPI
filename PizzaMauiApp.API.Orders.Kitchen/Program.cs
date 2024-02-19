using MassTransit;
using PizzaMaui.API.Orders.Kitchen.Consumers;
using PizzaMauiApp.API.Core.EnvironmentConfig;


var builder = WebApplication.CreateBuilder(args);

//decode configuration environment variables;
var rabbitMqConnectionConfig = new DbConnectionConfig(builder.Configuration, "RabbitMq");

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    x.AddConsumer<OrderCreatedConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host($"rabbitmq://{rabbitMqConnectionConfig.Host}:{rabbitMqConnectionConfig.Port}", hostconfig =>
        {
            hostconfig.Username(rabbitMqConnectionConfig.Username);
            hostconfig.Password(rabbitMqConnectionConfig.Password);
        });
        cfg.ConfigureEndpoints(context);
    }); 
});

var app = builder.Build();

app.Run();

