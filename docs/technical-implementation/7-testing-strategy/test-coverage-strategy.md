# 測試覆蓋率策略 (Test Coverage Strategy)

## 概述

本文檔定義 AI Workflow Platform 的測試覆蓋率目標、工具配置、分析方法和持續改進策略,確保代碼質量和測試有效性。

## 覆蓋率目標

### 分層覆蓋率標準

| 層級 | 最低覆蓋率 | 目標覆蓋率 | 優先級 | 說明 |
|------|----------|----------|--------|------|
| **Domain 層** | 90% | 95% | 🔴 最高 | 核心業務邏輯,必須高覆蓋 |
| **Application 層** | 80% | 90% | 🟡 高 | 應用服務和 CQRS 處理器 |
| **Infrastructure 層** | 70% | 80% | 🟡 中 | 數據訪問和外部服務 |
| **API 層** | 75% | 85% | 🟡 高 | 端點和控制器 |
| **前端業務邏輯** | 75% | 85% | 🟡 高 | Hooks, Services, Stores |
| **前端組件** | 60% | 75% | 🟢 中 | React/Vue 組件 |
| **整體項目** | **80%** | **90%** | 🔴 最高 | 總體覆蓋率目標 |

## 後端覆蓋率工具配置

### Coverlet 配置

```xml
<!-- AIWorkflow.UnitTests/AIWorkflow.UnitTests.csproj -->
<PropertyGroup>
  <!-- 啟用覆蓋率收集 -->
  <CollectCoverage>true</CollectCoverage>

  <!-- 輸出格式 -->
  <CoverletOutputFormat>opencover,json,cobertura</CoverletOutputFormat>

  <!-- 輸出路徑 -->
  <CoverletOutput>./coverage/</CoverletOutput>

  <!-- 排除文件 -->
  <Exclude>[*.Tests]*,[*]*.Migrations.*</Exclude>

  <!-- 覆蓋率閾值 -->
  <Threshold>80</Threshold>
  <ThresholdType>line,branch</ThresholdType>
  <ThresholdStat>total</ThresholdStat>
</PropertyGroup>
```

### 全局覆蓋率配置

```json
// coverlet.runsettings
{
  "DataCollectionRunSettings": {
    "DataCollectors": {
      "XPlat Code Coverage": {
        "Configuration": {
          "Format": "opencover,cobertura,json",
          "Exclude": [
            "[*.Tests]*",
            "[*]*.Migrations.*",
            "[*]Program",
            "[*]Startup"
          ],
          "ExcludeByAttribute": "Obsolete,GeneratedCode,CompilerGenerated",
          "ExcludeByFile": [
            "**/Migrations/**/*.cs"
          ],
          "IncludeDirectory": [],
          "SingleHit": false,
          "UseSourceLink": true,
          "IncludeTestAssembly": false,
          "SkipAutoProps": true,
          "DeterministicReport": true
        }
      }
    }
  }
}
```

### 測試執行命令

```bash
# 運行測試並生成覆蓋率
dotnet test /p:CollectCoverage=true \
             /p:CoverletOutputFormat=opencover \
             /p:CoverletOutput=./coverage/

# 使用 runsettings
dotnet test --collect:"XPlat Code Coverage" \
            --settings coverlet.runsettings

# 生成 HTML 報告
reportgenerator \
  -reports:"coverage/coverage.opencover.xml" \
  -targetdir:"coverage/report" \
  -reporttypes:"Html;Badges"

# 檢查覆蓋率閾值
dotnet test /p:CollectCoverage=true \
            /p:Threshold=80 \
            /p:ThresholdType=line,branch \
            /p:ThresholdStat=total
```

## 前端覆蓋率工具配置

### Vitest Coverage 配置

```typescript
// vitest.config.ts
import { defineConfig } from 'vitest/config'

export default defineConfig({
  test: {
    coverage: {
      provider: 'v8',

      // 報告類型
      reporter: ['text', 'json', 'html', 'lcov', 'json-summary'],

      // 輸出目錄
      reportsDirectory: './coverage',

      // 排除文件
      exclude: [
        'node_modules/',
        'dist/',
        'build/',
        '**/*.d.ts',
        '**/*.config.*',
        '**/mockData/',
        '**/test-utils/',
        'src/main.tsx',
        'src/vite-env.d.ts'
      ],

      // 覆蓋率閾值
      lines: 80,
      functions: 80,
      branches: 75,
      statements: 80,

      // 每個文件都需要達到閾值
      perFile: true,

      // 100% 覆蓋率的文件
      '100': true,

      // 啟用所有語句檢查
      all: true,

      // 包含的文件
      include: ['src/**/*.{ts,tsx}']
    }
  }
})
```

