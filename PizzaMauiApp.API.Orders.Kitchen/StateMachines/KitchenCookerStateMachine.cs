using MassTransit;
using PizzaMaui.API.Orders.Kitchen.Contracts;
using RabbitMQ.Client.Logging;

namespace PizzaMaui.API.Orders.Kitchen.StateMachines
{
    public class KitchenCookerStateMachine :
        MassTransitStateMachine<KitchenCookerState> 
    {
        public KitchenCookerStateMachine()
        {
            InstanceState(x => x.CurrentState, Ordered);

            Event(() => KitchenCookerEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
            Event(() => KitchenBeginCookingEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
            Event(() => KitchenFinishCookingEvent, x => x.CorrelateById(context => context.Message.CorrelationId));

            Initially(
                When(KitchenCookerEvent)
                    .Then(context =>
                    {
                        context.Saga.UserId = context.Message.UserId;
                        context.Saga.OrderId = context.Message.OrderId;
                        context.Saga.Items = context.Message.Items;
                        
                        Console.WriteLine($"Order {context.Saga.OrderId} in kitchen...");
                        
                        var beginEvent = new KitchenBeginCookingEvent()
                        {
                            CorrelationId = context.Saga.CorrelationId
                        };
                        context.Publish(beginEvent); 
                    })
                    .TransitionTo(BeginCooking)
            );
            
            During(BeginCooking,
                When(KitchenBeginCookingEvent)
                    .Then(context =>
                    {
                        Console.WriteLine($"Order {context.Saga.OrderId} is being cooked...");
                        var cookOrder = new CookOrder { CorrelationId = context.Saga.CorrelationId };
                        context.Publish(cookOrder); 
                    })
                    .TransitionTo(FinishCooking)
            );

            During(FinishCooking,
                When(KitchenFinishCookingEvent)
                    .Then(context =>
                    {
                        Console.WriteLine($"Order {context.Saga.OrderId} has been cooked... ready for delivery");
                    })
                    .TransitionTo(OrderCompleted)
            );

            SetCompletedWhenFinalized();
        }

        public State Ordered { get; private set; }
        public State BeginCooking { get; private set; }
        public State FinishCooking { get; private set; }
        public State OrderCompleted { get; private set; }

        public Event<KitchenOrderEvent> KitchenCookerEvent { get; private set; }
        public Event<KitchenBeginCookingEvent> KitchenBeginCookingEvent { get; private set; }
        public Event<KitchenFinishCookingEvent> KitchenFinishCookingEvent { get; private set; }
    }
}