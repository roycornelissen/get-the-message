using Azure.Identity;
using Conventions;
using CustomerServiceAgent;
using Messages;
using Messages.Commands;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var endpointConfiguration = new EndpointConfiguration("Sales");
endpointConfiguration.UseSerialization<SystemJsonSerializer>();

var routing = endpointConfiguration.UseTransport<AzureServiceBusTransport>(builder.Configuration.GetConnectionString("ServiceBus")!, TopicTopology.Default);
//endpointConfiguration.UseTransport<AzureServiceBusTransport>("sbns-get-the-message.servicebus.windows.net", new DefaultAzureCredential(), TopicTopology.Default);
  
endpointConfiguration.UsePersistence<LearningPersistence>();
endpointConfiguration.UniquelyIdentifyRunningInstance();

endpointConfiguration.ApplyNamespaceConventions();
endpointConfiguration.ConnectToServicePlatform();

builder.UseNServiceBus(endpointConfiguration);

builder.AddServiceDefaults();

builder.Services.RegisterCustomerServiceAgent(builder.Configuration);

var app = builder.Build();

app.MapGet("/place-order", async (IMessageSession messageSession) =>
{
    var orderId = Guid.NewGuid();
    await messageSession.SendLocal(new PlaceOrder
    {
        Id = orderId,
        Amount = new Random().Next(500, 600),
        CustomerId = Constants.DefaultCustomerId
    });
    return Results.Ok(orderId);
});

app.MapGet("/cancel/{id:guid}", async (IMessageSession messageSession, Guid id) =>
{
    await messageSession.SendLocal(new CancelOrder
    {
        OrderId = id
    });
    return Results.Accepted();
});

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

await app.RunAsync();

