# Prometheus & Grafana 監控系統設置指南

## 概述

### 文檔目的
本文檔提供 AI Agent 工作流平台的完整 Prometheus & Grafana 監控系統實施方案,涵蓋:
- **Prometheus 部署與配置** (Kubernetes Operator, 服務發現, 數據抓取)
- **Grafana 儀表板設計** (預定義 Dashboard, 自定義 Panel, 數據可視化)
- **指標收集標準** (Application Metrics, Infrastructure Metrics, Business Metrics)
- **長期存儲策略** (Thanos, Victoria Metrics, Azure Monitor 集成)
- **高可用性部署** (Prometheus HA, Grafana Clustering)
- **告警規則配置** (Alertmanager, 告警路由, 通知渠道)

### 技術背景
Prometheus 是雲原生監控的事實標準,與 Grafana 結合提供強大的可觀測性:
- **Prometheus**: 時序數據庫 + 數據抓取引擎,專為動態雲環境設計
- **Grafana**: 數據可視化平台,支持多種數據源,豐富的圖表類型
- **Pull-Based Model**: Prometheus 主動抓取目標指標,無需應用主動推送
- **Service Discovery**: 自動發現 Kubernetes Pod/Service,無需手動配置
- **PromQL**: 強大的查詢語言,支持聚合、過濾、數學運算

**Prometheus 核心概念**:
- **Metric**: 指標名稱 + 標籤 (Labels) + 時序數據
- **Target**: 被監控的端點 (HTTP `/metrics` endpoint)
- **Scrape**: Prometheus 定期抓取 Target 的指標數據
- **Recording Rule**: 預計算複雜查詢,提升查詢性能
- **Alerting Rule**: 基於 PromQL 的告警條件

---

## 系統架構

### 整體架構圖

```
┌─────────────────────────────────────────────────────────────────┐
│ Kubernetes Cluster (AKS)                                        │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ Prometheus Operator (kube-prometheus-stack)             │   │
│  │ ├─ Prometheus Server (HA: 2 replicas)                   │   │
│  │ │  ├─ Service Discovery (Kubernetes API)                │   │
│  │ │  ├─ Scrape Configs (ServiceMonitor, PodMonitor)       │   │
│  │ │  ├─ Recording Rules                                   │   │
│  │ │  └─ Alerting Rules                                    │   │
│  │ │                                                        │   │
│  │ ├─ Alertmanager (HA: 3 replicas)                        │   │
│  │ │  ├─ Alert Routing                                     │   │
│  │ │  ├─ Silencing & Inhibition                            │   │
│  │ │  └─ Notification Integrations (Slack, Email, PagerDuty)│  │
│  │ │                                                        │   │
│  │ ├─ Grafana (HA: 2 replicas)                             │   │
│  │ │  ├─ Pre-configured Dashboards                         │   │
│  │ │  ├─ Data Source: Prometheus + Azure Monitor           │   │
│  │ │  └─ User Management & RBAC                            │   │
│  │ │                                                        │   │
│  │ └─ Node Exporter (DaemonSet)                            │   │
│  │    └─ Host Metrics (CPU, Memory, Disk, Network)         │   │
│  └─────────────────────────────────────────────────────────┘   │
│                                                                 │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ Application Pods                                         │   │
│  │ ├─ API Service (expose /metrics on :8080/metrics)       │   │
│  │ ├─ Worker Service (expose /metrics on :8080/metrics)    │   │
│  │ └─ Web Service (expose /metrics on :8080/metrics)       │   │
│  └─────────────────────────────────────────────────────────┘   │
│                                                                 │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ ServiceMonitor (Auto-discovery)                          │   │
│  │ - Selector: app=api                                      │   │
│  │ - Endpoint: /metrics, port: metrics                      │   │
│  │ - Scrape Interval: 15s                                   │   │
│  └─────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
                           │
                           │ Remote Write (Long-term Storage)
                           ▼
┌─────────────────────────────────────────────────────────────────┐
│ Thanos / Victoria Metrics (Long-term Storage)                   │
│ - Retention: 1 year                                             │
│ - Downsampling: 5m resolution for data > 30 days               │
│ - Object Storage: Azure Blob Storage                            │
└─────────────────────────────────────────────────────────────────┘
                           │
                           │ Query
                           ▼
┌─────────────────────────────────────────────────────────────────┐
│ Grafana Dashboards                                              │
│ - Real-time Metrics (Prometheus: 15 days retention)            │
│ - Historical Metrics (Thanos: 1 year retention)                │
└─────────────────────────────────────────────────────────────────┘
```