### Coverage 報告命令

```json
// package.json
{
  "scripts": {
    "test:coverage": "vitest run --coverage",
    "test:coverage:watch": "vitest watch --coverage",
    "test:coverage:ui": "vitest --ui --coverage",
    "test:coverage:threshold": "vitest run --coverage --coverage.thresholds.lines=80"
  }
}
```

## SonarQube 集成

### SonarQube 配置

```properties
# sonar-project.properties
sonar.projectKey=ai-workflow-platform
sonar.projectName=AI Workflow Platform
sonar.projectVersion=1.0.0

# 源代碼路徑
sonar.sources=src
sonar.tests=tests

# 排除文件
sonar.exclusions=**/Migrations/**,**/node_modules/**,**/dist/**
sonar.test.exclusions=**/*.test.ts,**/*.spec.ts

# 覆蓋率報告路徑
sonar.cs.opencover.reportsPaths=coverage/coverage.opencover.xml
sonar.javascript.lcov.reportPaths=coverage/lcov.info

# 測試執行報告
sonar.testExecutionReportPaths=test-results/test-execution.xml

# 代碼質量閾值
sonar.qualitygate.wait=true
sonar.coverage.minimum=80
```

### GitHub Actions 集成

```yaml
# .github/workflows/code-quality.yml
name: Code Quality

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main, develop]

jobs:
  sonarcloud:
    runs-on: ubuntu-latest

    steps:
      - uses: actions/checkout@v4
        with:
          fetch-depth: 0

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'

      - name: Install SonarScanner
        run: dotnet tool install --global dotnet-sonarscanner

      - name: Begin SonarQube Analysis
        run: |
          dotnet sonarscanner begin \
            /k:"ai-workflow-platform" \
            /o:"your-org" \
            /d:sonar.host.url="https://sonarcloud.io" \
            /d:sonar.login="${{ secrets.SONAR_TOKEN }}" \
            /d:sonar.cs.opencover.reportsPaths="coverage/coverage.opencover.xml"

      - name: Build
        run: dotnet build --no-incremental

      - name: Test with Coverage
        run: |
          dotnet test --no-build \
            /p:CollectCoverage=true \
            /p:CoverletOutputFormat=opencover \
            /p:CoverletOutput=../coverage/

      - name: End SonarQube Analysis
        run: dotnet sonarscanner end /d:sonar.login="${{ secrets.SONAR_TOKEN }}"
```

## 覆蓋率分析與改進

### 查找未覆蓋代碼

```bash
# 生成詳細的覆蓋率報告
reportgenerator \
  -reports:"coverage/coverage.opencover.xml" \
  -targetdir:"coverage/report" \
  -reporttypes:"Html;JsonSummary" \
  -verbosity:"Info"

# 查看低覆蓋率文件
cat coverage/report/Summary.json | jq '.coverage.files[] | select(.lineCoverage < 80) | {name: .name, coverage: .lineCoverage}'
```

### 覆蓋率熱點分析

```csharp
// 使用 CoverageReport 分析工具
public sealed class CoverageAnalyzer
{
    public async Task<List<LowCoverageFile>> AnalyzeLowCoverageAsync(string coverageReportPath)
    {
        var report = await LoadCoverageReportAsync(coverageReportPath);

        return report.Files
            .Where(f => f.LineCoverage < 80)
            .OrderBy(f => f.LineCoverage)
            .Select(f => new LowCoverageFile
            {
                FilePath = f.Name,
                LineCoverage = f.LineCoverage,
                BranchCoverage = f.BranchCoverage,
                UncoveredLines = f.UncoveredLines
            })
            .ToList();
    }
}
```

## 覆蓋率質量門檻

### CI/CD 質量門檻

