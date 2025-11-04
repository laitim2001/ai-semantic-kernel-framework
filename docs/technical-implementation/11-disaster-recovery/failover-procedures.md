# 故障轉移流程指南

## 概述

### 文檔目的
本文檔提供 AI Agent 工作流平台的完整故障轉移 (Failover) 流程,涵蓋:
- **自動化故障轉移** (PostgreSQL, Redis, Kubernetes)
- **手動故障轉移流程** (計劃內維護、區域級災難)
- **Database Failover** (Primary → Replica Promotion)
- **Application Failover** (Pod 重新調度、流量切換)
- **DNS Failover** (Azure Traffic Manager, Front Door)
- **Health Check Based Routing** (基於健康檢查的流量路由)
- **Rollback Procedures** (故障轉移失敗後的回滾)
- **Failover Testing** (定期故障轉移演練)

### 故障轉移類型

| 故障轉移類型 | 觸發條件 | 自動化程度 | RTO | RPO | 風險級別 |
|-------------|---------|-----------|-----|-----|----------|
| **Pod Failover** | Pod Liveness Probe 失敗 | 🟢 全自動 | < 30 秒 | 0 | 🟢 低 |
| **Node Failover** | Kubernetes Node 不可用 | 🟢 全自動 | < 2 分鐘 | 0 | 🟡 中 |
| **Redis Failover** | Redis Master 失敗 | 🟢 全自動 (Sentinel) | < 10 秒 | < 1 秒 | 🟢 低 |
| **Database Failover (同區)** | PostgreSQL Primary 失敗 | 🟡 半自動 | < 5 分鐘 | 0 (同步複製) | 🟡 中 |
| **Database Failover (跨區)** | 整個 Region 不可用 | 🔴 手動 | < 2 小時 | < 15 分鐘 | 🔴 高 |
| **Application Gateway Failover** | Gateway 實例失敗 | 🟢 全自動 | < 30 秒 | 0 | 🟢 低 |
| **Region Failover** | Azure Region 完全不可用 | 🔴 手動 | < 4 小時 | < 15 分鐘 | 🔴 Critical |

---

## 自動化故障轉移

### Pod 級別自動故障轉移

**觸發條件**: Liveness Probe 連續失敗 3 次 (30 秒)

**Kubernetes 自動恢復流程**:
```
1. Liveness Probe 檢測到 Pod 無響應 (10s × 3 次 = 30s)
2. Kubernetes 標記 Pod 為 Unhealthy
3. Kubernetes 殺死並重啟 Pod
4. 新 Pod 啟動並通過 Readiness Probe
5. Service 將流量路由到新 Pod
```

**驗證自動恢復**:

```bash
# 模擬 Pod 崩潰
kubectl exec -n aiagent-prod api-pod-xxxxx -- kill 1

# 觀察 Pod 重啟
kubectl get pods -n aiagent-prod -w

# 查看事件日誌
kubectl describe pod api-pod-xxxxx -n aiagent-prod | grep -A 10 "Events:"
```

### Node 級別自動故障轉移

**觸發條件**: Kubernetes Node 失去心跳 (40 秒)

**Kubernetes 自動調度流程**:
```
1. kubelet 停止向 API Server 發送心跳
2. Node Controller 標記 Node 為 NotReady (40s)
3. Pod Eviction 開始 (5 分鐘寬限期)
4. Scheduler 在其他健康 Node 上重新調度 Pod
5. 新 Pod 啟動並加入 Service Endpoints
```

**Node 故障模擬測試**:

```bash
# 手動將 Node 標記為不可調度 (模擬故障)
kubectl cordon <node-name>

# 驅逐 Node 上的所有 Pod
kubectl drain <node-name> --ignore-daemonsets --delete-emptydir-data

# 觀察 Pod 重新調度
kubectl get pods -n aiagent-prod -o wide

# 恢復 Node
kubectl uncordon <node-name>
```

---

## Redis 自動故障轉移 (Sentinel)

### Sentinel 自動 Failover 流程

