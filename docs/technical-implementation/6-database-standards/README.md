# Phase 6: 數據庫標準 (Database Standards)

## 階段概述

本階段定義 AI Workflow Platform 的完整數據庫設計標準、Entity Framework Core 最佳實踐、遷移策略和性能優化指南。

## 目標

- ✅ 制定統一的數據庫設計原則和命名規範
- ✅ 定義 Entity Framework Core 9 配置標準
- ✅ 建立數據庫遷移和版本控制策略
- ✅ 實現查詢性能優化和索引策略
- ✅ 確保數據完整性和一致性

## 文檔結構

### 1. 數據庫設計原則
**文件**: `database-design-principles.md`

**內容**:
- 數據庫架構設計
- 表命名規範
- 列命名規範
- 主鍵和外鍵策略
- 索引設計原則
- 數據類型選擇
- 正規化與反正規化
- 軟刪除設計

**技術重點**:
- PostgreSQL 16 數據庫特性
- Clean Architecture 數據層設計
- Domain-Driven Design 實體映射

### 2. Entity Framework Core 配置
**文件**: `entity-framework-core-configuration.md`

**內容**:
- DbContext 配置
- Entity Configuration (IEntityTypeConfiguration)
- 關聯關係映射 (一對一、一對多、多對多)
- 值對象映射
- 繼承映射策略
- 並發控制 (Optimistic Concurrency)
- 全局查詢過濾器 (Global Query Filters)
- 種子數據 (Seed Data)

**技術重點**:
- Entity Framework Core 9.0
- Fluent API 配置
- Shadow Properties
- Owned Types

### 3. 數據庫遷移策略
**文件**: `database-migration-strategy.md`

**內容**:
- EF Core Migrations 工作流程
- 遷移文件管理
- 環境特定遷移
- 數據遷移腳本
- 回滾策略
- CI/CD 集成
- 生產環境遷移最佳實踐

**技術重點**:
- dotnet ef migrations 命令
- 遷移歷史追蹤 (__EFMigrationsHistory)
- 數據保護和備份

### 4. 查詢性能優化
**文件**: `query-performance-optimization.md`

**內容**:
- 查詢優化技巧
- N+1 查詢問題解決
- 預加載 (Eager Loading) vs 延遲加載 (Lazy Loading)
- 顯式加載 (Explicit Loading)
- AsNoTracking 使用
- 索引策略
- 查詢計劃分析
- 批量操作優化

**技術重點**:
- Include() 和 ThenInclude()
- AsSplitQuery() vs AsSingleQuery()
- 執行計劃 (EXPLAIN ANALYZE)
- 批量插入/更新/刪除

### 5. 數據完整性與約束
**文件**: `data-integrity-constraints.md`

**內容**:
- 主鍵約束
- 外鍵約束
- 唯一約束
- 檢查約束
- 默認值
- 觸發器使用指南
- 事務管理
- 分佈式事務

**技術重點**:
- PostgreSQL 約束語法
- EF Core Fluent API 約束配置
- Transaction Scope

## 技術棧

### 數據庫
```
PostgreSQL 16              # 主數據庫
Redis 7                    # 緩存層
MongoDB 7                  # 日誌和文檔存儲
```

### ORM
```
Entity Framework Core 9.0.0             # ORM 框架
Npgsql.EntityFrameworkCore.PostgreSQL 9.0.2  # PostgreSQL Provider
```

### 遷移工具
```
dotnet ef                   # EF Core 命令行工具
Npgsql                      # PostgreSQL .NET 驅動
```

### 性能監控
```
Npgsql.OpenTelemetry        # 性能追蹤
MiniProfiler.EntityFrameworkCore  # 查詢分析
```

## 數據庫架構概述

### PostgreSQL 數據庫結構

```sql
-- 主數據庫: aiworkflow
CREATE DATABASE aiworkflow;

-- Schema 組織
CREATE SCHEMA workflow;     -- 工作流相關表
CREATE SCHEMA identity;     -- 用戶和認證
CREATE SCHEMA execution;    -- 執行記錄
CREATE SCHEMA audit;        -- 審計日誌
```

### 核心表結構

```
workflow.workflows          -- 工作流主表
workflow.workflow_nodes     -- 工作流節點
workflow.workflow_edges     -- 工作流邊
workflow.workflow_templates -- 工作流模板

identity.users              -- 用戶表
identity.roles              -- 角色表
identity.user_roles         -- 用戶角色關聯

execution.workflow_executions      -- 執行記錄
execution.workflow_execution_logs  -- 執行日誌

audit.audit_logs            -- 審計日誌
```

## 命名規範

### 表命名

**規則**:
- 使用複數形式
- 小寫字母
- 單詞間用下劃線分隔
- Schema 前綴

```sql
✅ 正確
workflow.workflows
workflow.workflow_nodes
identity.users

❌ 錯誤
Workflow               -- 單數形式
WorkflowNode          -- Pascal Case
workflow_node         -- 單數形式
workflownodes         -- 無分隔符
```

