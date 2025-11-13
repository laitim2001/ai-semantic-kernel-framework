# 系統化開發策略 (Systematic Development Strategy)

**Semantic Kernel Agentic Framework - Workflow Engine Development Strategy**

**版本**: 2.0
**創建日期**: 2025-12-11
**最後更新**: 2025-12-12
**負責人**: Tech Lead + Solution Architect
**狀態**: 🟢 Active

---

## 📋 文檔目的

本文檔提供**工作流引擎**的系統化開發策略，基於 [docs/architecture/Architecture-Design-Document.md](../../docs/architecture/Architecture-Design-Document.md) 的整體架構設計和 [docs/user-stories/module-04-workflow-orchestration/README.md](../../docs/user-stories/module-04-workflow-orchestration/README.md) 的功能需求，包括：

- 🎯 **戰略目標**: 為什麼需要完整工作流引擎
- 🏗️ **架構設計**: 如何系統化設計複雜工作流引擎
- 📊 **Phase-by-Phase 策略**: 如何分階段實施降低風險
- 🔧 **技術決策**: 關鍵技術選擇的理由
- 🧩 **整合策略**: 如何與 Agent, Plugin, Knowledge 系統整合
- 📈 **演進路線**: 從 MVP 到完整功能的演進路徑

---

## 🎯 戰略目標與動機

### 為什麼需要完整工作流引擎？

**Product Owner 需求** (參考 [docs/brief/Project-Brief.md](../../docs/brief/Project-Brief.md)):
> "我認為需要準備一個完整的工作流引擎，可能會很複雜，但是也可以有系統地去開發"

**業務價值** (參考 [docs/user-stories/module-04-workflow-orchestration/US-7.1-Workflow-Definition-API.md](../../docs/user-stories/module-04-workflow-orchestration/US-7.1-Workflow-Definition-API.md)):
```yaml
價值主張:
  1. Multi-Agent 協作平台核心能力:
     - 單一 Agent 只能處理簡單任務
     - 工作流引擎實現複雜任務的自動化分解和協作
     - 參考: docs/user-stories/module-01-agent-management/US-1.4-Agent-Execution-Engine.md

  2. 用戶差異化優勢:
     - 與 AutoGPT, LangGraph 競爭的關鍵差異
     - 提供可視化工作流編輯器 (競品缺乏)
     - 參考: docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md

  3. 可擴展性:
     - 支援任意複雜度的業務流程
     - Sequential, Parallel, Conditional, Loop, Feedback
     - 參考: docs/user-stories/module-04-workflow-orchestration/US-7.2-Workflow-Execution-Engine.md

  4. 長期戰略:
     - 為 Phase 2 企業級功能奠定基礎
     - 支援未來的 Workflow Marketplace
     - 參考: docs/architecture/Architecture-Design-Document.md (Section 4.4)
```

**技術必要性** (參考 [docs/architecture/adr/ADR-003-workflow-execution-engine.md](../../docs/architecture/adr/ADR-003-workflow-execution-engine.md)):
```yaml
技術挑戰:
  1. Semantic Kernel Process Framework 不足:
     - 缺乏 Conditional, Loop 支援
     - 狀態管理不完整 (無 Checkpoint/Resume)
     - 不支援複雜的 Multi-Agent 協作模式
     - 參考: docs/architecture/adr/ADR-001-semantic-kernel-vs-agent-framework.md

  2. 自建工作流引擎必要性:
     - 完全控制工作流執行邏輯
     - 整合 LLM 驅動的智能任務規劃
     - 支援複雜的 Agent 協作模式
     - 參考: docs/architecture/adr/ADR-011-framework-abstraction-layer.md

  3. 開源方案不適合:
     - Temporal: 過於重量級，學習曲線陡峭
     - Apache Airflow: 為數據工程設計，不適合 Agent 協作
     - LangGraph: 缺乏可視化編輯器，整合成本高
     - 參考: docs/technical-implementation/01-backend-net9/12-workflow-orchestration-implementation.md
```

### 成功定義 (Definition of Success)

**MVP (Phase 1) 成功標準** (參考 [docs/user-stories/module-04-workflow-orchestration/README.md](../../docs/user-stories/module-04-workflow-orchestration/README.md)):
```yaml
功能完整性:
  ✅ Task Generator: LLM 驅動的智能任務規劃
     - 參考: docs/user-stories/module-04-workflow-orchestration/US-7.3-Task-Generation-with-LLM.md
  ✅ Multi-Agent Coordination: Sequential, Parallel, Conditional, Loop, GroupConversation
     - 參考: docs/user-stories/module-04-workflow-orchestration/US-7.4-Multi-Agent-Coordination.md
  ✅ Workflow Execution Engine: 狀態管理, Checkpoint/Resume
     - 參考: docs/user-stories/module-04-workflow-orchestration/US-7.2-Workflow-Execution-Engine.md
  ✅ Workflow Editor: 可視化編輯器 (VueFlow)
     - 參考: docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-*.md (7 部分)
  ✅ API 完整性: Workflow CRUD, Execution, Monitoring
     - 參考: docs/api/workflow-api-specification.md

性能指標 (參考 [docs/technical-implementation/08-performance-optimization/01-performance-requirements.md](../../docs/technical-implementation/08-performance-optimization/01-performance-requirements.md)):
  - 工作流啟動時間: <2 秒
  - 任務執行延遲: <5 秒
  - 並行任務數: ≥10 個
  - 執行成功率: ≥95%

質量指標 (參考 [docs/testing/testing-strategy.md](../../docs/testing/testing-strategy.md)):
  - 單元測試覆蓋率: ≥85%
  - 集成測試覆蓋率: ≥80%
  - Code Complexity: ≤15 (cyclomatic complexity)
  - 0 Critical/High severity bugs
```

---

## 🏗️ 架構設計策略

### 整體架構 (Layered Architecture)

基於 [docs/architecture/Architecture-Design-Document.md](../../docs/architecture/Architecture-Design-Document.md) 的 Clean Architecture + DDD + CQRS 設計原則：

```
┌─────────────────────────────────────────────────────────┐
│              Frontend: Workflow Editor (Vue 3)           │
│  VueFlow | Drag-drop | Node Config | Canvas | Save/Load │
│  參考: docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part5.md │
└─────────────────────────────────────────────────────────┘
                            ↕ HTTP/WebSocket
┌─────────────────────────────────────────────────────────┐
│                   API Layer (ASP.NET Core 8)            │
│   WorkflowController | ExecutionController | WebSocket  │
│   參考: docs/api/workflow-api-specification.md          │
└─────────────────────────────────────────────────────────┘
                            ↕ CQRS (MediatR)
┌─────────────────────────────────────────────────────────┐
│               Application Layer (MediatR)               │
│  Commands | Queries | Workflow Orchestrator | Executor  │
│  參考: docs/technical-implementation/01-backend-net9/12-workflow-orchestration-implementation.md │
└─────────────────────────────────────────────────────────┘
                            ↕ Domain Logic
┌─────────────────────────────────────────────────────────┐
│                   Domain Layer                          │
│  Workflow | WorkflowNode | WorkflowEdge | ExecutionState│
│  Task | TaskResult | TaskGenerator | CoordinationStrategy│
│  參考: docs/architecture/domain-model.md                │
└─────────────────────────────────────────────────────────┘
                            ↕ Infrastructure Services
┌─────────────────────────────────────────────────────────┐
│              Infrastructure Layer                       │
│  WorkflowRepository | AgentExecutor | MessageBus        │
│  StateStore (Checkpoint) | LLM Service | Plugin Loader  │
│  參考: docs/technical-implementation/01-backend-net9/03-infrastructure-layer.md │
└─────────────────────────────────────────────────────────┘
                            ↕ Persistence
┌─────────────────────────────────────────────────────────┐
│              Data Layer (PostgreSQL + Redis)            │
│  workflows | workflow_nodes | workflow_edges            │
│  workflow_executions | execution_states | checkpoints   │
│  參考: docs/database/database-schema.md                 │
└─────────────────────────────────────────────────────────┘
```

