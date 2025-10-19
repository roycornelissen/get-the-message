using Wolverine;
using Wolverine.AzureServiceBus;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.UseWolverine(options =>
{
    options
        .UseAzureServiceBus(builder.Configuration.GetConnectionString("ServiceBus")!)
        .SystemQueuesAreEnabled(false);
    options
        .ListenToAzureServiceBusQueue("shipping");
    options
        .ListenToAzureServiceBusSubscription("sales-events/shipping");
    options
        .PublishAllMessages()
        .ToAzureServiceBusTopic("shipping-events");
});

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Run();
