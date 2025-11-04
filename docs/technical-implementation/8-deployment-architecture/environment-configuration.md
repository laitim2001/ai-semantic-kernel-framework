# 環境配置管理指南

## 概述

### 文檔目的
本文檔提供 AI Agent 工作流平台的完整環境配置管理方案,涵蓋:
- **多環境配置策略** (Development, Staging, Production)
- **配置管理工具** (appsettings.json, Azure Key Vault, Kubernetes ConfigMap/Secret)
- **環境變量管理與注入**
- **配置熱更新機制**
- **配置驗證與安全性**
- **配置版本控制與審計**

### 技術背景
現代應用程序需要在多個環境中運行,每個環境具有不同的配置需求:
- **開發環境 (Development)**: 本地開發,使用本地數據庫和服務
- **測試環境 (Staging)**: 預生產環境,配置接近生產但使用獨立資源
- **生產環境 (Production)**: 實際用戶訪問,高可用性、安全性要求

**配置管理核心原則**:
1. **環境隔離**: 每個環境獨立配置,避免交叉污染
2. **安全優先**: 敏感信息 (密碼、API Key) 加密存儲
3. **可追溯性**: 配置變更記錄完整審計日誌
4. **易維護性**: 集中管理,避免配置散落各處
5. **自動化**: 配置部署自動化,減少人為錯誤

**ASP.NET Core 配置系統**:
- **分層配置**: appsettings.json → appsettings.{Environment}.json → 環境變量 → Azure Key Vault
- **強類型綁定**: 使用 Options Pattern 避免魔術字符串
- **熱更新支持**: 配置文件變更自動重新加載
- **Provider 擴展**: 支持自定義配置源 (如 Azure App Configuration)

---

## 環境配置架構

### 配置層次結構

```
┌─────────────────────────────────────────────────────┐
│ Layer 1: Base Configuration (appsettings.json)     │
│ - 所有環境共享的默認值                                  │
│ - 非敏感信息 (如日誌級別、超時設置)                        │
└──────────────────┬──────────────────────────────────┘
                   │ Overridden by
┌─────────────────────────────────────────────────────┐
│ Layer 2: Environment-Specific Configuration        │
│ - appsettings.Development.json                     │
│ - appsettings.Staging.json                         │
│ - appsettings.Production.json                      │
└──────────────────┬──────────────────────────────────┘
                   │ Overridden by
┌─────────────────────────────────────────────────────┐
│ Layer 3: Environment Variables                     │
│ - Kubernetes ConfigMap                             │
│ - Docker Compose .env                              │
│ - System Environment Variables                     │
└──────────────────┬──────────────────────────────────┘
                   │ Overridden by
┌─────────────────────────────────────────────────────┐
│ Layer 4: Secrets Management                        │
│ - Azure Key Vault (Production)                     │
│ - Kubernetes Secret (Staging)                      │
│ - User Secrets (Development)                       │
└─────────────────────────────────────────────────────┘
```

### 環境對比表

| 配置項 | Development | Staging | Production |
|--------|-------------|---------|------------|
| **數據庫** | PostgreSQL (本地 Docker) | Azure Database for PostgreSQL (Basic) | Azure Database for PostgreSQL (HA, 副本) |
| **Redis** | Redis (本地 Docker) | Azure Cache for Redis (Standard) | Azure Cache for Redis (Premium, Cluster) |
| **Azure OpenAI** | Shared Dev Instance | Dedicated Staging Instance | Dedicated Production Instance (多區域) |
| **Blob Storage** | Azurite Emulator | Azure Storage (LRS) | Azure Storage (GRS, CDN) |
| **Application Insights** | Disabled | Enabled (Sampling 50%) | Enabled (Sampling 10%, Alerts) |
| **日誌級別** | Debug | Information | Warning |
| **CORS** | Allow All | Specific Origins | Strict Production Origins |
| **HTTPS** | Optional | Required | Required + HSTS |
| **Authentication** | JWT (Simple) | JWT + Azure AD B2C | JWT + Azure AD B2C + MFA |

---

## 配置文件結構

