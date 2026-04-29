resource "azurerm_resource_group" "app" {
  name     = var.resource_group_name
  location = var.location
  tags     = var.tags
}

resource "azurerm_log_analytics_workspace" "app" {
  name                = var.log_analytics_workspace_name
  location            = azurerm_resource_group.app.location
  resource_group_name = azurerm_resource_group.app.name
  sku                 = "PerGB2018"
  retention_in_days   = 30
  tags                = var.tags
}

resource "azurerm_application_insights" "app" {
  name                = var.application_insights_name
  location            = azurerm_resource_group.app.location
  resource_group_name = azurerm_resource_group.app.name
  workspace_id        = azurerm_log_analytics_workspace.app.id
  application_type    = "web"
  tags                = var.tags
}

resource "azurerm_service_plan" "app" {
  name                = var.app_service_plan_name
  location            = azurerm_resource_group.app.location
  resource_group_name = azurerm_resource_group.app.name
  os_type             = "Linux"
  sku_name            = var.app_service_sku_name
  tags                = var.tags
}

resource "azurerm_linux_web_app" "app" {
  name                = var.web_app_name
  location            = azurerm_resource_group.app.location
  resource_group_name = azurerm_resource_group.app.name
  service_plan_id     = azurerm_service_plan.app.id
  https_only          = true
  tags                = var.tags

  site_config {
    always_on = false

    application_stack {
      dotnet_version = "8.0"
    }
  }

  app_settings = {
    APPLICATIONINSIGHTS_CONNECTION_STRING = azurerm_application_insights.app.connection_string
    APPINSIGHTS_INSTRUMENTATIONKEY        = azurerm_application_insights.app.instrumentation_key
    ASPNETCORE_ENVIRONMENT                = "Production"
    SCM_DO_BUILD_DURING_DEPLOYMENT        = "false"
    WEBSITE_RUN_FROM_PACKAGE              = "1"
  }
}

data "azurerm_monitor_diagnostic_categories" "web_app" {
  resource_id = azurerm_linux_web_app.app.id
}

resource "azurerm_monitor_diagnostic_setting" "web_app" {
  name                       = var.diagnostic_setting_name
  target_resource_id         = azurerm_linux_web_app.app.id
  log_analytics_workspace_id = azurerm_log_analytics_workspace.app.id

  dynamic "enabled_log" {
    for_each = data.azurerm_monitor_diagnostic_categories.web_app.log_category_types

    content {
      category = enabled_log.value
    }
  }

  dynamic "metric" {
    for_each = data.azurerm_monitor_diagnostic_categories.web_app.metrics

    content {
      category = metric.value
      enabled  = true
    }
  }
}