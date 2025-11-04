# 查詢性能優化 (Query Performance Optimization)

## 概述

本文檔詳細說明 Entity Framework Core 9.0 的查詢性能優化策略,包括 N+1 問題解決、加載策略、追蹤行為、索引優化、查詢計劃分析和批量操作優化。

## 目標

- ✅ 識別和解決常見性能問題
- ✅ 優化數據庫查詢效率
- ✅ 減少網絡往返次數
- ✅ 降低內存消耗
- ✅ 提升應用程序響應速度

## N+1 查詢問題

### 問題識別

**❌ N+1 查詢示例**:

```csharp
// 1 次查詢獲取工作流程
var workflows = await context.Workflows.ToListAsync();

// N 次查詢獲取每個工作流程的所有者 (N+1 問題!)
foreach (var workflow in workflows)
{
    var owner = await context.Users.FindAsync(workflow.OwnerId);
    Console.WriteLine($"{workflow.Name} - {owner.UserName}");
}

// 生成的 SQL:
// SELECT * FROM workflow.workflows;  -- 1 次
// SELECT * FROM identity.users WHERE id = 1;  -- 第 1 次
// SELECT * FROM identity.users WHERE id = 2;  -- 第 2 次
// ... (總共 N+1 次查詢)
```

### 解決方案 1: Eager Loading (預加載)

**✅ 使用 Include**:

```csharp
// 單次查詢獲取工作流程和所有者
var workflows = await context.Workflows
    .Include(w => w.Owner)
    .ToListAsync();

foreach (var workflow in workflows)
{
    Console.WriteLine($"{workflow.Name} - {workflow.Owner.UserName}");
}

// 生成的 SQL (單次查詢):
// SELECT w.*, u.*
// FROM workflow.workflows w
// INNER JOIN identity.users u ON w.owner_id = u.id;
```

**多級 Include**:

```csharp
// 加載多層關聯
var workflows = await context.Workflows
    .Include(w => w.Owner)
    .Include(w => w.Nodes)
        .ThenInclude(n => n.Configuration)
    .Include(w => w.Edges)
    .ToListAsync();

// 或使用字符串路徑
var workflows = await context.Workflows
    .Include("Owner")
    .Include("Nodes.Configuration")
    .Include("Edges")
    .ToListAsync();
```

**過濾 Include**:

```csharp
// EF Core 5.0+: 過濾包含的數據
var workflows = await context.Workflows
    .Include(w => w.Nodes.Where(n => n.IsActive))
    .Include(w => w.Owner)
    .ToListAsync();

// 生成的 SQL:
// SELECT w.*, u.*, n.*
// FROM workflow.workflows w
// LEFT JOIN identity.users u ON w.owner_id = u.id
// LEFT JOIN workflow.workflow_nodes n ON w.id = n.workflow_id AND n.is_active = TRUE;
```

### 解決方案 2: Explicit Loading (顯式加載)

**按需加載關聯數據**:

```csharp
var workflow = await context.Workflows
    .FirstOrDefaultAsync(w => w.Id == workflowId);

if (workflow != null)
{
    // 顯式加載所有者
    await context.Entry(workflow)
        .Reference(w => w.Owner)
        .LoadAsync();

    // 顯式加載節點集合
    await context.Entry(workflow)
        .Collection(w => w.Nodes)
        .LoadAsync();

    // 加載並過濾集合
    await context.Entry(workflow)
        .Collection(w => w.Edges)
        .Query()
        .Where(e => e.IsActive)
        .LoadAsync();
}
```

### 解決方案 3: Projection (投影)

**✅ 只選擇需要的列**:

```csharp
// 投影到 DTO,避免加載整個實體
var workflows = await context.Workflows
    .Select(w => new WorkflowListDto
    {
        Id = w.Id,
        Name = w.Name,
        OwnerName = w.Owner.UserName,  // 自動 JOIN
        NodeCount = w.Nodes.Count,     // 自動子查詢
        CreatedAt = w.CreatedAt
    })
    .ToListAsync();

// 生成的 SQL (高效):
// SELECT
//     w.id,
//     w.name,
//     u.user_name,
//     (SELECT COUNT(*) FROM workflow.workflow_nodes WHERE workflow_id = w.id) AS node_count,
//     w.created_at
// FROM workflow.workflows w
// LEFT JOIN identity.users u ON w.owner_id = u.id;
```

### 解決方案 4: Split Queries vs Single Query

**Single Query (默認)**:

```csharp
// 單次查詢,可能產生笛卡爾積
var workflows = await context.Workflows
    .Include(w => w.Nodes)
    .Include(w => w.Edges)
    .ToListAsync();

// 生成的 SQL (笛卡爾積):
// SELECT w.*, n.*, e.*
// FROM workflow.workflows w
// LEFT JOIN workflow.workflow_nodes n ON w.id = n.workflow_id
// LEFT JOIN workflow.workflow_edges e ON w.id = e.workflow_id;
//
// 問題: 如果一個工作流程有 10 個節點和 10 個邊,結果集會有 100 行重複數據
```

**Split Query (推薦)**:

```csharp
// 分割查詢,避免笛卡爾積
var workflows = await context.Workflows
    .Include(w => w.Nodes)
    .Include(w => w.Edges)
    .AsSplitQuery()
    .ToListAsync();

// 生成的 SQL (3 次獨立查詢):
// 1. SELECT * FROM workflow.workflows;
// 2. SELECT * FROM workflow.workflow_nodes WHERE workflow_id IN (...);
// 3. SELECT workflow.workflow_edges WHERE workflow_id IN (...);
```

**全局配置 Split Query**:

```csharp
services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    });
});
```

## 追蹤行為優化

### NoTracking 查詢

**只讀查詢使用 AsNoTracking**:

```csharp
// ❌ 默認行為: Change Tracker 追蹤所有實體
var workflows = await context.Workflows.ToListAsync();
// 內存開銷大,不適合只讀查詢

// ✅ NoTracking: 不追蹤實體變更
var workflows = await context.Workflows
    .AsNoTracking()
    .ToListAsync();

// 性能提升:
// - 查詢速度提升 30-50%
// - 內存消耗減少 40-60%
// - 無法使用 Update/Delete (需重新查詢)
```

**全局 NoTracking 配置**:

```csharp
services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString);
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

// 特定查詢啟用追蹤
var workflow = await context.Workflows
    .AsTracking()
    .FirstOrDefaultAsync(w => w.Id == id);
```

### NoTrackingWithIdentityResolution

**保持身份解析的 NoTracking**:

```csharp
// 避免重複實例,但不追蹤變更
var workflows = await context.Workflows
    .Include(w => w.Owner)
    .AsNoTrackingWithIdentityResolution()
    .ToListAsync();

// 好處:
// - 同一個 User 只有一個實例 (而非多個重複對象)
// - 性能介於 Tracking 和 NoTracking 之間
// - 適合需要身份解析但不修改數據的場景
```

## 分頁優化

### 基於 Offset 的分頁

**標準分頁實現**:

```csharp
public async Task<PagedResult<WorkflowDto>> GetWorkflowsAsync(
    int page,
    int pageSize,
    CancellationToken cancellationToken = default)
{
    var query = context.Workflows.AsNoTracking();

    // 總數查詢
    var totalCount = await query.CountAsync(cancellationToken);

    // 分頁數據
    var workflows = await query
        .OrderByDescending(w => w.CreatedAt)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(w => new WorkflowDto
        {
            Id = w.Id,
            Name = w.Name,
            OwnerName = w.Owner.UserName,
            CreatedAt = w.CreatedAt
        })
        .ToListAsync(cancellationToken);

    return new PagedResult<WorkflowDto>
    {
        Data = workflows,
        TotalCount = totalCount,
        Page = page,
        PageSize = pageSize
    };
}

// 生成的 SQL:
// SELECT COUNT(*) FROM workflow.workflows;
// SELECT ... FROM workflow.workflows ORDER BY created_at DESC OFFSET 20 LIMIT 10;
```

