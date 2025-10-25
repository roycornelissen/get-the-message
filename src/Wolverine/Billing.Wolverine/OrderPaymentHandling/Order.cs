using Messages.Commands;
using Messages.Events;
using Wolverine;

namespace Billing.OrderPaymentHandling;

public class Order : Saga
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public decimal Amount { get; set; }
    
    public static Order Start(OrderAccepted message, ILogger<Order> logger)
    {
        logger.LogInformation("💰 Billing order {OrderId} for amount {Amount}", message.OrderId, message.Amount);

        return new()
        {
            Id = message.OrderId,
            CustomerId = message.CustomerId,
            Amount = message.Amount
        };
    }
    
    public async ValueTask Handle(ProcessPayment message, IMessageContext context, ILogger<Order> logger)
    {
        logger.LogInformation("💳 Processing payment for order {OrderId}", message.OrderId);
        MarkCompleted();
        await context.PublishAsync(new PaymentReceived
        {
            OrderId = Id,
            CustomerId = CustomerId,
            Amount = Amount
        });
    }
}