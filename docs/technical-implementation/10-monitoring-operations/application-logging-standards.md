# 應用程序日誌標準實施指南

## 概述

### 文檔目的
本文檔提供 AI Agent 工作流平台的完整應用程序日誌標準實施方案,涵蓋:
- **結構化日誌設計** (Structured Logging with Serilog)
- **日誌級別規範** (Trace, Debug, Information, Warning, Error, Critical)
- **日誌上下文豐富** (Correlation ID, User Context, Request Context)
- **集中化日誌收集** (Fluentd/Fluent Bit, Azure Log Analytics)
- **日誌存儲與查詢** (Elasticsearch, Azure Monitor Logs)
- **敏感信息過濾** (PII Redaction, Security Compliance)

### 技術背景
結構化日誌是現代雲原生應用的核心可觀測性實踐:
- **傳統日誌**: 純文本,難以查詢和分析
  ```
  [2024-01-15 10:30:45] User alice@example.com logged in from 192.168.1.100
  ```
- **結構化日誌**: JSON 格式,易於查詢、過濾、聚合
  ```json
  {
    "timestamp": "2024-01-15T10:30:45.123Z",
    "level": "Information",
    "message": "User logged in",
    "user_email": "alice@example.com",
    "client_ip": "192.168.1.100",
    "correlation_id": "abc-123"
  }
  ```

**結構化日誌優勢**:
- **可查詢**: 支持複雜過濾 (如 `user_email = "alice@example.com" AND level = "Error"`)
- **可聚合**: 統計分析 (如 "過去 1 小時內每個用戶的登錄次數")
- **上下文豐富**: 自動附加 Correlation ID, Request ID, User ID
- **機器可讀**: 易於集成到監控系統 (如 Grafana, Splunk, Datadog)

**Serilog 核心概念**:
- **Logger**: 日誌記錄器,負責輸出日誌
- **Sink**: 日誌目標 (Console, File, Elasticsearch, Azure Monitor)
- **Enricher**: 日誌豐富器,自動添加上下文信息
- **Formatter**: 日誌格式化器 (JSON, Compact JSON, Plain Text)

---

## 日誌架構

### 整體日誌流

```
┌─────────────────────────────────────────────────────────────────┐
│ Application Layer (ASP.NET Core)                                │
├─────────────────────────────────────────────────────────────────┤
│ ┌─────────────────────────────────────────────────────────────┐ │
│ │ Serilog Logger                                              │ │
│ │ ├─ Log Levels: Trace → Debug → Info → Warning → Error      │ │
│ │ ├─ Enrichers: Correlation ID, User Context, Environment    │ │
│ │ ├─ Filters: Sensitive Data Redaction                       │ │
│ │ └─ Formatters: Compact JSON                                │ │
│ └─────────────────────────────────────────────────────────────┘ │
│                          │                                      │
│                          │ (Multiple Sinks)                     │
│          ┌───────────────┼───────────────┐                      │
│          │               │               │                      │
│     ┌────▼────┐    ┌────▼────┐    ┌─────▼─────┐                │
│     │ Console │    │  File   │    │ Azure     │                │
│     │  Sink   │    │  Sink   │    │ Monitor   │                │
│     └─────────┘    └────┬────┘    │  Sink     │                │
│     (Dev Only)          │         └─────┬─────┘                │
│                         │               │                      │
└─────────────────────────┼───────────────┼──────────────────────┘
                          │               │
                          │               │
            ┌─────────────▼───────────────▼─────────────┐
            │ Fluent Bit (Log Forwarder)                │
            │ - Collect logs from stdout + files        │
            │ - Parse JSON logs                         │
            │ - Add Kubernetes metadata                 │
            │ - Forward to multiple destinations        │
            └─────────────┬───────────────┬─────────────┘
                          │               │
         ┌────────────────▼──┐   ┌────────▼──────────────┐
         │ Azure Log         │   │ Elasticsearch         │
         │ Analytics         │   │ (Long-term Storage)   │
         │ (Real-time Query) │   │ (Full-text Search)    │
         └───────────────────┘   └───────────────────────┘
                          │               │
                          │               │
                          └───────┬───────┘
                                  │
                          ┌───────▼────────┐
                          │ Grafana / Kibana│
                          │ (Log Visualization)│
                          └─────────────────┘
```

---

## Serilog 配置

### 安裝 NuGet 套件

