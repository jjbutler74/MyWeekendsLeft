variable "environment" {
  description = "Name of the environment"
  default     = "uat"
}

variable "resource_group_name" {
  description = "Name of the existing resource group"
  default     = "uat"
}

variable "app_service_plan_name" {
  description = "Name of the existing App Service Plan"
  default     = "uat-myweekendsleft-api-plan"
}

variable "app_service_name" {
  description = "Name of the existing App Service"
  default     = "uat-myweekendsleft-api"
}

variable "location" {
  description = "Azure location (inferred from existing resources)"
  default     = "australiasoutheast"
}
