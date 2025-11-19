using System.Text;
using Billing.OrderPaymentHandling;
using JasperFx;
using Messages.Commands;
using Messages.Events;
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
    var connectionString = builder.Configuration.GetConnectionString("ServiceBus") ?? "empty";
    options
        .UseAzureServiceBus(connectionString)
        .SystemQueuesAreEnabled(false);

    options.AddSagaType<Order>();
    
    options
        .ListenToAzureServiceBusQueue("billing")
        .ConfigureDeadLetterQueue("dead-letter-billing");

    options.Publish()
        .MessagesFromNamespaceContaining<ProcessPayment>()
        .ToAzureServiceBusQueue("billing");
    
    options.PublishMessage<PaymentReceived>()
        .ToAzureServiceBusTopic("messages.events.paymentreceived");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.MapDefaultEndpoints();

app.Map("/pay-order/{id:guid}",
    (Guid id) => app.InvokeAsync(new ProcessPayment
        { OrderId = id, TransactionReference = DateTime.UtcNow.ToString("O") })
);

return await app.RunJasperFxCommands(args);