```
┌─────────────────────────────────────────────────────────────────┐
│ Redis Sentinel Auto-Failover Process                            │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  Step 1: Master Failure Detection (30s)                        │
│  ├─ Sentinel 1 檢測到 Master 無響應                            │
│  ├─ Sentinel 2 確認 Master 失敗                                │
│  └─ Sentinel 3 確認 Master 失敗                                │
│                                                                 │
│  Step 2: Quorum Reached (Majority: 2 out of 3)                 │
│  ├─ Sentinels 達成共識: Master 已失敗                          │
│  └─ 開始 Failover 流程                                         │
│                                                                 │
│  Step 3: Leader Election                                       │
│  └─ Sentinel 1 被選為 Failover 執行者                          │
│                                                                 │
│  Step 4: Replica Promotion (<10s)                              │
│  ├─ 選擇最佳 Replica (最低延遲, 最新數據)                      │
│  ├─ 將 Replica 1 提升為新 Master                               │
│  └─ Replica 1 開始接受寫入                                     │
│                                                                 │
│  Step 5: Reconfiguration                                       │
│  ├─ 其他 Replica 重新配置為新 Master 的從屬                    │
│  ├─ Sentinel 更新內部配置                                      │
│  └─ 客戶端自動重新連接到新 Master                              │
│                                                                 │
│  Total Failover Time: ~40s                                     │
│  (Detection 30s + Promotion 10s)                               │
└─────────────────────────────────────────────────────────────────┘
```

### 客戶端配置 (自動發現新 Master)

```csharp
// src/Infrastructure/Redis/RedisSentinelConnectionFactory.cs
using StackExchange.Redis;

public sealed class RedisSentinelConnectionFactory
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<RedisSentinelConnectionFactory> _logger;
    private ConnectionMultiplexer? _connection;

    public async Task<IConnectionMultiplexer> GetConnectionAsync(
        CancellationToken cancellationToken = default)
    {
        if (_connection != null && _connection.IsConnected)
            return _connection;

        var sentinelEndpoints = _configuration.GetSection("Redis:SentinelEndpoints")
            .Get<string[]>() ?? Array.Empty<string>();

        var masterName = _configuration["Redis:MasterName"] ?? "mymaster";

        var configOptions = new ConfigurationOptions
        {
            ServiceName = masterName,
            TieBreaker = "",
            CommandMap = CommandMap.Sentinel,
            AbortOnConnectFail = false,
            ConnectRetry = 5,
            ConnectTimeout = 5000,
            SyncTimeout = 5000,
            AllowAdmin = true
        };

        // 添加所有 Sentinel 節點
        foreach (var endpoint in sentinelEndpoints)
        {
            configOptions.EndPoints.Add(endpoint);
        }

        _logger.LogInformation("Connecting to Redis Sentinel: {Endpoints}",
            string.Join(", ", sentinelEndpoints));

        // 連接到 Sentinel
        var sentinelConnection = await ConnectionMultiplexer.ConnectAsync(configOptions);

        // 獲取當前 Master 地址
        var masterEndpoints = await sentinelConnection.GetSentinelMasterConnectionAsync(
            serviceName: masterName,
            cancellationToken: cancellationToken);

        _logger.LogInformation("Redis Master discovered: {MasterEndpoint}",
            masterEndpoints.ToString());

        // 連接到 Master
        var masterConfigOptions = new ConfigurationOptions
        {
            EndPoints = { masterEndpoints },
            Password = _configuration["Redis:Password"],
            AbortOnConnectFail = false,
            ConnectRetry = 3,
            ReconnectRetryPolicy = new ExponentialRetry(5000)  // 5 秒指數退避
        };

        _connection = await ConnectionMultiplexer.ConnectAsync(masterConfigOptions);

        // 訂閱 Sentinel 事件 (自動重連)
        sentinelConnection.GetSubscriber().Subscribe(
            "+switch-master",
            async (channel, message) =>
            {
                _logger.LogWarning("Redis Master switched: {Message}", message);
                await GetConnectionAsync(cancellationToken);  // 重新連接新 Master
            });

        return _connection;
    }
}
```

