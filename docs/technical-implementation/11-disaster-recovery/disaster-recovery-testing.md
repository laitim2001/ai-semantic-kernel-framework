# 災難恢復測試指南

## 概述

### 文檔目的
本文檔提供 AI Agent 工作流平台的完整災難恢復 (Disaster Recovery, DR) 測試計劃,涵蓋:
- **DR Testing Strategy** (測試策略和目標)
- **Scheduled DR Drills** (定期演練計劃)
- **Test Scenarios** (區域故障、數據損壞、安全事件)
- **Test Automation** (自動化測試腳本)
- **Recovery Time Measurement** (RTO/RPO 測量)
- **Documentation and Runbooks** (Runbook 編寫標準)
- **Post-Test Review** (測試後審查流程)
- **Continuous Improvement** (持續改進機制)

### DR 測試目標

**主要目標**:
- ✅ **驗證 RTO/RPO 目標**: 確認實際恢復時間符合 SLA (RTO ≤ 2 小時, RPO ≤ 15 分鐘)
- ✅ **識別流程缺陷**: 發現 Runbook 中的錯誤或遺漏步驟
- ✅ **培訓團隊**: 確保 DevOps/SRE 團隊熟悉 DR 流程
- ✅ **測試自動化**: 驗證自動化 Failover 腳本正確性
- ✅ **數據完整性驗證**: 確認恢復後數據無損壞
- ✅ **業務連續性**: 驗證關鍵業務流程可正常運行

**成功標準**:
- 測試執行無重大錯誤 (Critical Issues = 0)
- 實際 RTO ≤ 目標 RTO × 1.2 (允許 20% 誤差)
- 數據完整性檢查 100% 通過
- 所有參與人員完成測試後問卷

---

## DR 測試策略

### 測試金字塔模型

```
                     ▲
                    ╱ ╲
                   ╱   ╲
                  ╱     ╲         🔴 Region Failover (年度)
                 ╱_______╲        - 完整跨區域災難演練
                ╱         ╲       - 包含所有服務和數據
               ╱           ╲      - 客戶通知,實際流量切換
              ╱_____________╲
             ╱               ╲    🟡 Service Failover (季度)
            ╱                 ╲   - 單個服務故障轉移
           ╱                   ╲  - Database, Redis, Application
          ╱_____________________╲ - 測試環境或維護窗口
         ╱                       ╲
        ╱                         ╲ 🟢 Component Failover (月度)
       ╱___________________________╲ - Pod, Node, Container 故障
                                    - 自動化測試,生產環境
                                    - 無客戶影響
```

### 測試等級定義

| 測試等級 | 頻率 | 測試環境 | 客戶影響 | 測試範圍 |
|---------|-----|---------|---------|---------|
| **L1: Component Test** | 每月 | 生產環境 | 無 (自動恢復) | Pod/Node Failover |
| **L2: Service Test** | 每季度 | 測試環境 | 無 | Database/Redis/App Failover |
| **L3: Partial DR Test** | 每半年 | DR 環境 | 最小 (只讀流量) | 部分服務切換到 DR 站點 |
| **L4: Full DR Drill** | 每年 | DR 環境 | 計劃內停機 | 完整跨區域 Failover |

---

## 定期 DR 演練計劃

### 年度 DR 測試日曆

