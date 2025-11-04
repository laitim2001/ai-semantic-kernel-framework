# Backend Project Structure (.NET 8)

## 版本信息
- **.NET 版本**: 8.0
- **C# 版本**: 12
- **架構模式**: Clean Architecture + DDD
- **最後更新**: 2025-01-01

## 整體結構

```
backend/
├── AIWorkflow.sln                          # Visual Studio 解決方案
├── .editorconfig                           # 編輯器配置
├── .gitignore
├── Directory.Build.props                   # 共享 MSBuild 屬性
├── Directory.Packages.props                # 中央包管理
│
├── src/                                    # 源代碼目錄
│   ├── AIWorkflow.Domain/
│   ├── AIWorkflow.Application/
│   ├── AIWorkflow.Infrastructure/
│   ├── AIWorkflow.API/
│   └── AIWorkflow.Shared/
│
└── tests/                                  # 測試目錄
    ├── AIWorkflow.UnitTests/
    ├── AIWorkflow.IntegrationTests/
    ├── AIWorkflow.E2ETests/
    └── AIWorkflow.ArchitectureTests/
```

## Domain Layer (領域層)

### 職責範圍
- **核心業務邏輯**: 不依賴任何外部框架
- **領域模型**: Entities, Value Objects, Domain Events
- **業務規則**: Domain Services, Specifications
- **領域接口**: Repository, Unit of Work 抽象定義

### 項目結構

```
AIWorkflow.Domain/
├── Entities/                               # 實體
│   ├── User.cs
│   ├── Workflow.cs
│   ├── WorkflowNode.cs
│   ├── WorkflowEdge.cs
│   ├── Agent.cs
│   ├── AgentPersona.cs
│   └── ExecutionHistory.cs
│
├── ValueObjects/                           # 值對象
│   ├── Email.cs
│   ├── WorkflowNodeId.cs
│   ├── PersonaConfig.cs
│   └── ExecutionStatus.cs
│
├── Events/                                 # 領域事件
│   ├── WorkflowCreatedEvent.cs
│   ├── WorkflowExecutedEvent.cs
│   ├── AgentPersonaUpdatedEvent.cs
│   └── UserRegisteredEvent.cs
│
├── Interfaces/                             # 領域接口
│   ├── IRepository.cs
│   ├── IUserRepository.cs
│   ├── IWorkflowRepository.cs
│   ├── IAgentRepository.cs
│   └── IUnitOfWork.cs
│
├── Services/                               # 領域服務
│   ├── WorkflowValidationService.cs
│   ├── PersonaMatchingService.cs
│   └── ExecutionPolicyService.cs
│
├── Specifications/                         # 規格模式
│   ├── ISpecification.cs
│   ├── WorkflowIsExecutableSpec.cs
│   └── UserCanExecuteWorkflowSpec.cs
│
├── Exceptions/                             # 領域異常
│   ├── DomainException.cs
│   ├── WorkflowInvalidException.cs
│   └── PersonaNotFoundException.cs
│
└── Common/                                 # 共用基類
    ├── Entity.cs
    ├── ValueObject.cs
    ├── AggregateRoot.cs
    └── DomainEvent.cs
```

### 核心實體示例