### 列命名

**規則**:
- 小寫字母
- 單詞間用下劃線分隔
- 主鍵使用 `id`
- 外鍵使用 `{table}_id` 格式
- 時間戳使用 `_at` 後綴

```sql
✅ 正確
id
name
description
owner_id              -- 外鍵
created_at
last_modified_at

❌ 錯誤
Id                    -- 大寫
userName              -- camelCase
createdDate           -- 不一致的後綴
user                  -- 外鍵缺少 _id
```

### 索引命名

**格式**: `ix_{table}_{column(s)}`

```sql
✅ 正確
ix_workflows_owner_id
ix_workflows_status
ix_workflows_owner_id_status     -- 複合索引

❌ 錯誤
idx_workflow_owner
workflow_index
index1
```

### 約束命名

```sql
-- 主鍵: pk_{table}
pk_workflows

-- 外鍵: fk_{table}_{referenced_table}_{column}
fk_workflows_users_owner_id

-- 唯一約束: uq_{table}_{column(s)}
uq_users_email

-- 檢查約束: ck_{table}_{column}_{condition}
ck_workflows_status_valid
```

## Entity Framework Core 配置示例

### DbContext 配置

```csharp
public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<Workflow> Workflows => Set<Workflow>();
    public DbSet<WorkflowNode> WorkflowNodes => Set<WorkflowNode>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 應用所有配置
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // 全局查詢過濾器 (軟刪除)
        modelBuilder.Entity<Workflow>().HasQueryFilter(w => !w.IsDeleted);
        modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
    }
}
```

### Entity Configuration

```csharp
public sealed class WorkflowConfiguration : IEntityTypeConfiguration<Workflow>
{
    public void Configure(EntityTypeBuilder<Workflow> builder)
    {
        builder.ToTable("workflows", "workflow");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(w => w.Description)
            .HasMaxLength(2000);

        builder.Property(w => w.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(w => w.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        // 外鍵關聯
        builder.HasOne(w => w.Owner)
            .WithMany(u => u.Workflows)
            .HasForeignKey(w => w.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        // 集合導航屬性
        builder.HasMany(w => w.Nodes)
            .WithOne(n => n.Workflow)
            .HasForeignKey(n => n.WorkflowId)
            .OnDelete(DeleteBehavior.Cascade);

        // 索引
        builder.HasIndex(w => w.OwnerId)
            .HasDatabaseName("ix_workflows_owner_id");

        builder.HasIndex(w => w.Status)
            .HasDatabaseName("ix_workflows_status");

        builder.HasIndex(w => new { w.OwnerId, w.Status })
            .HasDatabaseName("ix_workflows_owner_id_status");

        // 並發標記
        builder.Property(w => w.Version)
            .IsConcurrencyToken();
    }
}
```

## 遷移工作流程

### 創建遷移

```bash
# 添加新遷移
dotnet ef migrations add InitialCreate --project AIWorkflow.Infrastructure --startup-project AIWorkflow.API

# 查看遷移 SQL
dotnet ef migrations script --project AIWorkflow.Infrastructure --startup-project AIWorkflow.API

# 應用遷移
dotnet ef database update --project AIWorkflow.Infrastructure --startup-project AIWorkflow.API
```

### 環境特定遷移

```bash
# 開發環境
dotnet ef database update --project AIWorkflow.Infrastructure --startup-project AIWorkflow.API --context ApplicationDbContext

# 生產環境 (生成 SQL 腳本)
dotnet ef migrations script --project AIWorkflow.Infrastructure --startup-project AIWorkflow.API --idempotent --output migration.sql
```

## 性能優化策略

### 1. 避免 N+1 查詢

```csharp
// ❌ N+1 查詢問題
var workflows = await context.Workflows.ToListAsync();
foreach (var workflow in workflows)
{
    var owner = await context.Users.FindAsync(workflow.OwnerId);  // N 次查詢
}

// ✅ 使用 Include 預加載
var workflows = await context.Workflows
    .Include(w => w.Owner)
    .ToListAsync();
```

### 2. AsNoTracking 用於只讀查詢

```csharp
// ✅ 只讀查詢使用 AsNoTracking
var workflows = await context.Workflows
    .AsNoTracking()
    .Where(w => w.Status == WorkflowStatus.Published)
    .ToListAsync();
```

### 3. 分頁查詢

```csharp
// ✅ 高效分頁
var workflows = await context.Workflows
    .AsNoTracking()
    .Where(w => w.OwnerId == userId)
    .OrderByDescending(w => w.CreatedAt)
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();
```

### 4. 批量操作

```csharp
// ✅ 批量插入
await context.Workflows.AddRangeAsync(workflows);
await context.SaveChangesAsync();

// ✅ 批量刪除
context.Workflows.RemoveRange(workflowsToDelete);
await context.SaveChangesAsync();
```

