# 錯誤處理與響應格式 (Error Handling and Response Format)

## 版本信息
- **標準版本**: v1.0.0
- **適用框架**: ASP.NET Core 8.0
- **錯誤標準**: RFC 7807 (Problem Details)
- **最後更新**: 2025-01-01

## 目錄

1. [錯誤處理原則](#錯誤處理原則)
2. [Problem Details (RFC 7807)](#problem-details-rfc-7807)
3. [錯誤代碼分類](#錯誤代碼分類)
4. [標準錯誤響應](#標準錯誤響應)
5. [驗證錯誤處理](#驗證錯誤處理)
6. [異常處理中間件](#異常處理中間件)
7. [日誌記錄策略](#日誌記錄策略)
8. [Correlation ID 追蹤](#correlation-id-追蹤)
9. [多語言錯誤消息](#多語言錯誤消息)
10. [ASP.NET Core 完整實現](#aspnet-core-完整實現)

---

## 錯誤處理原則

### 設計原則

1. **一致性**: 所有 API 錯誤使用統一的響應格式
2. **可操作性**: 錯誤消息清晰,提供足夠信息幫助客戶端處理錯誤
3. **安全性**: 不洩露內部實現細節或敏感信息
4. **可追溯性**: 提供唯一標識符用於日誌追蹤和問題排查
5. **用戶友好**: 提供人類可讀的錯誤描述

### 錯誤類型

| 錯誤類型 | HTTP 狀態碼 | 用途 |
|----------|-------------|------|
| **Validation Error** | 400, 422 | 輸入驗證失敗 |
| **Authentication Error** | 401 | 未認證或 Token 無效 |
| **Authorization Error** | 403 | 已認證但無權限 |
| **Not Found Error** | 404 | 資源不存在 |
| **Conflict Error** | 409 | 資源衝突或狀態不一致 |
| **Rate Limit Error** | 429 | 超過速率限制 |
| **Server Error** | 500 | 伺服器內部錯誤 |
| **Service Unavailable** | 503 | 服務暫時不可用 |

---

## Problem Details (RFC 7807)

### 標準

[RFC 7807 - Problem Details for HTTP APIs](https://tools.ietf.org/html/rfc7807)

**Content-Type**: `application/problem+json`

### 基本結構

```json
{
  "type": "https://api.aiworkflow.com/errors/validation-error",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "detail": "The request contains invalid data.",
  "instance": "/api/v1/workflows",
  "traceId": "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-00"
}
```

### 欄位說明

| 欄位 | 類型 | 必填 | 說明 |
|------|------|------|------|
| **type** | URI | 否 | 錯誤類型的唯一標識符,指向錯誤文檔 |
| **title** | String | 是 | 錯誤的簡短可讀標題 |
| **status** | Integer | 是 | HTTP 狀態碼 |
| **detail** | String | 否 | 錯誤的詳細描述 |
| **instance** | URI | 否 | 發生錯誤的具體請求路徑 |
| **traceId** | String | 是 | 請求追蹤 ID,用於日誌關聯 |

### 擴展欄位

可以添加自定義欄位以提供更多上下文:

```json
{
  "type": "https://api.aiworkflow.com/errors/validation-error",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "traceId": "00-abc123-def456-00",
  "errors": {
    "Name": ["The Name field is required."],
    "Description": ["The Description must be at least 10 characters."]
  },
  "timestamp": "2025-01-25T10:00:00Z",
  "path": "/api/v1/workflows"
}
```

---

## 錯誤代碼分類

### 錯誤代碼體系

**格式**: `{Domain}_{ErrorType}_{SpecificError}`

```
WORKFLOW_VALIDATION_NAME_REQUIRED
WORKFLOW_NOTFOUND_ID_INVALID
AUTH_UNAUTHORIZED_TOKEN_EXPIRED
```

### 錯誤代碼表

#### 驗證錯誤 (4xx)

| 錯誤代碼 | HTTP 狀態 | 描述 |
|----------|-----------|------|
| `VALIDATION_ERROR` | 400 | 通用驗證錯誤 |
| `REQUIRED_FIELD_MISSING` | 400 | 必填欄位缺失 |
| `INVALID_FORMAT` | 400 | 格式錯誤 |
| `INVALID_VALUE` | 400 | 值不合法 |
| `FIELD_TOO_LONG` | 400 | 欄位超長 |
| `FIELD_TOO_SHORT` | 400 | 欄位過短 |

#### 認證與授權錯誤 (401, 403)

| 錯誤代碼 | HTTP 狀態 | 描述 |
|----------|-----------|------|
| `UNAUTHORIZED` | 401 | 未認證 |
| `TOKEN_INVALID` | 401 | Token 無效 |
| `TOKEN_EXPIRED` | 401 | Token 過期 |
| `FORBIDDEN` | 403 | 無權限訪問 |
| `INSUFFICIENT_PERMISSIONS` | 403 | 權限不足 |

#### 資源錯誤 (404, 409)

| 錯誤代碼 | HTTP 狀態 | 描述 |
|----------|-----------|------|
| `RESOURCE_NOT_FOUND` | 404 | 資源不存在 |
| `WORKFLOW_NOT_FOUND` | 404 | 工作流不存在 |
| `RESOURCE_CONFLICT` | 409 | 資源衝突 |
| `DUPLICATE_RESOURCE` | 409 | 資源重複 |
| `STATE_CONFLICT` | 409 | 狀態衝突 |

#### 限流錯誤 (429)

| 錯誤代碼 | HTTP 狀態 | 描述 |
|----------|-----------|------|
| `RATE_LIMIT_EXCEEDED` | 429 | 超過速率限制 |
| `QUOTA_EXCEEDED` | 429 | 超過配額 |

#### 伺服器錯誤 (5xx)

| 錯誤代碼 | HTTP 狀態 | 描述 |
|----------|-----------|------|
| `INTERNAL_SERVER_ERROR` | 500 | 伺服器內部錯誤 |
| `DATABASE_ERROR` | 500 | 數據庫錯誤 |
| `SERVICE_UNAVAILABLE` | 503 | 服務不可用 |
| `GATEWAY_TIMEOUT` | 504 | 網關超時 |

---

## 標準錯誤響應

### 400 Bad Request - 驗證錯誤

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
  "traceId": "00-abc123-def456-00",
  "errors": {
    "Name": [
      "The Name field is required."
    ],
    "Description": [
      "The Description must be at least 10 characters."
    ]
  }
}
```

### 401 Unauthorized - 未認證

```http
GET /api/v1/workflows/123
Authorization: Bearer invalid_token

Response: 401 Unauthorized
Content-Type: application/problem+json

{
  "type": "https://api.aiworkflow.com/errors/unauthorized",
  "title": "Unauthorized",
  "status": 401,
  "detail": "The access token is invalid or has expired.",
  "traceId": "00-xyz789-abc123-00"
}
```

### 403 Forbidden - 無權限

```http
DELETE /api/v1/workflows/123
Authorization: Bearer {valid_token}

Response: 403 Forbidden
Content-Type: application/problem+json

{
  "type": "https://api.aiworkflow.com/errors/forbidden",
  "title": "Forbidden",
  "status": 403,
  "detail": "You don't have permission to delete this workflow.",
  "traceId": "00-def456-ghi789-00",
  "requiredPermission": "workflows:delete"
}
```

### 404 Not Found - 資源不存在

```http
GET /api/v1/workflows/999

Response: 404 Not Found
Content-Type: application/problem+json

{
  "type": "https://api.aiworkflow.com/errors/not-found",
  "title": "Resource not found",
  "status": 404,
  "detail": "Workflow with ID 999 was not found.",
  "traceId": "00-jkl012-mno345-00",
  "resourceType": "Workflow",
  "resourceId": "999"
}
```

### 409 Conflict - 資源衝突

```http
POST /api/v1/workflows
Content-Type: application/json

{
  "name": "Existing Workflow"
}

Response: 409 Conflict
Content-Type: application/problem+json

{
  "type": "https://api.aiworkflow.com/errors/conflict",
  "title": "Resource conflict",
  "status": 409,
  "detail": "A workflow with the name 'Existing Workflow' already exists.",
  "traceId": "00-pqr678-stu901-00",
  "conflictReason": "duplicate_name"
}
```

### 429 Too Many Requests - 超過限流

```http
GET /api/v1/workflows

Response: 429 Too Many Requests
Content-Type: application/problem+json
Retry-After: 60
X-RateLimit-Limit: 1000
X-RateLimit-Remaining: 0
X-RateLimit-Reset: 1640995200

{
  "type": "https://api.aiworkflow.com/errors/rate-limit-exceeded",
  "title": "Rate limit exceeded",
  "status": 429,
  "detail": "You have exceeded the rate limit of 1000 requests per hour.",
  "traceId": "00-vwx234-yza567-00",
  "retryAfter": 60,
  "limit": 1000,
  "remaining": 0,
  "resetAt": "2025-01-25T11:00:00Z"
}
```

### 500 Internal Server Error - 伺服器錯誤

```http
GET /api/v1/workflows

Response: 500 Internal Server Error
Content-Type: application/problem+json

{
  "type": "https://api.aiworkflow.com/errors/internal-server-error",
  "title": "Internal server error",
  "status": 500,
  "detail": "An unexpected error occurred while processing your request.",
  "traceId": "00-bcd890-efg123-00"
}
```

**注意**: 500 錯誤不應洩露內部異常詳情,僅提供通用錯誤消息和 traceId 用於日誌追蹤。

---

## 驗證錯誤處理

### FluentValidation 集成

#### 安裝包

```xml
<PackageReference Include="FluentValidation" Version="11.9.0" />
<PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="11.9.0" />
```

#### 定義驗證器

```csharp
public sealed class CreateWorkflowDtoValidator : AbstractValidator<CreateWorkflowDto>
{
    public CreateWorkflowDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("The Name field is required.")
            .WithErrorCode("REQUIRED_FIELD_MISSING")
            .MaximumLength(200)
            .WithMessage("The Name must not exceed 200 characters.")
            .WithErrorCode("FIELD_TOO_LONG");

        RuleFor(x => x.Description)
            .MinimumLength(10)
            .When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage("The Description must be at least 10 characters.")
            .WithErrorCode("FIELD_TOO_SHORT")
            .MaximumLength(2000)
            .WithMessage("The Description must not exceed 2000 characters.")
            .WithErrorCode("FIELD_TOO_LONG");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("The Status value is invalid.")
            .WithErrorCode("INVALID_VALUE");
    }
}
```

#### 註冊驗證器

```csharp
// Program.cs
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
```

#### 手動驗證

```csharp
public sealed class CreateWorkflowCommandHandler(
    IApplicationDbContext context,
    IValidator<CreateWorkflowDto> validator,
    ILogger<CreateWorkflowCommandHandler> logger)
    : IRequestHandler<CreateWorkflowCommand, Result<int>>
{
    public async Task<Result<int>> Handle(
        CreateWorkflowCommand request,
        CancellationToken cancellationToken)
    {
        // 驗證
        var validationResult = await validator.ValidateAsync(request.Dto, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            return Result<int>.ValidationFailure(errors);
        }

        // 業務邏輯
        var workflow = Workflow.Create(
            request.Dto.Name,
            request.Dto.Description,
            request.UserId
        );

        context.Workflows.Add(workflow);
        await context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(workflow.Id);
    }
}
```

### 自動驗證 Endpoint Filter

```csharp
public sealed class ValidationFilter<T> : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();

        if (validator is null)
            return await next(context);

        var dto = context.Arguments.OfType<T>().FirstOrDefault();

        if (dto is null)
            return await next(context);

        var validationResult = await validator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            var problemDetails = new ValidationProblemDetails(errors)
            {
                Type = "https://api.aiworkflow.com/errors/validation-error",
                Title = "One or more validation errors occurred.",
                Status = StatusCodes.Status400BadRequest
            };

            return Results.BadRequest(problemDetails);
        }

        return await next(context);
    }
}

// 使用 Filter
app.MapPost("/api/v1/workflows", async (
    CreateWorkflowDto dto,
    IMediator mediator) =>
{
    var command = new CreateWorkflowCommand(dto);
    var result = await mediator.Send(command);
    return Results.Created($"/api/v1/workflows/{result.Value}", result.Value);
})
.AddEndpointFilter<ValidationFilter<CreateWorkflowDto>>();
```

---

## 異常處理中間件

### 全局異常處理器

```csharp
public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        _logger.LogError(
            exception,
            "An unhandled exception occurred. TraceId: {TraceId}, Path: {Path}",
            traceId,
            httpContext.Request.Path
        );

        var (statusCode, problemDetails) = MapExceptionToProblemDetails(exception, traceId);

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static (int StatusCode, ProblemDetails ProblemDetails) MapExceptionToProblemDetails(
        Exception exception,
        string traceId)
    {
        return exception switch
        {
            ValidationException validationException => (
                StatusCodes.Status400BadRequest,
                new ValidationProblemDetails(validationException.Errors)
                {
                    Type = "https://api.aiworkflow.com/errors/validation-error",
                    Title = "One or more validation errors occurred.",
                    Status = StatusCodes.Status400BadRequest,
                    Extensions = { ["traceId"] = traceId }
                }
            ),

            UnauthorizedAccessException => (
                StatusCodes.Status401Unauthorized,
                new ProblemDetails
                {
                    Type = "https://api.aiworkflow.com/errors/unauthorized",
                    Title = "Unauthorized",
                    Status = StatusCodes.Status401Unauthorized,
                    Detail = "You are not authorized to access this resource.",
                    Extensions = { ["traceId"] = traceId }
                }
            ),

            ForbiddenAccessException => (
                StatusCodes.Status403Forbidden,
                new ProblemDetails
                {
                    Type = "https://api.aiworkflow.com/errors/forbidden",
                    Title = "Forbidden",
                    Status = StatusCodes.Status403Forbidden,
                    Detail = exception.Message,
                    Extensions = { ["traceId"] = traceId }
                }
            ),

            NotFoundException notFoundException => (
                StatusCodes.Status404NotFound,
                new ProblemDetails
                {
                    Type = "https://api.aiworkflow.com/errors/not-found",
                    Title = "Resource not found",
                    Status = StatusCodes.Status404NotFound,
                    Detail = notFoundException.Message,
                    Extensions = { ["traceId"] = traceId }
                }
            ),

            ConflictException conflictException => (
                StatusCodes.Status409Conflict,
                new ProblemDetails
                {
                    Type = "https://api.aiworkflow.com/errors/conflict",
                    Title = "Resource conflict",
                    Status = StatusCodes.Status409Conflict,
                    Detail = conflictException.Message,
                    Extensions = { ["traceId"] = traceId }
                }
            ),

            _ => (
                StatusCodes.Status500InternalServerError,
                new ProblemDetails
                {
                    Type = "https://api.aiworkflow.com/errors/internal-server-error",
                    Title = "Internal server error",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = "An unexpected error occurred while processing your request.",
                    Extensions = { ["traceId"] = traceId }
                }
            )
        };
    }
}

// 註冊全局異常處理器
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// 使用中間件
app.UseExceptionHandler();
```

### 自定義異常類

```csharp
public abstract class DomainException(string message) : Exception(message);

public sealed class NotFoundException(string message) : DomainException(message);

public sealed class ValidationException(IDictionary<string, string[]> errors)
    : DomainException("One or more validation errors occurred.")
{
    public IDictionary<string, string[]> Errors { get; } = errors;
}

public sealed class UnauthorizedException(string message) : DomainException(message);

public sealed class ForbiddenAccessException(string message) : DomainException(message);

public sealed class ConflictException(string message) : DomainException(message);
```

---

## 日誌記錄策略

### Serilog 配置

#### 安裝包

```xml
<PackageReference Include="Serilog.AspNetCore" Version="8.0.0" />
<PackageReference Include="Serilog.Sinks.Console" Version="5.0.1" />
<PackageReference Include="Serilog.Sinks.File" Version="5.0.0" />
<PackageReference Include="Serilog.Enrichers.Environment" Version="2.3.0" />
<PackageReference Include="Serilog.Enrichers.Thread" Version="3.1.0" />
```

#### Program.cs 配置

```csharp
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// 配置 Serilog
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithThreadId()
        .Enrich.WithProperty("Application", "AIWorkflow.API")
        .WriteTo.Console()
        .WriteTo.File(
            "logs/aiworkflow-.log",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 30
        );
});

var app = builder.Build();

// 添加 HTTP 請求日誌
app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("ClientIP", httpContext.Connection.RemoteIpAddress);
        diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());
        diagnosticContext.Set("TraceId", httpContext.TraceIdentifier);
    };
});
```

#### appsettings.json 配置

```json
{
  "Serilog": {
    "Using": ["Serilog.Sinks.Console", "Serilog.Sinks.File"],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.Hosting.Lifetime": "Information",
        "Microsoft.EntityFrameworkCore": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/aiworkflow-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30,
          "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj} {Properties:j}{NewLine}{Exception}"
        }
      }
    ]
  }
}
```

### 結構化日誌示例

```csharp
public sealed class WorkflowService(ILogger<WorkflowService> logger)
{
    private readonly ILogger<WorkflowService> _logger = logger;

