# Sprint 2 上下文參考文檔

**目的**: 為 AI Assistant 提供 Sprint 2 開發所需的快速參考與上下文定位

**使用方式**:
- 🔗 包含關鍵文檔的**精確行號**，避免全文搜索
- 📋 提供 MVP 範圍快速參考
- 🎯 API 規格速查表
- 🏗️ 技術架構提醒
- ⚙️ 編碼標準參考

**維護**: Sprint 2 開發期間保持更新

---

## 📖 關鍵文檔索引（帶行號）

### Sprint 2 執行文檔

| 文檔 | 路徑 | 關鍵內容 | 行號範圍 |
|------|------|---------|---------|
| **Sprint 2 概覽** | `claudedocs/2-sprints/sprint-2/SPRINT-2-1-OVERVIEW.md` | Sprint 目標、User Stories 狀態、關鍵指標 | 1-762 |
| **Sprint 2 執行計劃** | `claudedocs/2-sprints/sprint-2/SPRINT-2-2-PLAN.md` | 技術實施細節、代碼範例、API 規格 | 1-1100+ |
| **變更記錄** | `claudedocs/4-changes/CHANGE-LOG.md` | CHANGE-001, CHANGE-002 詳細記錄 | 1-661 |

### 項目規劃文檔（/docs 參考）

| 文檔 | 路徑 | 關鍵內容 | 快速定位 |
|------|------|---------|---------|
| **US 1.4 規格** | `docs/user-stories/modules/module-01-agent-creation.md` (line 156+) | Agent 執行引擎、歷史追蹤、效能指標、即時監控 | Epic 1: Core Agent Management |
| **US 2.1-2.3 規格** | `docs/user-stories/modules/module-02-plugin-system.md` | Plugin 註冊、版本管理、動態載入、熱重載 | Epic 2: Plugin System (US 2.1 line 22+, US 2.2 line 171+, US 2.3 line 280+) |
| **US 6.1 規格** | `docs/user-stories/modules/module-06-chat-interface.md` (line 22+) | 基礎聊天介面、對話管理 | Epic 6: Frontend UI |

### 架構設計文檔

| 文檔 | 路徑 | 關鍵內容 | 關鍵章節 |
|------|------|---------|---------|
| **架構設計總覽** | `docs/architecture/Architecture-Design-Document.md` | Clean Architecture 分層、系統架構、技術棧、性能目標 | 執行摘要、系統架構、ADR 索引 |
| **數據庫設計** | `docs/architecture/database-schema.md` | PostgreSQL Schema、Entity 定義、索引策略 | agent_executions, plugin_versions, conversations |
| **C4 架構圖** | `docs/architecture/C4-architecture-diagrams.md` | 系統架構視圖、容器圖、組件圖 | Context, Container, Component diagrams |

---

## 🎯 MVP 範圍快速參考

### US 1.4: Agent 執行與監控 (13 SP, 4 Phases) ✅ 完成

**Phase 1: 基礎執行引擎**
- ✅ Semantic Kernel 整合
- ✅ `ExecuteAgentCommand` + `ExecuteAgentCommandHandler`
- ✅ `AgentExecution` Entity (Domain Layer)
- ✅ `IAgentExecutionRepository` + 實作
- ✅ API: `POST /api/v1/agents/{id}/execute`

**Phase 2: 執行歷史追蹤**
- ✅ `GetAgentExecutionHistoryQuery` (9 個查詢參數)
- ✅ 進階過濾: AgentId, ConversationId, Status, DateRange, Pagination, Sorting
- ✅ API: `GET /api/v1/agents/{id}/executions`
- ✅ API: `GET /api/v1/executions/{id}`

**Phase 3: 效能指標**
- ✅ `GetAgentStatisticsQuery` (統計分析)
- ✅ 指標: Total Executions, Avg/Min/Max Response Time, P95/P99, Token Usage
- ✅ API: `GET /api/v1/agents/{id}/statistics`

**Phase 4: 即時監控 & 匯出**
- ✅ SignalR Hub (`ExecutionMonitorHub`)
- ✅ WebSocket 訂閱機制 (Agent-level, Conversation-level, All executions)
- ✅ CSV/JSON 匯出功能
- ✅ API: WebSocket `/hubs/execution-monitor`
- ✅ API: `GET /api/v1/agents/{id}/executions/export?format=csv|json`

