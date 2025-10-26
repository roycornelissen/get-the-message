using Dapr.Workflow;
using Messages.Events;

namespace Shipping; 

internal sealed class OrderShipperActivity(ILogger<OrderShipperActivity> logger) : WorkflowActivity<ShipOrder, OrderShipped>
{
    public override async Task<OrderShipped> RunAsync(WorkflowActivityContext context, ShipOrder command)
    {
        logger.LogInformation("🚚 Shipping order {OrderId} to address {Address}", command.OrderId, command.Address);

        // Simulate calling external shipping service to register shipping order
        await Task.Delay(5000);

        return new OrderShipped
        {
            OrderId = command.OrderId,
            ShippedAt = DateTime.UtcNow
        };
    }
}