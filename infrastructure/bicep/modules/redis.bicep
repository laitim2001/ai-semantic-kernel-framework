// =============================================================================
// Azure Cache for Redis Module
// =============================================================================

@description('Azure 部署區域')
param location string

@description('項目名稱前綴')
param projectName string

@description('環境名稱')
param environment string

@description('資源標籤')
param tags object

@description('Redis SKU')
param redisSku string = environment == 'prod' ? 'Premium' : 'Standard'

@description('Redis 系列')
param redisFamily string = environment == 'prod' ? 'P' : 'C'

@description('Redis 容量')
param redisCapacity int = environment == 'prod' ? 1 : 1

@description('啟用非 SSL 端口')
param enableNonSslPort bool = environment == 'dev'

@description('子網 ID (Private Link)')
param subnetId string = ''

// =============================================================================
// Variables
// =============================================================================

var redisName = 'redis-${projectName}-${environment}'

// =============================================================================
// Azure Cache for Redis
// =============================================================================

resource redis 'Microsoft.Cache/redis@2023-08-01' = {
  name: redisName
  location: location
  tags: tags
  properties: {
    sku: {
      name: redisSku
      family: redisFamily
      capacity: redisCapacity
    }
    enableNonSslPort: enableNonSslPort
    minimumTlsVersion: '1.2'
    publicNetworkAccess: empty(subnetId) ? 'Enabled' : 'Disabled'
    redisConfiguration: {
      'maxmemory-policy': 'allkeys-lru'
      'maxmemory-reserved': redisSku == 'Premium' ? '200' : '50'
      'maxfragmentationmemory-reserved': redisSku == 'Premium' ? '200' : '50'
    }
    redisVersion: '6'
  }
}

// =============================================================================
// Firewall Rules (公共訪問模式)
// =============================================================================

resource firewallRuleAllowAzureServices 'Microsoft.Cache/redis/firewallRules@2023-08-01' = if (empty(subnetId) && environment != 'dev') {
  parent: redis
  name: 'AllowAzureServices'
  properties: {
    startIP: '0.0.0.0'
    endIP: '0.0.0.0'
  }
}

// 本地開發訪問 (僅 dev 環境)
resource firewallRuleAllowAll 'Microsoft.Cache/redis/firewallRules@2023-08-01' = if (empty(subnetId) && environment == 'dev') {
  parent: redis
  name: 'AllowAllIPs'
  properties: {
    startIP: '0.0.0.0'
    endIP: '255.255.255.255'
  }
}

// =============================================================================
// Diagnostic Settings
// =============================================================================

resource diagnosticSettings 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = if (environment != 'dev') {
  name: '${redisName}-diagnostics'
  scope: redis
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
    ]
  }
}

// =============================================================================
// Outputs
// =============================================================================

output redisId string = redis.id
output redisName string = redis.name
output hostName string = redis.properties.hostName
output sslPort int = redis.properties.sslPort
output port int = redis.properties.port
output primaryKey string = redis.listKeys().primaryKey
output connectionString string = '${redis.properties.hostName}:${redis.properties.sslPort},password=${redis.listKeys().primaryKey},ssl=True,abortConnect=False'
