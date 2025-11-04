# 高可用性架構設計

## 概述

### 文檔目的
本文檔提供 AI Agent 工作流平台的完整高可用性 (High Availability, HA) 架構設計,涵蓋:
- **Multi-Zone Deployment** (跨可用區部署)
- **Database Replication** (PostgreSQL Streaming Replication)
- **Redis High Availability** (Redis Sentinel / Redis Cluster)
- **Load Balancing** (Azure Load Balancer + Application Gateway)
- **Auto-Scaling** (Horizontal Pod Autoscaler + Cluster Autoscaler)
- **Stateless Application Design** (無狀態服務設計)
- **Session Persistence** (Session 持久化策略)
- **Zero-Downtime Deployment** (無停機部署策略)

### 可用性目標

| 服務組件 | 可用性目標 | 年度停機時間 | 架構策略 |
|---------|-----------|-------------|---------|
| **API Gateway** | 99.99% | 52.56 分鐘 | Multi-Zone + Auto-Scaling + Health Checks |
| **應用服務 (API)** | 99.95% | 4.38 小時 | Multi-Replica + Rolling Update + Circuit Breaker |
| **PostgreSQL 數據庫** | 99.9% | 8.76 小時 | Primary-Replica + Auto-Failover + PITR Backup |
| **Redis 緩存** | 99.9% | 8.76 小時 | Sentinel 3-Node + Auto-Failover |
| **Blob Storage** | 99.99% | 52.56 分鐘 | Azure GRS (Geo-Redundant Storage) |
| **Kubernetes 控制平面** | 99.95% | 4.38 小時 | Azure AKS Managed Control Plane |

**整體系統可用性計算**:
```
系統可用性 = ∏(各組件可用性)
          = 0.9999 × 0.9995 × 0.999 × 0.999 × 0.9999 × 0.9995
          = 99.73% (年度停機時間 ≈ 23.65 小時)
```

**改進策略**: 通過 Circuit Breaker, Retry, Graceful Degradation 將實際可用性提升至 **99.9%+ (< 8.76 小時/年)**

---

## 高可用性架構拓撲

### 整體架構圖

```
┌────────────────────────────────────────────────────────────────────────┐
│ Global Traffic Manager (Azure Front Door)                             │
│ ├─ Geographic Load Balancing                                          │
│ ├─ DDoS Protection                                                    │
│ └─ SSL Termination                                                    │
└──────────────────────────────────┬─────────────────────────────────────┘
                                   │
        ┌──────────────────────────┴──────────────────────────┐
        │                                                      │
┌───────▼────────────────────────┐      ┌─────────────────────▼──────────┐
│ Primary Region: East US        │      │ DR Region: West US 2           │
├────────────────────────────────┤      ├────────────────────────────────┤
│                                │      │                                │
│  ┌──────────────────────────┐ │      │  ┌──────────────────────────┐  │
│  │ Azure Application Gateway │ │      │  │ Azure Application Gateway │  │
│  │ (WAF + Load Balancer)     │ │      │  │ (Standby)                │  │
│  └────────────┬─────────────── │      │  └──────────────────────────┘  │
│               │                │      │                                │
│  ┌────────────▼─────────────┐ │      │  ┌─────────────────────────┐   │
│  │ AKS Cluster              │ │      │  │ AKS Cluster (Standby)   │   │
│  │ ├─ Zone 1: 2 Nodes       │ │      │  │ └─ Warm Standby         │   │
│  │ ├─ Zone 2: 2 Nodes       │ │      │  └─────────────────────────┘   │
│  │ └─ Zone 3: 2 Nodes       │ │      │                                │
│  │                           │ │      │  ┌─────────────────────────┐   │
│  │  ┌─────────────────────┐ │ │      │  │ PostgreSQL Replica      │   │
│  │  │ API Pods (6 copies) │ │ │      │  │ (Read-Only, Async)      │   │
│  │  │ ├─ Pod 1-2 (Zone 1) │ │ │      │  └─────────────────────────┘   │
│  │  │ ├─ Pod 3-4 (Zone 2) │ │ │      │                                │
│  │  │ └─ Pod 5-6 (Zone 3) │ │ │      │  ┌─────────────────────────┐   │
│  │  └─────────────────────┘ │ │      │  │ Blob Storage (GRS)      │   │
│  │                           │ │      │  │ (Geo-Replicated)        │   │
│  └───────────────────────────┘ │      │  └─────────────────────────┘   │
│                                │      │                                │
│  ┌─────────────────────────┐  │      └────────────────────────────────┘
│  │ PostgreSQL Primary      │  │
│  │ ├─ Streaming Replication├──┼──────► To DR Region Replica
│  │ └─ Synchronous Local    │  │
│  │    Standby (Zone 2)     │  │
│  └─────────────────────────┘  │
│                                │
│  ┌─────────────────────────┐  │
│  │ Redis Sentinel Cluster  │  │
│  │ ├─ Master (Zone 1)      │  │
│  │ ├─ Replica 1 (Zone 2)   │  │
│  │ └─ Replica 2 (Zone 3)   │  │
│  │                          │  │
│  │ Sentinel Nodes:          │  │
│  │ ├─ Sentinel 1 (Zone 1)  │  │
│  │ ├─ Sentinel 2 (Zone 2)  │  │
│  │ └─ Sentinel 3 (Zone 3)  │  │
│  └─────────────────────────┘  │
│                                │
└────────────────────────────────┘
```

