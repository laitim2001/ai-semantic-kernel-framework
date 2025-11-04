# 成本監控與告警

## 概述

### 文檔目的
本文檔提供 AI Agent 工作流平台的完整成本監控與告警體系,涵蓋:
- **Azure Cost Management 配置** (成本分析, 預算, 告警)
- **Prometheus 成本指標收集** (自定義成本指標)
- **Grafana 成本儀表板** (實時成本可視化)
- **告警規則設計** (預算超支, 異常檢測, 資源浪費)
- **成本歸因分析** (按團隊, 項目, 環境分攤成本)
- **自動化成本報告** (每日/每週/每月成本報告)
- **成本異常響應** (Runbook, 自動化處理)
- **FinOps 儀表板** (高層管理視圖)

### 監控目標

**成本可見性目標**:
- ✅ **實時成本跟踪**: 成本數據延遲 ≤ 4 小時
- ✅ **預算遵守率**: 實際成本在預算 ±10% 範圍內
- ✅ **異常檢測**: 成本異常檢測時間 ≤ 24 小時
- ✅ **成本歸因準確性**: ≥ 95% 成本正確歸因到團隊/項目

**告警響應目標**:
- 預算超支告警: 達到 80%, 90%, 100% 觸發
- 異常成本告警: 超過歷史平均 50% 觸發
- 資源浪費告警: 閒置資源 > 7 天觸發
- 告警響應時間: ≤ 4 小時 (工作時間)

---

## Azure Cost Management 配置

### 預算設置

```bash
#!/bin/bash
# scripts/setup-azure-budgets.sh

set -e

SUBSCRIPTION_ID=$(az account show --query id -o tsv)
RESOURCE_GROUP="aiagent-prod-rg"

echo "=== 配置 Azure 成本預算 ==="

# 預算 1: 生產環境月度總預算
az consumption budget create \
  --budget-name "Production-Monthly-Budget" \
  --amount 15000 \
  --time-grain Monthly \
  --start-date "$(date -u +%Y-%m-01)" \
  --end-date "2026-12-31" \
  --resource-group "$RESOURCE_GROUP" \
  --subscription "$SUBSCRIPTION_ID" \
  --category Cost

echo "✅ 生產環境月度預算已設置: $15,000"

# 預算 2: AI 服務專項預算
az consumption budget create \
  --budget-name "AI-Services-Monthly-Budget" \
  --amount 7000 \
  --time-grain Monthly \
  --start-date "$(date -u +%Y-%m-01)" \
  --end-date "2026-12-31" \
  --resource-group "$RESOURCE_GROUP" \
  --subscription "$SUBSCRIPTION_ID" \
  --category Cost \
  --filter '{
    "tags": {
      "name": "Service",
      "operator": "In",
      "values": ["AzureOpenAI", "CognitiveServices"]
    }
  }'

echo "✅ AI 服務月度預算已設置: $7,000"

# 預算 3: 開發測試環境預算
az consumption budget create \
  --budget-name "DevTest-Monthly-Budget" \
  --amount 2000 \
  --time-grain Monthly \
  --start-date "$(date -u +%Y-%m-01)" \
  --end-date "2026-12-31" \
  --resource-group "aiagent-dev-rg" \
  --subscription "$SUBSCRIPTION_ID" \
  --category Cost

echo "✅ 開發測試環境月度預算已設置: $2,000"

echo ""
echo "=== 查看所有預算 ==="
az consumption budget list \
  --subscription "$SUBSCRIPTION_ID" \
  -o table
```

### 預算告警配置

```json
{
  "budgetName": "Production-Monthly-Budget",
  "notifications": {
    "Warning-80percent": {
      "enabled": true,
      "operator": "GreaterThan",
      "threshold": 80,
      "contactEmails": [
        "finops@company.com",
        "devops-lead@company.com"
      ],
      "contactRoles": [
        "Contributor",
        "Owner"
      ],
      "contactGroups": [
        "/subscriptions/{subscription-id}/resourceGroups/aiagent-prod-rg/providers/microsoft.insights/actionGroups/CostAlerts"
      ],
      "thresholdType": "Actual"
    },
    "Critical-90percent": {
      "enabled": true,
      "operator": "GreaterThan",
      "threshold": 90,
      "contactEmails": [
        "finops@company.com",
        "devops-lead@company.com",
        "cto@company.com"
      ],
      "contactRoles": [
        "Contributor",
        "Owner"
      ],
      "contactGroups": [
        "/subscriptions/{subscription-id}/resourceGroups/aiagent-prod-rg/providers/microsoft.insights/actionGroups/CostAlerts"
      ],
      "thresholdType": "Actual"
    },
    "Exceeded-100percent": {
      "enabled": true,
      "operator": "GreaterThan",
      "threshold": 100,
      "contactEmails": [
        "finops@company.com",
        "devops-lead@company.com",
        "cto@company.com",
        "cfo@company.com"
      ],
      "contactRoles": [
        "Contributor",
        "Owner"
      ],
      "contactGroups": [
        "/subscriptions/{subscription-id}/resourceGroups/aiagent-prod-rg/providers/microsoft.insights/actionGroups/CostAlerts"
      ],
      "thresholdType": "Actual"
    },
    "Forecasted-100percent": {
      "enabled": true,
      "operator": "GreaterThan",
      "threshold": 100,
      "contactEmails": [
        "finops@company.com",
        "devops-lead@company.com"
      ],
      "contactRoles": [
        "Contributor"
      ],
      "thresholdType": "Forecasted"
    }
  }
}
```

