using System.Text;
using CustomerServiceAgent;
using JasperFx;
using Messages;
using Messages.Commands;
using Messages.Events;
using Sales.BuyersRemorse;
using Sales.CustomerStatus;
using Wolverine;
using Wolverine.AzureServiceBus;
using Wolverine.Http;
using Wolverine.RDBMS;

Console.OutputEncoding = Encoding.UTF8;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ApplyJasperFxExtensions();

builder.AddServiceDefaults();

builder.Services.AddWolverineHttp();

builder.UseWolverine(options =>
{
    options
        .UseAzureServiceBus(builder.Configuration.GetConnectionString("ServiceBus")!)
        .SystemQueuesAreEnabled(false);

    options.AddSagaType<Order>();
    options.AddSagaType<Customer>();

    options
        .ListenToAzureServiceBusQueue("sales")
        .ConfigureDeadLetterQueue("dead-letter-sales");

    options.Publish()
        .MessagesFromNamespaceContaining<PlaceOrder>()
        .ToAzureServiceBusQueue("sales");
    
    options.PublishMessage<OrderPlaced>()
        .ToAzureServiceBusTopic("messages.events.orderplaced");
    options.PublishMessage<OrderAccepted>()
        .ToAzureServiceBusTopic("messages.events.orderaccepted");
    options.PublishMessage<OrderCanceled>()
        .ToAzureServiceBusTopic("messages.events.ordercanceled");
    options.PublishMessage<CustomerBecamePreferred>()
        .ToAzureServiceBusTopic("messages.events.customerbecamepreferred");
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.RegisterCustomerServiceAgent(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.MapDefaultEndpoints();

app.MapWolverineEndpoints(x => x.WarmUpRoutes = RouteWarmup.Eager);

app.MapGet("/cancel/{id:guid}", async (IMessageBus bus, Guid id) =>
{
    await bus.SendAsync(new CancelOrder
    {
        OrderId = id
    });
    return Results.Accepted();
});

return await app.RunJasperFxCommands(args);
