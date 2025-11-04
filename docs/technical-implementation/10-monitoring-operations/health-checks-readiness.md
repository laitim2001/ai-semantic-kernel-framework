# 健康檢查與就緒探針實施指南

## 概述

### 文檔目的
本文檔提供 AI Agent 工作流平台的完整健康檢查與就緒探針實施方案,涵蓋:
- **健康檢查架構設計** (Liveness, Readiness, Startup Probes)
- **ASP.NET Core Health Checks** (內建 Health Checks 中間件)
- **依賴項健康檢查** (PostgreSQL, Redis, Azure OpenAI, Blob Storage)
- **Kubernetes 探針配置** (HTTP, TCP, Exec Probes)
- **健康檢查端點安全** (認證、授權、速率限制)
- **健康儀表板** (Health Checks UI, Grafana 集成)

### 技術背景
健康檢查是雲原生應用的核心可靠性機制:
- **Liveness Probe (存活探針)**: 檢測應用是否處於死鎖狀態,失敗則重啟 Pod
- **Readiness Probe (就緒探針)**: 檢測應用是否準備好接收流量,失敗則從 Service 移除
- **Startup Probe (啟動探針)**: 檢測應用是否已完成啟動,避免 Liveness Probe 過早殺死應用

**Kubernetes 探針工作流**:
```
Pod 啟動
  │
  ├─ Startup Probe (首次啟動)
  │  ├─ 成功 → 進入 Liveness/Readiness 檢查
  │  └─ 失敗 (超過 failureThreshold) → 重啟 Pod
  │
  ├─ Liveness Probe (運行期間)
  │  ├─ 成功 → 繼續運行
  │  └─ 失敗 (超過 failureThreshold) → 重啟 Pod
  │
  └─ Readiness Probe (運行期間)
     ├─ 成功 → 加入 Service 接收流量
     └─ 失敗 (超過 failureThreshold) → 從 Service 移除
```

**健康檢查最佳實踐**:
- **快速響應**: 健康檢查應在 < 1 秒內完成
- **輕量級**: 避免執行耗時操作 (如複雜數據庫查詢)
- **明確語義**: Liveness 檢查應用狀態,Readiness 檢查依賴項
- **Graceful Degradation**: 部分依賴項失敗不應導致整體 Unhealthy

---

## 健康檢查架構

### 整體架構圖

```
┌─────────────────────────────────────────────────────────────────┐
│ Kubernetes Pod (API Service)                                    │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌───────────────────────────────────────────────────────────┐ │
│  │ ASP.NET Core Application                                  │ │
│  │                                                            │ │
│  │  ┌──────────────────────────────────────────────────────┐ │ │
│  │  │ Health Check Endpoints                               │ │ │
│  │  │ ├─ GET /health/live (Liveness)                       │ │ │
│  │  │ │  └─ 僅檢查應用自身狀態 (無 I/O)                      │ │ │
│  │  │ │                                                     │ │ │
│  │  │ ├─ GET /health/ready (Readiness)                     │ │ │
│  │  │ │  └─ 檢查所有依賴項 (Database, Redis, etc.)          │ │ │
│  │  │ │                                                     │ │ │
│  │  │ ├─ GET /health/startup (Startup)                     │ │ │
│  │  │ │  └─ 檢查啟動必需依賴項 (Database 遷移等)            │ │ │
│  │  │ │                                                     │ │ │
│  │  │ └─ GET /health (綜合健康狀態)                         │ │ │
│  │  │    └─ 所有健康檢查詳細報告 (JSON)                     │ │ │
│  │  └──────────────────────────────────────────────────────┘ │ │
│  │                                                            │ │
│  │  ┌──────────────────────────────────────────────────────┐ │ │
│  │  │ Health Checks                                        │ │ │
│  │  │ ├─ SelfHealthCheck (應用自身)                        │ │ │
│  │  │ ├─ PostgreSQLHealthCheck (數據庫連接)                │ │ │
│  │  │ ├─ RedisHealthCheck (緩存連接)                       │ │ │
│  │  │ ├─ AzureOpenAIHealthCheck (AI 服務)                  │ │ │
│  │  │ ├─ BlobStorageHealthCheck (存儲服務)                 │ │ │
│  │  │ └─ DiskSpaceHealthCheck (磁盤空間)                   │ │ │
│  │  └──────────────────────────────────────────────────────┘ │ │
│  └────────────────────────────────────────────────────────────┘ │
│                          │                                      │
└──────────────────────────┼──────────────────────────────────────┘
                           │
                           │ HTTP Probes
                           ▼
┌─────────────────────────────────────────────────────────────────┐
│ Kubernetes Control Plane (kubelet)                             │
├─────────────────────────────────────────────────────────────────┤
│ ├─ Liveness Probe → GET /health/live (every 10s)               │
│ │  └─ Failure → Restart Pod                                    │
│ │                                                               │
│ ├─ Readiness Probe → GET /health/ready (every 5s)              │
│ │  └─ Failure → Remove from Service                            │
│ │                                                               │
│ └─ Startup Probe → GET /health/startup (every 10s)             │
│    └─ Failure (30 times) → Restart Pod                         │
└─────────────────────────────────────────────────────────────────┘
```