```csharp
// AIWorkflow.Domain/Entities/Workflow.cs
namespace AIWorkflow.Domain.Entities;

/// <summary>
/// Workflow 聚合根 - 工作流程核心實體
/// </summary>
public class Workflow : AggregateRoot
{
    // 私有構造函數確保通過工廠方法創建
    private Workflow() { }

    // 基本屬性
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required int OwnerId { get; init; }

    // 導航屬性
    public User Owner { get; init; } = null!;

    // 集合屬性
    private readonly List<WorkflowNode> _nodes = [];
    public IReadOnlyCollection<WorkflowNode> Nodes => _nodes.AsReadOnly();

    private readonly List<WorkflowEdge> _edges = [];
    public IReadOnlyCollection<WorkflowEdge> Edges => _edges.AsReadOnly();

    // 值對象
    public ExecutionStatus Status { get; private set; } = ExecutionStatus.Draft;

    // 時間戳
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? LastModifiedAt { get; private set; }

    // 工廠方法
    public static Workflow Create(string name, string description, int ownerId)
    {
        var workflow = new Workflow
        {
            Name = name,
            Description = description,
            OwnerId = ownerId
        };

        // 發布領域事件
        workflow.AddDomainEvent(new WorkflowCreatedEvent(workflow));

        return workflow;
    }

    // 業務方法
    public void AddNode(WorkflowNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        if (_nodes.Any(n => n.Id == node.Id))
            throw new WorkflowInvalidException($"Node {node.Id} already exists");

        _nodes.Add(node);
        LastModifiedAt = DateTime.UtcNow;
    }

    public void AddEdge(WorkflowEdge edge)
    {
        ArgumentNullException.ThrowIfNull(edge);

        // 驗證節點存在
        if (!_nodes.Any(n => n.Id == edge.SourceNodeId) ||
            !_nodes.Any(n => n.Id == edge.TargetNodeId))
        {
            throw new WorkflowInvalidException("Source or target node does not exist");
        }

        _edges.Add(edge);
        LastModifiedAt = DateTime.UtcNow;
    }

    public bool CanExecute()
    {
        return _nodes.Count > 0 &&
               _nodes.All(n => n.IsConfigured) &&
               Status != ExecutionStatus.Executing;
    }

    public void MarkAsExecuting()
    {
        if (!CanExecute())
            throw new WorkflowInvalidException("Workflow cannot be executed");

        Status = ExecutionStatus.Executing;
        AddDomainEvent(new WorkflowExecutedEvent(this));
    }
}
```

### 值對象示例

```csharp
// AIWorkflow.Domain/ValueObjects/Email.cs
namespace AIWorkflow.Domain.ValueObjects;

/// <summary>
/// Email 值對象 - 確保 Email 格式有效性
/// </summary>
public sealed record Email
{
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        if (!email.Contains('@'))
            throw new ArgumentException("Invalid email format", nameof(email));

        return new Email(email.ToLowerInvariant().Trim());
    }

    public static implicit operator string(Email email) => email.Value;
}
```

### 領域事件示例

```csharp
// AIWorkflow.Domain/Events/WorkflowCreatedEvent.cs
namespace AIWorkflow.Domain.Events;

/// <summary>
/// Workflow 創建事件
/// </summary>
public sealed record WorkflowCreatedEvent(Workflow Workflow) : DomainEvent
{
    public int WorkflowId => Workflow.Id;
    public string WorkflowName => Workflow.Name;
    public int OwnerId => Workflow.OwnerId;
    public DateTime CreatedAt => Workflow.CreatedAt;
}
```

## Application Layer (應用層)

### 職責範圍
- **用例協調**: Commands, Queries (CQRS)
- **業務流程**: Application Services
- **數據傳輸**: DTOs, ViewModels
- **驗證**: FluentValidation
- **映射**: AutoMapper

### 項目結構

