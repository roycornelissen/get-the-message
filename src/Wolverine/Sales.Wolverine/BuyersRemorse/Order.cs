using Messages.Events;
using Wolverine;

namespace Sales.BuyersRemorse;

public record OrderTimeoutMessage(Guid Id) : TimeoutMessage(TimeSpan.FromSeconds(10));

public class Order : Saga
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public decimal Amount { get; set; }
    
    public static (Order, TimeoutMessage) Start(OrderPlaced message, ILogger<OrderPlaced> logger)
    {
        logger.LogInformation("💭 Starting buyers remorse timer for order {OrderId}", message.OrderId);
        
        return (
            new Order
            {
                Id = message.OrderId,
                Amount = message.Amount,
                CustomerId = message.CustomerId
            },
            new OrderTimeoutMessage(message.OrderId)
        );
    }

    public void Handle(OrderCanceled message, ILogger<OrderPlaced> logger)
    {
        logger.LogInformation("🛑 Order {OrderId} was canceled within Buyer's Remorse Period", message.OrderId);
        MarkCompleted();
    }
    
    public OrderAccepted Handle(OrderTimeoutMessage message, ILogger<OrderPlaced> logger)
    {
        logger.LogInformation("🕛 Time's up for OrderId {OrderId}, finalizing order", message.Id);

        MarkCompleted();

        return
            new OrderAccepted
            {
                OrderId = Id,
                CustomerId = CustomerId,
                Amount = Amount
            };
    }

    public static void NotFound(OrderTimeoutMessage message, ILogger<OrderTimeoutMessage> logger)
    {
        logger.LogInformation("🕛 Timeout received for OrderId {OrderId}, but saga not found - assuming order was canceled", message.Id);
    }
}