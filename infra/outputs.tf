output "servicebus_namespace_name" {
  description = "The name of the Service Bus Namespace"
  value       = azurerm_servicebus_namespace.this.name
}

output "servicebus_namespace_connection_string" {
  description = "The connection string of the Service Bus Namespace"
  value       = azurerm_servicebus_namespace.this.default_primary_connection_string
  sensitive   = true
}