```
AIWorkflow.Application/
├── Features/                               # 按功能組織
│   ├── Users/
│   │   ├── Commands/
│   │   │   ├── RegisterUser/
│   │   │   │   ├── RegisterUserCommand.cs
│   │   │   │   ├── RegisterUserCommandHandler.cs
│   │   │   │   └── RegisterUserCommandValidator.cs
│   │   │   └── UpdateUserProfile/
│   │   │       ├── UpdateUserProfileCommand.cs
│   │   │       └── UpdateUserProfileCommandHandler.cs
│   │   ├── Queries/
│   │   │   ├── GetUser/
│   │   │   │   ├── GetUserQuery.cs
│   │   │   │   └── GetUserQueryHandler.cs
│   │   │   └── GetUserList/
│   │   │       ├── GetUserListQuery.cs
│   │   │       └── GetUserListQueryHandler.cs
│   │   └── DTOs/
│   │       ├── UserDto.cs
│   │       └── UserProfileDto.cs
│   │
│   ├── Workflows/
│   │   ├── Commands/
│   │   │   ├── CreateWorkflow/
│   │   │   ├── UpdateWorkflow/
│   │   │   ├── DeleteWorkflow/
│   │   │   └── ExecuteWorkflow/
│   │   ├── Queries/
│   │   │   ├── GetWorkflow/
│   │   │   ├── GetWorkflowList/
│   │   │   └── GetWorkflowExecutionHistory/
│   │   └── DTOs/
│   │       ├── WorkflowDto.cs
│   │       ├── WorkflowNodeDto.cs
│   │       └── ExecutionResultDto.cs
│   │
│   └── Agents/
│       ├── Commands/
│       │   ├── CreateAgentPersona/
│       │   └── UpdateAgentPersona/
│       ├── Queries/
│       │   ├── GetAgentPersona/
│       │   └── GetPersonaList/
│       └── DTOs/
│           └── AgentPersonaDto.cs
│
├── Common/                                 # 共用組件
│   ├── Interfaces/
│   │   ├── IApplicationDbContext.cs
│   │   ├── IDateTimeProvider.cs
│   │   ├── ICurrentUserService.cs
│   │   └── IEmailService.cs
│   ├── Behaviors/                          # MediatR Pipeline Behaviors
│   │   ├── ValidationBehavior.cs
│   │   ├── LoggingBehavior.cs
│   │   ├── PerformanceBehavior.cs
│   │   └── UnhandledExceptionBehavior.cs
│   ├── Mappings/
│   │   ├── MappingProfile.cs
│   │   └── DtoExtensions.cs
│   ├── Exceptions/
│   │   ├── ValidationException.cs
│   │   ├── NotFoundException.cs
│   │   └── ForbiddenAccessException.cs
│   └── Models/
│       ├── Result.cs
│       ├── PaginatedList.cs
│       └── ApiResponse.cs
│
└── DependencyInjection.cs                  # 依賴注入配置
```

### Command Handler 示例 (CQRS)

```csharp
// AIWorkflow.Application/Features/Workflows/Commands/CreateWorkflow/CreateWorkflowCommand.cs
namespace AIWorkflow.Application.Features.Workflows.Commands.CreateWorkflow;

/// <summary>
/// 創建 Workflow 命令
/// </summary>
public sealed record CreateWorkflowCommand : IRequest<Result<int>>
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public List<WorkflowNodeDto> Nodes { get; init; } = [];
    public List<WorkflowEdgeDto> Edges { get; init; } = [];
}

/// <summary>
/// 命令驗證器
/// </summary>
public sealed class CreateWorkflowCommandValidator : AbstractValidator<CreateWorkflowCommand>
{
    public CreateWorkflowCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Workflow name is required")
            .MaximumLength(200).WithMessage("Workflow name must not exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(1000);

        RuleFor(x => x.Nodes)
            .NotEmpty().WithMessage("Workflow must have at least one node");
    }
}

/// <summary>
/// 命令處理器
/// </summary>
public sealed class CreateWorkflowCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService,
    ILogger<CreateWorkflowCommandHandler> logger)
    : IRequestHandler<CreateWorkflowCommand, Result<int>>
{
    public async Task<Result<int>> Handle(
        CreateWorkflowCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 獲取當前用戶
            var userId = currentUserService.UserId;

            // 創建領域實體
            var workflow = Workflow.Create(
                request.Name,
                request.Description,
                userId
            );

            // 添加節點和邊
            foreach (var nodeDto in request.Nodes)
            {
                var node = WorkflowNode.Create(
                    nodeDto.Type,
                    nodeDto.Config
                );
                workflow.AddNode(node);
            }

            foreach (var edgeDto in request.Edges)
            {
                var edge = WorkflowEdge.Create(
                    edgeDto.SourceNodeId,
                    edgeDto.TargetNodeId
                );
                workflow.AddEdge(edge);
            }

            // 保存到資料庫
            context.Workflows.Add(workflow);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Workflow {WorkflowId} created by user {UserId}",
                workflow.Id,
                userId
            );

            return Result<int>.Success(workflow.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating workflow");
            return Result<int>.Failure("Failed to create workflow");
        }
    }
}
```

### Query Handler 示例

