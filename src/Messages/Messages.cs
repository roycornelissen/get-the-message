namespace Messages;

public class PlaceOrder
{
    public Guid OrderId { get; set; }
    public Guid ClientId { get; set; }
    public decimal Amount { get; set; }
}

public class OrderPlaced
{
    public Guid OrderId { get; set; }
    public Guid ClientId { get; set; }
    public decimal Amount { get; set; }
}

public class OrderAccepted
{
    public Guid OrderId { get; set; }
}

public class CancelOrder
{
    public Guid OrderId { get; set; }
}

public class OrderCancelled
{
    public Guid OrderId { get; set; }
}

public class PaymentReceived
{
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
}

public class CustomerBecamePreferred
{
    public Guid ClientId { get; set; }
}

public class SendWelcomeEmail
{
    public Guid ClientId { get; set; }
}
