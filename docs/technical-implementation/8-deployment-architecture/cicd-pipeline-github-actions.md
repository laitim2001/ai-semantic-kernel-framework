# CI/CD Pipeline - GitHub Actions 實施指南

## 概述

### 文檔目的
本文檔提供 AI Agent 工作流平台的完整 CI/CD Pipeline 實施指南,基於 GitHub Actions 構建自動化部署流程。涵蓋:
- **多環境部署策略** (Development, Staging, Production)
- **自動化測試集成** (單元測試、整合測試、E2E 測試)
- **Docker 映像構建與發布**
- **Azure Kubernetes Service (AKS) 部署**
- **安全掃描與合規性檢查**
- **部署門控與審批流程**

### 技術背景
CI/CD (Continuous Integration/Continuous Deployment) 是現代軟件交付的核心實踐:
- **持續集成**: 自動構建、測試、驗證代碼變更
- **持續部署**: 自動將通過驗證的代碼部署到目標環境
- **基礎設施即代碼**: YAML 配置管理部署流程
- **GitOps 理念**: Git 作為單一事實來源,所有變更通過 Pull Request

**GitHub Actions 優勢**:
- 與 GitHub 無縫集成,無需額外工具
- 豐富的 Marketplace Actions 生態系統
- 靈活的 workflow 編排能力
- 內建 Secrets 管理和環境隔離
- 支持矩陣構建和並行執行

---

## CI/CD 架構設計

### 整體流程圖

```
┌─────────────┐
│ Git Push    │
│ Pull Request│
└──────┬──────┘
       │
       v
┌─────────────────────────────────────────┐
│  Continuous Integration (CI)            │
├─────────────────────────────────────────┤
│ 1. Code Checkout                        │
│ 2. Dependencies Restore (.NET + npm)    │
│ 3. Build (Backend + Frontend)           │
│ 4. Unit Tests + Code Coverage           │
│ 5. Integration Tests (TestContainers)   │
│ 6. Static Analysis (SonarQube)          │
│ 7. Security Scan (Snyk + Trivy)         │
│ 8. Docker Build + Push to ACR           │
└──────┬──────────────────────────────────┘
       │
       v
┌─────────────────────────────────────────┐
│  Continuous Deployment (CD)             │
├─────────────────────────────────────────┤
│ Dev Environment (Auto Deploy)           │
│  → Deploy to AKS Dev Cluster            │
│  → Smoke Tests                          │
│                                         │
│ Staging Environment (Manual Approval)   │
│  → Deploy to AKS Staging Cluster        │
│  → E2E Tests (Playwright)               │
│  → Performance Tests (k6)               │
│  → Security Tests (OWASP ZAP)           │
│                                         │
│ Production Environment (Manual Approval)│
│  → Blue-Green Deployment                │
│  → Health Checks                        │
│  → Gradual Traffic Shift (Canary)       │
│  → Rollback on Failure                  │
└─────────────────────────────────────────┘
```

### 環境策略

| 環境 | 觸發條件 | 部署方式 | 測試範圍 | 審批要求 |
|------|----------|----------|----------|----------|
| **Development** | Push to `develop` branch | 自動部署 | 單元測試 + 整合測試 | 無 |
| **Staging** | Push to `main` branch | 手動審批 | 完整測試套件 (E2E + 性能 + 安全) | Tech Lead 審批 |
| **Production** | GitHub Release (Tag) | 手動審批 | 生產煙霧測試 + 健康檢查 | PM + Tech Lead 雙審批 |

---

## GitHub Actions Workflows 實施

### 目錄結構

```
.github/
├── workflows/
│   ├── ci.yml                          # 主 CI Workflow
│   ├── cd-dev.yml                      # Dev 環境部署
│   ├── cd-staging.yml                  # Staging 環境部署
│   ├── cd-production.yml               # Production 環境部署
│   ├── security-scan.yml               # 定期安全掃描
│   └── dependency-update.yml           # Dependabot 自動更新
├── actions/
│   ├── dotnet-build/                   # 自定義 Action: .NET 構建
│   ├── docker-build-push/              # 自定義 Action: Docker 構建推送
│   └── deploy-to-aks/                  # 自定義 Action: AKS 部署
└── dependabot.yml                      # Dependabot 配置
```

