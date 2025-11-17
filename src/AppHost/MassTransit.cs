using Projects;

namespace AppHost;

internal static class MassTransit
{
    public static void RunMassTransit(this IDistributedApplicationBuilder builder,
        IResourceBuilder<IResourceWithConnectionString> serviceBusConnection)
    {
        builder
            .AddProject<Sales_MassTransit>("Sales")
            .WithUrl("/swagger")
            .WithEnvironment("CustomerServiceAgent:ApiKey", builder.Configuration["CustomerServiceAgent:ApiKey"])
            .WithReference(serviceBusConnection);
        
        builder
            .AddProject<Shipping_MassTransit>("Shipping")
            .WithUrl("/swagger")
            .WithReference(serviceBusConnection);

        builder
            .AddProject<Billing_MassTransit>("Billing")
            .WithUrl("/swagger")
            .WithReference(serviceBusConnection);
    }
}