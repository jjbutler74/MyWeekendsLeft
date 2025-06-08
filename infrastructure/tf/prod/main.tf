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
    kind = "Windows"
    
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
    https_only = "true"

    site_config {
            min_tls_version = "1.2"
    }

    app_settings = {
        "APPINSIGHTS_INSTRUMENTATIONKEY" = azurerm_application_insights.main.instrumentation_key
    }

}

resource "azurerm_template_deployment" "webapp-corestack" {
  # This will make it .NET CORE for Stack property, and add the dotnet core logging extension
  name                = "AspNetCoreStack"
  resource_group_name = azurerm_resource_group.main.name
  template_body       = <<DEPLOY
{
    "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
    "contentVersion": "1.0.0.0",
    "parameters": {
        "siteName": {
            "type": "string",
            "metadata": {
                "description": "The Azure App Service Name"
            }
        },
        "extensionName": {
            "type": "string",
            "metadata": {
                "description": "The Site Extension Name."
            }
        },
        "extensionVersion": {
            "type": "string",
            "metadata": {
                "description": "The Extension Version"
            }
        }
    },
    "resources": [
        {
            "apiVersion": "2018-02-01",
            "name": "[parameters('siteName')]",
            "type": "Microsoft.Web/sites",
            "location": "[resourceGroup().location]",
            "properties": {
                "name": "[parameters('siteName')]",
                "siteConfig": {
                    "appSettings": [],
                    "metadata": [
                        {
                            "name": "CURRENT_STACK",
                            "value": "dotnetcore"
                        }
                    ]
                }
            }
        },
        {
            "type": "Microsoft.Web/sites/siteextensions",
            "name": "[concat(parameters('siteName'), '/', parameters('extensionName'))]",
            "apiVersion": "2018-11-01",
            "location": "[resourceGroup().location]",
            "properties": {
                "version": "[parameters('extensionVersion')]"
            }
        }
    ]
}
  DEPLOY
  parameters = {
    "siteName"         = azurerm_app_service.main.name
    "extensionName"    = "Microsoft.AspNetCore.AzureAppServices.SiteExtension"
    "extensionVersion" = "9.0.0"
  }
  deployment_mode = "Incremental"
  depends_on      = [azurerm_app_service.main]
}

resource "azurerm_app_service_slot" "main" {
    name                = "slot-mwl-${var.environment}-staging"
    location            = azurerm_resource_group.main.location
    resource_group_name = azurerm_resource_group.main.name
    app_service_plan_id = azurerm_app_service_plan.main.id
    app_service_name    = azurerm_app_service.main.name

    site_config {
            min_tls_version = "1.2"
    }
}

resource "azurerm_application_insights" "main" {
  #name                = "appi-mwl-${var.environment}-001"
  name                = "appi-mwl-prod-001"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  application_type    = "web"
}