### appsettings.json 配置

```json
{
  "Redis": {
    "SentinelEndpoints": [
      "redis-sentinel-0.redis-sentinel.aiagent-prod.svc.cluster.local:26379",
      "redis-sentinel-1.redis-sentinel.aiagent-prod.svc.cluster.local:26379",
      "redis-sentinel-2.redis-sentinel.aiagent-prod.svc.cluster.local:26379"
    ],
    "MasterName": "mymaster",
    "Password": "<strong-password>"
  }
}
```

---

## PostgreSQL 手動故障轉移

### 同區域 Standby 提升 (Synchronous Standby)

**場景**: Primary (Zone 1) 失敗,提升 Synchronous Standby (Zone 2) 為新 Primary

#### Step 1: 確認 Primary 已失敗

```bash
# 嘗試連接 Primary
psql -h postgresql-primary.aiagent-prod.svc.cluster.local \
     -U postgres \
     -c "SELECT version();"

# 如果連接失敗,檢查 Pod 狀態
kubectl get pods -n aiagent-prod -l role=primary

# 檢查 Primary 的日誌
kubectl logs -n aiagent-prod postgresql-primary-0 --tail=100
```

#### Step 2: 檢查 Standby 狀態

```sql
-- 連接到 Standby
psql -h postgresql-standby.aiagent-prod.svc.cluster.local \
     -U postgres

-- 檢查複製狀態
SELECT
  pg_is_in_recovery() AS is_standby,
  pg_last_wal_receive_lsn() AS receive_lsn,
  pg_last_wal_replay_lsn() AS replay_lsn,
  pg_last_xact_replay_timestamp() AS last_replay_time,
  EXTRACT(EPOCH FROM (now() - pg_last_xact_replay_timestamp())) AS replay_lag_seconds;

-- 如果 is_standby = true 且 replay_lag_seconds < 10,可以安全提升
```

#### Step 3: 提升 Standby 為 Primary

```bash
#!/bin/bash
# scripts/promote-standby.sh

set -e

STANDBY_POD="postgresql-standby-0"
NAMESPACE="aiagent-prod"

echo "🚀 Promoting Standby to Primary..."

# 1. 執行 pg_ctl promote
kubectl exec -n $NAMESPACE $STANDBY_POD -- \
  su - postgres -c "pg_ctl promote -D /var/lib/postgresql/data/pgdata"

# 2. 等待提升完成
sleep 10

# 3. 驗證 Standby 已成為 Primary
RECOVERY_STATUS=$(kubectl exec -n $NAMESPACE $STANDBY_POD -- \
  su - postgres -c "psql -t -c 'SELECT pg_is_in_recovery();'")

if [ "$RECOVERY_STATUS" = "f" ]; then
  echo "✅ Standby successfully promoted to Primary"
else
  echo "❌ Promotion failed, still in recovery mode"
  exit 1
fi

# 4. 驗證可以寫入
kubectl exec -n $NAMESPACE $STANDBY_POD -- \
  su - postgres -c "psql -c 'CREATE TABLE failover_test (id int); DROP TABLE failover_test;'"

echo "✅ New Primary is accepting writes"
```

#### Step 4: 更新 Kubernetes Service

```bash
# 將流量切換到新 Primary (更新 Service Selector)
kubectl patch service postgresql-primary -n aiagent-prod \
  -p '{"spec":{"selector":{"statefulset.kubernetes.io/pod-name":"postgresql-standby-0"}}}'

# 驗證 Service Endpoints
kubectl get endpoints postgresql-primary -n aiagent-prod
```

#### Step 5: 應用重新連接

```bash
# 重啟應用 Pods 以重新連接數據庫
kubectl rollout restart deployment/api-deployment -n aiagent-prod

# 觀察 Rollout 進度
kubectl rollout status deployment/api-deployment -n aiagent-prod
```

---

## 跨區域手動故障轉移 (Region Failover)

### 場景: Primary Region (East US) 完全不可用

