# Azure 基礎設施即代碼 (Bicep)

Sprint 0 Day 3-4 完成的 Azure 基礎設施 Bicep 模板。

## 架構概覽

本 Bicep 模板部署完整的 Azure 基礎設施,支持 Semantic Kernel Agentic Framework:

```
Azure Subscription
├── Resource Group: rg-skagentic-{env}
│   ├── Virtual Network (10.0.0.0/16)
│   │   ├── AKS Subnet (10.0.1.0/24)
│   │   ├── Database Subnet (10.0.2.0/24)
│   │   ├── Redis Subnet (10.0.3.0/24)
│   │   └── Private Endpoints Subnet (10.0.4.0/24)
│   │
│   ├── Azure Database for PostgreSQL Flexible Server
│   │   ├── Database: semantic_kernel
│   │   ├── Extensions: uuid-ossp, pg_trgm, btree_gin, btree_gist
│   │   └── High Availability (prod only)
│   │
│   ├── Azure Cache for Redis
│   │   ├── SKU: Standard (dev), Premium (prod)
│   │   └── TLS 1.2+ enforced
│   │
│   ├── Azure Container Registry
│   │   ├── SKU: Standard (dev), Premium (prod)
│   │   ├── Geo-replication (prod only)
│   │   └── Content Trust (prod only)
│   │
│   ├── Azure OpenAI Service
│   │   ├── Deployment: gpt-4o
│   │   ├── Deployment: text-embedding-3-large
│   │   └── Deployment: gpt-4o-mini (prod only)
│   │
│   ├── Azure Key Vault
│   │   ├── Soft Delete: Enabled
│   │   ├── Purge Protection (prod only)
│   │   └── RBAC Authorization
│   │
│   └── Azure Storage Account
│       ├── Containers: agent-data, knowledge-base, workflow-definitions
│       ├── File Share: shared-data
│       └── Geo-Redundant Storage (prod only)
```

## 文件結構

```
infrastructure/bicep/
├── main.bicep                    # 主模板 (訂閱級別部署)
├── modules/
│   ├── networking.bicep          # VNet, Subnets, NSGs, Private DNS Zones
│   ├── postgresql.bicep          # PostgreSQL Flexible Server
│   ├── redis.bicep               # Azure Cache for Redis
│   ├── acr.bicep                 # Azure Container Registry
│   ├── keyvault.bicep            # Azure Key Vault
│   ├── openai.bicep              # Azure OpenAI Service
│   └── storage.bicep             # Azure Storage Account
├── parameters/
│   ├── dev.bicepparam            # 開發環境參數
│   ├── staging.bicepparam        # 測試環境參數 (待創建)
│   └── prod.bicepparam           # 生產環境參數
├── deploy.ps1                    # PowerShell 部署腳本
└── README.md                     # 本文檔
```

## 前置要求

### 工具安裝

1. **Azure CLI** (>= 2.50.0)
   ```powershell
   # Windows (使用 winget)
   winget install Microsoft.AzureCLI

   # macOS
   brew install azure-cli

   # 驗證安裝
   az version
   ```

2. **Bicep** (自動安裝)
   ```powershell
   # Azure CLI 會自動安裝 Bicep
   az bicep install

   # 驗證版本
   az bicep version
   ```

3. **Azure 訂閱**
   - 需要 Azure 訂閱 (免費或付費)
   - 需要 Contributor 或 Owner 權限
   - 需要在目標區域有足夠配額

### Azure 登入

```powershell
# 登入 Azure
az login

# 設置默認訂閱
az account set --subscription "<subscription-id>"

# 驗證當前訂閱
az account show
```

## 快速開始

### 1. 開發環境部署

```powershell
# 切換到 Bicep 目錄
cd infrastructure/bicep

# 部署開發環境 (使用默認密碼)
./deploy.ps1 -Environment dev

# 或指定密碼
./deploy.ps1 -Environment dev -PostgresAdminPassword 'YourSecurePassword123!'

# 僅驗證模板 (不部署)
./deploy.ps1 -Environment dev -ValidateOnly

# WhatIf 模式 (查看將創建的資源)
./deploy.ps1 -Environment dev -WhatIf
```

### 2. 生產環境部署

```powershell
# ⚠️ 生產環境必須指定強密碼
./deploy.ps1 -Environment prod -PostgresAdminPassword 'VerySecurePassword123!@#'

# 建議先驗證
./deploy.ps1 -Environment prod -PostgresAdminPassword 'YourPassword' -ValidateOnly
```

### 3. 使用 Azure CLI 直接部署

```powershell
# 開發環境
az deployment sub create \
  --name skagentic-dev-20250103 \
  --location eastus \
  --template-file main.bicep \
  --parameters parameters/dev.bicepparam \
  --parameters postgresAdminPassword='P@ssw0rd123!'

# 生產環境
az deployment sub create \
  --name skagentic-prod-20250103 \
  --location eastus \
  --template-file main.bicep \
  --parameters parameters/prod.bicepparam \
  --parameters postgresAdminPassword='<from-key-vault>'
```

