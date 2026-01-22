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
  #   key                  = "mwl-prod.terraform.tfstate"
  # }
}

provider "azurerm" {
  features {}
}

# Resource Group
resource "azurerm_resource_group" "main" {
  name     = "rg-mwl-${var.environment}-001"
  location = var.location

  tags = {
    Environment = var.environment
    Application = "MyWeekendsLeft"
    ManagedBy   = "Terraform"
    CostCenter  = "Personal"
  }
}

# App Service Plan
resource "azurerm_service_plan" "main" {
  name                = "plan-mwl-${var.environment}-001"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  os_type             = "Windows"
  sku_name            = "S1"

  tags = azurerm_resource_group.main.tags
}

# Application Insights
resource "azurerm_application_insights" "main" {
  name                = "appi-mwl-${var.environment}-001"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  application_type    = "web"
  retention_in_days   = 90

  tags = azurerm_resource_group.main.tags
}

# App Service (Production)
resource "azurerm_windows_web_app" "main" {
  name                = "app-mwl-${var.environment}-001"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  service_plan_id     = azurerm_service_plan.main.id
  https_only          = true

  site_config {
    always_on                               = true
    http2_enabled                           = true
    minimum_tls_version                     = "1.2"
    ftps_state                              = "FtpsOnly"
    health_check_path                       = "/health"
    health_check_eviction_time_in_min      = 2

    application_stack {
      current_stack  = "dotnet"
      dotnet_version = "v9.0"
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

  tags = azurerm_resource_group.main.tags
}

# App Service Staging Slot
resource "azurerm_windows_web_app_slot" "staging" {
  name           = "staging"
  app_service_id = azurerm_windows_web_app.main.id
  https_only     = true

  site_config {
    always_on                          = true
    http2_enabled                      = true
    minimum_tls_version                = "1.2"
    ftps_state                         = "FtpsOnly"
    health_check_path                  = "/health"
    health_check_eviction_time_in_min = 2

    application_stack {
      current_stack  = "dotnet"
      dotnet_version = "v9.0"
    }

    cors {
      allowed_origins = ["*"]
    }
  }

  app_settings = {
    "APPINSIGHTS_INSTRUMENTATIONKEY"             = azurerm_application_insights.main.instrumentation_key
    "APPLICATIONINSIGHTS_CONNECTION_STRING"      = azurerm_application_insights.main.connection_string
    "ApplicationInsightsAgent_EXTENSION_VERSION" = "~3"
    "ASPNETCORE_ENVIRONMENT"                     = "Staging"
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

  tags = azurerm_resource_group.main.tags
}

# Auto-healing rules
resource "azurerm_windows_web_app" "auto_heal" {
  count               = 0 # Set to 1 to enable auto-healing
  name                = azurerm_windows_web_app.main.name
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  service_plan_id     = azurerm_service_plan.main.id

  site_config {
    auto_heal_enabled = true

    auto_heal_setting {
      trigger {
        status_code {
          count             = 5
          interval          = "00:01:00"
          status_code_range = "500-599"
        }
      }

      action {
        action_type                    = "Recycle"
        minimum_process_execution_time = "00:01:00"
      }
    }
  }
}