### CI Workflow: `.github/workflows/ci.yml`

```yaml
name: Continuous Integration

on:
  push:
    branches: [ develop, main ]
  pull_request:
    branches: [ develop, main ]
  workflow_dispatch:

env:
  DOTNET_VERSION: '9.0.x'
  NODE_VERSION: '20.x'
  DOCKER_REGISTRY: aiagentplatform.azurecr.io
  SONAR_PROJECT_KEY: ai-agent-platform

jobs:
  # ==================== 代碼質量檢查 ====================
  code-quality:
    name: Code Quality Analysis
    runs-on: ubuntu-latest

    steps:
      - name: Checkout Code
        uses: actions/checkout@v4
        with:
          fetch-depth: 0  # 完整歷史記錄用於 SonarQube 分析

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: ${{ env.DOTNET_VERSION }}

      - name: Restore Dependencies
        run: dotnet restore src/AIAgentPlatform.sln

      - name: SonarQube Scan Begin
        uses: SonarSource/sonarcloud-github-action@master
        env:
          GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
          SONAR_TOKEN: ${{ secrets.SONAR_TOKEN }}
        with:
          args: >
            -Dsonar.projectKey=${{ env.SONAR_PROJECT_KEY }}
            -Dsonar.organization=your-org
            -Dsonar.cs.opencover.reportsPaths=**/coverage.opencover.xml
            -Dsonar.coverage.exclusions=**/*Tests/**,**/Migrations/**

      - name: Build Solution
        run: dotnet build src/AIAgentPlatform.sln --configuration Release --no-restore

      - name: Run Unit Tests with Coverage
        run: |
          dotnet test src/AIAgentPlatform.sln \
            --configuration Release \
            --no-build \
            --logger "trx;LogFileName=test-results.trx" \
            --collect:"XPlat Code Coverage" \
            --results-directory ./TestResults \
            -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover

      - name: SonarQube Scan End
        uses: SonarSource/sonarcloud-github-action@master
        env:
          GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
          SONAR_TOKEN: ${{ secrets.SONAR_TOKEN }}

      - name: Upload Test Results
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: test-results
          path: ./TestResults/**/*.trx

      - name: Upload Coverage Reports
        uses: actions/upload-artifact@v4
        with:
          name: coverage-reports
          path: ./TestResults/**/coverage.opencover.xml

  # ==================== 整合測試 ====================
  integration-tests:
    name: Integration Tests
    runs-on: ubuntu-latest
    needs: code-quality

    services:
      postgres:
        image: postgres:16-alpine
        env:
          POSTGRES_USER: testuser
          POSTGRES_PASSWORD: testpass
          POSTGRES_DB: aiagent_test
        options: >-
          --health-cmd pg_isready
          --health-interval 10s
          --health-timeout 5s
          --health-retries 5
        ports:
          - 5432:5432

      redis:
        image: redis:7-alpine
        options: >-
          --health-cmd "redis-cli ping"
          --health-interval 10s
          --health-timeout 5s
          --health-retries 5
        ports:
          - 6379:6379

    steps:
      - name: Checkout Code
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: ${{ env.DOTNET_VERSION }}

      - name: Restore Dependencies
        run: dotnet restore src/AIAgentPlatform.sln

      - name: Run Integration Tests
        env:
          ConnectionStrings__DefaultConnection: "Host=localhost;Port=5432;Database=aiagent_test;Username=testuser;Password=testpass"
          ConnectionStrings__Redis: "localhost:6379"
        run: |
          dotnet test src/Tests/Integration.Tests/Integration.Tests.csproj \
            --configuration Release \
            --logger "trx;LogFileName=integration-test-results.trx" \
            --results-directory ./IntegrationTestResults

      - name: Upload Integration Test Results
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: integration-test-results
          path: ./IntegrationTestResults/**/*.trx

  # ==================== 安全掃描 ====================
  security-scan:
    name: Security Scanning
    runs-on: ubuntu-latest
    needs: code-quality

    steps:
      - name: Checkout Code
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: ${{ env.DOTNET_VERSION }}

      - name: Snyk Security Scan - .NET Dependencies
        uses: snyk/actions/dotnet@master
        env:
          SNYK_TOKEN: ${{ secrets.SNYK_TOKEN }}
        with:
          args: --severity-threshold=high --fail-on=all

      - name: Snyk Security Scan - npm Dependencies
        uses: snyk/actions/node@master
        env:
          SNYK_TOKEN: ${{ secrets.SNYK_TOKEN }}
        with:
          args: --severity-threshold=high --file=src/Web/ClientApp/package.json

      - name: Trivy Filesystem Scan
        uses: aquasecurity/trivy-action@master
        with:
          scan-type: 'fs'
          scan-ref: '.'
          format: 'sarif'
          output: 'trivy-results.sarif'
          severity: 'CRITICAL,HIGH'

      - name: Upload Trivy Results to GitHub Security
        uses: github/codeql-action/upload-sarif@v3
        with:
          sarif_file: 'trivy-results.sarif'

  # ==================== Docker 構建與推送 ====================
  docker-build-push:
    name: Docker Build and Push
    runs-on: ubuntu-latest
    needs: [code-quality, integration-tests, security-scan]
    if: github.event_name == 'push' && (github.ref == 'refs/heads/develop' || github.ref == 'refs/heads/main')

    strategy:
      matrix:
        service:
          - name: api
            context: ./src
            dockerfile: ./src/Api/Dockerfile
          - name: worker
            context: ./src
            dockerfile: ./src/Worker/Dockerfile
          - name: web
            context: ./src/Web/ClientApp
            dockerfile: ./src/Web/ClientApp/Dockerfile

    steps:
      - name: Checkout Code
        uses: actions/checkout@v4

      - name: Set up Docker Buildx
        uses: docker/setup-buildx-action@v3

      - name: Login to Azure Container Registry
        uses: docker/login-action@v3
        with:
          registry: ${{ env.DOCKER_REGISTRY }}
          username: ${{ secrets.ACR_USERNAME }}
          password: ${{ secrets.ACR_PASSWORD }}

      - name: Extract Metadata
        id: meta
        uses: docker/metadata-action@v5
        with:
          images: ${{ env.DOCKER_REGISTRY }}/${{ matrix.service.name }}
          tags: |
            type=ref,event=branch
            type=sha,prefix={{branch}}-
            type=semver,pattern={{version}}
            type=semver,pattern={{major}}.{{minor}}

      - name: Build and Push Docker Image
        uses: docker/build-push-action@v5
        with:
          context: ${{ matrix.service.context }}
          file: ${{ matrix.service.dockerfile }}
          push: true
          tags: ${{ steps.meta.outputs.tags }}
          labels: ${{ steps.meta.outputs.labels }}
          cache-from: type=registry,ref=${{ env.DOCKER_REGISTRY }}/${{ matrix.service.name }}:buildcache
          cache-to: type=registry,ref=${{ env.DOCKER_REGISTRY }}/${{ matrix.service.name }}:buildcache,mode=max
          build-args: |
            BUILD_VERSION=${{ github.sha }}
            BUILD_DATE=${{ github.event.head_commit.timestamp }}

      - name: Trivy Image Scan
        uses: aquasecurity/trivy-action@master
        with:
          image-ref: ${{ env.DOCKER_REGISTRY }}/${{ matrix.service.name }}:${{ github.sha }}
          format: 'sarif'
          output: 'trivy-image-${{ matrix.service.name }}.sarif'
          severity: 'CRITICAL,HIGH'

      - name: Upload Trivy Image Results
        uses: github/codeql-action/upload-sarif@v3
        with:
          sarif_file: 'trivy-image-${{ matrix.service.name }}.sarif'
```