```
┌─────────────────────────────────────────────────────────────────┐
│ 2025 Disaster Recovery Testing Calendar                        │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  Q1 (Jan-Mar)                                                   │
│  ├─ 1月第二週: L1 Component Test (Pod Failover)                │
│  ├─ 2月第二週: L1 Component Test (Node Failover)               │
│  ├─ 3月第二週: L2 Service Test (Redis Failover)                │
│  └─ 3月最後週日: Post-Q1 Review Meeting                        │
│                                                                 │
│  Q2 (Apr-Jun)                                                   │
│  ├─ 4月第二週: L1 Component Test (Pod Failover)                │
│  ├─ 5月第二週: L1 Component Test (Node Failover)               │
│  ├─ 6月第二週: L2 Service Test (Database Failover)             │
│  ├─ 6月第三週日: L3 Partial DR Test (Read-Only to DR)          │
│  └─ 6月最後週日: Mid-Year DR Review                            │
│                                                                 │
│  Q3 (Jul-Sep)                                                   │
│  ├─ 7月第二週: L1 Component Test (Pod Failover)                │
│  ├─ 8月第二週: L1 Component Test (Node Failover)               │
│  ├─ 9月第二週: L2 Service Test (Application Failover)          │
│  └─ 9月最後週日: Post-Q3 Review Meeting                        │
│                                                                 │
│  Q4 (Oct-Dec)                                                   │
│  ├─ 10月第二週: L1 Component Test (Pod Failover)               │
│  ├─ 11月第二週: L1 Component Test (Node Failover)              │
│  ├─ 11月第三週日: L4 Full DR Drill (Region Failover) ⭐        │
│  ├─ 12月第一週: Post-DR Drill Review and Improvement           │
│  └─ 12月最後週: Year-End DR Report                             │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

---

## 測試場景設計

### Scenario 1: Region Failure (區域性災難)

**模擬場景**: Azure East US Region 完全不可用 (網絡中斷、數據中心災難)

**測試目標**:
- 驗證 DR Region (West US 2) 可完全接管服務
- 確認 RTO ≤ 4 小時, RPO ≤ 15 分鐘
- 驗證跨區域數據複製正確性

**執行計劃**:

```yaml
# scenario-1-region-failure.yml
scenario:
  name: "Region Failure - Complete East US Outage"
  severity: "Critical"
  estimated_duration: "4-6 hours"
  customer_impact: "Planned 15-minute downtime during DNS切換"

preparation:
  - task: "Notify customers 2 weeks in advance"
    assignee: "Customer Success Team"
    deadline: "2 weeks before test"

  - task: "Verify DR region readiness"
    assignee: "DevOps Team"
    checklist:
      - "DR AKS cluster running and healthy"
      - "DR PostgreSQL replica lag < 5 minutes"
      - "DR Redis standby available"
      - "DR Blob Storage geo-replicated"

  - task: "Prepare test data markers"
    assignee: "QA Team"
    details: "Create unique test workflows for validation"

execution_steps:
  - step: 1
    name: "T-0: Simulate region failure"
    action: "Block all traffic to East US via Network Security Groups"
    responsible: "DevOps Lead"
    duration: "5 minutes"

  - step: 2
    name: "T+5min: Detect failure and initiate DR activation"
    action: "Monitoring alerts trigger, DR activation decision"
    responsible: "SRE On-Call"
    duration: "10 minutes"

  - step: 3
    name: "T+15min: Promote DR PostgreSQL to Primary"
    action: "Execute promote-dr-database.sh script"
    responsible: "DBA"
    duration: "15 minutes"

  - step: 4
    name: "T+30min: Deploy applications to DR region"
    action: "kubectl apply -k k8s/overlays/dr/"
    responsible: "DevOps Engineer"
    duration: "30 minutes"

  - step: 5
    name: "T+1h: Switch DNS to DR region"
    action: "Update Azure Front Door backend pool"
    responsible: "Network Engineer"
    duration: "5 minutes"

  - step: 6
    name: "T+1h5min: Verify DR site operations"
    action: "Run smoke tests and health checks"
    responsible: "QA Engineer"
    duration: "15 minutes"

  - step: 7
    name: "T+1h20min: Monitor for 2 hours"
    action: "Observe metrics, logs, customer feedback"
    responsible: "SRE Team"
    duration: "2 hours"

validation:
  - metric: "RTO (Recovery Time Objective)"
    target: "≤ 4 hours"
    measurement: "Time from failure detection to full service restoration"

  - metric: "RPO (Recovery Point Objective)"
    target: "≤ 15 minutes"
    measurement: "Last transaction timestamp in DR database vs. Primary"

  - metric: "Data Integrity"
    target: "100% pass"
    tests:
      - "Foreign key constraint validation"
      - "Row count comparison (Primary vs DR)"
      - "Test workflow execution successful"

  - metric: "Application Functionality"
    target: "All critical features working"
    tests:
      - "User login successful"
      - "Workflow creation successful"
      - "Agent execution successful"
      - "API response time < 500ms (p95)"