### 核心組件設計

#### 1. Task Generator (LLM 驅動任務規劃)

**職責**: 將用戶輸入自動分解為可執行任務

**設計** (參考 [docs/user-stories/module-04-workflow-orchestration/US-7.3-Task-Generation-with-LLM.md](../../docs/user-stories/module-04-workflow-orchestration/US-7.3-Task-Generation-with-LLM.md)):
```csharp
public interface ITaskGenerator
{
    /// <summary>
    /// 根據用戶輸入和工作流定義，生成任務列表
    /// </summary>
    Task<List<WorkflowTask>> GenerateTasksAsync(
        string userInput,
        WorkflowDefinition workflow,
        CancellationToken cancellationToken);

    /// <summary>
    /// 動態生成下一步任務 (基於當前執行狀態)
    /// </summary>
    Task<List<WorkflowTask>> GenerateNextTasksAsync(
        ExecutionContext context,
        CancellationToken cancellationToken);
}

public class LLMTaskGenerator : ITaskGenerator
{
    private readonly IKernel _kernel;
    private readonly IPromptTemplateEngine _promptEngine;

    public async Task<List<WorkflowTask>> GenerateTasksAsync(...)
    {
        // 1. 構建 LLM Prompt (使用 Prompt Templates)
        // 參考: docs/technical-implementation/01-backend-net9/09-prompt-engineering.md
        var prompt = await _promptEngine.BuildTaskPlanningPromptAsync(
            userInput, workflow);

        // 2. 調用 LLM 生成任務計劃
        var response = await _kernel.InvokePromptAsync(prompt);

        // 3. 解析 JSON 任務列表
        var tasks = JsonSerializer.Deserialize<List<WorkflowTask>>(response);

        // 4. 識別任務依賴關係
        IdentifyDependencies(tasks);

        return tasks;
    }
}
```

**實施策略** (參考 [docs/technical-implementation/01-backend-net9/12-workflow-orchestration-implementation.md](../../docs/technical-implementation/01-backend-net9/12-workflow-orchestration-implementation.md)):
- Phase 1B: 基礎任務生成 (Sequential 任務鏈)
- Phase 1E: 進階任務生成 (Conditional, Loop 識別)
- Phase 2: 學習型任務生成 (基於歷史執行優化)

---

#### 2. Multi-Agent Coordination (多 Agent 協調)

**職責**: 管理多個 Agent 的協作執行

**設計** (參考 [docs/user-stories/module-04-workflow-orchestration/US-7.4-Multi-Agent-Coordination.md](../../docs/user-stories/module-04-workflow-orchestration/US-7.4-Multi-Agent-Coordination.md)):
```csharp
public interface ICoordinationStrategy
{
    Task<TaskResult> ExecuteAsync(
        List<WorkflowTask> tasks,
        ExecutionContext context,
        CancellationToken cancellationToken);
}

// Sequential: A → B → C
// 參考: docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part4.md (Sequential Node)
public class SequentialCoordination : ICoordinationStrategy
{
    public async Task<TaskResult> ExecuteAsync(...)
    {
        foreach (var task in tasks)
        {
            var result = await _agentExecutor.ExecuteAsync(task);
            context.UpdateState(task.Id, result);

            if (!result.Success)
                return TaskResult.Failed(result.Error);
        }
        return TaskResult.Success();
    }
}

// Parallel: A, B, C 同時執行
// 參考: docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part4.md (Parallel Node)
public class ParallelCoordination : ICoordinationStrategy
{
    public async Task<TaskResult> ExecuteAsync(...)
    {
        var executionTasks = tasks.Select(task =>
            _agentExecutor.ExecuteAsync(task));

        var results = await Task.WhenAll(executionTasks);

        // 所有任務都成功才算成功
        var allSuccess = results.All(r => r.Success);
        return allSuccess
            ? TaskResult.Success()
            : TaskResult.Failed("Some tasks failed");
    }
}

// Conditional: if (condition) A else B
// 參考: docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part4.md (Conditional Node)
public class ConditionalCoordination : ICoordinationStrategy
{
    public async Task<TaskResult> ExecuteAsync(...)
    {
        var condition = await EvaluateConditionAsync(context);

        var selectedTask = condition
            ? tasks.First(t => t.Branch == "true")
            : tasks.First(t => t.Branch == "false");

        return await _agentExecutor.ExecuteAsync(selectedTask);
    }
}

// Loop: while (condition) { A }
// 參考: docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part4.md (Loop Node)
public class LoopCoordination : ICoordinationStrategy
{
    public async Task<TaskResult> ExecuteAsync(...)
    {
        var maxIterations = 10; // 防止無限循環
        var iteration = 0;

        while (await EvaluateConditionAsync(context) &&
               iteration < maxIterations)
        {
            var result = await _agentExecutor.ExecuteAsync(tasks.First());
            context.UpdateState($"iteration_{iteration}", result);
            iteration++;
        }

        return TaskResult.Success();
    }
}
```

**實施策略**:
- Phase 1B: Sequential + Parallel
- Phase 1E: Conditional + Loop
- Phase 2: GroupConversation + Supervisor

**參考文檔**:
- [docs/user-stories/module-04-workflow-orchestration/US-7.4-Multi-Agent-Coordination.md](../../docs/user-stories/module-04-workflow-orchestration/US-7.4-Multi-Agent-Coordination.md)
- [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part4.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part4.md) - 28 種節點類型設計

---

#### 3. Workflow Execution Engine (執行引擎)

**職責**: 編排整個工作流的執行生命週期

**設計** (參考 [docs/user-stories/module-04-workflow-orchestration/US-7.2-Workflow-Execution-Engine.md](../../docs/user-stories/module-04-workflow-orchestration/US-7.2-Workflow-Execution-Engine.md)):
```csharp
public interface IWorkflowExecutor
{
    Task<ExecutionResult> ExecuteAsync(
        Guid workflowId,
        string userInput,
        CancellationToken cancellationToken);

    Task<ExecutionResult> ResumeAsync(
        Guid executionId,
        CancellationToken cancellationToken);

    Task PauseAsync(Guid executionId);
}

public class WorkflowExecutor : IWorkflowExecutor
{
    private readonly ITaskGenerator _taskGenerator;
    private readonly ICoordinationStrategyFactory _strategyFactory;
    private readonly IStateStore _stateStore;
    private readonly IMessageBus _messageBus;

    public async Task<ExecutionResult> ExecuteAsync(...)
    {
        // 1. 載入工作流定義
        var workflow = await _workflowRepository.GetByIdAsync(workflowId);

        // 2. 創建執行上下文
        var context = new ExecutionContext
        {
            WorkflowId = workflowId,
            UserInput = userInput,
            StartTime = DateTime.UtcNow,
            State = new Dictionary<string, object>()
        };

        // 3. 生成任務計劃 (LLM 驅動)
        // 參考: docs/user-stories/module-04-workflow-orchestration/US-7.3-Task-Generation-with-LLM.md
        var tasks = await _taskGenerator.GenerateTasksAsync(
            userInput, workflow, cancellationToken);

        // 4. 執行任務 (根據協調策略)
        foreach (var node in workflow.Nodes)
        {
            var strategy = _strategyFactory.Create(node.CoordinationType);
            var nodeTasks = tasks.Where(t => t.NodeId == node.Id).ToList();

            var result = await strategy.ExecuteAsync(
                nodeTasks, context, cancellationToken);

            // 5. Checkpoint (狀態保存)
            // 參考: docs/technical-implementation/01-backend-net9/12-workflow-orchestration-implementation.md
            await _stateStore.SaveCheckpointAsync(context);

            // 6. 發布執行事件 (WebSocket 實時更新)
            // 參考: docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part3.md
            await _messageBus.PublishAsync(
                new TaskCompletedEvent(node.Id, result));

            if (!result.Success && !node.ContinueOnError)
                return ExecutionResult.Failed(result.Error);
        }

        return ExecutionResult.Success(context.State);
    }

    public async Task<ExecutionResult> ResumeAsync(...)
    {
        // 1. 從 Checkpoint 恢復狀態
        var context = await _stateStore.LoadCheckpointAsync(executionId);

        // 2. 找到上次執行的節點
        var lastNode = context.LastExecutedNode;

        // 3. 從下一個節點繼續執行
        // ... (與 ExecuteAsync 類似邏輯)
    }
}
```

