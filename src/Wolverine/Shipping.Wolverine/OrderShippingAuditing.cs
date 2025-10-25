using Messages.Events;

namespace Shipping;

public class OrderShippingAuditing
{
    public void Handle(OrderShipped message, ILogger<OrderShipped> logger)
    {
        logger.LogInformation("🔍 AUDITING: Order {OrderId} was shipped at {ShippedAt}", message.OrderId, message.ShippedAt);
    }
}