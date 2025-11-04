# 集成測試標準 (Integration Testing Standards)

## 概述

本文檔定義 AI Workflow Platform 的集成測試標準,涵蓋 API 端點測試、數據庫集成測試、外部服務集成測試和測試容器化策略,確保系統組件之間的正確集成。

## 目標

- ✅ 實現 20-30% 的集成測試覆蓋率
- ✅ 驗證組件之間的正確交互
- ✅ 測試真實的數據庫和外部依賴
- ✅ 確保 API 契約的正確性
- ✅ 支持快速且可靠的集成測試執行

## 集成測試框架配置

### 後端集成測試 (.NET 8)

#### 項目配置

```xml
<!-- AIWorkflow.IntegrationTests/AIWorkflow.IntegrationTests.csproj -->
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <!-- xUnit 測試框架 -->
    <PackageReference Include="xunit" Version="2.6.6" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.6" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />

    <!-- ASP.NET Core 測試支持 -->
    <PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="8.0.0" />

    <!-- TestContainers - 容器化測試環境 -->
    <PackageReference Include="Testcontainers" Version="3.7.0" />
    <PackageReference Include="Testcontainers.PostgreSql" Version="3.7.0" />
    <PackageReference Include="Testcontainers.Redis" Version="3.7.0" />
    <PackageReference Include="Testcontainers.MongoDb" Version="3.7.0" />

    <!-- 數據庫測試工具 -->
    <PackageReference Include="Respawn" Version="6.2.0" />
    <PackageReference Include="Npgsql" Version="8.0.1" />

    <!-- HTTP 客戶端測試 -->
    <PackageReference Include="Microsoft.Extensions.Http" Version="8.0.0" />

    <!-- 斷言和模擬 -->
    <PackageReference Include="FluentAssertions" Version="6.12.0" />
    <PackageReference Include="Moq" Version="4.20.70" />

    <!-- 覆蓋率 -->
    <PackageReference Include="coverlet.collector" Version="6.0.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\AIWorkflow.API\AIWorkflow.API.csproj" />
    <ProjectReference Include="..\AIWorkflow.Infrastructure\AIWorkflow.Infrastructure.csproj" />
  </ItemGroup>

</Project>
```

### WebApplicationFactory 自定義配置

```csharp
// AIWorkflow.IntegrationTests/Infrastructure/CustomWebApplicationFactory.cs
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace AIWorkflow.IntegrationTests.Infrastructure;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer;
    private readonly RedisContainer _redisContainer;

    public CustomWebApplicationFactory()
    {
        // 配置 PostgreSQL 容器
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("aiworkflow_test")
            .WithUsername("test_user")
            .WithPassword("test_password")
            .WithPortBinding(5432, true)
            .WithCleanUp(true)
            .Build();

        // 配置 Redis 容器
        _redisContainer = new RedisBuilder()
            .WithImage("redis:7-alpine")
            .WithPortBinding(6379, true)
            .WithCleanUp(true)
            .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // 移除原有的 DbContext 配置
            services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));

            // 使用 TestContainers PostgreSQL
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(_postgresContainer.GetConnectionString());
            });

            // 使用 TestContainers Redis
            services.RemoveAll(typeof(IConnectionMultiplexer));
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var configuration = ConfigurationOptions.Parse(_redisContainer.GetConnectionString());
                return ConnectionMultiplexer.Connect(configuration);
            });

            // 覆蓋外部服務為 Mock
            services.RemoveAll(typeof(IAzureOpenAIService));
            services.AddScoped<IAzureOpenAIService, MockAzureOpenAIService>();

            // 禁用後台服務(避免干擾測試)
            services.RemoveAll(typeof(IHostedService));
        });

        builder.UseEnvironment("Testing");
    }

    public async Task InitializeAsync()
    {
        // 啟動容器
        await _postgresContainer.StartAsync();
        await _redisContainer.StartAsync();

        // 執行數據庫遷移
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        // 停止和清理容器
        await _postgresContainer.DisposeAsync();
        await _redisContainer.DisposeAsync();
        await base.DisposeAsync();
    }
}
```

