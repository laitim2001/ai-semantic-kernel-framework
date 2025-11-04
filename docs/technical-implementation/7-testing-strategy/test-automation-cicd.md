# 測試自動化與 CI/CD (Test Automation & CI/CD)

## 概述

本文檔定義 AI Workflow Platform 的測試自動化策略和 CI/CD 集成,確保每次代碼提交都經過完整的自動化測試驗證,實現持續質量保證。

## CI/CD 測試策略

### 測試金字塔在 CI/CD 中的應用

```
PR Pipeline (快速反饋 ~5 分鐘)
├── 單元測試 (2 分鐘)
├── Lint & 格式檢查 (1 分鐘)
└── 快速冒煙測試 (2 分鐘)

Main Branch Pipeline (完整驗證 ~15 分鐘)
├── 單元測試 (2 分鐘)
├── 集成測試 (5 分鐘)
├── E2E 關鍵場景 (5 分鐘)
├── 覆蓋率檢查 (1 分鐘)
└── 代碼質量掃描 (2 分鐘)

Nightly Build (全面測試 ~45 分鐘)
├── 完整測試套件 (30 分鐘)
├── E2E 完整測試 (10 分鐘)
├── 性能測試 (5 分鐘)
└── 安全掃描
```

## GitHub Actions 工作流程

### 1. Pull Request 測試流程

