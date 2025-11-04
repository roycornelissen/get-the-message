using Messages;
using Messages.Events;
using Wolverine.Http;

namespace Sales;

public record OrderCreationResponse(Guid OrderId) : CreationResponse("/cancel-order/" + OrderId);

public static class PlaceOrderEndpoint
{
    [WolverineGet("/place-order")]
    public static (OrderCreationResponse, OrderPlaced) Get(ILogger<OrderPlaced> logger)
    {
        var orderId = Guid.NewGuid();

        // Do all kinds of validation
        // Store order in database

        var message = new OrderPlaced
        {
            OrderId = orderId,
            Amount = new Random().Next(500, 600),
            CustomerId = Constants.DefaultCustomerId
        };

        logger.LogInformation("💸 A new order {OrderId} for amount {Amount} came in", orderId, message.Amount);

        return (
            new OrderCreationResponse(orderId),
            message);
    }
}
