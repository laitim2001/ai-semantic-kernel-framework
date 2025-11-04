# API 文檔標準 (Swagger/OpenAPI) (API Documentation Standards)

## 版本信息
- **標準版本**: v1.0.0
- **OpenAPI 規範**: 3.0.3
- **Swashbuckle 版本**: 6.5.0+
- **最後更新**: 2025-01-01

## 目錄

1. [OpenAPI 規範概述](#openapi-規範概述)
2. [Swashbuckle 配置](#swashbuckle-配置)
3. [XML 註解文檔](#xml-註解文檔)
4. [API 描述與示例](#api-描述與示例)
5. [認證配置](#認證配置)
6. [多版本 API 文檔](#多版本-api-文檔)
7. [Swagger UI 自定義](#swagger-ui-自定義)
8. [代碼生成](#代碼生成)

---

## OpenAPI 規範概述

### 什麼是 OpenAPI?

OpenAPI Specification (OAS) 是描述 RESTful API 的標準格式,定義:
- 可用的端點和操作
- 請求/響應參數
- 認證方法
- 聯繫信息、許可證、使用條款

### 為什麼需要 API 文檔?

1. **開發者體驗**: 清晰的文檔降低 API 學習曲線
2. **交互式測試**: Swagger UI 提供瀏覽器內測試
3. **客戶端生成**: 自動生成 SDK 和客戶端代碼
4. **契約優先**: API 規範作為前後端開發契約

---

## Swashbuckle 配置

### 安裝 NuGet 包

```xml
<!-- Directory.Packages.props -->
<ItemGroup Label="API Documentation">
  <PackageVersion Include="Swashbuckle.AspNetCore" Version="6.5.0" />
  <PackageVersion Include="Swashbuckle.AspNetCore.Annotations" Version="6.5.0" />
</ItemGroup>
```

### Program.cs 基礎配置

```csharp
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "AI Workflow Platform API",
        Description = "A comprehensive API for building and executing AI-powered workflows",
        TermsOfService = new Uri("https://aiworkflow.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "API Support",
            Email = "api-support@aiworkflow.com",
            Url = new Uri("https://aiworkflow.com/support")
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // XML 註解文檔
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    // JWT 認證
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "AI Workflow API V1");
        options.RoutePrefix = "swagger";
    });
}

app.Run();
```

---

## XML 註解文檔

### 啟用 XML 文檔生成

```xml
<!-- AIWorkflow.API.csproj -->
<PropertyGroup>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
  <NoWarn>$(NoWarn);1591</NoWarn> <!-- 忽略缺少 XML 註解的警告 -->
</PropertyGroup>
```

### Minimal API XML 註解

```csharp
/// <summary>
/// 獲取所有工作流程
/// </summary>
/// <param name="page">頁碼 (從 1 開始)</param>
/// <param name="pageSize">每頁項目數 (最大 100)</param>
/// <param name="mediator">MediatR 實例</param>
/// <returns>分頁的工作流程列表</returns>
/// <response code="200">成功返回工作流程列表</response>
/// <response code="400">請求參數無效</response>
/// <response code="401">未認證</response>
app.MapGet("/api/v1/workflows", async (
    [FromQuery] int page,
    [FromQuery] int pageSize,
    IMediator mediator) =>
{
    var query = new GetWorkflowsQuery(page, pageSize);
    var result = await mediator.Send(query);
    return Results.Ok(result.Value);
})
.WithName("GetWorkflows")
.WithTags("Workflows")
.WithSummary("Get all workflows with pagination")
.WithDescription("Retrieves a paginated list of workflows for the authenticated user.")
.Produces<PagedResult<WorkflowDto>>(StatusCodes.Status200OK)
.Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
.Produces<ProblemDetails>(StatusCodes.Status401Unauthorized);
```

### DTO XML 註解

```csharp
/// <summary>
/// 工作流程數據傳輸對象
/// </summary>
public sealed record WorkflowDto
{
    /// <summary>
    /// 工作流程唯一標識符
    /// </summary>
    /// <example>123</example>
    public required int Id { get; init; }

    /// <summary>
    /// 工作流程名稱
    /// </summary>
    /// <example>Customer Support Automation</example>
    public required string Name { get; init; }

    /// <summary>
    /// 工作流程描述
    /// </summary>
    /// <example>Automated customer inquiry handling workflow</example>
    public string? Description { get; init; }

    /// <summary>
    /// 工作流程狀態
    /// </summary>
    /// <example>published</example>
    public required WorkflowStatus Status { get; init; }

    /// <summary>
    /// 擁有者用戶 ID
    /// </summary>
    /// <example>42</example>
    public required int OwnerId { get; init; }

    /// <summary>
    /// 創建時間 (ISO 8601 格式)
    /// </summary>
    /// <example>2025-01-25T10:00:00Z</example>
    public required DateTime CreatedAt { get; init; }

    /// <summary>
    /// 最後修改時間 (ISO 8601 格式)
    /// </summary>
    /// <example>2025-01-25T14:30:00Z</example>
    public DateTime? LastModifiedAt { get; init; }
}
```

---

## API 描述與示例

### 使用 Swashbuckle.AspNetCore.Annotations

```csharp
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
});

app.MapPost("/api/v1/workflows", async (
    CreateWorkflowDto dto,
    IMediator mediator) =>
{
    var command = new CreateWorkflowCommand(dto);
    var result = await mediator.Send(command);
    return Results.Created($"/api/v1/workflows/{result.Value}", result.Value);
})
.WithOpenApi(operation =>
{
    operation.Summary = "Create a new workflow";
    operation.Description = "Creates a new workflow for the authenticated user. The workflow starts in 'draft' status.";

    operation.RequestBody.Description = "The workflow data to create";

    operation.Responses["201"].Description = "Workflow created successfully. Returns the created workflow with generated ID.";
    operation.Responses["400"].Description = "Invalid request data or validation errors.";
    operation.Responses["401"].Description = "Unauthorized. Valid JWT token required.";

    return operation;
});
```

### 請求體示例

```csharp
public sealed record CreateWorkflowDto
{
    /// <summary>
    /// 工作流程名稱
    /// </summary>
    /// <example>Customer Support Automation</example>
    [Required]
    [MaxLength(200)]
    public required string Name { get; init; }

    /// <summary>
    /// 工作流程描述
    /// </summary>
    /// <example>Automated workflow for handling customer inquiries using AI agents</example>
    [MaxLength(2000)]
    public string? Description { get; init; }
}
```

生成的 OpenAPI JSON:

```json
{
  "CreateWorkflowDto": {
    "type": "object",
    "required": ["name"],
    "properties": {
      "name": {
        "type": "string",
        "maxLength": 200,
        "description": "工作流程名稱",
        "example": "Customer Support Automation"
      },
      "description": {
        "type": "string",
        "maxLength": 2000,
        "nullable": true,
        "description": "工作流程描述",
        "example": "Automated workflow for handling customer inquiries using AI agents"
      }
    }
  }
}
```

---

## 認證配置

### JWT Bearer 認證

```csharp
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme.
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});
```

Swagger UI 將顯示 "Authorize" 按鈕,允許用戶輸入 JWT token。

---

## 多版本 API 文檔

### 配置多版本文檔

```csharp
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

// 添加 API 版本控制
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// 配置 Swagger 生成器
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 配置 Swagger UI
var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions.Reverse())
    {
        options.SwaggerEndpoint(
            $"/swagger/{description.GroupName}/swagger.json",
            description.GroupName.ToUpperInvariant()
        );
    }

    options.RoutePrefix = "swagger";
});
```

### ConfigureSwaggerOptions 實現

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
                CreateInfoForApiVersion(description)
            );
        }

        // XML 註解
        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

        // JWT 認證
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme.",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    }

    private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        var info = new OpenApiInfo
        {
            Title = $"AI Workflow Platform API {description.ApiVersion}",
            Version = description.ApiVersion.ToString(),
            Description = "A comprehensive API for building and executing AI-powered workflows",
            Contact = new OpenApiContact
            {
                Name = "API Support",
                Email = "api-support@aiworkflow.com"
            },
            License = new OpenApiLicense
            {
                Name = "MIT License",
                Url = new Uri("https://opensource.org/licenses/MIT")
            }
        };

        if (description.IsDeprecated)
        {
            info.Description += " ⚠️ **This API version is deprecated and will be retired soon.**";
        }

        return info;
    }
}
```

---

## Swagger UI 自定義

### 自定義主題和樣式

```csharp
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "AI Workflow API V1");
    options.RoutePrefix = "swagger";

    // 自定義 CSS
    options.InjectStylesheet("/swagger-ui/custom.css");

    // 自定義 JavaScript
    options.InjectJavascript("/swagger-ui/custom.js");

    // 文檔展開級別
    options.DocExpansion(DocExpansion.List);

    // 默認展開深度
    options.DefaultModelsExpandDepth(2);

    // 顯示請求持續時間
    options.DisplayRequestDuration();

    // 啟用深度鏈接
    options.EnableDeepLinking();

    // 啟用過濾
    options.EnableFilter();

    // 持久化授權數據
    options.EnablePersistAuthorization();
});

// 提供自定義 CSS 和 JS
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "swagger-ui")
    ),
    RequestPath = "/swagger-ui"
});
```

### 自定義 CSS (custom.css)

```css
/* wwwroot/swagger-ui/custom.css */
.swagger-ui .topbar {
    background-color: #1976d2;
}