### Base Configuration: `appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  },

  "AllowedHosts": "*",

  "Application": {
    "Name": "AI Agent Platform",
    "Version": "1.0.0",
    "Environment": "Development"
  },

  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=5432;Database=aiagent_dev;User Id=postgres;Password=dev_password;",
    "Redis": "localhost:6379"
  },

  "AzureOpenAI": {
    "Endpoint": "https://your-openai-instance.openai.azure.com/",
    "ApiKey": "",  // 實際值從 Key Vault 或環境變量讀取
    "DeploymentName": "gpt-4",
    "ApiVersion": "2024-02-15-preview",
    "MaxTokens": 4096,
    "Temperature": 0.7,
    "TopP": 0.95,
    "FrequencyPenalty": 0.0,
    "PresencePenalty": 0.0
  },

  "SemanticKernel": {
    "MaxRetries": 3,
    "RetryDelayMilliseconds": 1000,
    "TimeoutSeconds": 60,
    "EnableTelemetry": true
  },

  "BlobStorage": {
    "ConnectionString": "",  // 從 Key Vault 讀取
    "ContainerName": "agent-workflows",
    "MaxFileSizeMB": 10,
    "AllowedFileTypes": [".json", ".yaml", ".yml", ".txt"]
  },

  "Jwt": {
    "Issuer": "https://localhost:5001",
    "Audience": "https://localhost:5001",
    "SecretKey": "",  // 從 Key Vault 讀取
    "AccessTokenExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7,
    "RequireHttpsMetadata": true,
    "ValidateIssuerSigningKey": true,
    "ValidateIssuer": true,
    "ValidateAudience": true,
    "ValidateLifetime": true,
    "ClockSkew": 5  // 分鐘
  },

  "RateLimiting": {
    "EnableRateLimiting": true,
    "GlobalLimit": {
      "PermitLimit": 1000,
      "WindowSeconds": 60,
      "QueueLimit": 100
    },
    "PerUserLimit": {
      "PermitLimit": 100,
      "WindowSeconds": 60,
      "QueueLimit": 10
    }
  },

  "Cors": {
    "AllowedOrigins": ["http://localhost:3000", "http://localhost:5173"],
    "AllowedMethods": ["GET", "POST", "PUT", "DELETE", "PATCH"],
    "AllowedHeaders": ["Content-Type", "Authorization"],
    "AllowCredentials": true,
    "MaxAgeSeconds": 3600
  },

  "HealthChecks": {
    "Enabled": true,
    "DatabaseTimeoutSeconds": 5,
    "RedisTimeoutSeconds": 3,
    "AzureOpenAITimeoutSeconds": 10
  },

  "BackgroundJobs": {
    "EnableScheduledJobs": true,
    "WorkflowExecutionMaxConcurrency": 10,
    "JobPollingIntervalSeconds": 5,
    "JobTimeoutMinutes": 30
  }
}
```

### Development Configuration: `appsettings.Development.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    },
    "Console": {
      "FormatterName": "simple",
      "FormatterOptions": {
        "SingleLine": false,
        "IncludeScopes": true,
        "TimestampFormat": "yyyy-MM-dd HH:mm:ss "
      }
    }
  },

  "Application": {
    "Environment": "Development",
    "EnableSwagger": true,
    "EnableDetailedErrors": true
  },

  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=5432;Database=aiagent_dev;User Id=postgres;Password=dev_password;Include Error Detail=true;",
    "Redis": "localhost:6379,abortConnect=false"
  },

  "AzureOpenAI": {
    "Endpoint": "https://dev-openai.openai.azure.com/",
    "DeploymentName": "gpt-4-dev",
    "MaxTokens": 2048,
    "EnableCaching": false
  },

  "BlobStorage": {
    "ConnectionString": "UseDevelopmentStorage=true",  // Azurite Emulator
    "ContainerName": "dev-workflows"
  },

  "Jwt": {
    "Issuer": "https://localhost:5001",
    "Audience": "https://localhost:5001",
    "SecretKey": "dev-secret-key-change-in-production-minimum-32-characters",
    "RequireHttpsMetadata": false,
    "AccessTokenExpirationMinutes": 1440  // 24 小時,開發便利性
  },

  "RateLimiting": {
    "EnableRateLimiting": false  // 開發環境禁用限流
  },

  "Cors": {
    "AllowedOrigins": ["*"],  // 開發環境允許所有來源
    "AllowedMethods": ["*"],
    "AllowedHeaders": ["*"]
  },

  "ApplicationInsights": {
    "ConnectionString": "",  // 開發環境禁用
    "EnableAdaptiveSampling": false
  },

  "BackgroundJobs": {
    "EnableScheduledJobs": true,
    "JobPollingIntervalSeconds": 10  // 開發環境降低頻率
  }
}
```

### Staging Configuration: `appsettings.Staging.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  },

  "Application": {
    "Environment": "Staging",
    "EnableSwagger": true,
    "EnableDetailedErrors": false
  },

  "ConnectionStrings": {
    "DefaultConnection": "",  // 從 Azure Key Vault 讀取
    "Redis": ""  // 從 Azure Key Vault 讀取
  },

  "AzureOpenAI": {
    "Endpoint": "https://staging-openai.openai.azure.com/",
    "DeploymentName": "gpt-4-staging",
    "MaxTokens": 4096,
    "EnableCaching": true
  },

  "BlobStorage": {
    "ConnectionString": "",  // 從 Azure Key Vault 讀取
    "ContainerName": "staging-workflows"
  },

  "Jwt": {
    "Issuer": "https://staging.aiagent.example.com",
    "Audience": "https://staging.aiagent.example.com",
    "SecretKey": "",  // 從 Azure Key Vault 讀取
    "RequireHttpsMetadata": true,
    "AccessTokenExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  },

  "RateLimiting": {
    "EnableRateLimiting": true,
    "GlobalLimit": {
      "PermitLimit": 500,
      "WindowSeconds": 60
    }
  },

  "Cors": {
    "AllowedOrigins": [
      "https://staging.aiagent.example.com",
      "https://staging-admin.aiagent.example.com"
    ],
    "AllowedMethods": ["GET", "POST", "PUT", "DELETE", "PATCH"],
    "AllowedHeaders": ["Content-Type", "Authorization"]
  },

  "ApplicationInsights": {
    "ConnectionString": "",  // 從 Azure Key Vault 讀取
    "EnableAdaptiveSampling": true,
    "SamplingPercentage": 50
  },

  "HealthChecks": {
    "Enabled": true
  },

  "BackgroundJobs": {
    "EnableScheduledJobs": true,
    "WorkflowExecutionMaxConcurrency": 5,
    "JobPollingIntervalSeconds": 5
  }
}
```

### Production Configuration: `appsettings.Production.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Error",
      "Microsoft.EntityFrameworkCore": "Error",
      "AIAgentPlatform": "Information"
    }
  },

  "Application": {
    "Environment": "Production",
    "EnableSwagger": false,
    "EnableDetailedErrors": false
  },

  "ConnectionStrings": {
    "DefaultConnection": "",  // 從 Azure Key Vault 讀取
    "Redis": ""  // 從 Azure Key Vault 讀取
  },

  "AzureOpenAI": {
    "Endpoint": "https://prod-openai.openai.azure.com/",
    "ApiKey": "",  // 從 Azure Key Vault 讀取
    "DeploymentName": "gpt-4-prod",
    "MaxTokens": 4096,
    "Temperature": 0.7,
    "EnableCaching": true,
    "EnableRetry": true,
    "MaxRetries": 3
  },

  "BlobStorage": {
    "ConnectionString": "",  // 從 Azure Key Vault 讀取
    "ContainerName": "prod-workflows",
    "EnableCDN": true,
    "CDNEndpoint": "https://cdn.aiagent.example.com"
  },

  "Jwt": {
    "Issuer": "https://aiagent.example.com",
    "Audience": "https://aiagent.example.com",
    "SecretKey": "",  // 從 Azure Key Vault 讀取
    "RequireHttpsMetadata": true,
    "AccessTokenExpirationMinutes": 30,  // 生產環境縮短過期時間
    "RefreshTokenExpirationDays": 7,
    "ValidateIssuerSigningKey": true,
    "ValidateIssuer": true,
    "ValidateAudience": true,
    "ValidateLifetime": true
  },

  "RateLimiting": {
    "EnableRateLimiting": true,
    "GlobalLimit": {
      "PermitLimit": 1000,
      "WindowSeconds": 60,
      "QueueLimit": 100
    },
    "PerUserLimit": {
      "PermitLimit": 100,
      "WindowSeconds": 60,
      "QueueLimit": 10
    }
  },

  "Cors": {
    "AllowedOrigins": [
      "https://aiagent.example.com",
      "https://admin.aiagent.example.com"
    ],
    "AllowedMethods": ["GET", "POST", "PUT", "DELETE", "PATCH"],
    "AllowedHeaders": ["Content-Type", "Authorization"],
    "AllowCredentials": true,
    "MaxAgeSeconds": 3600
  },

  "ApplicationInsights": {
    "ConnectionString": "",  // 從 Azure Key Vault 讀取
    "EnableAdaptiveSampling": true,
    "SamplingPercentage": 10,  // 生產環境降低採樣率以減少成本
    "EnableHeartbeat": true,
    "EnableDependencyTracking": true,
    "EnableRequestTracking": true,
    "EnableExceptionTracking": true
  },

  "HealthChecks": {
    "Enabled": true,
    "DatabaseTimeoutSeconds": 5,
    "RedisTimeoutSeconds": 3,
    "AzureOpenAITimeoutSeconds": 10
  },

  "BackgroundJobs": {
    "EnableScheduledJobs": true,
    "WorkflowExecutionMaxConcurrency": 20,
    "JobPollingIntervalSeconds": 5,
    "JobTimeoutMinutes": 30
  },

  "Security": {
    "EnableHsts": true,
    "HstsMaxAgeSeconds": 31536000,  // 1 年
    "EnableHttpsRedirection": true,
    "RequireHttps": true
  }
}
```

---

## 強類型配置綁定

### Options Pattern 實現

**配置類定義**:

```csharp
// src/Api/Configuration/AzureOpenAIOptions.cs
namespace AIAgentPlatform.Api.Configuration;

