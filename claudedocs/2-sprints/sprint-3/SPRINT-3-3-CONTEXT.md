# Sprint 3 上下文參考文檔

**目的**: 為 AI Assistant 提供 Sprint 3 開發所需的快速參考與上下文定位

**使用方式**:
- 🔗 包含關鍵文檔的**精確行號**，避免全文搜索
- 📋 提供 MVP 範圍快速參考
- 🎯 API 規格速查表
- 🏗️ 技術架構提醒
- ⚙️ 編碼標準參考

**維護**: Sprint 3 開發期間保持更新

---

## 📖 關鍵文檔索引（帶行號）

### Sprint 3 執行文檔

| 文檔 | 路徑 | 關鍵內容 | 行號範圍 |
|------|------|---------|---------|
| **Sprint 3 概覽** | `claudedocs/2-sprints/sprint-3/SPRINT-3-1-OVERVIEW.md` | Sprint 目標、User Stories 狀態、關鍵指標 | 1-850+ |
| **Sprint 3 執行計劃** | `claudedocs/2-sprints/sprint-3/SPRINT-3-2-PLAN.md` | 技術實施細節、代碼範例、API 規格 | 1-1200+ |

### 項目規劃文檔（/claudedocs/1-planning 參考）

| 文檔 | 路徑 | 關鍵內容 | 快速定位 |
|------|------|---------|---------|
| **Sprint 分配分析** | `claudedocs/1-planning/SPRINT-ALLOCATION-ANALYSIS.md` | Sprint 3 = US 1.3 Plugin System (13 SP), Week 7-9, Phase 1A | Sprint 3 section |
| **MVP 範圍定義** | `claudedocs/1-planning/MVP-SCOPE-DEFINITION.md` | Plugin System 在 Phase 1A 的範圍、安全需求、技術邊界 | Phase 1A section |
| **開發策略** | `claudedocs/1-planning/DEVELOPMENT-STRATEGY.md` | Clean Architecture 實施策略、CQRS 模式、Repository Pattern | Architecture section |
| **依賴矩陣** | `claudedocs/1-planning/DEPENDENCY-MATRIX.md` | US 1.3 依賴 Sprint 2 Agent Engine (DEP-012) | US 1.3 dependencies |
| **風險登記簿** | `claudedocs/1-planning/RISK-REGISTER.md` | RISK-003 (AppDomain 隔離), RISK-019 (MCP 協議), RISK-020 (安全漏洞) | High/Medium risks |
| **技術決策日誌** | `claudedocs/1-planning/TECHNICAL-DECISIONS-LOG.md` | Plugin 隔離技術選擇（AppDomain vs AssemblyLoadContext） | Plugin isolation decisions |

### 參考層文檔（/docs 參考）

| 文檔 | 路徑 | 關鍵內容 | 快速定位 |
|------|------|---------|---------|
| **US 1.3 規格** | `docs/user-stories/modules/module-02-plugin-system.md` (line 1-150) | Plugin 系統基礎架構、Domain Model、CRUD API、安全隔離 | Epic 2: Plugin System - US 1.3 |
| **架構設計總覽** | `docs/architecture/Architecture-Design-Document.md` | Clean Architecture 分層、Plugin 系統架構、技術棧、性能目標 | Plugin System section |
| **數據庫設計** | `docs/architecture/database-schema.md` | PostgreSQL Schema、plugins 表、plugin_metadata 表、索引策略 | Plugin tables |
| **C4 架構圖** | `docs/architecture/C4-architecture-diagrams.md` | Plugin 系統組件圖、隔離架構 | Plugin System Component diagram |

---

## 🎯 MVP 範圍快速參考

### US 1.3: Plugin 系統基礎架構 (13 SP, 7 Tasks)

**Sprint 目標**: 建立 Plugin 基礎架構，支援 Native 和 Semantic Plugin 的 CRUD 管理與動態載入

#### Phase 1: Domain Layer Design (Week 1, Days 1-2, T3.1 - 2 SP) ✅

**核心實體**:

```csharp
// Plugin Entity - Domain Layer
public class Plugin : BaseEntity
{
    public Guid PluginId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public PluginType Type { get; private set; }        // Native | Semantic
    public PluginStatus Status { get; private set; }    // Draft | Active | Inactive | Archived
    public string FilePath { get; private set; }
    public PluginMetadata Metadata { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Factory Method
    public static Plugin Create(
        string name,
        string description,
        PluginType type,
        string filePath,
        PluginMetadata metadata)
    {
        // Domain validation logic
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Plugin name cannot be empty");
        if (string.IsNullOrWhiteSpace(filePath))
            throw new DomainException("Plugin file path is required");

        return new Plugin
        {
            PluginId = Guid.NewGuid(),
            Name = name,
            Description = description,
            Type = type,
            Status = PluginStatus.Draft,
            FilePath = filePath,
            Metadata = metadata,
            CreatedAt = DateTime.UtcNow
        };
    }

    // Business Logic Methods
    public void Activate()
    {
        if (Status == PluginStatus.Archived)
            throw new DomainException("Cannot activate archived plugin. Restore first.");
        Status = PluginStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (Status == PluginStatus.Draft)
            throw new DomainException("Cannot deactivate a draft plugin");
        Status = PluginStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Archive()
    {
        Status = PluginStatus.Archived;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Restore()
    {
        if (Status != PluginStatus.Archived)
            throw new DomainException("Only archived plugins can be restored");
        Status = PluginStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
    }
}
```

**Value Objects**:

```csharp
// PluginMetadata Value Object
public class PluginMetadata : ValueObject
{
    public SemanticVersion Version { get; private set; }
    public string Author { get; private set; }
    public List<string> Dependencies { get; private set; }
    public List<Permission> Permissions { get; private set; }
    public Dictionary<string, string> Tags { get; private set; }

    public static PluginMetadata Create(
        string version,
        string author,
        List<string> dependencies = null,
        List<Permission> permissions = null,
        Dictionary<string, string> tags = null)
    {
        return new PluginMetadata
        {
            Version = SemanticVersion.Parse(version),
            Author = author ?? "Unknown",
            Dependencies = dependencies ?? new List<string>(),
            Permissions = permissions ?? new List<Permission>(),
            Tags = tags ?? new Dictionary<string, string>()
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Version;
        yield return Author;
        yield return string.Join(",", Dependencies.OrderBy(d => d));
        yield return string.Join(",", Permissions.OrderBy(p => p));
    }
}

// SemanticVersion Value Object
public class SemanticVersion : ValueObject
{
    public int Major { get; private set; }
    public int Minor { get; private set; }
    public int Patch { get; private set; }

    public static SemanticVersion Parse(string version)
    {
        // Parse SemVer format: 1.2.3
        var parts = version.Split('.');
        if (parts.Length != 3)
            throw new ArgumentException("Invalid SemVer format. Expected: Major.Minor.Patch");

        return new SemanticVersion
        {
            Major = int.Parse(parts[0]),
            Minor = int.Parse(parts[1]),
            Patch = int.Parse(parts[2])
        };
    }

    public override string ToString() => $"{Major}.{Minor}.{Patch}";

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Major;
        yield return Minor;
        yield return Patch;
    }
}
```

**Enums**:

```csharp
public enum PluginType
{
    Native = 1,      // C# DLL Plugin with compiled code
    Semantic = 2     // YAML-based Prompt Plugin
}

public enum PluginStatus
{
    Draft = 0,       // Initial state, not loaded
    Active = 1,      // Loaded and available
    Inactive = 2,    // Temporarily disabled
    Archived = 3     // Soft deleted
}

public enum Permission
{
    FileAccess = 1,
    NetworkAccess = 2,
    DatabaseAccess = 3,
    EnvironmentVariableAccess = 4
}
```

---

#### Phase 2: Infrastructure Layer - Repository (Week 1, Days 3-5, T3.2 - 2 SP) ✅

**Repository Interface**:

```csharp
// IPluginRepository - Domain Layer Interface
public interface IPluginRepository
{
    // Basic CRUD
    Task<Plugin> GetByIdAsync(Guid pluginId, CancellationToken cancellationToken = default);
    Task<Plugin> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<PagedResult<Plugin>> GetPagedAsync(
        int page,
        int pageSize,
        PluginType? type = null,
        PluginStatus? status = null,
        CancellationToken cancellationToken = default);
    Task<Plugin> AddAsync(Plugin plugin, CancellationToken cancellationToken = default);
    Task UpdateAsync(Plugin plugin, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid pluginId, CancellationToken cancellationToken = default);

    // Advanced Queries
    Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default);
    Task<List<Plugin>> GetActivePluginsAsync(CancellationToken cancellationToken = default);
    Task<List<Plugin>> GetByTypeAsync(PluginType type, CancellationToken cancellationToken = default);
}
```

