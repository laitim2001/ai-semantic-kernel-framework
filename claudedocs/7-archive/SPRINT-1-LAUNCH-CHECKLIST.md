# Sprint 1 啟動準備清單

**狀態**: 🟡 準備中
**預計啟動時間**: Sprint 0 完成後 (當前狀態: ✅ 已完成)
**負責人**: DevOps Team + 開發團隊
**最後更新**: 2025-01-03

---

## 📋 執行摘要

Sprint 0 已於 2025-01-03 完成，所有基礎設施和開發環境已就緒。本清單涵蓋啟動 Sprint 1 所需的所有配置和驗證步驟。

**關鍵里程碑**:
- ✅ Sprint 0 完成 (100% 交付物完成)
- 🟡 GitHub 環境配置 (待執行)
- 🟡 Azure 基礎設施部署 (待執行)
- 🟡 CI/CD Pipeline 測試 (待執行)
- ⏳ Sprint 1 開發啟動 (待啟動)

---

## 🎯 Phase 1: GitHub 配置 (預計 2 小時)

### 1.1 創建 Azure Service Principals

**Development 環境**:
```powershell
# 創建 Development Service Principal
az ad sp create-for-rbac `
  --name "sp-skagentic-dev-cicd" `
  --role Contributor `
  --scopes /subscriptions/<subscription-id>/resourceGroups/rg-skagentic-dev `
  --sdk-auth

# 保存輸出的 JSON，將作為 AZURE_CREDENTIALS_DEV
```

**Staging 環境**:
```powershell
# 創建 Staging Service Principal
az ad sp create-for-rbac `
  --name "sp-skagentic-staging-cicd" `
  --role Contributor `
  --scopes /subscriptions/<subscription-id>/resourceGroups/rg-skagentic-staging `
  --sdk-auth

# 保存輸出的 JSON，將作為 AZURE_CREDENTIALS_STAGING
```

**Production 環境**:
```powershell
# 創建 Production Service Principal
az ad sp create-for-rbac `
  --name "sp-skagentic-prod-cicd" `
  --role Contributor `
  --scopes /subscriptions/<subscription-id>/resourceGroups/rg-skagentic-prod `
  --sdk-auth

# 保存輸出的 JSON，將作為 AZURE_CREDENTIALS_PROD
```

**Service Principal 權限驗證**:
```powershell
# 驗證 Service Principal 可以訪問資源組
az login --service-principal `
  -u <client-id> `
  -p <client-secret> `
  --tenant <tenant-id>

az group show --name rg-skagentic-dev
```

### 1.2 配置 GitHub Secrets

**前往**: `https://github.com/<your-org>/<repo>/settings/secrets/actions`

**必需的 Secrets**:

| Secret 名稱 | 值來源 | 用途 |
|------------|--------|------|
| `AZURE_CREDENTIALS_DEV` | Step 1.1 Dev SP JSON | Development 環境部署 |
| `AZURE_CREDENTIALS_STAGING` | Step 1.1 Staging SP JSON | Staging 環境部署 |
| `AZURE_CREDENTIALS_PROD` | Step 1.1 Prod SP JSON | Production 環境部署 |
| `POSTGRES_ADMIN_PASSWORD_DEV` | 隨機生成 (16+ chars) | Dev PostgreSQL 密碼 |
| `POSTGRES_ADMIN_PASSWORD_STAGING` | 隨機生成 (16+ chars) | Staging PostgreSQL 密碼 |
| `POSTGRES_ADMIN_PASSWORD_PROD` | 隨機生成 (16+ chars) | Prod PostgreSQL 密碼 |

**可選的 Secrets** (用於增強功能):

| Secret 名稱 | 用途 | 優先級 |
|------------|------|--------|
| `SONAR_TOKEN` | SonarQube 代碼質量分析 | 🟡 Medium |
| `SNYK_TOKEN` | Snyk 安全掃描 | 🟡 Medium |
| `SLACK_WEBHOOK` | Slack 部署通知 | 🟢 Low |
| `GITLEAKS_LICENSE` | Gitleaks 專業版 (可選) | 🟢 Low |

**生成安全密碼**:
```powershell
# 生成強密碼
-join ((33..126) | Get-Random -Count 24 | ForEach-Object {[char]$_})
```

### 1.3 配置 GitHub Environments

