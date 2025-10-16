using System.Collections.Immutable;
using CommunityToolkit.Aspire.Hosting.Dapr;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddDapr(dapr =>
{
    dapr.EnableTelemetry = true;
});

builder.AddProject<Sales_NServiceBus>("Sales-NServiceBus");

builder.AddProject<CustomerRelations_MassTransit>("CustomerRelations-MassTransit");

builder.AddProject<Shipping_Wolverine>("Shipping-Wolverine");

builder.AddProject<Payments_Dapr>("Payments-Dapr")
    .WithExternalHttpEndpoints()
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "payments-dapr",
        ResourcesPaths = ImmutableHashSet.Create("./dapr/components"),
        Config = "./dapr/components/config.yaml"
    });

builder.Build().Run();