**Repository Implementation**:

```csharp
// PluginRepository - Infrastructure Layer
public class PluginRepository : IPluginRepository
{
    private readonly ApplicationDbContext _context;

    public PluginRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Plugin> GetByIdAsync(Guid pluginId, CancellationToken cancellationToken = default)
    {
        return await _context.Plugins
            .FirstOrDefaultAsync(p => p.PluginId == pluginId && !p.IsDeleted, cancellationToken);
    }

    public async Task<Plugin> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Plugins
            .FirstOrDefaultAsync(p => p.Name == name && !p.IsDeleted, cancellationToken);
    }

    public async Task<PagedResult<Plugin>> GetPagedAsync(
        int page,
        int pageSize,
        PluginType? type = null,
        PluginStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Plugins.Where(p => !p.IsDeleted);

        if (type.HasValue)
            query = query.Where(p => p.Type == type.Value);

        if (status.HasValue)
            query = query.Where(p => p.Status == status.Value);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Plugin>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<Plugin> AddAsync(Plugin plugin, CancellationToken cancellationToken = default)
    {
        await _context.Plugins.AddAsync(plugin, cancellationToken);
        return plugin;
    }

    public Task UpdateAsync(Plugin plugin, CancellationToken cancellationToken = default)
    {
        _context.Plugins.Update(plugin);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid pluginId, CancellationToken cancellationToken = default)
    {
        var plugin = await GetByIdAsync(pluginId, cancellationToken);
        if (plugin != null)
        {
            plugin.IsDeleted = true;
            plugin.UpdatedAt = DateTime.UtcNow;
        }
    }

    public async Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Plugins
            .AnyAsync(p => p.Name == name && !p.IsDeleted, cancellationToken);
    }

    public async Task<List<Plugin>> GetActivePluginsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Plugins
            .Where(p => p.Status == PluginStatus.Active && !p.IsDeleted)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Plugin>> GetByTypeAsync(PluginType type, CancellationToken cancellationToken = default)
    {
        return await _context.Plugins
            .Where(p => p.Type == type && !p.IsDeleted)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }
}
```

**EF Core Configuration**:

```csharp
// PluginConfiguration - Infrastructure Layer
public class PluginConfiguration : IEntityTypeConfiguration<Plugin>
{
    public void Configure(EntityTypeBuilder<Plugin> builder)
    {
        builder.ToTable("plugins");

        builder.HasKey(p => p.PluginId);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.Property(p => p.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(p => p.FilePath)
            .IsRequired()
            .HasMaxLength(500);

        // Value Object: PluginMetadata stored as JSONB
        builder.OwnsOne(p => p.Metadata, metadata =>
        {
            metadata.ToJson();
            metadata.Property(m => m.Author).HasMaxLength(100);
        });

        builder.HasIndex(p => p.Name).IsUnique();
        builder.HasIndex(p => p.Type);
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.CreatedAt);
    }
}
```

**EF Migration**:

```bash
# Migration Command
cd src/AIAgentPlatform.Infrastructure
dotnet ef migrations add AddPluginEntity --startup-project ../AIAgentPlatform.API

# Migration File: 20251115000000_AddPluginEntity.cs
public partial class AddPluginEntity : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "plugins",
            columns: table => new
            {
                plugin_id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                type = table.Column<int>(type: "integer", nullable: false),
                status = table.Column<int>(type: "integer", nullable: false),
                file_path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                metadata = table.Column<string>(type: "jsonb", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                updated_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_plugins", x => x.plugin_id);
            });

        migrationBuilder.CreateIndex(
            name: "ix_plugins_name",
            table: "plugins",
            column: "name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_plugins_type",
            table: "plugins",
            column: "type");

        migrationBuilder.CreateIndex(
            name: "ix_plugins_status",
            table: "plugins",
            column: "status");

        migrationBuilder.CreateIndex(
            name: "ix_plugins_created_at",
            table: "plugins",
            column: "created_at");

        // JSONB GIN Index for metadata search
        migrationBuilder.Sql(
            "CREATE INDEX ix_plugins_metadata ON plugins USING GIN (metadata jsonb_path_ops);");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "plugins");
    }
}
```

---

#### Phase 3: Application Layer - CQRS Commands/Queries (Week 2, Days 6-8, T3.3 - 3 SP) ✅

**Create Plugin Command**:

```csharp
// CreatePluginCommand - Application Layer
public record CreatePluginCommand(
    string Name,
    string Description,
    PluginType Type,
    string FilePath,
    PluginMetadataDto Metadata
) : IRequest<Result<PluginDto>>;

// CreatePluginCommandHandler
public class CreatePluginCommandHandler : IRequestHandler<CreatePluginCommand, Result<PluginDto>>
{
    private readonly IPluginRepository _pluginRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreatePluginCommandHandler> _logger;

    public CreatePluginCommandHandler(
        IPluginRepository pluginRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreatePluginCommandHandler> logger)
    {
        _pluginRepository = pluginRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<PluginDto>> Handle(
        CreatePluginCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Check if plugin name already exists
            if (await _pluginRepository.ExistsAsync(request.Name, cancellationToken))
            {
                return Result<PluginDto>.Failure($"Plugin with name '{request.Name}' already exists");
            }

            // Convert DTO to Domain Value Object
            var metadata = PluginMetadata.Create(
                request.Metadata.Version,
                request.Metadata.Author,
                request.Metadata.Dependencies,
                request.Metadata.Permissions,
                request.Metadata.Tags
            );

            // Create Domain Entity using Factory Method
            var plugin = Plugin.Create(
                request.Name,
                request.Description,
                request.Type,
                request.FilePath,
                metadata
            );

            // Persist to Database
            await _pluginRepository.AddAsync(plugin, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation(
                "Plugin created successfully: {PluginId} - {PluginName}",
                plugin.PluginId,
                plugin.Name
            );

            // Convert Domain Entity to DTO
            return Result<PluginDto>.Success(PluginDto.FromDomain(plugin));
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain validation failed for plugin creation");
            return Result<PluginDto>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating plugin: {PluginName}", request.Name);
            return Result<PluginDto>.Failure("An error occurred while creating the plugin");
        }
    }
}

// FluentValidation Validator
public class CreatePluginCommandValidator : AbstractValidator<CreatePluginCommand>
{
    public CreatePluginCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Plugin name is required")
            .MaximumLength(200).WithMessage("Plugin name cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid plugin type");

        RuleFor(x => x.FilePath)
            .NotEmpty().WithMessage("File path is required")
            .MaximumLength(500).WithMessage("File path cannot exceed 500 characters");

        RuleFor(x => x.Metadata)
            .NotNull().WithMessage("Plugin metadata is required");

        RuleFor(x => x.Metadata.Version)
            .NotEmpty().WithMessage("Plugin version is required")
            .Matches(@"^\d+\.\d+\.\d+$").WithMessage("Version must follow SemVer format (e.g., 1.0.0)");

        RuleFor(x => x.Metadata.Author)
            .NotEmpty().WithMessage("Plugin author is required");
    }
}
```

**Get Plugins Query**:

```csharp
// GetPluginsQuery - Application Layer
public record GetPluginsQuery(
    int Page,
    int PageSize,
    PluginType? Type = null,
    PluginStatus? Status = null
) : IRequest<Result<PagedResult<PluginDto>>>;

// GetPluginsQueryHandler
public class GetPluginsQueryHandler : IRequestHandler<GetPluginsQuery, Result<PagedResult<PluginDto>>>
{
    private readonly IPluginRepository _pluginRepository;
    private readonly ILogger<GetPluginsQueryHandler> _logger;

    public GetPluginsQueryHandler(
        IPluginRepository pluginRepository,
        ILogger<GetPluginsQueryHandler> logger)
    {
        _pluginRepository = pluginRepository;
        _logger = logger;
    }

    public async Task<Result<PagedResult<PluginDto>>> Handle(
        GetPluginsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var pagedPlugins = await _pluginRepository.GetPagedAsync(
                request.Page,
                request.PageSize,
                request.Type,
                request.Status,
                cancellationToken
            );

            var pluginDtos = pagedPlugins.Items
                .Select(p => PluginDto.FromDomain(p))
                .ToList();

            var result = new PagedResult<PluginDto>
            {
                Items = pluginDtos,
                TotalCount = pagedPlugins.TotalCount,
                Page = pagedPlugins.Page,
                PageSize = pagedPlugins.PageSize
            };

            return Result<PagedResult<PluginDto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving plugins: Page {Page}, PageSize {PageSize}",
                request.Page, request.PageSize);
            return Result<PagedResult<PluginDto>>.Failure("An error occurred while retrieving plugins");
        }
    }
}

// GetPluginsQueryValidator
public class GetPluginsQueryValidator : AbstractValidator<GetPluginsQuery>
{
    public GetPluginsQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100");

        RuleFor(x => x.Type)
            .IsInEnum().When(x => x.Type.HasValue).WithMessage("Invalid plugin type");

        RuleFor(x => x.Status)
            .IsInEnum().When(x => x.Status.HasValue).WithMessage("Invalid plugin status");
    }
}
```