### 測試基類

```csharp
// AIWorkflow.IntegrationTests/Infrastructure/IntegrationTestBase.cs
using Respawn;
using Npgsql;

namespace AIWorkflow.IntegrationTests.Infrastructure;

public abstract class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    protected readonly CustomWebApplicationFactory Factory;
    protected readonly HttpClient Client;
    private Respawner _respawner = default!;
    private NpgsqlConnection _dbConnection = default!;

    protected IntegrationTestBase(CustomWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        // 創建數據庫連接
        _dbConnection = new NpgsqlConnection(GetConnectionString());
        await _dbConnection.OpenAsync();

        // 初始化 Respawner (用於重置數據庫)
        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = new[] { "workflow", "identity", "execution", "audit" },
            TablesToIgnore = new Respawn.Graph.Table[] { "__EFMigrationsHistory" }
        });
    }

    public async Task DisposeAsync()
    {
        // 重置數據庫到乾淨狀態
        await _respawner.ResetAsync(_dbConnection);
        await _dbConnection.CloseAsync();
        await _dbConnection.DisposeAsync();
    }

    protected string GetConnectionString()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return dbContext.Database.GetConnectionString()!;
    }

    protected async Task<T> ExecuteDbContextAsync<T>(Func<ApplicationDbContext, Task<T>> action)
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await action(dbContext);
    }

    protected async Task ExecuteDbContextAsync(Func<ApplicationDbContext, Task> action)
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await action(dbContext);
    }

    protected async Task<T> SendAsync<T>(IRequest<T> request)
    {
        using var scope = Factory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        return await mediator.Send(request);
    }
}
```

## API 端點集成測試

### 完整的 CRUD 測試示例