**告警閾值說明**:
- **80% 警告**: FinOps 團隊和 DevOps Lead 收到通知,開始審查成本
- **90% 嚴重**: 加上 CTO 收到通知,啟動成本優化措施
- **100% 超支**: 加上 CFO 收到通知,緊急成本控制
- **100% 預測**: 提前預警,基於歷史消費趨勢預測月底超支

---

## Prometheus 成本指標收集

### 自定義成本指標

```csharp
// src/Infrastructure/Metrics/CostMetricsCollector.cs
using Prometheus;
using Azure.ResourceManager;
using Azure.ResourceManager.CostManagement;

public sealed class CostMetricsCollector
{
    // Prometheus 指標定義
    private static readonly Gauge DailyCost = Metrics.CreateGauge(
        "azure_daily_cost_usd",
        "Azure daily cost in USD",
        new GaugeConfiguration
        {
            LabelNames = new[] { "resource_group", "service_name", "environment" }
        });

    private static readonly Gauge MonthToDateCost = Metrics.CreateGauge(
        "azure_month_to_date_cost_usd",
        "Azure month-to-date cost in USD",
        new GaugeConfiguration
        {
            LabelNames = new[] { "resource_group", "environment" }
        });

    private static readonly Gauge BudgetUsagePercentage = Metrics.CreateGauge(
        "azure_budget_usage_percentage",
        "Percentage of budget used",
        new GaugeConfiguration
        {
            LabelNames = new[] { "budget_name" }
        });

    private static readonly Gauge CostPerUser = Metrics.CreateGauge(
        "cost_per_user_usd",
        "Cost per active user in USD",
        new GaugeConfiguration
        {
            LabelNames = new[] { "environment" }
        });

    private static readonly Counter CostAnomaliesDetected = Metrics.CreateCounter(
        "cost_anomalies_detected_total",
        "Total number of cost anomalies detected",
        new CounterConfiguration
        {
            LabelNames = new[] { "resource_group", "severity" }
        });

    private readonly ArmClient _armClient;
    private readonly ILogger<CostMetricsCollector> _logger;

    public CostMetricsCollector(ArmClient armClient, ILogger<CostMetricsCollector> logger)
    {
        _armClient = armClient;
        _logger = logger;
    }

    public async Task CollectCostMetricsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("開始收集成本指標");

        try
        {
            // 收集每日成本
            await CollectDailyCostsAsync(cancellationToken);

            // 收集月度成本
            await CollectMonthToDateCostsAsync(cancellationToken);

            // 收集預算使用率
            await CollectBudgetUsageAsync(cancellationToken);

            // 計算單位用戶成本
            await CalculateCostPerUserAsync(cancellationToken);

            _logger.LogInformation("成本指標收集完成");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "成本指標收集失敗");
        }
    }

    private async Task CollectDailyCostsAsync(CancellationToken cancellationToken)
    {
        // 查詢過去 24 小時的成本
        var yesterday = DateTimeOffset.UtcNow.AddDays(-1).Date;
        var today = DateTimeOffset.UtcNow.Date;

        // 模擬成本查詢 (實際應使用 Azure Cost Management API)
        var costs = new Dictionary<string, decimal>
        {
            ["aiagent-prod-rg/AKS"] = 120.50m,
            ["aiagent-prod-rg/PostgreSQL"] = 35.20m,
            ["aiagent-prod-rg/Redis"] = 26.60m,
            ["aiagent-prod-rg/BlobStorage"] = 1.85m,
            ["aiagent-prod-rg/AzureOpenAI"] = 285.40m
        };

        foreach (var (resource, cost) in costs)
        {
            var parts = resource.Split('/');
            var resourceGroup = parts[0];
            var serviceName = parts[1];

            DailyCost.WithLabels(resourceGroup, serviceName, "production").Set((double)cost);
        }
    }

    private async Task CollectMonthToDateCostsAsync(CancellationToken cancellationToken)
    {
        // 查詢本月累計成本
        var monthStart = new DateTimeOffset(DateTimeOffset.UtcNow.Year, DateTimeOffset.UtcNow.Month, 1, 0, 0, 0, TimeSpan.Zero);
        var today = DateTimeOffset.UtcNow;

        // 模擬月度成本查詢
        var monthlyC costs = new Dictionary<string, decimal>
        {
            ["aiagent-prod-rg"] = 8540.30m,
            ["aiagent-dev-rg"] = 1250.80m
        };

        foreach (var (resourceGroup, cost) in monthlyCosts)
        {
            var environment = resourceGroup.Contains("prod") ? "production" : "development";
            MonthToDateCost.WithLabels(resourceGroup, environment).Set((double)cost);
        }
    }

    private async Task CollectBudgetUsageAsync(CancellationToken cancellationToken)
    {
        // 查詢預算使用率
        var budgets = new Dictionary<string, (decimal used, decimal total)>
        {
            ["Production-Monthly-Budget"] = (8540.30m, 15000m),
            ["AI-Services-Monthly-Budget"] = (6200.50m, 7000m),
            ["DevTest-Monthly-Budget"] = (1250.80m, 2000m)
        };

        foreach (var (budgetName, (used, total)) in budgets)
        {
            var usagePercentage = (double)((used / total) * 100);
            BudgetUsagePercentage.WithLabels(budgetName).Set(usagePercentage);

            // 檢測預算超支風險
            if (usagePercentage > 90)
            {
                _logger.LogWarning("預算 {BudgetName} 使用率已達 {Percentage}%", budgetName, usagePercentage);
            }
        }
    }

    private async Task CalculateCostPerUserAsync(CancellationToken cancellationToken)
    {
        // 查詢活躍用戶數
        var activeUsers = 10000;  // 從數據庫或分析服務查詢

        // 計算單位用戶成本
        var productionCost = 8540.30m;
        var costPerUser = productionCost / activeUsers;

        CostPerUser.WithLabels("production").Set((double)costPerUser);

        _logger.LogInformation("單位用戶成本: ${CostPerUser:F4}", costPerUser);
    }
}
```