**前往**: `https://github.com/<your-org>/<repo>/settings/environments`

#### Development Environment
- **Name**: `development`
- **Deployment branches**: `develop` 分支
- **Required reviewers**: 無
- **Wait timer**: 0 分鐘
- **Environment secrets**: 無 (使用 repository secrets)

#### Staging Environment
- **Name**: `staging`
- **Deployment branches**: `main` 分支
- **Required reviewers**: 1 人 (Tech Lead team)
- **Wait timer**: 0 分鐘
- **Prevent self-review**: ❌ 禁用 (小團隊)

#### Production Environment
- **Name**: `production`
- **Deployment branches**: `main` 分支
- **Required reviewers**: 2 人 (Tech Lead + PM)
- **Wait timer**: 5 分鐘
- **Prevent self-review**: ✅ 啟用

#### Production Traffic Switch Environment
- **Name**: `production-traffic-switch`
- **Deployment branches**: `main` 分支
- **Required reviewers**: 1 人 (Tech Lead)
- **Wait timer**: 0 分鐘
- **用途**: Blue-Green 流量切換審批

**配置團隊 Reviewer**:
```bash
# 前往 Settings > Access > Collaborators and teams
# 添加 "tech-lead-team" 和 "pm-team" 團隊
```

### 1.4 驗證 GitHub 配置

**檢查清單**:
- [ ] 所有 3 個 Azure Service Principals 已創建
- [ ] 所有 6 個必需 Secrets 已添加到 GitHub
- [ ] 所有 4 個 Environments 已配置
- [ ] Reviewer 團隊已設置
- [ ] 測試手動觸發一個 workflow (不部署，僅驗證觸發)

**手動觸發測試**:
```bash
# 使用 GitHub CLI 測試 workflow 觸發
gh workflow run ci.yml --ref develop

# 查看 workflow 執行狀態
gh run list --workflow=ci.yml --limit 1
```

---

## 🏗️ Phase 2: Azure 基礎設施部署 (預計 4-6 小時)

### 2.1 Development 環境部署

**部署順序**: Development → Staging → Production

#### Step 1: 驗證 Bicep 模板

```powershell
# 切換到 Bicep 目錄
cd infrastructure/bicep

# 驗證 Development 參數檔案
az bicep build --file main.bicep

# 驗證 Development 部署 (不執行)
az deployment sub validate `
  --location eastus `
  --template-file main.bicep `
  --parameters parameters/dev.bicepparam `
  --parameters postgresAdminPassword=$env:POSTGRES_ADMIN_PASSWORD_DEV
```

#### Step 2: 部署 Development 基礎設施

```powershell
# 部署 Development 環境
$deploymentName = "skagentic-dev-$(Get-Date -Format 'yyyyMMdd-HHmmss')"

az deployment sub create `
  --name $deploymentName `
  --location eastus `
  --template-file main.bicep `
  --parameters parameters/dev.bicepparam `
  --parameters postgresAdminPassword=$env:POSTGRES_ADMIN_PASSWORD_DEV `
  --output json | Tee-Object -FilePath "deployment-dev-output.json"
```

**預期部署時間**: 15-25 分鐘

**部署的資源**:
- ✅ Resource Group: `rg-skagentic-dev`
- ✅ Virtual Network: `vnet-skagentic-dev`
- ✅ Azure Kubernetes Service: `aks-skagentic-dev`
- ✅ Azure Container Registry: `skagentic.azurecr.io`
- ✅ Azure Database for PostgreSQL: `psql-skagentic-dev`
- ✅ Azure Cache for Redis: `redis-skagentic-dev`
- ✅ Qdrant Vector DB (ACI 或 AKS 內部署)
- ✅ Azure Key Vault: `kv-skagentic-dev`
- ✅ Log Analytics Workspace
- ✅ Application Insights

#### Step 3: 驗證部署

```powershell
# 檢查資源組
az group show --name rg-skagentic-dev --output table

# 列出所有資源
az resource list --resource-group rg-skagentic-dev --output table

# 驗證 AKS 連接
az aks get-credentials `
  --resource-group rg-skagentic-dev `
  --name aks-skagentic-dev `
  --overwrite-existing

kubectl get nodes
kubectl get namespaces

