using Messages.Commands;
using Messages.Events;

namespace Sales.NServiceBus;

public class OrderCancellationHandler(ILogger<OrderCancellationHandler> logger): IHandleMessages<CancelOrder>
{
    public async Task Handle(CancelOrder message, IMessageHandlerContext context)
    {
        logger.LogInformation("☹️ Cancelling order {OrderId}", message.OrderId);

        if (CoinFlipper.FlipCoin())
        {
            throw new InvalidOperationException("☠️☠️☠️☠️ SIMULATED ERROR ☠️☠️☠️☠️");
        }
        
        await context.Publish<OrderCanceled>(evt =>
        {
            evt.OrderId = message.OrderId;
        });
    }
}