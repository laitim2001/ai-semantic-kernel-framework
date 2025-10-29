# 性能與擴展性策略

本文檔定義 Semantic Kernel Agentic Framework 的性能優化和擴展性設計。

## 目標

### 性能目標（Performance SLA）

```yaml
響應時間:
  API Gateway:
    P50: < 100ms
    P95: < 300ms
    P99: < 500ms

  Agent 執行:
    P50: < 2 秒
    P95: < 5 秒
    P99: < 10 秒

  Code Interpreter:
    P50: < 3 秒 (含 Container 獲取)
    P95: < 8 秒
    P99: < 12 秒

吞吐量:
  MVP (單機):
    API QPS: 100-200
    Agent 並發執行: 10-20
    Code Interpreter QPS: 5-10

  Phase 2 (Kubernetes):
    API QPS: 1000-2000
    Agent 並發執行: 100-200
    Code Interpreter QPS: 50-100

可用性:
  目標: 99.9% (每月停機時間 <43.8 分鐘)
  MVP: 99.5% (可接受)
  Phase 2: 99.9% (生產級)
```

---

## 1. 緩存策略

### 1.1 Redis 緩存架構

```yaml
緩存層次:
  L1 - 內存緩存 (Memory Cache):
    用途: 高頻訪問的配置數據
    TTL: 5 分鐘
    大小: 100MB per instance
    實現: Microsoft.Extensions.Caching.Memory

  L2 - 分布式緩存 (Redis):
    用途: Agent 狀態、會話數據、API 響應
    TTL: 30 分鐘 - 24 小時
    大小: 2GB (MVP) → 10GB (Phase 2)
    實現: StackExchange.Redis
```

### 1.2 緩存鍵設計

```csharp
// 緩存鍵命名規範
public static class CacheKeys
{
    // Agent 狀態：agent:state:{agentId}
    public static string AgentState(string agentId) => $"agent:state:{agentId}";

    // Agent 配置：agent:config:{agentId}
    public static string AgentConfig(string agentId) => $"agent:config:{agentId}";

    // 用戶會話：session:{userId}:{sessionId}
    public static string UserSession(string userId, string sessionId) => $"session:{userId}:{sessionId}";

    // Plugin Schema：plugin:schema:{pluginId}
    public static string PluginSchema(string pluginId) => $"plugin:schema:{pluginId}";

    // API 響應緩存：api:{endpoint}:{hash(params)}
    public static string ApiResponse(string endpoint, string paramsHash) => $"api:{endpoint}:{paramsHash}";
}
```

### 1.3 緩存策略實現

```csharp
// Multi-Layer Caching
public class MultiLayerCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _redisCache;

    public async Task<T> GetOrSetAsync<T>(
        string key,
        Func<Task<T>> factory,
        TimeSpan? memoryCacheDuration = null,
        TimeSpan? redisCacheDuration = null)
    {
        // 1. 嘗試從 L1 (Memory) 獲取
        if (_memoryCache.TryGetValue(key, out T cachedValue))
        {
            return cachedValue;
        }

        // 2. 嘗試從 L2 (Redis) 獲取
        var redisValue = await _redisCache.GetStringAsync(key);
        if (redisValue != null)
        {
            var value = JsonSerializer.Deserialize<T>(redisValue);

            // 回填到 L1
            _memoryCache.Set(key, value, memoryCacheDuration ?? TimeSpan.FromMinutes(5));

            return value;
        }

        // 3. Cache Miss，從數據源獲取
        var newValue = await factory();

        // 4. 寫入 L2 (Redis)
        await _redisCache.SetStringAsync(
            key,
            JsonSerializer.Serialize(newValue),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = redisCacheDuration ?? TimeSpan.FromMinutes(30)
            }
        );

        // 5. 寫入 L1 (Memory)
        _memoryCache.Set(key, newValue, memoryCacheDuration ?? TimeSpan.FromMinutes(5));

        return newValue;
    }
}
```

