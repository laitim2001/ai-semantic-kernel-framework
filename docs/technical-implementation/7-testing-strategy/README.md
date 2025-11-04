# Phase 7: 測試策略 (Testing Strategy)

## 階段概述

本階段定義 AI Workflow Platform 的完整測試策略,包括單元測試、集成測試、端到端測試、性能測試和測試自動化最佳實踐。

## 目標

- ✅ 建立多層次測試金字塔
- ✅ 實現高測試覆蓋率 (>80%)
- ✅ 確保代碼質量和可維護性
- ✅ 支持持續集成/持續部署 (CI/CD)
- ✅ 建立自動化測試流程

## 文檔結構

### 1. 單元測試標準
**文件**: `unit-testing-standards.md`

**內容**:
- xUnit.net 測試框架配置
- Moq 模擬框架使用
- FluentAssertions 斷言庫
- AAA (Arrange-Act-Assert) 模式
- 測試命名規範
- 測試數據生成策略
- 領域邏輯測試
- 服務層測試
- Repository 測試

**技術重點**:
- xUnit.net 2.6
- Moq 4.20
- FluentAssertions 6.12
- AutoFixture 4.18
- Bogus (測試數據生成)

### 2. 集成測試標準
**文件**: `integration-testing-standards.md`

**內容**:
- WebApplicationFactory 配置
- TestContainers 使用 (PostgreSQL, Redis)
- 數據庫集成測試
- API 端點測試
- Repository 集成測試
- 外部服務模擬
- 測試隔離策略

**技術重點**:
- Microsoft.AspNetCore.Mvc.Testing
- Testcontainers.PostgreSQL
- Testcontainers.Redis
- Respawn (數據庫清理)

### 3. 端到端測試標準
**文件**: `end-to-end-testing-standards.md`

**內容**:
- Playwright for .NET 配置
- 用戶場景測試
- 工作流程測試
- 跨瀏覽器測試
- 視覺回歸測試
- 無障礙測試 (Accessibility)
- 性能測試

**技術重點**:
- Microsoft.Playwright 1.40
- Playwright Test Runner
- 跨瀏覽器支持 (Chromium, Firefox, WebKit)
- 截圖和視頻錄製

### 4. 測試覆蓋率策略
**文件**: `test-coverage-strategy.md`

**內容**:
- 覆蓋率目標定義
- Coverlet 配置
- ReportGenerator 報告生成
- SonarQube 集成
- 覆蓋率分析和改進
- CI/CD 覆蓋率閾值
- 代碼質量指標

**技術重點**:
- Coverlet.MSBuild
- ReportGenerator
- SonarQube Scanner
- 覆蓋率目標: >80%

### 5. 測試自動化與 CI/CD
**文件**: `test-automation-cicd.md`

**內容**:
- GitHub Actions 測試工作流程
- Azure DevOps Pipeline
- 並行測試執行
- 測試報告生成
- 失敗測試通知
- 測試數據管理
- 測試環境配置

**技術重點**:
- GitHub Actions
- Azure DevOps
- xUnit HTML Reporter
- Test Retry 策略

## 測試金字塔

```
           /\
          /  \
         / E2E \        少量 (5-10%)
        /--------\
       /          \
      / Integration \   中等 (20-30%)
     /--------------\
    /                \
   /   Unit Tests     \  大量 (60-75%)
  /____________________\
```

### 測試分層策略

| 測試類型 | 數量佔比 | 執行速度 | 覆蓋範圍 | 維護成本 |
|---------|---------|---------|---------|---------|
| 單元測試 | 60-75% | 快 (毫秒) | 單個類/方法 | 低 |
| 集成測試 | 20-30% | 中 (秒) | 多個組件 | 中 |
| E2E 測試 | 5-10% | 慢 (分鐘) | 整個系統 | 高 |

## 測試框架與工具

### 後端測試 (.NET)

```xml
<!-- AIWorkflow.UnitTests/AIWorkflow.UnitTests.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <!-- 測試框架 -->
    <PackageReference Include="xunit" Version="2.6.6" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.6" />

    <!-- 模擬框架 -->
    <PackageReference Include="Moq" Version="4.20.70" />

    <!-- 斷言庫 -->
    <PackageReference Include="FluentAssertions" Version="6.12.0" />

    <!-- 測試數據生成 -->
    <PackageReference Include="AutoFixture" Version="4.18.1" />
    <PackageReference Include="AutoFixture.Xunit2" Version="4.18.1" />
    <PackageReference Include="Bogus" Version="35.4.0" />

    <!-- 覆蓋率 -->
    <PackageReference Include="coverlet.msbuild" Version="6.0.0" />
  </ItemGroup>
</Project>
```

