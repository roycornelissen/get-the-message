namespace Shipping.NServiceBus.Messages;

public class ShipOrder : ICommand
{
    public Guid OrderId { get; set; }
    public string? Address { get; set; }
}