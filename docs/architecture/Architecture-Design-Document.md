# Architecture Design Document (ADD)

**Semantic Kernel Agentic Framework**

**版本**: 1.0.0
**日期**: 2025-10-28
**狀態**: ✅ 已批准
**作者**: System Architect (BMad Method)

---

## 文檔概覽

本文檔是 Semantic Kernel Agentic Framework 的完整架構設計文檔（Architecture Design Document, ADD），基於 BMad Method 的 System Architect Persona，在 **Project Brief** 基礎上進行的詳細架構設計。

[📚 返回主索引](../README.md) | [📋 User Stories](../user-stories/README.md) | [🎯 MVP 規劃](../user-stories/mvp-planning.md) | [📋 實施策略](../user-stories/implementation-strategy.md)

### 文檔結構

```
docs/
├── README.md                         # 主索引 - 完整導航和進度追蹤
├── brief.md                          # Project Brief (Analyst 完成)
├── user-stories/                     # User Stories (Product Owner 完成)
│   ├── README.md                     # User Stories 總覽
│   ├── implementation-strategy.md    # 技術實施策略
│   ├── mvp-planning.md               # MVP 範圍與時程
│   ├── modules/                      # 10 個功能模組
│   └── sprints/                      # Sprint 規劃
└── architecture/
    ├── Architecture-Design-Document.md   # 本文檔（總體概覽）
    ├── ADR-006-agent-state-management.md  # Agent 狀態管理決策
    ├── ADR-007-multi-agent-communication.md  # Multi-Agent 通訊決策
    ├── ADR-008-code-interpreter-execution-model.md  # Code Interpreter 執行決策
    ├── ADR-011-framework-migration-strategy.md  # Framework 遷移策略決策
    ├── C4-architecture-diagrams.md   # C4 Model 架構圖
    ├── openapi-specification.yaml    # OpenAPI 3.0 API 規範
    ├── database-schema.md            # PostgreSQL 數據庫設計
    └── performance-scalability-strategy.md  # 性能與擴展性策略
```

---

## 1. 執行摘要

### 1.1 項目概覽

**Semantic Kernel Agentic Framework** 是一個基於 Microsoft Semantic Kernel 的企業級 Multi-Agent 協作框架，專為彌補 Microsoft Copilot Studio 的核心能力缺失而設計。

**核心差異化能力**：
1. **Multi-Agent Orchestration**：多 Agent 協作編排
2. **Code Interpreter**：Python/R 代碼執行（數據分析）
3. **自定義 Plugin 系統**：靈活的擴展能力
4. **企業級安全性**：4 層安全防護

**目標用戶**：
- IT 開發者：創建和配置 Agent
- 業務分析師：設計業務流程
- 業務用戶：使用 Agent 完成任務

### 1.2 架構目標

```yaml
性能目標:
  API 響應時間: P95 <300ms
  Agent 執行時間: P95 <5s
  Code Interpreter: P95 <8s

可用性目標:
  MVP: 99.5%
  Phase 2: 99.9%

擴展性目標:
  MVP: 20 並發 Agent
  Phase 2: 200 並發 Agent

安全性目標:
  - 4 層 Code Interpreter 安全防護
  - OAuth 2.0 認證（Microsoft Entra ID）
  - 最小權限原則（RBAC）
```

---

## 2. 核心架構決策（ADRs）

本項目通過四個核心 Architecture Decision Records (ADRs) 定義關鍵技術決策：

### 2.1 ADR-006: Agent 狀態管理策略

**文檔**: [`ADR-006-agent-state-management.md`](./ADR-006-agent-state-management.md)

**決策**: Hybrid（混合模式）

```yaml
架構:
  短期執行 Agent (<5分鐘):
    狀態存儲: Redis (內存緩存)
    TTL: 30 分鐘
    持久化: 僅在執行完成時寫入 PostgreSQL

  長期運行 Agent (>5分鐘):
    狀態存儲: PostgreSQL (持久化)
    實時更新: 每 30 秒或關鍵節點
    緩存: Redis 緩存最近訪問的狀態

  關鍵狀態:
    雙寫: Redis + PostgreSQL（確保不丟失）
```

**理由**：
- ✅ 低延遲：大部分請求從 Redis 讀取（<1ms）
- ✅ 高可靠性：關鍵狀態持久化到 PostgreSQL
- ✅ 成本優化：短期 Agent 不浪費 DB 寫入

**實現**：`IAgentStateStore` 接口 + `HybridAgentStateStore` 實現

---

### 2.2 ADR-007: Multi-Agent 通訊機制

**文檔**: [`ADR-007-multi-agent-communication.md`](./ADR-007-multi-agent-communication.md)

**決策**: Phase-Based Strategy（階段性策略）

```yaml
Phase 1 (MVP - Month 1-8):
  技術選型: MediatR (內存 Event Bus)
  部署模式: 單進程（Docker Compose）
  通訊模式: In-Process Events

Phase 2 (Kubernetes - Month 9-18):
  技術選型: Azure Service Bus (Message Queue)
  部署模式: 多實例（Kubernetes Pods）
  通訊模式: Distributed Messaging

遷移策略:
  抽象層: IAgentMessaging 接口
  實現切換: MediatR → Azure Service Bus（無需修改業務邏輯）
```

**理由**：
- ✅ 低風險 MVP：MediatR 成熟、實現簡單
- ✅ 平滑遷移：通過抽象層無痛切換
- ✅ 高擴展性：Phase 2 支持分布式部署

**實現**：`IAgentMessaging` 接口 + `MediatRAgentMessaging` / `ServiceBusAgentMessaging`

---

### 2.3 ADR-008: Code Interpreter 執行模型

**文檔**: [`ADR-008-code-interpreter-execution-model.md`](./ADR-008-code-interpreter-execution-model.md)

**決策**: Hybrid Container Pool（混合容器池）

```yaml
架構:
  Container Pool:
    預熱池大小: 5 個 Standby Containers
    啟動時間: <2 秒（從池中獲取）
    安全策略: 執行後銷毀，不重用
    補充策略: 異步非阻塞補充

  安全防護（4 層）:
    Layer 1: Network Isolation（無外網訪問）
    Layer 2: Resource Limits（CPU/Memory/Disk）
    Layer 3: Execution Timeout（30秒強制終止）
    Layer 4: Read-Only Filesystem（僅 /tmp 可寫）
```

**理由**：
- ✅ 低延遲：從 Pool 獲取容器 <2 秒
- ✅ 高安全性：4 層防護 + 執行後銷毀
- ✅ 高吞吐量：5 個 Standby 容器並行處理

**實現**：`ICodeInterpreterPool` 接口 + `HybridCodeInterpreterPool` + `DockerCodeInterpreter`

---

### 2.4 ADR-011: Framework 遷移策略與抽象層設計

**文檔**: [`ADR-011-framework-migration-strategy.md`](./ADR-011-framework-migration-strategy.md)