---

## ASP.NET Core Health Checks 實施

### 安裝 NuGet 套件

```bash
dotnet add package Microsoft.Extensions.Diagnostics.HealthChecks
dotnet add package AspNetCore.HealthChecks.NpgSql
dotnet add package AspNetCore.HealthChecks.Redis
dotnet add package AspNetCore.HealthChecks.AzureStorage
dotnet add package AspNetCore.HealthChecks.Uris
dotnet add package AspNetCore.HealthChecks.UI
dotnet add package AspNetCore.HealthChecks.UI.Client
dotnet add package AspNetCore.HealthChecks.UI.InMemory.Storage
```

### Program.cs 配置

```csharp
using Microsoft.Extensions.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// 配置 Health Checks
builder.Services.AddHealthChecks()
    // 1. Self Health Check (應用自身狀態)
    .AddCheck("self", () => HealthCheckResult.Healthy("Application is running"), tags: new[] { "live" })

    // 2. PostgreSQL Health Check (數據庫連接)
    .AddNpgSql(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "postgresql",
        failureStatus: HealthStatus.Degraded,
        tags: new[] { "ready", "db" },
        timeout: TimeSpan.FromSeconds(3))

    // 3. Redis Health Check (緩存連接)
    .AddRedis(
        redisConnectionString: builder.Configuration.GetConnectionString("Redis")!,
        name: "redis",
        failureStatus: HealthStatus.Degraded,
        tags: new[] { "ready", "cache" },
        timeout: TimeSpan.FromSeconds(3))

    // 4. Azure Blob Storage Health Check
    .AddAzureBlobStorage(
        connectionString: builder.Configuration["BlobStorage:ConnectionString"]!,
        name: "azure-blob-storage",
        failureStatus: HealthStatus.Degraded,
        tags: new[] { "ready", "storage" },
        timeout: TimeSpan.FromSeconds(5))

    // 5. Custom Health Checks
    .AddCheck<AzureOpenAIHealthCheck>("azure-openai", tags: new[] { "ready", "ai" })
    .AddCheck<DiskSpaceHealthCheck>("disk-space", tags: new[] { "ready", "infrastructure" });

// Health Checks UI (可選,用於可視化健康狀態)
builder.Services.AddHealthChecksUI(options =>
{
    options.SetEvaluationTimeInSeconds(15);  // 每 15 秒檢查一次
    options.MaximumHistoryEntriesPerEndpoint(50);
    options.AddHealthCheckEndpoint("API Health", "/health");
})
.AddInMemoryStorage();

var app = builder.Build();

// Health Check Endpoints
// 1. Liveness Probe (僅檢查應用自身)
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live"),
    AllowCachingResponses = false
});

// 2. Readiness Probe (檢查所有依賴項)
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    AllowCachingResponses = false
});

// 3. Startup Probe (檢查啟動必需依賴項)
app.MapHealthChecks("/health/startup", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready") && check.Tags.Contains("db"),  // 僅檢查數據庫
    AllowCachingResponses = false
});

// 4. 詳細健康報告 (JSON 格式)
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,  // 返回詳細 JSON
    AllowCachingResponses = false
});

// 5. Health Checks UI (可視化儀表板)
app.MapHealthChecksUI(options =>
{
    options.UIPath = "/health-ui";
    options.ApiPath = "/health-ui-api";
});

app.MapControllers();
app.Run();
```

