// =============================================================================
// Azure Container Registry Module
// =============================================================================

@description('Azure 部署區域')
param location string

@description('項目名稱前綴')
param projectName string

@description('環境名稱')
param environment string

@description('資源標籤')
param tags object

@description('ACR SKU')
@allowed([
  'Basic'
  'Standard'
  'Premium'
])
param acrSku string = 'Standard'

@description('啟用管理員用戶')
param adminUserEnabled bool = environment == 'dev'

// =============================================================================
// Variables
// =============================================================================

// ACR 名稱必須全局唯一且只能包含字母數字字符
var acrName = replace('acr${projectName}${environment}', '-', '')
var uniqueSuffix = uniqueString(resourceGroup().id)
var acrUniqueName = '${acrName}${uniqueSuffix}'

// =============================================================================
// Azure Container Registry
// =============================================================================

resource acr 'Microsoft.ContainerRegistry/registries@2023-07-01' = {
  name: acrUniqueName
  location: location
  tags: tags
  sku: {
    name: acrSku
  }
  properties: {
    adminUserEnabled: adminUserEnabled
    publicNetworkAccess: 'Enabled'
    networkRuleBypassOptions: 'AzureServices'
    policies: {
      quarantinePolicy: {
        status: 'disabled'
      }
      trustPolicy: {
        type: 'Notary'
        status: environment == 'prod' ? 'enabled' : 'disabled'
      }
      retentionPolicy: {
        days: environment == 'prod' ? 30 : 7
        status: 'enabled'
      }
      exportPolicy: {
        status: 'enabled'
      }
    }
    encryption: {
      status: 'disabled'
    }
    dataEndpointEnabled: false
    networkRuleSet: {
      defaultAction: 'Allow'
    }
    zoneRedundancy: environment == 'prod' && acrSku == 'Premium' ? 'Enabled' : 'Disabled'
  }
}

// =============================================================================
// Geo-replication (僅 Premium SKU 且 Production 環境)
// =============================================================================

resource acrReplication 'Microsoft.ContainerRegistry/registries/replications@2023-07-01' = if (environment == 'prod' && acrSku == 'Premium') {
  parent: acr
  name: 'westus'
  location: 'westus'
  tags: tags
  properties: {
    zoneRedundancy: 'Enabled'
  }
}

// =============================================================================
// Webhook for CI/CD
// =============================================================================

resource webhook 'Microsoft.ContainerRegistry/registries/webhooks@2023-07-01' = if (environment != 'dev') {
  parent: acr
  name: 'webhook${environment}'
  location: location
  tags: tags
  properties: {
    status: 'enabled'
    scope: ''
    actions: [
      'push'
      'delete'
    ]
    serviceUri: 'https://placeholder.com/webhook' // 實際使用時替換為 CI/CD 端點
  }
}

// =============================================================================
// Diagnostic Settings
// =============================================================================

resource diagnosticSettings 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = if (environment != 'dev') {
  name: '${acrUniqueName}-diagnostics'
  scope: acr
  properties: {
    metrics: [
      {
        category: 'AllMetrics'
        enabled: true
        retentionPolicy: {
          enabled: true
          days: 30
        }
      }
    ]
    logs: [
      {
        categoryGroup: 'allLogs'
        enabled: true
        retentionPolicy: {
          enabled: true
          days: 30
        }
      }
      {
        categoryGroup: 'audit'
        enabled: true
        retentionPolicy: {
          enabled: true
          days: 90
        }
      }
    ]
  }
}

// =============================================================================
// Outputs
// =============================================================================

output acrId string = acr.id
output acrName string = acr.name
output loginServer string = acr.properties.loginServer
output adminUsername string = adminUserEnabled ? acr.listCredentials().username : ''
output adminPassword string = adminUserEnabled ? acr.listCredentials().passwords[0].value : ''
