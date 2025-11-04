# 性能監控實施指南

## 概述

### 文檔目的
本文檔提供 AI Agent 工作流平台的完整性能監控實施方案,涵蓋:
- **應用性能監控 (APM)** (Application Insights, OpenTelemetry)
- **資源利用率監控** (CPU, Memory, Disk, Network)
- **數據庫性能監控** (Query Performance, Connection Pooling, Slow Queries)
- **外部依賴性能** (Azure OpenAI, Redis, Blob Storage)
- **用戶體驗監控** (Real User Monitoring, Synthetic Monitoring)
- **性能基準與優化** (Load Testing, Performance Budgets, Optimization Strategies)

### 技術背景
性能監控是確保系統可靠性和用戶滿意度的核心實踐:
- **黃金信號 (Golden Signals)**: Latency, Traffic, Errors, Saturation
- **USE 方法**: Utilization, Saturation, Errors (針對資源)
- **RED 方法**: Rate, Errors, Duration (針對請求)
- **分佈式追蹤**: 跨服務追蹤請求路徑,識別瓶頸

**性能監控目標**:
- **及時發現**: 在用戶報告前發現性能問題
- **快速定位**: 快速識別性能瓶頸根因
- **持續優化**: 建立性能基準,持續改進
- **容量規劃**: 基於趨勢預測資源需求

---

## 性能監控架構

### 整體架構圖

```
┌─────────────────────────────────────────────────────────────────┐
│ Frontend (React/Vue)                                            │
├─────────────────────────────────────────────────────────────────┤
│ ├─ Real User Monitoring (RUM)                                   │
│ │  ├─ Page Load Time, Core Web Vitals                          │
│ │  ├─ JavaScript Errors, AJAX Request Timing                   │
│ │  └─ User Journey Tracking                                    │
│ │                                                               │
│ ├─ Application Insights Browser SDK                            │
│ │  └─ Auto-instrumentation                                     │
└──────────────────────┬──────────────────────────────────────────┘
                       │
                       │ Telemetry Data
                       ▼
┌─────────────────────────────────────────────────────────────────┐
│ Backend (ASP.NET Core API)                                      │
├─────────────────────────────────────────────────────────────────┤
│ ├─ OpenTelemetry Instrumentation                               │
│ │  ├─ HTTP Request/Response (Duration, Status)                 │
│ │  ├─ Database Queries (EF Core Instrumentation)               │
│ │  ├─ External HTTP Calls (Azure OpenAI, etc.)                 │
│ │  └─ Custom Business Metrics                                  │
│ │                                                               │
│ ├─ Application Insights SDK                                    │
│ │  ├─ Dependency Tracking                                      │
│ │  ├─ Exception Tracking                                       │
│ │  └─ Custom Events & Metrics                                  │
│ │                                                               │
│ └─ Prometheus Metrics Exporter                                 │
│    ├─ /metrics endpoint                                        │
│    └─ Histogram/Summary for latency distribution              │
└──────────────────────┬──────────────────────────────────────────┘
                       │
                       │ Export to Multiple Backends
       ┌───────────────┼───────────────┬───────────────┐
       │               │               │               │
┌──────▼──────┐ ┌──────▼──────┐ ┌─────▼──────┐ ┌─────▼──────┐
│ Application │ │ Prometheus  │ │ Azure      │ │ Jaeger     │
│ Insights    │ │ (Metrics)   │ │ Monitor    │ │ (Traces)   │
│ (APM)       │ │             │ │ (Logs)     │ │            │
└─────────────┘ └─────────────┘ └────────────┘ └────────────┘
       │               │               │               │
       └───────────────┴───────────────┴───────────────┘
                       │
                       ▼
┌─────────────────────────────────────────────────────────────────┐
│ Visualization & Alerting                                        │
│ ├─ Grafana Dashboards (Real-time Metrics)                      │
│ ├─ Application Insights Dashboard (APM Overview)               │
│ ├─ Alertmanager (Performance Alerts)                           │
│ └─ Custom Reports (Daily/Weekly Performance Summary)           │
└─────────────────────────────────────────────────────────────────┘
```

---

## Application Insights 集成

