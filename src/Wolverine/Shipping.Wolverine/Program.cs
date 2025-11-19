using System.Text;
using JasperFx;
using Messages.Events;
using Shipping;
using Shipping.ShippingProcess;
using Wolverine;
using Wolverine.AzureServiceBus;
using Wolverine.RDBMS;

Console.OutputEncoding = Encoding.UTF8;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ApplyJasperFxExtensions();

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

builder.UseWolverine(options =>
{
    options.AddSagaType<Order>();

    var connectionString = builder.Configuration.GetConnectionString("ServiceBus") ?? "empty";
    options
        .UseAzureServiceBus(connectionString)
        .SystemQueuesAreEnabled(false);
    
    options
        .ListenToAzureServiceBusQueue("shipping")
        .ConfigureDeadLetterQueue("dead-letter-shipping");

    options.Publish()
        .MessagesFromNamespaceContaining<ShipOrder>()
        .ToAzureServiceBusQueue("shipping");

    options.PublishMessage<OrderShipped>()
        .ToAzureServiceBusTopic("messages.events.ordershipped");

    options.Discovery.IncludeAssembly(typeof(OrderShipper).Assembly);
    Console.WriteLine(options.DescribeHandlerMatch(typeof(OrderShipper)));
});

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

return await app.RunJasperFxCommands(args);
