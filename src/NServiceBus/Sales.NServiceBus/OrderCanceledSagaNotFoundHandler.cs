using Messages.Events;

namespace Sales.NServiceBus;

public class OrderCanceledSagaNotFoundHandler(ILogger<OrderCanceledSagaNotFoundHandler> logger) : ISagaNotFoundHandler
{
    public Task Handle(object message, IMessageProcessingContext context)
    {
        if (message is OrderCanceled cancellation)
        {
            logger.LogInformation("😳 Received a cancellation for order {OrderId} but it came in too late. Could initiate some compensating transaction here.", cancellation.OrderId);
        }

        return Task.CompletedTask;
    }
}
