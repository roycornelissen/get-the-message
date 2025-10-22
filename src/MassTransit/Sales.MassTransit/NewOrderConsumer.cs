using MassTransit;
using Messages.Commands;

namespace Sales.MassTransit;

public class NewOrderConsumer(ILogger<NewOrderConsumer> logger) : IConsumer<PlaceOrder>
{
    public async Task Consume(ConsumeContext<PlaceOrder> context)
    {
        logger.LogInformation("💸 A new order {OrderId} for amount {Amount} came in", context.Message.Id, context.Message.Amount);

        // Do all kinds of validation
        // Store order in database

        await context.Publish(new OrderPlaced
        {
            CorrelationId = context.Message.Id,
            OrderId = context.Message.Id,
            Amount = context.Message.Amount,
            CustomerId = context.Message.CustomerId
        }, context.CancellationToken);
    }
}