**優化: 避免重複 COUNT 查詢**:

```csharp
// 緩存總數,僅在數據變更時重新計算
public async Task<PagedResult<WorkflowDto>> GetWorkflowsOptimizedAsync(
    int page,
    int pageSize,
    string? cacheKey = null,
    CancellationToken cancellationToken = default)
{
    var query = context.Workflows.AsNoTracking();

    // 從緩存獲取總數,或計算並緩存
    var totalCount = await cache.GetOrCreateAsync(
        cacheKey ?? "workflows_total_count",
        async entry =>
        {
            entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
            return await query.CountAsync(cancellationToken);
        });

    var workflows = await query
        .OrderByDescending(w => w.CreatedAt)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(w => new WorkflowDto { ... })
        .ToListAsync(cancellationToken);

    return new PagedResult<WorkflowDto> { ... };
}
```

### 基於 Cursor 的分頁

**推薦用於大數據集**:

```csharp
public async Task<CursorPagedResult<WorkflowDto>> GetWorkflowsCursorAsync(
    int? afterId,
    int pageSize,
    CancellationToken cancellationToken = default)
{
    var query = context.Workflows.AsNoTracking();

    // 基於游標過濾
    if (afterId.HasValue)
    {
        query = query.Where(w => w.Id > afterId.Value);
    }

    var workflows = await query
        .OrderBy(w => w.Id)
        .Take(pageSize + 1)  // 多取一條判斷是否有下一頁
        .Select(w => new WorkflowDto { ... })
        .ToListAsync(cancellationToken);

    var hasNextPage = workflows.Count > pageSize;
    if (hasNextPage)
    {
        workflows = workflows.Take(pageSize).ToList();
    }

    return new CursorPagedResult<WorkflowDto>
    {
        Data = workflows,
        HasNextPage = hasNextPage,
        NextCursor = workflows.LastOrDefault()?.Id
    };
}

// 生成的 SQL (高效,無 OFFSET):
// SELECT ... FROM workflow.workflows
// WHERE id > 100
// ORDER BY id
// LIMIT 11;
```

## 批量操作優化

### 批量插入

**❌ 低效方式**:

```csharp
// 每次插入都觸發一次數據庫往返
foreach (var node in nodes)
{
    context.WorkflowNodes.Add(node);
    await context.SaveChangesAsync();  // ❌ 每次都保存
}
// 總共 N 次數據庫往返
```

**✅ 高效方式**:

```csharp
// 批量插入
context.WorkflowNodes.AddRange(nodes);
await context.SaveChangesAsync();  // 單次批量插入

// 或使用 ExecuteUpdate (EF Core 7.0+)
await context.WorkflowNodes
    .Where(n => n.WorkflowId == workflowId)
    .ExecuteUpdateAsync(s => s
        .SetProperty(n => n.IsActive, false));
```

### 批量更新

**使用 ExecuteUpdate (EF Core 7.0+)**:

```csharp
// ❌ 傳統方式: 查詢 → 修改 → 保存
var workflows = await context.Workflows
    .Where(w => w.Status == WorkflowStatus.Draft)
    .ToListAsync();

foreach (var workflow in workflows)
{
    workflow.Status = WorkflowStatus.Published;
}

await context.SaveChangesAsync();
// 問題: 查詢所有數據到內存,然後逐行 UPDATE

// ✅ ExecuteUpdate: 直接執行 SQL UPDATE
var updated = await context.Workflows
    .Where(w => w.Status == WorkflowStatus.Draft)
    .ExecuteUpdateAsync(s => s
        .SetProperty(w => w.Status, WorkflowStatus.Published)
        .SetProperty(w => w.UpdatedAt, DateTime.UtcNow));

// 生成的 SQL (單條 UPDATE 語句):
// UPDATE workflow.workflows
// SET status = 'Published', updated_at = NOW()
// WHERE status = 'Draft';
```

