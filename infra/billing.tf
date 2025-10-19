resource "azurerm_servicebus_queue" "billing" {
  name                 = "billing"
  namespace_id         = azurerm_servicebus_namespace.this.id
  partitioning_enabled = true

  forward_dead_lettered_messages_to    = azurerm_servicebus_queue.dead-letter-billing.name
  dead_lettering_on_message_expiration = true
}

resource "azurerm_servicebus_queue" "dead-letter-billing" {
  name                 = "dead-letter-billing"
  namespace_id         = azurerm_servicebus_namespace.this.id
  partitioning_enabled = true
}

resource "azurerm_servicebus_topic" "billing-payment-received" {
  name                 = "messages.events.paymentreceived"
  namespace_id         = azurerm_servicebus_namespace.this.id
  partitioning_enabled = true
}