### ASP.NET Core 配置

**安裝 NuGet 套件**:

```bash
dotnet add package Microsoft.ApplicationInsights.AspNetCore
dotnet add package Microsoft.ApplicationInsights.DependencyCollector
```

**Program.cs 配置**:

```csharp
using Microsoft.ApplicationInsights.Extensibility;

var builder = WebApplication.CreateBuilder(args);

// 配置 Application Insights
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
    options.EnableAdaptiveSampling = true;  // 自適應採樣
    options.EnableQuickPulseMetricStream = true;  // Live Metrics
});

// 配置採樣 (生產環境降低成本)
builder.Services.Configure<TelemetryConfiguration>(config =>
{
    var builder = config.DefaultTelemetrySink.TelemetryProcessorChainBuilder;

    // 固定採樣率 (10%)
    builder.UseAdaptiveSampling(maxTelemetryItemsPerSecond: 5, excludedTypes: "Exception");

    builder.Build();
});

// 添加自定義 Telemetry Initializer
builder.Services.AddSingleton<ITelemetryInitializer, CustomTelemetryInitializer>();

var app = builder.Build();

app.MapControllers();
app.Run();
```

### 自定義 Telemetry Initializer

```csharp
// src/Infrastructure/Telemetry/CustomTelemetryInitializer.cs
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

public sealed class CustomTelemetryInitializer : ITelemetryInitializer
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CustomTelemetryInitializer(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Initialize(ITelemetry telemetry)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return;

        // 添加自定義屬性
        telemetry.Context.GlobalProperties["Environment"] = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown";
        telemetry.Context.GlobalProperties["Version"] = "1.0.0";

        // 添加用戶上下文
        var userId = httpContext.User?.FindFirst("sub")?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            telemetry.Context.User.Id = userId;
        }

        // 添加會話 ID
        var sessionId = httpContext.Request.Headers["X-Session-ID"].FirstOrDefault();
        if (!string.IsNullOrEmpty(sessionId))
        {
            telemetry.Context.Session.Id = sessionId;
        }

        // 添加 Correlation ID
        var correlationId = httpContext.TraceIdentifier;
        telemetry.Context.Operation.Id = correlationId;
    }
}
```

### 自定義事件和指標追蹤

```csharp
// src/Application/Services/WorkflowExecutionService.cs
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

public sealed class WorkflowExecutionService : IWorkflowExecutionService
{
    private readonly TelemetryClient _telemetryClient;
    private readonly ILogger<WorkflowExecutionService> _logger;

    public WorkflowExecutionService(
        TelemetryClient telemetryClient,
        ILogger<WorkflowExecutionService> logger)
    {
        _telemetryClient = telemetryClient;
        _logger = logger;
    }

    public async Task<WorkflowResult> ExecuteWorkflowAsync(
        Guid workflowId,
        string workflowType,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        // 追蹤自定義事件
        _telemetryClient.TrackEvent("WorkflowExecutionStarted", new Dictionary<string, string>
        {
            ["WorkflowId"] = workflowId.ToString(),
            ["WorkflowType"] = workflowType
        });

        try
        {
            var result = await ExecuteInternalAsync(workflowId, cancellationToken);

            stopwatch.Stop();

            // 追蹤自定義指標
            _telemetryClient.TrackMetric("WorkflowExecutionDuration", stopwatch.ElapsedMilliseconds, new Dictionary<string, string>
            {
                ["WorkflowType"] = workflowType,
                ["Status"] = "Success"
            });

            // 追蹤依賴項調用 (手動)
            var dependency = new DependencyTelemetry
            {
                Name = "Azure OpenAI API Call",
                Type = "HTTP",
                Data = "POST https://openai.azure.com/chat/completions",
                Duration = TimeSpan.FromMilliseconds(500),
                Success = true,
                ResultCode = "200"
            };
            _telemetryClient.TrackDependency(dependency);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            // 追蹤異常
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                ["WorkflowId"] = workflowId.ToString(),
                ["WorkflowType"] = workflowType,
                ["ExecutionDuration"] = stopwatch.ElapsedMilliseconds.ToString()
            });

            throw;
        }
    }
}
```

---

