namespace CustomerServiceAgent;

public interface ICustomerServiceAgent
{
    Task SendWelcomeEmail(string emailAddress, string name, CancellationToken cancellationToken) => Task.CompletedTask;
}

internal class NoOpCustomerServiceAgent : ICustomerServiceAgent {}
