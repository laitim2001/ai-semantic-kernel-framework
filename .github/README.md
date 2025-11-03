# CI/CD Pipeline 配置

本目錄包含 Semantic Kernel Agentic Framework 的完整 CI/CD pipeline 配置。

## 📁 目錄結構

```
.github/
├── workflows/
│   ├── ci.yml                    # 持續整合 (CI) 主流程
│   ├── cd-dev.yml                # Development 環境部署
│   ├── cd-staging.yml            # Staging 環境部署
│   ├── cd-production.yml         # Production 環境部署 (Blue-Green)
│   └── security-scan.yml         # 定期安全掃描
├── dependabot.yml                # 自動化依賴項更新
└── README.md                     # 本文檔
```

## 🔄 CI/CD 流程概覽

### Continuous Integration (CI)

**觸發條件**: Push 或 Pull Request 到 `develop`, `main`, `master` 分支

**執行內容**:
1. ✅ 代碼質量檢查 (.NET Build + Test)
2. ✅ Docker Compose 驗證 (PostgreSQL, Redis, Qdrant)
3. ✅ Bicep 模板驗證
4. ✅ 安全掃描 (Trivy)
5. ✅ Docker 構建測試

### Continuous Deployment (CD)

#### Development 環境
- **觸發**: CI 成功 (develop 分支) 或手動觸發
- **審批**: 無需審批
- **部署**: 自動部署到 Azure Dev 環境
- **測試**: 基本健康檢查

#### Staging 環境
- **觸發**: Push 到 main 分支或手動觸發
- **審批**: Tech Lead 審批
- **部署**: 自動部署到 Azure Staging 環境
- **測試**:
  - E2E 測試
  - 性能測試 (k6)
  - 安全測試 (OWASP ZAP)

#### Production 環境
- **觸發**: GitHub Release (Tag) 或手動觸發
- **審批**: Tech Lead + PM 雙重審批
- **部署策略**: Blue-Green 部署
  1. 部署 Green 環境
  2. 健康檢查
  3. Canary 流量切換 (10%)
  4. 監控 5 分鐘
  5. 完全切換 (100%)
  6. 清理 Blue 環境 (30 分鐘後)
- **測試**: 關鍵路徑煙霧測試

## 🔐 必要的 GitHub Secrets

### Azure Credentials

所有環境都需要 Azure Service Principal 憑證:

```bash
# Development
AZURE_CREDENTIALS_DEV

# Staging
AZURE_CREDENTIALS_STAGING

# Production
AZURE_CREDENTIALS_PROD
```

**格式** (JSON):
```json
{
  "clientId": "<service-principal-client-id>",
  "clientSecret": "<service-principal-secret>",
  "subscriptionId": "<azure-subscription-id>",
  "tenantId": "<azure-tenant-id>"
}
```

### Database Passwords

```bash
POSTGRES_ADMIN_PASSWORD_DEV
POSTGRES_ADMIN_PASSWORD_STAGING
POSTGRES_ADMIN_PASSWORD_PROD
```

### 其他 Secrets (可選)

```bash
# SonarQube (代碼質量)
SONAR_TOKEN

# Snyk (安全掃描)
SNYK_TOKEN

# Slack (通知)
SLACK_WEBHOOK

# Gitleaks (密鑰掃描)
GITLEAKS_LICENSE
```

## 🚀 設置 Azure Service Principal

```bash
# 創建 Service Principal (Development)
az ad sp create-for-rbac \
  --name "sp-skagentic-dev-cicd" \
  --role Contributor \
  --scopes /subscriptions/<subscription-id>/resourceGroups/rg-skagentic-dev \
  --sdk-auth

# 創建 Service Principal (Staging)
az ad sp create-for-rbac \
  --name "sp-skagentic-staging-cicd" \
  --role Contributor \
  --scopes /subscriptions/<subscription-id>/resourceGroups/rg-skagentic-staging \
  --sdk-auth

# 創建 Service Principal (Production)
az ad sp create-for-rbac \
  --name "sp-skagentic-prod-cicd" \
  --role Contributor \
  --scopes /subscriptions/<subscription-id>/resourceGroups/rg-skagentic-prod \
  --sdk-auth
```

將輸出的 JSON 存儲到對應的 GitHub Secrets。

## 🎯 GitHub Environments 配置

在 GitHub Repository Settings → Environments 中配置:

### Development
- **Deployment branches**: `develop`
- **Required reviewers**: 無
- **Wait timer**: 0 分鐘

### Staging
- **Deployment branches**: `main`
- **Required reviewers**: 1 (Tech Lead team)
- **Wait timer**: 0 分鐘

### Production
- **Deployment branches**: `main`
- **Required reviewers**: 2 (Tech Lead + PM)
- **Wait timer**: 5 分鐘
- **Prevent self-review**: 啟用

### Production Traffic Switch
- **Deployment branches**: `main`
- **Required reviewers**: 1 (Tech Lead)
- **Wait timer**: 0 分鐘

## 📝 使用指南

### 開發流程

1. **開發新功能**:
   ```bash
   git checkout -b feature/new-feature develop
   # 開發代碼...
   git commit -m "feat: add new feature"
   git push origin feature/new-feature
   ```

2. **創建 Pull Request**:
   - 針對 `develop` 分支
   - CI 自動執行
   - 代碼審查通過後合併

3. **部署到 Development**:
   - 合併到 `develop` 後自動部署
   - 無需手動操作

### 發布到 Staging

```bash
# 從 develop 創建 PR 到 main
git checkout main
git pull origin main
git merge develop
git push origin main
```

### 發布到 Production

```bash
# 創建 Git Tag
git tag -a v1.0.0 -m "Release version 1.0.0"
git push origin v1.0.0

# 在 GitHub 上創建 Release
# 1. 前往 Releases 頁面
# 2. 點擊 "Draft a new release"
# 3. 選擇 tag: v1.0.0
# 4. 填寫 Release notes
# 5. 點擊 "Publish release"

# Production 部署自動開始
```

### 手動部署

```bash
# 使用 GitHub CLI 手動觸發部署
gh workflow run cd-dev.yml
gh workflow run cd-staging.yml
gh workflow run cd-production.yml -f version=v1.0.0
```

## 🧪 測試策略

### CI 階段
- ✅ 單元測試 (.NET xUnit)
- ✅ 代碼覆蓋率 (>80%)
- ✅ 靜態分析 (SonarQube)
- ✅ 安全掃描 (Trivy, Snyk)

### Staging 階段
- ✅ 整合測試
- ✅ E2E 測試 (Playwright)
- ✅ 性能測試 (k6)
- ✅ 安全測試 (OWASP ZAP)

### Production 階段
- ✅ 煙霧測試 (關鍵路徑)
- ✅ 健康檢查
- ✅ 金絲雀部署監控

## 📊 監控與告警

### Workflow 狀態

在 GitHub Actions 頁面查看:
- 成功/失敗狀態
- 執行時間
- 部署日誌

### 部署摘要

每次部署完成後會在 GitHub Actions Summary 中生成報告:
- 部署狀態
- 環境詳情
- 測試結果
- 相關 URL

## 🔧 故障排除

### CI 失敗

1. **Docker Compose 服務無法啟動**:
   - 檢查 `docker-compose.yml` 配置
   - 確認 ports 沒有衝突

2. **Bicep 驗證失敗**:
   - 運行 `az bicep build --file main.bicep` 本地驗證
   - 檢查參數文件格式

3. **安全掃描失敗**:
   - 查看 Security tab 詳細報告
   - 更新有漏洞的依賴項

### CD 失敗

1. **Azure 認證失敗**:
   - 驗證 Service Principal 權限
   - 檢查 Secret 格式是否正確

2. **AKS 部署失敗**:
   - 檢查 AKS cluster 是否存在
   - 確認 namespace 配置正確

3. **健康檢查失敗**:
   - 查看 Pod 日誌: `kubectl logs <pod-name>`
   - 檢查服務端點是否正確

## 📚 相關文檔

- [GitHub Actions 文檔](https://docs.github.com/en/actions)
- [Azure Bicep 文檔](https://learn.microsoft.com/azure/azure-resource-manager/bicep/)
- [Kubernetes 部署文檔](../docs/technical-implementation/8-deployment-architecture/)
- [安全最佳實踐](../docs/technical-implementation/9-security-standards/)

## 🤝 貢獻指南

修改 CI/CD 配置時:

1. 在本地測試 workflow 語法
2. 在 feature branch 測試變更
3. 提交 PR 並請求 DevOps team 審查
4. 合併後監控首次執行

---

**維護者**: DevOps Team
**最後更新**: 2025-01-03
**版本**: Sprint 0 (初始版本)
