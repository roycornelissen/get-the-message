using System.Text;
using Dapr;
using Dapr.Client;
using Messages;
using Messages.Events;

Console.OutputEncoding = Encoding.UTF8;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddDaprClient();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseCloudEvents();
app.MapSubscribeHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapPost("/order-accepted", [Topic("pubsub", "order-accepted")] async (DaprClient daprClient, ILogger<OrderAccepted> logger, OrderAccepted order) => {

    if (CoinFlipper.FlipCoin())
    {
        Console.WriteLine("☠️☠️☠️☠️☠️☠️");
        return Results.UnprocessableEntity();
    }
    
    logger.LogInformation("💰 Billing order {OrderId} for amount {Amount}", order.OrderId, order.Amount);

    // save the 
    await daprClient.SaveStateAsync("statestore", order.OrderId.ToString(), order);
    
    return Results.Ok(order);
});

app.MapGet("/pay-order/{id:guid}", async (DaprClient daprClient, ILogger<PaymentReceived> logger, Guid id) =>
{
    PaymentReceived paymentReceived = new();
    
    var order = await daprClient.GetStateAsync<OrderAccepted>("statestore", id.ToString());
    if (order is null)
    {
        paymentReceived.OrderId = id;
        paymentReceived.Amount = 100m;
        paymentReceived.CustomerId = Constants.DefaultCustomerId;
    }
    else
    {
        await daprClient.DeleteStateAsync("statestore", id.ToString());

        paymentReceived.OrderId = order.OrderId;
        paymentReceived.Amount = order.Amount;
        paymentReceived.CustomerId = order.CustomerId;
    }

    logger.LogInformation("💳 Processing payment for order {OrderId}", id);
    await daprClient.PublishEventAsync("pubsub", "payment-received", paymentReceived);

    return Results.Accepted();
});

app.Run();
