using System.Text;
using Dapr;
using Dapr.Client;
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

app.MapPost("/order-accepted", [Topic("pubsub", "sales-events")] (OrderAccepted order) => {

    Console.WriteLine("Subscriber received order-accepted: " + order.OrderId);
    if (CoinFlipper.FlipCoin())
    {
        Console.WriteLine("☠️☠️☠️☠️☠️☠️");
        return Results.UnprocessableEntity();
//        return Results.InternalServerError();
    }
    
    return Results.Ok(order);
});

app.Run();
