using Messages.Events;

namespace Shipping.NServiceBus;

public class OrderShippingAuditing(ILogger<OrderShippingAuditing> logger) : IHandleMessages<OrderShipped>
{
    public Task Handle(OrderShipped message, IMessageHandlerContext context)
    {
        logger.LogInformation("🔍 AUDITING: Order {OrderId} was shipped at {ShippedAt}", message.OrderId, message.ShippedAt);
        return Task.CompletedTask;
    }
}