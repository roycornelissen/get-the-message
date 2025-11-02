using Aspire.Hosting;
using Projects;

namespace AppHost;

internal static class StorageQueuesSetup
{
    public static void RunStorageQueues(this IDistributedApplicationBuilder builder)
    {
        builder
            .AddProject<StorageQueues>("storagequeues");
    }
}