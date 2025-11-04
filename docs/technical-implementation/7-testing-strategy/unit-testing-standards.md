# 單元測試標準 (Unit Testing Standards)

## 概述

本文檔定義 AI Workflow Platform 的單元測試標準、框架配置、最佳實踐和代碼示例,確保高質量的測試覆蓋率和可維護的測試代碼。

## 目標

- ✅ 建立統一的單元測試標準和規範
- ✅ 實現 60-75% 的單元測試覆蓋率
- ✅ 確保測試的可讀性和可維護性
- ✅ 支持快速反饋和持續集成
- ✅ 建立測試最佳實踐和模式

## 測試框架配置

### 後端測試框架 (.NET 8)

#### xUnit.net 項目配置

```xml
<!-- AIWorkflow.UnitTests/AIWorkflow.UnitTests.csproj -->
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  </PropertyGroup>

  <ItemGroup>
    <!-- xUnit 測試框架 -->
    <PackageReference Include="xunit" Version="2.6.6" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.6">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>

    <!-- 模擬框架 -->
    <PackageReference Include="Moq" Version="4.20.70" />
    <PackageReference Include="NSubstitute" Version="5.1.0" />

    <!-- 斷言庫 -->
    <PackageReference Include="FluentAssertions" Version="6.12.0" />

    <!-- 測試數據生成 -->
    <PackageReference Include="AutoFixture" Version="4.18.1" />
    <PackageReference Include="AutoFixture.Xunit2" Version="4.18.1" />
    <PackageReference Include="AutoFixture.AutoMoq" Version="4.18.1" />
    <PackageReference Include="Bogus" Version="35.4.0" />

    <!-- 覆蓋率工具 -->
    <PackageReference Include="coverlet.collector" Version="6.0.0">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="coverlet.msbuild" Version="6.0.0">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>

    <!-- Microsoft 測試平台 -->
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
  </ItemGroup>

  <ItemGroup>
    <!-- 引用被測試項目 -->
    <ProjectReference Include="..\AIWorkflow.Domain\AIWorkflow.Domain.csproj" />
    <ProjectReference Include="..\AIWorkflow.Application\AIWorkflow.Application.csproj" />
  </ItemGroup>

</Project>
```

#### 全局測試配置

```csharp
// AIWorkflow.UnitTests/GlobalUsings.cs
global using Xunit;
global using Moq;
global using FluentAssertions;
global using AutoFixture;
global using AutoFixture.Xunit2;
global using System;
global using System.Threading;
global using System.Threading.Tasks;
global using System.Collections.Generic;
global using System.Linq;
```

```csharp
// AIWorkflow.UnitTests/xunit.runner.json
{
  "methodDisplay": "method",
  "diagnosticMessages": false,
  "parallelizeAssembly": true,
  "parallelizeTestCollections": true,
  "maxParallelThreads": -1,
  "culture": "en-US"
}
```

### 前端測試框架

#### Vitest 配置 (React/Vue 通用)

```typescript
// vitest.config.ts
import { defineConfig } from 'vitest/config'
import react from '@vitejs/plugin-react'  // 或 vue()

export default defineConfig({
  plugins: [react()],
  test: {
    // 測試環境
    environment: 'jsdom',

    // 全局測試設置
    globals: true,

    // 覆蓋率配置
    coverage: {
      provider: 'v8',
      reporter: ['text', 'json', 'html', 'lcov'],
      exclude: [
        'node_modules/',
        'src/test-utils/',
        '**/*.d.ts',
        '**/*.config.*',
        '**/mockData/',
        'src/main.tsx'
      ],
      lines: 80,
      functions: 80,
      branches: 75,
      statements: 80
    },

    // 設置文件
    setupFiles: ['./src/test-utils/setup.ts'],

    // 並行執行
    threads: true,

    // 超時設置
    testTimeout: 10000
  }
})
```

