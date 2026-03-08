using Projects;
using Scalar.Aspire;

namespace AppHost;

internal static class NServiceBus
{
    public static void RunNServiceBus(this IDistributedApplicationBuilder builder, IResourceBuilder<IResourceWithConnectionString> serviceBusConnection, IResourceBuilder<PapercutSmtpContainerResource> papercut)
    {
        var sales = builder
            .AddProject<Sales_NServiceBus>("Sales")
            .WithUrl("/swagger")
            .WithEnvironment("CustomerServiceAgent:ApiKey", builder.Configuration["CustomerServiceAgent:ApiKey"])
            .WithEnvironment("CustomerServiceAgent:InferenceEndpoint", builder.Configuration["CustomerServiceAgent:InferenceEndpoint"])
            .WithReference(serviceBusConnection)
            .WithReference(papercut)
            .WaitFor(papercut);

        var shipping = builder
            .AddProject<Shipping_NServiceBus>("Shipping")
            .WithUrl("/swagger")
            .WithReference(serviceBusConnection);

        var billing = builder
            .AddProject<Billing_NServiceBus>("Billing")
            .WithUrl("/swagger")
            .WithReference(serviceBusConnection);

        builder
            .AddScalarApiReference()
            .WithApiReference(sales)
            .WithApiReference(shipping)
            .WithApiReference(billing);
    }
}