public sealed class AzureOpenAIOptions
{
    public const string SectionName = "AzureOpenAI";

    public required string Endpoint { get; init; }
    public required string ApiKey { get; init; }
    public required string DeploymentName { get; init; }
    public string ApiVersion { get; init; } = "2024-02-15-preview";
    public int MaxTokens { get; init; } = 4096;
    public double Temperature { get; init; } = 0.7;
    public double TopP { get; init; } = 0.95;
    public double FrequencyPenalty { get; init; } = 0.0;
    public double PresencePenalty { get; init; } = 0.0;
    public bool EnableCaching { get; init; } = true;
    public bool EnableRetry { get; init; } = true;
    public int MaxRetries { get; init; } = 3;
}

// src/Api/Configuration/JwtOptions.cs
public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required string SecretKey { get; init; }
    public int AccessTokenExpirationMinutes { get; init; } = 60;
    public int RefreshTokenExpirationDays { get; init; } = 7;
    public bool RequireHttpsMetadata { get; init; } = true;
    public bool ValidateIssuerSigningKey { get; init; } = true;
    public bool ValidateIssuer { get; init; } = true;
    public bool ValidateAudience { get; init; } = true;
    public bool ValidateLifetime { get; init; } = true;
    public int ClockSkew { get; init; } = 5;
}