### CD Workflow - Development: `.github/workflows/cd-dev.yml`

```yaml
name: Deploy to Development

on:
  workflow_run:
    workflows: ["Continuous Integration"]
    types: [completed]
    branches: [develop]
  workflow_dispatch:

env:
  ENVIRONMENT: development
  AKS_CLUSTER: aks-aiagent-dev
  AKS_RESOURCE_GROUP: rg-aiagent-dev
  NAMESPACE: aiagent-dev

jobs:
  deploy-dev:
    name: Deploy to Development Environment
    runs-on: ubuntu-latest
    if: ${{ github.event.workflow_run.conclusion == 'success' }}
    environment:
      name: development
      url: https://dev.aiagent.example.com

    steps:
      - name: Checkout Code
        uses: actions/checkout@v4

      - name: Azure Login
        uses: azure/login@v2
        with:
          creds: ${{ secrets.AZURE_CREDENTIALS_DEV }}

      - name: Get AKS Credentials
        run: |
          az aks get-credentials \
            --resource-group ${{ env.AKS_RESOURCE_GROUP }} \
            --name ${{ env.AKS_CLUSTER }} \
            --overwrite-existing

      - name: Deploy to Kubernetes
        run: |
          # 替換環境變量
          export IMAGE_TAG=${{ github.sha }}
          export ENVIRONMENT=${{ env.ENVIRONMENT }}

          # 使用 Kustomize 部署
          kubectl apply -k k8s/overlays/development

          # 等待部署完成
          kubectl rollout status deployment/api-deployment -n ${{ env.NAMESPACE }} --timeout=5m
          kubectl rollout status deployment/worker-deployment -n ${{ env.NAMESPACE }} --timeout=5m
          kubectl rollout status deployment/web-deployment -n ${{ env.NAMESPACE }} --timeout=5m

      - name: Run Smoke Tests
        run: |
          # 等待服務就緒
          kubectl wait --for=condition=ready pod -l app=api -n ${{ env.NAMESPACE }} --timeout=2m

          # 執行健康檢查
          API_URL=$(kubectl get svc api-service -n ${{ env.NAMESPACE }} -o jsonpath='{.status.loadBalancer.ingress[0].ip}')
          curl -f http://${API_URL}/health || exit 1

      - name: Notify Deployment Status
        if: always()
        uses: 8398a7/action-slack@v3
        with:
          status: ${{ job.status }}
          text: 'Development deployment ${{ job.status }}'
          webhook_url: ${{ secrets.SLACK_WEBHOOK }}
```

