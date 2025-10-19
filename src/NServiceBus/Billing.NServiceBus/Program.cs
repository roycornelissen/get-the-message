using Azure.Identity;
using Conventions;
using Messages.Commands;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var builder = WebApplication.CreateBuilder(args);

var endpointConfiguration = new EndpointConfiguration("Billing");
endpointConfiguration.UseSerialization<SystemJsonSerializer>();

//endpointConfiguration.UseTransport<AzureServiceBusTransport>(builder.Configuration.GetConnectionString("ServiceBus")!, TopicTopology.Default);
endpointConfiguration.UseTransport<AzureServiceBusTransport>("sbns-get-the-message.servicebus.windows.net", new DefaultAzureCredential(), TopicTopology.Default);

endpointConfiguration.UsePersistence<LearningPersistence>();
endpointConfiguration.UniquelyIdentifyRunningInstance();
endpointConfiguration.ApplyNamespaceConventions();

builder.UseNServiceBus(endpointConfiguration);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/pay-order/{id:guid}", async (IMessageSession messageSession, Guid id) =>
{
    // Simulate billing logic here
    await Task.Delay(500); // Simulate some processing time
    
    await messageSession.SendLocal<ProcessPayment>(cmd =>
    {
        cmd.OrderId = id;
        cmd.TransactionReference = DateTime.UtcNow.ToString("O");
    });

    return Results.Accepted();
});

await app.RunAsync();