```csharp
// AIWorkflow.IntegrationTests/API/WorkflowsControllerTests.cs
using System.Net;
using System.Net.Http.Json;

namespace AIWorkflow.IntegrationTests.API;

public sealed class WorkflowsControllerTests : IntegrationTestBase
{
    public WorkflowsControllerTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task GetWorkflows_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await Client.GetAsync("/api/v1/workflows?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetWorkflows_WithValidAuthentication_ReturnsOkWithData()
    {
        // Arrange
        await SeedTestDataAsync();
        var token = await GetAuthTokenAsync("test@example.com", "Test@123");
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await Client.GetAsync("/api/v1/workflows?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<WorkflowDto>>();
        result.Should().NotBeNull();
        result!.Data.Should().NotBeEmpty();
        result.Total.Should().BeGreaterThan(0);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task GetWorkflowById_WhenWorkflowExists_ReturnsOkWithWorkflow()
    {
        // Arrange
        var workflowId = await CreateTestWorkflowAsync();
        var token = await GetAuthTokenAsync("test@example.com", "Test@123");
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await Client.GetAsync($"/api/v1/workflows/{workflowId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var workflow = await response.Content.ReadFromJsonAsync<WorkflowDto>();
        workflow.Should().NotBeNull();
        workflow!.Id.Should().Be(workflowId);
        workflow.Name.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetWorkflowById_WhenWorkflowNotExists_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthTokenAsync("test@example.com", "Test@123");
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await Client.GetAsync("/api/v1/workflows/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateWorkflow_WithValidData_ReturnsCreatedWithLocation()
    {
        // Arrange
        var token = await GetAuthTokenAsync("test@example.com", "Test@123");
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateWorkflowDto
        {
            Name = "Integration Test Workflow",
            Description = "Created in integration test",
            Category = "Automation"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/workflows", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var workflow = await response.Content.ReadFromJsonAsync<WorkflowDto>();
        workflow.Should().NotBeNull();
        workflow!.Name.Should().Be(createDto.Name);
        workflow.Description.Should().Be(createDto.Description);
        workflow.Status.Should().Be("Draft");

        // 驗證數據庫中確實創建了記錄
        var dbWorkflow = await ExecuteDbContextAsync(async db =>
            await db.Workflows.FirstOrDefaultAsync(w => w.Id == workflow.Id));

        dbWorkflow.Should().NotBeNull();
        dbWorkflow!.Name.Should().Be(createDto.Name);
    }

    [Fact]
    public async Task CreateWorkflow_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var token = await GetAuthTokenAsync("test@example.com", "Test@123");
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateWorkflowDto
        {
            Name = "",  // 無效:空名稱
            Description = "Test"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/workflows", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().ContainKey("Name");
    }

    [Fact]
    public async Task UpdateWorkflow_WithValidData_ReturnsOk()
    {
        // Arrange
        var workflowId = await CreateTestWorkflowAsync();
        var token = await GetAuthTokenAsync("test@example.com", "Test@123");
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateWorkflowDto
        {
            Name = "Updated Workflow Name",
            Description = "Updated Description",
            Category = "Integration"
        };

        // Act
        var response = await Client.PutAsJsonAsync($"/api/v1/workflows/{workflowId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var workflow = await response.Content.ReadFromJsonAsync<WorkflowDto>();
        workflow.Should().NotBeNull();
        workflow!.Name.Should().Be(updateDto.Name);
        workflow.Description.Should().Be(updateDto.Description);

        // 驗證數據庫更新
        var dbWorkflow = await ExecuteDbContextAsync(async db =>
            await db.Workflows.FirstOrDefaultAsync(w => w.Id == workflowId));

        dbWorkflow!.Name.Should().Be(updateDto.Name);
        dbWorkflow.UpdatedAt.Should().BeAfter(dbWorkflow.CreatedAt);
    }

    [Fact]
    public async Task DeleteWorkflow_WhenWorkflowExists_ReturnsNoContent()
    {
        // Arrange
        var workflowId = await CreateTestWorkflowAsync();
        var token = await GetAuthTokenAsync("test@example.com", "Test@123");
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await Client.DeleteAsync($"/api/v1/workflows/{workflowId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // 驗證軟刪除
        var dbWorkflow = await ExecuteDbContextAsync(async db =>
            await db.Workflows.IgnoreQueryFilters().FirstOrDefaultAsync(w => w.Id == workflowId));

        dbWorkflow.Should().NotBeNull();
        dbWorkflow!.IsDeleted.Should().BeTrue();
        dbWorkflow.DeletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task PublishWorkflow_WhenWorkflowIsDraft_ChangesStatusToPublished()
    {
        // Arrange
        var workflowId = await CreateTestWorkflowAsync();
        var token = await GetAuthTokenAsync("test@example.com", "Test@123");
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await Client.PostAsync($"/api/v1/workflows/{workflowId}/publish", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var workflow = await response.Content.ReadFromJsonAsync<WorkflowDto>();
        workflow!.Status.Should().Be("Published");

        // 驗證數據庫狀態
        var dbWorkflow = await ExecuteDbContextAsync(async db =>
            await db.Workflows.FirstOrDefaultAsync(w => w.Id == workflowId));

        dbWorkflow!.Status.Should().Be(WorkflowStatus.Published);
    }

    // 測試輔助方法
    private async Task<int> CreateTestWorkflowAsync()
    {
        return await ExecuteDbContextAsync(async db =>
        {
            var workflow = Workflow.Create("Test Workflow", "Test Description", 1);
            db.Workflows.Add(workflow);
            await db.SaveChangesAsync();
            return workflow.Id;
        });
    }

    private async Task SeedTestDataAsync()
    {
        await ExecuteDbContextAsync(async db =>
        {
            var workflows = new[]
            {
                Workflow.Create("Workflow 1", "Description 1", 1),
                Workflow.Create("Workflow 2", "Description 2", 1),
                Workflow.Create("Workflow 3", "Description 3", 1)
            };

            db.Workflows.AddRange(workflows);
            await db.SaveChangesAsync();
        });
    }

    private async Task<string> GetAuthTokenAsync(string email, string password)
    {
        var loginDto = new { Email = email, Password = password };
        var response = await Client.PostAsJsonAsync("/api/v1/auth/login", loginDto);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<AuthTokenResponse>();
        return result!.AccessToken;
    }
}
```

