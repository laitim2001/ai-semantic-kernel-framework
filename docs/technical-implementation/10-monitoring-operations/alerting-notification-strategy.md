# 告警通知策略實施指南

## 概述

### 文檔目的
本文檔提供 AI Agent 工作流平台的完整告警通知策略實施方案,涵蓋:
- **告警規則設計原則** (SLO-Based Alerting, Symptom-Based vs Cause-Based)
- **告警路由與分組** (Alertmanager Configuration, Alert Routing Trees)
- **通知渠道集成** (Slack, Email, PagerDuty, Microsoft Teams, Webhook)
- **告警降噪策略** (Silencing, Inhibition, Grouping, Throttling)
- **On-Call 輪班管理** (PagerDuty Schedules, Escalation Policies)
- **告警疲勞防治** (Alert Fatigue Prevention, SLO Budget Management)

### 技術背景
有效的告警策略是 SRE (Site Reliability Engineering) 的核心實踐:
- **告警目的**: 通知人員需要立即採取行動的問題,而非所有異常
- **症狀 vs 原因**: 告警應基於用戶體驗 (症狀),而非系統內部狀態 (原因)
- **可操作性**: 每個告警都應該有明確的處理步驟 (Runbook)
- **降噪**: 避免告警風暴,減少告警疲勞

**Google SRE 告警哲學**:
1. **每個告警都應該是可操作的** (Actionable)
2. **每個告警都應該需要人工介入** (Require Human Action)
3. **告警應該是緊急的** (Urgent)
4. **告警應該是新信息** (Novel)

**SLO-Based Alerting**:
- **Service Level Objective (SLO)**: 服務質量目標 (如 99.9% 可用性)
- **Error Budget**: 允許的錯誤預算 (100% - SLO = 0.1%)
- **Burn Rate**: 錯誤預算消耗速率
- **Multi-Window Alerting**: 短期 + 長期窗口結合,減少誤報

---

## 告警架構

### 整體告警流

```
┌─────────────────────────────────────────────────────────────────┐
│ Metrics Sources                                                 │
├─────────────────────────────────────────────────────────────────┤
│ ├─ Prometheus (Application + Infrastructure Metrics)           │
│ ├─ Azure Monitor (Azure Resources Metrics)                     │
│ ├─ Application Insights (APM, User Experience)                 │
│ └─ Custom Exporters (PostgreSQL, Redis, Workflow Engine)       │
└──────────────────────┬──────────────────────────────────────────┘
                       │
                       │ Evaluation (every 30s)
                       ▼
┌─────────────────────────────────────────────────────────────────┐
│ Prometheus Alerting Rules (PrometheusRule CRDs)                │
├─────────────────────────────────────────────────────────────────┤
│ ├─ SLO-Based Alerts (Error Budget Burn Rate)                   │
│ ├─ Symptom-Based Alerts (High Latency, High Error Rate)        │
│ ├─ Infrastructure Alerts (CPU, Memory, Disk)                   │
│ └─ Business Alerts (Workflow Failures, Queue Depth)            │
└──────────────────────┬──────────────────────────────────────────┘
                       │
                       │ Firing Alerts
                       ▼
┌─────────────────────────────────────────────────────────────────┐
│ Alertmanager (HA: 3 replicas)                                  │
├─────────────────────────────────────────────────────────────────┤
│ ├─ Grouping (by alertname, cluster, service)                   │
│ ├─ Inhibition (suppress lower severity alerts)                 │
│ ├─ Silencing (manual/scheduled maintenance)                    │
│ ├─ Throttling (rate limiting notifications)                    │
│ └─ Routing (based on severity, team, service)                  │
└──────────────────────┬──────────────────────────────────────────┘
                       │
                       │ Routed Notifications
       ┌───────────────┼───────────────┬───────────────┐
       │               │               │               │
┌──────▼──────┐ ┌──────▼──────┐ ┌─────▼──────┐ ┌─────▼──────┐
│ Slack       │ │ PagerDuty   │ │ Email      │ │ Webhook    │
│ (Warnings)  │ │ (Critical)  │ │ (All)      │ │ (Custom)   │
└─────────────┘ └─────────────┘ └────────────┘ └────────────┘
       │               │               │               │
       └───────────────┴───────────────┴───────────────┘
                       │
                       ▼
┌─────────────────────────────────────────────────────────────────┐
│ Incident Management                                             │
│ ├─ Acknowledge & Assign (via PagerDuty)                        │
│ ├─ Runbook Execution (documented procedures)                   │
│ ├─ Incident Communication (Status Page, Slack)                 │
│ └─ Post-Mortem (RCA, Action Items)                             │
└─────────────────────────────────────────────────────────────────┘
```