```bash
# 核心套件
dotnet add package Serilog.AspNetCore

# Sinks (日誌目標)
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Sinks.File
dotnet add package Serilog.Sinks.Async
dotnet add package Serilog.Sinks.AzureAnalytics

# Enrichers (上下文豐富)
dotnet add package Serilog.Enrichers.Environment
dotnet add package Serilog.Enrichers.Thread
dotnet add package Serilog.Enrichers.Process
dotnet add package Serilog.Enrichers.CorrelationId

# Formatters
dotnet add package Serilog.Formatting.Compact
```

### Program.cs 配置

```csharp
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

var builder = WebApplication.CreateBuilder(args);

// 配置 Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .Enrich.WithThreadId()
    .Enrich.WithProcessId()
    .Enrich.WithCorrelationId()
    .Enrich.WithProperty("Application", "AI Agent Platform")
    .Enrich.WithProperty("Version", "1.0.0")
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.Async(a => a.File(
        formatter: new CompactJsonFormatter(),
        path: "/var/log/aiagent/api-.json",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        fileSizeLimitBytes: 100 * 1024 * 1024,  // 100 MB
        shared: true))
    .WriteTo.AzureAnalytics(
        workspaceId: builder.Configuration["AzureLogAnalytics:WorkspaceId"],
        authenticationId: builder.Configuration["AzureLogAnalytics:SharedKey"],
        logName: "AIAgentPlatform")
    .CreateLogger();

// 使用 Serilog 作為 ASP.NET Core 日誌提供者
builder.Host.UseSerilog();

var app = builder.Build();

// Serilog Request Logging Middleware
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("ClientIP", httpContext.Connection.RemoteIpAddress);
        diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
        diagnosticContext.Set("UserId", httpContext.User?.FindFirst("sub")?.Value);
        diagnosticContext.Set("CorrelationId", httpContext.TraceIdentifier);
    };
});

app.MapControllers();

try
{
    Log.Information("Starting AI Agent Platform API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
```

### appsettings.json 配置

```json
{
  "Serilog": {
    "Using": ["Serilog.Sinks.Console", "Serilog.Sinks.File", "Serilog.Sinks.AzureAnalytics"],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.Hosting.Lifetime": "Information",
        "Microsoft.EntityFrameworkCore.Database.Command": "Warning",
        "System": "Warning"
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
        "Name": "Async",
        "Args": {
          "configure": [
            {
              "Name": "File",
              "Args": {
                "formatter": "Serilog.Formatting.Compact.CompactJsonFormatter, Serilog.Formatting.Compact",
                "path": "/var/log/aiagent/api-.json",
                "rollingInterval": "Day",
                "retainedFileCountLimit": 7,
                "fileSizeLimitBytes": 104857600,
                "shared": true
              }
            }
          ]
        }
      }
    ],
    "Enrich": [
      "FromLogContext",
      "WithMachineName",
      "WithEnvironmentName",
      "WithThreadId",
      "WithProcessId",
      "WithCorrelationId"
    ],
    "Properties": {
      "Application": "AI Agent Platform",
      "Environment": "Production"
    }
  },

  "AzureLogAnalytics": {
    "WorkspaceId": "your-workspace-id",
    "SharedKey": "your-shared-key"
  }
}
```

---

## 日誌級別規範

### 日誌級別定義

| 級別 | 用途 | 使用場景 | 生產環境啟用 |
|------|------|----------|-------------|
| **Trace** | 最詳細的診斷信息 | 追蹤代碼執行路徑、變量值 | ❌ 否 (僅開發) |
| **Debug** | 開發調試信息 | 函數調用、分支邏輯、中間結果 | ❌ 否 (僅開發/測試) |
| **Information** | 常規信息 | 業務流程關鍵節點 (如用戶登錄、訂單創建) | ✅ 是 |
| **Warning** | 潛在問題警告 | 可恢復錯誤、性能降級、廢棄 API 使用 | ✅ 是 |
| **Error** | 錯誤但不影響整體服務 | 請求處理失敗、外部 API 調用失敗 | ✅ 是 |
| **Critical** | 嚴重錯誤導致服務不可用 | 數據庫連接失敗、關鍵依賴項崩潰 | ✅ 是 |

### 日誌級別使用示例

