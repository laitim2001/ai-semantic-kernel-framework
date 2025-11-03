// =============================================================================
// Production Environment Parameters
// =============================================================================

using '../main.bicep'

param environment = 'prod'
param location = 'eastus'
param projectName = 'skagentic'

param tags = {
  Environment: 'Production'
  Project: 'Semantic Kernel Agentic Framework'
  ManagedBy: 'Bicep'
  CostCenter: 'Engineering'
  Owner: 'Platform Team'
  Compliance: 'SOC2'
  DataClassification: 'Confidential'
}

// PostgreSQL 配置 (生產環境必須使用強密碼並從 Key Vault 獲取)
param postgresAdminUsername = 'pgadmin'
param postgresAdminPassword = '' // 必須從 Azure Key Vault 或安全環境變數獲取

// 生產環境部署私有端點以提高安全性
param deployPrivateEndpoints = true

// 生產環境部署 OpenAI 服務
param deployOpenAI = true

param openAIDeployments = [
  {
    name: 'gpt-4o'
    model: {
      format: 'OpenAI'
      name: 'gpt-4o'
      version: '2024-05-13'
    }
    sku: {
      name: 'Standard'
      capacity: 50 // 生產環境更高容量
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
      capacity: 50
    }
  }
  {
    name: 'gpt-4o-mini'
    model: {
      format: 'OpenAI'
      name: 'gpt-4o-mini'
      version: '2024-07-18'
    }
    sku: {
      name: 'Standard'
      capacity: 100
    }
  }
]