# 驗證 PostgreSQL
az postgres flexible-server show `
  --resource-group rg-skagentic-dev `
  --name psql-skagentic-dev `
  --output table

# 驗證 Redis
az redis show `
  --resource-group rg-skagentic-dev `
  --name redis-skagentic-dev `
  --output table
```

#### Step 4: 配置網絡訪問

```powershell
# 配置 PostgreSQL 防火牆規則 (僅 Development)
az postgres flexible-server firewall-rule create `
  --resource-group rg-skagentic-dev `
  --name psql-skagentic-dev `
  --rule-name AllowDevelopmentAccess `
  --start-ip-address 0.0.0.0 `
  --end-ip-address 255.255.255.255

# 注意: Production 環境不應使用此規則，僅用於開發測試
```

#### Step 5: 初始化 Kubernetes 命名空間

```powershell
# 創建開發命名空間
kubectl create namespace skagentic-dev

# 創建 imagePullSecret (用於 ACR)
kubectl create secret docker-registry acr-secret `
  --namespace skagentic-dev `
  --docker-server=skagentic.azurecr.io `
  --docker-username=<acr-username> `
  --docker-password=<acr-password>

# 創建 ConfigMap (開發環境配置)
kubectl create configmap app-config `
  --namespace skagentic-dev `
  --from-literal=ASPNETCORE_ENVIRONMENT=Development `
  --from-literal=POSTGRES_HOST=psql-skagentic-dev.postgres.database.azure.com `
  --from-literal=REDIS_HOST=redis-skagentic-dev.redis.cache.windows.net
```

### 2.2 本地開發環境測試

**在部署 Azure 環境的同時，驗證本地開發環境**:

#### Step 1: 啟動本地服務

```powershell
# 確保在專案根目錄
cd "C:\AI Semantic Kernel"

# 啟動 Docker Compose 服務
docker-compose up -d

# 等待服務啟動
Start-Sleep -Seconds 10
```

#### Step 2: 驗證本地服務

```powershell
# 運行健康檢查腳本
.\scripts\health-check.ps1

# 預期輸出:
# ✅ PostgreSQL is healthy (localhost:5432)
# ✅ Redis is healthy (localhost:6379)
# ✅ Qdrant is healthy (localhost:6333)
```

#### Step 3: 運行集成測試 (如果已建立)

```powershell
# 當 .NET 專案建立後執行
# dotnet test --filter Category=Integration
```

### 2.3 Staging 環境部署 (可選 - Phase 3)

**僅在 Development 環境驗證成功後部署**

```powershell
# 驗證 Staging 參數
az deployment sub validate `
  --location eastus `
  --template-file main.bicep `
  --parameters parameters/staging.bicepparam `
  --parameters postgresAdminPassword=$env:POSTGRES_ADMIN_PASSWORD_STAGING

# 部署 Staging 環境
$deploymentName = "skagentic-staging-$(Get-Date -Format 'yyyyMMdd-HHmmss')"

az deployment sub create `
  --name $deploymentName `
  --location eastus `
  --template-file main.bicep `
  --parameters parameters/staging.bicepparam `
  --parameters postgresAdminPassword=$env:POSTGRES_ADMIN_PASSWORD_STAGING
```

### 2.4 部署驗證檢查清單

**Development 環境**:
- [ ] 所有 Azure 資源已成功部署
- [ ] AKS cluster 可訪問 (`kubectl get nodes`)
- [ ] PostgreSQL 可連接
- [ ] Redis 可連接
- [ ] ACR 可推送映像
- [ ] Key Vault 可訪問
- [ ] Application Insights 正在收集遙測數據
- [ ] 本地開發環境所有服務正常運行

**Staging 環境** (可選):
- [ ] 所有資源已部署
- [ ] 網絡隔離配置正確
- [ ] 無公開訪問端點 (僅內部)

---

## 🧪 Phase 3: CI/CD Pipeline 測試 (預計 1 小時)

### 3.1 測試 CI Workflow

#### Step 1: 創建測試 PR

```bash
# 創建測試分支
git checkout -b test/ci-validation

# 創建簡單的測試變更
echo "# CI Pipeline Test" > TEST-CI.md
git add TEST-CI.md
git commit -m "test: Validate CI pipeline"

# 推送到遠端
git push origin test/ci-validation

