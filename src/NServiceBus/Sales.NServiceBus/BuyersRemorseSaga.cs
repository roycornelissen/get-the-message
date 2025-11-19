using Messages.Events;

namespace Sales.NServiceBus;

public class BuyersRemorseSagaData: ContainSagaData
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public decimal Amount { get; set; }
}

public class BuyersRemorseExpired : IMessage
{
}

public class BuyersRemorseSaga(ILogger<BuyersRemorseSaga> logger): Saga<BuyersRemorseSagaData>,
    IAmStartedByMessages<OrderPlaced>,
    IHandleMessages<OrderCanceled>,
    IHandleTimeouts<BuyersRemorseExpired>
{
    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<BuyersRemorseSagaData> mapper)
    {
        mapper.MapSaga(m => m.OrderId)
            .ToMessage<OrderPlaced>(msg => msg.OrderId)
            .ToMessage<OrderCanceled>(msg => msg.OrderId);
    }

    public async Task Handle(OrderPlaced message, IMessageHandlerContext context)
    {
        logger.LogInformation("🕛 Started Buyer's Remorse Saga for OrderId {OrderId}", message.OrderId);

        Data.OrderId = message.OrderId;
        Data.CustomerId = message.CustomerId;
        Data.Amount = message.Amount;
        
        await RequestTimeout<BuyersRemorseExpired>(context, TimeSpan.FromSeconds(10));
    }

    public Task Handle(OrderCanceled message, IMessageHandlerContext context)
    {
        logger.LogInformation("🛑 Order {OrderId} was canceled within Buyer's Remorse Period", message.OrderId);
        MarkAsComplete();
        return Task.CompletedTask;
    }

    public async Task Timeout(BuyersRemorseExpired state, IMessageHandlerContext context)
    {
        logger.LogInformation("🕛 Time's up for OrderId {OrderId}, finalizing order", Data.OrderId);
        await context.Publish<OrderAccepted>(evt =>
        {
            evt.OrderId = Data.OrderId;
            evt.CustomerId = Data.CustomerId;
            evt.Amount = Data.Amount;
        });
        MarkAsComplete();
    }
}