```csharp
// AIWorkflow.Application/Features/Workflows/Queries/GetWorkflow/GetWorkflowQuery.cs
namespace AIWorkflow.Application.Features.Workflows.Queries.GetWorkflow;

/// <summary>
/// 獲取 Workflow 查詢
/// </summary>
public sealed record GetWorkflowQuery(int Id) : IRequest<Result<WorkflowDto>>;

/// <summary>
/// 查詢處理器
/// </summary>
public sealed class GetWorkflowQueryHandler(
    IApplicationDbContext context,
    IMapper mapper)
    : IRequestHandler<GetWorkflowQuery, Result<WorkflowDto>>
{
    public async Task<Result<WorkflowDto>> Handle(
        GetWorkflowQuery request,
        CancellationToken cancellationToken)
    {
        var workflow = await context.Workflows
            .Include(w => w.Owner)
            .Include(w => w.Nodes)
            .Include(w => w.Edges)
            .FirstOrDefaultAsync(w => w.Id == request.Id, cancellationToken);

        if (workflow is null)
            return Result<WorkflowDto>.Failure("Workflow not found");

        var dto = mapper.Map<WorkflowDto>(workflow);
        return Result<WorkflowDto>.Success(dto);
    }
}
```

### MediatR Pipeline Behavior 示例

```csharp
// AIWorkflow.Application/Common/Behaviors/ValidationBehavior.cs
namespace AIWorkflow.Application.Common.Behaviors;

/// <summary>
/// 驗證管道行為 - 在命令執行前自動驗證
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken))
        );

        var failures = validationResults
            .Where(r => !r.IsValid)
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures.Count != 0)
            throw new ValidationException(failures);

        return await next();
    }
}
```

## Infrastructure Layer (基礎設施層)

### 職責範圍
- **數據訪問**: Entity Framework Core, Repository 實現
- **外部服務**: Azure OpenAI, Semantic Kernel
- **緩存**: Redis
- **文件存儲**: Azure Blob Storage
- **消息**: SignalR, RabbitMQ

### 項目結構

```
AIWorkflow.Infrastructure/
├── Persistence/                            # 數據持久化
│   ├── ApplicationDbContext.cs
│   ├── Configurations/                     # EF Core 配置
│   │   ├── UserConfiguration.cs
│   │   ├── WorkflowConfiguration.cs
│   │   ├── WorkflowNodeConfiguration.cs
│   │   └── AgentPersonaConfiguration.cs
│   ├── Interceptors/                       # EF Core 攔截器
│   │   ├── AuditableEntityInterceptor.cs
│   │   └── DomainEventInterceptor.cs
│   ├── Migrations/                         # 資料庫遷移
│   └── Repositories/                       # Repository 實現
│       ├── UserRepository.cs
│       ├── WorkflowRepository.cs
│       └── AgentRepository.cs
│
├── Services/                               # 外部服務
│   ├── AI/
│   │   ├── SemanticKernelService.cs
│   │   ├── OpenAIService.cs
│   │   └── EmbeddingService.cs
│   ├── Caching/
│   │   ├── RedisCacheService.cs
│   │   └── CacheKeyBuilder.cs
│   ├── Storage/
│   │   ├── AzureBlobStorageService.cs
│   │   └── LocalFileStorageService.cs
│   ├── Messaging/
│   │   ├── SignalRNotificationService.cs
│   │   └── RabbitMQEventBus.cs
│   └── External/
│       ├── DateTimeProvider.cs
│       ├── EmailService.cs
│       └── CurrentUserService.cs
│
├── BackgroundJobs/                         # 背景工作
│   ├── WorkflowExecutionJob.cs
│   └── DataCleanupJob.cs
│
└── DependencyInjection.cs                  # 依賴注入配置
```

### DbContext 示例