---

## 自定義健康檢查

### Azure OpenAI Health Check

```csharp
// src/Infrastructure/HealthChecks/AzureOpenAIHealthCheck.cs
using Microsoft.Extensions.Diagnostics.HealthChecks;

public sealed class AzureOpenAIHealthCheck : IHealthCheck
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AzureOpenAIHealthCheck> _logger;

    public AzureOpenAIHealthCheck(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<AzureOpenAIHealthCheck> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Add("api-key", _configuration["AzureOpenAI:ApiKey"]);

            var endpoint = _configuration["AzureOpenAI:Endpoint"];
            var deploymentName = _configuration["AzureOpenAI:DeploymentName"];

            // 發送簡單的 Ping 請求 (不實際調用 Completion API 以節省成本)
            var requestUri = $"{endpoint}/openai/deployments/{deploymentName}?api-version=2024-02-15-preview";

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(5));  // 5 秒超時

            var response = await httpClient.GetAsync(requestUri, cts.Token);

            if (response.IsSuccessStatusCode)
            {
                return HealthCheckResult.Healthy("Azure OpenAI is reachable", new Dictionary<string, object>
                {
                    ["endpoint"] = endpoint,
                    ["deployment"] = deploymentName,
                    ["status_code"] = (int)response.StatusCode
                });
            }

            return HealthCheckResult.Degraded($"Azure OpenAI returned status code: {response.StatusCode}", data: new Dictionary<string, object>
            {
                ["endpoint"] = endpoint,
                ["status_code"] = (int)response.StatusCode
            });
        }
        catch (TaskCanceledException)
        {
            _logger.LogWarning("Azure OpenAI health check timed out");
            return HealthCheckResult.Degraded("Azure OpenAI health check timed out");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Azure OpenAI health check failed");
            return HealthCheckResult.Unhealthy("Azure OpenAI is unreachable", ex, new Dictionary<string, object>
            {
                ["error"] = ex.Message
            });
        }
    }
}
```

### Disk Space Health Check

```csharp
// src/Infrastructure/HealthChecks/DiskSpaceHealthCheck.cs
using Microsoft.Extensions.Diagnostics.HealthChecks;

public sealed class DiskSpaceHealthCheck : IHealthCheck
{
    private readonly ILogger<DiskSpaceHealthCheck> _logger;
    private const long MinimumFreeDiskSpaceGB = 5;  // 5 GB

    public DiskSpaceHealthCheck(ILogger<DiskSpaceHealthCheck> logger)
    {
        _logger = logger;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var driveInfo = new DriveInfo(Path.GetPathRoot(AppContext.BaseDirectory)!);

            var freeSpaceGB = driveInfo.AvailableFreeSpace / (1024.0 * 1024 * 1024);
            var totalSpaceGB = driveInfo.TotalSize / (1024.0 * 1024 * 1024);
            var usedSpaceGB = totalSpaceGB - freeSpaceGB;
            var usedPercentage = (usedSpaceGB / totalSpaceGB) * 100;

            var data = new Dictionary<string, object>
            {
                ["drive"] = driveInfo.Name,
                ["free_space_gb"] = Math.Round(freeSpaceGB, 2),
                ["total_space_gb"] = Math.Round(totalSpaceGB, 2),
                ["used_percentage"] = Math.Round(usedPercentage, 2)
            };

            if (freeSpaceGB < MinimumFreeDiskSpaceGB)
            {
                _logger.LogWarning("Low disk space: {FreeSpaceGB} GB remaining", freeSpaceGB);
                return Task.FromResult(HealthCheckResult.Degraded(
                    $"Low disk space: {freeSpaceGB:F2} GB remaining (minimum: {MinimumFreeDiskSpaceGB} GB)",
                    data: data));
            }

            return Task.FromResult(HealthCheckResult.Healthy(
                $"Disk space OK: {freeSpaceGB:F2} GB free",
                data: data));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Disk space health check failed");
            return Task.FromResult(HealthCheckResult.Unhealthy(
                "Unable to check disk space",
                ex));
        }
    }
}
```