rollback:
  - step: 1
    name: "Restore East US region access"
    action: "Remove NSG blocking rules"

  - step: 2
    name: "Switch DNS back to East US"
    action: "Update Azure Front Door to primary backend pool"

  - step: 3
    name: "Demote DR database back to replica"
    action: "Reconfigure replication from East US primary"

post_test_review:
  - task: "Document actual RTO/RPO achieved"
  - task: "Identify process improvements"
  - task: "Update DR runbooks"
  - task: "Share learnings with team"
```

### Scenario 2: Database Corruption

**模擬場景**: Primary PostgreSQL 數據損壞 (表刪除、數據加密攻擊)

**測試步驟**:

```bash
#!/bin/bash
# scripts/test-scenario-2-db-corruption.sh

set -e

echo "=== Scenario 2: Database Corruption Test ==="

# 準備階段: 創建測試數據
echo "Step 1: Creating test data..."
TEST_WORKFLOW_ID=$(uuidgen)
psql -h postgresql-primary -U postgres -d aiagent_prod <<EOF
INSERT INTO workflows (id, name, user_id, created_at, updated_at)
VALUES ('$TEST_WORKFLOW_ID', 'DR Test Workflow', 1, NOW(), NOW());
EOF

# 記錄測試開始時間
START_TIME=$(date +%s)

# 模擬數據損壞 (刪除測試工作流表)
echo "Step 2: Simulating data corruption (DROP TABLE workflows)..."
psql -h postgresql-primary -U postgres -d aiagent_prod -c "DROP TABLE workflows;"

# 檢測損壞
echo "Step 3: Detecting corruption..."
CORRUPTION_DETECTED_TIME=$(date +%s)
DETECTION_LAG=$((CORRUPTION_DETECTED_TIME - START_TIME))
echo "Detection time: ${DETECTION_LAG}s"

# 執行 PITR 恢復
echo "Step 4: Initiating Point-in-Time Recovery..."
TARGET_TIME=$(date -d "@$((START_TIME - 60))" '+%Y-%m-%d %H:%M:%S')  # 1 分鐘前
./scripts/pitr-restore.sh "$TARGET_TIME" /var/lib/postgresql/restore

# 驗證恢復的數據
echo "Step 5: Validating restored data..."
psql -h localhost -p 5433 -U postgres -d aiagent_prod <<EOF
SELECT COUNT(*) AS restored_rows
FROM workflows
WHERE id = '$TEST_WORKFLOW_ID';
EOF

# 計算總恢復時間
RECOVERY_COMPLETE_TIME=$(date +%s)
TOTAL_RTO=$((RECOVERY_COMPLETE_TIME - START_TIME))

echo ""
echo "=== Test Results ==="
echo "Detection Time: ${DETECTION_LAG}s"
echo "Total RTO: ${TOTAL_RTO}s"
echo "Target RTO: 7200s (2 hours)"

if [ "$TOTAL_RTO" -lt 7200 ]; then
  echo "✅ PASS: Recovery completed within RTO"
else
  echo "❌ FAIL: Recovery exceeded RTO"
fi
```

### Scenario 3: Security Breach (Ransomware)

**模擬場景**: 惡意加密所有生產數據,需要從未受影響的備份恢復

**測試步驟**:

```yaml
scenario:
  name: "Security Breach - Ransomware Attack Simulation"
  severity: "Critical"
  estimated_duration: "4-8 hours"