**實施策略**:
- Phase 1B: 基礎執行引擎 (Sequential, Parallel)
- Phase 1E: Checkpoint/Resume, Error Handling
- Phase 2: 分布式執行, 水平擴展

**參考文檔**:
- [docs/user-stories/module-04-workflow-orchestration/US-7.2-Workflow-Execution-Engine.md](../../docs/user-stories/module-04-workflow-orchestration/US-7.2-Workflow-Execution-Engine.md)
- [docs/technical-implementation/01-backend-net9/12-workflow-orchestration-implementation.md](../../docs/technical-implementation/01-backend-net9/12-workflow-orchestration-implementation.md)
- [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part3.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part3.md) - 執行視覺化設計

---

#### 4. State Management (狀態管理)

**職責**: 管理工作流執行狀態，支援 Checkpoint/Resume

**設計** (參考 [docs/architecture/adr/ADR-006-hybrid-state-management.md](../../docs/architecture/adr/ADR-006-hybrid-state-management.md)):
```csharp
public interface IStateStore
{
    Task SaveCheckpointAsync(ExecutionContext context);
    Task<ExecutionContext> LoadCheckpointAsync(Guid executionId);
    Task DeleteCheckpointAsync(Guid executionId);
}

public class RedisStateStore : IStateStore
{
    private readonly IConnectionMultiplexer _redis;

    public async Task SaveCheckpointAsync(ExecutionContext context)
    {
        var db = _redis.GetDatabase();
        var key = $"checkpoint:{context.ExecutionId}";
        var value = JsonSerializer.Serialize(context);

        // TTL 24 小時 (參考: docs/technical-implementation/03-data-layer/redis-configuration.md)
        await db.StringSetAsync(key, value, TimeSpan.FromHours(24));
    }

    public async Task<ExecutionContext> LoadCheckpointAsync(...)
    {
        var db = _redis.GetDatabase();
        var key = $"checkpoint:{executionId}";
        var value = await db.StringGetAsync(key);

        if (value.IsNullOrEmpty)
            throw new CheckpointNotFoundException(executionId);

        return JsonSerializer.Deserialize<ExecutionContext>(value);
    }
}
```

**實施策略**:
- Phase 1B: 基礎狀態保存 (In-Memory)
- Phase 1E: Redis Checkpoint Store
- Phase 2: PostgreSQL 持久化, 歷史查詢

**參考文檔**:
- [docs/architecture/adr/ADR-006-hybrid-state-management.md](../../docs/architecture/adr/ADR-006-hybrid-state-management.md)
- [docs/technical-implementation/03-data-layer/redis-configuration.md](../../docs/technical-implementation/03-data-layer/redis-configuration.md)

---

#### 5. Message Bus (消息總線)

**職責**: Agent 之間的異步消息傳遞

**設計** (參考 [docs/architecture/adr/ADR-007-phased-communication-architecture.md](../../docs/architecture/adr/ADR-007-phased-communication-architecture.md)):
```csharp
public interface IMessageBus
{
    Task PublishAsync<T>(T message) where T : IMessage;
    Task SubscribeAsync<T>(Func<T, Task> handler) where T : IMessage;
}

public class InMemoryMessageBus : IMessageBus
{
    private readonly ConcurrentDictionary<Type, List<Delegate>> _handlers = new();

    public Task PublishAsync<T>(T message) where T : IMessage
    {
        if (_handlers.TryGetValue(typeof(T), out var handlers))
        {
            var tasks = handlers
                .Cast<Func<T, Task>>()
                .Select(handler => handler(message));

            return Task.WhenAll(tasks);
        }
        return Task.CompletedTask;
    }
}
```

**實施策略** (參考 [docs/architecture/adr/ADR-007-phased-communication-architecture.md](../../docs/architecture/adr/ADR-007-phased-communication-architecture.md)):
- Phase 1B: In-Memory Message Bus (MediatR)
- Phase 2: RabbitMQ 分布式消息
- Phase 3: Event Sourcing

---

### Framework Abstraction Layer 策略

**目的**: 降低對 Semantic Kernel 的依賴，保留未來遷移能力

**5 個核心接口** (參考 [docs/architecture/adr/ADR-011-framework-abstraction-layer.md](../../docs/architecture/adr/ADR-011-framework-abstraction-layer.md)):

```csharp
// 1. IAgentRuntime - Agent 執行抽象
public interface IAgentRuntime
{
    Task<AgentResponse> ExecuteAsync(
        AgentConfig config,
        string input,
        CancellationToken cancellationToken);
}

public class SemanticKernelAgentRuntime : IAgentRuntime
{
    private readonly IKernel _kernel;

    public async Task<AgentResponse> ExecuteAsync(...)
    {
        // 使用 Semantic Kernel 執行
        // 參考: docs/technical-implementation/01-backend-net9/05-semantic-kernel-integration.md
        var result = await _kernel.InvokePromptAsync(...);
        return new AgentResponse(result);
    }
}

// 2. IToolRegistry - Tool/Plugin 註冊抽象 (MCP 支援)
// 參考: docs/user-stories/module-02-plugin-system/US-2.1-Plugin-System-Core.md
public interface IToolRegistry
{
    Task RegisterMcpToolAsync(McpToolDefinition tool);
    Task<ITool> GetToolAsync(string toolId);
}

// 3. IAgentMemory - Agent 記憶抽象
// 參考: docs/user-stories/module-04-workflow-orchestration/US-7.5-Agent-Memory-Management.md
public interface IAgentMemory
{
    Task SaveAsync(string key, object value);
    Task<T> LoadAsync<T>(string key);
}

// 4. IWorkflowOrchestrator - 工作流編排抽象
public interface IWorkflowOrchestrator
{
    Task<ExecutionResult> OrchestrateAsync(
        WorkflowDefinition workflow,
        string input);
}

// 5. IObservability - 可觀測性抽象 (OpenTelemetry)
// 參考: docs/architecture/observability-architecture.md
public interface IObservability
{
    ISpan StartSpan(string operationName, SpanKind kind);
    void RecordMetric(string name, double value, IDictionary<string, string> tags);
}
```

**實施策略**:
- Phase 1B: 定義接口 + SemanticKernelAdapter 實現
- Phase 2: LangChain Adapter (備選)
- Phase 3: 完全自建 Runtime (如果 SK 不滿足需求)

**參考文檔**:
- [docs/architecture/adr/ADR-011-framework-abstraction-layer.md](../../docs/architecture/adr/ADR-011-framework-abstraction-layer.md)
- [docs/architecture/adr/ADR-001-semantic-kernel-vs-agent-framework.md](../../docs/architecture/adr/ADR-001-semantic-kernel-vs-agent-framework.md)
- [docs/technical-implementation/01-backend-net9/05-semantic-kernel-integration.md](../../docs/technical-implementation/01-backend-net9/05-semantic-kernel-integration.md)

