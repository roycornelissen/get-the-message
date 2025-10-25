using AppHost;

var builder = DistributedApplication.CreateBuilder(args);

var serviceBusConnection = builder.AddConnectionString("ServiceBus");

//builder.RunNServiceBus(serviceBusConnection);
//builder.RunMassTransit(serviceBusConnection);
builder.RunWolverine(serviceBusConnection);
//await builder.RunDapr();

builder.Build().Run();
