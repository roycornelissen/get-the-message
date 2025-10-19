using Messages.Commands;
using Messages.Events;

namespace Billing.NServiceBus;

public class OrderBillingData : ContainSagaData
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public decimal Amount { get; set; }
}

public class OrderBillingSaga(ILogger<OrderBillingSaga> logger): Saga<OrderBillingData>,
    IAmStartedByMessages<OrderAccepted>,
    IHandleMessages<ProcessPayment>
{
    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<OrderBillingData> mapper)
    {
        mapper.MapSaga(s => s.OrderId)
            .ToMessage<OrderAccepted>(m => m.OrderId)
            .ToMessage<ProcessPayment>(m => m.OrderId);
    }

    public Task Handle(OrderAccepted message, IMessageHandlerContext context)
    {
        logger.LogInformation("💰 Billing order {OrderId} for amount {Amount}", message.OrderId, message.Amount);
        
        Data.OrderId = message.OrderId;
        Data.CustomerId = message.CustomerId;
        Data.Amount = message.Amount;
        return Task.CompletedTask;
    }

    public async Task Handle(ProcessPayment message, IMessageHandlerContext context)
    {
        logger.LogInformation("💳 Processing payment for order {OrderId}", message.OrderId);
        await context.Publish<PaymentReceived>(evt =>
        {
            evt.CustomerId = Data.CustomerId;
            evt.OrderId = Data.OrderId;
            evt.Amount = Data.Amount;
        });
        MarkAsComplete();
    }
}