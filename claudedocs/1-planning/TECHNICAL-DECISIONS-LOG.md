# 技術決策日誌

**Semantic Kernel Agentic Framework - Technical Decisions Log**

**目的**: 記錄項目開發過程中所有關鍵技術決策，包括決策背景、原因、影響和時間點

**版本**: 2.0
**創建日期**: 2025-11-12
**最後更新**: 2025-11-12

[返回 claudedocs 主頁](../README.md) | [MVP 範圍定義](./MVP-SCOPE-DEFINITION.md) | [架構演進路線圖](./ARCHITECTURE-EVOLUTION-ROADMAP.md)

---

## 📋 決策概覽

| 決策ID | 決策標題 | 決策日期 | 狀態 | 影響範圍 |
|--------|---------|---------|------|---------|
| TD-001 | SK + 自建編排層 (vs Microsoft Agent Framework) | 2025-10-28 | ✅ 已確認 | 架構核心 |
| TD-002 | Framework Abstraction Layer 設計 | 2025-10-28 | ✅ 已確認 | 架構核心 |
| TD-003 | MCP 標準化插件系統 | 2025-10-28 | ✅ 已確認 | Plugin 系統 |
| TD-004 | OpenTelemetry 可觀察性標準 | 2025-10-28 | ✅ 已確認 | 監控系統 |
| TD-005 | Monorepo 倉庫策略 | 2025-11-02 | ✅ 已確認 | 項目結構 |
| TD-006 | React + Material-UI 主應用 | 2025-11-02 | ✅ 已確認 | 前端架構 |
| TD-007 | VueFlow Workflow Editor | 2025-11-02 | ✅ 已確認 | 工作流編輯 |
| TD-008 | YAML Persona 配置系統 | 2025-10-28 | ✅ 已確認 | Persona 系統 |
| TD-009 | Clean Architecture + DDD + CQRS | 2025-10-28 | ✅ 已確認 | 後端架構 |
| TD-010 | Docker Container Pool (Code Interpreter) | 2025-10-28 | ✅ 已確認 | 安全隔離 |
| TD-011 | PostgreSQL + Redis + Qdrant 數據架構 | 2025-10-28 | ✅ 已確認 | 數據層 |
| TD-012 | JWT + RBAC 認證授權 | 2025-10-28 | ✅ 已確認 | 安全系統 |

---

## TD-001: SK + 自建編排層 (vs Microsoft Agent Framework)

### 決策日期
2025-10-28

### 決策者
Product Owner + Tech Lead

### 決策內容
**選擇**: Semantic Kernel 1.x + 自建編排層
**替代方案**: Microsoft Agent Framework (2025-10-01 發布)

### 決策背景

根據 [docs/analysis-microsoft-agent-framework-impact.md](../../docs/analysis-microsoft-agent-framework-impact.md) 的深入分析：

- Microsoft Agent Framework 於 2025-10-01 剛發布（決策時僅發布 4 週）
- 需要在穩定性和新特性之間做出選擇
- MVP 目標是 8-10 個月內交付
- [docs/brief/Project-Brief.md](../../docs/brief/Project-Brief.md) 明確要求「企業級穩定性」

### 決策理由

**為什麼選擇 Semantic Kernel**（參考 [docs/architecture/ADR-011-framework-migration-strategy.md](../../docs/architecture/ADR-011-framework-migration-strategy.md)）:

1. ✅ **穩定性優先**: SK 1.x 已發布超過 1 年，生產環境驗證充分
2. ✅ **風險控制**: Microsoft Agent Framework 太新，穩定性未知
3. ✅ **交付保障**: 使用成熟框架降低 MVP 交付風險
4. ✅ **Microsoft 官方支持**: 長期穩定性和技術支援保證
5. ✅ **完整的 .NET 生態整合**: 與企業技術棧無縫整合

**為什麼需要自建編排層**（參考 [docs/user-stories/modules/module-04-multi-agent-workflow.md](../../docs/user-stories/modules/module-04-multi-agent-workflow.md)）:

- SK 提供優秀的基礎能力，但不是完整的 agent 編排框架
- 需要在 SK 之上構建：Agent 角色、工作流編排、狀態管理、Knowledge 策略
- 自建編排層提供更高的靈活性和控制權
- 支持 7 種編排模式：Sequential、Parallel、Conditional、Loop、GroupConversation、Handoff、Supervisor

### 影響範圍

**架構核心**（參考 [docs/architecture/Architecture-Design-Document.md](../../docs/architecture/Architecture-Design-Document.md) Section 3.1）:
- 所有 Agent 執行邏輯基於 SK
- 開發工作量：需要開發完整的編排引擎
- 技術棧：.NET 9 + Semantic Kernel 1.x
- 未來遷移：通過抽象層保留遷移到 MAF 的可能性（Phase 2 評估）

**US 依賴**（參考 [docs/user-stories/README.md](../../docs/user-stories/README.md)）:
- US 1.4: Agent Execution Engine (直接依賴 SK)
- US 4.x: Multi-Agent Workflow (依賴自建編排層)
- US 7.x: Persona Framework (依賴 SK Prompt Template)

### 相關文檔
- [docs/architecture/ADR-011-framework-migration-strategy.md](../../docs/architecture/ADR-011-framework-migration-strategy.md) - Framework 選擇分析
- [docs/analysis-microsoft-agent-framework-impact.md](../../docs/analysis-microsoft-agent-framework-impact.md) - MAF 影響評估（42 頁詳細分析）
- [docs/architecture/Architecture-Design-Document.md](../../docs/architecture/Architecture-Design-Document.md) - Section 3.1 Framework Selection
- [docs/user-stories/modules/module-04-multi-agent-workflow.md](../../docs/user-stories/modules/module-04-multi-agent-workflow.md) - 編排層需求

---

## TD-002: Framework Abstraction Layer 設計

### 決策日期
2025-10-28

### 決策者
Tech Lead + Architecture Team

### 決策內容
**採用**: Framework Abstraction Layer（5 個核心接口）
**目的**: 隔離 SK 實現細節，保留未來框架遷移能力

### 5 個核心接口

詳細設計參考 [docs/architecture/ADR-011-framework-migration-strategy.md](../../docs/architecture/ADR-011-framework-migration-strategy.md) Section 2。

#### 1. IAgentRuntime
```csharp
public interface IAgentRuntime
{
    Task<AgentResponse> ExecuteAsync(
        AgentRequest request,
        CancellationToken cancellationToken = default);

    Task<AgentState> GetStateAsync(string agentId);

    Task<AgentResponse> ResumeAsync(
        string executionId,
        CancellationToken cancellationToken = default);
}
```

**職責**: Agent 執行抽象，隔離底層框架（SK/MAF）的執行細節
**實現**: SemanticKernelAdapter (Phase 1), MicrosoftAgentFrameworkAdapter (Phase 3 可選)

#### 2. IToolRegistry
```csharp
public interface IToolRegistry
{
    // MCP 標準支持 (參考 TD-003)
    Task RegisterMcpToolAsync(McpToolDefinition tool);
    Task RegisterFromOpenApiAsync(string openApiSpecUrl);

    // Tool 管理
    Task<IEnumerable<ToolDefinition>> GetAvailableToolsAsync();
    Task<ToolResult> InvokeToolAsync(
        string toolName,
        Dictionary<string, object> parameters);
}
```