---

## SLO-Based 告警規則

### SLO 定義

```yaml
# SLO Targets for AI Agent Platform
slos:
  # API Availability SLO: 99.9% (3 nines)
  api_availability:
    target: 0.999
    error_budget: 0.001  # 0.1%
    measurement_window: 30d

  # API Latency SLO: 95% of requests < 500ms
  api_latency_p95:
    target: 0.95
    threshold_ms: 500
    measurement_window: 30d

  # Workflow Success Rate SLO: 99.5%
  workflow_success_rate:
    target: 0.995
    error_budget: 0.005  # 0.5%
    measurement_window: 30d
```

### Multi-Window Burn Rate Alerts

```yaml
# k8s/base/prometheus-slo-rules.yaml
apiVersion: monitoring.coreos.com/v1
kind: PrometheusRule
metadata:
  name: slo-alerts
  namespace: monitoring
  labels:
    release: prometheus
spec:
  groups:
    - name: slo.api_availability
      interval: 30s
      rules:
        # Recording Rules: 計算 Error Rate
        - record: job:api:request_errors:rate5m
          expr: |
            sum(rate(http_requests_total{job="api",code=~"5.."}[5m]))
            /
            sum(rate(http_requests_total{job="api"}[5m]))

        - record: job:api:request_errors:rate1h
          expr: |
            sum(rate(http_requests_total{job="api",code=~"5.."}[1h]))
            /
            sum(rate(http_requests_total{job="api"}[1h]))

        - record: job:api:request_errors:rate6h
          expr: |
            sum(rate(http_requests_total{job="api",code=~"5.."}[6h]))
            /
            sum(rate(http_requests_total{job="api"}[6h]))

        # Alerting Rules: Multi-Window Burn Rate
        # Fast Burn (14.4x): 5% budget consumed in 1 hour → page immediately
        - alert: APIErrorBudgetFastBurn
          expr: |
            (
              job:api:request_errors:rate5m > (14.4 * 0.001)
              and
              job:api:request_errors:rate1h > (14.4 * 0.001)
            )
          for: 2m
          labels:
            severity: critical
            slo: api_availability
            team: platform
          annotations:
            summary: "API error budget is burning too fast"
            description: |
              Current error rate is {{ $value | humanizePercentage }}.
              At this rate, we will exhaust our monthly error budget in 2 days.
              SLO: 99.9% availability (0.1% error budget)
            runbook_url: "https://wiki.example.com/runbooks/api-error-budget-burn"

        # Slow Burn (6x): 5% budget consumed in 6 hours → ticket
        - alert: APIErrorBudgetSlowBurn
          expr: |
            (
              job:api:request_errors:rate6h > (6 * 0.001)
              and
              job:api:request_errors:rate1h > (6 * 0.001)
            )
          for: 15m
          labels:
            severity: warning
            slo: api_availability
            team: platform
          annotations:
            summary: "API error budget is burning slowly"
            description: |
              Current error rate is {{ $value | humanizePercentage }}.
              At this rate, we will exhaust our monthly error budget in 5 days.
            runbook_url: "https://wiki.example.com/runbooks/api-error-budget-burn"

    - name: slo.api_latency
      interval: 30s
      rules:
        # Recording Rule: P95 Latency
        - record: job:api:request_duration:p95
          expr: |
            histogram_quantile(0.95,
              sum(rate(http_request_duration_seconds_bucket{job="api"}[5m])) by (le)
            )

        # Alerting Rule: Latency SLO Violation
        - alert: APILatencySLOViolation
          expr: |
            job:api:request_duration:p95 > 0.5
          for: 5m
          labels:
            severity: warning
            slo: api_latency_p95
            team: platform
          annotations:
            summary: "API latency SLO violation (P95 > 500ms)"
            description: |
              Current P95 latency is {{ $value }}s (target: 0.5s).
              95% of requests should complete within 500ms.
            runbook_url: "https://wiki.example.com/runbooks/api-high-latency"

    - name: slo.workflow_success
      interval: 30s
      rules:
        # Recording Rule: Workflow Success Rate
        - record: job:workflow:success_rate:rate5m
          expr: |
            sum(rate(aiagent_workflow_executions_total{status="success"}[5m]))
            /
            sum(rate(aiagent_workflow_executions_total[5m]))

        # Alerting Rule: Workflow Success Rate SLO Violation
        - alert: WorkflowSuccessRateSLOViolation
          expr: |
            job:workflow:success_rate:rate5m < 0.995
          for: 10m
          labels:
            severity: critical
            slo: workflow_success_rate
            team: workflow-engine
          annotations:
            summary: "Workflow success rate below SLO (< 99.5%)"
            description: |
              Current success rate is {{ $value | humanizePercentage }} (target: 99.5%).
              Error budget: 0.5% (allowing ~360 failures per month for 1M workflows).
            runbook_url: "https://wiki.example.com/runbooks/workflow-failures"
```