    public async Task<Result<Workflow>> CreateWorkflowAsync(CreateWorkflowDto dto)
    {
        _logger.LogInformation(
            "Creating workflow. Name: {WorkflowName}, OwnerId: {OwnerId}",
            dto.Name,
            dto.OwnerId
        );

        try
        {
            var workflow = Workflow.Create(dto.Name, dto.Description, dto.OwnerId);

            _logger.LogInformation(
                "Workflow created successfully. WorkflowId: {WorkflowId}",
                workflow.Id
            );

            return Result<Workflow>.Success(workflow);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to create workflow. Name: {WorkflowName}, OwnerId: {OwnerId}",
                dto.Name,
                dto.OwnerId
            );

            return Result<Workflow>.Failure(Error.InternalServerError("Failed to create workflow"));
        }
    }
}
```

### 敏感數據脫敏

```csharp
public sealed class SensitiveDataMaskingEnricher : ILogEventEnricher
{
    private static readonly string[] SensitiveProperties =
    {
        "Password",
        "Token",
        "Secret",
        "ApiKey",
        "Authorization"
    };

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        foreach (var property in logEvent.Properties.ToList())
        {
            if (SensitiveProperties.Any(s => property.Key.Contains(s, StringComparison.OrdinalIgnoreCase)))
            {
                logEvent.RemovePropertyIfPresent(property.Key);
                logEvent.AddPropertyIfAbsent(
                    propertyFactory.CreateProperty(property.Key, "***REDACTED***")
                );
            }
        }
    }
}

