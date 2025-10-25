using CustomerServiceAgent;
using Messages.Commands;

namespace Sales.CustomerStatus;

public class EmailHandler
{
    public async Task Handle(SendWelcomeEmail message, ICustomerServiceAgent agent, ILogger<EmailHandler> logger, CancellationToken cancellationToken)
    {
        logger.LogInformation("💌 Sending welcome email to customer {MessageCustomerId}", message.CustomerId);
        await agent.SendWelcomeEmail(message.EmailAddress, message.Name, cancellationToken);
    }
}