using Messages.Events;
using Wolverine;

namespace Sales.CustomerStatus;

public record AmountTimeOutExpired(Guid CustomerId, decimal Amount) : TimeoutMessage(TimeSpan.FromMinutes(2));

public class Customer : Saga
{
    public Guid Id { get; set; }
    public decimal TotalOrdersAmount { get; set; }
    public bool IsPreferredCustomer { get; set; }
    
    public static Customer Start(OrderAccepted message, IMessageContext context, ILogger<Customer> logger)
    {
        logger.LogInformation("👤 Customer {CustomerId} is now a regular customer", message.CustomerId);
        return new Customer
        {
            Id = message.CustomerId
        };
    }

    public void Handle(OrderAccepted message, ILogger<Customer> logger)
    {
        logger.LogInformation("Order arrived for known customer {CustomerId}", message.CustomerId);
    }
    
    public (AmountTimeOutExpired, CustomerBecamePreferred?) Handle(PaymentReceived message, ILogger<Customer> logger)
    {
        TotalOrdersAmount += message.Amount;
        logger.LogInformation("🏅 Amount {Amount} purchased by Customer {CustomerId}, now has a total of {Total}",
            message.Amount, message.CustomerId, TotalOrdersAmount);

        CustomerBecamePreferred? preferredCustomer = null;
        if (TotalOrdersAmount > 1000 && !IsPreferredCustomer)
        {
            logger.LogInformation("🏆 Customer {CustomerId} became a preferred customer!", message.CustomerId);
            IsPreferredCustomer = true;

            preferredCustomer = new CustomerBecamePreferred { CustomerId = message.CustomerId };
        }
        return (
            new AmountTimeOutExpired(message.CustomerId, message.Amount),
            preferredCustomer
        );
    }
    
    public Task Handle(AmountTimeOutExpired message, ILogger<Customer> logger)
    {
        TotalOrdersAmount -= message.Amount;
        logger.LogInformation("⌛ An amount of {Amount} for Customer {CustomerId} is no longer eligible for customer status; total is now {Total}",
            message.Amount, message.CustomerId, TotalOrdersAmount);

        if (TotalOrdersAmount <= 1000)
        {
            logger.LogInformation("👤 Customer {CustomerId} is no longer a preferred customer!", message.CustomerId);
            IsPreferredCustomer = false;
        }
        
        return Task.CompletedTask;
    }
}