---

## Multi-Zone Kubernetes 部署

### AKS 集群配置

```bash
# 創建 Multi-Zone AKS 集群
az aks create \
  --resource-group aiagent-prod-rg \
  --name aiagent-aks-prod \
  --location eastus \
  --zones 1 2 3 \  # 跨 3 個可用區
  --node-count 6 \  # 每個 Zone 2 個節點
  --node-vm-size Standard_D4s_v3 \
  --enable-cluster-autoscaler \
  --min-count 6 \
  --max-count 18 \
  --network-plugin azure \
  --network-policy azure \
  --load-balancer-sku standard \
  --enable-managed-identity \
  --enable-addons monitoring \
  --generate-ssh-keys

# 驗證節點分佈在不同可用區
kubectl get nodes -o custom-columns=NAME:.metadata.name,ZONE:.metadata.labels.topology\\.kubernetes\\.io/zone
```

### Pod Anti-Affinity 配置

**目的**: 確保 Pod 副本分散在不同的可用區和節點,避免單點故障

```yaml
# k8s/base/api-deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: api-deployment
  namespace: aiagent-prod
spec:
  replicas: 6  # 每個 Zone 2 個副本
  selector:
    matchLabels:
      app: api
  template:
    metadata:
      labels:
        app: api
    spec:
      # Pod 反親和性 (Anti-Affinity)
      affinity:
        # 優先分散到不同可用區
        podAntiAffinity:
          preferredDuringSchedulingIgnoredDuringExecution:
          - weight: 100
            podAffinityTerm:
              labelSelector:
                matchLabels:
                  app: api
              topologyKey: topology.kubernetes.io/zone
          # 強制分散到不同節點
          - weight: 50
            podAffinityTerm:
              labelSelector:
                matchLabels:
                  app: api
              topologyKey: kubernetes.io/hostname

      # Pod Disruption Budget (限制同時中斷的 Pod 數量)
      topologySpreadConstraints:
      - maxSkew: 1
        topologyKey: topology.kubernetes.io/zone
        whenUnsatisfiable: DoNotSchedule
        labelSelector:
          matchLabels:
            app: api

      containers:
      - name: api
        image: aiagentplatform.azurecr.io/api:latest
        ports:
        - containerPort: 8080
        resources:
          requests:
            memory: "512Mi"
            cpu: "250m"
          limits:
            memory: "1Gi"
            cpu: "500m"

        # Liveness Probe (檢測死鎖)
        livenessProbe:
          httpGet:
            path: /health/live
            port: 8080
          initialDelaySeconds: 30
          periodSeconds: 10
          timeoutSeconds: 5
          failureThreshold: 3

        # Readiness Probe (檢測就緒狀態)
        readinessProbe:
          httpGet:
            path: /health/ready
            port: 8080
          initialDelaySeconds: 10
          periodSeconds: 5
          timeoutSeconds: 3
          failureThreshold: 3

        # Graceful Shutdown (優雅關閉)
        lifecycle:
          preStop:
            exec:
              command: ["/bin/sh", "-c", "sleep 15"]  # 等待 15 秒讓現有請求完成

      # 優雅終止寬限期
      terminationGracePeriodSeconds: 30
```

