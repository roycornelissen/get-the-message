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
}