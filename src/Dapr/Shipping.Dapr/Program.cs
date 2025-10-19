using Dapr;
using Messages.Events;

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
    return Results.Ok(order);
});

app.Run();