## OpenTelemetry 集成

### 配置 OpenTelemetry

**安裝 NuGet 套件**:

```bash
dotnet add package OpenTelemetry.Extensions.Hosting
dotnet add package OpenTelemetry.Instrumentation.AspNetCore
dotnet add package OpenTelemetry.Instrumentation.Http
dotnet add package OpenTelemetry.Instrumentation.EntityFrameworkCore
dotnet add package OpenTelemetry.Exporter.Prometheus.AspNetCore
dotnet add package OpenTelemetry.Exporter.Jaeger
```

**Program.cs 配置**:

```csharp
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// 配置 OpenTelemetry
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService(
            serviceName: "AI-Agent-Platform-API",
            serviceVersion: "1.0.0",
            serviceInstanceId: Environment.MachineName))
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation(options =>
        {
            options.RecordException = true;
            options.EnrichWithHttpRequest = (activity, httpRequest) =>
            {
                activity.SetTag("http.request.client_ip", httpRequest.HttpContext.Connection.RemoteIpAddress);
                activity.SetTag("http.request.user_agent", httpRequest.Headers["User-Agent"].ToString());
            };
            options.EnrichWithHttpResponse = (activity, httpResponse) =>
            {
                activity.SetTag("http.response.content_length", httpResponse.ContentLength);
            };
        })
        .AddHttpClientInstrumentation()
        .AddEntityFrameworkCoreInstrumentation(options =>
        {
            options.SetDbStatementForText = true;
            options.SetDbStatementForStoredProcedure = true;
        })
        .AddSource("AIAgentPlatform.*")
        .AddJaegerExporter(options =>
        {
            options.AgentHost = builder.Configuration["Jaeger:AgentHost"] ?? "localhost";
            options.AgentPort = int.Parse(builder.Configuration["Jaeger:AgentPort"] ?? "6831");
        }))
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation()
        .AddProcessInstrumentation()
        .AddMeter("AIAgentPlatform.*")
        .AddPrometheusExporter());

var app = builder.Build();

// 暴露 Prometheus Metrics Endpoint
app.MapPrometheusScrapingEndpoint();

app.MapControllers();
app.Run();
```

### 自定義 Span 和 Metrics

```csharp
// src/Application/Services/OpenAIService.cs
using System.Diagnostics;
using System.Diagnostics.Metrics;

public sealed class OpenAIService : IOpenAIService
{
    private static readonly ActivitySource ActivitySource = new("AIAgentPlatform.OpenAI");
    private static readonly Meter Meter = new("AIAgentPlatform.OpenAI");

    private readonly Counter<long> _requestCounter;
    private readonly Histogram<double> _requestDuration;

    public OpenAIService()
    {
        _requestCounter = Meter.CreateCounter<long>(
            "openai_requests_total",
            description: "Total number of OpenAI API requests");

        _requestDuration = Meter.CreateHistogram<double>(
            "openai_request_duration_seconds",
            description: "OpenAI API request duration in seconds");
    }

    public async Task<string> GenerateCompletionAsync(string prompt, CancellationToken cancellationToken)
    {
        // 創建自定義 Span
        using var activity = ActivitySource.StartActivity("GenerateCompletion", ActivityKind.Client);
        activity?.SetTag("openai.model", "gpt-4");
        activity?.SetTag("openai.prompt_length", prompt.Length);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var result = await CallOpenAIAsync(prompt, cancellationToken);

            stopwatch.Stop();

            // 記錄成功請求
            _requestCounter.Add(1, new KeyValuePair<string, object?>("status", "success"));
            _requestDuration.Record(stopwatch.Elapsed.TotalSeconds, new KeyValuePair<string, object?>("status", "success"));

            activity?.SetTag("openai.response_length", result.Length);
            activity?.SetStatus(ActivityStatusCode.Ok);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            // 記錄失敗請求
            _requestCounter.Add(1, new KeyValuePair<string, object?>("status", "failure"));
            _requestDuration.Record(stopwatch.Elapsed.TotalSeconds, new KeyValuePair<string, object?>("status", "failure"));

            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.RecordException(ex);

            throw;
        }
    }
}
```

---