**背景**: Microsoft 於 2025年10月1日發布了 Microsoft Agent Framework（整合 Semantic Kernel + AutoGen），需要評估是否遷移。

**決策**: Semantic Kernel 1.x + Framework Abstraction Layer + Open Standards 策略

```yaml
Phase 1 (MVP - Month 1-8):
  核心框架: Semantic Kernel 1.x（穩定性優先）
  抽象層: 5 個核心接口（IAgentRuntime、IToolRegistry、IAgentMemory、IWorkflowOrchestrator、IObservability）
  開放標準: MCP、OpenAPI、OpenTelemetry

Phase 2 (評估期 - Month 9-12):
  評估條件:
    - Microsoft Agent Framework 穩定運行 >6 個月
    - 3+ Fortune 500 企業生產案例
    - GitHub Issues 關閉率 >90%
    - 遷移成本 <2 個月
    - 性能提升 >20%

Phase 3 (遷移期 - Month 13-15，可選):
  實現: MicrosoftAgentFrameworkAdapter
  灰度切換: 10% → 50% → 100% 流量
  回滾能力: Feature Flag 支持一鍵回滾

Phase 4 (優化期 - Month 16+):
  移除舊適配器（如遷移成功）
  利用新框架特性優化
```

**理由**：
- ✅ 穩定性優先：Semantic Kernel 1.x 已驗證，降低 MVP 風險
- ✅ 未來靈活性：抽象層支持未來平滑遷移，無需重寫業務邏輯
- ✅ 開放標準：MCP、OpenAPI、OpenTelemetry 提升互操作性，避免供應商鎖定
- ✅ 平滑遷移：Feature Flag 支持灰度切換，降低遷移風險

**實現**：Framework Abstraction Layer（詳見 Section 3.3）+ Adapter Pattern

---

## 3. 系統架構

### 3.1 C4 Model 架構圖

**文檔**: [`C4-architecture-diagrams.md`](./C4-architecture-diagrams.md)

本文檔包含完整的 C4 Model 架構可視化：

1. **Level 1: System Context**
   - 展示系統與用戶、外部系統的交互
   - 用戶：業務用戶、IT 開發者、業務分析師
   - 外部系統：Azure OpenAI、Azure AI Search、Azure Blob Storage、Entra ID

2. **Level 2: Container**
   - 前端層：Web UI, Chat UI
   - 後端層：API Gateway, Agent Orchestrator, Plugin Registry, Code Interpreter
   - 消息層：Event Bus (MediatR) / Message Broker (Service Bus)
   - 存儲層：Redis, PostgreSQL, Vector DB
   - 基礎設施：Container Pool, Monitoring

3. **Level 3: Component**（Agent Orchestrator）
   - Agent Engine：Semantic Kernel Agent 執行引擎
   - Task Planner：任務分解與規劃
   - Plugin Executor：Plugin 調用管理
   - Memory Manager：對話記憶管理
   - State Manager：Agent 狀態管理
   - Messaging Hub：Agent 通訊中心

4. **部署架構**
   - Phase 1: Docker Compose 單機部署
   - Phase 2: Kubernetes 多副本部署

5. **數據流圖**
   - Agent 執行流程（單 Agent）
   - Multi-Agent 協作流程
   - Code Interpreter 執行流程

6. **安全架構**
   - 4 層安全防護可視化

7. **監控與可觀察性架構**
   - Application Insights + Grafana

---

### 3.2 6 層架構設計

基於 Project Brief Section 7.1，本架構實現完整的 6 層設計：

```
┌─────────────────────────────────────────────────────────┐
│  Layer 6: Presentation Layer (前端層)                   │
│  - Web UI (React + TypeScript)                          │
│  - Chat UI (React + SignalR)                            │
└─────────────────────────────────────────────────────────┘
                         ↓ HTTPS/WebSocket
┌─────────────────────────────────────────────────────────┐
│  Layer 5: API Gateway Layer (API 網關層)                │
│  - .NET 8 Minimal API                                   │
│  - 認證授權（OAuth 2.0）                                 │
│  - 路由與限流                                            │
└─────────────────────────────────────────────────────────┘
                         ↓ 內部調用
┌─────────────────────────────────────────────────────────┐
│  Layer 4: Agent Orchestration Layer (編排層)            │
│  - Agent Orchestrator (.NET 8 + Semantic Kernel)       │
│  - Task Planner                                         │
│  - Messaging Hub (MediatR / Service Bus)                │
└─────────────────────────────────────────────────────────┘
                         ↓ 調用
┌─────────────────────────────────────────────────────────┐
│  Layer 3: Plugin Execution Layer (插件層)               │
│  - Plugin Registry                                      │
│  - Code Interpreter Service                            │
│  - File Search Plugin                                   │
│  - Custom Plugins                                       │
└─────────────────────────────────────────────────────────┘
                         ↓ 調用
┌─────────────────────────────────────────────────────────┐
│  Layer 2: Integration Layer (集成層)                    │
│  - Azure OpenAI Connector                              │
│  - Azure AI Search Connector                           │
│  - Azure Blob Storage Connector                        │
│  - Docker API Connector                                │
└─────────────────────────────────────────────────────────┘
                         ↓ 調用
┌─────────────────────────────────────────────────────────┐
│  Layer 1: Data Layer (數據層)                           │
│  - PostgreSQL (持久化存儲)                               │
│  - Redis (狀態緩存)                                     │
│  - Azure AI Search (向量檢索)                           │
└─────────────────────────────────────────────────────────┘
```

---

### 3.3 Framework Abstraction Layer ⭐ 遷移策略

**目的**: 為支持未來從 Semantic Kernel 遷移到 Microsoft Agent Framework（如需要），設計框架無關的抽象層。

**設計原則**:
- 🎯 **框架解耦**: 核心業務邏輯不直接依賴任何特定框架
- 🔄 **平滑遷移**: 切換框架時僅需實現新的適配器，無需重寫業務邏輯
- 📖 **開放標準**: 優先使用 MCP、OpenAPI、OpenTelemetry 等行業標準
- 🧪 **可測試性**: 抽象層便於單元測試和模擬

#### 3.3.1 抽象層架構

