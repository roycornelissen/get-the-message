using Messages.Events;
using Shipping.NServiceBus.Messages;

namespace Shipping.NServiceBus;

public class OrderShipper(ILogger<OrderShipper> logger): IHandleMessages<ShipOrder>
{
    public async Task Handle(ShipOrder message, IMessageHandlerContext context)
    {
        logger.LogInformation("🚚 Shipping order {OrderId} to address {Address}", message.OrderId, message.Address);

        // Simulate calling external shipping service to register shipping order
        await Task.Delay(5000, context.CancellationToken);

        await context.Publish<OrderShipped>(evt =>
        {
            evt.OrderId = message.OrderId;
            evt.ShippedAt = DateTime.UtcNow;
        });
    }
}