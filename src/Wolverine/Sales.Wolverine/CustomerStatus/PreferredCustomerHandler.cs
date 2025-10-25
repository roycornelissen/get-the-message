using Messages.Commands;
using Messages.Events;

namespace Sales.CustomerStatus;

public class PreferredCustomerHandler
{
    public async Task<SendWelcomeEmail> Handle(CustomerBecamePreferred message, ILogger<CustomerBecamePreferred> logger, CancellationToken cancellationToken)
    {
        logger.LogInformation("🎉 Hooray, customer {CustomerId} became a preferred customer!", message.CustomerId);
        
        // Set discount percentage to 10% for preferred customer in database
        // lookup email address and name from database based on CustomerId
        // for demo purposes, we'll use dummy data

        await Task.Delay(500, cancellationToken);
        
        return new SendWelcomeEmail { 
            CustomerId = message.CustomerId,
            EmailAddress = "oscar@sesamestreet.com",
            Name = "Oscar"
        };
    }
}