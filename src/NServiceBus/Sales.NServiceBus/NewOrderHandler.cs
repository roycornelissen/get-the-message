using Messages.Commands;
using Messages.Events;

namespace Sales.NServiceBus;

public class NewOrderHandler(ILogger<NewOrderHandler> logger):
    IHandleMessages<PlaceOrder>
{
    public async Task Handle(PlaceOrder message, IMessageHandlerContext context)
    {
        logger.LogInformation("💸 A new order {OrderId} for amount {Amount} came in", message.Id, message.Amount);

        // Do all kinds of validation
        // Store order in database

        await context.Publish<OrderPlaced>(evt =>
        {
            evt.OrderId = message.Id;
            evt.Amount = message.Amount;
            evt.CustomerId = message.CustomerId;
        });
    }
}