using Aspire.Hosting;
using Projects;

namespace AppHost;

internal static class StorageQueuesSetup
{
    public static void RunStorageQueues(this IDistributedApplicationBuilder builder)
    {
        var storage = builder.AddAzureStorage("azurestorage")
            .RunAsEmulator()
            .AddQueue("sales");

        builder
            .AddProject<StorageQueues>("storagequeues")
            .WithUrl("/swagger")
            .WithReference(storage)
            .WaitFor(storage);
    }
}