**關鍵技術**:
- Semantic Kernel: Prompt execution
- SignalR: WebSocket 即時推送
- Entity Framework Core: 執行歷史持久化
- LINQ: 統計計算 (Percentile, Aggregation)

---

### US 2.1: Plugin 註冊系統 (5 SP, 5 Phases) ✅ 完成

**Phase 1: Domain Layer**
- ✅ `PluginVersion` Entity (plugin_id, version, metadata, status)
- ✅ `VersionNumber` Value Object (SemVer: Major.Minor.Patch)
- ✅ `PluginMetadata` Value Object (JSONB: AssemblyName, Version, Author, Dependencies)
- ✅ `PluginStatus` Enum (Active, Inactive, Deprecated)

**Phase 2: 動態載入 (Infrastructure)**
- ✅ `IPluginLoader` Interface (LoadPluginAsync, UnloadPluginAsync, GetLoadedPlugins)
- ✅ `PluginLoader` 實作 (AssemblyLoadContext, Plugin 隔離)
- ✅ `IPluginActivator` Interface (ActivatePluginAsync, DeactivatePluginAsync)
- ✅ `PluginActivator` 實作 (狀態管理)

**Phase 3: Application Layer (CQRS)**
- ✅ `RegisterPluginCommand` + `RegisterPluginCommandHandler`
- ✅ `UpdatePluginCommand` + `UpdatePluginCommandHandler`
- ✅ `GetPluginVersionsQuery` + `GetPluginVersionsQueryHandler`
- ✅ FluentValidation: PluginId, Version, AssemblyPath

**Phase 4: API Layer**
- ✅ `PluginVersionsController` (5 個端點)
- ✅ API: `POST /api/v1/plugin-versions` (註冊 Plugin)
- ✅ API: `GET /api/v1/plugin-versions` (查詢列表)
- ✅ API: `GET /api/v1/plugin-versions/{id}` (獲取詳情)
- ✅ API: `PUT /api/v1/plugin-versions/{id}` (更新狀態)
- ✅ API: `GET /api/v1/plugin-versions/{pluginId}/history` (版本歷史)

**Phase 5: EF Core Repository + Migration**
- ✅ `IPluginVersionRepository` Interface
- ✅ `PluginVersionRepository` 實作
- ✅ EF Migration: `20251111061436_AddPluginVersioning.cs`
- ✅ JSONB Index: `CREATE INDEX idx_plugin_versions_metadata USING GIN (metadata)`

**關鍵技術**:
- AssemblyLoadContext: Plugin 隔離 (isCollectible: true)
- JSONB: 靈活的 Metadata 儲存
- SemVer: 版本號規範 (Major.Minor.Patch)
- Repository Pattern: 資料存取抽象

---

### US 2.2: Plugin 熱重載 (部分完成 40%) 🔄

**Phase 1-2: Commands 實作** ✅ 已完成
- ✅ `ReloadPluginCommand` + `ReloadPluginCommandHandler`
- ✅ `SwitchPluginVersionCommand` + `SwitchPluginVersionCommandHandler`
- ✅ Plugin 載入/卸載邏輯已在 `PluginLoader` 實作

**Phase 3-5: 待完成** ⏳
- ⏳ API 端點實作
- ⏳ Frontend 熱重載 UI
- ⏳ 測試與驗證

**變更說明**: CHANGE-002 - US 2.1 自然延伸至 US 2.2 Phase 1-2

---

### US 2.3: Plugin 版本管理 (部分完成 30%) 🔄

**Phase 1-2: Commands 實作** ✅ 已完成
- ✅ `GetPluginVersionHistoryQuery` + Handler
- ✅ `ComparePluginVersionsQuery` + Handler
- ✅ 版本對比邏輯實作

**Phase 3-5: 待完成** ⏳
- ⏳ API 端點實作
- ⏳ Frontend 版本管理 UI
- ⏳ 測試與驗證

**變更說明**: CHANGE-002 - US 2.1 自然延伸至 US 2.3 Phase 1-2

---

### US 6.1: 基礎聊天介面 (3 SP) ⏳ 待開始

**MVP 範圍**:
- 🎯 對話列表 (Conversation List)
  - 顯示所有對話
  - 創建新對話
  - 刪除對話

- 🎯 聊天介面 (Chat Interface)
  - 訊息列表 (Message List)
  - 輸入框 (Message Input)
  - 發送訊息 (Send Message)