---

## 📊 Phase-by-Phase 實施策略

### Phase 1B: 工作流引擎核心 (Sprint 7-9, 26 SP)

**目標**: 建立工作流執行的核心能力

**交付物** (參考 [docs/user-stories/module-04-workflow-orchestration/README.md](../../docs/user-stories/module-04-workflow-orchestration/README.md)):
```yaml
Domain Layer:
  - Workflow 實體 (WorkflowDefinition, WorkflowNode, WorkflowEdge)
  - Task 實體 (WorkflowTask, TaskResult, ExecutionContext)
  - 協調策略 (ICoordinationStrategy 接口)
  - 參考: docs/architecture/domain-model.md

Application Layer:
  - Task Generator 實現 (LLMTaskGenerator)
  - Coordination Strategies (Sequential, Parallel)
  - Workflow Executor (基礎執行引擎)
  - 參考: docs/technical-implementation/01-backend-net9/02-application-layer.md

Infrastructure Layer:
  - WorkflowRepository (PostgreSQL)
  - AgentExecutor (呼叫 Agent 執行引擎)
  - InMemoryMessageBus
  - InMemoryStateStore
  - 參考: docs/technical-implementation/01-backend-net9/03-infrastructure-layer.md

API Layer:
  - POST /api/v1/workflows/execute (執行工作流)
  - GET /api/v1/workflows/executions/{id} (查詢執行狀態)
  - 參考: docs/api/workflow-api-specification.md

Framework Abstraction:
  - IAgentRuntime, IToolRegistry, IAgentMemory 接口定義
  - SemanticKernelAgentRuntime 實現
  - 參考: docs/architecture/adr/ADR-011-framework-abstraction-layer.md

Testing:
  - 單元測試: ≥85% 覆蓋率
  - 集成測試: Sequential, Parallel 執行場景
  - 性能測試: 工作流啟動 <2s
  - 參考: docs/testing/unit-testing-guidelines.md
  - 參考: docs/testing/integration-testing-strategy.md
```

**技術風險** (參考 [RISK-REGISTER.md](./RISK-REGISTER.md)):
- 🔴 RISK-006: LLM 任務生成準確性 (Prompt Engineering 複雜)
- 🟡 RISK-007: 並行任務執行的狀態管理
- 🟡 Semantic Kernel 整合的穩定性

**緩解措施**:
- ⏳ Sprint 6 技術 Spike (2 days)
- ⏳ LLM Prompt 模板庫建立
- ⏳ 詳細的單元測試覆蓋

**驗收標準**:
```yaml
功能驗收:
  - ✅ 能執行 Sequential 工作流 (3 個 Agent 順序執行)
  - ✅ 能執行 Parallel 工作流 (3 個 Agent 並行執行)
  - ✅ LLM 任務生成準確率 ≥80%

性能驗收 (參考: docs/technical-implementation/08-performance-optimization/01-performance-requirements.md):
  - ✅ 工作流啟動時間 <2 秒
  - ✅ 並行任務數 ≥10 個
  - ✅ 執行成功率 ≥95%

質量驗收 (參考: docs/standards/coding-standards.md):
  - ✅ 單元測試覆蓋率 ≥85%
  - ✅ 集成測試通過率 100%
  - ✅ 0 Critical/High bugs
```

---

### Phase 1C: Code Interpreter 整合 (Sprint 10, 8 SP)

**目標**: 工作流引擎支援 Code Interpreter 節點類型

**交付物** (參考 [docs/user-stories/module-05-code-interpreter/README.md](../../docs/user-stories/module-05-code-interpreter/README.md)):
```yaml
Domain Layer:
  - CodeExecutionNode (繼承 WorkflowNode)
  - CodeExecutionTask (繼承 WorkflowTask)
  - 參考: docs/architecture/domain-model.md

Infrastructure Layer:
  - DockerCodeExecutor (Docker 沙箱執行)
  - CodeExecutionCoordination (Code 執行協調策略)
  - 參考: docs/technical-implementation/01-backend-net9/08-code-interpreter-implementation.md

API Layer:
  - POST /api/v1/workflows/nodes/code-execution (註冊 Code 節點)
  - 參考: docs/api/code-interpreter-api-specification.md

Integration:
  - Workflow Engine ⊕ Code Interpreter
  - 支援工作流中插入 Code Execution 節點
  - 參考: docs/ux-design/wireframes/low-fidelity/07-code-interpreter.md

Security:
  - 4-Layer Security Architecture
  - 參考: docs/architecture/adr/ADR-008-code-interpreter-container-pool.md
```

**技術風險** (參考 [RISK-REGISTER.md](./RISK-REGISTER.md)):
- 🔴 RISK-007: Docker 安全隔離
- 🟡 Code 執行超時處理

**參考文檔**:
- [docs/user-stories/module-05-code-interpreter/US-5.1-Code-Execution-API.md](../../docs/user-stories/module-05-code-interpreter/US-5.1-Code-Execution-API.md)
- [docs/architecture/adr/ADR-008-code-interpreter-container-pool.md](../../docs/architecture/adr/ADR-008-code-interpreter-container-pool.md)
- [docs/security/code-execution-security.md](../../docs/security/code-execution-security.md)

---

### Phase 1D: Workflow Editor Backend (Sprint 11-12, 15-20 SP)

**目標**: 提供工作流定義的 CRUD API

**交付物** (參考 [docs/user-stories/module-04-workflow-orchestration/US-7.1-Workflow-Definition-API.md](../../docs/user-stories/module-04-workflow-orchestration/US-7.1-Workflow-Definition-API.md)):
```yaml
Application Layer (CQRS):
  - CreateWorkflowCommand / Handler
  - UpdateWorkflowCommand / Handler
  - DeleteWorkflowCommand / Handler
  - GetWorkflowQuery / Handler
  - 參考: docs/technical-implementation/01-backend-net9/02-application-layer.md

Domain Services:
  - NodeRegistry (Agent 節點, Tool 節點註冊)
  - EdgeValidator (連接合法性驗證)
  - WorkflowTemplateManager (模板管理)
  - VersionControl (基礎版本管理)
  - 參考: docs/architecture/domain-model.md

API Layer:
  - POST /api/v1/workflows (創建工作流)
  - PUT /api/v1/workflows/{id} (更新工作流)
  - DELETE /api/v1/workflows/{id} (刪除工作流)
  - GET /api/v1/workflows (列出工作流)
  - GET /api/v1/workflows/{id} (獲取詳情)
  - POST /api/v1/workflows/{id}/execute (執行工作流)
  - GET /api/v1/workflows/templates (列出模板)
  - POST /api/v1/workflows/from-template/{templateId} (從模板創建)
  - 參考: docs/api/workflow-api-specification.md

Database Schema:
  - workflows (id, name, description, definition_json, version, created_at)
  - workflow_nodes (id, workflow_id, node_type, config_json)
  - workflow_edges (id, workflow_id, source_node_id, target_node_id)
  - workflow_executions (id, workflow_id, status, start_time, end_time)
  - 參考: docs/database/database-schema.md
```

**API 契約設計** (⟷ Frontend) (參考 [docs/api/workflow-api-specification.md](../../docs/api/workflow-api-specification.md)):
```json
// POST /api/v1/workflows Request
{
  "name": "Customer Onboarding Workflow",
  "description": "Automate customer onboarding process",
  "nodes": [
    {
      "id": "node-1",
      "type": "agent",
      "agentId": "550e8400-e29b-41d4-a716-446655440000",
      "config": {
        "input": "{{workflow.input}}",
        "output": "verification_result"
      }
    },
    {
      "id": "node-2",
      "type": "conditional",
      "condition": "{{node-1.output.verified}} == true",
      "trueBranch": "node-3",
      "falseBranch": "node-4"
    }
  ],
  "edges": [
    {
      "source": "node-1",
      "target": "node-2"
    }
  ]
}

// Response (201 Created)
{
  "id": "660f9500-f39c-52e5-b827-557766551111",
  "name": "Customer Onboarding Workflow",
  "version": 1,
  "createdAt": "2025-12-15T10:00:00Z"
}
```

