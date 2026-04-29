output "resource_group_name" {
  description = "Azure resource group name."
  value       = azurerm_resource_group.app.name
}

output "web_app_name" {
  description = "Azure Web App name."
  value       = azurerm_linux_web_app.app.name
}

output "web_app_url" {
  description = "Azure Web App default hostname."
  value       = "https://${azurerm_linux_web_app.app.default_hostname}"
}