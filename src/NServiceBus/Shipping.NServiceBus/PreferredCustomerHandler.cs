using Messages.Events;

namespace Shipping.NServiceBus;

public class PreferredCustomerHandler(ILogger<PreferredCustomerHandler> logger): IHandleMessages<CustomerBecamePreferred>
{
    public Task Handle(CustomerBecamePreferred message, IMessageHandlerContext context)
    {
        logger.LogInformation("🎁 Sending preferred customer {CustomerId} a nice present...", message.CustomerId);
        return Task.CompletedTask;
    }
}