### 1.4 緩存失效策略

```yaml
主動失效:
  - Agent 配置更新 → 刪除 agent:config:{agentId}
  - Agent 狀態變更 → 更新 agent:state:{agentId}（不刪除）
  - Plugin 更新 → 刪除 plugin:schema:{pluginId}

被動失效:
  - TTL 過期自動失效
  - Redis 內存不足時 LRU 淘汰

預熱策略:
  - 系統啟動時：預加載所有 Plugin Schema
  - 用戶登錄時：預加載用戶的 Agent 配置
  - 定期任務：每 10 分鐘預加載熱點數據
```

---

## 2. 數據庫優化策略

### 2.1 連接池配置

```yaml
Connection Pooling (Npgsql):
  MinPoolSize: 5
  MaxPoolSize: 100
  ConnectionLifetime: 300 # 5 分鐘
  ConnectionIdleLifetime: 60 # 1 分鐘
  Timeout: 30

Connection Pooler (PgBouncer):
  pool_mode: transaction # 事務級別池化
  max_client_conn: 1000
  default_pool_size: 25
  reserve_pool_size: 5
```

### 2.2 查詢優化

#### 批量查詢

```csharp
// ❌ 壞實踐：N+1 查詢
foreach (var agentId in agentIds)
{
    var agent = await _db.Agents.FindAsync(agentId); // N 次查詢
}

// ✅ 好實踐：批量查詢
var agents = await _db.Agents
    .Where(a => agentIds.Contains(a.Id))
    .ToListAsync(); // 1 次查詢
```

#### 分頁查詢

```csharp
// ❌ 壞實踐：OFFSET 分頁（深分頁性能差）
var agents = await _db.Agents
    .OrderBy(a => a.CreatedAt)
    .Skip(1000) // OFFSET 1000
    .Take(20)
    .ToListAsync();

// ✅ 好實踐：Keyset 分頁（性能穩定）
var agents = await _db.Agents
    .Where(a => a.CreatedAt < lastCreatedAt) // Keyset
    .OrderByDescending(a => a.CreatedAt)
    .Take(20)
    .ToListAsync();
```

#### 投影查詢

```csharp
// ❌ 壞實踐：查詢所有列
var agents = await _db.Agents.ToListAsync();

// ✅ 好實踐：只查詢需要的列
var agents = await _db.Agents
    .Select(a => new { a.Id, a.Name, a.Status })
    .ToListAsync();
```

### 2.3 索引優化

#### 復合索引

```sql
-- 支持多條件查詢
CREATE INDEX idx_agents_user_status_created
ON agents(user_id, status, created_at DESC);

-- 查詢示例
SELECT * FROM agents
WHERE user_id = 'xxx' AND status = 'active'
ORDER BY created_at DESC;
```

#### 部分索引

```sql
-- 只索引活躍 Agent（減少索引大小）
CREATE INDEX idx_agents_active
ON agents(user_id, created_at DESC)
WHERE status = 'active';
```

#### 覆蓋索引

```sql
-- 避免回表（INCLUDE 常查詢列）
CREATE INDEX idx_agents_user_id_covering
ON agents(user_id)
INCLUDE (name, description, status);
```

### 2.4 讀寫分離（Phase 2）

```yaml
架構:
  Primary (寫入):
    - 處理所有 INSERT, UPDATE, DELETE
    - 同步複製到 Standby

  Standby (只讀):
    - 處理所有 SELECT 查詢
    - 自動故障轉移（Failover）

連接字符串:
  Write: "Host=postgres-primary;Database=agentic"
  Read: "Host=postgres-standby-1,postgres-standby-2;Database=agentic;Target Session Attributes=any"

應用層路由:
  - Entity Framework Core Interceptor
  - 自動將 SELECT 路由到 Read Replica
  - 寫入後短時間內讀取 → 路由到 Primary（避免複製延遲）
```

---