# 創建 PR (使用 GitHub CLI)
gh pr create \
  --base develop \
  --head test/ci-validation \
  --title "test: CI Pipeline Validation" \
  --body "驗證 CI pipeline 所有 jobs 正常執行"
```

#### Step 2: 監控 CI Execution

```bash
# 查看 workflow 執行狀態
gh run watch

# 或在瀏覽器中查看
# https://github.com/<your-org>/<repo>/actions
```

**預期 Jobs**:
- ✅ `code-quality`: .NET build 和 test
- ✅ `docker-compose-validation`: 服務健康檢查
- ✅ `bicep-validation`: 基礎設施驗證
- ✅ `security-scan`: Trivy 掃描
- ✅ `docker-build-test`: Docker 映像構建測試
- ✅ `build-summary`: 摘要報告

#### Step 3: 檢查 Security Tab

```bash
# 前往 Security > Code scanning alerts
# 驗證 Trivy 掃描結果已上傳
```

### 3.2 測試 Development 環境部署

#### Step 1: 合併測試 PR 到 develop

```bash
# 審查 PR 並合併
gh pr merge test/ci-validation --squash --delete-branch

# 或在 GitHub UI 中合併
```

#### Step 2: 監控 CD-Dev Workflow

```bash
# 查看 CD workflow 執行
gh run list --workflow=cd-dev.yml --limit 1

# 監控執行
gh run watch <run-id>
```

**預期 Jobs**:
- ✅ `deploy-infrastructure`: Bicep 部署 (如果首次)
- ✅ `deploy-application`: Kubernetes 部署
- ✅ `smoke-tests`: 健康檢查

#### Step 3: 驗證部署

```bash
# 連接到 Development AKS
az aks get-credentials \
  --resource-group rg-skagentic-dev \
  --name aks-skagentic-dev \
  --overwrite-existing

# 檢查 pods (如果應用已部署)
kubectl get pods -n skagentic-dev

# 檢查 services
kubectl get services -n skagentic-dev
```

### 3.3 測試 Dependabot

**Dependabot 會自動運行，但可以觸發手動檢查**:

```bash
# 前往 Insights > Dependency graph > Dependabot
# 點擊 "Check for updates" 觸發立即檢查
```

**預期行為**:
- 每週一 09:00 (Asia/Taipei): .NET 依賴項檢查
- 每週一 09:00 (Asia/Taipei): npm 依賴項檢查
- 每週二 09:00 (Asia/Taipei): Docker 映像更新檢查
- 每週三 09:00 (Asia/Taipei): GitHub Actions 版本更新

### 3.4 測試 Security Scan Workflow

```bash
# 手動觸發 security scan
gh workflow run security-scan.yml

# 監控執行
gh run watch
```

**預期 Jobs**:
- ✅ `dependency-scan`: .NET 和 npm audit
- ✅ `trivy-scan`: 文件系統掃描
- ✅ `secrets-scan`: Gitleaks 掃描
- ✅ `codeql-analysis`: C# 和 JavaScript 分析
- ✅ `infrastructure-scan`: Bicep Checkov 掃描
- ✅ `security-summary`: 摘要報告

### 3.5 CI/CD 驗證檢查清單

- [ ] CI workflow 在 PR 上成功執行
- [ ] 所有 CI jobs 通過 (code-quality, validation, security)
- [ ] Security 掃描結果上傳到 GitHub Security tab
- [ ] CD-Dev workflow 在合併到 develop 後自動觸發
- [ ] Development 環境成功部署 (或模擬成功)
- [ ] Dependabot 配置正確，可以檢測到更新
- [ ] Security scan workflow 可以手動觸發並成功執行
- [ ] Workflow 摘要清晰易讀

---

## 🚀 Phase 4: Sprint 1 開發啟動 (預計 30 分鐘)

### 4.1 Sprint 1 規劃會議準備

**前置工作**:
1. 審查 Sprint 1-2 Roadmap 文檔
2. 確認第一個 User Story: **US-1.1 Agent CRUD API**
3. 準備開發環境

**會議議程**:
- Sprint 0 成果回顧 (10 分鐘)
- Sprint 1 目標確認 (10 分鐘)
- User Story 分解和估算 (30 分鐘)
- 任務分配 (10 分鐘)

### 4.2 開發環境最終驗證

```powershell
# 驗證本地環境
cd "C:\AI Semantic Kernel"