### CD Workflow - Staging: `.github/workflows/cd-staging.yml`

```yaml
name: Deploy to Staging

on:
  push:
    branches: [main]
  workflow_dispatch:

env:
  ENVIRONMENT: staging
  AKS_CLUSTER: aks-aiagent-staging
  AKS_RESOURCE_GROUP: rg-aiagent-staging
  NAMESPACE: aiagent-staging

jobs:
  deploy-staging:
    name: Deploy to Staging Environment
    runs-on: ubuntu-latest
    environment:
      name: staging
      url: https://staging.aiagent.example.com

    steps:
      - name: Checkout Code
        uses: actions/checkout@v4

      - name: Azure Login
        uses: azure/login@v2
        with:
          creds: ${{ secrets.AZURE_CREDENTIALS_STAGING }}

      - name: Get AKS Credentials
        run: |
          az aks get-credentials \
            --resource-group ${{ env.AKS_RESOURCE_GROUP }} \
            --name ${{ env.AKS_CLUSTER }} \
            --overwrite-existing

      - name: Deploy to Kubernetes
        run: |
          export IMAGE_TAG=${{ github.sha }}
          export ENVIRONMENT=${{ env.ENVIRONMENT }}

          kubectl apply -k k8s/overlays/staging

          kubectl rollout status deployment/api-deployment -n ${{ env.NAMESPACE }} --timeout=10m
          kubectl rollout status deployment/worker-deployment -n ${{ env.NAMESPACE }} --timeout=10m
          kubectl rollout status deployment/web-deployment -n ${{ env.NAMESPACE }} --timeout=10m

      - name: Run E2E Tests
        run: |
          # 安裝 Playwright
          npm install -g @playwright/test
          playwright install

          # 執行 E2E 測試
          BASE_URL=https://staging.aiagent.example.com npm run test:e2e

      - name: Run Performance Tests
        run: |
          # 安裝 k6
          sudo gpg -k
          sudo gpg --no-default-keyring --keyring /usr/share/keyrings/k6-archive-keyring.gpg --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys C5AD17C747E3415A3642D57D77C6C491D6AC1D69
          echo "deb [signed-by=/usr/share/keyrings/k6-archive-keyring.gpg] https://dl.k6.io/deb stable main" | sudo tee /etc/apt/sources.list.d/k6.list
          sudo apt-get update
          sudo apt-get install k6

          # 執行性能測試
          k6 run tests/performance/load-test.js

      - name: Security Scan with OWASP ZAP
        uses: zaproxy/action-baseline@v0.10.0
        with:
          target: 'https://staging.aiagent.example.com'
          rules_file_name: '.zap/rules.tsv'
          cmd_options: '-a'
```

