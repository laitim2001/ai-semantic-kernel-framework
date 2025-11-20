#!/bin/bash
# =============================================================================
# IPA Platform - Deploy Staging Environment
# =============================================================================
# Description: Deploy staging environment to Azure
# Usage: ./deploy-staging.sh
# Prerequisites: Azure CLI installed and logged in (az login)
# =============================================================================

set -e  # Exit on error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# =============================================================================
# CONFIGURATION
# =============================================================================

ENVIRONMENT="staging"
LOCATION="eastus"
PROJECT_NAME="ipa"

# Resource names
RG_NAME="rg-${PROJECT_NAME}-${ENVIRONMENT}-${LOCATION}"
ASP_NAME="asp-${PROJECT_NAME}-${ENVIRONMENT}-${LOCATION}"
BACKEND_APP_NAME="app-${PROJECT_NAME}-backend-${ENVIRONMENT}"
FRONTEND_APP_NAME="app-${PROJECT_NAME}-frontend-${ENVIRONMENT}"
POSTGRES_SERVER_NAME="psql-${PROJECT_NAME}-${ENVIRONMENT}-${LOCATION}"
REDIS_NAME="redis-${PROJECT_NAME}-shared-${LOCATION}"
SERVICEBUS_NAMESPACE="sb-${PROJECT_NAME}-${ENVIRONMENT}-${LOCATION}"
KEYVAULT_NAME="kv-${PROJECT_NAME}-$(openssl rand -hex 4)"
STORAGE_ACCOUNT_NAME="stg${PROJECT_NAME}${ENVIRONMENT}$(openssl rand -hex 3)"
APPINSIGHTS_NAME="appi-${PROJECT_NAME}-${ENVIRONMENT}-${LOCATION}"
LOG_ANALYTICS_NAME="log-${PROJECT_NAME}-${ENVIRONMENT}-${LOCATION}"

# SKUs
APP_SERVICE_SKU="B1"
POSTGRES_SKU="Standard_B1ms"
POSTGRES_TIER="Burstable"
POSTGRES_STORAGE_SIZE="32"
POSTGRES_BACKUP_RETENTION="7"
REDIS_SKU="Standard"
REDIS_FAMILY="C"
REDIS_CAPACITY="1"

# Database credentials (will prompt if not set)
POSTGRES_ADMIN_USER="${POSTGRES_ADMIN_USER:-ipaadmin}"
POSTGRES_ADMIN_PASSWORD="${POSTGRES_ADMIN_PASSWORD}"

# =============================================================================
# FUNCTIONS
# =============================================================================

log_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

log_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

log_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

check_prerequisites() {
    log_info "Checking prerequisites..."

    # Check if Azure CLI is installed
    if ! command -v az &> /dev/null; then
        log_error "Azure CLI is not installed. Please install it first."
        exit 1
    fi

    # Check if logged in
    if ! az account show &> /dev/null; then
        log_error "Not logged in to Azure. Please run 'az login' first."
        exit 1
    fi

    log_success "Prerequisites check passed"
}

