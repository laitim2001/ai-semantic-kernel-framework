# Sprint 1 開發上下文

**版本**: 1.0
**目的**: 提供 Sprint 1 開發所需的完整技術參考和實施上下文
**使用場景**: Just-in-Time 技術查詢 - 開發者執行 US 1.1-1.3 時的技術指南
**Sprint 週期**: 第 1-3 週 (2025-11-04 ~ 2025-11-22)
**Sprint 定位**: Phase 1A - 基礎平台 (13 SP, 3 週)

**文檔導航**:
- 📋 [Sprint 1 Overview](./SPRINT-1-OVERVIEW.md) - Sprint 1 概覽與成果總結
- 📋 [Sprint 1 Plan](./SPRINT-1-PLAN.md) - 詳細計劃與任務分解
- 📊 [Sprint Allocation Analysis](../../1-planning/SPRINT-ALLOCATION-ANALYSIS.md#sprint-1) - Sprint 1 詳細分析

---

## 📖 使用說明 (How to Use This Document)

**本文檔定位**: "Just-in-Time" 技術參考手冊

**適用場景**:
- ✅ 開發 US 1.1 時，需要查詢 API 設計細節 → 第二章
- ✅ 開發 US 1.2 時，需要查詢 SDK Builder Pattern → 第三章
- ✅ 開發 US 1.3 時，需要查詢 CRUD 實施策略 → 第四章
- ✅ 需要快速找到架構設計文檔 → 第一章 Reference Layer
- ✅ 需要理解技術決策背景 → 各章節的「關鍵技術決策」部分

**不適用場景**:
- ❌ 尋找 Sprint 整體進度與成果 → 使用 [SPRINT-1-OVERVIEW.md](./SPRINT-1-OVERVIEW.md)
- ❌ 尋找詳細開發計劃與任務清單 → 使用 [SPRINT-1-PLAN.md](./SPRINT-1-PLAN.md)

---

## 一、核心技術參考層 (Reference Layer)

### 1.1 Module & Epic 引用

**User Story 完整規格**:
- 📖 [Module 01: Agent Creation](../../docs/user-stories/modules/module-01-agent-creation.md) - Agent 管理系統完整規格
  - **Section**: [US 1.1 - Web UI Agent Creation](../../docs/user-stories/modules/module-01-agent-creation.md#us-11---通過-web-ui-建立-agent) - 驗收標準與技術要求
  - **Section**: [US 1.2 - .NET SDK Agent Creation](../../docs/user-stories/modules/module-01-agent-creation.md#us-12---通過-net-sdk-建立-agent) - SDK 範例代碼
  - **Section**: [US 1.3 - Agent Configuration Management](../../docs/user-stories/modules/module-01-agent-creation.md#us-13---agent-配置管理) - CRUD 功能規格

**Epic 文檔**:
- 📖 [Epic 01: Agent Lifecycle Management](../../docs/user-stories/epics/epic-01.md) - Agent 生命週期管理策略
  - **用途**: 理解 Agent CRUD 在整體系統中的定位
  - **關鍵內容**: BDD 場景、驗收標準、依賴關係

### 1.2 架構設計引用

**核心架構決策記錄 (ADR)**:

**1. Clean Architecture** - [ADR-001](../../docs/architecture/adr/ADR-001-clean-architecture.md)
- **決策**: 採用 4 層架構 (API → Application → Infrastructure → Domain)
- **關鍵原則**: 依賴方向由外向內，Domain 層不依賴任何外部層
- **Section**: [Dependency Rules](../../docs/architecture/adr/ADR-001-clean-architecture.md#dependency-rules) - 依賴規則詳解
- **Section**: [Layer Responsibilities](../../docs/architecture/adr/ADR-001-clean-architecture.md#layer-responsibilities) - 各層職責定義
- **實施影響**: US 1.1-1.3 所有代碼必須遵循此架構分層

**2. CQRS Pattern** - [ADR-002](../../docs/architecture/adr/ADR-002-cqrs-pattern.md)
- **決策**: 使用 MediatR 實施命令查詢職責分離
- **關鍵設計**: Commands (CreateAgent, UpdateAgent, DeleteAgent) vs Queries (GetAgent, ListAgents)
- **Section**: [MediatR Integration](../../docs/architecture/adr/ADR-002-cqrs-pattern.md#mediatr-integration) - MediatR 配置與使用
- **Section**: [Command Handlers](../../docs/architecture/adr/ADR-002-cqrs-pattern.md#command-handlers) - Command Handler 實作模式
- **實施影響**: US 1.1-1.3 所有業務邏輯透過 CQRS 實施

**3. Repository Pattern** - [ADR-003](../../docs/architecture/adr/ADR-003-repository-pattern.md)
- **決策**: 使用 Repository Pattern 抽象資料存取
- **關鍵設計**: IAgentRepository 接口 + AgentRepository 實作
- **Section**: [Generic Repository](../../docs/architecture/adr/ADR-003-repository-pattern.md#generic-repository) - 泛型 Repository 設計
- **Section**: [Unit of Work](../../docs/architecture/adr/ADR-003-repository-pattern.md#unit-of-work) - 工作單元模式
- **實施影響**: US 1.3 的 CRUD 操作透過 Repository 實施

**4. Database Selection** - [ADR-004](../../docs/architecture/adr/ADR-004-database-selection.md)
- **決策**: 選擇 PostgreSQL 16 作為主要資料庫
- **關鍵考量**: ACID 保證、JSON 支援、全文搜尋、開源生態
- **Section**: [Indexing Strategy](../../docs/architecture/adr/ADR-004-database-selection.md#indexing-strategy) - 索引策略
- **Section**: [Migration Strategy](../../docs/architecture/adr/ADR-004-database-selection.md#migration-strategy) - 遷移管理
- **實施影響**: US 1.1 的 Database Schema 設計基於 PostgreSQL 特性

**完整架構設計文檔**:
- 🏗️ [Architecture Design Document](../../docs/architecture/Architecture-Design-Document.md) - 系統架構總覽
  - **Section**: [System Architecture](../../docs/architecture/Architecture-Design-Document.md#system-architecture) - 6 層架構圖
  - **Section**: [Component Diagram](../../docs/architecture/Architecture-Design-Document.md#component-diagram) - 組件關係圖
  - **Section**: [Data Flow](../../docs/architecture/Architecture-Design-Document.md#data-flow) - 數據流向設計

### 1.3 API 設計引用

**API 設計規範**:
- 📄 [API Design Specification](../../docs/api/api-design.md) - RESTful API 完整設計規範
  - **Section**: [Agent CRUD Endpoints](../../docs/api/api-design.md#agent-endpoints) - Agent API 端點詳細設計
    - `POST /api/v1/agents` - 創建 Agent (US 1.1, 1.2)
    - `GET /api/v1/agents` - 查詢 Agent 列表 (US 1.3)
    - `GET /api/v1/agents/{id}` - 獲取 Agent 詳情 (US 1.3)
    - `PUT /api/v1/agents/{id}` - 更新 Agent (US 1.3)
    - `DELETE /api/v1/agents/{id}` - 刪除 Agent (US 1.3)

  - **Section**: [Request/Response Format](../../docs/api/api-design.md#request-response-format) - 標準格式定義
  - **Section**: [Error Handling](../../docs/api/api-design.md#error-handling) - 統一錯誤處理策略
  - **Section**: [Validation Rules](../../docs/api/api-design.md#validation-rules) - API 驗證規則
  - **Section**: [Pagination Design](../../docs/api/api-design.md#pagination) - 分頁設計 (US 1.3)
  - **Section**: [Search & Filter](../../docs/api/api-design.md#search-filter) - 搜尋篩選設計 (US 1.3)
  - **Section**: [API Versioning](../../docs/api/api-design.md#versioning) - API 版本控制
  - **Section**: [HTTP Status Codes](../../docs/api/api-design.md#http-status-codes) - 狀態碼使用規範

**Database Schema 設計**:
- 🗄️ [Database Schema Design](../../docs/api/database-schema.md) - 完整資料庫 Schema 設計
  - **Section**: [Agents Table](../../docs/api/database-schema.md#agents-table) - agents 表結構定義
  - **Section**: [Indexing Strategy](../../docs/api/database-schema.md#indexing-strategy) - 索引設計策略
  - **Section**: [Soft Delete Pattern](../../docs/api/database-schema.md#soft-delete-pattern) - 軟刪除實施
  - **Section**: [Audit Fields](../../docs/api/database-schema.md#audit-fields) - 審計欄位設計

### 1.4 UI/UX 設計引用

**線框圖設計 (Low-Fidelity Wireframes)**:
- 🎨 [Agent List Wireframe](../../docs/ux-design/wireframes/low-fidelity/02-agent-list.md) - Agent 列表頁面設計 (US 1.3)
  - **Section**: [Desktop Layout](../../docs/ux-design/wireframes/low-fidelity/02-agent-list.md#desktop-layout) - 桌面版佈局
  - **Section**: [Mobile Layout](../../docs/ux-design/wireframes/low-fidelity/02-agent-list.md#mobile-layout) - 移動版佈局
  - **Section**: [Agent Card Component](../../docs/ux-design/wireframes/low-fidelity/02-agent-list.md#agent-card-component) - Agent 卡片設計
  - **Section**: [Search & Filter UI](../../docs/ux-design/wireframes/low-fidelity/02-agent-list.md#search-filter-ui) - 搜尋篩選界面

- 🎨 [Agent Create Wireframe](../../docs/ux-design/wireframes/low-fidelity/03-agent-create.md) - Agent 創建頁面設計 (US 1.1)
  - **Section**: [Form Layout](../../docs/ux-design/wireframes/low-fidelity/03-agent-create.md#form-layout) - 表單佈局設計
  - **Section**: [Model Selector](../../docs/ux-design/wireframes/low-fidelity/03-agent-create.md#model-selector) - 模型選擇器
  - **Section**: [Prompt Editor](../../docs/ux-design/wireframes/low-fidelity/03-agent-create.md#prompt-editor) - Prompt 編輯器
  - **Section**: [Parameter Controls](../../docs/ux-design/wireframes/low-fidelity/03-agent-create.md#parameter-controls) - 參數控制面板

- 🎨 [Agent Detail Wireframe](../../docs/ux-design/wireframes/low-fidelity/04-agent-detail.md) - Agent 詳情頁面設計 (US 1.3)
  - **Section**: [Detail View Layout](../../docs/ux-design/wireframes/low-fidelity/04-agent-detail.md#detail-view-layout) - 詳情頁佈局
  - **Section**: [Action Buttons](../../docs/ux-design/wireframes/low-fidelity/04-agent-detail.md#action-buttons) - 操作按鈕設計

**Design System 規範**:
- 🎨 [Design System](../../docs/ux-design/design-system/README.md) - Material-UI 設計系統規範
  - **Section**: [Design Tokens](../../docs/ux-design/design-system/design-tokens.md) - 設計令牌 (顏色、字體、間距)
  - **Section**: [Component Library](../../docs/ux-design/design-system/component-library.md) - Material-UI 組件使用規範
  - **Section**: [Typography](../../docs/ux-design/design-system/typography.md) - 字體系統
  - **Section**: [Color Palette](../../docs/ux-design/design-system/color-palette.md) - 色彩系統
  - **Section**: [Responsive Breakpoints](../../docs/ux-design/design-system/design-tokens.md#responsive-breakpoints) - 響應式斷點

### 1.5 測試策略引用

**測試策略文檔**:
- 🧪 [Testing Strategy](../../docs/testing/testing-strategy.md) - 完整測試策略
  - **Section**: [Coverage Requirements](../../docs/testing/testing-strategy.md#coverage-requirements) - 覆蓋率要求 (≥80%)
  - **Section**: [Testing Pyramid](../../docs/testing/testing-strategy.md#testing-pyramid) - 測試金字塔
  - **Section**: [Frontend Testing](../../docs/testing/testing-strategy.md#frontend-testing) - 前端測試策略
  - **Section**: [Backend Testing](../../docs/testing/testing-strategy.md#backend-testing) - 後端測試策略

**詳細測試指南**:
- 🧪 [Unit Testing Guidelines](../../docs/testing/unit-testing-guidelines.md) - 單元測試規範
  - **Section**: [Testing Tools](../../docs/testing/unit-testing-guidelines.md#testing-tools) - xUnit, Moq, FluentAssertions
  - **Section**: [Mocking Strategy](../../docs/testing/unit-testing-guidelines.md#mocking-strategy) - Mock 使用指南
  - **Section**: [Test Naming](../../docs/testing/unit-testing-guidelines.md#test-naming) - 測試命名規範

- 🧪 [Integration Testing Guidelines](../../docs/testing/integration-testing-guidelines.md) - 集成測試規範
  - **Section**: [API Testing](../../docs/testing/integration-testing-guidelines.md#api-testing) - API 測試策略
  - **Section**: [Database Testing](../../docs/testing/integration-testing-guidelines.md#database-testing) - 資料庫測試方法
  - **Section**: [Test Data Management](../../docs/testing/integration-testing-guidelines.md#test-data-management) - 測試數據管理

- 🧪 [E2E Testing Guidelines](../../docs/testing/e2e-testing-guidelines.md) - E2E 測試規範
  - **Section**: [User Journey Testing](../../docs/testing/e2e-testing-guidelines.md#user-journeys) - 用戶旅程測試
  - **Section**: [Playwright Setup](../../docs/testing/e2e-testing-guidelines.md#playwright-setup) - Playwright 配置

---

## 二、US 1.1: 透過 Web UI 創建 Agent - 詳細技術上下文

### 2.1 MVP 範圍定義與技術決策

**完整 User Story 規格**: [US 1.1 - Web UI Agent Creation](../../docs/user-stories/modules/module-01-agent-creation.md#us-11)

**MVP 範圍邊界**:

✅ **本 Sprint 必須實現 (P0)**:
1. **Agent 創建表單** - AgentCreateForm.tsx 組件
   - 基本資訊: Agent 名稱 (3-100 字符), 描述 (可選), 角色類型 (下拉選單)
   - LLM 模型選擇: GPT-4, GPT-4o, GPT-4o-mini (預設 GPT-4o)
   - System Prompt 編輯器: 多行文本輸入 (10-4000 字符)
   - 模型參數控制: Temperature (0-2, 步進 0.1), Max Tokens (1-4096), Top P (0-1, 步進 0.1)
   - 表單提交與驗證: 客戶端驗證 + API 調用
   - **參考**: [Agent Create Wireframe](../../docs/ux-design/wireframes/low-fidelity/03-agent-create.md)

2. **表單驗證策略**
   - 客戶端驗證: React Hook Form + Yup Schema
   - 服務端驗證: FluentValidation
   - **參考**: [Form Validation Strategy](../../docs/technical-implementation/02-frontend-react/04-form-validation.md)

3. **API 調用與錯誤處理**
   - 調用 `POST /api/v1/agents`
   - Loading 狀態顯示
   - 成功: 導航到 Agent 詳情頁
   - 失敗: 顯示錯誤訊息 (400/409/500)
   - **參考**: [API Integration](../../docs/technical-implementation/02-frontend-react/06-api-integration.md)

❌ **明確排除 Phase 2 功能 (延後到 Sprint 2-5)**:
- Plugin 配置界面 (US 2.x) - Sprint 2-3 實現
- 高級參數設定 (Frequency Penalty, Presence Penalty) - US 1.4 Phase 2
- Persona 選擇器 (US 1.5) - Sprint 4-5 實現
- 批量創建功能 (US 1.x Extended) - Phase 2

**關鍵技術決策**:

**決策 1: 表單狀態管理**
- **選擇**: React Hook Form + Yup
- **理由**: 性能優化 (非受控組件), 內建驗證整合, 減少重新渲染
- **替代方案**: Formik (較重), 純 React State (樣板代碼多)
- **參考**: [Form Validation](../../docs/technical-implementation/02-frontend-react/04-form-validation.md#react-hook-form)

**決策 2: UI 組件庫**
- **選擇**: Material-UI v5
- **理由**: 成熟生態、完整主題系統、無障礙支援
- **替代方案**: Ant Design (較不符合 Material Design 規範)
- **參考**: [Component Library](../../docs/ux-design/design-system/component-library.md)

### 2.2 Backend API 實施細節

**API 設計完整文檔**: [API Design Specification](../../docs/api/api-design.md)

**API 端點**: `POST /api/v1/agents`

**Request Schema**:
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

**驗證規則詳解** ([Validation Strategy](../../docs/technical-implementation/01-backend-net9/06-validation-strategy.md)):

```csharp
public class CreateAgentCommandValidator : AbstractValidator<CreateAgentCommand>
{
    public CreateAgentCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Agent name is required")
            .Length(3, 100).WithMessage("Name must be 3-100 characters")
            .Matches("^[a-zA-Z0-9 _-]+$").WithMessage("Name contains invalid characters");

        RuleFor(x => x.SystemPrompt)
            .NotEmpty().WithMessage("System prompt is required")
            .Length(10, 4000).WithMessage("Prompt must be 10-4000 characters");

        RuleFor(x => x.Model)
            .NotEmpty()
            .Must(BeValidModel).WithMessage("Invalid model selection");

        RuleFor(x => x.Parameters.Temperature)
            .InclusiveBetween(0.0, 2.0).When(x => x.Parameters != null);

        RuleFor(x => x.Parameters.MaxTokens)
            .InclusiveBetween(1, 4096).When(x => x.Parameters != null);

        RuleFor(x => x.Parameters.TopP)
            .InclusiveBetween(0.0, 1.0).When(x => x.Parameters != null);
    }

    private bool BeValidModel(string model)
    {
        var validModels = new[] { "gpt-4", "gpt-4o", "gpt-4o-mini" };
        return validModels.Contains(model);
    }
}
```

**CQRS Command Handler 實施** ([CQRS Implementation](../../docs/technical-implementation/01-backend-net9/05-cqrs-implementation.md)):

```csharp
public class CreateAgentCommandHandler : IRequestHandler<CreateAgentCommand, Result<AgentDto>>
{
    private readonly IAgentRepository _repository;
    private readonly IMapper _mapper;

    public async Task<Result<AgentDto>> Handle(
        CreateAgentCommand request,
        CancellationToken cancellationToken)
    {
        // 1. 檢查名稱唯一性
        var exists = await _repository.ExistsByNameAsync(request.Name);
        if (exists)
            return Result<AgentDto>.Failure("Agent name already exists");

        // 2. 創建 Domain Entity
        var agent = Agent.Create(
            name: request.Name,
            description: request.Description,
            role: request.Role,
            systemPrompt: request.SystemPrompt,
            model: request.Model,
            parameters: request.Parameters
        );

        // 3. 保存到資料庫
        await _repository.AddAsync(agent);
        await _repository.SaveChangesAsync(cancellationToken);

        // 4. 返回 DTO
        var dto = _mapper.Map<AgentDto>(agent);
        return Result<AgentDto>.Success(dto);
    }
}
```

**錯誤處理策略** ([Error Handling](../../docs/api/api-design.md#error-handling)):

- `400 Bad Request`: 驗證失敗，返回詳細錯誤字段
  ```json
  {
    "type": "ValidationError",
    "title": "Validation failed",
    "status": 400,
    "errors": {
      "Name": ["Name must be 3-100 characters"],
      "SystemPrompt": ["System prompt is required"]
    }
  }
  ```

- `409 Conflict`: Agent 名稱重複
  ```json
  {
    "type": "ConflictError",
    "title": "Agent name already exists",
    "status": 409,
    "detail": "An agent with name 'Customer Support Agent' already exists"
  }
  ```

- `500 Internal Server Error`: 服務器異常
  ```json
  {
    "type": "InternalError",
    "title": "An error occurred",
    "status": 500,
    "detail": "An unexpected error occurred. Please try again later."
  }
  ```

### 2.3 Database Schema 實施細節

**Database 設計完整文檔**: [Database Schema Design](../../docs/api/database-schema.md)

**agents 表完整定義**:

```sql
CREATE TABLE agents (
    -- Primary Key
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    -- 基本資訊
    name VARCHAR(100) NOT NULL UNIQUE,
    description TEXT,
    role VARCHAR(50) NOT NULL,

    -- LLM 配置
    system_prompt TEXT NOT NULL,
    model VARCHAR(50) NOT NULL,

    -- 模型參數
    temperature DECIMAL(3,2) DEFAULT 0.7,
    max_tokens INTEGER DEFAULT 2000,
    top_p DECIMAL(3,2) DEFAULT 1.0,

    -- 狀態管理
    is_active BOOLEAN DEFAULT true,
    is_deleted BOOLEAN DEFAULT false,

    -- 審計欄位
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP NOT NULL DEFAULT NOW(),
    created_by VARCHAR(100),
    updated_by VARCHAR(100),

    -- 約束
    CONSTRAINT chk_temperature CHECK (temperature >= 0.0 AND temperature <= 2.0),
    CONSTRAINT chk_max_tokens CHECK (max_tokens >= 1 AND max_tokens <= 4096),
    CONSTRAINT chk_top_p CHECK (top_p >= 0.0 AND top_p <= 1.0),
    CONSTRAINT chk_model CHECK (model IN ('gpt-4', 'gpt-4o', 'gpt-4o-mini'))
);
```

**索引策略** ([Indexing Strategy](../../docs/api/database-schema.md#indexing-strategy)):

```sql
-- 1. 名稱唯一索引 (自動創建，因為 UNIQUE 約束)
CREATE UNIQUE INDEX idx_agents_name ON agents(name);

-- 2. 活躍狀態索引 (常用於列表查詢篩選)
CREATE INDEX idx_agents_is_active ON agents(is_active) WHERE is_deleted = false;

-- 3. 軟刪除索引 (常用於過濾已刪除記錄)
CREATE INDEX idx_agents_is_deleted ON agents(is_deleted);

-- 4. 創建時間索引 (用於排序和時間範圍查詢)
CREATE INDEX idx_agents_created_at ON agents(created_at DESC);

-- 5. 複合索引 (角色 + 活躍狀態，用於篩選查詢)
CREATE INDEX idx_agents_role_active ON agents(role, is_active) WHERE is_deleted = false;
```

**EF Core Entity 配置** ([Data Access Layer](../../docs/technical-implementation/01-backend-net9/04-data-access-layer.md)):

```csharp
public class AgentConfiguration : IEntityTypeConfiguration<Agent>
{
    public void Configure(EntityTypeBuilder<Agent> builder)
    {
        builder.ToTable("agents");

        // Primary Key
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        // Properties
        builder.Property(a => a.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.Description)
            .HasColumnName("description");

        builder.Property(a => a.SystemPrompt)
            .HasColumnName("system_prompt")
            .IsRequired();

        builder.Property(a => a.Model)
            .HasColumnName("model")
            .HasMaxLength(50)
            .IsRequired();

        // Indexes
        builder.HasIndex(a => a.Name)
            .IsUnique()
            .HasDatabaseName("idx_agents_name");

        builder.HasIndex(a => a.IsActive)
            .HasDatabaseName("idx_agents_is_active");

        // Global Query Filter (軟刪除)
        builder.HasQueryFilter(a => !a.IsDeleted);
    }
}
```

**軟刪除實施** ([Soft Delete Pattern](../../docs/api/database-schema.md#soft-delete-pattern)):
- 刪除操作: 設置 `is_deleted = true`, `updated_at = NOW()`
- 查詢過濾: EF Core Global Query Filter 自動排除已刪除記錄
- 恢復功能: Phase 2 實現 (US 1.x Extended)

### 2.4 Frontend UI 實施細節

**Frontend 架構完整文檔**: [Frontend Architecture](../../docs/technical-implementation/02-frontend-react/01-frontend-architecture.md)

**AgentCreateForm 組件設計** ([Component Design](../../docs/technical-implementation/02-frontend-react/02-component-design.md)):

```typescript
// types/agent.ts
export interface AgentCreateDto {
  name: string;
  description?: string;
  role: AgentRole;
  model: LLMModel;
  systemPrompt: string;
  parameters: {
    temperature: number;
    maxTokens: number;
    topP: number;
  };
}

export enum AgentRole {
  CustomerService = 'CustomerService',
  DataAnalyst = 'DataAnalyst',
  Developer = 'Developer',
  General = 'General'
}

export enum LLMModel {
  GPT4 = 'gpt-4',
  GPT4o = 'gpt-4o',
  GPT4oMini = 'gpt-4o-mini'
}

// components/agents/AgentCreateForm.tsx
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';

const schema = yup.object().shape({
  name: yup.string()
    .required('Agent name is required')
    .min(3, 'Name must be at least 3 characters')
    .max(100, 'Name must not exceed 100 characters'),
  description: yup.string(),
  role: yup.string().required('Role is required'),
  model: yup.string().required('Model is required'),
  systemPrompt: yup.string()
    .required('System prompt is required')
    .min(10, 'Prompt must be at least 10 characters')
    .max(4000, 'Prompt must not exceed 4000 characters'),
  parameters: yup.object().shape({
    temperature: yup.number().min(0).max(2).required(),
    maxTokens: yup.number().min(1).max(4096).required(),
    topP: yup.number().min(0).max(1).required()
  })
});

export const AgentCreateForm: React.FC = () => {
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting }
  } = useForm<AgentCreateDto>({
    resolver: yupResolver(schema),
    defaultValues: {
      model: LLMModel.GPT4o,
      parameters: {
        temperature: 0.7,
        maxTokens: 2000,
        topP: 1.0
      }
    }
  });

  const createAgentMutation = useCreateAgent();

  const onSubmit = async (data: AgentCreateDto) => {
    try {
      const agent = await createAgentMutation.mutateAsync(data);
      navigate(`/agents/${agent.id}`);
    } catch (error) {
      // Error handling
    }
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      {/* Form fields */}
    </form>
  );
};
```

**React Query Hook 實施** ([Data Fetching Strategy](../../docs/technical-implementation/02-frontend-react/05-data-fetching.md)):

```typescript
// hooks/useAgents.ts
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { agentService } from '@/services/agentService';

export const useCreateAgent = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: AgentCreateDto) => agentService.create(data),
    onSuccess: (newAgent) => {
      // 更新緩存
      queryClient.invalidateQueries({ queryKey: ['agents'] });

      // 可選: 直接添加到緩存
      queryClient.setQueryData(['agents', newAgent.id], newAgent);
    },
    onError: (error) => {
      // 錯誤處理
      console.error('Failed to create agent:', error);
    }
  });
};

// services/agentService.ts
import axios from 'axios';

export const agentService = {
  create: async (data: AgentCreateDto): Promise<Agent> => {
    const response = await axios.post('/api/v1/agents', data);
    return response.data;
  },
  // 其他 CRUD 方法...
};
```

**Material-UI 組件使用** ([Component Library](../../docs/ux-design/design-system/component-library.md)):

```typescript
import {
  TextField,
  Select,
  MenuItem,
  Slider,
  Button,
  FormControl,
  FormLabel,
  FormHelperText
} from '@mui/material';

// Model Selector
<FormControl fullWidth error={!!errors.model}>
  <FormLabel>LLM Model</FormLabel>
  <Select {...register('model')}>
    <MenuItem value="gpt-4">GPT-4</MenuItem>
    <MenuItem value="gpt-4o">GPT-4o (Recommended)</MenuItem>
    <MenuItem value="gpt-4o-mini">GPT-4o Mini</MenuItem>
  </Select>
  <FormHelperText>{errors.model?.message}</FormHelperText>
</FormControl>

// Temperature Slider
<FormControl fullWidth>
  <FormLabel>Temperature: {watch('parameters.temperature')}</FormLabel>
  <Slider
    {...register('parameters.temperature')}
    min={0}
    max={2}
    step={0.1}
    marks={[
      { value: 0, label: '0 (Precise)' },
      { value: 1, label: '1' },
      { value: 2, label: '2 (Creative)' }
    ]}
  />
</FormControl>
```

### 2.5 關鍵技術決策點

**決策 1: Client-Side vs Server-Side Validation**
- **決策**: 雙重驗證 (Client + Server)
- **理由**:
  - Client-Side: 即時反饋，提升 UX
  - Server-Side: 安全保證，防止繞過前端驗證
- **實施**: React Hook Form + Yup (Client), FluentValidation (Server)
- **參考**: [Validation Strategy](../../docs/technical-implementation/01-backend-net9/06-validation-strategy.md)

**決策 2: 表單狀態重置策略**
- **決策**: 成功提交後重置表單
- **理由**: 允許快速創建多個 Agent
- **替代方案**: 不重置 (導航到詳情頁)
- **實施**: `reset()` 方法 (React Hook Form)

**決策 3: API 響應時間目標**
- **決策**: < 200ms (P95)
- **理由**: 用戶感知延遲閾值
- **監控**: Application Insights
- **參考**: [Performance Requirements](../../docs/api/api-design.md#performance-requirements)

---

## 三、US 1.2: 透過 .NET SDK 創建 Agent - 詳細技術上下文

### 3.1 SDK 架構設計上下文

**SDK 設計完整文檔**: [Agent SDK Architecture](../../docs/technical-implementation/01-backend-net9/03-agent-sdk.md)

**SDK 架構層次**:

```
SemanticKernel.Agentic NuGet Package
├── AgentClient (主要入口)
│   ├── HTTP 通訊層 (HttpClient)
│   ├── 身份驗證 (API Key)
│   └── 錯誤處理與重試
│
├── Builders (Fluent API)
│   ├── AgentBuilder (Agent 創建)
│   ├── PluginConfigBuilder (Plugin 配置)
│   └── ParameterBuilder (參數配置)
│
├── Models (資料模型)
│   ├── Agent (Agent 實體)
│   ├── AgentRole (角色枚舉)
│   ├── LLMModel (模型枚舉)
│   └── AgentParameters (參數配置)
│
└── Exceptions (自定義異常)
    ├── AgentCreationException
    ├── ValidationException
    └── ApiException
```

**核心設計原則** ([SDK Design Principles](../../docs/technical-implementation/01-backend-net9/03-agent-sdk.md#design-principles)):

1. **Fluent API**: 鏈式調用，提升可讀性
2. **強類型**: 避免 magic string，編譯期檢查
3. **異步優先**: 所有 I/O 操作使用 async/await
4. **完整文檔**: XML 註解提供 IntelliSense 支援
5. **錯誤友好**: 清晰的異常訊息和錯誤處理

### 3.2 Fluent API Builder 實施細節

**AgentBuilder 完整實現** ([Fluent API Builder](../../docs/technical-implementation/01-backend-net9/03-agent-sdk.md#fluent-api-builder)):

```csharp
namespace SemanticKernel.Agentic.Builders
{
    /// <summary>
    /// Fluent builder for creating Agent configurations
    /// </summary>
    public class AgentBuilder
    {
        private readonly AgentCreateDto _dto = new();

        /// <summary>
        /// Sets the agent name (3-100 characters, required)
        /// </summary>
        /// <param name="name">The unique name for the agent</param>
        /// <returns>The builder instance for chaining</returns>
        /// <exception cref="ArgumentException">Thrown when name is invalid</exception>
        public AgentBuilder WithName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Agent name cannot be empty", nameof(name));

            if (name.Length < 3 || name.Length > 100)
                throw new ArgumentException("Agent name must be 3-100 characters", nameof(name));

            _dto.Name = name;
            return this;
        }

        /// <summary>
        /// Sets the agent description (optional)
        /// </summary>
        /// <param name="description">A brief description of the agent's purpose</param>
        /// <returns>The builder instance for chaining</returns>
        public AgentBuilder WithDescription(string description)
        {
            _dto.Description = description;
            return this;
        }

        /// <summary>
        /// Sets the agent role
        /// </summary>
        /// <param name="role">The role type for the agent</param>
        /// <returns>The builder instance for chaining</returns>
        public AgentBuilder WithRole(AgentRole role)
        {
            _dto.Role = role.ToString();
            return this;
        }

        /// <summary>
        /// Sets the LLM model to use
        /// </summary>
        /// <param name="model">The LLM model selection</param>
        /// <returns>The builder instance for chaining</returns>
        public AgentBuilder WithModel(LLMModel model)
        {
            _dto.Model = model switch
            {
                LLMModel.GPT4 => "gpt-4",
                LLMModel.GPT4o => "gpt-4o",
                LLMModel.GPT4oMini => "gpt-4o-mini",
                _ => throw new ArgumentException($"Unsupported model: {model}")
            };
            return this;
        }

        /// <summary>
        /// Sets the system prompt (10-4000 characters, required)
        /// </summary>
        /// <param name="prompt">The system prompt defining agent behavior</param>
        /// <returns>The builder instance for chaining</returns>
        /// <exception cref="ArgumentException">Thrown when prompt is invalid</exception>
        public AgentBuilder WithSystemPrompt(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
                throw new ArgumentException("System prompt cannot be empty", nameof(prompt));

            if (prompt.Length < 10 || prompt.Length > 4000)
                throw new ArgumentException("Prompt must be 10-4000 characters", nameof(prompt));

            _dto.SystemPrompt = prompt;
            return this;
        }

        /// <summary>
        /// Sets the temperature parameter (0.0-2.0, default 0.7)
        /// </summary>
        /// <param name="temperature">Controls randomness in responses</param>
        /// <returns>The builder instance for chaining</returns>
        /// <exception cref="ArgumentException">Thrown when value is out of range</exception>
        public AgentBuilder WithTemperature(double temperature)
        {
            if (temperature < 0.0 || temperature > 2.0)
                throw new ArgumentException("Temperature must be 0.0-2.0", nameof(temperature));

            _dto.Parameters ??= new AgentParameters();
            _dto.Parameters.Temperature = temperature;
            return this;
        }

        /// <summary>
        /// Sets the max tokens parameter (1-4096, default 2000)
        /// </summary>
        /// <param name="maxTokens">Maximum tokens in response</param>
        /// <returns>The builder instance for chaining</returns>
        public AgentBuilder WithMaxTokens(int maxTokens)
        {
            if (maxTokens < 1 || maxTokens > 4096)
                throw new ArgumentException("MaxTokens must be 1-4096", nameof(maxTokens));

            _dto.Parameters ??= new AgentParameters();
            _dto.Parameters.MaxTokens = maxTokens;
            return this;
        }

        /// <summary>
        /// Sets the top_p parameter (0.0-1.0, default 1.0)
        /// </summary>
        /// <param name="topP">Controls diversity via nucleus sampling</param>
        /// <returns>The builder instance for chaining</returns>
        public AgentBuilder WithTopP(double topP)
        {
            if (topP < 0.0 || topP > 1.0)
                throw new ArgumentException("TopP must be 0.0-1.0", nameof(topP));

            _dto.Parameters ??= new AgentParameters();
            _dto.Parameters.TopP = topP;
            return this;
        }

        /// <summary>
        /// Validates and builds the agent configuration
        /// </summary>
        /// <returns>The validated agent DTO</returns>
        /// <exception cref="InvalidOperationException">Thrown when required fields are missing</exception>
        internal AgentCreateDto Build()
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(_dto.Name))
                throw new InvalidOperationException("Agent name is required");

            if (string.IsNullOrWhiteSpace(_dto.SystemPrompt))
                throw new InvalidOperationException("System prompt is required");

            if (string.IsNullOrWhiteSpace(_dto.Model))
                throw new InvalidOperationException("Model selection is required");

            // Set defaults if not provided
            _dto.Parameters ??= new AgentParameters
            {
                Temperature = 0.7,
                MaxTokens = 2000,
                TopP = 1.0
            };

            return _dto;
        }
    }
}
```

**AgentClient 實施** ([SDK Client Implementation](../../docs/technical-implementation/01-backend-net9/03-agent-sdk.md#client-implementation)):

```csharp
namespace SemanticKernel.Agentic
{
    /// <summary>
    /// Client for interacting with the Semantic Kernel Agentic API
    /// </summary>
    public class AgentClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        /// <summary>
        /// Initializes a new instance of AgentClient
        /// </summary>
        /// <param name="apiKey">API authentication key</param>
        /// <param name="endpoint">API endpoint URL</param>
        public AgentClient(string apiKey, string endpoint)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(endpoint)
            };
            _httpClient.DefaultRequestHeaders.Add("X-API-Key", _apiKey);
        }

        /// <summary>
        /// Creates a new agent using a fluent builder
        /// </summary>
        /// <param name="configure">Action to configure the agent builder</param>
        /// <returns>The created agent</returns>
        /// <exception cref="AgentCreationException">Thrown when creation fails</exception>
        public async Task<Agent> CreateAsync(Action<AgentBuilder> configure)
        {
            var builder = new AgentBuilder();
            configure(builder);

            var dto = builder.Build();

            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/v1/agents", dto);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new AgentCreationException(
                        $"Failed to create agent: {response.StatusCode}",
                        errorContent
                    );
                }

                var agent = await response.Content.ReadFromJsonAsync<Agent>();
                return agent ?? throw new AgentCreationException("Received null agent from API");
            }
            catch (HttpRequestException ex)
            {
                throw new AgentCreationException("Network error during agent creation", ex);
            }
        }

        // 其他方法: GetAsync, UpdateAsync, DeleteAsync, etc.
    }
}
```

### 3.3 NuGet 套件配置細節

**NuGet Package 配置** ([NuGet Packaging](../../docs/technical-implementation/01-backend-net9/03-agent-sdk.md#nuget-packaging)):

```xml
<!-- SemanticKernel.Agentic.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>

    <!-- NuGet Package Metadata -->
    <PackageId>SemanticKernel.Agentic</PackageId>
    <Version>0.1.0-alpha</Version>
    <Authors>Your Organization</Authors>
    <Company>Your Company</Company>
    <Description>Official .NET SDK for Semantic Kernel Agentic Framework</Description>
    <PackageTags>semantic-kernel;ai;agent;llm;sdk</PackageTags>
    <PackageProjectUrl>https://github.com/yourorg/semantic-kernel-agentic</PackageProjectUrl>
    <RepositoryUrl>https://github.com/yourorg/semantic-kernel-agentic</RepositoryUrl>
    <PackageLicenseExpression>MIT</PackageLicenseExpression>
    <PackageReadmeFile>README.md</PackageReadmeFile>

    <!-- Enable Source Link -->
    <PublishRepositoryUrl>true</PublishRepositoryUrl>
    <EmbedUntrackedSources>true</EmbedUntrackedSources>
    <IncludeSymbols>true</IncludeSymbols>
    <SymbolPackageFormat>snupkg</SymbolPackageFormat>
  </PropertyGroup>

  <ItemGroup>
    <None Include="README.md" Pack="true" PackagePath="\" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="System.Net.Http.Json" Version="8.0.0" />
    <PackageReference Include="System.Text.Json" Version="8.0.0" />
    <PackageReference Include="Microsoft.SourceLink.GitHub" Version="8.0.0" PrivateAssets="All" />
  </ItemGroup>
</Project>
```

**發布流程** ([SDK Publishing](../../docs/technical-implementation/01-backend-net9/03-agent-sdk.md#publishing)):

```bash
# 1. 建置套件
dotnet pack -c Release

# 2. 發布到 NuGet.org
dotnet nuget push bin/Release/SemanticKernel.Agentic.0.1.0-alpha.nupkg \
  --api-key <YOUR_API_KEY> \
  --source https://api.nuget.org/v3/index.json

# 3. 發布到內部 NuGet Feed (可選)
dotnet nuget push bin/Release/SemanticKernel.Agentic.0.1.0-alpha.nupkg \
  --source https://your-internal-feed.com
```

### 3.4 錯誤處理策略

**自定義異常設計** ([SDK Error Handling](../../docs/technical-implementation/01-backend-net9/03-agent-sdk.md#error-handling)):

```csharp
namespace SemanticKernel.Agentic.Exceptions
{
    /// <summary>
    /// Base exception for all SDK errors
    /// </summary>
    public class AgentSdkException : Exception
    {
        public AgentSdkException(string message) : base(message) { }
        public AgentSdkException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    /// <summary>
    /// Thrown when agent creation fails
    /// </summary>
    public class AgentCreationException : AgentSdkException
    {
        public string? ApiResponse { get; }

        public AgentCreationException(string message, string? apiResponse = null)
            : base(message)
        {
            ApiResponse = apiResponse;
        }

        public AgentCreationException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    /// <summary>
    /// Thrown when validation fails
    /// </summary>
    public class ValidationException : AgentSdkException
    {
        public Dictionary<string, string[]> Errors { get; }

        public ValidationException(Dictionary<string, string[]> errors)
            : base("Validation failed")
        {
            Errors = errors;
        }
    }
}
```

**使用範例與錯誤處理**:

```csharp
try
{
    var agent = await agentClient.CreateAsync(builder => builder
        .WithName("My Agent")
        .WithModel(LLMModel.GPT4o)
        .WithSystemPrompt("You are a helpful assistant...")
    );

    Console.WriteLine($"Agent created: {agent.Id}");
}
catch (ValidationException ex)
{
    Console.WriteLine("Validation errors:");
    foreach (var (field, errors) in ex.Errors)
    {
        Console.WriteLine($"  {field}: {string.Join(", ", errors)}");
    }
}
catch (AgentCreationException ex)
{
    Console.WriteLine($"Failed to create agent: {ex.Message}");
    if (!string.IsNullOrEmpty(ex.ApiResponse))
    {
        Console.WriteLine($"API Response: {ex.ApiResponse}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error: {ex.Message}");
}
```

---

## 四、US 1.3: Agent 配置管理 - 詳細技術上下文

### 4.1 CRUD API 實施細節

**API 設計完整文檔**: [API Design Specification](../../docs/api/api-design.md)

**列表查詢 API**: `GET /api/v1/agents`

**Query Parameters 詳解** ([Search & Filter Design](../../docs/api/api-design.md#search-filter)):

```
GET /api/v1/agents?page=1&pageSize=20&search=customer&role=CustomerService&model=gpt-4o&isActive=true&sortBy=createdAt&sortOrder=desc
```

參數說明:
- `page`: 頁碼 (從 1 開始)
- `pageSize`: 每頁記錄數 (預設 20, 最大 100)
- `search`: 搜尋關鍵字 (名稱或描述)
- `role`: 角色篩選
- `model`: 模型篩選
- `isActive`: 狀態篩選 (true/false)
- `sortBy`: 排序欄位 (createdAt/updatedAt/name)
- `sortOrder`: 排序方向 (asc/desc)

**CQRS Query Handler 實施** ([CQRS Implementation](../../docs/technical-implementation/01-backend-net9/05-cqrs-implementation.md)):

```csharp
public class GetAgentsListQuery : IRequest<Result<PagedResult<AgentDto>>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Search { get; set; }
    public string? Role { get; set; }
    public string? Model { get; set; }
    public bool? IsActive { get; set; }
    public string SortBy { get; set; } = "createdAt";
    public string SortOrder { get; set; } = "desc";
}

public class GetAgentsListQueryHandler
    : IRequestHandler<GetAgentsListQuery, Result<PagedResult<AgentDto>>>
{
    private readonly IAgentRepository _repository;
    private readonly IMapper _mapper;

    public async Task<Result<PagedResult<AgentDto>>> Handle(
        GetAgentsListQuery request,
        CancellationToken cancellationToken)
    {
        // 1. 構建查詢條件
        var specification = new AgentFilterSpecification(
            search: request.Search,
            role: request.Role,
            model: request.Model,
            isActive: request.IsActive
        );

        // 2. 執行分頁查詢
        var (agents, totalCount) = await _repository.GetPagedAsync(
            specification,
            request.Page,
            request.PageSize,
            request.SortBy,
            request.SortOrder,
            cancellationToken
        );

        // 3. 映射到 DTO
        var dtos = _mapper.Map<List<AgentDto>>(agents);

        // 4. 構建分頁結果
        var result = new PagedResult<AgentDto>
        {
            Data = dtos,
            Pagination = new PaginationInfo
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            }
        };

        return Result<PagedResult<AgentDto>>.Success(result);
    }
}
```

**Specification Pattern 實施** ([Repository Pattern](../../docs/architecture/adr/ADR-003-repository-pattern.md)):

```csharp
public class AgentFilterSpecification : Specification<Agent>
{
    public AgentFilterSpecification(
        string? search,
        string? role,
        string? model,
        bool? isActive)
    {
        // 搜尋條件: 名稱或描述包含關鍵字
        if (!string.IsNullOrWhiteSpace(search))
        {
            Criteria = a => a.Name.Contains(search) ||
                           (a.Description != null && a.Description.Contains(search));
        }

        // 角色篩選
        if (!string.IsNullOrWhiteSpace(role))
        {
            AddCriteria(a => a.Role == role);
        }

        // 模型篩選
        if (!string.IsNullOrWhiteSpace(model))
        {
            AddCriteria(a => a.Model == model);
        }

        // 狀態篩選
        if (isActive.HasValue)
        {
            AddCriteria(a => a.IsActive == isActive.Value);
        }
    }
}
```

**刪除 API**: `DELETE /api/v1/agents/{id}`

**軟刪除 Command Handler** ([Soft Delete Pattern](../../docs/api/database-schema.md#soft-delete-pattern)):

```csharp
public class DeleteAgentCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}

public class DeleteAgentCommandHandler : IRequestHandler<DeleteAgentCommand, Result>
{
    private readonly IAgentRepository _repository;

    public async Task<Result> Handle(
        DeleteAgentCommand request,
        CancellationToken cancellationToken)
    {
        // 1. 查詢 Agent
        var agent = await _repository.GetByIdAsync(request.Id);
        if (agent == null)
            return Result.Failure("Agent not found");

        // 2. 軟刪除 (設置 IsDeleted = true)
        agent.Delete(); // Domain method: this.IsDeleted = true;

        // 3. 保存變更
        await _repository.UpdateAsync(agent);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
```

### 4.2 Frontend 組件設計上下文

**Frontend 組件完整文檔**: [Component Design](../../docs/technical-implementation/02-frontend-react/02-component-design.md)

**AgentListView 組件設計** ([Agent List Wireframe](../../docs/ux-design/wireframes/low-fidelity/02-agent-list.md)):

```typescript
// components/agents/AgentListView.tsx
import { Grid, Pagination, TextField, Select, MenuItem } from '@mui/material';
import { useAgents } from '@/hooks/useAgents';

export const AgentListView: React.FC = () => {
  const [filters, setFilters] = useState({
    page: 1,
    pageSize: 20,
    search: '',
    role: '',
    model: '',
    isActive: undefined as boolean | undefined
  });

  const { data, isLoading, error } = useAgents(filters);

  return (
    <Box>
      {/* Search & Filter Bar */}
      <Box sx={{ mb: 3 }}>
        <Grid container spacing={2}>
          <Grid item xs={12} md={4}>
            <TextField
              fullWidth
              placeholder="Search agents..."
              value={filters.search}
              onChange={(e) => setFilters({ ...filters, search: e.target.value })}
            />
          </Grid>
          <Grid item xs={12} md={3}>
            <Select
              fullWidth
              value={filters.role}
              onChange={(e) => setFilters({ ...filters, role: e.target.value })}
            >
              <MenuItem value="">All Roles</MenuItem>
              <MenuItem value="CustomerService">Customer Service</MenuItem>
              <MenuItem value="DataAnalyst">Data Analyst</MenuItem>
            </Select>
          </Grid>
          {/* 其他篩選器... */}
        </Grid>
      </Box>

      {/* Agent Grid */}
      <Grid container spacing={3}>
        {data?.data.map((agent) => (
          <Grid item xs={12} md={4} key={agent.id}>
            <AgentCard agent={agent} />
          </Grid>
        ))}
      </Grid>

      {/* Pagination */}
      <Box sx={{ mt: 4, display: 'flex', justifyContent: 'center' }}>
        <Pagination
          count={data?.pagination.totalPages || 1}
          page={filters.page}
          onChange={(_, page) => setFilters({ ...filters, page })}
        />
      </Box>
    </Box>
  );
};
```

**AgentCard 組件** ([Component Library](../../docs/ux-design/design-system/component-library.md)):

```typescript
// components/agents/AgentCard.tsx
import { Card, CardContent, Typography, Chip, IconButton } from '@mui/material';
import { Edit, Delete, Visibility } from '@mui/icons-material';

interface AgentCardProps {
  agent: Agent;
}

export const AgentCard: React.FC<AgentCardProps> = ({ agent }) => {
  const navigate = useNavigate();
  const deleteAgentMutation = useDeleteAgent();

  const handleDelete = async () => {
    if (window.confirm(`Delete agent "${agent.name}"?`)) {
      await deleteAgentMutation.mutateAsync(agent.id);
    }
  };

  return (
    <Card>
      <CardContent>
        <Typography variant="h6" gutterBottom>
          {agent.name}
        </Typography>

        <Chip label={agent.model} size="small" sx={{ mb: 1 }} />

        <Typography variant="body2" color="text.secondary" noWrap>
          {agent.description}
        </Typography>

        <Box sx={{ mt: 2, display: 'flex', justifyContent: 'flex-end' }}>
          <IconButton onClick={() => navigate(`/agents/${agent.id}`)}>
            <Visibility />
          </IconButton>
          <IconButton onClick={() => navigate(`/agents/${agent.id}/edit`)}>
            <Edit />
          </IconButton>
          <IconButton onClick={handleDelete} color="error">
            <Delete />
          </IconButton>
        </Box>
      </CardContent>
    </Card>
  );
};
```

### 4.3 狀態管理策略

**React Query 狀態管理** ([State Management](../../docs/technical-implementation/02-frontend-react/03-state-management.md)):

```typescript
// hooks/useAgents.ts
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';

// 查詢 Agent 列表
export const useAgents = (filters: AgentFilters) => {
  return useQuery({
    queryKey: ['agents', filters],
    queryFn: () => agentService.list(filters),
    staleTime: 5 * 60 * 1000, // 5 分鐘
    cacheTime: 10 * 60 * 1000, // 10 分鐘
  });
};

// 查詢單個 Agent
export const useAgent = (id: string) => {
  return useQuery({
    queryKey: ['agents', id],
    queryFn: () => agentService.getById(id),
    enabled: !!id, // 僅當 ID 存在時查詢
  });
};

// 更新 Agent
export const useUpdateAgent = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: AgentUpdateDto }) =>
      agentService.update(id, data),
    onSuccess: (updatedAgent) => {
      // 更新列表緩存
      queryClient.invalidateQueries({ queryKey: ['agents'] });

      // 更新單個 Agent 緩存
      queryClient.setQueryData(['agents', updatedAgent.id], updatedAgent);
    },
  });
};

// 刪除 Agent
export const useDeleteAgent = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => agentService.delete(id),
    onSuccess: (_, deletedId) => {
      // 從緩存中移除
      queryClient.invalidateQueries({ queryKey: ['agents'] });
      queryClient.removeQueries({ queryKey: ['agents', deletedId] });
    },
  });
};
```

**緩存策略說明** ([Caching Strategy](../../docs/technical-implementation/02-frontend-react/05-data-fetching.md#caching)):

- `staleTime`: 數據被視為新鮮的時間 (5 分鐘)
- `cacheTime`: 數據在緩存中保留的時間 (10 分鐘)
- `invalidateQueries`: 使緩存失效，觸發重新查詢
- `setQueryData`: 手動更新緩存數據
- `removeQueries`: 從緩存中移除數據

### 4.4 分頁、搜尋、篩選實施細節

**Debounce 搜尋實施** ([Performance Optimization](../../docs/technical-implementation/02-frontend-react/01-frontend-architecture.md#performance-optimization)):

```typescript
import { useDebouncedValue } from '@/hooks/useDebouncedValue';

export const AgentListView: React.FC = () => {
  const [searchInput, setSearchInput] = useState('');
  const debouncedSearch = useDebouncedValue(searchInput, 500); // 500ms delay

  const { data } = useAgents({
    search: debouncedSearch, // 使用 debounced 值
    // ...其他 filters
  });

  return (
    <TextField
      value={searchInput}
      onChange={(e) => setSearchInput(e.target.value)}
      placeholder="Search agents..."
    />
  );
};

// hooks/useDebouncedValue.ts
import { useState, useEffect } from 'react';

export const useDebouncedValue = <T>(value: T, delay: number): T => {
  const [debouncedValue, setDebouncedValue] = useState(value);

  useEffect(() => {
    const handler = setTimeout(() => {
      setDebouncedValue(value);
    }, delay);

    return () => {
      clearTimeout(handler);
    };
  }, [value, delay]);

  return debouncedValue;
};
```

**URL State Sync (可選優化)** ([Routing Best Practices](../../docs/technical-implementation/02-frontend-react/01-frontend-architecture.md#routing)):

```typescript
import { useSearchParams } from 'react-router-dom';

export const AgentListView: React.FC = () => {
  const [searchParams, setSearchParams] = useSearchParams();

  const filters = {
    page: parseInt(searchParams.get('page') || '1'),
    search: searchParams.get('search') || '',
    role: searchParams.get('role') || '',
    // ...
  };

  const updateFilters = (newFilters: Partial<typeof filters>) => {
    const params = new URLSearchParams();
    Object.entries({ ...filters, ...newFilters }).forEach(([key, value]) => {
      if (value) params.set(key, value.toString());
    });
    setSearchParams(params);
  };

  // 好處: URL 可分享, 瀏覽器前進/後退支援
};
```

---

## 五、完整文檔引用清單 (按功能分類)

### 5.1 規劃與策略文檔 (7 refs)

1. [MVP Scope Definition](../../1-planning/MVP-SCOPE-DEFINITION.md) - Phase 1A 範圍定義
2. [Sprint Allocation Analysis](../../1-planning/SPRINT-ALLOCATION-ANALYSIS.md) - Sprint 1 詳細分析 (13 SP, 3 週)
3. [Development Strategy](../../1-planning/DEVELOPMENT-STRATEGY.md) - Phase 1A 開發策略
4. [Dependency Matrix](../../1-planning/DEPENDENCY-MATRIX.md) - 依賴關係追蹤
5. [Risk Register](../../1-planning/RISK-REGISTER.md) - 風險管理
6. [Technical Decisions Log](../../1-planning/TECHNICAL-DECISIONS-LOG.md) - 技術決策日誌
7. [Architecture Evolution Roadmap](../../1-planning/ARCHITECTURE-EVOLUTION-ROADMAP.md) - 架構演進路線圖

### 5.2 架構設計文檔 - ADRs (5 refs)

8. [Architecture Design Document](../../docs/architecture/Architecture-Design-Document.md) - 完整系統架構
9. [ADR-001: Clean Architecture](../../docs/architecture/adr/ADR-001-clean-architecture.md) - 架構分層決策
10. [ADR-002: CQRS Pattern](../../docs/architecture/adr/ADR-002-cqrs-pattern.md) - CQRS 架構決策
11. [ADR-003: Repository Pattern](../../docs/architecture/adr/ADR-003-repository-pattern.md) - Repository 架構決策
12. [ADR-004: PostgreSQL Selection](../../docs/architecture/adr/ADR-004-database-selection.md) - 資料庫選型決策

### 5.3 User Stories 與需求 (3 refs)

13. [User Stories Overview](../../docs/user-stories/README.md) - User Story 總覽
14. [Module 01: Agent Creation](../../docs/user-stories/modules/module-01-agent-creation.md) - Agent 管理系統完整規格
15. [Epic 01: Agent Lifecycle Management](../../docs/user-stories/epics/epic-01.md) - Agent 生命週期管理策略

### 5.4 Backend 技術實作 (9 refs)

16. [Backend Architecture](../../docs/technical-implementation/01-backend-net9/README.md) - Backend 架構總覽
17. [Monorepo Setup](../../docs/technical-implementation/01-backend-net9/01-monorepo-setup.md) - 專案結構設定
18. [Semantic Kernel Integration](../../docs/technical-implementation/01-backend-net9/02-semantic-kernel-integration.md) - SK 整合指南
19. [Agent SDK](../../docs/technical-implementation/01-backend-net9/03-agent-sdk.md) - SDK 完整設計
20. [Data Access Layer](../../docs/technical-implementation/01-backend-net9/04-data-access-layer.md) - EF Core 實作
21. [CQRS Implementation](../../docs/technical-implementation/01-backend-net9/05-cqrs-implementation.md) - MediatR 實作
22. [Validation Strategy](../../docs/technical-implementation/01-backend-net9/06-validation-strategy.md) - FluentValidation 實作
23. [Domain Layer](../../docs/technical-implementation/01-backend-net9/07-domain-layer.md) - Domain 設計
24. [API Layer](../../docs/technical-implementation/01-backend-net9/08-api-layer.md) - API 控制器設計

### 5.5 Frontend 技術實作 (6 refs)

25. [Frontend Architecture](../../docs/technical-implementation/02-frontend-react/01-frontend-architecture.md) - React 架構設計
26. [Component Design](../../docs/technical-implementation/02-frontend-react/02-component-design.md) - 組件設計規範
27. [State Management](../../docs/technical-implementation/02-frontend-react/03-state-management.md) - Zustand + React Query
28. [Form Validation](../../docs/technical-implementation/02-frontend-react/04-form-validation.md) - 表單驗證策略
29. [Data Fetching Strategy](../../docs/technical-implementation/02-frontend-react/05-data-fetching.md) - React Query 實作
30. [API Integration](../../docs/technical-implementation/02-frontend-react/06-api-integration.md) - API 調用封裝

### 5.6 UX 設計文檔 (5 refs)

31. [Design System](../../docs/ux-design/design-system/README.md) - Material-UI 設計系統
32. [Component Library](../../docs/ux-design/design-system/component-library.md) - 組件規範
33. [Agent List Wireframe](../../docs/ux-design/wireframes/low-fidelity/02-agent-list.md) - 列表頁線框圖
34. [Agent Create Wireframe](../../docs/ux-design/wireframes/low-fidelity/03-agent-create.md) - 創建頁線框圖
35. [Agent Detail Wireframe](../../docs/ux-design/wireframes/low-fidelity/04-agent-detail.md) - 詳情頁線框圖

### 5.7 API 與資料庫設計 (2 refs)

36. [API Design Specification](../../docs/api/api-design.md) - RESTful API 完整設計
37. [Database Schema Design](../../docs/api/database-schema.md) - PostgreSQL Schema 設計

### 5.8 測試文檔 (7 refs)

38. [Testing Strategy](../../docs/testing/testing-strategy.md) - 完整測試策略
39. [Unit Testing Guidelines](../../docs/testing/unit-testing-guidelines.md) - 單元測試規範
40. [Integration Testing Guidelines](../../docs/testing/integration-testing-guidelines.md) - 集成測試規範
41. [E2E Testing Guidelines](../../docs/testing/e2e-testing-guidelines.md) - E2E 測試規範
42. [SDK Testing Guidelines](../../docs/testing/sdk-testing-guidelines.md) - SDK 測試規範
43. [QA Standards](../../docs/testing/qa-standards.md) - 質量保證標準
44. [TDD Best Practices](../../docs/testing/tdd-best-practices.md) - TDD 最佳實踐

### 5.9 開發標準 (6 refs)

45. [Code Quality Standards](../../docs/development-standards/code-quality-standards.md) - 代碼質量標準
46. [Coding Conventions](../../docs/development-standards/coding-conventions.md) - 編碼規範
47. [Code Review Checklist](../../docs/development-standards/code-review-checklist.md) - Code Review 檢查清單
48. [Git Workflow](../../docs/development-standards/git-workflow.md) - Git 分支策略
49. [Documentation Standards](../../docs/development-standards/documentation-standards.md) - 文檔規範
50. [Technical Debt Management](../../docs/development-standards/technical-debt-management.md) - 技術債務管理

### 5.10 專案管理 (3 refs)

51. [Sprint Planning Guide](../../docs/project-management/sprint-planning-guide.md) - Sprint 規劃指南
52. [Definition of Done](../../docs/project-management/definition-of-done.md) - 完成定義
53. [Agile Practices](../../docs/project-management/agile-practices.md) - 敏捷實踐

---

## 📋 更新日誌

| 版本 | 日期 | 變更內容 | 負責人 |
|------|------|---------|--------|
| 1.0 | 2025-11-12 | 初版創建 - 提供 Sprint 1 完整技術上下文與 53+ 個技術參考 | Documentation Team |

**v1.0 內容摘要**:
- ✅ 建立完整技術參考層 (5 個主要類別)
- ✅ 提供 US 1.1 詳細技術上下文 (Backend API, Database, Frontend UI)
- ✅ 提供 US 1.2 詳細技術上下文 (SDK 架構, Fluent API, NuGet 配置)
- ✅ 提供 US 1.3 詳細技術上下文 (CRUD API, Frontend 組件, 狀態管理)
- ✅ 整合 53+ 個技術參考文獻，支持深度技術查詢
- ✅ 強化 section-level 引用 (符合 Sprint 1-9 強化要求)
- ✅ 提供關鍵技術決策點分析
- ✅ 提供完整代碼範例與實施細節
- ✅ 支持 "Just-in-Time" 技術查詢場景

**使用建議**:
- 開發前: 閱讀對應章節的「MVP 範圍定義」與「關鍵技術決策」
- 開發中: 查詢「實施細節」章節的代碼範例與技術參考
- 遇到問題: 使用「完整文檔引用清單」快速定位相關技術文檔

---

**創建日期**: 2025-11-12
**維護說明**: 本文檔為 Sprint 1 的技術上下文參考手冊，提供 53+ 個技術參考連結與詳細實施指導，支持 "Just-in-Time" 技術查詢場景。
