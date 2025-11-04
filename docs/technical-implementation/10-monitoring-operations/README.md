# Phase 10: 監控與運維 (Monitoring & Operations)

## 階段概述

本階段定義 AI Workflow Platform 的完整監控與運維策略,包括應用監控、日誌管理、告警策略、性能追蹤和運維自動化。

## 目標

- ✅ 實現全面的應用監控
- ✅ 建立集中化日誌管理
- ✅ 配置智能告警機制
- ✅ 追蹤性能指標和 SLA
- ✅ 實現運維自動化

## 文檔結構

### 1. 應用程序監控
**文件**: `application-monitoring.md`

**內容**:
- Application Insights 集成
- 性能指標收集 (CPU, Memory, Response Time)
- 依賴追蹤 (Database, Redis, External APIs)
- 用戶行為分析
- 錯誤追蹤和異常監控
- 自定義指標和事件
- 分佈式追蹤 (Distributed Tracing)

**技術重點**:
- Application Insights / Azure Monitor
- OpenTelemetry
- Prometheus + Grafana
- Jaeger (Distributed Tracing)

### 2. 日誌管理
**文件**: `logging-management.md`

**內容**:
- 結構化日誌 (Serilog)
- 日誌級別策略 (Debug, Info, Warning, Error, Fatal)
- 集中化日誌收集 (ELK Stack)
- 日誌輪轉和保留
- 敏感信息過濾
- 日誌搜索和分析
- 日誌安全和合規

**技術重點**:
- Serilog 8.0
- Elasticsearch + Logstash + Kibana (ELK)
- Seq (開發環境)
- Azure Log Analytics

### 3. 告警與通知
**文件**: `alerting-notification.md`

**內容**:
- 告警規則配置
- 閾值設置 (CPU > 80%, Memory > 90%)
- 告警通道 (Email, Slack, Teams)
- 告警優先級分類
- On-Call 輪值機制
- 告警降噪策略
- 告警響應流程

**技術重點**:
- Azure Monitor Alerts
- Prometheus Alertmanager
- PagerDuty / Opsgenie
- Slack/Teams Webhooks

### 4. 性能追蹤與優化
**文件**: `performance-tracking.md`

**內容**:
- APM (Application Performance Monitoring)
- 慢查詢檢測
- API 響應時間追蹤
- 數據庫性能監控
- 前端性能監控 (Core Web Vitals)
- 性能瓶頸分析
- 性能優化建議

**技術重點**:
- Application Insights
- MiniProfiler
- Lighthouse CI
- PostgreSQL pg_stat_statements

### 5. 運維自動化
**文件**: `operations-automation.md`

**內容**:
- 自動化部署 (GitOps)
- 自動擴容 (HPA)
- 自動備份和恢復
- 自動化健康檢查
- Runbook 自動化
- ChatOps 集成
- 自愈機制 (Self-Healing)

**技術重點**:
- ArgoCD (GitOps)
- Kubernetes HPA
- Azure Automation
- Ansible

## 監控架構圖

```
┌─────────────────────────────────────────────────┐
│              Monitoring Stack                    │
│                                                  │
│  ┌──────────────┐  ┌──────────────┐            │
│  │ Application  │  │  Kubernetes  │            │
│  │  Insights    │  │   Metrics    │            │
│  └──────┬───────┘  └──────┬───────┘            │
│         │                  │                     │
│         └──────────┬───────┘                     │
│                    │                             │
│         ┌──────────▼───────────┐                │
│         │    Prometheus         │                │
│         └──────────┬───────────┘                │
│                    │                             │
│         ┌──────────▼───────────┐                │
│         │      Grafana          │                │
│         │   (Visualization)     │                │
│         └───────────────────────┘                │
│                                                  │
│  ┌──────────────┐  ┌──────────────┐            │
│  │ Serilog/ELK  │  │  Jaeger      │            │
│  │  (Logging)   │  │  (Tracing)   │            │
│  └──────────────┘  └──────────────┘            │
│                                                  │
│  ┌──────────────────────────────────────────┐  │
│  │        Alertmanager / Azure Monitor       │  │
│  │              (Alerting)                   │  │
│  └─────────────────┬────────────────────────┘  │
│                    │                             │
└────────────────────┼─────────────────────────────┘
                     │
        ┌────────────┴──────────────┐
        │                           │
   ┌────▼────┐              ┌──────▼──────┐
   │  Email  │              │ Slack/Teams │
   └─────────┘              └─────────────┘
```

## Application Insights 配置

### .NET Configuration