execution_steps:
  - step: 1
    name: "Isolate infected environment"
    actions:
      - "Block all network traffic to production AKS"
      - "Stop all application pods"
      - "Disconnect database replication"

  - step: 2
    name: "Identify last clean backup"
    actions:
      - "Review backup history for timestamps before infection"
      - "Verify backup integrity (checksum validation)"
      - "Download backup to isolated restore environment"

  - step: 3
    name: "Restore to clean environment"
    actions:
      - "Provision new AKS cluster (clean environment)"
      - "Restore database from last clean backup"
      - "Deploy applications from trusted container registry"
      - "Restore blob storage from geo-redundant copy"

  - step: 4
    name: "Security hardening"
    actions:
      - "Reset all passwords and credentials"
      - "Rotate API keys and secrets"
      - "Update firewall rules"
      - "Enable enhanced monitoring"

  - step: 5
    name: "Validation and switchover"
    actions:
      - "Run full security scan on restored environment"
      - "Execute comprehensive smoke tests"
      - "Switch DNS to clean environment"
      - "Monitor for 24 hours"

validation:
  - "No malware detected in restored environment"
  - "All credentials rotated successfully"
  - "Data integrity verified (no encryption artifacts)"
  - "RTO ≤ 8 hours (breach detection to service restoration)"
```

---

## 測試自動化

### 自動化測試框架

```csharp
// tests/DisasterRecovery/DrTestFramework.cs
using Xunit;

public sealed class DrTestFramework
{
    private readonly ILogger<DrTestFramework> _logger;
    private readonly DrTestConfig _config;
    private readonly DrMetricsCollector _metrics;

    [Fact]
    public async Task Test_PodFailover_AutoRecovery()
    {
        // Arrange
        var targetPod = await SelectRandomHealthyPodAsync();
        var startTime = DateTime.UtcNow;

        // Act: Delete pod to simulate failure
        await KubernetesClient.DeletePodAsync(targetPod);
        _logger.LogInformation("Deleted pod {PodName} to simulate failure", targetPod.Name);

        // Wait for auto-recovery
        var newPod = await WaitForPodRecoveryAsync(targetPod.DeploymentName, timeout: TimeSpan.FromMinutes(5));

        var recoveryTime = DateTime.UtcNow - startTime;

        // Assert
        Assert.NotNull(newPod);
        Assert.True(recoveryTime < TimeSpan.FromSeconds(30), $"Recovery took {recoveryTime.TotalSeconds}s (target: <30s)");

        // Metrics
        _metrics.RecordRto("pod_failover", recoveryTime.TotalSeconds);
        _metrics.RecordRpo("pod_failover", 0);  // No data loss

        _logger.LogInformation(
            "Pod Failover Test: RTO={RtoSeconds}s, Status=PASS",
            recoveryTime.TotalSeconds);
    }

    [Fact]
    public async Task Test_DatabaseFailover_SynchronousStandby()
    {
        // Arrange
        var startTime = DateTime.UtcNow;
        var testWorkflowId = Guid.NewGuid();

        // Insert test data on Primary
        await DatabaseClient.ExecuteAsync(
            "INSERT INTO workflows (id, name, created_at) VALUES (@id, @name, NOW())",
            new { id = testWorkflowId, name = "DR Test Workflow" });

        // Act: Promote Standby to Primary
        await ExecuteScriptAsync("scripts/promote-standby.sh");

        var promotionTime = DateTime.UtcNow - startTime;

        // Verify new Primary can accept writes
        await DatabaseClient.ExecuteAsync(
            "INSERT INTO workflows (id, name, created_at) VALUES (@id, @name, NOW())",
            new { id = Guid.NewGuid(), name = "Post-Failover Test" });

        // Verify test data exists (RPO = 0)
        var testWorkflow = await DatabaseClient.QuerySingleOrDefaultAsync<Workflow>(
            "SELECT * FROM workflows WHERE id = @id",
            new { id = testWorkflowId });

        // Assert
        Assert.NotNull(testWorkflow);
        Assert.True(promotionTime < TimeSpan.FromMinutes(5), $"Promotion took {promotionTime.TotalMinutes}min (target: <5min)");

        // Metrics
        _metrics.RecordRto("database_failover", promotionTime.TotalSeconds);
        _metrics.RecordRpo("database_failover", 0);  // Synchronous replication

        _logger.LogInformation(
            "Database Failover Test: RTO={RtoSeconds}s, RPO=0, Status=PASS",
            promotionTime.TotalSeconds);
    }