## 部署參數

### 主要參數 (main.bicep)

| 參數 | 類型 | 預設值 | 描述 |
|------|------|--------|------|
| `environment` | string | 'dev' | 環境名稱 (dev, staging, prod) |
| `location` | string | 'eastus' | Azure 區域 |
| `projectName` | string | 'skagentic' | 項目名稱前綴 |
| `postgresAdminUsername` | string | - | PostgreSQL 管理員用戶名 |
| `postgresAdminPassword` | securestring | - | PostgreSQL 管理員密碼 |
| `deployOpenAI` | bool | true | 是否部署 Azure OpenAI |
| `deployPrivateEndpoints` | bool | false | 是否部署私有端點 |

### 環境差異

#### 開發環境 (dev)
- **成本優化**: 最小 SKU,無高可用性
- **訪問**: 公共網絡訪問 (允許本地開發)
- **備份**: 7 天保留
- **監控**: 基本監控

#### 生產環境 (prod)
- **高可用性**: Zone-redundant, Geo-replication
- **安全性**: 私有端點,網絡隔離
- **備份**: 35 天保留,異地備份
- **監控**: 完整診斷日誌,30-90 天保留

## 部署後配置

### 1. 獲取連接信息

```powershell
# 獲取部署輸出
az deployment sub show \
  --name <deployment-name> \
  --query properties.outputs

# 獲取 PostgreSQL 連接字串
az postgres flexible-server show \
  --resource-group rg-skagentic-dev \
  --name postgres-skagentic-dev \
  --query fullyQualifiedDomainName

# 獲取 Redis 連接信息
az redis show \
  --resource-group rg-skagentic-dev \
  --name redis-skagentic-dev \
  --query [hostName,sslPort]

# 獲取 OpenAI 密鑰
az cognitiveservices account keys list \
  --resource-group rg-skagentic-dev \
  --name openai-skagentic-dev
```

### 2. 更新 .env 文件

使用部署輸出更新項目根目錄的 `.env` 文件:

```env
# Azure PostgreSQL
DATABASE_CONNECTION_STRING=Host=<postgres-fqdn>;Database=semantic_kernel;Username=pgadmin;Password=<password>;Port=5432;SSL Mode=Require;

# Azure Redis
REDIS_CONNECTION_STRING=<redis-hostname>:<ssl-port>,password=<redis-key>,ssl=True,abortConnect=False

# Azure OpenAI
AZURE_OPENAI_ENDPOINT=https://<openai-name>.openai.azure.com/
AZURE_OPENAI_API_KEY=<openai-key>
AZURE_OPENAI_CHAT_DEPLOYMENT=gpt-4o
AZURE_OPENAI_EMBEDDING_DEPLOYMENT=text-embedding-3-large
```

### 3. 配置 Key Vault 密鑰

```powershell
$resourceGroup = "rg-skagentic-dev"
$keyVaultName = "kv-skagentic-dev-<unique>"

# 存儲 PostgreSQL 密碼
az keyvault secret set \
  --vault-name $keyVaultName \
  --name "PostgresAdminPassword" \
  --value "<your-password>"

# 存儲 Azure OpenAI 密鑰
az keyvault secret set \
  --vault-name $keyVaultName \
  --name "AzureOpenAIApiKey" \
  --value "<your-api-key>"

# 存儲 Redis 密鑰
az keyvault secret set \
  --vault-name $keyVaultName \
  --name "RedisPassword" \
  --value "<your-redis-key>"
```

### 4. 配置 Service Principal (CI/CD)

```powershell
# 創建 Service Principal
az ad sp create-for-rbac \
  --name "sp-skagentic-cicd" \
  --role Contributor \
  --scopes /subscriptions/<subscription-id>/resourceGroups/rg-skagentic-dev

# 授予 ACR pull 權限
az role assignment create \
  --assignee <service-principal-id> \
  --role AcrPull \
  --scope /subscriptions/<subscription-id>/resourceGroups/rg-skagentic-dev/providers/Microsoft.ContainerRegistry/registries/<acr-name>
```

## 資源管理

### 查看部署資源

```powershell
# 列出資源組中的所有資源
az resource list \
  --resource-group rg-skagentic-dev \
  --output table

# 查看資源成本
az consumption usage list \
  --start-date 2025-01-01 \
  --end-date 2025-01-31 \
  --output table
```

### 更新基礎設施

```powershell
# 修改參數文件後重新部署
./deploy.ps1 -Environment dev -PostgresAdminPassword '<password>'

# Bicep 會自動進行增量更新,只修改變更的資源
```

### 刪除資源

