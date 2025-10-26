using System.Text;
using Dapr;
using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Client;
using Messages;
using Messages.Events;
using Sales.BuyersRemorse;
using Sales.CustomerStatus;

Console.OutputEncoding = Encoding.UTF8;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddDaprClient();

builder.Services.AddActors(options =>
{
    // Register actor types and configure actor settings
    options.Actors.RegisterActor<CustomerActor>();
    options.Actors.RegisterActor<BuyersRemorseActor>();
    options.ReentrancyConfig = new ActorReentrancyConfig
    {
        Enabled = true,
        MaxStackDepth = 32,
    };
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseCloudEvents();
app.MapSubscribeHandler();
app.MapActorsHandlers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/place-order", async (CancellationToken cancellationToken) =>
{
    var orderId = Guid.NewGuid();
    var amount = new Random().Next(500, 600);

    var placeOrder = new OrderPlaced
    {
        OrderId = orderId,
        Amount = amount,
        CustomerId = Constants.DefaultCustomerId
    };

    var actorId = new ActorId(orderId.ToString());
    var actor = ActorProxy.Create<IBuyersRemorseActor>(actorId, nameof(BuyersRemorseActor));
    await actor.InitiateBuyersRemorsePeriod(placeOrder, cancellationToken);
    
    return Results.Ok(orderId);
});

app.MapGet("/cancel/{id:guid}", async (Guid id, CancellationToken cancellationToken) =>
{
    var actorId = new ActorId(id.ToString());
    var actor = ActorProxy.Create<IBuyersRemorseActor>(actorId, nameof(BuyersRemorseActor));
    await actor.CancelOrder(cancellationToken);

    return Results.Accepted();
});

app.MapPost("/payment-received", 
    [Topic("pubsub", "payment-received")] async (
        DaprClient client,
        PaymentReceived message,
        CancellationToken cancellationToken) => {

    var customerActorId = new ActorId(message.CustomerId.ToString());

    var actor = ActorProxy.Create<ICustomerActor>(customerActorId, nameof(CustomerActor));

    await actor.TrackPayment(message, cancellationToken);
    
    return Results.Accepted();
});
    
app.Run();