**驗收標準**:
```yaml
功能驗收:
  - ✅ Workflow CRUD API 完整
  - ✅ Node Registry 支援 Agent, Tool, Code 節點
  - ✅ Edge Validation 檢查循環依賴
  - ✅ Template 管理 (至少 3 個預定義模板)

API 驗收:
  - ✅ API 響應時間 <200ms (CRUD)
  - ✅ API 文檔完整 (Swagger)
  - ✅ API 契約穩定 (與 Frontend 約定)
```

**參考文檔**:
- [docs/user-stories/module-04-workflow-orchestration/US-7.1-Workflow-Definition-API.md](../../docs/user-stories/module-04-workflow-orchestration/US-7.1-Workflow-Definition-API.md)
- [docs/api/workflow-api-specification.md](../../docs/api/workflow-api-specification.md)
- [docs/database/database-schema.md](../../docs/database/database-schema.md)

---

### Phase 1D: Workflow Editor Frontend (Sprint 13-14, 20-25 SP)

**目標**: 可視化工作流編輯器

**交付物** (參考 [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md) - 完整 7 部分設計):
```yaml
Vue 3 Application:
  - WorkflowEditor.vue (主編輯器組件)
  - VueFlow 集成 (n8n 風格大卡片節點系統)
  - Drag-drop Node Editor
  - Node Configuration Panel (內聯配置)
  - Connection Line Drawing (智能連接線)
  - Workflow Canvas (Zoom, Pan, Mini-map)
  - Node Template Library (28 種節點類型)
  - Save/Load Workflow
  - Real-time Preview (執行視覺化)
  - 參考: docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part1.md (核心設計理念)

Components:
  - NodePalette.vue (可用節點列表)
  - NodeEditor.vue (節點編輯面板 - 內聯配置)
  - EdgeEditor.vue (連接線編輯)
  - CanvasToolbar.vue (工具欄: Zoom, Undo, Redo)
  - TemplatePicker.vue (模板選擇器)
  - ExecutionVisualizer.vue (執行視覺化)
  - 參考: docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part2.md (內聯配置)
  - 參考: docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part3.md (執行視覺化)

State Management (Pinia):
  - workflowStore (工作流狀態管理)
  - nodeStore (節點狀態管理)
  - edgeStore (連接線狀態管理)
  - executionStore (執行狀態管理)
  - 參考: docs/technical-implementation/02-frontend-react/05-state-management.md

API Integration:
  - workflowService.ts (API 調用封裝)
  - useWorkflow() hook (Composition API)
  - WebSocket 實時更新 (執行狀態)
  - 參考: docs/technical-implementation/02-frontend-react/06-api-integration.md

Node Types (28 種):
  - Agent Node, Tool Node, Code Interpreter Node
  - Sequential, Parallel, Conditional, Loop Node
  - Knowledge Retrieval, Text-to-SQL Node
  - Supervisor, Handoff, Feedback Node
  - 參考: docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part4.md (28 種節點類型)

Canvas Interaction:
  - Drag-drop, Zoom, Pan, Mini-map
  - Connection Line Drawing (智能吸附)
  - Node Selection, Multi-select
  - Undo/Redo (至少 10 步)
  - 參考: docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part5.md (畫布互動)

Collaboration:
  - Multi-user Real-time Collaboration (CRDT)
  - Version Control Integration
  - 參考: docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part6.md (協作與架構)

Responsive Design:
  - Desktop (1920x1080), Tablet (1024x768), Mobile (375x812)
  - 參考: docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part7.md (響應式與驗收)

Accessibility:
  - Keyboard Navigation (WCAG 2.1 AA)
  - Screen Reader Support
  - 參考: docs/ux-design/design-system/accessibility-guidelines.md
```

**技術風險** (參考 [RISK-REGISTER.md](./RISK-REGISTER.md)):
- 🔴 RISK-004: VueFlow 學習曲線
- 🟡 複雜交互的性能優化

**緩解措施**:
- ⏳ Sprint 9 VueFlow 培訓 (2 days)
- ⏳ 雇用 VueFlow 專家輔導 (Sprint 10-11)
- ⚠️ 備用: 簡化編輯器 (JSON 表單)

**驗收標準** (參考 [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part7.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part7.md)):
```yaml
功能驗收:
  - ✅ 能 Drag-drop 節點到 Canvas (28 種節點類型)
  - ✅ 能連接節點 (智能連接線，自動吸附)
  - ✅ 能配置節點屬性 (內聯配置面板)
  - ✅ 能保存/載入工作流
  - ✅ 能從模板創建工作流
  - ✅ 執行視覺化 (實時狀態更新)

UX 驗收:
  - ✅ 操作響應時間 <100ms
  - ✅ Canvas 縮放流暢 (60 FPS)
  - ✅ 支援 Undo/Redo (至少 10 步)
  - ✅ Keyboard Navigation 支援 (WCAG 2.1 AA)

Performance:
  - ✅ 支援 ≥100 個節點 (流暢渲染)
  - ✅ WebSocket 延遲 <100ms
```

**參考文檔**:
- [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md) - 完整 V2 設計索引
- [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part1.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part1.md) - 核心設計理念
- [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part2.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part2.md) - 內聯配置
- [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part3.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part3.md) - 執行視覺化
- [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part4.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part4.md) - 28 種節點類型
- [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part5.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part5.md) - 畫布互動
- [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part6.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part6.md) - 協作與架構
- [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part7.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part7.md) - 響應式與驗收

---

### Phase 1E: 進階工作流引擎 (Sprint 15-16, 13-18 SP)

**目標**: 支援 Conditional, Loop, State Management, Feedback Loop

**交付物** (參考 [docs/user-stories/module-04-workflow-orchestration/README.md](../../docs/user-stories/module-04-workflow-orchestration/README.md)):
```yaml
Coordination Strategies:
  - ConditionalCoordination (if-else 分支)
  - LoopCoordination (while/for 循環)
  - HandoffCoordination (Agent 切換)
  - SupervisorCoordination (監督者模式)
  - 參考: docs/user-stories/module-04-workflow-orchestration/US-7.4-Multi-Agent-Coordination.md

State Management:
  - RedisStateStore (Checkpoint 持久化)
  - CheckpointManager (自動 Checkpoint)
  - ResumeHandler (從 Checkpoint 恢復)
  - 參考: docs/architecture/adr/ADR-006-hybrid-state-management.md

Feedback Loop:
  - FeedbackCollector (收集執行反饋)
  - FeedbackAnalyzer (分析執行結果)
  - AdaptiveTaskGenerator (基於反饋調整任務)
  - 參考: docs/user-stories/module-04-workflow-orchestration/US-7.6-Feedback-Loop.md

API Enhancements:
  - POST /api/v1/workflows/executions/{id}/pause (暫停執行)
  - POST /api/v1/workflows/executions/{id}/resume (恢復執行)
  - GET /api/v1/workflows/executions/{id}/checkpoints (查詢 Checkpoint)
  - 參考: docs/api/workflow-api-specification.md
```

**驗收標準**:
```yaml
功能驗收:
  - ✅ Conditional 執行準確率 ≥95%
  - ✅ Loop 支援最大 10 次迭代
  - ✅ Checkpoint/Resume 成功率 100%
  - ✅ Feedback Loop 能自動調整任務

性能驗收:
  - ✅ Checkpoint 保存時間 <500ms
  - ✅ Resume 恢復時間 <1s
```

