# Configure the Azure provider
provider "azurerm" { 
    # The "feature" block is required for AzureRM provider 2.x. 
    # If you are using version 1.x, the "features" block is not allowed.
    version = "~>2.0"
    features {}
}

resource "azurerm_resource_group" "main" {
    name = "rg-mwl-${var.environment}-001"
    location = var.location
}

resource "azurerm_app_service_plan" "main" {
    name                = "plan-mwl-${var.environment}-001"
    location            = azurerm_resource_group.main.location
    resource_group_name = azurerm_resource_group.main.name
    kind = "Linux"
    reserved = true
    
    sku {
        tier = "Standard"
        size = "S1"
    }
}

resource "azurerm_app_service" "main" {
    name                = "app-mwl-${var.environment}-001"
    location            = azurerm_resource_group.main.location
    resource_group_name = azurerm_resource_group.main.name
    app_service_plan_id = azurerm_app_service_plan.main.id

    site_config {
            linux_fx_version = "DOTNETCORE|3.1"
            min_tls_version = "1.2"
    }
}

resource "azurerm_app_service_slot" "main" {
    name                = "slot-mwl-${var.environment}-staging"
    location            = azurerm_resource_group.main.location
    resource_group_name = azurerm_resource_group.main.name
    app_service_plan_id = azurerm_app_service_plan.main.id
    app_service_name    = azurerm_app_service.main.name

    site_config {
            linux_fx_version = "DOTNETCORE|3.1"
            min_tls_version = "1.2"
    }
}

resource "azurerm_application_insights" "main" {
  name                = "appi-mwl-${var.environment}-001"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  application_type    = "web"
}