**複雜批量更新**:

```csharp
// 根據條件設置不同值
await context.Workflows
    .Where(w => w.OwnerId == userId)
    .ExecuteUpdateAsync(s => s
        .SetProperty(
            w => w.Priority,
            w => w.NodeCount > 10 ? 5 : 3)
        .SetProperty(w => w.UpdatedAt, DateTime.UtcNow));
```

### 批量刪除

**使用 ExecuteDelete (EF Core 7.0+)**:

```csharp
// ❌ 傳統方式
var workflows = await context.Workflows
    .Where(w => w.IsDeleted && w.DeletedAt < DateTime.UtcNow.AddMonths(-3))
    .ToListAsync();

context.Workflows.RemoveRange(workflows);
await context.SaveChangesAsync();

// ✅ ExecuteDelete: 直接刪除
var deleted = await context.Workflows
    .Where(w => w.IsDeleted && w.DeletedAt < DateTime.UtcNow.AddMonths(-3))
    .ExecuteDeleteAsync();

// 生成的 SQL:
// DELETE FROM workflow.workflows
// WHERE is_deleted = TRUE AND deleted_at < '2024-10-01';
```

## 編譯查詢

### Compiled Queries

**提升重複查詢性能**:

```csharp
// 定義編譯查詢
private static readonly Func<ApplicationDbContext, int, Task<Workflow?>> _getWorkflowByIdCompiled =
    EF.CompileAsyncQuery((ApplicationDbContext context, int id) =>
        context.Workflows
            .Include(w => w.Owner)
            .Include(w => w.Nodes)
            .FirstOrDefault(w => w.Id == id));

// 使用編譯查詢
public async Task<Workflow?> GetWorkflowByIdAsync(int id)
{
    return await _getWorkflowByIdCompiled(context, id);
}

// 性能提升:
// - 首次執行: 編譯並緩存查詢計劃
// - 後續執行: 直接使用緩存的計劃,速度提升 30-40%
```

**帶參數的編譯查詢**:

```csharp
private static readonly Func<ApplicationDbContext, int, WorkflowStatus, IAsyncEnumerable<Workflow>>
    _getWorkflowsByOwnerAndStatusCompiled = EF.CompileAsyncQuery(
        (ApplicationDbContext context, int ownerId, WorkflowStatus status) =>
            context.Workflows
                .Where(w => w.OwnerId == ownerId && w.Status == status)
                .OrderByDescending(w => w.CreatedAt));

// 使用
await foreach (var workflow in _getWorkflowsByOwnerAndStatusCompiled(context, userId, WorkflowStatus.Published))
{
    // 處理每個工作流程
}
```

## 索引優化

### 分析缺失索引

**使用 PostgreSQL 查詢分析**:

```sql
-- 查找表掃描次數最多的表
SELECT
    schemaname,
    tablename,
    seq_scan,
    seq_tup_read,
    idx_scan,
    seq_tup_read / NULLIF(seq_scan, 0) AS avg_seq_tup_read
FROM pg_stat_user_tables
WHERE seq_scan > 0
ORDER BY seq_tup_read DESC
LIMIT 20;

-- 查找未使用的索引
SELECT
    schemaname,
    tablename,
    indexname,
    idx_scan,
    pg_size_pretty(pg_relation_size(indexrelid)) AS index_size
FROM pg_stat_user_indexes
WHERE idx_scan = 0
  AND indexrelname NOT LIKE 'pg_toast%'
ORDER BY pg_relation_size(indexrelid) DESC;

-- 查找缺失索引的建議 (pg_stat_statements 擴展)
SELECT
    schemaname,
    tablename,
    attname,
    n_distinct,
    correlation
FROM pg_stats
WHERE schemaname NOT IN ('pg_catalog', 'information_schema')
  AND n_distinct > 100  -- 高基數列適合索引
ORDER BY n_distinct DESC;
```