// 註冊 Enricher
configuration.Enrich.With<SensitiveDataMaskingEnricher>();
```

---

## Correlation ID 追蹤

### Correlation ID 中間件

```csharp
public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    private const string CorrelationIdHeaderName = "X-Correlation-ID";
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = GetOrGenerateCorrelationId(context);

        context.TraceIdentifier = correlationId;
        context.Response.Headers.Append(CorrelationIdHeaderName, correlationId);

        using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }

    private static string GetOrGenerateCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(CorrelationIdHeaderName, out var correlationId))
        {
            return correlationId.ToString();
        }

        return Activity.Current?.Id ?? Guid.NewGuid().ToString();
    }
}

// 註冊中間件
app.UseMiddleware<CorrelationIdMiddleware>();
```

### 在錯誤響應中包含 Correlation ID

```csharp
var problemDetails = new ProblemDetails
{
    Type = "https://api.aiworkflow.com/errors/internal-server-error",
    Title = "Internal server error",
    Status = 500,
    Detail = "An unexpected error occurred.",
    Extensions =
    {
        ["traceId"] = httpContext.TraceIdentifier,  // Correlation ID
        ["timestamp"] = DateTime.UtcNow
    }
};
```

---

## 多語言錯誤消息

### 資源文件結構

```
Resources/
├── Errors.resx               # 默認 (英文)
├── Errors.zh-TW.resx         # 繁體中文
└── Errors.ja.resx            # 日文
```

### Errors.resx

```xml
<data name="VALIDATION_NAME_REQUIRED" xml:space="preserve">
  <value>The Name field is required.</value>
