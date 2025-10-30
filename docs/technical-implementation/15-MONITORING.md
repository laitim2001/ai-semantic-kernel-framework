# 15. 監控系統設計 (Monitoring System Design)

## 文檔資訊

| 項目 | 內容 |
|------|------|
| **文檔版本** | 1.0.0 |
| **創建日期** | 2025-01-15 |
| **最後更新** | 2025-01-15 |
| **狀態** | Draft |
| **作者** | AI Workflow Platform Team |
| **關聯文檔** | 11-DEPLOYMENT-ARCHITECTURE.md, 16-LOGGING.md, 17-ALERTING.md |

---

## 目錄

- [15.1 監控架構總覽](#151-監控架構總覽)
- [15.2 Prometheus 指標收集](#152-prometheus-指標收集)
- [15.3 Grafana 可視化](#153-grafana-可視化)
- [15.4 應用性能監控 (APM)](#154-應用性能監控-apm)
- [15.5 基礎設施監控](#155-基礎設施監控)
- [15.6 業務指標監控](#156-業務指標監控)
- [15.7 健康檢查](#157-健康檢查)
- [15.8 監控最佳實踐](#158-監控最佳實踐)

---

## 15.1 監控架構總覽

### 15.1.1 監控層次架構

```mermaid
graph TB
    subgraph "Application Layer"
        Apps[Microservices<br/>8 Services]
    end

    subgraph "Collection Layer"
        Prometheus[Prometheus<br/>Metrics Collection]
        Jaeger[Jaeger<br/>Distributed Tracing]
        Loki[Loki<br/>Log Aggregation]
    end

    subgraph "Storage Layer"
        PrometheusDB[(Prometheus TSDB)]
        JaegerDB[(Jaeger Storage)]
        LokiDB[(Loki Storage)]
    end

    subgraph "Visualization Layer"
        Grafana[Grafana<br/>Dashboards]
        Alertmanager[Alertmanager<br/>Alert Routing]
    end

    subgraph "Notification Layer"
        Email[Email]
        Slack[Slack]
        PagerDuty[PagerDuty]
    end

    Apps --> Prometheus
    Apps --> Jaeger
    Apps --> Loki

    Prometheus --> PrometheusDB
    Jaeger --> JaegerDB
    Loki --> LokiDB

    PrometheusDB --> Grafana
    JaegerDB --> Grafana
    LokiDB --> Grafana

    Prometheus --> Alertmanager
    Alertmanager --> Email
    Alertmanager --> Slack
    Alertmanager --> PagerDuty
```

### 15.1.2 監控指標分類

**四個黃金信號 (Four Golden Signals):**

| 信號 | 含義 | 指標示例 | 告警閾值 |
|------|------|----------|----------|
| **Latency** | 延遲 | P50, P95, P99 響應時間 | P95 > 500ms |
| **Traffic** | 流量 | 每秒請求數 (RPS) | RPS > 1000 |
| **Errors** | 錯誤率 | 4xx/5xx 錯誤比例 | 錯誤率 > 1% |
| **Saturation** | 飽和度 | CPU/Memory 使用率 | CPU > 80% |

**RED 方法 (Rate, Errors, Duration):**
- **Rate**: 每秒請求數
- **Errors**: 錯誤請求數
- **Duration**: 請求持續時間

**USE 方法 (Utilization, Saturation, Errors):**
- **Utilization**: 資源使用率
- **Saturation**: 資源飽和度
- **Errors**: 錯誤計數

### 15.1.3 監控覆蓋範圍

```yaml
monitoring_scope:
  # 應用層監控
  application:
    - HTTP 請求指標 (請求數、延遲、錯誤率)
    - 業務指標 (Agent 創建數、執行次數)
    - 自定義指標 (OpenAI API 調用、費用)

  # 中間件監控
  middleware:
    - Database 連接池 (連接數、等待時間)
    - Redis 緩存 (命中率、延遲)
    - RabbitMQ 消息隊列 (隊列長度、消費速率)

  # 基礎設施監控
  infrastructure:
    - Kubernetes 集群 (Node/Pod 狀態)
    - Container 資源 (CPU/Memory/Network/Disk)
    - Network 流量 (帶寬、丟包率)

  # 外部依賴監控
  external:
    - OpenAI API (可用性、延遲、配額)
    - Azure Services (可用性、延遲)
```

---

## 15.2 Prometheus 指標收集

### 15.2.1 Prometheus 配置

**k8s/monitoring/prometheus/prometheus-config.yaml:**

```yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: prometheus-config
  namespace: monitoring
data:
  prometheus.yml: |
    global:
      scrape_interval: 15s
      evaluation_interval: 15s
      external_labels:
        cluster: 'ai-workflow-prod'
        environment: 'production'

    # Alertmanager 配置
    alerting:
      alertmanagers:
      - static_configs:
        - targets: ['alertmanager:9093']

    # 規則文件
    rule_files:
      - '/etc/prometheus/rules/*.yml'

    # 抓取配置
    scrape_configs:
      # Prometheus 自身
      - job_name: 'prometheus'
        static_configs:
          - targets: ['localhost:9090']

      # Kubernetes API Server
      - job_name: 'kubernetes-apiservers'
        kubernetes_sd_configs:
        - role: endpoints
        scheme: https
        tls_config:
          ca_file: /var/run/secrets/kubernetes.io/serviceaccount/ca.crt
        bearer_token_file: /var/run/secrets/kubernetes.io/serviceaccount/token
        relabel_configs:
        - source_labels: [__meta_kubernetes_namespace, __meta_kubernetes_service_name, __meta_kubernetes_endpoint_port_name]
          action: keep
          regex: default;kubernetes;https

      # Kubernetes Nodes
      - job_name: 'kubernetes-nodes'
        kubernetes_sd_configs:
        - role: node
        scheme: https
        tls_config:
          ca_file: /var/run/secrets/kubernetes.io/serviceaccount/ca.crt
        bearer_token_file: /var/run/secrets/kubernetes.io/serviceaccount/token
        relabel_configs:
        - action: labelmap
          regex: __meta_kubernetes_node_label_(.+)

      # Kubernetes Pods (自動發現)
      - job_name: 'kubernetes-pods'
        kubernetes_sd_configs:
        - role: pod
        relabel_configs:
        # 只抓取有 prometheus.io/scrape: "true" 注解的 Pod
        - source_labels: [__meta_kubernetes_pod_annotation_prometheus_io_scrape]
          action: keep
          regex: true

        # 從注解讀取抓取路徑
        - source_labels: [__meta_kubernetes_pod_annotation_prometheus_io_path]
          action: replace
          target_label: __metrics_path__
          regex: (.+)

        # 從注解讀取抓取端口
        - source_labels: [__address__, __meta_kubernetes_pod_annotation_prometheus_io_port]
          action: replace
          regex: ([^:]+)(?::\d+)?;(\d+)
          replacement: $1:$2
          target_label: __address__

        # 添加標籤
        - action: labelmap
          regex: __meta_kubernetes_pod_label_(.+)
        - source_labels: [__meta_kubernetes_namespace]
          action: replace
          target_label: kubernetes_namespace
        - source_labels: [__meta_kubernetes_pod_name]
          action: replace
          target_label: kubernetes_pod_name

      # Agent Service
      - job_name: 'agent-service'
        kubernetes_sd_configs:
        - role: pod
          namespaces:
            names:
            - ai-workflow-prod
        relabel_configs:
        - source_labels: [__meta_kubernetes_pod_label_app]
          action: keep
          regex: agent-service
        - source_labels: [__meta_kubernetes_pod_ip]
          action: replace
          target_label: __address__
          replacement: $1:8080

      # PostgreSQL Exporter
      - job_name: 'postgresql'
        static_configs:
          - targets: ['postgresql-exporter:9187']
            labels:
              service: 'postgresql'

      # Redis Exporter
      - job_name: 'redis'
        static_configs:
          - targets: ['redis-exporter:9121']
            labels:
              service: 'redis'

      # RabbitMQ
      - job_name: 'rabbitmq'
        static_configs:
          - targets: ['rabbitmq:15692']
            labels:
              service: 'rabbitmq'

      # Node Exporter (主機指標)
      - job_name: 'node-exporter'
        kubernetes_sd_configs:
        - role: node
        relabel_configs:
        - source_labels: [__address__]
          regex: '(.*):10250'
          replacement: '${1}:9100'
          target_label: __address__

      # Kube State Metrics
      - job_name: 'kube-state-metrics'
        static_configs:
          - targets: ['kube-state-metrics.kube-system.svc.cluster.local:8080']
```

### 15.2.2 應用指標暴露

**C# 應用程序指標暴露 (使用 prometheus-net):**

```csharp
// Program.cs
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// 添加 Prometheus 指標
builder.Services.AddSingleton<MetricsService>();

var app = builder.Build();

// 啟用 Prometheus HTTP 指標中間件
app.UseHttpMetrics(options =>
{
    // 添加路由標籤
    options.AddRouteParameter("controller");
    options.AddRouteParameter("action");
});

// 暴露 /metrics 端點
app.MapMetrics();

app.Run();
```

**自定義指標服務:**

```csharp
using Prometheus;

public class MetricsService
{
    // Counter: 只增不減的計數器
    private static readonly Counter AgentCreationCounter = Metrics
        .CreateCounter(
            "agent_creations_total",
            "Total number of agents created",
            new CounterConfiguration
            {
                LabelNames = new[] { "user_id", "persona_id", "status" }
            });

    // Gauge: 可增可減的計數器
    private static readonly Gauge ActiveAgentsGauge = Metrics
        .CreateGauge(
            "active_agents",
            "Number of currently active agents",
            new GaugeConfiguration
            {
                LabelNames = new[] { "status" }
            });

    // Histogram: 分佈統計
    private static readonly Histogram ExecutionDurationHistogram = Metrics
        .CreateHistogram(
            "execution_duration_seconds",
            "Histogram of execution duration in seconds",
            new HistogramConfiguration
            {
                LabelNames = new[] { "agent_id", "status" },
                Buckets = Histogram.ExponentialBuckets(start: 0.01, factor: 2, count: 10)
                // 0.01, 0.02, 0.04, 0.08, 0.16, 0.32, 0.64, 1.28, 2.56, 5.12
            });

    // Summary: 分位數統計
    private static readonly Summary OpenAIApiDurationSummary = Metrics
        .CreateSummary(
            "openai_api_duration_seconds",
            "Summary of OpenAI API call duration in seconds",
            new SummaryConfiguration
            {
                LabelNames = new[] { "model", "status" },
                Objectives = new[]
                {
                    new QuantileEpsilonPair(0.5, 0.05),   // P50
                    new QuantileEpsilonPair(0.9, 0.01),   // P90
                    new QuantileEpsilonPair(0.95, 0.005), // P95
                    new QuantileEpsilonPair(0.99, 0.001)  // P99
                }
            });

    // 業務指標: Agent 創建
    public void RecordAgentCreation(Guid userId, Guid personaId, string status)
    {
        AgentCreationCounter
            .WithLabels(userId.ToString(), personaId.ToString(), status)
            .Inc();
    }

    // 業務指標: 活躍 Agent 數量
    public void UpdateActiveAgents(string status, int count)
    {
        ActiveAgentsGauge
            .WithLabels(status)
            .Set(count);
    }

    // 性能指標: 執行時間
    public IDisposable MeasureExecutionDuration(Guid agentId, string status)
    {
        return ExecutionDurationHistogram
            .WithLabels(agentId.ToString(), status)
            .NewTimer();
    }

    // 外部 API 指標: OpenAI API 調用
    public void RecordOpenAIApiCall(string model, string status, double durationSeconds)
    {
        OpenAIApiDurationSummary
            .WithLabels(model, status)
            .Observe(durationSeconds);
    }
}
```

**使用自定義指標:**

```csharp
public class AgentService
{
    private readonly MetricsService _metrics;

    public AgentService(MetricsService metrics)
    {
        _metrics = metrics;
    }

    public async Task<Agent> CreateAgentAsync(CreateAgentRequest request)
    {
        try
        {
            var agent = new Agent
            {
                UserId = request.UserId,
                PersonaId = request.PersonaId,
                Name = request.Name
            };

            await _dbContext.Agents.AddAsync(agent);
            await _dbContext.SaveChangesAsync();

            // 記錄指標
            _metrics.RecordAgentCreation(
                request.UserId,
                request.PersonaId,
                "success"
            );

            return agent;
        }
        catch (Exception ex)
        {
            _metrics.RecordAgentCreation(
                request.UserId,
                request.PersonaId,
                "error"
            );
            throw;
        }
    }

    public async Task<ExecutionResult> ExecuteAgentAsync(Guid agentId)
    {
        // 測量執行時間
        using var timer = _metrics.MeasureExecutionDuration(agentId, "success");

        try
        {
            // 調用 OpenAI API
            var startTime = DateTime.UtcNow;
            var response = await _openAIClient.GetCompletionAsync(...);
            var duration = (DateTime.UtcNow - startTime).TotalSeconds;

            // 記錄 OpenAI API 指標
            _metrics.RecordOpenAIApiCall("gpt-4", "success", duration);

            return new ExecutionResult { ... };
        }
        catch (Exception ex)
        {
            _metrics.RecordOpenAIApiCall("gpt-4", "error", 0);
            throw;
        }
    }
}
```

### 15.2.3 PostgreSQL Exporter

**部署 PostgreSQL Exporter:**

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: postgresql-exporter
  namespace: monitoring
spec:
  replicas: 1
  selector:
    matchLabels:
      app: postgresql-exporter
  template:
    metadata:
      labels:
        app: postgresql-exporter
    spec:
      containers:
      - name: postgresql-exporter
        image: quay.io/prometheuscommunity/postgres-exporter:latest
        ports:
        - containerPort: 9187
        env:
        - name: DATA_SOURCE_NAME
          value: "postgresql://postgres:password@postgresql:5432/postgres?sslmode=disable"
        resources:
          requests:
            cpu: 100m
            memory: 128Mi
          limits:
            cpu: 500m
            memory: 512Mi

---
apiVersion: v1
kind: Service
metadata:
  name: postgresql-exporter
  namespace: monitoring
  labels:
    app: postgresql-exporter
spec:
  ports:
  - port: 9187
    targetPort: 9187
  selector:
    app: postgresql-exporter
```

**PostgreSQL 監控指標:**

```promql
# 數據庫連接數
pg_stat_database_numbackends{datname="agent_db"}

# 數據庫大小
pg_database_size_bytes{datname="agent_db"}

# 查詢執行時間
rate(pg_stat_statements_mean_exec_time_seconds[5m])

# 慢查詢數量
pg_stat_statements_calls{query=~".*SELECT.*"}

# 死鎖數量
rate(pg_stat_database_deadlocks[5m])

# 緩存命中率
rate(pg_stat_database_blks_hit[5m]) /
  (rate(pg_stat_database_blks_hit[5m]) + rate(pg_stat_database_blks_read[5m]))
```

### 15.2.4 Redis Exporter

**部署 Redis Exporter:**

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: redis-exporter
  namespace: monitoring
spec:
  replicas: 1
  selector:
    matchLabels:
      app: redis-exporter
  template:
    metadata:
      labels:
        app: redis-exporter
    spec:
      containers:
      - name: redis-exporter
        image: oliver006/redis_exporter:latest
        ports:
        - containerPort: 9121
        env:
        - name: REDIS_ADDR
          value: "redis:6379"
        - name: REDIS_PASSWORD
          valueFrom:
            secretKeyRef:
              name: redis-secrets
              key: redis-password
        resources:
          requests:
            cpu: 100m
            memory: 128Mi
```

**Redis 監控指標:**

```promql
# 內存使用
redis_memory_used_bytes

# 連接數
redis_connected_clients

# 緩存命中率
rate(redis_keyspace_hits_total[5m]) /
  (rate(redis_keyspace_hits_total[5m]) + rate(redis_keyspace_misses_total[5m]))

# 每秒操作數
rate(redis_commands_processed_total[1m])

# Key 過期數
rate(redis_expired_keys_total[5m])
```

---

## 15.3 Grafana 可視化

### 15.3.1 Grafana 配置

**部署 Grafana:**

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: grafana
  namespace: monitoring
spec:
  replicas: 1
  selector:
    matchLabels:
      app: grafana
  template:
    metadata:
      labels:
        app: grafana
    spec:
      containers:
      - name: grafana
        image: grafana/grafana:latest
        ports:
        - containerPort: 3000
        env:
        - name: GF_SECURITY_ADMIN_USER
          value: admin
        - name: GF_SECURITY_ADMIN_PASSWORD
          valueFrom:
            secretKeyRef:
              name: grafana-secrets
              key: admin-password
        - name: GF_USERS_ALLOW_SIGN_UP
          value: "false"
        - name: GF_INSTALL_PLUGINS
          value: "grafana-piechart-panel"
        volumeMounts:
        - name: grafana-storage
          mountPath: /var/lib/grafana
        - name: grafana-datasources
          mountPath: /etc/grafana/provisioning/datasources
        - name: grafana-dashboards-config
          mountPath: /etc/grafana/provisioning/dashboards
        - name: grafana-dashboards
          mountPath: /var/lib/grafana/dashboards
        resources:
          requests:
            cpu: 250m
            memory: 512Mi
          limits:
            cpu: 1000m
            memory: 2Gi
      volumes:
      - name: grafana-storage
        persistentVolumeClaim:
          claimName: grafana-pvc
      - name: grafana-datasources
        configMap:
          name: grafana-datasources
      - name: grafana-dashboards-config
        configMap:
          name: grafana-dashboards-config
      - name: grafana-dashboards
        configMap:
          name: grafana-dashboards
```

**數據源配置:**

```yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: grafana-datasources
  namespace: monitoring
data:
  prometheus.yaml: |
    apiVersion: 1
    datasources:
    - name: Prometheus
      type: prometheus
      access: proxy
      url: http://prometheus:9090
      isDefault: true
      editable: false
      jsonData:
        timeInterval: 15s

    - name: Loki
      type: loki
      access: proxy
      url: http://loki:3100
      editable: false

    - name: Jaeger
      type: jaeger
      access: proxy
      url: http://jaeger-query:16686
      editable: false
```

### 15.3.2 Dashboard 配置

**Agent Service Dashboard (JSON 格式):**

```json
{
  "dashboard": {
    "title": "Agent Service Monitoring",
    "tags": ["agent-service", "production"],
    "timezone": "browser",
    "panels": [
      {
        "id": 1,
        "title": "Request Rate",
        "type": "graph",
        "targets": [
          {
            "expr": "rate(http_requests_total{job=\"agent-service\"}[5m])",
            "legendFormat": "{{method}} {{path}}"
          }
        ],
        "gridPos": {"x": 0, "y": 0, "w": 12, "h": 8}
      },
      {
        "id": 2,
        "title": "Request Duration (P95)",
        "type": "graph",
        "targets": [
          {
            "expr": "histogram_quantile(0.95, rate(http_request_duration_seconds_bucket{job=\"agent-service\"}[5m]))",
            "legendFormat": "P95"
          }
        ],
        "gridPos": {"x": 12, "y": 0, "w": 12, "h": 8}
      },
      {
        "id": 3,
        "title": "Error Rate",
        "type": "graph",
        "targets": [
          {
            "expr": "rate(http_requests_total{job=\"agent-service\",status=~\"5..\"}[5m])",
            "legendFormat": "5xx Errors"
          }
        ],
        "gridPos": {"x": 0, "y": 8, "w": 12, "h": 8}
      },
      {
        "id": 4,
        "title": "Active Agents",
        "type": "stat",
        "targets": [
          {
            "expr": "active_agents{status=\"active\"}",
            "legendFormat": "Active"
          }
        ],
        "gridPos": {"x": 12, "y": 8, "w": 6, "h": 4}
      },
      {
        "id": 5,
        "title": "CPU Usage",
        "type": "graph",
        "targets": [
          {
            "expr": "rate(container_cpu_usage_seconds_total{pod=~\"agent-service.*\"}[5m]) * 100",
            "legendFormat": "{{pod}}"
          }
        ],
        "gridPos": {"x": 0, "y": 16, "w": 12, "h": 8}
      },
      {
        "id": 6,
        "title": "Memory Usage",
        "type": "graph",
        "targets": [
          {
            "expr": "container_memory_usage_bytes{pod=~\"agent-service.*\"} / 1024 / 1024 / 1024",
            "legendFormat": "{{pod}}"
          }
        ],
        "gridPos": {"x": 12, "y": 16, "w": 12, "h": 8}
      }
    ]
  }
}
```

### 15.3.3 關鍵 Dashboard 列表

**1. Cluster Overview Dashboard:**
- Kubernetes 集群整體健康狀態
- Node 資源使用情況
- Pod 狀態統計
- Network I/O 流量

**2. Service Dashboard (每個服務一個):**
- Request Rate (RPS)
- Request Duration (P50, P95, P99)
- Error Rate (4xx, 5xx)
- CPU/Memory Usage
- Database Connection Pool

**3. Database Dashboard:**
- PostgreSQL 連接數
- 查詢執行時間
- 慢查詢統計
- 數據庫大小
- 緩存命中率

**4. Redis Dashboard:**
- 內存使用
- 連接數
- 緩存命中率
- Key 過期率
- 每秒操作數

**5. Business Metrics Dashboard:**
- Agent 創建數
- 執行次數
- 用戶活躍度
- OpenAI API 調用統計
- 成本追蹤

---

## 15.4 應用性能監控 (APM)

### 15.4.1 分佈式追蹤 (Jaeger)

**部署 Jaeger:**

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: jaeger
  namespace: monitoring
spec:
  replicas: 1
  selector:
    matchLabels:
      app: jaeger
  template:
    metadata:
      labels:
        app: jaeger
    spec:
      containers:
      - name: jaeger
        image: jaegertracing/all-in-one:latest
        ports:
        - containerPort: 5775
          protocol: UDP
        - containerPort: 6831
          protocol: UDP
        - containerPort: 6832
          protocol: UDP
        - containerPort: 5778
          protocol: TCP
        - containerPort: 16686
          protocol: TCP
        - containerPort: 14268
          protocol: TCP
        - containerPort: 14250
          protocol: TCP
        env:
        - name: COLLECTOR_ZIPKIN_HTTP_PORT
          value: "9411"
        resources:
          requests:
            cpu: 500m
            memory: 1Gi
          limits:
            cpu: 2000m
            memory: 4Gi
```

**C# 應用集成 OpenTelemetry:**

```csharp
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// 添加 OpenTelemetry Tracing
builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .AddSource("AgentService")
            .SetResourceBuilder(
                ResourceBuilder.CreateDefault()
                    .AddService("agent-service", serviceVersion: "1.0.0"))
            .AddAspNetCoreInstrumentation(options =>
            {
                options.RecordException = true;
                options.EnrichWithHttpRequest = (activity, httpRequest) =>
                {
                    activity.SetTag("http.user_id", httpRequest.Headers["X-User-Id"]);
                };
            })
            .AddHttpClientInstrumentation()
            .AddEntityFrameworkCoreInstrumentation(options =>
            {
                options.SetDbStatementForText = true;
            })
            .AddJaegerExporter(options =>
            {
                options.AgentHost = "jaeger";
                options.AgentPort = 6831;
            });
    });

var app = builder.Build();
app.Run();
```

**手動創建 Span:**

```csharp
using System.Diagnostics;

public class AgentService
{
    private static readonly ActivitySource ActivitySource = new("AgentService");

    public async Task<ExecutionResult> ExecuteAgentAsync(Guid agentId)
    {
        using var activity = ActivitySource.StartActivity("ExecuteAgent");
        activity?.SetTag("agent.id", agentId);

        try
        {
            // 子 Span: 加載 Agent
            using (var loadActivity = ActivitySource.StartActivity("LoadAgent"))
            {
                var agent = await _dbContext.Agents.FindAsync(agentId);
                loadActivity?.SetTag("agent.name", agent.Name);
            }

            // 子 Span: 調用 OpenAI API
            using (var openaiActivity = ActivitySource.StartActivity("CallOpenAI"))
            {
                openaiActivity?.SetTag("openai.model", "gpt-4");
                var response = await _openAIClient.GetCompletionAsync(...);
                openaiActivity?.SetTag("openai.tokens", response.Usage.TotalTokens);
            }

            activity?.SetStatus(ActivityStatusCode.Ok);
            return new ExecutionResult { ... };
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.RecordException(ex);
            throw;
        }
    }
}
```

### 15.4.2 追蹤可視化

**Jaeger UI 功能:**
- 追蹤搜索: 按服務、操作、標籤搜索
- 追蹤詳情: 查看完整調用鏈
- 依賴圖: 服務依賴關系可視化
- 性能分析: 識別瓶頸和慢查詢

---

## 15.5 基礎設施監控

### 15.5.1 Node Exporter

**部署 Node Exporter (DaemonSet):**

```yaml
apiVersion: apps/v1
kind: DaemonSet
metadata:
  name: node-exporter
  namespace: monitoring
spec:
  selector:
    matchLabels:
      app: node-exporter
  template:
    metadata:
      labels:
        app: node-exporter
    spec:
      hostNetwork: true
      hostPID: true
      containers:
      - name: node-exporter
        image: prom/node-exporter:latest
        args:
        - '--path.procfs=/host/proc'
        - '--path.sysfs=/host/sys'
        - '--path.rootfs=/host/root'
        - '--collector.filesystem.mount-points-exclude=^/(sys|proc|dev|host|etc)($$|/)'
        ports:
        - containerPort: 9100
        volumeMounts:
        - name: proc
          mountPath: /host/proc
          readOnly: true
        - name: sys
          mountPath: /host/sys
          readOnly: true
        - name: root
          mountPath: /host/root
          readOnly: true
        resources:
          requests:
            cpu: 100m
            memory: 128Mi
      volumes:
      - name: proc
        hostPath:
          path: /proc
      - name: sys
        hostPath:
          path: /sys
      - name: root
        hostPath:
          path: /
```

**Node 監控指標:**

```promql
# CPU 使用率
100 - (avg by (instance) (rate(node_cpu_seconds_total{mode="idle"}[5m])) * 100)

# 內存使用率
(1 - (node_memory_MemAvailable_bytes / node_memory_MemTotal_bytes)) * 100

# 磁盤使用率
(1 - (node_filesystem_avail_bytes / node_filesystem_size_bytes)) * 100

# 網路流量
rate(node_network_receive_bytes_total[5m])
rate(node_network_transmit_bytes_total[5m])

# 磁盤 I/O
rate(node_disk_read_bytes_total[5m])
rate(node_disk_written_bytes_total[5m])
```

### 15.5.2 Kube State Metrics

**部署 Kube State Metrics:**

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: kube-state-metrics
  namespace: kube-system
spec:
  replicas: 1
  selector:
    matchLabels:
      app: kube-state-metrics
  template:
    metadata:
      labels:
        app: kube-state-metrics
    spec:
      serviceAccountName: kube-state-metrics
      containers:
      - name: kube-state-metrics
        image: registry.k8s.io/kube-state-metrics/kube-state-metrics:v2.10.0
        ports:
        - containerPort: 8080
        resources:
          requests:
            cpu: 100m
            memory: 128Mi
          limits:
            cpu: 500m
            memory: 512Mi
```

**Kubernetes 監控指標:**

```promql
# Pod 狀態
kube_pod_status_phase{namespace="ai-workflow-prod"}

# Deployment 副本數
kube_deployment_status_replicas{namespace="ai-workflow-prod"}

# Node 狀態
kube_node_status_condition{condition="Ready"}

# PVC 使用率
kubelet_volume_stats_used_bytes / kubelet_volume_stats_capacity_bytes * 100
```

---

## 15.6 業務指標監控

### 15.6.1 業務指標定義

```csharp
public class BusinessMetricsService
{
    // Agent 相關指標
    private static readonly Counter AgentCreationCounter = Metrics
        .CreateCounter("agent_creations_total", "Total agents created");

    private static readonly Gauge ActiveAgentsGauge = Metrics
        .CreateGauge("active_agents", "Currently active agents");

    // 執行相關指標
    private static readonly Counter ExecutionCounter = Metrics
        .CreateCounter("executions_total", "Total executions", new[] { "status" });

    private static readonly Histogram ExecutionDurationHistogram = Metrics
        .CreateHistogram("execution_duration_seconds", "Execution duration");

    // OpenAI API 指標
    private static readonly Counter OpenAIApiCallsCounter = Metrics
        .CreateCounter("openai_api_calls_total", "Total OpenAI API calls", new[] { "model", "status" });

    private static readonly Counter OpenAITokensCounter = Metrics
        .CreateCounter("openai_tokens_total", "Total OpenAI tokens used", new[] { "model", "type" });

    // 成本指標
    private static readonly Counter CostCounter = Metrics
        .CreateCounter("cost_usd_total", "Total cost in USD", new[] { "service" });

    // 用戶指標
    private static readonly Gauge ActiveUsersGauge = Metrics
        .CreateGauge("active_users", "Currently active users");
}
```

### 15.6.2 業務 Dashboard

**Business Metrics Dashboard PromQL:**

```promql
# 每小時 Agent 創建數
sum(increase(agent_creations_total[1h]))

# 執行成功率
sum(rate(executions_total{status="success"}[5m])) /
  sum(rate(executions_total[5m])) * 100

# OpenAI API 平均調用時間
avg(rate(openai_api_duration_seconds_sum[5m]) /
    rate(openai_api_duration_seconds_count[5m]))

# 每日 OpenAI Token 使用量
sum(increase(openai_tokens_total[24h]))

# 每日成本
sum(increase(cost_usd_total[24h]))

# 活躍用戶數
active_users
```

---

## 15.7 健康檢查

### 15.7.1 健康檢查端點實現

**C# 健康檢查實現:**

```csharp
using Microsoft.Extensions.Diagnostics.HealthChecks;

// Program.cs
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database")
    .AddCheck<RedisHealthCheck>("redis")
    .AddCheck<RabbitMQHealthCheck>("rabbitmq")
    .AddCheck<OpenAIHealthCheck>("openai");

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false  // 最基本的健康檢查
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

app.MapHealthChecks("/health/startup");

// DatabaseHealthCheck.cs
public class DatabaseHealthCheck : IHealthCheck
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync(cancellationToken);
            await dbContext.Database.CanConnectAsync(cancellationToken);

            return HealthCheckResult.Healthy("Database is reachable");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database is unreachable", ex);
        }
    }
}

// RedisHealthCheck.cs
public class RedisHealthCheck : IHealthCheck
{
    private readonly IConnectionMultiplexer _redis;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            await db.PingAsync();

            return HealthCheckResult.Healthy("Redis is reachable");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Redis is unreachable", ex);
        }
    }
}
```

### 15.7.2 Kubernetes 探針配置

```yaml
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
    path: /health/startup
    port: 8080
  initialDelaySeconds: 0
  periodSeconds: 5
  timeoutSeconds: 3
  failureThreshold: 30
```

---

## 15.8 監控最佳實踐

### 15.8.1 監控原則

**1. 監控目標:**
- **可用性**: 服務是否正常運行
- **性能**: 響應時間是否符合 SLA
- **準確性**: 業務邏輯是否正確執行
- **容量**: 資源使用是否接近上限

**2. 監控層次:**
- **USE 方法** (基礎設施): Utilization, Saturation, Errors
- **RED 方法** (應用服務): Rate, Errors, Duration
- **業務指標**: 業務 KPI 和成本

**3. 告警原則:**
- **可操作性**: 告警必須需要人工干預
- **簡潔性**: 避免告警疲勞
- **優先級**: Critical > Warning > Info
- **根因分析**: 告警包含上下文信息

### 15.8.2 監控 Checklist

**✅ 應用監控:**
- [ ] HTTP 請求指標 (RPS, 延遲, 錯誤率)
- [ ] 自定義業務指標
- [ ] 分佈式追蹤
- [ ] 健康檢查端點
- [ ] 依賴服務監控

**✅ 基礎設施監控:**
- [ ] Node 資源使用 (CPU, Memory, Disk, Network)
- [ ] Pod 狀態和資源使用
- [ ] PVC 使用率
- [ ] Network I/O 和延遲

**✅ 中間件監控:**
- [ ] Database 連接池和查詢性能
- [ ] Redis 緩存命中率和延遲
- [ ] RabbitMQ 隊列長度和消費速率

**✅ 外部依賴監控:**
- [ ] OpenAI API 可用性和延遲
- [ ] Azure Services 可用性

**✅ 可視化:**
- [ ] Grafana Dashboard 配置
- [ ] 關鍵指標實時顯示
- [ ] 歷史趨勢分析

**✅ 告警:**
- [ ] 告警規則配置
- [ ] 告警路由配置
- [ ] 告警靜默規則
- [ ] 告警通知渠道

---

## 總結

本文檔提供了 AI Workflow Platform 的完整監控系統設計，涵蓋:

1. **監控架構**: 四層監控架構 (應用、收集、存儲、可視化)
2. **Prometheus**: 指標收集和存儲，自定義指標暴露
3. **Grafana**: 可視化 Dashboard，多維度監控視圖
4. **APM**: Jaeger 分佈式追蹤，性能分析
5. **基礎設施**: Node Exporter, Kube State Metrics
6. **業務指標**: Agent/執行/成本監控
7. **健康檢查**: Liveness/Readiness/Startup 探針
8. **最佳實踐**: 監控原則和 Checklist

**關鍵指標覆蓋:**
- **應用**: RPS, P95 延遲, 錯誤率, CPU/Memory
- **數據庫**: 連接數, 查詢時間, 緩存命中率
- **業務**: Agent 創建數, 執行次數, OpenAI Token, 成本

**相關文檔:**
- 16-LOGGING.md - 日誌系統設計
- 17-ALERTING.md - 告警系統設計
- 11-DEPLOYMENT-ARCHITECTURE.md - 部署架構

---

**版本歷史:**

| 版本 | 日期 | 作者 | 變更說明 |
|------|------|------|----------|
| 1.0.0 | 2025-01-15 | AI Workflow Team | 初始版本 |
