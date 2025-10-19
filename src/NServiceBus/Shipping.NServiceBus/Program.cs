using Conventions;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var endpointConfiguration = new EndpointConfiguration("Shipping");
endpointConfiguration.UseSerialization<SystemJsonSerializer>();

var routing = endpointConfiguration.UseTransport<AzureServiceBusTransport>(builder.Configuration.GetConnectionString("ServiceBus")!, TopicTopology.Default);
//endpointConfiguration.UseTransport<AzureServiceBusTransport>("sbns-get-the-message.servicebus.windows.net", new DefaultAzureCredential(), TopicTopology.Default);
  
endpointConfiguration.UsePersistence<LearningPersistence>();
endpointConfiguration.UniquelyIdentifyRunningInstance();
endpointConfiguration.ApplyNamespaceConventions();

builder.UseNServiceBus(endpointConfiguration);

builder.AddServiceDefaults();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

await app.RunAsync();
