using Projects;

namespace AppHost;

internal static class Wolverine
{
    public static void RunWolverine(this IDistributedApplicationBuilder builder,
        IResourceBuilder<IResourceWithConnectionString> serviceBusConnection, IResourceBuilder<PapercutSmtpContainerResource> papercut)
    {
        var sales = builder
            .AddProject<Sales_Wolverine>("Sales")
            .WithUrl("/swagger")
            .WithEnvironment("CustomerServiceAgent:ApiKey", builder.Configuration["CustomerServiceAgent:ApiKey"])
            .WithEnvironment("CustomerServiceAgent:InferenceEndpoint", builder.Configuration["CustomerServiceAgent:InferenceEndpoint"])
            .WithReference(serviceBusConnection)
            .WithReference(papercut)
            .WaitFor(papercut);

        var shipping = builder
            .AddProject<Shipping_Wolverine>("Shipping")
            .WithUrl("/swagger")
            .WithReference(serviceBusConnection);

        var billing = builder
            .AddProject<Billing_Wolverine>("Billing")
            .WithUrl("/swagger")
            .WithReference(serviceBusConnection);
    }
}