### Database Migration Health Check

```csharp
// src/Infrastructure/HealthChecks/DatabaseMigrationHealthCheck.cs
public sealed class DatabaseMigrationHealthCheck : IHealthCheck
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<DatabaseMigrationHealthCheck> _logger;

    public DatabaseMigrationHealthCheck(
        ApplicationDbContext dbContext,
        ILogger<DatabaseMigrationHealthCheck> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // 檢查是否有待應用的遷移
            var pendingMigrations = await _dbContext.Database.GetPendingMigrationsAsync(cancellationToken);
            var appliedMigrations = await _dbContext.Database.GetAppliedMigrationsAsync(cancellationToken);

            var data = new Dictionary<string, object>
            {
                ["applied_migrations_count"] = appliedMigrations.Count(),
                ["pending_migrations_count"] = pendingMigrations.Count()
            };

            if (pendingMigrations.Any())
            {
                _logger.LogWarning("Database has {Count} pending migrations", pendingMigrations.Count());
                return HealthCheckResult.Degraded(
                    $"Database has {pendingMigrations.Count()} pending migrations",
                    data: data);
            }

            return HealthCheckResult.Healthy(
                "Database migrations are up-to-date",
                data: data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database migration health check failed");
            return HealthCheckResult.Unhealthy(
                "Unable to check database migrations",
                ex);
        }
    }
}
```

---

## Kubernetes 探針配置

### Deployment YAML 配置

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
          name: http
        env:
        - name: ASPNETCORE_URLS
          value: "http://+:8080"

        # 1. Startup Probe (啟動探針)
        # 用途: 保護慢啟動容器,避免 Liveness Probe 過早殺死應用
        # 場景: 數據庫遷移、緩存預熱等耗時啟動任務
        startupProbe:
          httpGet:
            path: /health/startup
            port: 8080
            httpHeaders:
            - name: Host
              value: localhost
          initialDelaySeconds: 0  # 立即開始檢查
          periodSeconds: 10  # 每 10 秒檢查一次
          timeoutSeconds: 5  # 5 秒超時
          successThreshold: 1  # 連續 1 次成功視為啟動完成
          failureThreshold: 30  # 連續 30 次失敗 (5 分鐘) 後重啟

        # 2. Liveness Probe (存活探針)
        # 用途: 檢測應用是否陷入死鎖或無響應狀態
        # 場景: 僅檢查應用自身,不檢查依賴項 (避免級聯失敗)
        livenessProbe:
          httpGet:
            path: /health/live
            port: 8080
            httpHeaders:
            - name: Host
              value: localhost
          initialDelaySeconds: 30  # 啟動後 30 秒開始檢查
          periodSeconds: 10  # 每 10 秒檢查一次
          timeoutSeconds: 5  # 5 秒超時
          successThreshold: 1  # 連續 1 次成功視為健康
          failureThreshold: 3  # 連續 3 次失敗 (30 秒) 後重啟 Pod

        # 3. Readiness Probe (就緒探針)
        # 用途: 檢測應用是否準備好接收流量
        # 場景: 檢查所有依賴項 (數據庫、Redis、外部 API)
        readinessProbe:
          httpGet:
            path: /health/ready
            port: 8080
            httpHeaders:
            - name: Host
              value: localhost
          initialDelaySeconds: 10  # 啟動後 10 秒開始檢查
          periodSeconds: 5  # 每 5 秒檢查一次 (比 Liveness 更頻繁)
          timeoutSeconds: 3  # 3 秒超時
          successThreshold: 1  # 連續 1 次成功視為就緒
          failureThreshold: 3  # 連續 3 次失敗 (15 秒) 後從 Service 移除

        resources:
          requests:
            memory: "512Mi"
            cpu: "250m"
          limits:
            memory: "1Gi"
            cpu: "500m"
