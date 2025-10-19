using Messages.Events;

namespace Sales.NServiceBus;

public class CustomerStatusSagaData : ContainSagaData
{
    public decimal TotalPurchases { get; set; }
    public Guid CustomerId { get; set; }
    public bool IsPreferred { get; set; }
}

public class StatusExpirationTimeout: IMessage
{
    public decimal Amount { get; set; }
}

public class CustomerStatusPolicy(ILogger<CustomerStatusPolicy> logger) : Saga<CustomerStatusSagaData>,
    IAmStartedByMessages<OrderAccepted>,
    IHandleMessages<PaymentReceived>,
    IHandleTimeouts<StatusExpirationTimeout>
{
    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<CustomerStatusSagaData> mapper)
    {
        mapper.MapSaga(saga => saga.CustomerId)
            .ToMessage<OrderAccepted>(msg => msg.CustomerId)
            .ToMessage<PaymentReceived>(msg => msg.CustomerId);
    }

    public async Task Handle(OrderAccepted message, IMessageHandlerContext context)
    {
        if (Data.CustomerId != Guid.Empty)
        {
            // already exists
            return;
        }
        logger.LogInformation("👤 Customer {CustomerId} is now a regular customer.", message.CustomerId);
        Data.CustomerId = message.CustomerId;
        await Task.CompletedTask;
    }

    public async Task Handle(PaymentReceived message, IMessageHandlerContext context)
    {
        Data.CustomerId = message.CustomerId;
        Data.TotalPurchases += message.Amount;

        logger.LogInformation("🏅 Amount {Amount} purchased by Customer {CustomerId}, now has a total of {Total}", message.Amount, message.CustomerId, Data.TotalPurchases);

        if (Data is { TotalPurchases: > 1000, IsPreferred: false })
        {
            logger.LogInformation("🏅 Customer {CustomerId} is now a preferred customer!", Data.CustomerId);
            Data.IsPreferred = true;
            await context.Publish<CustomerBecamePreferred>(evt =>
            {
                evt.CustomerId = Data.CustomerId;
            });
        }

        await RequestTimeout(context, TimeSpan.FromMinutes(2), new StatusExpirationTimeout() { Amount = message.Amount });
    }

    public Task Timeout(StatusExpirationTimeout state, IMessageHandlerContext context)
    {
        Data.TotalPurchases -= state.Amount;
        logger.LogInformation("⌛ An amount of {Amount} for Customer {CustomerId} is no longer eligible for customer status, total is now {Total}", state.Amount, Data.CustomerId, Data.TotalPurchases);
        if (Data is { IsPreferred: true, TotalPurchases: <= 1000 })
        {
            logger.LogInformation("👤 Customer {CustomerId} is now a regular customer", Data.CustomerId);
            Data.IsPreferred = false;
        }
        return Task.CompletedTask;
    }
}