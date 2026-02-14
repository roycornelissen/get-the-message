using AppHost;

var builder = DistributedApplication.CreateBuilder(args);

// Papercut SMTP for local email testing
var papercut = builder.AddPapercutSmtp("Smtp")
    .WithLifetime(ContainerLifetime.Session);

var serviceBusConnection = builder.AddConnectionString("ServiceBus");

builder.RunStorageQueues();
//builder.RunNServiceBus(serviceBusConnection, papercut);
//builder.RunMassTransit(serviceBusConnection, papercut);
//builder.RunWolverine(serviceBusConnection, papercut);
//builder.RunDaprWithRedis(papercut);

builder.Build().Run();
