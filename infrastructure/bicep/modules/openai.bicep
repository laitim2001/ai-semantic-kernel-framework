// =============================================================================
// Azure OpenAI Service Module
// =============================================================================

@description('Azure 部署區域')
param location string

@description('項目名稱前綴')
param projectName string

@description('環境名稱')
param environment string

@description('資源標籤')
param tags object

@description('OpenAI SKU')
param skuName string = 'S0'

@description('模型部署配置')
param deployments array = [
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

@description('啟用公共網絡訪問')
param publicNetworkAccess bool = environment == 'dev'

// =============================================================================
// Variables
// =============================================================================

var openAIName = 'openai-${projectName}-${environment}'

// =============================================================================
// Azure OpenAI Service
// =============================================================================

resource openai 'Microsoft.CognitiveServices/accounts@2023-10-01-preview' = {
  name: openAIName
  location: location
  tags: tags
  kind: 'OpenAI'
  sku: {
    name: skuName
  }
  properties: {
    customSubDomainName: openAIName
    publicNetworkAccess: publicNetworkAccess ? 'Enabled' : 'Disabled'
    networkAcls: {
      defaultAction: publicNetworkAccess ? 'Allow' : 'Deny'
      bypass: 'AzureServices'
      ipRules: []
      virtualNetworkRules: []
    }
    disableLocalAuth: false
    restore: false
  }
}

// =============================================================================
// Model Deployments
// =============================================================================

@batchSize(1)
resource deployment 'Microsoft.CognitiveServices/accounts/deployments@2023-10-01-preview' = [for item in deployments: {
  parent: openai
  name: item.name
  sku: item.sku
  properties: {
    model: item.model
    raiPolicyName: contains(item, 'raiPolicyName') ? item.raiPolicyName : null
  }
}]

// =============================================================================
// Diagnostic Settings
// =============================================================================

resource diagnosticSettings 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = if (environment != 'dev') {
  name: '${openAIName}-diagnostics'
  scope: openai
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
        category: 'Audit'
        enabled: true
        retentionPolicy: {
          enabled: true
          days: 90
        }
      }
      {
        category: 'RequestResponse'
        enabled: true
        retentionPolicy: {
          enabled: true
          days: 30
        }
      }
      {
        category: 'Trace'
        enabled: true
        retentionPolicy: {
          enabled: true
          days: 30
        }
      }
    ]
  }
}

// =============================================================================
// Outputs
// =============================================================================

output openaiId string = openai.id
output openaiName string = openai.name
output endpoint string = openai.properties.endpoint
output primaryKey string = openai.listKeys().key1
output secondaryKey string = openai.listKeys().key2
output deploymentNames array = [for (item, i) in deployments: deployment[i].name]
