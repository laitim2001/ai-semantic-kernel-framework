# Azure 部署檢查清單

Sprint 0 Day 3-4 完成的基礎設施部署驗證清單。

## 部署前檢查

### 工具與權限

- [ ] Azure CLI 已安裝 (`az version`)
- [ ] Bicep 已安裝 (`az bicep version`)
- [ ] 已登入 Azure (`az login`)
- [ ] 確認訂閱正確 (`az account show`)
- [ ] 確認擁有 Contributor 或 Owner 權限
- [ ] 確認目標區域有足夠配額

### 參數準備

- [ ] 已創建或更新環境參數文件 (`parameters/{env}.bicepparam`)
- [ ] PostgreSQL 管理員密碼已準備 (開發環境可使用默認,生產環境必須使用強密碼)
- [ ] 確認項目名稱前綴 (`projectName`)
- [ ] 確認部署區域 (`location`)
- [ ] 確認是否部署私有端點 (`deployPrivateEndpoints`)
- [ ] 確認是否部署 OpenAI 服務 (`deployOpenAI`)

## 部署步驟

### 1. 驗證模板

```powershell
./deploy.ps1 -Environment dev -ValidateOnly
```

**檢查項**:
- [ ] 模板語法正確
- [ ] 參數文件正確
- [ ] 沒有驗證錯誤

### 2. WhatIf 檢查

```powershell
./deploy.ps1 -Environment dev -WhatIf
```

**檢查項**:
- [ ] 資源創建列表正確
- [ ] 沒有意外的資源刪除
- [ ] SKU 和配置符合預期

### 3. 執行部署

```powershell
./deploy.ps1 -Environment dev -PostgresAdminPassword 'YourPassword'
```

**預期時間**: 15-30 分鐘

**檢查項**:
- [ ] 部署成功完成
- [ ] 所有資源創建成功
- [ ] 沒有錯誤或警告
- [ ] 輸出變量正確

## 部署後驗證

### 資源檢查

```powershell
# 檢查資源組
az group show --name rg-skagentic-dev

# 列出所有資源
az resource list --resource-group rg-skagentic-dev --output table
```

**驗證清單**:
- [ ] 資源組已創建
- [ ] Virtual Network 已創建 (包含 4 個子網)
- [ ] PostgreSQL Flexible Server 已創建並運行
- [ ] Redis Cache 已創建並運行
- [ ] Container Registry 已創建
- [ ] Key Vault 已創建
- [ ] Storage Account 已創建
- [ ] Azure OpenAI Service 已創建 (如果啟用)

### 網絡配置檢查

```powershell
# 檢查 VNet
az network vnet show \
  --resource-group rg-skagentic-dev \
  --name vnet-skagentic-dev

# 檢查子網
az network vnet subnet list \
  --resource-group rg-skagentic-dev \
  --vnet-name vnet-skagentic-dev \
  --output table
```

**驗證清單**:
- [ ] VNet 地址空間: 10.0.0.0/16
- [ ] AKS Subnet: 10.0.1.0/24
- [ ] Database Subnet: 10.0.2.0/24
- [ ] Redis Subnet: 10.0.3.0/24
- [ ] Private Endpoints Subnet: 10.0.4.0/24
- [ ] NSG 已附加到子網
- [ ] Private DNS Zones 已創建

### PostgreSQL 檢查

```powershell
# 檢查 PostgreSQL 狀態
az postgres flexible-server show \
  --resource-group rg-skagentic-dev \
  --name postgres-skagentic-dev \
  --query '{Name:name, State:state, FQDN:fullyQualifiedDomainName, Version:version}'
```

**驗證清單**:
- [ ] Server 狀態: Ready
- [ ] PostgreSQL 版本: 16
- [ ] Database 已創建: semantic_kernel
- [ ] 防火牆規則已配置 (dev 環境)
- [ ] 可以連接到數據庫

