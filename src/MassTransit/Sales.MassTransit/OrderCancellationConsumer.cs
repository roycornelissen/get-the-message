using MassTransit;
using Messages.Commands;
using Messages.Events;

namespace Sales.MassTransit;

public class OrderCancellationConsumer(ILogger<OrderCancellationConsumer> logger)
    : IConsumer<CancelOrder>
{
    public async Task Consume(ConsumeContext<CancelOrder> context)
    {
        logger.LogInformation("☹️ Cancelling order {OrderId}", context.Message.OrderId);

        if (CoinFlipper.FlipCoin())
        {
            throw new InvalidOperationException("☠️☠️☠️☠️ SIMULATED ERROR ☠️☠️☠️☠️");
        }
        
        await context.Publish(new OrderCanceled
        {
            OrderId = context.Message.OrderId
        }, context.CancellationToken);
    }
}