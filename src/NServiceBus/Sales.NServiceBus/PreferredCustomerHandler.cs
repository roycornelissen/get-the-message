using Messages.Commands;
using Messages.Events;

namespace Sales.NServiceBus;

public class PreferredCustomerHandler(ILogger<PreferredCustomerHandler> logger) :
    IHandleMessages<CustomerBecamePreferred>
{
    public async Task Handle(CustomerBecamePreferred message, IMessageHandlerContext context)
    {
        logger.LogInformation("🎉 Hooray, customer {CustomerId} became a preferred customer!", message.CustomerId);
        
        // Set discount percentage to 10% for preferred customer in database
        // lookup email address and name from database based on CustomerId
        // for demo purposes, we'll use dummy data

        await Task.Delay(500, context.CancellationToken);
        
        await context.SendLocal(new SendWelcomeEmail { 
            CustomerId = message.CustomerId,
            EmailAddress = "bert@sesamestreet.com",
            Name = "Bert"
        });
    }
}