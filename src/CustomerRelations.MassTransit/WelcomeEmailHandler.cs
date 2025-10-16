using MassTransit;
using Messages;

namespace CustomerRelations.MassTransit;

public class WelcomeEmailHandler : IConsumer<CustomerBecamePreferred>
{
    public Task Consume(ConsumeContext<CustomerBecamePreferred> context)
    {
        Console.WriteLine("Wow, customer became preferred!");
        return Task.CompletedTask;
    }
}