**連接測試** (使用 Docker):
```bash
docker run --rm -it postgres:16-alpine psql -h <server-fqdn> -U pgadmin -d semantic_kernel
```

### Redis 檢查

```powershell
# 檢查 Redis 狀態
az redis show \
  --resource-group rg-skagentic-dev \
  --name redis-skagentic-dev \
  --query '{Name:name, State:provisioningState, HostName:hostName, Port:sslPort}'
```

**驗證清單**:
- [ ] Redis 狀態: Succeeded
- [ ] SKU 正確 (Standard/Premium)
- [ ] SSL Port: 6380
- [ ] TLS 版本: 1.2+
- [ ] 可以連接到 Redis

**連接測試** (使用本地 Redis CLI):
```bash
redis-cli -h <redis-hostname> -p 6380 --tls -a <redis-key> PING
```

### Container Registry 檢查

```powershell
# 檢查 ACR 狀態
az acr show \
  --resource-group rg-skagentic-dev \
  --name <acr-name> \
  --query '{Name:name, LoginServer:loginServer, AdminEnabled:adminUserEnabled, SKU:sku.name}'
```

**驗證清單**:
- [ ] ACR 已創建
- [ ] Login Server 可訪問
- [ ] Admin 用戶已啟用 (dev 環境)
- [ ] SKU 正確 (Standard/Premium)

**登入測試**:
```bash
az acr login --name <acr-name>
```

### Key Vault 檢查

```powershell
# 檢查 Key Vault 狀態
az keyvault show \
  --name <keyvault-name> \
  --query '{Name:name, VaultUri:properties.vaultUri, SKU:properties.sku.name, EnabledForDeployment:properties.enabledForDeployment}'
```

**驗證清單**:
- [ ] Key Vault 已創建
- [ ] RBAC 授權已啟用
- [ ] 軟刪除已啟用
- [ ] 可以訪問 Key Vault

**訪問測試**:
```powershell
az keyvault secret list --vault-name <keyvault-name>
```

### Azure OpenAI 檢查

```powershell
# 檢查 OpenAI 狀態
az cognitiveservices account show \
  --resource-group rg-skagentic-dev \
  --name openai-skagentic-dev \
  --query '{Name:name, Endpoint:properties.endpoint, State:properties.provisioningState}'

# 列出模型部署
az cognitiveservices account deployment list \
  --resource-group rg-skagentic-dev \
  --name openai-skagentic-dev \
  --output table
```

**驗證清單**:
- [ ] OpenAI Service 已創建
- [ ] 部署狀態: Succeeded
- [ ] gpt-4o 模型已部署
- [ ] text-embedding-3-large 模型已部署
- [ ] Endpoint 可訪問

**API 測試** (使用 curl):
```bash
curl https://<openai-name>.openai.azure.com/openai/deployments/<deployment-name>/chat/completions?api-version=2024-02-15-preview \
  -H "Content-Type: application/json" \
  -H "api-key: <api-key>" \
  -d '{
    "messages": [{"role": "user", "content": "Hello!"}],
    "max_tokens": 10
  }'
```

### Storage Account 檢查

```powershell
# 檢查 Storage Account
az storage account show \
  --resource-group rg-skagentic-dev \
  --name <storage-account-name> \
  --query '{Name:name, Kind:kind, SKU:sku.name, EnableHttpsTrafficOnly:enableHttpsTrafficOnly}'

# 列出 Blob 容器
az storage container list \
  --account-name <storage-account-name> \
  --output table
```

**驗證清單**:
- [ ] Storage Account 已創建
- [ ] SKU 正確 (Standard_LRS/Standard_GRS)
- [ ] HTTPS Only: true
- [ ] Blob 容器已創建: agent-data, knowledge-base, workflow-definitions, etc.
- [ ] File Share 已創建: shared-data

## 配置與集成

### 更新 .env 文件

使用部署輸出更新項目根目錄的 `.env` 文件:

```powershell
# 獲取連接字串
az deployment sub show \
  --name <deployment-name> \
  --query properties.outputs
```

