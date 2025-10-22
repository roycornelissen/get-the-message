using System.Reflection;
using System.Text;
using MassTransit;
using Shipping.MassTransit;
using Shipping.MassTransit.InternalCommands;

Console.OutputEncoding = Encoding.UTF8;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

EndpointConvention.Map<ShipOrder>(new Uri("queue:shipping"));

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    
    // By default, sagas are in-memory, but should be changed to a durable
    // saga repository.
    x.SetInMemorySagaRepositoryProvider();

    var entryAssembly = Assembly.GetEntryAssembly();

    x.AddConsumersFromNamespaceContaining<OrderShipper>();
    x.AddSagaStateMachines(entryAssembly);
    x.AddSagas(entryAssembly);
    x.AddActivities(entryAssembly);

    x.AddServiceBusMessageScheduler();
    
    x.UsingAzureServiceBus((context, cfg) =>
    {
        cfg.UseServiceBusMessageScheduler();
        cfg.Host(builder.Configuration.GetConnectionString("ServiceBus"));

        cfg.UseDelayedRedelivery(r => r.Interval(1, TimeSpan.FromSeconds(10)));
        cfg.UseMessageRetry(retry =>
        {
            retry.Intervals(100, 200, 500, 800, 1000);
        });

        cfg.ReceiveEndpoint("shipping", e =>
        {
            e.ConfigureConsumers(context);
            e.ConfigureSagas(context);
        });
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapDefaultEndpoints();

app.Run();