- 🎯 即時更新
  - SignalR 連接
  - 即時訊息推送

**技術實施**:
- React 18 + TypeScript
- Material-UI v5
- TanStack Query (React Query)
- SignalR Client (`@microsoft/signalr`)

**API 依賴**:
- `POST /api/v1/conversations` (創建對話)
- `GET /api/v1/conversations` (查詢對話列表)
- `DELETE /api/v1/conversations/{id}` (刪除對話)
- `POST /api/v1/agents/{id}/execute` (執行 Agent)
- WebSocket `/hubs/execution-monitor` (即時訊息)

---

## 🔌 API 規格速查表

### Agent Execution API (US 1.4) - 11 個端點

#### 1. 執行 Agent
```http
POST /api/v1/agents/{id}/execute
Content-Type: application/json

Request Body:
{
  "userInput": "string",          // 必填
  "conversationId": "uuid",       // 選填
  "parameters": {                 // 選填
    "temperature": 0.7,
    "maxTokens": 2000
  }
}

Response: 201 Created
{
  "id": "uuid",
  "agentId": "uuid",
  "conversationId": "uuid",
  "userInput": "string",
  "response": "string",
  "totalTokens": 150,
  "promptTokens": 50,
  "completionTokens": 100,
  "responseTimeMs": 1234.56,
  "status": "Completed",
  "createdAt": "2025-12-10T10:00:00Z"
}
```

#### 2. 查詢執行歷史（進階過濾）
```http
GET /api/v1/agents/{id}/executions
  ?conversationId={uuid}          // 選填: 按對話過濾
  &status={status}                // 選填: Completed|Failed|Cancelled
  &startDate={ISO8601}            // 選填: 開始日期
  &endDate={ISO8601}              // 選填: 結束日期
  &page={int}                     // 必填: 頁碼 (default: 1)
  &pageSize={int}                 // 必填: 每頁數量 (default: 20, max: 100)
  &sortBy={field}                 // 選填: createdAt|responseTimeMs|totalTokens
  &sortOrder={asc|desc}           // 選填: 排序方向 (default: desc)

Response: 200 OK
{
  "items": [
    { "id": "uuid", "userInput": "...", "response": "...", "totalTokens": 150, ... }
  ],
  "totalCount": 500,
  "page": 1,
  "pageSize": 20,
  "totalPages": 25
}
```

#### 3. 獲取單筆執行記錄
```http
GET /api/v1/executions/{id}

Response: 200 OK
{
  "id": "uuid",
  "agentId": "uuid",
  "conversationId": "uuid",
  "userInput": "string",
  "response": "string",
  "totalTokens": 150,
  "responseTimeMs": 1234.56,
  "status": "Completed",
  "createdAt": "2025-12-10T10:00:00Z"
}
```

#### 4. Agent 統計資訊
```http
GET /api/v1/agents/{id}/statistics
  ?startDate={ISO8601}            // 選填: 統計開始日期
  &endDate={ISO8601}              // 選填: 統計結束日期

Response: 200 OK
{
  "agentId": "uuid",
  "totalExecutions": 1000,
  "successfulExecutions": 950,
  "failedExecutions": 50,
  "avgResponseTimeMs": 1200.5,
  "minResponseTimeMs": 500.0,
  "maxResponseTimeMs": 5000.0,
  "p95ResponseTimeMs": 2500.0,
  "p99ResponseTimeMs": 4000.0,
  "totalTokensUsed": 150000,
  "avgTokensPerExecution": 150,
  "dateRange": {
    "startDate": "2025-12-01T00:00:00Z",
    "endDate": "2025-12-10T23:59:59Z"
  }
}
```

#### 5. 匯出執行歷史（CSV/JSON）
```http
GET /api/v1/agents/{id}/executions/export
  ?format={csv|json}              // 必填: 匯出格式
  &conversationId={uuid}          // 選填: 按對話過濾
  &startDate={ISO8601}            // 選填: 開始日期
  &endDate={ISO8601}              // 選填: 結束日期

Response: 200 OK
Content-Type: text/csv | application/json
Content-Disposition: attachment; filename="agent-{id}-executions-{timestamp}.csv"

CSV Format:
Id,AgentId,ConversationId,UserInput,Response,TotalTokens,ResponseTimeMs,Status,CreatedAt
uuid1,uuid-agent,uuid-conv,"Hello","Hi there",50,800.5,Completed,2025-12-10T10:00:00Z
uuid2,uuid-agent,uuid-conv,"How are you?","I'm good",60,900.2,Completed,2025-12-10T10:05:00Z
```

