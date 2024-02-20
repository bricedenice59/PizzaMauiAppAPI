using MassTransit;

namespace PizzaMaui.API.Orders.Kitchen.Consumers
{
    public class CookOrderConsumerDefinition :
        ConsumerDefinition<CookOrderConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<CookOrderConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseMessageRetry(r => r.Intervals(500, 1000));
        }
    }
}