---

## 症狀導向告警規則

### 用戶體驗告警

```yaml
# k8s/base/prometheus-symptom-rules.yaml
apiVersion: monitoring.coreos.com/v1
kind: PrometheusRule
metadata:
  name: symptom-alerts
  namespace: monitoring
spec:
  groups:
    # User-Facing Symptoms (High Priority)
    - name: symptoms.user_facing
      interval: 30s
      rules:
        # High 5xx Error Rate (Users seeing errors)
        - alert: HighErrorRate
          expr: |
            sum(rate(http_requests_total{code=~"5.."}[5m])) by (service)
            /
            sum(rate(http_requests_total[5m])) by (service)
            > 0.05
          for: 3m
          labels:
            severity: critical
            category: symptom
            impact: user_facing
          annotations:
            summary: "High 5xx error rate on {{$labels.service}}"
            description: |
              Error rate is {{ $value | humanizePercentage }} (threshold: 5%).
              Users are experiencing service errors.
            runbook_url: "https://wiki.example.com/runbooks/high-error-rate"

        # High HTTP Latency (Users experiencing slow responses)
        - alert: HighHTTPLatency
          expr: |
            histogram_quantile(0.95,
              sum(rate(http_request_duration_seconds_bucket[5m])) by (le, service)
            ) > 2.0
          for: 10m
          labels:
            severity: warning
            category: symptom
            impact: user_facing
          annotations:
            summary: "High HTTP latency on {{$labels.service}}"
            description: |
              P95 latency is {{ $value }}s (threshold: 2s).
              Users are experiencing slow page loads.
            runbook_url: "https://wiki.example.com/runbooks/high-latency"

        # API Downtime (Service completely unavailable)
        - alert: APIDown
          expr: |
            up{job="api"} == 0
          for: 1m
          labels:
            severity: critical
            category: symptom
            impact: service_down
          annotations:
            summary: "API service is down"
            description: |
              API service {{$labels.instance}} has been down for more than 1 minute.
              All users are affected.
            runbook_url: "https://wiki.example.com/runbooks/service-down"

    # Business Logic Symptoms
    - name: symptoms.business_logic
      interval: 30s
      rules:
        # Workflow Execution Failures
        - alert: HighWorkflowFailureRate
          expr: |
            sum(rate(aiagent_workflow_executions_total{status="failure"}[10m]))
            /
            sum(rate(aiagent_workflow_executions_total[10m]))
            > 0.1
          for: 5m
          labels:
            severity: warning
            category: symptom
            impact: business_logic
          annotations:
            summary: "High workflow failure rate"
            description: |
              Workflow failure rate is {{ $value | humanizePercentage }} (threshold: 10%).
              Workflows are failing at an abnormal rate.

        # Queue Depth Growing (Processing lag)
        - alert: HighQueueDepth
          expr: |
            aiagent_queue_depth > 1000
          for: 10m
          labels:
            severity: warning
            category: symptom
            impact: processing_lag
          annotations:
            summary: "High queue depth on {{$labels.queue_name}}"
            description: |
              Queue depth is {{ $value }} (threshold: 1000).
              Job processing is lagging behind ingestion rate.

        # Azure OpenAI Rate Limiting
        - alert: AzureOpenAIRateLimited
          expr: |
            sum(rate(aiagent_azure_openai_requests_total{status="429"}[5m])) > 10
          for: 5m
          labels:
            severity: critical
            category: symptom
            impact: external_dependency
          annotations:
            summary: "Azure OpenAI rate limiting detected"
            description: |
              Rate limit errors: {{ $value }} requests/s.
              Workflows dependent on OpenAI are failing.
```