```yaml
# .github/workflows/pr-test.yml
name: PR Test & Quality Check

on:
  pull_request:
    branches: [main, develop]
    paths:
      - 'src/**'
      - 'tests/**'
      - '*.csproj'
      - 'package.json'

concurrency:
  group: ${{ github.workflow }}-${{ github.event.pull_request.number }}
  cancel-in-progress: true

jobs:
  # 後端測試
  backend-test:
    runs-on: ubuntu-latest
    timeout-minutes: 10

    services:
      postgres:
        image: postgres:16-alpine
        env:
          POSTGRES_DB: aiworkflow_test
          POSTGRES_USER: test_user
          POSTGRES_PASSWORD: test_password
        ports:
          - 5432:5432
        options: >-
          --health-cmd pg_isready
          --health-interval 10s
          --health-timeout 5s
          --health-retries 5

      redis:
        image: redis:7-alpine
        ports:
          - 6379:6379
        options: >-
          --health-cmd "redis-cli ping"
          --health-interval 10s
          --health-timeout 5s
          --health-retries 5

    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'
          cache: true
          cache-dependency-path: '**/*.csproj'

      - name: Restore dependencies
        run: dotnet restore

      - name: Build
        run: dotnet build --no-restore --configuration Release

      - name: Run Unit Tests
        run: |
          dotnet test --no-build --configuration Release \
            --filter "FullyQualifiedName~UnitTests" \
            --logger "trx;LogFileName=unit-tests.trx" \
            --collect:"XPlat Code Coverage" \
            -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover

      - name: Run Integration Tests
        env:
          ConnectionStrings__DefaultConnection: "Host=localhost;Port=5432;Database=aiworkflow_test;Username=test_user;Password=test_password"
          ConnectionStrings__Redis: "localhost:6379"
        run: |
          dotnet test --no-build --configuration Release \
            --filter "FullyQualifiedName~IntegrationTests" \
            --logger "trx;LogFileName=integration-tests.trx"

      - name: Upload Test Results
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: backend-test-results
          path: |
            **/TestResults/*.trx
            **/coverage.opencover.xml

  # 前端測試
  frontend-test:
    runs-on: ubuntu-latest
    timeout-minutes: 10

    steps:
      - uses: actions/checkout@v4

      - name: Setup Node.js
        uses: actions/setup-node@v4
        with:
          node-version: '20'
          cache: 'npm'

      - name: Install dependencies
        run: npm ci

      - name: Run Lint
        run: npm run lint

      - name: Run Type Check
        run: npm run type-check

      - name: Run Unit Tests
        run: npm run test:coverage

      - name: Upload Coverage
        uses: codecov/codecov-action@v3
        with:
          files: ./coverage/lcov.info
          flags: frontend
          name: frontend-coverage

      - name: Upload Test Results
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: frontend-test-results
          path: coverage/

  # 代碼質量檢查
  code-quality:
    runs-on: ubuntu-latest
    needs: [backend-test, frontend-test]

    steps:
      - uses: actions/checkout@v4
        with:
          fetch-depth: 0

      - name: Download Test Results
        uses: actions/download-artifact@v4
        with:
          path: test-results/

      - name: SonarCloud Scan
        uses: SonarSource/sonarcloud-github-action@master
        env:
          GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
          SONAR_TOKEN: ${{ secrets.SONAR_TOKEN }}
        with:
          args: >
            -Dsonar.organization=your-org
            -Dsonar.projectKey=ai-workflow-platform
            -Dsonar.sources=src
            -Dsonar.tests=tests
            -Dsonar.cs.opencover.reportsPaths=test-results/backend-test-results/coverage.opencover.xml
            -Dsonar.javascript.lcov.reportPaths=test-results/frontend-test-results/lcov.info
            -Dsonar.coverage.exclusions=**/*.test.ts,**/*.spec.ts,**/Migrations/**

  # 覆蓋率檢查
  coverage-check:
    runs-on: ubuntu-latest
    needs: [backend-test, frontend-test]

    steps:
      - uses: actions/checkout@v4

      - name: Download Coverage Reports
        uses: actions/download-artifact@v4

      - name: Check Coverage Threshold
        run: |
          # 後端覆蓋率檢查
          BACKEND_COVERAGE=$(grep -oP 'line-rate="\K[^"]+' backend-test-results/coverage.opencover.xml | head -1)
          BACKEND_PERCENT=$(echo "$BACKEND_COVERAGE * 100" | bc)

          # 前端覆蓋率檢查
          FRONTEND_COVERAGE=$(jq '.total.lines.pct' frontend-test-results/coverage-summary.json)

          echo "Backend Coverage: $BACKEND_PERCENT%"
          echo "Frontend Coverage: $FRONTEND_COVERAGE%"

          if (( $(echo "$BACKEND_PERCENT < 80" | bc -l) )); then
            echo "❌ Backend coverage $BACKEND_PERCENT% is below 80% threshold"
            exit 1
          fi

          if (( $(echo "$FRONTEND_COVERAGE < 75" | bc -l) )); then
            echo "❌ Frontend coverage $FRONTEND_COVERAGE% is below 75% threshold"
            exit 1
          fi

          echo "✅ All coverage thresholds met"

      - name: Comment PR with Coverage
        uses: actions/github-script@v7
        with:
          script: |
            const fs = require('fs')

            // 讀取覆蓋率數據
            const backendXml = fs.readFileSync('backend-test-results/coverage.opencover.xml', 'utf8')
            const backendMatch = backendXml.match(/line-rate="([^"]+)"/)
            const backendCoverage = (parseFloat(backendMatch[1]) * 100).toFixed(2)

            const frontendSummary = JSON.parse(fs.readFileSync('frontend-test-results/coverage-summary.json'))
            const frontendCoverage = frontendSummary.total.lines.pct.toFixed(2)

            const comment = `## 📊 Coverage Report

            ### Backend (.NET)
            | Metric | Coverage | Status |
            |--------|----------|--------|
            | Lines | ${backendCoverage}% | ${backendCoverage >= 80 ? '✅' : '❌'} |
            | Threshold | 80% | |

            ### Frontend (TypeScript)
            | Metric | Coverage | Status |
            |--------|----------|--------|
            | Lines | ${frontendCoverage}% | ${frontendCoverage >= 75 ? '✅' : '❌'} |
            | Branches | ${frontendSummary.total.branches.pct.toFixed(2)}% | ${frontendSummary.total.branches.pct >= 70 ? '✅' : '❌'} |
            | Functions | ${frontendSummary.total.functions.pct.toFixed(2)}% | ${frontendSummary.total.functions.pct >= 75 ? '✅' : '❌'} |
            | Threshold | 75% | |

            ${backendCoverage >= 80 && frontendCoverage >= 75 ? '✅ All coverage thresholds met!' : '❌ Some thresholds not met'}
            `

            github.rest.issues.createComment({
              issue_number: context.issue.number,
              owner: context.repo.owner,
              repo: context.repo.repo,
              body: comment
            })
```

### 2. Main Branch 完整測試流程

```yaml
# .github/workflows/main-test.yml
name: Main Branch Full Test

on:
  push:
    branches: [main]
  workflow_dispatch:

jobs:
  full-test:
    runs-on: ubuntu-latest
    timeout-minutes: 30

    services:
      postgres:
        image: postgres:16-alpine
        env:
          POSTGRES_DB: aiworkflow_test
          POSTGRES_USER: test_user
          POSTGRES_PASSWORD: test_password
        ports:
          - 5432:5432
        options: --health-cmd pg_isready --health-interval 10s

      redis:
        image: redis:7-alpine
        ports:
          - 6379:6379
        options: --health-cmd "redis-cli ping" --health-interval 10s

    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'

      - name: Setup Node.js
        uses: actions/setup-node@v4
        with:
          node-version: '20'
          cache: 'npm'

      - name: Install dependencies
        run: |
          dotnet restore
          npm ci

      - name: Build Backend
        run: dotnet build --configuration Release

      - name: Build Frontend
        run: npm run build

      - name: Run All Backend Tests
        run: |
          dotnet test --configuration Release \
            --logger "trx;LogFileName=all-tests.trx" \
            --collect:"XPlat Code Coverage" \
            /p:CollectCoverage=true \
            /p:CoverletOutputFormat=opencover

      - name: Run All Frontend Tests
        run: npm run test:coverage

      - name: Install Playwright
        run: npx playwright install --with-deps

      - name: Run E2E Tests
        run: npx playwright test --project=chromium
        env:
          BASE_URL: http://localhost:5173

      - name: Upload Playwright Report
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: playwright-report
          path: playwright-report/

      - name: Publish Test Results
        if: always()
        uses: dorny/test-reporter@v1
        with:
          name: Test Results
          path: '**/TestResults/*.trx'
          reporter: dotnet-trx

      - name: Upload to Codecov
        uses: codecov/codecov-action@v3
        with:
          files: |
            **/coverage.opencover.xml
            coverage/lcov.info
```

