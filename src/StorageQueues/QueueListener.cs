
using Azure.Storage.Queues;
using Messages.Events;
using System.Text.Json;

namespace StorageQueues;

public class QueueListener(IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = serviceScopeFactory.CreateScope();

        var client = scope.ServiceProvider.GetRequiredService<QueueClient>();

        while (!stoppingToken.IsCancellationRequested)
        {
            var peekedMessage = await client.PeekMessageAsync(stoppingToken);
            if (peekedMessage.Value != null)
            {
                Console.WriteLine($"Peeked message: {peekedMessage.Value.MessageText}");

                // What about concurrency (multiple workers)?
                var message = await client.ReceiveMessageAsync(cancellationToken: stoppingToken);

                // Figure out the message type and deserialize
                var data = await JsonSerializer
                    .DeserializeAsync<OrderAccepted>(message.Value.Body.ToStream(), cancellationToken: stoppingToken);

                if (data != null)
                {
                    try
                    {
                        // Find the appropriate handler and process the message
                        var handler = scope.ServiceProvider.GetRequiredService<OrderAcceptedHandler>();
                        await handler.Handle(data, stoppingToken);

                        await client.DeleteMessageAsync(message.Value.MessageId, message.Value.PopReceipt, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        // Error handling logic...
                        // What about retries?
                        // What about logging?
                        // What about transactions?
                        // What about dead-lettering?
                        Console.WriteLine($"Error processing message: {ex.Message}");
                    }
                }
            }

            // Some kind of backoff mechanism to avoid tight loop
            await Task.Delay(5000, stoppingToken);
        }
    }
}