```

### TCP 探針 (用於非 HTTP 服務)

```yaml
# k8s/base/worker-deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: worker-deployment
spec:
  template:
    spec:
      containers:
      - name: worker
        livenessProbe:
          tcpSocket:
            port: 8080
          initialDelaySeconds: 30
          periodSeconds: 10
          timeoutSeconds: 5
          failureThreshold: 3
```

### Exec 探針 (執行命令)

```yaml
# k8s/base/database-deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: postgres-deployment
spec:
  template:
    spec:
      containers:
      - name: postgres
        livenessProbe:
          exec:
            command:
            - /bin/sh
            - -c
            - pg_isready -U postgres
          initialDelaySeconds: 30
          periodSeconds: 10
          timeoutSeconds: 5
          failureThreshold: 3
```

---

## 健康檢查響應格式

### Healthy 響應

```http
GET /health HTTP/1.1
Host: localhost:8080

HTTP/1.1 200 OK
Content-Type: application/json

{
  "status": "Healthy",
  "totalDuration": "00:00:00.0234567",
  "entries": {
    "self": {
      "status": "Healthy",
      "description": "Application is running",
      "duration": "00:00:00.0001234"
    },
    "postgresql": {
      "status": "Healthy",
      "description": "PostgreSQL is reachable",
      "duration": "00:00:00.0123456",
      "data": {
        "server_version": "16.1",
        "database": "aiagent_prod"
      }
    },
    "redis": {
      "status": "Healthy",
      "description": "Redis is reachable",
      "duration": "00:00:00.0045678",
      "data": {
        "connected_clients": "10",
        "used_memory_mb": "256"
      }
    },
    "azure-openai": {
      "status": "Healthy",
      "description": "Azure OpenAI is reachable",
      "duration": "00:00:00.1234567",
      "data": {
        "endpoint": "https://prod-openai.openai.azure.com",
        "deployment": "gpt-4-prod",
        "status_code": 200
      }
    }
  }
}
```

### Degraded 響應

```http
GET /health HTTP/1.1

HTTP/1.1 200 OK
Content-Type: application/json

{
  "status": "Degraded",
  "totalDuration": "00:00:03.0234567",
  "entries": {
    "self": {
      "status": "Healthy",
      "description": "Application is running"
    },
    "postgresql": {
      "status": "Healthy",
      "description": "PostgreSQL is reachable"
    },
    "redis": {
      "status": "Degraded",
      "description": "Redis connection pool exhausted",
      "duration": "00:00:03.0000000",
      "data": {
        "connected_clients": "1000",
        "max_clients": "1000"
      }
    }
  }
}
```

### Unhealthy 響應

```http
GET /health HTTP/1.1

HTTP/1.1 503 Service Unavailable
Content-Type: application/json

{
  "status": "Unhealthy",
  "totalDuration": "00:00:05.0234567",
  "entries": {
    "self": {
      "status": "Healthy",
      "description": "Application is running"
    },
    "postgresql": {
      "status": "Unhealthy",
      "description": "Unable to connect to PostgreSQL",
      "duration": "00:00:05.0000000",
      "exception": "Npgsql.NpgsqlException: Connection refused",
      "data": {
        "error": "Connection refused"
      }
    }
  }
}
```

---

## 健康檢查安全

### 限制健康檢查訪問

```csharp
// Program.cs
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
})
.RequireAuthorization("AdminOnly");  // 僅管理員可訪問詳細健康報告

// Liveness/Readiness 無需認證 (Kubernetes 需要訪問)
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live")
});  // 無認證要求
```

### IP 白名單

```csharp
// 自定義中間件: 限制健康檢查僅允許 Kubernetes Service CIDR
app.UseWhen(
    context => context.Request.Path.StartsWithSegments("/health"),
    appBuilder =>
    {
        appBuilder.Use(async (context, next) =>
        {
            var remoteIp = context.Connection.RemoteIpAddress;
            var allowedCIDRs = new[]
            {
                "10.0.0.0/8",      // Kubernetes Service CIDR
                "127.0.0.1/32"     // Localhost
            };

            if (!IsIpAllowed(remoteIp, allowedCIDRs))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Access denied");
                return;
            }

            await next();
        });
    });