```json
// package.json
{
  "devDependencies": {
    "@testing-library/react": "^14.1.2",
    "@testing-library/jest-dom": "^6.1.5",
    "@testing-library/user-event": "^14.5.1",
    "@vitest/coverage-v8": "^1.1.0",
    "vitest": "^1.1.0",
    "jsdom": "^23.0.1",
    "happy-dom": "^12.10.3"
  },
  "scripts": {
    "test": "vitest",
    "test:ui": "vitest --ui",
    "test:coverage": "vitest run --coverage",
    "test:watch": "vitest watch"
  }
}
```

## 測試命名規範

### 後端測試命名 (C#)

#### 測試類命名

**格式**: `{ClassUnderTest}Tests`

```csharp
// ✅ 正確命名
public sealed class WorkflowServiceTests { }
public sealed class CreateWorkflowCommandHandlerTests { }
public sealed class WorkflowRepositoryTests { }
public sealed class WorkflowValidatorTests { }

// ❌ 錯誤命名
public sealed class TestWorkflowService { }
public sealed class WorkflowTest { }
public sealed class WorkflowServiceTest { }
```

#### 測試方法命名

**格式**: `MethodName_Scenario_ExpectedResult`

```csharp
// ✅ 正確命名 - 清晰描述測試場景和預期結果
[Fact]
public async Task CreateWorkflow_WithValidData_ReturnsSuccessResult() { }

[Fact]
public async Task CreateWorkflow_WithDuplicateName_ReturnsValidationError() { }

[Fact]
public async Task CreateWorkflow_WithEmptyName_ThrowsArgumentException() { }

[Fact]
public async Task DeleteWorkflow_WhenWorkflowNotExists_ReturnsNotFoundError() { }

[Fact]
public async Task ExecuteWorkflow_WhenNodeHasNoConnections_SkipsNode() { }

// ❌ 錯誤命名 - 不清楚、太簡略
[Fact]
public async Task Test1() { }

[Fact]
public async Task CreateWorkflow() { }

[Fact]
public async Task CreateWorkflowTest() { }

[Fact]
public async Task ShouldCreateWorkflow() { }
```

### 前端測試命名 (TypeScript)

```typescript
// ✅ 正確命名 - 使用 describe 和 it 嵌套結構
describe('WorkflowService', () => {
  describe('createWorkflow', () => {
    it('should create workflow with valid data', async () => {
      // ...
    })

    it('should throw error when name is empty', async () => {
      // ...
    })

    it('should return validation error for duplicate name', async () => {
      // ...
    })
  })

  describe('deleteWorkflow', () => {
    it('should delete workflow successfully', async () => {
      // ...
    })

    it('should return not found error when workflow does not exist', async () => {
      // ...
    })
  })
})

// ❌ 錯誤命名
describe('test workflow service', () => {
  it('test1', () => { })
  it('creates workflow', () => { })
})
```

## AAA 模式 (Arrange-Act-Assert)

### 基本結構

```csharp
[Fact]
public async Task MethodName_Scenario_ExpectedResult()
{
    // Arrange (準備) - 設置測試數據和依賴
    var dependency = new Mock<IDependency>();
    var sut = new SystemUnderTest(dependency.Object);
    var input = new InputData { /* ... */ };

    // Act (執行) - 執行被測試的方法
    var result = await sut.MethodUnderTest(input);

    // Assert (斷言) - 驗證結果
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeTrue();
}
```

### 詳細示例 - 服務層測試

