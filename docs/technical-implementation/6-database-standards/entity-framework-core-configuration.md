# Entity Framework Core 配置 (Entity Framework Core Configuration)

## 概述

本文檔詳細說明 Entity Framework Core 9.0 在 AI Workflow Platform 中的完整配置標準,包括 DbContext 設計、實體配置、關聯映射、值對象、繼承策略和性能優化。

## 目標

- ✅ 建立標準化的 EF Core 配置模式
- ✅ 實現 Clean Architecture 的數據訪問層
- ✅ 優化查詢性能和追蹤行為
- ✅ 確保類型安全的數據庫操作
- ✅ 支持複雜的領域模型映射

## DbContext 配置

### 基礎 DbContext 設計

**IApplicationDbContext 接口**:

```csharp
// AIWorkflow.Application/Common/Interfaces/IApplicationDbContext.cs
public interface IApplicationDbContext
{
    DbSet<Workflow> Workflows { get; }
    DbSet<WorkflowNode> WorkflowNodes { get; }
    DbSet<WorkflowEdge> WorkflowEdges { get; }
    DbSet<WorkflowTemplate> WorkflowTemplates { get; }
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<WorkflowExecution> WorkflowExecutions { get; }
    DbSet<WorkflowExecutionLog> WorkflowExecutionLogs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

**ApplicationDbContext 實現**:

```csharp
// AIWorkflow.Infrastructure/Persistence/ApplicationDbContext.cs
public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    ICurrentUserService currentUserService,
    IDateTimeService dateTimeService,
    IDomainEventDispatcher domainEventDispatcher)
    : DbContext(options), IApplicationDbContext
{
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly IDateTimeService _dateTimeService = dateTimeService;
    private readonly IDomainEventDispatcher _domainEventDispatcher = domainEventDispatcher;

    public DbSet<Workflow> Workflows => Set<Workflow>();
    public DbSet<WorkflowNode> WorkflowNodes => Set<WorkflowNode>();
    public DbSet<WorkflowEdge> WorkflowEdges => Set<WorkflowEdge>();
    public DbSet<WorkflowTemplate> WorkflowTemplates => Set<WorkflowTemplate>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<WorkflowExecution> WorkflowExecutions => Set<WorkflowExecution>();
    public DbSet<WorkflowExecutionLog> WorkflowExecutionLogs => Set<WorkflowExecutionLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 應用所有 IEntityTypeConfiguration
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // 配置全局查詢過濾器
        ConfigureGlobalQueryFilters(modelBuilder);

        // 配置數值精度
        ConfigurePrecision(modelBuilder);
    }

    private static void ConfigureGlobalQueryFilters(ModelBuilder modelBuilder)
    {
        // 軟刪除過濾器
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
                var filterExpression = Expression.Lambda(Expression.Not(property), parameter);

                entityType.SetQueryFilter(filterExpression);
            }
        }
    }

    private static void ConfigurePrecision(ModelBuilder modelBuilder)
    {
        // 全局配置 decimal 精度
        foreach (var property in modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
        {
            property.SetPrecision(18);
            property.SetScale(2);
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // 處理審計字段
        ProcessAuditableEntities();

        // 保存變更
        var result = await base.SaveChangesAsync(cancellationToken);

        // 分發領域事件
        await DispatchDomainEventsAsync(cancellationToken);

        return result;
    }

    private void ProcessAuditableEntities()
    {
        var now = _dateTimeService.UtcNow;
        var userId = _currentUserService.UserId;

        foreach (var entry in ChangeTracker.Entries<IAuditable>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.CreatedBy = userId;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    entry.Entity.UpdatedBy = userId;
                    break;

                case EntityState.Deleted when entry.Entity is ISoftDeletable softDeletable:
                    entry.State = EntityState.Modified;
                    softDeletable.IsDeleted = true;
                    softDeletable.DeletedAt = now;
                    softDeletable.DeletedBy = userId;
                    break;
            }
        }
    }

    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        var domainEvents = ChangeTracker.Entries<AggregateRoot>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Count != 0)
            .SelectMany(e =>
            {
                var events = e.DomainEvents.ToList();
                e.ClearDomainEvents();
                return events;
            })
            .ToList();

        foreach (var domainEvent in domainEvents)
        {
            await _domainEventDispatcher.DispatchAsync(domainEvent, cancellationToken);
        }
    }
}
```

### DbContext 註冊

```csharp
// AIWorkflow.API/Program.cs 或 DependencyInjection.cs
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
                npgsqlOptions.CommandTimeout(30);
                npgsqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });

            // 開發環境設置
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
                options.LogTo(Console.WriteLine, LogLevel.Information);
            }

            // 性能優化
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        return services;
    }
}
```

## Entity Configuration 模式

### IEntityTypeConfiguration 實現

**基礎實體配置**:

```csharp
// AIWorkflow.Infrastructure/Persistence/Configurations/WorkflowConfiguration.cs
public sealed class WorkflowConfiguration : IEntityTypeConfiguration<Workflow>
{
    public void Configure(EntityTypeBuilder<Workflow> builder)
    {
        // 表映射
        builder.ToTable("workflows", "workflow");

        // 主鍵
        builder.HasKey(w => w.Id);

        // 屬性配置
        builder.Property(w => w.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(w => w.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(w => w.Description)
            .HasColumnName("description")
            .HasMaxLength(2000);

        // 枚舉轉換為字符串
        builder.Property(w => w.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        // 值對象配置
        builder.OwnsOne(w => w.Metadata, metadata =>
        {
            metadata.ToJson("metadata");
        });

        // 審計字段
        builder.Property(w => w.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(w => w.CreatedBy)
            .HasColumnName("created_by")
            .IsRequired();

        builder.Property(w => w.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp with time zone");

        builder.Property(w => w.UpdatedBy)
            .HasColumnName("updated_by");

        // 軟刪除字段
        builder.Property(w => w.IsDeleted)
            .HasColumnName("is_deleted")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(w => w.DeletedAt)
            .HasColumnName("deleted_at")
            .HasColumnType("timestamp with time zone");

        builder.Property(w => w.DeletedBy)
            .HasColumnName("deleted_by");

        // 並發標記
        builder.Property(w => w.Version)
            .HasColumnName("version")
            .IsConcurrencyToken()
            .IsRequired()
            .HasDefaultValue(1);

        // 外鍵關聯
        builder.HasOne(w => w.Owner)
            .WithMany(u => u.Workflows)
            .HasForeignKey(w => w.OwnerId)
            .HasConstraintName("fk_workflows_users_owner_id")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(w => w.Template)
            .WithMany()
            .HasForeignKey(w => w.TemplateId)
            .HasConstraintName("fk_workflows_templates_template_id")
            .OnDelete(DeleteBehavior.SetNull);

        // 集合導航屬性
        builder.HasMany(w => w.Nodes)
            .WithOne(n => n.Workflow)
            .HasForeignKey(n => n.WorkflowId)
            .HasConstraintName("fk_workflow_nodes_workflows_workflow_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(w => w.Edges)
            .WithOne(e => e.Workflow)
            .HasForeignKey(e => e.WorkflowId)
            .HasConstraintName("fk_workflow_edges_workflows_workflow_id")
            .OnDelete(DeleteBehavior.Cascade);

        // 索引
        builder.HasIndex(w => w.OwnerId)
            .HasDatabaseName("ix_workflows_owner_id");

        builder.HasIndex(w => w.Status)
            .HasDatabaseName("ix_workflows_status");

        builder.HasIndex(w => new { w.OwnerId, w.Status })
            .HasDatabaseName("ix_workflows_owner_id_status");

        builder.HasIndex(w => w.CreatedAt)
            .HasDatabaseName("ix_workflows_created_at")
            .IsDescending();

        builder.HasIndex(w => w.IsDeleted)
            .HasDatabaseName("ix_workflows_is_deleted")
            .HasFilter("is_deleted = false");

        // 唯一約束
        builder.HasIndex(w => w.Name)
            .HasDatabaseName("uq_workflows_name_owner")
            .IsUnique()
            .HasFilter("is_deleted = false");

        // 忽略領域事件集合
        builder.Ignore(w => w.DomainEvents);
    }
}
```

### 複雜實體配置示例

**WorkflowNode 配置 (多態支持)**:

```csharp
// AIWorkflow.Infrastructure/Persistence/Configurations/WorkflowNodeConfiguration.cs
public sealed class WorkflowNodeConfiguration : IEntityTypeConfiguration<WorkflowNode>
{
    public void Configure(EntityTypeBuilder<WorkflowNode> builder)
    {
        builder.ToTable("workflow_nodes", "workflow");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(n => n.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(n => n.Type)
            .HasColumnName("type")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        // 位置信息 (值對象)
        builder.OwnsOne(n => n.Position, position =>
        {
            position.Property(p => p.X)
                .HasColumnName("position_x")
                .IsRequired();

            position.Property(p => p.Y)
                .HasColumnName("position_y")
                .IsRequired();
        });

        // 配置信息 (JSONB)
        builder.Property(n => n.Configuration)
            .HasColumnName("configuration")
            .HasColumnType("jsonb")
            .IsRequired();

        // 外鍵
        builder.HasOne(n => n.Workflow)
            .WithMany(w => w.Nodes)
            .HasForeignKey(n => n.WorkflowId)
            .HasConstraintName("fk_workflow_nodes_workflows_workflow_id")
            .OnDelete(DeleteBehavior.Cascade);

        // 索引
        builder.HasIndex(n => n.WorkflowId)
            .HasDatabaseName("ix_workflow_nodes_workflow_id");

        builder.HasIndex(n => n.Type)
            .HasDatabaseName("ix_workflow_nodes_type");

        // GIN 索引用於 JSONB 查詢
        builder.HasIndex(n => n.Configuration)
            .HasDatabaseName("ix_workflow_nodes_configuration_gin")
            .HasMethod("gin");

        builder.Ignore(n => n.DomainEvents);
    }
}
```

## 關聯關係映射

### 一對多 (One-to-Many)

```csharp
// 一對多: User → Workflows
public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users", "identity");

        builder.HasKey(u => u.Id);

        // 一對多關聯
        builder.HasMany(u => u.Workflows)
            .WithOne(w => w.Owner)
            .HasForeignKey(w => w.OwnerId)
            .HasConstraintName("fk_workflows_users_owner_id")
            .OnDelete(DeleteBehavior.Restrict);  // 防止刪除有工作流程的用戶

        builder.HasMany(u => u.CreatedWorkflows)
            .WithOne()
            .HasForeignKey(w => w.CreatedBy)
            .HasConstraintName("fk_workflows_users_created_by")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

### 一對一 (One-to-One)

```csharp
// 一對一: User → UserProfile
public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users", "identity");

        builder.HasKey(u => u.Id);

        // 一對一關聯
        builder.HasOne(u => u.Profile)
            .WithOne(p => p.User)
            .HasForeignKey<UserProfile>(p => p.UserId)
            .HasConstraintName("fk_user_profiles_users_user_id")
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("user_profiles", "identity");

        builder.HasKey(p => p.Id);

        // 唯一外鍵確保一對一
        builder.HasIndex(p => p.UserId)
            .IsUnique()
            .HasDatabaseName("uq_user_profiles_user_id");
    }
}
```

### 多對多 (Many-to-Many)

**顯式連接表 (推薦)**:

```csharp
// 多對多: User ←→ Role (顯式連接表)
public sealed class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("user_roles", "identity");

        // 複合主鍵
        builder.HasKey(ur => new { ur.UserId, ur.RoleId });

        // 關聯配置
        builder.HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .HasConstraintName("fk_user_roles_users_user_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId)
            .HasConstraintName("fk_user_roles_roles_role_id")
            .OnDelete(DeleteBehavior.Cascade);

        // 額外字段
        builder.Property(ur => ur.AssignedAt)
            .HasColumnName("assigned_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(ur => ur.AssignedBy)
            .HasColumnName("assigned_by")
            .IsRequired();

        // 索引
        builder.HasIndex(ur => ur.UserId)
            .HasDatabaseName("ix_user_roles_user_id");

        builder.HasIndex(ur => ur.RoleId)
            .HasDatabaseName("ix_user_roles_role_id");
    }
}
```

**隱式連接表 (簡單場景)**:

```csharp
// 多對多: Workflow ←→ Tag (隱式連接表,無額外字段)
public sealed class WorkflowConfiguration : IEntityTypeConfiguration<Workflow>
{
    public void Configure(EntityTypeBuilder<Workflow> builder)
    {
        builder.ToTable("workflows", "workflow");

        // 多對多關聯 (EF Core 自動創建連接表)
        builder.HasMany(w => w.Tags)
            .WithMany(t => t.Workflows)
            .UsingEntity<Dictionary<string, object>>(
                "WorkflowTags",  // 連接表名稱
                j => j.HasOne<Tag>()
                    .WithMany()
                    .HasForeignKey("TagId")
                    .HasConstraintName("fk_workflow_tags_tags_tag_id")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<Workflow>()
                    .WithMany()
                    .HasForeignKey("WorkflowId")
                    .HasConstraintName("fk_workflow_tags_workflows_workflow_id")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.ToTable("workflow_tags", "workflow");
                    j.HasKey("WorkflowId", "TagId");
                    j.HasIndex("WorkflowId").HasDatabaseName("ix_workflow_tags_workflow_id");
                    j.HasIndex("TagId").HasDatabaseName("ix_workflow_tags_tag_id");
                });
    }
}
```

### 自引用關聯 (Self-Referencing)

```csharp
// 自引用: WorkflowNode 父子關係
public sealed class WorkflowNodeConfiguration : IEntityTypeConfiguration<WorkflowNode>
{
    public void Configure(EntityTypeBuilder<WorkflowNode> builder)
    {
        builder.ToTable("workflow_nodes", "workflow");

        // 自引用一對多
        builder.HasOne(n => n.ParentNode)
            .WithMany(n => n.ChildNodes)
            .HasForeignKey(n => n.ParentNodeId)
            .HasConstraintName("fk_workflow_nodes_parent_node_id")
            .OnDelete(DeleteBehavior.Restrict);  // 防止級聯刪除循環

        builder.HasIndex(n => n.ParentNodeId)
            .HasDatabaseName("ix_workflow_nodes_parent_node_id");
    }
}
```

## 值對象映射

### Owned Types (值對象)

**簡單值對象**:

```csharp
// 值對象: Address
public sealed record Address
{
    public required string Street { get; init; }
    public required string City { get; init; }
    public required string Country { get; init; }
    public required string PostalCode { get; init; }
}

// 實體配置
public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users", "identity");

        // Owned Type 配置
        builder.OwnsOne(u => u.Address, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("address_street")
                .HasMaxLength(200)
                .IsRequired();

            address.Property(a => a.City)
                .HasColumnName("address_city")
                .HasMaxLength(100)
                .IsRequired();

            address.Property(a => a.Country)
                .HasColumnName("address_country")
                .HasMaxLength(100)
                .IsRequired();

            address.Property(a => a.PostalCode)
                .HasColumnName("address_postal_code")
                .HasMaxLength(20)
                .IsRequired();
        });
    }
}
```

**JSON 值對象 (PostgreSQL JSONB)**:

```csharp
// 值對象: WorkflowMetadata
public sealed record WorkflowMetadata
{
    public required string Version { get; init; }
    public required string Author { get; init; }
    public Dictionary<string, string> Tags { get; init; } = [];
    public Dictionary<string, object> CustomData { get; init; } = [];
}

// 實體配置
public sealed class WorkflowConfiguration : IEntityTypeConfiguration<Workflow>
{
    public void Configure(EntityTypeBuilder<Workflow> builder)
    {
        builder.ToTable("workflows", "workflow");

        // JSON 值對象
        builder.OwnsOne(w => w.Metadata, metadata =>
        {
            metadata.ToJson("metadata");  // PostgreSQL JSONB 列

            // 可選: 配置 JSON 內部屬性
            metadata.Property(m => m.Version).IsRequired();
            metadata.Property(m => m.Author).IsRequired();
        });
    }
}
```

**集合值對象**:

```csharp
// 值對象集合: EmailAddress
public sealed record EmailAddress
{
    public required string Value { get; init; }
    public required EmailType Type { get; init; }
    public required bool IsPrimary { get; init; }
}

// 實體配置
public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users", "identity");

        // Owned Type 集合
        builder.OwnsMany(u => u.Emails, email =>
        {
            email.ToTable("user_emails", "identity");

            email.Property(e => e.Value)
                .HasColumnName("email")
                .HasMaxLength(255)
                .IsRequired();

            email.Property(e => e.Type)
                .HasColumnName("type")
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            email.Property(e => e.IsPrimary)
                .HasColumnName("is_primary")
                .IsRequired();

            // 唯一約束: 每個用戶只能有一個主郵箱
            email.HasIndex(e => new { e.Value, e.IsPrimary })
                .HasDatabaseName("uq_user_emails_primary")
                .IsUnique()
                .HasFilter("is_primary = true");
        });
    }
}
```

## 繼承映射策略

### TPH (Table Per Hierarchy) - 單表繼承

```csharp
// 基類
public abstract class WorkflowNode
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required NodeType Type { get; set; }
}

// 派生類
public sealed class ActionNode : WorkflowNode
{
    public required string ActionType { get; set; }
    public required string Endpoint { get; set; }
}

public sealed class ConditionNode : WorkflowNode
{
    public required string Expression { get; set; }
}

// TPH 配置
public sealed class WorkflowNodeConfiguration : IEntityTypeConfiguration<WorkflowNode>
{
    public void Configure(EntityTypeBuilder<WorkflowNode> builder)
    {
        builder.ToTable("workflow_nodes", "workflow");

        // 鑑別器列 (Discriminator)
        builder.HasDiscriminator<string>("node_type")
            .HasValue<ActionNode>("Action")
            .HasValue<ConditionNode>("Condition");

        // 共享字段
        builder.Property(n => n.Id).HasColumnName("id");
        builder.Property(n => n.Name).HasColumnName("name").HasMaxLength(200);

        // ActionNode 專屬字段
        builder.Property<string>("ActionType")
            .HasColumnName("action_type")
            .HasMaxLength(100);

        builder.Property<string>("Endpoint")
            .HasColumnName("endpoint")
            .HasMaxLength(500);

        // ConditionNode 專屬字段
        builder.Property<string>("Expression")
            .HasColumnName("expression")
            .HasColumnType("text");
    }
}
```

### TPT (Table Per Type) - 每類型一表

```csharp
// TPT 配置 (不推薦,性能較差)
public sealed class WorkflowNodeConfiguration : IEntityTypeConfiguration<WorkflowNode>
{
    public void Configure(EntityTypeBuilder<WorkflowNode> builder)
    {
        builder.ToTable("workflow_nodes", "workflow");
        builder.HasKey(n => n.Id);
    }
}

public sealed class ActionNodeConfiguration : IEntityTypeConfiguration<ActionNode>
{
    public void Configure(EntityTypeBuilder<ActionNode> builder)
    {
        builder.ToTable("action_nodes", "workflow");

        builder.Property(n => n.ActionType)
            .HasColumnName("action_type")
            .HasMaxLength(100)
            .IsRequired();
    }
}
```

**推薦**: 使用 TPH 或 JSONB 存儲多態數據,避免 TPT 的性能問題。

## 並發控制

### 樂觀並發 (Optimistic Concurrency)

**使用 Timestamp/RowVersion**:

```csharp
// 實體定義
public sealed class Workflow
{
    public int Id { get; set; }
    public required string Name { get; set; }

    // 並發標記
    public byte[] RowVersion { get; set; } = [];
}

// 配置
public sealed class WorkflowConfiguration : IEntityTypeConfiguration<Workflow>
{
    public void Configure(EntityTypeBuilder<Workflow> builder)
    {
        builder.Property(w => w.RowVersion)
            .HasColumnName("row_version")
            .IsRowVersion();  // PostgreSQL: xmin 系統列
    }
}

// 使用示例
try
{
    var workflow = await context.Workflows.FindAsync(id);
    workflow.Name = "Updated Name";
    await context.SaveChangesAsync();
}
catch (DbUpdateConcurrencyException ex)
{
    // 處理並發衝突
    var entry = ex.Entries.Single();
    var databaseValues = await entry.GetDatabaseValuesAsync();

    if (databaseValues == null)
    {
        // 記錄已被刪除
        throw new NotFoundException("Workflow not found");
    }

    // 顯示衝突並讓用戶選擇
    var databaseWorkflow = (Workflow)databaseValues.ToObject();
    // ... 處理邏輯
}
```

**使用 Version 字段**:

```csharp
// 實體定義
public sealed class Workflow
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int Version { get; set; } = 1;
}

// 配置
public sealed class WorkflowConfiguration : IEntityTypeConfiguration<Workflow>
{
    public void Configure(EntityTypeBuilder<Workflow> builder)
    {
        builder.Property(w => w.Version)
            .HasColumnName("version")
            .IsConcurrencyToken()
            .IsRequired()
            .HasDefaultValue(1);
    }
}
```

## 全局查詢過濾器

### 軟刪除過濾器

```csharp
// 接口定義
public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }
    int? DeletedBy { get; set; }
}

// DbContext 配置
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // 全局軟刪除過濾器
    foreach (var entityType in modelBuilder.Model.GetEntityTypes())
    {
        if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
        {
            var parameter = Expression.Parameter(entityType.ClrType, "e");
            var property = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
            var filterExpression = Expression.Lambda(Expression.Not(property), parameter);

            entityType.SetQueryFilter(filterExpression);
        }
    }
}

// 查詢時自動應用過濾器
var workflows = await context.Workflows.ToListAsync();  // 自動過濾 IsDeleted = true

// 忽略過濾器查詢所有記錄
var allWorkflows = await context.Workflows
    .IgnoreQueryFilters()
    .ToListAsync();
```

### 多租戶過濾器

```csharp
// 接口定義
public interface ITenantEntity
{
    int TenantId { get; set; }
}

// DbContext 配置
public sealed class ApplicationDbContext : DbContext
{
    private readonly ITenantService _tenantService;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ITenantService tenantService)
        : base(options)
    {
        _tenantService = tenantService;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 多租戶過濾器
        var tenantId = _tenantService.GetCurrentTenantId();

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(ITenantEntity.TenantId));
                var constant = Expression.Constant(tenantId);
                var filterExpression = Expression.Lambda(
                    Expression.Equal(property, constant),
                    parameter);

                entityType.SetQueryFilter(filterExpression);
            }
        }
    }
}
```

## 種子數據 (Seed Data)

### 配置種子數據

```csharp
// AIWorkflow.Infrastructure/Persistence/Configurations/SeedData/RoleSeedData.cs
public sealed class RoleSeedData : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasData(
            new Role
            {
                Id = 1,
                Name = "Admin",
                NormalizedName = "ADMIN",
                Description = "系統管理員",
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = 0  // 系統創建
            },
            new Role
            {
                Id = 2,
                Name = "User",
                NormalizedName = "USER",
                Description = "一般用戶",
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = 0
            },
            new Role
            {
                Id = 3,
                Name = "Guest",
                NormalizedName = "GUEST",
                Description = "訪客",
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = 0
            }
        );
    }
}
```

### 開發環境種子數據

```csharp
// AIWorkflow.Infrastructure/Persistence/ApplicationDbContextSeed.cs
public static class ApplicationDbContextSeed
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        ILogger<ApplicationDbContext> logger)
    {
        try
        {
            await SeedRolesAsync(context);
            await SeedUsersAsync(context);
            await SeedWorkflowTemplatesAsync(context);

            await context.SaveChangesAsync();

            logger.LogInformation("Database seeded successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    private static async Task SeedRolesAsync(ApplicationDbContext context)
    {
        if (!await context.Roles.AnyAsync())
        {
            var roles = new List<Role>
            {
                new() { Name = "Admin", NormalizedName = "ADMIN", CreatedAt = DateTime.UtcNow },
                new() { Name = "User", NormalizedName = "USER", CreatedAt = DateTime.UtcNow }
            };

            await context.Roles.AddRangeAsync(roles);
        }
    }

    private static async Task SeedUsersAsync(ApplicationDbContext context)
    {
        if (!await context.Users.AnyAsync())
        {
            var adminRole = await context.Roles.FirstAsync(r => r.Name == "Admin");

            var admin = new User
            {
                Email = "admin@aiworkflow.com",
                UserName = "admin",
                FirstName = "System",
                LastName = "Administrator",
                CreatedAt = DateTime.UtcNow
            };

            await context.Users.AddAsync(admin);
        }
    }
}

// Program.cs 中調用
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

    await context.Database.MigrateAsync();
    await ApplicationDbContextSeed.SeedAsync(context, logger);
}
```

## 性能優化

### Split Queries vs Single Query

```csharp
// Single Query (默認): 一次查詢,可能產生笛卡爾積
var workflows = await context.Workflows
    .Include(w => w.Nodes)
    .Include(w => w.Edges)
    .ToListAsync();

// Split Query (推薦用於多個集合): 分別查詢,避免笛卡爾積
var workflows = await context.Workflows
    .Include(w => w.Nodes)
    .Include(w => w.Edges)
    .AsSplitQuery()
    .ToListAsync();

// 全局配置 Split Query
services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    });
});
```

### Compiled Queries

```csharp
// 編譯查詢提升性能
private static readonly Func<ApplicationDbContext, int, Task<Workflow?>> _getWorkflowById =
    EF.CompileAsyncQuery((ApplicationDbContext context, int id) =>
        context.Workflows
            .Include(w => w.Nodes)
            .Include(w => w.Edges)
            .FirstOrDefault(w => w.Id == id));

// 使用
var workflow = await _getWorkflowById(context, workflowId);
```

### Projection 優化

```csharp
// ❌ 不推薦: 加載整個實體
var workflows = await context.Workflows
    .Include(w => w.Owner)
    .ToListAsync();

// ✅ 推薦: 只選擇需要的字段
var workflows = await context.Workflows
    .Select(w => new WorkflowDto
    {
        Id = w.Id,
        Name = w.Name,
        OwnerName = w.Owner.UserName,
        NodeCount = w.Nodes.Count
    })
    .ToListAsync();
```

## 檢查清單

### DbContext 配置
- [ ] 定義 IApplicationDbContext 接口
- [ ] 實現 ApplicationDbContext
- [ ] 配置全局查詢過濾器
- [ ] 處理審計字段自動更新
- [ ] 實現領域事件分發
- [ ] 註冊依賴注入

### Entity Configuration
- [ ] 為每個實體創建 IEntityTypeConfiguration
- [ ] 配置表名和 Schema
- [ ] 配置主鍵和外鍵
- [ ] 配置索引和約束
- [ ] 配置並發標記
- [ ] 配置值對象映射

### 關聯關係
- [ ] 正確配置一對多關係
- [ ] 正確配置一對一關係
- [ ] 正確配置多對多關係
- [ ] 設置正確的刪除行為
- [ ] 配置必要的導航屬性

### 性能優化
- [ ] 使用 AsNoTracking 用於只讀查詢
- [ ] 配置 Split Query 避免笛卡爾積
- [ ] 使用 Projection 減少數據傳輸
- [ ] 實現 Compiled Queries
- [ ] 配置適當的連接池大小

## 相關文檔

- [數據庫設計原則](./database-design-principles.md)
- [數據庫遷移策略](./database-migration-strategy.md)
- [查詢性能優化](./query-performance-optimization.md)
- [Phase 3: 後端項目結構](../3-project-structure/backend-project-structure.md)

## 參考資源

### 官方文檔
- [Entity Framework Core Documentation](https://learn.microsoft.com/en-us/ef/core/)
- [Entity Framework Core 9.0 What's New](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-9.0/whatsnew)
- [Npgsql Entity Framework Core Provider](https://www.npgsql.org/efcore/)

### 最佳實踐
- [EF Core Performance Best Practices](https://learn.microsoft.com/en-us/ef/core/performance/)
- [Entity Configuration Best Practices](https://learn.microsoft.com/en-us/ef/core/modeling/)

### 書籍
- *Entity Framework Core in Action* by Jon P Smith
- *Programming Entity Framework Core* by Julia Lerman

---

**文檔維護**: Database Team
**最後更新**: 2025-01-01
**狀態**: ✅ 完整