## 3. 消息隊列優化

### 3.1 Phase 1: MediatR（內存事件總線）

```csharp
// 優化：並行處理 Notification
public class MediatRConfiguration
{
    public void Configure(IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);

            // 啟用並行 Notification 處理
            cfg.NotificationPublisherType = typeof(ParallelNoWaitPublisher);
        });
    }
}

// 並行 Notification Publisher
public class ParallelNoWaitPublisher : INotificationPublisher
{
    public async Task Publish(
        IEnumerable<NotificationHandlerExecutor> handlerExecutors,
        INotification notification,
        CancellationToken cancellationToken)
    {
        // 並行執行所有 Handler（不等待）
        var tasks = handlerExecutors
            .Select(executor => Task.Run(() => executor.HandlerCallback(notification, cancellationToken), cancellationToken));

        await Task.WhenAll(tasks);
    }
}
```

### 3.2 Phase 2: Azure Service Bus

#### 消息批量處理

```csharp
// ❌ 壞實踐：逐個發送消息
foreach (var message in messages)
{
    await _sender.SendMessageAsync(message);
}

// ✅ 好實踐：批量發送
await _sender.SendMessagesAsync(messages);
```

#### 會話與有序處理

```csharp
// 確保同一 Agent 的消息有序處理
var message = new ServiceBusMessage(messageBody)
{
    SessionId = agentId, // 使用 Session 保證順序
    MessageId = Guid.NewGuid().ToString(),
    TimeToLive = TimeSpan.FromMinutes(5)
};

await _sender.SendMessageAsync(message);
```

#### 死信隊列處理

```csharp
// 監控死信隊列並告警
var deadLetterReceiver = _client.CreateReceiver(
    queueName,
    new ServiceBusReceiverOptions { SubQueue = SubQueue.DeadLetter }
);

await foreach (var message in deadLetterReceiver.ReceiveMessagesAsync())
{
    // 記錄錯誤並告警
    _logger.LogError($"Dead letter message: {message.MessageId}, Reason: {message.DeadLetterReason}");

    // 可選：重試或人工介入
}
```

---

## 4. 水平擴展策略

### 4.1 無狀態 API 設計

```yaml
原則:
  - API 實例無狀態（所有狀態存儲在 Redis/PostgreSQL）
  - 支持任意數量實例水平擴展
  - 負載均衡器隨機路由（無 Sticky Session）

實現:
  - Agent 狀態 → Redis + PostgreSQL
  - 會話數據 → Redis
  - 文件上傳 → Azure Blob Storage
  - 分布式鎖 → Redis (RedLock)
```

### 4.2 Kubernetes 自動伸縮

#### HPA（Horizontal Pod Autoscaler）

```yaml
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: agent-orchestrator-hpa
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: agent-orchestrator
  minReplicas: 3
  maxReplicas: 20
  metrics:
    - type: Resource
      resource:
        name: cpu
        target:
          type: Utilization
          averageUtilization: 70 # CPU 使用率 >70% 時擴展
    - type: Resource
      resource:
        name: memory
        target:
          type: Utilization
          averageUtilization: 80
  behavior:
    scaleDown:
      stabilizationWindowSeconds: 300 # 5 分鐘穩定期
      policies:
        - type: Percent
          value: 50 # 每次最多縮容 50%
          periodSeconds: 60
    scaleUp:
      stabilizationWindowSeconds: 60
      policies:
        - type: Percent
          value: 100 # 每次最多擴容 100%
          periodSeconds: 30
```

#### VPA（Vertical Pod Autoscaler）

```yaml
apiVersion: autoscaling.k8s.io/v1
kind: VerticalPodAutoscaler
metadata:
  name: agent-orchestrator-vpa
spec:
  targetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: agent-orchestrator
  updatePolicy:
    updateMode: "Auto" # 自動調整資源請求
  resourcePolicy:
    containerPolicies:
      - containerName: orchestrator
        minAllowed:
          cpu: "500m"
          memory: "512Mi"
        maxAllowed:
          cpu: "4"
          memory: "8Gi"
```

