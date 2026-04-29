variable "location" {
  description = "Azure region for all resources."
  type        = string
  default     = "eastus"
}

variable "resource_group_name" {
  description = "Resource group name for the application resources."
  type        = string
  default     = "lol-demo1-rg"
}

variable "app_service_plan_name" {
  description = "App Service plan name."
  type        = string
  default     = "lol-demo1-asp"
}

variable "web_app_name" {
  description = "Globally unique Azure Web App name."
  type        = string
  default     = "kishoreismac-lol-demo1-webapp"
}

variable "log_analytics_workspace_name" {
  description = "Log Analytics workspace name."
  type        = string
  default     = "lol-demo1-law"
}

variable "application_insights_name" {
  description = "Application Insights name."
  type        = string
  default     = "lol-demo1-ai"
}

variable "diagnostic_setting_name" {
  description = "Diagnostic setting name for the web app."
  type        = string
  default     = "lol-demo1-webapp-diag"
}

variable "app_service_sku_name" {
  description = "App Service plan SKU name."
  type        = string
  default     = "B1"
}

variable "tags" {
  description = "Tags applied to all Azure resources."
  type        = map(string)
  default = {
    application = "LoL-Demo1"
    environment = "prod"
    managed_by  = "terraform"
  }
}