### 集成測試

```xml
<!-- AIWorkflow.IntegrationTests/AIWorkflow.IntegrationTests.csproj -->
<ItemGroup>
  <!-- Web 測試 -->
  <PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="8.0.0" />

  <!-- TestContainers -->
  <PackageReference Include="Testcontainers.PostgreSql" Version="3.7.0" />
  <PackageReference Include="Testcontainers.Redis" Version="3.7.0" />

  <!-- 數據庫重置 -->
  <PackageReference Include="Respawn" Version="6.2.0" />

  <!-- HTTP 客戶端測試 -->
  <PackageReference Include="Microsoft.Extensions.Http" Version="8.0.0" />
</ItemGroup>
```

### 前端測試

```json
// package.json
{
  "devDependencies": {
    // React 測試
    "@testing-library/react": "^14.1.2",
    "@testing-library/jest-dom": "^6.1.5",
    "@testing-library/user-event": "^14.5.1",
    "vitest": "^1.1.0",
    "jsdom": "^23.0.1",

    // Vue 測試
    "@vue/test-utils": "^2.4.3",
    "vitest": "^1.1.0",
    "happy-dom": "^12.10.3",

    // E2E 測試
    "@playwright/test": "^1.40.1",

    // 覆蓋率
    "@vitest/coverage-v8": "^1.1.0"
  }
}
```

## 測試命名規範

### 單元測試命名

**格式**: `MethodName_Scenario_ExpectedResult`

```csharp
// ✅ 好的命名
[Fact]
public async Task CreateWorkflow_WithValidData_ReturnsSuccessResult()
{
    // ...
}

[Fact]
public async Task CreateWorkflow_WithDuplicateName_ReturnsValidationError()
{
    // ...
}

[Fact]
public async Task DeleteWorkflow_WhenWorkflowNotExists_ReturnsNotFoundError()
{
    // ...
}

// ❌ 不好的命名
[Fact]
public async Task Test1() { }

[Fact]
public async Task CreateWorkflowTest() { }
```

### 測試類命名

**格式**: `{ClassUnderTest}Tests`

```csharp
// 測試 WorkflowService
public sealed class WorkflowServiceTests
{
    // ...
}

// 測試 CreateWorkflowCommandHandler
public sealed class CreateWorkflowCommandHandlerTests
{
    // ...
}
```

## 單元測試示例

### AAA 模式

```csharp
public sealed class WorkflowServiceTests
{
    [Fact]
    public async Task CreateWorkflow_WithValidData_ReturnsSuccessResult()
    {
        // Arrange
        var mockRepository = new Mock<IWorkflowRepository>();
        var mockUserService = new Mock<ICurrentUserService>();
        mockUserService.Setup(s => s.UserId).Returns(1);

        var service = new WorkflowService(
            mockRepository.Object,
            mockUserService.Object);

        var createDto = new CreateWorkflowDto
        {
            Name = "Test Workflow",
            Description = "Test Description"
        };

        // Act
        var result = await service.CreateWorkflowAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeGreaterThan(0);

        mockRepository.Verify(
            r => r.AddAsync(It.IsAny<Workflow>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
```

## 集成測試示例

### WebApplicationFactory

```csharp
public sealed class WorkflowApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public WorkflowApiTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetWorkflows_ReturnsSuccessStatusCode()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/workflows?page=1&pageSize=10");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<PagedResult<WorkflowDto>>(content);

        result.Should().NotBeNull();
        result!.Data.Should().NotBeEmpty();
    }
}
```

## 前端測試示例

### React 組件測試

```typescript
// WorkflowCard.test.tsx
import { render, screen, fireEvent } from '@testing-library/react'
import { describe, it, expect, vi } from 'vitest'
import { WorkflowCard } from './WorkflowCard'

describe('WorkflowCard', () => {
  it('should render workflow information correctly', () => {
    const workflow = {
      id: 1,
      name: 'Test Workflow',
      description: 'Test Description',
      status: 'Published'
    }

    render(<WorkflowCard workflow={workflow} />)

    expect(screen.getByText('Test Workflow')).toBeInTheDocument()
    expect(screen.getByText('Test Description')).toBeInTheDocument()
    expect(screen.getByText('Published')).toBeInTheDocument()
  })

  it('should call onDelete when delete button clicked', async () => {
    const onDelete = vi.fn()
    const workflow = { id: 1, name: 'Test' }

    render(<WorkflowCard workflow={workflow} onDelete={onDelete} />)

    const deleteButton = screen.getByRole('button', { name: /delete/i })
    fireEvent.click(deleteButton)

    expect(onDelete).toHaveBeenCalledWith(1)
  })
})
```