### Prometheus 抓取配置

```yaml
# prometheus/prometheus.yml
global:
  scrape_interval: 15s
  evaluation_interval: 15s

scrape_configs:
  - job_name: 'cost-metrics'
    scrape_interval: 5m  # 每 5 分鐘抓取一次成本指標
    static_configs:
      - targets:
          - 'cost-metrics-exporter:8080'
    relabel_configs:
      - source_labels: [__address__]
        target_label: instance
        replacement: 'cost-metrics-exporter'

  - job_name: 'azure-monitor-exporter'
    scrape_interval: 5m
    static_configs:
      - targets:
          - 'azure-monitor-exporter:9090'
    metric_relabel_configs:
      # 僅保留成本相關指標
      - source_labels: [__name__]
        regex: 'azure_(cost|budget|usage).*'
        action: keep
```

### 定時成本指標收集 CronJob

```yaml
# k8s/cost-metrics-collector-cronjob.yaml
apiVersion: batch/v1
kind: CronJob
metadata:
  name: cost-metrics-collector
  namespace: kube-system
spec:
  schedule: "*/30 * * * *"  # 每 30 分鐘執行一次
  jobTemplate:
    spec:
      template:
        spec:
          serviceAccountName: cost-metrics-collector
          containers:
          - name: collector
            image: aiagent-acr.azurecr.io/cost-metrics-collector:latest
            env:
            - name: AZURE_SUBSCRIPTION_ID
              valueFrom:
                secretKeyRef:
                  name: azure-credentials
                  key: subscription-id
            - name: AZURE_CLIENT_ID
              valueFrom:
                secretKeyRef:
                  name: azure-credentials
                  key: client-id
            - name: AZURE_CLIENT_SECRET
              valueFrom:
                secretKeyRef:
                  name: azure-credentials
                  key: client-secret
            - name: AZURE_TENANT_ID
              valueFrom:
                secretKeyRef:
                  name: azure-credentials
                  key: tenant-id
            resources:
              requests:
                memory: "128Mi"
                cpu: "100m"
              limits:
                memory: "256Mi"
                cpu: "200m"
          restartPolicy: OnFailure
```

