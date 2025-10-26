using Dapr.Workflow;
using Messages.Events;

namespace Shipping;

internal sealed partial class WaitForPaymentActivity(ILogger<WaitForPaymentActivity> logger) : WorkflowActivity<Guid, PaymentReceived>
{
    public override async Task<PaymentReceived> RunAsync(WorkflowActivityContext context, Guid orderId)
    {
        LogWaitingForPayment(logger, orderId);
        
        // Simulate slow processing & sending the approval to the recipient
        await Task.Delay(TimeSpan.FromSeconds(2));

        return new PaymentReceived { OrderId = orderId };
    }
    
    [LoggerMessage(LogLevel.Information, "📦 Order {OrderId} accepted, waiting for payment")]
    static partial void LogWaitingForPayment(ILogger logger, Guid orderId);
}