### 4.3 數據庫水平擴展

#### Sharding 策略（Phase 3）

```yaml
分片鍵: user_id (按用戶分片)

分片規則:
  Shard 0: user_id % 4 = 0
  Shard 1: user_id % 4 = 1
  Shard 2: user_id % 4 = 2
  Shard 3: user_id % 4 = 3

路由層:
  工具: Citus (PostgreSQL 分布式擴展)
  或: 應用層路由（Sharding Middleware）

限制:
  - 跨分片查詢困難（避免跨用戶查詢）
  - 跨分片事務複雜（盡量避免）
```

---

## 5. 性能監控與優化

### 5.1 關鍵性能指標（KPI）

```yaml
應用層指標:
  - API 響應時間 (P50, P95, P99)
  - Agent 執行時間分布
  - Plugin 調用成功率
  - Code Interpreter 容器池使用率

基礎設施指標:
  - CPU 使用率
  - Memory 使用率
  - Network I/O
  - Disk I/O

數據庫指標:
  - 查詢響應時間
  - 連接池使用率
  - 慢查詢數量 (>1 秒)
  - 索引命中率

緩存指標:
  - Redis 命中率 (目標: >90%)
  - Redis 內存使用率
  - 緩存響應時間
```

### 5.2 Application Insights 集成

```csharp
// Telemetry 配置
public class TelemetryConfiguration
{
    public void Configure(IServiceCollection services)
    {
        services.AddApplicationInsightsTelemetry(options =>
        {
            options.EnableAdaptiveSampling = true;
            options.EnableQuickPulseMetricStream = true;
        });

        // 自定義遙測初始化器
        services.AddSingleton<ITelemetryInitializer, CustomTelemetryInitializer>();
    }
}

// 自定義遙測
public class AgentExecutionTelemetry
{
    private readonly TelemetryClient _telemetry;

    public void TrackAgentExecution(string agentId, long executionTimeMs, bool success)
    {
        var metrics = new Dictionary<string, double>
        {
            { "ExecutionTime", executionTimeMs },
            { "Success", success ? 1 : 0 }
        };

        var properties = new Dictionary<string, string>
        {
            { "AgentId", agentId }
        };

        _telemetry.TrackEvent("AgentExecution", properties, metrics);
    }
}
```

### 5.3 Distributed Tracing（分布式追蹤）

```csharp
// OpenTelemetry 配置
public class OpenTelemetryConfiguration
{
    public void Configure(IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .WithTracing(builder =>
            {
                builder
                    .AddAspNetCoreInstrumentation() // HTTP 請求追蹤
                    .AddHttpClientInstrumentation() // HTTP 客戶端追蹤
                    .AddNpgsql() // PostgreSQL 追蹤
                    .AddRedisInstrumentation() // Redis 追蹤
                    .AddSource("SemanticKernel.*") // Semantic Kernel 追蹤
                    .AddAzureMonitorTraceExporter(options =>
                    {
                        options.ConnectionString = "InstrumentationKey=...";
                    });
            });
    }
}

// 自定義 Span
using var activity = ActivitySource.StartActivity("AgentExecution");
activity?.SetTag("agent.id", agentId);
activity?.SetTag("agent.model", "gpt-4o");

try
{
    var result = await ExecuteAgentAsync(agentId);
    activity?.SetTag("agent.result", "success");
    return result;
}
catch (Exception ex)
{
    activity?.SetTag("agent.result", "failed");
    activity?.SetTag("error.message", ex.Message);
    throw;
}
```

---

## 6. 負載測試計劃

### 6.1 測試場景