---

## Alertmanager 配置

### 完整配置示例

```yaml
# alertmanager-config.yaml
global:
  resolve_timeout: 5m
  slack_api_url: 'https://hooks.slack.com/services/YOUR/SLACK/WEBHOOK'
  pagerduty_url: 'https://events.pagerduty.com/v2/enqueue'

# 路由樹
route:
  receiver: 'default'
  group_by: ['alertname', 'cluster', 'service']
  group_wait: 10s       # 等待同一組內的其他告警
  group_interval: 10s   # 同一組內新告警的發送間隔
  repeat_interval: 12h  # 重複發送未解決告警的間隔

  routes:
    # Critical Alerts → Page on-call engineer via PagerDuty
    - match:
        severity: critical
      receiver: 'pagerduty-critical'
      group_wait: 10s
      group_interval: 5m
      repeat_interval: 4h
      continue: true  # 繼續匹配後續路由

    # Critical Alerts → Also send to Slack #alerts-critical
    - match:
        severity: critical
      receiver: 'slack-critical'
      group_wait: 10s
      repeat_interval: 12h

    # Warning Alerts → Slack #alerts-warnings only
    - match:
        severity: warning
      receiver: 'slack-warnings'
      group_wait: 30s
      repeat_interval: 24h

    # Database Alerts → Database Team
    - match_re:
        service: '(postgres|redis|mongodb)'
      receiver: 'slack-database-team'
      group_by: ['alertname', 'service', 'instance']

    # Workflow Engine Alerts → Workflow Team
    - match:
        team: workflow-engine
      receiver: 'slack-workflow-team'

    # SLO Budget Alerts → Engineering Leads
    - match_re:
        alertname: '.*ErrorBudget.*'
      receiver: 'email-engineering-leads'
      repeat_interval: 6h

# 接收器定義
receivers:
  - name: 'default'
    slack_configs:
      - channel: '#monitoring-default'
        title: '{{ .GroupLabels.alertname }}'
        text: '{{ range .Alerts }}{{ .Annotations.description }}{{ end }}'
        send_resolved: true

  - name: 'pagerduty-critical'
    pagerduty_configs:
      - service_key: 'YOUR_PAGERDUTY_INTEGRATION_KEY'
        description: '{{ .GroupLabels.alertname }}: {{ .CommonAnnotations.summary }}'
        details:
          firing: '{{ .Alerts.Firing | len }}'
          resolved: '{{ .Alerts.Resolved | len }}'
          severity: '{{ .CommonLabels.severity }}'
          runbook: '{{ .CommonAnnotations.runbook_url }}'
        severity: 'critical'

  - name: 'slack-critical'
    slack_configs:
      - channel: '#alerts-critical'
        color: 'danger'
        title: '🚨 CRITICAL: {{ .GroupLabels.alertname }}'
        text: |
          *Summary:* {{ .CommonAnnotations.summary }}
          *Description:* {{ .CommonAnnotations.description }}
          *Firing Alerts:* {{ .Alerts.Firing | len }}
          *Runbook:* {{ .CommonAnnotations.runbook_url }}
        actions:
          - type: button
            text: 'View in Grafana'
            url: 'https://grafana.aiagent.example.com'
          - type: button
            text: 'Silence for 1h'
            url: 'https://alertmanager.aiagent.example.com'
        send_resolved: true

  - name: 'slack-warnings'
    slack_configs:
      - channel: '#alerts-warnings'
        color: 'warning'
        title: '⚠️ WARNING: {{ .GroupLabels.alertname }}'
        text: '{{ range .Alerts }}{{ .Annotations.description }}{{ end }}'
        send_resolved: true

  - name: 'slack-database-team'
    slack_configs:
      - channel: '#team-database'
        title: 'Database Alert: {{ .GroupLabels.alertname }}'
        text: '{{ .CommonAnnotations.description }}'

  - name: 'slack-workflow-team'
    slack_configs:
      - channel: '#team-workflow'
        title: 'Workflow Alert: {{ .GroupLabels.alertname }}'
        text: '{{ .CommonAnnotations.description }}'

  - name: 'email-engineering-leads'
    email_configs:
      - to: 'engineering-leads@example.com'
        from: 'alertmanager@aiagent.example.com'
        smarthost: 'smtp.sendgrid.net:587'
        auth_username: 'apikey'
        auth_password: 'YOUR_SENDGRID_API_KEY'
        headers:
          Subject: '[{{ .GroupLabels.severity | toUpper }}] {{ .GroupLabels.alertname }}'
        html: |
          <h2>Alert: {{ .GroupLabels.alertname }}</h2>
          <p><strong>Summary:</strong> {{ .CommonAnnotations.summary }}</p>
          <p><strong>Description:</strong> {{ .CommonAnnotations.description }}</p>
          <p><strong>Firing Alerts:</strong> {{ .Alerts.Firing | len }}</p>
          <p><strong>Runbook:</strong> <a href="{{ .CommonAnnotations.runbook_url }}">{{ .CommonAnnotations.runbook_url }}</a></p>

# 抑制規則 (Inhibition Rules)
inhibit_rules:
  # 如果 APIDown 告警觸發,抑制該服務的所有其他告警
  - source_match:
      alertname: 'APIDown'
    target_match_re:
      alertname: '(HighErrorRate|HighLatency|HighCPU|HighMemory)'
    equal: ['service', 'instance']

  # Critical 告警觸發時,抑制相同服務的 Warning 告警
  - source_match:
      severity: 'critical'
    target_match:
      severity: 'warning'
    equal: ['alertname', 'service']

  # 如果 PostgreSQL 主節點宕機,抑制副本延遲告警
  - source_match:
      alertname: 'PostgreSQLDown'
      role: 'primary'
    target_match:
      alertname: 'PostgreSQLReplicationLag'
    equal: ['cluster']
```

