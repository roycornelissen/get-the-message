using Messages.Commands;
using Messages.Events;

namespace Sales;

public static class OrderCancellationHandler
{
    public static OrderCanceled Handle(CancelOrder message, ILogger<OrderCanceled> logger)
    {
        logger.LogInformation("☹️ Cancelling order {OrderId}", message.OrderId);

        if (CoinFlipper.FlipCoin())
        {
            throw new InvalidOperationException("☠️☠️☠️☠️ SIMULATED ERROR ☠️☠️☠️☠️");
        }

        return new()
        {
            OrderId = message.OrderId
        };
    }
}