```csharp
// Program.cs
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
    options.EnableAdaptiveSampling = true;
    options.EnableQuickPulseMetricStream = true;
});

// 自定義遙測初始化器
builder.Services.AddSingleton<ITelemetryInitializer, CustomTelemetryInitializer>();

public sealed class CustomTelemetryInitializer : ITelemetryInitializer
{
    public void Initialize(ITelemetry telemetry)
    {
        if (telemetry is RequestTelemetry requestTelemetry)
        {
            requestTelemetry.Properties["Environment"] = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            requestTelemetry.Properties["ApplicationVersion"] = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
        }
    }
}
```

### Custom Metrics

```csharp
public sealed class WorkflowMetricsService(TelemetryClient telemetryClient)
{
    public void TrackWorkflowCreated(int userId)
    {
        telemetryClient.TrackEvent("WorkflowCreated", new Dictionary<string, string>
        {
            { "UserId", userId.ToString() }
        });

        telemetryClient.GetMetric("WorkflowCreatedCount").TrackValue(1);
    }

    public void TrackWorkflowExecutionTime(TimeSpan duration, bool success)
    {
        telemetryClient.TrackMetric("WorkflowExecutionDuration", duration.TotalMilliseconds, new Dictionary<string, string>
        {
            { "Success", success.ToString() }
        });
    }

    public void TrackDependency(string dependencyName, TimeSpan duration, bool success)
    {
        telemetryClient.TrackDependency(
            dependencyTypeName: "External API",
            dependencyName: dependencyName,
            data: dependencyName,
            startTime: DateTimeOffset.UtcNow.Subtract(duration),
            duration: duration,
            success: success);
    }
}
```

## Serilog 配置

### Structured Logging

```csharp
// Program.cs
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("ApplicationName", "AIWorkflow")
    .Enrich.WithProperty("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
    .Enrich.WithMachineName()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.File(
        path: "logs/aiworkflow-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
    {
        AutoRegisterTemplate = true,
        IndexFormat = $"aiworkflow-logs-{DateTime.UtcNow:yyyy.MM}",
        NumberOfShards = 2,
        NumberOfReplicas = 1
    })
    .WriteTo.ApplicationInsights(
        telemetryConfiguration: TelemetryConfiguration.CreateDefault(),
        telemetryConverter: TelemetryConverter.Traces)
    .CreateLogger();

builder.Host.UseSerilog();
```

### Structured Log Example

```csharp
public sealed class WorkflowService(ILogger<WorkflowService> logger)
{
    public async Task<Result<int>> CreateWorkflowAsync(CreateWorkflowDto dto)
    {
        using (logger.BeginScope(new Dictionary<string, object>
        {
            ["UserId"] = userId,
            ["WorkflowName"] = dto.Name
        }))
        {
            logger.LogInformation("Creating workflow {WorkflowName} for user {UserId}", dto.Name, userId);

            try
            {
                var workflow = Workflow.Create(dto.Name, dto.Description, userId);
                await repository.AddAsync(workflow);

                logger.LogInformation("Workflow {WorkflowId} created successfully", workflow.Id);

                return Result<int>.Success(workflow.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create workflow {WorkflowName} for user {UserId}", dto.Name, userId);
                throw;
            }
        }
    }
}
```

## Prometheus + Grafana

### Prometheus Metrics Endpoint

```csharp
// Install: dotnet add package prometheus-net.AspNetCore
// Program.cs
app.UseHttpMetrics();  // 收集 HTTP 指標

app.MapMetrics();      // 暴露 /metrics 端點

// 自定義指標
public sealed class CustomMetrics
{
    public static readonly Counter WorkflowCreatedCounter = Metrics
        .CreateCounter("aiworkflow_workflows_created_total", "Total workflows created");

    public static readonly Histogram WorkflowExecutionDuration = Metrics
        .CreateHistogram("aiworkflow_workflow_execution_duration_seconds", "Workflow execution duration");

    public static readonly Gauge ActiveWorkflowExecutions = Metrics
        .CreateGauge("aiworkflow_active_workflow_executions", "Number of active workflow executions");
}

// 使用示例
public async Task ExecuteWorkflowAsync(int workflowId)
{
    CustomMetrics.ActiveWorkflowExecutions.Inc();
    using (CustomMetrics.WorkflowExecutionDuration.NewTimer())
    {
        try
        {
            await DoExecuteWorkflowAsync(workflowId);
            CustomMetrics.WorkflowCreatedCounter.Inc();
        }
        finally
        {
            CustomMetrics.ActiveWorkflowExecutions.Dec();
        }
    }
}
```

### Grafana Dashboard

```yaml
# grafana-dashboard.json (簡化)
{
  "dashboard": {
    "title": "AI Workflow Platform",
    "panels": [
      {
        "title": "Request Rate",
        "targets": [
          {
            "expr": "rate(http_requests_received_total[5m])"
          }
        ]
      },
      {
        "title": "Error Rate",
        "targets": [
          {
            "expr": "rate(http_requests_received_total{code=~\"5..\"}[5m])"
          }
        ]
      },
      {
        "title": "Response Time (95th percentile)",
        "targets": [
          {
            "expr": "histogram_quantile(0.95, rate(http_request_duration_seconds_bucket[5m]))"
          }
        ]
      }
    ]
  }
}
```