---

## Prometheus 部署

### 使用 kube-prometheus-stack Helm Chart

**添加 Helm Repository**:

```bash
# 添加 Prometheus Community Helm Repo
helm repo add prometheus-community https://prometheus-community.github.io/helm-charts
helm repo update

# 創建 Namespace
kubectl create namespace monitoring
```

**自定義 Values 文件**: `prometheus-values.yaml`

```yaml
# prometheus-values.yaml
fullnameOverride: prometheus

# Prometheus Server Configuration
prometheus:
  prometheusSpec:
    # Retention Period (15 days for recent data)
    retention: 15d
    retentionSize: "50GB"

    # Resource Limits
    resources:
      requests:
        memory: 2Gi
        cpu: 500m
      limits:
        memory: 4Gi
        cpu: 1000m

    # Storage Configuration (Persistent Volume)
    storageSpec:
      volumeClaimTemplate:
        spec:
          storageClassName: managed-premium  # Azure Premium SSD
          accessModes: ["ReadWriteOnce"]
          resources:
            requests:
              storage: 100Gi

    # High Availability (2 replicas)
    replicas: 2

    # Service Discovery Configuration
    serviceMonitorSelectorNilUsesHelmValues: false  # Discover all ServiceMonitors
    podMonitorSelectorNilUsesHelmValues: false      # Discover all PodMonitors

    # External Labels (for federation/remote write)
    externalLabels:
      cluster: "aks-aiagent-prod"
      environment: "production"
      region: "eastus"

    # Remote Write to Thanos/Victoria Metrics
    remoteWrite:
      - url: "http://thanos-receive.monitoring.svc.cluster.local:19291/api/v1/receive"
        queueConfig:
          capacity: 10000
          maxShards: 50
          minShards: 1
          maxSamplesPerSend: 5000
          batchSendDeadline: 5s

    # Additional Scrape Configs
    additionalScrapeConfigs:
      - job_name: 'azure-postgres-exporter'
        static_configs:
          - targets: ['postgres-exporter.monitoring.svc.cluster.local:9187']
      - job_name: 'azure-redis-exporter'
        static_configs:
          - targets: ['redis-exporter.monitoring.svc.cluster.local:9121']

# Alertmanager Configuration
alertmanager:
  alertmanagerSpec:
    # High Availability (3 replicas for quorum)
    replicas: 3

    # Resource Limits
    resources:
      requests:
        memory: 256Mi
        cpu: 100m
      limits:
        memory: 512Mi
        cpu: 200m

    # Storage
    storage:
      volumeClaimTemplate:
        spec:
          storageClassName: managed-premium
          accessModes: ["ReadWriteOnce"]
          resources:
            requests:
              storage: 10Gi

  # Alertmanager Configuration File
  config:
    global:
      resolve_timeout: 5m
      slack_api_url: 'https://hooks.slack.com/services/YOUR/SLACK/WEBHOOK'

    route:
      group_by: ['alertname', 'cluster', 'service']
      group_wait: 10s
      group_interval: 10s
      repeat_interval: 12h
      receiver: 'default'
      routes:
        # Critical Alerts -> PagerDuty + Slack
        - match:
            severity: critical
          receiver: 'pagerduty-critical'
          continue: true
        - match:
            severity: critical
          receiver: 'slack-critical'

        # Warning Alerts -> Slack Only
        - match:
            severity: warning
          receiver: 'slack-warnings'

    receivers:
      - name: 'default'
        slack_configs:
          - channel: '#monitoring-alerts'
            title: '{{ .GroupLabels.alertname }}'
            text: '{{ range .Alerts }}{{ .Annotations.description }}{{ end }}'

      - name: 'slack-critical'
        slack_configs:
          - channel: '#alerts-critical'
            title: '🚨 CRITICAL: {{ .GroupLabels.alertname }}'
            text: '{{ range .Alerts }}{{ .Annotations.description }}{{ end }}'
            send_resolved: true

      - name: 'slack-warnings'
        slack_configs:
          - channel: '#alerts-warnings'
            title: '⚠️ WARNING: {{ .GroupLabels.alertname }}'
            text: '{{ range .Alerts }}{{ .Annotations.description }}{{ end }}'

      - name: 'pagerduty-critical'
        pagerduty_configs:
          - service_key: 'YOUR_PAGERDUTY_SERVICE_KEY'
            description: '{{ .GroupLabels.alertname }}: {{ .CommonAnnotations.summary }}'

# Grafana Configuration
grafana:
  enabled: true
  adminPassword: "admin-change-me"  # 從 Secret 讀取

  # High Availability
  replicas: 2

  # Persistence
  persistence:
    enabled: true
    storageClassName: managed-premium
    size: 10Gi

  # Resource Limits
  resources:
    requests:
      memory: 256Mi
      cpu: 100m
    limits:
      memory: 512Mi
      cpu: 200m

  # Ingress Configuration
  ingress:
    enabled: true
    ingressClassName: nginx
    annotations:
      cert-manager.io/cluster-issuer: "letsencrypt-prod"
    hosts:
      - grafana.aiagent.example.com
    tls:
      - secretName: grafana-tls
        hosts:
          - grafana.aiagent.example.com

  # Data Sources
  datasources:
    datasources.yaml:
      apiVersion: 1
      datasources:
        - name: Prometheus
          type: prometheus
          url: http://prometheus-kube-prometheus-prometheus.monitoring.svc.cluster.local:9090
          access: proxy
          isDefault: true
          editable: false

        - name: Thanos
          type: prometheus
          url: http://thanos-query.monitoring.svc.cluster.local:9090
          access: proxy
          isDefault: false
          editable: false

        - name: Azure Monitor
          type: grafana-azure-monitor-datasource
          access: proxy
          jsonData:
            subscriptionId: "YOUR_AZURE_SUBSCRIPTION_ID"
            tenantId: "YOUR_AZURE_TENANT_ID"
            clientId: "YOUR_AZURE_CLIENT_ID"
          secureJsonData:
            clientSecret: "YOUR_AZURE_CLIENT_SECRET"

  # Pre-configured Dashboards
  dashboardProviders:
    dashboardproviders.yaml:
      apiVersion: 1
      providers:
        - name: 'default'
          orgId: 1
          folder: ''
          type: file
          disableDeletion: false
          editable: true
          options:
            path: /var/lib/grafana/dashboards/default

  dashboards:
    default:
      # Kubernetes Cluster Monitoring
      kubernetes-cluster:
        gnetId: 7249  # Kubernetes Cluster (Prometheus)
        revision: 1
        datasource: Prometheus

      # Node Exporter Full
      node-exporter:
        gnetId: 1860  # Node Exporter Full
        revision: 27
        datasource: Prometheus

      # ASP.NET Core Monitoring
      aspnet-core:
        gnetId: 10915  # ASP.NET Core
        revision: 1
        datasource: Prometheus

      # PostgreSQL Database
      postgresql:
        gnetId: 9628  # PostgreSQL Database
        revision: 7
        datasource: Prometheus

      # Redis
      redis:
        gnetId: 11835  # Redis Dashboard
        revision: 1
        datasource: Prometheus

# Node Exporter (Host Metrics)
nodeExporter:
  enabled: true

# Kube State Metrics (Kubernetes Object Metrics)
kubeStateMetrics:
  enabled: true

# Prometheus Operator
prometheusOperator:
  resources:
    requests:
      memory: 128Mi
      cpu: 50m
    limits:
      memory: 256Mi
      cpu: 100m
```

