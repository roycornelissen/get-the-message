using Aspire.Hosting;
using CommunityToolkit.Aspire.Hosting.Dapr;
using Projects;
using Scalar.Aspire;

namespace AppHost;

internal static class Dapr
{
    public static void RunDaprWithRedis(this IDistributedApplicationBuilder builder)
    {
        var pubSubPassword = builder.AddParameter("pubsub-password", secret: true);

        builder.AddDapr(dapr =>
        {
            dapr.EnableTelemetry = true;
        });

        var redis = builder
            .AddRedis("redis")
            .WithPassword(pubSubPassword);

        var stateStore = builder
                .AddDaprStateStore("statestore")
                .WithMetadata("actorStateStore", "true")
                .WithMetadata("redisHost", redis.Resource.PrimaryEndpoint.Property(EndpointProperty.Host).ValueExpression)
                .WithMetadata("redisPort", redis.Resource.PrimaryEndpoint.Property(EndpointProperty.Port).ValueExpression)
                .WithMetadata("redisPassword", pubSubPassword.Resource)
                .WaitFor(redis);

        var redisHost= redis.Resource.PrimaryEndpoint.Property(EndpointProperty.Host);
        var redisPort = redis.Resource.PrimaryEndpoint.Property(EndpointProperty.Port);

        var pubSub = builder
            .AddDaprPubSub("pubsub")
            .WithMetadata(
                "redisHost",
                ReferenceExpression.Create(
                    $"{redisHost}:{redisPort}"
                )
            )
            .WithMetadata(
                "redisPassword",
                pubSubPassword.Resource
            )
            .WaitFor(redis);
        
        RunApps(builder, redis, sidecar =>
        {
            sidecar
                .WithReference(stateStore)
                .WithReference(pubSub)
                .WaitFor(redis);
        });
    }
    
    private static void RunApps(
        IDistributedApplicationBuilder builder,
        IResourceBuilder<IResource> brokerDependency,
        Action<IResourceBuilder<IDaprSidecarResource>> configureSidecar)
    {
        var sales = builder.AddProject<Sales_Dapr>("Sales")
            .WithExternalHttpEndpoints()
            .WithUrl("/swagger")
            .WithDaprSidecar(sidecar =>
            {
                sidecar.WithOptions(new DaprSidecarOptions
                {
                    AppId = "sales",
                    AppProtocol = "http"
                });
                configureSidecar.Invoke(sidecar);
            })
            .WaitFor(brokerDependency);

        var billing = builder.AddProject<Billing_Dapr>("Billing")
            .WithExternalHttpEndpoints()
            .WithUrl("/swagger")
            .WithDaprSidecar(sidecar =>
            {
                sidecar.WithOptions(new DaprSidecarOptions
                {
                    AppId = "billing"
                });
                configureSidecar.Invoke(sidecar);
            })
            .WaitFor(brokerDependency);

        var shipping = builder.AddProject<Shipping_Dapr>("Shipping")
            .WithExternalHttpEndpoints()
            .WithUrl("/swagger")
            .WithDaprSidecar(sidecar =>
            {
                sidecar.WithOptions(new DaprSidecarOptions
                {
                    AppId = "shipping"
                });
                configureSidecar.Invoke(sidecar);
            })
            .WaitFor(brokerDependency);

        builder
            .AddScalarApiReference()
            .WithApiReference(sales)
            .WithApiReference(shipping)
            .WithApiReference(billing);

    }
}