```
┌──────────────────────────────────────────────────────────────┐
│  Application Layer (業務邏輯層)                               │
│  - Agent 管理服務                                             │
│  - Multi-Agent 編排服務                                       │
│  - Plugin 管理服務                                            │
│  - Knowledge 檢索服務                                         │
└──────────────────────────────────────────────────────────────┘
                          ↓ 依賴接口（不依賴具體實現）
┌──────────────────────────────────────────────────────────────┐
│  Framework Abstraction Layer (框架抽象層) ← 新增              │
│  - IAgentRuntime: Agent 執行抽象                             │
│  - IToolRegistry: 工具註冊抽象                               │
│  - IAgentMemory: 記憶體管理抽象                              │
│  - IWorkflowOrchestrator: 工作流編排抽象                     │
│  - IObservability: 可觀測性抽象                              │
└──────────────────────────────────────────────────────────────┘
                          ↓ 實現接口
┌──────────────────────────────────────────────────────────────┐
│  Framework Adapters (框架適配器層)                            │
│                                                               │
│  [Phase 1 - MVP]                                             │
│  ├─ SemanticKernelAdapter: Semantic Kernel 1.x 實現         │
│  │  └─ 實現所有抽象接口                                      │
│                                                               │
│  [Phase 2 - 可選遷移]                                         │
│  ├─ MicrosoftAgentFrameworkAdapter: MAF 實現 (如果需要)      │
│  │  └─ 實現相同的抽象接口                                    │
│  │                                                            │
│  ├─ HybridAdapter: 混合模式 (兩個框架並行)                   │
│  │  └─ Feature Flag 控制運行時選擇                          │
└──────────────────────────────────────────────────────────────┘
                          ↓ 調用
┌──────────────────────────────────────────────────────────────┐
│  External Services (外部服務)                                 │
│  - Azure OpenAI                                               │
│  - Azure AI Search                                            │
│  - Docker (Code Interpreter)                                  │
└──────────────────────────────────────────────────────────────┘
```

---

#### 3.3.2 核心抽象接口定義

##### IAgentRuntime - Agent 執行抽象

```csharp
/// <summary>
/// Agent 運行時抽象，支持多種框架實現
/// </summary>
public interface IAgentRuntime
{
    /// <summary>
    /// 執行 Agent 任務
    /// </summary>
    Task<AgentResponse> ExecuteAsync(
        AgentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 獲取 Agent 狀態
    /// </summary>
    Task<AgentState> GetStateAsync(string agentId);

    /// <summary>
    /// 恢復 Agent 執行（支持長時間運行任務）
    /// </summary>
    Task<AgentResponse> ResumeAsync(
        string executionId,
        CancellationToken cancellationToken = default);
}

// Semantic Kernel 實現
public class SemanticKernelRuntime : IAgentRuntime
{
    private readonly Kernel _kernel;

    public async Task<AgentResponse> ExecuteAsync(
        AgentRequest request,
        CancellationToken cancellationToken)
    {
        // 使用 Semantic Kernel 執行
        var result = await _kernel.InvokeAsync(request.Prompt, cancellationToken);
        return new AgentResponse { Content = result.ToString() };
    }
    // ...
}

// Microsoft Agent Framework 實現（未來）
public class MicrosoftAgentRuntime : IAgentRuntime
{
    private readonly AgentHost _agentHost;

    public async Task<AgentResponse> ExecuteAsync(
        AgentRequest request,
        CancellationToken cancellationToken)
    {
        // 使用 Microsoft Agent Framework 執行
        var result = await _agentHost.RunAsync(request, cancellationToken);
        return new AgentResponse { Content = result.Output };
    }
    // ...
}
```

---

##### IToolRegistry - 工具註冊抽象（支持 MCP）

```csharp
/// <summary>
/// 工具註冊抽象，支持 MCP (Model Context Protocol) 標準
/// </summary>
public interface IToolRegistry
{
    /// <summary>
    /// 註冊 MCP 工具
    /// </summary>
    Task RegisterMcpToolAsync(McpToolDefinition tool);

    /// <summary>
    /// 從 OpenAPI 規範自動註冊工具
    /// </summary>
    Task RegisterFromOpenApiAsync(string openApiSpecUrl);

    /// <summary>
    /// 獲取可用工具列表
    /// </summary>
    Task<IEnumerable<ToolDefinition>> GetAvailableToolsAsync();

    /// <summary>
    /// 調用工具
    /// </summary>
    Task<ToolResult> InvokeToolAsync(
        string toolName,
        Dictionary<string, object> parameters);
}

// MCP Tool Definition (標準化格式)
public record McpToolDefinition
{
    public string Name { get; init; }
    public string Description { get; init; }
    public JsonSchema InputSchema { get; init; }  // JSON Schema for parameters
    public McpTransport Transport { get; init; }  // stdio, HTTP, SSE
    public string Endpoint { get; init; }
}
```

---

##### IAgentMemory - 記憶體管理抽象

```csharp
/// <summary>
/// Agent 記憶體抽象，支持多種向量資料庫
/// </summary>
public interface IAgentMemory
{
    /// <summary>
    /// 儲存對話歷史
    /// </summary>
    Task StoreConversationAsync(string agentId, ConversationTurn turn);

    /// <summary>
    /// 檢索相關記憶
    /// </summary>
    Task<IEnumerable<MemoryRecord>> RetrieveAsync(
        string query,
        int topK = 5,
        double minRelevanceScore = 0.7);

    /// <summary>
    /// 儲存 Knowledge 文件
    /// </summary>
    Task StoreKnowledgeAsync(string collectionName, KnowledgeDocument document);

    /// <summary>
    /// 清除記憶體
    /// </summary>
    Task ClearMemoryAsync(string agentId);
}

// 配置支持多種向量資料庫
public enum MemoryProvider
{
    AzureAISearch,      // Azure AI Search (推薦)
    Redis,              // Redis with Vector Similarity
    Pinecone,           // Pinecone
    Qdrant,             // Qdrant
    Weaviate            // Weaviate
}
```

---

##### IWorkflowOrchestrator - 工作流編排抽象

```csharp
/// <summary>
/// Multi-Agent 工作流編排抽象
/// </summary>
public interface IWorkflowOrchestrator
{
    /// <summary>
    /// 執行工作流（支持多種編排模式）
    /// </summary>
    Task<WorkflowResult> ExecuteWorkflowAsync(
        WorkflowDefinition workflow,
        WorkflowContext context);

    /// <summary>
    /// 暫停工作流（支持長時間運行任務）
    /// </summary>
    Task<string> PauseWorkflowAsync(string workflowId);

    /// <summary>
    /// 恢復工作流
    /// </summary>
    Task<WorkflowResult> ResumeWorkflowAsync(string workflowId);
}

// 工作流定義（聲明式，支持 YAML）
public record WorkflowDefinition
{
    public string Name { get; init; }
    public OrchestrationMode Mode { get; init; }  // Sequential, Concurrent, GroupChat, Supervisor
    public List<AgentStep> Steps { get; init; }
    public Dictionary<string, object> Configuration { get; init; }
}

// 編排模式（兼容 Microsoft Agent Framework）
public enum OrchestrationMode
{
    Sequential,         // 順序執行
    Concurrent,         // 並發執行
    GroupConversation,  // 群組對話 (來自 AutoGen)
    Handoff,            // 移交模式
    Supervisor          // 管理器協調 (來自 AutoGen)
}
```

---

##### IObservability - 可觀測性抽象（OpenTelemetry）

