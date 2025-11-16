namespace Conventions;

public static class MessageConventions
{
    public static void ApplyNamespaceConventions(this EndpointConfiguration endpointConfiguration)
    {
        var conventions = endpointConfiguration.Conventions();

        conventions.DefiningCommandsAs(
            type =>
                type.Namespace != null &&
                type.Namespace.EndsWith("Commands"));

        conventions.DefiningEventsAs(
            type =>
                type.Namespace != null &&
                type.Namespace.EndsWith("Events"));

        conventions.DefiningMessagesAs(
            type => type.Namespace == "Messages");

        conventions.DefiningTimeToBeReceivedAs(
            type =>
            {
                if (type.Name.EndsWith("Expires"))
                {
                    return TimeSpan.FromSeconds(30);
                }
                return TimeSpan.MaxValue;
            });
    }

    public static void ConnectToServicePlatform(this EndpointConfiguration endpointConfiguration)
    {
        var servicePlatformConnection = ServicePlatformConnectionConfiguration.Parse(@"{
            ""Heartbeats"": {
                ""Enabled"": true,
                ""HeartbeatsQueue"": ""Particular.ServiceControl"",
                ""Frequency"": ""00:00:10"",
                ""TimeToLive"": ""00:00:40""
            },
            ""CustomChecks"": {
                ""Enabled"": true,
                ""CustomChecksQueue"": ""Particular.ServiceControl""
            },
            ""ErrorQueue"": ""error"",
            ""SagaAudit"": {
                ""Enabled"": true,
                ""SagaAuditQueue"": ""audit""
            },
            ""MessageAudit"": {
                ""Enabled"": true,
                ""AuditQueue"": ""audit""
            },
            ""Metrics"": {
                ""Enabled"": true,
                ""MetricsQueue"": ""Particular.Monitoring"",
                ""Interval"": ""00:00:01""
            }
        }");

        endpointConfiguration.ConnectToServicePlatform(servicePlatformConnection);
    }
}