```csharp
// AIWorkflow.Infrastructure/Persistence/ApplicationDbContext.cs
namespace AIWorkflow.Infrastructure.Persistence;

/// <summary>
/// 應用程序資料庫上下文
/// </summary>
public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    IMediator mediator)
    : DbContext(options), IApplicationDbContext
{
    // DbSets
    public DbSet<User> Users => Set<User>();
    public DbSet<Workflow> Workflows => Set<Workflow>();
    public DbSet<WorkflowNode> WorkflowNodes => Set<WorkflowNode>();
    public DbSet<WorkflowEdge> WorkflowEdges => Set<WorkflowEdge>();
    public DbSet<Agent> Agents => Set<Agent>();
    public DbSet<AgentPersona> AgentPersonas => Set<AgentPersona>();
    public DbSet<ExecutionHistory> ExecutionHistories => Set<ExecutionHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 應用所有配置
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // 發布領域事件
        await DispatchDomainEventsAsync();

        // 保存變更
        return await base.SaveChangesAsync(cancellationToken);
    }

    private async Task DispatchDomainEventsAsync()
    {
        var domainEvents = ChangeTracker
            .Entries<AggregateRoot>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
            .SelectMany(e =>
            {
                var events = e.DomainEvents.ToList();
                e.ClearDomainEvents();
                return events;
            })
            .ToList();

        foreach (var domainEvent in domainEvents)
        {
            await mediator.Publish(domainEvent);
        }
    }
}
```

### Entity Configuration 示例

```csharp
// AIWorkflow.Infrastructure/Persistence/Configurations/WorkflowConfiguration.cs
namespace AIWorkflow.Infrastructure.Persistence.Configurations;

/// <summary>
/// Workflow 實體配置
/// </summary>
public sealed class WorkflowConfiguration : IEntityTypeConfiguration<Workflow>
{
    public void Configure(EntityTypeBuilder<Workflow> builder)
    {
        // 表名
        builder.ToTable("Workflows");

        // 主鍵
        builder.HasKey(w => w.Id);

        // 屬性配置
        builder.Property(w => w.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(w => w.Description)
            .IsRequired()
            .HasMaxLength(1000);

        // 值對象配置
        builder.Property(w => w.Status)
            .HasConversion(
                v => v.Value,
                v => ExecutionStatus.FromValue(v)
            )
            .HasMaxLength(50);

        // 索引
        builder.HasIndex(w => w.OwnerId);
        builder.HasIndex(w => w.CreatedAt);

        // 關係配置
        builder.HasOne(w => w.Owner)
            .WithMany()
            .HasForeignKey(w => w.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(w => w.Nodes)
            .WithOne()
            .HasForeignKey("WorkflowId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(w => w.Edges)
            .WithOne()
            .HasForeignKey("WorkflowId")
            .OnDelete(DeleteBehavior.Cascade);

        // 忽略領域事件集合
        builder.Ignore(w => w.DomainEvents);
    }
}
```

### Repository 實現示例

```csharp
// AIWorkflow.Infrastructure/Persistence/Repositories/WorkflowRepository.cs
namespace AIWorkflow.Infrastructure.Persistence.Repositories;

/// <summary>
/// Workflow Repository 實現
/// </summary>
public sealed class WorkflowRepository(ApplicationDbContext context)
    : IWorkflowRepository
{
    public async Task<Workflow?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.Workflows
            .Include(w => w.Owner)
            .Include(w => w.Nodes)
            .Include(w => w.Edges)
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public async Task<List<Workflow>> GetByOwnerIdAsync(
        int ownerId,
        CancellationToken cancellationToken = default)
    {
        return await context.Workflows
            .Where(w => w.OwnerId == ownerId)
            .Include(w => w.Nodes)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> AddAsync(Workflow workflow, CancellationToken cancellationToken = default)
    {
        context.Workflows.Add(workflow);
        await context.SaveChangesAsync(cancellationToken);
        return workflow.Id;
    }

    public async Task UpdateAsync(Workflow workflow, CancellationToken cancellationToken = default)
    {
        context.Workflows.Update(workflow);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Workflow workflow, CancellationToken cancellationToken = default)
    {
        context.Workflows.Remove(workflow);
        await context.SaveChangesAsync(cancellationToken);
    }
}
```

### Semantic Kernel Service 示例

