using Projects;

namespace AppHost;

internal static class Wolverine
{
    public static void RunWolverine(this IDistributedApplicationBuilder builder,
        IResourceBuilder<IResourceWithConnectionString> serviceBusConnection)
    {
        builder
            .AddProject<Shipping_Wolverine>("Shipping")
            .WithReference(serviceBusConnection);
    }
}