```csharp
/// <summary>
/// 可觀測性抽象，基於 OpenTelemetry 標準
/// </summary>
public interface IObservability
{
    /// <summary>
    /// 開始追蹤 Span
    /// </summary>
    IDisposable StartSpan(string operationName, Dictionary<string, object> attributes = null);

    /// <summary>
    /// 記錄指標
    /// </summary>
    void RecordMetric(string metricName, double value, Dictionary<string, string> tags = null);

    /// <summary>
    /// 記錄錯誤
    /// </summary>
    void RecordError(Exception exception, Dictionary<string, object> context = null);

    /// <summary>
    /// 記錄 LLM 調用
    /// </summary>
    Task RecordLlmCallAsync(LlmCallMetadata metadata);
}

// LLM 調用元數據（標準化）
public record LlmCallMetadata
{
    public string Model { get; init; }
    public int InputTokens { get; init; }
    public int OutputTokens { get; init; }
    public TimeSpan Latency { get; init; }
    public bool Success { get; init; }
    public string ErrorMessage { get; init; }
}
```

---

#### 3.3.3 依賴注入配置

```csharp
// Program.cs - 框架切換通過配置完成

var builder = WebApplication.CreateBuilder(args);

// 從配置讀取使用哪個框架
var frameworkConfig = builder.Configuration.GetSection("AgentFramework");
var frameworkType = frameworkConfig.GetValue<string>("Type"); // "SemanticKernel" or "MicrosoftAgentFramework"

// 註冊抽象層實現（依賴注入）
switch (frameworkType)
{
    case "SemanticKernel":
        builder.Services.AddSemanticKernelAdapter();  // 使用 Semantic Kernel
        break;

    case "MicrosoftAgentFramework":
        builder.Services.AddMicrosoftAgentAdapter();  // 使用 Microsoft Agent Framework
        break;

    case "Hybrid":
        builder.Services.AddHybridAdapter();          // 混合模式（Feature Flag 控制）
        break;
}

// 註冊開放標準支持
builder.Services.AddMcpToolRegistry();           // MCP 工具註冊
builder.Services.AddOpenTelemetry();             // OpenTelemetry 可觀測性

// 業務服務（不依賴具體框架）
builder.Services.AddScoped<IAgentService, AgentService>();
builder.Services.AddScoped<IMultiAgentService, MultiAgentService>();
```

---

#### 3.3.4 遷移路徑

```yaml
Phase_1_MVP: "Month 1-12"
  Framework: Semantic Kernel 1.x
  狀態: 穩定基礎
  行動:
    - 實現所有抽象接口（SemanticKernelAdapter）
    - 採用 MCP、OpenAPI、OpenTelemetry 標準
    - 確保業務邏輯與框架解耦

Phase_2_Evaluation: "Month 12-15"
  Framework: 評估 Microsoft Agent Framework
  條件:
    ✅ 框架穩定運行 >6 個月
    ✅ 有 3+ 個企業生產案例
    ✅ GitHub Issues 關閉率 >90%
    ✅ 遷移成本 <2 個月
  行動:
    - 建立 PoC 驗證專案
    - 實現 MicrosoftAgentFrameworkAdapter
    - 性能和穩定性對比測試

Phase_3_Migration: "Month 15-18 (如果決定遷移)"
  Framework: 混合模式 → Microsoft Agent Framework
  策略:
    - 分模組逐步遷移（監控 → Plugin → Multi-Agent）
    - 雙運行時並行 3-6 個月（Feature Flag 控制）
    - A/B 測試驗證穩定性
    - 保留回滾能力

Phase_4_Optimization: "Month 18+"
  Framework: 根據評估結果
  選項:
    A. 完全遷移到 Microsoft Agent Framework
    B. 保持 Semantic Kernel（如果 MAF 不符合需求）
    C. 長期維持混合模式（不同場景使用不同框架）
```

---

#### 3.3.5 Feature Flag 配置範例

```yaml
# appsettings.json - 運行時切換框架

{
  "AgentFramework": {
    "Type": "SemanticKernel",  # 選項: "SemanticKernel", "MicrosoftAgentFramework", "Hybrid"

    "SemanticKernel": {
      "Version": "1.x",
      "EnableExperimentalFeatures": false
    },

    "MicrosoftAgentFramework": {
      "Version": "preview",
      "EnableDurableExecution": true,
      "EnableA2ACommunication": true
    },

    "Hybrid": {
      "DefaultRuntime": "SemanticKernel",
      "FeatureFlags": {
        "UseMAFForMonitoring": true,        # 監控使用 MAF (OpenTelemetry)
        "UseMAFForPlugins": false,          # Plugin 仍使用 SK
        "UseMAFForMultiAgent": false        # Multi-Agent 仍使用 SK
      }
    }
  },

  "OpenStandards": {
    "MCP": {
      "Enabled": true,
      "DiscoveryEndpoint": "https://mcp.example.com/discover"
    },
    "OpenTelemetry": {
      "Enabled": true,
      "Endpoint": "http://localhost:4317",
      "ServiceName": "semantic-kernel-framework"
    }
  }
}
```

---

#### 3.3.6 測試策略

```csharp
// 單元測試 - 使用 Mock 適配器，不依賴具體框架

public class AgentServiceTests
{
    [Fact]
    public async Task ExecuteAgent_ShouldReturnResponse()
    {
        // Arrange
        var mockRuntime = new Mock<IAgentRuntime>();
        mockRuntime
            .Setup(r => r.ExecuteAsync(It.IsAny<AgentRequest>(), default))
            .ReturnsAsync(new AgentResponse { Content = "Test Response" });

        var agentService = new AgentService(mockRuntime.Object);

        // Act
        var result = await agentService.ExecuteAgentAsync("test-agent", "Hello");

        // Assert
        Assert.Equal("Test Response", result.Content);
        mockRuntime.Verify(r => r.ExecuteAsync(It.IsAny<AgentRequest>(), default), Times.Once);
    }
}

// 整合測試 - 可以切換真實框架測試

[Collection("Integration")]
public class AgentServiceIntegrationTests
{
    [Theory]
    [InlineData("SemanticKernel")]
    [InlineData("MicrosoftAgentFramework")]  // 未來添加
    public async Task ExecuteAgent_WithRealFramework_ShouldWork(string frameworkType)
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddAgentFramework(frameworkType);
        var provider = services.BuildServiceProvider();
        var agentService = provider.GetRequiredService<IAgentService>();

        // Act
        var result = await agentService.ExecuteAgentAsync("test-agent", "Hello");

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Content);
    }
}
```

---

#### 3.3.7 風險緩解

| 風險 | 緩解策略 |
|------|----------|
| 抽象層增加複雜度 | - 保持接口簡潔，僅抽象核心功能<br>- 詳細文檔和範例代碼<br>- 定期 Review 和重構 |
| 性能損失 | - 抽象層設計輕量級，避免過度包裝<br>- 性能測試對比（有/無抽象層）<br>- 關鍵路徑優化 |
| 維護兩套適配器 | - 僅在必要時實現第二個適配器<br>- 共享通用邏輯<br>- 自動化測試覆蓋 |
| 框架特性無法抽象 | - 允許適配器提供擴展接口<br>- 漸進式遷移，不強制一次性完成<br>- 保留直接調用框架的逃生艙口 |

