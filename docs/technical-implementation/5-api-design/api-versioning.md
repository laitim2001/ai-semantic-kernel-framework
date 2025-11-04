# API 版本控制策略 (API Versioning Strategy)

## 版本信息
- **標準版本**: v1.0.0
- **適用框架**: ASP.NET Core 8.0 with Asp.Versioning 8.0
- **推薦策略**: URL Path Versioning
- **最後更新**: 2025-01-01

## 目錄

1. [版本控制原則](#版本控制原則)
2. [版本控制方法](#版本控制方法)
3. [URL Path Versioning 實現](#url-path-versioning-實現)
4. [Header Versioning 實現](#header-versioning-實現)
5. [Media Type Versioning 實現](#media-type-versioning-實現)
6. [版本生命週期管理](#版本生命週期管理)
7. [向後兼容性](#向後兼容性)
8. [API 棄用策略](#api-棄用策略)
9. [版本遷移指南](#版本遷移指南)
10. [ASP.NET Core 完整實現](#aspnet-core-完整實現)

---

## 版本控制原則

### 為什麼需要 API 版本控制?

1. **向後兼容性**: 允許舊客戶端繼續使用已有 API,同時為新客戶端提供改進功能
2. **獨立演進**: API 和客戶端可以獨立升級,無需強制同步
3. **穩定性保證**: 為生產客戶端提供穩定的 API 契約
4. **漸進式遷移**: 允許客戶端按自己的節奏遷移到新版本

### 何時需要新版本?

#### ✅ 需要 Breaking Changes (破壞性變更)

以下變更**必須**引入新版本:

1. **移除端點或資源**
   ```http
   # v1: 存在
   GET /api/v1/workflows/{id}/summary

   # v2: 移除 (Breaking)
   # 端點不再存在
   ```

2. **修改請求/響應結構**
   ```json
   // v1 Response
   {
     "id": 123,
     "name": "Workflow",
     "created": "2025-01-01"  // 字符串格式
   }

   // v2 Response (Breaking)
   {
     "id": 123,
     "name": "Workflow",
     "createdAt": 1704067200  // Unix timestamp (Breaking)
   }
   ```

3. **移除或重命名欄位**
   ```json
   // v1
   { "name": "Workflow" }

   // v2 (Breaking)
   { "title": "Workflow" }  // 'name' 重命名為 'title'
   ```

4. **更改 HTTP 狀態碼**
   ```http
   # v1: 返回 200
   DELETE /api/v1/workflows/123 → 200 OK

   # v2: 返回 204 (Breaking)
   DELETE /api/v2/workflows/123 → 204 No Content
   ```

5. **更改資源 URL 結構**
   ```http
   # v1
   GET /api/v1/workflows/{id}/nodes

   # v2 (Breaking)
   GET /api/v2/workflow-nodes?workflowId={id}
   ```

#### ✅ 非 Breaking Changes (無需新版本)

以下變更可以在**同一版本**中進行:

1. **添加新端點**
   ```http
   # 現有
   GET /api/v1/workflows

   # 新增 (Not Breaking)
   POST /api/v1/workflows/bulk-delete
   ```

2. **添加可選欄位**
   ```json
   // 原有
   {
     "name": "Workflow",
     "description": "Description"
   }

   // 添加可選欄位 (Not Breaking)
   {
     "name": "Workflow",
     "description": "Description",
     "tags": ["ai", "automation"]  // 新增,但可選
   }
   ```

3. **添加可選查詢參數**
   ```http
   # 原有
   GET /api/v1/workflows?page=1&pageSize=20

   # 新增 (Not Breaking)
   GET /api/v1/workflows?page=1&pageSize=20&status=published&tags=ai
   ```

4. **更寬鬆的驗證規則**
   ```csharp
   // v1: 必填
   RuleFor(x => x.Description).NotEmpty();

   // v1.1: 改為可選 (Not Breaking)
   RuleFor(x => x.Description).MaximumLength(2000);
   ```

5. **性能改進或 Bug 修復**
   - 不改變行為的性能優化
   - 修復明顯錯誤的行為

### 語義化版本控制 (Semantic Versioning)

遵循 [Semantic Versioning 2.0.0](https://semver.org/)

**格式**: `MAJOR.MINOR.PATCH`

```
v1.2.3
│ │ │
│ │ └─ PATCH: Bug 修復,不影響 API 契約
│ └─── MINOR: 添加功能,向後兼容
└───── MAJOR: Breaking Changes,不向後兼容
```

**示例**:

- `v1.0.0` → `v1.0.1`: Bug 修復
- `v1.0.1` → `v1.1.0`: 添加新端點,向後兼容
- `v1.1.0` → `v2.0.0`: Breaking Changes

**API 版本簡化**: 通常只使用 **MAJOR** 版本

```
/api/v1/workflows  # Major version 1
/api/v2/workflows  # Major version 2
```

---

## 版本控制方法

### 對比表

| 方法 | 示例 | 優點 | 缺點 | 推薦度 |
|------|------|------|------|--------|
| **URL Path** | `/api/v1/workflows` | ✅ 清晰可見<br>✅ 易於測試<br>✅ 緩存友好 | ❌ URL 變更 | ⭐⭐⭐⭐⭐ |
| **Query String** | `/api/workflows?version=1` | ✅ 保持 URL 穩定 | ❌ 容易被忽略<br>❌ 緩存複雜 | ⭐⭐ |
| **Header** | `X-API-Version: 1` | ✅ URL 保持穩定<br>✅ 靈活 | ❌ 不可見<br>❌ 測試困難 | ⭐⭐⭐ |
| **Media Type** | `Accept: application/vnd.aiworkflow.v1+json` | ✅ RESTful 純粹 | ❌ 複雜<br>❌ 客戶端支持差 | ⭐⭐ |

### 項目選擇: URL Path Versioning (主要) + Header Versioning (備選)

**原因**:
- URL Path 最直觀和常用
- Header Versioning 作為備選方案提供靈活性
- 兩者可以共存,優先使用 URL Path

---

## URL Path Versioning 實現

### URL 格式

```
/api/v{major}/resource
```

**示例**:

```http
GET  /api/v1/workflows
POST /api/v1/workflows
GET  /api/v1/workflows/123
PUT  /api/v1/workflows/123

GET  /api/v2/workflows
POST /api/v2/workflows
```

### ASP.NET Core 配置

#### 1. 安裝 NuGet 包

```xml
<PackageReference Include="Asp.Versioning.Http" Version="8.0.0" />
<PackageReference Include="Asp.Versioning.Mvc.ApiExplorer" Version="8.0.0" />
```

#### 2. 註冊服務

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioning(options =>
{
    // 默認版本
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;

    // 在響應頭中報告支持的版本
    options.ReportApiVersions = true;

    // 版本讀取器 (支持多種方式)
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),      // URL Path: /api/v1/workflows
        new HeaderApiVersionReader("X-API-Version")  // Header: X-API-Version: 1.0
    );
})
.AddApiExplorer(options =>
{
    // API Explorer 配置 (用於 Swagger)
    options.GroupNameFormat = "'v'VVV";  // v1, v2
    options.SubstituteApiVersionInUrl = true;
});

var app = builder.Build();
```

#### 3. 定義版本化端點 (Minimal API)

```csharp
// V1 Endpoints
var v1Group = app.MapGroup("/api/v1/workflows")
    .HasApiVersion(new ApiVersion(1, 0))
    .WithTags("Workflows V1");

v1Group.MapGet("/", async (IMediator mediator) =>
{
    var query = new GetWorkflowsQueryV1();
    var result = await mediator.Send(query);
    return Results.Ok(result.Value);
})
.WithName("GetWorkflowsV1");

v1Group.MapGet("/{id:int}", async (int id, IMediator mediator) =>
{
    var query = new GetWorkflowByIdQueryV1(id);
    var result = await mediator.Send(query);
    return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound();
})
.WithName("GetWorkflowByIdV1");

// V2 Endpoints
var v2Group = app.MapGroup("/api/v2/workflows")
    .HasApiVersion(new ApiVersion(2, 0))
    .WithTags("Workflows V2");

v2Group.MapGet("/", async (IMediator mediator) =>
{
    var query = new GetWorkflowsQueryV2();
    var result = await mediator.Send(query);
    return Results.Ok(result.Value);
})
.WithName("GetWorkflowsV2");

v2Group.MapGet("/{id:int}", async (int id, IMediator mediator) =>
{
    var query = new GetWorkflowByIdQueryV2(id);
    var result = await mediator.Send(query);
    return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound();
})
.WithName("GetWorkflowByIdV2");
```

#### 4. 使用 Controller-based API (可選)

```csharp
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/workflows")]
public sealed class WorkflowsV1Controller(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetWorkflows([FromQuery] GetWorkflowsQueryV1 query)
    {
        var result = await _mediator.Send(query);
        return Ok(result.Value);
    }

    [HttpGet("{id:int}")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetWorkflowById(int id)
    {
        var query = new GetWorkflowByIdQueryV1(id);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }
}

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/workflows")]
public sealed class WorkflowsV2Controller(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> GetWorkflows([FromQuery] GetWorkflowsQueryV2 query)
    {
        var result = await _mediator.Send(query);
        return Ok(result.Value);
    }

    [HttpGet("{id:int}")]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> GetWorkflowById(int id)
    {
        var query = new GetWorkflowByIdQueryV2(id);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }
}
```

### 響應頭

API 應返回以下響應頭:

```http
api-supported-versions: 1.0, 2.0
api-deprecated-versions: 1.0
```

---

## Header Versioning 實現

### 請求格式

```http
GET /api/workflows
X-API-Version: 1.0
```

### ASP.NET Core 配置

```csharp
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;

    // Header-based versioning
    options.ApiVersionReader = new HeaderApiVersionReader("X-API-Version");
});

// 端點定義
var workflowsGroup = app.MapGroup("/api/workflows")
    .WithTags("Workflows");

workflowsGroup.MapGet("/", async (HttpContext context, IMediator mediator) =>
{
    var apiVersion = context.GetRequestedApiVersion();

    if (apiVersion?.MajorVersion == 2)
    {
        var query = new GetWorkflowsQueryV2();
        var result = await mediator.Send(query);
        return Results.Ok(result.Value);
    }

    // 默認 v1
    var queryV1 = new GetWorkflowsQueryV1();
    var resultV1 = await mediator.Send(queryV1);
    return Results.Ok(resultV1.Value);
})
.HasApiVersion(new ApiVersion(1, 0))
.HasApiVersion(new ApiVersion(2, 0))
.WithName("GetWorkflows");
```

---

## Media Type Versioning 實現

### 請求格式

```http
GET /api/workflows
Accept: application/vnd.aiworkflow.v1+json
```

### ASP.NET Core 配置

```csharp
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;

    // Media Type versioning
    options.ApiVersionReader = new MediaTypeApiVersionReader("version");
});

// 響應格式
app.MapGet("/api/workflows", async (HttpContext context, IMediator mediator) =>
{
    var acceptHeader = context.Request.Headers.Accept.ToString();

    if (acceptHeader.Contains("vnd.aiworkflow.v2"))
    {
        context.Response.ContentType = "application/vnd.aiworkflow.v2+json";
        var query = new GetWorkflowsQueryV2();
        var result = await mediator.Send(query);
        return Results.Ok(result.Value);
    }

    context.Response.ContentType = "application/vnd.aiworkflow.v1+json";
    var queryV1 = new GetWorkflowsQueryV1();
    var resultV1 = await mediator.Send(queryV1);
    return Results.Ok(resultV1.Value);
});
```

---

## 版本生命週期管理

### 版本狀態

```
Alpha → Beta → Stable → Deprecated → Retired
```

| 狀態 | 說明 | 使用建議 |
|------|------|----------|
| **Alpha** | 實驗性版本,API 可能頻繁變更 | 僅限內部測試 |
| **Beta** | 功能完整但可能有細微調整 | 可用於測試環境 |
| **Stable** | 穩定版本,生產可用 | 推薦生產使用 |
| **Deprecated** | 已棄用,計劃移除 | 應盡快遷移 |
| **Retired** | 已移除,不再可用 | 必須遷移 |

### 版本生命週期時間表

```
┌─────────────┬──────────────┬──────────────┬──────────────┬──────────────┐
│   Release   │   Stable     │  Deprecated  │   Sunset     │   Retired    │
│   v2.0      │    (now)     │  (+6 months) │ (+12 months) │ (+18 months) │
├─────────────┼──────────────┼──────────────┼──────────────┼──────────────┤
│   v1.0      │              │   (now)      │  (+6 months) │ (+12 months) │
└─────────────┴──────────────┴──────────────┴──────────────┴──────────────┘
```

**推薦時間線**:
- **Stable Period**: 至少 12 個月
- **Deprecation Period**: 6-12 個月 (給客戶端遷移時間)
- **Sunset Notice**: 提前 3-6 個月通知
- **Total Support**: 至少 18-24 個月

### 版本棄用通知

#### 響應頭通知

```http
HTTP/1.1 200 OK
api-supported-versions: 1.0, 2.0
api-deprecated-versions: 1.0
Sunset: Sat, 01 Jul 2025 23:59:59 GMT
Link: </api/v2/workflows>; rel="successor-version"

{
  "data": [ /* ... */ ]
}
```

#### 響應體警告 (可選)

```json
{
  "data": [ /* ... */ ],
  "warnings": [
    {
      "code": "API_VERSION_DEPRECATED",
      "message": "API version 1.0 is deprecated and will be retired on July 1, 2025. Please migrate to v2.0.",
      "link": "https://docs.aiworkflow.com/api/migration/v1-to-v2"
    }
  ]
}
```

#### ASP.NET Core 實現

```csharp
// Deprecation Middleware
public sealed class ApiDeprecationMiddleware(RequestDelegate next, ILogger<ApiDeprecationMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ApiDeprecationMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        var apiVersion = context.GetRequestedApiVersion();

        if (apiVersion?.MajorVersion == 1)
        {
            // 添加 Deprecation 響應頭
            context.Response.Headers.Append("api-deprecated-versions", "1.0");
            context.Response.Headers.Append("Sunset", "Sat, 01 Jul 2025 23:59:59 GMT");
            context.Response.Headers.Append("Link", "</api/v2/workflows>; rel=\"successor-version\"");

            _logger.LogWarning("Deprecated API version {Version} accessed by {UserAgent}",
                apiVersion,
                context.Request.Headers.UserAgent);
        }

        await _next(context);
    }
}

// 註冊中間件
app.UseMiddleware<ApiDeprecationMiddleware>();
```

---

## 向後兼容性

### 兼容性原則

#### ✅ DO: 保證向後兼容

1. **添加可選欄位**
   ```json
   // v1.0
   { "name": "Workflow" }

   // v1.1 (Compatible)
   {
     "name": "Workflow",
     "description": "Optional field"  // 新增但可選
   }
   ```

2. **添加新端點**
   ```http
   # v1.0 existing
   GET /api/v1/workflows

   # v1.1 new (Compatible)
   POST /api/v1/workflows/bulk-delete
   ```

3. **放寬驗證**
   ```csharp
   // v1.0: 必填
   RuleFor(x => x.Tags).NotEmpty();

   // v1.1: 改為可選 (Compatible)
   RuleFor(x => x.Tags).MaximumLength(10).When(x => x.Tags != null);
   ```

#### ❌ DON'T: 破壞向後兼容

1. **移除或重命名欄位**
   ```json
   // v1.0
   { "name": "Workflow" }

   // v2.0 (Breaking)
   { "title": "Workflow" }  // 'name' 被移除
   ```

2. **更改數據類型**
   ```json
   // v1.0
   { "createdAt": "2025-01-01T00:00:00Z" }  // ISO 8601 string

   // v2.0 (Breaking)
   { "createdAt": 1704067200 }  // Unix timestamp
   ```

3. **收緊驗證規則**
   ```csharp
   // v1.0: 可選
   RuleFor(x => x.Description).MaximumLength(2000);

   // v1.1: 改為必填 (Breaking)
   RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
   ```

### 欄位演進策略

#### 重命名欄位

**階段 1: 添加新欄位,保留舊欄位**
```json
{
  "name": "Workflow",        // 舊欄位 (Deprecated)
  "title": "Workflow"        // 新欄位
}
```

**階段 2: 標記舊欄位為 Deprecated**
```csharp
public sealed record WorkflowDto
{
    [Obsolete("Use Title instead. This property will be removed in v2.0")]
    public string? Name { get; init; }

    public required string Title { get; init; }
}
```

**階段 3: 移除舊欄位 (新主版本)**
```json
// v2.0
{
  "title": "Workflow"  // 'name' 已移除
}
```

---

## API 棄用策略

### 棄用流程

```
1. Announce Deprecation (提前 6-12 個月)
   ↓
2. Mark as Deprecated (添加響應頭和警告)
   ↓
3. Provide Migration Guide (遷移文檔)
   ↓
4. Monitor Usage (追蹤舊版本使用情況)
   ↓
5. Sunset Date (停止支持日期)
   ↓
6. Retire API (完全移除)
```

### 棄用通知示例

#### 官方公告

```markdown
# API v1.0 Deprecation Notice

**發布日期**: 2025-01-01
**棄用日期**: 2025-07-01 (6 個月後)
**停用日期**: 2026-01-01 (12 個月後)

## 概述
API v1.0 將於 2025 年 7 月 1 日正式棄用,並於 2026 年 1 月 1 日完全停用。

## 遷移路徑
請遷移至 API v2.0,查看遷移指南: https://docs.aiworkflow.com/api/migration/v1-to-v2

## 主要變更
- Workflow 響應結構優化
- 改進的錯誤處理
- 新增批量操作端點

## 支持
如有問題,請聯繫 api-support@aiworkflow.com
```

#### 響應頭通知

```http
HTTP/1.1 200 OK
api-deprecated-versions: 1.0
Sunset: Sat, 01 Jul 2025 23:59:59 GMT
Deprecation: true
Link: </api/v2/workflows>; rel="successor-version"
```

#### 監控棄用版本使用

```csharp
public sealed class DeprecatedApiUsageMiddleware(
    RequestDelegate next,
    ILogger<DeprecatedApiUsageMiddleware> logger,
    IMetrics metrics)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var apiVersion = context.GetRequestedApiVersion();

        if (apiVersion?.MajorVersion == 1)
        {
            // 記錄使用情況
            metrics.IncrementCounter("deprecated_api_usage", new[]
            {
                new KeyValuePair<string, object?>("version", "1.0"),
                new KeyValuePair<string, object?>("endpoint", context.Request.Path),
                new KeyValuePair<string, object?>("client", context.Request.Headers.UserAgent.ToString())
            });

            logger.LogWarning(
                "Deprecated API v{Version} accessed: {Method} {Path} by {UserAgent}",
                apiVersion,
                context.Request.Method,
                context.Request.Path,
                context.Request.Headers.UserAgent
            );
        }

        await next(context);
    }
}
```

---

## 版本遷移指南

### V1 → V2 遷移示例

#### 變更摘要

| 變更類型 | V1 | V2 | 影響 |
|----------|----|----|------|
| **欄位重命名** | `name` | `title` | Breaking |
| **日期格式** | ISO 8601 字符串 | Unix timestamp | Breaking |
| **新增欄位** | - | `tags` | Compatible |
| **URL 變更** | `/api/v1/workflows` | `/api/v2/workflows` | Breaking |

#### 詳細對比

**V1 響應**:
```json
{
  "id": 123,
  "name": "Customer Automation",
  "createdAt": "2025-01-15T08:30:00Z",
  "lastModifiedAt": "2025-01-20T14:22:00Z"
}
```

**V2 響應**:
```json
{
  "id": 123,
  "title": "Customer Automation",     // 'name' → 'title'
  "createdAt": 1705308600,            // Unix timestamp
  "lastModifiedAt": 1705762920,
  "tags": ["automation", "customer"]  // 新增欄位
}
```

#### 客戶端遷移代碼

**TypeScript/React 遷移**:

```typescript
// V1 Client
interface WorkflowV1 {
  id: number
  name: string
  createdAt: string  // ISO 8601
  lastModifiedAt?: string
}

async function getWorkflowV1(id: number): Promise<WorkflowV1> {
  const response = await fetch(`/api/v1/workflows/${id}`)
  return response.json()
}

// V2 Client
interface WorkflowV2 {
  id: number
  title: string       // 重命名
  createdAt: number   // Unix timestamp
  lastModifiedAt?: number
  tags?: string[]     // 新增
}

async function getWorkflowV2(id: number): Promise<WorkflowV2> {
  const response = await fetch(`/api/v2/workflows/${id}`)
  return response.json()
}

// 適配器模式 (平滑遷移)
function adaptV1ToV2(v1Workflow: WorkflowV1): WorkflowV2 {
  return {
    id: v1Workflow.id,
    title: v1Workflow.name,  // 映射欄位名
    createdAt: new Date(v1Workflow.createdAt).getTime() / 1000,  // 轉換格式
    lastModifiedAt: v1Workflow.lastModifiedAt
      ? new Date(v1Workflow.lastModifiedAt).getTime() / 1000
      : undefined,
    tags: []  // 默認空數組
  }
}
```

#### 批量遷移腳本

```csharp
// 數據遷移 Command
public sealed class MigrateWorkflowsToV2Command : IRequest<Result<MigrationSummary>>;

public sealed class MigrateWorkflowsToV2CommandHandler(
    IApplicationDbContext context,
    ILogger<MigrateWorkflowsToV2CommandHandler> logger)
    : IRequestHandler<MigrateWorkflowsToV2Command, Result<MigrationSummary>>
{
    public async Task<Result<MigrationSummary>> Handle(
        MigrateWorkflowsToV2Command request,
        CancellationToken cancellationToken)
    {
        var workflows = await context.Workflows
            .Where(w => w.ApiVersion == 1)
            .ToListAsync(cancellationToken);

        var migrated = 0;
        var failed = 0;

        foreach (var workflow in workflows)
        {
            try
            {
                // 執行數據轉換
                workflow.ApiVersion = 2;
                workflow.LastModifiedAt = DateTime.UtcNow;

                migrated++;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to migrate workflow {WorkflowId}", workflow.Id);
                failed++;
            }
        }

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Migration completed: {Migrated} migrated, {Failed} failed", migrated, failed);

        return Result<MigrationSummary>.Success(new MigrationSummary
        {
            TotalCount = workflows.Count,
            MigratedCount = migrated,
            FailedCount = failed
        });
    }
}

public sealed record MigrationSummary
{
    public required int TotalCount { get; init; }
    public required int MigratedCount { get; init; }
    public required int FailedCount { get; init; }
}
```

---

## ASP.NET Core 完整實現

### 項目結構

```
AIWorkflow.API/
├── V1/
│   ├── Endpoints/
│   │   └── WorkflowEndpoints.cs
│   ├── Dtos/
│   │   ├── WorkflowDtoV1.cs
│   │   └── CreateWorkflowDtoV1.cs
│   └── Queries/
│       └── GetWorkflowsQueryV1.cs
├── V2/
│   ├── Endpoints/
│   │   └── WorkflowEndpoints.cs
│   ├── Dtos/
│   │   ├── WorkflowDtoV2.cs
│   │   └── CreateWorkflowDtoV2.cs
│   └── Queries/
│       └── GetWorkflowsQueryV2.cs
├── Shared/
│   └── Middleware/
│       ├── ApiDeprecationMiddleware.cs
│       └── ApiVersioningMiddleware.cs
└── Program.cs
```

### 完整 Program.cs 配置

```csharp
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;

    // 支持多種版本讀取方式
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),              // URL: /api/v1/workflows
        new HeaderApiVersionReader("X-API-Version"),   // Header: X-API-Version: 1.0
        new QueryStringApiVersionReader("api-version") // Query: ?api-version=1.0
    );
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Swagger with API Versioning
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware
app.UseMiddleware<ApiDeprecationMiddleware>();

// Swagger UI for each API version
var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
    {
        options.SwaggerEndpoint(
            $"/swagger/{description.GroupName}/swagger.json",
            description.GroupName.ToUpperInvariant()
        );
    }
});