.swagger-ui .topbar .link {
    content: url('/images/logo-white.png');
    width: 150px;
    height: auto;
}

.swagger-ui .info .title {
    color: #1976d2;
}

.swagger-ui .scheme-container {
    background: #f5f5f5;
    padding: 15px;
    border-radius: 4px;
}
```

---

## 代碼生成

### 使用 NSwag 生成 TypeScript 客戶端

#### 安裝 NSwag CLI

```bash
dotnet tool install -g NSwag.ConsoleCore
```

#### nswag.json 配置

```json
{
  "runtime": "Net80",
  "defaultVariables": null,
  "documentGenerator": {
    "aspNetCoreToOpenApi": {
      "project": "AIWorkflow.API.csproj",
      "msBuildProjectExtensionsPath": null,
      "configuration": "Release",
      "runtime": null,
      "targetFramework": "net8.0",
      "noBuild": false,
      "msBuildOutputPath": null,
      "verbose": true,
      "workingDirectory": null,
      "requireParametersWithoutDefault": true,
      "apiGroupNames": null,
      "defaultPropertyNameHandling": "Default",
      "defaultReferenceTypeNullHandling": "Null",
      "defaultDictionaryValueReferenceTypeNullHandling": "NotNull",
      "defaultResponseReferenceTypeNullHandling": "NotNull",
      "defaultEnumHandling": "Integer",
      "flattenInheritanceHierarchy": false,
      "generateKnownTypes": true,
      "generateEnumMappingDescription": false,
      "generateXmlObjects": false,
      "generateAbstractProperties": false,
      "generateAbstractSchemas": true,
      "ignoreObsoleteProperties": false,
      "allowReferencesWithProperties": false,
      "excludedTypeNames": [],
      "serviceHost": null,
      "serviceBasePath": null,
      "serviceSchemes": [],
      "infoTitle": "AI Workflow Platform API",
      "infoDescription": null,
      "infoVersion": "1.0.0",
      "documentTemplate": null,
      "documentProcessorTypes": [],
      "operationProcessorTypes": [],
      "typeNameGeneratorType": null,
      "schemaNameGeneratorType": null,
      "contractResolverType": null,
      "serializerSettingsType": null,
      "useDocumentProvider": true,
      "documentName": "v1",
      "aspNetCoreEnvironment": "Development",
      "createWebHostBuilderMethod": null,
      "startupType": null,
      "allowNullableBodyParameters": true,
      "output": "swagger.json",
      "outputType": "OpenApi3",
      "assemblyPaths": [],
      "assemblyConfig": null,
      "referencePaths": [],
      "useNuGetCache": false
    }
  },
  "codeGenerators": {
    "openApiToTypeScriptClient": {
      "className": "AIWorkflowApiClient",
      "moduleName": "",
      "namespace": "",
      "typeScriptVersion": 5.0,
      "template": "Fetch",
      "promiseType": "Promise",
      "httpClass": "HttpClient",
      "withCredentials": false,
      "useSingletonProvider": false,
      "injectionTokenType": "InjectionToken",
      "rxJsVersion": 7.0,
      "dateTimeType": "Date",
      "nullValue": "Undefined",
      "generateClientClasses": true,
      "generateClientInterfaces": false,
      "generateOptionalParameters": true,
      "exportTypes": true,
      "wrapDtoExceptions": true,
      "exceptionClass": "ApiException",
      "clientBaseClass": null,
      "wrapResponses": false,
      "wrapResponseMethods": [],
      "generateResponseClasses": true,
      "responseClass": "SwaggerResponse",
      "protectedMethods": [],
      "configurationClass": null,
      "useTransformOptionsMethod": false,
      "useTransformResultMethod": false,
      "generateDtoTypes": true,
      "operationGenerationMode": "MultipleClientsFromOperationId",
      "markOptionalProperties": true,
      "generateCloneMethod": false,
      "typeStyle": "Interface",
      "enumStyle": "Enum",
      "useLeafType": false,
      "classTypes": [],
      "extendedClasses": [],
      "extensionCode": null,
      "generateDefaultValues": true,
      "excludedTypeNames": [],
      "excludedParameterNames": [],
      "handleReferences": false,
      "generateConstructorInterface": true,
      "convertConstructorInterfaceData": false,
      "importRequiredTypes": true,
      "useGetBaseUrlMethod": false,
      "baseUrlTokenName": "API_BASE_URL",
      "queryNullValue": "",
      "useAbortSignal": false,
      "inlineNamedDictionaries": false,
      "inlineNamedAny": false,
      "includeHttpContext": false,
      "templateDirectory": null,
      "typeNameGeneratorType": null,
      "propertyNameGeneratorType": null,
      "enumNameGeneratorType": null,
      "serviceHost": null,
      "serviceSchemes": null,
      "output": "../frontend/react-app/src/api/generated/api-client.ts"
    }
  }
}
```

#### 生成客戶端

```bash
nswag run nswag.json
```

生成的 TypeScript 客戶端可直接在 React/Vue 應用中使用:

```typescript
import { AIWorkflowApiClient, WorkflowDto } from '@/api/generated/api-client'

