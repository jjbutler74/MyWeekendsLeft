variable "environment" {
  description = "Name of the environment"
  default     = "dev"
}

variable "resource_group_name" {
  description = "Name of the existing resource group"
  default     = "dev"
}

variable "app_service_plan_name" {
  description = "Name of the existing App Service Plan"
  default     = "dev-myweekendsleft-api-plan"
}

variable "app_service_name" {
  description = "Name of the existing App Service"
  default     = "dev-myweekendsleft-api"
}

variable "location" {
  description = "Azure location (inferred from existing resources)"
  default     = "australiasoutheast"
}