// V1 Endpoints
var v1Group = app.MapGroup("/api/v1")
    .HasApiVersion(new ApiVersion(1, 0))
    .WithOpenApi();

v1Group.MapWorkflowEndpointsV1();

// V2 Endpoints
var v2Group = app.MapGroup("/api/v2")
    .HasApiVersion(new ApiVersion(2, 0))
    .WithOpenApi();

v2Group.MapWorkflowEndpointsV2();

app.Run();
```

### Swagger 多版本配置

```csharp
public sealed class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider = provider;

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(
                description.GroupName,
                new OpenApiInfo
                {
                    Title = $"AI Workflow API {description.ApiVersion}",
                    Version = description.ApiVersion.ToString(),
                    Description = description.IsDeprecated
                        ? "⚠️ This API version is deprecated and will be retired soon."
                        : "Current stable API version.",
                    Contact = new OpenApiContact
                    {
                        Name = "AI Workflow Support",
                        Email = "api-support@aiworkflow.com"
                    }
                }
            );
        }
    }
}
```

---

## 最佳實踐檢查清單

### 設計階段
- [ ] 選擇適合的版本控制方法 (URL Path 推薦)
- [ ] 定義版本生命週期策略
- [ ] 制定 Breaking Changes 判斷標準
- [ ] 規劃向後兼容性策略

### 實現階段
- [ ] 配置 ASP.NET Core API Versioning
- [ ] 實現版本化端點
- [ ] 添加 API 版本響應頭
- [ ] 實現 Deprecation Middleware
- [ ] 配置 Swagger 多版本文檔

### 棄用階段
- [ ] 提前 6-12 個月公告棄用
- [ ] 添加 Sunset 響應頭
- [ ] 提供詳細遷移指南
- [ ] 監控舊版本使用情況
- [ ] 主動聯繫高流量客戶

### 遷移階段
- [ ] 提供遷移腳本和工具
- [ ] 創建適配器層平滑遷移
- [ ] 執行數據遷移 (如需要)
- [ ] 驗證遷移結果
- [ ] 更新客戶端代碼

---

**文檔維護**: Tech Lead Team
**最後更新**: 2025-01-01
**狀態**: ✅ 完整