---

## Grafana 成本儀表板

### 主成本儀表板配置

```json
{
  "dashboard": {
    "title": "AI Agent Platform - Cost Dashboard",
    "uid": "cost-dashboard",
    "tags": ["cost", "finops"],
    "timezone": "browser",
    "panels": [
      {
        "id": 1,
        "title": "月度成本趨勢",
        "type": "graph",
        "targets": [
          {
            "expr": "azure_month_to_date_cost_usd{environment=\"production\"}",
            "legendFormat": "生產環境月度成本"
          },
          {
            "expr": "azure_month_to_date_cost_usd{environment=\"development\"}",
            "legendFormat": "開發環境月度成本"
          }
        ],
        "yaxes": [
          {
            "label": "成本 (USD)",
            "format": "currencyUSD"
          }
        ],
        "gridPos": {
          "x": 0,
          "y": 0,
          "w": 12,
          "h": 8
        }
      },
      {
        "id": 2,
        "title": "預算使用率",
        "type": "gauge",
        "targets": [
          {
            "expr": "azure_budget_usage_percentage{budget_name=\"Production-Monthly-Budget\"}",
            "legendFormat": "生產環境預算"
          }
        ],
        "fieldConfig": {
          "defaults": {
            "min": 0,
            "max": 100,
            "unit": "percent",
            "thresholds": {
              "steps": [
                { "value": 0, "color": "green" },
                { "value": 80, "color": "yellow" },
                { "value": 90, "color": "orange" },
                { "value": 100, "color": "red" }
              ]
            }
          }
        },
        "gridPos": {
          "x": 12,
          "y": 0,
          "w": 6,
          "h": 8
        }
      },
      {
        "id": 3,
        "title": "每日成本分解 (按服務)",
        "type": "piechart",
        "targets": [
          {
            "expr": "azure_daily_cost_usd{resource_group=\"aiagent-prod-rg\"}",
            "legendFormat": "{{service_name}}"
          }
        ],
        "gridPos": {
          "x": 18,
          "y": 0,
          "w": 6,
          "h": 8
        }
      },
      {
        "id": 4,
        "title": "單位用戶成本趨勢",
        "type": "graph",
        "targets": [
          {
            "expr": "cost_per_user_usd{environment=\"production\"}",
            "legendFormat": "單位用戶成本"
          }
        ],
        "yaxes": [
          {
            "label": "成本 (USD/用戶)",
            "format": "currencyUSD"
          }
        ],
        "alert": {
          "name": "單位用戶成本過高",
          "conditions": [
            {
              "evaluator": {
                "params": [0.60],
                "type": "gt"
              },
              "query": {
                "params": ["A", "5m", "now"]
              }
            }
          ],
          "message": "單位用戶成本超過 $0.60,需要檢查成本效率"
        },
        "gridPos": {
          "x": 0,
          "y": 8,
          "w": 12,
          "h": 8
        }
      },
      {
        "id": 5,
        "title": "成本異常檢測",
        "type": "table",
        "targets": [
          {
            "expr": "rate(cost_anomalies_detected_total[24h])",
            "format": "table"
          }
        ],
        "gridPos": {
          "x": 12,
          "y": 8,
          "w": 12,
          "h": 8
        }
      },
      {
        "id": 6,
        "title": "Top 10 成本資源",
        "type": "table",
        "targets": [
          {
            "expr": "topk(10, azure_daily_cost_usd)",
            "format": "table",
            "instant": true
          }
        ],
        "transformations": [
          {
            "id": "organize",
            "options": {
              "excludeByName": {
                "Time": true
              },
              "indexByName": {
                "resource_group": 0,
                "service_name": 1,
                "Value": 2
              },
              "renameByName": {
                "resource_group": "資源組",
                "service_name": "服務名稱",
                "Value": "每日成本 (USD)"
              }
            }
          }
        ],
        "gridPos": {
          "x": 0,
          "y": 16,
          "w": 24,
          "h": 8
        }
      }
    ]
  }
}
```

---

## 告警規則設計

### Prometheus 告警規則