---

#### 3.3.8 預期收益

✅ **降低遷移成本**: 切換框架時僅需實現新適配器，業務邏輯不變
✅ **提高可測試性**: 使用 Mock 適配器，單元測試不依賴真實框架
✅ **保持靈活性**: 未來可以根據需求選擇最佳框架，甚至混合使用
✅ **標準化**: 採用 MCP、OpenAPI、OpenTelemetry 等行業標準
✅ **風險控制**: 漸進式遷移，隨時可以回滾

---

## 4. API 設計

### 4.1 OpenAPI 3.0 規範

**文檔**: [`openapi-specification.yaml`](./openapi-specification.yaml)

本規範定義了完整的 RESTful API，包括：

**核心端點**：
- `/agents` - Agent 管理（CRUD）
- `/agents/{id}/execute` - Agent 執行（同步）
- `/agents/{id}/execute-async` - Agent 異步執行
- `/plugins` - Plugin 管理
- `/code-interpreter/execute` - 代碼執行
- `/chat/completions` - 對話與 Streaming
- `/orchestration/execute` - Multi-Agent 編排
- `/health` - 健康檢查
- `/metrics` - Prometheus 指標

**認證方式**：
OAuth 2.0（Microsoft Entra ID）

**API 版本**：
v1（URL 前綴：`/v1`）

**響應格式**：
JSON（標準 RESTful）

**Streaming 支持**：
Server-Sent Events (SSE)（`text/event-stream`）

---

## 5. 數據庫設計

### 5.1 PostgreSQL Schema

**文檔**: [`database-schema.md`](./database-schema.md)

本文檔定義了完整的數據庫架構，包括：

**核心表（12 個）**：
1. `users` - 用戶基本信息
2. `agents` - Agent 配置
3. `plugins` - Plugin 註冊
4. `agent_plugins` - Agent-Plugin 關聯
5. `agent_executions` - 執行歷史
6. `agent_states` - Agent 狀態（實現 ADR-006）
7. `plugin_invocations` - Plugin 調用記錄
8. `code_executions` - Code Interpreter 執行記錄
9. `conversations` - 對話會話
10. `messages` - 消息記錄
11. `files` - 文件管理
12. `file_references` - 文件引用

**ER 圖**：
完整的實體關係圖（Mermaid 格式）

**索引策略**：
- 主鍵索引（UUID）
- 外鍵索引
- 複合索引（支持多條件查詢）
- 部分索引（減少索引大小）
- 覆蓋索引（避免回表）
- JSONB GIN 索引（復雜查詢）

**數據完整性**：
- 外鍵約束（CASCADE / SET NULL）
- 檢查約束（CHECK）
- 唯一約束（UNIQUE）
- 自動更新觸發器（`updated_at`）

**優化策略**：
- 連接池（PgBouncer）
- 讀寫分離（Phase 2）
- 分區（按時間，Phase 3）
- 數據歸檔（6 個月）

---

## 6. 性能與擴展性

### 6.1 性能與擴展性策略

**文檔**: [`performance-scalability-strategy.md`](./performance-scalability-strategy.md)

本文檔定義了完整的性能優化和擴展性設計：

**1. 緩存策略**：
- L1: 內存緩存（Memory Cache, 5 分鐘 TTL）
- L2: 分布式緩存（Redis, 30 分鐘 TTL）
- 緩存命中率目標：>90%
- 緩存鍵命名規範
- 緩存預熱與失效策略

**2. 數據庫優化**：
- 連接池配置（Npgsql + PgBouncer）
- 查詢優化（批量查詢、Keyset 分頁、投影查詢）
- 索引優化（復合索引、部分索引、覆蓋索引）
- 讀寫分離（Phase 2）
- 分片策略（Phase 3）

**3. 消息隊列優化**：
- MediatR：並行 Notification 處理
- Azure Service Bus：批量發送、會話、死信隊列

**4. 水平擴展策略**：
- 無狀態 API 設計
- Kubernetes HPA（CPU/Memory 自動伸縮）
- Kubernetes VPA（資源請求自動調整）
- 數據庫水平擴展（Sharding）

**5. 性能監控**：
- Application Insights 集成
- OpenTelemetry Distributed Tracing
- 關鍵性能指標（KPI）
- Prometheus + Grafana Dashboard

**6. 負載測試**：
- k6 測試腳本
- 測試場景（API 壓力測試、Agent 並發執行、Code Interpreter 壓力測試）

**7. 成本優化**：
- Azure 資源優化（Reserved Instances, Spot Instances）
- LLM 成本優化（Prompt 緩存、模型選擇、Token 優化）

**8. 擴展路徑**：
- MVP → Phase 2 灰度遷移策略

---

## 7. 安全架構

### 7.1 4 層安全防護

基於 ADR-008 和 C4 架構圖，本系統實現了完整的 4 層安全防護：

**Application Security（應用層安全）**：
- 身份認證：Microsoft Entra ID（OAuth 2.0）
- 角色授權：RBAC（Role-Based Access Control）
- 輸入驗證：Anti-XSS、Anti-SQLi

**Code Interpreter Security（代碼執行安全）**：
- **Layer 1: Network Isolation** - Docker Network Mode = none（無外網訪問）
- **Layer 2: Resource Limits** - CPU/Memory/Disk/Pids 限制
- **Layer 3: Execution Timeout** - 30 秒強制終止
- **Layer 4: Read-Only Filesystem** - 僅 /tmp 可寫（100MB, noexec）

**Infrastructure Security（基礎設施安全）**：
- TLS 1.3 加密（HTTPS/WSS）
- 防火牆（NSG + WAF）
- DDoS 防護（Azure DDoS Protection）

**Data Security（數據安全）**：
- 數據加密（AES-256）
- 密鑰管理（Azure Key Vault）
- 數據備份（每日備份，30 天保留）

### 7.2 安全合規

```yaml
合規標準:
  - GDPR（一般數據保護條例）
  - SOC 2 Type II
  - ISO 27001

數據隱私:
  - 用戶數據加密存儲
  - 敏感數據脫敏（日誌）
  - 數據訪問審計（Audit Log）

漏洞管理:
  - 依賴掃描（Dependabot）
  - 容器掃描（Trivy）
  - SAST（靜態代碼分析）
  - DAST（動態安全測試）
```

---

## 8. 技術棧

### 8.1 後端技術棧

