terraform {
  required_version = ">= 1.0"

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
  }

  # Uncomment and configure backend for remote state
  # backend "azurerm" {
  #   resource_group_name  = "rg-terraform-state"
  #   storage_account_name = "sttfstate"
  #   container_name       = "tfstate"
  #   key                  = "mwl-uat.terraform.tfstate"
  # }
}

provider "azurerm" {
  features {}
}

# Reference existing Resource Group
data "azurerm_resource_group" "main" {
  name = var.resource_group_name
}

# Reference existing App Service Plan
data "azurerm_service_plan" "main" {
  name                = var.app_service_plan_name
  resource_group_name = data.azurerm_resource_group.main.name
}

# Create Application Insights if it doesn't exist
resource "azurerm_application_insights" "main" {
  name                = "${var.app_service_name}-insights"
  location            = data.azurerm_resource_group.main.location
  resource_group_name = data.azurerm_resource_group.main.name
  application_type    = "web"
  retention_in_days   = 90

  tags = {
    Environment = var.environment
    Application = "MyWeekendsLeft"
    ManagedBy   = "Terraform"
  }

  lifecycle {
    ignore_changes = [tags]
  }
}

# Reference existing App Service
resource "azurerm_windows_web_app" "main" {
  name                = var.app_service_name
  location            = data.azurerm_resource_group.main.location
  resource_group_name = data.azurerm_resource_group.main.name
  service_plan_id     = data.azurerm_service_plan.main.id
  https_only          = true

  site_config {
    always_on                          = true
    http2_enabled                      = true
    minimum_tls_version                = "1.2"
    ftps_state                         = "FtpsOnly"
    health_check_path                  = "/health"
    health_check_eviction_time_in_min = 2

    application_stack {
      current_stack  = "dotnet"
      dotnet_version = "v10.0"
    }

    cors {
      allowed_origins = ["*"]
    }
  }

  app_settings = {
    "APPINSIGHTS_INSTRUMENTATIONKEY"             = azurerm_application_insights.main.instrumentation_key
    "APPLICATIONINSIGHTS_CONNECTION_STRING"      = azurerm_application_insights.main.connection_string
    "ApplicationInsightsAgent_EXTENSION_VERSION" = "~3"
    "ASPNETCORE_ENVIRONMENT"                     = var.environment
    "MwlConfiguration__LifeExpectancyApiUri"     = "https://d6wn6bmjj722w.population.io/1.0/life-expectancy/remaining"
  }

  logs {
    application_logs {
      file_system_level = "Information"
    }

    http_logs {
      file_system {
        retention_in_days = 7
        retention_in_mb   = 35
      }
    }
  }

  tags = {
    Environment = var.environment
    Application = "MyWeekendsLeft"
    ManagedBy   = "Terraform"
  }

  lifecycle {
    ignore_changes = [tags]
  }
}