### 覆蓋索引 (Covering Index)

**PostgreSQL INCLUDE 索引**:

```sql
-- 創建覆蓋索引
CREATE INDEX ix_workflows_owner_id_status_covering
ON workflow.workflows (owner_id, status)
INCLUDE (name, created_at);

-- 查詢可以直接從索引獲取所有數據 (Index-Only Scan)
SELECT name, status, created_at
FROM workflow.workflows
WHERE owner_id = 42 AND status = 'Published';
```

**EF Core 配置**:

```csharp
public sealed class WorkflowConfiguration : IEntityTypeConfiguration<Workflow>
{
    public void Configure(EntityTypeBuilder<Workflow> builder)
    {
        // EF Core 8.0+ 支持 INCLUDE 索引
        builder.HasIndex(w => new { w.OwnerId, w.Status })
            .HasDatabaseName("ix_workflows_owner_id_status_covering")
            .IncludeProperties(w => new { w.Name, w.CreatedAt });
    }
}
```

## 查詢計劃分析

### 啟用查詢日誌

**appsettings.Development.json**:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

**DbContext 配置**:

```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder
        .UseNpgsql(connectionString)
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging()  // 顯示參數值
        .EnableDetailedErrors();       // 詳細錯誤信息
}
```

### 使用 MiniProfiler

**安裝 NuGet 包**:

```bash
dotnet add package MiniProfiler.EntityFrameworkCore
dotnet add package MiniProfiler.AspNetCore.Mvc
```

**配置**:

```csharp
// Program.cs
builder.Services.AddMiniProfiler(options =>
{
    options.RouteBasePath = "/profiler";
    options.ColorScheme = ColorScheme.Auto;
}).AddEntityFramework();

var app = builder.Build();

app.UseMiniProfiler();
```

**查看性能分析**: 訪問 `/profiler/results-index` 查看查詢性能。

### PostgreSQL EXPLAIN ANALYZE

**手動分析查詢計劃**:

```sql
EXPLAIN ANALYZE
SELECT w.*, u.*
FROM workflow.workflows w
INNER JOIN identity.users u ON w.owner_id = u.id
WHERE w.status = 'Published'
ORDER BY w.created_at DESC
LIMIT 10;

-- 輸出示例:
-- Limit  (cost=0.42..23.45 rows=10 width=...)
--   ->  Nested Loop  (cost=0.42..1150.23 rows=500 width=...)
--         ->  Index Scan using ix_workflows_status on workflows w  (cost=0.28..300.12 rows=500 width=...)
--               Index Cond: (status = 'Published'::text)
--         ->  Index Scan using pk_users on users u  (cost=0.14..1.70 rows=1 width=...)
--               Index Cond: (id = w.owner_id)
-- Planning Time: 0.234 ms
-- Execution Time: 1.567 ms
```

### EF Core 查詢標籤

**添加查詢標籤便於分析**:

```csharp
var workflows = await context.Workflows
    .TagWith("GetPublishedWorkflowsForDashboard")
    .Where(w => w.Status == WorkflowStatus.Published)
    .OrderByDescending(w => w.CreatedAt)
    .Take(10)
    .ToListAsync();

// 生成的 SQL:
// -- GetPublishedWorkflowsForDashboard
// SELECT ... FROM workflow.workflows
// WHERE status = 'Published'
// ORDER BY created_at DESC
// LIMIT 10;
```

## 緩存策略

### 查詢結果緩存

**使用 IMemoryCache**:

