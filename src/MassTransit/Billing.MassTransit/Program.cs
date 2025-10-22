using System.Text;
using Billing.MassTransit;
using MassTransit;
using Messages.Commands;

Console.OutputEncoding = Encoding.UTF8;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    x.AddSagaStateMachine<OrderBillingSaga, OrderBillingState>();

    // By default, sagas are in-memory, but should be changed to a durable
    // saga repository.
    x.SetInMemorySagaRepositoryProvider();
    
    x.UsingAzureServiceBus((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("ServiceBus"));
        
        cfg.ReceiveEndpoint("billing", endpoint =>
        {
            endpoint.ConfigureSagas(context);
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

app.MapGet("/pay-order/{id:guid}", async (IBus bus, Guid id, CancellationToken cancellationToken) =>
{
    // Simulate billing logic here
    await Task.Delay(500); // Simulate some processing time
    
    var endpoint = await bus.GetSendEndpoint(new Uri("queue:billing"));

    await endpoint.Send(new ProcessPayment
    {
        OrderId = id,
        TransactionReference = DateTime.UtcNow.ToString("O")
    }, cancellationToken);

    return Results.Accepted();
});

app.Run();