```csharp
namespace AIWorkflow.Application.UnitTests.Workflows;

public sealed class WorkflowServiceTests
{
    [Fact]
    public async Task CreateWorkflow_WithValidData_ReturnsSuccessResult()
    {
        // Arrange
        var mockRepository = new Mock<IWorkflowRepository>();
        var mockUserService = new Mock<ICurrentUserService>();
        var mockDateTimeService = new Mock<IDateTimeService>();
        var mockValidator = new Mock<IValidator<CreateWorkflowDto>>();

        var userId = 1;
        var now = new DateTime(2025, 1, 15, 10, 0, 0, DateTimeKind.Utc);

        mockUserService.Setup(s => s.UserId).Returns(userId);
        mockDateTimeService.Setup(s => s.UtcNow).Returns(now);
        mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<CreateWorkflowDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Workflow>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Workflow wf, CancellationToken ct) => wf);

        var service = new WorkflowService(
            mockRepository.Object,
            mockUserService.Object,
            mockDateTimeService.Object,
            mockValidator.Object);

        var createDto = new CreateWorkflowDto
        {
            Name = "Test Workflow",
            Description = "Test Description",
            Category = "Automation"
        };

        // Act
        var result = await service.CreateWorkflowAsync(createDto, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Name.Should().Be("Test Workflow");
        result.Value.Description.Should().Be("Test Description");
        result.Value.OwnerId.Should().Be(userId);
        result.Value.CreatedAt.Should().Be(now);
        result.Value.Status.Should().Be(WorkflowStatus.Draft);

        // 驗證依賴調用
        mockRepository.Verify(
            r => r.AddAsync(It.Is<Workflow>(w =>
                w.Name == "Test Workflow" &&
                w.OwnerId == userId &&
                w.CreatedAt == now
            ), It.IsAny<CancellationToken>()),
            Times.Once);

        mockValidator.Verify(
            v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateWorkflow_WithDuplicateName_ReturnsValidationError()
    {
        // Arrange
        var mockRepository = new Mock<IWorkflowRepository>();
        var mockUserService = new Mock<ICurrentUserService>();
        var mockDateTimeService = new Mock<IDateTimeService>();
        var mockValidator = new Mock<IValidator<CreateWorkflowDto>>();

        mockUserService.Setup(s => s.UserId).Returns(1);

        // 模擬驗證失敗
        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("Name", "工作流程名稱已存在")
        };
        mockValidator
            .Setup(v => v.ValidateAsync(It.IsAny<CreateWorkflowDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationFailures));

        var service = new WorkflowService(
            mockRepository.Object,
            mockUserService.Object,
            mockDateTimeService.Object,
            mockValidator.Object);

        var createDto = new CreateWorkflowDto
        {
            Name = "Duplicate Workflow",
            Description = "Test"
        };

        // Act
        var result = await service.CreateWorkflowAsync(createDto, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle();
        result.Errors.First().Should().Contain("已存在");

        // 驗證 Repository 沒有被調用
        mockRepository.Verify(
            r => r.AddAsync(It.IsAny<Workflow>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task DeleteWorkflow_WhenWorkflowExists_DeletesSuccessfully()
    {
        // Arrange
        var mockRepository = new Mock<IWorkflowRepository>();
        var mockUserService = new Mock<ICurrentUserService>();
        var mockDateTimeService = new Mock<IDateTimeService>();
        var mockValidator = new Mock<IValidator<CreateWorkflowDto>>();

        var workflowId = 42;
        var userId = 1;
        var workflow = new Workflow
        {
            Id = workflowId,
            Name = "Test Workflow",
            OwnerId = userId,
            IsDeleted = false
        };

        mockUserService.Setup(s => s.UserId).Returns(userId);
        mockRepository
            .Setup(r => r.GetByIdAsync(workflowId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workflow);
        mockRepository
            .Setup(r => r.DeleteAsync(workflow, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = new WorkflowService(
            mockRepository.Object,
            mockUserService.Object,
            mockDateTimeService.Object,
            mockValidator.Object);

        // Act
        var result = await service.DeleteWorkflowAsync(workflowId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        workflow.IsDeleted.Should().BeTrue();

        mockRepository.Verify(
            r => r.GetByIdAsync(workflowId, It.IsAny<CancellationToken>()),
            Times.Once);
        mockRepository.Verify(
            r => r.DeleteAsync(workflow, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteWorkflow_WhenWorkflowNotExists_ReturnsNotFoundError()
    {
        // Arrange
        var mockRepository = new Mock<IWorkflowRepository>();
        var mockUserService = new Mock<ICurrentUserService>();
        var mockDateTimeService = new Mock<IDateTimeService>();
        var mockValidator = new Mock<IValidator<CreateWorkflowDto>>();

        var workflowId = 999;

        mockRepository
            .Setup(r => r.GetByIdAsync(workflowId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Workflow?)null);

        var service = new WorkflowService(
            mockRepository.Object,
            mockUserService.Object,
            mockDateTimeService.Object,
            mockValidator.Object);

        // Act
        var result = await service.DeleteWorkflowAsync(workflowId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("找不到"));

        mockRepository.Verify(
            r => r.DeleteAsync(It.IsAny<Workflow>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
```