### CD Workflow - Production: `.github/workflows/cd-production.yml`

```yaml
name: Deploy to Production

on:
  release:
    types: [published]
  workflow_dispatch:
    inputs:
      version:
        description: 'Release version to deploy'
        required: true
        type: string

env:
  ENVIRONMENT: production
  AKS_CLUSTER: aks-aiagent-prod
  AKS_RESOURCE_GROUP: rg-aiagent-prod
  NAMESPACE: aiagent-prod

jobs:
  deploy-production:
    name: Deploy to Production Environment
    runs-on: ubuntu-latest
    environment:
      name: production
      url: https://aiagent.example.com

    steps:
      - name: Checkout Code
        uses: actions/checkout@v4
        with:
          ref: ${{ github.event.release.tag_name || github.event.inputs.version }}

      - name: Azure Login
        uses: azure/login@v2
        with:
          creds: ${{ secrets.AZURE_CREDENTIALS_PROD }}

      - name: Get AKS Credentials
        run: |
          az aks get-credentials \
            --resource-group ${{ env.AKS_RESOURCE_GROUP }} \
            --name ${{ env.AKS_CLUSTER }} \
            --overwrite-existing

      - name: Create Backup of Current Deployment
        run: |
          kubectl get all -n ${{ env.NAMESPACE }} -o yaml > backup-${{ github.sha }}.yaml

      - name: Blue-Green Deployment - Deploy Green
        run: |
          export IMAGE_TAG=${{ github.event.release.tag_name || github.event.inputs.version }}
          export ENVIRONMENT=${{ env.ENVIRONMENT }}
          export DEPLOYMENT_COLOR=green

          # 部署 Green 環境
          kubectl apply -k k8s/overlays/production-green

          # 等待 Green 環境就緒
          kubectl rollout status deployment/api-deployment-green -n ${{ env.NAMESPACE }} --timeout=15m
          kubectl rollout status deployment/worker-deployment-green -n ${{ env.NAMESPACE }} --timeout=15m
          kubectl rollout status deployment/web-deployment-green -n ${{ env.NAMESPACE }} --timeout=15m

      - name: Health Check - Green Environment
        run: |
          # 內部健康檢查
          kubectl wait --for=condition=ready pod -l app=api,color=green -n ${{ env.NAMESPACE }} --timeout=5m

          # API 健康檢查
          API_POD=$(kubectl get pod -l app=api,color=green -n ${{ env.NAMESPACE }} -o jsonpath='{.items[0].metadata.name}')
          kubectl exec -n ${{ env.NAMESPACE }} ${API_POD} -- curl -f http://localhost:8080/health

      - name: Smoke Tests - Green Environment
        run: |
          # 執行關鍵業務流程煙霧測試
          npm run test:smoke -- --environment=production-green

      - name: Switch Traffic to Green (Canary - 10%)
        run: |
          # 逐步切換流量到 Green 環境
          kubectl patch service api-service -n ${{ env.NAMESPACE }} -p '{"spec":{"selector":{"color":"green","canary":"true"}}}'

          # 等待 5 分鐘觀察
          sleep 300

      - name: Monitor Canary Metrics
        run: |
          # 檢查錯誤率、延遲等指標
          # 這裡應該查詢 Prometheus/Grafana API
          echo "Checking canary metrics..."
          # 如果錯誤率 > 1%, 自動回滾

      - name: Switch Traffic to Green (100%)
        run: |
          # 完全切換到 Green 環境
          kubectl patch service api-service -n ${{ env.NAMESPACE }} -p '{"spec":{"selector":{"color":"green"}}}'
          kubectl patch service worker-service -n ${{ env.NAMESPACE }} -p '{"spec":{"selector":{"color":"green"}}}'
          kubectl patch service web-service -n ${{ env.NAMESPACE }} -p '{"spec":{"selector":{"color":"green"}}}'

      - name: Delete Blue Environment
        run: |
          # 保留 Blue 環境 30 分鐘以便快速回滾
          sleep 1800
          kubectl delete deployment api-deployment-blue -n ${{ env.NAMESPACE }} --ignore-not-found
          kubectl delete deployment worker-deployment-blue -n ${{ env.NAMESPACE }} --ignore-not-found
          kubectl delete deployment web-deployment-blue -n ${{ env.NAMESPACE }} --ignore-not-found

      - name: Notify Production Deployment
        if: always()
        uses: 8398a7/action-slack@v3
        with:
          status: ${{ job.status }}
          text: |
            Production Deployment ${{ job.status }}
            Version: ${{ github.event.release.tag_name || github.event.inputs.version }}
            Deployed by: ${{ github.actor }}
          webhook_url: ${{ secrets.SLACK_WEBHOOK }}
```