```yaml
# prometheus/rules/cost-alerts.yml
groups:
  - name: cost_alerts
    interval: 5m
    rules:
      # 預算超支告警
      - alert: BudgetUsageHigh
        expr: azure_budget_usage_percentage > 80
        for: 10m
        labels:
          severity: warning
          category: cost
        annotations:
          summary: "預算使用率高 ({{ $labels.budget_name }})"
          description: "預算 {{ $labels.budget_name }} 使用率已達 {{ $value | humanize }}%"

      - alert: BudgetUsageCritical
        expr: azure_budget_usage_percentage > 90
        for: 5m
        labels:
          severity: critical
          category: cost
        annotations:
          summary: "預算使用率嚴重 ({{ $labels.budget_name }})"
          description: "預算 {{ $labels.budget_name }} 使用率已達 {{ $value | humanize }}%,接近超支"

      - alert: BudgetExceeded
        expr: azure_budget_usage_percentage > 100
        for: 1m
        labels:
          severity: critical
          category: cost
        annotations:
          summary: "預算已超支 ({{ $labels.budget_name }})"
          description: "預算 {{ $labels.budget_name }} 已超支,使用率 {{ $value | humanize }}%"

      # 成本異常告警
      - alert: DailyCostAnomaly
        expr: |
          (azure_daily_cost_usd - avg_over_time(azure_daily_cost_usd[7d]))
          / avg_over_time(azure_daily_cost_usd[7d]) > 0.50
        for: 2h
        labels:
          severity: warning
          category: cost
        annotations:
          summary: "每日成本異常 ({{ $labels.resource_group }}/{{ $labels.service_name }})"
          description: "成本比 7 天平均值高 50% 以上"

      # 單位用戶成本過高
      - alert: CostPerUserHigh
        expr: cost_per_user_usd{environment="production"} > 0.60
        for: 30m
        labels:
          severity: warning
          category: cost
        annotations:
          summary: "單位用戶成本過高"
          description: "生產環境單位用戶成本為 ${{ $value | humanize }},超過目標 $0.50"

      # 閒置資源告警
      - alert: IdleResourcesDetected
        expr: |
          (
            avg_over_time(node_cpu_seconds_total{mode="idle"}[24h])
            / avg_over_time(node_cpu_seconds_total[24h])
          ) > 0.90
        for: 7d
        labels:
          severity: info
          category: cost
        annotations:
          summary: "檢測到閒置資源 ({{ $labels.instance }})"
          description: "節點 {{ $labels.instance }} CPU 閒置率 > 90% 持續 7 天"

      # AI 服務成本激增
      - alert: AIServiceCostSpike
        expr: |
          (
            azure_daily_cost_usd{service_name="AzureOpenAI"}
            - azure_daily_cost_usd{service_name="AzureOpenAI"} offset 1d
          ) > 500
        for: 1h
        labels:
          severity: warning
          category: cost
        annotations:
          summary: "AI 服務成本激增"
          description: "Azure OpenAI 每日成本比昨天增加 ${{ $value | humanize }}"
```

### AlertManager 配置

```yaml
# alertmanager/alertmanager.yml
global:
  resolve_timeout: 5m

route:
  receiver: 'default-receiver'
  group_by: ['category', 'severity']
  group_wait: 30s
  group_interval: 5m
  repeat_interval: 4h

  routes:
    # 成本告警路由
    - match:
        category: cost
      receiver: 'cost-alerts'
      group_by: ['budget_name', 'resource_group']
      routes:
        - match:
            severity: critical
          receiver: 'cost-alerts-critical'
          repeat_interval: 1h

receivers:
  - name: 'default-receiver'
    email_configs:
      - to: 'devops@company.com'

  - name: 'cost-alerts'
    email_configs:
      - to: 'finops@company.com'
        headers:
          Subject: '⚠️  成本告警: {{ .GroupLabels.budget_name }}'
    slack_configs:
      - api_url: 'https://hooks.slack.com/services/YOUR/SLACK/WEBHOOK'
        channel: '#cost-alerts'
        title: '{{ .GroupLabels.severity | toUpper }}: {{ .CommonAnnotations.summary }}'
        text: '{{ .CommonAnnotations.description }}'

  - name: 'cost-alerts-critical'
    email_configs:
      - to: 'finops@company.com,cto@company.com'
        headers:
          Subject: '🚨 嚴重成本告警: {{ .GroupLabels.budget_name }}'
    slack_configs:
      - api_url: 'https://hooks.slack.com/services/YOUR/SLACK/WEBHOOK'
        channel: '#cost-alerts-critical'
        title: '🚨 CRITICAL: {{ .CommonAnnotations.summary }}'
        text: '{{ .CommonAnnotations.description }}'
    pagerduty_configs:
      - service_key: 'YOUR_PAGERDUTY_SERVICE_KEY'
        description: '{{ .CommonAnnotations.summary }}'
```

---

## 成本歸因分析

### 資源標記策略

