using Messages.Events;
using Wolverine;

namespace Shipping.Wolverine.UseCases;

public static class OrderAcceptedHandler
{
    public static async Task Handle(OrderAccepted orderAccepted, IMessageBus bus)
    {
        // Simulate some shipping processing logic
        await Task.Delay(500); // Simulating async work, e.g., updating a database or calling an external service

        Console.WriteLine($"Order {orderAccepted.OrderId} accepted for shipping.");

        var @event = new CustomerBecamePreferred() { CustomerId = Guid.NewGuid() };

        await bus.PublishAsync(@event);
    }
}