## 告警配置

### Azure Monitor Alerts

```yaml
# azure-alert-rules.yaml
alerts:
  - name: "High CPU Usage"
    condition: "avg_cpu_usage > 80%"
    duration: "5 minutes"
    severity: "Warning"
    action_group: "ops-team"

  - name: "High Memory Usage"
    condition: "avg_memory_usage > 90%"
    duration: "5 minutes"
    severity: "Critical"
    action_group: "ops-team"

  - name: "High Error Rate"
    condition: "error_rate > 5%"
    duration: "1 minute"
    severity: "Critical"
    action_group: "dev-team"

  - name: "Slow API Response"
    condition: "p95_response_time > 2000ms"
    duration: "5 minutes"
    severity: "Warning"
    action_group: "dev-team"

  - name: "Database Connection Failure"
    condition: "db_connection_failures > 0"
    duration: "1 minute"
    severity: "Critical"
    action_group: "ops-team,dev-team"
```

### Prometheus Alertmanager

```yaml
# alertmanager.yml
groups:
  - name: aiworkflow
    rules:
      - alert: HighErrorRate
        expr: rate(http_requests_received_total{code=~"5.."}[5m]) > 0.05
        for: 1m
        labels:
          severity: critical
        annotations:
          summary: "High error rate detected"
          description: "Error rate is {{ $value }} per second"

      - alert: HighMemoryUsage
        expr: container_memory_usage_bytes / container_spec_memory_limit_bytes > 0.9
        for: 5m
        labels:
          severity: warning
        annotations:
          summary: "High memory usage"
          description: "Memory usage is {{ $value | humanizePercentage }}"

      - alert: PodCrashLooping
        expr: rate(kube_pod_container_status_restarts_total[15m]) > 0
        for: 1m
        labels:
          severity: critical
        annotations:
          summary: "Pod is crash looping"
          description: "Pod {{ $labels.pod }} is crash looping"
```

## 健康檢查

### ASP.NET Core Health Checks

```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddNpgSql(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "postgres",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "db", "postgres" })
    .AddRedis(
        redisConnectionString: builder.Configuration["Redis:Configuration"]!,
        name: "redis",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "cache", "redis" })
    .AddDbContextCheck<ApplicationDbContext>(
        name: "efcore",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "db" });

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false
});
```

### Kubernetes Probes

```yaml
# k8s/backend-deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: backend-api
spec:
  template:
    spec:
      containers:
      - name: api
        image: backend-api:latest
        ports:
        - containerPort: 8080
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
        startupProbe:
          httpGet:
            path: /health
            port: 8080
          initialDelaySeconds: 0
          periodSeconds: 5
          failureThreshold: 30
```

## 檢查清單

### 監控實施
- [ ] 配置 Application Insights
- [ ] 實現自定義指標收集
- [ ] 配置分佈式追蹤
- [ ] 設置 Prometheus + Grafana
- [ ] 創建監控儀表板

### 日誌管理
- [ ] 配置 Serilog 結構化日誌
- [ ] 設置 ELK Stack
- [ ] 配置日誌輪轉
- [ ] 實現敏感信息過濾
- [ ] 建立日誌保留策略

### 告警配置
- [ ] 定義告警規則
- [ ] 配置告警通道
- [ ] 設置 On-Call 輪值
- [ ] 實現告警降噪
- [ ] 建立告警響應流程

### 性能監控
- [ ] 追蹤 API 響應時間
- [ ] 監控數據庫性能
- [ ] 配置前端性能監控
- [ ] 識別性能瓶頸
- [ ] 定期性能優化

### 運維自動化
- [ ] 實現 GitOps 部署
- [ ] 配置自動擴容 (HPA)
- [ ] 實現自動備份
- [ ] 配置健康檢查
- [ ] 建立 Runbook

## 相關文檔

- [Phase 8: 部署架構](../8-deployment-architecture/README.md)
- [Phase 9: 安全標準](../9-security-standards/README.md)
- [Phase 11: 災難恢復計劃](../11-disaster-recovery/README.md)

## 參考資源

### 官方文檔
- [Application Insights](https://learn.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview)
- [Prometheus Documentation](https://prometheus.io/docs/)
- [Grafana Documentation](https://grafana.com/docs/)
- [ELK Stack](https://www.elastic.co/what-is/elk-stack)

### 最佳實踐
- [Site Reliability Engineering (SRE)](https://sre.google/)
- [Observability Best Practices](https://opentelemetry.io/docs/)

---

**文檔維護**: SRE Team
**最後更新**: 2025-01-01
**狀態**: ✅ 完整
