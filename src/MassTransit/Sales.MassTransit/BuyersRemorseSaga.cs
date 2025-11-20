using System.Linq.Expressions;
using MassTransit;
using Messages.Events;

namespace Sales.MassTransit;

public class OrderPlaced : CorrelatedBy<Guid>
{
    public Guid CorrelationId { get; set; }
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public Guid CustomerId { get; set; }
}

public class BuyersRemorseSaga: ISaga,
    InitiatedByOrOrchestrates<OrderPlaced>,
    Observes<OrderCanceled, BuyersRemorseSaga>
{
    public Guid CorrelationId { get; set; }
    public bool Canceled { get; set; }
    public bool Accepted { get; set; }

    public async Task Consume(ConsumeContext<OrderPlaced> context)
    {
        var logger = context.GetServiceOrCreateInstance<ILogger<BuyersRemorseSaga>>();
        if (context.GetRedeliveryCount() == 0)
        {
            CorrelationId = context.Message.CorrelationId;
            logger.LogInformation("🕛 Started Buyer's Remorse Saga for OrderId {OrderId}", CorrelationId);
            await context.Redeliver(TimeSpan.FromSeconds(10));
        }
        else
        {
            if (!Canceled)
            {
                logger.LogInformation("🕛 Time's up for OrderId {OrderId}, finalizing order", CorrelationId);
                Accepted = true;
                await context.Publish(new OrderAccepted
                {
                    OrderId = CorrelationId,
                    CustomerId = context.Message.CustomerId,
                    Amount = context.Message.Amount
                });
            }
            else
            {
                logger.LogInformation("🙈 Ignoring redelivery of OrderPlaced for OrderId {OrderId} - order was already canceled", CorrelationId);
            }
        }
    }

    public Task Consume(ConsumeContext<OrderCanceled> context)
    {
        var logger = context.GetServiceOrCreateInstance<ILogger<BuyersRemorseSaga>>();
        if (!Accepted)
        {
            logger.LogInformation("🛑 Order {OrderId} was canceled within Buyer's Remorse Period", CorrelationId);
            Canceled = true;
        }
        else
        {
            logger.LogInformation("🙈 Order {OrderId} was already accepted, ignoring OrderCanceled", CorrelationId);
        }
        return Task.CompletedTask;
    }

    public Expression<Func<BuyersRemorseSaga, OrderCanceled, bool>> CorrelationExpression =>
        (saga, message) => saga.CorrelationId == message.OrderId;
}