output "resource_group_name" {
  description = "The name of the resource group"
  value       = azurerm_resource_group.main.name
}

output "location" {
  description = "The Azure region location"
  value       = var.location
}

output "environment" {
  description = "The environment name"
  value       = var.environment
}

output "app_service_name" {
  description = "The name of the App Service"
  value       = azurerm_windows_web_app.main.name
}

output "app_service_default_hostname" {
  description = "The default hostname of the App Service"
  value       = "https://${azurerm_windows_web_app.main.default_hostname}"
}

output "instrumentation_key" {
  description = "Application Insights instrumentation key"
  value       = azurerm_application_insights.main.instrumentation_key
  sensitive   = true
}

output "app_insights_connection_string" {
  description = "Application Insights connection string"
  value       = azurerm_application_insights.main.connection_string
  sensitive   = true
}

output "app_id" {
  description = "Application Insights App ID"
  value       = azurerm_application_insights.main.app_id
}
