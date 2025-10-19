using System.Collections.Immutable;
using System.Text.Encodings.Web;
using System.Text.Json;
using CommunityToolkit.Aspire.Hosting.Dapr;
using Microsoft.Extensions.Configuration;
using Projects;

namespace AppHost;

internal static class Dapr
{
    public static async Task RunDapr(this IDistributedApplicationBuilder builder)
    {
        await TransferServiceBusConnectionStringToDaprSecrets(builder);

        builder.AddDapr(dapr =>
        {
            dapr.EnableTelemetry = true;
        });

        builder.AddProject<Sales_Dapr>("Sales")
            .WithExternalHttpEndpoints()
            .WithEnvironment("CustomerServiceAgent:ApiKey", builder.Configuration["CustomerServiceAgent:ApiKey"])
            .WithDaprSidecar(new DaprSidecarOptions
            {
                AppId = "sales",
                ResourcesPaths = ImmutableHashSet.Create("./dapr/components")
            });
        
        builder.AddProject<Billing_Dapr>("Billing")
            .WithExternalHttpEndpoints()
            .WithDaprSidecar(new DaprSidecarOptions
            {
                AppId = "billing",
                ResourcesPaths = ImmutableHashSet.Create("./dapr/components")
            });
        
        builder.AddProject<Shipping_Dapr>("Shipping")
            .WithExternalHttpEndpoints()
            .WithDaprSidecar(new DaprSidecarOptions
            {
                AppId = "shipping",
                ResourcesPaths = ImmutableHashSet.Create("./dapr/components")
            });
    }
    
    private static async Task TransferServiceBusConnectionStringToDaprSecrets(IDistributedApplicationBuilder distributedApplicationBuilder)
    {
        // Write the Service Bus connection string to Dapr secrets file before starting
        var connectionStringValue = distributedApplicationBuilder.Configuration.GetConnectionString("ServiceBus");
        if (!string.IsNullOrEmpty(connectionStringValue))
        {
            var secretsPath = Path.Combine(distributedApplicationBuilder.AppHostDirectory, "dapr", "components", "secrets.json");
            var secrets = new
            {
                azure = new
                {
                    servicebus = connectionStringValue
                }
            };
    
            var secretsJson = JsonSerializer.Serialize(secrets, new JsonSerializerOptions 
            { 
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
            await File.WriteAllTextAsync(secretsPath, secretsJson);
        }
    }
}