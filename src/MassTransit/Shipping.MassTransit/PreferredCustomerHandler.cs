using MassTransit;
using Messages.Events;

namespace Shipping.MassTransit;

public class PreferredCustomerHandler(ILogger<PreferredCustomerHandler> logger): IConsumer<CustomerBecamePreferred>
{
    public Task Consume(ConsumeContext<CustomerBecamePreferred> context)
    {
        logger.LogInformation("🎁 Sending preferred customer {CustomerId} a nice present...", context.Message.CustomerId);
        return Task.CompletedTask;
    }
}