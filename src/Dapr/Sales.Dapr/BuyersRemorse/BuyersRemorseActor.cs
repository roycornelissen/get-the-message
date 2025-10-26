using Dapr.Actors;
using Dapr.Actors.Runtime;
using Dapr.Client;
using Messages.Events;

namespace Sales.BuyersRemorse;

public interface IBuyersRemorseActor : IActor
{
    public Task InitiateBuyersRemorsePeriod(OrderPlaced message, CancellationToken cancellationToken);
    public Task CancelOrder(CancellationToken cancellationToken);
}

public class BuyersRemorseActor(ActorHost host, DaprClient daprClient):
    Actor(host), IBuyersRemorseActor, IRemindable
{
    private readonly string _stateDataKey = "order-data";
    private readonly string _waitExpiredReminder = "wait-expired";
    
    public async Task InitiateBuyersRemorsePeriod(OrderPlaced message, CancellationToken cancellationToken)
    {
        Logger.LogInformation("🕛 Started Buyer's Remorse Saga for OrderId {OrderId}", message.OrderId);

        await StateManager.SetStateAsync(_stateDataKey, message, cancellationToken);
        await RegisterReminderAsync(_waitExpiredReminder, [], TimeSpan.FromSeconds(30), TimeSpan.FromMilliseconds(-1));
    }

    public async Task CancelOrder(CancellationToken cancellationToken)
    {
        var state = await StateManager.TryGetStateAsync<OrderPlaced>(_stateDataKey, cancellationToken);

        if (state.HasValue)
        {
            Logger.LogInformation("🛑 Order {OrderId} was canceled within Buyer's Remorse Period", state.Value.OrderId);
            await StateManager.RemoveStateAsync(_stateDataKey, cancellationToken);
            await UnregisterReminderAsync(_waitExpiredReminder);
        }
        else
        {
            Logger.LogInformation("🙈 Order {OrderId} was canceled but Buyer's Remorse Period has already expired, so ignoring", Id.ToString());
        }
    }

    public async Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
    {
        if (reminderName == _waitExpiredReminder)
        {
            var order = await StateManager.GetStateAsync<OrderPlaced>(_stateDataKey);
            Logger.LogInformation("🕛 Time's up for OrderId {OrderId}, finalizing order", order.OrderId);
            await daprClient.PublishEventAsync("pubsub", "order-accepted", new OrderAccepted
            {
                OrderId = order.OrderId,
                CustomerId = order.CustomerId,
                Amount = order.Amount
            });

            await StateManager.RemoveStateAsync(_stateDataKey);
        }
    }
}