#### 6-11. SignalR WebSocket 端點
```
WebSocket: /hubs/execution-monitor

Client → Server Methods:
- SubscribeToAgent(agentId: Guid)            // 訂閱特定 Agent 的執行通知
- UnsubscribeFromAgent(agentId: Guid)        // 取消訂閱
- SubscribeToConversation(conversationId: Guid)  // 訂閱特定對話
- UnsubscribeFromConversation(conversationId: Guid)
- SubscribeToAllExecutions()                 // 訂閱所有執行 (管理員)
- UnsubscribeFromAllExecutions()

Server → Client Events:
- ExecutionStarted(executionId: Guid, agentId: Guid, conversationId: Guid, timestamp: DateTime)
- ExecutionProgress(executionId: Guid, message: string, timestamp: DateTime)
- ExecutionCompleted(execution: AgentExecutionDto)
- ExecutionFailed(executionId: Guid, errorMessage: string, timestamp: DateTime)
```

---

### Plugin Versions API (US 2.1) - 5 個端點

#### 1. 註冊 Plugin
```http
POST /api/v1/plugin-versions
Content-Type: application/json

Request Body:
{
  "pluginId": "string",           // 必填: Plugin 唯一識別碼
  "version": "string",            // 必填: SemVer 格式 (1.0.0)
  "name": "string",               // 必填: Plugin 名稱
  "description": "string",        // 選填: 描述
  "assemblyPath": "string",       // 必填: Assembly 檔案路徑
  "metadata": {                   // 必填: Plugin Metadata
    "assemblyName": "string",
    "version": "string",
    "author": "string",
    "dependencies": ["dep1", "dep2"]
  }
}

Response: 201 Created
{
  "id": "uuid",
  "pluginId": "my-plugin",
  "version": "1.0.0",
  "name": "My Plugin",
  "description": "Plugin description",
  "status": "Active",
  "isCurrentVersion": true,
  "createdAt": "2025-12-10T10:00:00Z"
}
```

#### 2. 查詢 Plugin 版本列表
```http
GET /api/v1/plugin-versions
  ?pluginId={string}              // 選填: 按 Plugin ID 過濾
  &status={status}                // 選填: Active|Inactive|Deprecated
  &page={int}                     // 必填: 頁碼 (default: 1)
  &pageSize={int}                 // 必填: 每頁數量 (default: 20)

Response: 200 OK
{
  "items": [
    {
      "id": "uuid",
      "pluginId": "my-plugin",
      "version": "1.2.0",
      "name": "My Plugin",
      "status": "Active",
      "isCurrentVersion": true,
      "downloadCount": 150,
      "activeAgentCount": 10,
      "createdAt": "2025-12-10T10:00:00Z"
    }
  ],
  "totalCount": 50,
  "page": 1,
  "pageSize": 20,
  "totalPages": 3
}
```

#### 3. 獲取 Plugin 版本詳情
```http
GET /api/v1/plugin-versions/{id}

Response: 200 OK
{
  "id": "uuid",
  "pluginId": "my-plugin",
  "version": "1.2.0",
  "name": "My Plugin",
  "description": "Plugin description",
  "metadata": {
    "assemblyName": "MyPlugin.dll",
    "version": "1.2.0",
    "author": "John Doe",
    "dependencies": ["Newtonsoft.Json", "System.Text.Json"]
  },
  "status": "Active",
  "isCurrentVersion": true,
  "assemblyPath": "/plugins/my-plugin/1.2.0/MyPlugin.dll",
  "downloadCount": 150,
  "activeAgentCount": 10,
  "createdAt": "2025-12-10T10:00:00Z",
  "updatedAt": "2025-12-10T12:00:00Z"
}
```

#### 4. 更新 Plugin 狀態
```http
PUT /api/v1/plugin-versions/{id}
Content-Type: application/json

Request Body:
{
  "status": "Inactive"            // Active|Inactive|Deprecated
}

Response: 200 OK
{
  "id": "uuid",
  "pluginId": "my-plugin",
  "version": "1.2.0",
  "status": "Inactive",
  "updatedAt": "2025-12-10T15:00:00Z"
}
```