### Pod Disruption Budget

**目的**: 限制在維護期間同時中斷的 Pod 數量,確保最小可用副本數

```yaml
# k8s/base/api-pdb.yaml
apiVersion: policy/v1
kind: PodDisruptionBudget
metadata:
  name: api-pdb
  namespace: aiagent-prod
spec:
  minAvailable: 4  # 至少保持 4 個 Pod 可用 (總共 6 個)
  selector:
    matchLabels:
      app: api
  unhealthyPodEvictionPolicy: AlwaysAllow
```

---

## PostgreSQL High Availability

### Streaming Replication 架構

```
┌───────────────────────────────────────────────────┐
│ Primary (East US - Zone 1)                        │
│ ├─ Read/Write Operations                         │
│ ├─ WAL Streaming to Standby                      │
│ └─ Auto-Failover with pg_auto_failover           │
└────────────────────┬──────────────────────────────┘
                     │ Synchronous Replication
                     ▼
┌───────────────────────────────────────────────────┐
│ Synchronous Standby (East US - Zone 2)           │
│ ├─ Immediate Consistency (0 Data Loss)           │
│ ├─ Read-Only Queries                             │
│ └─ Auto-Promotion on Primary Failure             │
└────────────────────┬──────────────────────────────┘
                     │ Asynchronous Replication
                     ▼
┌───────────────────────────────────────────────────┐
│ Asynchronous Standby (West US 2)                 │
│ ├─ DR Region Replica                             │
│ ├─ < 5 min Replication Lag                       │
│ └─ Manual Promotion                              │
└───────────────────────────────────────────────────┘
```

### PostgreSQL HA 配置

#### Primary 配置 (postgresql.conf)

```conf
# postgresql.conf (Primary)

# Replication Settings
wal_level = replica
max_wal_senders = 5
wal_keep_size = 1GB
max_replication_slots = 5

# Synchronous Replication (同步複製到本地 Standby)
synchronous_commit = on
synchronous_standby_names = 'standby1'  # Zone 2 Standby

# Connection Settings
listen_addresses = '*'
max_connections = 200

# Performance Tuning
shared_buffers = 4GB
effective_cache_size = 12GB
maintenance_work_mem = 1GB
checkpoint_completion_target = 0.9
wal_buffers = 16MB
default_statistics_target = 100
random_page_cost = 1.1  # SSD 優化
effective_io_concurrency = 200

# Logging
log_destination = 'csvlog'
logging_collector = on
log_directory = 'log'
log_filename = 'postgresql-%Y-%m-%d_%H%M%S.log'
log_rotation_age = 1d
log_rotation_size = 1GB
log_min_duration_statement = 1000  # 記錄 > 1s 的查詢
log_checkpoints = on
log_connections = on
log_disconnections = on
log_lock_waits = on
log_replication_commands = on
```

#### Standby 配置 (postgresql.conf + standby.signal)

```conf
# postgresql.conf (Standby)

# Standby Settings
hot_standby = on
max_standby_streaming_delay = 30s
hot_standby_feedback = on

# Recovery Settings (在 standby.signal 存在時生效)
primary_conninfo = 'host=postgresql-primary.aiagent-prod.svc.cluster.local port=5432 user=replicator password=xxx application_name=standby1'
primary_slot_name = 'standby1_slot'
```

```bash
# 創建 standby.signal 文件 (啟用 Standby 模式)
touch /var/lib/postgresql/data/standby.signal
```

#### 創建 Replication User 和 Slot

```sql
-- 在 Primary 上執行

-- 1. 創建複製用戶
CREATE USER replicator WITH REPLICATION ENCRYPTED PASSWORD 'strong_password';

-- 2. 創建 Replication Slot (防止 WAL 被過早清理)
SELECT pg_create_physical_replication_slot('standby1_slot');
SELECT pg_create_physical_replication_slot('standby2_slot');

-- 3. 驗證 Slot
SELECT slot_name, slot_type, active, restart_lsn
FROM pg_replication_slots;
```

