using MassTransit;
using Messages.Events;
using Shipping.MassTransit.InternalCommands;

namespace Shipping.MassTransit;

public class OrderShipper(ILogger<OrderShipper> logger): IConsumer<ShipOrder>
{
    public async Task Consume(ConsumeContext<ShipOrder> context)
    {
        logger.LogInformation("🚚 Shipping order {OrderId} to address {Address}", context.Message.OrderId, context.Message.Address);

        // Simulate calling external shipping service to register shipping order
        await Task.Delay(5000, context.CancellationToken);

        await context.Publish(new OrderShipped
        {
            OrderId = context.Message.OrderId,
            ShippedAt = DateTime.UtcNow
        }, context.CancellationToken);
    }
}