```csharp
public async Task<List<WorkflowDto>> GetPublishedWorkflowsCachedAsync(
    CancellationToken cancellationToken = default)
{
    const string cacheKey = "published_workflows";

    return await cache.GetOrCreateAsync(cacheKey, async entry =>
    {
        entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
        entry.SetSlidingExpiration(TimeSpan.FromMinutes(2));

        return await context.Workflows
            .AsNoTracking()
            .Where(w => w.Status == WorkflowStatus.Published)
            .Select(w => new WorkflowDto { ... })
            .ToListAsync(cancellationToken);
    });
}
```

### 分佈式緩存 (Redis)

```csharp
public async Task<WorkflowDto?> GetWorkflowCachedAsync(
    int id,
    CancellationToken cancellationToken = default)
{
    var cacheKey = $"workflow:{id}";

    // 嘗試從 Redis 獲取
    var cachedData = await distributedCache.GetStringAsync(cacheKey, cancellationToken);
    if (cachedData != null)
    {
        return JsonSerializer.Deserialize<WorkflowDto>(cachedData);
    }

    // 從數據庫查詢
    var workflow = await context.Workflows
        .AsNoTracking()
        .Where(w => w.Id == id)
        .Select(w => new WorkflowDto { ... })
        .FirstOrDefaultAsync(cancellationToken);

    if (workflow != null)
    {
        // 緩存到 Redis
        var serialized = JsonSerializer.Serialize(workflow);
        await distributedCache.SetStringAsync(
            cacheKey,
            serialized,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            },
            cancellationToken);
    }

    return workflow;
}
```

## 性能監控

### 慢查詢日誌

**PostgreSQL 配置**:

```sql
-- postgresql.conf
log_min_duration_statement = 1000  -- 記錄超過 1 秒的查詢
log_statement = 'all'
log_duration = on

-- 查看慢查詢
SELECT
    query,
    calls,
    total_exec_time,
    mean_exec_time,
    max_exec_time
FROM pg_stat_statements
WHERE mean_exec_time > 1000  -- 平均執行時間超過 1 秒
ORDER BY mean_exec_time DESC
LIMIT 20;
```

### Application Insights / OpenTelemetry

```csharp
// Program.cs
builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString);
    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
});

// 自動追蹤 EF Core 查詢性能
```

## 檢查清單

### 查詢優化
- [ ] 識別和解決 N+1 查詢問題
- [ ] 使用 AsNoTracking 用於只讀查詢
- [ ] 實現高效分頁策略
- [ ] 使用 Projection 減少數據傳輸
- [ ] 配置 Split Query 避免笛卡爾積

### 批量操作
- [ ] 使用 AddRange/RemoveRange
- [ ] 使用 ExecuteUpdate/ExecuteDelete
- [ ] 避免循環內的 SaveChanges

### 索引策略
- [ ] 分析查詢計劃識別缺失索引
- [ ] 創建覆蓋索引
- [ ] 移除未使用的索引
- [ ] 定期重建索引

### 監控與分析
- [ ] 啟用查詢日誌
- [ ] 配置 MiniProfiler
- [ ] 使用 EXPLAIN ANALYZE 分析慢查詢
- [ ] 監控數據庫性能指標

## 相關文檔

- [數據庫設計原則](./database-design-principles.md)
- [Entity Framework Core 配置](./entity-framework-core-configuration.md)
- [數據庫遷移策略](./database-migration-strategy.md)
- [Phase 10: 監控與運維](../10-monitoring-operations/README.md)

## 參考資源

### 官方文檔
- [EF Core Performance](https://learn.microsoft.com/en-us/ef/core/performance/)
- [PostgreSQL Performance Tips](https://wiki.postgresql.org/wiki/Performance_Optimization)
- [Query Performance Best Practices](https://learn.microsoft.com/en-us/ef/core/performance/efficient-querying)

### 工具
- [MiniProfiler](https://miniprofiler.com/)
- [EF Core Bulk Extensions](https://github.com/borisdj/EFCore.BulkExtensions)

---

**文檔維護**: Database Team
**最後更新**: 2025-01-01
**狀態**: ✅ 完整
