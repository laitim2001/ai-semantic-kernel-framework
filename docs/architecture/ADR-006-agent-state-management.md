# ADR-006: Agent 狀態管理策略

## 狀態

✅ **已接受** - 2025-10-28

## 背景

Agent 在執行過程中需要維護狀態（例如：對話上下文、執行進度、Plugin 調用歷史）。我們需要決定如何管理這些狀態，以平衡性能、可靠性和可擴展性。

## 決策

採用 **Hybrid（混合模式）**：

```yaml
策略:
  短期執行 Agent (<5分鐘):
    狀態存儲: Redis (內存緩存)
    TTL: 30 分鐘
    持久化: 僅在執行完成時寫入 PostgreSQL

  長期運行 Agent (>5分鐘):
    狀態存儲: PostgreSQL (持久化)
    實時更新: 每 30 秒或關鍵節點
    緩存: Redis 緩存最近訪問的狀態

  關鍵狀態:
    雙寫: Redis + PostgreSQL (確保不丟失)
    包括: Agent 配置、用戶數據、敏感信息
```

## 架構設計

### 狀態層次結構

```csharp
// 狀態抽象層
public interface IAgentStateStore
{
    Task SaveStateAsync(string agentId, AgentState state, bool persistent = false);
    Task<AgentState> LoadStateAsync(string agentId);
    Task DeleteStateAsync(string agentId);
}

// Agent 狀態結構
public class AgentState
{
    public string AgentId { get; set; }
    public string SessionId { get; set; }
    public AgentStatus Status { get; set; } // Running, Paused, Completed, Failed

    // 對話上下文
    public List<Message> ConversationHistory { get; set; }
    public Dictionary<string, object> Variables { get; set; }

    // 執行狀態
    public string CurrentTaskId { get; set; }
    public List<PluginInvocation> PluginHistory { get; set; }

    // 元數據
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
    public int ExecutionTimeSeconds { get; set; }
}

// Hybrid 實現
public class HybridAgentStateStore : IAgentStateStore
{
    private readonly IDistributedCache _redis; // StackExchange.Redis
    private readonly IAgentRepository _repository; // PostgreSQL

    public async Task SaveStateAsync(string agentId, AgentState state, bool persistent = false)
    {
        // 1. 總是寫入 Redis（快速訪問）
        await _redis.SetStringAsync(
            key: $"agent:state:{agentId}",
            value: JsonSerializer.Serialize(state),
            options: new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) }
        );

        // 2. 如果是關鍵狀態或長期運行，寫入 PostgreSQL
        if (persistent || state.ExecutionTimeSeconds > 300) // 5分鐘
        {
            await _repository.UpsertAgentStateAsync(agentId, state);
        }
    }

    public async Task<AgentState> LoadStateAsync(string agentId)
    {
        // 1. 優先從 Redis 加載（低延遲）
        var cachedState = await _redis.GetStringAsync($"agent:state:{agentId}");
        if (cachedState != null)
        {
            return JsonSerializer.Deserialize<AgentState>(cachedState);
        }

        // 2. Redis 未命中，從 PostgreSQL 加載
        var state = await _repository.GetAgentStateAsync(agentId);

        // 3. 回填到 Redis
        if (state != null)
        {
            await _redis.SetStringAsync(
                key: $"agent:state:{agentId}",
                value: JsonSerializer.Serialize(state),
                options: new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) }
            );
        }

        return state;
    }
}
```

### 狀態生命週期