## 測試數據生成策略

### AutoFixture 使用

```csharp
// 基本使用
public sealed class WorkflowTests
{
    private readonly IFixture _fixture;

    public WorkflowTests()
    {
        _fixture = new Fixture();
        _fixture.Customize(new AutoMoqCustomization());
    }

    [Fact]
    public void Workflow_ShouldHaveValidProperties()
    {
        // AutoFixture 自動生成測試數據
        var workflow = _fixture.Create<Workflow>();

        workflow.Should().NotBeNull();
        workflow.Name.Should().NotBeNullOrEmpty();
        workflow.Id.Should().BeGreaterThan(0);
    }
}

// 使用 AutoData 特性簡化
public sealed class WorkflowCalculatorTests
{
    [Theory]
    [AutoData]
    public void Calculate_WithAutoGeneratedData_ReturnsCorrectResult(
        int input1,
        int input2,
        WorkflowCalculator sut)  // AutoFixture 自動創建和注入依賴
    {
        // Act
        var result = sut.Calculate(input1, input2);

        // Assert
        result.Should().Be(input1 + input2);
    }

    [Theory]
    [InlineAutoData(10)]
    [InlineAutoData(20)]
    [InlineAutoData(30)]
    public void Calculate_WithSpecificValue_ReturnsCorrectResult(
        int fixedValue,
        int autoValue,  // 自動生成
        WorkflowCalculator sut)
    {
        // ...
    }
}
```

### Bogus 假數據生成

```csharp
using Bogus;

public sealed class WorkflowTestDataBuilder
{
    private readonly Faker<Workflow> _workflowFaker;
    private readonly Faker<User> _userFaker;

    public WorkflowTestDataBuilder()
    {
        // 配置 User 假數據生成器
        _userFaker = new Faker<User>()
            .RuleFor(u => u.Id, f => f.Random.Int(1, 10000))
            .RuleFor(u => u.UserName, f => f.Internet.UserName())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
            .RuleFor(u => u.LastName, f => f.Name.LastName())
            .RuleFor(u => u.CreatedAt, f => f.Date.Past(2));

        // 配置 Workflow 假數據生成器
        _workflowFaker = new Faker<Workflow>()
            .RuleFor(w => w.Id, f => f.Random.Int(1, 10000))
            .RuleFor(w => w.Name, f => f.Commerce.ProductName())
            .RuleFor(w => w.Description, f => f.Lorem.Sentence(10))
            .RuleFor(w => w.Category, f => f.PickRandom("Automation", "Integration", "Analysis"))
            .RuleFor(w => w.Status, f => f.PickRandom<WorkflowStatus>())
            .RuleFor(w => w.OwnerId, f => f.Random.Int(1, 100))
            .RuleFor(w => w.CreatedAt, f => f.Date.Past(1))
            .RuleFor(w => w.UpdatedAt, (f, w) => f.Date.Between(w.CreatedAt, DateTime.UtcNow))
            .RuleFor(w => w.IsDeleted, f => f.Random.Bool(0.1f));  // 10% 機率為 true
    }

    public Workflow BuildWorkflow() => _workflowFaker.Generate();

    public List<Workflow> BuildWorkflows(int count) => _workflowFaker.Generate(count);

    public User BuildUser() => _userFaker.Generate();

    public List<User> BuildUsers(int count) => _userFaker.Generate(count);

    // 建造者模式 - 自定義特定屬性
    public Workflow BuildPublishedWorkflow()
    {
        return _workflowFaker
            .RuleFor(w => w.Status, WorkflowStatus.Published)
            .RuleFor(w => w.IsDeleted, false)
            .Generate();
    }

    public Workflow BuildDraftWorkflow()
    {
        return _workflowFaker
            .RuleFor(w => w.Status, WorkflowStatus.Draft)
            .RuleFor(w => w.IsDeleted, false)
            .Generate();
    }
}

// 使用示例
public sealed class WorkflowRepositoryTests
{
    private readonly WorkflowTestDataBuilder _testDataBuilder = new();

    [Fact]
    public async Task GetPublishedWorkflows_ReturnsOnlyPublished()
    {
        // Arrange
        var publishedWorkflows = _testDataBuilder.BuildPublishedWorkflow();
        var draftWorkflow = _testDataBuilder.BuildDraftWorkflow();

        // ... 測試邏輯
    }
}
```

