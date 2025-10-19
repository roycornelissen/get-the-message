data "azurerm_client_config" "current" {}

data "azurerm_resource_group" "this" {
  name     = "rg-messaging"
}

resource "azurerm_servicebus_namespace" "this" {
  name                = "sbns-get-the-message"
  location            = data.azurerm_resource_group.this.location
  resource_group_name = data.azurerm_resource_group.this.name
  sku                 = "Standard"

  tags = data.azurerm_resource_group.this.tags
}

resource "azurerm_role_assignment" "service_bus_data_owner" {
  principal_id         = data.azurerm_client_config.current.object_id
  role_definition_name = "Azure Service Bus Data Owner"
  scope                = azurerm_servicebus_namespace.this.id
}
