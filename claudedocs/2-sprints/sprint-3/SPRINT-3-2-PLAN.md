# Sprint 3 執行計劃 (PLAN)

**版本**: v2.0
**創建日期**: 2025-11-13
**最後更新**: 2025-11-13
**Sprint 週期**: Week 7-9 (3 週)
**Sprint 定位**: Phase 1A - 基礎平台 (13 SP, 3 週)

**規劃文檔參考**:
- 📋 [MVP Scope Definition](../../1-planning/MVP-SCOPE-DEFINITION.md) - Phase 1A 範圍定義
- 📊 [Sprint Allocation Analysis](../../1-planning/SPRINT-ALLOCATION-ANALYSIS.md) - Sprint 3 詳細分析 (US 1.3, 13 SP)
- 🎯 [Development Strategy](../../1-planning/DEVELOPMENT-STRATEGY.md) - Phase 1A Plugin 系統開發策略
- 📐 [Architecture Design Document](../../../docs/architecture/Architecture-Design-Document.md) - Clean Architecture 總覽

---

## 📋 目錄

1. [本 Sprint 要建立什麼](#本-sprint-要建立什麼)
2. [如何建立](#如何建立)
3. [任務時間表](#任務時間表)
4. [驗收標準](#驗收標準)
5. [風險緩解計劃](#風險緩解計劃)
6. [完整參考文獻索引](#完整參考文獻索引)

---

## 本 Sprint 要建立什麼

### US 1.3: Plugin 系統基礎架構 (13 SP)

**User Story 完整規格**: [US 2.1 - Plugin System Foundation](../../../docs/user-stories/us-2-plugin-system.md)

#### MVP 範圍定義

**必須實現功能 (P0 - 本 Sprint)**:

- [x] **Plugin Entity 設計 (Domain Layer)** - 2 SP
  - Plugin Entity: PluginId, Name, Description, Type, Status, FilePath, Metadata
  - PluginMetadata Value Object: Version (SemVer), Author, Dependencies, Permissions
  - PluginType enum: Native, Semantic
  - PluginStatus enum: Draft, Active, Inactive, Archived
  - **參考**: [Domain Layer Design](../../../docs/architecture/layered-architecture/Domain-Layer.md)
  - **參考**: [Plugin Schema](../../../docs/database/plugin-schema.md)

- [x] **Plugin Repository (Infrastructure Layer)** - 2 SP
  - IPluginRepository 介面: CRUD 操作
  - PluginRepository 實作: EF Core
  - Database Migration: 20XX_AddPluginTables
  - **參考**: [Infrastructure Layer](../../../docs/architecture/layered-architecture/Infrastructure-Layer.md)
  - **參考**: [Repository Pattern](../../../docs/architecture/adr/ADR-003-repository-pattern.md)

- [x] **Plugin CQRS Commands/Queries (Application Layer)** - 3 SP
  - CreatePluginCommand + Handler
  - ActivatePluginCommand + Handler
  - DeactivatePluginCommand + Handler
  - DeletePluginCommand + Handler
  - GetPluginByIdQuery + Handler
  - GetPluginsQuery + Handler (支援篩選和分頁)
  - FluentValidation 驗證規則
  - **參考**: [Application Layer](../../../docs/architecture/layered-architecture/Application-Layer.md)
  - **參考**: [CQRS Implementation](../../../docs/technical-implementation/01-backend-net9/05-cqrs-implementation.md)

- [x] **IPluginLoader 介面設計與實作** - 2 SP
  - IPluginLoader 介面: LoadPlugin, UnloadPlugin, GetLoadedPlugins
  - PluginLoader 實作: 支援 Native Plugin (DLL) 和 Semantic Plugin (YAML)
  - Plugin 元數據解析
  - **參考**: [Plugin System MCP](../../../docs/technical-implementation/01-backend-net9/04-plugin-system-mcp.md)

- [x] **AppDomain 隔離機制** - 2 SP
  - 每個 Native Plugin 在獨立 AppDomain 中執行
  - AppDomain 配置: 記憶體限制 256MB, 禁止檔案/網路訪問
  - Plugin 簽名驗證
  - 權限檢查機制
  - **參考**: [Plugin Security](../../../docs/security/plugin-security.md)
  - **參考**: [Code Execution Security](../../../docs/security/code-execution-security.md)

- [x] **MCP 協議整合** - 1 SP
  - IMCPAdapter 介面設計
  - 基礎 MCP 協議支援 (tool call, message passing)
  - **參考**: [Plugin System MCP](../../../docs/technical-implementation/01-backend-net9/04-plugin-system-mcp.md#mcp-integration)

- [x] **Plugin API Controller** - 1 SP
  - POST /api/plugins - 建立 Plugin
  - GET /api/plugins/{id} - 取得 Plugin 詳情
  - GET /api/plugins - 列表 (支援篩選: type, status)
  - PUT /api/plugins/{id}/activate - 啟用 Plugin
  - PUT /api/plugins/{id}/deactivate - 停用 Plugin
  - DELETE /api/plugins/{id} - 刪除 Plugin
  - Swagger 文檔
  - **參考**: [Plugin API Design](../../../docs/api/plugin-api-design.md)

**明確排除 Phase 2 功能 (延後到 Sprint 4)**:
- ❌ **Plugin 熱重載 (Hot Reload)**: US 2.2 - Sprint 4
- ❌ **Plugin 版本控制 (完整)**: US 2.2 - Sprint 4
- ❌ **Plugin Marketplace**: Phase 2
- ❌ **Plugin 相依性自動解析**: Phase 2

---

## 如何建立

### Week 1: Domain Layer + Infrastructure Layer (5 SP)

#### Day 1-2: Plugin Entity 設計 (T3.1 - 2 SP)

**負責人**: Backend Dev 1

**詳細技術規格**:

##### Plugin Entity (Domain Layer)

```csharp
// Domain/Entities/Plugin.cs
public class Plugin : BaseEntity
{
    public Guid PluginId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public PluginType Type { get; private set; }
    public PluginStatus Status { get; private set; }
    public string FilePath { get; private set; }
    public PluginMetadata Metadata { get; private set; }

    // Business Rules
    public void Activate()
    {
        if (Status == PluginStatus.Archived)
            throw new DomainException("Cannot activate archived plugin");

        Status = PluginStatus.Active;
    }

    public void Deactivate()
    {
        if (Status == PluginStatus.Active)
            Status = PluginStatus.Inactive;
    }
}
```

**參考文檔**:
- [Domain Layer Design](../../../docs/architecture/layered-architecture/Domain-Layer.md#entities)
- [Domain-Driven Design](../../../docs/architecture/Architecture-Design-Document.md#domain-driven-design)

##### Value Objects

```csharp
// Domain/ValueObjects/PluginMetadata.cs
public class PluginMetadata : ValueObject
{
    public SemanticVersion Version { get; private set; }
    public string Author { get; private set; }
    public List<string> Dependencies { get; private set; }
    public List<Permission> Permissions { get; private set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Version;
        yield return Author;
    }
}

// Domain/ValueObjects/SemanticVersion.cs
public class SemanticVersion : ValueObject
{
    public int Major { get; private set; }
    public int Minor { get; private set; }
    public int Patch { get; private set; }

    public static SemanticVersion Parse(string version)
    {
        // Parse "1.0.0" format
    }
}
```

**參考文檔**:
- [Value Objects](../../../docs/architecture/layered-architecture/Domain-Layer.md#value-objects)
- [Semantic Versioning](https://semver.org/)

##### Enums

```csharp
public enum PluginType
{
    Native,    // C# DLL
    Semantic   // Prompt YAML
}

public enum PluginStatus
{
    Draft,
    Active,
    Inactive,
    Archived
}

public enum Permission
{
    FileAccess,
    NetworkAccess,
    DatabaseAccess,
    ApiAccess
}
```

#### Day 3-5: Plugin Repository 實作 (T3.2 - 2 SP)

**負責人**: Backend Dev 1

##### IPluginRepository 介面

```csharp
// Domain/Interfaces/IPluginRepository.cs
public interface IPluginRepository
{
    Task<Plugin> GetByIdAsync(Guid pluginId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Plugin>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PagedResult<Plugin>> GetPagedAsync(
        int page,
        int pageSize,
        PluginType? type = null,
        PluginStatus? status = null,
        CancellationToken cancellationToken = default);
    Task<Plugin> AddAsync(Plugin plugin, CancellationToken cancellationToken = default);
    Task UpdateAsync(Plugin plugin, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid pluginId, CancellationToken cancellationToken = default);
}
```

**參考文檔**:
- [Repository Pattern](../../../docs/architecture/adr/ADR-003-repository-pattern.md)
- [Infrastructure Layer](../../../docs/architecture/layered-architecture/Infrastructure-Layer.md)

##### PluginRepository 實作

```csharp
// Infrastructure/Repositories/PluginRepository.cs
public class PluginRepository : IPluginRepository
{
    private readonly ApplicationDbContext _context;

    public async Task<Plugin> GetByIdAsync(Guid pluginId, CancellationToken cancellationToken)
    {
        return await _context.Plugins
            .FirstOrDefaultAsync(p => p.PluginId == pluginId, cancellationToken);
    }

    public async Task<PagedResult<Plugin>> GetPagedAsync(
        int page, int pageSize, PluginType? type, PluginStatus? status,
        CancellationToken cancellationToken)
    {
        var query = _context.Plugins.AsQueryable();

        if (type.HasValue)
            query = query.Where(p => p.Type == type.Value);

        if (status.HasValue)
            query = query.Where(p => p.Status == status.Value);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Plugin>(items, totalCount, page, pageSize);
    }

    // ... other methods
}
```

##### Database Migration

```csharp
// Infrastructure/Migrations/20XX_AddPluginTables.cs
public partial class AddPluginTables : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Plugins",
            columns: table => new
            {
                PluginId = table.Column<Guid>(nullable: false),
                Name = table.Column<string>(maxLength: 100, nullable: false),
                Description = table.Column<string>(maxLength: 500, nullable: true),
                Type = table.Column<int>(nullable: false),
                Status = table.Column<int>(nullable: false),
                FilePath = table.Column<string>(maxLength: 500, nullable: false),
                Version = table.Column<string>(maxLength: 20, nullable: false),
                Author = table.Column<string>(maxLength: 100, nullable: true),
                CreatedAt = table.Column<DateTime>(nullable: false),
                UpdatedAt = table.Column<DateTime>(nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Plugins", x => x.PluginId);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Plugins_Name",
            table: "Plugins",
            column: "Name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Plugins_Type",
            table: "Plugins",
            column: "Type");

        migrationBuilder.CreateIndex(
            name: "IX_Plugins_Status",
            table: "Plugins",
            column: "Status");
    }
}
```

**參考文檔**:
- [Plugin Schema](../../../docs/database/plugin-schema.md)
- [EF Core Migrations](../../../docs/database/migration-guide.md)

---

### Week 2: Application Layer + Security (6 SP)

#### Day 6-8: Plugin CQRS Commands/Queries (T3.3 - 3 SP)

**負責人**: Backend Dev 2

##### CreatePluginCommand

```csharp
// Application/Plugins/Commands/CreatePlugin/CreatePluginCommand.cs
public record CreatePluginCommand(
    string Name,
    string Description,
    PluginType Type,
    string FilePath,
    PluginMetadataDto Metadata
) : IRequest<Result<PluginDto>>;

// Application/Plugins/Commands/CreatePlugin/CreatePluginCommandHandler.cs
public class CreatePluginCommandHandler : IRequestHandler<CreatePluginCommand, Result<PluginDto>>
{
    private readonly IPluginRepository _pluginRepository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<Result<PluginDto>> Handle(
        CreatePluginCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Create Plugin Entity
        var plugin = Plugin.Create(
            request.Name,
            request.Description,
            request.Type,
            request.FilePath,
            request.Metadata.ToDomain()
        );

        // 2. Save to Repository
        await _pluginRepository.AddAsync(plugin, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        // 3. Return DTO
        return Result<PluginDto>.Success(PluginDto.FromDomain(plugin));
    }
}
```

**參考文檔**:
- [CQRS Implementation](../../../docs/technical-implementation/01-backend-net9/05-cqrs-implementation.md)
- [Application Layer](../../../docs/architecture/layered-architecture/Application-Layer.md)

##### FluentValidation

```csharp
// Application/Plugins/Commands/CreatePlugin/CreatePluginCommandValidator.cs
public class CreatePluginCommandValidator : AbstractValidator<CreatePluginCommand>
{
    public CreatePluginCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Plugin name is required")
            .Length(3, 100).WithMessage("Plugin name must be between 3 and 100 characters");

        RuleFor(x => x.FilePath)
            .NotEmpty().WithMessage("Plugin file path is required")
            .Must(BeValidFilePath).WithMessage("Invalid file path format");

        RuleFor(x => x.Metadata.Version)
            .NotEmpty().WithMessage("Plugin version is required")
            .Must(BeValidSemVer).WithMessage("Version must follow SemVer format (e.g., 1.0.0)");
    }

    private bool BeValidSemVer(string version)
    {
        return SemanticVersion.TryParse(version, out _);
    }
}
```

**參考文檔**:
- [Validation Strategy](../../../docs/technical-implementation/01-backend-net9/06-validation-strategy.md)

#### Day 9-10: PluginLoader 實作 (T3.4 - 2 SP)

**負責人**: Backend Dev 3

```csharp
// Infrastructure/Services/IPluginLoader.cs
public interface IPluginLoader
{
    Task<LoadedPlugin> LoadPluginAsync(Plugin plugin, CancellationToken cancellationToken = default);
    Task UnloadPluginAsync(Guid pluginId, CancellationToken cancellationToken = default);
    IReadOnlyList<LoadedPlugin> GetLoadedPlugins();
}

// Infrastructure/Services/PluginLoader.cs
public class PluginLoader : IPluginLoader
{
    private readonly Dictionary<Guid, LoadedPlugin> _loadedPlugins = new();
    private readonly ILogger<PluginLoader> _logger;

    public async Task<LoadedPlugin> LoadPluginAsync(Plugin plugin, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Loading plugin {PluginName} of type {PluginType}",
            plugin.Name, plugin.Type);

        LoadedPlugin loadedPlugin = plugin.Type switch
        {
            PluginType.Native => await LoadNativePluginAsync(plugin, cancellationToken),
            PluginType.Semantic => await LoadSemanticPluginAsync(plugin, cancellationToken),
            _ => throw new NotSupportedException($"Plugin type {plugin.Type} is not supported")
        };

        _loadedPlugins[plugin.PluginId] = loadedPlugin;
        return loadedPlugin;
    }

    private async Task<LoadedPlugin> LoadNativePluginAsync(Plugin plugin, CancellationToken cancellationToken)
    {
        // 1. Create AppDomain for isolation
        var appDomainSetup = new AppDomainSetup
        {
            ApplicationBase = Path.GetDirectoryName(plugin.FilePath),
            PrivateBinPath = Path.GetDirectoryName(plugin.FilePath)
        };

        var appDomain = AppDomain.CreateDomain(
            $"Plugin_{plugin.PluginId}",
            null,
            appDomainSetup
        );

        // 2. Load assembly in isolated AppDomain
        var assembly = appDomain.Load(AssemblyName.GetAssemblyName(plugin.FilePath));

        // 3. Extract plugin functions
        var functions = ExtractPluginFunctions(assembly);

        return new LoadedPlugin(plugin.PluginId, appDomain, functions);
    }

    private async Task<LoadedPlugin> LoadSemanticPluginAsync(Plugin plugin, CancellationToken cancellationToken)
    {
        // Parse YAML and create semantic functions
        var yamlContent = await File.ReadAllTextAsync(plugin.FilePath, cancellationToken);
        var semanticFunctions = ParseSemanticPluginYaml(yamlContent);

        return new LoadedPlugin(plugin.PluginId, null, semanticFunctions);
    }
}
```

**參考文檔**:
- [Plugin System MCP](../../../docs/technical-implementation/01-backend-net9/04-plugin-system-mcp.md#plugin-loader)

#### Day 11: AppDomain 隔離與安全 (T3.5 - 2 SP)

**負責人**: Backend Dev 3

```csharp
// Infrastructure/Security/PluginSandbox.cs
public class PluginSandbox
{
    public static AppDomain CreateSecureAppDomain(Plugin plugin)
    {
        var permissions = new PermissionSet(PermissionState.None);

        // Add only required permissions based on plugin manifest
        if (plugin.Metadata.Permissions.Contains(Permission.FileAccess))
        {
            permissions.AddPermission(new FileIOPermission(
                FileIOPermissionAccess.Read,
                Path.GetDirectoryName(plugin.FilePath)));
        }

        if (plugin.Metadata.Permissions.Contains(Permission.NetworkAccess))
        {
            // Add limited network access
            permissions.AddPermission(new WebPermission(
                NetworkAccess.Connect,
                "https://api.allowed-domain.com"));
        }

        var appDomainSetup = new AppDomainSetup
        {
            ApplicationBase = Path.GetDirectoryName(plugin.FilePath),
            ApplicationName = $"Plugin_{plugin.PluginId}"
        };

        var appDomain = AppDomain.CreateDomain(
            appDomainSetup.ApplicationName,
            null,
            appDomainSetup,
            permissions
        );

        return appDomain;
    }
}
```

**參考文檔**:
- [Plugin Security](../../../docs/security/plugin-security.md#appdomain-isolation)
- [Code Execution Security](../../../docs/security/code-execution-security.md)

---

### Week 3: API + Integration + Testing (2 SP)

#### Day 12: MCP 協議整合 (T3.6 - 1 SP)

**負責人**: Backend Dev 2

```csharp
// Infrastructure/MCP/IMCPAdapter.cs
public interface IMCPAdapter
{
    Task<MCPResponse> SendToolCallAsync(string pluginId, MCPToolCall toolCall);
    Task<MCPMessage> SendMessageAsync(string pluginId, MCPMessage message);
}

// Infrastructure/MCP/MCPAdapter.cs
public class MCPAdapter : IMCPAdapter
{
    public async Task<MCPResponse> SendToolCallAsync(string pluginId, MCPToolCall toolCall)
    {
        // Implement MCP tool call protocol
        // Reference: https://modelcontextprotocol.io/docs/concepts/tools
    }
}
```

**參考文檔**:
- [Plugin System MCP](../../../docs/technical-implementation/01-backend-net9/04-plugin-system-mcp.md#mcp-integration)
- [Model Context Protocol Specification](https://modelcontextprotocol.io/)

#### Day 13-14: Plugin API Controller (T3.7 - 1 SP)

**負責人**: Backend Dev 1

```csharp
// API/Controllers/PluginsController.cs
[ApiController]
[Route("api/plugins")]
public class PluginsController : ControllerBase
{
    private readonly IMediator _mediator;

    [HttpPost]
    [ProducesResponseType(typeof(PluginDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePlugin(
        [FromBody] CreatePluginCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return CreatedAtAction(
            nameof(GetPluginById),
            new { id = result.Value.PluginId },
            result.Value);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PluginDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPluginById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetPluginByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<PluginDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlugins(
        [FromQuery] GetPluginsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result.Value);
    }

    [HttpPut("{id}/activate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivatePlugin(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new ActivatePluginCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return NotFound(result.Error);

        return Ok();
    }

    [HttpPut("{id}/deactivate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeactivatePlugin(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeactivatePluginCommand(id);
        await _mediator.Send(command, cancellationToken);
        return Ok();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeletePlugin(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeletePluginCommand(id);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
```

**參考文檔**:
- [Plugin API Design](../../../docs/api/plugin-api-design.md)
- [API Design Guidelines](../../../docs/api/API-Design-Guidelines.md)

#### Day 15: 整合測試 + 文檔

**所有開發者**

- 整合測試: Plugin CRUD 端到端測試
- API 測試: Postman Collection
- 效能測試: Plugin 載入效能 (目標 P95 <2 秒)
- 安全測試: AppDomain 隔離驗證
- Swagger 文檔完善

**參考文檔**:
- [Integration Testing](../../../docs/testing/integration-testing-guidelines.md)
- [Performance Testing](../../../docs/testing/performance-testing.md)

---

## 任務時間表

### Week 1: Domain + Infrastructure (Week 7)

| Day | 任務 | 負責人 | SP | 狀態 | 交付物 |
|-----|------|--------|----|----|--------|
| Mon-Tue | T3.1: Plugin Entity 設計 | Backend Dev 1 | 2 SP | 📋 | Plugin.cs, PluginMetadata.cs, Enums |
| Wed-Fri | T3.2: Plugin Repository | Backend Dev 1 | 2 SP | 📋 | IPluginRepository, PluginRepository, Migration |
| **Week 1 Total** | | | **5 SP** | | **Domain + Infrastructure 完成** |

### Week 2: Application + Security (Week 8)

| Day | 任務 | 負責人 | SP | 狀態 | 交付物 |
|-----|------|--------|----|----|--------|
| Mon-Wed | T3.3: CQRS Commands/Queries | Backend Dev 2 | 3 SP | 📋 | Commands, Queries, Handlers, Validators |
| Thu-Fri | T3.4: PluginLoader 實作 | Backend Dev 3 | 2 SP | 📋 | IPluginLoader, PluginLoader |
| Sat | T3.5: AppDomain 隔離 | Backend Dev 3 | 2 SP | 📋 | PluginSandbox, Security Config |
| **Week 2 Total** | | | **7 SP** (Over by 1 SP) | | **Application Layer + Security 完成** |

### Week 3: API + Integration (Week 9)

| Day | 任務 | 負責人 | SP | 狀態 | 交付物 |
|-----|------|--------|----|----|--------|
| Mon | T3.6: MCP 協議整合 | Backend Dev 2 | 1 SP | 📋 | IMCPAdapter, MCPAdapter |
| Tue-Wed | T3.7: Plugin API Controller | Backend Dev 1 | 1 SP | 📋 | PluginsController, Swagger Docs |
| Thu-Fri | 整合測試 + 文檔 | All Devs | - | 📋 | Tests, Postman, Performance Report |
| **Week 3 Total** | | | **2 SP** (Buffer: -1 SP) | | **API + Tests 完成** |

**總計**: 13 SP (實際: 14 SP，包含 1 SP Buffer)

---

## 驗收標準

### 功能驗收 (Functional Acceptance)

```yaml
Plugin CRUD API:
  ✅ 所有 6 個 API 端點正常運作
  測試方法: Postman Collection (50+ test cases)
  成功標準: 100% 測試通過
  參考: docs/api/plugin-api-design.md

Plugin 載入成功率:
  ✅ Native Plugin 載入: ≥95%
  ✅ Semantic Plugin 載入: ≥98%
  測試方法: 載入 20 個測試 Plugin (10 Native + 10 Semantic)
  成功標準: 符合目標
  參考: docs/testing/integration-testing-guidelines.md

安全隔離驗證:
  ✅ AppDomain 隔離有效: 100%
  測試方法: 5 個惡意 Plugin 測試 (檔案訪問、網路訪問、記憶體溢出、反射攻擊、序列化漏洞)
  成功標準: 所有惡意操作被阻擋
  參考: docs/security/plugin-security.md
```

### 非功能驗收 (Non-Functional Acceptance)

```yaml
Plugin 載入效能:
  目標: P95 <2 秒
  測試方法: k6 負載測試，載入 100 次
  成功標準: 95% 載入時間 <2 秒
  參考: docs/testing/performance-testing.md

API 響應時間:
  目標: P95 <500ms (GET), P95 <1s (POST)
  測試方法: k6 負載測試，50 併發用戶
  成功標準: 符合目標
  參考: docs/testing/performance-testing.md

記憶體使用:
  目標: 每個 Plugin AppDomain ≤256MB
  測試方法: .NET Memory Profiler
  成功標準: 10 個 Plugin 同時載入，總記憶體 ≤2.5GB
  參考: docs/testing/performance-testing.md
```

### 質量驗收 (Quality Acceptance)

```yaml
單元測試覆蓋率:
  目標: ≥80% (Domain + Application Layer)
  測試工具: dotnet test --collect:"XPlat Code Coverage"
  成功標準: Coverage Report ≥80%
  參考: docs/testing/unit-testing-guidelines.md

Code Review:
  目標: 100% PR 經過 Code Review
  審查標準: SOLID, Clean Code, Security Best Practices
  成功標準: 所有 PR 至少 1 個 Approve
  參考: docs/development-standards/code-review-guidelines.md

Bug Density:
  目標: ≤2 bugs / 100 LOC (critical + high)
  測量方法: Sprint Retrospective 統計
  成功標準: 符合目標
```

---

## 風險緩解計劃

### RISK-003: AppDomain 隔離技術複雜度 ⚠️ 高風險

**緩解計劃**:

**Week 1 (PoC 驗證)**:
- Day 1: 建立 AppDomain 隔離 PoC 專案
- Day 2: 測試跨 AppDomain 通訊效能
- Day 3: 評估記憶體使用情況
- **決策點**: 如 P95 >3 秒，考慮 AssemblyLoadContext 替代方案

**Fallback 方案**:
```yaml
Option 1: AssemblyLoadContext (Recommended)
  優點: .NET Core 原生支援，效能更好
  缺點: 隔離能力略弱於 AppDomain
  切換成本: 2 SP (重構 PluginLoader)

Option 2: Docker Container 隔離
  優點: 最強隔離能力
  缺點: 效能開銷大，複雜度高
  切換成本: 5 SP (延後到 Sprint 4)
```

**參考文檔**:
- [Risk Register](../../1-planning/RISK-REGISTER.md#risk-003)
- [Plugin Security](../../../docs/security/plugin-security.md#isolation-alternatives)

### RISK-019: MCP 協議整合複雜度 🟡 中風險

**緩解計劃**:

**Week 1 (研究階段)**:
- 調研 anthropic/mcp GitHub Repository
- 分析 .NET 社群 MCP 實作 (如有)
- 評估 MCP C# SDK 成熟度

**簡化策略**:
- Sprint 3 僅實作基礎 MCP 功能 (tool call)
- 高級功能 (streaming, resources) 延後到 Sprint 4
- 設計 IMCPAdapter 介面，允許未來替換實作

**Fallback 方案**:
```yaml
Option 1: 自建輕量級 MCP Adapter
  成本: +1 SP
  風險: 可能不符合 MCP 完整規範

Option 2: 暫時跳過 MCP，使用自定義協議
  成本: 0 SP
  風險: 未來需要重構以支援 MCP
```

**參考文檔**:
- [Risk Register](../../1-planning/RISK-REGISTER.md#risk-019)
- [Plugin System MCP](../../../docs/technical-implementation/01-backend-net9/04-plugin-system-mcp.md#mcp-integration)

---

## 📚 完整參考文獻索引

本執行計劃的技術規格、實作細節與驗收標準基於以下文檔，按類別組織以便快速定位：

### Planning 文檔（優先查閱）

- [MVP Scope Definition](../../1-planning/MVP-SCOPE-DEFINITION.md) - Plugin 系統在 MVP Phase 1A 的範圍定義
- [Sprint Allocation Analysis](../../1-planning/SPRINT-ALLOCATION-ANALYSIS.md) - Sprint 3 詳細任務分配 (US 1.3, 13 SP)
- [Dependency Matrix](../../1-planning/DEPENDENCY-MATRIX.md) - Sprint 3 依賴關係 (DEP-012: 依賴 Sprint 2 Agent 引擎)
- [Risk Register](../../1-planning/RISK-REGISTER.md) - Sprint 3 風險管理 (RISK-003, RISK-019, RISK-020)
- [Development Strategy](../../1-planning/DEVELOPMENT-STRATEGY.md) - Plugin 系統開發策略與最佳實踐

### User Stories

- [US 2: Plugin System](../../../docs/user-stories/us-2-plugin-system.md) - Plugin 系統完整需求 (US 2.1 基礎架構 in Sprint 3)

### Architecture 文檔

- [Architecture Design Document](../../../docs/architecture/Architecture-Design-Document.md) - Clean Architecture 完整設計
- [Domain Layer](../../../docs/architecture/layered-architecture/Domain-Layer.md) - Plugin Entity, Value Objects 設計規範
- [Application Layer](../../../docs/architecture/layered-architecture/Application-Layer.md) - CQRS Commands/Queries 實作規範
- [Infrastructure Layer](../../../docs/architecture/layered-architecture/Infrastructure-Layer.md) - Repository, PluginLoader 實作規範

### ADR（架構決策）

- [ADR-003: Repository Pattern](../../../docs/architecture/adr/ADR-003-repository-pattern.md) - Repository 模式選擇理由
- [ADR-011: Framework Abstraction](../../../docs/architecture/adr/ADR-011-framework-abstraction-layer.md) - IToolRegistry 與 Plugin 整合

### 技術實施文檔

- [Plugin System MCP](../../../docs/technical-implementation/01-backend-net9/04-plugin-system-mcp.md) - Plugin 系統完整實作指南 (Native + Semantic, MCP 整合)
- [CQRS Implementation](../../../docs/technical-implementation/01-backend-net9/05-cqrs-implementation.md) - MediatR Command/Query 實作
- [Validation Strategy](../../../docs/technical-implementation/01-backend-net9/06-validation-strategy.md) - FluentValidation 實作規範

### API 與 Database 文檔

- [Plugin API Design](../../../docs/api/plugin-api-design.md) - Plugin RESTful API 完整設計
- [API Design Guidelines](../../../docs/api/API-Design-Guidelines.md) - RESTful API 設計規範
- [Plugin Schema](../../../docs/database/plugin-schema.md) - Plugins, PluginMetadata 資料表設計
- [Migration Guide](../../../docs/database/migration-guide.md) - EF Core Migration 最佳實踐

### 安全性文檔

- [Plugin Security](../../../docs/security/plugin-security.md) - AppDomain 隔離、Plugin 簽名、權限模型
- [Code Execution Security](../../../docs/security/code-execution-security.md) - 代碼執行安全最佳實踐

### 測試文檔

- [Unit Testing Guidelines](../../../docs/testing/unit-testing-guidelines.md) - Plugin 單元測試規範 (覆蓋率 ≥80%)
- [Integration Testing Guidelines](../../../docs/testing/integration-testing-guidelines.md) - Plugin 整合測試規範
- [Performance Testing](../../../docs/testing/performance-testing.md) - Plugin 載入效能測試 (目標 P95 <2 秒)

### 開發標準

- [Code Review Guidelines](../../../docs/development-standards/code-review-guidelines.md) - Plugin 代碼審查標準
- [Coding Conventions](../../../docs/development-standards/coding-conventions.md) - C# 編碼規範

---

**文檔建立日期**: 2025-11-13
**文檔維護者**: Backend Team Lead
**最後審查日期**: 2025-11-13

---

**變更日誌 (Change Log)**:

- **v2.0** (2025-11-13): 初始版本建立
  - ✅ 添加完整 3 週執行計劃 (7 tasks, 13 SP)
  - ✅ 詳細技術規格與代碼範例
  - ✅ 完整驗收標準 (功能、非功能、質量)
  - ✅ 風險緩解計劃 (RISK-003, RISK-019)
  - ✅ 完整參考文獻索引 (40+ 文檔)
  - ✅ 遵循 v2.0 標準與 Sprint 1/2 模板結構
