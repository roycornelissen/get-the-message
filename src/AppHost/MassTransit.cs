using Projects;

namespace AppHost;

internal static class MassTransit
{
    public static void RunMassTransit(this IDistributedApplicationBuilder builder,
        IResourceBuilder<IResourceWithConnectionString> serviceBusConnection)
    {
        builder
            .AddProject<Sales_MassTransit>("Sales")
            .WithEnvironment("CustomerServiceAgent:ApiKey", builder.Configuration["CustomerServiceAgent:ApiKey"])
            .WithReference(serviceBusConnection);
        
        builder
            .AddProject<Shipping_MassTransit>("Shipping")
            .WithReference(serviceBusConnection);

        builder
            .AddProject<Billing_MassTransit>("Billing")
            .WithReference(serviceBusConnection);
    }
}