```yaml
核心框架:
  - .NET 8（C# 12）
  - ASP.NET Core Minimal API
  - Semantic Kernel 1.x + Framework Abstraction Layer（參見 3.3 節）

開放標準（Open Standards）:
  - MCP (Model Context Protocol)（Plugin/Tool 發現與調用）
  - OpenAPI 3.0+（API-First Tool Definition）
  - OpenTelemetry（可觀察性標準：日誌、指標、追蹤）

框架抽象層:
  - IAgentRuntime（Agent 執行抽象）
  - IToolRegistry（Tool 註冊與 MCP 支持）
  - IAgentMemory（記憶體管理）
  - IWorkflowOrchestrator（Multi-Agent 編排）
  - IObservability（OpenTelemetry 集成）

消息通訊:
  - Phase 1: MediatR 12.x（內存事件總線）
  - Phase 2: Azure Service Bus（分布式消息隊列）

數據存儲:
  - PostgreSQL 15（主數據庫）
  - Redis 7.x（狀態緩存）
  - Azure AI Search（向量檢索）
  - Azure Blob Storage（文件存儲）

容器化:
  - Docker（Code Interpreter）
  - Docker Compose（MVP 部署）
  - Kubernetes（Phase 2 部署）

監控與日誌:
  - OpenTelemetry（可觀察性標準層）
  - Application Insights（監控與追蹤）
  - Prometheus + Grafana（指標收集與可視化）
  - Serilog（結構化日誌）
```

### 8.2 前端技術棧

```yaml
框架:
  - React 18
  - TypeScript 5.x
  - Vite（構建工具）

UI 組件:
  - Material-UI (MUI)
  - React Query（數據獲取）
  - Zustand（狀態管理）

實時通訊:
  - SignalR（WebSocket）
  - Server-Sent Events（Streaming）
```

### 8.3 DevOps 工具

```yaml
CI/CD:
  - GitHub Actions
  - Azure DevOps Pipelines

基礎設施即代碼 (IaC):
  - Terraform
  - Bicep（Azure 專用）

容器編排:
  - Kubernetes (AKS)
  - Helm（Kubernetes 包管理）

測試:
  - xUnit（單元測試）
  - k6（負載測試）
  - Playwright（E2E 測試）
```

---

## 9. 部署架構

### 9.1 MVP 部署（Phase 1: Month 1-8）

```yaml
環境:
  - Docker Compose
  - 單機部署（Azure VM or Azure Container Instances）

組件:
  - Web UI（Port 3000）
  - API Gateway（Port 5000）
  - Agent Orchestrator（Port 5001）
  - Code Interpreter（Port 5002）
  - Redis（Port 6379）
  - PostgreSQL（Port 5432）
  - Container Pool（5 個 Standby Containers）

資源配置:
  - VM: Standard_D4s_v3（4 vCPUs, 16GB RAM）
  - PostgreSQL: Basic Tier（1 vCore, 2GB RAM）
  - Redis: Basic Tier（250MB）

成本:
  - ~$100/月（基礎設施 + LLM 調用）
```

### 9.2 Kubernetes 部署（Phase 2: Month 9-18）

```yaml
環境:
  - Azure Kubernetes Service (AKS)
  - 3 Nodes（Standard_D4s_v3）

組件:
  - Web UI (2 replicas, HPA: 2-5)
  - API Gateway (2 replicas, HPA: 2-10)
  - Agent Orchestrator (3 replicas, HPA: 3-20)
  - Code Interpreter (2 replicas, HPA: 2-10)
  - Redis Cluster (3 masters + 3 slaves)
  - PostgreSQL HA (Primary + Standby)

高可用性:
  - Multi-AZ 部署
  - 自動故障轉移
  - 滾動更新（Zero Downtime）

成本:
  - ~$680/月（基礎設施）
  - ~$500-1000/月（LLM 調用）
```

---

## 10. 監控與可觀察性

### 10.1 監控架構

```yaml
應用監控:
  - Application Insights（日誌、追蹤、指標）
  - OpenTelemetry（分布式追蹤）

基礎設施監控:
  - Azure Monitor（VM、AKS、Database）
  - Prometheus（Kubernetes 指標）
  - Grafana（可視化 Dashboard）

日誌管理:
  - Serilog（結構化日誌）
  - Log Analytics（日誌查詢與分析）

告警:
  - Azure Monitor Alerts
  - Email / Teams 通知
```

### 10.2 關鍵指標

```yaml
應用層指標:
  - API 響應時間（P50, P95, P99）
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
  - 慢查詢數量（>1 秒）
  - 索引命中率

緩存指標:
  - Redis 命中率（目標: >90%）
  - Redis 內存使用率
  - 緩存響應時間

業務指標:
  - 每日活躍用戶（DAU）
  - Agent 執行次數
  - Code Interpreter 使用率
  - Plugin 調用次數
```

---

## 11. 災難恢復與業務連續性

### 11.1 備份策略

```yaml
PostgreSQL 備份:
  - 頻率: 每日全量備份
  - 保留期: 30 天
  - RPO（恢復點目標）: 24 小時
  - RTO（恢復時間目標）: 4 小時

Redis 備份:
  - 頻率: 每 6 小時 RDB 快照
  - 保留期: 7 天
  - RPO: 6 小時
  - RTO: 1 小時

Azure Blob 備份:
  - 頻率: 連續複製（Geo-Redundant）
  - 保留期: 無限期
  - RPO: 近乎 0
  - RTO: <1 小時
```

### 11.2 災難恢復流程

1. **檢測**：監控系統自動檢測故障
2. **通知**：告警運維團隊
3. **評估**：評估影響範圍
4. **恢復**：
   - 數據庫故障 → 從備份恢復
   - 服務故障 → 重啟服務/容器
   - 基礎設施故障 → 切換到備用區域
5. **驗證**：驗證數據完整性
6. **監控**：持續監控直至完全恢復

---

## 12. 開發路線圖

### Phase 1: MVP - Semantic Kernel + Abstraction Layer（Month 1-8）

**目標**：驗證核心能力，快速交付 MVP，建立框架抽象層為未來遷移做準備

```yaml
Week 1-4: 基礎設施搭建
  - 創建 Azure 資源（PostgreSQL, Redis, Blob Storage）
  - 設置 Docker Compose 開發環境
  - 配置 CI/CD Pipeline
  - 設計 Framework Abstraction Layer 接口

Week 5-12: 核心功能開發 + 抽象層實現
  - 實現 IAgentRuntime, IToolRegistry, IAgentMemory 接口
  - 實現 SemanticKernelRuntime 適配器
  - Agent 管理 API
  - MediatR 內存事件總線
  - MCP Tool Registry 集成
  - OpenAPI 3.0+ Tool Definition

Week 13-20: Code Interpreter 開發
  - Docker Container Pool 實現
  - 4 層安全防護
  - Python/R 代碼執行
  - OpenTelemetry 可觀察性集成

Week 21-28: Multi-Agent 協作
  - 實現 IWorkflowOrchestrator 接口
  - Agent 編排引擎（支持 Sequential, Concurrent 模式）
  - Task Planner
  - Agent 狀態管理（Hybrid）

Week 29-32: 前端與集成測試
  - Web UI（Agent 管理界面）
  - Chat UI（對話界面）
  - 集成測試與修復
  - MCP Tool 發現與調用測試

Week 33-36: MVP 驗收與部署
  - 性能測試與優化
  - 安全測試
  - 生產環境部署（Docker Compose）
  - 用戶驗收測試（UAT）
  - Framework Abstraction Layer 驗證

交付成果:
  ✅ 完整的 MVP 功能（P0 + P1 User Stories）
  ✅ Framework Abstraction Layer（5 個核心接口）
  ✅ MCP、OpenAPI、OpenTelemetry 集成
  ✅ 系統穩定性 >99.5%
  ✅ 單元測試覆蓋率 >80%
```