</data>
<data name="VALIDATION_DESCRIPTION_TOO_SHORT" xml:space="preserve">
  <value>The Description must be at least 10 characters.</value>
</data>
<data name="NOT_FOUND_WORKFLOW" xml:space="preserve">
  <value>Workflow with ID {0} was not found.</value>
</data>
```

### Errors.zh-TW.resx

```xml
<data name="VALIDATION_NAME_REQUIRED" xml:space="preserve">
  <value>名稱欄位為必填。</value>
</data>
<data name="VALIDATION_DESCRIPTION_TOO_SHORT" xml:space="preserve">
  <value>描述至少需要 10 個字符。</value>
</data>
<data name="NOT_FOUND_WORKFLOW" xml:space="preserve">
  <value>找不到 ID 為 {0} 的工作流程。</value>
</data>
```

### 本地化服務

```csharp
public interface IErrorMessageLocalizer
{
    string GetLocalizedMessage(string errorCode, params object[] args);
}

public sealed class ErrorMessageLocalizer(IStringLocalizer<Errors> localizer)
    : IErrorMessageLocalizer
{
    private readonly IStringLocalizer<Errors> _localizer = localizer;

    public string GetLocalizedMessage(string errorCode, params object[] args)
    {
        var message = _localizer[errorCode];

        return args.Length > 0
            ? string.Format(message.Value, args)
            : message.Value;
    }
}