---

## PagerDuty 集成

### PagerDuty 服務配置

```bash
# 1. 在 PagerDuty 創建服務
# Services → + New Service
# Name: AI Agent Platform - Production
# Integration Type: Prometheus
# Copy Integration Key

# 2. 創建 Escalation Policy
# Escalation Policies → + New Escalation Policy
# Name: Engineering On-Call
# Escalate to:
#   - Level 1: Primary On-Call Engineer (immediately)
#   - Level 2: Secondary On-Call Engineer (after 5 minutes)
#   - Level 3: Engineering Manager (after 15 minutes)

# 3. 創建 On-Call Schedule
# Schedules → + New Schedule
# Name: Engineering Weekly Rotation
# Rotation: Weekly (Monday 9:00 AM)
# Participants: [List of engineers]
```

### Kubernetes Secret 配置

```bash
kubectl create secret generic alertmanager-pagerduty \
  --from-literal=integration-key='YOUR_PAGERDUTY_INTEGRATION_KEY' \
  --namespace monitoring
```

---

## 告警降噪策略

### 1. Grouping (分組)

```yaml
# 按照 alertname, cluster, service 分組
route:
  group_by: ['alertname', 'cluster', 'service']
  group_wait: 10s

# 好處: 如果 10 個 API Pod 同時觸發 HighMemory 告警,
# 只會發送 1 條通知,而不是 10 條
```

### 2. Throttling (限流)

```yaml
# 同一組告警每 12 小時重複發送一次
route:
  repeat_interval: 12h

# 好處: 避免持續告警每分鐘都發送通知
```

### 3. Silencing (靜默)

```bash
# 維護窗口期間靜默所有告警 (通過 UI 或 API)
amtool silence add \
  --alertmanager.url=http://alertmanager:9093 \
  --start='2024-01-15T02:00:00Z' \
  --end='2024-01-15T04:00:00Z' \
  --comment='Planned maintenance: Database upgrade' \
  alertname=~ '.+'

# 靜默特定服務的告警
amtool silence add \
  --alertmanager.url=http://alertmanager:9093 \
  --duration=2h \
  --comment='Investigating HighLatency on api-service' \
  service='api' \
  alertname='HighLatency'
```

### 4. Inhibition (抑制)