**Update Plugin Command**:

```csharp
// UpdatePluginCommand - Application Layer
public record UpdatePluginCommand(
    Guid PluginId,
    string Name,
    string Description,
    PluginMetadataDto Metadata
) : IRequest<Result<PluginDto>>;

// UpdatePluginCommandHandler
public class UpdatePluginCommandHandler : IRequestHandler<UpdatePluginCommand, Result<PluginDto>>
{
    private readonly IPluginRepository _pluginRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdatePluginCommandHandler> _logger;

    public async Task<Result<PluginDto>> Handle(
        UpdatePluginCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var plugin = await _pluginRepository.GetByIdAsync(request.PluginId, cancellationToken);
            if (plugin == null)
            {
                return Result<PluginDto>.Failure($"Plugin with ID '{request.PluginId}' not found");
            }

            // Update plugin properties (via reflection or explicit setters)
            plugin.UpdateName(request.Name);
            plugin.UpdateDescription(request.Description);

            var metadata = PluginMetadata.Create(
                request.Metadata.Version,
                request.Metadata.Author,
                request.Metadata.Dependencies,
                request.Metadata.Permissions,
                request.Metadata.Tags
            );
            plugin.UpdateMetadata(metadata);

            await _pluginRepository.UpdateAsync(plugin, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("Plugin updated successfully: {PluginId}", plugin.PluginId);

            return Result<PluginDto>.Success(PluginDto.FromDomain(plugin));
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain validation failed for plugin update");
            return Result<PluginDto>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating plugin: {PluginId}", request.PluginId);
            return Result<PluginDto>.Failure("An error occurred while updating the plugin");
        }
    }
}
```

**Activate/Deactivate Plugin Commands**:

```csharp
// ActivatePluginCommand
public record ActivatePluginCommand(Guid PluginId) : IRequest<Result<PluginDto>>;

// ActivatePluginCommandHandler
public class ActivatePluginCommandHandler : IRequestHandler<ActivatePluginCommand, Result<PluginDto>>
{
    private readonly IPluginRepository _pluginRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPluginActivator _pluginActivator;
    private readonly ILogger<ActivatePluginCommandHandler> _logger;

    public async Task<Result<PluginDto>> Handle(
        ActivatePluginCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var plugin = await _pluginRepository.GetByIdAsync(request.PluginId, cancellationToken);
            if (plugin == null)
            {
                return Result<PluginDto>.Failure($"Plugin with ID '{request.PluginId}' not found");
            }

            // Domain logic: Change status to Active
            plugin.Activate();

            // Infrastructure: Activate plugin in runtime
            await _pluginActivator.ActivatePluginAsync(plugin, cancellationToken);

            await _pluginRepository.UpdateAsync(plugin, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("Plugin activated successfully: {PluginId}", plugin.PluginId);

            return Result<PluginDto>.Success(PluginDto.FromDomain(plugin));
        }
        catch (DomainException ex)
        {
            return Result<PluginDto>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating plugin: {PluginId}", request.PluginId);
            return Result<PluginDto>.Failure("An error occurred while activating the plugin");
        }
    }
}

// DeactivatePluginCommand
public record DeactivatePluginCommand(Guid PluginId) : IRequest<Result<PluginDto>>;

// DeactivatePluginCommandHandler (similar structure)
```

**Delete Plugin Command**:

```csharp
// DeletePluginCommand
public record DeletePluginCommand(Guid PluginId) : IRequest<Result>;

// DeletePluginCommandHandler
public class DeletePluginCommandHandler : IRequestHandler<DeletePluginCommand, Result>
{
    private readonly IPluginRepository _pluginRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPluginLoader _pluginLoader;
    private readonly ILogger<DeletePluginCommandHandler> _logger;

    public async Task<Result> Handle(
        DeletePluginCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var plugin = await _pluginRepository.GetByIdAsync(request.PluginId, cancellationToken);
            if (plugin == null)
            {
                return Result.Failure($"Plugin with ID '{request.PluginId}' not found");
            }

            // Infrastructure: Unload plugin from runtime if loaded
            await _pluginLoader.UnloadPluginAsync(request.PluginId, cancellationToken);

            // Domain: Soft delete
            await _pluginRepository.DeleteAsync(request.PluginId, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("Plugin deleted successfully: {PluginId}", request.PluginId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting plugin: {PluginId}", request.PluginId);
            return Result.Failure("An error occurred while deleting the plugin");
        }
    }
}
```

---

#### Phase 4: Infrastructure - PluginLoader (Week 2, Days 9-10, T3.4 - 2 SP) ✅

**PluginLoader Interface**:

```csharp
// IPluginLoader - Application Layer Interface
public interface IPluginLoader
{
    Task<LoadedPlugin> LoadPluginAsync(Plugin plugin, CancellationToken cancellationToken = default);
    Task UnloadPluginAsync(Guid pluginId, CancellationToken cancellationToken = default);
    IReadOnlyList<LoadedPlugin> GetLoadedPlugins();
    LoadedPlugin GetLoadedPlugin(Guid pluginId);
    bool IsPluginLoaded(Guid pluginId);
}

// LoadedPlugin DTO
public record LoadedPlugin(
    Guid PluginId,
    string Name,
    PluginType Type,
    object Instance,    // Plugin instance (Native: IPlugin, Semantic: KernelFunction)
    DateTime LoadedAt
);
```

**PluginLoader Implementation**:

```csharp
// PluginLoader - Infrastructure Layer
public class PluginLoader : IPluginLoader
{
    private readonly ILogger<PluginLoader> _logger;
    private readonly ISemanticKernelFactory _kernelFactory;
    private readonly Dictionary<Guid, LoadedPlugin> _loadedPlugins = new();
    private readonly Dictionary<Guid, AssemblyLoadContext> _loadContexts = new();

    public PluginLoader(
        ILogger<PluginLoader> logger,
        ISemanticKernelFactory kernelFactory)
    {
        _logger = logger;
        _kernelFactory = kernelFactory;
    }

    public async Task<LoadedPlugin> LoadPluginAsync(Plugin plugin, CancellationToken cancellationToken)
    {
        if (_loadedPlugins.ContainsKey(plugin.PluginId))
        {
            _logger.LogWarning("Plugin {PluginId} is already loaded", plugin.PluginId);
            return _loadedPlugins[plugin.PluginId];
        }

        LoadedPlugin loadedPlugin = plugin.Type switch
        {
            PluginType.Native => await LoadNativePluginAsync(plugin, cancellationToken),
            PluginType.Semantic => await LoadSemanticPluginAsync(plugin, cancellationToken),
            _ => throw new NotSupportedException($"Plugin type {plugin.Type} is not supported")
        };

        _loadedPlugins[plugin.PluginId] = loadedPlugin;
        _logger.LogInformation(
            "Plugin loaded successfully: {PluginId} - {PluginName} ({PluginType})",
            plugin.PluginId,
            plugin.Name,
            plugin.Type
        );

        return loadedPlugin;
    }

    private async Task<LoadedPlugin> LoadNativePluginAsync(
        Plugin plugin,
        CancellationToken cancellationToken)
    {
        // Create isolated AssemblyLoadContext for plugin
        var loadContext = new AssemblyLoadContext(
            $"Plugin_{plugin.PluginId}",
            isCollectible: true
        );

        try
        {
            // Load plugin assembly from file path
            var assembly = loadContext.LoadFromAssemblyPath(plugin.FilePath);

            // Find IPlugin implementation
            var pluginType = assembly.GetTypes()
                .FirstOrDefault(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            if (pluginType == null)
            {
                throw new InvalidOperationException(
                    $"Plugin assembly {plugin.FilePath} does not contain a valid IPlugin implementation"
                );
            }

            // Create plugin instance
            var pluginInstance = Activator.CreateInstance(pluginType) as IPlugin;
            if (pluginInstance == null)
            {
                throw new InvalidOperationException($"Failed to create instance of plugin type {pluginType.Name}");
            }

            // Initialize plugin
            await pluginInstance.InitializeAsync(cancellationToken);

            // Store load context for later unloading
            _loadContexts[plugin.PluginId] = loadContext;

            return new LoadedPlugin(
                plugin.PluginId,
                plugin.Name,
                PluginType.Native,
                pluginInstance,
                DateTime.UtcNow
            );
        }
        catch
        {
            // Cleanup on failure
            loadContext.Unload();
            throw;
        }
    }

    private async Task<LoadedPlugin> LoadSemanticPluginAsync(
        Plugin plugin,
        CancellationToken cancellationToken)
    {
        // Load YAML file
        var yamlContent = await File.ReadAllTextAsync(plugin.FilePath, cancellationToken);

        // Parse YAML to KernelFunction
        var kernel = _kernelFactory.CreateKernel();
        var function = kernel.CreateFunctionFromPromptYaml(yamlContent);

        return new LoadedPlugin(
            plugin.PluginId,
            plugin.Name,
            PluginType.Semantic,
            function,
            DateTime.UtcNow
        );
    }

    public async Task UnloadPluginAsync(Guid pluginId, CancellationToken cancellationToken)
    {
        if (!_loadedPlugins.TryGetValue(pluginId, out var loadedPlugin))
        {
            _logger.LogWarning("Plugin {PluginId} is not loaded", pluginId);
            return;
        }

        // Dispose plugin instance if disposable
        if (loadedPlugin.Instance is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync();
        }
        else if (loadedPlugin.Instance is IDisposable disposable)
        {
            disposable.Dispose();
        }

        // Unload AssemblyLoadContext for Native plugins
        if (loadedPlugin.Type == PluginType.Native &&
            _loadContexts.TryGetValue(pluginId, out var loadContext))
        {
            loadContext.Unload();
            _loadContexts.Remove(pluginId);
        }

        _loadedPlugins.Remove(pluginId);
        _logger.LogInformation("Plugin unloaded successfully: {PluginId}", pluginId);
    }

    public IReadOnlyList<LoadedPlugin> GetLoadedPlugins()
    {
        return _loadedPlugins.Values.ToList().AsReadOnly();
    }

    public LoadedPlugin GetLoadedPlugin(Guid pluginId)
    {
        return _loadedPlugins.TryGetValue(pluginId, out var plugin) ? plugin : null;
    }

    public bool IsPluginLoaded(Guid pluginId)
    {
        return _loadedPlugins.ContainsKey(pluginId);
    }
}

// IPlugin Interface - Domain Layer
public interface IPlugin : IAsyncDisposable
{
    string Name { get; }
    string Version { get; }
    Task InitializeAsync(CancellationToken cancellationToken = default);
    Task<string> ExecuteAsync(string input, CancellationToken cancellationToken = default);
}
```

---

#### Phase 5: Security - AppDomain Isolation (Week 2, Day 11, T3.5 - 2 SP) ✅

**PluginSandbox for Security Isolation**:

```csharp
// PluginSandbox - Infrastructure Layer
public class PluginSandbox
{
    private readonly ILogger<PluginSandbox> _logger;

    public PluginSandbox(ILogger<PluginSandbox> logger)
    {
        _logger = logger;
    }

    public AppDomain CreateSecureAppDomain(Plugin plugin)
    {
        // Create restricted permission set
        var permissions = new PermissionSet(PermissionState.None);

        // Grant permissions based on plugin metadata
        if (plugin.Metadata.Permissions.Contains(Permission.FileAccess))
        {
            var filePermission = new FileIOPermission(
                FileIOPermissionAccess.Read,
                Path.GetDirectoryName(plugin.FilePath)
            );
            permissions.AddPermission(filePermission);
        }

        if (plugin.Metadata.Permissions.Contains(Permission.NetworkAccess))
        {
            var webPermission = new WebPermission(PermissionState.Unrestricted);
            permissions.AddPermission(webPermission);
        }

        // Setup AppDomain with restricted permissions
        var appDomainSetup = new AppDomainSetup
        {
            ApplicationBase = Path.GetDirectoryName(plugin.FilePath),
            DisallowCodeDownload = true
        };

        var appDomain = AppDomain.CreateDomain(
            $"Plugin_{plugin.PluginId}",
            null,
            appDomainSetup,
            permissions
        );

        _logger.LogInformation(
            "Created secure AppDomain for plugin {PluginId} with permissions: {Permissions}",
            plugin.PluginId,
            string.Join(", ", plugin.Metadata.Permissions)
        );

        return appDomain;
    }

    public void UnloadAppDomain(AppDomain appDomain)
    {
        try
        {
            AppDomain.Unload(appDomain);
            _logger.LogInformation("AppDomain unloaded successfully: {AppDomainName}", appDomain.FriendlyName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unloading AppDomain: {AppDomainName}", appDomain.FriendlyName);
            throw;
        }
    }
}
```

**Enhanced PluginLoader with AppDomain Isolation**:

```csharp
// Enhanced PluginLoader with AppDomain support
public class SecurePluginLoader : IPluginLoader
{
    private readonly ILogger<SecurePluginLoader> _logger;
    private readonly PluginSandbox _sandbox;
    private readonly Dictionary<Guid, (LoadedPlugin Plugin, AppDomain AppDomain)> _loadedPlugins = new();

    public async Task<LoadedPlugin> LoadPluginAsync(Plugin plugin, CancellationToken cancellationToken)
    {
        if (plugin.Type == PluginType.Native && plugin.Metadata.Permissions.Any())
        {
            // Create secure AppDomain for Native plugins with permissions
            var appDomain = _sandbox.CreateSecureAppDomain(plugin);

            try
            {
                // Load plugin in isolated AppDomain
                var proxyType = typeof(PluginProxy);
                var proxy = (PluginProxy)appDomain.CreateInstanceAndUnwrap(
                    proxyType.Assembly.FullName,
                    proxyType.FullName
                );

                await proxy.LoadPluginAsync(plugin.FilePath, cancellationToken);

                var loadedPlugin = new LoadedPlugin(
                    plugin.PluginId,
                    plugin.Name,
                    PluginType.Native,
                    proxy,
                    DateTime.UtcNow
                );

                _loadedPlugins[plugin.PluginId] = (loadedPlugin, appDomain);

                _logger.LogInformation(
                    "Plugin loaded in secure AppDomain: {PluginId} - {PluginName}",
                    plugin.PluginId,
                    plugin.Name
                );

                return loadedPlugin;
            }
            catch
            {
                _sandbox.UnloadAppDomain(appDomain);
                throw;
            }
        }
        else
        {
            // Use AssemblyLoadContext for plugins without special permissions
            return await LoadWithAssemblyLoadContextAsync(plugin, cancellationToken);
        }
    }

    public async Task UnloadPluginAsync(Guid pluginId, CancellationToken cancellationToken)
    {
        if (_loadedPlugins.TryGetValue(pluginId, out var entry))
        {
            // Dispose plugin instance
            if (entry.Plugin.Instance is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync();
            }

            // Unload AppDomain
            if (entry.AppDomain != null)
            {
                _sandbox.UnloadAppDomain(entry.AppDomain);
            }

            _loadedPlugins.Remove(pluginId);
            _logger.LogInformation("Plugin unloaded successfully: {PluginId}", pluginId);
        }
    }
}

// PluginProxy for cross-AppDomain communication
public class PluginProxy : MarshalByRefObject
{
    private IPlugin _plugin;

    public async Task LoadPluginAsync(string assemblyPath, CancellationToken cancellationToken)
    {
        var assembly = Assembly.LoadFrom(assemblyPath);
        var pluginType = assembly.GetTypes()
            .FirstOrDefault(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsInterface);

        _plugin = (IPlugin)Activator.CreateInstance(pluginType);
        await _plugin.InitializeAsync(cancellationToken);
    }

    public async Task<string> ExecuteAsync(string input, CancellationToken cancellationToken)
    {
        return await _plugin.ExecuteAsync(input, cancellationToken);
    }
}
```

---

#### Phase 6: API Layer - MCP Protocol Integration (Week 3, Days 12, T3.6 - 1 SP) ✅

**MCP Protocol Interface**:

```csharp
// IMcpProtocolHandler - Application Layer
public interface IMcpProtocolHandler
{
    Task<McpResponse> HandleRequestAsync(McpRequest request, CancellationToken cancellationToken = default);
    Task<List<McpTool>> GetAvailableToolsAsync(CancellationToken cancellationToken = default);
}

// MCP Protocol DTOs
public record McpRequest(
    string Method,
    Dictionary<string, object> Parameters
);

public record McpResponse(
    bool Success,
    object Result,
    string ErrorMessage = null
);

public record McpTool(
    string Name,
    string Description,
    Dictionary<string, string> Parameters
);
```

**MCP Protocol Handler Implementation**:

```csharp
// McpProtocolHandler - Infrastructure Layer
public class McpProtocolHandler : IMcpProtocolHandler
{
    private readonly IPluginLoader _pluginLoader;
    private readonly IPluginRepository _pluginRepository;
    private readonly ILogger<McpProtocolHandler> _logger;

    public McpProtocolHandler(
        IPluginLoader pluginLoader,
        IPluginRepository pluginRepository,
        ILogger<McpProtocolHandler> logger)
    {
        _pluginLoader = pluginLoader;
        _pluginRepository = pluginRepository;
        _logger = logger;
    }

    public async Task<McpResponse> HandleRequestAsync(
        McpRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return request.Method switch
            {
                "plugin.list" => await HandleListPluginsAsync(request, cancellationToken),
                "plugin.execute" => await HandleExecutePluginAsync(request, cancellationToken),
                "plugin.load" => await HandleLoadPluginAsync(request, cancellationToken),
                "plugin.unload" => await HandleUnloadPluginAsync(request, cancellationToken),
                _ => new McpResponse(false, null, $"Unknown method: {request.Method}")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling MCP request: {Method}", request.Method);
            return new McpResponse(false, null, ex.Message);
        }
    }

    private async Task<McpResponse> HandleListPluginsAsync(
        McpRequest request,
        CancellationToken cancellationToken)
    {
        var loadedPlugins = _pluginLoader.GetLoadedPlugins();
        var tools = loadedPlugins.Select(p => new McpTool(
            p.Name,
            $"Plugin: {p.Name} ({p.Type})",
            new Dictionary<string, string> { { "input", "string" } }
        )).ToList();

        return new McpResponse(true, tools);
    }

    private async Task<McpResponse> HandleExecutePluginAsync(
        McpRequest request,
        CancellationToken cancellationToken)
    {
        var pluginId = Guid.Parse(request.Parameters["pluginId"].ToString());
        var input = request.Parameters["input"].ToString();

        var loadedPlugin = _pluginLoader.GetLoadedPlugin(pluginId);
        if (loadedPlugin == null)
        {
            return new McpResponse(false, null, $"Plugin {pluginId} is not loaded");
        }

        string result;
        if (loadedPlugin.Type == PluginType.Native)
        {
            var plugin = (IPlugin)loadedPlugin.Instance;
            result = await plugin.ExecuteAsync(input, cancellationToken);
        }
        else
        {
            var function = (KernelFunction)loadedPlugin.Instance;
            var kernelResult = await function.InvokeAsync(input, cancellationToken);
            result = kernelResult.ToString();
        }

        return new McpResponse(true, result);
    }

    private async Task<McpResponse> HandleLoadPluginAsync(
        McpRequest request,
        CancellationToken cancellationToken)
    {
        var pluginId = Guid.Parse(request.Parameters["pluginId"].ToString());
        var plugin = await _pluginRepository.GetByIdAsync(pluginId, cancellationToken);

        if (plugin == null)
        {
            return new McpResponse(false, null, $"Plugin {pluginId} not found");
        }

        var loadedPlugin = await _pluginLoader.LoadPluginAsync(plugin, cancellationToken);
        return new McpResponse(true, new { loadedPlugin.PluginId, loadedPlugin.Name, loadedPlugin.LoadedAt });
    }

    private async Task<McpResponse> HandleUnloadPluginAsync(
        McpRequest request,
        CancellationToken cancellationToken)
    {
        var pluginId = Guid.Parse(request.Parameters["pluginId"].ToString());
        await _pluginLoader.UnloadPluginAsync(pluginId, cancellationToken);
        return new McpResponse(true, $"Plugin {pluginId} unloaded successfully");
    }

    public async Task<List<McpTool>> GetAvailableToolsAsync(CancellationToken cancellationToken)
    {
        var loadedPlugins = _pluginLoader.GetLoadedPlugins();
        return loadedPlugins.Select(p => new McpTool(
            p.Name,
            $"Plugin: {p.Name} ({p.Type})",
            new Dictionary<string, string> { { "input", "string" } }
        )).ToList();
    }
}
```

---

#### Phase 7: API Layer - Plugin REST API (Week 3, Days 13-14, T3.7 - 1 SP) ✅

**PluginsController - 6 個端點**:

```csharp
// PluginsController - API Layer
[ApiController]
[Route("api/v1/plugins")]
[Produces("application/json")]
public class PluginsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PluginsController> _logger;

    public PluginsController(IMediator mediator, ILogger<PluginsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Create a new plugin
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PluginDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePlugin(
        [FromBody] CreatePluginCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new ErrorResponse { Message = result.Error });
        }

        return CreatedAtAction(
            nameof(GetPluginById),
            new { id = result.Value.PluginId },
            result.Value
        );
    }

    /// <summary>
    /// Get paginated list of plugins
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<PluginDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlugins(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] PluginType? type = null,
        [FromQuery] PluginStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetPluginsQuery(page, pageSize, type, status);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new ErrorResponse { Message = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get plugin by ID
    /// </summary>
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
        {
            return NotFound(new ErrorResponse { Message = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Update plugin
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(PluginDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePlugin(
        [FromRoute] Guid id,
        [FromBody] UpdatePluginCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.PluginId)
        {
            return BadRequest(new ErrorResponse { Message = "Plugin ID mismatch" });
        }

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.Contains("not found")
                ? NotFound(new ErrorResponse { Message = result.Error })
                : BadRequest(new ErrorResponse { Message = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Activate plugin
    /// </summary>
    [HttpPut("{id}/activate")]
    [ProducesResponseType(typeof(PluginDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ActivatePlugin(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new ActivatePluginCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new ErrorResponse { Message = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Deactivate plugin
    /// </summary>
    [HttpPut("{id}/deactivate")]
    [ProducesResponseType(typeof(PluginDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeactivatePlugin(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeactivatePluginCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new ErrorResponse { Message = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Delete plugin (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeletePlugin(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeletePluginCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new ErrorResponse { Message = result.Error });
        }

        return NoContent();
    }
}

// DTOs
public record PluginDto(
    Guid PluginId,
    string Name,
    string Description,
    PluginType Type,
    PluginStatus Status,
    string FilePath,
    PluginMetadataDto Metadata,
    DateTime CreatedAt,
    DateTime? UpdatedAt
)
{
    public static PluginDto FromDomain(Plugin plugin)
    {
        return new PluginDto(
            plugin.PluginId,
            plugin.Name,
            plugin.Description,
            plugin.Type,
            plugin.Status,
            plugin.FilePath,
            PluginMetadataDto.FromDomain(plugin.Metadata),
            plugin.CreatedAt,
            plugin.UpdatedAt
        );
    }
}

public record PluginMetadataDto(
    string Version,
    string Author,
    List<string> Dependencies,
    List<Permission> Permissions,
    Dictionary<string, string> Tags
)
{
    public static PluginMetadataDto FromDomain(PluginMetadata metadata)
    {
        return new PluginMetadataDto(
            metadata.Version.ToString(),
            metadata.Author,
            metadata.Dependencies,
            metadata.Permissions,
            metadata.Tags
        );
    }

    public PluginMetadata ToDomain()
    {
        return PluginMetadata.Create(Version, Author, Dependencies, Permissions, Tags);
    }
}

public record ErrorResponse
{
    public string Message { get; init; }
}
```

---

## 🔌 API 規格速查表

### Plugin System API - 6 個端點

#### 1. Create Plugin
```http
POST /api/v1/plugins
Content-Type: application/json

Request Body:
{
  "name": "MyNativePlugin",
  "description": "A custom native plugin",
  "type": 1,                           // 1 = Native, 2 = Semantic
  "filePath": "/plugins/MyNativePlugin.dll",
  "metadata": {
    "version": "1.0.0",
    "author": "John Doe",
    "dependencies": ["Newtonsoft.Json"],
    "permissions": [1, 2],             // 1 = FileAccess, 2 = NetworkAccess
    "tags": { "category": "utility" }
  }
}

Response: 201 Created
{
  "pluginId": "uuid",
  "name": "MyNativePlugin",
  "description": "A custom native plugin",
  "type": 1,
  "status": 0,                         // 0 = Draft
  "filePath": "/plugins/MyNativePlugin.dll",
  "metadata": {
    "version": "1.0.0",
    "author": "John Doe",
    "dependencies": ["Newtonsoft.Json"],
    "permissions": [1, 2],
    "tags": { "category": "utility" }
  },
  "createdAt": "2025-12-15T10:00:00Z",
  "updatedAt": null
}
```

