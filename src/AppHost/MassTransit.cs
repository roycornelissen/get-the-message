using Projects;

namespace AppHost;

internal static class MassTransit
{
    public static void RunMassTransit(this IDistributedApplicationBuilder builder,
        IResourceBuilder<IResourceWithConnectionString> serviceBusConnection, IResourceBuilder<PapercutSmtpContainerResource> papercut)
    {
        var sales = builder
            .AddProject<Sales_MassTransit>("Sales")
            .WithUrl("/swagger")
            .WithEnvironment("CustomerServiceAgent:ApiKey", builder.Configuration["CustomerServiceAgent:ApiKey"])
            .WithEnvironment("CustomerServiceAgent:InferenceEndpoint", builder.Configuration["CustomerServiceAgent:InferenceEndpoint"])
            .WithReference(serviceBusConnection)
            .WithReference(papercut)
            .WaitFor(papercut);

        var shipping = builder
            .AddProject<Shipping_MassTransit>("Shipping")
            .WithUrl("/swagger")
            .WithReference(serviceBusConnection);

        var billing = builder
            .AddProject<Billing_MassTransit>("Billing")
            .WithUrl("/swagger")
            .WithReference(serviceBusConnection);
    }
}