```bash
#!/bin/bash
# scripts/apply-cost-allocation-tags.sh

set -e

RESOURCE_GROUP="aiagent-prod-rg"

echo "=== 應用成本分配標記 ==="

# 標記 AKS 集群
az aks update \
  --resource-group "$RESOURCE_GROUP" \
  --name "aiagent-aks-prod" \
  --tags \
    Environment="Production" \
    Team="Platform" \
    CostCenter="Engineering" \
    Project="AIAgent" \
    Owner="devops-lead@company.com"

# 標記 PostgreSQL
az postgres flexible-server update \
  --resource-group "$RESOURCE_GROUP" \
  --name "aiagent-postgres-prod" \
  --tags \
    Environment="Production" \
    Team="Platform" \
    CostCenter="Engineering" \
    Project="AIAgent" \
    Service="Database"

# 標記 Redis
az redis update \
  --resource-group "$RESOURCE_GROUP" \
  --name "aiagent-redis-prod" \
  --tags \
    Environment="Production" \
    Team="Platform" \
    CostCenter="Engineering" \
    Project="AIAgent" \
    Service="Cache"

# 標記 Azure OpenAI
az cognitiveservices account update \
  --resource-group "$RESOURCE_GROUP" \
  --name "aiagent-openai-prod" \
  --tags \
    Environment="Production" \
    Team="DataScience" \
    CostCenter="R&D" \
    Project="AIAgent" \
    Service="AI"

echo "✅ 成本分配標記已應用"
```

### 成本歸因報告生成器

```python
# scripts/cost_attribution_report.py
"""
成本歸因報告生成器 - 按團隊、項目、環境分攤成本
"""
from typing import Dict, List
from dataclasses import dataclass
from collections import defaultdict

@dataclass
class CostAllocation:
    """成本分配"""
    dimension: str  # Team, Project, Environment, CostCenter
    value: str
    cost: float
    percentage: float

class CostAttributionReportGenerator:
    """成本歸因報告生成器"""

    def __init__(self, total_cost: float, cost_by_tags: Dict[str, Dict[str, float]]):
        self.total_cost = total_cost
        self.cost_by_tags = cost_by_tags

    def generate_team_allocation(self) -> List[CostAllocation]:
        """生成團隊成本分配"""
        team_costs = self.cost_by_tags.get("Team", {})
        allocations = []

        for team, cost in team_costs.items():
            percentage = (cost / self.total_cost) * 100
            allocations.append(CostAllocation(
                dimension="Team",
                value=team,
                cost=cost,
                percentage=percentage
            ))

        return sorted(allocations, key=lambda x: x.cost, reverse=True)

    def generate_project_allocation(self) -> List[CostAllocation]:
        """生成項目成本分配"""
        project_costs = self.cost_by_tags.get("Project", {})
        allocations = []

        for project, cost in project_costs.items():
            percentage = (cost / self.total_cost) * 100
            allocations.append(CostAllocation(
                dimension="Project",
                value=project,
                cost=cost,
                percentage=percentage
            ))

        return sorted(allocations, key=lambda x: x.cost, reverse=True)

    def generate_environment_allocation(self) -> List[CostAllocation]:
        """生成環境成本分配"""
        env_costs = self.cost_by_tags.get("Environment", {})
        allocations = []

        for env, cost in env_costs.items():
            percentage = (cost / self.total_cost) * 100
            allocations.append(CostAllocation(
                dimension="Environment",
                value=env,
                cost=cost,
                percentage=percentage
            ))

        return sorted(allocations, key=lambda x: x.cost, reverse=True)

    def generate_report(self) -> str:
        """生成完整成本歸因報告"""
        report = f"""
=== 成本歸因報告 ===
總成本: ${self.total_cost:,.2f}

按團隊分配:
{"="*60}
"""

        team_allocations = self.generate_team_allocation()
        for allocation in team_allocations:
            report += f"{allocation.value:<20} ${allocation.cost:>12,.2f}  ({allocation.percentage:>5.1f}%)\n"

        report += f"""
按項目分配:
{"="*60}
"""

        project_allocations = self.generate_project_allocation()
        for allocation in project_allocations:
            report += f"{allocation.value:<20} ${allocation.cost:>12,.2f}  ({allocation.percentage:>5.1f}%)\n"

        report += f"""
按環境分配:
{"="*60}
"""

        env_allocations = self.generate_environment_allocation()
        for allocation in env_allocations:
            report += f"{allocation.value:<20} ${allocation.cost:>12,.2f}  ({allocation.percentage:>5.1f}%)\n"

        return report

# 使用示例
cost_by_tags = {
    "Team": {
        "Platform": 8500.00,
        "DataScience": 3700.00,
        "Operations": 1003.00
    },
    "Project": {
        "AIAgent": 12200.00,
        "Analytics": 800.00,
        "Infrastructure": 203.00
    },
    "Environment": {
        "Production": 11500.00,
        "Development": 1500.00,
        "Staging": 203.00
    }
}

generator = CostAttributionReportGenerator(
    total_cost=13203.00,
    cost_by_tags=cost_by_tags
)

print(generator.generate_report())
```