### Phase 2: Kubernetes 遷移 + Framework 評估（Month 9-12）

**目標**：支持生產級負載，實現高可用性，評估 Microsoft Agent Framework

```yaml
Week 37-40: Kubernetes 準備
  - 創建 AKS 集群
  - 部署 PostgreSQL HA
  - 部署 Redis Cluster
  - 配置 Azure Service Bus

Week 41-44: 消息隊列遷移
  - 實現 ServiceBusAgentMessaging
  - 雙寫模式（MediatR + Service Bus）
  - 灰度切換（10% → 50% → 100%）
  - 清理 MediatR 代碼

Week 45-48: 自動伸縮與優化
  - 配置 HPA/VPA
  - 性能優化（目標：API P95 <300ms, Agent P95 <5s）
  - 成本優化

Week 49-52: Microsoft Agent Framework 評估（ADR-011）
  - 建立 Microsoft Agent Framework PoC 環境
  - 實現核心功能 Proof of Concept
  - 性能對比測試（Benchmark: Semantic Kernel vs MAF）
  - 安全性評估和合規性檢查
  - 評估報告與決策建議

  評估維度:
    - 穩定性: GitHub Issues 關閉率 >90% (30%)
    - 生產驗證: 3+ Fortune 500 企業案例 (25%)
    - 性能: 性能提升 >20%, 延遲降低 >15% (20%)
    - 功能: 支持所有現有功能 + 額外價值 (15%)
    - 遷移成本: <2 個月, 代碼變更 <30% (10%)

  決策閾值: 總分 >80% → 進入 Phase 3 遷移期
           總分 <80% → 繼續使用 Semantic Kernel

交付成果:
  ✅ Kubernetes 生產環境運行
  ✅ 分布式消息隊列（Azure Service Bus）
  ✅ 系統穩定性 >99.9%
  ✅ Microsoft Agent Framework 評估報告
  ✅ Go/No-Go 決策（是否進入 Phase 3）
```

### Phase 3: Framework 遷移（Month 13-15，條件性執行）

**前置條件**：Phase 2 評估通過（總分 >80%），無 Critical 缺陷

**目標**：平滑遷移到 Microsoft Agent Framework，保持業務連續性

```yaml
Week 53-54: 適配器實現
  - 實現 MicrosoftAgentRuntime
  - 實現 MicrosoftAgentToolRegistry
  - 實現 MicrosoftAgentMemory
  - 實現 MicrosoftAgentOrchestrator
  - 實現 IObservability (OpenTelemetry 集成)
  - 單元測試覆蓋率 >80%

Week 55-56: 集成測試
  - 端到端測試（E2E）
  - 性能測試（Benchmark vs Semantic Kernel）
  - 安全測試（Security Scan）
  - MCP、OpenAPI 兼容性測試
  - AutoGen Multi-Agent 功能測試

Week 57-60: 灰度發布與切換
  Week 57: 10% 流量（內部用戶）
    - Feature Flag 配置: MAF: 10%, SK: 90%
    - 監控: 錯誤率 <0.1%, P95 延遲 <500ms
    - 每日評審會議

  Week 58: 25% 流量（Beta 用戶）
    - Feature Flag 配置: MAF: 25%, SK: 75%
    - 用戶反饋收集（積極率 >90%）

  Week 59: 50% 流量（一般用戶）
    - Feature Flag 配置: MAF: 50%, SK: 50%
    - A/B Testing 性能對比

  Week 60: 100% 流量（全量切換）
    - Feature Flag 配置: MAF: 100%, SK: 0%
    - 保留 Semantic Kernel 適配器 3 個月（回滾能力）

回滾計劃:
  - 錯誤率 >1% → 立即回滾到 Semantic Kernel
  - P95 延遲 >500ms 持續 1 小時 → 回滾
  - 用戶反饋積極率 <80% → 回滾
  - Feature Flag 支持一鍵回滾（<5 分鐘）

交付成果:
  ✅ Microsoft Agent Framework 適配器完整實現
  ✅ 100% 流量成功切換（或決定維持 Semantic Kernel）
  ✅ 性能提升驗證（如承諾）
  ✅ 系統穩定性維持 >99.9%
  ✅ 回滾能力驗證
```

### Phase 4: 優化與規模化（Month 16-18）

**目標**：充分利用框架特性，優化系統性能，支持更大規模

```yaml
Week 61-64: 框架優化
  如果 Phase 3 執行（已遷移到 MAF）:
    - 移除 Semantic Kernel 適配器（確認無需回滾）
    - 利用 AutoGen Multi-Agent 特性優化編排
    - 優化 OpenTelemetry 可觀察性配置
    - 性能調優（目標：P95 延遲降低 20%）

  如果 Phase 3 未執行（繼續 SK）:
    - Semantic Kernel 深度優化
    - 自定義 Plugin 性能優化
    - MCP Tool 發現優化
    - 持續改進 Abstraction Layer

Week 65-68: 規模化準備
  - 水平擴展測試（100+ 並發 Agent）
  - 數據庫讀寫分離（PostgreSQL Primary + Replica）
  - Redis Cluster 優化
  - 成本優化（Prompt 緩存、資源調度）

Week 69-72: 高級功能與文檔
  - Agent Marketplace 準備（Phase 3 功能預研）
  - 完整用戶文檔
  - 運維手冊（Runbook）
  - 開發者文檔（SDK）
  - 架構文檔更新

Week 73-76: 監控與安全加固
  - 高級監控（Business Metrics Dashboard）
  - 安全加固（SAST/DAST 自動化）
  - 漏洞修復
  - 合規認證準備（SOC 2, ISO 27001）
  - Phase 4 驗收與總結

交付成果:
  ✅ 框架特性充分利用，性能最優化
  ✅ 支持 200+ 並發 Agent
  ✅ 完整的運維和開發文檔
  ✅ 系統穩定性維持 >99.9%
  ✅ 為 Phase 3 (Agent Marketplace) 做好準備
```

### 風險管理與決策點

```yaml
關鍵決策點:
  Decision Point 1 (Week 36 - MVP 完成):
    評估: MVP 是否達到驗收標準？
    Pass: 進入 Phase 2
    Fail: 延長 Phase 1, 修復關鍵問題

  Decision Point 2 (Week 52 - Framework 評估完成):
    評估: Microsoft Agent Framework 是否滿足遷移條件？
    Pass (>80%): 進入 Phase 3 遷移
    Fail (<80%): 跳過 Phase 3, 繼續使用 Semantic Kernel

  Decision Point 3 (Week 57-60 - 灰度發布期間):
    評估: 每個灰度階段是否達標？
    Pass: 繼續增加流量
    Fail: 立即回滾到 Semantic Kernel

  Decision Point 4 (Week 64 - 框架優化完成):
    評估: 是否可以移除舊框架適配器？
    Pass: 移除 Semantic Kernel 適配器
    Fail: 保留雙框架支持能力
```

