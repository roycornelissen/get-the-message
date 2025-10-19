resource "azurerm_servicebus_queue" "shipping" {
  name                 = "shipping"
  namespace_id         = azurerm_servicebus_namespace.this.id
  partitioning_enabled = true

  forward_dead_lettered_messages_to    = azurerm_servicebus_queue.dead-letter-shipping.name
  dead_lettering_on_message_expiration = true
}

resource "azurerm_servicebus_queue" "dead-letter-shipping" {
  name                 = "dead-letter-shipping"
  namespace_id         = azurerm_servicebus_namespace.this.id
  partitioning_enabled = true
}

resource "azurerm_servicebus_topic" "shipping-order-shipped" {
  name                 = "messages.events.ordershipped"
  namespace_id         = azurerm_servicebus_namespace.this.id
  partitioning_enabled = true
}