    private async Task<Pod> WaitForPodRecoveryAsync(string deploymentName, TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow + timeout;

        while (DateTime.UtcNow < deadline)
        {
            var pods = await KubernetesClient.ListPodsAsync(deploymentName);
            var readyPods = pods.Where(p => p.Status == "Running" && p.Ready).ToList();

            if (readyPods.Any())
            {
                return readyPods.First();
            }

            await Task.Delay(TimeSpan.FromSeconds(5));
        }

        throw new TimeoutException($"Pod recovery timed out after {timeout.TotalSeconds}s");
    }
}
```

### 持續集成測試

```yaml
# .github/workflows/dr-tests.yml
name: Disaster Recovery Tests

on:
  schedule:
    # 每月第二個星期一 10:00 UTC
    - cron: '0 10 8-14 * 1'
  workflow_dispatch:  # 手動觸發

jobs:
  l1-component-tests:
    name: L1 Component Failover Tests
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'

      - name: Setup kubectl
        uses: azure/setup-kubectl@v3

      - name: Azure Login
        uses: azure/login@v1
        with:
          creds: ${{ secrets.AZURE_CREDENTIALS }}

      - name: Get AKS credentials
        run: |
          az aks get-credentials \
            --resource-group aiagent-prod-rg \
            --name aiagent-aks-prod \
            --overwrite-existing

      - name: Run Pod Failover Test
        run: |
          dotnet test tests/DisasterRecovery \
            --filter "FullyQualifiedName~PodFailover" \
            --logger "trx;LogFileName=pod-failover-results.trx"

      - name: Run Node Failover Test
        run: |
          dotnet test tests/DisasterRecovery \
            --filter "FullyQualifiedName~NodeFailover" \
            --logger "trx;LogFileName=node-failover-results.trx"

      - name: Upload test results
        uses: actions/upload-artifact@v3
        with:
          name: dr-test-results
          path: "**/*.trx"

      - name: Notify Slack
        if: always()
        uses: slackapi/slack-github-action@v1
        with:
          payload: |
            {
              "text": "DR Test Results: ${{ job.status }}",
              "attachments": [{
                "color": "${{ job.status == 'success' && 'good' || 'danger' }}",
                "fields": [
                  { "title": "Test Suite", "value": "L1 Component Tests", "short": true },
                  { "title": "Status", "value": "${{ job.status }}", "short": true }
                ]
              }]
            }
        env:
          SLACK_WEBHOOK_URL: ${{ secrets.SLACK_WEBHOOK_URL }}
```

---

## RTO/RPO 測量與報告

### 自動化指標收集

```csharp
// src/Infrastructure/DR/DrMetricsCollector.cs
using Prometheus;

public sealed class DrMetricsCollector
{
    private static readonly Histogram DrRtoSeconds = Metrics.CreateHistogram(
        "dr_rto_seconds",
        "Disaster Recovery RTO in seconds",
        new HistogramConfiguration
        {
            LabelNames = new[] { "scenario_type" },
            Buckets = new[] { 30, 60, 300, 600, 1800, 3600, 7200, 14400 }
        });

    private static readonly Histogram DrRpoSeconds = Metrics.CreateHistogram(
        "dr_rpo_seconds",
        "Disaster Recovery RPO in seconds",
        new HistogramConfiguration
        {
            LabelNames = new[] { "scenario_type" },
            Buckets = new[] { 0, 1, 5, 15, 60, 300, 900 }
        });

    private static readonly Counter DrTestsTotal = Metrics.CreateCounter(
        "dr_tests_total",
        "Total number of DR tests executed",
        new CounterConfiguration
        {
            LabelNames = new[] { "scenario_type", "result" }
        });

