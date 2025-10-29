# C4 Model 系統架構圖

本文檔基於 [C4 Model](https://c4model.com/) 架構可視化方法，從不同抽象層次展示 Semantic Kernel Agentic Framework 的系統架構。

## Level 1: System Context（系統上下文圖）

展示系統與外部用戶、外部系統的交互關係。

```mermaid
C4Context
    title System Context Diagram - Semantic Kernel Agentic Framework

    Person(businessUser, "業務用戶", "使用 Agent 完成業務任務")
    Person(developer, "IT 開發者", "創建和配置 Agent")
    Person(analyst, "業務分析師", "設計業務流程")

    System(agenticFramework, "Semantic Kernel Agentic Framework", "多 Agent 協作框架，支持 Code Interpreter")

    System_Ext(azureOpenAI, "Azure OpenAI", "提供 GPT-4 模型")
    System_Ext(azureSearch, "Azure AI Search", "向量檢索")
    System_Ext(azureBlob, "Azure Blob Storage", "文件存儲")
    System_Ext(entraID, "Microsoft Entra ID", "身份認證")

    Rel(businessUser, agenticFramework, "使用", "HTTPS")
    Rel(developer, agenticFramework, "開發和配置", "HTTPS")
    Rel(analyst, agenticFramework, "設計流程", "HTTPS")

    Rel(agenticFramework, azureOpenAI, "調用 LLM", "HTTPS/REST")
    Rel(agenticFramework, azureSearch, "向量檢索", "HTTPS/REST")
    Rel(agenticFramework, azureBlob, "存儲文件", "HTTPS/REST")
    Rel(agenticFramework, entraID, "認證授權", "OAuth 2.0")

    UpdateLayoutConfig($c4ShapeInRow="3", $c4BoundaryInRow="1")
```

### 說明

**用戶角色**：
- **業務用戶**：使用 Agent 執行任務（如數據分析、報告生成）
- **IT 開發者**：創建自定義 Plugin、配置 Agent
- **業務分析師**：設計 Multi-Agent 協作流程

**外部系統**：
- **Azure OpenAI**：核心 LLM 服務（GPT-4、GPT-4o）
- **Azure AI Search**：向量數據庫（RAG 檢索）
- **Azure Blob Storage**：文件和知識庫存儲
- **Microsoft Entra ID**：統一身份認證

---

## Level 2: Container（容器圖）

展示系統內部的主要容器（應用程序、數據庫、服務）及其交互。

```mermaid
C4Container
    title Container Diagram - Semantic Kernel Agentic Framework

    Person(user, "用戶", "業務用戶/開發者")

    Container_Boundary(frontend, "前端層") {
        Container(webUI, "Web UI", "React + TypeScript", "Agent 管理界面")
        Container(chatUI, "Chat UI", "React + SignalR", "對話界面")
    }

    Container_Boundary(backend, "後端層") {
        Container(apiGateway, "API Gateway", ".NET 8", "統一 API 入口")
        Container(agentOrchestrator, "Agent Orchestrator", ".NET 8 + Semantic Kernel", "Agent 編排引擎")
        Container(pluginRegistry, "Plugin Registry", ".NET 8", "Plugin 管理")
        Container(codeInterpreter, "Code Interpreter Service", ".NET 8 + Docker", "Python/R 代碼執行")
    }

    Container_Boundary(messaging, "消息層") {
        Container(eventBus, "Event Bus", "MediatR (Phase 1)", "Agent 通訊")
        Container(messageBroker, "Message Broker", "Azure Service Bus (Phase 2)", "分布式消息隊列")
    }

    Container_Boundary(storage, "存儲層") {
        ContainerDb(redis, "Redis", "Redis 7.x", "Agent 狀態緩存")
        ContainerDb(postgres, "PostgreSQL", "PostgreSQL 15", "持久化存儲")
        ContainerDb(vectorDB, "Vector DB", "Azure AI Search", "向量檢索")
    }

    Container_Boundary(infrastructure, "基礎設施層") {
        Container(containerPool, "Container Pool", "Docker", "Code Interpreter 容器池")
        Container(monitoring, "Monitoring", "Application Insights", "監控與日誌")
    }

    System_Ext(azureOpenAI, "Azure OpenAI", "LLM 服務")

    Rel(user, webUI, "管理 Agent", "HTTPS")
    Rel(user, chatUI, "對話", "HTTPS/WebSocket")

    Rel(webUI, apiGateway, "調用 API", "HTTPS/REST")
    Rel(chatUI, apiGateway, "調用 API", "HTTPS/REST")

    Rel(apiGateway, agentOrchestrator, "編排 Agent", "內部調用")
    Rel(apiGateway, pluginRegistry, "管理 Plugin", "內部調用")

    Rel(agentOrchestrator, codeInterpreter, "執行代碼", "內部調用")
    Rel(agentOrchestrator, eventBus, "發送消息", "In-Process")
    Rel(agentOrchestrator, messageBroker, "發送消息", "AMQP")
    Rel(agentOrchestrator, azureOpenAI, "調用 LLM", "HTTPS/REST")

    Rel(agentOrchestrator, redis, "讀寫狀態", "Redis Protocol")
    Rel(agentOrchestrator, postgres, "持久化", "SQL")
    Rel(agentOrchestrator, vectorDB, "向量檢索", "HTTPS/REST")

    Rel(codeInterpreter, containerPool, "獲取容器", "Docker API")

    Rel(apiGateway, monitoring, "發送日誌", "HTTPS")
    Rel(agentOrchestrator, monitoring, "發送日誌", "HTTPS")

    UpdateLayoutConfig($c4ShapeInRow="2", $c4BoundaryInRow="1")
```

### 容器說明

**前端層**：
- **Web UI**：Agent 管理、配置、監控界面（React + TypeScript）
- **Chat UI**：對話界面，支持 Streaming（React + SignalR）

**後端層**：
- **API Gateway**：統一 API 入口，處理認證、授權、路由
- **Agent Orchestrator**：核心編排引擎，基於 Semantic Kernel
- **Plugin Registry**：Plugin 註冊、發現、版本管理
- **Code Interpreter Service**：Python/R 代碼執行服務

**消息層**：
- **Event Bus (MediatR)**：Phase 1 內存事件總線
- **Message Broker (Azure Service Bus)**：Phase 2 分布式消息隊列

**存儲層**：
- **Redis**：Agent 狀態緩存（短期狀態）
- **PostgreSQL**：持久化存儲（Agent 配置、執行歷史）
- **Vector DB (Azure AI Search)**：向量檢索（RAG）

**基礎設施層**：
- **Container Pool**：Docker 容器池（Code Interpreter）
- **Monitoring (Application Insights)**：監控、日誌、追蹤

---

## Level 3: Component（組件圖）

展示 **Agent Orchestrator** 內部的主要組件及其職責。

```mermaid
C4Component
    title Component Diagram - Agent Orchestrator

    Container_Boundary(orchestrator, "Agent Orchestrator") {
        Component(agentEngine, "Agent Engine", "Semantic Kernel Agent", "Agent 執行引擎")
        Component(taskPlanner, "Task Planner", "Task Decomposition", "任務分解與規劃")
        Component(pluginExecutor, "Plugin Executor", "Plugin Invocation", "Plugin 調用管理")
        Component(memoryManager, "Memory Manager", "Conversation History", "對話記憶管理")
        Component(stateManager, "State Manager", "Agent State", "Agent 狀態管理")
        Component(messagingHub, "Messaging Hub", "Agent Communication", "Agent 通訊中心")
    }

    Container(codeInterpreter, "Code Interpreter Service", ".NET 8 + Docker", "代碼執行")
    ContainerDb(redis, "Redis", "State Cache", "狀態緩存")
    ContainerDb(postgres, "PostgreSQL", "Persistent Storage", "持久化")
    System_Ext(azureOpenAI, "Azure OpenAI", "LLM 服務")

    Rel(agentEngine, taskPlanner, "請求任務規劃", "內部調用")
    Rel(agentEngine, pluginExecutor, "調用 Plugin", "內部調用")
    Rel(agentEngine, memoryManager, "讀寫記憶", "內部調用")
    Rel(agentEngine, stateManager, "管理狀態", "內部調用")
    Rel(agentEngine, messagingHub, "Agent 通訊", "內部調用")

    Rel(taskPlanner, azureOpenAI, "生成計劃", "HTTPS/REST")
    Rel(pluginExecutor, codeInterpreter, "執行代碼", "內部調用")

    Rel(memoryManager, redis, "緩存對話", "Redis Protocol")
    Rel(memoryManager, postgres, "持久化對話", "SQL")

    Rel(stateManager, redis, "緩存狀態", "Redis Protocol")
    Rel(stateManager, postgres, "持久化狀態", "SQL")

    Rel(messagingHub, messagingHub, "Multi-Agent 通訊", "Event Bus")

    UpdateLayoutConfig($c4ShapeInRow="3", $c4BoundaryInRow="1")
```

### 組件說明

**Agent Engine（核心引擎）**：
- 基於 Semantic Kernel Agent API
- 執行 Agent 生命週期管理（創建、運行、暫停、終止）
- 協調其他組件完成 Agent 任務

**Task Planner（任務規劃器）**：
- 將複雜任務分解為子任務
- 使用 LLM 生成執行計劃（Stepwise Planner）
- 決定 Plugin 調用順序

**Plugin Executor（Plugin 執行器）**：
- Plugin 調用管理（本地 Plugin、Code Interpreter）
- 參數驗證與類型轉換
- 錯誤處理與重試邏輯

**Memory Manager（記憶管理器）**：
- 對話歷史管理（短期記憶）
- 向量檢索（長期記憶 - RAG）
- 記憶壓縮與摘要

**State Manager（狀態管理器）**：
- 實現 ADR-006 的 Hybrid 狀態管理
- Redis 緩存 + PostgreSQL 持久化
- 狀態序列化與反序列化

**Messaging Hub（通訊中心）**：
- 實現 ADR-007 的 Multi-Agent 通訊
- Phase 1: MediatR（內存事件）
- Phase 2: Azure Service Bus（分布式消息）

---

## 部署架構圖

### Phase 1 (MVP): Docker Compose 部署

```mermaid
graph TB
    subgraph "Docker Compose - 單機部署"
        subgraph "Frontend Containers"
            WebUI[Web UI<br/>React App<br/>Port: 3000]
        end

        subgraph "Backend Containers"
            API[API Gateway<br/>.NET 8<br/>Port: 5000]
            Orchestrator[Agent Orchestrator<br/>.NET 8 + SK<br/>Port: 5001]
            CodeInterpreter[Code Interpreter<br/>.NET 8<br/>Port: 5002]
        end

        subgraph "Storage Containers"
            Redis[Redis<br/>Port: 6379]
            Postgres[PostgreSQL<br/>Port: 5432]
        end

        subgraph "Container Pool"
            Pool[Docker Container Pool<br/>5 Standby Containers]
        end

        WebUI -->|HTTPS| API
        API -->|HTTP| Orchestrator
        Orchestrator -->|HTTP| CodeInterpreter
        CodeInterpreter -->|Docker API| Pool

        Orchestrator -->|Redis Protocol| Redis
        Orchestrator -->|SQL| Postgres
    end

    subgraph "Azure Services"
        OpenAI[Azure OpenAI<br/>GPT-4]
        Search[Azure AI Search<br/>Vector DB]
        Blob[Azure Blob<br/>File Storage]
    end

    Orchestrator -->|HTTPS| OpenAI
    Orchestrator -->|HTTPS| Search
    Orchestrator -->|HTTPS| Blob

    style WebUI fill:#4CAF50
    style API fill:#2196F3
    style Orchestrator fill:#FF9800
    style Redis fill:#F44336
    style Postgres fill:#9C27B0
```

### Phase 2: Kubernetes 部署

```mermaid
graph TB
    subgraph "Kubernetes Cluster"
        subgraph "Frontend Namespace"
            WebPod1[Web UI Pod 1]
            WebPod2[Web UI Pod 2]
            WebService[Web UI Service<br/>LoadBalancer]

            WebService --> WebPod1
            WebService --> WebPod2
        end

        subgraph "Backend Namespace"
            APIPod1[API Gateway Pod 1]
            APIPod2[API Gateway Pod 2]
            APIService[API Service<br/>ClusterIP]

            OrchestratorPod1[Orchestrator Pod 1]
            OrchestratorPod2[Orchestrator Pod 2]
            OrchestratorPod3[Orchestrator Pod 3]
            OrchestratorService[Orchestrator Service<br/>ClusterIP]

            CodeInterpreterPod1[Code Interpreter Pod 1]
            CodeInterpreterPod2[Code Interpreter Pod 2]
            CodeInterpreterService[Code Interpreter Service<br/>ClusterIP]

            APIService --> APIPod1
            APIService --> APIPod2

            OrchestratorService --> OrchestratorPod1
            OrchestratorService --> OrchestratorPod2
            OrchestratorService --> OrchestratorPod3

            CodeInterpreterService --> CodeInterpreterPod1
            CodeInterpreterService --> CodeInterpreterPod2
        end

        subgraph "Storage Namespace"
            RedisCluster[Redis Cluster<br/>3 Masters + 3 Slaves]
            PostgresHA[PostgreSQL HA<br/>Primary + Standby]
        end

        WebService -->|HTTPS| APIService
        APIService -->|HTTP| OrchestratorService
        OrchestratorService -->|HTTP| CodeInterpreterService

        OrchestratorService -->|Redis Protocol| RedisCluster
        OrchestratorService -->|SQL| PostgresHA
    end

    subgraph "Azure Services"
        ServiceBus[Azure Service Bus<br/>Multi-Agent Messaging]
        OpenAI[Azure OpenAI]
        Search[Azure AI Search]
        Blob[Azure Blob Storage]
    end

    OrchestratorService -->|AMQP| ServiceBus
    OrchestratorService -->|HTTPS| OpenAI
    OrchestratorService -->|HTTPS| Search
    OrchestratorService -->|HTTPS| Blob

    style WebService fill:#4CAF50
    style APIService fill:#2196F3
    style OrchestratorService fill:#FF9800
    style RedisCluster fill:#F44336
    style PostgresHA fill:#9C27B0
```

---

## 數據流圖

### Agent 執行流程（單 Agent）

```mermaid
sequenceDiagram
    participant User
    participant API as API Gateway
    participant Orchestrator as Agent Orchestrator
    participant LLM as Azure OpenAI
    participant Plugin as Plugin Executor
    participant State as State Manager
    participant Redis
    participant Postgres

    User->>API: POST /api/agents/{id}/execute
    API->>Orchestrator: ExecuteAgentAsync(request)

    Orchestrator->>State: LoadStateAsync(agentId)
    State->>Redis: GET agent:state:{agentId}
    Redis-->>State: 狀態數據
    State-->>Orchestrator: AgentState

    Orchestrator->>LLM: 生成執行計劃
    LLM-->>Orchestrator: TaskPlan

    loop 執行每個任務
        Orchestrator->>Plugin: ExecutePluginAsync(pluginName, params)
        Plugin-->>Orchestrator: PluginResult

        Orchestrator->>State: SaveStateAsync(agentId, state)
        State->>Redis: SET agent:state:{agentId}
        Redis-->>State: OK
    end

    Orchestrator->>LLM: 生成最終響應
    LLM-->>Orchestrator: FinalResponse

    Orchestrator->>State: SaveStateAsync(agentId, state, persistent: true)
    State->>Redis: SET agent:state:{agentId}
    State->>Postgres: UpsertAgentStateAsync(agentId, state)
    Postgres-->>State: OK
    Redis-->>State: OK

    Orchestrator-->>API: ExecutionResult
    API-->>User: 200 OK + Result
```

### Multi-Agent 協作流程

```mermaid
sequenceDiagram
    participant User
    participant OrchestratorAgent
    participant EventBus as Event Bus (MediatR)
    participant Worker1 as Worker Agent 1
    participant Worker2 as Worker Agent 2
    participant LLM as Azure OpenAI

    User->>OrchestratorAgent: 執行複雜任務
    OrchestratorAgent->>LLM: 分解任務
    LLM-->>OrchestratorAgent: 子任務列表

    OrchestratorAgent->>EventBus: Publish(TaskDelegation, Worker1)
    OrchestratorAgent->>EventBus: Publish(TaskDelegation, Worker2)

    EventBus-->>Worker1: TaskDelegation 消息
    EventBus-->>Worker2: TaskDelegation 消息

    par 並行執行子任務
        Worker1->>LLM: 執行子任務 1
        LLM-->>Worker1: 結果 1
        Worker1->>EventBus: Publish(TaskCompleted, Orchestrator)

        Worker2->>LLM: 執行子任務 2
        LLM-->>Worker2: 結果 2
        Worker2->>EventBus: Publish(TaskCompleted, Orchestrator)
    end

    EventBus-->>OrchestratorAgent: TaskCompleted (Worker1)
    EventBus-->>OrchestratorAgent: TaskCompleted (Worker2)

    OrchestratorAgent->>LLM: 綜合結果
    LLM-->>OrchestratorAgent: 最終結果

    OrchestratorAgent-->>User: 完成
```

### Code Interpreter 執行流程

```mermaid
sequenceDiagram
    participant Agent as Agent Orchestrator
    participant CodeService as Code Interpreter Service
    participant Pool as Container Pool
    participant Container as Docker Container
    participant Docker as Docker Engine

    Agent->>CodeService: ExecuteCodeAsync(pythonCode, context)
    CodeService->>Pool: AcquireAsync()

    alt Pool 有可用容器
        Pool->>Container: 從 Pool 獲取容器
        Pool-->>CodeService: Container (<2s)
    else Pool 為空
        Pool->>Docker: CreateContainerAsync(securityConfig)
        Docker-->>Pool: 新容器 (5-10s)
        Pool-->>CodeService: Container
    end

    CodeService->>Container: 注入代碼 + 上下文
    CodeService->>Container: ExecuteAsync(timeout: 30s)

    alt 執行成功
        Container-->>CodeService: 執行結果 + 輸出
    else 執行超時
        CodeService->>Container: 強制終止
        Container-->>CodeService: Timeout Error
    else 執行錯誤
        Container-->>CodeService: Runtime Error
    end

    CodeService->>Pool: ReleaseAsync(container)
    Pool->>Docker: StopContainerAsync(container)
    Pool->>Docker: RemoveContainerAsync(container)
    Docker-->>Pool: 容器已銷毀

    Pool->>Pool: 異步補充 Pool（非阻塞）

    CodeService-->>Agent: CodeExecutionResult
```

---

## 安全架構圖

### 4 層安全防護

```mermaid
graph TB
    subgraph "Layer 4: Application Security"
        Auth[身份認證<br/>Entra ID]
        RBAC[角色授權<br/>RBAC]
        InputVal[輸入驗證<br/>Anti-XSS/SQLi]
    end

    subgraph "Layer 3: Code Interpreter Security"
        NetworkIsolation[Layer 1: Network Isolation<br/>Docker Network Mode = none]
        ResourceLimit[Layer 2: Resource Limits<br/>CPU/Memory/Disk/Pids]
        Timeout[Layer 3: Execution Timeout<br/>30 秒強制終止]
        ReadOnlyFS[Layer 4: Read-Only Filesystem<br/>僅 /tmp 可寫]
    end

    subgraph "Layer 2: Infrastructure Security"
        TLS[TLS 1.3 加密<br/>HTTPS/WSS]
        Firewall[防火牆<br/>NSG + WAF]
        DDoSProtection[DDoS 防護<br/>Azure DDoS]
    end

    subgraph "Layer 1: Data Security"
        Encryption[數據加密<br/>AES-256]
        KeyVault[密鑰管理<br/>Azure Key Vault]
        Backup[數據備份<br/>每日備份]
    end

    User[用戶] -->|HTTPS| Auth
    Auth --> RBAC
    RBAC --> InputVal

    InputVal --> TLS
    TLS --> Firewall
    Firewall --> DDoSProtection

    DDoSProtection --> NetworkIsolation
    NetworkIsolation --> ResourceLimit
    ResourceLimit --> Timeout
    Timeout --> ReadOnlyFS

    ReadOnlyFS --> Encryption
    Encryption --> KeyVault
    KeyVault --> Backup

    style NetworkIsolation fill:#F44336,color:#fff
    style ResourceLimit fill:#FF9800,color:#fff
    style Timeout fill:#FF9800,color:#fff
    style ReadOnlyFS fill:#FF9800,color:#fff
```

---

## 監控與可觀察性架構

```mermaid
graph TB
    subgraph "應用層"
        WebUI[Web UI]
        API[API Gateway]
        Orchestrator[Agent Orchestrator]
        CodeInterpreter[Code Interpreter]
    end

    subgraph "監控層"
        AppInsights[Application Insights<br/>日誌 + 追蹤]
        Metrics[Metrics<br/>Prometheus + Grafana]
        Alerts[Alerts<br/>Azure Monitor]
    end

    subgraph "可視化層"
        Dashboard[Grafana Dashboard<br/>實時監控]
        Logs[Log Analytics<br/>日誌查詢]
        Traces[Distributed Tracing<br/>請求追蹤]
    end

    WebUI -->|Telemetry| AppInsights
    API -->|Telemetry| AppInsights
    Orchestrator -->|Telemetry| AppInsights
    CodeInterpreter -->|Telemetry| AppInsights

    AppInsights --> Metrics
    AppInsights --> Alerts

    Metrics --> Dashboard
    AppInsights --> Logs
    AppInsights --> Traces

    Alerts -->|通知| Email[Email/Teams 通知]

    style AppInsights fill:#00BCD4
    style Dashboard fill:#4CAF50
```

### 關鍵監控指標

```yaml
性能指標:
  - Agent 執行延遲 (P50, P95, P99)
  - Plugin 調用成功率
  - Code Interpreter 執行時間
  - Container Pool 使用率

可用性指標:
  - API 可用性 (目標: 99.9%)
  - Agent 執行成功率
  - 錯誤率 (目標: <1%)

資源指標:
  - CPU 使用率
  - Memory 使用率
  - Redis 連接數
  - PostgreSQL 連接數
  - Container Pool 大小

業務指標:
  - 每日活躍用戶 (DAU)
  - Agent 執行次數
  - Code Interpreter 使用率
  - Plugin 調用次數
```

---

## 災難恢復架構

### 備份策略

```yaml
PostgreSQL 備份:
  頻率: 每日全量備份
  保留期: 30 天
  恢復點目標 (RPO): 24 小時
  恢復時間目標 (RTO): 4 小時

Redis 備份:
  頻率: 每 6 小時 RDB 快照
  保留期: 7 天
  RPO: 6 小時
  RTO: 1 小時

Azure Blob 備份:
  頻率: 連續複製 (Geo-Redundant)
  保留期: 無限期
  RPO: 近乎 0
  RTO: <1 小時
```

### 故障恢復流程

```mermaid
graph TB
    Failure[系統故障] --> Detect[監控檢測]
    Detect --> Notify[通知運維團隊]
    Notify --> Assess[評估影響範圍]

    Assess --> DB{數據庫故障?}
    DB -->|是| RestoreDB[從備份恢復數據庫]
    DB -->|否| Service{服務故障?}

    Service -->|是| RestartService[重啟服務/容器]
    Service -->|否| Infrastructure{基礎設施故障?}

    Infrastructure -->|是| Failover[切換到備用區域]
    Infrastructure -->|否| Manual[人工介入]

    RestoreDB --> Verify[驗證數據完整性]
    RestartService --> Verify
    Failover --> Verify
    Manual --> Verify

    Verify --> Monitor[持續監控]
    Monitor --> Complete[恢復完成]

    style Failure fill:#F44336,color:#fff
    style Complete fill:#4CAF50,color:#fff
```

---

## 總結

本文檔基於 C4 Model 提供了 Semantic Kernel Agentic Framework 的完整架構可視化：

1. **System Context**：展示系統與用戶、外部系統的交互
2. **Container**：展示系統內部容器及其通訊
3. **Component**：展示 Agent Orchestrator 內部組件
4. **部署架構**：Phase 1 (Docker Compose) 和 Phase 2 (Kubernetes)
5. **數據流圖**：單 Agent、Multi-Agent、Code Interpreter 執行流程
6. **安全架構**：4 層安全防護
7. **監控架構**：可觀察性和關鍵指標
8. **災難恢復**：備份策略和故障恢復流程

這些圖表將作為 **Architecture Design Document (ADD)** 的核心部分，指導後續的開發和部署工作。