// 註冊服務
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddSingleton<IErrorMessageLocalizer, ErrorMessageLocalizer>();

// 配置支持的語言
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "en-US", "zh-TW", "ja-JP" };
    options.SetDefaultCulture("en-US")
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);
});

app.UseRequestLocalization();
```

### 使用本地化錯誤消息

```csharp
public sealed class CreateWorkflowCommandHandler(
    IApplicationDbContext context,
    IErrorMessageLocalizer errorLocalizer)
    : IRequestHandler<CreateWorkflowCommand, Result<int>>
{
    public async Task<Result<int>> Handle(
        CreateWorkflowCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            var errorMessage = errorLocalizer.GetLocalizedMessage("VALIDATION_NAME_REQUIRED");
            return Result<int>.Failure(Error.Validation(errorMessage));
        }

        // 業務邏輯...
    }
}
```

---

## ASP.NET Core 完整實現

### Program.cs 完整配置

```csharp
using Microsoft.AspNetCore.Diagnostics;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithThreadId();
});

// Services
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddSingleton<IErrorMessageLocalizer, ErrorMessageLocalizer>();

var app = builder.Build();

// Middleware
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseRequestLocalization();
app.UseSerilogRequestLogging();
app.UseExceptionHandler();

// Endpoints
var workflowsGroup = app.MapGroup("/api/v1/workflows")
    .WithTags("Workflows")
    .RequireAuthorization();