static bool IsIpAllowed(IPAddress? ip, string[] allowedCIDRs)
{
    if (ip == null) return false;

    foreach (var cidr in allowedCIDRs)
    {
        // CIDR 匹配邏輯 (簡化示例)
        if (ip.ToString() == "127.0.0.1" || ip.ToString().StartsWith("10."))
            return true;
    }

    return false;
}
```

---

## Health Checks UI

### 配置 UI

```csharp
// Program.cs
builder.Services.AddHealthChecksUI(options =>
{
    options.SetEvaluationTimeInSeconds(15);  // 每 15 秒評估一次
    options.MaximumHistoryEntriesPerEndpoint(100);  // 保留 100 條歷史記錄

    // 添加多個健康檢查端點
    options.AddHealthCheckEndpoint("API", "/health");
    options.AddHealthCheckEndpoint("Worker", "http://worker-service:8080/health");
    options.AddHealthCheckEndpoint("Database", "http://postgres-exporter:9187/health");
})
.AddInMemoryStorage();

var app = builder.Build();

// 啟用 Health Checks UI
app.MapHealthChecksUI(options =>
{
    options.UIPath = "/health-ui";  // UI 路徑: https://api.example.com/health-ui
    options.ApiPath = "/health-ui-api";
});
```

### UI 訪問

訪問 `https://api.aiagent.example.com/health-ui`,可以看到實時健康狀態儀表板:
- **當前健康狀態** (Healthy/Degraded/Unhealthy)
- **各檢查項詳細信息**
- **歷史健康狀態趨勢圖**
- **失敗檢查項錯誤詳情**

---

## Grafana 集成

### Prometheus Exporter 暴露健康指標

```csharp
// src/Infrastructure/Metrics/HealthCheckMetrics.cs
using Prometheus;

public sealed class HealthCheckMetrics : IHealthCheckPublisher
{
    private static readonly Gauge HealthStatus = Metrics.CreateGauge(
        "health_check_status",
        "Health check status (1 = Healthy, 0.5 = Degraded, 0 = Unhealthy)",
        new GaugeConfiguration
        {
            LabelNames = new[] { "check_name" }
        });

    private static readonly Gauge HealthCheckDuration = Metrics.CreateGauge(
        "health_check_duration_seconds",
        "Health check duration in seconds",
        new GaugeConfiguration
        {
            LabelNames = new[] { "check_name" }
        });

    public Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
    {
        foreach (var entry in report.Entries)
        {
            var statusValue = entry.Value.Status switch
            {
                HealthStatus.Healthy => 1.0,
                HealthStatus.Degraded => 0.5,
                HealthStatus.Unhealthy => 0.0,
                _ => 0.0
            };

            HealthStatus.WithLabels(entry.Key).Set(statusValue);
            HealthCheckDuration.WithLabels(entry.Key).Set(entry.Value.Duration.TotalSeconds);
        }

        return Task.CompletedTask;
    }
}

// Program.cs 註冊
builder.Services.AddSingleton<IHealthCheckPublisher, HealthCheckMetrics>();
```

### Grafana Dashboard

```json
{
  "dashboard": {
    "title": "Health Checks Dashboard",
    "panels": [
      {
        "id": 1,
        "title": "Health Check Status",
        "type": "stat",
        "targets": [
          {
            "expr": "health_check_status",
            "legendFormat": "{{check_name}}"
          }
        ],
        "fieldConfig": {
          "defaults": {
            "thresholds": {
              "steps": [
                { "value": 0, "color": "red" },
                { "value": 0.5, "color": "yellow" },
                { "value": 1, "color": "green" }
              ]
            }
          }
        }
      },
      {
        "id": 2,
        "title": "Health Check Duration",
        "type": "graph",
        "targets": [
          {
            "expr": "health_check_duration_seconds",
            "legendFormat": "{{check_name}}"
          }
        ]
      }
    ]
  }
}
```