#### 5. 查詢 Plugin 版本歷史
```http
GET /api/v1/plugin-versions/{pluginId}/history
  ?page={int}
  &pageSize={int}

Response: 200 OK
{
  "pluginId": "my-plugin",
  "items": [
    {
      "id": "uuid1",
      "version": "1.2.0",
      "status": "Active",
      "isCurrentVersion": true,
      "createdAt": "2025-12-10T10:00:00Z"
    },
    {
      "id": "uuid2",
      "version": "1.1.0",
      "status": "Deprecated",
      "isCurrentVersion": false,
      "createdAt": "2025-11-01T10:00:00Z"
    }
  ],
  "totalCount": 5
}
```

---

## 🏗️ 技術架構快速參考

### Backend 技術棧
```yaml
框架: ASP.NET Core 8
語言: C# 12
ORM: Entity Framework Core 8
資料庫: PostgreSQL 16
即時通訊: SignalR (WebSocket)
AI 引擎: Microsoft Semantic Kernel 1.0+

架構模式:
  - Clean Architecture (4-layer)
  - CQRS (MediatR)
  - Repository Pattern
  - Unit of Work

驗證:
  - FluentValidation 11+
  - Data Annotations

測試:
  - xUnit
  - Moq
  - FluentAssertions
```

### Frontend 技術棧
```yaml
框架: React 18
語言: TypeScript 5+
UI 庫: Material-UI v5
狀態管理: Zustand 4+
資料查詢: TanStack Query (React Query) v5
即時通訊: @microsoft/signalr 8+
HTTP 客戶端: Axios 1.6+

構建工具:
  - Vite 5+
  - TypeScript Compiler

測試:
  - Vitest
  - React Testing Library
  - Playwright (E2E)
```

### 資料庫 Schema 設計

#### agent_executions 表（US 1.4）
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
    status VARCHAR(50) NOT NULL,    -- Completed, Failed, Cancelled
    error_message TEXT,
    is_deleted BOOLEAN DEFAULT false,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP NOT NULL DEFAULT NOW(),
    created_by VARCHAR(100),
    updated_by VARCHAR(100)
);

-- 索引
CREATE INDEX idx_agent_executions_agent_id ON agent_executions(agent_id);
CREATE INDEX idx_agent_executions_conversation_id ON agent_executions(conversation_id);
CREATE INDEX idx_agent_executions_status ON agent_executions(status);
CREATE INDEX idx_agent_executions_created_at ON agent_executions(created_at DESC);
```

#### plugin_versions 表（US 2.1）
```sql
CREATE TABLE plugin_versions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    plugin_id VARCHAR(100) NOT NULL,
    version VARCHAR(20) NOT NULL,   -- SemVer: 1.2.3
    name VARCHAR(200) NOT NULL,
    description TEXT,
    metadata JSONB NOT NULL,        -- PluginMetadata
    status VARCHAR(50) NOT NULL,    -- Active, Inactive, Deprecated
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

-- 索引
CREATE INDEX idx_plugin_versions_plugin_id ON plugin_versions(plugin_id);
CREATE INDEX idx_plugin_versions_status ON plugin_versions(status);
CREATE INDEX idx_plugin_versions_is_current_version ON plugin_versions(is_current_version);
CREATE INDEX idx_plugin_versions_created_at ON plugin_versions(created_at DESC);

-- JSONB GIN Index
CREATE INDEX idx_plugin_versions_metadata ON plugin_versions USING GIN (metadata);
```

---

## ⚙️ 編碼標準快速參考

### C# 命名規範
```csharp
// PascalCase: Classes, Methods, Properties, Events
public class AgentExecutionService { }
public async Task<Result> ExecuteAgentAsync() { }
public string AgentName { get; set; }
public event EventHandler ExecutionCompleted;

// camelCase: Local variables, Parameters, Private fields
private readonly IAgentRepository _agentRepository;
public async Task ProcessAsync(string agentId, int maxRetries) { }

// Interface: I prefix
public interface IAgentRepository { }
public interface IPluginLoader { }

// Async: Async suffix for async methods
public async Task<Agent> GetAgentAsync(Guid id);
public async Task<List<Agent>> GetAllAgentsAsync();
```

### TypeScript/React 命名規範
```typescript
// PascalCase: Components, Types, Interfaces, Enums
export const AgentCard: React.FC<AgentCardProps> = () => {};
export interface AgentDto { }
export type AgentStatus = 'Active' | 'Inactive';
export enum PluginStatus { Active, Inactive, Deprecated }