#### Kubernetes StatefulSet 配置

```yaml
# k8s/base/postgresql-statefulset.yaml
apiVersion: apps/v1
kind: StatefulSet
metadata:
  name: postgresql-primary
  namespace: aiagent-prod
spec:
  serviceName: postgresql-primary
  replicas: 1
  selector:
    matchLabels:
      app: postgresql
      role: primary
  template:
    metadata:
      labels:
        app: postgresql
        role: primary
    spec:
      affinity:
        nodeAffinity:
          requiredDuringSchedulingIgnoredDuringExecution:
            nodeSelectorTerms:
            - matchExpressions:
              - key: topology.kubernetes.io/zone
                operator: In
                values:
                - eastus-1  # 強制部署在 Zone 1
      containers:
      - name: postgresql
        image: postgres:16
        env:
        - name: POSTGRES_PASSWORD
          valueFrom:
            secretKeyRef:
              name: postgresql-credentials
              key: password
        - name: PGDATA
          value: /var/lib/postgresql/data/pgdata
        ports:
        - containerPort: 5432
          name: postgresql
        volumeMounts:
        - name: data
          mountPath: /var/lib/postgresql/data
        - name: config
          mountPath: /etc/postgresql
        livenessProbe:
          exec:
            command:
            - /bin/sh
            - -c
            - pg_isready -U postgres
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          exec:
            command:
            - /bin/sh
            - -c
            - pg_isready -U postgres
          initialDelaySeconds: 5
          periodSeconds: 5
      volumes:
      - name: config
        configMap:
          name: postgresql-config
  volumeClaimTemplates:
  - metadata:
      name: data
    spec:
      accessModes: ["ReadWriteOnce"]
      storageClassName: managed-premium-retain
      resources:
        requests:
          storage: 256Gi
```

---

## Redis High Availability (Sentinel)

### Redis Sentinel 架構

```
┌─────────────────────────────────────────────────────────┐
│ Redis Sentinel Cluster                                  │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │ Master       │  │ Replica 1    │  │ Replica 2    │  │
│  │ (Zone 1)     │◄─┤ (Zone 2)     │◄─┤ (Zone 3)     │  │
│  │ Read/Write   │  │ Read-Only    │  │ Read-Only    │  │
│  └──────┬───────┘  └──────────────┘  └──────────────┘  │
│         │                                               │
│         │ Monitored by                                  │
│         ▼                                               │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │ Sentinel 1   │  │ Sentinel 2   │  │ Sentinel 3   │  │
│  │ (Zone 1)     ├──┤ (Zone 2)     ├──┤ (Zone 3)     │  │
│  │ Quorum: 2    │  │              │  │              │  │
│  └──────────────┘  └──────────────┘  └──────────────┘  │
│                                                         │
│  Auto-Failover:                                         │
│  1. Master failure detected by Sentinels (30s)          │
│  2. Quorum reached (2 out of 3 Sentinels)               │
│  3. Replica promoted to Master (<10s)                   │
│  4. Clients redirected to new Master                    │
└─────────────────────────────────────────────────────────┘
```

### Redis Master 配置

```conf
# redis-master.conf
bind 0.0.0.0
port 6379
protected-mode yes
requirepass strong_redis_password

# Persistence
save 900 1
save 300 10
save 60 10000
appendonly yes
appendfsync everysec

# Replication
replica-announce-ip redis-master.aiagent-prod.svc.cluster.local
replica-announce-port 6379

# Memory
maxmemory 4gb
maxmemory-policy allkeys-lru

# Performance
tcp-backlog 511
timeout 300
tcp-keepalive 300
```

### Redis Replica 配置

```conf
# redis-replica.conf
bind 0.0.0.0
port 6379
requirepass strong_redis_password

# Replication
replicaof redis-master.aiagent-prod.svc.cluster.local 6379
masterauth strong_redis_password
replica-read-only yes

# Same persistence and performance settings as Master
appendonly yes
appendfsync everysec
maxmemory 4gb
maxmemory-policy allkeys-lru
```

