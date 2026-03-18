using Dapr.Workflow;
using Messages.Events;

namespace Shipping;

internal sealed class OrderShippingWorkflow : Workflow<OrderAccepted, OrderShipped>
{
    public override async Task<OrderShipped> RunAsync(WorkflowContext context, OrderAccepted input)
    {
        var logger = context.CreateReplaySafeLogger<OrderShippingWorkflow>();

        logger.LogInformation("📦 Order {OrderId} accepted, waiting for payment", input.OrderId);

        var paymentReceived = await context.WaitForExternalEventAsync<PaymentReceived>("PaymentReceived");
        
        logger.LogInformation("🚚 Payment {Amount} received for order {OrderId}, shipping order", paymentReceived.Amount, paymentReceived.OrderId);
        
        var retryPolicy = new WorkflowRetryPolicy(
            maxNumberOfAttempts: 3,
            firstRetryInterval: TimeSpan.FromSeconds(2));

        var address = await context.CallActivityAsync<string>(
            nameof(FetchShippingAddressActivity),
            input.CustomerId,
            new WorkflowTaskOptions { RetryPolicy = retryPolicy });
        
        // Fetch the shipping address
        var orderShipped = await context.CallActivityAsync<OrderShipped>(nameof(OrderShipperActivity), new ShipOrder { Address = address, OrderId = input.OrderId });

        return orderShipped;
    }
}