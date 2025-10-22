using MassTransit;
using Messages.Commands;
using Messages.Events;

namespace Billing.MassTransit;

public class OrderBillingState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public required State CurrentState { get; set; }
    public Guid CustomerId { get; set; }
    public decimal Amount { get; set; }
}

public class OrderBillingSaga : MassTransitStateMachine<OrderBillingState>
{
    public OrderBillingSaga(ILogger<OrderBillingSaga> logger)
    {
        InstanceState(x => x.CurrentState);

        Event(() => OrderAccepted, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => ProcessPayment, x => x.CorrelateById(context => context.Message.OrderId));

        Initially(
        When(OrderAccepted)
                .Then(context =>
                    logger.LogInformation("⌛ Order {OrderId} accepted, waiting for payment.", context.Message.OrderId))
                .Then(context =>
                {
                    context.Saga.CorrelationId = context.Message.OrderId;
                    context.Saga.Amount = context.Message.Amount;
                    context.Saga.CustomerId = context.Message.CustomerId;
                })
                .TransitionTo(Waiting)
        );

        During(Waiting,
            Ignore(OrderAccepted),
            When(ProcessPayment)
                .Then(context => logger.LogInformation("💰 Order {OrderId} was paid, dispatching shipping order.",
                    context.Message.OrderId))
                .ThenAsync(context => context.Publish(new PaymentReceived
                {
                    OrderId = context.Saga.CorrelationId,
                    CustomerId = context.Saga.CustomerId,
                    Amount = context.Saga.Amount
                }))
                .Then(context => context.SetCompleted())
        );
    }
    
    public State Waiting { get; private set; }
    
    public Event<OrderAccepted> OrderAccepted { get; private set; }
    public Event<ProcessPayment> ProcessPayment { get; private set; }
}