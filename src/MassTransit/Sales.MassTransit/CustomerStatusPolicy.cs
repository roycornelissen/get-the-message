using MassTransit;
using Messages.Events;

namespace Sales.MassTransit;

public class AmountExpired
{
    public Guid CustomerId { get; init; }
    public decimal Amount { get; init; }
}

// state that is kept in saga persistence
public class CustomerStatus : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public required string CurrentState { get; set; }
    public decimal TotalPurchases { get; set; }
}

public class CustomerStatusPolicy : MassTransitStateMachine<CustomerStatus>
{
    public CustomerStatusPolicy(ILogger<CustomerStatusPolicy> logger)
    {
        InstanceState(x => x.CurrentState);

        Event(() => OrderAccepted, x => x.CorrelateById(context => context.Message.CustomerId));
        Event(() => PaymentReceived, x => x.CorrelateById(context => context.Message.CustomerId));
        Event(() => AmountExpired, x => x.CorrelateById(context => context.Message.CustomerId));
        
        Initially(
            When(OrderAccepted)
                .TransitionTo(Regular));
        
        WhenEnter(Regular, callback => callback
            .Then(context => logger.LogInformation("👤 Customer {CustomerId} is now a regular customer.", context.Saga.CorrelationId)));
        
        DuringAny(
            Ignore(OrderAccepted),

            When(PaymentReceived)
                .ThenAsync(async context => { await TrackOrder(logger, context); }),

            When(AmountExpired)
                .Then(context =>
                {
                    context.Saga.TotalPurchases -= context.Message.Amount;
                    logger.LogInformation(
                        "⌛ An amount of {Amount} for Customer {CustomerId} is no longer eligible for customer status; total is now {Total}",
                        context.Message.Amount, context.Saga.CorrelationId, context.Saga.TotalPurchases);
                })
        );
        
        During(Regular,
            When(PaymentReceived)
                .If(
                    context => context.Saga.TotalPurchases > 1000,
                    then => then.TransitionTo(Preferred)));
        
        WhenEnter(Preferred, callback =>
            callback
                .Then(context => logger.LogInformation("🏆 Customer {CustomerId} became a preferred customer!", context.Saga.CorrelationId))
                .ThenAsync(context => context.Publish(new CustomerBecamePreferred
                {
                    CustomerId = context.Saga.CorrelationId
                })));

        During(Preferred,
            When(AmountExpired)
                .If(context => context.Saga.TotalPurchases <= 1000,
                    then => then.TransitionTo(Regular)
                ));
    }

    private static async Task TrackOrder(ILogger<CustomerStatusPolicy> logger, BehaviorContext<CustomerStatus, PaymentReceived> context)
    {
        context.Saga.TotalPurchases += context.Message.Amount;
        logger.LogInformation(
            "➕ Amount {Amount} purchased by Customer {CustomerId}, now has a total of {Total}",
            context.Message.Amount, context.Saga.CorrelationId, context.Saga.TotalPurchases);

        await context
            .SchedulePublish(TimeSpan.FromMinutes(1), new AmountExpired
            {
                CustomerId = context.Saga.CorrelationId,
                Amount = context.Message.Amount
            }, context.CancellationToken);
    }

    public State Regular { get; private set; }
    public State Preferred { get; private set; }

    public Event<OrderAccepted> OrderAccepted { get; private set; }
    public Event<PaymentReceived> PaymentReceived { get; private set; }
    public Event<AmountExpired> AmountExpired { get; private set; }
}