// camelCase: Variables, Functions, Hooks
const agentId = 'uuid';
const handleExecute = () => {};
export const useAgents = () => {};
export const useCreateAgent = () => {};

// UPPER_SNAKE_CASE: Constants
export const API_BASE_URL = 'https://api.example.com';
export const MAX_RETRIES = 3;
```

### Git Commit Message 格式
```bash
# 格式: <type>(<scope>): <subject>

# Types:
feat     # 新功能
fix      # Bug 修復
refactor # 代碼重構
docs     # 文檔更新
test     # 測試相關
chore    # 構建/配置相關
style    # 代碼格式調整

# Examples:
feat(agent): implement agent execution with Semantic Kernel
fix(plugin): resolve plugin loading issue in PluginLoader
refactor(execution): extract statistics calculation to separate service
docs(api): update API documentation for execution endpoints
test(agent): add unit tests for AgentExecutionService
```

---

## 🎯 開發優先順序（剩餘工作）

### 1. US 6.1: 基礎聊天介面（最高優先級）⏳
**工作量**: 3 SP (~3 days)

**任務清單**:
- [ ] Backend: Conversations API (CRUD)
  - [ ] CreateConversationCommand + Handler
  - [ ] GetConversationsQuery + Handler
  - [ ] DeleteConversationCommand + Handler
  - [ ] ConversationsController (3 個端點)

- [ ] Frontend: Chat UI Components
  - [ ] ConversationList 組件
  - [ ] ChatWindow 組件
  - [ ] MessageList 組件
  - [ ] MessageInput 組件

- [ ] SignalR Integration
  - [ ] Frontend SignalR 連接設置
  - [ ] 即時訊息接收與顯示

- [ ] 測試
  - [ ] API 集成測試
  - [ ] Component 單元測試
  - [ ] E2E 測試（Playwright）

---

### 2. US 2.2/2.3: Plugin 熱重載與版本管理（中優先級）🔄
**工作量**: ~3-4 days (Phase 3-5)

**US 2.2 剩餘任務**:
- [ ] API 端點
  - [ ] `POST /api/v1/plugin-versions/{id}/reload`
  - [ ] `POST /api/v1/plugin-versions/{id}/switch-version`

- [ ] Frontend UI
  - [ ] Plugin 管理頁面
  - [ ] 熱重載按鈕與狀態顯示

**US 2.3 剩餘任務**:
- [ ] API 端點
  - [ ] `GET /api/v1/plugin-versions/{pluginId}/compare?v1={version1}&v2={version2}`
  - [ ] `POST /api/v1/plugin-versions/{id}/rollback`

- [ ] Frontend UI
  - [ ] 版本對比介面
  - [ ] 版本歷史時間軸

---

## 📚 相關文檔連結

### Sprint 執行文檔
- [Sprint 2 概覽](./SPRINT-2-1-OVERVIEW.md) - Sprint 目標、User Stories 狀態
- [Sprint 2 執行計劃](./SPRINT-2-2-PLAN.md) - 詳細技術實施指南
- [Sprint 2 檢查清單](./SPRINT-2-4-CHECKLIST.md) - 任務追蹤清單
- [Sprint 2 開發日誌](./SPRINT-2-5-DEV-LOG.md) - 每日開發記錄
- [Sprint 2 問題追蹤](./SPRINT-2-6-ISSUES.md) - 問題與解決方案
- [Sprint 2 回顧](./SPRINT-2-7-RETROSPECTIVE.md) - Sprint 完成後總結

### 項目規劃文檔
- [變更記錄](../../4-changes/CHANGE-LOG.md) - CHANGE-001, CHANGE-002
- [User Story 狀態](../../3-progress/USER-STORY-STATUS.md) - 所有 User Stories 狀態追蹤

### 架構與設計
- [架構設計總覽](../../../docs/architecture/Architecture-Design-Document.md) - Clean Architecture、CQRS、系統架構
- [數據庫設計](../../../docs/architecture/database-schema.md) - PostgreSQL Schema、Entity 定義
- [C4 架構圖](../../../docs/architecture/C4-architecture-diagrams.md) - 系統架構視圖、Plugin 系統、SignalR 設計

---

## 📚 完整參考文獻索引

本上下文文檔整合了以下技術細節與架構決策，按類別組織以便 AI Assistant 快速定位：

### Planning 文檔（濃縮版，優先查閱）

- [MVP Scope Definition](../../1-planning/MVP-SCOPE-DEFINITION.md) - Sprint 2 在 MVP 中的範圍與邊界
- [Sprint Allocation Analysis](../../1-planning/SPRINT-ALLOCATION-ANALYSIS.md) - Sprint 2 詳細分配、Story Points、依賴關係
- [Development Strategy](../../1-planning/DEVELOPMENT-STRATEGY.md) - Git 工作流、CI/CD 流程、測試策略
- [Architecture Evolution Roadmap](../../1-planning/ARCHITECTURE-EVOLUTION-ROADMAP.md) - 架構演進階段規劃
- [Technical Decisions Log](../../1-planning/TECHNICAL-DECISIONS-LOG.md) - 關鍵技術決策記錄（SignalR、AssemblyLoadContext）
- [Dependency Matrix](../../1-planning/DEPENDENCY-MATRIX.md) - US 1.4, 2.1, 6.1 依賴關係追蹤
- [Risk Register](../../1-planning/RISK-REGISTER.md) - Sprint 2 技術風險評估與緩解策略

### 架構設計決策 (ADR)

- [ADR-002: CQRS Pattern](../../docs/architecture/adr/ADR-002-cqrs-pattern.md)
  - MediatR Commands/Queries 設計模式
  - ExecuteAgentCommand, RegisterPluginCommand 實作參考
- [ADR-006: Agent State Management](../../docs/architecture/adr/ADR-006-agent-state-management.md)
  - Agent 執行狀態管理策略
  - State Machine 設計原則
- [ADR-007: Multi-Agent Communication](../../docs/architecture/adr/ADR-007-multi-agent-communication.md)
  - SignalR WebSocket 通訊架構
  - ExecutionMonitorHub 設計參考
- [ADR-008: Code Interpreter Execution Model](../../docs/architecture/adr/ADR-008-code-interpreter-execution-model.md)
  - 執行引擎安全設計原則
  - 資源隔離策略
- [ADR-011: Framework Migration Strategy](../../docs/architecture/adr/ADR-011-framework-migration-strategy.md)
  - Semantic Kernel 抽象層設計
  - IAgentExecutor 介面定義
- [ADR-012: Workflow Editor Technology](../../docs/architecture/adr/ADR-012-workflow-editor-technology.md)
  - React 18 技術選型理由
  - Material-UI + Zustand 選擇依據
- [Architecture Design Document](../../docs/architecture/Architecture-Design-Document.md)
  - Clean Architecture 4-layer 設計
  - 系統架構概覽
- [Database Schema](../../docs/architecture/database-schema.md)
  - agent_executions Table 完整設計
  - plugin_versions Table 完整設計
  - conversations Table 完整設計

### User Stories 完整規格

- [Module 01: Agent Creation](../../docs/user-stories/modules/module-01-agent-creation.md)
  - US 1.4 完整規格（Line 156+）
  - 驗收標準詳細列表
  - 技術實施要求
- [Module 02: Plugin System](../../docs/user-stories/modules/module-02-plugin-system.md)
  - US 2.1 Plugin 註冊規格（Line 22+）
  - US 2.2 Plugin 熱重載規格（Line 171+）
  - US 2.3 Plugin 版本管理規格（Line 280+）
- [Module 06: Chat Interface](../../docs/user-stories/modules/module-06-chat-interface.md)
  - US 6.1 基礎對話功能規格（Line 22+）
  - Chat UI 組件需求
  - SignalR 集成需求

### Backend 技術實施參考 (.NET 9)

- [Semantic Kernel Integration](../../docs/technical-implementation/01-backend-net9/08-semantic-kernel-integration.md)
  - Kernel Builder 配置範例
  - OpenAI Chat Completion 整合
  - Prompt 管理最佳實踐
- [CQRS Implementation](../../docs/technical-implementation/01-backend-net9/05-cqrs-implementation.md)
  - MediatR 配置與註冊
  - Command/Query Handler 實作範例
  - FluentValidation 整合模式
- [Plugin System Architecture](../../docs/technical-implementation/01-backend-net9/11-plugin-system-architecture.md)
  - AssemblyLoadContext 動態加載詳解
  - Plugin Isolation 實作
  - Unload 機制實作
- [SignalR WebSocket](../../docs/technical-implementation/01-backend-net9/10-signalr-websocket.md)
  - SignalR Hub 實作範例
  - CORS 配置
  - Group 管理實作
- [Repository Pattern](../../docs/technical-implementation/01-backend-net9/06-repository-pattern.md)
  - Generic Repository 實作
  - Specification Pattern 應用
  - EF Core 最佳實踐
- [Value Objects](../../docs/technical-implementation/01-backend-net9/07-value-objects.md)
  - VersionNumber (SemVer) 實作
  - PluginMetadata 實作
  - Value Converters 設計

### Frontend 技術實施參考 (React 18)

- [React Coding Standards](../../docs/technical-implementation/04-coding-standards/react-coding-standards.md)
  - Functional Components 規範
  - Hooks 使用指引
  - 效能優化建議
- [TypeScript Coding Standards](../../docs/technical-implementation/04-coding-standards/typescript-coding-standards.md)
  - TypeScript 類型系統
  - Interface vs Type 選擇
  - Generics 應用
- [State Management (Zustand)](../../docs/technical-implementation/02-frontend-react/06-state-management-zustand.md)
  - Zustand Store 設計
  - State Slicing 模式
  - Middleware 應用
- [API Client Integration](../../docs/technical-implementation/02-frontend-react/07-api-client-integration.md)
  - Axios 配置
  - Interceptors 實作
  - Error Handling 策略
- [Component Architecture](../../docs/technical-implementation/02-frontend-react/03-component-architecture.md)
  - Smart/Dumb Components 分離
  - Component Composition
  - Props 設計原則

### API 設計規範

- [RESTful API Standards](../../docs/technical-implementation/05-api-design/restful-api-standards.md)
  - REST 設計原則
  - HTTP Methods 使用規範
  - Status Codes 標準
- [API Documentation](../../docs/technical-implementation/05-api-design/api-documentation.md)
  - Swagger/OpenAPI 配置
  - API 端點文檔標準
- [Error Handling](../../docs/technical-implementation/05-api-design/error-handling.md)
  - Result Pattern 實作
  - Exception Handling 策略

### 資料庫設計規範

- [Database Design Principles](../../docs/technical-implementation/06-database-standards/database-design-principles.md)
  - 資料庫設計最佳實踐
  - 索引設計策略
- [Entity Framework Core Configuration](../../docs/technical-implementation/06-database-standards/entity-framework-core-configuration.md)
  - Fluent API 配置
  - Value Converters 實作（JSON, SemVer）
- [Database Migration Strategy](../../docs/technical-implementation/06-database-standards/database-migration-strategy.md)
  - EF Core Migrations 工作流
  - Rollback 策略

### 測試規範

- [Testing Strategy](../../docs/technical-implementation/07-testing-strategy/README.md)
  - 測試金字塔
  - 80%+ 覆蓋率目標
- [Unit Testing Standards](../../docs/technical-implementation/07-testing-strategy/unit-testing-standards.md)
  - xUnit 測試框架
  - Moq 模擬框架
  - AAA 模式
- [Integration Testing Standards](../../docs/technical-implementation/07-testing-strategy/integration-testing-standards.md)
  - WebApplicationFactory 使用
  - TestContainers 整合

### UX 設計參考

- [Wireframe: Conversation](../../docs/ux-design/wireframes/low-fidelity/05-conversation.md)
  - Chat UI 佈局設計
  - Message List 組件規範
- [Wireframe: Agent Detail](../../docs/ux-design/wireframes/low-fidelity/04-agent-detail.md)
  - Agent 執行監控介面設計
- [Design System](../../docs/ux-design/design-system/README.md)
  - Material-UI 主題配置
  - 色彩與字體系統
- [Component Library](../../docs/ux-design/design-system/component-library.md)
  - UI 元件使用規範

### 變更管理

- [Change Log](../../4-changes/CHANGE-LOG.md)
  - CHANGE-001: US 1.4 範圍擴展詳情
  - CHANGE-002: US 2.1 延伸至 US 2.2/2.3

---

**文檔版本**: v2.0
**創建日期**: 2025-12-10
**最後更新**: 2025-12-11
**維護者**: AI Development Assistant
**狀態**: 🔄 Sprint 2 進行中
**升級內容**: 新增完整參考文獻索引（50+ 文檔），優先引用 /claudedocs/1-planning