```yaml
# .github/workflows/test.yml
name: Test & Coverage

on:
  pull_request:
    branches: [main]

jobs:
  test:
    runs-on: ubuntu-latest

    steps:
      - uses: actions/checkout@v4

      - name: Run Tests with Coverage
        run: |
          dotnet test /p:CollectCoverage=true \
                      /p:Threshold=80 \
                      /p:ThresholdType=line,branch \
                      /p:ThresholdStat=total

      - name: Check Coverage Threshold
        run: |
          COVERAGE=$(cat coverage/coverage.json | jq '.summary.lineCoverage')
          if (( $(echo "$COVERAGE < 80" | bc -l) )); then
            echo "Coverage $COVERAGE% is below 80% threshold"
            exit 1
          fi

      - name: Comment PR with Coverage
        uses: actions/github-script@v7
        with:
          script: |
            const fs = require('fs')
            const coverage = JSON.parse(fs.readFileSync('coverage/coverage.json'))
            const summary = coverage.summary

            const comment = `## 📊 Coverage Report

            | Metric | Coverage |
            |--------|----------|
            | Lines | ${summary.lineCoverage.toFixed(2)}% |
            | Branches | ${summary.branchCoverage.toFixed(2)}% |
            | Functions | ${summary.methodCoverage.toFixed(2)}% |

            ${summary.lineCoverage >= 80 ? '✅' : '❌'} Coverage threshold: 80%
            `

            github.rest.issues.createComment({
              issue_number: context.issue.number,
              owner: context.repo.owner,
              repo: context.repo.repo,
              body: comment
            })
```

## 覆蓋率報告範例

### 文本報告

```
+-------------------+--------+--------+--------+
| Module            | Line   | Branch | Method |
+-------------------+--------+--------+--------+
| AIWorkflow.Domain | 94.2%  | 87.5%  | 96.1%  |
| AIWorkflow.Application | 86.7%  | 81.3%  | 89.4%  |
| AIWorkflow.Infrastructure | 78.9%  | 72.1%  | 82.3%  |
| AIWorkflow.API    | 83.5%  | 76.8%  | 85.7%  |
+-------------------+--------+--------+--------+
| Summary           | 85.8%  | 79.4%  | 88.4%  |
+-------------------+--------+--------+--------+
```

### Badge 生成

```markdown
<!-- 覆蓋率 Badge -->
![Coverage](https://img.shields.io/badge/coverage-85.8%25-brightgreen)
![Lines](https://img.shields.io/badge/lines-85.8%25-brightgreen)
![Branches](https://img.shields.io/badge/branches-79.4%25-yellow)
![Methods](https://img.shields.io/badge/methods-88.4%25-brightgreen)
```

## 改進策略

### 1. 識別低覆蓋率區域

```bash
# 生成低覆蓋率文件列表
reportgenerator -reports:coverage.xml -targetdir:report -reporttypes:TextSummary

# 輸出示例:
# Low Coverage Files (< 80%):
# - WorkflowExecutionEngine.cs: 62.5%
# - NodeValidator.cs: 71.3%
# - EmailService.cs: 58.9%
```

### 2. 優先級排序

1. **Domain 層**: 最高優先級,目標 95%
2. **Application 層**: 高優先級,目標 90%
3. **Critical Path**: 核心業務流程必須 100%
4. **API Endpoints**: 確保所有端點都有測試

### 3. 定期審查

```yaml
coverage_review_checklist:
  weekly:
    - "檢查整體覆蓋率趨勢"
    - "識別新增未覆蓋代碼"
    - "更新覆蓋率目標"

  monthly:
    - "深度分析低覆蓋率模塊"
    - "制定改進計劃"
    - "團隊覆蓋率培訓"

  quarterly:
    - "評估覆蓋率策略有效性"
    - "調整覆蓋率目標"
    - "表彰高覆蓋率貢獻者"
```

## 檢查清單

### 配置檢查
- [ ] Coverlet 正確配置
- [ ] Vitest coverage 正確配置
- [ ] SonarQube 集成完成
- [ ] CI/CD 覆蓋率檢查啟用
- [ ] 覆蓋率報告自動生成

### 質量檢查
- [ ] 整體覆蓋率 ≥ 80%
- [ ] Domain 層覆蓋率 ≥ 90%
- [ ] 關鍵業務邏輯 100% 覆蓋
- [ ] 無重要遺漏測試
- [ ] 測試質量高(非無效測試)

### 持續改進
- [ ] 定期覆蓋率審查
- [ ] 低覆蓋率代碼改進計劃
- [ ] 團隊覆蓋率意識培養
- [ ] 覆蓋率趨勢監控
- [ ] 最佳實踐分享

## 參考資源

### 工具文檔
- [Coverlet Documentation](https://github.com/coverlet-coverage/coverlet)
- [ReportGenerator](https://github.com/danielpalme/ReportGenerator)
- [Vitest Coverage](https://vitest.dev/guide/coverage.html)
- [SonarQube](https://www.sonarsource.com/products/sonarqube/)

### 最佳實踐
- [Microsoft Code Coverage Best Practices](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-code-coverage)
- [Google Testing Blog](https://testing.googleblog.com/)

---

**文檔維護**: QA Team
**最後更新**: 2025-01-01
**狀態**: ✅ 完整