## 數據庫性能監控

### Entity Framework Core 性能優化

**啟用查詢日誌**:

```csharp
// appsettings.Development.json
{
  "Logging": {
    "LogLevel": {
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

**配置連接池監控**:

```csharp
// src/Infrastructure/Persistence/ApplicationDbContext.cs
public sealed class ApplicationDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseNpgsql(connectionString, options =>
            {
                options.EnableRetryOnFailure(maxRetryCount: 3);
                options.CommandTimeout(30);  // 30 seconds timeout
            })
            .EnableSensitiveDataLogging(false)  // 生產環境禁用
            .EnableDetailedErrors(false)  // 生產環境禁用
            .LogTo(Console.WriteLine, LogLevel.Warning);  // 僅記錄警告級別以上
    }
}
```

### 慢查詢監控

**PostgreSQL pg_stat_statements 擴展**:

```sql
-- 啟用 pg_stat_statements (追蹤所有 SQL 查詢)
CREATE EXTENSION IF NOT EXISTS pg_stat_statements;

-- 查詢最慢的 10 條 SQL
SELECT
    query,
    calls,
    total_exec_time,
    mean_exec_time,
    max_exec_time,
    stddev_exec_time
FROM pg_stat_statements
ORDER BY mean_exec_time DESC
LIMIT 10;

-- 查詢最耗時的 SQL (總執行時間)
SELECT
    query,
    calls,
    total_exec_time,
    (total_exec_time / calls) AS avg_time
FROM pg_stat_statements
ORDER BY total_exec_time DESC
LIMIT 10;

-- 重置統計
SELECT pg_stat_statements_reset();
```

**Prometheus Exporter 暴露慢查詢指標**:

```yaml
# postgres-exporter 配置
queries:
  - name: pg_slow_queries
    help: "Slow queries (> 1 second mean execution time)"
    labels:
      - "query_hash"
    values:
      - "calls"
      - "mean_exec_time"
    query: |
      SELECT
        md5(query) AS query_hash,
        calls,
        mean_exec_time
      FROM pg_stat_statements
      WHERE mean_exec_time > 1000  -- 1 second
      ORDER BY mean_exec_time DESC
      LIMIT 20;
```

---

## 資源利用率監控

### Kubernetes 資源監控

**Prometheus 查詢示例**:

```promql
# CPU 使用率 (按 Pod)
rate(container_cpu_usage_seconds_total{namespace="aiagent-prod"}[5m])

# 內存使用率 (按 Pod)
container_memory_usage_bytes{namespace="aiagent-prod"}

# 磁盤 I/O (讀取速率)
rate(container_fs_reads_bytes_total{namespace="aiagent-prod"}[5m])

# 網絡流量 (接收速率)
rate(container_network_receive_bytes_total{namespace="aiagent-prod"}[5m])

# Pod 重啟次數
kube_pod_container_status_restarts_total{namespace="aiagent-prod"}

# Node CPU 使用率
100 - (avg by (instance) (irate(node_cpu_seconds_total{mode="idle"}[5m])) * 100)

# Node 內存使用率
(1 - (node_memory_MemAvailable_bytes / node_memory_MemTotal_bytes)) * 100
```

### 資源限制與請求

```yaml
# k8s/base/api-deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: api-deployment
spec:
  template:
    spec:
      containers:
      - name: api
        resources:
          requests:
            memory: "512Mi"
            cpu: "250m"
          limits:
            memory: "1Gi"
            cpu: "500m"
        livenessProbe:
          httpGet:
            path: /health/live
            port: 8080
          initialDelaySeconds: 30
          periodSeconds: 10
          timeoutSeconds: 5
          failureThreshold: 3
        readinessProbe:
          httpGet:
            path: /health/ready
            port: 8080
          initialDelaySeconds: 10
          periodSeconds: 5
          timeoutSeconds: 3
          failureThreshold: 3
```

---

## 外部依賴性能監控

### Azure OpenAI 性能追蹤

```csharp
// src/Infrastructure/Services/AzureOpenAIMonitoringService.cs
using Microsoft.ApplicationInsights;