**職責**: Plugin/Tool 註冊和調用，支持 MCP 標準
**參考**: [docs/user-stories/modules/module-02-plugin-system.md](../../docs/user-stories/modules/module-02-plugin-system.md)

#### 3. IAgentMemory
```csharp
public interface IAgentMemory
{
    Task<string> StoreAsync(string agentId, string content, MemoryMetadata metadata);
    Task<IEnumerable<MemoryChunk>> SearchAsync(string query, int topK = 5);
    Task<bool> DeleteAsync(string memoryId);
}
```

**職責**: Agent 記憶管理抽象（短期/長期記憶）
**參考**: [docs/user-stories/modules/module-08-knowledge-management.md](../../docs/user-stories/modules/module-08-knowledge-management.md) Section 2.2

#### 4. IWorkflowOrchestrator
```csharp
public interface IWorkflowOrchestrator
{
    Task<WorkflowResult> ExecuteAsync(
        WorkflowDefinition workflow,
        OrchestrationMode mode); // Sequential, Concurrent, GroupConversation, Handoff, Supervisor

    Task<WorkflowState> GetWorkflowStateAsync(string workflowId);
    Task PauseWorkflowAsync(string workflowId);
    Task ResumeWorkflowAsync(string workflowId);
}
```

**職責**: 工作流編排抽象，支持多種編排模式
**參考**: [docs/user-stories/modules/module-04-multi-agent-workflow.md](../../docs/user-stories/modules/module-04-multi-agent-workflow.md) Section 3

#### 5. IObservability
```csharp
public interface IObservability
{
    ISpan StartSpan(string operationName, SpanContext? parentContext = null);
    void RecordMetric(string metricName, double value, Dictionary<string, string> tags);
    void LogEvent(string eventName, LogLevel level, Dictionary<string, object> properties);
}
```

**職責**: 可觀察性抽象，基於 OpenTelemetry 標準
**參考**: [docs/user-stories/modules/module-10-monitoring.md](../../docs/user-stories/modules/module-10-monitoring.md) Section 4

### 決策理由

1. ✅ **未來靈活性**: 保留未來遷移到 Microsoft Agent Framework 的可能性
2. ✅ **多框架並存**: 支持 Hybrid 模式（部分功能使用新框架）
3. ✅ **開放標準兼容**: MCP、OpenAPI、OpenTelemetry 是框架無關的標準
4. ✅ **保護業務邏輯**: 業務邏輯不受框架變更影響
5. ✅ **Feature Flag 支持**: 支持灰度切換和 A/B Testing

### 影響範圍

**開發工作量**（參考 [docs/technical-implementation/sprint-planning/sprint-allocation.md](../../docs/technical-implementation/sprint-planning/sprint-allocation.md)）:
- 需要額外開發抽象層和適配器（5-8 SP）
- 性能影響：抽象層可能引入輕微性能損失（<5%）
- 架構複雜度：增加架構層次，但提升長期靈活性
- 測試複雜度：需要測試多個框架適配器

### 相關文檔
- [docs/architecture/ADR-011-framework-migration-strategy.md](../../docs/architecture/ADR-011-framework-migration-strategy.md) - Section 2: Framework Abstraction Layer
- [docs/architecture/Architecture-Design-Document.md](../../docs/architecture/Architecture-Design-Document.md) - Section 3.2: Abstraction Layer Design
- [docs/technical-implementation/backend/core-abstractions.md](../../docs/technical-implementation/backend/core-abstractions.md) - 抽象層實現指南

---

## TD-003: MCP 標準化插件系統

### 決策日期
2025-10-28

### 決策者
Tech Lead + Plugin Team

### 決策內容
**採用**: Model Context Protocol (MCP) 作為插件標準
**目標**: 實現標準化的 Plugin/Tool 定義和調用機制

### MCP 核心特性

參考 [docs/user-stories/modules/module-02-plugin-system.md](../../docs/user-stories/modules/module-02-plugin-system.md) Section 3.2：

- **標準協議**: 行業標準的 Plugin/Tool 協議
- **多 Transport 支持**: HTTP、stdio、SSE
- **框架無關**: SK 和 MAF 都支持
- **生態系統**: 豐富的第三方 Tool 支持

### 實施方式

**IToolRegistry 接口集成**:
```csharp
public interface IToolRegistry
{
    // MCP 標準支持
    Task RegisterMcpToolAsync(McpToolDefinition tool);
    Task RegisterFromOpenApiAsync(string openApiSpecUrl);
}
```

**MCP Tool Definition 範例**:
```json
{
  "name": "web_search",
  "description": "搜索網路資料",
  "version": "1.0.0",
  "transport": "HTTP",
  "endpoint": "https://api.example.com/tools/web_search",
  "inputSchema": { ... },
  "outputSchema": { ... }
}
```

### 決策理由

1. ✅ **開放標準**: 避免供應商鎖定
2. ✅ **跨框架兼容**: SK 和 MAF 都支持
3. ✅ **第三方生態**: 豐富的 Tool 生態系統
4. ✅ **標準化接口**: 統一的 Tool 定義和調用方式

### 影響範圍

**User Stories**（參考 [docs/user-stories/README.md](../../docs/user-stories/README.md) Module 2）:
- **US 2.1**: Plugin 註冊和管理（需要調整為 MCP 標準）
- **US 2.2**: Plugin 版本管理（MCP Versioning 支持）
- **US 2.3**: Plugin 熱重載（MCP Transport 支持）

**開發工作量**（參考 [docs/technical-implementation/backend/plugin-system-implementation.md](../../docs/technical-implementation/backend/plugin-system-implementation.md)）:
- MCP 協議實現和集成（8-10 SP）
- Plugin Registry 重構（5 SP）
- 測試覆蓋（3 SP）

### 相關文檔
- [docs/user-stories/modules/module-02-plugin-system.md](../../docs/user-stories/modules/module-02-plugin-system.md) - Plugin 系統完整設計
- [docs/architecture/ADR-011-framework-migration-strategy.md](../../docs/architecture/ADR-011-framework-migration-strategy.md) - Line 74-81: MCP 集成
- [docs/technical-implementation/backend/plugin-system-implementation.md](../../docs/technical-implementation/backend/plugin-system-implementation.md) - MCP 實現指南
- [docs/api/plugin-api-specification.md](../../docs/api/plugin-api-specification.md) - Plugin API 規範

---

## TD-004: OpenTelemetry 可觀察性標準

### 決策日期
2025-10-28

### 決策者
Tech Lead + DevOps Team

### 決策內容
**採用**: OpenTelemetry 作為可觀察性標準
**目標**: 統一 Metrics、Traces、Logs 收集和監控

### OpenTelemetry 核心組件

參考 [docs/user-stories/modules/module-10-monitoring.md](../../docs/user-stories/modules/module-10-monitoring.md) Section 4：

- **Metrics**: 性能指標（CPU、Memory、API 響應時間）
- **Traces**: 分布式追蹤（跨服務調用鏈）
- **Logs**: 結構化日誌（事件日誌、錯誤日誌）

### 實施方式

**IObservability 接口**:
```csharp
public interface IObservability
{
    ISpan StartSpan(string operationName, SpanContext? parentContext = null);
    void RecordMetric(string metricName, double value, Dictionary<string, string> tags);
    void LogEvent(string eventName, LogLevel level, Dictionary<string, object> properties);
}
```

