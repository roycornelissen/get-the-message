using Projects;
using Scalar.Aspire;

namespace AppHost;

internal static class Wolverine
{
    public static void RunWolverine(this IDistributedApplicationBuilder builder,
        IResourceBuilder<IResourceWithConnectionString> serviceBusConnection)
    {
        var sales = builder
            .AddProject<Sales_Wolverine>("Sales")
            .WithUrl("/swagger")
            .WithEnvironment("CustomerServiceAgent:ApiKey", builder.Configuration["CustomerServiceAgent:ApiKey"])
            .WithReference(serviceBusConnection);

        var shipping = builder
            .AddProject<Shipping_Wolverine>("Shipping")
            .WithUrl("/swagger")
            .WithReference(serviceBusConnection);

        var billing = builder
            .AddProject<Billing_Wolverine>("Billing")
            .WithUrl("/swagger")
            .WithReference(serviceBusConnection);

        builder
            .AddScalarApiReference()
            .WithApiReference(sales)
            .WithApiReference(shipping)
            .WithApiReference(billing);
    }
}