**部署 kube-prometheus-stack**:

```bash
# 安裝 Prometheus Stack
helm install prometheus prometheus-community/kube-prometheus-stack \
  --namespace monitoring \
  --values prometheus-values.yaml \
  --version 55.5.0

# 查看部署狀態
kubectl get pods -n monitoring

# 查看 Services
kubectl get svc -n monitoring

# Port Forward 訪問 Prometheus UI (開發/調試)
kubectl port-forward -n monitoring svc/prometheus-kube-prometheus-prometheus 9090:9090

# Port Forward 訪問 Grafana UI
kubectl port-forward -n monitoring svc/prometheus-grafana 3000:80
```

---

## 應用程序指標暴露

### ASP.NET Core Prometheus 集成

**安裝 NuGet 套件**:

```bash
dotnet add package prometheus-net.AspNetCore
```

**配置 Prometheus Middleware**: `Program.cs`

```csharp
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// ... other services

var app = builder.Build();

// Prometheus Metrics Endpoint
app.UseMetricServer();  // 暴露 /metrics endpoint on default port 80

// HTTP Metrics Middleware (自動收集 HTTP 請求指標)
app.UseHttpMetrics(options =>
{
    options.AddCustomLabel("environment", builder.Environment.EnvironmentName);
    options.AddCustomLabel("version", "1.0.0");
});

app.MapControllers();
app.Run();
```