public sealed class AzureOpenAIMonitoringService
{
    private readonly TelemetryClient _telemetryClient;
    private readonly IHttpClientFactory _httpClientFactory;

    public async Task<OpenAIResponse> CallOpenAIWithMonitoringAsync(
        string prompt,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var httpClient = _httpClientFactory.CreateClient("AzureOpenAI");

        try
        {
            var response = await httpClient.PostAsJsonAsync("/chat/completions", new
            {
                model = "gpt-4",
                messages = new[] { new { role = "user", content = prompt } }
            }, cancellationToken);

            stopwatch.Stop();

            // 追蹤依賴項
            var dependency = new DependencyTelemetry
            {
                Name = "Azure OpenAI - Chat Completion",
                Type = "HTTP",
                Data = $"POST /chat/completions (model: gpt-4, tokens: {prompt.Length / 4})",
                Duration = stopwatch.Elapsed,
                Success = response.IsSuccessStatusCode,
                ResultCode = ((int)response.StatusCode).ToString(),
                Target = "openai.azure.com"
            };

            dependency.Properties["prompt_length"] = prompt.Length.ToString();
            dependency.Properties["model"] = "gpt-4";

            if (response.Headers.Contains("x-ratelimit-remaining-requests"))
            {
                dependency.Properties["rate_limit_remaining"] = response.Headers.GetValues("x-ratelimit-remaining-requests").First();
            }

            _telemetryClient.TrackDependency(dependency);

            // 追蹤速率限制指標
            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                _telemetryClient.TrackMetric("AzureOpenAI_RateLimitExceeded", 1);
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<OpenAIResponse>(cancellationToken);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            var dependency = new DependencyTelemetry
            {
                Name = "Azure OpenAI - Chat Completion",
                Type = "HTTP",
                Duration = stopwatch.Elapsed,
                Success = false,
                ResultCode = "Exception"
            };

            _telemetryClient.TrackDependency(dependency);
            _telemetryClient.TrackException(ex);

            throw;
        }
    }
}
```

### Redis 性能監控

```csharp
// src/Infrastructure/Caching/RedisCacheMonitoringService.cs
public sealed class RedisCacheMonitoringService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly TelemetryClient _telemetryClient;

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var db = _redis.GetDatabase();

        try
        {
            var value = await db.StringGetAsync(key);
            stopwatch.Stop();

            // 追蹤 Redis 操作
            var dependency = new DependencyTelemetry
            {
                Name = $"Redis GET {key}",
                Type = "Redis",
                Duration = stopwatch.Elapsed,
                Success = true,
                ResultCode = value.HasValue ? "Hit" : "Miss"
            };

            _telemetryClient.TrackDependency(dependency);

            // 追蹤緩存命中率
            _telemetryClient.TrackMetric("Redis_CacheHitRate", value.HasValue ? 1 : 0, new Dictionary<string, string>
            {
                ["CacheKey"] = key
            });

            return value.HasValue ? JsonSerializer.Deserialize<T>(value!) : default;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _telemetryClient.TrackDependency(new DependencyTelemetry
            {
                Name = $"Redis GET {key}",
                Type = "Redis",
                Duration = stopwatch.Elapsed,
                Success = false,
                ResultCode = "Exception"
            });

            _telemetryClient.TrackException(ex);
            throw;
        }
    }

    public async Task<RedisHealthStatus> GetHealthStatusAsync()
    {
        var server = _redis.GetServer(_redis.GetEndPoints().First());
        var info = await server.InfoAsync();

        var memoryInfo = info.First(x => x.Key == "Memory");
        var statsInfo = info.First(x => x.Key == "Stats");

        return new RedisHealthStatus
        {
            UsedMemoryMB = long.Parse(memoryInfo.First(x => x.Key == "used_memory").Value) / 1024 / 1024,
            ConnectedClients = int.Parse(statsInfo.First(x => x.Key == "connected_clients").Value),
            TotalCommandsProcessed = long.Parse(statsInfo.First(x => x.Key == "total_commands_processed").Value),
            OpsPerSecond = int.Parse(statsInfo.First(x => x.Key == "instantaneous_ops_per_sec").Value)
        };
    }
}
```

---

## 用戶體驗監控

### Real User Monitoring (RUM)

**前端 Application Insights 集成** (React):

```typescript
// src/Web/ClientApp/src/telemetry.ts
import { ApplicationInsights } from '@microsoft/applicationinsights-web';
import { ReactPlugin } from '@microsoft/applicationinsights-react-js';