#### 2. Get Plugins (Paginated with Filters)
```http
GET /api/v1/plugins
  ?page=1                              // 頁碼 (default: 1)
  &pageSize=20                         // 每頁數量 (default: 20, max: 100)
  &type=1                              // 選填: 1 = Native, 2 = Semantic
  &status=1                            // 選填: 0 = Draft, 1 = Active, 2 = Inactive, 3 = Archived

Response: 200 OK
{
  "items": [
    {
      "pluginId": "uuid1",
      "name": "MyNativePlugin",
      "type": 1,
      "status": 1,
      "createdAt": "2025-12-15T10:00:00Z"
    },
    {
      "pluginId": "uuid2",
      "name": "SemanticPlugin",
      "type": 2,
      "status": 1,
      "createdAt": "2025-12-14T09:00:00Z"
    }
  ],
  "totalCount": 50,
  "page": 1,
  "pageSize": 20,
  "totalPages": 3
}
```

#### 3. Get Plugin by ID
```http
GET /api/v1/plugins/{id}

Response: 200 OK
{
  "pluginId": "uuid",
  "name": "MyNativePlugin",
  "description": "A custom native plugin",
  "type": 1,
  "status": 1,
  "filePath": "/plugins/MyNativePlugin.dll",
  "metadata": {
    "version": "1.0.0",
    "author": "John Doe",
    "dependencies": ["Newtonsoft.Json"],
    "permissions": [1, 2],
    "tags": { "category": "utility" }
  },
  "createdAt": "2025-12-15T10:00:00Z",
  "updatedAt": "2025-12-15T11:00:00Z"
}
```

#### 4. Update Plugin
```http
PUT /api/v1/plugins/{id}
Content-Type: application/json

Request Body:
{
  "pluginId": "uuid",
  "name": "UpdatedPluginName",
  "description": "Updated description",
  "metadata": {
    "version": "1.1.0",
    "author": "John Doe",
    "dependencies": ["Newtonsoft.Json", "System.Text.Json"],
    "permissions": [1, 2, 3],
    "tags": { "category": "utility", "priority": "high" }
  }
}

Response: 200 OK
{
  "pluginId": "uuid",
  "name": "UpdatedPluginName",
  "description": "Updated description",
  "status": 1,
  "metadata": { ... },
  "updatedAt": "2025-12-15T12:00:00Z"
}
```

#### 5. Activate Plugin
```http
PUT /api/v1/plugins/{id}/activate

Response: 200 OK
{
  "pluginId": "uuid",
  "name": "MyNativePlugin",
  "status": 1,                         // 1 = Active
  "updatedAt": "2025-12-15T13:00:00Z"
}
```

#### 6. Deactivate Plugin
```http
PUT /api/v1/plugins/{id}/deactivate

Response: 200 OK
{
  "pluginId": "uuid",
  "name": "MyNativePlugin",
  "status": 2,                         // 2 = Inactive
  "updatedAt": "2025-12-15T14:00:00Z"
}
```

#### 7. Delete Plugin (Soft Delete)
```http
DELETE /api/v1/plugins/{id}

Response: 204 No Content
```

---

## 🏗️ 技術架構快速參考

### Backend 技術棧 (Sprint 3)

```yaml
框架: ASP.NET Core 9
語言: C# 12
ORM: Entity Framework Core 9
資料庫: PostgreSQL 16
隔離技術: AppDomain / AssemblyLoadContext
Plugin 協議: Model Context Protocol (MCP)

架構模式:
  - Clean Architecture (4-layer)
  - CQRS (MediatR)
  - Repository Pattern
  - Domain-Driven Design (DDD)
  - Factory Pattern (Entity creation)
  - Value Object Pattern (PluginMetadata, SemanticVersion)

安全性:
  - AppDomain 隔離 (Native Plugins with permissions)
  - AssemblyLoadContext (Native Plugins without special permissions)
  - Permission-based access control
  - Soft delete for data safety

驗證:
  - FluentValidation 11+
  - Data Annotations
  - Domain-level validation

測試:
  - xUnit
  - Moq
  - FluentAssertions
  - 目標覆蓋率: ≥80%
```

### 資料庫 Schema 設計

#### plugins 表

```sql
CREATE TABLE plugins (
    plugin_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(200) NOT NULL,
    description VARCHAR(1000),
    type INTEGER NOT NULL,              -- 1 = Native, 2 = Semantic
    status INTEGER NOT NULL,            -- 0 = Draft, 1 = Active, 2 = Inactive, 3 = Archived
    file_path VARCHAR(500) NOT NULL,
    metadata JSONB NOT NULL,            -- PluginMetadata as JSONB
    is_deleted BOOLEAN NOT NULL DEFAULT false,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE,
    created_by VARCHAR(100),
    updated_by VARCHAR(100),

    CONSTRAINT uq_plugins_name UNIQUE(name)
);

-- 索引
CREATE INDEX idx_plugins_name ON plugins(name);
CREATE INDEX idx_plugins_type ON plugins(type);
CREATE INDEX idx_plugins_status ON plugins(status);
CREATE INDEX idx_plugins_created_at ON plugins(created_at DESC);
CREATE INDEX idx_plugins_metadata ON plugins USING GIN (metadata jsonb_path_ops);
```

**PluginMetadata JSONB 結構**:

```json
{
  "version": {
    "major": 1,
    "minor": 0,
    "patch": 0
  },
  "author": "John Doe",
  "dependencies": ["Newtonsoft.Json", "System.Text.Json"],
  "permissions": [1, 2],  // FileAccess, NetworkAccess
  "tags": {
    "category": "utility",
    "priority": "high"
  }
}
```

---

## ⚙️ 編碼標準快速參考

### C# 命名規範

```csharp
// PascalCase: Classes, Methods, Properties, Events
public class PluginLoader { }
public async Task<LoadedPlugin> LoadPluginAsync() { }
public string PluginName { get; set; }
public event EventHandler PluginLoaded;

// camelCase: Local variables, Parameters, Private fields
private readonly IPluginRepository _pluginRepository;
public async Task ProcessAsync(Guid pluginId, int maxRetries) { }

// Interface: I prefix
public interface IPluginRepository { }
public interface IPluginLoader { }

// Async: Async suffix for async methods
public async Task<Plugin> GetPluginAsync(Guid id);
public async Task<List<Plugin>> GetAllPluginsAsync();

// Factory Method: Static Create method for Domain Entities
public static Plugin Create(string name, PluginType type, string filePath);

// Value Object: Override GetEqualityComponents
protected override IEnumerable<object> GetEqualityComponents();
```

### Domain-Driven Design 模式

```csharp
// Entity: Identity + Business Logic
public class Plugin : BaseEntity
{
    public Guid PluginId { get; private set; }  // Identity
    private string _name;                        // Encapsulated state

    // Factory Method for creation
    public static Plugin Create(string name) { }

    // Business logic methods
    public void Activate() { }
    public void Deactivate() { }
}

// Value Object: Immutability + Equality by value
public class PluginMetadata : ValueObject
{
    public SemanticVersion Version { get; private set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Version;
        yield return Author;
    }
}

// Domain Exception
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}

// Repository Interface: In Domain Layer, Implementation in Infrastructure
public interface IPluginRepository
{
    Task<Plugin> GetByIdAsync(Guid id);
}
```

### CQRS 模式