**執行前提**:
- DR Region (West US 2) 的 Async Replica 正常運行
- Replication Lag < 15 分鐘
- 已獲得管理層批准

### Step 1: 激活 DR 站點決策

```bash
#!/bin/bash
# scripts/activate-dr-site.sh

set -e

echo "⚠️  WARNING: Activating DR Site (West US 2)"
echo "This will make DR Region the new Primary."
read -p "Are you sure? Type 'ACTIVATE DR' to confirm: " CONFIRMATION

if [ "$CONFIRMATION" != "ACTIVATE DR" ]; then
  echo "❌ Activation cancelled"
  exit 1
fi

echo "🚀 Starting DR Site activation..."
```

### Step 2: 提升 DR Region Replica 為 Primary

```bash
# 連接到 DR Region PostgreSQL
psql -h aiagent-postgres-dr.postgres.database.azure.com \
     -U dbadmin \
     -d aiagent_prod

-- 檢查 Replication Lag
SELECT
  EXTRACT(EPOCH FROM (now() - pg_last_xact_replay_timestamp())) AS lag_seconds;

-- 如果 lag_seconds < 900 (15 分鐘),執行提升
\! exit

# 使用 Azure CLI 停止複製 (提升為 Primary)
az postgres flexible-server replica stop-replication \
  --resource-group aiagent-dr-rg \
  --name aiagent-postgres-dr \
  --yes

echo "✅ DR Replica promoted to Primary"
```

### Step 3: 部署應用到 DR Region

```bash
# 確保 DR Region AKS 集群正常
kubectl get nodes --context=aiagent-aks-dr

# 部署應用到 DR Region
kubectl apply -k k8s/overlays/dr/ --context=aiagent-aks-dr

# 等待 Pods 就緒
kubectl wait --for=condition=ready pod \
  -l app=api \
  -n aiagent-prod \
  --timeout=300s \
  --context=aiagent-aks-dr
```

### Step 4: DNS 流量切換 (Azure Front Door)

```bash
# 更新 Azure Front Door 後端池,指向 DR Region
az afd route update \
  --resource-group aiagent-prod-rg \
  --profile-name aiagent-frontdoor \
  --endpoint-name aiagent-api \
  --route-name default-route \
  --origin-group dr-region-backend-pool \
  --enable-compression true

echo "✅ Traffic routed to DR Region"
```

### Step 5: 驗證 DR 站點運行

```bash
# 健康檢查
curl -s https://api.aiagent.com/health | jq .

# 驗證數據庫連接
curl -s https://api.aiagent.com/health/ready | jq .

# 執行冒煙測試
curl -X POST https://api.aiagent.com/api/v1/workflows \
  -H "Authorization: Bearer $TEST_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"name":"DR Test Workflow"}'
```

---

## Health Check Based Routing

### Azure Application Gateway Health Probe

**配置自定義 Health Probe**:

```bash
# 創建 Health Probe
az network application-gateway probe create \
  --gateway-name aiagent-appgw \
  --resource-group aiagent-prod-rg \
  --name api-health-probe \
  --protocol Http \
  --host-name-from-http-settings true \
  --path /health/ready \
  --interval 30 \
  --timeout 30 \
  --unhealthy-threshold 3 \
  --match-status-codes 200

# 關聯 Backend HTTP Settings
az network application-gateway http-settings update \
  --gateway-name aiagent-appgw \
  --resource-group aiagent-prod-rg \
  --name api-http-settings \
  --probe api-health-probe
```

### Application Gateway Backend Health 監控

```bash
# 查看後端健康狀態
az network application-gateway show-backend-health \
  --resource-group aiagent-prod-rg \
  --name aiagent-appgw \
  --query "backendAddressPools[0].backendHttpSettingsCollection[0].servers[*].{Address:address, Health:health}" \
  --output table

# 預期輸出:
# Address                  Health
# ----------------------   --------
# 10.0.1.10 (Pod 1)        Healthy
# 10.0.1.11 (Pod 2)        Healthy
# 10.0.1.12 (Pod 3)        Unhealthy  # 自動從 LB 移除
```

---

