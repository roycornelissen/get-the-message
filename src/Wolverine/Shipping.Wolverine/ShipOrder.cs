namespace Shipping;

public class ShipOrder
{
    public Guid OrderId { get; set; }
    public required string Address { get; set; }
}