```yaml
場景 1: API 壓力測試
  目標: 驗證 API Gateway 性能
  工具: k6 或 Artillery
  配置:
    Virtual Users: 100
    Duration: 5 分鐘
    Endpoints:
      - GET /api/agents (50%)
      - POST /api/agents/{id}/execute (40%)
      - GET /api/executions/{id} (10%)
  成功標準:
    - P95 < 300ms
    - 錯誤率 < 1%

場景 2: Agent 並發執行
  目標: 驗證 Agent Orchestrator 並發能力
  工具: 自定義腳本
  配置:
    並發 Agent: 20
    每個 Agent 執行時間: 5-10 秒
    持續時間: 10 分鐘
  成功標準:
    - 所有 Agent 成功執行
    - P95 執行時間 < 12 秒

場景 3: Code Interpreter 壓力測試
  目標: 驗證容器池性能
  配置:
    並發代碼執行: 10
    代碼複雜度: 簡單 → 複雜
    持續時間: 5 分鐘
  成功標準:
    - P95 < 10 秒
    - 容器池無耗盡
```

### 6.2 k6 測試腳本範例

```javascript
import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
  stages: [
    { duration: '1m', target: 50 }, // Ramp-up
    { duration: '3m', target: 100 }, // Peak
    { duration: '1m', target: 0 }, // Ramp-down
  ],
  thresholds: {
    http_req_duration: ['p(95)<300'], // P95 < 300ms
    http_req_failed: ['rate<0.01'], // 錯誤率 < 1%
  },
};

export default function () {
  // 1. 獲取 Agent 列表
  let agentsRes = http.get('https://api.example.com/v1/agents', {
    headers: { 'Authorization': 'Bearer YOUR_TOKEN' },
  });

  check(agentsRes, {
    'agents list status is 200': (r) => r.status === 200,
  });

  sleep(1);

  // 2. 執行 Agent
  let payload = JSON.stringify({
    input: '分析這個 CSV 文件',
  });

  let executeRes = http.post('https://api.example.com/v1/agents/xxx/execute', payload, {
    headers: {
      'Content-Type': 'application/json',
      'Authorization': 'Bearer YOUR_TOKEN',
    },
  });

  check(executeRes, {
    'agent execution status is 200': (r) => r.status === 200,
    'execution time < 5s': (r) => r.timings.duration < 5000,
  });

  sleep(2);
}
```

---

## 7. 成本優化策略

### 7.1 Azure 資源優化

```yaml
計算資源:
  MVP (Phase 1):
    - Azure Container Instances: $20-50/月
    - Azure Database for PostgreSQL Basic: $30/月
    - Azure Cache for Redis Basic: $15/月
    - Azure Blob Storage: $5/月
    總計: ~$70-100/月

  Phase 2 (Kubernetes):
    - AKS Cluster (3 nodes, Standard_D4s_v3): $350/月
    - Azure Database for PostgreSQL Flexible HA: $200/月
    - Azure Cache for Redis Standard: $100/月
    - Azure Service Bus Standard: $10/月
    - Azure Blob Storage: $20/月
    總計: ~$680/月

優化建議:
  - 使用 Reserved Instances（節省 30-50%）
  - 非生產環境使用 Spot Instances
  - 自動伸縮避免資源浪費
  - 定期審查未使用資源
```

### 7.2 LLM 成本優化

```yaml
策略:
  1. Prompt 緩存:
     - 緩存常見問題的響應
     - 使用 Semantic Cache（向量相似度檢索）

  2. 模型選擇:
     - 簡單任務使用 GPT-4o-mini
     - 複雜任務使用 GPT-4o
     - 動態路由到最合適的模型

  3. Token 優化:
     - 精簡 Prompt（減少不必要的指令）
     - 使用 Function Calling（而非長 Prompt）
     - 壓縮對話歷史（摘要舊消息）

  4. 批量處理:
     - 合併多個請求（Batch API）
     - 異步處理非緊急任務

預期成本:
  - MVP: $100-200/月 (1000 次 Agent 執行)
  - Phase 2: $500-1000/月 (10000 次 Agent 執行)
```

---