---

## 部署門控與審批流程

### GitHub Environments 配置

在 GitHub Repository Settings → Environments 中配置:

#### Development Environment
```yaml
name: development
deployment_branches:
  - develop
protection_rules:
  required_reviewers: 0
  wait_timer: 0
secrets:
  AZURE_CREDENTIALS_DEV: <Azure Service Principal JSON>
  ACR_USERNAME: <ACR Username>
  ACR_PASSWORD: <ACR Password>
```

#### Staging Environment
```yaml
name: staging
deployment_branches:
  - main
protection_rules:
  required_reviewers: 1
  reviewers:
    - tech-lead-team
  wait_timer: 0
secrets:
  AZURE_CREDENTIALS_STAGING: <Azure Service Principal JSON>
```

#### Production Environment
```yaml
name: production
deployment_branches:
  - main
protection_rules:
  required_reviewers: 2
  reviewers:
    - tech-lead-team
    - product-management
  wait_timer: 300  # 5 分鐘等待時間
  prevent_self_review: true
secrets:
  AZURE_CREDENTIALS_PROD: <Azure Service Principal JSON>
```

---

## 回滾策略

### 自動回滾觸發條件

```yaml
# 在 Production 部署中添加自動回滾邏輯
- name: Monitor Deployment Health
  id: health_check
  run: |
    # 檢查錯誤率
    ERROR_RATE=$(curl -s "http://prometheus:9090/api/v1/query?query=rate(http_requests_total{status=~\"5..\"}[5m])" | jq '.data.result[0].value[1]')

    if (( $(echo "$ERROR_RATE > 0.01" | bc -l) )); then
      echo "::set-output name=rollback::true"
      echo "Error rate too high: $ERROR_RATE"
      exit 1
    fi

    # 檢查延遲 (P95 > 2 秒)
    LATENCY_P95=$(curl -s "http://prometheus:9090/api/v1/query?query=histogram_quantile(0.95,http_request_duration_seconds_bucket)" | jq '.data.result[0].value[1]')

    if (( $(echo "$LATENCY_P95 > 2.0" | bc -l) )); then
      echo "::set-output name=rollback::true"
      echo "Latency too high: $LATENCY_P95"
      exit 1
    fi

- name: Rollback on Failure
  if: failure() && steps.health_check.outputs.rollback == 'true'
  run: |
    echo "🚨 Automatic rollback triggered"

    # 切換回 Blue 環境
    kubectl patch service api-service -n ${{ env.NAMESPACE }} -p '{"spec":{"selector":{"color":"blue"}}}'
    kubectl patch service worker-service -n ${{ env.NAMESPACE }} -p '{"spec":{"selector":{"color":"blue"}}}'
    kubectl patch service web-service -n ${{ env.NAMESPACE }} -p '{"spec":{"selector":{"color":"blue"}}}'

    # 刪除有問題的 Green 環境
    kubectl delete deployment api-deployment-green -n ${{ env.NAMESPACE }}
    kubectl delete deployment worker-deployment-green -n ${{ env.NAMESPACE }}
    kubectl delete deployment web-deployment-green -n ${{ env.NAMESPACE }}

    # 通知團隊
    curl -X POST ${{ secrets.SLACK_WEBHOOK }} \
      -H 'Content-Type: application/json' \
      -d '{
        "text": "🚨 Production deployment rolled back automatically",
        "attachments": [{
          "color": "danger",
          "fields": [
            {"title": "Version", "value": "${{ github.event.release.tag_name }}", "short": true},
            {"title": "Reason", "value": "Health check failed", "short": true}
          ]
        }]
      }'
```