# 確認 Docker 服務運行
docker-compose ps

# 確認 Git 狀態
git status
git branch

# 確認遠端連接
git fetch --all
```

### 4.3 創建 Sprint 1 Feature 分支

```bash
# 從 develop 創建 feature 分支
git checkout develop
git pull origin develop

# 創建第一個 feature 分支
git checkout -b feature/us-1.1-agent-crud-api

# 推送到遠端
git push -u origin feature/us-1.1-agent-crud-api
```

### 4.4 Sprint 1 啟動檢查清單

**開發環境**:
- [ ] 本地 Docker 服務運行正常
- [ ] Git 配置正確，feature 分支已創建
- [ ] IDE/編輯器配置完成 (VS Code/Visual Studio)
- [ ] .NET SDK 9.0 已安裝並可用
- [ ] Azure CLI 已登入並配置正確

**Azure 環境**:
- [ ] Development 環境已部署並可訪問
- [ ] AKS credentials 已配置
- [ ] ACR 可推送映像

**CI/CD**:
- [ ] GitHub Secrets 已配置
- [ ] GitHub Environments 已設置
- [ ] CI workflow 已驗證
- [ ] CD-Dev workflow 已測試

**文檔**:
- [ ] Sprint 1-2 Roadmap 已審查
- [ ] User Story 1.1 已理解
- [ ] API 設計文檔已準備 (docs/technical-implementation/5-api-design/)

---

## 📊 時間線和里程碑

### 預期時間線

| Phase | 任務 | 預估時間 | 累計時間 |
|-------|------|----------|----------|
| **Phase 1** | GitHub 配置 | 2 小時 | 2 小時 |
| **Phase 2** | Azure 部署 (Dev) | 4-6 小時 | 6-8 小時 |
| **Phase 3** | CI/CD 測試 | 1 小時 | 7-9 小時 |
| **Phase 4** | Sprint 1 啟動 | 30 分鐘 | 7.5-9.5 小時 |
| **總計** | - | **7.5-9.5 小時** | - |

### 建議執行順序

**Day 1 (2-3 小時)**:
- ✅ Phase 1: GitHub 配置完成
- ✅ Phase 2.1: Development 環境部署開始

**Day 2 (4-5 小時)**:
- ✅ Phase 2: Azure 部署完成並驗證
- ✅ Phase 3: CI/CD 測試完成

**Day 3 (1 小時)**:
- ✅ Phase 4: Sprint 1 正式啟動
- ✅ 開始 User Story 1.1 開發

### 關鍵決策點

**Decision Point 1: Azure 環境範圍**
- **選項 A**: 僅部署 Development 環境 (推薦)
- **選項 B**: 同時部署 Dev + Staging
- **選項 C**: 完整三環境部署

**建議**: 選項 A，先驗證 Dev 環境穩定後再擴展

**Decision Point 2: CI/CD 測試深度**
- **選項 A**: 最小驗證 (CI + CD-Dev)
- **選項 B**: 完整驗證 (所有 workflows)

**建議**: 選項 A，Production workflows 等實際需要時測試

**Decision Point 3: Sprint 1 啟動時機**
- **選項 A**: 等待所有配置完成
- **選項 B**: 並行開始開發 (本地環境)

**建議**: 選項 B，本地開發可以先行，部署稍後

---

## 🚨 風險和緩解措施

### 風險 1: Azure Service Principal 權限不足
**影響**: 無法部署 Azure 資源
**緩解**:
- 使用 Owner 或 Contributor 角色
- 驗證 Service Principal 權限: `az role assignment list --assignee <sp-client-id>`

### 風險 2: Azure 資源配額限制
**影響**: 部署失敗
**緩解**:
- 提前檢查訂閱配額
- 申請配額增加 (如需要)
- 使用較小的 SKU 進行初始部署

### 風險 3: GitHub Actions 執行配額
**影響**: CI/CD 執行受限
**緩解**:
- 監控 GitHub Actions 使用量
- 優化 workflow 執行效率
- 考慮自託管 runners (如需要)

### 風險 4: 網絡連接問題
**影響**: 無法訪問 Azure 資源
**緩解**:
- 配置防火牆規則
- 使用 Azure Bastion 或 VPN (Staging/Prod)
- Development 環境允許廣泛訪問 (測試期間)

### 風險 5: 配置錯誤導致安全問題
**影響**: 潛在安全漏洞
**緩解**:
- 所有密碼使用強隨機生成
- 定期輪換密鑰和密碼
- 啟用 Azure Security Center 建議
- 運行安全掃描並修復高危問題

---

## ✅ 成功標準

### Phase 1 成功標準
- ✅ 所有 3 個 Azure Service Principals 已創建並驗證
- ✅ 所有 6 個 GitHub Secrets 已配置
- ✅ 所有 4 個 GitHub Environments 已設置並測試
- ✅ 可以手動觸發 workflow 且成功運行

### Phase 2 成功標準
- ✅ Development 環境所有 Azure 資源已部署
- ✅ AKS cluster 可訪問，nodes 正常運行
- ✅ PostgreSQL 可連接並創建數據庫
- ✅ Redis 可連接並執行基本操作
- ✅ ACR 可推送和拉取映像
- ✅ 本地開發環境所有服務健康

### Phase 3 成功標準
- ✅ CI workflow 在 PR 上自動執行並通過
- ✅ CD-Dev workflow 在 develop 分支合併後自動部署
- ✅ Security scan workflow 可手動觸發並生成報告
- ✅ Dependabot 可以檢測並創建更新 PR
- ✅ 所有 workflow 摘要清晰且有用

### Phase 4 成功標準
- ✅ Sprint 1 規劃會議完成
- ✅ User Story 1.1 已分解為可執行任務
- ✅ Feature 分支已創建並推送
- ✅ 開發團隊準備開始編碼

---

## 📚 參考文檔

### Sprint 0 完成文檔
- ✅ `claudedocs/SPRINT-0-COMPLETION-REPORT.md` - 完整的 Sprint 0 成果總結
- ✅ `QUICK-START-GUIDE.md` - 快速上手指南
- ✅ `.github/README.md` - CI/CD Pipeline 文檔

### Azure 部署文檔
- ✅ `infrastructure/bicep/README.md` - Bicep 模板說明
- ✅ `infrastructure/bicep/parameters/*.bicepparam` - 環境參數
- ✅ `docs/technical-implementation/8-deployment-architecture/` - 部署架構設計

### Sprint 1 規劃文檔
- ✅ `docs/project-management/sprint-planning/sprint-1-2-roadmap.md` - Sprint 1-2 路線圖
- ✅ `docs/user-stories/us-1.1-agent-crud-api.md` - User Story 1.1 詳情
- ✅ `docs/technical-implementation/5-api-design/` - API 設計標準

### 故障排除文檔
- ✅ `.github/README.md` - "故障排除" 章節
- ✅ `docs/technical-implementation/9-security-standards/` - 安全最佳實踐
- ✅ `docker/README.md` - Docker 環境故障排除

---

## 🤝 團隊協作

### 角色和職責

**DevOps Team** (Phase 1-3):
- 創建 Azure Service Principals
- 配置 GitHub Secrets 和 Environments
- 部署 Azure 基礎設施
- 驗證 CI/CD pipelines

**Backend Team** (Phase 4):
- 審查 API 設計文檔
- 開始 User Story 1.1 開發
- 編寫單元測試和集成測試

**Frontend Team** (Phase 4):
- 審查 UI/UX 設計
- 準備前端開發環境
- 等待 API 就緒

**Tech Lead**:
- 審批 Staging/Production 部署
- 審查架構決策
- 協調跨團隊工作

### 溝通計劃

**每日站會** (15 分鐘):
- 配置和部署進度更新
- 阻塞問題討論
- 當日目標確認

**每週回顧** (60 分鐘):
- Sprint 1 進度評估
- 持續改進討論
- 下週計劃調整

---

## 📞 聯繫和支持

### 關鍵聯繫人
- **DevOps Lead**: [配置和部署問題]
- **Tech Lead**: [架構和技術決策]
- **PM**: [Sprint 規劃和優先級]

### 支持資源
- **GitHub Discussions**: 團隊協作和問題討論
- **Azure Support**: 基礎設施問題
- **Documentation Wiki**: 內部知識庫

---

**最後更新**: 2025-01-03
**版本**: 1.0
**維護者**: DevOps Team + Tech Lead
**審查週期**: Sprint 1 啟動後每週更新