**參考文檔**:
- [docs/user-stories/module-04-workflow-orchestration/US-7.4-Multi-Agent-Coordination.md](../../docs/user-stories/module-04-workflow-orchestration/US-7.4-Multi-Agent-Coordination.md)
- [docs/user-stories/module-04-workflow-orchestration/US-7.6-Feedback-Loop.md](../../docs/user-stories/module-04-workflow-orchestration/US-7.6-Feedback-Loop.md)
- [docs/architecture/adr/ADR-006-hybrid-state-management.md](../../docs/architecture/adr/ADR-006-hybrid-state-management.md)

---

## 🧩 系統整合策略

### 與 Agent 系統整合

**整合點 1: Agent Executor** (參考 [docs/user-stories/module-01-agent-management/US-1.4-Agent-Execution-Engine.md](../../docs/user-stories/module-01-agent-management/US-1.4-Agent-Execution-Engine.md)):
```csharp
// Workflow Engine 調用 Agent 執行引擎
public class AgentExecutor
{
    private readonly IAgentRuntime _runtime;

    public async Task<TaskResult> ExecuteAsync(WorkflowTask task)
    {
        var agent = await _agentRepository.GetByIdAsync(task.AgentId);

        var response = await _runtime.ExecuteAsync(
            agent.ToConfig(),
            task.Input,
            CancellationToken.None);

        return new TaskResult
        {
            Success = response.Success,
            Output = response.Output,
            Metadata = response.Metadata
        };
    }
}
```

**整合點 2: Agent 配置**:
```yaml
Agent 配置用於 Workflow Node:
  - AgentId (引用 Agent CRUD 中的 Agent)
  - InputMapping (將 Workflow 變數映射到 Agent 輸入)
  - OutputMapping (將 Agent 輸出映射到 Workflow 變數)
  - 參考: docs/user-stories/module-01-agent-management/README.md
```

---

### 與 Plugin 系統整合

**整合點 1: Plugin 作為 Workflow Node** (參考 [docs/user-stories/module-02-plugin-system/README.md](../../docs/user-stories/module-02-plugin-system/README.md)):
```csharp
// Plugin 可以註冊為 Workflow Node 類型
public class PluginNode : WorkflowNode
{
    public Guid PluginId { get; set; }
    public string PluginMethod { get; set; }
    public Dictionary<string, object> Parameters { get; set; }
}

// Workflow Engine 執行 Plugin Node
public class PluginExecutor
{
    public async Task<TaskResult> ExecuteAsync(PluginNode node)
    {
        var plugin = await _pluginLoader.LoadAsync(node.PluginId);
        var method = plugin.GetMethod(node.PluginMethod);

        var result = await method.InvokeAsync(node.Parameters);

        return new TaskResult
        {
            Success = true,
            Output = result
        };
    }
}
```

**整合點 2: Plugin 熱重載** (參考 [docs/user-stories/module-02-plugin-system/US-2.3-Plugin-Hot-Reload.md](../../docs/user-stories/module-02-plugin-system/US-2.3-Plugin-Hot-Reload.md)):
```yaml
Workflow 執行中 Plugin 更新:
  - 如果 Plugin 版本更新，Workflow Engine 自動載入新版本
  - 使用 PluginVersionRepository 查詢當前版本
  - 使用 PluginLoader.ReloadAsync() 重新載入
```

---

### 與 Knowledge 系統整合

**整合點: RAG 作為 Workflow Input** (參考 [docs/user-stories/module-03-knowledge-management/README.md](../../docs/user-stories/module-03-knowledge-management/README.md)):
```csharp
// Workflow 可以從 Knowledge 檢索相關資訊
public class KnowledgeRetrievalNode : WorkflowNode
{
    public string Query { get; set; }
    public int TopK { get; set; } = 5;
}

public class KnowledgeExecutor
{
    private readonly IRagService _ragService;

    public async Task<TaskResult> ExecuteAsync(KnowledgeRetrievalNode node)
    {
        var documents = await _ragService.RetrieveAsync(
            node.Query,
            node.TopK);

        return new TaskResult
        {
            Success = true,
            Output = documents
        };
    }
}
```

**參考文檔**:
- [docs/user-stories/module-03-knowledge-management/US-4.1-Knowledge-Base-CRUD.md](../../docs/user-stories/module-03-knowledge-management/US-4.1-Knowledge-Base-CRUD.md)
- [docs/user-stories/module-03-knowledge-management/US-4.2-RAG-Implementation.md](../../docs/user-stories/module-03-knowledge-management/US-4.2-RAG-Implementation.md)
- [docs/ux-design/wireframes/low-fidelity/06-knowledge-base.md](../../docs/ux-design/wireframes/low-fidelity/06-knowledge-base.md)

---

## 📈 演進路線圖

### MVP (Phase 1) → Phase 2 → Phase 3

基於 [ARCHITECTURE-EVOLUTION-ROADMAP.md](./ARCHITECTURE-EVOLUTION-ROADMAP.md) 和 [docs/architecture/Architecture-Design-Document.md](../../docs/architecture/Architecture-Design-Document.md) 的演進規劃：

```
Phase 1 (MVP - Sprint 1-21, 63 週):
├─ Phase 1B: 工作流引擎核心 (Sequential, Parallel)
│   - Sprint 7-9, 26 SP
│   - 參考: docs/user-stories/module-04-workflow-orchestration/README.md
├─ Phase 1C: Code Interpreter 整合
│   - Sprint 10, 8 SP
│   - 參考: docs/user-stories/module-05-code-interpreter/README.md
├─ Phase 1D: Workflow Editor (Backend + Frontend)
│   - Sprint 11-14, 35-45 SP
│   - 參考: docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md
└─ Phase 1E: 進階工作流 (Conditional, Loop, State)
    - Sprint 15-16, 13-18 SP
    - 參考: docs/user-stories/module-04-workflow-orchestration/US-7.4-Multi-Agent-Coordination.md

  ↓ 演進方向

Phase 2 (企業級功能 - Sprint 22-30, 27 週):
├─ Workflow Marketplace (模板共享)
├─ 分布式工作流執行 (Kubernetes)
│   - 參考: docs/deployment/kubernetes-deployment-guide.md
├─ 進階監控與可觀測性 (Prometheus, Grafana)
│   - 參考: docs/architecture/observability-architecture.md
├─ Workflow 版本管理與回滾
├─ A/B Testing (工作流變體測試)
└─ 學習型任務生成 (基於歷史優化)

  ↓ 演進方向

Phase 3 (AI 驅動自動化 - Sprint 31+):
├─ 自動工作流生成 (從需求描述生成工作流)
├─ 自我優化工作流 (基於執行數據自動調整)
├─ 跨組織工作流協作
├─ Workflow 安全與合規 (RBAC, Audit)
│   - 參考: docs/security/rbac-design.md
└─ 低代碼/無代碼工作流設計器
```

---

## 🔧 技術決策總結

### TD-009: 自建完整工作流引擎

參考 [TECHNICAL-DECISIONS-LOG.md](./TECHNICAL-DECISIONS-LOG.md) 和 [docs/architecture/adr/ADR-003-workflow-execution-engine.md](../../docs/architecture/adr/ADR-003-workflow-execution-engine.md)

**決策**: 不使用 Semantic Kernel Process Framework，自建完整工作流引擎

