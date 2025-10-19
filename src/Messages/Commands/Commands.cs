namespace Messages.Commands;

public class PlaceOrder
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public decimal Amount { get; set; }
}

public class SendWelcomeEmail
{
    public Guid CustomerId { get; init; }
    public required string EmailAddress { get; init; }
    public required string Name { get; init; }
}

public class CancelOrder
{
    public Guid OrderId { get; set; }
}

public class ProcessPayment
{
    public Guid OrderId { get; set; }
    public required string TransactionReference { get; set; }
}