### Sentinel 配置

```conf
# sentinel.conf
port 26379
bind 0.0.0.0

# Monitor Master
sentinel monitor mymaster redis-master.aiagent-prod.svc.cluster.local 6379 2  # Quorum = 2
sentinel auth-pass mymaster strong_redis_password
sentinel down-after-milliseconds mymaster 30000  # 30s 無響應視為 Down
sentinel parallel-syncs mymaster 1
sentinel failover-timeout mymaster 180000  # 3 分鐘 Failover 超時

# Notification (可選)
sentinel notification-script mymaster /usr/local/bin/notify.sh
sentinel client-reconfig-script mymaster /usr/local/bin/reconfig.sh
```

### Kubernetes Redis + Sentinel 部署

```yaml
# k8s/base/redis-sentinel.yaml
apiVersion: apps/v1
kind: StatefulSet
metadata:
  name: redis
  namespace: aiagent-prod
spec:
  serviceName: redis
  replicas: 3
  selector:
    matchLabels:
      app: redis
  template:
    metadata:
      labels:
        app: redis
    spec:
      affinity:
        podAntiAffinity:
          requiredDuringSchedulingIgnoredDuringExecution:
          - labelSelector:
              matchLabels:
                app: redis
            topologyKey: topology.kubernetes.io/zone
      containers:
      # Redis Container
      - name: redis
        image: redis:7
        command:
        - redis-server
        - /etc/redis/redis.conf
        ports:
        - containerPort: 6379
          name: redis
        volumeMounts:
        - name: redis-config
          mountPath: /etc/redis
        - name: data
          mountPath: /data
        livenessProbe:
          exec:
            command:
            - redis-cli
            - ping
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          exec:
            command:
            - redis-cli
            - ping
          initialDelaySeconds: 5
          periodSeconds: 5

      # Sentinel Container
      - name: sentinel
        image: redis:7
        command:
        - redis-sentinel
        - /etc/redis/sentinel.conf
        ports:
        - containerPort: 26379
          name: sentinel
        volumeMounts:
        - name: sentinel-config
          mountPath: /etc/redis

      volumes:
      - name: redis-config
        configMap:
          name: redis-config
      - name: sentinel-config
        configMap:
          name: sentinel-config

  volumeClaimTemplates:
  - metadata:
      name: data
    spec:
      accessModes: ["ReadWriteOnce"]
      storageClassName: managed-premium
      resources:
        requests:
          storage: 20Gi
```

---

## Load Balancing 與 Auto-Scaling

### Azure Application Gateway 配置

```bash
# 創建 Application Gateway (WAF v2 SKU)
az network application-gateway create \
  --name aiagent-appgw \
  --resource-group aiagent-prod-rg \
  --location eastus \
  --sku WAF_v2 \
  --capacity 2 \  # 最小 2 個實例
  --max-capacity 10 \  # 自動擴展到 10 個實例
  --vnet-name aiagent-vnet \
  --subnet appgw-subnet \
  --public-ip-address aiagent-appgw-ip \
  --http-settings-cookie-based-affinity Disabled \
  --frontend-port 443 \
  --http-settings-port 80 \
  --http-settings-protocol Http \
  --priority 100

# 配置 Health Probe
az network application-gateway probe create \
  --gateway-name aiagent-appgw \
  --resource-group aiagent-prod-rg \
  --name api-health-probe \
  --protocol Http \
  --host-name-from-http-settings true \
  --path /health/ready \
  --interval 30 \
  --timeout 30 \
  --unhealthy-threshold 3
```

### Horizontal Pod Autoscaler (HPA)