---

## 自動化成本報告

### 每日成本報告

```python
# scripts/daily_cost_report.py
"""
每日成本報告 - 自動發送到 Slack 和 Email
"""
import requests
from datetime import datetime, timedelta

class DailyCostReporter:
    """每日成本報告器"""

    def __init__(self, slack_webhook_url: str):
        self.slack_webhook_url = slack_webhook_url

    def fetch_daily_costs(self) -> dict:
        """獲取每日成本數據"""
        # 模擬成本數據
        return {
            "date": datetime.utcnow().strftime("%Y-%m-%d"),
            "total_cost": 469.55,
            "by_service": {
                "AzureOpenAI": 285.40,
                "AKS": 120.50,
                "PostgreSQL": 35.20,
                "Redis": 26.60,
                "Other": 1.85
            },
            "vs_yesterday": {
                "change": 15.30,
                "percentage": 3.4
            },
            "month_to_date": 8540.30,
            "budget_remaining": 6459.70,
            "budget_usage_percentage": 56.9
        }

    def generate_slack_message(self, cost_data: dict) -> dict:
        """生成 Slack 消息"""
        change_icon = "📈" if cost_data["vs_yesterday"]["change"] > 0 else "📉"

        return {
            "blocks": [
                {
                    "type": "header",
                    "text": {
                        "type": "plain_text",
                        "text": f"📊 每日成本報告 - {cost_data['date']}"
                    }
                },
                {
                    "type": "section",
                    "fields": [
                        {
                            "type": "mrkdwn",
                            "text": f"*總成本:*\n${cost_data['total_cost']:,.2f}"
                        },
                        {
                            "type": "mrkdwn",
                            "text": f"*與昨日比較:*\n{change_icon} ${cost_data['vs_yesterday']['change']:+,.2f} ({cost_data['vs_yesterday']['percentage']:+.1f}%)"
                        },
                        {
                            "type": "mrkdwn",
                            "text": f"*月度累計:*\n${cost_data['month_to_date']:,.2f}"
                        },
                        {
                            "type": "mrkdwn",
                            "text": f"*預算剩餘:*\n${cost_data['budget_remaining']:,.2f} ({100 - cost_data['budget_usage_percentage']:.1f}%)"
                        }
                    ]
                },
                {
                    "type": "section",
                    "text": {
                        "type": "mrkdwn",
                        "text": "*成本分解:*"
                    }
                },
                {
                    "type": "section",
                    "fields": [
                        {
                            "type": "mrkdwn",
                            "text": f"• Azure OpenAI: ${cost_data['by_service']['AzureOpenAI']:,.2f}"
                        },
                        {
                            "type": "mrkdwn",
                            "text": f"• AKS: ${cost_data['by_service']['AKS']:,.2f}"
                        },
                        {
                            "type": "mrkdwn",
                            "text": f"• PostgreSQL: ${cost_data['by_service']['PostgreSQL']:,.2f}"
                        },
                        {
                            "type": "mrkdwn",
                            "text": f"• Redis: ${cost_data['by_service']['Redis']:,.2f}"
                        }
                    ]
                },
                {
                    "type": "divider"
                },
                {
                    "type": "context",
                    "elements": [
                        {
                            "type": "mrkdwn",
                            "text": f"查看詳細儀表板: <https://grafana.company.com/d/cost-dashboard|Cost Dashboard>"
                        }
                    ]
                }
            ]
        }

    def send_report(self):
        """發送每日報告"""
        cost_data = self.fetch_daily_costs()
        message = self.generate_slack_message(cost_data)

        response = requests.post(
            self.slack_webhook_url,
            json=message,
            headers={"Content-Type": "application/json"}
        )

        if response.status_code == 200:
            print(f"✅ 每日成本報告已發送")
        else:
            print(f"❌ 報告發送失敗: {response.status_code}")

# 使用示例 (通過 Kubernetes CronJob 每天執行)
if __name__ == "__main__":
    reporter = DailyCostReporter(slack_webhook_url="https://hooks.slack.com/services/YOUR/WEBHOOK/URL")
    reporter.send_report()
```

---

## 成本異常響應 Runbook

### 預算超支響應流程