prompt_postgres_password() {
    if [ -z "$POSTGRES_ADMIN_PASSWORD" ]; then
        log_warning "PostgreSQL admin password not set."
        read -s -p "Enter PostgreSQL admin password: " POSTGRES_ADMIN_PASSWORD
        echo
        read -s -p "Confirm password: " POSTGRES_ADMIN_PASSWORD_CONFIRM
        echo

        if [ "$POSTGRES_ADMIN_PASSWORD" != "$POSTGRES_ADMIN_PASSWORD_CONFIRM" ]; then
            log_error "Passwords do not match"
            exit 1
        fi

        # Validate password complexity
        if [ ${#POSTGRES_ADMIN_PASSWORD} -lt 8 ]; then
            log_error "Password must be at least 8 characters long"
            exit 1
        fi
    fi
}

create_resource_group() {
    log_info "Creating resource group: $RG_NAME"

    if az group show --name "$RG_NAME" &> /dev/null; then
        log_warning "Resource group already exists"
    else
        az group create \
            --name "$RG_NAME" \
            --location "$LOCATION" \
            --tags \
                Environment="$ENVIRONMENT" \
                Project="ipa-platform" \
                ManagedBy="azure-cli" \
                CostCenter="engineering"

        log_success "Resource group created"
    fi
}

create_log_analytics() {
    log_info "Creating Log Analytics workspace: $LOG_ANALYTICS_NAME"

    az monitor log-analytics workspace create \
        --resource-group "$RG_NAME" \
        --workspace-name "$LOG_ANALYTICS_NAME" \
        --location "$LOCATION" \
        --retention-time 30

    log_success "Log Analytics workspace created"
}

create_app_insights() {
    log_info "Creating Application Insights: $APPINSIGHTS_NAME"

    WORKSPACE_ID=$(az monitor log-analytics workspace show \
        --resource-group "$RG_NAME" \
        --workspace-name "$LOG_ANALYTICS_NAME" \
        --query id -o tsv)

    az monitor app-insights component create \
        --app "$APPINSIGHTS_NAME" \
        --location "$LOCATION" \
        --resource-group "$RG_NAME" \
        --workspace "$WORKSPACE_ID" \
        --application-type web

    log_success "Application Insights created"
}

create_app_service_plan() {
    log_info "Creating App Service Plan: $ASP_NAME"

    az appservice plan create \
        --name "$ASP_NAME" \
        --resource-group "$RG_NAME" \
        --location "$LOCATION" \
        --is-linux \
        --sku "$APP_SERVICE_SKU"

    log_success "App Service Plan created"
}

create_backend_app() {
    log_info "Creating Backend App Service: $BACKEND_APP_NAME"

    az webapp create \
        --name "$BACKEND_APP_NAME" \
        --resource-group "$RG_NAME" \
        --plan "$ASP_NAME" \
        --runtime "PYTHON:3.11"

    # Get Application Insights connection string
    APPINSIGHTS_CONNECTION_STRING=$(az monitor app-insights component show \
        --app "$APPINSIGHTS_NAME" \
        --resource-group "$RG_NAME" \
        --query connectionString -o tsv)

    # Configure app settings
    az webapp config appsettings set \
        --name "$BACKEND_APP_NAME" \
        --resource-group "$RG_NAME" \
        --settings \
            ENVIRONMENT="$ENVIRONMENT" \
            APPLICATIONINSIGHTS_CONNECTION_STRING="$APPINSIGHTS_CONNECTION_STRING" \
            SCM_DO_BUILD_DURING_DEPLOYMENT=true \
            ENABLE_ORYX_BUILD=true

    # Configure startup command
    az webapp config set \
        --name "$BACKEND_APP_NAME" \
        --resource-group "$RG_NAME" \
        --startup-file "gunicorn -w 2 -k uvicorn.workers.UvicornWorker main:app"

    # Enable HTTPS only
    az webapp update \
        --name "$BACKEND_APP_NAME" \
        --resource-group "$RG_NAME" \
        --https-only true

    log_success "Backend App Service created"
}

create_frontend_app() {
    log_info "Creating Frontend App Service: $FRONTEND_APP_NAME"

    az webapp create \
        --name "$FRONTEND_APP_NAME" \
        --resource-group "$RG_NAME" \
        --plan "$ASP_NAME" \
        --runtime "NODE:20-lts"

    # Get Application Insights connection string
    APPINSIGHTS_KEY=$(az monitor app-insights component show \
        --app "$APPINSIGHTS_NAME" \
        --resource-group "$RG_NAME" \
        --query instrumentationKey -o tsv)

    # Configure app settings
    az webapp config appsettings set \
        --name "$FRONTEND_APP_NAME" \
        --resource-group "$RG_NAME" \
        --settings \
            ENVIRONMENT="$ENVIRONMENT" \
            NEXT_PUBLIC_API_URL="https://${BACKEND_APP_NAME}.azurewebsites.net" \
            NEXT_PUBLIC_APP_INSIGHTS_KEY="$APPINSIGHTS_KEY"

    # Enable HTTPS only
    az webapp update \
        --name "$FRONTEND_APP_NAME" \
        --resource-group "$RG_NAME" \
        --https-only true

    log_success "Frontend App Service created"
}

create_postgresql() {
    log_info "Creating PostgreSQL Flexible Server: $POSTGRES_SERVER_NAME"

    az postgres flexible-server create \
        --name "$POSTGRES_SERVER_NAME" \
        --resource-group "$RG_NAME" \
        --location "$LOCATION" \
        --admin-user "$POSTGRES_ADMIN_USER" \
        --admin-password "$POSTGRES_ADMIN_PASSWORD" \
        --sku-name "$POSTGRES_SKU" \
        --tier "$POSTGRES_TIER" \
        --storage-size "$POSTGRES_STORAGE_SIZE" \
        --version 16 \
        --public-access 0.0.0.0-255.255.255.255 \
        --backup-retention "$POSTGRES_BACKUP_RETENTION"

    # Create database
    az postgres flexible-server db create \
        --resource-group "$RG_NAME" \
        --server-name "$POSTGRES_SERVER_NAME" \
        --database-name "ipa_platform_staging"

    log_success "PostgreSQL server created"
}

create_redis() {
    log_info "Creating Azure Cache for Redis: $REDIS_NAME"

    # Check if already exists (might be shared)
    if az redis show --name "$REDIS_NAME" --resource-group "$RG_NAME" &> /dev/null; then
        log_warning "Redis cache already exists (shared resource)"
    else
        az redis create \
            --name "$REDIS_NAME" \
            --resource-group "$RG_NAME" \
            --location "$LOCATION" \
            --sku "$REDIS_SKU" \
            --vm-size "$REDIS_FAMILY$REDIS_CAPACITY" \
            --enable-non-ssl-port false \
            --minimum-tls-version 1.2

        log_success "Redis cache created"
    fi
}

create_service_bus() {
    log_info "Creating Service Bus namespace: $SERVICEBUS_NAMESPACE"

    az servicebus namespace create \
        --name "$SERVICEBUS_NAMESPACE" \
        --resource-group "$RG_NAME" \
        --location "$LOCATION" \
        --sku Standard

    # Create queues
    for queue in "workflow-execution-queue" "agent-task-queue" "notification-queue"; do
        log_info "Creating queue: $queue"
        az servicebus queue create \
            --name "$queue" \
            --namespace-name "$SERVICEBUS_NAMESPACE" \
            --resource-group "$RG_NAME" \
            --default-message-time-to-live P14D \
            --enable-dead-lettering-on-message-expiration true \
            --duplicate-detection-history-time-window PT10M
    done

    log_success "Service Bus namespace and queues created"
}

create_key_vault() {
    log_info "Creating Key Vault: $KEYVAULT_NAME"

    az keyvault create \
        --name "$KEYVAULT_NAME" \
        --resource-group "$RG_NAME" \
        --location "$LOCATION" \
        --enable-soft-delete true \
        --soft-delete-retention-days 90 \
        --enable-purge-protection false \
        --enabled-for-template-deployment true

    log_success "Key Vault created"
}

create_storage_account() {
    log_info "Creating Storage Account: $STORAGE_ACCOUNT_NAME"

    az storage account create \
        --name "$STORAGE_ACCOUNT_NAME" \
        --resource-group "$RG_NAME" \
        --location "$LOCATION" \
        --sku Standard_LRS \
        --kind StorageV2 \
        --https-only true \
        --min-tls-version TLS1_2 \
        --allow-blob-public-access false

    # Get storage account key
    STORAGE_KEY=$(az storage account keys list \
        --account-name "$STORAGE_ACCOUNT_NAME" \
        --resource-group "$RG_NAME" \
        --query '[0].value' -o tsv)

    # Create containers
    for container in "${ENVIRONMENT}-uploads" "${ENVIRONMENT}-logs" "backups"; do
        log_info "Creating container: $container"
        az storage container create \
            --name "$container" \
            --account-name "$STORAGE_ACCOUNT_NAME" \
            --account-key "$STORAGE_KEY"
    done

    log_success "Storage Account created"
}

store_secrets_in_keyvault() {
    log_info "Storing secrets in Key Vault..."

    # Get connection strings
    POSTGRES_CONNECTION_STRING="postgresql://${POSTGRES_ADMIN_USER}:${POSTGRES_ADMIN_PASSWORD}@${POSTGRES_SERVER_NAME}.postgres.database.azure.com/ipa_platform_staging?sslmode=require"

    REDIS_KEY=$(az redis list-keys \
        --name "$REDIS_NAME" \
        --resource-group "$RG_NAME" \
        --query primaryKey -o tsv)
    REDIS_CONNECTION_STRING="${REDIS_NAME}.redis.cache.windows.net:6380,password=${REDIS_KEY},ssl=True,abortConnect=False,db=1"

    SERVICEBUS_CONNECTION_STRING=$(az servicebus namespace authorization-rule keys list \
        --resource-group "$RG_NAME" \
        --namespace-name "$SERVICEBUS_NAMESPACE" \
        --name RootManageSharedAccessKey \
        --query primaryConnectionString -o tsv)

    APPINSIGHTS_CONNECTION_STRING=$(az monitor app-insights component show \
        --app "$APPINSIGHTS_NAME" \
        --resource-group "$RG_NAME" \
        --query connectionString -o tsv)

    # Store secrets
    az keyvault secret set --vault-name "$KEYVAULT_NAME" \
        --name "staging-database-connection-string" \
        --value "$POSTGRES_CONNECTION_STRING"

    az keyvault secret set --vault-name "$KEYVAULT_NAME" \
        --name "staging-redis-connection-string" \
        --value "$REDIS_CONNECTION_STRING"

    az keyvault secret set --vault-name "$KEYVAULT_NAME" \
        --name "staging-servicebus-connection-string" \
        --value "$SERVICEBUS_CONNECTION_STRING"

    az keyvault secret set --vault-name "$KEYVAULT_NAME" \
        --name "staging-appinsights-connection-string" \
        --value "$APPINSIGHTS_CONNECTION_STRING"

    # Generate JWT secret
    JWT_SECRET=$(openssl rand -base64 32)
    az keyvault secret set --vault-name "$KEYVAULT_NAME" \
        --name "staging-jwt-secret-key" \
        --value "$JWT_SECRET"

    log_success "Secrets stored in Key Vault"
}

configure_app_service_keyvault_access() {
    log_info "Configuring App Service access to Key Vault..."

    # Enable system-assigned managed identity for backend app
    BACKEND_PRINCIPAL_ID=$(az webapp identity assign \
        --name "$BACKEND_APP_NAME" \
        --resource-group "$RG_NAME" \
        --query principalId -o tsv)

    # Grant access to Key Vault
    az keyvault set-policy \
        --name "$KEYVAULT_NAME" \
        --object-id "$BACKEND_PRINCIPAL_ID" \
        --secret-permissions get list

    log_success "App Service access configured"
}

print_summary() {
    log_success "==================================================================="
    log_success "Deployment Complete!"
    log_success "==================================================================="
    echo ""
    echo "Resource Group: $RG_NAME"
    echo "Location: $LOCATION"
    echo ""
    echo "App Services:"
    echo "  Backend:  https://${BACKEND_APP_NAME}.azurewebsites.net"
    echo "  Frontend: https://${FRONTEND_APP_NAME}.azurewebsites.net"
    echo ""
    echo "Database:"
    echo "  PostgreSQL: ${POSTGRES_SERVER_NAME}.postgres.database.azure.com"
    echo "  Database: ipa_platform_staging"
    echo ""
    echo "Key Vault: $KEYVAULT_NAME"
    echo ""
    echo "Next Steps:"
    echo "  1. Configure GitHub Actions secrets (see deployment-guide.md)"
    echo "  2. Push code to trigger deployment"
    echo "  3. Run database migrations"
    echo "  4. Verify health endpoints"
    echo ""
}

# =============================================================================
# MAIN EXECUTION
# =============================================================================

main() {
    log_info "Starting deployment of $ENVIRONMENT environment"
    echo ""

    check_prerequisites
    prompt_postgres_password

    create_resource_group
    create_log_analytics
    create_app_insights
    create_app_service_plan
    create_backend_app
    create_frontend_app
    create_postgresql
    create_redis
    create_service_bus
    create_key_vault
    create_storage_account
    store_secrets_in_keyvault
    configure_app_service_keyvault_access

    print_summary
}

# Run main function
main
