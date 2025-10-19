using Messages.Events;
using Shipping.NServiceBus.Messages;

namespace Shipping.NServiceBus;

public class ShippingSagaData : ContainSagaData
{
    public Guid OrderId { get; set; }
    public bool Paid { get; set; }
}

public class ShippingSaga(ILogger<ShippingSaga> logger) : Saga<ShippingSagaData>,
    IAmStartedByMessages<OrderAccepted>,
    IHandleMessages<PaymentReceived>,
    IHandleMessages<OrderShipped>
{
    public Task Handle(OrderAccepted message, IMessageHandlerContext context)
    {
        logger.LogInformation("⌛ Order {OrderId} accepted, waiting for payment.", message.OrderId);
        Data.OrderId = message.OrderId;
        return Task.CompletedTask;
    }

    public async Task Handle(PaymentReceived message, IMessageHandlerContext context)
    {
        logger.LogInformation("💰 Order {OrderId} was paid, dispatching shipping order.", message.OrderId);
        Data.Paid = true;
        await context.SendLocal(new ShipOrder { OrderId = message.OrderId, Address = Guid.NewGuid().ToString() });
    }

    public Task Handle(OrderShipped message, IMessageHandlerContext context)
    {
        logger.LogInformation("✅ Order {OrderId} was shipped, completing saga.", message.OrderId);
        if (Data is { Paid: true })
        {
            MarkAsComplete();
        }

        return Task.CompletedTask;
    }
    
    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<ShippingSagaData> mapper)
    {
        mapper.MapSaga(saga => saga.OrderId)
            .ToMessage<OrderAccepted>(m => m.OrderId)
            .ToMessage<PaymentReceived>(m => m.OrderId)
            .ToMessage<OrderShipped>(m => m.OrderId);
    }
}