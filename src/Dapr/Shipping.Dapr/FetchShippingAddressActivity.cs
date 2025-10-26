using Dapr.Workflow;

namespace Shipping;

internal sealed class FetchShippingAddressActivity(ILogger<FetchShippingAddressActivity> logger) : WorkflowActivity<Guid, string>
{
    public override Task<string> RunAsync(WorkflowActivityContext context, Guid customerId)
    {
        logger.LogInformation("📬 Fetching shipping address for CustomerId {CustomerId}", customerId);
        
        // simulate an error occasionally
        if (CoinFlipper.FlipCoin())
        {
            throw new InvalidOperationException("☠️☠️☠️☠️ SIMULATED ERROR ☠️☠️☠️☠️");
        }
        
        var shippingAddress = "123 Main St, Anytown, USA"; // random address for demo purposes
        return Task.FromResult(shippingAddress);
    }
}