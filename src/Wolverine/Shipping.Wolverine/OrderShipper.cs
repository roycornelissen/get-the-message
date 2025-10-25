using Messages.Events;
using Wolverine.Attributes;

namespace Shipping;

[WolverineHandler]
public class OrderShipper
{
    public async Task<OrderShipped> Handle(ShipOrder message, ILogger<ShipOrder> logger, CancellationToken cancellationToken)
    {
        logger.LogInformation("🚚 Shipping order {OrderId} to address {Address}", message.OrderId, message.Address);

        // Simulate calling external shipping service to register shipping order
        await Task.Delay(5000, cancellationToken);

        return new OrderShipped
        {
            OrderId = message.OrderId,
            ShippedAt = DateTime.UtcNow
        };
    }
}