### 自定義業務指標

```csharp
// src/Application/Metrics/ApplicationMetrics.cs
using Prometheus;

namespace AIAgentPlatform.Application.Metrics;

public static class ApplicationMetrics
{
    // Counter: 單調遞增計數器 (如請求總數、錯誤總數)
    public static readonly Counter WorkflowExecutionsTotal = Metrics
        .CreateCounter(
            "aiagent_workflow_executions_total",
            "Total number of workflow executions",
            new CounterConfiguration
            {
                LabelNames = new[] { "workflow_type", "status" }
            });

    public static readonly Counter AgentInvocationsTotal = Metrics
        .CreateCounter(
            "aiagent_agent_invocations_total",
            "Total number of agent invocations",
            new CounterConfiguration
            {
                LabelNames = new[] { "agent_name", "status" }
            });

    // Gauge: 可增可減的數值 (如當前活躍用戶數、隊列長度)
    public static readonly Gauge ActiveWorkflowsGauge = Metrics
        .CreateGauge(
            "aiagent_active_workflows",
            "Number of currently active workflows");

    public static readonly Gauge QueueDepthGauge = Metrics
        .CreateGauge(
            "aiagent_queue_depth",
            "Current depth of the job queue",
            new GaugeConfiguration
            {
                LabelNames = new[] { "queue_name" }
            });

    // Histogram: 分布統計 (如請求延遲、請求大小)
    public static readonly Histogram WorkflowExecutionDuration = Metrics
        .CreateHistogram(
            "aiagent_workflow_execution_duration_seconds",
            "Workflow execution duration in seconds",
            new HistogramConfiguration
            {
                LabelNames = new[] { "workflow_type" },
                Buckets = new[] { 0.1, 0.5, 1, 2.5, 5, 10, 30, 60, 120 }
            });

    public static readonly Histogram AzureOpenAIRequestDuration = Metrics
        .CreateHistogram(
            "aiagent_azure_openai_request_duration_seconds",
            "Azure OpenAI API request duration in seconds",
            new HistogramConfiguration
            {
                LabelNames = new[] { "model", "operation" },
                Buckets = Histogram.ExponentialBuckets(0.01, 2, 10)  // 10ms to 10s
            });

    // Summary: 百分位統計 (如 P50, P95, P99 延遲)
    public static readonly Summary WorkflowExecutionSummary = Metrics
        .CreateSummary(
            "aiagent_workflow_execution_summary_seconds",
            "Workflow execution duration summary",
            new SummaryConfiguration
            {
                LabelNames = new[] { "workflow_type" },
                Objectives = new[]
                {
                    new QuantileEpsilonPair(0.5, 0.05),   // P50 ± 5%
                    new QuantileEpsilonPair(0.9, 0.01),   // P90 ± 1%
                    new QuantileEpsilonPair(0.95, 0.01),  // P95 ± 1%
                    new QuantileEpsilonPair(0.99, 0.001)  // P99 ± 0.1%
                }
            });
}
```

**使用自定義指標**:

```csharp
// src/Application/Services/WorkflowExecutionService.cs
public sealed class WorkflowExecutionService : IWorkflowExecutionService
{
    private readonly ILogger<WorkflowExecutionService> _logger;

    public async Task<WorkflowResult> ExecuteWorkflowAsync(
        Guid workflowId,
        string workflowType,
        CancellationToken cancellationToken)
    {
        // 增加活躍工作流計數
        ApplicationMetrics.ActiveWorkflowsGauge.Inc();

        // 記錄執行時間
        using (ApplicationMetrics.WorkflowExecutionDuration
            .WithLabels(workflowType)
            .NewTimer())
        {
            try
            {
                var result = await ExecuteInternalAsync(workflowId, cancellationToken);

                // 記錄成功執行
                ApplicationMetrics.WorkflowExecutionsTotal
                    .WithLabels(workflowType, "success")
                    .Inc();

                return result;
            }
            catch (Exception ex)
            {
                // 記錄失敗執行
                ApplicationMetrics.WorkflowExecutionsTotal
                    .WithLabels(workflowType, "failure")
                    .Inc();

                _logger.LogError(ex, "Workflow execution failed: {WorkflowId}", workflowId);
                throw;
            }
            finally
            {
                // 減少活躍工作流計數
                ApplicationMetrics.ActiveWorkflowsGauge.Dec();
            }
        }
    }

    private async Task<WorkflowResult> ExecuteInternalAsync(
        Guid workflowId,
        CancellationToken cancellationToken)
    {
        // 工作流執行邏輯...
        await Task.Delay(1000, cancellationToken);
        return new WorkflowResult { Success = true };
    }
}
```

