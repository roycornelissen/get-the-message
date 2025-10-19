namespace Messages.Events;

public class OrderPlaced
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public decimal Amount { get; set; }
}

public class OrderAccepted
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public decimal Amount { get; set; }
}

public class OrderCanceled
{
    public Guid OrderId { get; set; }
}

public class PaymentReceived
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public decimal Amount { get; set; }
}

public class CustomerBecamePreferred
{
    public Guid CustomerId { get; set; }
}

public class OrderShipped
{
    public Guid OrderId { get; set; }
    public DateTime ShippedAt { get; set; }
}