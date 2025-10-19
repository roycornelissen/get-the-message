using Projects;

namespace AppHost;

internal static class NServiceBus
{
    public static void RunNServiceBus(this IDistributedApplicationBuilder builder, IResourceBuilder<IResourceWithConnectionString> serviceBusConnection)
    {
        builder
            .AddProject<Sales_NServiceBus>("Sales")
            .WithEnvironment("CustomerServiceAgent:ApiKey", builder.Configuration["CustomerServiceAgent:ApiKey"])
            .WithReference(serviceBusConnection);
        
        builder
            .AddProject<Shipping_NServiceBus>("Shipping")
            .WithReference(serviceBusConnection);

        builder
            .AddProject<Billing_NServiceBus>("Billing")
            .WithReference(serviceBusConnection);
    }
}