### 手動回滾

```bash
# 使用 GitHub CLI 手動觸發回滾
gh workflow run cd-production.yml \
  -f version=v1.2.3 \
  -f rollback=true
```

---

## 最佳實踐

### 1. Secrets 管理

**使用 GitHub Secrets**:
```bash
# 設置 Azure Credentials (Service Principal)
gh secret set AZURE_CREDENTIALS_PROD --body '{
  "clientId": "<client-id>",
  "clientSecret": "<client-secret>",
  "subscriptionId": "<subscription-id>",
  "tenantId": "<tenant-id>"
}'

# 設置 ACR Credentials
gh secret set ACR_USERNAME --body "aiagentplatform"
gh secret set ACR_PASSWORD --body "<acr-password>"

# 設置 SonarQube Token
gh secret set SONAR_TOKEN --body "<sonar-token>"
```

### 2. 構建緩存優化

```yaml
# 使用 Docker Layer Caching 加速構建
- name: Build and Push Docker Image
  uses: docker/build-push-action@v5
  with:
    cache-from: type=registry,ref=${{ env.DOCKER_REGISTRY }}/${{ matrix.service.name }}:buildcache
    cache-to: type=registry,ref=${{ env.DOCKER_REGISTRY }}/${{ matrix.service.name }}:buildcache,mode=max
```

### 3. 並行執行優化

```yaml
jobs:
  test:
    strategy:
      matrix:
        test-suite: [unit, integration, e2e]
    steps:
      - run: npm run test:${{ matrix.test-suite }}
```

### 4. Workflow 重用

創建可重用 Workflow:

`.github/workflows/reusable-deploy.yml`:
```yaml
name: Reusable Deploy Workflow

on:
  workflow_call:
    inputs:
      environment:
        required: true
        type: string
      image_tag:
        required: true
        type: string
    secrets:
      azure_credentials:
        required: true

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      # ... 通用部署邏輯
```

使用方式:
```yaml
jobs:
  deploy-dev:
    uses: ./.github/workflows/reusable-deploy.yml
    with:
      environment: development
      image_tag: ${{ github.sha }}
    secrets:
      azure_credentials: ${{ secrets.AZURE_CREDENTIALS_DEV }}
```

---

## 監控與告警

### Workflow 執行監控

```yaml
- name: Send Deployment Metrics to Datadog
  if: always()
  run: |
    curl -X POST "https://api.datadoghq.com/api/v1/events?api_key=${{ secrets.DATADOG_API_KEY }}" \
      -H "Content-Type: application/json" \
      -d '{
        "title": "Deployment to ${{ env.ENVIRONMENT }}",
        "text": "Status: ${{ job.status }}",
        "tags": ["environment:${{ env.ENVIRONMENT }}", "version:${{ github.sha }}"],
        "alert_type": "${{ job.status == 'success' && 'info' || 'error' }}"
      }'
```