    public void RecordRto(string scenarioType, double rtoSeconds)
    {
        DrRtoSeconds.WithLabels(scenarioType).Observe(rtoSeconds);
    }

    public void RecordRpo(string scenarioType, double rpoSeconds)
    {
        DrRpoSeconds.WithLabels(scenarioType).Observe(rpoSeconds);
    }

    public void RecordTestResult(string scenarioType, bool passed)
    {
        DrTestsTotal.WithLabels(scenarioType, passed ? "pass" : "fail").Inc();
    }
}
```

### Grafana Dashboard

```json
{
  "dashboard": {
    "title": "Disaster Recovery Test Dashboard",
    "panels": [
      {
        "id": 1,
        "title": "RTO Trends (Last 12 Months)",
        "type": "graph",
        "targets": [
          {
            "expr": "dr_rto_seconds{scenario_type=\"database_failover\"}",
            "legendFormat": "Database Failover RTO"
          },
          {
            "expr": "dr_rto_seconds{scenario_type=\"region_failover\"}",
            "legendFormat": "Region Failover RTO"
          }
        ],
        "yaxes": [
          {
            "label": "RTO (seconds)",
            "format": "s"
          }
        ],
        "alert": {
          "conditions": [
            {
              "evaluator": {
                "params": [7200],
                "type": "gt"
              },
              "query": {
                "params": ["A", "5m", "now"]
              }
            }
          ],
          "message": "RTO exceeded target (2 hours)",
          "name": "RTO Alert"
        }
      },
      {
        "id": 2,
        "title": "DR Test Pass Rate",
        "type": "stat",
        "targets": [
          {
            "expr": "sum(rate(dr_tests_total{result=\"pass\"}[30d])) / sum(rate(dr_tests_total[30d])) * 100",
            "legendFormat": "Pass Rate %"
          }
        ],
        "fieldConfig": {
          "defaults": {
            "thresholds": {
              "steps": [
                { "value": 0, "color": "red" },
                { "value": 80, "color": "yellow" },
                { "value": 95, "color": "green" }
              ]
            }
          }
        }
      }
    ]
  }
}
```

---

## Runbook 編寫標準

### Runbook 模板

```markdown
# Runbook: [Scenario Name]

## Overview
**Purpose**: [What this runbook covers]
**Scope**: [Which systems/services]
**Owner**: [Team/Individual responsible]
**Last Updated**: [Date]

## Prerequisites
- [ ] Access to Azure Portal with Contributor role
- [ ] kubectl access to AKS clusters (Primary + DR)
- [ ] Azure CLI installed and authenticated
- [ ] Approval from [Manager/CTO] (for production)

## Trigger Conditions
- [Condition 1: e.g., Primary database unresponsive for 5 minutes]
- [Condition 2: e.g., Monitoring alert "Database Primary Down"]

## Execution Steps

### Step 1: [Step Name]
**Duration**: [Estimated time]
**Responsible**: [Role]

\`\`\`bash
# Command to execute
kubectl get pods -n aiagent-prod
\`\`\`

**Expected Output**:
\`\`\`
NAME                READY   STATUS
api-pod-1           1/1     Running
\`\`\`

**If Failed**:
- [Troubleshooting action 1]
- [Escalation path]

### Step 2: [Next Step]
...

## Rollback Procedure
[Steps to revert changes if something goes wrong]

## Verification Checklist
- [ ] [Check 1]
- [ ] [Check 2]

## Post-Execution Tasks
- [ ] Update incident ticket
- [ ] Notify stakeholders
- [ ] Schedule post-mortem

## Related Documents
- [Link to DR architecture]
- [Link to monitoring dashboards]
```

---

## Post-Test Review

### 測試後審查會議議程