**配置範例**（參考 [docs/deployment/monitoring-setup.md](../../docs/deployment/monitoring-setup.md)）:
```json
{
  "OpenTelemetry": {
    "Enabled": true,
    "Endpoint": "http://localhost:4317",
    "ServiceName": "semantic-kernel-agent",
    "Exporters": ["Console", "OTLP", "Prometheus"]
  }
}
```

### 決策理由

1. ✅ **事實標準**: 可觀察性領域的行業標準
2. ✅ **雲平台無關**: 避免 Azure/AWS 鎖定
3. ✅ **完整覆蓋**: Metrics + Traces + Logs 統一收集
4. ✅ **生態系統**: 豐富的監控工具支持（Grafana、Prometheus、Jaeger）

### 影響範圍

**User Stories**（參考 [docs/user-stories/README.md](../../docs/user-stories/README.md) Module 10）:
- **US 10.1**: 性能監控（OpenTelemetry Metrics）
- **US 10.2**: 執行追蹤（OpenTelemetry Traces）
- **US 10.3**: 日誌管理（OpenTelemetry Logs）

**基礎設施**（參考 [docs/deployment/infrastructure-setup.md](../../docs/deployment/infrastructure-setup.md)）:
- 需要部署 OpenTelemetry Collector
- Prometheus + Grafana 監控堆棧
- Jaeger 分布式追蹤

### 相關文檔
- [docs/user-stories/modules/module-10-monitoring.md](../../docs/user-stories/modules/module-10-monitoring.md) - 監控系統完整設計
- [docs/architecture/ADR-011-framework-migration-strategy.md](../../docs/architecture/ADR-011-framework-migration-strategy.md) - Line 109-115: Observability Abstraction
- [docs/deployment/monitoring-setup.md](../../docs/deployment/monitoring-setup.md) - 監控部署指南
- [docs/technical-implementation/devops/observability-implementation.md](../../docs/technical-implementation/devops/observability-implementation.md) - OpenTelemetry 實現

---

## TD-005: Monorepo 倉庫策略

### 決策日期
2025-11-02

### 決策者
Product Owner + Tech Lead

### 決策內容
**選擇**: Monorepo（單一倉庫）
**替代方案**: Polyrepo（多倉庫）

### Monorepo 架構

參考 [docs/PROJECT-INITIALIZATION-DECISION.md](../../docs/PROJECT-INITIALIZATION-DECISION.md) Section 3：

```
ai-workflow-platform/
├── apps/
│   ├── web-app/                # React 主應用
│   ├── workflow-editor/        # VueFlow Workflow Editor (Phase 2)
│   ├── api-gateway/            # ASP.NET Core API Gateway
│   ├── agent-service/          # Semantic Kernel Agent Service
│   └── ...
├── packages/
│   ├── shared-types/           # TypeScript 類型定義（前後端共享）
│   ├── ui-components/          # React 共享組件庫
│   ├── sdk-dotnet/             # .NET SDK
│   └── sdk-typescript/         # TypeScript SDK
├── docs/                       # 所有規劃文檔
└── claudedocs/                 # AI 助手執行指南
```

### 工具鏈

參考 [docs/technical-implementation/frontend/build-system.md](../../docs/technical-implementation/frontend/build-system.md)：

- **Package Manager**: pnpm（速度快，節省磁碟空間）
- **Build System**: Turborepo（智能 caching，並行構建）
- **CI/CD**: GitHub Actions
- **Linting**: ESLint + Prettier（統一代碼風格）
- **Type Checking**: TypeScript（嚴格模式）

### 決策理由

1. ✅ **代碼共享簡單**: 前後端類型定義同步更新
2. ✅ **原子性提交**: 前後端 API 變更在同一 commit
3. ✅ **統一 CI/CD**: 單一 Pipeline 構建所有服務
4. ✅ **重構友好**: 跨服務重構容易
5. ✅ **新人 Onboarding**: Clone 一次即可開發

### 影響範圍

**項目結構**（參考 [docs/technical-implementation/README.md](../../docs/technical-implementation/README.md)）:
- 所有代碼在單一倉庫
- 開發體驗：統一的開發環境和工具鏈
- CI/CD：Turborepo 智能 caching 加速構建

**開發工具**（參考 [docs/development-standards/coding-standards.md](../../docs/development-standards/coding-standards.md)）:
- ESLint 配置共享
- Prettier 配置共享
- TypeScript 配置共享

### 相關文檔
- [docs/PROJECT-INITIALIZATION-DECISION.md](../../docs/PROJECT-INITIALIZATION-DECISION.md) - Line 29-183: Monorepo 決策分析
- [docs/technical-implementation/README.md](../../docs/technical-implementation/README.md) - 項目結構說明
- [docs/technical-implementation/frontend/build-system.md](../../docs/technical-implementation/frontend/build-system.md) - 構建系統設計
- [docs/development-standards/coding-standards.md](../../docs/development-standards/coding-standards.md) - 代碼標準

---

## TD-006: React + Material-UI 主應用

### 決策日期
2025-11-02

### 決策者
Tech Lead + Frontend Team

### 決策內容
**主應用**: React 18 + TypeScript + Material-UI (MUI) v5
**理由**: 企業級 UI 組件庫，完整的設計系統支持

### 技術選型

參考 [docs/PROJECT-INITIALIZATION-DECISION.md](../../docs/PROJECT-INITIALIZATION-DECISION.md) Section 4 和 [docs/ux-design/design-system/README.md](../../docs/ux-design/design-system/README.md)：

```yaml
UI 框架: React 18
語言: TypeScript 5
UI 組件庫: Material-UI (MUI) v5
設計系統: Material Design 3
狀態管理: Zustand（輕量）
路由: React Router v6
表單管理: React Hook Form + Zod
API 客戶端: TanStack Query (React Query)
構建工具: Vite
樣式方案: MUI sx prop + CSS-in-JS
```

### Material-UI 選擇理由

參考 [docs/ux-design/design-system/component-library.md](../../docs/ux-design/design-system/component-library.md)：

1. ✅ **企業級成熟度**: 7+ 年生產驗證，超過 100 萬週下載
2. ✅ **完整的組件庫**: 50+ 基礎組件，覆蓋所有 UI 需求
3. ✅ **Material Design 3**: 遵循 Google 最新設計規範
4. ✅ **無障礙性支持**: 完整的 WCAG 2.1 AA 支持
5. ✅ **TypeScript 支持**: 原生 TypeScript，類型安全
6. ✅ **主題系統**: 強大的主題定制能力（亮色/暗色模式）
7. ✅ **中文社區**: 豐富的中文文檔和社區支持

### UI 組件分類

**基礎組件**（參考 [docs/ux-design/design-system/component-library.md](../../docs/ux-design/design-system/component-library.md) Section 3）:
- Inputs: TextField, Select, Checkbox, Radio, Switch, Slider
- Buttons: Button, IconButton, Fab, ButtonGroup
- Navigation: AppBar, Drawer, Menu, Tabs, Breadcrumbs
- Data Display: Table, List, Card, Chip, Avatar, Badge
- Feedback: Alert, Dialog, Snackbar, Progress, Skeleton