## 測試執行命令

### 後端測試

```bash
# 運行所有測試
dotnet test

# 運行單元測試
dotnet test --filter "FullyQualifiedName~UnitTests"

# 運行集成測試
dotnet test --filter "FullyQualifiedName~IntegrationTests"

# 生成覆蓋率報告
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# 生成 HTML 報告
reportgenerator \
  -reports:coverage.opencover.xml \
  -targetdir:coveragereport \
  -reporttypes:Html
```

### 前端測試

```bash
# 運行所有測試
npm test

# 運行測試並生成覆蓋率
npm run test:coverage

# 運行 E2E 測試
npx playwright test

# 運行特定瀏覽器的 E2E 測試
npx playwright test --project=chromium
```

## 測試覆蓋率目標

### 覆蓋率閾值

| 層級 | 最低覆蓋率 | 目標覆蓋率 | 說明 |
|------|----------|----------|------|
| Domain 層 | 90% | 95% | 核心業務邏輯 |
| Application 層 | 80% | 90% | 應用服務和用例 |
| Infrastructure 層 | 70% | 80% | 數據訪問和外部服務 |
| API 層 | 75% | 85% | 端點和控制器 |
| **整體** | **80%** | **90%** | 整個專案 |

## CI/CD 集成

### GitHub Actions 工作流程

```yaml
name: Test & Coverage

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main, develop]

jobs:
  test:
    runs-on: ubuntu-latest

    services:
      postgres:
        image: postgres:16
        env:
          POSTGRES_DB: aiworkflow_test
          POSTGRES_USER: postgres
          POSTGRES_PASSWORD: test_password
        ports:
          - 5432:5432

    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'

      - name: Restore dependencies
        run: dotnet restore

      - name: Build
        run: dotnet build --no-restore

      - name: Run Unit Tests
        run: dotnet test --no-build --filter "FullyQualifiedName~UnitTests" --verbosity normal

      - name: Run Integration Tests
        run: dotnet test --no-build --filter "FullyQualifiedName~IntegrationTests" --verbosity normal

      - name: Generate Coverage Report
        run: |
          dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
          dotnet tool install -g dotnet-reportgenerator-globaltool
          reportgenerator -reports:coverage.opencover.xml -targetdir:coveragereport -reporttypes:Html

      - name: Upload Coverage Report
        uses: codecov/codecov-action@v3
        with:
          files: ./coverage.opencover.xml

      - name: Check Coverage Threshold
        run: |
          COVERAGE=$(grep -oP 'line-rate="\K[^"]+' coverage.opencover.xml | head -1)
          if (( $(echo "$COVERAGE < 0.80" | bc -l) )); then
            echo "Coverage $COVERAGE is below 80% threshold"
            exit 1
          fi
```

## 檢查清單

### 測試實施
- [ ] 建立單元測試框架和標準
- [ ] 實現集成測試基礎設施
- [ ] 配置 E2E 測試環境
- [ ] 設置測試覆蓋率工具
- [ ] 集成 CI/CD 測試流程

### 測試質量
- [ ] 達到 80% 以上測試覆蓋率
- [ ] 所有關鍵業務邏輯有單元測試
- [ ] API 端點有集成測試
- [ ] 主要用戶場景有 E2E 測試
- [ ] 測試穩定且可重複

### 自動化
- [ ] CI 自動運行所有測試
- [ ] 測試失敗時自動通知
- [ ] 自動生成測試報告
- [ ] 覆蓋率低於閾值時構建失敗
- [ ] 定期運行性能測試

## 相關文檔

- [Phase 4: 編碼標準](../4-coding-standards/README.md)
- [Phase 8: 部署架構](../8-deployment-architecture/README.md)
- [Phase 10: 監控與運維](../10-monitoring-operations/README.md)

## 參考資源

### 官方文檔
- [xUnit.net Documentation](https://xunit.net/)
- [Moq Documentation](https://github.com/moq/moq4)
- [Playwright for .NET](https://playwright.dev/dotnet/)
- [Testing Library](https://testing-library.com/)

### 最佳實踐
- [Unit Testing Best Practices](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)
- [Integration Testing in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests)
- [Test Pyramid](https://martinfowler.com/articles/practical-test-pyramid.html)

### 書籍
- *Unit Testing Principles, Practices, and Patterns* by Vladimir Khorikov
- *The Art of Unit Testing* by Roy Osherove
- *xUnit Test Patterns* by Gerard Meszaros

---

**文檔維護**: QA Team
**最後更新**: 2025-01-01
**狀態**: ✅ 完整
