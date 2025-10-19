using System.ClientModel;
using Microsoft.Agents.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;
using OpenAI.Chat;

namespace CustomerServiceAgent;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterCustomerServiceAgent(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration["CustomerServiceAgent:ApiKey"] is not { } apiKey)
        {
            services.AddScoped<ICustomerServiceAgent, NoOpCustomerServiceAgent>();
            return services;
        }

        services.AddKeyedScoped<AIAgent>("customer-service", (_, name) =>
        {
            var model = "openai/gpt-4o";
            var endpoint = "https://models.github.ai/inference";

            var chatClient = new ChatClient(model, new ApiKeyCredential(apiKey), new OpenAIClientOptions
            {
                Endpoint = new Uri(endpoint)
            });

            return chatClient.CreateAIAgent("You are a helpful customer service assistant that composes personalized welcome emails for new preferred customers. Your tone is warm and friendly.", name: (string)name!);
        });
        services.AddScoped<ICustomerServiceAgent, SmartCustomerServiceAgent>();
        return services;
    }
}