**業務組件**（參考 [docs/ux-design/design-system/component-library.md](../../docs/ux-design/design-system/component-library.md) Section 6）:
- AgentCard: Agent 卡片展示
- ConversationPanel: 對話面板
- ExecutionLog: 執行日誌
- MetricCard: 指標卡片
- WorkflowNode: 工作流節點（VueFlow 集成）

### 影響範圍

**前端開發**（參考 [docs/technical-implementation/frontend/README.md](../../docs/technical-implementation/frontend/README.md)）:
- 所有 UI 組件基於 Material-UI
- Design System 基於 Material Design 3
- 開發效率：使用現成組件，減少自定義開發

**UX 設計**（參考 [docs/ux-design/README.md](../../docs/ux-design/README.md)）:
- 12 個核心頁面全部基於 MUI 設計
- Wireframes 基於 MUI 組件規範
- Design Tokens 映射到 MUI Theme

### 相關文檔
- [docs/PROJECT-INITIALIZATION-DECISION.md](../../docs/PROJECT-INITIALIZATION-DECISION.md) - Line 186-308: 前端技術選型
- [docs/ux-design/design-system/README.md](../../docs/ux-design/design-system/README.md) - Design System 概述
- [docs/ux-design/design-system/component-library.md](../../docs/ux-design/design-system/component-library.md) - 完整組件庫文檔（1,100 行）
- [docs/ux-design/design-system/design-tokens.md](../../docs/ux-design/design-system/design-tokens.md) - MUI Theme 配置
- [docs/technical-implementation/frontend/README.md](../../docs/technical-implementation/frontend/README.md) - 前端實現指南
- [docs/ux-design/README.md](../../docs/ux-design/README.md) - UX 設計文檔結構

---

## TD-007: VueFlow Workflow Editor

### 決策日期
2025-11-02

### 決策者
Tech Lead + Frontend Team

### 決策內容
**Workflow Editor**: Vue 3 + VueFlow
**理由**: PoC 6 驗證可行性（98.2% 質量評分），n8n 風格大卡片節點系統

### 技術選型

參考 [docs/PROJECT-INITIALIZATION-DECISION.md](../../docs/PROJECT-INITIALIZATION-DECISION.md) Section 5 和 [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md)：

```yaml
UI 框架: Vue 3（Composition API）
語言: TypeScript 5
Flow Library: VueFlow
協作: Yjs（CRDT，Phase 2）
狀態管理: Pinia
構建工具: Vite
樣式方案: Tailwind CSS
```

### VueFlow 選擇理由

參考 [poc-projects/poc6-vueflow-crdt/](../../poc-projects/poc6-vueflow-crdt/) 和 [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part1.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part1.md)：

1. ✅ **PoC 驗證完成**: PoC 6 質量評分 98.2%（A+ 級別）
2. ✅ **n8n 風格支持**: 大卡片節點系統，內聯配置
3. ✅ **性能優越**: Canvas 渲染優化，支持 100+ 節點
4. ✅ **CRDT 集成**: Yjs 無縫集成，支持多用戶協作（Phase 2）
5. ✅ **TypeScript 原生**: 完整的類型支持
6. ✅ **自定義能力**: 高度可定制的節點和連接線

### 工作流編輯器設計

**核心功能**（參考 [docs/user-stories/modules/module-04-multi-agent-workflow.md](../../docs/user-stories/modules/module-04-multi-agent-workflow.md) Section 5）:

- **節點類型**（MVP: 10 種，Phase 4: 28 種）:
  - Agent 節點：LLM Agent, Tool Agent, Knowledge Agent
  - 控制節點：If/Else, Loop, Switch, Merge
  - 資料節點：Input, Output, Transform
  - 特殊節點：Trigger, End

- **內聯配置**（參考 [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part2.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part2.md)）:
  - n8n 風格表單編輯
  - 參數表達式系統（`{{ $input.data }}`）
  - 實時驗證與預覽

- **執行視覺化**（參考 [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part3.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part3.md)）:
  - 節點執行狀態（執行中、成功、失敗、跳過）
  - 執行時間與 Token 消耗
  - 輸入/輸出數據預覽

### 與 React 主應用整合

參考 [docs/technical-implementation/frontend/micro-frontend-integration.md](../../docs/technical-implementation/frontend/micro-frontend-integration.md)：

- **獨立應用**: React 和 Vue 各自獨立運行
- **iframe 嵌入**: React 主應用通過 iframe 嵌入 Vue Workflow Editor
- **postMessage API**: 跨應用通訊
- **共享狀態**: 通過 API Gateway 同步

### 影響範圍

**US 依賴**（參考 [docs/user-stories/README.md](../../docs/user-stories/README.md) Module 4）:
- **US 4.3**: Workflow Editor Backend API（15-20 SP）
- **US 4.4**: Workflow Editor Frontend（20-25 SP）
- **US 4.5**: Workflow Template Management（5 SP）

**開發工作量**:
- Phase 1（MVP）：40-50 SP（10 種節點，基礎編輯器）
- Phase 2：+30 SP（CRDT 協作，版本控制）
- Phase 4：+40 SP（28 種節點，完整功能）

### 相關文檔
- [docs/PROJECT-INITIALIZATION-DECISION.md](../../docs/PROJECT-INITIALIZATION-DECISION.md) - Line 186-308: Workflow Editor 決策
- [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md) - 完整 V2 設計索引
- [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part1.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part1.md) - 核心設計理念
- [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part4.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part4.md) - 28 種節點類型
- [docs/user-stories/modules/module-04-multi-agent-workflow.md](../../docs/user-stories/modules/module-04-multi-agent-workflow.md) - Workflow 系統需求
- [poc-projects/poc6-vueflow-crdt/](../../poc-projects/poc6-vueflow-crdt/) - PoC 6 驗證報告
- [docs/technical-implementation/frontend/micro-frontend-integration.md](../../docs/technical-implementation/frontend/micro-frontend-integration.md) - 微前端整合

---

## TD-008: YAML Persona 配置系統

### 決策日期
2025-10-28

### 決策者
Tech Lead + Product Team

### 決策內容
**採用**: YAML 格式的 Persona 配置文件
**目標**: 簡化 Persona 定義，提升可維護性

### YAML Persona 範例

參考 [docs/user-stories/modules/module-07-persona-framework.md](../../docs/user-stories/modules/module-07-persona-framework.md) Section 4：

```yaml
persona:
  id: "customer-service-agent"
  name: "客服專員"
  description: "處理客戶諮詢和問題解決"

  model:
    provider: "azure-openai"
    deployment: "gpt-4"
    temperature: 0.7
    max_tokens: 2000

  system_prompt: |
    你是一位專業的客服專員。
    你的職責是解答客戶問題，提供準確的資訊。

  tools:
    - name: "knowledge_search"
      description: "搜索知識庫"
    - name: "order_lookup"
      description: "查詢訂單狀態"

  constraints:
    - "不提供未經驗證的資訊"
    - "保持專業和禮貌"
    - "不透露公司機密"

  few_shot_examples:
    - input: "我的訂單什麼時候到貨？"
      output: "請提供您的訂單編號，我將為您查詢物流狀態。"
```

### 決策理由

1. ✅ **可讀性高**: YAML 格式比 JSON 更適合人類閱讀和編輯
2. ✅ **易於維護**: Persona 配置與代碼分離
3. ✅ **版本控制友好**: YAML 文件易於 diff 和追蹤變更
4. ✅ **快速迭代**: 非技術人員也可以調整 Persona 配置
5. ✅ **模板化支持**: 支持 Persona 模板庫（US 8.2）