### 3. Nightly Build 完整測試

```yaml
# .github/workflows/nightly.yml
name: Nightly Build

on:
  schedule:
    - cron: '0 2 * * *'  # 每天凌晨 2 點執行
  workflow_dispatch:

jobs:
  comprehensive-test:
    runs-on: ubuntu-latest
    timeout-minutes: 60

    strategy:
      matrix:
        browser: [chromium, firefox, webkit]

    steps:
      - uses: actions/checkout@v4

      - name: Setup Environment
        run: |
          # 設置所有依賴
          dotnet restore
          npm ci
          npx playwright install --with-deps

      - name: Run Full Test Suite
        run: |
          # 後端所有測試
          dotnet test --configuration Release --logger trx

          # 前端所有測試
          npm run test:coverage

          # E2E 跨瀏覽器測試
          npx playwright test --project=${{ matrix.browser }}

      - name: Performance Testing
        run: |
          # 使用 k6 進行性能測試
          k6 run performance-tests/load-test.js

      - name: Security Scan
        run: |
          # OWASP Dependency Check
          dotnet tool install --global dotnet-retire
          dotnet retire

          # npm audit
          npm audit --audit-level=moderate

      - name: Generate Comprehensive Report
        run: |
          # 生成完整測試報告
          ./scripts/generate-test-report.sh

      - name: Notify Team
        if: failure()
        uses: 8398a7/action-slack@v3
        with:
          status: ${{ job.status }}
          text: 'Nightly build failed! Check the results.'
          webhook_url: ${{ secrets.SLACK_WEBHOOK }}
```

## Azure DevOps Pipelines

### 多階段 Pipeline 配置

```yaml
# azure-pipelines.yml
trigger:
  branches:
    include:
      - main
      - develop

pr:
  branches:
    include:
      - main

pool:
  vmImage: 'ubuntu-latest'

variables:
  buildConfiguration: 'Release'
  dotnetVersion: '8.0.x'
  nodeVersion: '20.x'

stages:
  # Stage 1: Build
  - stage: Build
    displayName: 'Build Application'
    jobs:
      - job: BuildBackend
        displayName: 'Build .NET Backend'
        steps:
          - task: UseDotNet@2
            inputs:
              version: $(dotnetVersion)

          - task: DotNetCoreCLI@2
            displayName: 'Restore dependencies'
            inputs:
              command: 'restore'
              projects: '**/*.csproj'

          - task: DotNetCoreCLI@2
            displayName: 'Build projects'
            inputs:
              command: 'build'
              projects: '**/*.csproj'
              arguments: '--configuration $(buildConfiguration)'

          - task: DotNetCoreCLI@2
            displayName: 'Publish artifacts'
            inputs:
              command: 'publish'
              publishWebProjects: false
              projects: '**/AIWorkflow.API.csproj'
              arguments: '--configuration $(buildConfiguration) --output $(Build.ArtifactStagingDirectory)'
              zipAfterPublish: true

          - publish: $(Build.ArtifactStagingDirectory)
            artifact: backend

      - job: BuildFrontend
        displayName: 'Build Frontend'
        steps:
          - task: NodeTool@0
            inputs:
              versionSpec: $(nodeVersion)

          - script: npm ci
            displayName: 'Install dependencies'

          - script: npm run build
            displayName: 'Build frontend'

          - publish: dist
            artifact: frontend

  # Stage 2: Test
  - stage: Test
    displayName: 'Run Tests'
    dependsOn: Build
    jobs:
      - job: UnitTests
        displayName: 'Unit Tests'
        steps:
          - task: UseDotNet@2
            inputs:
              version: $(dotnetVersion)

          - task: DotNetCoreCLI@2
            displayName: 'Run unit tests'
            inputs:
              command: 'test'
              projects: '**/*UnitTests.csproj'
              arguments: '--configuration $(buildConfiguration) --collect:"XPlat Code Coverage"'

          - task: PublishCodeCoverageResults@1
            inputs:
              codeCoverageTool: 'Cobertura'
              summaryFileLocation: '$(Agent.TempDirectory)/**/coverage.cobertura.xml'

      - job: IntegrationTests
        displayName: 'Integration Tests'
        services:
          postgres:
            image: postgres:16-alpine
            ports:
              - 5432:5432
            env:
              POSTGRES_DB: aiworkflow_test
              POSTGRES_USER: test_user
              POSTGRES_PASSWORD: test_password

        steps:
          - task: DotNetCoreCLI@2
            displayName: 'Run integration tests'
            inputs:
              command: 'test'
              projects: '**/*IntegrationTests.csproj'
              arguments: '--configuration $(buildConfiguration)'

      - job: E2ETests
        displayName: 'E2E Tests'
        steps:
          - task: NodeTool@0
            inputs:
              versionSpec: $(nodeVersion)

          - script: |
              npm ci
              npx playwright install --with-deps
            displayName: 'Setup Playwright'

          - script: npx playwright test
            displayName: 'Run E2E tests'

          - publish: playwright-report
            artifact: e2e-report
            condition: always()

  # Stage 3: Quality Gates
  - stage: QualityGates
    displayName: 'Quality Gates'
    dependsOn: Test
    jobs:
      - job: CodeQuality
        displayName: 'Code Quality Check'
        steps:
          - task: SonarCloudPrepare@1
            inputs:
              SonarCloud: 'SonarCloud'
              organization: 'your-org'
              scannerMode: 'MSBuild'
              projectKey: 'ai-workflow-platform'

          - task: DotNetCoreCLI@2
            inputs:
              command: 'build'

          - task: SonarCloudAnalyze@1

          - task: SonarCloudPublish@1
            inputs:
              pollingTimeoutSec: '300'
```