```yaml
# 如果 PostgreSQL 主節點宕機,自動抑制副本延遲告警
inhibit_rules:
  - source_match:
      alertname: 'PostgreSQLDown'
      role: 'primary'
    target_match:
      alertname: 'PostgreSQLReplicationLag'
    equal: ['cluster']

# 原理: 主節點宕機時,副本延遲告警是預期的,無需額外通知
```

---

## Runbook 編寫規範

### Runbook 模板

```markdown
# Runbook: High API Error Rate

## Severity
**Critical** - User-facing impact

## Summary
API is returning 5xx errors at a rate exceeding 5% of all requests.

## Impact
- Users are experiencing service errors
- Some workflows may fail to execute
- Potential data loss if errors are unrecoverable

## Diagnosis Steps

1. **Check Error Distribution**
   ```promql
   sum by (code) (rate(http_requests_total{code=~"5.."}[5m]))
   ```
   - Identify which specific error codes are most common (500, 502, 503, 504)

2. **Check Recent Deployments**
   ```bash
   kubectl rollout history deployment/api-deployment -n aiagent-prod
   ```
   - Was there a recent deployment? (last 30 minutes)

3. **Check Database Connectivity**
   ```bash
   kubectl exec -n aiagent-prod api-pod-xxxx -- curl -f http://localhost:8080/health/db
   ```
   - Is database reachable?
   - Are database connections exhausted?

4. **Check External Dependencies**
   - Azure OpenAI: Rate limiting? (429 errors)
   - Redis: Connection failures?

5. **Check Application Logs**
   ```bash
   kubectl logs -n aiagent-prod deployment/api-deployment --tail=100 | grep ERROR
   ```

## Resolution Steps

### Scenario 1: Recent Bad Deployment
```bash
# Rollback to previous version
kubectl rollout undo deployment/api-deployment -n aiagent-prod
kubectl rollout status deployment/api-deployment -n aiagent-prod
```

### Scenario 2: Database Connection Pool Exhausted
```bash
# Scale up API pods to distribute connection load
kubectl scale deployment/api-deployment -n aiagent-prod --replicas=10

# Or increase database connection limit (requires PostgreSQL restart)
```

### Scenario 3: External Dependency Failure
- If Azure OpenAI is down: Enable circuit breaker, return cached responses
- If Redis is down: Fall back to in-memory cache

## Escalation
If unable to resolve within 15 minutes:
1. Escalate to Engineering Manager (via PagerDuty)
2. Start incident communication in #incidents Slack channel
3. Update status page: https://status.aiagent.example.com

## Post-Incident
1. Write post-mortem within 48 hours
2. Identify root cause and contributing factors
3. Create action items to prevent recurrence
4. Update this runbook with lessons learned

## Related Alerts
- `APIDown`: Complete service outage
- `HighHTTPLatency`: Performance degradation
- `DatabaseConnectionFailures`: Database issues

## Grafana Dashboards
- [API Overview](https://grafana.aiagent.example.com/d/api-overview)
- [Database Metrics](https://grafana.aiagent.example.com/d/database)

## References
- [Error Handling Documentation](https://wiki.example.com/error-handling)
- [Database Troubleshooting Guide](https://wiki.example.com/db-troubleshooting)
```

---

## 告警疲勞防治

### 1. Alert Fatigue 指標監控

```promql
# 告警觸發頻率
sum(rate(alertmanager_alerts_received_total[24h]))

# 告警解決時間 (MTTR - Mean Time To Resolution)
avg(alertmanager_alerts_resolution_time_seconds)

# 告警噪音比 (False Positive Rate)
sum(rate(alertmanager_alerts_resolved_total{resolution="false_positive"}[7d]))
/
sum(rate(alertmanager_alerts_received_total[7d]))
```

### 2. 定期審查告警規則

```yaml
# 每月審查告警規則有效性
alert_review_checklist:
  - name: "Review High-Frequency Alerts"
    question: "Which alerts fired most frequently last month?"
    action: "Consider increasing threshold or adding `for` duration"

  - name: "Review Actionability"
    question: "For each alert fired, was action taken?"
    action: "If no action taken consistently, consider removing alert"

  - name: "Review Resolution Time"
    question: "What was average MTTR for each alert type?"
    action: "Update runbooks for slow-resolving alerts"

  - name: "Review False Positives"
    question: "Which alerts had highest false positive rate?"
    action: "Refine alert conditions or add context"
```

