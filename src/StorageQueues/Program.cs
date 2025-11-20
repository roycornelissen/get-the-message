using Azure.Storage.Queues;
using StorageQueues;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("sales");
    var queueName = "sales";
    connectionString = connectionString!.Replace(";;QueueName=sales", "");
    return new QueueClient(connectionString, queueName);
});
builder.Services.AddScoped<OrderAcceptedHandler>();
builder.Services.AddHostedService<QueueListener>();

builder.AddServiceDefaults();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/place-order", async (QueueClient queueClient) => {
    var message = new Messages.Events.OrderAccepted
    {
        OrderId = Guid.NewGuid(),
        CustomerId = Guid.NewGuid(),
        Amount = new Random().Next(100, 1000)
    };

    await queueClient.SendMessageAsync(System.Text.Json.JsonSerializer.Serialize(message));
});

app.Run();