## 模擬 (Mocking) 最佳實踐

### Moq 框架使用

```csharp
public sealed class CreateWorkflowCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_CreatesWorkflow()
    {
        // Arrange - 模擬依賴
        var mockRepository = new Mock<IWorkflowRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockPublisher = new Mock<IDomainEventPublisher>();

        // 設置 Repository 行為
        mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Workflow>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Workflow wf, CancellationToken ct) => wf);

        // 設置 UnitOfWork 行為
        mockUnitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // 驗證特定參數
        mockPublisher
            .Setup(p => p.PublishAsync(
                It.Is<WorkflowCreatedEvent>(e => e.WorkflowId > 0),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new CreateWorkflowCommandHandler(
            mockRepository.Object,
            mockUnitOfWork.Object,
            mockPublisher.Object);

        var command = new CreateWorkflowCommand
        {
            Name = "Test Workflow",
            Description = "Test"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        // 驗證方法被調用
        mockRepository.Verify(
            r => r.AddAsync(It.IsAny<Workflow>(), It.IsAny<CancellationToken>()),
            Times.Once);

        mockUnitOfWork.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        mockPublisher.Verify(
            p => p.PublishAsync(
                It.Is<WorkflowCreatedEvent>(e =>
                    e.WorkflowId > 0 &&
                    e.WorkflowName == "Test Workflow"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrows_ReturnsFailureResult()
    {
        // Arrange
        var mockRepository = new Mock<IWorkflowRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockPublisher = new Mock<IDomainEventPublisher>();

        // 模擬異常
        mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Workflow>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        var handler = new CreateWorkflowCommandHandler(
            mockRepository.Object,
            mockUnitOfWork.Object,
            mockPublisher.Object);

        var command = new CreateWorkflowCommand { Name = "Test" };

        // Act
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Database error");

        // 驗證 UnitOfWork 和 Publisher 沒有被調用
        mockUnitOfWork.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);

        mockPublisher.Verify(
            p => p.PublishAsync(It.IsAny<DomainEvent>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
```

### 模擬返回序列值

```csharp
[Fact]
public async Task ProcessWorkflows_CallsRepositoryMultipleTimes()
{
    // Arrange
    var mockRepository = new Mock<IWorkflowRepository>();

    // 設置序列返回值
    mockRepository
        .SetupSequence(r => r.GetNextWorkflowAsync(It.IsAny<CancellationToken>()))
        .ReturnsAsync(new Workflow { Id = 1, Name = "Workflow 1" })
        .ReturnsAsync(new Workflow { Id = 2, Name = "Workflow 2" })
        .ReturnsAsync(new Workflow { Id = 3, Name = "Workflow 3" })
        .ReturnsAsync((Workflow?)null);  // 第四次調用返回 null

    var processor = new WorkflowProcessor(mockRepository.Object);

    // Act
    var processedCount = await processor.ProcessAllAsync();

    // Assert
    processedCount.Should().Be(3);
    mockRepository.Verify(
        r => r.GetNextWorkflowAsync(It.IsAny<CancellationToken>()),
        Times.Exactly(4));  // 調用 4 次(3 個工作流程 + 1 次 null)
}
```

## 領域邏輯測試

### 實體測試

