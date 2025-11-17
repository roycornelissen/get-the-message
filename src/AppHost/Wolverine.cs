using Projects;

namespace AppHost;

internal static class Wolverine
{
    public static void RunWolverine(this IDistributedApplicationBuilder builder,
        IResourceBuilder<IResourceWithConnectionString> serviceBusConnection)
    {
        builder
            .AddProject<Sales_Wolverine>("Sales")
            .WithUrl("/swagger")
            .WithEnvironment("CustomerServiceAgent:ApiKey", builder.Configuration["CustomerServiceAgent:ApiKey"])
            .WithReference(serviceBusConnection);

        builder
            .AddProject<Shipping_Wolverine>("Shipping")
            .WithUrl("/swagger")
            .WithReference(serviceBusConnection);

        builder
            .AddProject<Billing_Wolverine>("Billing")
            .WithUrl("/swagger")
            .WithReference(serviceBusConnection);
    }
}