workflowsGroup.MapPost("/", async (
    CreateWorkflowDto dto,
    IMediator mediator) =>
{
    var command = new CreateWorkflowCommand(dto);
    var result = await mediator.Send(command);

    return result.IsSuccess
        ? Results.Created($"/api/v1/workflows/{result.Value}", result.Value)
        : Results.BadRequest(result.Error);
})
.AddEndpointFilter<ValidationFilter<CreateWorkflowDto>>();

app.Run();
```

### Result Pattern 實現

```csharp
public sealed class Result
{
    public bool IsSuccess { get; }
    public Error? Error { get; }

    protected Result(bool isSuccess, Error? error)
    {
        if (isSuccess && error != null)
            throw new InvalidOperationException("Success result cannot have an error.");

        if (!isSuccess && error == null)
            throw new InvalidOperationException("Failure result must have an error.");

        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(Error error) => new(false, error);
}

public sealed class Result<T> : Result
{
    public T? Value { get; }

    private Result(bool isSuccess, T? value, Error? error)
        : base(isSuccess, error)
    {
        Value = value;
    }

    public static Result<T> Success(T value) => new(true, value, null);
    public static new Result<T> Failure(Error error) => new(false, default, error);
    public static Result<T> ValidationFailure(IDictionary<string, string[]> errors) =>
        new(false, default, Error.Validation(errors));
}

public sealed record Error(
    string Code,
    string Message,
    IDictionary<string, string[]>? ValidationErrors = null)
{
    public static Error Validation(IDictionary<string, string[]> errors) =>
        new("VALIDATION_ERROR", "One or more validation errors occurred.", errors);

    public static Error Validation(string message) =>
        new("VALIDATION_ERROR", message);

    public static Error NotFound(string message) =>
        new("NOT_FOUND", message);

    public static Error Unauthorized(string message) =>
        new("UNAUTHORIZED", message);

    public static Error Forbidden(string message) =>
        new("FORBIDDEN", message);

    public static Error Conflict(string message) =>
        new("CONFLICT", message);

    public static Error InternalServerError(string message) =>
        new("INTERNAL_SERVER_ERROR", message);
}
```

---

## 最佳實踐檢查清單

### 設計階段
- [ ] 採用 RFC 7807 Problem Details 標準
- [ ] 定義統一的錯誤代碼體系
- [ ] 設計多語言錯誤消息策略
- [ ] 規劃 Correlation ID 追蹤機制

### 實現階段
- [ ] 實現全局異常處理器
- [ ] 集成 FluentValidation
- [ ] 配置 Serilog 結構化日誌
- [ ] 實現 Correlation ID 中間件
- [ ] 添加敏感數據脫敏
- [ ] 實現本地化錯誤消息

### 安全階段
- [ ] 500 錯誤不洩露內部細節
- [ ] 日誌中脫敏敏感數據
- [ ] 限制錯誤消息詳細程度
- [ ] 實現 Rate Limiting

### 監控階段
- [ ] 配置錯誤告警
- [ ] 追蹤錯誤指標
- [ ] 關聯 Correlation ID 日誌
- [ ] 分析錯誤趨勢

---

**文檔維護**: Tech Lead Team
**最後更新**: 2025-01-01
**狀態**: ✅ 完整