---

## ServiceMonitor 配置

### 為應用程序創建 ServiceMonitor

```yaml
# k8s/base/servicemonitor.yaml
apiVersion: monitoring.coreos.com/v1
kind: ServiceMonitor
metadata:
  name: api-service-monitor
  namespace: aiagent-prod
  labels:
    app: api
    release: prometheus  # 必須與 Prometheus Operator 的 release 標籤匹配
spec:
  selector:
    matchLabels:
      app: api
  endpoints:
    - port: metrics  # Service 中定義的 port 名稱
      path: /metrics
      interval: 15s
      scrapeTimeout: 10s
      relabelings:
        # 添加自定義標籤
        - sourceLabels: [__meta_kubernetes_pod_name]
          targetLabel: pod
        - sourceLabels: [__meta_kubernetes_pod_node_name]
          targetLabel: node
        - sourceLabels: [__meta_kubernetes_namespace]
          targetLabel: namespace
```

**對應的 Service 配置**:

```yaml
# k8s/base/api-service.yaml
apiVersion: v1
kind: Service
metadata:
  name: api-service
  namespace: aiagent-prod
  labels:
    app: api
spec:
  selector:
    app: api
  ports:
    - name: http
      port: 80
      targetPort: 8080
    - name: metrics  # Prometheus 抓取端口
      port: 8080
      targetPort: 8080
  type: ClusterIP
```

### PostgreSQL Exporter

```yaml
# k8s/base/postgres-exporter.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: postgres-exporter
  namespace: monitoring
spec:
  replicas: 1
  selector:
    matchLabels:
      app: postgres-exporter
  template:
    metadata:
      labels:
        app: postgres-exporter
    spec:
      containers:
      - name: postgres-exporter
        image: prometheuscommunity/postgres-exporter:v0.15.0
        ports:
        - containerPort: 9187
          name: metrics
        env:
        - name: DATA_SOURCE_NAME
          valueFrom:
            secretKeyRef:
              name: postgres-exporter-secret
              key: data-source-name
        resources:
          requests:
            memory: 64Mi
            cpu: 50m
          limits:
            memory: 128Mi
            cpu: 100m
---
apiVersion: v1
kind: Service
metadata:
  name: postgres-exporter
  namespace: monitoring
  labels:
    app: postgres-exporter
spec:
  selector:
    app: postgres-exporter
  ports:
    - name: metrics
      port: 9187
      targetPort: 9187
---
apiVersion: monitoring.coreos.com/v1
kind: ServiceMonitor
metadata:
  name: postgres-exporter
  namespace: monitoring
  labels:
    release: prometheus
spec:
  selector:
    matchLabels:
      app: postgres-exporter
  endpoints:
    - port: metrics
      interval: 30s
```

**創建 Secret**:

```bash
# PostgreSQL Connection String
kubectl create secret generic postgres-exporter-secret \
  --from-literal=data-source-name="postgresql://username:password@postgres-aiagent-prod.postgres.database.azure.com:5432/aiagent_prod?sslmode=require" \
  --namespace monitoring
```

---

## Grafana 儀表板設計

### 自定義應用程序 Dashboard