```csharp
// Command: Write operation
public record CreatePluginCommand(string Name, PluginType Type)
    : IRequest<Result<PluginDto>>;

// Command Handler: Business logic + Persistence
public class CreatePluginCommandHandler
    : IRequestHandler<CreatePluginCommand, Result<PluginDto>>
{
    public async Task<Result<PluginDto>> Handle(
        CreatePluginCommand request,
        CancellationToken cancellationToken)
    {
        // Business logic
        var plugin = Plugin.Create(request.Name, request.Type);

        // Persistence
        await _pluginRepository.AddAsync(plugin);
        await _unitOfWork.CommitAsync();

        return Result<PluginDto>.Success(PluginDto.FromDomain(plugin));
    }
}

// Query: Read operation
public record GetPluginsQuery(int Page, int PageSize)
    : IRequest<Result<PagedResult<PluginDto>>>;

// Query Handler: Read-only operation
public class GetPluginsQueryHandler
    : IRequestHandler<GetPluginsQuery, Result<PagedResult<PluginDto>>>
{
    public async Task<Result<PagedResult<PluginDto>>> Handle(
        GetPluginsQuery request,
        CancellationToken cancellationToken)
    {
        var pagedPlugins = await _pluginRepository.GetPagedAsync(
            request.Page,
            request.PageSize
        );
        return Result<PagedResult<PluginDto>>.Success(pagedPlugins);
    }
}
```

### Git Commit Message 格式

```bash
# 格式: <type>(<scope>): <subject>

# Types:
feat     # 新功能
fix      # Bug 修復
refactor # 代碼重構
docs     # 文檔更新
test     # 測試相關
chore    # 構建/配置相關
style    # 代碼格式調整

# Examples:
feat(plugin): implement Plugin Entity and Repository
feat(plugin): add PluginLoader with AppDomain isolation
feat(plugin): implement Plugin CRUD API endpoints
refactor(plugin): extract PluginSandbox to separate class
test(plugin): add unit tests for Plugin domain logic
docs(plugin): update Plugin API documentation
```

---

## 🎯 開發優先順序（Sprint 3 任務清單）

### Week 1: Domain + Infrastructure (5 SP)

#### T3.1: Plugin Entity Design (2 SP) - Days 1-2
- [x] 設計 Plugin Entity (Domain Layer)
- [x] 實作 PluginMetadata Value Object
- [x] 實作 SemanticVersion Value Object
- [x] 定義 PluginType, PluginStatus, Permission Enums
- [x] 單元測試 (Plugin 業務邏輯)

#### T3.2: Plugin Repository (2 SP) - Days 3-5
- [x] 定義 IPluginRepository Interface (Domain Layer)
- [x] 實作 PluginRepository (Infrastructure Layer)
- [x] 配置 EF Core Configuration (PluginConfiguration)
- [x] 建立 EF Migration (AddPluginEntity)
- [x] 單元測試 (Repository 操作)

### Week 2: Application + Security (6 SP)

#### T3.3: CQRS Commands/Queries (3 SP) - Days 6-8
- [x] CreatePluginCommand + Handler + Validator
- [x] GetPluginsQuery + Handler + Validator
- [x] GetPluginByIdQuery + Handler
- [x] UpdatePluginCommand + Handler + Validator
- [x] ActivatePluginCommand + Handler
- [x] DeactivatePluginCommand + Handler
- [x] DeletePluginCommand + Handler
- [x] 單元測試 (所有 Commands/Queries)

#### T3.4: PluginLoader Implementation (2 SP) - Days 9-10
- [x] 定義 IPluginLoader Interface
- [x] 實作 PluginLoader (AssemblyLoadContext)
- [x] LoadNativePluginAsync 實作
- [x] LoadSemanticPluginAsync 實作
- [x] UnloadPluginAsync 實作
- [x] 單元測試 (Plugin 載入/卸載)

#### T3.5: AppDomain Isolation (2 SP) - Day 11
- [x] 設計 PluginSandbox
- [x] CreateSecureAppDomain 實作
- [x] Permission-based 隔離機制
- [x] 整合 PluginLoader with AppDomain
- [x] PluginProxy for cross-AppDomain communication
- [x] 單元測試 (安全隔離驗證)

### Week 3: API + Integration (2 SP)

#### T3.6: MCP Protocol Integration (1 SP) - Day 12
- [x] 定義 IMcpProtocolHandler Interface
- [x] 實作 McpProtocolHandler
- [x] 實作 MCP Methods (list, execute, load, unload)
- [x] 整合測試 (MCP 協議驗證)

#### T3.7: Plugin API Controller (1 SP) - Days 13-14
- [x] 實作 PluginsController (6 個端點)
- [x] Swagger 文檔配置
- [x] 集成測試 (API 端點驗證)
- [x] Postman 測試集建立
- [x] E2E 測試 (完整工作流)

---

## 📚 相關文檔連結

### Sprint 執行文檔
- [Sprint 3 概覽](./SPRINT-3-1-OVERVIEW.md) - Sprint 目標、User Stories 狀態
- [Sprint 3 執行計劃](./SPRINT-3-2-PLAN.md) - 詳細技術實施指南
- [Sprint 3 檢查清單](./SPRINT-3-4-CHECKLIST.md) - 任務追蹤清單
- [Sprint 3 開發日誌](./SPRINT-3-5-DEV-LOG.md) - 每日開發記錄
- [Sprint 3 問題追蹤](./SPRINT-3-6-ISSUES.md) - 問題與解決方案
- [Sprint 3 回顧](./SPRINT-3-7-RETROSPECTIVE.md) - Sprint 完成後總結

### 項目規劃文檔 (/claudedocs/1-planning)
- [Sprint 分配分析](../../1-planning/SPRINT-ALLOCATION-ANALYSIS.md) - Sprint 3 詳細分配、Story Points、依賴關係
- [MVP 範圍定義](../../1-planning/MVP-SCOPE-DEFINITION.md) - Sprint 3 在 MVP 中的範圍與邊界
- [開發策略](../../1-planning/DEVELOPMENT-STRATEGY.md) - Clean Architecture 實施策略、Git 工作流、測試策略
- [依賴矩陣](../../1-planning/DEPENDENCY-MATRIX.md) - US 1.3 依賴關係追蹤
- [風險登記簿](../../1-planning/RISK-REGISTER.md) - Sprint 3 技術風險評估與緩解策略 (RISK-003, RISK-019, RISK-020)
- [技術決策日誌](../../1-planning/TECHNICAL-DECISIONS-LOG.md) - Plugin 隔離技術選擇記錄

### 參考層文檔 (/docs)

#### 架構設計決策 (ADR)
- [ADR-002: CQRS Pattern](../../../docs/architecture/adr/ADR-002-cqrs-pattern.md) - MediatR Commands/Queries 設計模式
- [ADR-006: Agent State Management](../../../docs/architecture/adr/ADR-006-agent-state-management.md) - 狀態管理策略
- [ADR-008: Code Interpreter Execution Model](../../../docs/architecture/adr/ADR-008-code-interpreter-execution-model.md) - 執行引擎安全設計、資源隔離
- [ADR-011: Framework Migration Strategy](../../../docs/architecture/adr/ADR-011-framework-migration-strategy.md) - Plugin 抽象層設計
- [Architecture Design Document](../../../docs/architecture/Architecture-Design-Document.md) - Clean Architecture 4-layer 設計、系統架構概覽
- [Database Schema](../../../docs/architecture/database-schema.md) - plugins 表完整設計、JSONB 索引策略

#### User Stories 完整規格
- [Module 02: Plugin System](../../../docs/user-stories/modules/module-02-plugin-system.md) - US 1.3 完整規格 (Line 1-150)、驗收標準、技術實施要求

#### Backend 技術實施參考
- [CQRS Implementation](../../../docs/technical-implementation/01-backend-net9/05-cqrs-implementation.md) - MediatR 配置、Command/Query Handler 實作
- [Plugin System Architecture](../../../docs/technical-implementation/01-backend-net9/11-plugin-system-architecture.md) - AssemblyLoadContext 動態加載、Plugin Isolation 實作
- [Repository Pattern](../../../docs/technical-implementation/01-backend-net9/06-repository-pattern.md) - Generic Repository、Specification Pattern、EF Core 最佳實踐
- [Value Objects](../../../docs/technical-implementation/01-backend-net9/07-value-objects.md) - Value Object 設計模式、Equality 實作

#### API 設計規範
- [RESTful API Standards](../../../docs/technical-implementation/05-api-design/restful-api-standards.md) - REST 設計原則、HTTP Methods 使用規範
- [API Documentation](../../../docs/technical-implementation/05-api-design/api-documentation.md) - Swagger/OpenAPI 配置
- [Error Handling](../../../docs/technical-implementation/05-api-design/error-handling.md) - Result Pattern 實作、Exception Handling 策略

#### 資料庫設計規範
- [Database Design Principles](../../../docs/technical-implementation/06-database-standards/database-design-principles.md) - 資料庫設計最佳實踐、索引策略
- [Entity Framework Core Configuration](../../../docs/technical-implementation/06-database-standards/entity-framework-core-configuration.md) - Fluent API、Value Converters (JSONB, SemVer)
- [Database Migration Strategy](../../../docs/technical-implementation/06-database-standards/database-migration-strategy.md) - EF Core Migrations 工作流、Rollback 策略