```csharp
public sealed class WorkflowExecutionService : IWorkflowExecutionService
{
    private readonly ILogger<WorkflowExecutionService> _logger;

    public async Task<WorkflowResult> ExecuteWorkflowAsync(
        Guid workflowId,
        CancellationToken cancellationToken)
    {
        // Information: 業務流程開始
        _logger.LogInformation(
            "Starting workflow execution. WorkflowId: {WorkflowId}",
            workflowId);

        try
        {
            // Debug: 詳細執行步驟 (僅開發環境)
            _logger.LogDebug(
                "Loading workflow definition from database. WorkflowId: {WorkflowId}",
                workflowId);

            var workflow = await LoadWorkflowAsync(workflowId, cancellationToken);

            // Trace: 變量值追蹤 (僅開發環境)
            _logger.LogTrace(
                "Workflow loaded. Name: {WorkflowName}, Steps: {StepCount}",
                workflow.Name,
                workflow.Steps.Count);

            var result = await ExecuteStepsAsync(workflow, cancellationToken);

            // Information: 業務流程完成
            _logger.LogInformation(
                "Workflow execution completed. WorkflowId: {WorkflowId}, Duration: {Duration}ms, Status: {Status}",
                workflowId,
                result.Duration,
                result.Status);

            return result;
        }
        catch (WorkflowNotFoundException ex)
        {
            // Warning: 可預期的錯誤,不影響服務
            _logger.LogWarning(
                ex,
                "Workflow not found. WorkflowId: {WorkflowId}",
                workflowId);
            throw;
        }
        catch (ExternalServiceException ex)
        {
            // Error: 外部依賴失敗,請求失敗但服務仍可用
            _logger.LogError(
                ex,
                "External service call failed during workflow execution. WorkflowId: {WorkflowId}, Service: {ServiceName}",
                workflowId,
                ex.ServiceName);
            throw;
        }
        catch (Exception ex)
        {
            // Critical: 未預期的嚴重錯誤
            _logger.LogCritical(
                ex,
                "Unexpected critical error during workflow execution. WorkflowId: {WorkflowId}",
                workflowId);
            throw;
        }
    }
}
```

---

## 結構化日誌最佳實踐

### 1. 使用消息模板 (Message Templates)

```csharp
// ❌ 錯誤: 字符串插值,無法結構化
_logger.LogInformation($"User {userId} logged in at {DateTime.Now}");

// ✅ 正確: 消息模板,自動提取屬性
_logger.LogInformation(
    "User logged in. UserId: {UserId}, Timestamp: {Timestamp}",
    userId,
    DateTime.UtcNow);

// 輸出 JSON:
// {
//   "message": "User logged in. UserId: 12345, Timestamp: 2024-01-15T10:30:45.123Z",
//   "userId": 12345,
//   "timestamp": "2024-01-15T10:30:45.123Z"
// }
```

### 2. 使用 LogContext 傳遞上下文

```csharp
// 在整個請求生命週期中添加 CorrelationId
public sealed class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault()
            ?? Guid.NewGuid().ToString();

        context.Response.Headers.Add("X-Correlation-ID", correlationId);

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }
}

// 在任何地方記錄日誌時,自動包含 CorrelationId
_logger.LogInformation("Processing order");
// {
//   "message": "Processing order",
//   "correlationId": "abc-123-def-456"
// }
```

### 3. 記錄強類型對象

```csharp
public sealed class UserLoginEvent
{
    public Guid UserId { get; init; }
    public string Email { get; init; }
    public string ClientIP { get; init; }
    public DateTime Timestamp { get; init; }
}

// 記錄整個對象 (使用 @ 前綴進行結構化序列化)
var loginEvent = new UserLoginEvent
{
    UserId = Guid.NewGuid(),
    Email = "alice@example.com",
    ClientIP = "192.168.1.100",
    Timestamp = DateTime.UtcNow
};

_logger.LogInformation(
    "User login event. {@UserLoginEvent}",
    loginEvent);

// 輸出 JSON:
// {
//   "message": "User login event.",
//   "userLoginEvent": {
//     "userId": "...",
//     "email": "alice@example.com",
//     "clientIP": "192.168.1.100",
//     "timestamp": "2024-01-15T10:30:45.123Z"
//   }
// }
```

### 4. 避免日誌爆炸

```csharp
// ❌ 錯誤: 在循環中記錄每個元素
foreach (var item in items)
{
    _logger.LogInformation("Processing item {ItemId}", item.Id);
}

// ✅ 正確: 記錄批次摘要
_logger.LogInformation(
    "Processing batch. TotalItems: {TotalItems}, ItemIds: {ItemIds}",
    items.Count,
    items.Select(i => i.Id).Take(10));  // 僅記錄前 10 個
```