```json
{
  "dashboard": {
    "title": "AI Agent Platform - Application Metrics",
    "tags": ["aiagent", "application"],
    "timezone": "browser",
    "panels": [
      {
        "id": 1,
        "title": "Workflow Executions (Rate)",
        "type": "graph",
        "targets": [
          {
            "expr": "rate(aiagent_workflow_executions_total[5m])",
            "legendFormat": "{{workflow_type}} - {{status}}"
          }
        ],
        "gridPos": {"h": 8, "w": 12, "x": 0, "y": 0}
      },
      {
        "id": 2,
        "title": "Active Workflows",
        "type": "stat",
        "targets": [
          {
            "expr": "aiagent_active_workflows"
          }
        ],
        "gridPos": {"h": 4, "w": 6, "x": 12, "y": 0}
      },
      {
        "id": 3,
        "title": "Workflow Execution Duration (P95)",
        "type": "graph",
        "targets": [
          {
            "expr": "histogram_quantile(0.95, rate(aiagent_workflow_execution_duration_seconds_bucket[5m]))",
            "legendFormat": "{{workflow_type}} - P95"
          }
        ],
        "gridPos": {"h": 8, "w": 12, "x": 0, "y": 8}
      },
      {
        "id": 4,
        "title": "Azure OpenAI Request Duration (Heatmap)",
        "type": "heatmap",
        "targets": [
          {
            "expr": "rate(aiagent_azure_openai_request_duration_seconds_bucket[5m])",
            "format": "heatmap"
          }
        ],
        "gridPos": {"h": 8, "w": 12, "x": 12, "y": 8}
      },
      {
        "id": 5,
        "title": "HTTP Request Rate",
        "type": "graph",
        "targets": [
          {
            "expr": "rate(http_requests_received_total[5m])",
            "legendFormat": "{{method}} {{endpoint}}"
          }
        ],
        "gridPos": {"h": 8, "w": 12, "x": 0, "y": 16}
      },
      {
        "id": 6,
        "title": "HTTP Request Duration (P50, P95, P99)",
        "type": "graph",
        "targets": [
          {
            "expr": "histogram_quantile(0.50, rate(http_request_duration_seconds_bucket[5m]))",
            "legendFormat": "P50"
          },
          {
            "expr": "histogram_quantile(0.95, rate(http_request_duration_seconds_bucket[5m]))",
            "legendFormat": "P95"
          },
          {
            "expr": "histogram_quantile(0.99, rate(http_request_duration_seconds_bucket[5m]))",
            "legendFormat": "P99"
          }
        ],
        "gridPos": {"h": 8, "w": 12, "x": 12, "y": 16}
      },
      {
        "id": 7,
        "title": "Error Rate (4xx, 5xx)",
        "type": "graph",
        "targets": [
          {
            "expr": "rate(http_requests_received_total{code=~\"4..\"}[5m])",
            "legendFormat": "4xx Errors"
          },
          {
            "expr": "rate(http_requests_received_total{code=~\"5..\"}[5m])",
            "legendFormat": "5xx Errors"
          }
        ],
        "gridPos": {"h": 8, "w": 12, "x": 0, "y": 24}
      },
      {
        "id": 8,
        "title": "Queue Depth",
        "type": "graph",
        "targets": [
          {
            "expr": "aiagent_queue_depth",
            "legendFormat": "{{queue_name}}"
          }
        ],
        "gridPos": {"h": 8, "w": 12, "x": 12, "y": 24}
      }
    ]
  }
}
```

### 使用 ConfigMap 部署 Dashboard

```yaml
# k8s/base/grafana-dashboard-configmap.yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: grafana-dashboard-aiagent
  namespace: monitoring
  labels:
    grafana_dashboard: "1"
data:
  aiagent-dashboard.json: |
    {
      "dashboard": {
        "title": "AI Agent Platform - Application Metrics",
        ...
      }
    }
```

---

## 告警規則配置

### PrometheusRule 定義

