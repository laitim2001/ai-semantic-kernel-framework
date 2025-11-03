// =============================================================================
// Development Environment Parameters
// =============================================================================

using '../main.bicep'

param environment = 'dev'
param location = 'eastus'
param projectName = 'skagentic'

param tags = {
  Environment: 'Development'
  Project: 'Semantic Kernel Agentic Framework'
  ManagedBy: 'Bicep'
  CostCenter: 'Engineering'
  Owner: 'DevOps Team'
}

// PostgreSQL 配置 (開發環境使用簡單密碼,生產環境必須使用強密碼)
param postgresAdminUsername = 'pgadmin'
param postgresAdminPassword = 'P@ssw0rd123!' // 實際部署時從 Key Vault 或環境變數獲取

// 開發環境不部署私有端點以節省成本
param deployPrivateEndpoints = false

// 開發環境部署 OpenAI 服務
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