## Rollback Procedures

### 故障轉移失敗回滾

**場景**: Standby 提升後發現數據損壞或應用無法連接

#### Step 1: 停止新 Primary 寫入

```bash
# 將 PostgreSQL 設為只讀模式
kubectl exec -n aiagent-prod postgresql-standby-0 -- \
  su - postgres -c "psql -c 'ALTER SYSTEM SET default_transaction_read_only = on;'"

kubectl exec -n aiagent-prod postgresql-standby-0 -- \
  su - postgres -c "psql -c 'SELECT pg_reload_conf();'"

echo "🔒 New Primary set to READ-ONLY mode"
```

#### Step 2: 恢復原 Primary

```bash
# 如果原 Primary 可恢復,重新啟動
kubectl delete pod postgresql-primary-0 -n aiagent-prod
kubectl wait --for=condition=ready pod/postgresql-primary-0 -n aiagent-prod --timeout=300s

# 驗證原 Primary 狀態
psql -h postgresql-primary.aiagent-prod.svc.cluster.local \
     -U postgres \
     -c "SELECT pg_is_in_recovery();"
```

#### Step 3: 重新配置 Standby

```bash
# 將新 Primary 降級為 Standby
kubectl exec -n aiagent-prod postgresql-standby-0 -- \
  bash -c "touch /var/lib/postgresql/data/pgdata/standby.signal"

# 重啟 PostgreSQL
kubectl delete pod postgresql-standby-0 -n aiagent-prod
```

#### Step 4: 切換流量回原 Primary

```bash
# 恢復 Service 指向原 Primary
kubectl patch service postgresql-primary -n aiagent-prod \
  -p '{"spec":{"selector":{"statefulset.kubernetes.io/pod-name":"postgresql-primary-0"}}}'

# 重啟應用
kubectl rollout restart deployment/api-deployment -n aiagent-prod
```

---

## Failover Testing

### 定期故障轉移演練計劃

| 測試類型 | 頻率 | 執行時間 | 參與人員 | 成功標準 |
|---------|------|---------|---------|---------|
| **Pod Failover 測試** | 每月 | 工作日 10:00-11:00 | DevOps Engineer | Pod 重啟 < 30 秒 |
| **Node Failover 測試** | 每季度 | 週日 02:00-04:00 | DevOps + SRE | Pod 重新調度 < 2 分鐘 |
| **Redis Failover 測試** | 每季度 | 週日 02:00-03:00 | DevOps Engineer | Master 切換 < 10 秒 |
| **Database Failover 測試** | 每半年 | 週日 01:00-05:00 | DBA + DevOps + SRE | RTO < 5 分鐘, RPO = 0 |
| **Region Failover 演練** | 每年 | 事先通知客戶 | 全體技術團隊 | RTO < 4 小時, RPO < 15 分鐘 |

### Database Failover 測試腳本

