dotnet tool install -g NServiceBus.Transport.AzureServiceBus.CommandLine

asb-transport queue create "error"
asb-transport endpoint create "sales"
asb-transport endpoint create "billing"
asb-transport endpoint create "shipping"

asb-transport endpoint subscribe "sales" "Messages.Events.OrderPlaced" -p
asb-transport endpoint subscribe "sales" "Messages.Events.OrderCanceled" -p
asb-transport endpoint subscribe "sales" "Messages.Events.CustomerBecamePreferred" -p

asb-transport endpoint subscribe "billing" "Messages.Events.OrderAccepted" -p

asb-transport endpoint subscribe "shipping" "Messages.Events.OrderAccepted" -p
asb-transport endpoint subscribe "shipping" "Messages.Events.PaymentReceived" -p
asb-transport endpoint subscribe "shipping" "Messages.Events.OrderShipped" -p
asb-transport endpoint subscribe "shipping" "Messages.Events.CustomerBecamePreferred" -p
