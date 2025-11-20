# IPA Platform - Infrastructure

此目錄包含 IPA Platform 的基礎設施即代碼 (Infrastructure as Code) 配置。

---

## 📁 目錄結構

```
infrastructure/
├── azure/
│   ├── bicep/                    # Azure Bicep 模板 (IaC)
│   │   ├── main.bicep           # 主要部署模板
│   │   └── modules/             # 可重用模塊
│   ├── scripts/                  # 部署腳本
│   │   ├── deploy-staging.sh   # Staging 環境部署
│   │   └── deploy-production.sh # Production 環境部署
│   └── config/                   # 配置文件
└── README.md                     # 本文件
```

---

## 🚀 快速開始

### 前置條件

1. **Azure CLI** 已安裝並登入
   ```bash
   az login
   az account set --subscription "<your-subscription-id>"
   ```

2. **權限**: 訂閱的 Contributor 或 Owner 角色

---

### 方式 1: 使用自動化腳本 (推薦初次部署)

#### 部署 Staging 環境

```bash
cd azure/scripts
chmod +x deploy-staging.sh
./deploy-staging.sh
```

這個腳本會:
- ✅ 創建所有必需的 Azure 資源
- ✅ 配置網絡和安全設置
- ✅ 將 secrets 存儲到 Key Vault
- ✅ 配置 App Service Managed Identity
- ✅ 大約需要 15-20 分鐘

#### 部署 Production 環境

```bash
chmod +x deploy-production.sh
./deploy-production.sh
```

---

### 方式 2: 使用 Azure Bicep

```bash
# 部署 Staging
az deployment sub create \
  --name ipa-staging-deployment \
  --location eastus \
  --template-file azure/bicep/main.bicep \
  --parameters \
    environment=staging \
    location=eastus \
    postgresAdminUsername=ipaadmin \
    postgresAdminPassword='<your-strong-password>'

# 查看部署結果
az deployment sub show \
  --name ipa-staging-deployment \
  --query properties.outputs
```

---

## 📋 部署後配置

### 1. 配置 GitHub Secrets

前往 GitHub Repository Settings → Secrets → Actions，添加:

| Secret 名稱 | 獲取方式 |
|------------|---------|
| `AZURE_CREDENTIALS_STAGING` | 運行腳本後查看輸出 |
| `AZURE_KEYVAULT_NAME` | 從部署輸出獲取 |

### 2. 測試部署

```bash
# Health Check
curl https://app-ipa-backend-staging.azurewebsites.net/health

# 預期輸出: {"status":"healthy","version":"0.1.1"}
```

### 3. 查看日誌

```bash
az webapp log tail \
  --name app-ipa-backend-staging \
  --resource-group rg-ipa-staging-eastus
```

---

## 🏗️ 部署的資源

### Staging Environment (~$85/month)

- App Service Plan (B1)
- 2x Web Apps (Backend + Frontend)
- PostgreSQL Flexible Server (B1ms)
- Azure Cache for Redis (C1, shared)
- Service Bus Namespace (Standard)
- Key Vault (Standard)
- Storage Account (Standard LRS)
- Application Insights
- Log Analytics Workspace

### Production Environment (~$315-365/month)

- App Service Plan (P1V2 with auto-scaling)
- 2x Web Apps (Backend + Frontend) with deployment slots
- PostgreSQL Flexible Server (GP_Gen5_2)
- Azure Cache for Redis (C1, shared)
- Service Bus Namespace (Standard)
- Key Vault (Standard)
- Storage Account (Standard LRS)
- Application Insights
- Log Analytics Workspace
- Azure OpenAI Service (pay-as-you-go)

---

## 🔐 安全最佳實踐

### Secrets 管理

所有敏感信息都存儲在 Azure Key Vault 中:
- ✅ Database connection strings
- ✅ Redis connection strings
- ✅ Service Bus connection strings
- ✅ JWT secret keys
- ✅ API keys

App Service 使用 **Managed Identity** 訪問 Key Vault，無需在代碼中硬編碼任何密鑰。

### 網絡安全

- ✅ 所有服務強制 HTTPS
- ✅ 最低 TLS 1.2
- ✅ PostgreSQL firewall 僅允許 App Service IPs
- ✅ Storage Account 禁用公共訪問
- ✅ Production 使用 VNet integration (P1V2 plan)

---

## 📊 成本優化建議

1. **Staging 降級**: B1 → Free tier (F1) = 節省 $13/month
2. **Redis 共用**: 兩環境共用一個 C1 instance
3. **PostgreSQL Dev/Test**: 使用 Dev/Test 定價 = 節省 15%
4. **Reserved Capacity**: 預付 1-3 年 = 節省 30-50%
5. **Auto-shutdown**: Staging 環境非工作時間自動關閉

**優化後成本**: ~$300-350/month (Staging + Production)

---

## 🔄 更新基礎設施

### 修改資源配置

1. 編輯 Bicep 文件或腳本
2. 重新運行部署命令
3. Azure 會自動檢測變更並更新資源

### 刪除環境

```bash
# ⚠️ 危險操作 - 會刪除所有資源！
az group delete --name rg-ipa-staging-eastus --yes --no-wait
```

---

## 🆘 故障排除

### 常見問題

**Q: 部署失敗，提示資源名稱已存在**
```bash
# 檢查是否有殘留資源
az group list --query "[?contains(name,'ipa')]"

# 刪除殘留資源
az group delete --name <resource-group-name>
```

**Q: PostgreSQL 連接失敗**
```bash
# 檢查 firewall 規則
az postgres flexible-server firewall-rule list \
  --resource-group rg-ipa-staging-eastus \
  --name psql-ipa-staging-eastus

# 添加你的 IP
az postgres flexible-server firewall-rule create \
  --resource-group rg-ipa-staging-eastus \
  --name psql-ipa-staging-eastus \
  --rule-name AllowMyIP \
  --start-ip-address <your-ip> \
  --end-ip-address <your-ip>
```

**Q: App Service 無法訪問 Key Vault**
```bash
# 確認 Managed Identity 已啟用
az webapp identity show \
  --name app-ipa-backend-staging \
  --resource-group rg-ipa-staging-eastus

# 確認 Key Vault access policy
az keyvault show \
  --name <your-keyvault-name> \
  --query properties.accessPolicies
```

---

## 📚 相關文檔

- [Azure Architecture Design](../docs/03-implementation/azure-architecture-design.md) - 完整的架構設計文檔
- [Deployment Guide](../docs/03-implementation/deployment-guide.md) - 詳細的部署指南
- [Sprint Status](../docs/03-implementation/sprint-status.yaml) - 項目進度追蹤

---

## 🤝 貢獻

如果你改進了部署腳本或發現問題，請:
1. 創建 issue 描述問題或建議
2. 提交 PR 包含你的改進
3. 更新相關文檔

---

**維護者**: DevOps Team
**最後更新**: 2025-11-20