// src/Api/Configuration/ApplicationOptions.cs
public sealed class ApplicationOptions
{
    public const string SectionName = "Application";

    public required string Name { get; init; }
    public required string Version { get; init; }
    public required string Environment { get; init; }
    public bool EnableSwagger { get; init; } = false;
    public bool EnableDetailedErrors { get; init; } = false;
}
```

**註冊配置**:

```csharp
// src/Api/Program.cs
using AIAgentPlatform.Api.Configuration;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// 註冊強類型配置
builder.Services.Configure<AzureOpenAIOptions>(
    builder.Configuration.GetSection(AzureOpenAIOptions.SectionName));

builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection(JwtOptions.SectionName));

builder.Services.Configure<ApplicationOptions>(
    builder.Configuration.GetSection(ApplicationOptions.SectionName));

// 配置驗證 (啟動時驗證)
builder.Services.AddOptions<AzureOpenAIOptions>()
    .BindConfiguration(AzureOpenAIOptions.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();
```

**使用配置**:

```csharp
// src/Application/Services/OpenAIService.cs
using Microsoft.Extensions.Options;

public sealed class OpenAIService : IOpenAIService
{
    private readonly AzureOpenAIOptions _options;
    private readonly ILogger<OpenAIService> _logger;

    public OpenAIService(
        IOptions<AzureOpenAIOptions> options,
        ILogger<OpenAIService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<string> GenerateCompletionAsync(string prompt, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Generating completion using deployment: {DeploymentName}, MaxTokens: {MaxTokens}",
            _options.DeploymentName,
            _options.MaxTokens);

        // 使用配置值
        var kernel = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(
                deploymentName: _options.DeploymentName,
                endpoint: _options.Endpoint,
                apiKey: _options.ApiKey)
            .Build();

        // ...
    }
}
```

---

## 環境變量管理

### Kubernetes ConfigMap

```yaml
# k8s/base/configmap.yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: aiagent-config
  namespace: aiagent-prod
data:
  ASPNETCORE_ENVIRONMENT: "Production"
  Application__Name: "AI Agent Platform"
  Application__Version: "1.0.0"
  Logging__LogLevel__Default: "Warning"
  Logging__LogLevel__Microsoft.AspNetCore: "Error"
  RateLimiting__EnableRateLimiting: "true"
  RateLimiting__GlobalLimit__PermitLimit: "1000"
  RateLimiting__GlobalLimit__WindowSeconds: "60"
  HealthChecks__Enabled: "true"
  BackgroundJobs__EnableScheduledJobs: "true"
  BackgroundJobs__WorkflowExecutionMaxConcurrency: "20"
  Cors__AllowedOrigins__0: "https://aiagent.example.com"
  Cors__AllowedOrigins__1: "https://admin.aiagent.example.com"
```

### Kubernetes Secret (敏感信息)

```yaml
# k8s/base/secret.yaml
apiVersion: v1
kind: Secret
metadata:
  name: aiagent-secrets
  namespace: aiagent-prod
type: Opaque
stringData:
  ConnectionStrings__DefaultConnection: "Server=prod-postgres.postgres.database.azure.com;Port=5432;Database=aiagent_prod;User Id=aiagent_admin;Password=${DB_PASSWORD};SslMode=Require;"
  ConnectionStrings__Redis: "prod-redis.redis.cache.windows.net:6380,password=${REDIS_PASSWORD},ssl=True,abortConnect=False"
  AzureOpenAI__ApiKey: "${AZURE_OPENAI_API_KEY}"
  Jwt__SecretKey: "${JWT_SECRET_KEY}"
  BlobStorage__ConnectionString: "DefaultEndpointsProtocol=https;AccountName=prodaiagent;AccountKey=${STORAGE_ACCOUNT_KEY};EndpointSuffix=core.windows.net"
  ApplicationInsights__ConnectionString: "InstrumentationKey=${APPINSIGHTS_KEY};IngestionEndpoint=https://eastus-8.in.applicationinsights.azure.com/"
```

**創建 Secret (使用外部值)**:

```bash
# 從 Azure Key Vault 讀取並創建 Kubernetes Secret
DB_PASSWORD=$(az keyvault secret show --name db-password --vault-name aiagent-keyvault --query value -o tsv)
REDIS_PASSWORD=$(az keyvault secret show --name redis-password --vault-name aiagent-keyvault --query value -o tsv)
AZURE_OPENAI_API_KEY=$(az keyvault secret show --name openai-api-key --vault-name aiagent-keyvault --query value -o tsv)

kubectl create secret generic aiagent-secrets \
  --from-literal=ConnectionStrings__DefaultConnection="Server=prod-postgres.postgres.database.azure.com;Port=5432;Database=aiagent_prod;User Id=aiagent_admin;Password=${DB_PASSWORD};SslMode=Require;" \
  --from-literal=ConnectionStrings__Redis="prod-redis.redis.cache.windows.net:6380,password=${REDIS_PASSWORD},ssl=True,abortConnect=False" \
  --from-literal=AzureOpenAI__ApiKey="${AZURE_OPENAI_API_KEY}" \
  --namespace aiagent-prod
```

### Deployment 注入配置

```yaml
# k8s/base/api-deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: api-deployment
  namespace: aiagent-prod
spec:
  replicas: 3
  selector:
    matchLabels:
      app: api
  template:
    metadata:
      labels:
        app: api
    spec:
      containers:
      - name: api
        image: aiagentplatform.azurecr.io/api:latest
        ports:
        - containerPort: 8080
        envFrom:
        - configMapRef:
            name: aiagent-config  # 注入 ConfigMap
        - secretRef:
            name: aiagent-secrets  # 注入 Secret
        env:
        # 可以單獨覆蓋特定環境變量
        - name: ASPNETCORE_URLS
          value: "http://+:8080"
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        resources:
          requests:
            memory: "512Mi"
            cpu: "250m"
          limits:
            memory: "1Gi"
            cpu: "500m"
```

---

## Azure Key Vault 集成

### Key Vault 配置

**創建 Key Vault**:

```bash
# 創建 Key Vault
az keyvault create \
  --name aiagent-keyvault \
  --resource-group rg-aiagent-prod \
  --location eastus \
  --enable-rbac-authorization false \
  --enabled-for-deployment true

# 設置訪問策略
az keyvault set-policy \
  --name aiagent-keyvault \
  --spn <service-principal-id> \
  --secret-permissions get list

# 添加 Secrets
az keyvault secret set \
  --vault-name aiagent-keyvault \
  --name db-password \
  --value "SuperSecurePassword123!"

az keyvault secret set \
  --vault-name aiagent-keyvault \
  --name redis-password \
  --value "RedisSecurePassword456!"

az keyvault secret set \
  --vault-name aiagent-keyvault \
  --name openai-api-key \
  --value "sk-..."

az keyvault secret set \
  --vault-name aiagent-keyvault \
  --name jwt-secret-key \
  --value "jwt-super-secret-key-minimum-32-characters-long"
```

### ASP.NET Core 集成

**安裝 NuGet 套件**:

```bash
dotnet add package Azure.Identity
dotnet add package Azure.Extensions.AspNetCore.Configuration.Secrets
```

**Program.cs 配置**:

```csharp
// src/Api/Program.cs
using Azure.Identity;
using Azure.Extensions.AspNetCore.Configuration.Secrets;

var builder = WebApplication.CreateBuilder(args);

// 僅在非開發環境中啟用 Key Vault
if (!builder.Environment.IsDevelopment())
{
    var keyVaultEndpoint = builder.Configuration["KeyVault:Endpoint"]
        ?? throw new InvalidOperationException("KeyVault:Endpoint not configured");

    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultEndpoint),
        new DefaultAzureCredential());  // 使用 Managed Identity
}
else
{
    // 開發環境使用 User Secrets
    builder.Configuration.AddUserSecrets<Program>();
}

