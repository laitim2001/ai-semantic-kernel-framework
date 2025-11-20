// =============================================================================
// IPA Platform - Azure Infrastructure as Code (Bicep)
// =============================================================================
// Description: Main Bicep template for deploying IPA Platform infrastructure
// Version: 1.0.0
// Date: 2025-11-20
// =============================================================================

targetScope = 'subscription'

// =============================================================================
// PARAMETERS
// =============================================================================

@description('Environment name (staging or production)')
@allowed([
  'staging'
  'production'
])
param environment string

@description('Azure region for resources')
@allowed([
  'eastus'
  'westus2'
  'westeurope'
])
param location string = 'eastus'

@description('Project name prefix')
param projectName string = 'ipa'

@description('Tags for all resources')
param tags object = {
  Project: 'ipa-platform'
  Environment: environment
  ManagedBy: 'bicep'
  CostCenter: 'engineering'
}

@description('PostgreSQL administrator username')
@secure()
param postgresAdminUsername string

@description('PostgreSQL administrator password')
@secure()
param postgresAdminPassword string

@description('Deploy production-grade resources (higher SKUs)')
param deployProductionGrade bool = (environment == 'production')

// =============================================================================
// VARIABLES
// =============================================================================

var resourceGroupName = 'rg-${projectName}-${environment}-${location}'
var appServicePlanName = 'asp-${projectName}-${environment}-${location}'
var backendAppName = 'app-${projectName}-backend-${environment}'
var frontendAppName = 'app-${projectName}-frontend-${environment}'
var postgresServerName = 'psql-${projectName}-${environment}-${location}'
var redisName = 'redis-${projectName}-shared-${location}'
var serviceBusNamespaceName = 'sb-${projectName}-${environment}-${location}'
var keyVaultName = 'kv-${projectName}-${take(uniqueString(subscription().subscriptionId), 8)}'
var storageAccountName = 'stg${projectName}${environment}${take(uniqueString(subscription().subscriptionId), 6)}'
var appInsightsName = 'appi-${projectName}-${environment}-${location}'
var logAnalyticsName = 'log-${projectName}-${environment}-${location}'

// SKU selection based on environment
var appServicePlanSku = deployProductionGrade ? {
  name: 'P1V2'
  tier: 'PremiumV2'
  capacity: 1
} : {
  name: 'B1'
  tier: 'Basic'
  capacity: 1
}

var postgresSku = deployProductionGrade ? {
  name: 'Standard_D2s_v3'
  tier: 'GeneralPurpose'
} : {
  name: 'Standard_B1ms'
  tier: 'Burstable'
}

var postgresStorageSizeGB = deployProductionGrade ? 128 : 32
var postgresBackupRetentionDays = deployProductionGrade ? 35 : 7

// =============================================================================
// RESOURCE GROUP
// =============================================================================

resource resourceGroup 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: resourceGroupName
  location: location
  tags: tags
}

// =============================================================================
// MODULES
// =============================================================================

// Monitoring and Logging
module monitoring './modules/monitoring.bicep' = {
  scope: resourceGroup
  name: 'monitoring-deployment'
  params: {
    location: location
    environment: environment
    logAnalyticsName: logAnalyticsName
    appInsightsName: appInsightsName
    tags: tags
  }
}

// App Service Plan
module appServicePlan './modules/app-service-plan.bicep' = {
  scope: resourceGroup
  name: 'app-service-plan-deployment'
  params: {
    location: location
    appServicePlanName: appServicePlanName
    sku: appServicePlanSku
    tags: tags
  }
}

// Backend App Service
module backendApp './modules/app-service.bicep' = {
  scope: resourceGroup
  name: 'backend-app-deployment'
  params: {
    location: location
    appName: backendAppName
    appServicePlanId: appServicePlan.outputs.appServicePlanId
    runtime: 'PYTHON|3.11'
    appInsightsConnectionString: monitoring.outputs.appInsightsConnectionString
    appInsightsInstrumentationKey: monitoring.outputs.appInsightsInstrumentationKey
    environment: environment
    tags: tags
    alwaysOn: deployProductionGrade
    healthCheckPath: '/health'
  }
}

// Frontend App Service
module frontendApp './modules/app-service.bicep' = {
  scope: resourceGroup
  name: 'frontend-app-deployment'
  params: {
    location: location
    appName: frontendAppName
    appServicePlanId: appServicePlan.outputs.appServicePlanId
    runtime: 'NODE|20-lts'
    appInsightsConnectionString: monitoring.outputs.appInsightsConnectionString
    appInsightsInstrumentationKey: monitoring.outputs.appInsightsInstrumentationKey
    environment: environment
    tags: tags
    alwaysOn: deployProductionGrade
    healthCheckPath: '/'
  }
}

// PostgreSQL Flexible Server
module postgres './modules/postgresql.bicep' = {
  scope: resourceGroup
  name: 'postgresql-deployment'
  params: {
    location: location
    serverName: postgresServerName
    administratorLogin: postgresAdminUsername
    administratorPassword: postgresAdminPassword
    sku: postgresSku
    storageSizeGB: postgresStorageSizeGB
    backupRetentionDays: postgresBackupRetentionDays
    environment: environment
    tags: tags
  }
}

// Azure Cache for Redis
module redis './modules/redis.bicep' = {
  scope: resourceGroup
  name: 'redis-deployment'
  params: {
    location: location
    redisName: redisName
    sku: {
      name: 'Standard'
      family: 'C'
      capacity: 1
    }
    tags: tags
  }
}

// Service Bus Namespace
module serviceBus './modules/service-bus.bicep' = {
  scope: resourceGroup
  name: 'service-bus-deployment'
  params: {
    location: location
    namespaceName: serviceBusNamespaceName
    sku: 'Standard'
    tags: tags
  }
}

// Key Vault
module keyVault './modules/key-vault.bicep' = {
  scope: resourceGroup
  name: 'key-vault-deployment'
  params: {
    location: location
    keyVaultName: keyVaultName
    tenantId: subscription().tenantId
    tags: tags
    enabledForDeployment: false
    enabledForDiskEncryption: false
    enabledForTemplateDeployment: true
    enableSoftDelete: true
    softDeleteRetentionInDays: 90
    enablePurgeProtection: deployProductionGrade
  }
}

// Storage Account
module storage './modules/storage.bicep' = {
  scope: resourceGroup
  name: 'storage-deployment'
  params: {
    location: location
    storageAccountName: storageAccountName
    sku: 'Standard_LRS'
    containers: [
      '${environment}-uploads'
      '${environment}-logs'
      'backups'
    ]
    tags: tags
  }
}

// =============================================================================
// OUTPUTS
// =============================================================================

output resourceGroupName string = resourceGroup.name
output appServicePlanId string = appServicePlan.outputs.appServicePlanId
output backendAppName string = backendApp.outputs.appName
output backendAppUrl string = backendApp.outputs.appUrl
output frontendAppName string = frontendApp.outputs.appName
output frontendAppUrl string = frontendApp.outputs.appUrl
output postgresServerFqdn string = postgres.outputs.serverFqdn
output postgresServerName string = postgres.outputs.serverName
output redisHostname string = redis.outputs.redisHostname
output serviceBusNamespace string = serviceBus.outputs.namespaceName
output keyVaultName string = keyVault.outputs.keyVaultName
output keyVaultUri string = keyVault.outputs.keyVaultUri
output storageAccountName string = storage.outputs.storageAccountName
output appInsightsInstrumentationKey string = monitoring.outputs.appInsightsInstrumentationKey
output appInsightsConnectionString string = monitoring.outputs.appInsightsConnectionString