## 數據庫集成測試

### Repository 集成測試

```csharp
// AIWorkflow.IntegrationTests/Infrastructure/Persistence/WorkflowRepositoryTests.cs
namespace AIWorkflow.IntegrationTests.Infrastructure.Persistence;

public sealed class WorkflowRepositoryTests : IntegrationTestBase
{
    public WorkflowRepositoryTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task GetByIdAsync_WhenWorkflowExists_ReturnsWorkflow()
    {
        // Arrange
        var workflowId = await SeedWorkflowAsync();

        // Act
        var result = await ExecuteDbContextAsync(async db =>
        {
            var repository = new WorkflowRepository(db);
            return await repository.GetByIdAsync(workflowId, CancellationToken.None);
        });

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(workflowId);
    }

    [Fact]
    public async Task GetByIdAsync_WhenWorkflowNotExists_ReturnsNull()
    {
        // Act
        var result = await ExecuteDbContextAsync(async db =>
        {
            var repository = new WorkflowRepository(db);
            return await repository.GetByIdAsync(99999, CancellationToken.None);
        });

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_WithValidWorkflow_AddsToDatabase()
    {
        // Arrange
        var workflow = Workflow.Create("New Workflow", "Description", 1);

        // Act
        await ExecuteDbContextAsync(async db =>
        {
            var repository = new WorkflowRepository(db);
            await repository.AddAsync(workflow, CancellationToken.None);
            await db.SaveChangesAsync();
        });

        // Assert
        var savedWorkflow = await ExecuteDbContextAsync(async db =>
            await db.Workflows.FirstOrDefaultAsync(w => w.Name == "New Workflow"));

        savedWorkflow.Should().NotBeNull();
        savedWorkflow!.Id.Should().BeGreaterThan(0);
        savedWorkflow.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsCorrectPageOfWorkflows()
    {
        // Arrange
        await SeedMultipleWorkflowsAsync(15);

        // Act
        var result = await ExecuteDbContextAsync(async db =>
        {
            var repository = new WorkflowRepository(db);
            return await repository.GetPagedAsync(
                page: 2,
                pageSize: 5,
                cancellationToken: CancellationToken.None);
        });

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(5);
        result.Total.Should().Be(15);
        result.Page.Should().Be(2);
        result.TotalPages.Should().Be(3);
    }

    [Fact]
    public async Task GetByOwnerIdAsync_ReturnsOnlyOwnersWorkflows()
    {
        // Arrange
        var ownerId = 1;
        await SeedWorkflowsForMultipleOwnersAsync();

        // Act
        var result = await ExecuteDbContextAsync(async db =>
        {
            var repository = new WorkflowRepository(db);
            return await repository.GetByOwnerIdAsync(ownerId, CancellationToken.None);
        });

        // Assert
        result.Should().NotBeEmpty();
        result.Should().OnlyContain(w => w.OwnerId == ownerId);
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletesWorkflow()
    {
        // Arrange
        var workflowId = await SeedWorkflowAsync();

        // Act
        await ExecuteDbContextAsync(async db =>
        {
            var repository = new WorkflowRepository(db);
            var workflow = await repository.GetByIdAsync(workflowId, CancellationToken.None);
            await repository.DeleteAsync(workflow!, CancellationToken.None);
            await db.SaveChangesAsync();
        });

        // Assert - 使用 IgnoreQueryFilters 檢查軟刪除
        var deletedWorkflow = await ExecuteDbContextAsync(async db =>
            await db.Workflows.IgnoreQueryFilters().FirstOrDefaultAsync(w => w.Id == workflowId));

        deletedWorkflow.Should().NotBeNull();
        deletedWorkflow!.IsDeleted.Should().BeTrue();
        deletedWorkflow.DeletedAt.Should().NotBeNull();

        // 正常查詢應該找不到(被全局查詢過濾器過濾)
        var normalQuery = await ExecuteDbContextAsync(async db =>
            await db.Workflows.FirstOrDefaultAsync(w => w.Id == workflowId));

        normalQuery.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_UpdatesWorkflowProperties()
    {
        // Arrange
        var workflowId = await SeedWorkflowAsync();

        // Act
        await ExecuteDbContextAsync(async db =>
        {
            var workflow = await db.Workflows.FirstAsync(w => w.Id == workflowId);
            workflow.Name = "Updated Name";
            workflow.Description = "Updated Description";
            await db.SaveChangesAsync();
        });

        // Assert
        var updatedWorkflow = await ExecuteDbContextAsync(async db =>
            await db.Workflows.FirstAsync(w => w.Id == workflowId));

        updatedWorkflow.Name.Should().Be("Updated Name");
        updatedWorkflow.Description.Should().Be("Updated Description");
        updatedWorkflow.UpdatedAt.Should().BeAfter(updatedWorkflow.CreatedAt);
    }

    // 測試輔助方法
    private async Task<int> SeedWorkflowAsync()
    {
        return await ExecuteDbContextAsync(async db =>
        {
            var workflow = Workflow.Create("Test Workflow", "Description", 1);
            db.Workflows.Add(workflow);
            await db.SaveChangesAsync();
            return workflow.Id;
        });
    }

    private async Task SeedMultipleWorkflowsAsync(int count)
    {
        await ExecuteDbContextAsync(async db =>
        {
            for (int i = 1; i <= count; i++)
            {
                var workflow = Workflow.Create($"Workflow {i}", $"Description {i}", 1);
                db.Workflows.Add(workflow);
            }
            await db.SaveChangesAsync();
        });
    }

    private async Task SeedWorkflowsForMultipleOwnersAsync()
    {
        await ExecuteDbContextAsync(async db =>
        {
            // Owner 1 的工作流程
            db.Workflows.Add(Workflow.Create("Owner1 Workflow 1", "Desc", 1));
            db.Workflows.Add(Workflow.Create("Owner1 Workflow 2", "Desc", 1));

            // Owner 2 的工作流程
            db.Workflows.Add(Workflow.Create("Owner2 Workflow 1", "Desc", 2));
            db.Workflows.Add(Workflow.Create("Owner2 Workflow 2", "Desc", 2));

            await db.SaveChangesAsync();
        });
    }
}
```