var app = builder.Build();
app.Run();
```

**appsettings.Production.json 配置**:

```json
{
  "KeyVault": {
    "Endpoint": "https://aiagent-keyvault.vault.azure.net/"
  }
}
```

### Key Vault 命名約定

```
Secret Name in Key Vault       →  Configuration Key
───────────────────────────────────────────────────────────
db-password                     →  db:password
ConnectionStrings--DefaultConnection  →  ConnectionStrings:DefaultConnection
AzureOpenAI--ApiKey             →  AzureOpenAI:ApiKey
Jwt--SecretKey                  →  Jwt:SecretKey
```

**注意**: Key Vault 中的 `--` 會自動轉換為 `:` 用於配置層次結構。

---

## 配置驗證

### DataAnnotations 驗證

```csharp
// src/Api/Configuration/AzureOpenAIOptions.cs
using System.ComponentModel.DataAnnotations;

public sealed class AzureOpenAIOptions
{
    public const string SectionName = "AzureOpenAI";

    [Required(ErrorMessage = "Azure OpenAI Endpoint is required")]
    [Url(ErrorMessage = "Azure OpenAI Endpoint must be a valid URL")]
    public required string Endpoint { get; init; }

    [Required(ErrorMessage = "Azure OpenAI API Key is required")]
    [MinLength(32, ErrorMessage = "API Key must be at least 32 characters")]
    public required string ApiKey { get; init; }