```csharp
// AIWorkflow.Infrastructure/Services/AI/SemanticKernelService.cs
namespace AIWorkflow.Infrastructure.Services.AI;

/// <summary>
/// Semantic Kernel 服務 - AI 工作流程執行引擎
/// </summary>
public sealed class SemanticKernelService(
    IOptions<AzureOpenAIConfig> config,
    ILogger<SemanticKernelService> logger)
{
    private readonly AzureOpenAIConfig _config = config.Value;

    public async Task<string> ExecuteWorkflowAsync(
        Workflow workflow,
        Dictionary<string, object> inputs,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // 構建 Kernel
            var kernel = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(
                    deploymentName: _config.DeploymentName,
                    endpoint: _config.Endpoint,
                    apiKey: _config.ApiKey
                )
                .Build();

            // 執行工作流程節點
            var results = new Dictionary<string, object>();

            foreach (var node in workflow.Nodes.OrderBy(n => n.Order))
            {
                var nodeResult = await ExecuteNodeAsync(
                    kernel,
                    node,
                    inputs,
                    results,
                    cancellationToken
                );

                results[node.Id.ToString()] = nodeResult;
            }

            return JsonSerializer.Serialize(results);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error executing workflow {WorkflowId}", workflow.Id);
            throw;
        }
    }

    private async Task<object> ExecuteNodeAsync(
        Kernel kernel,
        WorkflowNode node,
        Dictionary<string, object> inputs,
        Dictionary<string, object> previousResults,
        CancellationToken cancellationToken)
    {
        // 根據節點類型執行不同的 AI 操作
        return node.Type switch
        {
            "prompt" => await ExecutePromptNodeAsync(kernel, node, inputs, cancellationToken),
            "agent" => await ExecuteAgentNodeAsync(kernel, node, inputs, cancellationToken),
            "function" => await ExecuteFunctionNodeAsync(kernel, node, inputs, cancellationToken),
            _ => throw new NotSupportedException($"Node type {node.Type} not supported")
        };
    }

    private async Task<object> ExecutePromptNodeAsync(
        Kernel kernel,
        WorkflowNode node,
        Dictionary<string, object> inputs,
        CancellationToken cancellationToken)
    {
        var promptTemplate = node.Config["template"]?.ToString()
            ?? throw new InvalidOperationException("Prompt template not found");

        var result = await kernel.InvokePromptAsync(
            promptTemplate,
            new KernelArguments(inputs),
            cancellationToken: cancellationToken
        );

        return result.ToString();
    }

    private async Task<object> ExecuteAgentNodeAsync(
        Kernel kernel,
        WorkflowNode node,
        Dictionary<string, object> inputs,
        CancellationToken cancellationToken)
    {
        // Agent 節點執行邏輯
        var personaId = node.Config["personaId"]?.ToString();
        var instruction = node.Config["instruction"]?.ToString();

        // 使用 ChatCompletion 執行 Agent 任務
        var chatCompletion = kernel.GetRequiredService<IChatCompletionService>();

        var chatHistory = new ChatHistory();
        chatHistory.AddSystemMessage($"You are {personaId}. {instruction}");
        chatHistory.AddUserMessage(inputs["userInput"]?.ToString() ?? "");

        var response = await chatCompletion.GetChatMessageContentAsync(
            chatHistory,
            cancellationToken: cancellationToken
        );

        return response.Content ?? "";
    }
}
```

## API Layer (表示層)

### 職責範圍
- **HTTP 端點**: RESTful API Controllers
- **即時通訊**: SignalR Hubs
- **中間件**: 認證、授權、異常處理
- **API 文檔**: Swagger/OpenAPI

### 項目結構

```
AIWorkflow.API/
├── Controllers/                            # API 控制器
│   ├── UsersController.cs
│   ├── WorkflowsController.cs
│   ├── AgentsController.cs
│   └── ExecutionController.cs
│
├── Hubs/                                   # SignalR Hubs
│   ├── WorkflowHub.cs
│   └── NotificationHub.cs
│
├── Middleware/                             # 中間件
│   ├── ExceptionHandlingMiddleware.cs
│   ├── RequestLoggingMiddleware.cs
│   └── PerformanceMonitoringMiddleware.cs
│
├── Filters/                                # Action Filters
│   ├── ApiExceptionFilter.cs
│   └── ValidateModelStateFilter.cs
│
├── Extensions/                             # 擴展方法
│   ├── ServiceCollectionExtensions.cs
│   └── ApplicationBuilderExtensions.cs
│
├── Program.cs                              # 應用程序入口
├── appsettings.json                        # 配置文件
├── appsettings.Development.json
└── appsettings.Production.json
```

### Controller 示例

