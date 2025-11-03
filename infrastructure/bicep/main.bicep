// =============================================================================
// Semantic Kernel Agentic Framework - Main Infrastructure Template
// =============================================================================
// 主 Bicep 模板 - 部署所有 Azure 資源

targetScope = 'subscription'

// =============================================================================
// Parameters
// =============================================================================

@description('環境名稱 (dev, staging, prod)')
@allowed([
  'dev'
  'staging'
  'prod'
])
param environment string = 'dev'

@description('Azure 部署區域')
param location string = 'eastus'

@description('項目名稱前綴')
param projectName string = 'skagentic'

@description('資源標籤')
param tags object = {
  Environment: environment
  Project: 'Semantic Kernel Agentic Framework'
  ManagedBy: 'Bicep'
  CostCenter: 'Engineering'
}

@description('PostgreSQL 管理員用戶名')
@secure()
param postgresAdminUsername string

@description('PostgreSQL 管理員密碼')
@secure()
param postgresAdminPassword string

@description('部署 Azure OpenAI 服務')
param deployOpenAI bool = true

@description('部署私有端點')
param deployPrivateEndpoints bool = false

@description('Azure OpenAI 部署配置')
param openAIDeployments array = [
  {
    name: 'gpt-4o'
    model: {
      format: 'OpenAI'
      name: 'gpt-4o'
      version: '2024-05-13'
    }
    sku: {
      name: 'Standard'
      capacity: 10
    }
  }
  {
    name: 'text-embedding-3-large'
    model: {
      format: 'OpenAI'
      name: 'text-embedding-3-large'
      version: '1'
    }
    sku: {
      name: 'Standard'
      capacity: 10
    }
  }
]

// =============================================================================
// Variables
// =============================================================================

var resourceGroupName = 'rg-${projectName}-${environment}'
var uniqueSuffix = uniqueString(subscription().subscriptionId, resourceGroupName)

// =============================================================================
// Resource Group
// =============================================================================

resource resourceGroup 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: resourceGroupName
  location: location
  tags: tags
}

// =============================================================================
// Networking Module
// =============================================================================

module networking 'modules/networking.bicep' = {
  name: 'networking-deployment'
  scope: resourceGroup
  params: {
    location: location
    projectName: projectName
    environment: environment
    tags: tags
  }
}

// =============================================================================
// PostgreSQL Flexible Server Module
// =============================================================================

module postgresql 'modules/postgresql.bicep' = {
  name: 'postgresql-deployment'
  scope: resourceGroup
  params: {
    location: location
    projectName: projectName
    environment: environment
    tags: tags
    administratorLogin: postgresAdminUsername
    administratorPassword: postgresAdminPassword
    subnetId: deployPrivateEndpoints ? networking.outputs.databaseSubnetId : ''
    delegatedSubnetResourceId: deployPrivateEndpoints ? networking.outputs.databaseSubnetId : ''
    privateDnsZoneArmResourceId: deployPrivateEndpoints ? networking.outputs.postgresDnsZoneId : ''
  }
}

// =============================================================================
// Redis Cache Module
// =============================================================================

module redis 'modules/redis.bicep' = {
  name: 'redis-deployment'
  scope: resourceGroup
  params: {
    location: location
    projectName: projectName
    environment: environment
    tags: tags
    subnetId: deployPrivateEndpoints ? networking.outputs.redisSubnetId : ''
  }
}

// =============================================================================
// Container Registry Module
// =============================================================================

module acr 'modules/acr.bicep' = {
  name: 'acr-deployment'
  scope: resourceGroup
  params: {
    location: location
    projectName: projectName
    environment: environment
    tags: tags
    acrSku: environment == 'prod' ? 'Premium' : 'Standard'
  }
}

// =============================================================================
// Key Vault Module
// =============================================================================

module keyVault 'modules/keyvault.bicep' = {
  name: 'keyvault-deployment'
  scope: resourceGroup
  params: {
    location: location
    projectName: projectName
    environment: environment
    tags: tags
    subnetId: deployPrivateEndpoints ? networking.outputs.privateEndpointSubnetId : ''
  }
}

// =============================================================================
// Azure OpenAI Module (Optional)
// =============================================================================

module openai 'modules/openai.bicep' = if (deployOpenAI) {
  name: 'openai-deployment'
  scope: resourceGroup
  params: {
    location: location
    projectName: projectName
    environment: environment
    tags: tags
    deployments: openAIDeployments
  }
}

// =============================================================================
// Storage Account Module
// =============================================================================

module storage 'modules/storage.bicep' = {
  name: 'storage-deployment'
  scope: resourceGroup
  params: {
    location: location
    projectName: projectName
    environment: environment
    tags: tags
    uniqueSuffix: uniqueSuffix
  }
}

// =============================================================================
// Outputs
// =============================================================================

output resourceGroupName string = resourceGroup.name
output vnetId string = networking.outputs.vnetId
output postgresqlServerName string = postgresql.outputs.serverName
output postgresqlConnectionString string = postgresql.outputs.connectionString
output redisHostName string = redis.outputs.hostName
output redisPrimaryKey string = redis.outputs.primaryKey
output acrLoginServer string = acr.outputs.loginServer
output keyVaultUri string = keyVault.outputs.vaultUri
output openAIEndpoint string = deployOpenAI ? openai.outputs.endpoint : ''
output openAIPrimaryKey string = deployOpenAI ? openai.outputs.primaryKey : ''
output storageAccountName string = storage.outputs.storageAccountName