### Slack 通知集成

```yaml
- name: Notify Team on Slack
  if: always()
  uses: 8398a7/action-slack@v3
  with:
    status: custom
    custom_payload: |
      {
        "text": "Deployment to ${{ env.ENVIRONMENT }}",
        "attachments": [{
          "color": "${{ job.status == 'success' && 'good' || 'danger' }}",
          "fields": [
            {"title": "Status", "value": "${{ job.status }}", "short": true},
            {"title": "Environment", "value": "${{ env.ENVIRONMENT }}", "short": true},
            {"title": "Version", "value": "${{ github.sha }}", "short": true},
            {"title": "Actor", "value": "${{ github.actor }}", "short": true}
          ],
          "actions": [{
            "type": "button",
            "text": "View Logs",
            "url": "${{ github.server_url }}/${{ github.repository }}/actions/runs/${{ github.run_id }}"
          }]
        }]
      }
    webhook_url: ${{ secrets.SLACK_WEBHOOK }}
```

---

## 故障排查

### 常見問題

| 問題 | 原因 | 解決方案 |
|------|------|----------|
| **Docker 構建失敗** | 依賴項下載超時 | 使用 Azure Artifacts Mirror: `RUN dotnet restore --source https://pkgs.dev.azure.com/...` |
| **Kubernetes 部署超時** | Image Pull 失敗 | 檢查 ACR 憑證: `kubectl get secret acr-secret -n $NAMESPACE -o yaml` |
| **健康檢查失敗** | 數據庫遷移未完成 | 使用 Init Container 等待數據庫就緒: `initContainers: [{name: wait-db, ...}]` |
| **Canary 回滾** | 錯誤率過高 | 檢查 Application Insights 日誌: `az monitor app-insights query ...` |

### 調試技巧

```bash
# 查看 Workflow Run 日誌
gh run view <run-id> --log

# 重新運行失敗的 Job
gh run rerun <run-id> --failed

# 手動觸發 Workflow 並啟用調試
gh workflow run ci.yml -f debug_enabled=true

# 查看 Kubernetes Pod 日誌
kubectl logs -n aiagent-prod deployment/api-deployment --tail=100 --follow

# 檢查 Deployment 狀態
kubectl describe deployment api-deployment -n aiagent-prod
```

---

## 安全性檢查清單

- [ ] **Secrets 安全**
  - [ ] 所有敏感信息存儲在 GitHub Secrets
  - [ ] 定期輪換 Service Principal 憑證
  - [ ] 使用最小權限原則 (RBAC)

- [ ] **代碼掃描**
  - [ ] 啟用 SonarQube 質量門控 (Quality Gate)
  - [ ] Snyk 依賴掃描通過
  - [ ] Trivy 容器映像掃描無 CRITICAL 漏洞

- [ ] **部署安全**
  - [ ] Production 環境需要雙重審批
  - [ ] 使用 Blue-Green 部署降低風險
  - [ ] 自動回滾機制已測試

- [ ] **審計日誌**
  - [ ] 所有部署操作記錄到 Azure Monitor
  - [ ] Slack 通知包含完整上下文
  - [ ] GitHub Audit Log 啟用

---

## 總結

本文檔提供了完整的 GitHub Actions CI/CD Pipeline 實施方案,涵蓋:

✅ **多環境部署策略** (Dev/Staging/Prod)
✅ **完整的測試覆蓋** (單元/整合/E2E/性能/安全)
✅ **Docker 多服務構建與推送**
✅ **AKS Blue-Green 部署與 Canary 發布**
✅ **自動化安全掃描** (Snyk, Trivy, OWASP ZAP)
✅ **部署門控與審批流程**
✅ **自動/手動回滾機制**
✅ **監控與告警集成** (Slack, Datadog)

透過本指南,開發團隊可以實現:
- **快速交付**: 從代碼提交到生產部署 < 30 分鐘
- **高可靠性**: Blue-Green 部署零停機時間
- **安全合規**: 多層安全掃描與審批流程
- **可觀測性**: 完整的部署日誌與告警
