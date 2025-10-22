using MassTransit;
using Messages.Events;

namespace Shipping.MassTransit;

public class OrderShippingAuditing(ILogger<OrderShippingAuditing> logger) : IConsumer<OrderShipped>
{
    public Task Consume(ConsumeContext<OrderShipped> context)
    {
        logger.LogInformation("🔍 AUDITING: Order {OrderId} was shipped at {ShippedAt}", context.Message.OrderId, context.Message.ShippedAt);
        return Task.CompletedTask;
    }
}