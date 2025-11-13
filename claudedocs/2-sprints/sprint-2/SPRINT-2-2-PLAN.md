# Sprint 2 執行計劃

**Sprint**: Sprint 2
**週次**: Week 4-6
**日期**: 2025-11-25 ~ 2025-12-21 (27 days, 延遲 6 days)
**狀態**: 🔄 進行中 (Day 17/27, 86% 完成)

---

## 目錄

1. [第一部分：本 Sprint 要建立什麼 (What to Build)](#第一部分本-sprint-要建立什麼-what-to-build)
2. [第二部分：技術實施方案 (How to Build)](#第二部分技術實施方案-how-to-build)
3. [第三部分：編碼規範](#第三部分編碼規範)
4. [第四部分：參考文檔](#第四部分參考文檔)

---

# 第一部分：本 Sprint 要建立什麼 (What to Build)

## US 1.4 - Agent 執行與監控 (13 SP, 4 Phases) ✅

### MVP 範圍定義

**Phase 1: 基礎執行引擎** ✅ (2025-11-27 完成)

**必須實現** (P0):
- [x] **POST /api/v1/agents/{id}/invoke** - Agent 執行 API
  - 接收 userInput, conversationId (可選)
  - 調用 Semantic Kernel 執行 Agent
  - 返回 AgentExecutionDto (包含 response, tokens, responseTime)

- [x] **Semantic Kernel 集成**
  - 建立 Kernel Builder 配置
  - 支援 OpenAI Chat Completion
  - Agent System Prompt 注入
  - Temperature, MaxTokens 參數支援

- [x] **Agent 執行引擎**
  - ExecuteAgentCommand / CommandHandler (CQRS)
  - AgentExecutionService (業務邏輯)
  - AgentExecution 實體 (Domain Layer)
  - AgentExecutionRepository (Infrastructure)

- [x] **Conversation 管理**
  - Conversation 實體設計
  - 多輪對話追蹤
  - ConversationId 關聯

- [x] **基礎執行記錄**
  - AgentExecution Table Schema
  - 記錄: userInput, response, tokens, responseTime, status
  - 軟刪除支援 (is_deleted)

**API 規格** (Phase 1):
```json
// POST /api/v1/agents/{id}/invoke
// Request
{
  "userInput": "What is the weather today?",
  "conversationId": "optional-conversation-id"
}

// Response (201 Created)
{
  "id": "execution-id-uuid",
  "agentId": "agent-id-uuid",
  "conversationId": "conversation-id-uuid",
  "userInput": "What is the weather today?",
  "response": "I don't have real-time weather information...",
  "totalTokens": 150,
  "promptTokens": 50,
  "completionTokens": 100,
  "responseTimeMs": 1250.5,
  "status": "Completed",
  "createdAt": "2025-11-27T10:00:00Z"
}
```

---

**Phase 2: 執行歷史追蹤** ✅ (2025-12-01 完成)

**必須實現** (P0):
- [x] **GET /api/v1/agents/{id}/AgentExecution/history** - 進階查詢
  - 9 個查詢參數支援:
    - `startDate`, `endDate` (DateTime, 日期範圍過濾)
    - `status` (string, "Completed"/"Failed"/"Cancelled")
    - `conversationId` (Guid, 特定對話過濾)
    - `minTokens`, `maxTokens` (int, Token 使用量範圍)
    - `minResponseTimeMs`, `maxResponseTimeMs` (double, 響應時間範圍)
    - `searchTerm` (string, 全文搜索 userInput/response)
  - 排序支援: `sortBy` (CreatedAt/ResponseTimeMs/TotalTokens), `sortDescending` (bool)
  - 分頁支援: `skip`, `take` (max 100)

- [x] **GET /api/v1/agents/{id}/AgentExecution/{executionId}** - 詳細資訊
  - 返回完整的 AgentExecution 詳情
  - 包含 Conversation 關聯資訊

- [x] **GET /api/v1/agents/{id}/AgentExecution/conversations/{conversationId}/executions**
  - 查詢特定對話的所有執行記錄
  - 按時間順序排序

**Repository 增強**:
```csharp
Task<(List<AgentExecution> executions, int total)> GetByAgentIdAsync(
    Guid agentId,
    DateTime? startDate = null,
    DateTime? endDate = null,
    string? status = null,
    Guid? conversationId = null,
    int? minTokens = null,
    int? maxTokens = null,
    double? minResponseTimeMs = null,
    double? maxResponseTimeMs = null,
    string? searchTerm = null,
    string? sortBy = null,
    bool sortDescending = true,
    int skip = 0,
    int take = 50,
    CancellationToken cancellationToken = default);
```

**API 規格** (Phase 2):
```json
// GET /api/v1/agents/{id}/AgentExecution/history?startDate=2025-11-25&status=Completed&skip=0&take=20
// Response (200 OK)
{
  "executions": [
    {
      "id": "...",
      "userInput": "...",
      "response": "...",
      "totalTokens": 150,
      "responseTimeMs": 1250.5,
      "status": "Completed",
      "createdAt": "2025-11-27T10:00:00Z"
    }
  ],
  "total": 245,
  "page": 1,
  "pageSize": 20
}
```

---

**Phase 3: 效能指標追蹤** ✅ (2025-12-04 完成)

**必須實現** (P0):
- [x] **GET /api/v1/agents/{id}/AgentExecution/statistics** - 基礎統計
  - 執行次數統計: `total`, `successful`, `failed`, `cancelled`
  - 響應時間統計: `average`, `min`, `max`, `median`, **`p95`**, **`p99`**
  - Token 使用統計: `totalTokens`, `averageTokens`, `minTokens`, `maxTokens`
  - 成功率計算: `successRate` (percentage)

- [x] **GET /api/v1/agents/{id}/AgentExecution/statistics/timeseries** - 時序分析
  - 4 種時間粒度: `hour`, `day`, `week`, `month`
  - 每個時間點的統計數據: executionCount, avgResponseTime, totalTokens
  - 趨勢分析支援 (前端可繪製折線圖)

**百分位數計算邏輯**:
```csharp
// P95 計算 (95% 的請求響應時間都小於此值)
var sortedTimes = executions
    .Where(e => e.ResponseTimeMs.HasValue)
    .Select(e => e.ResponseTimeMs.Value)
    .OrderBy(t => t)
    .ToList();

var p95Index = (int)Math.Ceiling(sortedTimes.Count * 0.95) - 1;
var p99Index = (int)Math.Ceiling(sortedTimes.Count * 0.99) - 1;

var p95ResponseTime = sortedTimes[p95Index]; // P95
var p99ResponseTime = sortedTimes[p99Index]; // P99
```

**API 規格** (Phase 3):
```json
// GET /api/v1/agents/{id}/AgentExecution/statistics
// Response (200 OK)
{
  "executionCount": {
    "total": 1250,
    "successful": 1200,
    "failed": 45,
    "cancelled": 5
  },
  "responseTime": {
    "average": 1350.5,
    "min": 250.0,
    "max": 5000.0,
    "median": 1200.0,
    "p95": 3500.0,
    "p99": 4800.0
  },
  "tokens": {
    "total": 187500,
    "average": 150,
    "min": 50,
    "max": 500
  },
  "successRate": 96.0
}
```

---

**Phase 4: 即時監控與匯出** ✅ (2025-12-07 完成)

**必須實現** (P0):
- [x] **SignalR WebSocket** - 即時監控
  - Hub: `/hubs/execution-monitor`
  - 4 種生命週期通知:
    - `ExecutionStarted` (agentId, executionId, timestamp)
    - `ExecutionCompleted` (agentId, executionId, responseTime, tokens, status)
    - `ExecutionFailed` (agentId, executionId, error)
    - `StatisticsUpdated` (agentId, statistics)
  - Group 管理: `agent-{agentId}`, `all-executions`

- [x] **CSV/JSON 匯出** (4 個 API):
  - `GET /api/v1/agents/{id}/AgentExecution/export/csv` - 執行歷史 CSV
  - `GET /api/v1/agents/{id}/AgentExecution/export/json` - 執行歷史 JSON
  - `GET /api/v1/agents/{id}/AgentExecution/export/statistics/csv` - 統計資料 CSV
  - `GET /api/v1/agents/{id}/AgentExecution/export/statistics/json` - 統計資料 JSON
  - 日期範圍過濾: `startDate`, `endDate`

**SignalR Hub 實現**:
```csharp
public class ExecutionMonitorHub : Hub
{
    public async Task SubscribeToAgent(Guid agentId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"agent-{agentId}");
    }

    public async Task UnsubscribeFromAgent(Guid agentId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"agent-{agentId}");
    }

    public async Task SubscribeToAllExecutions()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "all-executions");
    }
}
```

**通知服務**:
```csharp
public interface IExecutionNotificationService
{
    Task NotifyExecutionStartedAsync(Guid agentId, Guid executionId);
    Task NotifyExecutionCompletedAsync(Guid agentId, AgentExecutionDto execution);
    Task NotifyExecutionFailedAsync(Guid agentId, Guid executionId, string error);
    Task NotifyStatisticsUpdatedAsync(Guid agentId, object statistics);
}
```

**CSV 匯出範例**:
```csv
ExecutionId,AgentId,ConversationId,UserInput,Response,TotalTokens,ResponseTimeMs,Status,CreatedAt
"uuid-1","agent-id","conv-id","What is AI?","AI stands for...","150","1250.5","Completed","2025-11-27T10:00:00Z"
```

---

### 驗收標準 (Definition of Done)

**Phase 1-4 共同 DoD**:
- [x] 單元測試覆蓋率 >80%
  - ExecuteAgentCommandHandlerTests (更新 mock)
  - ExecutionNotificationServiceTests (新增)
  - 226 tests, 100% 通過
- [x] 集成測試通過
  - 42 tests (41 通過, 1 已知問題)
  - 已知問題: SignalR 測試環境配置 (不影響功能)
- [x] API 文檔完整 (Swagger)
  - 11 個端點/Hub 文檔完整
- [x] 性能達標
  - P95 響應時間 <500ms (實際: ~350ms)
  - 百分位數計算準確
- [x] Code Review 通過
  - Tech Lead 審核完成
- [x] 4 個 Phase 完成報告
  - US-1.4-Phase1-Summary.md
  - US-1.4-Phase2-Summary.md
  - US-1.4-Phase3-Performance-Metrics-Summary.md
  - US-1.4-Phase4-Realtime-Monitoring-Export-Summary.md

---

## US 2.1 - 註冊 .NET Plugin (5 SP, 5 Phases) ✅

### MVP 範圍定義

**Phase 1: Domain Layer** ✅ (2025-12-08 完成)

**必須實現** (P0):
- [x] **PluginVersion 實體** (Domain Entity)
  - 屬性: Id, PluginId, Version, Name, Description, Metadata, Status, IsCurrentVersion
  - 方法: Create(), Activate(), Deactivate(), MarkAsCurrentVersion()
  - 264 lines, 完整的領域邏輯

- [x] **VersionNumber 值對象** (SemVer 格式)
  - 格式: "1.2.3" (Major.Minor.Patch)
  - 驗證: 必須符合 SemVer 規範
  - 比較: IComparable 實現

- [x] **PluginMetadata 值對象**
  - 屬性: AssemblyName, Version, Author, Description, Dependencies
  - 從 Assembly 提取 Metadata

- [x] **PluginStatus 值對象**
  - 值: Active, Inactive, Deprecated
  - 狀態轉換邏輯

- [x] **Repository Interfaces**
  - IPluginVersionRepository (7 個方法)
  - IPluginVersionHistoryRepository

**PluginVersion 實體設計**:
```csharp
public sealed class PluginVersion : BaseEntity
{
    // 屬性
    public string PluginId { get; private set; }
    public VersionNumber Version { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public PluginMetadata Metadata { get; private set; }
    public PluginStatus Status { get; private set; }
    public bool IsCurrentVersion { get; private set; }
    public string? AssemblyPath { get; private set; }
    public int DownloadCount { get; private set; }
    public int ActiveAgentCount { get; private set; }

    // 工廠方法
    public static PluginVersion Create(
        Guid userId, string pluginId, VersionNumber version,
        string name, PluginMetadata metadata, ...)
    {
        var pluginVersion = new PluginVersion(userId, pluginId, version, name, metadata);
        // 領域事件: PluginVersionCreated
        return pluginVersion;
    }

    // 領域方法
    public void Activate() { Status = PluginStatus.Active; }
    public void Deactivate() { Status = PluginStatus.Inactive; }
    public void MarkAsCurrentVersion() { IsCurrentVersion = true; }
}
```

---

**Phase 2: Infrastructure Layer - Dynamic Loading** ✅ (2025-12-09 完成)

**必須實現** (P0):
- [x] **PluginLoader 服務** (動態加載)
  - AssemblyLoadContext 隔離上下文
  - LoadPluginAsync() - 加載 Plugin DLL
  - UnloadPluginAsync() - 卸載 Plugin
  - ReloadPluginAsync() - 重載 Plugin
  - GetLoadedPlugins() - 查詢已加載 Plugins
  - 372 lines

- [x] **PluginActivator 服務** (實例創建)
  - CreateInstanceAsync() - 創建 Plugin 實例
  - GetExportedTypesAsync() - 獲取導出類型
  - InvokeStaticMethodAsync() - 調用靜態方法
  - 258 lines

- [x] **PluginAssemblyLoadContext** (隔離上下文)
  - 繼承 AssemblyLoadContext
  - `isCollectible: true` (支援卸載)
  - AssemblyDependencyResolver (依賴解析)

- [x] **多版本共存支持**
  - 不同版本的同一 Plugin 可同時加載
  - 使用 PluginId + Version 作為唯一標識
  - 避免 Assembly 衝突

**PluginLoader 核心邏輯**:
```csharp
public class PluginLoader : IPluginLoader, IDisposable
{
    private readonly ConcurrentDictionary<string, LoadedPluginInfo> _loadedPlugins = new();

    public async Task<PluginLoadResult> LoadPluginAsync(
        string assemblyPath, string pluginId, string version)
    {
        // 1. 驗證文件存在
        if (!File.Exists(assemblyPath))
            return PluginLoadResult.Failure("Assembly file not found");

        // 2. 創建隔離上下文
        var loadContext = new PluginAssemblyLoadContext(assemblyPath, pluginId, version);

        // 3. 加載 Assembly
        var assembly = loadContext.LoadFromAssemblyPath(assemblyPath);

        // 4. 記錄已加載 Plugin
        var key = $"{pluginId}_{version}";
        _loadedPlugins.TryAdd(key, new LoadedPluginInfo
        {
            PluginId = pluginId,
            Version = version,
            Assembly = assembly,
            LoadContext = loadContext,
            LoadedAt = DateTime.UtcNow
        });

        return PluginLoadResult.Success(assembly.FullName, assembly.GetTypes().Length);
    }

    public async Task<bool> UnloadPluginAsync(string pluginId, string version)
    {
        var key = $"{pluginId}_{version}";
        if (_loadedPlugins.TryRemove(key, out var pluginInfo))
        {
            pluginInfo.LoadContext?.Unload();
            return true;
        }
        return false;
    }
}
```

---

**Phase 3: Application Layer - CQRS** ✅ (2025-12-09 完成)

**必須實現** (P0):
- [x] **Commands** (3 個):
  - RegisterPluginVersionCommand / CommandHandler
  - UpdatePluginVersionCommand / CommandHandler
  - DeletePluginVersionCommand / CommandHandler

- [x] **Queries** (3 個):
  - GetPluginVersionQuery / QueryHandler
  - GetPluginVersionsQuery / QueryHandler (支援過濾)
  - GetPluginVersionHistoryQuery / QueryHandler

- [x] **FluentValidation 驗證器**
  - RegisterPluginVersionCommandValidator
  - VersionNumber 格式驗證 (SemVer)
  - AssemblyPath 存在性驗證

**Commands & Queries 定義**:
```csharp
// Commands
public record RegisterPluginVersionCommand(
    Guid UserId,
    string PluginId,
    string Version,
    string Name,
    string AssemblyPath,
    string? Description = null
) : IRequest<PluginVersionDto>;

public record UpdatePluginVersionCommand(
    Guid Id,
    string? Name,
    string? Description,
    string? Status
) : IRequest<PluginVersionDto>;

// Queries
public record GetPluginVersionsQuery(
    string? PluginId = null,
    string? Status = null,
    int Skip = 0,
    int Take = 50
) : IRequest<List<PluginVersionDto>>;
```

---

**Phase 4: API Layer** ✅ (2025-12-09 完成)

**必須實現** (P0):
- [x] **PluginVersionsController** (5 個 API 端點)
  - POST /api/v1/plugin-versions - 註冊新版本
  - GET /api/v1/plugin-versions - 查詢所有版本
  - GET /api/v1/plugin-versions/{id} - 取得特定版本
  - PUT /api/v1/plugin-versions/{id} - 更新版本資訊
  - DELETE /api/v1/plugin-versions/{id} - 刪除版本
  - GET /api/v1/plugin-versions/{id}/history - 版本歷史

- [x] **RESTful API 設計**
  - HTTP Status Codes: 200 OK, 201 Created, 400 Bad Request, 404 Not Found
  - Error Handling: ProblemDetails 格式
  - Swagger 文檔完整

**API 規格**:
```json
// POST /api/v1/plugin-versions
// Request
{
  "pluginId": "weather-plugin",
  "version": "1.0.0",
  "name": "Weather Plugin",
  "assemblyPath": "/plugins/weather-plugin-1.0.0.dll",
  "description": "Provides weather information"
}

// Response (201 Created)
{
  "id": "uuid",
  "pluginId": "weather-plugin",
  "version": "1.0.0",
  "name": "Weather Plugin",
  "status": "Active",
  "isCurrentVersion": true,
  "metadata": {
    "assemblyName": "WeatherPlugin",
    "version": "1.0.0",
    "author": "Development Team"
  },
  "createdAt": "2025-12-09T10:00:00Z"
}
```

---

**Phase 5: EF Core Repository & Migration** ✅ (2025-12-10 完成)

**必須實現** (P0):
- [x] **PluginVersionRepository** (7 個方法實現)
  - GetByIdAsync()
  - GetByPluginIdAndVersionAsync()
  - GetAllVersionsByPluginIdAsync()
  - GetCurrentVersionAsync()
  - AddAsync()
  - UpdateAsync()
  - DeleteAsync()

- [x] **PluginVersionHistoryRepository**
  - AddHistoryAsync()
  - GetHistoryByPluginVersionIdAsync()

- [x] **EF Core Value Converters**
  - VersionNumber Converter (string ↔ VersionNumber)
  - PluginMetadata Converter (JSON ↔ PluginMetadata)
  - PluginStatus Converter (string ↔ PluginStatus)

- [x] **Database Migration**
  - Migration Name: `AddPluginVersioning`
  - Tables: plugin_versions, plugin_version_history
  - Indexes: idx_plugin_id, idx_version, idx_status, idx_is_current_version

**Repository 實現**:
```csharp
public class PluginVersionRepository : IPluginVersionRepository
{
    private readonly ApplicationDbContext _context;

    public async Task<PluginVersion?> GetByPluginIdAndVersionAsync(
        string pluginId, string version, CancellationToken cancellationToken = default)
    {
        return await _context.PluginVersions
            .FirstOrDefaultAsync(p => p.PluginId == pluginId
                && p.Version.ToString() == version, cancellationToken);
    }

    public async Task<PluginVersion?> GetCurrentVersionAsync(
        string pluginId, CancellationToken cancellationToken = default)
    {
        return await _context.PluginVersions
            .Where(p => p.PluginId == pluginId && p.IsCurrentVersion)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
```

**EF Core Configuration**:
```csharp
public class PluginVersionConfiguration : IEntityTypeConfiguration<PluginVersion>
{
    public void Configure(EntityTypeBuilder<PluginVersion> builder)
    {
        builder.ToTable("plugin_versions");

        // VersionNumber Value Converter
        builder.Property(p => p.Version)
            .HasConversion(
                v => v.ToString(),
                v => VersionNumber.Parse(v))
            .HasMaxLength(20);

        // PluginMetadata Value Converter (JSON)
        builder.Property(p => p.Metadata)
            .HasConversion(
                m => JsonSerializer.Serialize(m, (JsonSerializerOptions?)null),
                json => JsonSerializer.Deserialize<PluginMetadata>(json, (JsonSerializerOptions?)null)!)
            .HasColumnType("jsonb"); // PostgreSQL JSONB

        // Indexes
        builder.HasIndex(p => p.PluginId).HasDatabaseName("idx_plugin_id");
        builder.HasIndex(p => p.Status).HasDatabaseName("idx_status");
        builder.HasIndex(p => p.IsCurrentVersion).HasDatabaseName("idx_is_current_version");
    }
}
```

---

### US 2.2/2.3 部分實現 (CHANGE-002) 🔄

**US 2.2: Plugin 熱重載** (Phase 1-2 完成 40%)

**已完成** (Phase 1-2):
- [x] LoadPluginCommand / CommandHandler
- [x] UnloadPluginCommand / CommandHandler
- [x] ReloadPluginCommand / CommandHandler
- [x] PluginLoader.UnloadPluginAsync() 實現
- [x] PluginLoader.ReloadPluginAsync() 實現

**待完成** (Phase 3-5):
- [ ] API Layer (PluginManagementController)
- [ ] 集成測試 (熱重載測試)
- [ ] 文檔與驗收

**US 2.3: Plugin 版本管理** (Phase 1-2 完成 30%)

**已完成** (Phase 1-2):
- [x] ActivatePluginVersionCommand / CommandHandler
- [x] DeactivatePluginVersionCommand / CommandHandler
- [x] SetCurrentVersionCommand / CommandHandler
- [x] GetPluginVersionHistoryQuery / QueryHandler
- [ ] PluginVersionHistoryRepository (部分實現)

**待完成** (Phase 3-5):
- [ ] API Layer (版本管理端點)
- [ ] 版本切換邏輯完善
- [ ] 測試與驗收

---

### 驗收標準 (Definition of Done)

**US 2.1 完整 DoD**:
- [x] 單元測試覆蓋率 >80%
  - PluginLoaderTests: 11 tests (100% 通過)
  - PluginActivatorTests: 10 tests (100% 通過)
  - TestPlugin.dll: 測試用 Plugin
- [x] 5 個 API 端點測試通過
- [x] 動態加載/卸載功能驗證
- [x] 多版本共存測試通過
- [x] EF Core Migration 成功執行
- [x] PostgreSQL JSONB 支持驗證
- [x] Code Review 通過
- [x] Pull Request #6 合併

---

## US 6.1 - 文字對話 (基礎) (3 SP) ⏳

### MVP 範圍定義

**必須實現** (P0):
- [ ] **Chat UI 組件**
  - ChatWindow (聊天窗口容器)
  - MessageList (消息列表,支持虛擬滾動)
  - MessageBubble (單條消息氣泡)
  - InputBox (輸入框 + 發送按鈕)
  - TypingIndicator (正在輸入指示器)

- [ ] **Markdown 渲染**
  - 使用 react-markdown 或 marked.js
  - 代碼高亮 (使用 prism.js)
  - 支持表格、列表、引用

- [ ] **SignalR 集成**
  - 複用 US 1.4 的 ExecutionMonitorHub
  - 監聽 ExecutionCompleted 事件
  - 實時顯示 Agent 回覆

- [ ] **Chat Session 管理**
  - 創建新對話
  - 查看對話列表
  - 刪除對話
  - ConversationId 關聯 AgentExecution

- [ ] **Message 持久化**
  - Message 實體 (id, conversationId, role, content, timestamp)
  - MessageRepository (CRUD)
  - 對話歷史查詢

**可選實現** (P1):
- [ ] Message 搜索功能
- [ ] 對話導出 (PDF/Markdown)
- [ ] 多模態支持 (圖片、文件)

**明確排除** (Phase 2):
- ❌ Function Calling 可視化
- ❌ 3 欄布局 (Sidebar, Chat, Function Panel)
- ❌ Agent Streaming (逐字輸出)
- ❌ Voice Input/Output

### UI 設計參考

**Layout**: 2 欄布局
```
+----------------------+---------------------------+
| Sidebar (280px)      | Chat Area (flex-grow)     |
| -------------------- | ------------------------- |
| - Conversations      | - Header                  |
| - New Chat Button    | - Message List            |
| - Agent Selector     | - Input Box               |
+----------------------+---------------------------+
```

**React 組件結構**:
```tsx
// ChatPage.tsx
<ChatPage>
  <ConversationSidebar>
    <NewChatButton />
    <ConversationList />
  </ConversationSidebar>

  <ChatWindow>
    <ChatHeader agentName="Customer Support Agent" />
    <MessageList messages={messages}>
      {messages.map(msg => (
        <MessageBubble key={msg.id} message={msg} />
      ))}
    </MessageList>
    <InputBox onSend={handleSend} />
  </ChatWindow>
</ChatPage>
```

### API 規格

```json
// POST /api/v1/conversations (創建新對話)
{
  "agentId": "agent-id-uuid",
  "title": "Customer Support Chat"
}

// Response (201 Created)
{
  "id": "conversation-id-uuid",
  "agentId": "agent-id-uuid",
  "title": "Customer Support Chat",
  "createdAt": "2025-12-13T10:00:00Z"
}

// POST /api/v1/agents/{id}/invoke (複用 US 1.4)
{
  "userInput": "How do I reset my password?",
  "conversationId": "conversation-id-uuid"
}

// GET /api/v1/conversations/{id}/messages (查詢對話歷史)
// Response (200 OK)
{
  "messages": [
    {
      "id": "msg-1",
      "role": "user",
      "content": "How do I reset my password?",
      "timestamp": "2025-12-13T10:00:00Z"
    },
    {
      "id": "msg-2",
      "role": "assistant",
      "content": "To reset your password, go to...",
      "timestamp": "2025-12-13T10:00:05Z"
    }
  ]
}
```

### SignalR 集成

```typescript
// useChatSignalR.ts
import { HubConnectionBuilder } from '@microsoft/signalr';

const useChatSignalR = (agentId: string, conversationId: string) => {
  const [connection, setConnection] = useState<HubConnection | null>(null);

  useEffect(() => {
    const newConnection = new HubConnectionBuilder()
      .withUrl('/hubs/execution-monitor')
      .withAutomaticReconnect()
      .build();

    newConnection.on('ExecutionCompleted', (data) => {
      if (data.conversationId === conversationId) {
        // 添加 Agent 回覆到消息列表
        addMessage({
          role: 'assistant',
          content: data.response,
          timestamp: data.completedAt
        });
      }
    });

    newConnection.start().then(() => {
      newConnection.invoke('SubscribeToAgent', agentId);
    });

    setConnection(newConnection);

    return () => {
      newConnection.stop();
    };
  }, [agentId, conversationId]);

  return connection;
};
```

### 驗收標準 (Definition of Done)

- [ ] 可以創建新對話
- [ ] 可以發送消息並收到 Agent 回覆
- [ ] Markdown 渲染正確 (代碼高亮、表格、列表)
- [ ] SignalR 實時更新正常
- [ ] 對話歷史可以查詢和顯示
- [ ] 響應式設計 (桌面、平板、手機)
- [ ] 單元測試通過 (React Testing Library)
- [ ] E2E 測試通過 (Playwright)
- [ ] 性能達標 (首屏渲染 <2s, 虛擬滾動支持 1000+ 消息)

---

# 第二部分：技術實施方案 (How to Build)

## Backend 技術棧

### 核心框架
- **ASP.NET Core 8**: Web API 框架
- **Semantic Kernel**: LLM Agent 編排框架
- **Entity Framework Core 8**: ORM, Code-First Migrations
- **MediatR**: CQRS 實現, Pipeline Behaviors
- **FluentValidation**: 輸入驗證
- **SignalR**: WebSocket 即時通訊

### 資料庫
- **PostgreSQL 16**: 關聯式資料庫
- **JSONB**: Plugin Metadata 儲存
- **Redis 7**: Cache (未來 Phase)

### Clean Architecture 分層

```
┌──────────────────────────────────────────┐
│  Presentation Layer (API)                │
│  - Controllers                           │
│  - SignalR Hubs                          │
│  - Middleware                            │
└──────────────┬───────────────────────────┘
               │
┌──────────────▼───────────────────────────┐
│  Application Layer                       │
│  - CQRS Commands / Queries               │
│  - Handlers (MediatR)                    │
│  - DTOs, Mappers                         │
│  - FluentValidation Validators           │
└──────────────┬───────────────────────────┘
               │
┌──────────────▼───────────────────────────┐
│  Domain Layer                            │
│  - Entities (Agent, AgentExecution, etc.)│
│  - Value Objects (VersionNumber, etc.)   │
│  - Interfaces (Repositories)             │
│  - Domain Events                         │
└──────────────┬───────────────────────────┘
               │
┌──────────────▼───────────────────────────┐
│  Infrastructure Layer                    │
│  - EF Core Repositories                  │
│  - Services (PluginLoader, etc.)         │
│  - External Services (OpenAI, etc.)      │
│  - Database Configuration                │
└──────────────────────────────────────────┘
```

### Semantic Kernel 集成

```csharp
// Program.cs - Semantic Kernel 註冊
builder.Services.AddScoped<IKernel>(sp =>
{
    var openAiApiKey = configuration["OpenAI:ApiKey"];

    return Kernel.CreateBuilder()
        .AddOpenAIChatCompletion("gpt-4", openAiApiKey)
        .Build();
});

// ExecuteAgentCommandHandler.cs - 使用 Semantic Kernel
public async Task<AgentExecutionDto> Handle(
    ExecuteAgentCommand request, CancellationToken cancellationToken)
{
    var agent = await _agentRepository.GetByIdAsync(request.AgentId);

    // 1. 構建 Prompt
    var prompt = $"{agent.SystemPrompt}\n\nUser: {request.UserInput}";

    // 2. 調用 Semantic Kernel
    var result = await _kernel.InvokePromptAsync(prompt, cancellationToken: cancellationToken);

    // 3. 記錄執行歷史
    var execution = AgentExecution.Create(
        userId: request.UserId,
        agentId: agent.Id,
        conversationId: request.ConversationId,
        userInput: request.UserInput,
        response: result.ToString(),
        tokens: ...,
        responseTimeMs: ...
    );

    await _executionRepository.AddAsync(execution);

    // 4. 發送 SignalR 通知
    await _notificationService.NotifyExecutionCompletedAsync(agent.Id, execution);

    return _mapper.Map<AgentExecutionDto>(execution);
}
```

### Plugin 動態加載架構

```csharp
// PluginAssemblyLoadContext.cs - 隔離上下文
private sealed class PluginAssemblyLoadContext : AssemblyLoadContext
{
    private readonly AssemblyDependencyResolver _resolver;

    public PluginAssemblyLoadContext(string assemblyPath, string pluginId, string version)
        : base(name: $"Plugin_{pluginId}_{version}", isCollectible: true)
    {
        _resolver = new AssemblyDependencyResolver(assemblyPath);
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
        if (assemblyPath != null)
        {
            return LoadFromAssemblyPath(assemblyPath);
        }
        return null;
    }
}
```

### SignalR 配置

```csharp
// Program.cs
builder.Services.AddSignalR();
builder.Services.AddScoped<IExecutionNotificationService, ExecutionNotificationService>();

// CORS for SignalR
builder.Services.AddCors(options =>
{
    options.AddPolicy("SignalRCors", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

app.UseCors("SignalRCors");
app.MapHub<ExecutionMonitorHub>("/hubs/execution-monitor")
   .RequireCors("SignalRCors");
```

---

## Frontend 技術棧

### 核心框架
- **React 18**: UI 框架
- **TypeScript**: 類型安全
- **Material-UI v5**: UI 組件庫
- **React Router v6**: 路由管理
- **TanStack Query (React Query)**: 服務器狀態管理
- **Zustand**: 客戶端狀態管理 (輕量級)

### 通訊與資料
- **Axios**: HTTP Client
- **@microsoft/signalr**: SignalR Client
- **React Hook Form + Yup**: 表單驗證
- **react-markdown**: Markdown 渲染
- **prism-react-renderer**: 代碼高亮

### 測試與工具
- **React Testing Library**: 組件測試
- **Playwright**: E2E 測試
- **Vite**: 構建工具
- **ESLint + Prettier**: 代碼格式化

### 項目結構

```
apps/web-app/src/
├── features/                    # 功能模組
│   ├── agents/                 # Agent 管理
│   ├── chat/                   # Chat UI (US 6.1)
│   │   ├── components/
│   │   │   ├── ChatWindow.tsx
│   │   │   ├── MessageList.tsx
│   │   │   ├── MessageBubble.tsx
│   │   │   ├── InputBox.tsx
│   │   │   └── ConversationSidebar.tsx
│   │   ├── hooks/
│   │   │   ├── useChatSignalR.ts
│   │   │   ├── useConversations.ts
│   │   │   └── useMessages.ts
│   │   └── ChatPage.tsx
│   └── plugins/                # Plugin 管理 (US 2.1)
├── shared/
│   ├── components/             # 共用組件
│   ├── hooks/                  # 共用 Hooks
│   ├── services/               # API Services
│   │   ├── agentService.ts
│   │   ├── conversationService.ts
│   │   └── pluginService.ts
│   ├── types/                  # TypeScript Types
│   └── utils/                  # 工具函數
└── App.tsx
```

### React Query 配置

```typescript
// queryClient.ts
import { QueryClient } from '@tanstack/react-query';

export const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 5 * 60 * 1000, // 5 minutes
      cacheTime: 10 * 60 * 1000, // 10 minutes
      refetchOnWindowFocus: false,
      retry: 1,
    },
  },
});

// useConversations.ts - React Query Hook
export const useConversations = (agentId: string) => {
  return useQuery({
    queryKey: ['conversations', agentId],
    queryFn: () => conversationService.getConversations(agentId),
    enabled: !!agentId,
  });
};

export const useCreateConversation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: conversationService.createConversation,
    onSuccess: (data, variables) => {
      queryClient.invalidateQueries({ queryKey: ['conversations', variables.agentId] });
    },
  });
};
```

### SignalR Client 整合

```typescript
// useChatSignalR.ts
import { HubConnectionBuilder, HubConnection } from '@microsoft/signalr';
import { useEffect, useState, useCallback } from 'react';

export const useChatSignalR = (agentId: string) => {
  const [connection, setConnection] = useState<HubConnection | null>(null);
  const [connected, setConnected] = useState(false);

  useEffect(() => {
    const newConnection = new HubConnectionBuilder()
      .withUrl('http://localhost:5000/hubs/execution-monitor')
      .withAutomaticReconnect()
      .build();

    // 訂閱事件
    newConnection.on('ExecutionStarted', (data) => {
      console.log('Execution started:', data);
    });

    newConnection.on('ExecutionCompleted', (data) => {
      console.log('Execution completed:', data);
      // 處理收到的 Agent 回覆
    });

    newConnection.on('ExecutionFailed', (data) => {
      console.error('Execution failed:', data);
    });

    // 啟動連接
    newConnection.start()
      .then(() => {
        console.log('SignalR Connected');
        newConnection.invoke('SubscribeToAgent', agentId);
        setConnected(true);
      })
      .catch(err => console.error('SignalR Connection Error:', err));

    setConnection(newConnection);

    return () => {
      if (newConnection.state === 'Connected') {
        newConnection.stop();
      }
    };
  }, [agentId]);

  return { connection, connected };
};
```

---

## 資料庫 Schema

### AgentExecution Table (US 1.4)

```sql
CREATE TABLE agent_executions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    agent_id UUID NOT NULL REFERENCES agents(id),
    conversation_id UUID REFERENCES conversations(id),
    user_input TEXT NOT NULL,
    response TEXT NOT NULL,
    total_tokens INTEGER,
    prompt_tokens INTEGER,
    completion_tokens INTEGER,
    response_time_ms DOUBLE PRECISION,
    status VARCHAR(50) NOT NULL, -- Completed, Failed, Cancelled
    error_message TEXT,
    is_deleted BOOLEAN DEFAULT false,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP NOT NULL DEFAULT NOW(),
    created_by VARCHAR(100),
    updated_by VARCHAR(100)
);

-- Indexes
CREATE INDEX idx_agent_executions_agent_id ON agent_executions(agent_id);
CREATE INDEX idx_agent_executions_conversation_id ON agent_executions(conversation_id);
CREATE INDEX idx_agent_executions_status ON agent_executions(status);
CREATE INDEX idx_agent_executions_created_at ON agent_executions(created_at DESC);
CREATE INDEX idx_agent_executions_is_deleted ON agent_executions(is_deleted);
```

### PluginVersion Table (US 2.1)

```sql
CREATE TABLE plugin_versions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    plugin_id VARCHAR(100) NOT NULL,
    version VARCHAR(20) NOT NULL, -- SemVer format: 1.2.3
    name VARCHAR(200) NOT NULL,
    description TEXT,
    metadata JSONB NOT NULL, -- PluginMetadata (AssemblyName, Version, Author, etc.)
    status VARCHAR(50) NOT NULL, -- Active, Inactive, Deprecated
    is_current_version BOOLEAN DEFAULT false,
    assembly_path TEXT NOT NULL,
    download_count INTEGER DEFAULT 0,
    active_agent_count INTEGER DEFAULT 0,
    is_deleted BOOLEAN DEFAULT false,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP NOT NULL DEFAULT NOW(),
    created_by VARCHAR(100),
    updated_by VARCHAR(100),

    UNIQUE(plugin_id, version)
);

-- Indexes
CREATE INDEX idx_plugin_versions_plugin_id ON plugin_versions(plugin_id);
CREATE INDEX idx_plugin_versions_status ON plugin_versions(status);
CREATE INDEX idx_plugin_versions_is_current_version ON plugin_versions(is_current_version);
CREATE INDEX idx_plugin_versions_created_at ON plugin_versions(created_at DESC);

-- JSONB GIN Index for fast queries
CREATE INDEX idx_plugin_versions_metadata ON plugin_versions USING GIN (metadata);
```

### Conversation & Message Tables (US 6.1)

```sql
CREATE TABLE conversations (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    agent_id UUID NOT NULL REFERENCES agents(id),
    title VARCHAR(255),
    is_deleted BOOLEAN DEFAULT false,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP NOT NULL DEFAULT NOW(),
    created_by VARCHAR(100)
);

CREATE TABLE messages (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    conversation_id UUID NOT NULL REFERENCES conversations(id),
    role VARCHAR(50) NOT NULL, -- user, assistant, system
    content TEXT NOT NULL,
    metadata JSONB, -- 可選的額外資訊
    created_at TIMESTAMP NOT NULL DEFAULT NOW()
);

-- Indexes
CREATE INDEX idx_conversations_agent_id ON conversations(agent_id);
CREATE INDEX idx_conversations_created_at ON conversations(created_at DESC);
CREATE INDEX idx_messages_conversation_id ON messages(conversation_id);
CREATE INDEX idx_messages_created_at ON messages(created_at ASC);
```

---

# 第三部分：編碼規範

## C# 編碼規範

### 命名規範
- **PascalCase**: Classes, Methods, Properties, Events
  ```csharp
  public class AgentExecutionService { }
  public async Task<AgentDto> GetAgentAsync(Guid id) { }
  public string UserInput { get; set; }
  ```

- **camelCase**: Private fields, local variables, parameters
  ```csharp
  private readonly IAgentRepository _agentRepository;
  var agentId = Guid.NewGuid();
  public async Task DoSomething(string inputText) { }
  ```

- **I prefix**: Interfaces
  ```csharp
  public interface IAgentRepository { }
  ```

### Clean Code 原則
- **Single Responsibility**: 每個類別只負責一件事
- **Dependency Injection**: 使用 DI 容器管理依賴
- **Async/Await**: 所有 I/O 操作使用異步
- **Null Safety**: 使用 nullable reference types (`string?`)
- **Error Handling**: 使用 Result Pattern 或自定義 Exception

### CQRS Pattern
```csharp
// Command (改變狀態)
public record ExecuteAgentCommand(
    Guid UserId,
    Guid AgentId,
    string UserInput,
    Guid? ConversationId = null
) : IRequest<AgentExecutionDto>;

// Query (讀取狀態)
public record GetAgentExecutionsQuery(
    Guid AgentId,
    int Skip = 0,
    int Take = 50
) : IRequest<List<AgentExecutionDto>>;
```

---

## TypeScript / React 編碼規範

### 命名規範
- **PascalCase**: React Components, Types, Interfaces
  ```typescript
  export const ChatWindow: React.FC<ChatWindowProps> = ({ ... }) => { };
  export interface AgentDto { }
  export type MessageRole = 'user' | 'assistant';
  ```

- **camelCase**: Functions, Variables, Props
  ```typescript
  const handleSendMessage = () => { };
  const [messages, setMessages] = useState([]);
  ```

### React Best Practices
- **Functional Components**: 使用函數式組件 + Hooks
- **TypeScript Strict Mode**: 啟用 strict 模式
- **Custom Hooks**: 提取可複用邏輯到自定義 Hooks
- **Prop Types**: 使用 TypeScript Interface 定義 Props
- **Memoization**: 使用 React.memo, useMemo, useCallback 優化性能

### 範例: Custom Hook
```typescript
// useConversations.ts
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { conversationService } from '@/services/conversationService';

export const useConversations = (agentId: string) => {
  return useQuery({
    queryKey: ['conversations', agentId],
    queryFn: () => conversationService.getConversations(agentId),
    enabled: !!agentId,
  });
};

export const useCreateConversation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: conversationService.createConversation,
    onSuccess: (data, variables) => {
      queryClient.invalidateQueries({ queryKey: ['conversations', variables.agentId] });
    },
  });
};
```

---

## Git Commit 規範

### Commit Message 格式
```
<type>(<scope>): <subject>

<body>

<footer>
```

### Type 類型
- **feat**: 新功能 (feature)
- **fix**: Bug 修復
- **docs**: 文檔更新
- **style**: 代碼格式調整 (不影響功能)
- **refactor**: 代碼重構
- **test**: 測試代碼
- **chore**: 構建過程或輔助工具變動

### 範例
```
feat(agents): add agent execution history API

- Implement GetAgentExecutionHistoryQuery
- Add 9 query parameters support (date range, status, tokens, etc.)
- Add pagination and sorting support

Closes #123
```

---

# 第四部分：參考文獻索引

本執行計劃整合了以下文檔的技術細節，按類別組織以便快速查找實施指引：

## Planning & Sprint 文檔

- [SPRINT-2-1-OVERVIEW.md](./SPRINT-2-1-OVERVIEW.md) - Sprint 2 概覽與進度追蹤
- [SPRINT-2-3-CONTEXT.md](./SPRINT-2-3-CONTEXT.md) - 上下文參考與技術決策
- [SPRINT-2-4-CHECKLIST.md](./SPRINT-2-4-CHECKLIST.md) - 詳細任務檢查清單
- [MVP Scope Definition](../../1-planning/MVP-SCOPE-DEFINITION.md) - Sprint 2 在 MVP 中的範圍定義
- [Sprint Allocation Analysis](../../1-planning/SPRINT-ALLOCATION-ANALYSIS.md) - Sprint 2 詳細分配與依賴關係
- [Development Strategy](../../1-planning/DEVELOPMENT-STRATEGY.md) - Git 工作流、CI/CD、測試策略

## User Stories 規格

- [Module 01: Agent Creation](../../docs/user-stories/modules/module-01-agent-creation.md) - US 1.4 完整規格
  - Agent 執行引擎需求
  - 執行歷史追蹤需求
  - 效能指標需求
  - 即時監控需求
  - 驗收標準
- [Module 02: Plugin System](../../docs/user-stories/modules/module-02-plugin-system.md) - US 2.1-2.3 完整規格
  - Plugin 註冊與管理需求
  - 熱重載機制需求
  - 版本管理需求
  - 驗收標準
- [Module 06: Chat Interface](../../docs/user-stories/modules/module-06-chat-interface.md) - US 6.1 基礎對話規格
  - Chat UI 組件需求
  - Message 管理需求
  - 驗收標準

## 架構設計 (ADR)

- [ADR-002: CQRS Pattern](../../docs/architecture/adr/ADR-002-cqrs-pattern.md)
  - MediatR Commands/Queries 設計
  - Command/Query 責任分離
  - Handler 實作模式
- [ADR-006: Agent State Management](../../docs/architecture/adr/ADR-006-agent-state-management.md)
  - Agent 執行狀態管理
  - State Machine 設計
  - 狀態轉換規則
- [ADR-007: Multi-Agent Communication](../../docs/architecture/adr/ADR-007-multi-agent-communication.md)
  - SignalR WebSocket 通訊架構
  - Hub 設計模式
  - Group 管理策略
- [ADR-008: Code Interpreter Execution Model](../../docs/architecture/adr/ADR-008-code-interpreter-execution-model.md)
  - 執行引擎設計原則
  - 安全沙箱架構
  - 資源管理策略
- [ADR-011: Framework Migration Strategy](../../docs/architecture/adr/ADR-011-framework-migration-strategy.md)
  - Semantic Kernel 抽象層設計
  - IAgentExecutor 介面定義
  - Framework Abstraction Layer 實作
- [ADR-012: Workflow Editor Technology](../../docs/architecture/adr/ADR-012-workflow-editor-technology.md)
  - React 18 技術選型
  - Material-UI 元件庫選型
  - Zustand 狀態管理選型
- [Architecture Design Document](../../docs/architecture/Architecture-Design-Document.md)
  - Clean Architecture 分層設計
  - Domain-Driven Design 實踐
  - 系統架構概覽
- [Database Schema](../../docs/architecture/database-schema.md)
  - AgentExecution Table 設計
  - Conversation Table 設計
  - PluginVersion Table 設計
  - Entity Relationships

## Backend 技術實施 (.NET 9)

- [Semantic Kernel Integration](../../docs/technical-implementation/01-backend-net9/08-semantic-kernel-integration.md)
  - Kernel Builder 配置
  - OpenAI Chat Completion 整合
  - Prompt 管理最佳實踐
  - Plugin 整合模式
- [CQRS Implementation](../../docs/technical-implementation/01-backend-net9/05-cqrs-implementation.md)
  - MediatR 配置與註冊
  - Command Handler 實作範例
  - Query Handler 實作範例
  - FluentValidation 整合
- [Plugin System Architecture](../../docs/technical-implementation/01-backend-net9/11-plugin-system-architecture.md)
  - AssemblyLoadContext 動態加載
  - Plugin Isolation 設計
  - 多版本共存實作
  - Unload 機制
- [SignalR WebSocket](../../docs/technical-implementation/01-backend-net9/10-signalr-websocket.md)
  - SignalR Hub 實作
  - CORS 配置
  - Group 管理實作
  - 通知服務設計
- [Repository Pattern](../../docs/technical-implementation/01-backend-net9/06-repository-pattern.md)
  - Generic Repository 實作
  - Specification Pattern 應用
  - 複雜查詢實作
  - EF Core 最佳實踐
- [Value Objects](../../docs/technical-implementation/01-backend-net9/07-value-objects.md)
  - VersionNumber 實作 (SemVer)
  - PluginMetadata 實作
  - PluginStatus 實作
  - Value Converters 設計

## Frontend 技術實施 (React 18)

- [React Coding Standards](../../docs/technical-implementation/04-coding-standards/react-coding-standards.md)
  - React 18 最佳實踐
  - Functional Components 規範
  - Hooks 使用指引
  - 效能優化建議
- [TypeScript Coding Standards](../../docs/technical-implementation/04-coding-standards/typescript-coding-standards.md)
  - TypeScript 類型系統
  - Interface vs Type
  - Generics 應用
  - 類型安全實踐
- [State Management (Zustand)](../../docs/technical-implementation/02-frontend-react/06-state-management-zustand.md)
  - Zustand Store 設計
  - State Slicing 模式
  - Middleware 應用
  - DevTools 整合
- [API Client Integration](../../docs/technical-implementation/02-frontend-react/07-api-client-integration.md)
  - Axios 配置
  - Interceptors 實作
  - Error Handling 策略
  - TypeScript 類型定義
- [Component Architecture](../../docs/technical-implementation/02-frontend-react/03-component-architecture.md)
  - Smart/Dumb Components 分離
  - Component Composition
  - Props 設計原則
  - Component Reusability

## API 設計與規範

- [RESTful API Standards](../../docs/technical-implementation/05-api-design/restful-api-standards.md)
  - REST 設計原則
  - HTTP Methods 使用規範
  - Status Codes 標準
  - 資源命名規範
- [API Documentation](../../docs/technical-implementation/05-api-design/api-documentation.md)
  - Swagger/OpenAPI 配置
  - API 端點文檔標準
  - Request/Response 範例
  - Error Response 格式
- [Error Handling](../../docs/technical-implementation/05-api-design/error-handling.md)
  - Result Pattern 實作
  - Exception Handling 策略
  - Error Response 標準化
  - Logging 整合

## 資料庫設計與 EF Core

- [Database Design Principles](../../docs/technical-implementation/06-database-standards/database-design-principles.md)
  - 資料庫設計最佳實踐
  - 正規化原則
  - 索引設計策略
  - 效能優化建議
- [Entity Framework Core Configuration](../../docs/technical-implementation/06-database-standards/entity-framework-core-configuration.md)
  - Fluent API 配置
  - Value Converters 實作 (JSON, SemVer)
  - Entity Relationships 配置
  - Query Performance
- [Database Migration Strategy](../../docs/technical-implementation/06-database-standards/database-migration-strategy.md)
  - EF Core Migrations 工作流
  - Migration 命名規範
  - 資料遷移腳本
  - Rollback 策略

## 測試策略與實施

- [Testing Strategy](../../docs/technical-implementation/07-testing-strategy/README.md)
  - 測試金字塔
  - 覆蓋率目標 (80%+)
  - 測試分層策略
  - TDD 實踐
- [Unit Testing Standards](../../docs/technical-implementation/07-testing-strategy/unit-testing-standards.md)
  - xUnit 測試框架
  - Moq 模擬框架
  - AAA 模式 (Arrange-Act-Assert)
  - Test Naming 規範
- [Integration Testing Standards](../../docs/technical-implementation/07-testing-strategy/integration-testing-standards.md)
  - WebApplicationFactory 使用
  - TestContainers 整合
  - Database Seeding 策略
  - API 測試實作
- [Test Coverage Strategy](../../docs/technical-implementation/07-testing-strategy/test-coverage-strategy.md)
  - Coverage 測量工具
  - 覆蓋率報告
  - 關鍵路徑覆蓋
  - 品質閾值

## UX 設計參考

- [Wireframe: Conversation](../../docs/ux-design/wireframes/low-fidelity/05-conversation.md)
  - Chat UI 佈局設計
  - Message List 組件
  - Input Box 設計
  - Markdown 渲染
- [Wireframe: Agent Detail](../../docs/ux-design/wireframes/low-fidelity/04-agent-detail.md)
  - Agent 執行監控介面
  - 執行歷史展示
  - 效能指標圖表
  - 即時狀態顯示
- [Design System](../../docs/ux-design/design-system/README.md)
  - Material-UI 主題配置
  - 色彩系統
  - 字體系統
  - 間距系統
- [Component Library](../../docs/ux-design/design-system/component-library.md)
  - Button 組件規範
  - Input 組件規範
  - Card 組件規範
  - Modal 組件規範

## 變更管理與風險

- [Change Log](../../4-changes/CHANGE-LOG.md)
  - CHANGE-001: US 1.4 範圍擴展 (+8 SP)
  - CHANGE-002: US 2.1 延伸至 US 2.2/2.3
- [Risk Register](../../1-planning/RISK-REGISTER.md)
  - Sprint 2 技術風險評估
  - SignalR 整合風險
  - Plugin 動態加載風險
  - 緩解策略
- [Technical Decisions Log](../../1-planning/TECHNICAL-DECISIONS-LOG.md)
  - SignalR vs SSE 技術決策
  - AssemblyLoadContext 選擇
  - SemVer 版本管理策略

## 執行完成報告

- [US-1.4-Phase1-Summary.md](../../7-archive/US-1.4-Phase1-Summary.md) - 基礎執行引擎
- [US-1.4-Phase2-Summary.md](../../7-archive/US-1.4-Phase2-Summary.md) - 執行歷史追蹤
- [US-1.4-Phase3-Performance-Metrics-Summary.md](../../7-archive/US-1.4-Phase3-Performance-Metrics-Summary.md) - 效能指標
- [US-1.4-Phase4-Realtime-Monitoring-Export-Summary.md](../../7-archive/US-1.4-Phase4-Realtime-Monitoring-Export-Summary.md) - 即時監控與匯出

---

**維護說明**: 本文檔為 Sprint 2 的詳細執行計劃,在 Sprint 進行中持續更新。
**最後更新**: 2025-12-11
**版本**: v2.0 (新增完整參考文獻索引 - 50+ 文檔)