    [Required(ErrorMessage = "Deployment Name is required")]
    public required string DeploymentName { get; init; }

    [Range(1, 128000, ErrorMessage = "MaxTokens must be between 1 and 128000")]
    public int MaxTokens { get; init; } = 4096;

    [Range(0.0, 2.0, ErrorMessage = "Temperature must be between 0.0 and 2.0")]
    public double Temperature { get; init; } = 0.7;
}
```

### 自定義驗證邏輯

```csharp
// src/Api/Configuration/Validation/AzureOpenAIOptionsValidator.cs
using Microsoft.Extensions.Options;

public sealed class AzureOpenAIOptionsValidator : IValidateOptions<AzureOpenAIOptions>
{
    public ValidateOptionsResult Validate(string? name, AzureOpenAIOptions options)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(options.Endpoint))
            errors.Add("Azure OpenAI Endpoint cannot be empty");

        if (!Uri.TryCreate(options.Endpoint, UriKind.Absolute, out var uri))
            errors.Add("Azure OpenAI Endpoint must be a valid absolute URI");

        if (string.IsNullOrWhiteSpace(options.ApiKey))
            errors.Add("Azure OpenAI API Key cannot be empty");

        if (options.MaxTokens < 1 || options.MaxTokens > 128000)
            errors.Add("MaxTokens must be between 1 and 128000");

        if (options.Temperature < 0.0 || options.Temperature > 2.0)
            errors.Add("Temperature must be between 0.0 and 2.0");

        if (errors.Any())
            return ValidateOptionsResult.Fail(errors);

        return ValidateOptionsResult.Success;
    }
}

