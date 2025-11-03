// =============================================================================
// Azure Storage Account Module
// =============================================================================

@description('Azure 部署區域')
param location string

@description('項目名稱前綴')
param projectName string

@description('環境名稱')
param environment string

@description('資源標籤')
param tags object

@description('唯一後綴')
param uniqueSuffix string

@description('Storage SKU')
@allowed([
  'Standard_LRS'
  'Standard_GRS'
  'Standard_RAGRS'
  'Standard_ZRS'
  'Premium_LRS'
  'Premium_ZRS'
])
param skuName string = environment == 'prod' ? 'Standard_GRS' : 'Standard_LRS'

@description('Storage Kind')
@allowed([
  'Storage'
  'StorageV2'
  'BlobStorage'
])
param kind string = 'StorageV2'

@description('啟用分層命名空間 (Data Lake Gen2)')
param enableHierarchicalNamespace bool = false

// =============================================================================
// Variables
// =============================================================================

// Storage account 名稱必須全局唯一,長度 3-24,只能包含小寫字母和數字
var storageAccountName = toLower('st${replace(projectName, '-', '')}${environment}${uniqueSuffix}')

// =============================================================================
// Storage Account
// =============================================================================

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: storageAccountName
  location: location
  tags: tags
  sku: {
    name: skuName
  }
  kind: kind
  properties: {
    accessTier: 'Hot'
    allowBlobPublicAccess: false
    allowSharedKeyAccess: true
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
    isHnsEnabled: enableHierarchicalNamespace
    networkAcls: {
      bypass: 'AzureServices'
      defaultAction: environment == 'dev' ? 'Allow' : 'Deny'
      ipRules: []
      virtualNetworkRules: []
    }
    encryption: {
      services: {
        blob: {
          enabled: true
          keyType: 'Account'
        }
        file: {
          enabled: true
          keyType: 'Account'
        }
        table: {
          enabled: true
          keyType: 'Account'
        }
        queue: {
          enabled: true
          keyType: 'Account'
        }
      }
      keySource: 'Microsoft.Storage'
    }
  }
}

// =============================================================================
// Blob Service
// =============================================================================

resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2023-01-01' = {
  parent: storageAccount
  name: 'default'
  properties: {
    deleteRetentionPolicy: {
      enabled: true
      days: environment == 'prod' ? 30 : 7
    }
    containerDeleteRetentionPolicy: {
      enabled: true
      days: environment == 'prod' ? 30 : 7
    }
    changeFeed: {
      enabled: environment != 'dev'
      retentionInDays: environment == 'prod' ? 30 : 7
    }
    isVersioningEnabled: environment == 'prod'
  }
}

// =============================================================================
// Blob Containers
// =============================================================================

var containers = [
  'agent-data'
  'knowledge-base'
  'workflow-definitions'
  'code-executions'
  'user-uploads'
  'backups'
]

resource blobContainers 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-01-01' = [for container in containers: {
  parent: blobService
  name: container
  properties: {
    publicAccess: 'None'
  }
}]

// =============================================================================
// File Service
// =============================================================================

resource fileService 'Microsoft.Storage/storageAccounts/fileServices@2023-01-01' = {
  parent: storageAccount
  name: 'default'
  properties: {
    shareDeleteRetentionPolicy: {
      enabled: true
      days: environment == 'prod' ? 30 : 7
    }
  }
}

// =============================================================================
// File Shares
// =============================================================================

resource fileShare 'Microsoft.Storage/storageAccounts/fileServices/shares@2023-01-01' = {
  parent: fileService
  name: 'shared-data'
  properties: {
    accessTier: 'TransactionOptimized'
    shareQuota: environment == 'prod' ? 1024 : 100
    enabledProtocols: 'SMB'
  }
}

// =============================================================================
// Table Service
// =============================================================================

resource tableService 'Microsoft.Storage/storageAccounts/tableServices@2023-01-01' = {
  parent: storageAccount
  name: 'default'
}

// =============================================================================
// Queue Service
// =============================================================================

resource queueService 'Microsoft.Storage/storageAccounts/queueServices@2023-01-01' = {
  parent: storageAccount
  name: 'default'
}

// =============================================================================
// Diagnostic Settings
// =============================================================================

resource diagnosticSettings 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = if (environment != 'dev') {
  name: '${storageAccountName}-diagnostics'
  scope: storageAccount
  properties: {
    metrics: [
      {
        category: 'Transaction'
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

output storageAccountId string = storageAccount.id
output storageAccountName string = storageAccount.name
output primaryEndpoints object = storageAccount.properties.primaryEndpoints
output primaryKey string = storageAccount.listKeys().keys[0].value
output connectionString string = 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};AccountKey=${storageAccount.listKeys().keys[0].value};EndpointSuffix=${az.environment().suffixes.storage}'
