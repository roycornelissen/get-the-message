
# resource "azurerm_servicebus_subscription" "shipping-to-sales-order-placed" {
#   name           = "shipping"
#   topic_id       = azurerm_servicebus_topic.sales-order-placed.id
#   forward_to     = azurerm_servicebus_queue.shipping.name
#   max_delivery_count = 10
# }

# resource "azurerm_servicebus_subscription" "shipping-to-billing-payment-received" {
#   name           = "shipping"
#   topic_id       = azurerm_servicebus_topic.billing-payment-received.id
#   forward_to     = azurerm_servicebus_queue.shipping.name
#   max_delivery_count = 10
# }

# resource "azurerm_servicebus_subscription" "shipping-to-sales-customer-became-preferred" {
#   name           = "shipping"
#   topic_id       = azurerm_servicebus_topic.sales-customer-became-preferred.id
#   forward_to     = azurerm_servicebus_queue.shipping.name
#   max_delivery_count = 10
# }


# resource "azurerm_servicebus_subscription" "sales-to-sales-order-placed" {
#   name           = "sales"
#   topic_id       = azurerm_servicebus_topic.sales-order-placed.id
#   forward_to     = azurerm_servicebus_queue.sales.name
#   max_delivery_count = 10
# }

# resource "azurerm_servicebus_subscription" "sales-to-billing-payment-received" {
#   name           = "sales"
#   topic_id       = azurerm_servicebus_topic.billing-payment-received.id
#   forward_to     = azurerm_servicebus_queue.sales.name
#   max_delivery_count = 10
# }

# resource "azurerm_servicebus_subscription" "billing-to-sales-order-accepted" {
#   name           = "billing"
#   topic_id       = azurerm_servicebus_topic.sales-order-accepted.id
#   forward_to     = azurerm_servicebus_queue.billing.name
#   max_delivery_count = 10
# }