// Program.cs 註冊
builder.Services.AddSingleton<IValidateOptions<AzureOpenAIOptions>, AzureOpenAIOptionsValidator>();
builder.Services.AddOptions<AzureOpenAIOptions>()
    .BindConfiguration(AzureOpenAIOptions.SectionName)
    .ValidateOnStart();  // 啟動時驗證
```

---

## 配置熱更新

### 使用 IOptionsSnapshot

```csharp
// 每個請求重新讀取配置 (支持熱更新)
public sealed class MyService
{
    private readonly IOptionsSnapshot<AzureOpenAIOptions> _optionsSnapshot;

    public MyService(IOptionsSnapshot<AzureOpenAIOptions> optionsSnapshot)
    {
        _optionsSnapshot = optionsSnapshot;
    }

    public void DoSomething()
    {
        // 每次訪問時獲取最新配置
        var currentOptions = _optionsSnapshot.Value;
        Console.WriteLine($"Current MaxTokens: {currentOptions.MaxTokens}");
    }
}
```

### 使用 IOptionsMonitor

```csharp
// 監聽配置變更事件
public sealed class ConfigurationMonitorService : BackgroundService
{
    private readonly IOptionsMonitor<AzureOpenAIOptions> _optionsMonitor;
    private readonly ILogger<ConfigurationMonitorService> _logger;

    public ConfigurationMonitorService(
        IOptionsMonitor<AzureOpenAIOptions> optionsMonitor,
        ILogger<ConfigurationMonitorService> logger)
    {
        _optionsMonitor = optionsMonitor;
        _logger = logger;

        // 註冊配置變更回調
        _optionsMonitor.OnChange((options, name) =>
        {
            _logger.LogWarning(
                "Azure OpenAI configuration changed. New MaxTokens: {MaxTokens}",
                options.MaxTokens);
        });
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }
}
```

### Kubernetes ConfigMap 熱更新

```yaml
# 使用 Volume 掛載 ConfigMap 實現熱更新
apiVersion: apps/v1
kind: Deployment
metadata:
  name: api-deployment
spec:
  template:
    spec:
      containers:
      - name: api
        volumeMounts:
        - name: config-volume
          mountPath: /app/config
          readOnly: true
      volumes:
      - name: config-volume
        configMap:
          name: aiagent-config
```

**更新 ConfigMap**:

```bash
# 更新 ConfigMap
kubectl create configmap aiagent-config \
  --from-file=appsettings.Production.json \
  --dry-run=client -o yaml | kubectl apply -f -

# Pod 會在 1-2 分鐘內自動獲取新配置 (無需重啟)
```

---

## 安全性最佳實踐

### 1. Secrets 加密存儲

```bash
# 開發環境: 使用 User Secrets (不提交到 Git)
dotnet user-secrets init --project src/Api
dotnet user-secrets set "AzureOpenAI:ApiKey" "sk-dev-key-123"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;..."

# 查看已設置的 Secrets
dotnet user-secrets list --project src/Api

# 生產環境: 使用 Azure Key Vault
az keyvault secret set \
  --vault-name aiagent-keyvault \
  --name AzureOpenAI--ApiKey \
  --value "sk-prod-key-456"
```

### 2. 敏感信息過濾

```csharp
// 日誌中自動過濾敏感信息
public sealed class SensitiveDataFilter : ILoggerProvider
{
    private static readonly Regex[] SensitivePatterns =
    [
        new Regex(@"password[\""\s:=]+([^\s,}""]+)", RegexOptions.IgnoreCase),
        new Regex(@"api[_-]?key[\""\s:=]+([^\s,}""]+)", RegexOptions.IgnoreCase),
        new Regex(@"secret[\""\s:=]+([^\s,}""]+)", RegexOptions.IgnoreCase),
        new Regex(@"token[\""\s:=]+([^\s,}""]+)", RegexOptions.IgnoreCase)
    ];

    public ILogger CreateLogger(string categoryName)
    {
        return new SensitiveDataLogger(categoryName);
    }

    private sealed class SensitiveDataLogger : ILogger
    {
        private readonly string _categoryName;

        public SensitiveDataLogger(string categoryName)
        {
            _categoryName = categoryName;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var message = formatter(state, exception);

            // 替換敏感信息
            foreach (var pattern in SensitivePatterns)
            {
                message = pattern.Replace(message, "$0***REDACTED***");
            }

            Console.WriteLine($"[{logLevel}] {_categoryName}: {message}");
        }

        public bool IsEnabled(LogLevel logLevel) => true;
        public IDisposable BeginScope<TState>(TState state) where TState : notnull => default!;
    }

    public void Dispose() { }
}
```

### 3. 配置文件權限控制

```bash
# Linux/macOS: 限制配置文件權限
chmod 600 appsettings.Production.json