const client = new AIWorkflowApiClient('https://api.aiworkflow.com')

// 獲取工作流列表
const workflows = await client.workflows_GetWorkflows(1, 20)

// 創建工作流
const newWorkflow = await client.workflows_CreateWorkflow({
  name: 'New Workflow',
  description: 'Description'
})
```

---

## 最佳實踐檢查清單

### 配置階段
- [ ] 啟用 XML 文檔生成
- [ ] 配置 Swashbuckle
- [ ] 添加 OpenAPI 元數據
- [ ] 配置 JWT 認證

### 文檔階段
- [ ] 為所有 DTO 添加 XML 註解和示例
- [ ] 為所有端點添加描述和響應說明
- [ ] 提供請求/響應示例
- [ ] 記錄錯誤響應格式

### 多版本階段
- [ ] 配置 API 版本控制
- [ ] 為每個版本生成獨立文檔
- [ ] 標記已棄用的版本

### UI 階段
- [ ] 自定義 Swagger UI 主題
- [ ] 添加公司 Logo 和品牌元素
- [ ] 啟用持久化授權
- [ ] 配置展開級別和過濾

### 生成階段
- [ ] 配置 NSwag 生成 TypeScript 客戶端
- [ ] 將生成的客戶端集成到前端項目
- [ ] 測試生成的代碼

---

**文檔維護**: Tech Lead Team
**最後更新**: 2025-01-01
**狀態**: ✅ 完整