const reactPlugin = new ReactPlugin();

const appInsights = new ApplicationInsights({
    config: {
        connectionString: process.env.REACT_APP_APPINSIGHTS_CONNECTION_STRING,
        enableAutoRouteTracking: true,
        enableCorsCorrelation: true,
        enableRequestHeaderTracking: true,
        enableResponseHeaderTracking: true,
        extensions: [reactPlugin],
        extensionConfig: {
            [reactPlugin.identifier]: { history: browserHistory }
        }
    }
});

appInsights.loadAppInsights();

// 追蹤頁面瀏覽
appInsights.trackPageView({ name: 'Home Page' });

// 追蹤自定義事件
export const trackEvent = (name: string, properties?: { [key: string]: any }) => {
    appInsights.trackEvent({ name }, properties);
};

// 追蹤自定義指標
export const trackMetric = (name: string, average: number, properties?: { [key: string]: any }) => {
    appInsights.trackMetric({ name, average }, properties);
};

// 追蹤異常
export const trackException = (error: Error, severityLevel?: number) => {
    appInsights.trackException({ exception: error, severityLevel });
};

export { appInsights, reactPlugin };
```

**使用示例**:

```typescript
// src/Web/ClientApp/src/pages/WorkflowPage.tsx
import { trackEvent, trackMetric } from '../telemetry';

export const WorkflowPage: React.FC = () => {
    const executeWorkflow = async (workflowId: string) => {
        const startTime = performance.now();

        trackEvent('WorkflowExecutionStarted', { workflowId });

        try {
            const result = await apiClient.executeWorkflow(workflowId);

            const duration = performance.now() - startTime;
            trackMetric('WorkflowExecutionDuration', duration, {
                workflowId,
                status: 'success'
            });

            trackEvent('WorkflowExecutionCompleted', { workflowId, status: 'success' });
        } catch (error) {
            const duration = performance.now() - startTime;
            trackMetric('WorkflowExecutionDuration', duration, {
                workflowId,
                status: 'failure'
            });

            trackException(error as Error);
            trackEvent('WorkflowExecutionFailed', { workflowId, error: (error as Error).message });
        }
    };

    return <div>...</div>;
};
```

### Core Web Vitals 監控

```typescript
// src/Web/ClientApp/src/webVitals.ts
import { getCLS, getFID, getFCP, getLCP, getTTFB } from 'web-vitals';
import { trackMetric } from './telemetry';

export const reportWebVitals = () => {
    // Cumulative Layout Shift (視覺穩定性)
    getCLS((metric) => {
        trackMetric('WebVital_CLS', metric.value);
    });

    // First Input Delay (互動性)
    getFID((metric) => {
        trackMetric('WebVital_FID', metric.value);
    });

    // First Contentful Paint (加載性能)
    getFCP((metric) => {
        trackMetric('WebVital_FCP', metric.value);
    });

    // Largest Contentful Paint (加載性能)
    getLCP((metric) => {
        trackMetric('WebVital_LCP', metric.value);
    });

    // Time to First Byte (服務器響應時間)
    getTTFB((metric) => {
        trackMetric('WebVital_TTFB', metric.value);
    });
};

// index.tsx
reportWebVitals();
```

---

## 性能基準與負載測試

### k6 負載測試

**安裝 k6**:

```bash
# macOS
brew install k6

# Linux
sudo apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys C5AD17C747E3415A3642D57D77C6C491D6AC1D69
echo "deb https://dl.k6.io/deb stable main" | sudo tee /etc/apt/sources.list.d/k6.list
sudo apt-get update
sudo apt-get install k6
```

**負載測試腳本**: `tests/performance/load-test.js`

```javascript
import http from 'k6/http';
import { check, sleep } from 'k6';
import { Rate } from 'k6/metrics';

// 自定義指標
const errorRate = new Rate('errors');

