// =============================================================================
// Networking Module - Virtual Network & Subnets
// =============================================================================

@description('Azure 部署區域')
param location string

@description('項目名稱前綴')
param projectName string

@description('環境名稱')
param environment string

@description('資源標籤')
param tags object

// =============================================================================
// Variables
// =============================================================================

var vnetName = 'vnet-${projectName}-${environment}'
var vnetAddressPrefix = '10.0.0.0/16'

var subnets = [
  {
    name: 'snet-aks'
    addressPrefix: '10.0.1.0/24'
    delegation: []
  }
  {
    name: 'snet-database'
    addressPrefix: '10.0.2.0/24'
    delegation: [
      {
        name: 'PostgreSQLFlexibleServer'
        properties: {
          serviceName: 'Microsoft.DBforPostgreSQL/flexibleServers'
        }
      }
    ]
  }
  {
    name: 'snet-redis'
    addressPrefix: '10.0.3.0/24'
    delegation: []
  }
  {
    name: 'snet-privateendpoints'
    addressPrefix: '10.0.4.0/24'
    delegation: []
  }
]

// =============================================================================
// Network Security Groups
// =============================================================================

resource aksNsg 'Microsoft.Network/networkSecurityGroups@2023-05-01' = {
  name: 'nsg-aks-${environment}'
  location: location
  tags: tags
  properties: {
    securityRules: [
      {
        name: 'AllowHTTPS'
        properties: {
          priority: 100
          direction: 'Inbound'
          access: 'Allow'
          protocol: 'Tcp'
          sourcePortRange: '*'
          destinationPortRange: '443'
          sourceAddressPrefix: '*'
          destinationAddressPrefix: '*'
        }
      }
      {
        name: 'AllowHTTP'
        properties: {
          priority: 110
          direction: 'Inbound'
          access: 'Allow'
          protocol: 'Tcp'
          sourcePortRange: '*'
          destinationPortRange: '80'
          sourceAddressPrefix: '*'
          destinationAddressPrefix: '*'
        }
      }
    ]
  }
}

resource databaseNsg 'Microsoft.Network/networkSecurityGroups@2023-05-01' = {
  name: 'nsg-database-${environment}'
  location: location
  tags: tags
  properties: {
    securityRules: [
      {
        name: 'AllowPostgreSQL'
        properties: {
          priority: 100
          direction: 'Inbound'
          access: 'Allow'
          protocol: 'Tcp'
          sourcePortRange: '*'
          destinationPortRange: '5432'
          sourceAddressPrefix: 'VirtualNetwork'
          destinationAddressPrefix: '*'
        }
      }
    ]
  }
}

resource redisNsg 'Microsoft.Network/networkSecurityGroups@2023-05-01' = {
  name: 'nsg-redis-${environment}'
  location: location
  tags: tags
  properties: {
    securityRules: [
      {
        name: 'AllowRedis'
        properties: {
          priority: 100
          direction: 'Inbound'
          access: 'Allow'
          protocol: 'Tcp'
          sourcePortRange: '*'
          destinationPortRange: '6379-6380'
          sourceAddressPrefix: 'VirtualNetwork'
          destinationAddressPrefix: '*'
        }
      }
    ]
  }
}

resource privateEndpointNsg 'Microsoft.Network/networkSecurityGroups@2023-05-01' = {
  name: 'nsg-privateendpoints-${environment}'
  location: location
  tags: tags
  properties: {
    securityRules: []
  }
}

// =============================================================================
// Virtual Network
// =============================================================================

resource vnet 'Microsoft.Network/virtualNetworks@2023-05-01' = {
  name: vnetName
  location: location
  tags: tags
  properties: {
    addressSpace: {
      addressPrefixes: [
        vnetAddressPrefix
      ]
    }
    subnets: [
      {
        name: subnets[0].name
        properties: {
          addressPrefix: subnets[0].addressPrefix
          networkSecurityGroup: {
            id: aksNsg.id
          }
          serviceEndpoints: [
            {
              service: 'Microsoft.ContainerRegistry'
            }
            {
              service: 'Microsoft.Storage'
            }
          ]
        }
      }
      {
        name: subnets[1].name
        properties: {
          addressPrefix: subnets[1].addressPrefix
          networkSecurityGroup: {
            id: databaseNsg.id
          }
          delegations: subnets[1].delegation
          serviceEndpoints: [
            {
              service: 'Microsoft.Storage'
            }
          ]
        }
      }
      {
        name: subnets[2].name
        properties: {
          addressPrefix: subnets[2].addressPrefix
          networkSecurityGroup: {
            id: redisNsg.id
          }
          serviceEndpoints: []
        }
      }
      {
        name: subnets[3].name
        properties: {
          addressPrefix: subnets[3].addressPrefix
          networkSecurityGroup: {
            id: privateEndpointNsg.id
          }
          privateEndpointNetworkPolicies: 'Disabled'
          privateLinkServiceNetworkPolicies: 'Disabled'
        }
      }
    ]
  }
}

// =============================================================================
// Private DNS Zones
// =============================================================================

resource postgresDnsZone 'Microsoft.Network/privateDnsZones@2020-06-01' = {
  name: 'privatelink.postgres.database.azure.com'
  location: 'global'
  tags: tags
}

resource postgresDnsZoneLink 'Microsoft.Network/privateDnsZones/virtualNetworkLinks@2020-06-01' = {
  parent: postgresDnsZone
  name: '${vnetName}-link'
  location: 'global'
  properties: {
    registrationEnabled: false
    virtualNetwork: {
      id: vnet.id
    }
  }
}

resource redisDnsZone 'Microsoft.Network/privateDnsZones@2020-06-01' = {
  name: 'privatelink.redis.cache.windows.net'
  location: 'global'
  tags: tags
}

resource redisDnsZoneLink 'Microsoft.Network/privateDnsZones/virtualNetworkLinks@2020-06-01' = {
  parent: redisDnsZone
  name: '${vnetName}-link'
  location: 'global'
  properties: {
    registrationEnabled: false
    virtualNetwork: {
      id: vnet.id
    }
  }
}

resource keyVaultDnsZone 'Microsoft.Network/privateDnsZones@2020-06-01' = {
  name: 'privatelink.vaultcore.azure.net'
  location: 'global'
  tags: tags
}

resource keyVaultDnsZoneLink 'Microsoft.Network/privateDnsZones/virtualNetworkLinks@2020-06-01' = {
  parent: keyVaultDnsZone
  name: '${vnetName}-link'
  location: 'global'
  properties: {
    registrationEnabled: false
    virtualNetwork: {
      id: vnet.id
    }
  }
}

// =============================================================================
// Outputs
// =============================================================================

output vnetId string = vnet.id
output vnetName string = vnet.name
output aksSubnetId string = vnet.properties.subnets[0].id
output databaseSubnetId string = vnet.properties.subnets[1].id
output redisSubnetId string = vnet.properties.subnets[2].id
output privateEndpointSubnetId string = vnet.properties.subnets[3].id
output postgresDnsZoneId string = postgresDnsZone.id
output redisDnsZoneId string = redisDnsZone.id
output keyVaultDnsZoneId string = keyVaultDnsZone.id