```yaml
# k8s/base/api-hpa.yaml
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: api-hpa
  namespace: aiagent-prod
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: api-deployment
  minReplicas: 6   # 最少 6 個副本 (每個 Zone 2 個)
  maxReplicas: 24  # 最多 24 個副本 (每個 Zone 8 個)
  metrics:
  # CPU 利用率
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 70  # 目標 70% CPU 利用率
  # Memory 利用率
  - type: Resource
    resource:
      name: memory
      target:
        type: Utilization
        averageUtilization: 80  # 目標 80% Memory 利用率
  # 自定義指標: HTTP 請求速率
  - type: Pods
    pods:
      metric:
        name: http_requests_per_second
      target:
        type: AverageValue
        averageValue: "100"  # 每個 Pod 處理 100 req/s
  behavior:
    scaleDown:
      stabilizationWindowSeconds: 300  # 5 分鐘穩定期
      policies:
      - type: Percent
        value: 50  # 每次最多縮減 50%
        periodSeconds: 60
    scaleUp:
      stabilizationWindowSeconds: 0  # 立即擴展
      policies:
      - type: Percent
        value: 100  # 每次最多擴展 100% (翻倍)
        periodSeconds: 15
      - type: Pods
        value: 4  # 每次最多增加 4 個 Pod
        periodSeconds: 15
      selectPolicy: Max
```

### Cluster Autoscaler

```bash
# 啟用 Cluster Autoscaler (已在 AKS 創建時配置)
az aks update \
  --resource-group aiagent-prod-rg \
  --name aiagent-aks-prod \
  --enable-cluster-autoscaler \
  --min-count 6 \   # 最少 6 個節點
  --max-count 18    # 最多 18 個節點 (每個 Zone 最多 6 個)
```

---

## Stateless Application Design

### 無狀態服務設計原則

**1. 無本地狀態存儲**:
- 所有狀態存儲在外部系統 (PostgreSQL, Redis, Blob Storage)
- Pod 可以被隨時終止和重建而不丟失數據

**2. Session 外部化**:
```csharp
// Program.cs
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "aiagent:sessions:";
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});
```

**3. 配置外部化**:
```csharp
// 使用 Azure App Configuration
builder.Configuration.AddAzureAppConfiguration(options =>
{
    options.Connect(builder.Configuration["AppConfig:ConnectionString"])
           .ConfigureRefresh(refresh =>
           {
               refresh.Register("Settings:*", refreshAll: true)
                      .SetCacheExpiration(TimeSpan.FromMinutes(5));
           });
});
```

---

## 檢查清單

### High Availability 實施檢查清單

- [ ] **Multi-Zone Deployment**
  - [ ] AKS 集群跨 3 個可用區部署
  - [ ] Pod Anti-Affinity 配置正確
  - [ ] Pod Disruption Budget 已設置

- [ ] **Database HA**
  - [ ] PostgreSQL Streaming Replication 正常運行
  - [ ] Synchronous Standby 配置正確 (0 數據丟失)
  - [ ] Asynchronous Standby (DR Region) 延遲 < 5 分鐘

- [ ] **Redis HA**
  - [ ] Redis Sentinel 3 節點運行正常
  - [ ] Auto-Failover 測試通過
  - [ ] 客戶端自動重連配置正確

- [ ] **Load Balancing**
  - [ ] Azure Application Gateway 配置正確
  - [ ] Health Probe 正常工作
  - [ ] SSL/TLS 證書已配置

- [ ] **Auto-Scaling**
  - [ ] Horizontal Pod Autoscaler 正常工作
  - [ ] Cluster Autoscaler 正常擴展節點
  - [ ] 擴展策略經過測試

- [ ] **Stateless Design**
  - [ ] Session 存儲在 Redis
  - [ ] 配置存儲在 Azure App Configuration
  - [ ] 無本地狀態文件

---

## 總結

本文檔提供了完整的高可用性架構設計,涵蓋:

✅ **Multi-Zone Deployment** (跨 3 個可用區,年度可用性 99.9%+)
✅ **Database Replication** (Synchronous + Asynchronous Standby)
✅ **Redis Sentinel** (自動故障轉移,< 10 秒)
✅ **Load Balancing** (Azure Application Gateway + Health Probes)
✅ **Auto-Scaling** (HPA + Cluster Autoscaler)
✅ **Stateless Design** (Session 外部化,配置外部化)

透過本指南,運維團隊可以實現:
- **99.9% 可用性**: 年度停機時間 < 8.76 小時
- **自動故障恢復**: 無需人工介入
- **彈性伸縮**: 根據負載自動調整資源
- **零停機部署**: Rolling Update 策略