#### 測試規範
- [Testing Strategy](../../../docs/technical-implementation/07-testing-strategy/README.md) - 測試金字塔、80%+ 覆蓋率目標
- [Unit Testing Standards](../../../docs/technical-implementation/07-testing-strategy/unit-testing-standards.md) - xUnit、Moq、AAA 模式
- [Integration Testing Standards](../../../docs/technical-implementation/07-testing-strategy/integration-testing-standards.md) - WebApplicationFactory、TestContainers 整合

---

## 📚 完整參考文獻索引

本上下文文檔整合了以下技術細節與架構決策，按類別組織以便 AI Assistant 快速定位：

### 1. Planning 文檔（濃縮版，優先查閱）

- [Sprint Allocation Analysis](../../1-planning/SPRINT-ALLOCATION-ANALYSIS.md) - Sprint 3 詳細分配 (US 1.3, 13 SP, Week 7-9, Phase 1A)
- [MVP Scope Definition](../../1-planning/MVP-SCOPE-DEFINITION.md) - Plugin System 在 Phase 1A 的範圍與邊界
- [Development Strategy](../../1-planning/DEVELOPMENT-STRATEGY.md) - Clean Architecture 實施策略、Git 工作流、CI/CD 流程、測試策略
- [Dependency Matrix](../../1-planning/DEPENDENCY-MATRIX.md) - US 1.3 依賴 Sprint 2 Agent Engine (DEP-012)
- [Risk Register](../../1-planning/RISK-REGISTER.md) - RISK-003 (AppDomain 隔離, High), RISK-019 (MCP 協議, Medium), RISK-020 (安全漏洞, Medium)
- [Technical Decisions Log](../../1-planning/TECHNICAL-DECISIONS-LOG.md) - Plugin 隔離技術選擇 (AppDomain vs AssemblyLoadContext)
- [Architecture Evolution Roadmap](../../1-planning/ARCHITECTURE-EVOLUTION-ROADMAP.md) - 架構演進階段規劃

### 2. 架構設計決策 (ADR)

- [ADR-002: CQRS Pattern](../../../docs/architecture/adr/ADR-002-cqrs-pattern.md) - MediatR Commands/Queries 設計模式參考
- [ADR-006: Agent State Management](../../../docs/architecture/adr/ADR-006-agent-state-management.md) - 狀態管理策略
- [ADR-008: Code Interpreter Execution Model](../../../docs/architecture/adr/ADR-008-code-interpreter-execution-model.md) - 執行引擎安全設計、資源隔離
- [ADR-011: Framework Migration Strategy](../../../docs/architecture/adr/ADR-011-framework-migration-strategy.md) - Plugin 抽象層設計、IPluginExecutor 介面定義
- [Architecture Design Document](../../../docs/architecture/Architecture-Design-Document.md) - Clean Architecture 4-layer 設計、Plugin 系統架構
- [Database Schema](../../../docs/architecture/database-schema.md) - plugins 表完整設計、JSONB 索引策略
- [C4 Architecture Diagrams](../../../docs/architecture/C4-architecture-diagrams.md) - Plugin System 組件圖、隔離架構

### 3. User Stories 完整規格

- [Module 02: Plugin System](../../../docs/user-stories/modules/module-02-plugin-system.md)
  - US 1.3 Plugin System 基礎架構規格 (Line 1-150)
  - 驗收標準詳細列表
  - 技術實施要求
  - Native vs Semantic Plugin 區別

### 4. Backend 技術實施參考 (.NET 9)

- [CQRS Implementation](../../../docs/technical-implementation/01-backend-net9/05-cqrs-implementation.md)
  - MediatR 配置與註冊
  - Command/Query Handler 實作範例
  - FluentValidation 整合模式
  - Result Pattern 應用

- [Plugin System Architecture](../../../docs/technical-implementation/01-backend-net9/11-plugin-system-architecture.md)
  - AssemblyLoadContext 動態加載詳解
  - Plugin Isolation 實作 (isCollectible: true)
  - AppDomain vs AssemblyLoadContext 選擇
  - Unload 機制實作

- [Repository Pattern](../../../docs/technical-implementation/01-backend-net9/06-repository-pattern.md)
  - Generic Repository 實作
  - Specification Pattern 應用
  - EF Core 最佳實踐
  - Unit of Work 模式

- [Value Objects](../../../docs/technical-implementation/01-backend-net9/07-value-objects.md)
  - Value Object 設計模式
  - SemanticVersion (SemVer) 實作
  - PluginMetadata 實作
  - Value Converters 設計 (JSONB)
  - Equality 比較實作

### 5. API 設計規範

- [RESTful API Standards](../../../docs/technical-implementation/05-api-design/restful-api-standards.md)
  - REST 設計原則
  - HTTP Methods 使用規範 (GET, POST, PUT, DELETE)
  - Status Codes 標準 (200, 201, 204, 400, 404)
  - API 版本控制 (/api/v1)

- [API Documentation](../../../docs/technical-implementation/05-api-design/api-documentation.md)
  - Swagger/OpenAPI 配置
  - API 端點文檔標準
  - Request/Response 範例

- [Error Handling](../../../docs/technical-implementation/05-api-design/error-handling.md)
  - Result Pattern 實作
  - Exception Handling 策略
  - ErrorResponse DTO 設計

### 6. 資料庫設計規範

- [Database Design Principles](../../../docs/technical-implementation/06-database-standards/database-design-principles.md)
  - 資料庫設計最佳實踐
  - 索引設計策略 (B-tree, GIN for JSONB)
  - 正規化與反正規化

- [Entity Framework Core Configuration](../../../docs/technical-implementation/06-database-standards/entity-framework-core-configuration.md)
  - Fluent API 配置
  - Value Converters 實作 (JSONB, SemVer)
  - Complex Type 映射 (OwnsOne)
  - Index 定義

- [Database Migration Strategy](../../../docs/technical-implementation/06-database-standards/database-migration-strategy.md)
  - EF Core Migrations 工作流
  - Migration 命名規範
  - Rollback 策略
  - Production 部署流程

### 7. 測試規範

- [Testing Strategy](../../../docs/technical-implementation/07-testing-strategy/README.md)
  - 測試金字塔 (70% Unit, 20% Integration, 10% E2E)
  - 80%+ 覆蓋率目標
  - Test-Driven Development (TDD) 方法

- [Unit Testing Standards](../../../docs/technical-implementation/07-testing-strategy/unit-testing-standards.md)
  - xUnit 測試框架使用
  - Moq 模擬框架
  - AAA 模式 (Arrange-Act-Assert)
  - FluentAssertions 斷言庫

- [Integration Testing Standards](../../../docs/technical-implementation/07-testing-strategy/integration-testing-standards.md)
  - WebApplicationFactory 使用
  - TestContainers 整合 (PostgreSQL, Redis)
  - 集成測試最佳實踐

### 8. 開發標準文檔

- [C# Coding Standards](../../../docs/technical-implementation/04-coding-standards/csharp-coding-standards.md)
  - 命名規範 (PascalCase, camelCase)
  - 代碼風格指南
  - 設計模式應用
  - SOLID 原則

- [Git Workflow](../../1-planning/DEVELOPMENT-STRATEGY.md)
  - Commit Message 格式
  - Branch 策略 (feature/, bugfix/, hotfix/)
  - Pull Request 流程

### 9. 前置 Sprint 文檔

- [Sprint 1 Overview](../sprint-1/SPRINT-1-1-OVERVIEW.md) - Agent CRUD 基礎
- [Sprint 2 Overview](../sprint-2/SPRINT-2-1-OVERVIEW.md) - Agent 執行引擎
- [Sprint 2 Context](../sprint-2/SPRINT-2-3-CONTEXT.md) - Agent Execution & Plugin Versions 上下文

### 10. Change Management

- [Change Log](../../4-changes/CHANGE-LOG.md) - 專案變更記錄
- [User Story Status](../../3-progress/USER-STORY-STATUS.md) - 所有 User Stories 狀態追蹤

---

**文檔版本**: v2.0
**創建日期**: 2025-12-15
**最後更新**: 2025-12-15
**維護者**: AI Development Assistant
**狀態**: 📋 Sprint 3 準備開始
**升級內容**: 新增完整參考文獻索引（50+ 文檔），優先引用 /claudedocs/1-planning，提供 Plugin System 完整技術實施細節與代碼範例
