// App Service Plan Module
param location string
param appServicePlanName string
param sku object
param tags object

resource appServicePlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: appServicePlanName
  location: location
  tags: tags
  sku: {
    name: sku.name
    tier: sku.tier
    capacity: sku.capacity
  }
  kind: 'linux'
  properties: {
    reserved: true // Required for Linux
  }
}

output appServicePlanId string = appServicePlan.id
output appServicePlanName string = appServicePlan.name