**檢查項**:
- [ ] DATABASE_CONNECTION_STRING 已更新
- [ ] REDIS_CONNECTION_STRING 已更新
- [ ] AZURE_OPENAI_ENDPOINT 已更新
- [ ] AZURE_OPENAI_API_KEY 已更新
- [ ] 所有連接字串格式正確

### 存儲密鑰到 Key Vault

```powershell
$kvName = "<keyvault-name>"

# 存儲 PostgreSQL 密碼
az keyvault secret set --vault-name $kvName --name "PostgresAdminPassword" --value "<password>"

# 存儲 Redis 密鑰
az keyvault secret set --vault-name $kvName --name "RedisPrimaryKey" --value "<redis-key>"

# 存儲 OpenAI API 密鑰
az keyvault secret set --vault-name $kvName --name "AzureOpenAIApiKey" --value "<api-key>"
```

**檢查項**:
- [ ] PostgreSQL 密碼已存儲
- [ ] Redis 密鑰已存儲
- [ ] OpenAI API 密鑰已存儲
- [ ] 可以讀取所有密鑰

### 配置訪問權限

```powershell
# 為應用程序 Service Principal 授予權限
$spId = "<service-principal-id>"

# Key Vault 權限
az role assignment create \
  --assignee $spId \
  --role "Key Vault Secrets User" \
  --scope /subscriptions/<subscription-id>/resourceGroups/rg-skagentic-dev/providers/Microsoft.KeyVault/vaults/<kv-name>

# ACR 權限
az role assignment create \
  --assignee $spId \
  --role "AcrPull" \
  --scope /subscriptions/<subscription-id>/resourceGroups/rg-skagentic-dev/providers/Microsoft.ContainerRegistry/registries/<acr-name>
```

**檢查項**:
- [ ] Service Principal 已創建
- [ ] Key Vault 訪問權限已授予
- [ ] ACR Pull 權限已授予
- [ ] 其他必要權限已授予

## 成本與監控

### 檢查成本

```powershell
# 查看資源組成本
az consumption usage list \
  --start-date $(Get-Date -Format yyyy-MM-dd) \
  --end-date $(Get-Date -Format yyyy-MM-dd) \
  --output table
```

**檢查項**:
- [ ] 成本在預算範圍內
- [ ] 沒有意外的高成本資源
- [ ] 開發環境使用適當的 SKU

### 配置監控

```powershell
# 檢查診斷設置
az monitor diagnostic-settings list \
  --resource <resource-id> \
  --output table
```

**檢查項**:
- [ ] 診斷設置已配置 (非 dev 環境)
- [ ] 日誌保留策略正確
- [ ] 指標收集已啟用

## 故障排除

### 部署失敗

如果部署失敗,檢查:

1. **查看部署日誌**:
   ```powershell
   az deployment sub show --name <deployment-name>
   ```

2. **常見錯誤**:
   - QuotaExceeded: 請求增加配額或更換區域
   - InvalidTemplate: 檢查 Bicep 語法
   - AuthorizationFailed: 檢查權限
   - ResourceNotFound: 檢查依賴資源是否存在

3. **清理失敗部署**:
   ```powershell
   az group delete --name rg-skagentic-dev --yes --no-wait
   ```

### 連接問題

如果無法連接到服務:

1. **檢查防火牆規則**
2. **檢查網絡安全組 (NSG)**
3. **檢查服務端點/私有端點配置**
4. **檢查 DNS 解析**
5. **檢查憑證和密鑰**

## 簽核

部署完成後,由以下角色簽核:

- [ ] **DevOps Engineer**: 基礎設施部署成功
- [ ] **Backend Lead**: 數據庫和服務可訪問
- [ ] **Security Lead**: 安全配置符合要求
- [ ] **Project Manager**: 成本在預算範圍內

---

**部署日期**: _________________
**部署人員**: _________________
**環境**: _________________
**部署名稱**: _________________
