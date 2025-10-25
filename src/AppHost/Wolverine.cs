using Projects;

namespace AppHost;

internal static class Wolverine
{
    public static void RunWolverine(this IDistributedApplicationBuilder builder,
        IResourceBuilder<IResourceWithConnectionString> serviceBusConnection)
    {
        builder
            .AddProject<Sales_Wolverine>("Sales")
            .WithEnvironment("CustomerServiceAgent:ApiKey", builder.Configuration["CustomerServiceAgent:ApiKey"])
            .WithReference(serviceBusConnection);

        builder
            .AddProject<Shipping_Wolverine>("Shipping")
            .WithReference(serviceBusConnection);

        builder
            .AddProject<Billing_Wolverine>("Billing")
            .WithReference(serviceBusConnection);
    }
}