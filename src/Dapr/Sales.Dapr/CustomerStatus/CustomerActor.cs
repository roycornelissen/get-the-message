using System.Text.Json;
using Dapr.Actors;
using Dapr.Actors.Runtime;
using Messages.Events;

namespace Sales.CustomerStatus;

public interface ICustomerActor: IActor
{
    Task TrackPayment(PaymentReceived message, CancellationToken cancellationToken);
}

public class CustomerActor(ActorHost host) :
    Actor(host), ICustomerActor, IRemindable
{
    private readonly string _stateDataKey = "customer-data";
    private readonly string _amountExpiredReminder = "amount-expired";
    
    public async Task TrackPayment(PaymentReceived message, CancellationToken cancellationToken)
    {
        var stateOrNot = await StateManager.TryGetStateAsync<CustomerStatusData>(_stateDataKey, cancellationToken);
        
        var state = stateOrNot.HasValue ? stateOrNot.Value : new CustomerStatusData();
        
        state.CustomerId = message.CustomerId;
        state.TotalPurchases += message.Amount;
        Logger.LogInformation("🏅 Amount {Amount} purchased by Customer {CustomerId}, now has a total of {Total}", message.Amount, message.CustomerId, state.TotalPurchases);

        if (state is { TotalPurchases: > 1000, IsPreferred: false })
        {
            Logger.LogInformation("🏅 Customer {CustomerId} is now a preferred customer!", state.CustomerId);
            state.IsPreferred = true;
            await StateManager.SetStateAsync(_stateDataKey, state, cancellationToken);

            var amount = JsonSerializer.SerializeToUtf8Bytes(message.Amount);
            
            await RegisterReminderAsync(_amountExpiredReminder, amount, TimeSpan.FromMinutes(2),
                TimeSpan.FromMilliseconds(-1));
        }
    }

    public async Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
    {
        if (reminderName == _amountExpiredReminder)
        {
            var customerState = await StateManager.GetStateAsync<CustomerStatusData>(_stateDataKey);
            var amount = await JsonSerializer.DeserializeAsync<decimal>(new MemoryStream(state));
            
            Logger.LogInformation("⌛ Amount {Amount} for Customer {CustomerId} has expired from total purchases", amount, customerState.CustomerId);
            customerState.TotalPurchases -= amount;
        }
    }
}