using System.Reflection;
using System.Text;
using MassTransit;
using Messages.Commands;
using CustomerServiceAgent;
using Messages;

Console.OutputEncoding = Encoding.UTF8;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.RegisterCustomerServiceAgent(builder.Configuration);

var endpointUri = new Uri("queue:sales");

// using conventions
EndpointConvention.Map<PlaceOrder>(endpointUri);
EndpointConvention.Map<SendWelcomeEmail>(endpointUri);

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    
    // By default, sagas are in-memory, but should be changed to a durable
    // saga repository.
    x.SetInMemorySagaRepositoryProvider();

    var entryAssembly = Assembly.GetEntryAssembly();

    x.AddConsumers(entryAssembly);
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

        cfg.ReceiveEndpoint("sales", e =>
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

app.MapGet("/place-order", async (IBus bus, CancellationToken cancellationToken) =>
{
    var orderId = Guid.NewGuid();
    await bus.Send(new PlaceOrder
    {
        Id = orderId,
        Amount = new Random().Next(500, 600),
        CustomerId = Constants.DefaultCustomerId
    }, cancellationToken);
    return Results.Ok(orderId);
});

app.MapGet("/cancel/{id:guid}", async (ISendEndpointProvider endpointProvider, Guid id, CancellationToken cancellationToken) =>
{
    // using explicit endpoint
    var endpoint = await endpointProvider.GetSendEndpoint(new Uri("queue:sales"));
    await endpoint.Send(new CancelOrder
    {
        OrderId = id
    }, cancellationToken);
    return Results.Accepted();
});

app.Run();