### 影響範圍

**User Stories**（參考 [docs/user-stories/modules/module-07-persona-framework.md](../../docs/user-stories/modules/module-07-persona-framework.md)）:
- **US 8.1**: Persona 創建與管理（YAML 解析和驗證）
- **US 8.2**: Persona 模板庫（預定義 YAML 模板）
- **US 8.3**: Persona Few-Shot Learning（YAML 範例配置）
- **US 8.4**: Persona 測試工具（YAML 配置測試）

**技術實現**（參考 [docs/technical-implementation/backend/persona-system-implementation.md](../../docs/technical-implementation/backend/persona-system-implementation.md)）:
- YAML 解析：使用 YamlDotNet 庫
- Schema 驗證：JSON Schema 驗證 YAML 結構
- 熱重載：監聽 YAML 文件變更，動態重載

### 相關文檔
- [docs/user-stories/modules/module-07-persona-framework.md](../../docs/user-stories/modules/module-07-persona-framework.md) - Persona 系統完整設計
- [docs/technical-implementation/backend/persona-system-implementation.md](../../docs/technical-implementation/backend/persona-system-implementation.md) - Persona 實現指南
- [docs/ux-design/wireframes/low-fidelity/10-persona-builder.md](../../docs/ux-design/wireframes/low-fidelity/10-persona-builder.md) - Persona Builder UI 設計

---

## TD-009: Clean Architecture + DDD + CQRS

### 決策日期
2025-10-28

### 決策者
Tech Lead + Architecture Team

### 決策內容
**採用**: Clean Architecture + Domain-Driven Design + CQRS 模式
**目標**: 構建可維護、可測試、可擴展的後端架構

### 架構分層

參考 [docs/architecture/Architecture-Design-Document.md](../../docs/architecture/Architecture-Design-Document.md) Section 4：

```
┌─────────────────────────────────────────────┐
│  Presentation Layer (API)                   │
│  - Controllers, Middleware, DTOs            │
├─────────────────────────────────────────────┤
│  Application Layer                          │
│  - CQRS (Commands/Queries via MediatR)     │
│  - Use Cases, DTOs, Interfaces             │
├─────────────────────────────────────────────┤
│  Domain Layer                               │
│  - Entities, Value Objects, Domain Events  │
│  - Business Rules, Domain Services         │
├─────────────────────────────────────────────┤
│  Infrastructure Layer                       │
│  - EF Core, Repositories, External APIs    │
│  - Redis, Qdrant, Email Services           │
├─────────────────────────────────────────────┤
│  Shared Layer                               │
│  - Common utilities, Constants, Extensions │
└─────────────────────────────────────────────┘
```

### CQRS 模式

參考 [docs/development-standards/cqrs-patterns.md](../../docs/development-standards/cqrs-patterns.md)：

**Command 範例**:
```csharp
// Command (寫入操作)
public record CreateAgentCommand(
    string Name,
    string Description,
    string SystemPrompt) : IRequest<Result<AgentDto>>;

// Command Handler
public class CreateAgentCommandHandler : IRequestHandler<CreateAgentCommand, Result<AgentDto>>
{
    private readonly IAgentRepository _repository;

    public async Task<Result<AgentDto>> Handle(CreateAgentCommand request, CancellationToken cancellationToken)
    {
        var agent = Agent.Create(request.Name, request.Description, request.SystemPrompt);
        await _repository.AddAsync(agent, cancellationToken);
        return Result.Success(agent.ToDto());
    }
}
```

**Query 範例**:
```csharp
// Query (讀取操作)
public record GetAgentByIdQuery(Guid AgentId) : IRequest<Result<AgentDto>>;

// Query Handler
public class GetAgentByIdQueryHandler : IRequestHandler<GetAgentByIdQuery, Result<AgentDto>>
{
    private readonly IAgentRepository _repository;

    public async Task<Result<AgentDto>> Handle(GetAgentByIdQuery request, CancellationToken cancellationToken)
    {
        var agent = await _repository.GetByIdAsync(request.AgentId, cancellationToken);
        return agent != null ? Result.Success(agent.ToDto()) : Result.Failure<AgentDto>("Agent not found");
    }
}
```

### Domain-Driven Design

**核心概念**（參考 [docs/development-standards/ddd-patterns.md](../../docs/development-standards/ddd-patterns.md)）:

- **Entities**: 具有唯一標識的領域對象（Agent, Conversation, Plugin）
- **Value Objects**: 無標識的值對象（Email, PersonaConfig, WorkflowDefinition）
- **Aggregates**: 聚合根（Agent 聚合，Conversation 聚合）
- **Domain Events**: 領域事件（AgentCreated, ConversationStarted）
- **Repositories**: 倉儲模式（IAgentRepository, IConversationRepository）

### 決策理由

1. ✅ **關注點分離**: 各層職責清晰，易於維護
2. ✅ **可測試性**: 業務邏輯獨立於基礎設施，易於單元測試
3. ✅ **可擴展性**: 新功能不影響現有代碼
4. ✅ **企業級成熟度**: 業界驗證的架構模式
5. ✅ **團隊協作**: 分層設計便於並行開發

### 影響範圍

**項目結構**（參考 [docs/technical-implementation/backend/README.md](../../docs/technical-implementation/backend/README.md)）:
```
src/
├── AIAgentPlatform.API/              # Presentation Layer
├── AIAgentPlatform.Application/      # Application Layer (CQRS)
├── AIAgentPlatform.Domain/           # Domain Layer (Entities, VOs)
├── AIAgentPlatform.Infrastructure/   # Infrastructure Layer (EF Core, Redis)
├── AIAgentPlatform.Shared/           # Shared utilities
```

**開發工作量**:
- 初始架構搭建：8-10 SP
- 每個 US 的 CQRS 實現：+2-3 SP（相比簡單架構）
- 長期收益：維護成本降低 30-40%

### 相關文檔
- [docs/architecture/Architecture-Design-Document.md](../../docs/architecture/Architecture-Design-Document.md) - Section 4: 架構分層詳細設計
- [docs/development-standards/cqrs-patterns.md](../../docs/development-standards/cqrs-patterns.md) - CQRS 模式指南
- [docs/development-standards/ddd-patterns.md](../../docs/development-standards/ddd-patterns.md) - DDD 模式指南
- [docs/technical-implementation/backend/README.md](../../docs/technical-implementation/backend/README.md) - 後端實現指南
- [docs/development-standards/clean-architecture-guidelines.md](../../docs/development-standards/clean-architecture-guidelines.md) - Clean Architecture 指南

---

## TD-010: Docker Container Pool (Code Interpreter)

### 決策日期
2025-10-28

### 決策者
Tech Lead + Security Team

### 決策內容
**採用**: Docker Container Pool 隔離執行用戶代碼
**目標**: 4 層安全防護 + 動態生命週期管理

### 4 層安全防護

參考 [docs/architecture/Architecture-Design-Document.md](../../docs/architecture/Architecture-Design-Document.md) Section 7.3 和 [docs/security/code-execution-security.md](../../docs/security/code-execution-security.md)：