```yaml
# k8s/base/prometheus-rules.yaml
apiVersion: monitoring.coreos.com/v1
kind: PrometheusRule
metadata:
  name: aiagent-alerts
  namespace: monitoring
  labels:
    release: prometheus
spec:
  groups:
    # Application-Level Alerts
    - name: aiagent.application
      interval: 30s
      rules:
        # High Error Rate
        - alert: HighErrorRate
          expr: |
            rate(http_requests_received_total{code=~"5.."}[5m]) > 0.05
          for: 5m
          labels:
            severity: critical
            component: api
          annotations:
            summary: "High 5xx error rate detected"
            description: "Error rate is {{ $value | humanizePercentage }} (threshold: 5%)"

        # Slow HTTP Requests (P95 > 2 seconds)
        - alert: SlowHTTPRequests
          expr: |
            histogram_quantile(0.95, rate(http_request_duration_seconds_bucket[5m])) > 2
          for: 10m
          labels:
            severity: warning
            component: api
          annotations:
            summary: "HTTP requests are slow (P95 > 2s)"
            description: "P95 latency is {{ $value }}s"

        # Workflow Execution Failures
        - alert: WorkflowExecutionFailures
          expr: |
            rate(aiagent_workflow_executions_total{status="failure"}[10m]) > 0.1
          for: 5m
          labels:
            severity: warning
            component: workflow-engine
          annotations:
            summary: "High workflow execution failure rate"
            description: "Failure rate is {{ $value | humanizePercentage }}"

        # Queue Depth Too High
        - alert: HighQueueDepth
          expr: |
            aiagent_queue_depth > 1000
          for: 10m
          labels:
            severity: warning
            component: background-jobs
          annotations:
            summary: "Job queue depth is too high"
            description: "Queue {{$labels.queue_name}} has {{$value}} pending jobs"

    # Infrastructure-Level Alerts
    - name: aiagent.infrastructure
      interval: 30s
      rules:
        # High CPU Usage
        - alert: HighCPUUsage
          expr: |
            100 - (avg by (instance) (irate(node_cpu_seconds_total{mode="idle"}[5m])) * 100) > 80
          for: 10m
          labels:
            severity: warning
            component: infrastructure
          annotations:
            summary: "High CPU usage on {{$labels.instance}}"
            description: "CPU usage is {{ $value | humanize }}%"

        # High Memory Usage
        - alert: HighMemoryUsage
          expr: |
            (1 - (node_memory_MemAvailable_bytes / node_memory_MemTotal_bytes)) * 100 > 85
          for: 10m
          labels:
            severity: warning
            component: infrastructure
          annotations:
            summary: "High memory usage on {{$labels.instance}}"
            description: "Memory usage is {{ $value | humanize }}%"

        # Disk Space Low
        - alert: DiskSpaceLow
          expr: |
            (node_filesystem_avail_bytes{fstype!="tmpfs"} / node_filesystem_size_bytes{fstype!="tmpfs"}) * 100 < 15
          for: 10m
          labels:
            severity: critical
            component: infrastructure
          annotations:
            summary: "Low disk space on {{$labels.instance}}"
            description: "Only {{ $value | humanize }}% disk space remaining on {{$labels.mountpoint}}"

        # Pod Crash Looping
        - alert: PodCrashLooping
          expr: |
            rate(kube_pod_container_status_restarts_total[15m]) > 0
          for: 5m
          labels:
            severity: critical
            component: kubernetes
          annotations:
            summary: "Pod {{$labels.namespace}}/{{$labels.pod}} is crash looping"
            description: "Pod has restarted {{ $value }} times in the last 15 minutes"

        # Deployment Replica Mismatch
        - alert: DeploymentReplicaMismatch
          expr: |
            kube_deployment_spec_replicas != kube_deployment_status_replicas_available
          for: 10m
          labels:
            severity: warning
            component: kubernetes
          annotations:
            summary: "Deployment {{$labels.namespace}}/{{$labels.deployment}} has replica mismatch"
            description: "Desired: {{$labels.spec_replicas}}, Available: {{$labels.status_replicas_available}}"

    # Database Alerts
    - name: aiagent.database
      interval: 30s
      rules:
        # High Database Connection Usage
        - alert: HighDatabaseConnections
          expr: |
            pg_stat_database_numbackends / pg_settings_max_connections * 100 > 80
          for: 5m
          labels:
            severity: warning
            component: postgresql
          annotations:
            summary: "High database connection usage"
            description: "Database connection usage is {{ $value | humanize }}%"

        # Slow Queries
        - alert: SlowDatabaseQueries
          expr: |
            rate(pg_stat_statements_mean_exec_time_seconds[5m]) > 1
          for: 10m
          labels:
            severity: warning
            component: postgresql
          annotations:
            summary: "Slow database queries detected"
            description: "Average query execution time is {{ $value }}s"

        # Replication Lag
        - alert: PostgreSQLReplicationLag
          expr: |
            pg_replication_lag_seconds > 60
          for: 5m
          labels:
            severity: critical
            component: postgresql
          annotations:
            summary: "PostgreSQL replication lag is high"
            description: "Replication lag is {{ $value }}s"
```

---

## 長期存儲 (Thanos)

### Thanos 架構

```yaml
# k8s/base/thanos-storage-secret.yaml
apiVersion: v1
kind: Secret
metadata:
  name: thanos-objstore-config
  namespace: monitoring
type: Opaque
stringData:
  objstore.yml: |
    type: AZURE
    config:
      storage_account: "staiagentprodmetrics"
      storage_account_key: "YOUR_STORAGE_ACCOUNT_KEY"
      container: "thanos"
```

### Thanos Sidecar (與 Prometheus 一起運行)

```yaml
# 在 prometheus-values.yaml 中添加 Thanos Sidecar
prometheus:
  prometheusSpec:
    thanos:
      image: quay.io/thanos/thanos:v0.32.5
      version: v0.32.5
      objectStorageConfig:
        name: thanos-objstore-config
        key: objstore.yml
```

### Thanos Query (統一查詢接口)