```csharp
public sealed class WorkflowEntityTests
{
    [Fact]
    public void Workflow_Create_ShouldInitializeWithCorrectDefaults()
    {
        // Arrange & Act
        var workflow = Workflow.Create(
            name: "Test Workflow",
            description: "Test Description",
            ownerId: 1);

        // Assert
        workflow.Should().NotBeNull();
        workflow.Name.Should().Be("Test Workflow");
        workflow.Description.Should().Be("Test Description");
        workflow.OwnerId.Should().Be(1);
        workflow.Status.Should().Be(WorkflowStatus.Draft);
        workflow.IsDeleted.Should().BeFalse();
        workflow.Version.Should().Be(1);
        workflow.Nodes.Should().BeEmpty();
        workflow.Edges.Should().BeEmpty();
    }

    [Fact]
    public void Workflow_Publish_ShouldChangeStatusToPublished()
    {
        // Arrange
        var workflow = Workflow.Create("Test", "Desc", 1);

        // Act
        workflow.Publish();

        // Assert
        workflow.Status.Should().Be(WorkflowStatus.Published);
    }

    [Fact]
    public void Workflow_Publish_WhenAlreadyPublished_ThrowsInvalidOperationException()
    {
        // Arrange
        var workflow = Workflow.Create("Test", "Desc", 1);
        workflow.Publish();

        // Act
        Action act = () => workflow.Publish();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*已經發布*");
    }

    [Fact]
    public void Workflow_AddNode_ShouldAddNodeToCollection()
    {
        // Arrange
        var workflow = Workflow.Create("Test", "Desc", 1);
        var node = WorkflowNode.Create("Node1", "start", new { });

        // Act
        workflow.AddNode(node);

        // Assert
        workflow.Nodes.Should().ContainSingle();
        workflow.Nodes.First().Should().Be(node);
    }

    [Fact]
    public void Workflow_AddNode_WithDuplicateId_ThrowsInvalidOperationException()
    {
        // Arrange
        var workflow = Workflow.Create("Test", "Desc", 1);
        var node1 = WorkflowNode.Create("Node1", "start", new { });
        var node2 = WorkflowNode.Create("Node1", "end", new { });

        workflow.AddNode(node1);

        // Act
        Action act = () => workflow.AddNode(node2);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*節點 ID 已存在*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Workflow_Create_WithInvalidName_ThrowsArgumentException(string invalidName)
    {
        // Act
        Action act = () => Workflow.Create(invalidName, "Desc", 1);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("name");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Workflow_Create_WithInvalidOwnerId_ThrowsArgumentException(int invalidOwnerId)
    {
        // Act
        Action act = () => Workflow.Create("Test", "Desc", invalidOwnerId);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("ownerId");
    }
}
```

### 值對象測試

```csharp
public sealed class EmailAddressTests
{
    [Theory]
    [InlineData("user@example.com")]
    [InlineData("test.user@company.com")]
    [InlineData("admin+test@domain.co.uk")]
    public void EmailAddress_WithValidFormat_ShouldCreateSuccessfully(string validEmail)
    {
        // Act
        var email = EmailAddress.Create(validEmail);

        // Assert
        email.Should().NotBeNull();
        email.Value.Should().Be(validEmail.ToLowerInvariant());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("invalid")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    [InlineData("user @example.com")]
    public void EmailAddress_WithInvalidFormat_ShouldThrowArgumentException(string invalidEmail)
    {
        // Act
        Action act = () => EmailAddress.Create(invalidEmail);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*無效的電子郵件格式*");
    }

    [Fact]
    public void EmailAddress_Equals_WithSameValue_ShouldReturnTrue()
    {
        // Arrange
        var email1 = EmailAddress.Create("test@example.com");
        var email2 = EmailAddress.Create("TEST@example.com");  // 不區分大小寫

        // Act & Assert
        email1.Should().Be(email2);
        (email1 == email2).Should().BeTrue();
        email1.GetHashCode().Should().Be(email2.GetHashCode());
    }

    [Fact]
    public void EmailAddress_Equals_WithDifferentValue_ShouldReturnFalse()
    {
        // Arrange
        var email1 = EmailAddress.Create("test1@example.com");
        var email2 = EmailAddress.Create("test2@example.com");

        // Act & Assert
        email1.Should().NotBe(email2);
        (email1 != email2).Should().BeTrue();
    }
}
```