**Layer 1: Container 隔離**
- 每個代碼執行任務在獨立 Docker Container 中運行
- Container 間完全隔離，無法互相訪問
- 使用 Docker Security Profile（AppArmor/SELinux）

**Layer 2: Resource Limits**
```yaml
# docker-compose.yml
code-interpreter:
  image: code-interpreter:latest
  deploy:
    resources:
      limits:
        cpus: '0.5'        # 限制 CPU 使用
        memory: 512M        # 限制記憶體
        pids: 100           # 限制進程數
```

**Layer 3: Network Isolation**
- Container 預設無網路訪問（`network_mode: "none"`）
- 如需網路訪問，使用白名單機制（只允許特定 API endpoint）
- 禁止訪問內部網路和雲端 metadata API

**Layer 4: Execution Timeout**
- 代碼執行時間限制（預設 30 秒，最大 300 秒）
- 超時自動終止 Container
- 防止無限循環和資源耗盡攻擊

### Container Pool 管理

參考 [docs/technical-implementation/backend/code-interpreter-implementation.md](../../docs/technical-implementation/backend/code-interpreter-implementation.md)：

**動態生命週期管理**:
```csharp
public class ContainerPool
{
    // Pool 配置
    private readonly int _minPoolSize = 3;      // 最小 Container 數量
    private readonly int _maxPoolSize = 10;     // 最大 Container 數量
    private readonly int _idleTimeout = 300;    // 閒置 Container 超時（秒）

    // Container 狀態
    public enum ContainerState
    {
        Ready,      // 就緒（可立即執行）
        Running,    // 執行中
        Idle,       // 閒置（等待回收）
        Recycling   // 回收中
    }

    // 獲取可用 Container
    public async Task<Container> AcquireContainerAsync()
    {
        // 1. 嘗試從 Pool 獲取 Ready 狀態的 Container
        // 2. 如果 Pool 為空且未達到 maxPoolSize，創建新 Container
        // 3. 如果達到 maxPoolSize，等待或拒絕請求
    }

    // 歸還 Container
    public async Task ReleaseContainerAsync(Container container)
    {
        // 1. 清理 Container 狀態（刪除臨時文件、重置環境）
        // 2. 如果 Pool 未滿，將 Container 標記為 Ready
        // 3. 如果 Pool 已滿，銷毀 Container
    }
}
```

### 支持的語言環境

參考 [docs/user-stories/modules/module-09-code-interpreter.md](../../docs/user-stories/modules/module-09-code-interpreter.md) Section 3：

- **Python 3.11**: NumPy, Pandas, Matplotlib, Scikit-learn
- **JavaScript (Node.js 20)**: 基礎 npm 包支持
- **R 4.3**: 統計分析和可視化
- **Phase 2 擴展**: Java, Go, C#

### 決策理由

1. ✅ **安全性優先**: 4 層防護確保用戶代碼不會危害系統
2. ✅ **性能優化**: Container Pool 減少冷啟動時間（<1 秒 vs 5-10 秒）
3. ✅ **資源控制**: Resource Limits 防止惡意代碼耗盡資源
4. ✅ **可擴展性**: Pool 管理支持動態擴縮容

### 影響範圍

**User Stories**（參考 [docs/user-stories/modules/module-09-code-interpreter.md](../../docs/user-stories/modules/module-09-code-interpreter.md)）:
- **US 5.1**: Code Execution Engine（Container Pool 實現）
- **US 5.2**: Multi-Language Support（多語言 Docker Image）
- **US 5.3**: Sandbox Security（4 層安全防護）
- **US 5.4**: Result Visualization（執行結果處理）

**基礎設施**（參考 [docs/deployment/docker-setup.md](../../docs/deployment/docker-setup.md)）:
- Docker Engine 20.10+
- Container Registry（Harbor 或 Docker Hub）
- 監控：Container 資源使用監控

### 相關文檔
- [docs/architecture/Architecture-Design-Document.md](../../docs/architecture/Architecture-Design-Document.md) - Section 7.3: Code Interpreter Security
- [docs/security/code-execution-security.md](../../docs/security/code-execution-security.md) - 完整安全設計
- [docs/user-stories/modules/module-09-code-interpreter.md](../../docs/user-stories/modules/module-09-code-interpreter.md) - Code Interpreter 需求
- [docs/technical-implementation/backend/code-interpreter-implementation.md](../../docs/technical-implementation/backend/code-interpreter-implementation.md) - 實現指南
- [docs/deployment/docker-setup.md](../../docs/deployment/docker-setup.md) - Docker 部署指南

---

## TD-011: PostgreSQL + Redis + Qdrant 數據架構

### 決策日期
2025-10-28

### 決策者
Tech Lead + DevOps Team

### 決策內容
**採用**: 混合數據架構（PostgreSQL + Redis + Qdrant）
**目標**: 關係型數據 + 快取 + 向量搜索

### 數據架構設計

參考 [docs/architecture/Architecture-Design-Document.md](../../docs/architecture/Architecture-Design-Document.md) Section 6 和 [docs/architecture/ADR-006-hybrid-state-management.md](../../docs/architecture/ADR-006-hybrid-state-management.md)：

```yaml
# PostgreSQL 16 - 主數據存儲
職責:
  - Agent、Conversation、Plugin、User 等實體數據
  - 關係型數據（外鍵、事務支持）
  - 審計日誌（Audit Logs）
配置:
  - Connection Pool: 20-50 連接
  - EF Core 9 ORM
  - Code-First Migrations

# Redis 7 - 快取與會話
職責:
  - Agent 狀態快取（減少資料庫查詢）
  - Conversation 會話數據（臨時數據）
  - Rate Limiting（API 速率限制）
  - Distributed Lock（分佈式鎖）
配置:
  - Redis Cluster（Phase 4）
  - 資料過期策略（TTL）
  - LRU 淘汰策略

# Qdrant 1.7.4 - 向量數據庫
職責:
  - Knowledge Base 向量存儲
  - Semantic Search（語義搜索）
  - Agent Memory（長期記憶）
配置:
  - Collection per Knowledge Base
  - HNSW Index（快速搜索）
  - Qdrant Cluster（Phase 4）
```

### 數據流設計

參考 [docs/architecture/ADR-006-hybrid-state-management.md](../../docs/architecture/ADR-006-hybrid-state-management.md) Section 3：

**寫入流程（Write-Through Cache）**:
```
User Request
    ↓
1. Write to PostgreSQL (持久化)
    ↓
2. Update Redis Cache (快取同步)
    ↓
3. Return Response
```

**讀取流程（Cache-Aside Pattern）**:
```
User Request
    ↓
1. Check Redis Cache
    ↓
   Hit? → Return from Cache
    ↓
   Miss?
    ↓
2. Query PostgreSQL
    ↓
3. Write to Redis Cache
    ↓
4. Return Response
```

### 決策理由

1. ✅ **性能優化**: Redis 快取減少資料庫查詢（50-70% 查詢命中快取）
2. ✅ **可擴展性**: PostgreSQL + Redis 支持讀寫分離和分片（Phase 4）
3. ✅ **功能完整**: Qdrant 提供企業級向量搜索能力
4. ✅ **成本控制**: 開源方案，無供應商鎖定

### 影響範圍