```yaml
# k8s/base/thanos-query.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: thanos-query
  namespace: monitoring
spec:
  replicas: 2
  selector:
    matchLabels:
      app: thanos-query
  template:
    metadata:
      labels:
        app: thanos-query
    spec:
      containers:
      - name: thanos-query
        image: quay.io/thanos/thanos:v0.32.5
        args:
          - query
          - --http-address=0.0.0.0:9090
          - --grpc-address=0.0.0.0:10901
          - --store=dnssrv+_grpc._tcp.prometheus-operated.monitoring.svc.cluster.local
          - --store=dnssrv+_grpc._tcp.thanos-store.monitoring.svc.cluster.local
        ports:
        - containerPort: 9090
          name: http
        - containerPort: 10901
          name: grpc
---
apiVersion: v1
kind: Service
metadata:
  name: thanos-query
  namespace: monitoring
spec:
  selector:
    app: thanos-query
  ports:
    - name: http
      port: 9090
      targetPort: 9090
```

---

## 最佳實踐

### 1. 指標命名規範

```
# 格式: <namespace>_<subsystem>_<name>_<unit>
aiagent_workflow_executions_total        # Counter
aiagent_active_workflows                 # Gauge
aiagent_workflow_execution_duration_seconds  # Histogram
aiagent_queue_depth                      # Gauge

# 添加有意義的標籤
aiagent_workflow_executions_total{workflow_type="data_processing", status="success"}
```

### 2. 告警規則設計原則

- **症狀導向**: 告警基於用戶體驗 (如延遲、錯誤率),而非原因 (如 CPU 使用率)
- **可操作性**: 每個告警都應該有明確的處理步驟
- **降噪**: 使用 `for` 子句避免瞬時抖動觸發告警
- **分級**: Critical (影響用戶) vs Warning (潛在問題)

### 3. 查詢性能優化

```promql
# 不推薦: 高基數標籤查詢
rate(http_requests_total{user_id="123"}[5m])

# 推薦: 聚合後再過濾
sum by (status) (rate(http_requests_total[5m]))

# 不推薦: 多個獨立查詢
rate(metric1[5m])
rate(metric2[5m])

# 推薦: 使用 Recording Rule 預計算
job:metric1:rate5m
job:metric2:rate5m
```

---

## 故障排查

### 常見問題

| 問題 | 原因 | 解決方案 |
|------|------|----------|
| **ServiceMonitor 未被發現** | Label 不匹配 | 檢查 `serviceMonitorSelector` 和 ServiceMonitor 的 labels |
| **指標未抓取** | Target 不健康 | 訪問 Prometheus UI → Targets,檢查錯誤信息 |
| **Grafana Dashboard 無數據** | 數據源配置錯誤 | 檢查 Grafana → Configuration → Data Sources |
| **告警未觸發** | PromQL 語法錯誤 | 在 Prometheus UI → Alerts 中測試表達式 |

### 調試技巧

```bash
# 查看 Prometheus 配置
kubectl exec -n monitoring prometheus-prometheus-kube-prometheus-prometheus-0 -- cat /etc/prometheus/config_out/prometheus.env.yaml

# 查看 Alertmanager 配置
kubectl exec -n monitoring alertmanager-prometheus-kube-prometheus-alertmanager-0 -- cat /etc/alertmanager/config/alertmanager.yaml

# 測試告警路由
amtool config routes --config.file=/etc/alertmanager/config/alertmanager.yaml

# 查看 Prometheus Targets
curl http://localhost:9090/api/v1/targets

# 查詢指標
curl 'http://localhost:9090/api/v1/query?query=up'
```

---

## 總結

本文檔提供了完整的 Prometheus & Grafana 監控系統實施方案,涵蓋:

✅ **Prometheus 高可用部署** (kube-prometheus-stack, HA 配置)
✅ **應用程序指標暴露** (prometheus-net, 自定義業務指標)
✅ **ServiceMonitor 自動發現** (Kubernetes Service Discovery)
✅ **Grafana 儀表板設計** (預定義 Dashboard, 自定義 Panel)
✅ **告警規則配置** (Alertmanager, 多渠道通知)
✅ **長期存儲策略** (Thanos + Azure Blob Storage)

透過本指南,運維團隊可以實現:
- **全面可觀測性**: 應用程序 + 基礎設施 + 業務指標
- **實時告警**: 基於症狀的智能告警,降低噪音
- **長期分析**: 1 年歷史數據存儲,趨勢分析
- **高可用性**: Prometheus HA, Grafana Clustering
