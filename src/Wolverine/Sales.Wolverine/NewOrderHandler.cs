using Messages.Commands;
using Messages.Events;
using Wolverine;

namespace Sales;

public static class NewOrderHandler
{
    public static async ValueTask Handle(PlaceOrder message, IMessageBus bus, ILogger<PlaceOrder> logger)
    {
        logger.LogInformation("💸 A new order {OrderId} for amount {Amount} came in", message.Id, message.Amount);

        // Do all kinds of validation
        // Store order in database

        await bus.PublishAsync(new OrderPlaced
        {
            OrderId = message.Id,
            Amount = message.Amount,
            CustomerId = message.CustomerId
        });
    }
}