## 測試執行優化

### 並行測試執行

```yaml
# 使用矩陣策略並行執行
strategy:
  matrix:
    unit_tests:
      testCategory: 'UnitTests'
      timeout: 10
    integration_tests:
      testCategory: 'IntegrationTests'
      timeout: 20
    e2e_tests:
      testCategory: 'E2ETests'
      timeout: 30

steps:
  - script: |
      dotnet test --filter "Category=$(testCategory)" --logger trx
    displayName: 'Run $(testCategory)'
    timeoutInMinutes: $(timeout)
```

### 測試緩存策略

```yaml
- name: Cache Test Results
  uses: actions/cache@v3
  with:
    path: |
      ~/.nuget/packages
      ~/.npm
      **/node_modules
    key: ${{ runner.os }}-test-${{ hashFiles('**/package-lock.json', '**/*.csproj') }}
    restore-keys: |
      ${{ runner.os }}-test-
```

## 測試失敗處理

### 自動重試機制

```yaml
- name: Run Tests with Retry
  uses: nick-fields/retry@v2
  with:
    timeout_minutes: 10
    max_attempts: 3
    command: dotnet test --configuration Release
```

### 失敗通知

```yaml
- name: Notify on Failure
  if: failure()
  uses: 8398a7/action-slack@v3
  with:
    status: ${{ job.status }}
    text: |
      🚨 Test failure in ${{ github.repository }}
      Branch: ${{ github.ref }}
      Author: ${{ github.actor }}
      Commit: ${{ github.sha }}
    fields: repo,message,commit,author
    webhook_url: ${{ secrets.SLACK_WEBHOOK }}
```

## 檢查清單

### CI/CD 配置
- [ ] PR 測試流程配置完成
- [ ] Main Branch 完整測試配置
- [ ] Nightly Build 配置
- [ ] 測試並行執行啟用
- [ ] 測試緩存配置

### 質量門檻
- [ ] 覆蓋率閾值檢查啟用
- [ ] SonarQube/SonarCloud 集成
- [ ] 測試失敗自動通知
- [ ] 測試報告自動生成
- [ ] Badge 顯示測試狀態

### 性能優化
- [ ] 測試執行時間 < 15 分鐘(主流程)
- [ ] 並行測試配置
- [ ] 依賴緩存啟用
- [ ] 增量測試執行
- [ ] 資源使用優化

## 參考資源

### CI/CD 工具
- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Azure DevOps Pipelines](https://learn.microsoft.com/en-us/azure/devops/pipelines/)
- [GitLab CI/CD](https://docs.gitlab.com/ee/ci/)

### 測試自動化
- [xUnit CI/CD Integration](https://xunit.net/docs/running-tests-in-cicd)
- [Playwright CI](https://playwright.dev/docs/ci)

---

**文檔維護**: DevOps Team / QA Team
**最後更新**: 2025-01-01
**狀態**: ✅ 完整