**User Stories**（參考 [docs/user-stories/README.md](../../docs/user-stories/README.md)）:
- **US 1.x**: Agent 管理（PostgreSQL + Redis）
- **US 3.x**: Conversation 管理（Redis 會話，PostgreSQL 歷史）
- **US 4.x**: Knowledge Base（Qdrant 向量存儲）
- **US 10.x**: Monitoring（Redis 計數器，PostgreSQL 日誌）

**數據庫設計**（參考 [docs/api/database-schema.md](../../docs/api/database-schema.md)）:
- 14 個主要資料表（Agents, Conversations, Plugins, Users, etc.）
- EF Core Migrations 管理 Schema 變更
- Seed Data 提供初始數據

### 相關文檔
- [docs/architecture/Architecture-Design-Document.md](../../docs/architecture/Architecture-Design-Document.md) - Section 6: 數據架構設計
- [docs/architecture/ADR-006-hybrid-state-management.md](../../docs/architecture/ADR-006-hybrid-state-management.md) - 混合狀態管理決策
- [docs/api/database-schema.md](../../docs/api/database-schema.md) - 完整資料庫 Schema
- [docs/deployment/database-setup.md](../../docs/deployment/database-setup.md) - 資料庫部署指南
- [docs/technical-implementation/backend/data-access-layer.md](../../docs/technical-implementation/backend/data-access-layer.md) - 數據訪問層實現

---

## TD-012: JWT + RBAC 認證授權

### 決策日期
2025-10-28

### 決策者
Tech Lead + Security Team

### 決策內容
**採用**: JWT (JSON Web Token) + RBAC (Role-Based Access Control)
**目標**: 無狀態認證 + 細粒度授權

### JWT 認證設計

參考 [docs/architecture/Architecture-Design-Document.md](../../docs/architecture/Architecture-Design-Document.md) Section 7.1 和 [docs/security/authentication-authorization.md](../../docs/security/authentication-authorization.md)：

**JWT Token 結構**:
```json
{
  "header": {
    "alg": "RS256",
    "typ": "JWT"
  },
  "payload": {
    "sub": "user-id-123",
    "email": "user@example.com",
    "roles": ["Developer", "Admin"],
    "permissions": ["agent:read", "agent:write", "workflow:execute"],
    "iat": 1699000000,
    "exp": 1699086400
  }
}
```

**Token 管理**:
- **Access Token**: 短期有效（1 小時），包含用戶資訊和權限
- **Refresh Token**: 長期有效（7 天），用於刷新 Access Token
- **Token 存儲**: Access Token 存儲在記憶體，Refresh Token 存儲在 HttpOnly Cookie
- **Token 撤銷**: Redis 黑名單機制（logout 時加入黑名單）

### RBAC 授權設計

參考 [docs/user-stories/modules/module-11-user-authentication.md](../../docs/user-stories/modules/module-11-user-authentication.md) Section 3：

**4 個核心角色**:
```yaml
Admin (管理員):
  permissions:
    - user:*           # 用戶管理（CRUD）
    - agent:*          # Agent 管理（全部權限）
    - plugin:*         # Plugin 管理（全部權限）
    - workflow:*       # Workflow 管理（全部權限）
    - system:config    # 系統配置
    - audit:read       # 審計日誌查看

Developer (開發者):
  permissions:
    - agent:*          # Agent 管理（CRUD）
    - plugin:read      # Plugin 查看（不能修改）
    - plugin:use       # Plugin 使用
    - workflow:*       # Workflow 管理（CRUD）
    - conversation:*   # Conversation 管理（CRUD）

Analyst (分析師):
  permissions:
    - agent:read       # Agent 查看（只讀）
    - agent:execute    # Agent 執行（Text-to-SQL, Code Interpreter）
    - workflow:read    # Workflow 查看（只讀）
    - workflow:execute # Workflow 執行
    - conversation:*   # Conversation 管理（CRUD）

Viewer (查看者):
  permissions:
    - agent:read       # Agent 查看（只讀）
    - workflow:read    # Workflow 查看（只讀）
    - conversation:read # Conversation 查看（只讀）
```

**授權檢查實現**:
```csharp
// ASP.NET Core Authorization Policy
services.AddAuthorization(options =>
{
    options.AddPolicy("AgentWrite", policy =>
        policy.RequirePermission("agent:write"));

    options.AddPolicy("WorkflowExecute", policy =>
        policy.RequirePermission("workflow:execute"));
});

// Controller 使用
[Authorize(Policy = "AgentWrite")]
[HttpPost("agents")]
public async Task<IActionResult> CreateAgent([FromBody] CreateAgentRequest request)
{
    // ...
}
```

### 決策理由

1. ✅ **無狀態**: JWT 無需伺服器端會話存儲，易於水平擴展
2. ✅ **細粒度控制**: RBAC 提供精確的權限控制
3. ✅ **標準化**: JWT 和 RBAC 是業界標準
4. ✅ **安全性**: RS256 簽名算法，防止 Token 偽造

### 影響範圍

**User Stories**（參考 [docs/user-stories/modules/module-11-user-authentication.md](../../docs/user-stories/modules/module-11-user-authentication.md)）:
- **US 9.1**: User Registration & Login（JWT 認證）
- **US 9.2**: RBAC Authorization（4 角色權限）
- **US 9.3**: Token Management（Token 刷新和撤銷）
- **US 9.4**: Password Management（密碼重置）

**API 安全**（參考 [docs/api/api-security.md](../../docs/api/api-security.md)）:
- 所有 API 端點需要 JWT 認證（除了 `/auth/login` 和 `/auth/register`）
- 敏感操作需要額外權限檢查（Agent 刪除、系統配置）
- Rate Limiting（每個 Token 限制請求速率）

### 相關文檔
- [docs/architecture/Architecture-Design-Document.md](../../docs/architecture/Architecture-Design-Document.md) - Section 7.1: 認證授權設計
- [docs/security/authentication-authorization.md](../../docs/security/authentication-authorization.md) - 完整認證授權設計
- [docs/user-stories/modules/module-11-user-authentication.md](../../docs/user-stories/modules/module-11-user-authentication.md) - 用戶認證需求
- [docs/api/api-security.md](../../docs/api/api-security.md) - API 安全規範
- [docs/technical-implementation/backend/authentication-implementation.md](../../docs/technical-implementation/backend/authentication-implementation.md) - 認證實現指南

---

## 🔄 決策變更歷史

### 2025-11-12 (Version 2.0)
- **升級 TD-006**: 明確主應用使用 Material-UI (MUI) v5
- **新增 TD-007**: VueFlow Workflow Editor 決策
- **新增 TD-009**: Clean Architecture + DDD + CQRS 決策
- **新增 TD-010**: Docker Container Pool 決策
- **新增 TD-011**: PostgreSQL + Redis + Qdrant 數據架構決策
- **新增 TD-012**: JWT + RBAC 認證授權決策
- **文檔增強**: 所有決策增加 `/docs` 引用（總計 47 個引用）

### 2025-11-02 (Version 1.0)
- **新增 TD-005**: Monorepo 倉庫策略
- **新增 TD-006**: React + Vue 混合前端方案
- **理由**: 項目初始化決策

### 2025-10-28 (Version 1.0)
- **新增 TD-001**: SK + 自建編排層
- **新增 TD-002**: Framework Abstraction Layer 設計
- **新增 TD-003**: MCP 標準化插件系統
- **新增 TD-004**: OpenTelemetry 可觀察性標準
- **新增 TD-008**: YAML Persona 配置系統
- **理由**: Microsoft Agent Framework 影響分析完成

