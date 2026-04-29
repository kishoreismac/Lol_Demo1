# Workflow Run Guide

## Azure OIDC Federated Credential Setup

Create a Microsoft Entra application or user-assigned managed identity for GitHub Actions and add a federated credential that trusts this repository on the `main` branch.

Use these values as GitHub repository secrets:

- `AZURE_CLIENT_ID`
- `AZURE_TENANT_ID`
- `AZURE_SUBSCRIPTION_ID`

The Azure principal used by GitHub Actions should have permission to:

- create and update the target resource group and App Service resources
- create the Terraform backend resource group, storage account, and blob container
- list storage account keys for the backend storage account

## Backend Storage Setup for Terraform State

Terraform uses the `azurerm` remote backend in Azure Storage. The backend is bootstrapped in `.github/workflows/infra.yml` before `terraform init` runs.

The workflow creates or reuses:

- backend resource group
- backend storage account
- backend blob container
- backend state file key

Default backend values:

- `TFSTATE_RESOURCE_GROUP=lol-demo1-tfstate-rg`
- `TFSTATE_STORAGE_ACCOUNT=loldemo1tfstate`
- `TFSTATE_CONTAINER_NAME=tfstate`
- `TFSTATE_KEY=lol-demo1-prod.tfstate`

If you need different stable names, define GitHub repository variables with the same names. Do not change these values between runs unless you intentionally want a different state location.

The state file persists across runs in Azure Storage, so Terraform tracks existing resources instead of recreating them.

## Required GitHub Secrets

Add these repository secrets:

- `AZURE_CLIENT_ID`
- `AZURE_TENANT_ID`
- `AZURE_SUBSCRIPTION_ID`

## Optional GitHub Variables

You can override these stable defaults with repository variables:

- `AZURE_LOCATION`
- `TF_RESOURCE_GROUP_NAME`
- `TF_APP_SERVICE_PLAN_NAME`
- `TF_WEB_APP_NAME`
- `TF_LOG_ANALYTICS_WORKSPACE_NAME`
- `TF_APPLICATION_INSIGHTS_NAME`
- `TF_DIAGNOSTIC_SETTING_NAME`
- `TF_APP_SERVICE_SKU_NAME`
- `TFSTATE_RESOURCE_GROUP`
- `TFSTATE_STORAGE_ACCOUNT`
- `TFSTATE_CONTAINER_NAME`
- `TFSTATE_KEY`

## Workflow Execution Flow

Execution order:

1. `infra.yml`
2. `app.yml`

### Infrastructure workflow

Trigger:

- push to `main`

Steps:

1. Azure login with OIDC using `azure/login@v2`
2. Bootstrap the remote backend storage resources
3. Run `terraform init`
4. Run `terraform validate`
5. Run `terraform plan`
6. Run `terraform apply -auto-approve`
7. Write `deployment-outputs.env`
8. Upload the `deployment-outputs` artifact

The outputs artifact contains:

- `RESOURCE_GROUP_NAME`
- `WEB_APP_NAME`

### Application workflow

Triggers:

- push to `main`
- `workflow_run` after successful `Infrastructure`

The `workflow_run` path is the primary `infra -> app` deployment flow. On direct `push`, the workflow downloads the latest successful `deployment-outputs` artifact from `infra.yml` on `main`.

Steps:

1. Download the `deployment-outputs` artifact
2. Verify `deployment-outputs.env` exists
3. Load `RESOURCE_GROUP_NAME` and `WEB_APP_NAME`
4. Fail immediately if either value is empty
5. Azure login with OIDC
6. Run `az account show`
7. Run `dotnet restore`
8. Run `dotnet build --configuration Release`
9. Run `dotnet test`
10. Run `dotnet publish --configuration Release --output publish`
11. Ensure `zip` is installed
12. Create `app.zip`
13. Set `WEBSITE_RUN_FROM_PACKAGE=1`
14. Set `SCM_DO_BUILD_DURING_DEPLOYMENT=false`
15. Deploy with `az webapp deploy --type zip`
16. Restart the app
17. Print the application URLs

## How State Prevents Resource Recreation

Terraform remote state stores the binding between Terraform resources and the Azure resources that already exist.

Because `infra.yml` always initializes Terraform with the same backend resource group, storage account, container, and state key:

- the same state file is reused across runs
- Terraform sees the existing resource IDs already under management
- `terraform plan` calculates incremental updates instead of creating duplicates
- stable resource names remain attached to the same managed resources

If the state file is deleted or the backend key is changed, Terraform will no longer have the same tracked state and may attempt to create resources again.

## How to Verify

Verify the deployed app:

- `https://<WEB_APP_NAME>.azurewebsites.net`
- `https://<WEB_APP_NAME>.azurewebsites.net/swagger`

Verify monitoring:

1. Open the Log Analytics workspace created by Terraform.
2. Run queries such as:

```kusto
AppRequests
| where TimeGenerated > ago(1h)
| take 20
```

```kusto
AppExceptions
| where TimeGenerated > ago(1h)
| take 20
```

```kusto
AppTraces
| where TimeGenerated > ago(1h)
| take 20
```

3. In the Web App resource, confirm diagnostic settings are sending logs to the Log Analytics workspace.

## Troubleshooting Common Failures

### Azure login fails

- Verify `AZURE_CLIENT_ID`, `AZURE_TENANT_ID`, and `AZURE_SUBSCRIPTION_ID`
- Verify the federated credential matches this repository and the `main` branch
- Verify the Azure principal has access to the subscription or resource group used by the workflows

### Terraform backend bootstrap fails

- Verify `TFSTATE_STORAGE_ACCOUNT` is globally unique if you override it
- Verify the Azure principal can create storage resources and list storage account keys
- Verify the resource group location and name are valid

### Terraform init uses the wrong state

- Verify `TFSTATE_RESOURCE_GROUP`, `TFSTATE_STORAGE_ACCOUNT`, `TFSTATE_CONTAINER_NAME`, and `TFSTATE_KEY` are unchanged
- Verify the backend blob still exists in Azure Storage

### Terraform tries to recreate resources

- Verify the backend still points to the same state file
- Verify resource names in GitHub variables were not changed unexpectedly
- Verify the Azure resources were not manually deleted outside Terraform

### deployment-outputs.env is missing

- Verify `infra.yml` completed successfully
- Verify the `deployment-outputs` artifact upload step succeeded
- Verify `app.yml` downloaded the artifact into `./.deploy`

### App workflow fails on empty values

- Open `./.deploy/deployment-outputs.env`
- Verify it contains both `RESOURCE_GROUP_NAME` and `WEB_APP_NAME`

### ZIP deploy fails

- Verify `app.zip` exists before the deploy step runs
- Verify the App Service exists in the resource group from `deployment-outputs.env`
- Verify the Azure principal can update web app settings and deploy packages

### App starts but returns errors

- Review Application Insights and Log Analytics traces for startup failures
- Verify the app was published successfully before zipping
- Verify the app restart step completed

### Swagger URL does not load

- Verify Swagger is enabled for the deployed environment
- Verify the app is healthy at the root URL first