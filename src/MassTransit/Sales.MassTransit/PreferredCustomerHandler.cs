using MassTransit;
using Messages.Commands;
using Messages.Events;

namespace Sales.MassTransit;

public class PreferredCustomerHandler(ILogger<PreferredCustomerHandler> logger) :
    IConsumer<CustomerBecamePreferred>
{
    public async Task Consume(ConsumeContext<CustomerBecamePreferred> context)
    {
        logger.LogInformation("🎉 Hooray, customer {CustomerId} became a preferred customer!", context.Message.CustomerId);
        
        // Set discount percentage to 10% for preferred customer in database
        // lookup email address and name from database based on CustomerId
        // for demo purposes, we'll use dummy data

        await Task.Delay(500, context.CancellationToken);
        
        await context.Send(new SendWelcomeEmail { 
            CustomerId = context.Message.CustomerId,
            EmailAddress = "ernie@sesamestreet.com",
            Name = "Ernie"
        });
    }
}