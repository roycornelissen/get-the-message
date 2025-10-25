using Messages.Events;
using Wolverine;

namespace Shipping.ShippingProcess;

public class Order : Saga
{
    public Guid Id { get; set; }
    
    public static Order Start(OrderAccepted message, ILogger<Order> logger)
    {
        logger.LogInformation("📦 Order {OrderId} accepted, waiting for payment", message.OrderId);
        
        return new Order
        {
            Id = message.OrderId,
        };
    }
    
    public ShipOrder Handle(PaymentReceived message, ILogger<Order> logger)
    {
        logger.LogInformation("🚚 Payment received for order {OrderId}, shipping order", message.OrderId);
        return new ShipOrder
        {
            OrderId = message.OrderId,
            Address = "some address"
        };
    }

    public void Handle(OrderShipped message, ILogger<Order> logger)
    {
        logger.LogInformation("✅ Order {OrderId} was shipped, done with this order", message.OrderId);
        MarkCompleted();
    }
}