---

## 📚 參考文檔

### 1. Core Architecture & Design (10 個引用)
- [docs/architecture/Architecture-Design-Document.md](../../docs/architecture/Architecture-Design-Document.md) - 完整架構設計文檔
- [docs/architecture/ADR-006-hybrid-state-management.md](../../docs/architecture/ADR-006-hybrid-state-management.md) - 混合狀態管理決策
- [docs/architecture/ADR-011-framework-migration-strategy.md](../../docs/architecture/ADR-011-framework-migration-strategy.md) - Framework 遷移策略
- [docs/brief/Project-Brief.md](../../docs/brief/Project-Brief.md) - 項目簡介
- [docs/PROJECT-INITIALIZATION-DECISION.md](../../docs/PROJECT-INITIALIZATION-DECISION.md) - 項目初始化決策
- [docs/analysis-microsoft-agent-framework-impact.md](../../docs/analysis-microsoft-agent-framework-impact.md) - MAF 影響評估

### 2. User Stories (8 個引用)
- [docs/user-stories/README.md](../../docs/user-stories/README.md) - User Stories 索引
- [docs/user-stories/modules/module-02-plugin-system.md](../../docs/user-stories/modules/module-02-plugin-system.md) - Plugin 系統
- [docs/user-stories/modules/module-04-multi-agent-workflow.md](../../docs/user-stories/modules/module-04-multi-agent-workflow.md) - Multi-Agent Workflow
- [docs/user-stories/modules/module-07-persona-framework.md](../../docs/user-stories/modules/module-07-persona-framework.md) - Persona Framework
- [docs/user-stories/modules/module-08-knowledge-management.md](../../docs/user-stories/modules/module-08-knowledge-management.md) - Knowledge Management
- [docs/user-stories/modules/module-09-code-interpreter.md](../../docs/user-stories/modules/module-09-code-interpreter.md) - Code Interpreter
- [docs/user-stories/modules/module-10-monitoring.md](../../docs/user-stories/modules/module-10-monitoring.md) - Monitoring
- [docs/user-stories/modules/module-11-user-authentication.md](../../docs/user-stories/modules/module-11-user-authentication.md) - User Authentication

### 3. Technical Implementation (10 個引用)
- [docs/technical-implementation/README.md](../../docs/technical-implementation/README.md) - 技術實現指南
- [docs/technical-implementation/backend/README.md](../../docs/technical-implementation/backend/README.md) - 後端實現
- [docs/technical-implementation/backend/core-abstractions.md](../../docs/technical-implementation/backend/core-abstractions.md) - 核心抽象層
- [docs/technical-implementation/backend/plugin-system-implementation.md](../../docs/technical-implementation/backend/plugin-system-implementation.md) - Plugin 實現
- [docs/technical-implementation/backend/persona-system-implementation.md](../../docs/technical-implementation/backend/persona-system-implementation.md) - Persona 實現
- [docs/technical-implementation/backend/code-interpreter-implementation.md](../../docs/technical-implementation/backend/code-interpreter-implementation.md) - Code Interpreter 實現
- [docs/technical-implementation/backend/data-access-layer.md](../../docs/technical-implementation/backend/data-access-layer.md) - 數據訪問層
- [docs/technical-implementation/backend/authentication-implementation.md](../../docs/technical-implementation/backend/authentication-implementation.md) - 認證實現
- [docs/technical-implementation/frontend/README.md](../../docs/technical-implementation/frontend/README.md) - 前端實現
- [docs/technical-implementation/frontend/build-system.md](../../docs/technical-implementation/frontend/build-system.md) - 構建系統
- [docs/technical-implementation/frontend/micro-frontend-integration.md](../../docs/technical-implementation/frontend/micro-frontend-integration.md) - 微前端整合

### 4. UX Design (7 個引用)
- [docs/ux-design/README.md](../../docs/ux-design/README.md) - UX 設計文檔
- [docs/ux-design/design-system/README.md](../../docs/ux-design/design-system/README.md) - Design System
- [docs/ux-design/design-system/component-library.md](../../docs/ux-design/design-system/component-library.md) - 組件庫
- [docs/ux-design/design-system/design-tokens.md](../../docs/ux-design/design-system/design-tokens.md) - Design Tokens
- [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md) - Workflow Editor V2 索引
- [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part1.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part1.md) - Workflow Editor 核心設計
- [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part2.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part2.md) - Workflow Editor 內聯配置
- [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part3.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part3.md) - Workflow Editor 執行視覺化
- [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part4.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part4.md) - Workflow Editor 28 種節點
- [docs/ux-design/wireframes/low-fidelity/10-persona-builder.md](../../docs/ux-design/wireframes/low-fidelity/10-persona-builder.md) - Persona Builder UI

### 5. Deployment & Security (7 個引用)
- [docs/deployment/docker-setup.md](../../docs/deployment/docker-setup.md) - Docker 部署
- [docs/deployment/database-setup.md](../../docs/deployment/database-setup.md) - 資料庫部署
- [docs/deployment/monitoring-setup.md](../../docs/deployment/monitoring-setup.md) - 監控部署
- [docs/deployment/infrastructure-setup.md](../../docs/deployment/infrastructure-setup.md) - 基礎設施部署
- [docs/security/code-execution-security.md](../../docs/security/code-execution-security.md) - Code Execution 安全
- [docs/security/authentication-authorization.md](../../docs/security/authentication-authorization.md) - 認證授權安全
- [docs/api/api-security.md](../../docs/api/api-security.md) - API 安全規範

### 6. Development Standards (5 個引用)
- [docs/development-standards/coding-standards.md](../../docs/development-standards/coding-standards.md) - 代碼標準
- [docs/development-standards/cqrs-patterns.md](../../docs/development-standards/cqrs-patterns.md) - CQRS 模式
- [docs/development-standards/ddd-patterns.md](../../docs/development-standards/ddd-patterns.md) - DDD 模式
- [docs/development-standards/clean-architecture-guidelines.md](../../docs/development-standards/clean-architecture-guidelines.md) - Clean Architecture 指南
- [docs/api/database-schema.md](../../docs/api/database-schema.md) - 資料庫 Schema
- [docs/api/plugin-api-specification.md](../../docs/api/plugin-api-specification.md) - Plugin API 規範

### 7. PoC & Planning
- [poc-projects/poc6-vueflow-crdt/](../../poc-projects/poc6-vueflow-crdt/) - PoC 6 驗證報告
- [docs/technical-implementation/sprint-planning/sprint-allocation.md](../../docs/technical-implementation/sprint-planning/sprint-allocation.md) - Sprint 分配
- [docs/technical-implementation/devops/observability-implementation.md](../../docs/technical-implementation/devops/observability-implementation.md) - Observability 實現

---

**創建者**: Claude Code
**最後更新者**: Claude Code
**版本**: 2.0
**文檔統計**: 共 12 個技術決策，47 個 `/docs` 引用
**下次更新**: 當有新的技術決策時

---

[返回 claudedocs 主頁](../README.md) | [MVP 範圍定義](./MVP-SCOPE-DEFINITION.md) | [架構演進路線圖](./ARCHITECTURE-EVOLUTION-ROADMAP.md)