export const options = {
    stages: [
        { duration: '2m', target: 100 },   // Ramp up to 100 users over 2 minutes
        { duration: '5m', target: 100 },   // Stay at 100 users for 5 minutes
        { duration: '2m', target: 200 },   // Ramp up to 200 users over 2 minutes
        { duration: '5m', target: 200 },   // Stay at 200 users for 5 minutes
        { duration: '2m', target: 0 },     // Ramp down to 0 users over 2 minutes
    ],
    thresholds: {
        'http_req_duration': ['p(95)<500', 'p(99)<1000'],  // 95% < 500ms, 99% < 1s
        'http_req_failed': ['rate<0.01'],  // Error rate < 1%
        'errors': ['rate<0.05'],  // Custom error rate < 5%
    },
};

const BASE_URL = 'https://api.aiagent.example.com';

export default function () {
    // 1. Login
    const loginRes = http.post(`${BASE_URL}/api/auth/login`, JSON.stringify({
        email: 'test@example.com',
        password: 'Test123!'
    }), {
        headers: { 'Content-Type': 'application/json' },
    });

    check(loginRes, {
        'login status is 200': (r) => r.status === 200,
        'login has token': (r) => r.json('accessToken') !== undefined,
    }) || errorRate.add(1);

    const token = loginRes.json('accessToken');
    sleep(1);

    // 2. List Workflows
    const listRes = http.get(`${BASE_URL}/api/workflows`, {
        headers: { 'Authorization': `Bearer ${token}` },
    });

    check(listRes, {
        'list status is 200': (r) => r.status === 200,
        'list has items': (r) => r.json().length > 0,
    }) || errorRate.add(1);

    sleep(1);

    // 3. Execute Workflow
    const workflowId = listRes.json()[0].id;
    const executeRes = http.post(`${BASE_URL}/api/workflows/${workflowId}/execute`, null, {
        headers: { 'Authorization': `Bearer ${token}` },
    });

    check(executeRes, {
        'execute status is 200': (r) => r.status === 200,
        'execute has result': (r) => r.json('status') !== undefined,
    }) || errorRate.add(1);

    sleep(2);
}
```

**運行負載測試**:

```bash
# 本地運行
k6 run tests/performance/load-test.js

# 輸出到 InfluxDB (可視化)
k6 run --out influxdb=http://localhost:8086/k6 tests/performance/load-test.js

# 輸出到 Prometheus
k6 run --out experimental-prometheus-rw tests/performance/load-test.js
```

### 性能預算 (Performance Budget)

```yaml
# performance-budget.yaml
budgets:
  # Frontend Performance Budget
  frontend:
    - metric: FCP
      budget: 1.8s  # First Contentful Paint < 1.8s
    - metric: LCP
      budget: 2.5s  # Largest Contentful Paint < 2.5s
    - metric: FID
      budget: 100ms  # First Input Delay < 100ms
    - metric: CLS
      budget: 0.1  # Cumulative Layout Shift < 0.1
    - metric: TTFB
      budget: 600ms  # Time to First Byte < 600ms

  # Backend Performance Budget
  backend:
    - metric: p95_latency
      budget: 500ms  # 95% of requests < 500ms
    - metric: p99_latency
      budget: 1000ms  # 99% of requests < 1s
    - metric: error_rate
      budget: 1%  # Error rate < 1%
    - metric: availability
      budget: 99.9%  # Uptime > 99.9%

  # Database Performance Budget
  database:
    - metric: avg_query_time
      budget: 50ms  # Average query time < 50ms
    - metric: slow_query_count
      budget: 10  # < 10 slow queries/hour (> 1s)
    - metric: connection_pool_usage
      budget: 80%  # Connection pool usage < 80%
```

---

## 性能優化建議

### 1. 應用層優化

```csharp
// ❌ N+1 Query Problem
public async Task<List<WorkflowDto>> GetWorkflowsAsync()
{
    var workflows = await _context.Workflows.ToListAsync();
    foreach (var workflow in workflows)
    {
        workflow.Steps = await _context.WorkflowSteps
            .Where(s => s.WorkflowId == workflow.Id)
            .ToListAsync();  // N queries!
    }
    return workflows;
}

