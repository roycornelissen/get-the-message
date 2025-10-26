namespace Sales.CustomerStatus;

public class CustomerStatusData
{
    public Guid CustomerId { get; set; }
    public decimal TotalPurchases { get; set; }
    public bool IsPreferred { get; set; }
}