using MassTransit;
using Messages.Events;
using Shipping.MassTransit.InternalCommands;

namespace Shipping.MassTransit;

public class ShippingState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public required State CurrentState { get; set; }
}

public class ShippingSaga : MassTransitStateMachine<ShippingState>
{
    public ShippingSaga(ILogger<ShippingSaga> logger)
    {
        InstanceState(x => x.CurrentState);

        Event(() => OrderAccepted, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => PaymentReceived, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => OrderShipped, x => x.CorrelateById(context => context.Message.OrderId));
        
        Initially(
            When(OrderAccepted)
                .Then(context => logger.LogInformation("⌛ Order {OrderId} accepted, waiting for payment.", context.Message.OrderId))
                .TransitionTo(Waiting));

        During(Waiting,
            When(PaymentReceived)
                .Then(context => logger.LogInformation("💰 Order {OrderId} was paid, dispatching shipping order.", context.Message.OrderId))
                .ThenAsync(context => context.Send(new ShipOrder
                    { OrderId = context.Saga.CorrelationId, Address = "some address" }))
                .TransitionTo(Paid));
        
        During(Paid,
                When(OrderShipped)
                .Then(context => logger.LogInformation("✅ Order {OrderId} was shipped, completing saga.", context.Message.OrderId))
                .Then(context => context.SetCompleted()));
    }
    
    public State Waiting { get; private set; } = null!;
    public State Paid { get; private set; } = null!;
    
    public Event<OrderAccepted> OrderAccepted { get; private set; } = null!;
    public Event<PaymentReceived> PaymentReceived { get; private set; } = null!;
    public Event<OrderShipped> OrderShipped { get; private set; } = null!;
}