```powershell
# ⚠️ 刪除整個資源組 (不可恢復)
az group delete \
  --name rg-skagentic-dev \
  --yes \
  --no-wait

# 查看刪除進度
az group show --name rg-skagentic-dev
```

## 監控與診斷

### 查看資源狀態

```powershell
# PostgreSQL
az postgres flexible-server show \
  --resource-group rg-skagentic-dev \
  --name postgres-skagentic-dev \
  --query state

# Redis
az redis show \
  --resource-group rg-skagentic-dev \
  --name redis-skagentic-dev \
  --query provisioningState

# OpenAI
az cognitiveservices account show \
  --resource-group rg-skagentic-dev \
  --name openai-skagentic-dev \
  --query properties.provisioningState
```

### 查看診斷日誌

```powershell
# 啟用診斷設置 (已在模板中配置)
# 查看 Log Analytics workspace
az monitor log-analytics workspace list \
  --resource-group rg-skagentic-dev \
  --output table
```

## 成本優化

### 開發環境成本估算

| 服務 | SKU | 月度成本 (USD) |
|------|-----|----------------|
| PostgreSQL Flexible Server | B2s (2 vCore, 4GB) | ~$30 |
| Redis Cache | Standard C1 (1GB) | ~$30 |
| Container Registry | Standard | ~$20 |
| Azure OpenAI | Pay-per-use | 變動 |
| Storage Account | Standard LRS | ~$5 |
| **總計** | | **~$85 + OpenAI 使用費** |

### 生產環境成本估算

| 服務 | SKU | 月度成本 (USD) |
|------|-----|----------------|
| PostgreSQL Flexible Server | D4s_v3 (4 vCore, 16GB, HA) | ~$350 |
| Redis Cache | Premium P1 (6GB, Cluster) | ~$350 |
| Container Registry | Premium (Geo-rep) | ~$168 |
| Azure OpenAI | Pay-per-use | 變動 |
| Storage Account | Standard GRS | ~$30 |
| **總計** | | **~$898 + OpenAI 使用費** |

### 節省成本技巧

1. **使用 Azure Reserved Instances** (生產環境 1-3 年預留,節省高達 72%)
2. **開發環境定時關閉** (非工作時間停止資源)
3. **使用 Azure Dev/Test 定價** (開發/測試訂閱可節省成本)
4. **監控 OpenAI Token 使用** (設置預算警報)

## 故障排除

### 常見問題

#### 1. 部署失敗: QuotaExceeded

```
Error: The subscription quota has been exceeded
```

**解決方案**:
- 請求增加配額: Azure Portal → Subscription → Usage + quotas
- 或選擇其他 Azure 區域部署

#### 2. PostgreSQL 無法連接

```
Error: connection refused
```

**檢查清單**:
- 防火牆規則是否正確
- SSL 模式是否正確 (Require)
- 密碼是否正確
- 網絡連接是否正常

#### 3. OpenAI 模型部署失敗

```
Error: Model deployment failed
```

**解決方案**:
- 檢查區域可用性 (不是所有區域都支持所有模型)
- 檢查配額限制
- 確認模型版本正確

### 驗證部署

```powershell
# 運行驗證腳本 (待創建)
./validate-deployment.ps1 -Environment dev
```

## 安全最佳實踐

### 密碼管理

- ✅ 使用 Azure Key Vault 存儲所有密碼和密鑰
- ✅ 啟用 Key Vault 軟刪除和清除保護
- ✅ 定期輪換密碼和密鑰
- ❌ 不要在代碼或參數文件中硬編碼密碼

### 網絡安全

- ✅ 生產環境使用私有端點
- ✅ 配置網絡安全組 (NSG) 限制訪問
- ✅ 啟用 DDoS Protection (生產環境)
- ✅ 使用 Azure Firewall 或 Application Gateway

### 身份與訪問

- ✅ 使用 Azure AD 身份驗證 (而非 SQL 身份驗證)
- ✅ 實施最小權限原則 (RBAC)
- ✅ 啟用 Managed Identity
- ✅ 定期審計訪問日誌

## 下一步

✅ Sprint 0 Day 3-4 完成

**Sprint 0 Day 5**: CI/CD Pipeline 設置
- 配置 GitHub Actions workflows
- 設置自動化部署
- 配置環境審批流程
- 設置自動化測試

查看 `docs/technical-implementation/8-deployment-architecture/cicd-pipeline.md` 獲取詳細信息。

## 參考資源

- [Azure Bicep Documentation](https://docs.microsoft.com/azure/azure-resource-manager/bicep/)
- [Azure OpenAI Service Documentation](https://learn.microsoft.com/azure/cognitive-services/openai/)
- [PostgreSQL Flexible Server Documentation](https://learn.microsoft.com/azure/postgresql/flexible-server/)
- [Azure Cache for Redis Documentation](https://learn.microsoft.com/azure/azure-cache-for-redis/)
