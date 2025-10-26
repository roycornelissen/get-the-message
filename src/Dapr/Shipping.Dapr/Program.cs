using System.Text;
using Dapr;
using Dapr.Client;
using Dapr.Workflow;
using Messages.Events;
using Shipping;

Console.OutputEncoding = Encoding.UTF8;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults(useHttpResiliency: false); // the standard resiliency policies can interfere with workflow long polling operations

builder.Services.AddDaprClient();
builder.Services.AddDaprWorkflow(options =>
{
    // Note that it's also possible to register a lambda function as the workflow
    // or activity implementation instead of a class.
    options.RegisterWorkflow<OrderShippingWorkflow>();

    // These are the activities that get invoked by the workflow(s).
    options.RegisterActivity<OrderShipperActivity>();
    options.RegisterActivity<WaitForPaymentActivity>();
    options.RegisterActivity<FetchShippingAddressActivity>();
});

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

app.MapPost("/order-accepted", [Topic("pubsub", "order-accepted")] async (DaprWorkflowClient workflowClient, OrderAccepted order, CancellationToken cancellationToken) => {
    await workflowClient.ScheduleNewWorkflowAsync(nameof(OrderShippingWorkflow), order.OrderId.ToString(), order);
    await workflowClient.WaitForWorkflowStartAsync(order.OrderId.ToString(), false, cancellationToken);
    return Results.Ok(order);
});

app.MapPost("/payment-received", [Topic("pubsub", "payment-received")] async (DaprWorkflowClient workflowClient, DaprClient daprClient, ILogger<PaymentReceived> logger, PaymentReceived payment, CancellationToken cancellationToken) =>
{
    var workflowState = await workflowClient.GetWorkflowStateAsync(payment.OrderId.ToString(), true);

    if (workflowState.Exists)
    {
        await workflowClient.RaiseEventAsync(payment.OrderId.ToString(), nameof(PaymentReceived), payment,
            cancellationToken);
        var state = await workflowClient.WaitForWorkflowCompletionAsync(payment.OrderId.ToString(), true,
            cancellationToken);

        var @event = state.ReadOutputAs<OrderShipped>();
        await daprClient.PublishEventAsync("pubsub", "order-shipped", @event, cancellationToken);

        return Results.Accepted();
    }
    
    logger.LogWarning("No workflow found for order {OrderId} to receive payment.", payment.OrderId);
    return Results.NotFound();
});

app.MapPost("/order-shipped", [Topic("pubsub", "order-shipped")] (OrderShipped message, ILogger<OrderShipped> logger) =>
{
    logger.LogInformation("🔍 AUDITING: Order {OrderId} was shipped at {ShippedAt}", message.OrderId, message.ShippedAt);
    return Results.Accepted();
});

app.Run();