// ✅ Eager Loading
public async Task<List<WorkflowDto>> GetWorkflowsAsync()
{
    var workflows = await _context.Workflows
        .Include(w => w.Steps)  // 1 query with JOIN
        .ToListAsync();
    return workflows;
}

// ✅ 異步批處理
public async Task ProcessWorkflowsAsync(List<Guid> workflowIds)
{
    // ❌ 順序執行
    foreach (var id in workflowIds)
    {
        await ProcessWorkflowAsync(id);  // 每個耗時 1 秒,總共 10 秒
    }

    // ✅ 並行執行
    var tasks = workflowIds.Select(id => ProcessWorkflowAsync(id));
    await Task.WhenAll(tasks);  // 並行執行,總共 1 秒
}
```

### 2. 緩存策略

```csharp
// 分佈式緩存 (Redis)
public async Task<Workflow> GetWorkflowAsync(Guid id)
{
    var cacheKey = $"workflow:{id}";

    // 1. 嘗試從緩存讀取
    var cached = await _cache.GetStringAsync(cacheKey);
    if (cached != null)
    {
        return JsonSerializer.Deserialize<Workflow>(cached);
    }

    // 2. 緩存未命中,從數據庫讀取
    var workflow = await _context.Workflows.FindAsync(id);

    // 3. 寫入緩存 (TTL: 5 分鐘)
    await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(workflow), new DistributedCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
    });

    return workflow;
}
```

### 3. 數據庫索引優化

```sql
-- 創建索引加速查詢
CREATE INDEX idx_workflows_user_id ON workflows(user_id);
CREATE INDEX idx_workflow_steps_workflow_id ON workflow_steps(workflow_id);
CREATE INDEX idx_workflow_executions_created_at ON workflow_executions(created_at DESC);

-- 複合索引
CREATE INDEX idx_workflows_user_status ON workflows(user_id, status);

-- 部分索引 (僅索引活躍工作流)
CREATE INDEX idx_active_workflows ON workflows(user_id) WHERE status = 'active';

-- 分析查詢計劃
EXPLAIN ANALYZE SELECT * FROM workflows WHERE user_id = '123' AND status = 'active';
```

---

## 最佳實踐

### 1. 性能監控檢查清單

- [ ] **應用性能監控 (APM)** 已集成 Application Insights 或 OpenTelemetry
- [ ] **分佈式追蹤** 已啟用,可追蹤跨服務請求
- [ ] **資源利用率** CPU、內存、磁盤、網絡指標已收集
- [ ] **數據庫性能** 慢查詢監控已啟用,查詢計劃定期審查
- [ ] **外部依賴** Azure OpenAI、Redis、Blob Storage 性能已追蹤
- [ ] **用戶體驗** RUM 已啟用,Core Web Vitals 已監控
- [ ] **性能預算** 已定義並監控關鍵指標
- [ ] **負載測試** 定期執行,驗證 SLO 達成

### 2. 性能優化優先級

1. **修復 P0 性能問題** (影響 SLO)
2. **優化高頻代碼路徑** (80/20 法則)
3. **數據庫查詢優化** (N+1 問題、缺失索引)
4. **緩存策略優化** (提高命中率)
5. **資源擴展** (水平/垂直擴展)

---

## 總結

本文檔提供了完整的性能監控實施方案,涵蓋:

✅ **APM 集成** (Application Insights, OpenTelemetry)
✅ **分佈式追蹤** (Jaeger, Custom Spans)
✅ **資源監控** (CPU, Memory, Disk, Network)
✅ **數據庫性能** (Slow Queries, Connection Pooling, Indexing)
✅ **外部依賴** (Azure OpenAI, Redis, Blob Storage)
✅ **用戶體驗** (RUM, Core Web Vitals)
✅ **負載測試** (k6, Performance Budgets)

透過本指南,開發與運維團隊可以實現:
- **全面可見性**: 應用、基礎設施、用戶體驗全方位監控
- **快速定位**: 分佈式追蹤快速識別性能瓶頸
- **持續優化**: 性能預算驅動持續改進
- **容量規劃**: 基於趨勢數據預測資源需求
