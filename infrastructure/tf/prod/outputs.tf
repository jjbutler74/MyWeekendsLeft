output "resource_group_name" {
  value = "${azurerm_resource_group.main.name}"
}

output "location" {
  value = "${var.location}"
}

output "environment" {
  value = "${var.environment}"
}

output "app_service_name" {
  value = "${azurerm_app_service.main.name}"
}

output "app_service_default_hostname" {
  value = "https://${azurerm_app_service.main.default_site_hostname}"
}

output "instrumentation_key" {
  value = azurerm_application_insights.main.instrumentation_key
}

output "app_id" {
  value = azurerm_application_insights.main.app_id
}