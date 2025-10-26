using AppHost;

var builder = DistributedApplication.CreateBuilder(args);

var serviceBusConnection = builder.AddConnectionString("ServiceBus");

//builder.RunNServiceBus(serviceBusConnection);
//builder.RunMassTransit(serviceBusConnection);
//builder.RunWolverine(serviceBusConnection);

builder.RunDaprWithRedis();

builder.Build().Run();