## 外部服務集成測試

### Mock 外部服務

```csharp
// AIWorkflow.IntegrationTests/Mocks/MockAzureOpenAIService.cs
public sealed class MockAzureOpenAIService : IAzureOpenAIService
{
    private readonly List<ChatMessage> _conversationHistory = new();

    public Task<ChatCompletionResponse> GetChatCompletionAsync(
        string prompt,
        CancellationToken cancellationToken = default)
    {
        // 模擬 AI 回應
        var response = new ChatCompletionResponse
        {
            Content = $"Mock AI response for: {prompt}",
            FinishReason = "stop",
            TokensUsed = 50
        };

        return Task.FromResult(response);
    }

    public Task<ChatCompletionResponse> GetChatCompletionWithHistoryAsync(
        List<ChatMessage> messages,
        CancellationToken cancellationToken = default)
    {
        _conversationHistory.AddRange(messages);

        var response = new ChatCompletionResponse
        {
            Content = $"Mock AI response with {messages.Count} messages in history",
            FinishReason = "stop",
            TokensUsed = messages.Count * 10
        };

        return Task.FromResult(response);
    }

    public Task<EmbeddingResponse> GetEmbeddingAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        // 模擬向量嵌入 (1536 維度)
        var embedding = Enumerable.Range(0, 1536)
            .Select(i => (float)Random.Shared.NextDouble())
            .ToArray();

        var response = new EmbeddingResponse
        {
            Embedding = embedding,
            TokensUsed = text.Length / 4
        };

        return Task.FromResult(response);
    }
}
```