```bash
#!/bin/bash
# scripts/test-database-failover.sh

set -e

echo "=== PostgreSQL Failover Test ==="
echo "Test Date: $(date)"

# 1. 記錄測試開始時間
START_TIME=$(date +%s)

# 2. 驗證複製狀態
echo "Step 1: Checking replication status..."
REPLICATION_LAG=$(psql -h postgresql-primary -U postgres -t -c \
  "SELECT EXTRACT(EPOCH FROM (now() - pg_last_xact_replay_timestamp())) FROM pg_stat_replication;")

if (( $(echo "$REPLICATION_LAG > 5" | bc -l) )); then
  echo "❌ Replication lag too high: ${REPLICATION_LAG}s"
  exit 1
fi

echo "✅ Replication lag: ${REPLICATION_LAG}s"

# 3. 創建測試數據
echo "Step 2: Creating test data on Primary..."
TEST_ID=$(uuidgen)
psql -h postgresql-primary -U postgres -d aiagent_prod -c \
  "INSERT INTO failover_tests (id, test_time) VALUES ('$TEST_ID', NOW());"

# 4. 驗證數據已複製到 Standby
sleep 2
STANDBY_COUNT=$(psql -h postgresql-standby -U postgres -d aiagent_prod -t -c \
  "SELECT COUNT(*) FROM failover_tests WHERE id = '$TEST_ID';")

if [ "$STANDBY_COUNT" != "1" ]; then
  echo "❌ Test data not replicated to Standby"
  exit 1
fi

echo "✅ Test data replicated successfully"

# 5. 執行 Failover
echo "Step 3: Executing Failover (promoting Standby)..."
./scripts/promote-standby.sh

# 6. 計算 Failover 時間
FAILOVER_COMPLETE_TIME=$(date +%s)
FAILOVER_DURATION=$((FAILOVER_COMPLETE_TIME - START_TIME))

echo "⏱️  Failover Duration: ${FAILOVER_DURATION}s"

# 7. 驗證新 Primary 可寫入
echo "Step 4: Verifying new Primary is writable..."
psql -h postgresql-primary -U postgres -d aiagent_prod -c \
  "INSERT INTO failover_tests (id, test_time) VALUES ('$(uuidgen)', NOW());"

echo "✅ New Primary accepting writes"

# 8. 生成測試報告
echo ""
echo "=== Failover Test Results ==="
echo "Start Time: $(date -d @$START_TIME)"
echo "Completion Time: $(date -d @$FAILOVER_COMPLETE_TIME)"
echo "Total Duration: ${FAILOVER_DURATION}s"
echo "Target RTO: 300s (5 minutes)"

if [ "$FAILOVER_DURATION" -lt 300 ]; then
  echo "✅ PASS: Failover completed within RTO"
else
  echo "❌ FAIL: Failover exceeded RTO"
  exit 1
fi
```

---

## 檢查清單

### Failover 執行前檢查清單

- [ ] **決策確認**
  - [ ] 確認 Primary 確實不可恢復
  - [ ] 獲得必要的審批 (Manager, DBA, CTO)
  - [ ] 通知所有相關人員 (DevOps, SRE, Support)

- [ ] **Standby/Replica 狀態檢查**
  - [ ] Replication Lag < 15 分鐘
  - [ ] Standby 健康狀態正常
  - [ ] 磁盤空間充足

- [ ] **備份驗證**
  - [ ] 最新備份可用 (< 1 小時)
  - [ ] 備份完整性已驗證
  - [ ] 恢復演練記錄可查

- [ ] **文檔準備**
  - [ ] Failover 流程文檔已更新
  - [ ] Rollback 流程已準備
  - [ ] 通訊計劃已制定

### Failover 執行後檢查清單

- [ ] **新 Primary 驗證**
  - [ ] 數據庫可讀寫
  - [ ] 應用連接正常
  - [ ] 業務功能正常

- [ ] **監控確認**
  - [ ] Prometheus 指標正常
  - [ ] Grafana Dashboard 無告警
  - [ ] Application Insights 無錯誤

- [ ] **數據一致性檢查**
  - [ ] 執行完整性檢查腳本
  - [ ] 驗證關鍵業務數據
  - [ ] 檢查事務日誌

- [ ] **文檔記錄**
  - [ ] 記錄 Failover 原因和過程
  - [ ] 記錄實際 RTO/RPO
  - [ ] 更新故障後審查 (Post-Mortem)

---

## 總結

本文檔提供了完整的故障轉移流程指南,涵蓋:

✅ **自動化故障轉移** (Pod, Node, Redis, Application Gateway)
✅ **手動故障轉移** (Database Primary-Standby, Region Failover)
✅ **Health Check Routing** (自動移除不健康後端)
✅ **Rollback Procedures** (故障轉移失敗回滾)
✅ **Failover Testing** (定期演練計劃和自動化測試腳本)

透過本指南,運維團隊可以在各種故障場景下快速、準確地執行故障轉移,確保:
- **RTO < 5 分鐘**: 數據庫同區域故障轉移
- **RTO < 4 小時**: 跨區域災難恢復
- **RPO = 0**: 同步複製零數據丟失
- **自動恢復**: Pod/Node/Redis 無需人工介入