## 前端測試示例

### React 組件測試

```typescript
// WorkflowCard.test.tsx
import { render, screen, fireEvent, waitFor } from '@testing-library/react'
import { describe, it, expect, vi, beforeEach } from 'vitest'
import userEvent from '@testing-library/user-event'
import { WorkflowCard } from './WorkflowCard'
import type { Workflow } from '@/types'

describe('WorkflowCard', () => {
  const mockWorkflow: Workflow = {
    id: 1,
    name: 'Test Workflow',
    description: 'Test Description',
    status: 'Published',
    category: 'Automation',
    ownerId: 1,
    createdAt: '2025-01-01T00:00:00Z',
    updatedAt: '2025-01-15T00:00:00Z'
  }

  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('should render workflow information correctly', () => {
    // Arrange & Act
    render(<WorkflowCard workflow={mockWorkflow} />)

    // Assert
    expect(screen.getByText('Test Workflow')).toBeInTheDocument()
    expect(screen.getByText('Test Description')).toBeInTheDocument()
    expect(screen.getByText('Published')).toBeInTheDocument()
    expect(screen.getByText('Automation')).toBeInTheDocument()
  })

  it('should call onEdit when edit button is clicked', async () => {
    // Arrange
    const onEdit = vi.fn()
    render(<WorkflowCard workflow={mockWorkflow} onEdit={onEdit} />)

    // Act
    const editButton = screen.getByRole('button', { name: /edit/i })
    await userEvent.click(editButton)

    // Assert
    expect(onEdit).toHaveBeenCalledTimes(1)
    expect(onEdit).toHaveBeenCalledWith(mockWorkflow.id)
  })

  it('should call onDelete when delete button is clicked', async () => {
    // Arrange
    const onDelete = vi.fn()
    render(<WorkflowCard workflow={mockWorkflow} onDelete={onDelete} />)

    // Act
    const deleteButton = screen.getByRole('button', { name: /delete/i })
    await userEvent.click(deleteButton)

    // Assert
    expect(onDelete).toHaveBeenCalledTimes(1)
    expect(onDelete).toHaveBeenCalledWith(mockWorkflow.id)
  })

  it('should show confirmation dialog before deleting', async () => {
    // Arrange
    const onDelete = vi.fn()
    render(<WorkflowCard workflow={mockWorkflow} onDelete={onDelete} />)

    // Act
    const deleteButton = screen.getByRole('button', { name: /delete/i })
    await userEvent.click(deleteButton)

    // Assert - 確認對話框出現
    await waitFor(() => {
      expect(screen.getByText(/確定要刪除/i)).toBeInTheDocument()
    })

    // Act - 點擊確認
    const confirmButton = screen.getByRole('button', { name: /確定/i })
    await userEvent.click(confirmButton)

    // Assert - onDelete 被調用
    expect(onDelete).toHaveBeenCalledWith(mockWorkflow.id)
  })

  it('should display status badge with correct color', () => {
    // Test Published status
    const { rerender } = render(
      <WorkflowCard workflow={{ ...mockWorkflow, status: 'Published' }} />
    )
    expect(screen.getByText('Published')).toHaveClass('badge-success')

    // Test Draft status
    rerender(<WorkflowCard workflow={{ ...mockWorkflow, status: 'Draft' }} />)
    expect(screen.getByText('Draft')).toHaveClass('badge-warning')

    // Test Archived status
    rerender(<WorkflowCard workflow={{ ...mockWorkflow, status: 'Archived' }} />)
    expect(screen.getByText('Archived')).toHaveClass('badge-secondary')
  })
})
```

### 自定義 Hook 測試