### 測試 AI Agent 集成

```csharp
// AIWorkflow.IntegrationTests/Application/AIAgents/ChatAgentTests.cs
public sealed class ChatAgentTests : IntegrationTestBase
{
    public ChatAgentTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task SendMessage_WithValidPrompt_ReturnsAIResponse()
    {
        // Arrange
        var command = new SendChatMessageCommand
        {
            SessionId = Guid.NewGuid(),
            Message = "Hello, AI assistant!",
            UserId = 1
        };

        // Act
        var result = await SendAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNullOrEmpty();
        result.Value.Should().Contain("Mock AI response");
    }

    [Fact]
    public async Task SendMessage_WithConversationHistory_MaintainsContext()
    {
        // Arrange
        var sessionId = Guid.NewGuid();

        // 第一條消息
        await SendAsync(new SendChatMessageCommand
        {
            SessionId = sessionId,
            Message = "My name is Alice",
            UserId = 1
        });

        // 第二條消息(引用上下文)
        var command = new SendChatMessageCommand
        {
            SessionId = sessionId,
            Message = "What is my name?",
            UserId = 1
        };

        // Act
        var result = await SendAsync(command);

        // Assert
        result.IsSuccess.Should().BeTrue();

        // 驗證會話歷史被保存
        var history = await ExecuteDbContextAsync(async db =>
            await db.ChatMessages
                .Where(m => m.SessionId == sessionId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync());

        history.Should().HaveCount(4);  // 2 個用戶消息 + 2 個 AI 回應
    }
}
```

## 測試隔離與數據重置

### Respawn 配置

```csharp
// Respawn 重置數據庫到乾淨狀態
_respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
{
    DbAdapter = DbAdapter.Postgres,

    // 要重置的 Schema
    SchemasToInclude = new[] { "workflow", "identity", "execution", "audit" },

    // 要忽略的表
    TablesToIgnore = new[]
    {
        new Respawn.Graph.Table("__EFMigrationsHistory"),
        new Respawn.Graph.Table("AspNetRoles"),  // 保留角色數據
        new Respawn.Graph.Table("configuration")  // 保留配置數據
    },

    // 檢查臨時表
    CheckTemporalTables = false
});
```

## 並行測試執行

### xUnit Collection 配置

```csharp
// 測試集合定義 - 共享 Factory 實例
[CollectionDefinition("Integration Tests")]
public sealed class IntegrationTestCollection : ICollectionFixture<CustomWebApplicationFactory>
{
}

// 使用測試集合
[Collection("Integration Tests")]
public sealed class WorkflowsControllerTests : IntegrationTestBase
{
    public WorkflowsControllerTests(CustomWebApplicationFactory factory) : base(factory) { }

    // ... 測試方法
}
```

## 檢查清單

### 測試配置
- [ ] WebApplicationFactory 正確配置
- [ ] TestContainers 正常啟動和清理
- [ ] Respawner 正確重置數據庫
- [ ] 外部服務正確 Mock
- [ ] 測試環境變量配置正確

### API 測試
- [ ] 所有 CRUD 端點都有測試
- [ ] 測試成功和失敗場景
- [ ] 驗證 HTTP 狀態碼
- [ ] 驗證響應 body
- [ ] 測試授權和認證

### 數據庫測試
- [ ] Repository 主要方法都有測試
- [ ] 測試數據持久化
- [ ] 測試查詢過濾器
- [ ] 測試事務處理
- [ ] 測試並發控制

## 參考資源

### 官方文檔
- [ASP.NET Core Integration Tests](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests)
- [TestContainers Documentation](https://dotnet.testcontainers.org/)
- [Respawn Documentation](https://github.com/jbogard/Respawn)

---

**文檔維護**: QA Team / DevOps Team
**最後更新**: 2025-01-01
**狀態**: ✅ 完整