**理由**:
```yaml
Semantic Kernel 限制:
  - ❌ 缺乏 Conditional, Loop 支援
  - ❌ 狀態管理不完整 (無 Checkpoint/Resume)
  - ❌ 不支援複雜的 Multi-Agent 協作模式
  - 參考: docs/architecture/adr/ADR-001-semantic-kernel-vs-agent-framework.md

自建優勢:
  - ✅ 完全控制工作流執行邏輯
  - ✅ 整合 LLM 驅動的智能任務規劃
  - ✅ 支援複雜的 Agent 協作模式
  - ✅ 未來可擴展性 (分布式執行, A/B Testing)

實施可行性:
  - 🟢 分階段實施降低風險
  - 🟢 Framework Abstraction Layer 保留遷移能力
  - 🟡 需要額外 21-30 SP (但長期價值高)
```

---

### TD-002: Framework Abstraction Layer

參考 [TECHNICAL-DECISIONS-LOG.md](./TECHNICAL-DECISIONS-LOG.md) 和 [docs/architecture/adr/ADR-011-framework-abstraction-layer.md](../../docs/architecture/adr/ADR-011-framework-abstraction-layer.md)

**決策**: 建立 5 個核心接口抽象 Semantic Kernel

**理由**:
```yaml
風險緩解:
  - 降低對 Semantic Kernel 的依賴
  - 保留未來遷移到 LangChain, AutoGPT 的能力
  - 如果 SK 不滿足需求，可完全自建 Runtime

架構清晰性:
  - 明確的抽象層邊界
  - 易於測試 (可 Mock IAgentRuntime)
  - 符合依賴反轉原則 (DIP)
  - 參考: docs/architecture/Architecture-Design-Document.md (Section 3.2)
```

---

### ADR-012: Workflow Editor 技術選型 (Vue 3 + VueFlow)

參考 [docs/architecture/adr/ADR-012-workflow-editor-technology.md](../../docs/architecture/adr/ADR-012-workflow-editor-technology.md)

**決策**: 使用 Vue 3 + VueFlow (而非 React + ReactFlow)

**理由**:
```yaml
Vue 3 優勢:
  - 🟢 更輕量級 (比 React 小 30%)
  - 🟢 開發體驗好 (Composition API)
  - 🟢 與 React 主應用獨立 (降低耦合)

VueFlow 優勢:
  - 🟢 專為工作流設計 (比 ReactFlow 更適合)
  - 🟢 性能優秀 (支援 1000+ 節點)
  - 🟢 社群活躍，文檔完整
  - 🟢 n8n 風格大卡片節點系統支援

風險:
  - 🟡 前端團隊需學習 Vue 3 (2 days 培訓)
  - ⚠️ 備用: 如果學習曲線太陡，降級為 JSON 編輯器

PoC 驗證:
  - PoC 6 已驗證可行性 (98.2% 質量評分)
  - 參考: poc-projects/poc6-vueflow-crdt/
```

---

## 📊 成功度量與 KPI

### 開發效率 KPI

參考 [SPRINT-ALLOCATION-ANALYSIS.md](./SPRINT-ALLOCATION-ANALYSIS.md)

| KPI | 目標值 | 測量方式 |
|-----|-------|---------|
| Phase 1B 完成時間 | 9 週 (Sprint 7-9) | Sprint Review |
| Phase 1D Backend 完成時間 | 6 週 (Sprint 11-12) | Sprint Review |
| Phase 1D Frontend 完成時間 | 6 週 (Sprint 13-14) | Sprint Review |
| Phase 1E 完成時間 | 6 週 (Sprint 15-16) | Sprint Review |
| 總工作流引擎開發時間 | 27 週 (Sprint 7-16) | 專案時程表 |

### 質量 KPI

參考 [docs/testing/testing-strategy.md](../../docs/testing/testing-strategy.md) 和 [docs/standards/coding-standards.md](../../docs/standards/coding-standards.md)

| KPI | 目標值 | 測量方式 |
|-----|-------|---------|
| 單元測試覆蓋率 | ≥85% | Code Coverage Report |
| 集成測試覆蓋率 | ≥80% | Integration Test Suite |
| Code Complexity | ≤15 | SonarQube |
| Critical/High Bugs | 0 | Bug Tracking System |
| API 文檔完整性 | 100% | Swagger Coverage |

### 性能 KPI

參考 [docs/technical-implementation/08-performance-optimization/01-performance-requirements.md](../../docs/technical-implementation/08-performance-optimization/01-performance-requirements.md)

| KPI | 目標值 | 測量方式 |
|-----|-------|---------|
| 工作流啟動時間 | <2 秒 | Performance Test |
| 任務執行延遲 | <5 秒 | Performance Test |
| 並行任務數 | ≥10 個 | Load Test |
| 執行成功率 | ≥95% | Production Metrics |
| Checkpoint 保存時間 | <500ms | Performance Test |
| API 響應時間 | <200ms | APM (Application Performance Monitoring) |

---

## 🔄 更新歷史

| 版本 | 日期 | 更新內容 | 更新人 |
|-----|------|---------|-------|
| 1.0.0 | 2025-12-11 | 初始版本，完整開發策略 | AI Assistant |
| 2.0 | 2025-12-12 | 添加 52+ `/docs` 參考文獻，增強與現有文檔的集成 | AI Assistant |

---

## 📖 參考文檔

### 核心架構與設計
- [docs/architecture/Architecture-Design-Document.md](../../docs/architecture/Architecture-Design-Document.md) - 整體系統架構
- [docs/architecture/domain-model.md](../../docs/architecture/domain-model.md) - 領域模型設計
- [docs/architecture/observability-architecture.md](../../docs/architecture/observability-architecture.md) - 可觀測性架構
- [docs/brief/Project-Brief.md](../../docs/brief/Project-Brief.md) - 專案概述

### 架構決策記錄 (ADR)
- [docs/architecture/adr/ADR-001-semantic-kernel-vs-agent-framework.md](../../docs/architecture/adr/ADR-001-semantic-kernel-vs-agent-framework.md) - SK vs MAF 技術選型
- [docs/architecture/adr/ADR-003-workflow-execution-engine.md](../../docs/architecture/adr/ADR-003-workflow-execution-engine.md) - 工作流執行引擎設計
- [docs/architecture/adr/ADR-006-hybrid-state-management.md](../../docs/architecture/adr/ADR-006-hybrid-state-management.md) - 混合狀態管理
- [docs/architecture/adr/ADR-007-phased-communication-architecture.md](../../docs/architecture/adr/ADR-007-phased-communication-architecture.md) - 階段式通訊架構
- [docs/architecture/adr/ADR-008-code-interpreter-container-pool.md](../../docs/architecture/adr/ADR-008-code-interpreter-container-pool.md) - Code Interpreter 容器池
- [docs/architecture/adr/ADR-011-framework-abstraction-layer.md](../../docs/architecture/adr/ADR-011-framework-abstraction-layer.md) - Framework Abstraction Layer
- [docs/architecture/adr/ADR-012-workflow-editor-technology.md](../../docs/architecture/adr/ADR-012-workflow-editor-technology.md) - Workflow Editor 技術選型

