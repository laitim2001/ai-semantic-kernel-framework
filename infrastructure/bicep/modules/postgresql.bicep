// =============================================================================
// PostgreSQL Flexible Server Module
// =============================================================================

@description('Azure 部署區域')
param location string

@description('項目名稱前綴')
param projectName string

@description('環境名稱')
param environment string

@description('資源標籤')
param tags object

@description('管理員登入名稱')
param administratorLogin string

@description('管理員密碼')
@secure()
param administratorPassword string

@description('PostgreSQL 版本')
param postgresVersion string = '16'

@description('SKU 名稱')
param skuName string = environment == 'prod' ? 'Standard_D4s_v3' : 'Standard_B2s'

@description('SKU Tier')
param skuTier string = environment == 'prod' ? 'GeneralPurpose' : 'Burstable'

@description('存儲大小 (GB)')
param storageSizeGB int = environment == 'prod' ? 128 : 32

@description('備份保留天數')
param backupRetentionDays int = environment == 'prod' ? 35 : 7

@description('啟用高可用性')
param highAvailability bool = environment == 'prod'

@description('委派子網資源 ID')
param delegatedSubnetResourceId string = ''

@description('私有 DNS 區域資源 ID')
param privateDnsZoneArmResourceId string = ''

@description('子網 ID (用於防火牆規則)')
param subnetId string = ''

// =============================================================================
// Variables
// =============================================================================

var serverName = 'postgres-${projectName}-${environment}'
var databaseName = 'semantic_kernel'

// =============================================================================
// PostgreSQL Flexible Server
// =============================================================================

resource postgresServer 'Microsoft.DBforPostgreSQL/flexibleServers@2023-03-01-preview' = {
  name: serverName
  location: location
  tags: tags
  sku: {
    name: skuName
    tier: skuTier
  }
  properties: {
    version: postgresVersion
    administratorLogin: administratorLogin
    administratorLoginPassword: administratorPassword
    storage: {
      storageSizeGB: storageSizeGB
      autoGrow: 'Enabled'
    }
    backup: {
      backupRetentionDays: backupRetentionDays
      geoRedundantBackup: environment == 'prod' ? 'Enabled' : 'Disabled'
    }
    highAvailability: highAvailability ? {
      mode: 'ZoneRedundant'
    } : {
      mode: 'Disabled'
    }
    network: !empty(delegatedSubnetResourceId) ? {
      delegatedSubnetResourceId: delegatedSubnetResourceId
      privateDnsZoneArmResourceId: privateDnsZoneArmResourceId
    } : null
    authConfig: {
      activeDirectoryAuth: 'Disabled'
      passwordAuth: 'Enabled'
    }
  }
}

// =============================================================================
// Firewall Rules (公共訪問模式)
// =============================================================================

resource firewallRuleAllowAzureServices 'Microsoft.DBforPostgreSQL/flexibleServers/firewallRules@2023-03-01-preview' = if (empty(delegatedSubnetResourceId)) {
  parent: postgresServer
  name: 'AllowAllAzureServicesAndResourcesWithinAzureIps'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

// 本地開發訪問 (僅 dev 環境)
resource firewallRuleAllowDevelopment 'Microsoft.DBforPostgreSQL/flexibleServers/firewallRules@2023-03-01-preview' = if (environment == 'dev' && empty(delegatedSubnetResourceId)) {
  parent: postgresServer
  name: 'AllowDevelopmentAccess'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '255.255.255.255'
  }
}

// =============================================================================
// PostgreSQL Configuration
// =============================================================================

resource configMaxConnections 'Microsoft.DBforPostgreSQL/flexibleServers/configurations@2023-03-01-preview' = {
  parent: postgresServer
  name: 'max_connections'
  properties: {
    value: environment == 'prod' ? '200' : '100'
    source: 'user-override'
  }
}

resource configSharedBuffers 'Microsoft.DBforPostgreSQL/flexibleServers/configurations@2023-03-01-preview' = {
  parent: postgresServer
  name: 'shared_buffers'
  properties: {
    value: environment == 'prod' ? '524288' : '16384'
    source: 'user-override'
  }
}

// =============================================================================
// Database
// =============================================================================

resource database 'Microsoft.DBforPostgreSQL/flexibleServers/databases@2023-03-01-preview' = {
  parent: postgresServer
  name: databaseName
  properties: {
    charset: 'UTF8'
    collation: 'en_US.utf8'
  }
}

// =============================================================================
// Outputs
// =============================================================================

output serverId string = postgresServer.id
output serverName string = postgresServer.name
output serverFqdn string = postgresServer.properties.fullyQualifiedDomainName
output databaseName string = database.name
output connectionString string = 'Host=${postgresServer.properties.fullyQualifiedDomainName};Database=${database.name};Username=${administratorLogin};Password=${administratorPassword};Port=5432;SSL Mode=Require;'