---

## 13. 風險與緩解措施

### 13.1 技術風險

| 風險 | 影響 | 可能性 | 緩解措施 |
|------|------|--------|----------|
| Container 逃逸漏洞 | 🔴 Critical | 🟡 Medium | 4 層安全防護 + 定期安全掃描 |
| Agent 執行超時 | 🟡 High | 🟢 Low | 30 秒超時 + 異步執行模式 |
| Redis 單點故障（MVP） | 🟡 High | 🟡 Medium | Phase 2 遷移到 Redis Cluster |
| LLM 成本超預算 | 🟡 High | 🟡 Medium | Prompt 緩存 + 成本監控告警 |
| 數據庫性能瓶頸 | 🟡 High | 🟢 Low | 索引優化 + 讀寫分離（Phase 2） |

### 13.2 運維風險

| 風險 | 影響 | 可能性 | 緩解措施 |
|------|------|--------|----------|
| 部署失敗 | 🟡 High | 🟢 Low | 藍綠部署 + 自動回滾 |
| 數據丟失 | 🔴 Critical | 🟢 Low | 每日備份 + 定期恢復演練 |
| 監控盲點 | 🟡 High | 🟡 Medium | 多層監控 + 告警測試 |
| DDoS 攻擊 | 🟡 High | 🟡 Medium | Azure DDoS Protection + Rate Limiting |

---

## 14. 成功標準

### 14.1 技術指標

```yaml
性能:
  - API P95 響應時間 <300ms ✅
  - Agent P95 執行時間 <5s ✅
  - Code Interpreter P95 <8s ✅

可用性:
  - MVP: 99.5% ✅
  - Phase 2: 99.9% ✅

安全:
  - 無 P0/P1 安全漏洞 ✅
  - 通過安全審計 ✅

擴展性:
  - MVP: 支持 20 並發 Agent ✅
  - Phase 2: 支持 200 並發 Agent ✅
```

### 14.2 業務指標

```yaml
用戶採用:
  - Month 3: 10 個活躍用戶
  - Month 6: 50 個活躍用戶
  - Month 12: 200 個活躍用戶

使用率:
  - Month 3: 100 次 Agent 執行/天
  - Month 6: 500 次 Agent 執行/天
  - Month 12: 2000 次 Agent 執行/天

滿意度:
  - NPS（淨推薦值）>50
  - 用戶滿意度 >4.0/5.0
```

---

## 15. 附錄

### 15.1 參考文檔

- [Project Brief](../brief.md) - 項目概述與需求
- [ADR-006](./ADR-006-agent-state-management.md) - Agent 狀態管理
- [ADR-007](./ADR-007-multi-agent-communication.md) - Multi-Agent 通訊
- [ADR-008](./ADR-008-code-interpreter-execution-model.md) - Code Interpreter 執行模型
- [ADR-011](./ADR-011-framework-migration-strategy.md) - Framework 遷移策略
- [Microsoft Agent Framework 影響分析](../analysis-microsoft-agent-framework-impact.md) - 戰略分析
- [C4 架構圖](./C4-architecture-diagrams.md) - 系統架構可視化
- [OpenAPI 規範](./openapi-specification.yaml) - API 詳細定義
- [數據庫 Schema](./database-schema.md) - PostgreSQL 設計
- [性能與擴展性策略](./performance-scalability-strategy.md) - 性能優化

### 15.2 外部參考

**Microsoft 框架**:
- [Semantic Kernel Documentation](https://learn.microsoft.com/en-us/semantic-kernel/)
- [Microsoft Agent Framework](https://devblogs.microsoft.com/semantic-kernel/microsoft-agent-framework/)
- [AutoGen Documentation](https://microsoft.github.io/autogen/)
- [Microsoft Entra ID](https://learn.microsoft.com/en-us/entra/identity/)
- [Azure OpenAI Service](https://learn.microsoft.com/en-us/azure/ai-services/openai/)
- [Azure Kubernetes Service](https://learn.microsoft.com/en-us/azure/aks/)

**開放標準**:
- [MCP (Model Context Protocol)](https://spec.modelcontextprotocol.io/)
- [OpenAPI Specification 3.0+](https://swagger.io/specification/)
- [OpenTelemetry Documentation](https://opentelemetry.io/docs/)

**架構與設計**:
- [C4 Model](https://c4model.com/)
- [Adapter Pattern](https://refactoring.guru/design-patterns/adapter)
- [Feature Flags Best Practices](https://martinfowler.com/articles/feature-toggles.html)

### 15.3 工具與資源

```yaml
開發工具:
  - Visual Studio 2022 / Visual Studio Code
  - Postman / Thunder Client（API 測試）
  - pgAdmin / DBeaver（數據庫管理）
  - Redis Insight（Redis 管理）

協作工具:
  - GitHub（代碼倉庫）
  - Azure DevOps（項目管理）
  - Confluence（文檔協作）
  - Slack / Teams（團隊溝通）
```

---

## 16. 版本歷史

| 版本 | 日期 | 作者 | 變更說明 |
|------|------|------|----------|
| 1.0.0 | 2025-10-28 | System Architect | 初始版本：完整架構設計 |
| 1.1.0 | 2025-10-28 | Product Owner | 新增 3.3 節：Framework Abstraction Layer<br>更新 8.1 節：採用開放標準（MCP、OpenAPI、OpenTelemetry） |

---

## 17. 批准與簽署

| 角色 | 姓名 | 簽署日期 | 狀態 |
|------|------|----------|------|
| System Architect | BMad Method | 2025-10-28 | ✅ 已批准 |
| Technical Lead | 待定 | - | ⏳ 待批准 |
| Project Manager | 待定 | - | ⏳ 待批准 |
| Stakeholder | 待定 | - | ⏳ 待批准 |

---

**結論**：

本 Architecture Design Document (ADD) 基於 BMad Method 的 System Architect Persona，在 Project Brief 基礎上進行了詳細的架構設計。通過三個核心 ADR、完整的 C4 架構圖、詳細的 API 規範、數據庫 Schema 設計以及性能與擴展性策略，我們已經完整定義了 Semantic Kernel Agentic Framework 的技術架構。

本架構設計支持從 MVP 到 Phase 2 的平滑過渡，並為未來的擴展（Phase 3: Agent Marketplace）預留了空間。所有關鍵技術決策都經過深思熟慮，並充分考慮了性能、安全性、可擴展性和成本優化。

下一階段將進入 **Project Management（PM）** 階段，基於本 ADD 進行詳細的項目計劃、資源分配和風險管理。