---

## 敏感信息過濾

### 自定義 Enricher 實現 PII 過濾

```csharp
// src/Infrastructure/Logging/SensitiveDataEnricher.cs
using Serilog.Core;
using Serilog.Events;
using System.Text.RegularExpressions;

public sealed class SensitiveDataEnricher : ILogEventEnricher
{
    private static readonly Regex EmailRegex = new(@"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b");
    private static readonly Regex PhoneRegex = new(@"\b\d{3}[-.]?\d{3}[-.]?\d{4}\b");
    private static readonly Regex CreditCardRegex = new(@"\b\d{4}[\s-]?\d{4}[\s-]?\d{4}[\s-]?\d{4}\b");
    private static readonly Regex SsnRegex = new(@"\b\d{3}-\d{2}-\d{4}\b");

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (logEvent.MessageTemplate.Text.Contains("password", StringComparison.OrdinalIgnoreCase) ||
            logEvent.MessageTemplate.Text.Contains("secret", StringComparison.OrdinalIgnoreCase) ||
            logEvent.MessageTemplate.Text.Contains("token", StringComparison.OrdinalIgnoreCase))
        {
            // 標記包含敏感信息的日誌
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("SensitiveData", true));
        }

        // 過濾消息中的敏感信息
        foreach (var property in logEvent.Properties)
        {
            if (property.Value is ScalarValue scalarValue && scalarValue.Value is string stringValue)
            {
                var redacted = RedactSensitiveData(stringValue);
                if (redacted != stringValue)
                {
                    logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(property.Key, redacted));
                }
            }
        }
    }

    private static string RedactSensitiveData(string input)
    {
        input = EmailRegex.Replace(input, "***@***.***");
        input = PhoneRegex.Replace(input, "***-***-****");
        input = CreditCardRegex.Replace(input, "**** **** **** ****");
        input = SsnRegex.Replace(input, "***-**-****");
        return input;
    }
}

// 註冊 Enricher
Log.Logger = new LoggerConfiguration()
    .Enrich.With<SensitiveDataEnricher>()
    .CreateLogger();
```

### 屬性級過濾

```csharp
// src/Infrastructure/Logging/DestructuringPolicy.cs
using Serilog.Core;
using Serilog.Events;

public sealed class SensitiveDataDestructuringPolicy : IDestructuringPolicy
{
    private static readonly HashSet<string> SensitivePropertyNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Password", "PasswordHash", "Secret", "ApiKey", "Token", "AccessToken", "RefreshToken",
        "ClientSecret", "ConnectionString", "CreditCardNumber", "SSN", "TaxId"
    };

    public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory, out LogEventPropertyValue result)
    {
        var type = value.GetType();

        if (type.IsClass && type != typeof(string))
        {
            var properties = type.GetProperties();
            var logProperties = new List<LogEventProperty>();

            foreach (var prop in properties)
            {
                var propValue = prop.GetValue(value);

                if (SensitivePropertyNames.Contains(prop.Name))
                {
                    // 替換敏感屬性為 ***REDACTED***
                    logProperties.Add(new LogEventProperty(prop.Name, new ScalarValue("***REDACTED***")));
                }
                else
                {
                    logProperties.Add(new LogEventProperty(prop.Name, propertyValueFactory.CreatePropertyValue(propValue, true)));
                }
            }

            result = new StructureValue(logProperties);
            return true;
        }

        result = null;
        return false;
    }
}

// 註冊 Policy
Log.Logger = new LoggerConfiguration()
    .Destructure.With<SensitiveDataDestructuringPolicy>()
    .CreateLogger();
```

---

## 集中化日誌收集 (Fluent Bit)

### Fluent Bit DaemonSet 配置