### 3. Error Budget Policy

```yaml
# Error Budget Exhaustion Policy
error_budget_policy:
  # 當錯誤預算消耗 > 50%, 暫停所有非關鍵功能發布
  - threshold: 50%
    action: "Freeze non-critical feature deployments"

  # 當錯誤預算消耗 > 75%, 專注於可靠性工作
  - threshold: 75%
    action: "Dedicated SRE sprint for reliability improvements"

  # 當錯誤預算消耗 > 100%, 觸發 postmortem
  - threshold: 100%
    action: "Mandatory postmortem, identify systemic issues"
```

---

## 最佳實踐

### 1. 告警命名規範

```
格式: <Component><Symptom><Severity>
示例:
- APIHighErrorRate           (API 組件, 高錯誤率, 隱含 Critical)
- WorkflowSlowExecution      (Workflow 組件, 執行慢, 隱含 Warning)
- DatabaseConnectionFailure  (Database 組件, 連接失敗, Critical)

避免:
- Alert1, Alert2 (無意義名稱)
- HighCPU (缺少組件上下文)
```

### 2. 告警 Annotation 最佳實踐

```yaml
annotations:
  summary: "簡短描述 (1 句話,適合 Slack 通知標題)"
  description: "詳細描述 (包含當前值、閾值、影響範圍)"
  runbook_url: "Runbook URL (必須提供處理步驟)"
  dashboard_url: "Grafana Dashboard URL (可選,方便查看詳細指標)"
  severity_justification: "為何是 Critical/Warning (有助於審查)"
```

### 3. 告警測試

```bash
# 手動觸發測試告警
amtool alert add \
  alertname='TestAlert' \
  severity='warning' \
  summary='This is a test alert' \
  --alertmanager.url=http://alertmanager:9093

# 驗證告警路由
amtool config routes test \
  --config.file=alertmanager.yaml \
  severity=critical \
  service=api
```

---

## 故障排查

### 常見問題

| 問題 | 原因 | 解決方案 |
|------|------|----------|
| **告警未觸發** | PromQL 語法錯誤 | 在 Prometheus UI → Alerts 測試表達式 |
| **告警未發送到 Slack** | Webhook URL 錯誤 | 檢查 `slack_api_url` 配置,測試 webhook |
| **PagerDuty 未收到通知** | Integration Key 錯誤 | 驗證 PagerDuty Service Key |
| **告警風暴** | 缺少 grouping/throttling | 調整 `group_wait`, `repeat_interval` |

### 調試技巧

```bash
# 查看 Alertmanager 配置
kubectl exec -n monitoring alertmanager-prometheus-kube-prometheus-alertmanager-0 -- \
  amtool config show

# 查看當前活躍告警
kubectl exec -n monitoring alertmanager-prometheus-kube-prometheus-alertmanager-0 -- \
  amtool alert query

# 查看靜默規則
kubectl exec -n monitoring alertmanager-prometheus-kube-prometheus-alertmanager-0 -- \
  amtool silence query

# 測試告警路由
amtool config routes test \
  --config.file=/etc/alertmanager/config/alertmanager.yaml \
  severity=critical service=api
```

---

## 總結

本文檔提供了完整的告警通知策略實施方案,涵蓋:

✅ **SLO-Based Alerting** (Error Budget Burn Rate, Multi-Window Alerts)
✅ **症狀導向告警** (User-Facing Impact, Business Logic Symptoms)
✅ **Alertmanager 配置** (Routing, Grouping, Inhibition, Silencing)
✅ **多渠道集成** (Slack, PagerDuty, Email, Webhook)
✅ **告警降噪** (Grouping, Throttling, Inhibition)
✅ **Runbook 規範** (診斷步驟, 解決方案, 升級路徑)
✅ **告警疲勞防治** (Alert Fatigue Metrics, Regular Review, Error Budget Policy)

透過本指南,運維團隊可以實現:
- **可操作告警**: 每個告警都有明確處理步驟
- **降低噪音**: 通過分組、限流、抑制減少 80% 告警量
- **快速響應**: PagerDuty 集成,On-Call 工程師第一時間收到通知
- **持續改進**: 定期審查告警有效性,消除 False Positives
