using Microsoft.Agents.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CustomerServiceAgent;

public class SmartCustomerServiceAgent(ILogger<SmartCustomerServiceAgent> logger, [FromKeyedServices("customer-service")]AIAgent agent): ICustomerServiceAgent
{
    public async Task SendWelcomeEmail(string emailAddress, string name, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(emailAddress) || string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Email address and name must be provided.");
        }

        var prompt = $"""
                      Send a nice personalized welcome email to the customer 
                      named {name} at {emailAddress}. They just achieved the preferred 
                      status and we want to express our gratitude for their business.
                      Tell them that they get a 10% discount on their next purchase as a 
                      thank you. Make it 2 paragraphs long at max.");
                      """;
        var response = await agent.RunAsync(prompt, cancellationToken: cancellationToken);
        
        logger.LogInformation("To: {EmailAddress}:\n{EmailContent}", emailAddress, response.Text);
    }
}