## 索引策略

### 單列索引

```sql
CREATE INDEX ix_workflows_owner_id ON workflow.workflows (owner_id);
CREATE INDEX ix_workflows_status ON workflow.workflows (status);
CREATE INDEX ix_workflows_created_at ON workflow.workflows (created_at DESC);
```

### 複合索引

```sql
-- 覆蓋查詢: WHERE owner_id = ? AND status = ?
CREATE INDEX ix_workflows_owner_id_status ON workflow.workflows (owner_id, status);

-- 覆蓋排序: WHERE owner_id = ? ORDER BY created_at DESC
CREATE INDEX ix_workflows_owner_id_created_at ON workflow.workflows (owner_id, created_at DESC);
```

### 部分索引

```sql
-- 只索引已發布的工作流
CREATE INDEX ix_workflows_published ON workflow.workflows (owner_id)
WHERE status = 'Published';
```

## 數據完整性

### 外鍵約束

```sql
ALTER TABLE workflow.workflows
ADD CONSTRAINT fk_workflows_users_owner_id
FOREIGN KEY (owner_id)
REFERENCES identity.users(id)
ON DELETE RESTRICT;
```

### 唯一約束

```sql
ALTER TABLE identity.users
ADD CONSTRAINT uq_users_email UNIQUE (email);
```

### 檢查約束

```sql
ALTER TABLE workflow.workflows
ADD CONSTRAINT ck_workflows_status_valid
CHECK (status IN ('Draft', 'Published', 'Archived'));
```

## 軟刪除實現

### Entity 定義

```csharp
public abstract class AuditableEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public sealed class Workflow : AuditableEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    // ...
}
```

### 全局查詢過濾器

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    foreach (var entityType in modelBuilder.Model.GetEntityTypes())
    {
        if (typeof(AuditableEntity).IsAssignableFrom(entityType.ClrType))
        {
            var parameter = Expression.Parameter(entityType.ClrType, "e");
            var property = Expression.Property(parameter, nameof(AuditableEntity.IsDeleted));
            var filter = Expression.Lambda(Expression.Not(property), parameter);

            entityType.SetQueryFilter(filter);
        }
    }
}
```

## 監控與診斷

### 啟用日誌

```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString)
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging(builder.Environment.IsDevelopment())
        .EnableDetailedErrors();
});
```

### 查詢計劃分析

```sql
EXPLAIN ANALYZE
SELECT * FROM workflow.workflows
WHERE owner_id = 42 AND status = 'Published'
ORDER BY created_at DESC;
```

## 實現檢查清單

### 設計階段
- [ ] 定義數據庫 Schema 結構
- [ ] 設計表和列命名規範
- [ ] 規劃索引策略
- [ ] 設計軟刪除機制
- [ ] 定義外鍵關聯

### 配置階段
- [ ] 配置 DbContext
- [ ] 實現 IEntityTypeConfiguration
- [ ] 配置關聯關係
- [ ] 設置全局查詢過濾器
- [ ] 配置並發控制

### 遷移階段
- [ ] 創建初始遷移
- [ ] 測試遷移腳本
- [ ] 配置 CI/CD 遷移流程
- [ ] 制定回滾策略
- [ ] 準備生產環境遷移

### 優化階段
- [ ] 識別 N+1 查詢問題
- [ ] 添加必要索引
- [ ] 使用 AsNoTracking
- [ ] 優化批量操作
- [ ] 分析查詢計劃

### 監控階段
- [ ] 啟用 EF Core 日誌
- [ ] 配置性能監控
- [ ] 設置慢查詢告警
- [ ] 追蹤數據庫連接池
- [ ] 監控死鎖情況

## 相關文檔

- [Phase 3: 項目結構 - 後端項目結構](../3-project-structure/backend-project-structure.md)
- [Phase 5: API 設計規範](../5-api-design/README.md)
- [Phase 7: 測試策略](../7-testing-strategy/README.md)
- [Phase 10: 監控與運維](../10-monitoring-operations/README.md)

## 參考資源

### 官方文檔
- [Entity Framework Core Documentation](https://learn.microsoft.com/en-us/ef/core/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [Npgsql Entity Framework Core Provider](https://www.npgsql.org/efcore/)

### 最佳實踐
- [EF Core Performance Best Practices](https://learn.microsoft.com/en-us/ef/core/performance/)
- [PostgreSQL Performance Tips](https://wiki.postgresql.org/wiki/Performance_Optimization)
- [Database Design Best Practices](https://www.postgresql.org/docs/current/ddl.html)

### 書籍
- *Entity Framework Core in Action* by Jon P Smith
- *PostgreSQL: Up and Running* by Regina Obe, Leo Hsu
- *Database Design for Mere Mortals* by Michael J. Hernandez

---

**文檔維護**: Tech Lead Team
**最後更新**: 2025-01-01
**狀態**: ✅ 完整
