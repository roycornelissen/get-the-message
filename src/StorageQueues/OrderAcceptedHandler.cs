using Messages.Events;

namespace StorageQueues;

public class OrderAcceptedHandler(ILogger<OrderAcceptedHandler> logger)
{
    public async Task Handle(OrderAccepted message, CancellationToken cancellationToken)
    {
        await Task.Delay(500, cancellationToken);

        if (CoinFlipper.FlipCoin())
        {
            throw new Exception("Simulated random failure processing OrderAccepted message.");
        }

        logger.LogInformation("⚡ Received OrderAccepted event for OrderId: {OrderId}, CustomerId: {CustomerId}, Amount: {Amount}",
            message.OrderId, message.CustomerId, message.Amount);
    }
}