```markdown
# Runbook: 預算超支響應

## 觸發條件
- 預算使用率 > 90%
- 預測月底超支 > 10%

## 緊急響應 (1-4 小時內)

### Step 1: 評估超支原因
```bash
# 查看成本明細
az consumption usage list \
  --start-date "$(date -u +%Y-%m-01)" \
  --end-date "$(date -u +%Y-%m-%d)" \
  --query '[].{Service:meterCategory, Cost:pretaxCost}' \
  -o table

# 識別成本激增服務
az cost-analysis query \
  --type "Usage" \
  --timeframe "MonthToDate" \
  --dataset-grouping name="ServiceName" type="Dimension"
```

### Step 2: 立即成本控制措施
- **高成本服務**:
  - Azure OpenAI: 檢查異常高 token 使用,啟用速率限制
  - AKS: 縮減非關鍵節點池,延遲非緊急擴展
  - PostgreSQL: 檢查長時間運行查詢,優化查詢性能

- **開發測試環境**:
  - 暫停開發集群 (非工作時間)
  - 清理未使用的測試資源
  - 延遲非緊急測試任務

### Step 3: 通知利益相關者
```bash
# 發送緊急通知
./scripts/send-cost-alert-notification.sh \
  --severity "critical" \
  --budget "Production-Monthly-Budget" \
  --usage "92%" \
  --recipients "finops@company.com,cto@company.com"
```

### Step 4: 執行臨時優化
```bash
# 縮減開發環境
az aks scale \
  --resource-group aiagent-dev-rg \
  --name aiagent-aks-dev \
  --node-count 1 \
  --nodepool-name default

# 降級非生產數據庫
az postgres flexible-server update \
  --resource-group aiagent-dev-rg \
  --name aiagent-postgres-dev \
  --sku-name Standard_D2ds_v4

# 清理舊備份
./scripts/cleanup-old-backups.sh --days 60
```

## 中期優化 (1-3 天內)
- 購買預留實例降低長期成本
- 實施更激進的自動擴展策略
- 優化 AI 服務 token 使用
- 審查並調整預算分配

## 長期改進 (1-2 週內)
- 完善成本監控告警
- 加強成本教育和培訓
- 建立成本審查機制
- 優化資源架構
```

---

## 檢查清單

### 成本監控與告警檢查清單

- [ ] **Azure Cost Management**
  - [ ] 所有環境預算已設置
  - [ ] 預算告警閾值已配置 (80%, 90%, 100%)
  - [ ] 預算通知接收人已更新

- [ ] **Prometheus 指標收集**
  - [ ] 成本指標 Exporter 已部署
  - [ ] 定時收集 CronJob 已配置 (每 30 分鐘)
  - [ ] 成本指標正確導入 Prometheus

- [ ] **Grafana 儀表板**
  - [ ] 主成本儀表板已創建
  - [ ] 儀表板權限已配置
  - [ ] 成本告警已集成到儀表板

- [ ] **告警規則**
  - [ ] Prometheus 告警規則已部署
  - [ ] AlertManager 配置已更新
  - [ ] 告警通知渠道已測試 (Email, Slack, PagerDuty)

- [ ] **成本歸因**
  - [ ] 所有資源已標記 (Team, Project, Environment, CostCenter)
  - [ ] 成本歸因報告自動生成
  - [ ] 團隊成本分攤機制已建立

- [ ] **自動化報告**
  - [ ] 每日成本報告已配置
  - [ ] 每週成本摘要已配置
  - [ ] 每月成本審查會議已安排

- [ ] **成本異常響應**
  - [ ] 預算超支 Runbook 已編寫
  - [ ] 響應團隊已培訓
  - [ ] 成本控制措施已測試

---

## 總結

本文檔提供了完整的成本監控與告警體系,涵蓋:

✅ **Azure Cost Management 配置** (預算設置, 多層級告警)
✅ **Prometheus 成本指標收集** (自定義指標, 定時採集)
✅ **Grafana 成本儀表板** (實時可視化, 多維度分析)
✅ **告警規則設計** (預算超支, 成本異常, 資源浪費)
✅ **成本歸因分析** (按團隊/項目/環境分攤)
✅ **自動化成本報告** (每日/每週/每月報告)
✅ **成本異常響應** (Runbook, 緊急措施)

**關鍵指標**:
- 成本數據延遲: **≤ 4 小時**
- 預算遵守率: **±10%**
- 異常檢測時間: **≤ 24 小時**
- 成本歸因準確性: **≥ 95%**

透過本指南,FinOps 團隊可以建立完善的成本監控體系,實現成本可見性和可控性。