```markdown
# DR Test Post-Mortem Meeting

**Date**: [Test Date + 3 days]
**Duration**: 90 minutes
**Attendees**: DevOps Team, SRE Team, DBA, Engineering Manager, CTO

## Agenda

### 1. Test Execution Summary (15 min)
- Scenario overview
- Actual vs. target RTO/RPO
- Test pass/fail status

### 2. What Went Well (20 min)
- Successful aspects of the test
- Effective procedures and tools
- Team coordination highlights

### 3. What Went Wrong (30 min)
- Failed steps or exceeded targets
- Process gaps or documentation errors
- Unexpected issues

### 4. Action Items (20 min)
- Runbook updates required
- Automation improvements
- Training needs
- Infrastructure changes

### 5. Next Steps (5 min)
- Assign action item owners
- Set deadlines
- Schedule next test
```

### 測試報告模板

```markdown
# Disaster Recovery Test Report

**Test Date**: 2025-11-15
**Scenario**: Region Failover (East US → West US 2)
**Test Level**: L4 Full DR Drill

## Executive Summary
[Brief overview of test outcome, RTO/RPO achieved, major findings]

## Objectives
- [Objective 1]
- [Objective 2]

## Test Results

| Metric | Target | Actual | Status |
|--------|--------|--------|--------|
| RTO    | ≤ 4 hours | 3h 45min | ✅ Pass |
| RPO    | ≤ 15 min | 12 min | ✅ Pass |
| Data Integrity | 100% pass | 100% pass | ✅ Pass |

## Timeline
| Time | Event | Responsible |
|------|-------|-------------|
| 02:00 | Test initiated | DevOps Lead |
| 02:15 | DR activation | SRE On-Call |
| ...   | ...   | ... |

## Issues Encountered
1. **Issue**: DNS propagation delay (15 min vs. expected 5 min)
   - **Severity**: Medium
   - **Root Cause**: TTL misconfiguration
   - **Action Item**: Update TTL to 60 seconds

## Lessons Learned
- [Lesson 1]
- [Lesson 2]

## Action Items
| ID | Action | Owner | Deadline | Status |
|----|--------|-------|----------|--------|
| 1  | Update DNS TTL | Network Eng | 2025-11-20 | Open |

## Appendix
- [Test logs]
- [Screenshots]
- [Metrics dashboards]
```

---

## 檢查清單

### DR 測試執行檢查清單

- [ ] **測試前準備**
  - [ ] 測試計劃已審批
  - [ ] 參與人員已通知
  - [ ] 客戶已通知 (L4 測試)
  - [ ] Runbook 已更新到最新版本

- [ ] **測試執行**
  - [ ] 所有步驟按 Runbook 執行
  - [ ] 時間戳記錄準確
  - [ ] 問題實時記錄

- [ ] **測試後驗證**
  - [ ] RTO/RPO 測量完成
  - [ ] 數據完整性檢查通過
  - [ ] 應用功能測試通過

- [ ] **測試後清理**
  - [ ] 環境恢復到初始狀態
  - [ ] 測試數據已清理
  - [ ] Rollback 驗證完成

- [ ] **文檔與報告**
  - [ ] 測試報告已完成
  - [ ] Action Items 已分配
  - [ ] Post-Mortem 會議已安排

---

## 總結

本文檔提供了完整的災難恢復測試指南,涵蓋:

✅ **DR Testing Strategy** (測試金字塔,L1-L4 測試等級)
✅ **Scheduled DR Drills** (年度測試日曆,定期演練計劃)
✅ **Test Scenarios** (Region Failure, Database Corruption, Security Breach)
✅ **Test Automation** (自動化測試框架, CI/CD 集成)
✅ **RTO/RPO Measurement** (Prometheus 指標收集, Grafana Dashboard)
✅ **Runbook Standards** (標準化 Runbook 模板)
✅ **Post-Test Review** (測試後審查流程, 持續改進機制)

透過本指南,運維團隊可以建立系統化的 DR 測試流程,確保:
- **季度驗證**: 每季度至少一次 Service-Level Failover 測試
- **年度演練**: 每年一次完整 Region Failover 演練
- **持續改進**: 每次測試後的 Runbook 和流程優化
- **團隊準備**: 所有團隊成員熟悉 DR 流程