```typescript
// useWorkflows.test.ts
import { renderHook, waitFor } from '@testing-library/react'
import { describe, it, expect, vi, beforeEach } from 'vitest'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { useWorkflows } from './useWorkflows'
import { workflowApi } from '@/api/workflow'

// Mock API
vi.mock('@/api/workflow', () => ({
  workflowApi: {
    getWorkflows: vi.fn()
  }
}))

describe('useWorkflows', () => {
  let queryClient: QueryClient

  beforeEach(() => {
    queryClient = new QueryClient({
      defaultOptions: {
        queries: { retry: false }
      }
    })
    vi.clearAllMocks()
  })

  const wrapper = ({ children }: { children: React.ReactNode }) => (
    <QueryClientProvider client={queryClient}>
      {children}
    </QueryClientProvider>
  )

  it('should fetch workflows successfully', async () => {
    // Arrange
    const mockWorkflows = [
      { id: 1, name: 'Workflow 1', status: 'Published' },
      { id: 2, name: 'Workflow 2', status: 'Draft' }
    ]

    vi.mocked(workflowApi.getWorkflows).mockResolvedValue({
      data: mockWorkflows,
      total: 2,
      page: 1,
      pageSize: 10
    })

    // Act
    const { result } = renderHook(() => useWorkflows({ page: 1, pageSize: 10 }), { wrapper })

    // Assert - 初始狀態
    expect(result.current.isLoading).toBe(true)
    expect(result.current.data).toBeUndefined()

    // Assert - 加載完成後
    await waitFor(() => {
      expect(result.current.isLoading).toBe(false)
    })

    expect(result.current.data).toEqual(mockWorkflows)
    expect(result.current.isSuccess).toBe(true)
    expect(workflowApi.getWorkflows).toHaveBeenCalledWith({ page: 1, pageSize: 10 })
  })

  it('should handle error when fetching workflows fails', async () => {
    // Arrange
    const errorMessage = 'Network error'
    vi.mocked(workflowApi.getWorkflows).mockRejectedValue(new Error(errorMessage))

    // Act
    const { result } = renderHook(() => useWorkflows({ page: 1 }), { wrapper })

    // Assert
    await waitFor(() => {
      expect(result.current.isError).toBe(true)
    })

    expect(result.current.error).toBeDefined()
    expect(result.current.error?.message).toBe(errorMessage)
    expect(result.current.data).toBeUndefined()
  })
})
```

## 檢查清單

### 測試編寫
- [ ] 所有公共方法都有單元測試
- [ ] 測試覆蓋所有主要執行路徑
- [ ] 測試覆蓋邊界條件和異常情況
- [ ] 測試命名清晰描述場景和預期結果
- [ ] 使用 AAA 模式組織測試代碼

### 測試質量
- [ ] 測試獨立且可重複執行
- [ ] 測試不依賴外部系統(數據庫、網絡)
- [ ] Mock 使用合理且不過度
- [ ] 斷言精確且有意義
- [ ] 測試執行速度快(毫秒級)

### 維護性
- [ ] 測試代碼清晰易讀
- [ ] 避免測試代碼重複
- [ ] 使用測試輔助方法和建造者模式
- [ ] 測試數據生成自動化
- [ ] 定期重構測試代碼

## 參考資源

### 官方文檔
- [xUnit.net Documentation](https://xunit.net/)
- [Moq Documentation](https://github.com/moq/moq4)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [AutoFixture Documentation](https://github.com/AutoFixture/AutoFixture)
- [Vitest Documentation](https://vitest.dev/)
- [Testing Library](https://testing-library.com/)

### 最佳實踐
- [Unit Testing Best Practices - Microsoft](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)
- [The Art of Unit Testing](https://www.artofunittesting.com/)
- [xUnit Test Patterns](http://xunitpatterns.com/)

### 書籍
- *Unit Testing Principles, Practices, and Patterns* by Vladimir Khorikov
- *The Art of Unit Testing* by Roy Osherove
- *Growing Object-Oriented Software, Guided by Tests* by Steve Freeman

---

**文檔維護**: QA Team / Development Team
**最後更新**: 2025-01-01
**狀態**: ✅ 完整