## 8. 擴展路徑（MVP → Phase 2）

### MVP (Month 1-8)

```yaml
架構:
  - 單機部署（Docker Compose）
  - MediatR 內存事件總線
  - 單機 Redis + PostgreSQL
  - Container Pool (5 個容器)

限制:
  - 無高可用性（單點故障）
  - 垂直擴展為主
  - 並發能力有限（~20 並發 Agent）

優勢:
  - 實現簡單
  - 成本低（~$100/月）
  - 快速驗證 MVP
```

### Phase 2 (Month 9-18)

```yaml
架構:
  - Kubernetes 集群部署
  - Azure Service Bus 消息隊列
  - PostgreSQL HA (Primary + Standby)
  - Redis Cluster (分片)
  - Container Pool 自動伸縮 (5-50 個容器)

增強:
  - 高可用性（多副本）
  - 水平擴展（HPA）
  - 並發能力強（~200 並發 Agent）
  - 支持跨地區部署

成本:
  - ~$680/月（基礎設施）
  - ~$500-1000/月（LLM 調用）
```

### 遷移策略

```yaml
Step 1: 準備工作 (Week 25-26)
  - 創建 Kubernetes 集群
  - 部署 PostgreSQL HA
  - 部署 Redis Cluster
  - 配置 Azure Service Bus

Step 2: 並行部署 (Week 27-28)
  - MVP 和 Phase 2 環境並行運行
  - 使用 Feature Flag 控制流量
  - 雙寫數據（確保一致性）

Step 3: 灰度發布 (Week 29-30)
  - 10% 流量 → Phase 2
  - 監控錯誤率和性能
  - 逐步增加到 50% → 100%

Step 4: 清理與優化 (Week 31-32)
  - 關閉 MVP 環境
  - 優化 Kubernetes 配置
  - 性能調優和成本優化
```

---

## 9. 性能優化檢查清單

### 開發階段

- [ ] 使用異步 I/O（async/await）
- [ ] 避免阻塞調用（Task.Run for CPU-bound）
- [ ] 實現緩存（Memory + Redis）
- [ ] 批量數據庫操作
- [ ] 使用連接池（Database, HTTP Client）
- [ ] 啟用 Response Compression
- [ ] 優化序列化（System.Text.Json）

### 數據庫

- [ ] 創建適當索引
- [ ] 使用覆蓋索引避免回表
- [ ] 分析慢查詢並優化
- [ ] 使用批量插入/更新
- [ ] 實現 Keyset 分頁
- [ ] 配置連接池
- [ ] 定期 VACUUM ANALYZE

### 緩存

- [ ] 實現多層緩存（Memory + Redis）
- [ ] 設置合理的 TTL
- [ ] 監控緩存命中率（目標 >90%）
- [ ] 實現緩存預熱
- [ ] 處理緩存雪崩/穿透
- [ ] 使用 Redis Pipeline 減少網絡往返

### 基礎設施

- [ ] 配置 CDN（靜態資源）
- [ ] 啟用 HTTP/2
- [ ] 配置負載均衡器
- [ ] 實現健康檢查
- [ ] 設置自動伸縮
- [ ] 監控資源使用率
- [ ] 定期負載測試

---

## 總結

本文檔定義了完整的性能與擴展性策略：

1. **緩存策略**：L1 Memory + L2 Redis，命中率 >90%
2. **數據庫優化**：連接池、索引、讀寫分離、分片
3. **消息隊列**：MediatR → Azure Service Bus 平滑遷移
4. **水平擴展**：無狀態 API + Kubernetes HPA
5. **性能監控**：Application Insights + OpenTelemetry
6. **負載測試**：k6 壓力測試計劃
7. **成本優化**：Reserved Instances + LLM 優化
8. **擴展路徑**：MVP → Phase 2 灰度遷移

這些策略確保系統從 MVP 到 Phase 2 的平滑過渡，並滿足生產級性能和可用性要求。
