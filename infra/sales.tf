resource "azurerm_servicebus_queue" "sales" {
  name                 = "sales"
  namespace_id         = azurerm_servicebus_namespace.this.id
  partitioning_enabled = true

  forward_dead_lettered_messages_to    = azurerm_servicebus_queue.dead-letter-sales.name
  dead_lettering_on_message_expiration = true
}

resource "azurerm_servicebus_queue" "dead-letter-sales" {
  name                 = "dead-letter-sales"
  namespace_id         = azurerm_servicebus_namespace.this.id
  partitioning_enabled = true
}

resource "azurerm_servicebus_topic" "sales-order-placed" {
  name                 = "messages.events.orderplaced"
  namespace_id         = azurerm_servicebus_namespace.this.id
  partitioning_enabled = true
}

resource "azurerm_servicebus_topic" "sales-order-canceled" {
  name                 = "messages.events.ordercanceled"
  namespace_id         = azurerm_servicebus_namespace.this.id
  partitioning_enabled = true
}

resource "azurerm_servicebus_topic" "sales-order-accepted" {
  name                 = "messages.events.orderaccepted"
  namespace_id         = azurerm_servicebus_namespace.this.id
  partitioning_enabled = true
}

resource "azurerm_servicebus_topic" "sales-customer-became-preferred" {
  name                 = "messages.events.customerbecamepreferred"
  namespace_id         = azurerm_servicebus_namespace.this.id
  partitioning_enabled = true
}