```csharp
// AIWorkflow.API/Controllers/WorkflowsController.cs
namespace AIWorkflow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class WorkflowsController(ISender mediator) : ControllerBase
{
    /// <summary>
    /// 創建新的 Workflow
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateWorkflow(
        [FromBody] CreateWorkflowCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return CreatedAtAction(
            nameof(GetWorkflow),
            new { id = result.Value },
            result.Value
        );
    }

    /// <summary>
    /// 獲取 Workflow 詳情
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(WorkflowDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWorkflow(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetWorkflowQuery(id), cancellationToken);

        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// 獲取當前用戶的 Workflow 列表
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<WorkflowDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWorkflows(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(
            new GetWorkflowListQuery(page, pageSize),
            cancellationToken
        );

        return Ok(result.Value);
    }

    /// <summary>
    /// 執行 Workflow
    /// </summary>
    [HttpPost("{id:int}/execute")]
    [ProducesResponseType(typeof(ExecutionResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ExecuteWorkflow(
        int id,
        [FromBody] Dictionary<string, object> inputs,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new ExecuteWorkflowCommand(id, inputs),
            cancellationToken
        );

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// 刪除 Workflow
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteWorkflow(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteWorkflowCommand(id), cancellationToken);

        if (!result.IsSuccess)
            return NotFound(result.Error);

        return NoContent();
    }
}
```

### SignalR Hub 示例

```csharp
// AIWorkflow.API/Hubs/WorkflowHub.cs
namespace AIWorkflow.API.Hubs;

/// <summary>
/// Workflow 即時協作 Hub
/// </summary>
[Authorize]
public sealed class WorkflowHub(
    ILogger<WorkflowHub> logger,
    IMediator mediator) : Hub
{
    /// <summary>
    /// 加入 Workflow 編輯室
    /// </summary>
    public async Task JoinWorkflow(int workflowId)
    {
        var groupName = $"workflow-{workflowId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        logger.LogInformation(
            "User {ConnectionId} joined workflow {WorkflowId}",
            Context.ConnectionId,
            workflowId
        );

        await Clients.Group(groupName).SendAsync(
            "UserJoined",
            Context.User?.Identity?.Name
        );
    }

    /// <summary>
    /// 離開 Workflow 編輯室
    /// </summary>
    public async Task LeaveWorkflow(int workflowId)
    {
        var groupName = $"workflow-{workflowId}";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

        await Clients.Group(groupName).SendAsync(
            "UserLeft",
            Context.User?.Identity?.Name
        );
    }

    /// <summary>
    /// 廣播節點變更
    /// </summary>
    public async Task BroadcastNodeChange(int workflowId, object nodeData)
    {
        var groupName = $"workflow-{workflowId}";

        await Clients.OthersInGroup(groupName).SendAsync(
            "NodeChanged",
            nodeData
        );
    }

    /// <summary>
    /// 廣播邊變更
    /// </summary>
    public async Task BroadcastEdgeChange(int workflowId, object edgeData)
    {
        var groupName = $"workflow-{workflowId}";

        await Clients.OthersInGroup(groupName).SendAsync(
            "EdgeChanged",
            edgeData
        );
    }
}
```

### Program.cs 示例

```csharp
// AIWorkflow.API/Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AI Workflow Platform API",
        Version = "v1",
        Description = "API for managing AI-powered workflows"
    });

    // JWT 認證配置
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
});

// Add Application services
builder.Services.AddApplication();

// Add Infrastructure services
builder.Services.AddInfrastructure(builder.Configuration);

// Add Authentication & Authorization
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };
    });

// Add SignalR
builder.Services.AddSignalR();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<WorkflowHub>("/hubs/workflow");
app.MapHub<NotificationHub>("/hubs/notifications");

app.Run();
```

## Shared Layer (共享層)

### 項目結構

```
AIWorkflow.Shared/
├── Constants/
│   ├── ApiRoutes.cs
│   ├── CacheKeys.cs
│   └── ErrorCodes.cs
├── Extensions/
│   ├── StringExtensions.cs
│   ├── DateTimeExtensions.cs
│   └── EnumerableExtensions.cs
└── Helpers/
    ├── JsonHelper.cs
    └── HashHelper.cs
```

## 測試項目結構