```
┌─────────────────────────────────────────────────────────┐
│  Agent 狀態生命週期                                      │
├─────────────────────────────────────────────────────────┤
│  1. 創建 (Created)                                       │
│     → Redis: ✅ 初始狀態                                 │
│     → PostgreSQL: ✅ 持久化 (agentId, sessionId)        │
├─────────────────────────────────────────────────────────┤
│  2. 執行中 (Running)                                     │
│     → Redis: ✅ 每次狀態變更                             │
│     → PostgreSQL: ⏱️ 每 30 秒或關鍵節點                 │
├─────────────────────────────────────────────────────────┤
│  3. 暫停 (Paused)                                        │
│     → Redis: ✅ 保留狀態 (30分鐘 TTL)                    │
│     → PostgreSQL: ✅ 立即持久化                          │
├─────────────────────────────────────────────────────────┤
│  4. 完成 (Completed / Failed)                            │
│     → Redis: ⏱️ 保留 30 分鐘後自動清理                   │
│     → PostgreSQL: ✅ 最終狀態持久化                      │
│     → 歷史記錄: ✅ 歸檔到 agent_execution_history 表     │
└─────────────────────────────────────────────────────────┘
```

## 理由

### 為什麼選擇 Hybrid？

1. **性能優化**：
   - Redis 提供低延遲訪問（<1ms）
   - 減少 PostgreSQL 的寫入壓力（不需要每次都寫 DB）

2. **可靠性保證**：
   - 關鍵狀態持久化到 PostgreSQL（不怕 Redis 重啟）
   - 雙寫機制確保數據不丟失

3. **成本優化**：
   - 短期 Agent 僅使用 Redis（節省 DB 寫入）
   - 長期 Agent 才持久化（避免浪費存儲）

4. **擴展性**：
   - Redis 支持分片（未來可擴展到 Redis Cluster）
   - PostgreSQL 支持讀寫分離（未來可擴展到主從架構）

### 為什麼不選擇純 Stateless？

- **LLM 對話需要上下文**：每次都從頭開始會影響 Agent 質量
- **Plugin 調用歷史重要**：需要知道之前調用過哪些 Plugin（避免重複）
- **性能考量**：每次從 DB 重建狀態會增加延遲

### 為什麼不選擇純 Stateful？

- **容錯性差**：API 實例掛掉，狀態丟失
- **水平擴展困難**：需要 Sticky Session，負載均衡複雜
- **不符合 Cloud-Native**：Kubernetes 重啟 Pod 會丟失狀態

## 影響

### 正面影響

- ✅ **低延遲**：大部分請求從 Redis 讀取（<1ms）
- ✅ **高可靠性**：關鍵狀態持久化到 PostgreSQL
- ✅ **易擴展**：Redis 和 PostgreSQL 都支持水平擴展
- ✅ **成本優化**：短期 Agent 不浪費 DB 寫入

### 負面影響

- ⚠️ **實現複雜度**：需要管理兩套存儲（Redis + PostgreSQL）
- ⚠️ **數據一致性**：需要處理 Redis 和 PostgreSQL 之間的同步
- ⚠️ **調試困難**：需要檢查兩個地方的狀態

### 緩解措施

1. **統一抽象層**：`IAgentStateStore` 隱藏底層實現細節
2. **自動同步機制**：`HybridAgentStateStore` 自動管理 Redis ↔ PostgreSQL
3. **監控與日誌**：記錄每次狀態讀寫操作（便於調試）

## 遷移策略

### Phase 1 (MVP)
- 實現 `HybridAgentStateStore`
- 使用單機 Redis（Azure Redis Cache Basic 250MB）
- 使用單機 PostgreSQL（Azure PostgreSQL Flexible Server Basic）

### Phase 2 (Kubernetes)
- 升級到 Redis Cluster（支持分片）
- 升級到 PostgreSQL 主從架構（讀寫分離）
- 引入狀態壓縮（減少存儲大小）

### Phase 3 (Enterprise)
- 引入 Distributed Cache（跨地區同步）
- 引入 Event Sourcing（完整狀態歷史回溯）

## 相關決策

- ADR-007: Multi-Agent 通訊機制
- ADR-008: Code Interpreter 執行模型
- ADR-004: 選擇 PostgreSQL 作為主要關聯式數據庫

## 參考資料

- [Microsoft Distributed Caching](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/distributed)
- [Redis Best Practices](https://redis.io/docs/manual/patterns/)
- [12-Factor App - Stateless Processes](https://12factor.net/processes)