### User Stories & Modules
- [docs/user-stories/module-01-agent-management/README.md](../../docs/user-stories/module-01-agent-management/README.md) - Agent 管理模組
- [docs/user-stories/module-01-agent-management/US-1.4-Agent-Execution-Engine.md](../../docs/user-stories/module-01-agent-management/US-1.4-Agent-Execution-Engine.md) - Agent 執行引擎
- [docs/user-stories/module-02-plugin-system/README.md](../../docs/user-stories/module-02-plugin-system/README.md) - Plugin 系統模組
- [docs/user-stories/module-02-plugin-system/US-2.1-Plugin-System-Core.md](../../docs/user-stories/module-02-plugin-system/US-2.1-Plugin-System-Core.md) - Plugin 系統核心
- [docs/user-stories/module-02-plugin-system/US-2.3-Plugin-Hot-Reload.md](../../docs/user-stories/module-02-plugin-system/US-2.3-Plugin-Hot-Reload.md) - Plugin 熱重載
- [docs/user-stories/module-03-knowledge-management/README.md](../../docs/user-stories/module-03-knowledge-management/README.md) - Knowledge 管理模組
- [docs/user-stories/module-03-knowledge-management/US-4.1-Knowledge-Base-CRUD.md](../../docs/user-stories/module-03-knowledge-management/US-4.1-Knowledge-Base-CRUD.md) - Knowledge Base CRUD
- [docs/user-stories/module-03-knowledge-management/US-4.2-RAG-Implementation.md](../../docs/user-stories/module-03-knowledge-management/US-4.2-RAG-Implementation.md) - RAG 實作
- [docs/user-stories/module-04-workflow-orchestration/README.md](../../docs/user-stories/module-04-workflow-orchestration/README.md) - Workflow 編排模組
- [docs/user-stories/module-04-workflow-orchestration/US-7.1-Workflow-Definition-API.md](../../docs/user-stories/module-04-workflow-orchestration/US-7.1-Workflow-Definition-API.md) - Workflow 定義 API
- [docs/user-stories/module-04-workflow-orchestration/US-7.2-Workflow-Execution-Engine.md](../../docs/user-stories/module-04-workflow-orchestration/US-7.2-Workflow-Execution-Engine.md) - Workflow 執行引擎
- [docs/user-stories/module-04-workflow-orchestration/US-7.3-Task-Generation-with-LLM.md](../../docs/user-stories/module-04-workflow-orchestration/US-7.3-Task-Generation-with-LLM.md) - LLM 任務生成
- [docs/user-stories/module-04-workflow-orchestration/US-7.4-Multi-Agent-Coordination.md](../../docs/user-stories/module-04-workflow-orchestration/US-7.4-Multi-Agent-Coordination.md) - Multi-Agent 協調
- [docs/user-stories/module-04-workflow-orchestration/US-7.5-Agent-Memory-Management.md](../../docs/user-stories/module-04-workflow-orchestration/US-7.5-Agent-Memory-Management.md) - Agent 記憶管理
- [docs/user-stories/module-04-workflow-orchestration/US-7.6-Feedback-Loop.md](../../docs/user-stories/module-04-workflow-orchestration/US-7.6-Feedback-Loop.md) - Feedback Loop
- [docs/user-stories/module-05-code-interpreter/README.md](../../docs/user-stories/module-05-code-interpreter/README.md) - Code Interpreter 模組
- [docs/user-stories/module-05-code-interpreter/US-5.1-Code-Execution-API.md](../../docs/user-stories/module-05-code-interpreter/US-5.1-Code-Execution-API.md) - Code Execution API

### Technical Implementation
- [docs/technical-implementation/01-backend-net9/02-application-layer.md](../../docs/technical-implementation/01-backend-net9/02-application-layer.md) - Application Layer 實作
- [docs/technical-implementation/01-backend-net9/03-infrastructure-layer.md](../../docs/technical-implementation/01-backend-net9/03-infrastructure-layer.md) - Infrastructure Layer 實作
- [docs/technical-implementation/01-backend-net9/05-semantic-kernel-integration.md](../../docs/technical-implementation/01-backend-net9/05-semantic-kernel-integration.md) - Semantic Kernel 整合
- [docs/technical-implementation/01-backend-net9/08-code-interpreter-implementation.md](../../docs/technical-implementation/01-backend-net9/08-code-interpreter-implementation.md) - Code Interpreter 實作
- [docs/technical-implementation/01-backend-net9/09-prompt-engineering.md](../../docs/technical-implementation/01-backend-net9/09-prompt-engineering.md) - Prompt Engineering
- [docs/technical-implementation/01-backend-net9/12-workflow-orchestration-implementation.md](../../docs/technical-implementation/01-backend-net9/12-workflow-orchestration-implementation.md) - Workflow Orchestration 實作
- [docs/technical-implementation/02-frontend-react/05-state-management.md](../../docs/technical-implementation/02-frontend-react/05-state-management.md) - 狀態管理
- [docs/technical-implementation/02-frontend-react/06-api-integration.md](../../docs/technical-implementation/02-frontend-react/06-api-integration.md) - API 整合
- [docs/technical-implementation/03-data-layer/redis-configuration.md](../../docs/technical-implementation/03-data-layer/redis-configuration.md) - Redis 配置
- [docs/technical-implementation/08-performance-optimization/01-performance-requirements.md](../../docs/technical-implementation/08-performance-optimization/01-performance-requirements.md) - 性能需求

### UX Design - Workflow Editor V2 (完整 7 部分設計)
- [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md) - Workflow Editor V2 完整索引
- [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part1.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part1.md) - 核心設計理念
- [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part2.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part2.md) - 內聯配置
- [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part3.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part3.md) - 執行視覺化
- [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part4.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part4.md) - 28 種節點類型
- [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part5.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part5.md) - 畫布互動
- [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part6.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part6.md) - 協作與架構
- [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part7.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part7.md) - 響應式與驗收
- [docs/ux-design/wireframes/low-fidelity/06-knowledge-base.md](../../docs/ux-design/wireframes/low-fidelity/06-knowledge-base.md) - Knowledge Base 設計
- [docs/ux-design/wireframes/low-fidelity/07-code-interpreter.md](../../docs/ux-design/wireframes/low-fidelity/07-code-interpreter.md) - Code Interpreter 設計
- [docs/ux-design/design-system/accessibility-guidelines.md](../../docs/ux-design/design-system/accessibility-guidelines.md) - 無障礙指南

### API, Database & Security
- [docs/api/workflow-api-specification.md](../../docs/api/workflow-api-specification.md) - Workflow API 規格
- [docs/api/code-interpreter-api-specification.md](../../docs/api/code-interpreter-api-specification.md) - Code Interpreter API 規格
- [docs/database/database-schema.md](../../docs/database/database-schema.md) - 資料庫 Schema
- [docs/security/code-execution-security.md](../../docs/security/code-execution-security.md) - Code 執行安全
- [docs/security/rbac-design.md](../../docs/security/rbac-design.md) - RBAC 設計

### Testing, Deployment & Standards
- [docs/testing/testing-strategy.md](../../docs/testing/testing-strategy.md) - 測試策略
- [docs/testing/unit-testing-guidelines.md](../../docs/testing/unit-testing-guidelines.md) - 單元測試指南
- [docs/testing/integration-testing-strategy.md](../../docs/testing/integration-testing-strategy.md) - 整合測試策略
- [docs/deployment/kubernetes-deployment-guide.md](../../docs/deployment/kubernetes-deployment-guide.md) - Kubernetes 部署指南
- [docs/standards/coding-standards.md](../../docs/standards/coding-standards.md) - 編碼標準

### 內部 Claudedocs
- [TECHNICAL-DECISIONS-LOG.md](./TECHNICAL-DECISIONS-LOG.md) - 技術決策日誌
- [ARCHITECTURE-EVOLUTION-ROADMAP.md](./ARCHITECTURE-EVOLUTION-ROADMAP.md) - 架構演進路線圖
- [RISK-REGISTER.md](./RISK-REGISTER.md) - 風險登記
- [DEPENDENCY-MATRIX.md](./DEPENDENCY-MATRIX.md) - 依賴矩陣
- [SPRINT-ALLOCATION-ANALYSIS.md](./SPRINT-ALLOCATION-ANALYSIS.md) - Sprint 分配分析

---

**維護說明**:
- ✅ 每個 Phase 開始前更新實施細節
- ✅ 每個 Phase 結束後更新實際成果
- ✅ 技術決策變更時立即更新
- ✅ 每季度檢視演進路線圖
- ✅ 同步更新所有相關 `/docs` 參考文獻