### Unit Tests

```
AIWorkflow.UnitTests/
├── Domain/
│   ├── Entities/
│   │   ├── WorkflowTests.cs
│   │   └── UserTests.cs
│   ├── ValueObjects/
│   │   └── EmailTests.cs
│   └── Services/
│       └── WorkflowValidationServiceTests.cs
│
├── Application/
│   ├── Workflows/
│   │   ├── Commands/
│   │   │   └── CreateWorkflowCommandHandlerTests.cs
│   │   └── Queries/
│   │       └── GetWorkflowQueryHandlerTests.cs
│   └── Behaviors/
│       └── ValidationBehaviorTests.cs
│
└── Infrastructure/
    └── Repositories/
        └── WorkflowRepositoryTests.cs
```

### Integration Tests

```
AIWorkflow.IntegrationTests/
├── Controllers/
│   ├── WorkflowsControllerTests.cs
│   └── UsersControllerTests.cs
├── Hubs/
│   └── WorkflowHubTests.cs
└── Database/
    └── DatabaseIntegrationTests.cs
```

### Architecture Tests

```
AIWorkflow.ArchitectureTests/
├── DependencyTests.cs
└── NamingConventionTests.cs
```

## 依賴注入配置

### Application Layer DI

```csharp
// AIWorkflow.Application/DependencyInjection.cs
namespace AIWorkflow.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // MediatR
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);

            // Pipeline Behaviors
            config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        });

        // FluentValidation
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        // AutoMapper
        services.AddAutoMapper(typeof(DependencyInjection).Assembly);

        return services;
    }
}
```

### Infrastructure Layer DI

```csharp
// AIWorkflow.Infrastructure/DependencyInjection.cs
namespace AIWorkflow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("PostgreSQL"),
                npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    npgsqlOptions.EnableRetryOnFailure(3);
                }
            );
        });

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IWorkflowRepository, WorkflowRepository>();
        services.AddScoped<IAgentRepository, AgentRepository>();

        // Services
        services.AddScoped<ISemanticKernelService, SemanticKernelService>();
        services.AddScoped<ICacheService, RedisCacheService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // Redis
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
        });

        // Azure OpenAI Configuration
        services.Configure<AzureOpenAIConfig>(
            configuration.GetSection("AzureOpenAI")
        );

        return services;
    }
}
```

## 配置文件

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "PostgreSQL": "Host=localhost;Port=5432;Database=aiworkflow;Username=postgres;Password=postgres123",
    "Redis": "localhost:6379"
  },
  "AzureOpenAI": {
    "Endpoint": "https://your-resource.openai.azure.com/",
    "ApiKey": "",
    "DeploymentName": "gpt-4",
    "EmbeddingDeployment": "text-embedding-3-small",
    "MaxTokens": 4000,
    "Temperature": 0.7
  },
  "Jwt": {
    "Key": "your-secret-key-min-32-characters",
    "Issuer": "AIWorkflowPlatform",
    "Audience": "AIWorkflowPlatformUsers",
    "ExpiryMinutes": 60
  }
}
```

## 最佳實踐

### 1. Clean Architecture 原則
- ✅ Domain 層不依賴任何外部庫
- ✅ Application 層只依賴 Domain
- ✅ Infrastructure 實現 Application 定義的接口
- ✅ API 層只依賴 Application

### 2. CQRS 模式
- ✅ 分離 Commands (寫操作) 和 Queries (讀操作)
- ✅ Commands 返回 `Result<T>`
- ✅ Queries 直接返回 DTO

### 3. Repository 模式
- ✅ Repository 只處理聚合根
- ✅ 使用 Specification 模式進行複雜查詢
- ✅ Repository 返回領域實體,不是 DTO

### 4. 領域事件
- ✅ 使用領域事件解耦業務邏輯
- ✅ 在 SaveChanges 時自動發布事件
- ✅ 事件處理器在 Application 層

### 5. 驗證策略
- ✅ 使用 FluentValidation 進行輸入驗證
- ✅ 領域實體內部進行業務規則驗證
- ✅ ValidationBehavior 自動執行驗證

---

**文檔維護**: Tech Lead Team
**最後審核**: 2025-01-01
