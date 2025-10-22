using CustomerServiceAgent;
using MassTransit;
using Messages.Commands;

namespace Sales.MassTransit;

public class EmailSender(ILogger<EmailSender> logger, ICustomerServiceAgent agent): 
    IConsumer<SendWelcomeEmail>
{
    public async Task Consume(ConsumeContext<SendWelcomeEmail> context)
    {
        logger.LogInformation("💌 Sending welcome email to customer {MessageCustomerId}", context.Message.CustomerId);
        await agent.SendWelcomeEmail(context.Message.EmailAddress, context.Message.Name, context.CancellationToken);
    }
}