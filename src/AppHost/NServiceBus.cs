using Projects;

namespace AppHost;

internal static class NServiceBus
{
    public static void RunNServiceBus(this IDistributedApplicationBuilder builder, IResourceBuilder<IResourceWithConnectionString> serviceBusConnection)
    {
        builder
            .AddProject<Sales_NServiceBus>("Sales")
            .WithUrl("/swagger")
            .WithEnvironment("CustomerServiceAgent:ApiKey", builder.Configuration["CustomerServiceAgent:ApiKey"])
            .WithReference(serviceBusConnection);
        
        builder
            .AddProject<Shipping_NServiceBus>("Shipping")
            .WithUrl("/swagger")
            .WithReference(serviceBusConnection);

        builder
            .AddProject<Billing_NServiceBus>("Billing")
            .WithUrl("/swagger")
            .WithReference(serviceBusConnection);
    }
}