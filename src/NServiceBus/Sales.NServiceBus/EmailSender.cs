using CustomerServiceAgent;
using Messages.Commands;

namespace Sales.NServiceBus;

public class EmailSender(ILogger<EmailSender> logger, ICustomerServiceAgent agent): 
    IHandleMessages<SendWelcomeEmail>
{
    public async Task Handle(SendWelcomeEmail message, IMessageHandlerContext context)
    {
        logger.LogInformation("💌 Sending welcome email to customer {MessageCustomerId}", message.CustomerId);
        await agent.SendWelcomeEmail(message.EmailAddress, message.Name, context.CancellationToken);
    }
}