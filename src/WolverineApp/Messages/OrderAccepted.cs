namespace WolverineApp.Messages;

public record OrderAccepted(Guid Id);

public class OrderAcceptedHandler
{
    public void Handle(OrderAccepted message, ILogger<OrderAccepted> logger)
    {
        logger.LogInformation($"OrderAccepted received: {message.Id}");
    }
}