```yaml
# k8s/base/fluent-bit-daemonset.yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: fluent-bit-config
  namespace: logging
data:
  fluent-bit.conf: |
    [SERVICE]
        Flush         5
        Daemon        off
        Log_Level     info
        Parsers_File  parsers.conf

    [INPUT]
        Name              tail
        Path              /var/log/containers/*.log
        Parser            docker
        Tag               kube.*
        Refresh_Interval  5
        Mem_Buf_Limit     5MB
        Skip_Long_Lines   On

    [FILTER]
        Name                kubernetes
        Match               kube.*
        Kube_URL            https://kubernetes.default.svc:443
        Kube_CA_File        /var/run/secrets/kubernetes.io/serviceaccount/ca.crt
        Kube_Token_File     /var/run/secrets/kubernetes.io/serviceaccount/token
        Kube_Tag_Prefix     kube.var.log.containers.
        Merge_Log           On
        Keep_Log            Off
        K8S-Logging.Parser  On
        K8S-Logging.Exclude On

    [FILTER]
        Name    modify
        Match   kube.*
        Add     cluster_name aks-aiagent-prod
        Add     environment production

    [OUTPUT]
        Name        azure
        Match       kube.*
        Customer_ID ${AZURE_LOG_ANALYTICS_WORKSPACE_ID}
        Shared_Key  ${AZURE_LOG_ANALYTICS_SHARED_KEY}
        Log_Type    KubernetesLogs

    [OUTPUT]
        Name            es
        Match           kube.*
        Host            elasticsearch.logging.svc.cluster.local
        Port            9200
        Index           aiagent-logs
        Type            _doc
        Logstash_Format On
        Retry_Limit     5

  parsers.conf: |
    [PARSER]
        Name   docker
        Format json
        Time_Key time
        Time_Format %Y-%m-%dT%H:%M:%S.%LZ
        Time_Keep On

    [PARSER]
        Name        syslog
        Format      regex
        Regex       ^\<(?<pri>[0-9]+)\>(?<time>[^ ]* {1,2}[^ ]* [^ ]*) (?<host>[^ ]*) (?<ident>[a-zA-Z0-9_\/\.\-]*)(?:\[(?<pid>[0-9]+)\])?(?:[^\:]*\:)? *(?<message>.*)$
        Time_Key    time
        Time_Format %b %d %H:%M:%S
---
apiVersion: apps/v1
kind: DaemonSet
metadata:
  name: fluent-bit
  namespace: logging
spec:
  selector:
    matchLabels:
      app: fluent-bit
  template:
    metadata:
      labels:
        app: fluent-bit
    spec:
      serviceAccountName: fluent-bit
      tolerations:
      - key: node-role.kubernetes.io/master
        effect: NoSchedule
      containers:
      - name: fluent-bit
        image: fluent/fluent-bit:2.2
        env:
        - name: AZURE_LOG_ANALYTICS_WORKSPACE_ID
          valueFrom:
            secretKeyRef:
              name: fluent-bit-secrets
              key: workspace-id
        - name: AZURE_LOG_ANALYTICS_SHARED_KEY
          valueFrom:
            secretKeyRef:
              name: fluent-bit-secrets
              key: shared-key
        volumeMounts:
        - name: varlog
          mountPath: /var/log
        - name: varlibdockercontainers
          mountPath: /var/lib/docker/containers
          readOnly: true
        - name: fluent-bit-config
          mountPath: /fluent-bit/etc/
      volumes:
      - name: varlog
        hostPath:
          path: /var/log
      - name: varlibdockercontainers
        hostPath:
          path: /var/lib/docker/containers
      - name: fluent-bit-config
        configMap:
          name: fluent-bit-config
```

---

## Azure Log Analytics 查詢

### Kusto 查詢語言 (KQL) 示例

```kql
// 查詢過去 1 小時內的所有錯誤日誌
KubernetesLogs
| where TimeGenerated > ago(1h)
| where LogLevel == "Error"
| project TimeGenerated, PodName, Message, Exception
| order by TimeGenerated desc

// 統計每個 Pod 的日誌數量
KubernetesLogs
| where TimeGenerated > ago(24h)
| summarize Count = count() by PodName, LogLevel
| order by Count desc

// 查詢特定 CorrelationId 的所有日誌 (分佈式追蹤)
KubernetesLogs
| where TimeGenerated > ago(1h)
| where Message contains "CorrelationId: abc-123"
| project TimeGenerated, PodName, LogLevel, Message
| order by TimeGenerated asc

// 查詢 P95 響應時間
KubernetesLogs
| where TimeGenerated > ago(1h)
| where Message contains "HTTP"
| extend Duration = extract(@"in (\d+\.\d+) ms", 1, Message, typeof(double))
| summarize P95 = percentile(Duration, 95) by bin(TimeGenerated, 5m)
| render timechart

// 查詢錯誤率趨勢
KubernetesLogs
| where TimeGenerated > ago(24h)
| summarize ErrorCount = countif(LogLevel == "Error"), TotalCount = count() by bin(TimeGenerated, 1h)
| extend ErrorRate = ErrorCount * 100.0 / TotalCount
| project TimeGenerated, ErrorRate
| render timechart
```

