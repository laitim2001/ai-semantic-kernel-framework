# RESTful API 設計規範 (RESTful API Design Standards)

## 版本信息
- **標準版本**: v1.0.0
- **適用框架**: ASP.NET Core 8.0
- **API 風格**: REST (Richardson Maturity Model Level 2-3)
- **最後更新**: 2025-01-01

## 目錄

1. [資源設計原則](#資源設計原則)
2. [HTTP 方法規範](#http-方法規範)
3. [URL 路徑設計](#url-路徑設計)
4. [查詢參數設計](#查詢參數設計)
5. [請求/響應格式](#請求響應格式)
6. [HTTP 狀態碼](#http-狀態碼)
7. [分頁與過濾](#分頁與過濾)
8. [排序與搜索](#排序與搜索)
9. [批量操作](#批量操作)
10. [冪等性設計](#冪等性設計)
11. [Content Negotiation](#content-negotiation)
12. [HATEOAS](#hateoas)
13. [實現示例](#實現示例)

---

## 資源設計原則

### Resource-Oriented Architecture

REST API 應該以**資源**(Resources)為中心進行設計,而不是以**操作**(Actions)為中心。

#### 核心概念

**資源 (Resource)**:
- 資源是 API 中的核心抽象概念
- 每個資源都有一個唯一標識符 (Resource ID)
- 資源使用名詞表示,通常使用複數形式
- 資源可以包含子資源形成層次結構

**資源示例**:
```
Workflows        # 工作流集合
Workflow         # 單個工作流
WorkflowNodes    # 工作流節點集合
AgentPersonas    # AI 代理角色集合
Users            # 用戶集合
```

### 設計規則

#### ✅ DO: 使用名詞而非動詞

```http
# 正確 - 資源導向
GET    /api/v1/workflows
POST   /api/v1/workflows
GET    /api/v1/workflows/{id}
PUT    /api/v1/workflows/{id}
DELETE /api/v1/workflows/{id}
```

```http
# 錯誤 - 操作導向 (RPC 風格)
POST   /api/v1/createWorkflow
POST   /api/v1/getWorkflows
POST   /api/v1/updateWorkflow
POST   /api/v1/deleteWorkflow
```

#### ✅ DO: 使用複數名詞

```http
# 正確
GET /api/v1/workflows
GET /api/v1/users
GET /api/v1/agent-personas
```

```http
# 錯誤
GET /api/v1/workflow      # 單數形式
GET /api/v1/user
GET /api/v1/agentPersona  # 也應該使用複數
```

#### ✅ DO: 使用層次結構表達資源關係

```http
# 子資源設計
GET /api/v1/workflows/{workflowId}/nodes
GET /api/v1/workflows/{workflowId}/nodes/{nodeId}
GET /api/v1/workflows/{workflowId}/executions
GET /api/v1/users/{userId}/workflows
```

#### ❌ DON'T: 嵌套超過 2-3 層

```http
# 正確 - 適度嵌套
GET /api/v1/workflows/{id}/nodes/{nodeId}

# 避免 - 過度嵌套
GET /api/v1/organizations/{orgId}/teams/{teamId}/users/{userId}/workflows/{workflowId}/nodes/{nodeId}

# 替代方案 - 使用查詢參數
GET /api/v1/workflows/{workflowId}/nodes?userId={userId}&teamId={teamId}
```

---

## HTTP 方法規範

### 標準方法語義

| HTTP 方法 | 語義 | 冪等性 | 安全性 | 響應碼 |
|-----------|------|--------|--------|--------|
| **GET** | 獲取資源 | ✅ 是 | ✅ 是 | 200, 404 |
| **POST** | 創建資源 | ❌ 否 | ❌ 否 | 201, 400, 409 |
| **PUT** | 完整替換資源 | ✅ 是 | ❌ 否 | 200, 204, 404 |
| **PATCH** | 部分更新資源 | ❌ 否* | ❌ 否 | 200, 204, 404 |
| **DELETE** | 刪除資源 | ✅ 是 | ❌ 否 | 204, 404 |
| **HEAD** | 獲取資源元數據 | ✅ 是 | ✅ 是 | 200, 404 |
| **OPTIONS** | 獲取可用操作 | ✅ 是 | ✅ 是 | 200 |

*註: PATCH 可設計為冪等,取決於實現方式

### GET - 獲取資源

**用途**: 檢索資源或資源集合

**特性**:
- 冪等: 多次調用返回相同結果
- 安全: 不應產生任何副作用
- 可緩存

**示例**:

```http
# 獲取資源列表
GET /api/v1/workflows
Accept: application/json

Response: 200 OK
{
  "data": [
    { "id": 1, "name": "Customer Support Automation" },
    { "id": 2, "name": "Data Processing Pipeline" }
  ],
  "pagination": {
    "currentPage": 1,
    "pageSize": 20,
    "totalCount": 45
  }
}
```

```http
# 獲取單個資源
GET /api/v1/workflows/1
Accept: application/json

Response: 200 OK
{
  "id": 1,
  "name": "Customer Support Automation",
  "description": "Automated customer inquiry handling",
  "status": "published",
  "ownerId": 42,
  "createdAt": "2025-01-15T08:30:00Z",
  "lastModifiedAt": "2025-01-20T14:22:00Z"
}
```

```http
# 資源不存在
GET /api/v1/workflows/999

Response: 404 Not Found
{
  "type": "https://api.aiworkflow.com/errors/not-found",
  "title": "Resource not found",
  "status": 404,
  "detail": "Workflow with ID 999 was not found.",
  "traceId": "00-abc123-def456-00"
}
```

**ASP.NET Core 實現**:

```csharp
// Minimal API
app.MapGet("/api/v1/workflows", async (
    [FromQuery] int page,
    [FromQuery] int pageSize,
    IMediator mediator) =>
{
    var query = new GetWorkflowsQuery(page, pageSize);
    var result = await mediator.Send(query);

    return result.IsSuccess
        ? Results.Ok(result.Value)
        : Results.Problem(result.Error);
})
.WithName("GetWorkflows")
.WithTags("Workflows")
.Produces<WorkflowListResponse>(200)
.Produces<ProblemDetails>(400);

app.MapGet("/api/v1/workflows/{id:int}", async (
    int id,
    IMediator mediator) =>
{
    var query = new GetWorkflowByIdQuery(id);
    var result = await mediator.Send(query);

    return result.IsSuccess
        ? Results.Ok(result.Value)
        : Results.NotFound(new ProblemDetails
        {
            Title = "Resource not found",
            Status = 404,
            Detail = $"Workflow with ID {id} was not found."
        });
})
.WithName("GetWorkflowById")
.WithTags("Workflows")
.Produces<WorkflowDto>(200)
.Produces<ProblemDetails>(404);
```

### POST - 創建資源

**用途**: 創建新資源

**特性**:
- 非冪等: 多次調用會創建多個資源
- 不安全: 產生副作用
- 不可緩存

**響應**:
- **201 Created**: 成功創建資源
- **Location Header**: 指向新創建的資源 URL
- **Response Body**: 包含完整的資源表示 (包括服務器生成的欄位如 ID, timestamp)

**示例**:

```http
POST /api/v1/workflows
Content-Type: application/json
Authorization: Bearer {token}

{
  "name": "New Automation Workflow",
  "description": "Handles automated data processing",
  "templateId": null
}

Response: 201 Created
Location: /api/v1/workflows/123
Content-Type: application/json

{
  "id": 123,
  "name": "New Automation Workflow",
  "description": "Handles automated data processing",
  "status": "draft",
  "ownerId": 42,
  "createdAt": "2025-01-25T10:00:00Z",
  "lastModifiedAt": null
}
```

**驗證錯誤**:

```http
POST /api/v1/workflows
Content-Type: application/json

{
  "name": "",
  "description": "Short"
}

Response: 400 Bad Request
Content-Type: application/problem+json

{
  "type": "https://api.aiworkflow.com/errors/validation-error",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "traceId": "00-xyz789-abc123-00",
  "errors": {
    "Name": ["The Name field is required."],
    "Description": ["The Description must be at least 10 characters."]
  }
}
```

**ASP.NET Core 實現**:

```csharp
app.MapPost("/api/v1/workflows", async (
    [FromBody] CreateWorkflowDto dto,
    IMediator mediator,
    LinkGenerator linkGenerator,
    HttpContext httpContext) =>
{
    var command = new CreateWorkflowCommand(
        dto.Name,
        dto.Description,
        dto.TemplateId
    );

    var result = await mediator.Send(command);

    if (!result.IsSuccess)
        return Results.BadRequest(ProblemDetailsFactory.CreateValidationProblem(result.Errors));

    var workflowId = result.Value;
    var location = linkGenerator.GetPathByName("GetWorkflowById", new { id = workflowId });

    // 獲取完整的工作流資料返回
    var getQuery = new GetWorkflowByIdQuery(workflowId);
    var workflowResult = await mediator.Send(getQuery);

    return Results.Created(location, workflowResult.Value);
})
.WithName("CreateWorkflow")
.WithTags("Workflows")
.Produces<WorkflowDto>(201)
.Produces<ValidationProblemDetails>(400)
.RequireAuthorization();
```

**驗證器實現 (FluentValidation)**:

```csharp
public sealed class CreateWorkflowDtoValidator : AbstractValidator<CreateWorkflowDto>
{
    public CreateWorkflowDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("The Name field is required.")
            .MaximumLength(200)
            .WithMessage("The Name must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MinimumLength(10)
            .When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage("The Description must be at least 10 characters.")
            .MaximumLength(2000)
            .WithMessage("The Description must not exceed 2000 characters.");
    }
}
```

### PUT - 完整替換資源

**用途**: 完全替換現有資源的所有欄位

**特性**:
- 冪等: 多次使用相同數據調用產生相同結果
- 不安全: 產生副作用
- 必須提供資源的完整表示

**響應**:
- **200 OK**: 更新成功並返回更新後的資源
- **204 No Content**: 更新成功但不返回內容
- **404 Not Found**: 資源不存在

**示例**:

```http
PUT /api/v1/workflows/123
Content-Type: application/json
Authorization: Bearer {token}

{
  "name": "Updated Workflow Name",
  "description": "Updated description with new details",
  "status": "published"
}

Response: 200 OK
Content-Type: application/json

{
  "id": 123,
  "name": "Updated Workflow Name",
  "description": "Updated description with new details",
  "status": "published",
  "ownerId": 42,
  "createdAt": "2025-01-25T10:00:00Z",
  "lastModifiedAt": "2025-01-25T11:30:00Z"
}
```

**ASP.NET Core 實現**:

```csharp
app.MapPut("/api/v1/workflows/{id:int}", async (
    int id,
    [FromBody] UpdateWorkflowDto dto,
    IMediator mediator) =>
{
    var command = new UpdateWorkflowCommand(
        id,
        dto.Name,
        dto.Description,
        dto.Status
    );

    var result = await mediator.Send(command);

    if (!result.IsSuccess)
    {
        return result.Error.Code == "NotFound"
            ? Results.NotFound(ProblemDetailsFactory.CreateNotFound(id))
            : Results.BadRequest(ProblemDetailsFactory.CreateValidationProblem(result.Errors));
    }

    return Results.Ok(result.Value);
})
.WithName("UpdateWorkflow")
.WithTags("Workflows")
.Produces<WorkflowDto>(200)
.Produces<ProblemDetails>(404)
.Produces<ValidationProblemDetails>(400)
.RequireAuthorization();
```

### PATCH - 部分更新資源

**用途**: 僅更新資源的部分欄位

**特性**:
- 可設計為冪等 (使用 JSON Patch)
- 不安全: 產生副作用
- 只需提供要更新的欄位

**兩種實現方式**:

#### 1. JSON Merge Patch (RFC 7396) - 推薦用於簡單場景

```http
PATCH /api/v1/workflows/123
Content-Type: application/merge-patch+json
Authorization: Bearer {token}

{
  "name": "Partially Updated Workflow"
}

Response: 200 OK
{
  "id": 123,
  "name": "Partially Updated Workflow",
  "description": "Original description unchanged",
  "status": "published",
  "ownerId": 42,
  "lastModifiedAt": "2025-01-25T12:00:00Z"
}
```

**ASP.NET Core 實現**:

```csharp
app.MapPatch("/api/v1/workflows/{id:int}", async (
    int id,
    [FromBody] JsonElement patchDocument,
    IMediator mediator) =>
{
    var command = new PatchWorkflowCommand(id, patchDocument);
    var result = await mediator.Send(command);

    return result.IsSuccess
        ? Results.Ok(result.Value)
        : Results.NotFound();
})
.WithName("PatchWorkflow")
.WithTags("Workflows")
.Produces<WorkflowDto>(200)
.Produces<ProblemDetails>(404)
.RequireAuthorization();
```

#### 2. JSON Patch (RFC 6902) - 推薦用於複雜場景

```http
PATCH /api/v1/workflows/123
Content-Type: application/json-patch+json
Authorization: Bearer {token}

[
  { "op": "replace", "path": "/name", "value": "New Workflow Name" },
  { "op": "replace", "path": "/description", "value": "Updated description" },
  { "op": "add", "path": "/tags/-", "value": "automation" }
]

Response: 200 OK
```

**ASP.NET Core 實現 (使用 Microsoft.AspNetCore.JsonPatch)**:

```csharp
app.MapPatch("/api/v1/workflows/{id:int}", async (
    int id,
    [FromBody] JsonPatchDocument<WorkflowDto> patchDoc,
    IMediator mediator,
    IValidator<WorkflowDto> validator) =>
{
    // 獲取當前資源
    var getQuery = new GetWorkflowByIdQuery(id);
    var currentResult = await mediator.Send(getQuery);

    if (!currentResult.IsSuccess)
        return Results.NotFound();

    var workflowDto = currentResult.Value;

    // 應用 patch 操作
    patchDoc.ApplyTo(workflowDto);

    // 驗證更新後的資源
    var validationResult = await validator.ValidateAsync(workflowDto);
    if (!validationResult.IsValid)
        return Results.BadRequest(validationResult.Errors);

    // 執行更新
    var updateCommand = new UpdateWorkflowCommand(id, workflowDto);
    var updateResult = await mediator.Send(updateCommand);

    return Results.Ok(updateResult.Value);
})
.WithName("PatchWorkflowJsonPatch")
.WithTags("Workflows")
.Accepts<JsonPatchDocument<WorkflowDto>>("application/json-patch+json")
.Produces<WorkflowDto>(200)
.Produces<ProblemDetails>(404)
.Produces<ValidationProblemDetails>(400)
.RequireAuthorization();
```

### DELETE - 刪除資源

**用途**: 刪除指定資源

**特性**:
- 冪等: 多次刪除同一資源應該是安全的
- 不安全: 產生副作用

**響應**:
- **204 No Content**: 刪除成功,不返回內容
- **200 OK**: 刪除成功並返回已刪除資源的信息
- **404 Not Found**: 資源不存在
- **409 Conflict**: 資源存在依賴關係無法刪除

**示例**:

```http
DELETE /api/v1/workflows/123
Authorization: Bearer {token}

Response: 204 No Content
```

```http
# 資源不存在 (冪等性 - 也可返回 204)
DELETE /api/v1/workflows/999

Response: 404 Not Found
{
  "type": "https://api.aiworkflow.com/errors/not-found",
  "title": "Resource not found",
  "status": 404,
  "detail": "Workflow with ID 999 was not found."
}
```

```http
# 存在依賴關係
DELETE /api/v1/workflows/123

Response: 409 Conflict
{
  "type": "https://api.aiworkflow.com/errors/conflict",
  "title": "Cannot delete resource",
  "status": 409,
  "detail": "Workflow 123 cannot be deleted because it has active executions."
}
```

**ASP.NET Core 實現**:

```csharp
app.MapDelete("/api/v1/workflows/{id:int}", async (
    int id,
    IMediator mediator) =>
{
    var command = new DeleteWorkflowCommand(id);
    var result = await mediator.Send(command);

    if (!result.IsSuccess)
    {
        return result.Error.Code switch
        {
            "NotFound" => Results.NotFound(ProblemDetailsFactory.CreateNotFound(id)),
            "Conflict" => Results.Conflict(new ProblemDetails
            {
                Title = "Cannot delete resource",
                Status = 409,
                Detail = result.Error.Message
            }),
            _ => Results.Problem()
        };
    }

    return Results.NoContent();
})
.WithName("DeleteWorkflow")
.WithTags("Workflows")
.Produces(204)
.Produces<ProblemDetails>(404)
.Produces<ProblemDetails>(409)
.RequireAuthorization();
```

**Command Handler 實現**:

```csharp
public sealed class DeleteWorkflowCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService,
    ILogger<DeleteWorkflowCommandHandler> logger)
    : IRequestHandler<DeleteWorkflowCommand, Result>
{
    public async Task<Result> Handle(
        DeleteWorkflowCommand request,
        CancellationToken cancellationToken)
    {
        var workflow = await context.Workflows
            .Include(w => w.Executions)
            .FirstOrDefaultAsync(w => w.Id == request.Id, cancellationToken);

        if (workflow is null)
            return Result.Failure(Error.NotFound("Workflow not found"));

        // 檢查所有權
        var userId = currentUserService.UserId;
        if (workflow.OwnerId != userId && !currentUserService.IsAdmin)
            return Result.Failure(Error.Forbidden("You don't have permission to delete this workflow"));

        // 檢查依賴關係
        if (workflow.Executions.Any(e => e.Status == ExecutionStatus.Running))
            return Result.Failure(Error.Conflict("Workflow has active executions and cannot be deleted"));

        context.Workflows.Remove(workflow);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Workflow {WorkflowId} deleted by user {UserId}", request.Id, userId);

        return Result.Success();
    }
}
```

---

## URL 路徑設計

### 命名約定

#### ✅ DO: 使用小寫 kebab-case

```http
GET /api/v1/workflows
GET /api/v1/agent-personas
GET /api/v1/workflow-templates
```

#### ❌ DON'T: 使用 camelCase, PascalCase, 或 snake_case

```http
# 避免
GET /api/v1/workflowTemplates  # camelCase
GET /api/v1/WorkflowTemplates  # PascalCase
GET /api/v1/workflow_templates # snake_case
```

### 路徑結構

```
/api/{version}/{resource}
/api/{version}/{resource}/{id}
/api/{version}/{resource}/{id}/{sub-resource}
/api/{version}/{resource}/{id}/{sub-resource}/{sub-id}
```

**示例**:

```http
/api/v1/workflows
/api/v1/workflows/123
/api/v1/workflows/123/nodes
/api/v1/workflows/123/nodes/node-456
/api/v1/workflows/123/executions
/api/v1/users/42/workflows
```

### 特殊端點

某些操作不適合標準 CRUD 模式,可使用**控制器風格**端點:

```http
# 資源操作
POST /api/v1/workflows/123/publish
POST /api/v1/workflows/123/execute
POST /api/v1/workflows/123/duplicate

# 批量操作
POST /api/v1/workflows/bulk-delete
POST /api/v1/workflows/bulk-publish

# 搜索和過濾
POST /api/v1/workflows/search
GET  /api/v1/workflows/search?q=automation&status=published
```

**ASP.NET Core 實現**:

```csharp
// 發布工作流
app.MapPost("/api/v1/workflows/{id:int}/publish", async (
    int id,
    IMediator mediator) =>
{
    var command = new PublishWorkflowCommand(id);
    var result = await mediator.Send(command);

    return result.IsSuccess
        ? Results.Ok(result.Value)
        : Results.NotFound();
})
.WithName("PublishWorkflow")
.WithTags("Workflows")
.Produces<WorkflowDto>(200)
.Produces<ProblemDetails>(404)
.RequireAuthorization();

// 執行工作流
app.MapPost("/api/v1/workflows/{id:int}/execute", async (
    int id,
    [FromBody] ExecuteWorkflowDto dto,
    IMediator mediator) =>
{
    var command = new ExecuteWorkflowCommand(id, dto.InputData);
    var result = await mediator.Send(command);

    return result.IsSuccess
        ? Results.Accepted($"/api/v1/executions/{result.Value.ExecutionId}", result.Value)
        : Results.BadRequest(result.Error);
})
.WithName("ExecuteWorkflow")
.WithTags("Workflows")
.Produces<ExecutionResultDto>(202)
.Produces<ProblemDetails>(400)
.RequireAuthorization();
```

---

## 查詢參數設計

### 命名約定

使用 **camelCase** 命名查詢參數:

```http
GET /api/v1/workflows?page=1&pageSize=20&sortBy=createdAt&sortOrder=desc
```

### 常見查詢參數

| 參數 | 用途 | 示例 |
|------|------|------|
| `page` | 當前頁碼 (從 1 開始) | `page=1` |
| `pageSize` | 每頁項目數 | `pageSize=20` |
| `sortBy` | 排序欄位 | `sortBy=createdAt` |
| `sortOrder` | 排序方向 | `sortOrder=desc` |
| `search` | 全文搜索 | `search=automation` |
| `filter` | 過濾條件 | `filter=status:published` |
| `fields` | 欄位選擇 | `fields=id,name,status` |
| `include` | 包含關聯資源 | `include=owner,nodes` |

### 過濾參數

```http
# 單個條件
GET /api/v1/workflows?status=published

# 多個條件 (AND 邏輯)
GET /api/v1/workflows?status=published&ownerId=42

# 範圍查詢
GET /api/v1/workflows?createdAfter=2025-01-01&createdBefore=2025-01-31

# 列表查詢 (OR 邏輯)
GET /api/v1/workflows?status=published,draft&tags=ai,automation
```

### ASP.NET Core 實現

```csharp
public sealed record GetWorkflowsQuery(
    int Page = 1,
    int PageSize = 20,
    string? SortBy = "createdAt",
    string? SortOrder = "desc",
    string? Search = null,
    string? Status = null,
    int? OwnerId = null,
    DateTime? CreatedAfter = null,
    DateTime? CreatedBefore = null,
    string[]? Tags = null
) : IRequest<Result<WorkflowListResponse>>;

app.MapGet("/api/v1/workflows", async (
    [AsParameters] GetWorkflowsQuery query,
    IMediator mediator) =>
{
    var result = await mediator.Send(query);
    return Results.Ok(result.Value);
})
.WithName("GetWorkflows")
.WithTags("Workflows");
```

---

## 請求/響應格式

### Content-Type

**請求**:
```http
Content-Type: application/json; charset=utf-8
```

**響應**:
```http
Content-Type: application/json; charset=utf-8
```

### 請求體格式

```json
{
  "name": "Workflow Name",
  "description": "Workflow description",
  "status": "draft",
  "nodes": [
    {
      "id": "node-1",
      "type": "prompt",
      "position": { "x": 100, "y": 100 },
      "data": { "template": "Hello {{name}}" }
    }
  ]
}
```

### 響應體格式

#### 成功響應 (單個資源)

```json
{
  "id": 123,
  "name": "Workflow Name",
  "description": "Workflow description",
  "status": "published",
  "ownerId": 42,
  "createdAt": "2025-01-25T10:00:00Z",
  "lastModifiedAt": "2025-01-25T11:30:00Z",
  "nodes": [ /* ... */ ],
  "edges": [ /* ... */ ]
}
```

#### 成功響應 (資源列表)

```json
{
  "data": [
    { "id": 1, "name": "Workflow 1" },
    { "id": 2, "name": "Workflow 2" }
  ],
  "pagination": {
    "currentPage": 1,
    "pageSize": 20,
    "totalCount": 45,
    "totalPages": 3
  },
  "links": {
    "self": "/api/v1/workflows?page=1&pageSize=20",
    "first": "/api/v1/workflows?page=1&pageSize=20",
    "next": "/api/v1/workflows?page=2&pageSize=20",
    "last": "/api/v1/workflows?page=3&pageSize=20"
  }
}
```

#### 錯誤響應 (Problem Details RFC 7807)

```json
{
  "type": "https://api.aiworkflow.com/errors/validation-error",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "traceId": "00-abc123-def456-00",
  "errors": {
    "Name": ["The Name field is required."],
    "Description": ["The Description must be at least 10 characters."]
  }
}
```

---

## HTTP 狀態碼

### 2xx Success

| 狀態碼 | 名稱 | 用途 |
|--------|------|------|
| **200** | OK | 成功的 GET, PUT, PATCH 請求 |
| **201** | Created | 成功創建資源 (POST) |
| **202** | Accepted | 請求已接受,異步處理中 |
| **204** | No Content | 成功但無返回內容 (DELETE, PUT) |

### 4xx Client Errors

| 狀態碼 | 名稱 | 用途 |
|--------|------|------|
| **400** | Bad Request | 請求格式錯誤或語法錯誤 |
| **401** | Unauthorized | 未認證或 Token 無效 |
| **403** | Forbidden | 已認證但無權限訪問 |
| **404** | Not Found | 資源不存在 |
| **405** | Method Not Allowed | HTTP 方法不支持 |
| **409** | Conflict | 資源衝突 (如重複創建) |
| **422** | Unprocessable Entity | 語義錯誤 (驗證失敗) |
| **429** | Too Many Requests | 超過 Rate Limit |

### 5xx Server Errors

| 狀態碼 | 名稱 | 用途 |
|--------|------|------|
| **500** | Internal Server Error | 伺服器內部錯誤 |
| **502** | Bad Gateway | 上游服務錯誤 |
| **503** | Service Unavailable | 服務暫時不可用 |
| **504** | Gateway Timeout | 上游服務超時 |

---

## 分頁與過濾

### Offset-based Pagination (推薦用於小數據集)

**請求**:
```http
GET /api/v1/workflows?page=2&pageSize=20
```

**響應**:
```json
{
  "data": [ /* 20 items */ ],
  "pagination": {
    "currentPage": 2,
    "pageSize": 20,
    "totalCount": 156,
    "totalPages": 8,
    "hasPreviousPage": true,
    "hasNextPage": true
  },
  "links": {
    "self": "/api/v1/workflows?page=2&pageSize=20",
    "first": "/api/v1/workflows?page=1&pageSize=20",
    "prev": "/api/v1/workflows?page=1&pageSize=20",
    "next": "/api/v1/workflows?page=3&pageSize=20",
    "last": "/api/v1/workflows?page=8&pageSize=20"
  }
}
```

**ASP.NET Core 實現**:

```csharp
public sealed record PagedResult<T>
{
    public required IReadOnlyList<T> Data { get; init; }
    public required PaginationMetadata Pagination { get; init; }
    public required PaginationLinks Links { get; init; }
}

public sealed record PaginationMetadata
{
    public required int CurrentPage { get; init; }
    public required int PageSize { get; init; }
    public required int TotalCount { get; init; }
    public required int TotalPages { get; init; }
    public required bool HasPreviousPage { get; init; }
    public required bool HasNextPage { get; init; }
}

public sealed record PaginationLinks
{
    public required string Self { get; init; }
    public required string First { get; init; }
    public string? Prev { get; init; }
    public string? Next { get; init; }
    public required string Last { get; init; }
}

// Extension method for IQueryable
public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
    this IQueryable<T> query,
    int page,
    int pageSize,
    string baseUrl,
    CancellationToken cancellationToken = default)
{
    var totalCount = await query.CountAsync(cancellationToken);
    var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

    var items = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(cancellationToken);

    return new PagedResult<T>
    {
        Data = items,
        Pagination = new PaginationMetadata
        {
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            HasPreviousPage = page > 1,
            HasNextPage = page < totalPages
        },
        Links = new PaginationLinks
        {
            Self = $"{baseUrl}?page={page}&pageSize={pageSize}",
            First = $"{baseUrl}?page=1&pageSize={pageSize}",
            Prev = page > 1 ? $"{baseUrl}?page={page - 1}&pageSize={pageSize}" : null,
            Next = page < totalPages ? $"{baseUrl}?page={page + 1}&pageSize={pageSize}" : null,
            Last = $"{baseUrl}?page={totalPages}&pageSize={pageSize}"
        }
    };
}
```

### Cursor-based Pagination (推薦用於大數據集)

**請求**:
```http
GET /api/v1/workflows?cursor=eyJpZCI6MTIzLCJjcmVhdGVkQXQiOiIyMDI1LTAxLTI1VDEwOjAwOjAwWiJ9&limit=20
```

**響應**:
```json
{
  "data": [ /* items */ ],
  "pagination": {
    "limit": 20,
    "hasMore": true,
    "nextCursor": "eyJpZCI6MTQzLCJjcmVhdGVkQXQiOiIyMDI1LTAxLTI1VDExOjAwOjAwWiJ9"
  },
  "links": {
    "self": "/api/v1/workflows?cursor=...&limit=20",
    "next": "/api/v1/workflows?cursor=eyJpZCI6MTQzLCJjcmVhdGVkQXQiOiIyMDI1LTAxLTI1VDExOjAwOjAwWiJ9&limit=20"
  }
}
```

**優點**:
- 性能穩定,不受數據集大小影響
- 實時數據插入不影響分頁一致性
- 適合無限滾動場景

---

## 排序與搜索

### 排序

```http
# 單欄位排序
GET /api/v1/workflows?sortBy=createdAt&sortOrder=desc

# 多欄位排序
GET /api/v1/workflows?sortBy=status,createdAt&sortOrder=asc,desc
```

### 搜索

```http
# 全文搜索
GET /api/v1/workflows?search=automation

# 欄位特定搜索
GET /api/v1/workflows?name=automation&description=customer
```

---

## 批量操作

### 批量刪除

```http
POST /api/v1/workflows/bulk-delete
Content-Type: application/json

{
  "ids": [1, 2, 3, 4, 5]
}

Response: 200 OK
{
  "deleted": 5,
  "failed": 0,
  "errors": []
}
```

### 批量更新

```http
POST /api/v1/workflows/bulk-update
Content-Type: application/json

{
  "ids": [1, 2, 3],
  "updates": {
    "status": "published"
  }
}
```

---

## 冪等性設計

### Idempotency-Key Header

用於 POST 請求的冪等性保證:

```http
POST /api/v1/workflows
Idempotency-Key: 550e8400-e29b-41d4-a716-446655440000
Content-Type: application/json

{
  "name": "New Workflow"
}
```

**ASP.NET Core 實現**:

```csharp
public sealed class IdempotencyMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context, IDistributedCache cache)
    {
        if (!context.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue("Idempotency-Key", out var idempotencyKey))
        {
            await _next(context);
            return;
        }

        var cacheKey = $"idempotency:{idempotencyKey}";
        var cachedResponse = await cache.GetStringAsync(cacheKey);

        if (cachedResponse is not null)
        {
            // 返回緩存的響應
            var response = JsonSerializer.Deserialize<CachedResponse>(cachedResponse);
            context.Response.StatusCode = response!.StatusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(response.Body);
            return;
        }

        // 捕獲響應
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await _next(context);

        // 緩存響應
        responseBody.Seek(0, SeekOrigin.Begin);
        var responseBodyText = await new StreamReader(responseBody).ReadToEndAsync();

        var cachedResponseData = new CachedResponse
        {
            StatusCode = context.Response.StatusCode,
            Body = responseBodyText
        };

        await cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(cachedResponseData),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
            }
        );

        responseBody.Seek(0, SeekOrigin.Begin);
        await responseBody.CopyToAsync(originalBodyStream);
    }
}

public sealed record CachedResponse
{
    public required int StatusCode { get; init; }
    public required string Body { get; init; }
}
```

---

## Content Negotiation

### 請求格式協商

```http
Accept: application/json
Accept: application/xml
Accept: text/csv
```

### 響應格式

```http
Content-Type: application/json; charset=utf-8
Content-Type: application/xml; charset=utf-8
Content-Type: text/csv; charset=utf-8
```

---

## HATEOAS

Hypermedia As The Engine Of Application State (可選實現,達到 Richardson Maturity Model Level 3)

### 示例

```json
{
  "id": 123,
  "name": "Customer Support Automation",
  "status": "draft",
  "links": [
    {
      "rel": "self",
      "href": "/api/v1/workflows/123",
      "method": "GET"
    },
    {
      "rel": "update",
      "href": "/api/v1/workflows/123",
      "method": "PUT"
    },
    {
      "rel": "delete",
      "href": "/api/v1/workflows/123",
      "method": "DELETE"
    },
    {
      "rel": "publish",
      "href": "/api/v1/workflows/123/publish",
      "method": "POST"
    },
    {
      "rel": "nodes",
      "href": "/api/v1/workflows/123/nodes",
      "method": "GET"
    }
  ]
}
```

---

## 實現示例

### 完整的 Minimal API 端點組

```csharp
public static class WorkflowEndpoints
{
    public static RouteGroupBuilder MapWorkflowEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetWorkflowsAsync)
            .WithName("GetWorkflows")
            .WithSummary("Get all workflows with pagination")
            .Produces<PagedResult<WorkflowDto>>(200);

        group.MapGet("/{id:int}", GetWorkflowByIdAsync)
            .WithName("GetWorkflowById")
            .WithSummary("Get a workflow by ID")
            .Produces<WorkflowDto>(200)
            .Produces<ProblemDetails>(404);

        group.MapPost("/", CreateWorkflowAsync)
            .WithName("CreateWorkflow")
            .WithSummary("Create a new workflow")
            .Produces<WorkflowDto>(201)
            .Produces<ValidationProblemDetails>(400);

        group.MapPut("/{id:int}", UpdateWorkflowAsync)
            .WithName("UpdateWorkflow")
            .WithSummary("Update an existing workflow")
            .Produces<WorkflowDto>(200)
            .Produces<ProblemDetails>(404)
            .Produces<ValidationProblemDetails>(400);

        group.MapPatch("/{id:int}", PatchWorkflowAsync)
            .WithName("PatchWorkflow")
            .WithSummary("Partially update a workflow")
            .Produces<WorkflowDto>(200)
            .Produces<ProblemDetails>(404);

        group.MapDelete("/{id:int}", DeleteWorkflowAsync)
            .WithName("DeleteWorkflow")
            .WithSummary("Delete a workflow")
            .Produces(204)
            .Produces<ProblemDetails>(404)
            .Produces<ProblemDetails>(409);

        return group;
    }

    private static async Task<IResult> GetWorkflowsAsync(
        [AsParameters] GetWorkflowsQuery query,
        IMediator mediator,
        LinkGenerator linkGenerator,
        HttpContext httpContext)
    {
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
            return Results.Problem();

        var baseUrl = linkGenerator.GetPathByName("GetWorkflows");
        var pagedResult = await result.Value.ToPagedResultAsync(
            query.Page,
            query.PageSize,
            baseUrl!
        );

        return Results.Ok(pagedResult);
    }

    private static async Task<IResult> GetWorkflowByIdAsync(
        int id,
        IMediator mediator)
    {
        var query = new GetWorkflowByIdQuery(id);
        var result = await mediator.Send(query);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.NotFound();
    }

    private static async Task<IResult> CreateWorkflowAsync(
        CreateWorkflowDto dto,
        IMediator mediator,
        LinkGenerator linkGenerator)
    {
        var command = new CreateWorkflowCommand(dto.Name, dto.Description);
        var result = await mediator.Send(command);

        if (!result.IsSuccess)
            return Results.BadRequest(result.Errors);

        var workflowId = result.Value;
        var location = linkGenerator.GetPathByName("GetWorkflowById", new { id = workflowId });

        var getQuery = new GetWorkflowByIdQuery(workflowId);
        var workflowResult = await mediator.Send(getQuery);

        return Results.Created(location, workflowResult.Value);
    }

    private static async Task<IResult> UpdateWorkflowAsync(
        int id,
        UpdateWorkflowDto dto,
        IMediator mediator)
    {
        var command = new UpdateWorkflowCommand(id, dto.Name, dto.Description, dto.Status);
        var result = await mediator.Send(command);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.NotFound();
    }

    private static async Task<IResult> PatchWorkflowAsync(
        int id,
        JsonElement patchDocument,
        IMediator mediator)
    {
        var command = new PatchWorkflowCommand(id, patchDocument);
        var result = await mediator.Send(command);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.NotFound();
    }

    private static async Task<IResult> DeleteWorkflowAsync(
        int id,
        IMediator mediator)
    {
        var command = new DeleteWorkflowCommand(id);
        var result = await mediator.Send(command);

        if (!result.IsSuccess)
        {
            return result.Error.Code switch
            {
                "NotFound" => Results.NotFound(),
                "Conflict" => Results.Conflict(new ProblemDetails
                {
                    Title = "Cannot delete resource",
                    Detail = result.Error.Message
                }),
                _ => Results.Problem()
            };
        }

        return Results.NoContent();
    }
}

// Program.cs 註冊
var workflowsGroup = app.MapGroup("/api/v1/workflows")
    .WithTags("Workflows")
    .RequireAuthorization();

workflowsGroup.MapWorkflowEndpoints();
```

---

## 最佳實踐檢查清單

### 設計階段
- [ ] 資源使用名詞複數形式
- [ ] URL 路徑使用小寫 kebab-case
- [ ] 正確使用 HTTP 方法語義
- [ ] 設計冪等的 GET, PUT, DELETE
- [ ] 定義清晰的錯誤響應結構

### 實現階段
- [ ] 所有端點返回適當的 HTTP 狀態碼
- [ ] 實現 Problem Details (RFC 7807) 錯誤格式
- [ ] POST 創建資源返回 201 和 Location header
- [ ] 列表端點提供分頁支持
- [ ] 實現排序、過濾、搜索功能
- [ ] 添加輸入驗證 (FluentValidation)
- [ ] 實現認證授權機制
- [ ] 添加 Rate Limiting
- [ ] 配置 CORS
- [ ] 實現 Idempotency-Key 支持

### 文檔階段
- [ ] 配置 Swagger/OpenAPI 文檔
- [ ] 添加 XML 註解
- [ ] 提供請求/響應示例
- [ ] 記錄錯誤代碼和消息

### 測試階段
- [ ] 單元測試 (>80% 覆蓋率)
- [ ] 集成測試 (WebApplicationFactory)
- [ ] Contract Testing
- [ ] 安全測試
- [ ] 性能測試

---

**文檔維護**: Tech Lead Team
**最後更新**: 2025-01-01
**狀態**: ✅ 完整
