using Projects;
using Scalar.Aspire;

namespace AppHost;

internal static class MassTransit
{
    public static void RunMassTransit(this IDistributedApplicationBuilder builder,
        IResourceBuilder<IResourceWithConnectionString> serviceBusConnection)
    {
        var sales = builder
            .AddProject<Sales_MassTransit>("Sales")
            .WithUrl("/swagger")
            .WithEnvironment("CustomerServiceAgent:ApiKey", builder.Configuration["CustomerServiceAgent:ApiKey"])
            .WithReference(serviceBusConnection);
        
        var shipping = builder
            .AddProject<Shipping_MassTransit>("Shipping")
            .WithUrl("/swagger")
            .WithReference(serviceBusConnection);

        var billing = builder
            .AddProject<Billing_MassTransit>("Billing")
            .WithUrl("/swagger")
            .WithReference(serviceBusConnection);

        builder
            .AddScalarApiReference()
            .WithApiReference(sales)
            .WithApiReference(shipping)
            .WithApiReference(billing);
    }
}