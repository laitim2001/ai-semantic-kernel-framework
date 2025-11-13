# /docs 目錄完整內容索引

**生成時間**: 2025-11-12
**範圍**: /docs 目錄完整深入閱讀
**總文件數**: 56+
**總行數**: ~57,436+
**閱讀方式**: 6 個並行 Task agents 系統化閱讀

---

## 📋 目錄

- [1. Brief 文件摘要](#1-brief-文件摘要)
- [2. Architecture 架構設計摘要](#2-architecture-架構設計摘要)
- [3. User Stories 用戶故事摘要](#3-user-stories-用戶故事摘要)
- [4. Technical Implementation 技術實施摘要](#4-technical-implementation-技術實施摘要)
- [5. UX Design 用戶體驗設計摘要](#5-ux-design-用戶體驗設計摘要)
- [6. Project Management 項目管理摘要](#6-project-management-項目管理摘要)
- [7. Sprint 2 關鍵信息整合](#7-sprint-2-關鍵信息整合)
- [8. 跨文件主題分析](#8-跨文件主題分析)

---

## 1. Brief 文件摘要

### 1.1 文件清單

| 文件 | 行數 | 主要內容 |
|------|------|----------|
| brief-1-overview.md | 420 | 項目總覽、背景、目標 |
| brief-2-requirements.md | 827 | 核心需求、功能範圍 |
| brief-3-technical.md | 2000+ | 技術規格、架構概述 |
| brief-4-constraints.md | 2000+ | 約束條件、限制因素 |

### 1.2 執行摘要

Semantic Kernel Agentic Framework 是為了解決 **Fujitsu Copilot Studio 的 3 大限制**而設計：
1. 知識庫準確度不足（無向量搜索）
2. 僅支持 RAG，無 Multi-Agent 協作
3. 無多模態輸出（圖表、代碼執行）

**核心差異化功能**：**Code Interpreter**（4層安全防護），對標 Fujitsu Kozuchi 的殺手級功能。

### 1.3 關鍵決策與約束

#### 項目範圍 (brief-2, line 45-67)
- **MVP 範圍**: 10 個 User Stories（縮減至核心功能）
- **開發時間**: 8 個月（含 POC 驗證）
- **預算**: **800K USD**（從 600K 調整，因加入 Code Interpreter）
- **團隊**: 7 人（1 PM + 3 Backend + 2 Frontend + 1 DevOps）

#### 技術框架 (brief-3, line 78-96)
- **核心框架**: Semantic Kernel 1.x + **Framework Abstraction Layer**（支持未來遷移到 AutoGen/LangGraph）
- **Language**: .NET 8 (C# 12) + Python SDK（Code Interpreter）
- **Frontend**: React 18 + TypeScript + Material-UI (95%) + Vue 3 + VueFlow (5%)
- **Database**: PostgreSQL 16 (主) + Redis 7.x (緩存)
- **Messaging**: RabbitMQ（MVP）→ Azure Service Bus（Phase 2）

#### Code Interpreter 安全防護 (brief-3, line 280-306)
```yaml
4層安全防護:
  Layer 1: 靜態代碼分析（AST 解析，禁止 import os, subprocess）
  Layer 2: Docker 容器隔離（每個執行獨立容器）
  Layer 3: User Namespace 隔離（非 root 用戶執行）
  Layer 4: Seccomp 系統調用過濾（禁止危險系統調用）

開發時間: 22 工作日
能力評分提升: 69% → 82%（vs Fujitsu Kozuchi）
```

#### MVP 哲學 (brief-3, line 112-134)
**完整但精簡（Complete but Minimal）**:
- ✅ **架構完整**: 6 層架構全部實現，但簡化實現
- ✅ **功能精簡**: 10 個核心 US，延後 14 個進階 US
- ✅ **質量不妥協**: 單元測試覆蓋率 ≥80%，性能要求不降低

**簡化策略**:
- Docker Compose 替代 Kubernetes（Phase 2 遷移）
- RabbitMQ 替代 Azure Service Bus（Phase 2 遷移）
- Redis 單實例替代 Cluster（Phase 2 升級）

### 1.4 關鍵對比分析

#### vs. Fujitsu Copilot Studio (brief-1, line 89-123)
| 功能 | Copilot Studio | 我們的框架 | 優勢 |
|------|---------------|-----------|------|
| 知識庫 | 簡單匹配 | Azure AI Search（向量） | ✅ 準確度高 |
| Agent 類型 | RAG only | Multi-Agent 協作 | ✅ 複雜場景 |
| 代碼執行 | ❌ 無 | ✅ Code Interpreter | ✅ 差異化 |
| 多模態輸出 | ❌ 無 | ✅ 圖表、代碼結果 | ✅ 豐富輸出 |

#### vs. Fujitsu Kozuchi (brief-1, line 125-156)
| 功能 | Kozuchi | 我們的框架 | 對比 |
|------|---------|-----------|------|
| Code Interpreter | ✅ 有 | ✅ 有（4層防護） | 🟰 對等 |
| Multi-Agent | ✅ 有 | ✅ 有（MediatR） | 🟰 對等 |
| 知識庫 | ✅ RAG | ✅ RAG | 🟰 對等 |
| Text-to-SQL | ✅ 有 | ✅ 有 | 🟰 對等 |
| 開源框架 | ❌ 專有 | ✅ Semantic Kernel | ✅ 優勢 |
| Python SDK | ❌ 無 | ✅ 一等公民 | ✅ 優勢 |

### 1.5 約束條件 (brief-4)

#### 時間約束 (line 34-58)
- **硬性截止日期**: 2026-06-30（8 個月後）
- **Phase 1（MVP）**: 2025-10-28 ~ 2026-06-30（36 週）
- **Phase 2（K8s）**: 2026-07-01 ~ 2027-04-30（40 週）

**關鍵里程碑**:
- M1: Week 1 - 項目啟動
- M3: Week 12 - 核心功能完成
- M6: Week 32 - 前端完成
- M7: Week 36 - **MVP 驗收** ⭐

#### 預算約束 (line 60-89)
```yaml
總預算: 800K USD
  Phase 1 (MVP): 300K USD
    - 人力成本: 256K (80%)
    - Azure 基礎設施: 56K (18.7%)
    - 其他（培訓、軟件、緩衝）: 32K (10.7%)

  Phase 2 (K8s): 500K USD
    - 人力成本: 320K (64%)
    - Azure 基礎設施: 263K (52.6%)
    - 安全審計與合規: 80K (16%)
    - 其他: 37K (7.4%)
```

#### 技術約束 (line 91-128)
- **必須使用 Semantic Kernel 1.x**（公司標準框架）
- **必須部署到 Azure**（公司雲端政策）
- **必須使用 .NET 8**（公司技術棧）
- **Code Interpreter 僅支持 Python 3.11**（安全考量）
- **LLM 限制**: Azure OpenAI GPT-4o / GPT-4o-mini（無 Claude 支持）

#### 質量約束 (line 130-159)
```yaml
代碼質量:
  單元測試覆蓋率: ≥80%
  代碼重複率: <5%
  Cyclomatic Complexity: <15

性能要求:
  API 響應時間 P95: <300ms
  Agent 執行時間 P95: <5s
  Code Interpreter P95: <8s

安全要求:
  OWASP Top 10: 全部緩解
  Container 逃逸: 4 層防護
  P0/P1 漏洞: 0 個

可用性:
  MVP: ≥99.5%
  Phase 2: ≥99.9%
```

---

## 2. Architecture 架構設計摘要

### 2.1 文件清單

| 文件 | 行數 | 主要內容 |
|------|------|----------|
| Architecture-Design-Document.md | 1609 | 架構總覽、6層架構 |
| database-schema.md | 816 | 數據庫設計、12 核心表 |
| C4-architecture-diagrams.md | 676 | C4 模型架構圖 |
| performance-scalability-strategy.md | ~500 | 性能與可擴展性策略 |
| ADR-006-agent-state-management.md | ~800 | Agent 狀態管理 ADR |
| ADR-007-multi-agent-communication.md | ~800 | Multi-Agent 通訊 ADR |
| ADR-008-code-interpreter-execution-model.md | ~900 | Code Interpreter ADR |
| ADR-011-framework-migration-strategy.md | ~700 | 框架遷移策略 ADR |
| ADR-012-workflow-editor-technology.md | ~600 | Workflow 編輯器技術 ADR |

### 2.2 核心架構決策

#### 6層架構 (Architecture-Design-Document, line 45-89)
```
┌─────────────────────────────────────────────┐
│ Layer 1: Presentation Layer (前端)         │
│ - React Web App, Admin Portal, Swagger UI  │
├─────────────────────────────────────────────┤
│ Layer 2: API Gateway Layer (API 閘道)      │
│ - Authentication, Rate Limiting, Routing    │
├─────────────────────────────────────────────┤
│ Layer 3: Agent Orchestration Layer (編排)  │
│ - Semantic Kernel, Task Planner, Multi-Agent│
├─────────────────────────────────────────────┤
│ Layer 4: Plugin Execution Layer (插件執行) │
│ - Built-in Plugins, Custom Plugins, MCP    │
├─────────────────────────────────────────────┤
│ Layer 5: Integration Layer (集成)          │
│ - LLM Connectors, Vector DB, External APIs │
├─────────────────────────────────────────────┤
│ Layer 6: Data Layer (數據)                 │
│ - PostgreSQL, Redis, Blob Storage           │
└─────────────────────────────────────────────┘
```

#### 微服務架構 (Architecture-Design-Document, line 123-178)
```yaml
6個獨立服務:
  1. API Gateway Service (.NET 8)
     - Port: 5000
     - 功能: 認證、路由、限流

  2. Agent Orchestration Service (.NET 8)
     - Port: 5001
     - 功能: Agent 創建、執行、狀態管理

  3. Plugin Management Service (.NET 8)
     - Port: 5002
     - 功能: Plugin 註冊、Hot Reload、版本管理

  4. Code Interpreter Service (.NET 8 + Python 3.11)
     - Port: 5003
     - 功能: Python/R 代碼執行、Container Pool

  5. Knowledge Base Service (.NET 8)
     - Port: 5004
     - 功能: RAG、向量搜索、文檔索引

  6. Monitoring & Logging Service (.NET 8)
     - Port: 5005
     - 功能: Application Insights 集成、日誌聚合
```

### 2.3 Architecture Decision Records (ADRs)

#### ADR-006: Hybrid Agent State Management (line 34-156)

**決策**: 使用 **Redis (緩存) + PostgreSQL (持久化)** 混合模式管理 Agent 狀態

**理由**:
1. **性能需求**: Agent 執行需要頻繁讀取狀態（P95 <100ms）
2. **持久化需求**: 狀態必須持久化以支持 Agent 恢復和審計
3. **成本效益**: Redis 單實例成本低（MVP 階段）

**實現方案** (line 78-134):
```csharp
public class HybridStateStore : IStateStore
{
    private readonly IDistributedCache _redis;  // Redis 緩存
    private readonly ApplicationDbContext _db;  // PostgreSQL 持久化

    public async Task<AgentState?> GetStateAsync(Guid agentId)
    {
        // 1. 先從 Redis 讀取（熱數據）
        var cachedState = await _redis.GetStringAsync($"agent:{agentId}");
        if (cachedState != null) {
            return JsonSerializer.Deserialize<AgentState>(cachedState);
        }

        // 2. Cache Miss，從 PostgreSQL 讀取（冷數據）
        var dbState = await _db.AgentStates
            .Where(s => s.AgentId == agentId)
            .FirstOrDefaultAsync();

        if (dbState != null) {
            // 3. 寫回 Redis（預熱緩存）
            await _redis.SetStringAsync(
                $"agent:{agentId}",
                JsonSerializer.Serialize(dbState),
                new DistributedCacheEntryOptions {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                }
            );
        }

        return dbState;
    }

    public async Task SaveStateAsync(AgentState state)
    {
        // 1. 同時寫入 Redis 和 PostgreSQL（雙寫）
        var json = JsonSerializer.Serialize(state);

        // 寫入 Redis（快速訪問）
        await _redis.SetStringAsync(
            $"agent:{state.AgentId}",
            json,
            new DistributedCacheEntryOptions {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
            }
        );

        // 寫入 PostgreSQL（持久化）
        var existing = await _db.AgentStates.FindAsync(state.AgentId);
        if (existing != null) {
            existing.State = state.State;
            existing.UpdatedAt = DateTime.UtcNow;
        } else {
            await _db.AgentStates.AddAsync(new AgentStateEntity {
                AgentId = state.AgentId,
                State = state.State,
                CreatedAt = DateTime.UtcNow
            });
        }

        await _db.SaveChangesAsync();
    }
}
```

**權衡** (line 136-156):
- ✅ 優點: 高性能（Redis）+ 高持久性（PostgreSQL）
- ❌ 缺點: 雙寫增加複雜度、Redis 故障時降級為 PostgreSQL only
- 🔄 Phase 2: 遷移到 Redis Cluster（高可用）

---

#### ADR-007: Phase-based Multi-Agent Communication (line 34-189)

**決策**: **MVP 使用 MediatR (In-Process)，Phase 2 遷移到 Azure Service Bus (Cross-Process)**

**理由** (line 56-78):
1. **MVP 簡化**: Docker Compose 環境，所有服務在同一主機
2. **降低成本**: Azure Service Bus Standard 每月 $10+
3. **開發速度**: MediatR 熟悉且易於測試
4. **未來擴展**: Phase 2 K8s 部署時需要跨進程通訊

**MediatR 實現** (line 89-134):
```csharp
// 1. 定義 Agent 通訊消息
public record AgentMessage : INotification
{
    public Guid FromAgentId { get; init; }
    public Guid ToAgentId { get; init; }
    public string Content { get; init; }
    public Dictionary<string, object> Metadata { get; init; }
}

// 2. 實現 Handler（接收方 Agent）
public class AgentMessageHandler : INotificationHandler<AgentMessage>
{
    private readonly IAgentOrchestrator _orchestrator;

    public async Task Handle(AgentMessage message, CancellationToken ct)
    {
        var agent = await _orchestrator.GetAgentAsync(message.ToAgentId);
        await agent.ReceiveMessageAsync(message.Content, message.Metadata);
    }
}

// 3. 發送消息
public class LeaderAgent
{
    private readonly IMediator _mediator;

    public async Task DelegateTaskAsync(Guid workerAgentId, string task)
    {
        await _mediator.Publish(new AgentMessage {
            FromAgentId = this.Id,
            ToAgentId = workerAgentId,
            Content = task,
            Metadata = new Dictionary<string, object> {
                { "priority", "high" },
                { "timeout", 30 }
            }
        });
    }
}
```

**Phase 2 遷移策略** (line 136-189):
```csharp
// 1. 定義統一接口（抽象 MediatR 和 Service Bus）
public interface IAgentMessaging
{
    Task PublishAsync(AgentMessage message, CancellationToken ct = default);
    Task SubscribeAsync(Func<AgentMessage, Task> handler, CancellationToken ct = default);
}

// 2. MediatR 實現（MVP）
public class MediatRMessaging : IAgentMessaging
{
    private readonly IMediator _mediator;
    public Task PublishAsync(AgentMessage message, CancellationToken ct)
        => _mediator.Publish(message, ct);
}

// 3. Service Bus 實現（Phase 2）
public class ServiceBusMessaging : IAgentMessaging
{
    private readonly ServiceBusSender _sender;
    private readonly ServiceBusProcessor _processor;

    public async Task PublishAsync(AgentMessage message, CancellationToken ct)
    {
        var busMessage = new ServiceBusMessage(JsonSerializer.Serialize(message));
        await _sender.SendMessageAsync(busMessage, ct);
    }

    public async Task SubscribeAsync(Func<AgentMessage, Task> handler, CancellationToken ct)
    {
        _processor.ProcessMessageAsync += async args => {
            var message = JsonSerializer.Deserialize<AgentMessage>(args.Message.Body);
            await handler(message);
        };
        await _processor.StartProcessingAsync(ct);
    }
}

// 4. DI 配置（根據環境切換）
services.AddSingleton<IAgentMessaging>(sp => {
    var config = sp.GetRequiredService<IConfiguration>();
    if (config["Environment"] == "Production") {
        return new ServiceBusMessaging(sp.GetRequiredService<ServiceBusSender>());
    }
    return new MediatRMessaging(sp.GetRequiredService<IMediator>());
});
```

---

#### ADR-008: Code Interpreter Container Pool (line 34-223)

**決策**: 使用 **Container Pool（5 個 Standby Containers）** 優化 Python 代碼執行延遲

**問題背景** (line 56-78):
- Docker Container 冷啟動耗時 **2-5 秒**
- 用戶期望代碼執行延遲 <1 秒
- 每次執行都創建新容器無法滿足性能要求

**Container Pool 設計** (line 89-156):
```csharp
public class ContainerPool
{
    private readonly ConcurrentQueue<ICodeInterpreter> _availableContainers = new();
    private readonly int _minPoolSize = 5;  // Standby 容器數
    private readonly int _maxPoolSize = 20; // 最大容器數
    private int _currentPoolSize = 0;

    // 初始化時預熱 5 個容器
    public async Task InitializeAsync()
    {
        for (int i = 0; i < _minPoolSize; i++) {
            var container = await CreateContainerAsync();
            _availableContainers.Enqueue(container);
            Interlocked.Increment(ref _currentPoolSize);
        }
    }

    // 獲取容器（優先從 Pool，Pool 空時創建新容器）
    public async Task<ICodeInterpreter> AcquireAsync(CancellationToken ct = default)
    {
        // 1. 嘗試從 Pool 獲取
        if (_availableContainers.TryDequeue(out var container)) {
            // 2. 異步補充 Pool（保持 Pool 大小）
            _ = Task.Run(() => ReplenishPoolAsync(CancellationToken.None));
            return container;
        }

        // 3. Pool 空，創建新容器（Degraded Mode）
        return await CreateContainerAsync(ct);
    }

    // 釋放容器（歸還 Pool 或銷毀）
    public async Task ReleaseAsync(ICodeInterpreter container)
    {
        // 1. 重置容器狀態（清理臨時文件、重啟 Python 進程）
        await container.ResetAsync();

        // 2. 檢查 Pool 大小
        if (_currentPoolSize < _maxPoolSize) {
            _availableContainers.Enqueue(container);
        } else {
            // Pool 已滿，銷毀容器
            await container.DisposeAsync();
            Interlocked.Decrement(ref _currentPoolSize);
        }
    }

    // 創建新容器
    private async Task<ICodeInterpreter> CreateContainerAsync(CancellationToken ct = default)
    {
        var container = await _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters {
            Image = "python:3.11-slim",
            HostConfig = new HostConfig {
                Memory = 512 * 1024 * 1024,      // 512MB
                CPUQuota = 50000,                 // 0.5 CPU
                NetworkMode = "none",             // 禁用網絡
                ReadonlyRootfs = true,            // 只讀文件系統
                SecurityOpt = new List<string> {
                    "no-new-privileges",          // 禁止權限提升
                    "seccomp=default"             // Seccomp 配置
                }
            }
        }, ct);

        await _dockerClient.Containers.StartContainerAsync(container.ID, null, ct);
        Interlocked.Increment(ref _currentPoolSize);

        return new DockerCodeInterpreter(container.ID, _dockerClient);
    }

    // 補充 Pool（異步）
    private async Task ReplenishPoolAsync(CancellationToken ct)
    {
        while (_availableContainers.Count < _minPoolSize && _currentPoolSize < _maxPoolSize) {
            var container = await CreateContainerAsync(ct);
            _availableContainers.Enqueue(container);
        }
    }
}
```

**4 層安全防護** (line 158-203):
```yaml
Layer 1: 靜態代碼分析
  實現: AST 解析 (ast.parse)
  禁止: import os, subprocess, socket, urllib
  允許: import numpy, pandas, matplotlib

Layer 2: Docker 容器隔離
  每個執行獨立容器
  執行完成後銷毀容器

Layer 3: User Namespace 隔離
  非 root 用戶執行 (uid=1000)
  容器內無 sudo 權限

Layer 4: Seccomp 系統調用過濾
  禁止危險系統調用: execve, fork, clone, mount
  允許: read, write, open, close, mmap
```

**性能指標** (line 205-223):
```yaml
指標:
  容器啟動時間:
    冷啟動: 2-5s
    Pool 預熱: 0.1s ⭐

  代碼執行延遲 (P95):
    目標: <8s
    分解:
      - 容器獲取: <0.1s (from Pool)
      - 代碼分析: <0.2s
      - Python 執行: <7s
      - 結果返回: <0.7s

  Pool 配置:
    Standby: 5 containers
    Max: 20 containers
    補充策略: 異步補充（不阻塞主流程）
```

---

#### ADR-011: Framework Migration Strategy (line 34-178)

**決策**: 使用 **Framework Abstraction Layer** 支持從 Semantic Kernel 遷移到 AutoGen/LangGraph

**理由** (line 56-89):
1. **技術風險**: Semantic Kernel 1.x 可能無法滿足未來複雜場景
2. **生態演進**: AutoGen 和 LangGraph 快速發展，可能成為更好選擇
3. **降低遷移成本**: 抽象層使遷移成為「實現替換」而非「架構重寫」
4. **保護投資**: 業務邏輯代碼不受框架變更影響

**抽象層設計** (line 91-156):
```csharp
// 1. 定義核心接口（與具體框架無關）
public interface IAgentFramework
{
    Task<IAgent> CreateAgentAsync(AgentConfig config, CancellationToken ct = default);
    Task<AgentResponse> ExecuteAsync(IAgent agent, string input, CancellationToken ct = default);
}

public interface IAgent
{
    Guid Id { get; }
    string Name { get; }
    AgentConfig Config { get; }
    Task<AgentResponse> InvokeAsync(string input, CancellationToken ct = default);
}

// 2. Semantic Kernel 實現（MVP）
public class SemanticKernelFramework : IAgentFramework
{
    private readonly Kernel _kernel;

    public async Task<IAgent> CreateAgentAsync(AgentConfig config, CancellationToken ct)
    {
        var skConfig = new KernelConfig {
            DefaultAIService = config.Model,
            Temperature = config.Temperature
        };

        return new SemanticKernelAgent(_kernel, config);
    }

    public async Task<AgentResponse> ExecuteAsync(IAgent agent, string input, CancellationToken ct)
    {
        var skAgent = (SemanticKernelAgent)agent;
        var result = await skAgent.Kernel.InvokePromptAsync(input, ct);
        return new AgentResponse { Content = result.ToString() };
    }
}

// 3. AutoGen 實現（未來）
public class AutoGenFramework : IAgentFramework
{
    public async Task<IAgent> CreateAgentAsync(AgentConfig config, CancellationToken ct)
    {
        var autoGenAgent = new AssistantAgent(
            name: config.Name,
            model: config.Model,
            systemMessage: config.SystemPrompt
        );

        return new AutoGenAgentAdapter(autoGenAgent, config);
    }

    public async Task<AgentResponse> ExecuteAsync(IAgent agent, string input, CancellationToken ct)
    {
        var autoGenAgent = (AutoGenAgentAdapter)agent;
        var messages = await autoGenAgent.Agent.SendMessageAsync(input);
        return new AgentResponse { Content = messages.Last().Content };
    }
}

// 4. DI 配置（動態切換框架）
services.AddSingleton<IAgentFramework>(sp => {
    var config = sp.GetRequiredService<IConfiguration>();
    var framework = config["AgentFramework"]; // "SemanticKernel" | "AutoGen"

    return framework switch {
        "AutoGen" => new AutoGenFramework(),
        _ => new SemanticKernelFramework(sp.GetRequiredService<Kernel>())
    };
});
```

**遷移路徑** (line 158-178):
```yaml
Phase 1 (MVP):
  框架: Semantic Kernel 1.x
  抽象層: 已實現
  遷移準備: 無

Phase 2 (Kubernetes):
  框架: Semantic Kernel 1.x
  抽象層: 穩定使用
  遷移準備: 監控 SK 1.x 性能和限制

Phase 3 (未來):
  評估標準:
    - Semantic Kernel 是否滿足需求
    - AutoGen/LangGraph 生態成熟度
    - 遷移成本評估

  遷移步驟:
    1. 實現 AutoGenFramework adapter
    2. 並行運行（A/B 測試）
    3. 灰度切換（10% → 50% → 100%）
    4. 刪除 SemanticKernelFramework 代碼
```

---

#### ADR-012: Workflow Editor Technology (line 34-189)

**決策**: 使用 **Micro-frontend（React Host + Vue Remote）+ Module Federation** 集成 VueFlow

**問題背景** (line 56-78):
- Workflow 編輯器需要可視化拖拽（節點 + 連線）
- React 生態缺少成熟的 Flow 庫（React Flow 功能不足）
- **VueFlow** 是最佳 Flow 庫（基於 Vue 3）
- 主應用是 React 18，無法直接集成 Vue 組件

**技術選型分析** (line 89-134):
```yaml
選項 1: React Flow (放棄)
  優點:
    - React 生態原生支持
    - 無需跨框架集成
  缺點:
    - 功能不足（無 CRDT 協作、無 Minimap）
    - 社區活躍度低
    - 不滿足需求

選項 2: 完全重寫為 React (放棄)
  優點:
    - 統一技術棧
  缺點:
    - 開發時間長（2-3 個月）
    - 重複造輪子
    - 無法利用 VueFlow 成熟生態

選項 3: Micro-frontend + Module Federation (✅ 採用)
  優點:
    - 利用 VueFlow 成熟功能
    - Isolated Runtime（Vue 和 React 隔離）
    - 開發時間短（1 週集成）
  缺點:
    - Bundle 稍大（+100KB）
    - 跨框架通訊複雜度
```

**實現架構** (line 136-189):
```typescript
// 1. React Host 應用（主應用）
// webpack.config.js
module.exports = {
  plugins: [
    new ModuleFederationPlugin({
      name: 'host',
      remotes: {
        vueWorkflow: 'vueWorkflow@http://localhost:3001/remoteEntry.js'
      },
      shared: {
        react: { singleton: true },
        'react-dom': { singleton: true }
      }
    })
  ]
};

// WorkflowEditorPage.tsx
const VueWorkflowEditor = React.lazy(() => import('vueWorkflow/WorkflowEditor'));

export function WorkflowEditorPage() {
  return (
    <React.Suspense fallback={<Loading />}>
      <VueWorkflowEditor onSave={handleSave} />
    </React.Suspense>
  );
}

// 2. Vue Remote 應用（Workflow Editor）
// webpack.config.js
module.exports = {
  plugins: [
    new ModuleFederationPlugin({
      name: 'vueWorkflow',
      filename: 'remoteEntry.js',
      exposes: {
        './WorkflowEditor': './src/components/WorkflowEditor.vue'
      },
      shared: {
        vue: { singleton: true }
      }
    })
  ]
};

// WorkflowEditor.vue
<template>
  <VueFlow v-model="nodes" v-model:edges="edges">
    <Controls />
    <MiniMap />
    <Background />
  </VueFlow>
</template>

<script setup lang="ts">
import { VueFlow, Controls, MiniMap, Background } from '@vue-flow/core';

const emit = defineEmits(['save']);
const nodes = ref([]);
const edges = ref([]);

function handleSave() {
  emit('save', { nodes: nodes.value, edges: edges.value });
}
</script>

// 3. 跨框架通訊（Event Bridge）
// React → Vue (props)
<VueWorkflowEditor
  initialNodes={nodes}
  onSave={handleSave}
  onNodeClick={handleNodeClick}
/>

// Vue → React (events)
emit('save', workflowData);
emit('nodeClick', node);
```

**性能與維護** (line 191-223):
```yaml
性能影響:
  Bundle 增加: ~100KB (gzip)
  首屏加載: +200ms
  Runtime Overhead: 可忽略（Isolated Runtime）

維護考量:
  開發分離:
    - React Team: 主應用 + 其他頁面
    - Vue Team: Workflow Editor only

  部署獨立:
    - React Host: host.example.com
    - Vue Remote: workflow.example.com/remoteEntry.js

  版本管理:
    - React 18 升級不影響 Vue 3
    - VueFlow 升級不影響 React
```

---

### 2.4 數據庫設計 (database-schema.md)

#### 核心表結構 (line 45-816)

**1. agents 表** (line 78-123):
```sql
CREATE TABLE agents (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    model VARCHAR(100) NOT NULL,  -- 'gpt-4o', 'gpt-4o-mini'
    temperature DECIMAL(3, 2) DEFAULT 0.7,
    system_prompt TEXT,
    plugins JSONB DEFAULT '[]',  -- [{"name": "knowledge-base", "enabled": true}]
    status VARCHAR(20) NOT NULL CHECK (status IN ('draft','active','paused','archived')),
    metadata JSONB DEFAULT '{}',
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

CREATE INDEX idx_agents_user_id ON agents(user_id);
CREATE INDEX idx_agents_status ON agents(status);
CREATE INDEX idx_agents_created_at ON agents(created_at DESC);
```

**2. agent_executions 表** (line 234-289):
```sql
CREATE TABLE agent_executions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    agent_id UUID NOT NULL REFERENCES agents(id) ON DELETE CASCADE,
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    conversation_id UUID REFERENCES conversations(id) ON DELETE SET NULL,

    input TEXT NOT NULL,
    output TEXT,
    function_calls JSONB DEFAULT '[]',  -- [{"name": "get_weather", "args": {...}, "result": {...}}]

    status VARCHAR(20) NOT NULL CHECK (status IN ('pending','running','completed','failed','timeout')),
    error_message TEXT,
    execution_time_ms INTEGER,

    metadata JSONB DEFAULT '{}',  -- { "model": "gpt-4o", "tokens": 1234 }
    started_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    completed_at TIMESTAMP WITH TIME ZONE
);

CREATE INDEX idx_executions_agent_id ON agent_executions(agent_id);
CREATE INDEX idx_executions_conversation_id ON agent_executions(conversation_id);
CREATE INDEX idx_executions_status ON agent_executions(status);
CREATE INDEX idx_executions_started_at ON agent_executions(started_at DESC);
```

**3. plugins 表** (line 345-412):
```sql
CREATE TABLE plugins (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(255) NOT NULL UNIQUE,
    display_name VARCHAR(255) NOT NULL,
    description TEXT,

    plugin_type VARCHAR(50) NOT NULL CHECK (plugin_type IN ('built-in','custom','mcp')),
    category VARCHAR(100),  -- 'knowledge','data','code','communication'

    version VARCHAR(50) NOT NULL,
    dll_path VARCHAR(500),  -- 'plugins/MyPlugin/v1.0.0/MyPlugin.dll'
    entry_class VARCHAR(255),  -- 'MyNamespace.MyPlugin'

    configuration_schema JSONB,  -- JSON Schema for plugin config
    default_configuration JSONB DEFAULT '{}',

    status VARCHAR(20) NOT NULL CHECK (status IN ('active','deprecated','disabled')),

    created_by UUID REFERENCES users(id),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

CREATE INDEX idx_plugins_type ON plugins(plugin_type);
CREATE INDEX idx_plugins_status ON plugins(status);
CREATE INDEX idx_plugins_name ON plugins(name);
```

**4. plugin_versions 表** (US 2.2 熱更新, line 423-478):
```sql
CREATE TABLE plugin_versions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    plugin_id UUID NOT NULL REFERENCES plugins(id) ON DELETE CASCADE,

    version VARCHAR(50) NOT NULL,  -- '1.0.0', '1.1.0'
    dll_path VARCHAR(500) NOT NULL,

    changelog TEXT,
    breaking_changes BOOLEAN DEFAULT FALSE,

    status VARCHAR(20) NOT NULL CHECK (status IN ('active','inactive','deprecated')),

    activated_at TIMESTAMP WITH TIME ZONE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),

    UNIQUE(plugin_id, version)
);

CREATE INDEX idx_plugin_versions_plugin_id ON plugin_versions(plugin_id);
CREATE INDEX idx_plugin_versions_status ON plugin_versions(status);
```

**5. conversations 表** (US 6.1 Chat UI, line 534-589):
```sql
CREATE TABLE conversations (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    agent_id UUID NOT NULL REFERENCES agents(id) ON DELETE CASCADE,

    title VARCHAR(500),
    branch_from_conversation_id UUID REFERENCES conversations(id),  -- 對話分支
    branch_from_message_id UUID,

    status VARCHAR(20) NOT NULL CHECK (status IN ('active','locked','archived')),

    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    last_message_at TIMESTAMP WITH TIME ZONE
);

CREATE INDEX idx_conversations_user_id ON conversations(user_id);
CREATE INDEX idx_conversations_agent_id ON conversations(agent_id);
CREATE INDEX idx_conversations_last_message_at ON conversations(last_message_at DESC);
```

**6. messages 表** (US 6.1 Chat UI, line 601-678):
```sql
CREATE TABLE messages (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    conversation_id UUID NOT NULL REFERENCES conversations(id) ON DELETE CASCADE,

    role VARCHAR(20) NOT NULL CHECK (role IN ('user','assistant','system')),
    content TEXT NOT NULL,

    attachments JSONB DEFAULT '[]',  -- [{"name": "data.csv", "url": "...", "size": 1024}]
    function_calls JSONB DEFAULT '[]',

    token_count INTEGER,
    metadata JSONB DEFAULT '{}',

    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

CREATE INDEX idx_messages_conversation_id ON messages(conversation_id);
CREATE INDEX idx_messages_created_at ON messages(created_at DESC);
```

**其他核心表**:
- `users` (line 125-178) - 用戶管理
- `knowledge_bases` (line 489-532) - 知識庫
- `code_executions` (line 680-734) - Code Interpreter 執行記錄
- `audit_logs` (line 736-789) - 審計日誌

---

## 3. User Stories 用戶故事摘要

### 3.1 文件清單

| Module | User Stories | 行數 | 主要內容 |
|--------|--------------|------|----------|
| module-01-agent-creation.md | US 1.1-1.6 | ~750 | Agent 創建與生命週期 |
| module-02-plugin-system.md | US 2.1-2.3 | ~680 | Plugin 系統 |
| module-03-code-interpreter.md | US 3.1-3.3 | ~590 | Code Interpreter |
| module-04-multi-agent.md | US 4.1-4.4 | ~620 | Multi-Agent 協作 |
| module-05-agent-memory.md | US 5.1-5.3 | ~450 | Agent 記憶與知識 |
| module-06-chat-interface.md | US 6.1-6.6 | ~530 | Chat 界面 |
| module-07-persona-framework.md | US 7.1-7.3 | ~410 | Persona Framework |
| module-08-structured-data.md | US 8.1-8.3 | ~370 | Structured Data |
| module-09-enterprise-features.md | US 9.1-9.3 | ~320 | Enterprise Features |
| module-10-monitoring.md | US 10.1-10.3 | ~289 | 監控與可觀測性 |

### 3.2 Sprint 2 User Stories 詳細分析

Sprint 2 包含 **4 個 User Stories**，總計 **31 Story Points**：
- US 1.4: Agent 執行與監控（5 SP）
- US 2.1: Plugin 註冊（8 SP）
- US 2.2: Plugin 熱更新（13 SP）⭐ 最複雜
- US 6.1: 基本 Chat UI（5 SP）

---

#### US 1.4: Agent 執行與監控 (module-01, line 156-234)

**Story**: 作為 **IT 開發者**，我希望 **執行 Agent 並實時監控執行狀態**，以便 **快速發現和解決問題**

**Story Points**: 5 SP
**Priority**: P0（必須）
**Dependencies**: US 1.1 (Agent 創建), US 1.2 (Agent 配置)

**驗收標準** (line 178-203):
```yaml
AC1: Agent 執行 API
  - POST /api/v1/agents/{id}/execute
  - Request Body: { "input": "用戶輸入", "streaming": true }
  - Response (202 Accepted):
    {
      "execution_id": "uuid",
      "status": "pending",
      "estimated_time": "5s"
    }

AC2: Streaming 回覆 (SignalR)
  - 連接: /hubs/agent-execution
  - 事件:
    - OnExecutionStarted: { "execution_id", "started_at" }
    - OnContentDelta: { "execution_id", "delta": "新增內容" }
    - OnFunctionCall: { "execution_id", "function": {...} }
    - OnExecutionCompleted: { "execution_id", "output", "execution_time_ms" }
    - OnExecutionFailed: { "execution_id", "error" }

AC3: 執行歷史查詢
  - GET /api/v1/agents/{id}/executions?page=1&page_size=20
  - 支持過濾: status, date_range
  - 返回: 執行列表（id, input, output, status, execution_time, started_at）

AC4: 性能指標
  - P95 執行時間: <5s
  - P99 執行時間: <10s
  - Streaming 首字延遲: <2s
```

**技術實施細節** (line 205-234):
```csharp
// 1. Async 執行模式（202 Accepted 模式）
[HttpPost("agents/{id}/execute")]
public async Task<IActionResult> ExecuteAsync(Guid id, [FromBody] ExecuteRequest request)
{
    // 1. 創建 execution 記錄
    var execution = new Execution {
        Id = Guid.NewGuid(),
        AgentId = id,
        Input = request.Input,
        Status = ExecutionStatus.Pending,
        StartedAt = DateTime.UtcNow
    };
    await _context.Executions.AddAsync(execution);
    await _context.SaveChangesAsync();

    // 2. 發布到 RabbitMQ（異步處理）
    await _messagePublisher.PublishAsync(new ExecuteAgentMessage {
        ExecutionId = execution.Id,
        AgentId = id,
        Input = request.Input,
        Streaming = request.Streaming
    });

    // 3. 立即返回 202 Accepted
    return Accepted(new {
        execution_id = execution.Id,
        status = "pending",
        estimated_time = "5s"
    });
}

// 2. Background Worker（RabbitMQ Consumer）
public class AgentExecutionWorker : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var message in _messageConsumer.ConsumeAsync<ExecuteAgentMessage>(stoppingToken))
        {
            // 1. 更新狀態為 Running
            var execution = await _context.Executions.FindAsync(message.ExecutionId);
            execution.Status = ExecutionStatus.Running;
            await _context.SaveChangesAsync();

            // 2. 執行 Agent（Streaming）
            try {
                var agent = await _agentService.GetAgentAsync(message.AgentId);

                if (message.Streaming) {
                    // SignalR Streaming
                    await _hubContext.Clients.User(execution.UserId.ToString())
                        .SendAsync("OnExecutionStarted", new {
                            execution_id = execution.Id,
                            started_at = execution.StartedAt
                        });

                    await foreach (var chunk in agent.ExecuteStreamingAsync(message.Input)) {
                        if (chunk.Type == "content") {
                            await _hubContext.Clients.User(execution.UserId.ToString())
                                .SendAsync("OnContentDelta", new {
                                    execution_id = execution.Id,
                                    delta = chunk.Content
                                });
                        } else if (chunk.Type == "function_call") {
                            await _hubContext.Clients.User(execution.UserId.ToString())
                                .SendAsync("OnFunctionCall", new {
                                    execution_id = execution.Id,
                                    function = chunk.FunctionCall
                                });
                        }
                    }
                }

                var result = await agent.ExecuteAsync(message.Input);

                // 3. 更新狀態為 Completed
                execution.Status = ExecutionStatus.Completed;
                execution.Output = result.Output;
                execution.CompletedAt = DateTime.UtcNow;
                execution.ExecutionTimeMs = (int)(execution.CompletedAt.Value - execution.StartedAt).TotalMilliseconds;

                await _hubContext.Clients.User(execution.UserId.ToString())
                    .SendAsync("OnExecutionCompleted", new {
                        execution_id = execution.Id,
                        output = result.Output,
                        execution_time_ms = execution.ExecutionTimeMs
                    });

            } catch (Exception ex) {
                // 4. 錯誤處理
                execution.Status = ExecutionStatus.Failed;
                execution.ErrorMessage = ex.Message;

                await _hubContext.Clients.User(execution.UserId.ToString())
                    .SendAsync("OnExecutionFailed", new {
                        execution_id = execution.Id,
                        error = ex.Message
                    });
            } finally {
                await _context.SaveChangesAsync();
            }
        }
    }
}
```

---

#### US 2.1: Plugin 註冊與啟用 (module-02, line 45-132)

**Story**: 作為 **IT 開發者**，我希望 **註冊和啟用 Plugin**，以便 **擴展 Agent 功能**

**Story Points**: 8 SP
**Priority**: P0（必須）
**Dependencies**: 無

**驗收標準** (line 67-102):
```yaml
AC1: Plugin 上傳
  - POST /api/v1/plugins/upload
  - Request: multipart/form-data (DLL 文件)
  - Response:
    {
      "plugin_id": "uuid",
      "name": "MyPlugin",
      "version": "1.0.0",
      "status": "uploaded"
    }

AC2: Plugin 註冊
  - POST /api/v1/plugins/register
  - Request Body:
    {
      "name": "MyPlugin",
      "dll_path": "plugins/MyPlugin/v1.0.0/MyPlugin.dll",
      "entry_class": "MyNamespace.MyPlugin",
      "category": "data",
      "configuration_schema": { JSON Schema }
    }
  - Validation:
    - DLL 文件存在且有效
    - Entry Class 繼承 IPlugin 接口
    - Configuration Schema 符合 JSON Schema 規範
  - Response:
    {
      "plugin_id": "uuid",
      "status": "registered"
    }

AC3: Plugin 激活
  - POST /api/v1/plugins/{id}/activate
  - 流程:
    1. 加載 DLL 到 AssemblyLoadContext
    2. 實例化 Plugin 類
    3. 調用 Initialize() 方法
    4. 註冊 Plugin 函數到 Kernel
  - Response:
    {
      "plugin_id": "uuid",
      "status": "active"
    }

AC4: Plugin 列表查詢
  - GET /api/v1/plugins?category=data&status=active
  - Response:
    {
      "plugins": [
        {
          "id": "uuid",
          "name": "MyPlugin",
          "version": "1.0.0",
          "category": "data",
          "status": "active"
        }
      ],
      "total": 10
    }
```

**技術實施細節** (line 104-132):
```csharp
// 1. Plugin 接口定義
public interface IPlugin
{
    string Name { get; }
    string Version { get; }
    Task InitializeAsync(PluginConfig config, CancellationToken ct = default);
    IEnumerable<KernelFunction> GetFunctions();
}

// 2. Plugin 註冊服務
public class PluginRegistrationService
{
    public async Task<Plugin> RegisterPluginAsync(PluginRegistrationRequest request)
    {
        // 1. 驗證 DLL 文件
        if (!File.Exists(request.DllPath)) {
            throw new FileNotFoundException($"DLL not found: {request.DllPath}");
        }

        // 2. 加載 Assembly（臨時）
        var assembly = Assembly.LoadFrom(request.DllPath);
        var pluginType = assembly.GetType(request.EntryClass);

        if (pluginType == null || !typeof(IPlugin).IsAssignableFrom(pluginType)) {
            throw new InvalidOperationException($"Class {request.EntryClass} does not implement IPlugin");
        }

        // 3. 創建 Plugin 記錄
        var plugin = new Plugin {
            Id = Guid.NewGuid(),
            Name = request.Name,
            DisplayName = request.DisplayName,
            PluginType = PluginType.Custom,
            Category = request.Category,
            Version = request.Version,
            DllPath = request.DllPath,
            EntryClass = request.EntryClass,
            ConfigurationSchema = request.ConfigurationSchema,
            Status = PluginStatus.Registered,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Plugins.AddAsync(plugin);
        await _context.SaveChangesAsync();

        return plugin;
    }
}

// 3. Plugin 激活服務
public class PluginActivationService
{
    private readonly Dictionary<Guid, AssemblyLoadContext> _loadContexts = new();

    public async Task<Plugin> ActivatePluginAsync(Guid pluginId)
    {
        var plugin = await _context.Plugins.FindAsync(pluginId);
        if (plugin == null) throw new NotFoundException("Plugin not found");

        // 1. 創建 AssemblyLoadContext（隔離）
        var context = new AssemblyLoadContext($"Plugin_{plugin.Name}_{plugin.Version}", isCollectible: true);
        _loadContexts[pluginId] = context;

        // 2. 加載 Plugin DLL
        var assembly = context.LoadFromAssemblyPath(plugin.DllPath);
        var pluginType = assembly.GetType(plugin.EntryClass);
        var pluginInstance = (IPlugin)Activator.CreateInstance(pluginType);

        // 3. 初始化 Plugin
        await pluginInstance.InitializeAsync(new PluginConfig {
            Configuration = plugin.DefaultConfiguration
        });

        // 4. 註冊函數到 Kernel
        var functions = pluginInstance.GetFunctions();
        foreach (var function in functions) {
            _kernel.Plugins.Add(function);
        }

        // 5. 更新狀態
        plugin.Status = PluginStatus.Active;
        plugin.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return plugin;
    }
}
```

---

#### US 2.2: Plugin 熱更新（Hot Reload）(module-02, line 134-267) ⭐ **最複雜**

**Story**: 作為 **IT 開發者**，我希望 **在不重啟系統的情況下更新 Plugin**，以便 **快速迭代和修復 Bug**

**Story Points**: 13 SP ⭐
**Priority**: P0（必須）
**Dependencies**: US 2.1

**驗收標準** (line 156-207):
```yaml
AC1: 熱更新流程
  1. 上傳新版本 Plugin DLL（通過 UI 或 API）
  2. 系統自動檢測版本變更
  3. 執行中的 Agent 完成當前任務後自動切換到新版本
  4. 新啟動的 Agent 立即使用新版本
  5. 熱更新操作記錄和審計日誌

AC2: 版本管理
  1. 支援多版本並存（v1.0.0 和 v1.1.0 同時存在）
  2. A/B 測試支援（部分 Agent 使用新版本）
  3. 版本切換策略配置（立即切換、優雅切換、手動切換）
  4. 版本回滾機制（5 分鐘內可回滾到舊版本）
  5. 版本相容性檢查（API 破壞性變更警告）

AC3: 優雅卸載
  1. 檢測 Plugin 是否有活躍 Agent 使用
  2. 等待活躍 Agent 完成當前執行
  3. 卸載舊版本 AssemblyLoadContext
  4. 觸發 GC 回收記憶體
  5. 驗證記憶體釋放成功（無記憶體泄漏）

AC4: 熱更新監控
  1. 熱更新成功率監控
  2. 版本切換時間監控
  3. 記憶體使用監控（GC 回收）
  4. 錯誤和回滾次數監控
```

**技術實施細節** (line 209-267):
```csharp
// 1. Plugin 版本管理器
public class PluginVersionManager
{
    private readonly ConcurrentDictionary<Guid, List<PluginVersion>> _versions = new();
    private readonly ConcurrentDictionary<Guid, AssemblyLoadContext> _loadContexts = new();

    // 上傳新版本
    public async Task<PluginVersion> UploadNewVersionAsync(Guid pluginId, string dllPath, string version)
    {
        var plugin = await _context.Plugins.FindAsync(pluginId);

        // 1. 創建版本記錄
        var newVersion = new PluginVersion {
            Id = Guid.NewGuid(),
            PluginId = pluginId,
            Version = version,
            DllPath = dllPath,
            Status = PluginVersionStatus.Inactive,
            CreatedAt = DateTime.UtcNow
        };

        await _context.PluginVersions.AddAsync(newVersion);
        await _context.SaveChangesAsync();

        // 2. 觸發熱更新事件
        await _eventBus.PublishAsync(new PluginVersionUploadedEvent {
            PluginId = pluginId,
            VersionId = newVersion.Id
        });

        return newVersion;
    }

    // 激活新版本（熱更新）
    public async Task ActivateVersionAsync(Guid pluginId, Guid versionId)
    {
        var version = await _context.PluginVersions.FindAsync(versionId);
        var activeAgents = await GetActiveAgentsUsingPluginAsync(pluginId);

        // 1. 創建新的 AssemblyLoadContext
        var newContext = new AssemblyLoadContext($"Plugin_{pluginId}_{version.Version}", isCollectible: true);
        var assembly = newContext.LoadFromAssemblyPath(version.DllPath);
        var pluginType = assembly.GetType(plugin.EntryClass);
        var pluginInstance = (IPlugin)Activator.CreateInstance(pluginType);

        // 2. 初始化新版本
        await pluginInstance.InitializeAsync(new PluginConfig {...});

        // 3. 優雅切換
        if (activeAgents.Any()) {
            // 策略 A: 優雅切換（等待現有 Agent 完成）
            foreach (var agent in activeAgents) {
                agent.OnExecutionCompleted += async (s, e) => {
                    await SwitchPluginVersionAsync(agent, newContext, pluginInstance);
                };
            }
        } else {
            // 策略 B: 立即切換（無活躍 Agent）
            await SwitchAllAgentsAsync(pluginId, newContext, pluginInstance);
        }

        // 4. 標記舊版本為 Inactive
        var oldVersion = await _context.PluginVersions
            .Where(v => v.PluginId == pluginId && v.Status == PluginVersionStatus.Active)
            .FirstOrDefaultAsync();

        if (oldVersion != null) {
            oldVersion.Status = PluginVersionStatus.Inactive;
        }

        // 5. 激活新版本
        version.Status = PluginVersionStatus.Active;
        version.ActivatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // 6. 卸載舊版本（延遲 5 分鐘，支持回滾）
        _ = Task.Delay(TimeSpan.FromMinutes(5)).ContinueWith(async _ => {
            await UnloadOldVersionAsync(oldVersion);
        });
    }

    // 優雅卸載舊版本
    private async Task UnloadOldVersionAsync(PluginVersion oldVersion)
    {
        // 1. 檢查是否還有活躍 Agent 使用
        var activeCount = await GetActiveAgentCountAsync(oldVersion.PluginId);
        if (activeCount > 0) {
            _logger.LogWarning($"Cannot unload version {oldVersion.Version}, still has {activeCount} active agents");
            return;
        }

        // 2. 卸載 AssemblyLoadContext
        if (_loadContexts.TryRemove(oldVersion.Id, out var context)) {
            context.Unload();

            // 3. 觸發 GC 回收
            for (int i = 0; i < 3; i++) {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            _logger.LogInformation($"Successfully unloaded version {oldVersion.Version}");
        }
    }

    // 版本回滾
    public async Task RollbackVersionAsync(Guid pluginId, Guid targetVersionId)
    {
        var targetVersion = await _context.PluginVersions.FindAsync(targetVersionId);

        // 1. 快速回滾（立即切換，無優雅期）
        await ActivateVersionAsync(pluginId, targetVersionId);

        // 2. 記錄回滾事件
        await _auditLogService.LogAsync(new AuditLog {
            Action = "PluginRollback",
            EntityType = "PluginVersion",
            EntityId = targetVersionId,
            Details = $"Rolled back to version {targetVersion.Version}"
        });
    }
}

// 2. 熱更新監控
public class PluginHotReloadMonitor
{
    public async Task RecordHotReloadAsync(Guid pluginId, Guid versionId, bool success, TimeSpan duration)
    {
        await _metricsService.RecordAsync(new Metric {
            Name = "plugin.hot_reload",
            Tags = new Dictionary<string, string> {
                { "plugin_id", pluginId.ToString() },
                { "version_id", versionId.ToString() },
                { "success", success.ToString() }
            },
            Value = duration.TotalMilliseconds,
            Timestamp = DateTime.UtcNow
        });
    }
}
```

---

#### US 6.1: 基本 Chat UI (module-06, line 45-178)

**Story**: 作為 **業務分析師**，我希望 **通過 Chat 界面與 Agent 對話**，以便 **獲取數據分析和報告**

**Story Points**: 5 SP
**Priority**: P0（必須）
**Dependencies**: US 1.4 (Agent 執行)

**驗收標準** (line 67-128):
```yaml
AC1: Chat UI 基本佈局
  - 左側: 對話歷史列表（可搜尋、按時間分組）
  - 中間: 消息流（用戶消息 + Agent 回覆）
  - 右側: Agent 資訊（名稱、模型、狀態）
  - 響應式設計（Desktop / Tablet / Mobile）

AC2: 消息輸入與發送
  - 多行文本輸入框（支持 Shift+Enter 換行）
  - 發送按鈕（Enter 快捷鍵）
  - Token 計數器（實時顯示當前輸入的 Token 數）
  - 禁用狀態（發送中、Agent 執行中）

AC3: Streaming 回覆顯示
  - 實時顯示 Agent 回覆（逐字顯示）
  - 打字動畫（光標閃爍）
  - Stop 按鈕（中斷執行）
  - Function Calling 可視化（展開/收起詳細信息）

AC4: 消息操作
  - 複製消息內容
  - 編輯已發送消息（創建對話分支）
  - 刪除消息
  - 重試失敗消息
  - 消息評分（👍 / 👎）

AC5: 對話管理
  - 創建新對話
  - 搜尋對話歷史
  - 刪除對話
  - 對話標題自動生成（基於首條消息）
```

**技術實施細節** (line 130-178):
```typescript
// 1. 前端狀態管理（Zustand）
interface ConversationStore {
  conversations: Conversation[];
  activeConversationId: string | null;
  isStreaming: boolean;

  // 對話操作
  createConversation: (agentId: string) => Promise<void>;
  loadConversation: (id: string) => Promise<void>;
  deleteConversation: (id: string) => Promise<void>;

  // 消息操作
  sendMessage: (content: string) => Promise<void>;
  editMessage: (messageId: string, newContent: string) => Promise<void>;
  retryMessage: (messageId: string) => Promise<void>;
}

// 2. SignalR 集成（Streaming）
const connection = new HubConnectionBuilder()
  .withUrl("/hubs/agent-execution")
  .withAutomaticReconnect()
  .build();

connection.on("OnContentDelta", (data) => {
  // 更新消息內容（逐字追加）
  updateMessage(data.execution_id, (msg) => ({
    ...msg,
    content: msg.content + data.delta
  }));
});

connection.on("OnFunctionCall", (data) => {
  // 顯示 Function Call 卡片
  addFunctionCall(data.execution_id, data.function);
});

connection.on("OnExecutionCompleted", (data) => {
  // 停止 Streaming 動畫
  setStreaming(false);
});

// 3. 虛擬滾動（支持 >1000 條消息）
import { useVirtualizer } from '@tanstack/react-virtual';

const virtualizer = useVirtualizer({
  count: messages.length,
  getScrollElement: () => parentRef.current,
  estimateSize: () => 100,  // 預估消息高度
  overscan: 5,              // 上下各渲染 5 條額外消息
});

// 4. Token 計數（實時）
const tokenCount = useMemo(() => {
  // 簡化計算：1 token ≈ 4 個字符
  return Math.ceil(inputText.length / 4);
}, [inputText]);
```

---

### 3.3 跨 Module 主題分析

#### 主題 1: Async-first 架構
**出現在**: US 1.4, US 2.2, US 3.1, US 4.2
**模式**: 202 Accepted → RabbitMQ → Background Worker → SignalR
**目的**: 降低感知延遲、提高系統吞吐量

#### 主題 2: Plugin 生命週期管理
**出現在**: US 2.1, US 2.2
**階段**: Upload → Register → Activate → Hot Reload → Deactivate → Unload
**關鍵技術**: AssemblyLoadContext (isCollectible: true)

#### 主題 3: Real-time 通訊
**出現在**: US 1.4, US 6.1
**技術**: SignalR WebSocket
**應用**: Streaming 回覆、執行狀態更新、Token 監控

---

## 4. Technical Implementation 技術實施摘要

### 4.1 文件清單

| 文件 | 行數 | 主要內容 |
|------|------|----------|
| 01-SYSTEM-ARCHITECTURE.md | 1114 | 系統架構總覽、6層架構 |
| 02-MICROSERVICES-DESIGN.md | 1372 | 微服務設計、通訊模式 |
| 03-DATA-FLOW.md | 1500 | 數據流、請求處理流程 |
| 04-API-SPECIFICATION.md | 1192 | RESTful API 規範 |
| 06-DATABASE-SCHEMA.md | ~800 | 數據庫設計（與 architecture/ 重複）|
| 08-SECURITY-ARCHITECTURE.md | ~900 | 7層安全防護模型 |

### 4.2 核心技術架構

#### Microservices 設計模式 (02-MICROSERVICES-DESIGN, line 45-178)

**服務間通訊**:
```yaml
同步通訊:
  Protocol: HTTP/REST
  Use Cases:
    - API Gateway → Agent Orchestration (創建 Agent)
    - Agent Orchestration → Plugin Management (查詢 Plugin)
  優點: 簡單、易於調試
  缺點: 緊耦合、級聯失敗風險

異步通訊:
  Protocol: RabbitMQ (MVP) → Azure Service Bus (Phase 2)
  Use Cases:
    - Agent Execution (長時間任務)
    - Code Interpreter Execution
    - Knowledge Base Indexing
  優點: 解耦、彈性、高吞吐
  缺點: 複雜度高、最終一致性

Event-Driven:
  Pattern: Event Sourcing (部分場景)
  Use Cases:
    - Agent 狀態變更事件
    - Plugin 熱更新事件
  優點: 審計日誌、重放能力
  缺點: 複雜度高、存儲開銷大
```

#### API 設計原則 (04-API-SPECIFICATION, line 34-156)

**RESTful 最佳實踐**:
```yaml
資源命名:
  - 使用名詞複數: /api/v1/agents, /api/v1/plugins
  - 嵌套資源: /api/v1/agents/{id}/executions
  - 避免動詞: ❌ /api/v1/agents/create → ✅ POST /api/v1/agents

HTTP Methods:
  GET: 查詢資源（冪等）
  POST: 創建資源 / 執行操作
  PUT: 完整更新資源（冪等）
  PATCH: 部分更新資源
  DELETE: 刪除資源（冪等）

Status Codes:
  200 OK: 成功（同步）
  202 Accepted: 已接受（異步）
  400 Bad Request: 客戶端錯誤
  401 Unauthorized: 未認證
  403 Forbidden: 已認證但無權限
  404 Not Found: 資源不存在
  500 Internal Server Error: 服務器錯誤

分頁與過濾:
  - Offset-based: ?page=1&page_size=20
  - Cursor-based: ?cursor=abc&limit=20 (推薦)
  - 過濾: ?status=active&category=data
  - 排序: ?sort_by=created_at&order=desc
```

**API 速率限制** (line 189-234):
```csharp
// 1. Token Bucket 算法
public class RateLimitMiddleware
{
    private readonly ConcurrentDictionary<string, TokenBucket> _buckets = new();

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var userId = context.User.FindFirst("sub")?.Value;
        if (userId == null) {
            await next(context);
            return;
        }

        var bucket = _buckets.GetOrAdd(userId, _ => new TokenBucket {
            Capacity = 100,      // 最多 100 個 tokens
            RefillRate = 10,     // 每秒補充 10 個 tokens
            Tokens = 100         // 初始 100 個 tokens
        });

        if (bucket.TryConsume(1)) {
            await next(context);
        } else {
            context.Response.StatusCode = 429; // Too Many Requests
            context.Response.Headers.Add("Retry-After", "60");
            await context.Response.WriteAsJsonAsync(new {
                error = "Rate limit exceeded",
                retry_after = 60
            });
        }
    }
}

public class TokenBucket
{
    public int Capacity { get; set; }
    public int RefillRate { get; set; }
    public int Tokens { get; set; }
    private DateTime LastRefillTime { get; set; } = DateTime.UtcNow;

    public bool TryConsume(int tokens)
    {
        Refill();
        if (Tokens >= tokens) {
            Tokens -= tokens;
            return true;
        }
        return false;
    }

    private void Refill()
    {
        var now = DateTime.UtcNow;
        var elapsed = (now - LastRefillTime).TotalSeconds;
        var tokensToAdd = (int)(elapsed * RefillRate);

        Tokens = Math.Min(Tokens + tokensToAdd, Capacity);
        LastRefillTime = now;
    }
}
```

### 4.3 安全架構 (08-SECURITY-ARCHITECTURE)

#### 7層安全防護模型 (line 34-156)

```yaml
Layer 1: Network Security
  - Azure NSG (Network Security Group)
  - 只允許 HTTPS (443) 和 SSH (22) 入站
  - 內部服務只能通過內部網絡通訊

Layer 2: API Gateway Security
  - OAuth 2.0 + OIDC 認證
  - JWT Token 驗證（RS256 簽名）
  - API Key 管理（用於 M2M 通訊）
  - Rate Limiting（防止 DDoS）

Layer 3: Application Security
  - OWASP Top 10 緩解措施
  - 輸入驗證（ASP.NET Core Model Validation）
  - 輸出編碼（防止 XSS）
  - SQL Injection 防護（EF Core Parameterized Queries）

Layer 4: Data Security
  - Encryption at Rest（Azure Storage Service Encryption）
  - Encryption in Transit（TLS 1.3）
  - 敏感數據脫敏（日誌中不記錄密碼、API Key）

Layer 5: Code Interpreter Security（4層防護）
  - Static Analysis（AST 解析）
  - Container Isolation（Docker）
  - User Namespace Isolation（非 root）
  - Seccomp（系統調用過濾）

Layer 6: Audit & Compliance
  - 所有操作記錄 Audit Log
  - GDPR 合規（數據刪除權、數據導出權）
  - SOC 2 Type II 準備（Phase 2）

Layer 7: Incident Response
  - 安全事件監控（Azure Security Center）
  - 自動告警（P0 漏洞 → Email + Teams）
  - Playbook（漏洞修復流程）
```

#### Authentication & Authorization (line 189-267)

```csharp
// 1. JWT 認證配置
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.Authority = "https://login.microsoftonline.com/{tenant-id}/v2.0";
        options.Audience = "api://semantic-kernel-platform";
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// 2. 授權策略
services.AddAuthorization(options => {
    // 基於角色的授權（RBAC）
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("Developer", policy => policy.RequireRole("Developer", "Admin"));

    // 基於資源的授權（Resource-based）
    options.AddPolicy("AgentOwner", policy => {
        policy.RequireAssertion(context => {
            var userId = context.User.FindFirst("sub")?.Value;
            var agentOwnerId = context.Resource as string;
            return userId == agentOwnerId;
        });
    });

    // 基於聲明的授權（Claims-based）
    options.AddPolicy("CanUseCodeInterpreter", policy => {
        policy.RequireClaim("features", "code-interpreter");
    });
});

// 3. API 使用授權
[HttpGet("agents/{id}")]
[Authorize(Policy = "Developer")]
public async Task<IActionResult> GetAgentAsync(Guid id)
{
    var agent = await _agentService.GetAgentAsync(id);

    // 檢查用戶是否是 Agent 的所有者
    var userId = User.FindFirst("sub")?.Value;
    if (agent.UserId.ToString() != userId && !User.IsInRole("Admin")) {
        return Forbid(); // 403
    }

    return Ok(agent);
}

// 4. Code Interpreter 授權（基於 Feature Flag）
[HttpPost("code-interpreter/execute")]
[Authorize(Policy = "CanUseCodeInterpreter")]
public async Task<IActionResult> ExecuteCodeAsync([FromBody] CodeExecutionRequest request)
{
    // 只有擁有 "code-interpreter" 功能的用戶可以執行
    var result = await _codeInterpreterService.ExecuteAsync(request);
    return Accepted(result);
}
```

---

## 5. UX Design 用戶體驗設計摘要

（Agent 5 已提供完整的 UX Design 摘要，此處簡化引用）

### 5.1 Chat Interface 核心組件（US 6.1）

**3-Column 佈局**:
- 左側欄 (280px): 對話歷史列表
- 中間區 (彈性): 消息流 + 輸入區
- 右側欄 (320px): Agent 資訊 + Token 監控

**關鍵組件**:
1. **MessageBubble** - 消息氣泡（用戶/Agent）
2. **FunctionCallBlock** - Function Calling 可視化
3. **InputArea** - 多行輸入 + Toolbar
4. **TokenMonitor** - Token 使用量監控
5. **VirtualMessageList** - 虛擬滾動（支持 >1000 條）

**性能要求**:
- 消息發送延遲: <100ms
- Agent 回覆首字: <2s
- 對話載入時間: <500ms

**無障礙標準**: WCAG 2.1 AA（鍵盤導航、ARIA 標籤、螢幕閱讀器）

### 5.2 Design System

**色彩系統**:
- Primary: `#2196F3` (藍色)
- Success: `#4CAF50` (綠色)
- Error: `#F44336` (紅色)
- Grey-900: `#212121` (主要文字)

**字體系統**:
- 主要字體: Inter, Noto Sans TC, Segoe UI
- 正文字體: **14px** / 400 / 行高 1.43
- 代碼字體: 13px / JetBrains Mono

**響應式斷點**:
- Desktop: ≥1280px (3-column)
- Tablet: 768-1279px (2-column)
- Mobile: <768px (1-column)

---

## 6. Project Management 項目管理摘要

（Agent 6 已提供完整摘要，此處簡化引用）

### 6.1 項目時間線

**Phase 1 (MVP)**: Week 1-36 (8 個月)
- Week 1-4: 基礎設施搭建
- Week 5-12: 核心功能開發
- Week 13-20: Code Interpreter 開發
- Week 21-28: Multi-Agent 協作
- Week 29-32: 前端與集成測試
- Week 33-36: MVP 驗收與部署

**Phase 2 (Kubernetes)**: Week 37-60 (10 個月)
- Week 37-44: Kubernetes 遷移
- Week 45-48: 自動伸縮與優化
- Week 49-56: 監控、安全與合規
- Week 57-60: 文檔與交付

### 6.2 Sprint 分配

**Sprint 週期**: 2 週
**Phase 1**: 18 個 Sprint
**Phase 2**: 12 個 Sprint

**Sprint 2 範圍** (Week 3-4):
- 關鍵任務: Docker Compose、CI/CD、Application Insights
- 資源: DevOps 100%, Backend 50%, Frontend 25%
- 里程碑: M2 基礎設施就緒 (Week 4)

### 6.3 風險管理

**Top 5 風險**:
1. 🔴 R001: Container 逃逸漏洞
2. 🟡 R002: Agent 執行超時
3. 🟡 R003: Redis 單點故障
4. 🟡 R004: LLM 成本超預算
5. 🟡 R006: 關鍵人員離職

### 6.4 預算控制

**總預算**: TWD $799,000
- Phase 1: TWD $344,000（人力 256K + 基礎設施 56K + 其他 32K）
- Phase 2: TWD $700,000（人力 320K + 基礎設施 263K + 安全審計 80K + 其他 37K）

---

## 7. Sprint 2 關鍵信息整合

### 7.1 Sprint 2 範圍確認

**User Stories** (31 Story Points):
- **US 1.4**: Agent 執行與監控（5 SP）
- **US 2.1**: Plugin 註冊與啟用（8 SP）
- **US 2.2**: Plugin 熱更新（13 SP）⭐ 最複雜
- **US 6.1**: 基本 Chat UI（5 SP）

**基礎設施任務** (Week 3-4):
- Docker Compose 環境完善
- CI/CD Pipeline 優化
- Application Insights 監控配置

### 7.2 技術實施關鍵點

#### Backend 關鍵實現

1. **Async 執行模式** (US 1.4):
   - 202 Accepted → RabbitMQ → Background Worker → SignalR
   - P95 執行時間 <5s
   - Streaming 首字延遲 <2s

2. **Plugin 熱更新** (US 2.2):
   - AssemblyLoadContext (isCollectible: true)
   - Container Pool（5 個 Standby）
   - 優雅卸載（等待活躍 Agent 完成）
   - 版本回滾（5 分鐘內）

3. **安全防護** (US 2.1):
   - 4 層 Code Interpreter 防護
   - OAuth 2.0 + JWT 認證
   - Rate Limiting (Token Bucket)

#### Frontend 關鍵實現

1. **Chat UI 架構** (US 6.1):
   - 3-Column 佈局（Desktop）
   - SignalR Streaming 集成
   - 虛擬滾動（@tanstack/react-virtual）
   - Token 監控（實時計數）

2. **組件優先級**:
   - P0: MessageBubble, InputArea, ConversationList
   - P1: FunctionCallBlock, VirtualMessageList
   - P2: AttachmentUpload, VoiceInput

### 7.3 驗收標準綜合

**Backend**:
- ✅ API 響應時間 P95 <300ms
- ✅ Agent 執行時間 P95 <5s
- ✅ Plugin 熱更新成功率 >95%
- ✅ 單元測試覆蓋率 ≥80%

**Frontend**:
- ✅ 消息發送延遲 <100ms
- ✅ 對話載入時間 <500ms
- ✅ Streaming 首字延遲 <2s
- ✅ 無障礙合規（WCAG 2.1 AA）

**基礎設施**:
- ✅ CI/CD Pipeline 運行正常
- ✅ Docker Compose 可啟動所有服務
- ✅ Application Insights 收集基本指標

### 7.4 依賴與風險

**關鍵依賴**:
- Azure 資源創建（Week 1 完成）
- Docker Compose 環境（Week 1-4）
- SignalR 配置（Week 3-4）

**Sprint 2 特定風險**:
- 🟡 Redis 單點故障（Phase 1 接受）
- 🟡 預算超支（嚴格成本控制）
- 🟢 Plugin 熱更新複雜度（13 SP）

---

## 8. 跨文件主題分析

### 8.1 主題：Async-first 架構模式

**出現在**:
- Brief-3 (line 189-234): 技術架構異步設計
- Architecture ADR-007: Multi-Agent 通訊
- User Stories US 1.4: Agent 執行異步模式
- Technical Implementation 02-MICROSERVICES: 異步通訊模式

**核心模式**:
```
Client → API (202 Accepted) → RabbitMQ → Background Worker → SignalR (Real-time Update) → Client
```

**優勢**:
- 降低感知延遲（立即返回 202）
- 提高吞吐量（解耦）
- 容錯能力（消息隊列重試）

**應用場景**:
- Agent 執行（長時間 LLM 調用）
- Code Interpreter 執行（Python 代碼運行）
- Knowledge Base 索引（大文件處理）

---

### 8.2 主題：Plugin 生命週期管理

**出現在**:
- Brief-3 (line 267-312): Plugin 系統概述
- Architecture ADR-011: Framework Abstraction
- User Stories US 2.1, US 2.2: Plugin 註冊與熱更新
- Database Schema: plugins, plugin_versions 表

**生命週期階段**:
```yaml
1. Upload: 上傳 DLL 文件
2. Register: 驗證並註冊 Plugin
3. Activate: 加載到 AssemblyLoadContext
4. Hot Reload: 更新版本（優雅切換）
5. Deactivate: 停用 Plugin
6. Unload: 卸載 AssemblyLoadContext + GC
```

**關鍵技術**:
- **AssemblyLoadContext (isCollectible: true)**: 支持動態卸載
- **Container Pool**: 預熱容器降低冷啟動
- **優雅切換**: 等待活躍 Agent 完成後切換
- **版本回滾**: 5 分鐘內可回滾

---

### 8.3 主題：Real-time 通訊（SignalR）

**出現在**:
- Brief-3 (line 334-378): Real-time 通訊需求
- User Stories US 1.4, US 6.1: Streaming 回覆
- UX Design 05-conversation: Chat UI Streaming
- Technical Implementation 03-DATA-FLOW: 數據流設計

**應用場景**:
```yaml
Agent 執行狀態更新:
  - OnExecutionStarted
  - OnContentDelta (Streaming)
  - OnFunctionCall
  - OnExecutionCompleted / OnExecutionFailed

Chat UI 實時互動:
  - 打字動畫（逐字顯示）
  - Token 計數更新
  - 執行進度提示

Multi-Agent 協作:
  - Agent 間消息傳遞
  - 狀態同步
```

**性能優化**:
- **WebSocket 長連接**: 降低握手開銷
- **Binary 編碼**: MessagePack 替代 JSON
- **背壓管理**: 客戶端處理速度控制

---

### 8.4 主題：4 層 Code Interpreter 安全防護

**出現在**:
- Brief-3 (line 280-306): 安全防護概述
- Architecture ADR-008: Container Pool 設計
- Technical Implementation 08-SECURITY: 安全架構
- User Stories US 3.1: Code Interpreter 執行

**4 層防護**:
```yaml
Layer 1: 靜態代碼分析（AST）
  工具: ast.parse (Python)
  禁止: import os, subprocess, socket
  允許: numpy, pandas, matplotlib

Layer 2: Docker 容器隔離
  每個執行獨立容器
  執行完成後銷毀

Layer 3: User Namespace 隔離
  非 root 用戶執行 (uid=1000)
  無 sudo 權限

Layer 4: Seccomp 系統調用過濾
  禁止: execve, fork, clone, mount
  允許: read, write, open, close
```

**性能 vs 安全平衡**:
- Container Pool（5 個 Standby）→ 冷啟動 2-5s 降至 0.1s
- 安全隔離 vs 性能開銷 → 可接受（P95 <8s）

---

### 8.5 主題：MVP 簡化策略

**出現在**:
- Brief-3 (line 112-134): MVP 哲學
- Architecture ADR-007: MediatR vs Service Bus
- Project Management: Phase 1 vs Phase 2
- Technical Implementation: Docker Compose vs Kubernetes

**簡化決策**:
```yaml
基礎設施:
  MVP: Docker Compose
  Phase 2: Kubernetes (AKS)
  理由: 降低複雜度、加快交付

消息隊列:
  MVP: RabbitMQ
  Phase 2: Azure Service Bus
  理由: 降低成本（Service Bus $10+/月）

狀態存儲:
  MVP: Redis 單實例
  Phase 2: Redis Cluster
  理由: 成本優化、可接受單點風險

Multi-Agent 通訊:
  MVP: MediatR (In-Process)
  Phase 2: Service Bus (Cross-Process)
  理由: 簡化開發、Docker Compose 單主機環境
```

**遷移策略**:
- **抽象層設計**: IAgentMessaging, IStateStore（支持無縫切換）
- **灰度遷移**: 10% → 50% → 100%
- **藍綠部署**: 保留 MVP 環境作為備份

---

## 9. 總結與下一步

### 9.1 /docs 閱讀完成度

✅ **已完成深入閱讀**:
- Brief 文件（4 個，~4,809 行）
- Architecture 設計（9 個，~5,500 行）
- User Stories（10 modules，42 US，~4,809 行）
- Technical Implementation（6+ 個，~1,192+ 行）
- UX Design（26 個，~40,000+ 行）
- Project Management（1 個，1,126 行）

**總計**: 56+ 文件，~57,436+ 行代碼和文檔

### 9.2 關鍵發現總結

1. **Code Interpreter 是核心差異化功能**（4層安全、Container Pool）
2. **Async-first 架構貫穿始終**（202 Accepted → RabbitMQ → SignalR）
3. **Plugin 熱更新是技術難點**（AssemblyLoadContext, 優雅切換, 13 SP）
4. **Chat UI 是用戶體驗核心**（3-Column, Streaming, 無障礙）
5. **MVP 簡化策略明確**（Docker Compose, MediatR, Redis 單實例）

### 9.3 Sprint 2 準備充分度評估

**✅ 充分準備**:
- 詳細的驗收標準（US 1.4, 2.1, 2.2, 6.1）
- 完整的技術實施細節（代碼範例、架構圖）
- 明確的 UI 設計規格（組件、佈局、性能）
- 清晰的項目管理計劃（時間、資源、風險）

**⚠️ 需要注意**:
- US 2.2（Plugin 熱更新）複雜度高（13 SP），需要額外技術預研
- Redis 單點故障風險（Phase 1 接受，Phase 2 遷移到 Cluster）
- LLM 成本控制（實施 Prompt 緩存、動態模型選擇）

### 9.4 下一步行動（Phase 2）

**立即任務**:
1. ✅ 完成 /docs 內容索引（本文檔）
2. ⏳ 基於索引重建 /claudedocs/1-planning（7 個文件）
3. ⏳ 基於 Planning 重建 Sprint 1 文檔
4. ⏳ 基於 Planning 重建 Sprint 2 文檔
5. ⏳ 創建標準化 Sprint 文檔創建流程 SOP

**長期目標**:
- 為每個 Sprint 提供完整的引用材料（/docs line numbers）
- 確保 /claudedocs 與 /docs 保持一致性
- 建立自動化文檔同步機制（可選）

---

**文檔版本**: v1.0
**最後更新**: 2025-11-12
**維護者**: AI Agent Team
**使用指引**:
- 本索引作為 /docs 內容的快速查找參考
- 所有關鍵信息都包含行號引用，方便回溯原文
- 為 /claudedocs/1-planning 重建提供完整的素材庫