---

## 日誌保留策略

### 保留期限規劃

| 環境 | 熱存儲 (快速查詢) | 冷存儲 (歸檔) | 總保留期 |
|------|-------------------|---------------|----------|
| **Development** | 3 天 | 無 | 3 天 |
| **Staging** | 7 天 | 30 天 | 37 天 |
| **Production** | 30 天 | 1 年 | 13 個月 |

### Azure Log Analytics 保留配置

```bash
# 設置工作區數據保留期為 90 天
az monitor log-analytics workspace update \
  --resource-group rg-aiagent-prod \
  --workspace-name law-aiagent-prod \
  --retention-time 90

# 配置數據導出到 Storage Account (長期歸檔)
az monitor log-analytics workspace data-export create \
  --resource-group rg-aiagent-prod \
  --workspace-name law-aiagent-prod \
  --name export-to-storage \
  --tables KubernetesLogs \
  --destination /subscriptions/<sub-id>/resourceGroups/rg-aiagent-prod/providers/Microsoft.Storage/storageAccounts/staiagentlogs
```

---

## 最佳實踐

### 1. 日誌採樣 (高流量場景)

```csharp
// 僅記錄 10% 的成功請求,100% 記錄錯誤請求
public sealed class SamplingEnricher : ILogEventEnricher
{
    private readonly Random _random = new();
    private const double SamplingRate = 0.1;  // 10%

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (logEvent.Level < LogEventLevel.Warning)
        {
            if (_random.NextDouble() > SamplingRate)
            {
                // 丟棄這條日誌
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Sampled", false));
            }
        }
    }
}
```

### 2. 異步日誌輸出

```csharp
// 使用 Async Sink 避免阻塞主線程
.WriteTo.Async(a => a.File(
    path: "/var/log/aiagent/api-.json",
    rollingInterval: RollingInterval.Day,
    bufferSize: 10000  // 緩衝區大小
))
```

### 3. 日誌壓縮

```yaml
# Fluent Bit 配置 Gzip 壓縮
[OUTPUT]
    Name        es
    Match       *
    Compress    gzip
```

---

## 故障排查

### 常見問題

| 問題 | 原因 | 解決方案 |
|------|------|----------|
| **日誌未出現在 Azure Log Analytics** | Workspace ID 或 Shared Key 錯誤 | 檢查 Fluent Bit Secret 配置 |
| **日誌延遲高** | Fluent Bit 緩衝區滿 | 增加 `Mem_Buf_Limit` 或減少 `Flush` 間隔 |
| **敏感信息洩漏** | 過濾器未生效 | 驗證 `SensitiveDataEnricher` 已註冊 |
| **日誌文件過大** | 未配置滾動策略 | 設置 `rollingInterval` 和 `fileSizeLimitBytes` |

### 調試技巧

```bash
# 查看 Fluent Bit 日誌
kubectl logs -n logging daemonset/fluent-bit

# 驗證 Fluent Bit 配置
kubectl exec -n logging fluent-bit-xxxxx -- /fluent-bit/bin/fluent-bit -c /fluent-bit/etc/fluent-bit.conf --dry-run

# 測試日誌輸出
curl -X POST http://localhost:9880/api/v1/logs \
  -H 'Content-Type: application/json' \
  -d '{"message":"test log"}'
```

---

## 總結

本文檔提供了完整的應用程序日誌標準實施方案,涵蓋:

✅ **結構化日誌設計** (Serilog, JSON 格式)
✅ **日誌級別規範** (Trace → Debug → Info → Warning → Error → Critical)
✅ **上下文豐富** (Correlation ID, User Context, Kubernetes Metadata)
✅ **集中化日誌收集** (Fluent Bit, Azure Log Analytics, Elasticsearch)
✅ **敏感信息過濾** (PII Redaction, GDPR 合規)
✅ **日誌查詢與分析** (KQL, Grafana, Kibana)

透過本指南,開發團隊可以實現:
- **高可查詢性**: 結構化日誌支持複雜過濾和聚合
- **分佈式追蹤**: Correlation ID 關聯整個請求鏈路
- **安全合規**: 自動過濾敏感信息,符合 GDPR/CCPA
- **長期存儲**: 熱 + 冷存儲策略,平衡成本與性能