# Kubernetes: 使用 RBAC 限制 Secret 訪問
kubectl create role secret-reader \
  --verb=get,list \
  --resource=secrets \
  --namespace=aiagent-prod

kubectl create rolebinding api-secret-reader \
  --role=secret-reader \
  --serviceaccount=aiagent-prod:api-service-account \
  --namespace=aiagent-prod
```

---

## 故障排查

### 常見問題

| 問題 | 原因 | 解決方案 |
|------|------|----------|
| **配置值為 null** | 配置路徑錯誤或環境變量未注入 | 檢查 `appsettings.{Environment}.json` 和環境變量名稱 (區分大小寫) |
| **Key Vault 訪問拒絕** | Managed Identity 權限不足 | 檢查 Key Vault Access Policy: `az keyvault set-policy ...` |
| **配置驗證失敗** | 必需配置項缺失 | 啟動日誌會顯示詳細錯誤,檢查 `ValidateOnStart()` 輸出 |
| **熱更新未生效** | 使用了 `IOptions<T>` 而非 `IOptionsSnapshot<T>` | 改用 `IOptionsSnapshot<T>` 或 `IOptionsMonitor<T>` |

### 調試配置

```csharp
// Program.cs: 啟動時打印所有配置 (僅開發環境)
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    var configuration = app.Services.GetRequiredService<IConfiguration>();
    var configRoot = (IConfigurationRoot)configuration;

    Console.WriteLine("Configuration Sources:");
    foreach (var provider in configRoot.Providers)
    {
        Console.WriteLine($"  - {provider.GetType().Name}");
    }

    Console.WriteLine("\nConfiguration Values:");
    foreach (var pair in configuration.AsEnumerable())
    {
        var value = pair.Value?.Contains("password", StringComparison.OrdinalIgnoreCase) == true
            ? "***REDACTED***"
            : pair.Value;
        Console.WriteLine($"  {pair.Key} = {value}");
    }
}

app.Run();
```

---

## 配置檢查清單

- [ ] **配置文件結構**
  - [ ] 基礎配置 (`appsettings.json`) 包含所有默認值
  - [ ] 環境特定配置 (`appsettings.{Environment}.json`) 覆蓋必要值
  - [ ] 配置文件不包含敏感信息 (密碼、API Key)

- [ ] **Secrets 管理**
  - [ ] 開發環境使用 User Secrets (`dotnet user-secrets`)
  - [ ] 生產環境使用 Azure Key Vault
  - [ ] Kubernetes Secret 使用外部值注入 (不硬編碼)

- [ ] **配置驗證**
  - [ ] 所有 Options 類使用 DataAnnotations 驗證
  - [ ] 啟用 `ValidateOnStart()` 提前發現配置錯誤
  - [ ] 自定義驗證邏輯處理複雜規則

- [ ] **環境隔離**
  - [ ] 每個環境使用獨立的資源 (數據庫、Redis、Key Vault)
  - [ ] 環境變量正確注入 (ConfigMap + Secret)
  - [ ] CORS、JWT、Rate Limiting 按環境配置

- [ ] **安全性**
  - [ ] 敏感信息加密存儲
  - [ ] 日誌中過濾密碼、API Key
  - [ ] HTTPS 在 Staging/Production 強制啟用

---

## 總結

本文檔提供了完整的環境配置管理方案,涵蓋:

✅ **多環境配置策略** (Development/Staging/Production)
✅ **分層配置架構** (appsettings.json → 環境變量 → Key Vault)
✅ **強類型配置綁定** (Options Pattern)
✅ **Secrets 安全管理** (User Secrets, Azure Key Vault, Kubernetes Secret)
✅ **配置驗證與熱更新**
✅ **Kubernetes ConfigMap/Secret 集成**

透過本指南,開發團隊可以實現:
- **環境隔離**: 開發/測試/生產環境完全獨立
- **安全合規**: 敏感信息加密存儲,絕不提交到 Git
- **易維護性**: 集中管理,強類型綁定避免錯誤
- **靈活部署**: 支持 Docker、Kubernetes、Azure App Service
