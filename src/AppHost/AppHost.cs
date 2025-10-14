using Projects;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Sales_NServiceBus>("Sales-NServiceBus");

builder.AddProject<CustomerRelations_MassTransit>("CustomerRelations-MassTransit");

builder.AddProject<Shipping_Wolverine>("Shipping-Wolverine");

builder.AddProject<Payments_Dapr>("Payments-Dapr")
    .WithDaprSidecar();

builder.Build().Run();
