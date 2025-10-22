namespace Shipping.MassTransit.InternalCommands;

public class ShipOrder
{
    public Guid OrderId { get; set; }
    public string? Address { get; set; }
}