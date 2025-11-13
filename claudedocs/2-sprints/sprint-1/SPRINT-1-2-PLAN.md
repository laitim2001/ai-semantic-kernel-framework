# Sprint 1 計劃書

**版本**: 2.0
**目的**: 詳細技術規格和實施計劃，指導 Sprint 1 整體開發工作
**Sprint 週期**: 第 1-3 週 (2025-11-04 ~ 2025-11-22)
**Sprint 定位**: Phase 1A - 基礎平台 (13 SP, 3 週)

**規劃文檔參考**:
- 📋 [MVP Scope Definition](../../1-planning/MVP-SCOPE-DEFINITION.md) - Phase 1A 範圍定義
- 📊 [Sprint Allocation Analysis](../../1-planning/SPRINT-ALLOCATION-ANALYSIS.md#sprint-1) - Sprint 1 詳細分析
- 🎯 [Development Strategy](../../1-planning/DEVELOPMENT-STRATEGY.md) - Phase 1A 開發策略
- 📐 [Architecture Design Document](../../docs/architecture/Architecture-Design-Document.md) - 系統架構總覽

---

## 第一部分：本 Sprint 要建立什麼 (What to Build)

### US 1.1: 透過 Web UI 創建 Agent (5 SP)

**User Story 完整規格**: [US 1.1 - Web UI Agent Creation](../../docs/user-stories/us-1-agent-crud.md#us-11)

#### 一、MVP 範圍定義

**必須實現功能 (P0 - 本 Sprint)**:
- [x] **Agent 創建表單**: Web UI 表單組件
  - 輸入欄位: Agent 名稱, 描述, 角色類型, LLM 模型, System Prompt, 模型參數
  - 輸出結果: 成功創建後返回 Agent ID 和詳情
  - 業務邏輯: 表單驗證 → API 調用 → 成功顯示/錯誤處理
  - **參考**: [Agent Create Wireframe](../../docs/ux-design/wireframes/low-fidelity/03-agent-create.md)
  - **參考**: [Form Validation Strategy](../../docs/technical-implementation/02-frontend-react/04-form-validation.md)

- [x] **模型選擇器**: LLM 模型下拉選單
  - 支援模型: GPT-4, GPT-4o, GPT-4o-mini
  - 預設值: GPT-4o
  - **參考**: [Component Design](../../docs/technical-implementation/02-frontend-react/02-component-design.md#model-selector)

- [x] **Prompt 編輯器**: System Prompt 文本編輯器
  - 支援多行輸入
  - 顯示字數統計
  - 提供範例模板 (可選)
  - **參考**: [Component Library](../../docs/ux-design/design-system/component-library.md#text-editors)

- [x] **參數控制面板**: 模型參數調整
  - Temperature: 0-2 (步進 0.1, 預設 0.7)
  - Max Tokens: 1-4096 (預設 2000)
  - Top P: 0-1 (步進 0.1, 預設 1)
  - **參考**: [Agent Configuration Spec](../../docs/user-stories/us-1-agent-crud.md#model-parameters)

- [x] **表單提交**: 完成 Agent 創建
  - 驗證所有必填欄位
  - 顯示 Loading 狀態
  - 成功後導航到: Agent 詳情頁面
  - **參考**: [Error Handling Strategy](../../docs/api/api-design.md#error-handling)

**明確排除 Phase 2 功能 (延後到下個 Sprint)**:
- ❌ **Plugin 配置**: 為 Agent 綁定和配置 Plugin (US 2.x)
- ❌ **高級參數**: Frequency Penalty, Presence Penalty (US 1.4)
- ❌ **批量創建**: 批量上傳多個 Agent 配置 (US 1.x Extended)

**MVP 範圍參考**:
- 📖 [MVP Scope Definition](../../1-planning/MVP-SCOPE-DEFINITION.md#phase-1a) - Phase 1A 詳細範圍
- 📖 [User Story Prioritization](../../docs/user-stories/README.md#prioritization) - User Story 優先級

#### 二、詳細技術規格

##### Backend API 規格

**API 設計完整文檔**: [API Design Specification](../../docs/api/api-design.md)

**API 端點**: `POST /api/v1/agents`

**Request**:
```json
{
  "name": "Customer Support Agent",
  "description": "24/7 customer service assistant",
  "role": "CustomerService",
  "model": "gpt-4o",
  "systemPrompt": "You are a helpful customer service agent...",
  "parameters": {
    "temperature": 0.7,
    "maxTokens": 2000,
    "topP": 1.0
  }
}
```

**Response** (201 Created):
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "name": "Customer Support Agent",
  "description": "24/7 customer service assistant",
  "role": "CustomerService",
  "model": "gpt-4o",
  "systemPrompt": "You are a helpful customer service agent...",
  "parameters": {
    "temperature": 0.7,
    "maxTokens": 2000,
    "topP": 1.0
  },
  "isActive": true,
  "createdAt": "2025-11-05T10:30:00Z",
  "updatedAt": "2025-11-05T10:30:00Z"
}
```

**驗證規則**:
- `name`: 必填, 長度 3-100 字符, 不可重複
- `systemPrompt`: 必填, 長度 10-4000 字符
- `model`: 必填, 必須是 ["gpt-4", "gpt-4o", "gpt-4o-mini"] 之一
- `temperature`: 可選, 範圍 0.0-2.0
- `maxTokens`: 可選, 範圍 1-4096
- `topP`: 可選, 範圍 0.0-1.0

**驗證策略參考**:
- 📄 [Validation Strategy](../../docs/technical-implementation/01-backend-net9/06-validation-strategy.md) - FluentValidation 實作
- 📄 [API Validation Rules](../../docs/api/api-design.md#validation-rules) - 統一驗證規則

**錯誤處理**:
- `400 Bad Request`: 驗證失敗 (詳細錯誤信息)
- `409 Conflict`: Agent 名稱已存在
- `500 Internal Server Error`: 服務器錯誤

**錯誤處理參考**:
- 📄 [Error Handling Strategy](../../docs/api/api-design.md#error-handling) - 統一錯誤處理
- 📄 [HTTP Status Codes](../../docs/api/api-design.md#http-status-codes) - 狀態碼規範

##### Database Schema

**Database 設計完整文檔**: [Database Schema Design](../../docs/api/database-schema.md)

**Table**: `agents`

```sql
CREATE TABLE agents (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(100) NOT NULL UNIQUE,
    description TEXT,
    role VARCHAR(50) NOT NULL,
    system_prompt TEXT NOT NULL,
    model VARCHAR(50) NOT NULL,
    temperature DECIMAL(3,2) DEFAULT 0.7,
    max_tokens INTEGER DEFAULT 2000,
    top_p DECIMAL(3,2) DEFAULT 1.0,
    is_active BOOLEAN DEFAULT true,
    is_deleted BOOLEAN DEFAULT false,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP NOT NULL DEFAULT NOW(),
    created_by VARCHAR(100),
    updated_by VARCHAR(100)
);

-- Indexes
CREATE INDEX idx_agents_name ON agents(name);
CREATE INDEX idx_agents_is_active ON agents(is_active);
CREATE INDEX idx_agents_is_deleted ON agents(is_deleted);
CREATE INDEX idx_agents_created_at ON agents(created_at DESC);
```

**Database 設計參考**:
- 🗄️ [Database Schema Design](../../docs/api/database-schema.md) - 完整 Schema 設計
- 🗄️ [Indexing Strategy](../../docs/api/database-schema.md#indexing-strategy) - 索引策略
- 🗄️ [ADR-004: PostgreSQL Selection](../../docs/architecture/adr/ADR-004-database-selection.md) - 資料庫選型
- 🗄️ [Migration Strategy](../../docs/technical-implementation/01-backend-net9/04-data-access-layer.md#ef-core-migrations) - EF Core Migration

**說明**:
- 使用軟刪除機制 (Phase 1 保留歷史)
- 參考: [Soft Delete Pattern](../../docs/technical-implementation/01-backend-net9/04-data-access-layer.md#soft-delete-pattern)

##### Frontend UI 規格

**Frontend 設計完整文檔**: [Frontend Architecture](../../docs/technical-implementation/02-frontend-react/01-frontend-architecture.md)

**本 Sprint 開發的 UI 組件**:

**1. AgentCreateForm** (React Component)
- **位置**: `/agents/create` 頁面
- **Layout**: 單欄表單佈局
- **主要元素**:
  - Input (Text): Agent 名稱 (Top)
  - Textarea: Agent 描述
  - Select: 角色類型下拉選單
  - Select: LLM 模型選擇器
  - Textarea + Toolbar: System Prompt 編輯器
  - Slider Group: 參數控制面板 (Temperature, Max Tokens, Top P)
  - Button Group: 提交/取消按鈕 (Bottom)

- **行為邏輯**:
  - 表單驗證: 使用客戶端驗證 (Yup schema)
  - 提交: 調用 POST /api/v1/agents → 成功導航/錯誤處理
  - 取消: 返回列表頁面

**UI 設計參考**:
- 🎨 [Agent Create Wireframe](../../docs/ux-design/wireframes/low-fidelity/03-agent-create.md) - 完整線框圖
- 🎨 [Component Design](../../docs/technical-implementation/02-frontend-react/02-component-design.md) - 組件設計規範
- 🎨 [Form Validation](../../docs/technical-implementation/02-frontend-react/04-form-validation.md) - 表單驗證策略
- 🎨 [Design System](../../docs/ux-design/design-system/README.md) - 設計系統規範

**本 Sprint 不開發的 UI** (Phase 2):
- ❌ **Plugin 選擇器**: Phase 2 (Sprint 2-3 實現)
- ❌ **Persona 選擇**: Phase 2 (Sprint 4-5 實現)
- ❌ **高級參數面板**: Frequency Penalty, Presence Penalty

---

### US 1.2: 透過 .NET SDK 創建 Agent (5 SP)

**User Story 完整規格**: [US 1.2 - .NET SDK Agent Creation](../../docs/user-stories/us-1-agent-crud.md#us-12)
**SDK 設計文檔**: [Agent SDK Architecture](../../docs/technical-implementation/01-backend-net9/03-agent-sdk.md)

#### 一、MVP 範圍定義

**必須實現功能 (P0 - 本 Sprint)**:
- [x] **AgentClient SDK**: C# Client SDK
  - NuGet 套件打包
  - Fluent API Builder Pattern
  - 異步操作支援 (async/await)
  - **參考**: [SDK Architecture](../../docs/technical-implementation/01-backend-net9/03-agent-sdk.md#architecture)

- [x] **NuGet 套件**: `SemanticKernel.Agentic`
  - 版本: 0.1.0-alpha
  - 目標框架: .NET 8.0
  - 依賴: HttpClient, System.Text.Json
  - **參考**: [SDK NuGet Configuration](../../docs/technical-implementation/01-backend-net9/03-agent-sdk.md#nuget-packaging)

- [x] **XML 文檔註釋**: 完整 IntelliSense 支援
  - 所有公開 API 都有註釋
  - 使用範例
  - **參考**: [Documentation Standards](../../docs/development-standards/documentation-standards.md#xml-documentation)

- [x] **範例代碼**: SDK 使用文檔
  - GitHub Repository 根目錄 samples/ 資料夾
  - README.md 快速開始指南
  - **參考**: [SDK Usage Examples](../../docs/technical-implementation/01-backend-net9/03-agent-sdk.md#usage-examples)

#### 二、詳細技術規格

##### SDK API 設計

**SDK 設計模式參考**:
- 📦 [Fluent API Builder Pattern](../../docs/technical-implementation/01-backend-net9/03-agent-sdk.md#fluent-api-builder)
- 📦 [SDK Error Handling](../../docs/technical-implementation/01-backend-net9/03-agent-sdk.md#error-handling)
- 📦 [Async/Await Best Practices](../../docs/development-standards/coding-conventions.md#async-await)

**使用範例**:
```csharp
using SemanticKernel.Agentic;

// 初始化 Client
var agentClient = new AgentClient(
    apiKey: "your-api-key",
    endpoint: "https://api.example.com"
);

// 創建 Agent (Fluent API)
var agent = await agentClient
    .CreateAgent()
    .WithName("Customer Support Agent")
    .WithDescription("24/7 customer service assistant")
    .WithRole(AgentRole.CustomerService)
    .WithModel(LLMModel.GPT4o)
    .WithSystemPrompt("You are a helpful customer service agent...")
    .WithTemperature(0.7)
    .WithMaxTokens(2000)
    .WithTopP(1.0)
    .BuildAsync();

Console.WriteLine($"Agent created: {agent.Id}");

// 執行 Agent (Phase 2)
// var response = await agentClient.InvokeAsync(
//     agentId: agent.Id,
//     message: "How can I track my order?",
//     sessionId: "session-123"
// );
```

**核心類型**:
- `AgentClient`: 主要 SDK 入口
- `AgentBuilder`: Fluent API Builder
- `Agent`: Agent 實體模型
- `AgentRole`: 角色枚舉
- `LLMModel`: LLM 模型枚舉
- `AgentParameters`: 模型參數配置

**SDK 類型設計參考**:
- 📦 [SDK Type System](../../docs/technical-implementation/01-backend-net9/03-agent-sdk.md#type-system)
- 📦 [Builder Pattern Implementation](../../docs/technical-implementation/01-backend-net9/03-agent-sdk.md#builder-implementation)

---

### US 1.3: Agent 配置管理 (3 SP)

**User Story 完整規格**: [US 1.3 - Agent Configuration Management](../../docs/user-stories/us-1-agent-crud.md#us-13)

#### 一、MVP 範圍定義

**必須實現功能 (P0 - 本 Sprint)**:
- [x] **Agent 列表頁面**: 顯示所有 Agent
  - 卡片佈局 (Grid Layout)
  - 分頁 (每頁 20 條)
  - 搜尋 (名稱, 描述)
  - 篩選 (角色, 模型, 狀態)
  - **參考**: [Agent List Wireframe](../../docs/ux-design/wireframes/low-fidelity/02-agent-list.md)
  - **參考**: [Data Fetching Strategy](../../docs/technical-implementation/02-frontend-react/05-data-fetching.md)

- [x] **Agent 詳情頁面**: 顯示 Agent 完整配置
  - 顯示所有欄位
  - 操作按鈕 (編輯, 刪除, 啟用/停用)
  - **參考**: [Agent Detail Wireframe](../../docs/ux-design/wireframes/low-fidelity/04-agent-detail.md)

- [x] **Agent 編輯頁面**: 更新 Agent 配置
  - 重用 AgentCreateForm 組件
  - 預填現有資料
  - 更新 API 調用
  - **參考**: [Component Reusability](../../docs/technical-implementation/02-frontend-react/02-component-design.md#reusability)

- [x] **Agent 刪除**: 軟刪除功能
  - 刪除確認對話框
  - 調用 DELETE API
  - 刪除後返回列表頁面
  - **參考**: [Soft Delete Pattern](../../docs/technical-implementation/01-backend-net9/04-data-access-layer.md#soft-delete-pattern)

**明確排除 Phase 2 功能 (延後)**:
- ❌ **批量操作**: 批量啟用/停用/刪除 (US 1.3 Extended)
- ❌ **版本管理**: 保存 Agent 配置的歷史版本 (US 2.x)
- ❌ **匯入匯出**: 匯出為 JSON/匯入配置 (US 1.x Extended)
- ❌ **複製 Agent**: 快速複製現有 Agent 配置 (US 1.x Extended)

#### 二、詳細技術規格

##### Backend API 規格

**API 設計參考**:
- 📄 [API Design Specification](../../docs/api/api-design.md) - RESTful API 設計
- 📄 [Pagination Strategy](../../docs/api/api-design.md#pagination) - 分頁設計
- 📄 [Search & Filter Design](../../docs/api/api-design.md#search-filter) - 搜尋篩選設計

**列表端點**: `GET /api/v1/agents`

**Query Parameters**:
```
?page=1&pageSize=20&search=customer&role=CustomerService&model=gpt-4o&isActive=true
```

**Response** (200 OK):
```json
{
  "data": [
    {
      "id": "uuid",
      "name": "Customer Support Agent",
      "description": "24/7 customer service assistant",
      "role": "CustomerService",
      "model": "gpt-4o",
      "isActive": true,
      "createdAt": "2025-11-05T10:30:00Z",
      "updatedAt": "2025-11-05T10:30:00Z"
    }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "totalCount": 45,
    "totalPages": 3
  }
}
```

**詳情端點**: `GET /api/v1/agents/{id}`

**更新端點**: `PUT /api/v1/agents/{id}`

**刪除端點**: `DELETE /api/v1/agents/{id}`
- 軟刪除: 設置 `is_deleted = true`
- 響應: 204 No Content

**CRUD API 參考**:
- 📄 [CQRS Implementation](../../docs/technical-implementation/01-backend-net9/05-cqrs-implementation.md) - 命令查詢分離
- 📄 [Repository Pattern](../../docs/architecture/adr/ADR-003-repository-pattern.md) - 資料存取抽象

##### Frontend UI 規格

**Frontend 組件設計參考**:
- 🎨 [Component Design](../../docs/technical-implementation/02-frontend-react/02-component-design.md)
- 🎨 [State Management](../../docs/technical-implementation/02-frontend-react/03-state-management.md)
- 🎨 [Component Library](../../docs/ux-design/design-system/component-library.md)

**1. AgentListView** (React Component)
- **位置**: `/agents` 頁面
- **Layout**: Grid Layout (3 欄 on Desktop, 1 欄 on Mobile)
- **卡片組件**: AgentCard
  - Agent 名稱 (Header)
  - 模型 badge
  - 描述 (truncate 100 字)
  - 操作按鈕: View, Edit, Delete
- **參考**: [Agent List Wireframe](../../docs/ux-design/wireframes/low-fidelity/02-agent-list.md)
- **參考**: [Responsive Design](../../docs/ux-design/design-system/design-tokens.md#responsive-breakpoints)

**2. AgentDetailView** (React Component)
- **位置**: `/agents/:id` 頁面
- **Layout**: 單欄詳情佈局
- **顯示內容**:
  - 基本信息欄
  - System Prompt 欄 (只讀)
  - 參數配置欄 (只讀)
  - 操作按鈕欄 (可選操作)
- **參考**: [Agent Detail Wireframe](../../docs/ux-design/wireframes/low-fidelity/04-agent-detail.md)

---

## 第二部分：技術實施方案 (How to Build)

### Backend 實施

**Backend 架構完整文檔**: [Backend Technical Guide](../../docs/technical-implementation/01-backend-net9/README.md)

#### Clean Architecture 目錄結構

**架構設計參考**:
- 🏗️ [ADR-001: Clean Architecture](../../docs/architecture/adr/ADR-001-clean-architecture.md) - 架構分層決策
- 🏗️ [Monorepo Setup](../../docs/technical-implementation/01-backend-net9/01-monorepo-setup.md) - 專案結構

```
SemanticKernelFramework.sln
├── src/
│   ├── Domain/                         # 領域層 (不依賴任何外部層)
│   │   └── Entities/
│   │       └── Agent.cs               # Agent 實體
│   ├── Application/                    # 應用層 (依賴 Domain)
│   │   ├── Agents/
│   │   │   ├── Commands/
│   │   │   │   ├── CreateAgentCommand.cs
│   │   │   │   ├── UpdateAgentCommand.cs
│   │   │   │   └── DeleteAgentCommand.cs
│   │   │   └── Queries/
│   │   │       ├── GetAgentByIdQuery.cs
│   │   │       └── GetAgentsListQuery.cs
│   │   └── Interfaces/
│   │       └── IAgentRepository.cs
│   ├── Infrastructure/                 # 基礎設施層 (依賴 Application 接口)
│   │   ├── Persistence/
│   │   │   ├── ApplicationDbContext.cs
│   │   │   └── Repositories/
│   │   │       └── AgentRepository.cs
│   │   └── Migrations/
│   └── API/                            # API 層 (最外層)
│       └── Controllers/
│           └── AgentsController.cs
```

**分層設計參考**:
- 📐 [Domain Layer Design](../../docs/technical-implementation/01-backend-net9/07-domain-layer.md)
- 📐 [Application Layer Design](../../docs/technical-implementation/01-backend-net9/05-cqrs-implementation.md)
- 📐 [Infrastructure Layer Design](../../docs/technical-implementation/01-backend-net9/04-data-access-layer.md)
- 📐 [API Layer Design](../../docs/technical-implementation/01-backend-net9/08-api-layer.md)

#### 核心實施策略

**架構模式參考**:
- 📚 [Architecture Design Document](../../docs/architecture/Architecture-Design-Document.md) - 完整架構設計

**1. CQRS with MediatR**:
- Commands: 創建, 更新, 刪除 (寫入操作)
- Queries: 查詢 (讀取操作)
- 好處: 關注點分離, 易於測試

**參考**:
- 📐 [ADR-002: CQRS Pattern](../../docs/architecture/adr/ADR-002-cqrs-pattern.md) - CQRS 架構決策
- 📐 [CQRS Implementation](../../docs/technical-implementation/01-backend-net9/05-cqrs-implementation.md) - MediatR 實作指南

**2. Repository Pattern + Unit of Work**:
- AgentRepository: 封裝資料存取
- 好處: 隔離業務邏輯與資料存取實現

**參考**:
- 📐 [ADR-003: Repository Pattern](../../docs/architecture/adr/ADR-003-repository-pattern.md) - Repository 架構決策
- 📐 [Data Access Layer](../../docs/technical-implementation/01-backend-net9/04-data-access-layer.md) - Repository 實作

**3. FluentValidation**:
- 驗證與業務邏輯分離
- 可讀性高, 易於維護

**參考**:
- 📐 [Validation Strategy](../../docs/technical-implementation/01-backend-net9/06-validation-strategy.md) - FluentValidation 實作

**4. Entity Framework Core 8**:
- Code-First Migration
- LINQ 查詢
- 自動追蹤變更

**參考**:
- 📐 [Data Access Layer](../../docs/technical-implementation/01-backend-net9/04-data-access-layer.md#ef-core-configuration)
- 📐 [Migration Strategy](../../docs/technical-implementation/01-backend-net9/04-data-access-layer.md#ef-core-migrations)

### Frontend 實施

**Frontend 架構完整文檔**: [Frontend Technical Guide](../../docs/technical-implementation/02-frontend-react/README.md)

#### 目錄結構

**前端架構參考**:
- 🎨 [Frontend Architecture](../../docs/technical-implementation/02-frontend-react/01-frontend-architecture.md) - React 架構設計
- 🎨 [Project Structure](../../docs/technical-implementation/02-frontend-react/01-frontend-architecture.md#project-structure)

```
web-app/
├── src/
│   ├── pages/                          # 頁面組件
│   │   ├── AgentListPage.tsx
│   │   ├── AgentCreatePage.tsx
│   │   ├── AgentDetailPage.tsx
│   │   └── AgentEditPage.tsx
│   ├── components/                     # 組件庫
│   │   ├── agents/
│   │   │   ├── AgentCreateForm.tsx
│   │   │   ├── AgentCard.tsx
│   │   │   ├── AgentDetailView.tsx
│   │   │   ├── ModelSelector.tsx
│   │   │   ├── PromptEditor.tsx
│   │   │   └── ParameterPanel.tsx
│   │   └── common/
│   │       ├── ConfirmDialog.tsx
│   │       └── LoadingSpinner.tsx
│   ├── hooks/                          # React Hooks
│   │   └── useAgents.ts               # React Query hooks
│   ├── services/                       # API 服務
│   │   └── agentService.ts            # API 調用封裝
│   └── types/                          # TypeScript 類型
│       └── agent.ts                   # TypeScript 類型定義
```

**組件設計參考**:
- 🎨 [Component Design](../../docs/technical-implementation/02-frontend-react/02-component-design.md)
- 🎨 [Component Library](../../docs/ux-design/design-system/component-library.md)

#### 狀態管理策略

**狀態管理完整文檔**: [State Management](../../docs/technical-implementation/02-frontend-react/03-state-management.md)

**1. React Query** (TanStack Query):
- Server State Management
- 自動緩存, 智能重試
- 好處: 減少樣板代碼, 提升 UX

**參考**:
- 🎨 [Data Fetching Strategy](../../docs/technical-implementation/02-frontend-react/05-data-fetching.md) - React Query 實作
- 🎨 [Caching Strategy](../../docs/technical-implementation/02-frontend-react/05-data-fetching.md#caching)

**2. React Hook Form**:
- 表單狀態管理
- 好處: 性能優化, 驗證簡化

**參考**:
- 🎨 [Form Validation](../../docs/technical-implementation/02-frontend-react/04-form-validation.md) - React Hook Form + Yup

### 測試策略

**測試策略完整文檔**: [Testing Strategy](../../docs/testing/testing-strategy.md)

#### Backend 測試

**Backend 測試指南**: [Backend Testing Guidelines](../../docs/testing/unit-testing-guidelines.md)

**1. 單元測試** (45 tests):
- Domain Entity 測試
- Application Service 測試 (CQRS Handlers)
- Validation 測試
- Repository 測試 (Mock)

**參考**:
- 🧪 [Unit Testing Guidelines](../../docs/testing/unit-testing-guidelines.md)
- 🧪 [Mocking Strategy](../../docs/testing/unit-testing-guidelines.md#mocking-strategy)
- 🧪 [Test Coverage Requirements](../../docs/testing/testing-strategy.md#coverage-requirements)

**2. 集成測試** (12 tests):
- API 端點測試
- 資料庫操作測試
- 錯誤處理測試

**參考**:
- 🧪 [Integration Testing Guidelines](../../docs/testing/integration-testing-guidelines.md)
- 🧪 [API Testing Strategy](../../docs/testing/integration-testing-guidelines.md#api-testing)
- 🧪 [Database Testing](../../docs/testing/integration-testing-guidelines.md#database-testing)

**測試工具**:
- xUnit
- Moq (Mocking)
- FluentAssertions

**參考**:
- 🧪 [Testing Tools Setup](../../docs/testing/unit-testing-guidelines.md#testing-tools)

#### Frontend 測試

**Frontend 測試指南**: [Frontend Testing Guidelines](../../docs/testing/testing-strategy.md#frontend-testing)

**1. 組件測試**:
- React Testing Library
- 組件渲染測試
- 用戶互動測試

**參考**:
- 🧪 [Component Testing Guidelines](../../docs/testing/testing-strategy.md#component-testing)
- 🧪 [React Testing Library Best Practices](../../docs/testing/testing-strategy.md#rtl-best-practices)

**2. E2E 測試** (5 tests):
- Playwright
- 完整用戶流程測試

**參考**:
- 🧪 [E2E Testing Guidelines](../../docs/testing/e2e-testing-guidelines.md)
- 🧪 [User Journey Testing](../../docs/testing/e2e-testing-guidelines.md#user-journeys)
- 🧪 [Playwright Configuration](../../docs/testing/e2e-testing-guidelines.md#playwright-setup)

---

## 第三部分：編碼規範

**編碼規範完整文檔**: [Coding Conventions](../../docs/development-standards/coding-conventions.md)

### Backend (.NET 8 / C#)

**Backend 編碼標準**: [.NET Coding Standards](../../docs/development-standards/coding-conventions.md#dotnet)

**命名規範**:
- Pascal Case: Classes, Methods, Properties
- Camel Case: Local variables, parameters
- 接口: `IAgentRepository`

**參考**:
- 📝 [Naming Conventions](../../docs/development-standards/coding-conventions.md#naming-conventions)
- 📝 [C# Style Guide](../../docs/development-standards/coding-conventions.md#csharp-style)

**SOLID 原則**:
- 單一職責: 每個類別只有一個職責
- 開放封閉: 開放擴展, 封閉修改
- 里氏替換: 派生類別可以替換基類

**參考**:
- 📝 [SOLID Principles](../../docs/development-standards/code-quality-standards.md#solid-principles)
- 📝 [Design Patterns](../../docs/development-standards/code-quality-standards.md#design-patterns)

**Clean Architecture**:
- Domain 層: 不依賴任何外部層
- Application 層: 依賴 Domain
- Infrastructure 層: 依賴 Application 接口
- API 層: 最外層, 處理 HTTP 請求/響應

**參考**:
- 🏗️ [ADR-001: Clean Architecture](../../docs/architecture/adr/ADR-001-clean-architecture.md)
- 🏗️ [Dependency Rules](../../docs/architecture/adr/ADR-001-clean-architecture.md#dependency-rules)

### Frontend (React 18 / TypeScript)

**Frontend 編碼標準**: [React/TypeScript Coding Standards](../../docs/development-standards/coding-conventions.md#react-typescript)

**命名規範**:
- Pascal Case: Components, Types, Interfaces
- Camel Case: Functions, variables
- 組件: 組件名稱與檔案名稱一致

**參考**:
- 📝 [React Naming Conventions](../../docs/development-standards/coding-conventions.md#react-naming)
- 📝 [TypeScript Style Guide](../../docs/development-standards/coding-conventions.md#typescript-style)

**組件設計原則**:
- 單一職責: 每個組件專注一個功能
- Props 驗證: 使用 TypeScript types
- 可讀性: 優先可讀性勝過簡潔性

**參考**:
- 🎨 [Component Design Principles](../../docs/technical-implementation/02-frontend-react/02-component-design.md#design-principles)
- 🎨 [TypeScript Best Practices](../../docs/development-standards/coding-conventions.md#typescript-best-practices)

**Hooks 規範**:
- 自訂 Hook 以 `use` 開頭
- 不在條件語句中使用 Hooks
- 遵循 React Hooks 規則

**參考**:
- 🎨 [React Hooks Guidelines](../../docs/technical-implementation/02-frontend-react/01-frontend-architecture.md#hooks-guidelines)
- 🎨 [Custom Hooks Best Practices](../../docs/technical-implementation/02-frontend-react/01-frontend-architecture.md#custom-hooks)

---

## 第四部分：質量保證

### Code Review

**Code Review 完整指南**: [Code Review Checklist](../../docs/development-standards/code-review-checklist.md)

**Code Review 重點**:
- 功能正確性
- 架構一致性
- 代碼可讀性
- 測試覆蓋率
- 安全性檢查

**參考**:
- ✅ [Code Review Checklist](../../docs/development-standards/code-review-checklist.md)
- ✅ [Code Review Process](../../docs/development-standards/git-workflow.md#pull-request-process)

### Definition of Done

**完成定義文檔**: [Definition of Done](../../docs/project-management/definition-of-done.md)

**Sprint 1 Definition of Done**:
- ✅ 所有驗收標準通過
- ✅ 單元測試覆蓋率 ≥ 80%
- ✅ 集成測試通過
- ✅ Code Review 通過
- ✅ 文檔更新完成
- ✅ 部署到 Development 環境

**參考**:
- ✅ [Definition of Done](../../docs/project-management/definition-of-done.md)
- ✅ [Acceptance Criteria](../../docs/user-stories/us-1-agent-crud.md#acceptance-criteria)

### 技術債務管理

**技術債務管理指南**: [Technical Debt Management](../../docs/development-standards/technical-debt-management.md)

**技術債務追蹤**:
- 記錄所有已知技術債務
- 評估債務影響和優先級
- 規劃還債時程

**參考**:
- 📝 [Technical Debt Management](../../docs/development-standards/technical-debt-management.md)
- 📝 [Continuous Improvement Log](../../4-changes/CONTINUOUS-IMPROVEMENT-LOG.md)

---

## 第五部分：參考文檔

### 核心規劃文檔 (6 refs)
1. [MVP-SCOPE-DEFINITION.md](../../1-planning/MVP-SCOPE-DEFINITION.md) - Phase 1A 詳細範圍
2. [SPRINT-ALLOCATION-ANALYSIS.md](../../1-planning/SPRINT-ALLOCATION-ANALYSIS.md) - Sprint 1 詳細分析
3. [DEVELOPMENT-STRATEGY.md](../../1-planning/DEVELOPMENT-STRATEGY.md) - Phase 1A 開發策略
4. [DEPENDENCY-MATRIX.md](../../1-planning/DEPENDENCY-MATRIX.md) - 依賴關係追蹤
5. [RISK-REGISTER.md](../../1-planning/RISK-REGISTER.md) - 風險管理
6. [TECHNICAL-DECISIONS-LOG.md](../../1-planning/TECHNICAL-DECISIONS-LOG.md) - 技術決策日誌

### 架構設計文檔 - ADRs (5 refs)
7. [Architecture-Design-Document.md](../../docs/architecture/Architecture-Design-Document.md) - 完整系統架構
8. [ADR-001: Clean Architecture](../../docs/architecture/adr/ADR-001-clean-architecture.md) - 架構分層決策
9. [ADR-002: CQRS Pattern](../../docs/architecture/adr/ADR-002-cqrs-pattern.md) - CQRS 架構決策
10. [ADR-003: Repository Pattern](../../docs/architecture/adr/ADR-003-repository-pattern.md) - Repository 架構決策
11. [ADR-004: PostgreSQL Selection](../../docs/architecture/adr/ADR-004-database-selection.md) - 資料庫選型決策

### User Stories (2 refs)
12. [User Stories Overview](../../docs/user-stories/README.md) - User Story 總覽
13. [US 1.1-1.3 Complete Spec](../../docs/user-stories/us-1-agent-crud.md) - Agent CRUD 完整規格

### Technical Implementation - Backend (9 refs)
14. [Backend Architecture](../../docs/technical-implementation/01-backend-net9/README.md)
15. [Monorepo Setup](../../docs/technical-implementation/01-backend-net9/01-monorepo-setup.md)
16. [Semantic Kernel Integration](../../docs/technical-implementation/01-backend-net9/02-semantic-kernel-integration.md)
17. [Agent SDK](../../docs/technical-implementation/01-backend-net9/03-agent-sdk.md)
18. [Data Access Layer](../../docs/technical-implementation/01-backend-net9/04-data-access-layer.md)
19. [CQRS Implementation](../../docs/technical-implementation/01-backend-net9/05-cqrs-implementation.md)
20. [Validation Strategy](../../docs/technical-implementation/01-backend-net9/06-validation-strategy.md)
21. [Domain Layer](../../docs/technical-implementation/01-backend-net9/07-domain-layer.md)
22. [API Layer](../../docs/technical-implementation/01-backend-net9/08-api-layer.md)

### Technical Implementation - Frontend (6 refs)
23. [Frontend Architecture](../../docs/technical-implementation/02-frontend-react/01-frontend-architecture.md)
24. [Component Design](../../docs/technical-implementation/02-frontend-react/02-component-design.md)
25. [State Management](../../docs/technical-implementation/02-frontend-react/03-state-management.md)
26. [Form Validation](../../docs/technical-implementation/02-frontend-react/04-form-validation.md)
27. [Data Fetching Strategy](../../docs/technical-implementation/02-frontend-react/05-data-fetching.md)
28. [API Integration](../../docs/technical-implementation/02-frontend-react/06-api-integration.md)

### UX Design Documents (5 refs)
29. [Design System](../../docs/ux-design/design-system/README.md)
30. [Component Library](../../docs/ux-design/design-system/component-library.md)
31. [Agent List Wireframe](../../docs/ux-design/wireframes/low-fidelity/02-agent-list.md)
32. [Agent Create Wireframe](../../docs/ux-design/wireframes/low-fidelity/03-agent-create.md)
33. [Agent Detail Wireframe](../../docs/ux-design/wireframes/low-fidelity/04-agent-detail.md)

### API & Database Design (2 refs)
34. [API Design Specification](../../docs/api/api-design.md)
35. [Database Schema Design](../../docs/api/database-schema.md)

### Testing Documentation (7 refs)
36. [Testing Strategy](../../docs/testing/testing-strategy.md)
37. [Unit Testing Guidelines](../../docs/testing/unit-testing-guidelines.md)
38. [Integration Testing Guidelines](../../docs/testing/integration-testing-guidelines.md)
39. [E2E Testing Guidelines](../../docs/testing/e2e-testing-guidelines.md)
40. [SDK Testing Guidelines](../../docs/testing/sdk-testing-guidelines.md)
41. [QA Standards](../../docs/testing/qa-standards.md)
42. [TDD Best Practices](../../docs/testing/tdd-best-practices.md)

### Development Standards (6 refs)
43. [Code Quality Standards](../../docs/development-standards/code-quality-standards.md)
44. [Coding Conventions](../../docs/development-standards/coding-conventions.md)
45. [Code Review Checklist](../../docs/development-standards/code-review-checklist.md)
46. [Git Workflow](../../docs/development-standards/git-workflow.md)
47. [Documentation Standards](../../docs/development-standards/documentation-standards.md)
48. [Technical Debt Management](../../docs/development-standards/technical-debt-management.md)

### Project Management (3 refs)
49. [Sprint Planning Guide](../../docs/project-management/sprint-planning-guide.md)
50. [Definition of Done](../../docs/project-management/definition-of-done.md)
51. [Agile Practices](../../docs/project-management/agile-practices.md)

### Sprint 1 文檔系列 (6 refs)
52. [SPRINT-1-OVERVIEW.md](./SPRINT-1-OVERVIEW.md) - Sprint 1 概覽
53. [SPRINT-1-CONTEXT.md](./SPRINT-1-CONTEXT.md) - Sprint 1 背景與技術決策
54. [SPRINT-1-CHECKLIST.md](./SPRINT-1-CHECKLIST.md) - Sprint 1 檢查清單
55. [SPRINT-1-DEV-LOG.md](./SPRINT-1-DEV-LOG.md) - Sprint 1 開發日誌
56. [SPRINT-1-ISSUES.md](./SPRINT-1-ISSUES.md) - Sprint 1 問題追蹤
57. [SPRINT-1-RETROSPECTIVE.md](./SPRINT-1-RETROSPECTIVE.md) - Sprint 1 回顧

**總計**: 57+ 個參考文檔連結，涵蓋規劃、架構、實作、測試、開發標準、專案管理等所有面向。

---

## 📋 更新日誌

| 版本 | 日期 | 變更內容 | 負責人 |
|------|------|---------|--------|
| 1.0 | 2025-11-04 | Sprint 1 計劃書初版 | Sprint Team |
| 2.0 | 2025-11-12 | 添加 57+ 個 `/docs` 參考文獻，增強技術規格可追溯性與實施指導 | Documentation Team |

**v2.0 改進摘要**:
- ✅ 添加 57+ 個技術參考文獻，覆蓋所有關鍵技術領域
- ✅ 增強所有 User Story 的技術規格與設計參考
- ✅ 整合 Planning 文檔參考 (MVP Scope, Sprint Allocation, etc.)
- ✅ 整合架構決策參考 (5 個 ADR)
- ✅ 整合完整技術實作參考 (Backend 9 refs, Frontend 6 refs)
- ✅ 整合 UX 設計參考 (5 個線框圖與設計系統文檔)
- ✅ 整合測試策略參考 (7 個測試文檔)
- ✅ 整合開發標準參考 (6 個標準文檔)
- ✅ 新增質量保證章節 (Code Review, Definition of Done, Technical Debt)
- ✅ 建立完整參考文獻索引 (9 個分類)

---

**創建日期**: 2025-11-04 (Sprint 1 開始)
**完成日期**: 2025-11-22 (Sprint 1 結束)
**維護說明**: 本文檔為 Sprint 1 的技術計劃書，v2.0 版本已建立完整的文檔追溯系統，提供 57+ 個技術參考連結，支持深度技術查詢與實施指導。