---

## 最佳實踐

### 1. 探針配置建議

| 探針類型 | initialDelaySeconds | periodSeconds | timeoutSeconds | failureThreshold | 建議用途 |
|---------|---------------------|---------------|----------------|------------------|----------|
| **Startup** | 0 | 10 | 5 | 30 (5 分鐘) | 慢啟動應用 (數據庫遷移、緩存預熱) |
| **Liveness** | 30 | 10 | 5 | 3 (30 秒) | 檢測死鎖、無響應狀態 (僅應用自身) |
| **Readiness** | 10 | 5 | 3 | 3 (15 秒) | 檢測依賴項健康 (數據庫、Redis 等) |

### 2. 健康檢查分離原則

```
Liveness Probe 應該檢查:
✅ 應用進程是否存活
✅ HTTP 服務是否響應
✅ 無死鎖、無無限循環

❌ 不應該檢查:
❌ 數據庫連接 (依賴項失敗不應重啟應用)
❌ 外部 API 可用性
❌ 磁盤空間 (基礎設施問題)

Readiness Probe 應該檢查:
✅ 數據庫連接
✅ Redis 連接
✅ 必需的外部 API
✅ 關鍵配置加載完成

❌ 不應該檢查:
❌ 非關鍵外部 API (應該 Graceful Degradation)
```

### 3. 超時配置

```csharp
// 健康檢查超時應該 < Kubernetes Probe 超時
builder.Services.AddHealthChecks()
    .AddNpgSql(
        connectionString,
        timeout: TimeSpan.FromSeconds(3)  // 健康檢查超時: 3 秒
    );

// Kubernetes Probe 超時: 5 秒
// 3 秒 (健康檢查超時) + 2 秒 (緩衝) = 5 秒 (Probe 超時)
```

---

## 故障排查

### 常見問題

| 問題 | 原因 | 解決方案 |
|------|------|----------|
| **Pod 頻繁重啟** | Liveness Probe 配置過於敏感 | 增加 `initialDelaySeconds` 或 `failureThreshold` |
| **Pod 從 Service 移除** | Readiness Probe 失敗 | 檢查依賴項健康 (數據庫、Redis),調整超時設置 |
| **啟動失敗** | Startup Probe 超時 | 增加 `failureThreshold` (允許更長啟動時間) |
| **健康檢查超時** | 檢查邏輯過於耗時 | 優化健康檢查邏輯,避免複雜查詢 |

### 調試技巧

```bash
# 查看 Pod 事件 (包含探針失敗原因)
kubectl describe pod api-pod-xxxxx -n aiagent-prod

# 手動測試健康檢查端點
kubectl exec -n aiagent-prod api-pod-xxxxx -- curl http://localhost:8080/health/live

# 查看 Pod 日誌 (健康檢查相關錯誤)
kubectl logs -n aiagent-prod api-pod-xxxxx | grep "health"

# 查看 Pod 重啟次數
kubectl get pods -n aiagent-prod -o custom-columns=NAME:.metadata.name,RESTARTS:.status.containerStatuses[0].restartCount
```

---

## 總結

本文檔提供了完整的健康檢查與就緒探針實施方案,涵蓋:

✅ **健康檢查架構** (Liveness, Readiness, Startup Probes)
✅ **ASP.NET Core 集成** (Health Checks 中間件, 自定義 Health Checks)
✅ **依賴項檢查** (PostgreSQL, Redis, Azure OpenAI, Blob Storage)
✅ **Kubernetes 探針** (HTTP, TCP, Exec Probes)
✅ **健康檢查安全** (認證、IP 白名單)
✅ **可視化儀表板** (Health Checks UI, Grafana)

透過本指南,開發與運維團隊可以實現:
- **自動故障恢復**: Liveness Probe 自動重啟死鎖應用
- **流量隔離**: Readiness Probe 自動移除不健康 Pod
- **平滑啟動**: Startup Probe 保護慢啟動應用
- **全面監控**: 依賴項健康狀態實時可見
