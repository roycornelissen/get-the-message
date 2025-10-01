namespace WolverineApp.Messages;

public record OrderPlaced(Guid Id, Guid CustomerId, decimal Amount);

public class OrderPlacedHandler
{
    public async Task<OrderAccepted> Handle(OrderPlaced message, ILogger<OrderPlaced> logger, CancellationToken cancellationToken)
    {
        await Task.Delay(1000, cancellationToken);
        logger.LogInformation($"OrderPlaced received: {message.Id}, Customer: {message.CustomerId}, Amount: {message.Amount}");
        return new OrderAccepted(message.Id);
    }
}