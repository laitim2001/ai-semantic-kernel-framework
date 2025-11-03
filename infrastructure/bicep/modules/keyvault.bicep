// =============================================================================
// Azure Key Vault Module
// =============================================================================

@description('Azure 部署區域')
param location string

@description('項目名稱前綴')
param projectName string

@description('環境名稱')
param environment string

@description('資源標籤')
param tags object

@description('Key Vault SKU')
@allowed([
  'standard'
  'premium'
])
param skuName string = environment == 'prod' ? 'premium' : 'standard'

@description('啟用軟刪除')
param enableSoftDelete bool = true

@description('軟刪除保留天數')
param softDeleteRetentionDays int = 90

@description('啟用清除保護')
param enablePurgeProtection bool = environment == 'prod'

@description('子網 ID (Private Link)')
param subnetId string = ''

// =============================================================================
// Variables
// =============================================================================

var keyVaultName = 'kv-${projectName}-${environment}-${uniqueString(resourceGroup().id)}'

// =============================================================================
// Azure Key Vault
// =============================================================================

resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: keyVaultName
  location: location
  tags: tags
  properties: {
    sku: {
      family: 'A'
      name: skuName
    }
    tenantId: subscription().tenantId
    enabledForDeployment: true
    enabledForDiskEncryption: true
    enabledForTemplateDeployment: true
    enableSoftDelete: enableSoftDelete
    softDeleteRetentionInDays: softDeleteRetentionDays
    enablePurgeProtection: enablePurgeProtection ? true : null
    enableRbacAuthorization: true
    publicNetworkAccess: empty(subnetId) ? 'Enabled' : 'Disabled'
    networkAcls: {
      bypass: 'AzureServices'
      defaultAction: empty(subnetId) ? (environment == 'dev' ? 'Allow' : 'Deny') : 'Deny'
      ipRules: []
      virtualNetworkRules: []
    }
  }
}

// =============================================================================
// Diagnostic Settings
// =============================================================================

resource diagnosticSettings 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = if (environment != 'dev') {
  name: '${keyVaultName}-diagnostics'
  scope: keyVault
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
        category: 'AuditEvent'
        enabled: true
        retentionPolicy: {
          enabled: true
          days: 90
        }
      }
      {
        category: 'AzurePolicyEvaluationDetails'
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

output keyVaultId string = keyVault.id
output keyVaultName string = keyVault.name
output vaultUri string = keyVault.properties.vaultUri
