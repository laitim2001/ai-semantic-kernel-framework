# 文件運用策略

**Semantic Kernel Agentic Framework - Document Usage Strategy**

**版本**: 1.0.0
**日期**: 2025-11-02
**狀態**: 規劃中
**目的**: 定義所有規劃文檔在開發過程中的具體運用方式

[📚 返回主索引](README.md)

---

## 📋 文檔目的

本文檔詳細說明如何在**開發階段**系統性地運用所有規劃文檔,確保:
1. ✅ 開發團隊清楚知道查閱哪些文檔
2. ✅ 每個開發活動有明確的文檔支持
3. ✅ 避免文檔孤島,發揮文檔的最大價值
4. ✅ 確保實作與規劃的一致性

---

## 🗂️ 文檔庫存總覽

### 當前文檔結構

```
C:\AI Semantic Kernel\
├── docs/
│   ├── brief.md                            # 1. 商業規劃
│   ├── user-stories/                       # 2. 需求文檔
│   ├── architecture/                       # 3. 架構文檔
│   ├── project-management/                 # 4. 專案管理
│   ├── ux-design/                          # 5. UX 設計
│   ├── technical-implementation/           # 6. 技術實施
│   ├── integration-validation/             # 7. 整合驗證
│   ├── BMAD-STAGE-ANALYSIS.md              # BMad 階段分析
│   ├── PO-VALIDATION-REPORT.md             # PO 驗證報告
│   ├── DOCUMENT-SHARDING-REPORT.md         # 文檔切分報告
│   ├── PROJECT-INITIALIZATION-DECISION.md  # 專案初始化決策
│   └── DOCUMENT-USAGE-STRATEGY.md          # 本文檔
│
├── poc-projects/                           # 8. PoC 實作代碼
└── poc-validation/                         # 9. PoC 驗證報告
```

---

## 📖 文件分類與運用策略

### 分類 1: brief.md (商業規劃)

**文檔位置**: `docs/brief.md`

**內容概覽**:
- 商業問題陳述 (Problem Statement)
- 解決方案描述 (Solution)
- 目標市場與用戶 (Target Market)
- 核心價值主張 (Value Proposition)
- 競品分析 (Competitor Analysis)
- 成功指標 (Success Metrics)

**運用場景**:

#### 1.1 產品決策時
**使用者**: Product Owner, Product Manager
**查閱時機**:
- 需要決策功能優先級時
- 評估新功能需求時
- 與利益相關者溝通時

**使用方式**:
```
場景: 評估是否將 Workflow Editor 納入 MVP
查閱: brief.md → Section "Market Strategy" → Phase 1/2 定義
決策依據: Brief 明確定義 Workflow Editor 在 Phase 2
```

#### 1.2 開發目標對齊時
**使用者**: 全體開發團隊
**查閱時機**: Sprint Planning, Sprint Review
**使用方式**:
- 確保開發的功能符合商業目標
- 理解"為什麼"要開發某個功能

#### 1.3 對外溝通時
**使用者**: Sales, Marketing, Executives
**查閱時機**: 客戶簡報, 投資者會議
**使用方式**:
- 提取核心價值主張
- 使用競品分析數據

---

### 分類 2: /user-stories (需求文檔)

**文檔位置**: `docs/user-stories/`

**文檔結構**:
```
user-stories/
├── README.md                          # User Stories 總覽
├── implementation-strategy.md         # 技術實施策略
├── mvp-planning.md                    # MVP 範圍與時程
├── modules/                           # 10 個功能模組
│   ├── module-01-agent-creation.md
│   ├── module-02-plugin-system.md
│   ├── module-03-code-interpreter.md
│   ├── module-04-multi-agent.md
│   ├── module-05-agent-memory.md
│   ├── module-06-chat-interface.md
│   ├── module-07-persona-framework.md
│   ├── module-08-structured-data.md
│   ├── module-09-enterprise-features.md
│   └── module-10-monitoring.md
└── epics/                             # Epic 切分文檔
    ├── README.md                      # Epic 導航
    ├── epic-05.1-conversation-memory.md
    ├── epic-05.2-knowledge-rag.md
    ├── epic-09.1-authentication.md
    ├── epic-09.2-multi-tenant.md
    ├── epic-09.3-i18n-api-metering.md
    ├── epic-10.1-monitoring-dashboard.md
    ├── epic-10.2-quality-monitoring.md
    └── epic-10.3-user-analytics.md
```

**運用場景**:

#### 2.1 Sprint Planning 時 ⭐ 核心用途
**使用者**: Product Owner, Scrum Master, 開發團隊
**查閱時機**: 每個 Sprint 開始前 (每 2 週)

**使用方式**:
```
Step 1: 確定 Sprint 目標
查閱: mvp-planning.md → 確認當前 Sprint 應該完成哪個 Epic

Step 2: 選擇 User Stories
查閱: epics/epic-XX.md → 選擇該 Epic 下的 Stories

Step 3: 理解驗收標準
查閱: 每個 Story 的驗收標準 (Acceptance Criteria)

Step 4: 估算工作量
查閱: Story 的技術要求 (Technical Requirements)
```

**範例**:
```
Sprint 1 目標: Agent 生命週期管理
查閱文檔:
1. mvp-planning.md → 確認 Sprint 1 = Epic 1
2. modules/module-01-agent-creation.md → 閱讀完整 Epic 1
3. 選擇 Stories: US 1.1, US 1.2, US 1.3
4. 閱讀每個 Story 的驗收標準和技術要求
```

#### 2.2 開發實作時 ⭐ 核心用途
**使用者**: 開發工程師
**查閱時機**: 實作每個 User Story 時

**使用方式**:
```
開發 US 1.1 (通過 Web UI 建立 Agent):

查閱順序:
1. modules/module-01-agent-creation.md → US 1.1 完整描述
   - 理解用戶場景 ("作為...我想要...以便...")
   - 閱讀驗收標準 (8 個必須項)
   - 閱讀技術要求 (React + TypeScript, 表單驗證)

2. implementation-strategy.md → 查找技術難點
   - 是否有特殊技術挑戰?
   - 推薦的實作方式?

3. 開始編碼
   - 根據驗收標準編寫測試
   - 根據技術要求選擇技術棧
```

#### 2.3 測試與 QA 時
**使用者**: QA 工程師
**查閱時機**: 功能測試, 驗收測試

**使用方式**:
```
測試 US 1.1:
查閱: modules/module-01-agent-creation.md → US 1.1 驗收標準
建立測試案例:
- [ ] 測試案例 1: 填寫 Agent 基本資訊
- [ ] 測試案例 2: 從下拉選單選擇 LLM 模型
- [ ] 測試案例 3: 設定 System Prompt
- [ ] ...
```

#### 2.4 Code Review 時
**使用者**: Senior Developer, Tech Lead
**查閱時機**: Pull Request Review

**使用方式**:
```
Review PR for US 1.1:
1. 查閱 US 1.1 驗收標準
2. 檢查 PR 是否滿足所有驗收標準
3. 檢查技術要求是否符合 (React + TypeScript)
4. 檢查是否有單元測試
```

---

### 分類 3: /architecture (架構文檔)

**文檔位置**: `docs/architecture/`

**文檔結構**:
```
architecture/
├── Architecture-Design-Document.md    # 總體架構文檔
├── ADR-006-agent-state-management.md  # Agent 狀態管理決策
├── ADR-007-multi-agent-communication.md  # Multi-Agent 通訊決策
├── ADR-008-code-interpreter-execution-model.md  # Code Interpreter 決策
├── ADR-011-framework-migration-strategy.md  # Framework 遷移策略
├── ADR-012-workflow-editor-technology.md  # Workflow Editor 技術選型
├── C4-architecture-diagrams.md        # C4 架構圖
├── database-schema.md                 # PostgreSQL 數據庫設計
├── performance-scalability-strategy.md  # 性能與擴展性策略
└── components/                        # 組件文檔 (待建立)
    └── README.md
```

**運用場景**:

#### 3.1 系統設計時 ⭐ 核心用途
**使用者**: System Architect, Senior Developer
**查閱時機**: 開始開發新組件或服務時

**使用方式**:
```
設計 Agent Service:

查閱順序:
1. Architecture-Design-Document.md
   - Section 3: 系統組件設計
   - Section 4: API 設計
   - Section 5: 數據模型

2. ADR-006-agent-state-management.md
   - 理解 Agent 狀態如何管理 (Hybrid 模式)
   - Redis vs PostgreSQL 使用場景

3. database-schema.md
   - 查看 Agents 表結構
   - 理解數據關係

4. performance-scalability-strategy.md
   - 理解性能目標 (P95 <5s)
   - 查看 Caching 策略
```

#### 3.2 API 開發時
**使用者**: Backend Developer
**查閱時機**: 實作 RESTful API 時

**使用方式**:
```
開發 POST /api/v1/agents:

查閱順序:
1. Architecture-Design-Document.md → Section 4: API 設計
   - 查看 API 端點定義
   - 查看 Request/Response Schema

2. database-schema.md
   - 查看需要插入的表 (Agents, AgentConfigurations)
   - 理解必填欄位

3. openapi-specification.yaml (待補充)
   - 查看完整的 OpenAPI Spec
```

#### 3.3 技術決策時
**使用者**: Tech Lead, System Architect
**查閱時機**: 遇到技術選型或設計問題時

**使用方式**:
```
問題: Agent 狀態應該存在哪裡?

查閱: ADR-006-agent-state-management.md
獲得:
- 決策: Hybrid 模式 (Redis + PostgreSQL)
- 理由: 短期執行用 Redis,長期狀態用 PostgreSQL
- 實作細節: TTL 30 分鐘
```

#### 3.4 數據庫操作時
**使用者**: Backend Developer, DBA
**查閱時機**: 編寫數據庫查詢或遷移腳本時

**使用方式**:
```
建立 Agent:

查閱: database-schema.md
獲得:
- Agents 表結構
- 外鍵關係 (Agents → Users, Agents → Tenants)
- 索引設計
- 必填欄位

編寫 SQL:
INSERT INTO Agents (Id, Name, TenantId, UserId, ...)
VALUES (...)
```

#### 3.5 性能優化時
**使用者**: Performance Engineer, Senior Developer
**查閱時機**: 系統性能不符合目標時

**使用方式**:
```
問題: Agent 執行時間 >5s

查閱: performance-scalability-strategy.md
獲得:
- 性能目標: P95 <5s
- Caching 策略: Redis for Prompt Templates
- 優化建議: Connection Pooling, Async Processing
```

---

### 分類 4: /project-management (專案管理)

**文檔位置**: `docs/project-management/`

**內容 (推測,需確認實際內容)**:
- Sprint 規劃
- 團隊結構與角色
- 開發流程 (Git Workflow, Code Review)
- Definition of Done
- Release Planning

**運用場景**:

#### 4.1 Sprint Planning 時
**使用者**: Scrum Master, Product Owner
**查閱時機**: 每個 Sprint 開始前

**使用方式**:
- 查閱 Sprint Template
- 確認 Sprint 目標格式
- 確認估算方式 (Story Points)

#### 4.2 Daily Standup 時
**使用者**: 開發團隊
**查閱時機**: 每日站會

**使用方式**:
- 查閱 Standup Meeting Format
- 更新 Sprint Backlog

#### 4.3 Code Review 時
**使用者**: Reviewer, Developer
**查閱時機**: Pull Request Review

**使用方式**:
```
查閱: Code Review Checklist
確認:
- [ ] 符合編碼標準
- [ ] 有單元測試
- [ ] 通過 CI 檢查
- [ ] 更新文檔
```

#### 4.4 Release Planning 時
**使用者**: Product Owner, Tech Lead
**查閱時機**: 準備發布時

**使用方式**:
- 查閱 Release Checklist
- 確認 Release Notes 格式

---

### 分類 5: /ux-design (UX 設計)

**文檔位置**: `docs/ux-design/`

**文檔結構**:
```
ux-design/
├── README.md                          # UX 設計總覽
├── user-research/                     # 用戶研究
│   ├── personas.md                    # 3 個用戶畫像
│   └── user-journey-maps.md           # 5 個用戶旅程
├── information-architecture/          # 資訊架構
│   ├── sitemap.md                     # 網站地圖
│   ├── navigation-structure.md        # 導航結構
│   └── content-hierarchy.md           # 內容層次
├── wireframes/                        # 線框圖
│   ├── low-fidelity/                  # 低保真 (12 個畫面)
│   │   ├── 01-dashboard.md
│   │   ├── 02-agent-list.md
│   │   ├── 03-agent-create.md
│   │   ├── 04-agent-detail.md
│   │   ├── 05-conversation.md
│   │   ├── 06-knowledge-base.md
│   │   ├── 07-code-interpreter.md
│   │   ├── 08-text-to-sql.md
│   │   ├── 09-workflow-editor-v2-index.md
│   │   ├── 10-persona-builder.md
│   │   ├── 11-settings.md
│   │   └── 12-monitoring.md
│   └── high-fidelity/                 # 高保真 (Figma, 待完成)
└── design-system/                     # 設計系統
    ├── README.md
    ├── color-palette.md               # 顏色系統
    ├── typography.md                  # 字體系統
    ├── component-library.md           # 組件庫
    ├── design-tokens.md               # Design Tokens
    └── accessibility-guidelines.md    # 無障礙指南
```

**運用場景**:

#### 5.1 前端開發時 ⭐ 核心用途
**使用者**: Frontend Developer, UI Engineer
**查閱時機**: 實作每個畫面時

**使用方式**:
```
開發 Agent 建立頁面 (對應 US 1.1):

查閱順序:
1. wireframes/low-fidelity/03-agent-create.md
   - 查看頁面佈局
   - 理解欄位組織
   - 查看互動流程

2. design-system/component-library.md
   - 選擇適合的組件 (Button, Input, Select, Textarea)
   - 查看組件 Props 定義

3. design-system/color-palette.md
   - 使用正確的顏色 (Primary, Secondary, Success...)
   - 確保顏色對比符合 WCAG

4. design-system/typography.md
   - 使用正確的字體大小和字重
   - Heading, Body, Caption 層次

5. 開始編碼
   - 根據 Wireframe 建立組件結構
   - 使用 Design System 定義的組件和樣式
```

**實際範例**:
```tsx
// 根據 03-agent-create.md Wireframe 和 Design System

import { Input, Select, Textarea, Button } from '@/components/ui';

function AgentCreateForm() {
  return (
    <form>
      {/* Wireframe Section 1: 基本資訊 */}
      <div className="space-y-4">
        <Input
          label="Agent 名稱"
          placeholder="例如: 資料分析助手"
          // 從 design-system/component-library.md 獲得 Props 定義
        />

        {/* Wireframe Section 2: 模型選擇 */}
        <Select
          label="LLM 模型"
          options={['GPT-4', 'GPT-4o', 'GPT-4o-mini']}
          // 從 Wireframe 獲得選項
        />

        {/* Wireframe Section 3: System Prompt */}
        <Textarea
          label="System Prompt"
          rows={6}
          // 從 Wireframe 獲得元件類型和大小
        />
      </div>

      {/* Wireframe Section 4: 操作按鈕 */}
      <div className="flex justify-end gap-2">
        <Button variant="outline">取消</Button>
        <Button variant="primary">建立 Agent</Button>
        {/* 從 design-system/color-palette.md 獲得 variant 顏色 */}
      </div>
    </form>
  );
}
```

#### 5.2 UI Review 時
**使用者**: UI/UX Designer, Product Owner
**查閱時機**: UI 實作完成後

**使用方式**:
```
Review Agent 建立頁面:

查閱: wireframes/low-fidelity/03-agent-create.md
檢查:
- [ ] 頁面佈局與 Wireframe 一致
- [ ] 所有欄位都存在
- [ ] 互動流程正確 (預覽功能, 驗證提示)
- [ ] 顏色符合 Design System
- [ ] 字體大小正確
```

#### 5.3 Accessibility 測試時
**使用者**: QA Engineer, Accessibility Specialist
**查閱時機**: 無障礙測試時

**使用方式**:
```
查閱: design-system/accessibility-guidelines.md

測試:
- [ ] 顏色對比 ≥ 4.5:1 (WCAG AA)
- [ ] 鍵盤導航完整
- [ ] Screen Reader 友好
- [ ] Focus 狀態明確
```

#### 5.4 用戶體驗優化時
**使用者**: Product Owner, UX Researcher
**查閱時機**: 用戶反饋分析時

**使用方式**:
```
用戶反饋: Agent 建立流程太複雜

查閱:
1. user-research/user-journey-maps.md
   - 查看 "建立 Agent" 用戶旅程
   - 理解用戶期望的流程

2. user-research/personas.md
   - 確認目標用戶 (Agent Developer, Data Analyst)
   - 理解用戶技能水平

3. 優化建議
   - 簡化流程步驟
   - 提供更多預設範本
```

---

### 分類 6: /technical-implementation (技術實施)

**文檔位置**: `docs/technical-implementation/`

**文檔結構** (根據之前的工作):
```
technical-implementation/
├── IMPLEMENTATION-STRUCTURE.md        # 實施結構總覽
├── TECH-STACK-ANALYSIS.md             # 技術棧分析
├── STAGE-3.4-EXECUTION-PLAN.md        # Stage 3.4 執行計劃
├── 1-poc-validation/                  # Phase 1: PoC 驗證
├── 2-dev-environment/                 # Phase 2: 開發環境
├── 3-project-structure/               # Phase 3: 專案結構
├── 4-coding-standards/                # Phase 4: 編碼標準
├── 5-api-design/                      # Phase 5: API 設計
├── 6-database-standards/              # Phase 6: 數據庫標準
├── 7-testing-strategy/                # Phase 7: 測試策略
├── 8-deployment-architecture/         # Phase 8: 部署架構
├── 9-security-standards/              # Phase 9: 安全標準
├── 10-monitoring-operations/          # Phase 10: 監控運維
├── 11-disaster-recovery/              # Phase 11: 災難恢復
└── 12-cost-estimation/                # Phase 12: 成本估算
```

**運用場景**:

#### 6.1 環境設置時
**使用者**: DevOps Engineer, Developer
**查閱時機**: 新人 Onboarding, 環境配置

**使用方式**:
```
設置開發環境:

查閱: 2-dev-environment/ 底下的所有文檔
獲得:
- 所需軟體清單 (Node.js, .NET, Docker...)
- 安裝步驟 (Windows/macOS/Linux)
- IDE 配置 (VSCode 擴展, 設定檔)
- 本地服務啟動 (Docker Compose)
- 環境變數配置
```

#### 6.2 編寫代碼時
**使用者**: Developer
**查閱時機**: 開發過程中

**使用方式**:
```
編寫 C# 代碼:

查閱: 4-coding-standards/ (待補充 ISSUE-002)
獲得:
- 命名規範 (PascalCase for classes, camelCase for variables)
- 代碼組織 (文件夾結構, 命名空間)
- 最佳實踐 (SOLID 原則, Async/Await 使用)
- 禁止事項 (避免的反模式)

編寫 TypeScript/React 代碼:

查閱: 4-coding-standards/ (待補充 ISSUE-003)
獲得:
- 命名規範 (PascalCase for components, camelCase for functions)
- 組件設計 (Functional components, Hooks 使用)
- 狀態管理 (Zustand patterns)
- 最佳實踐 (Props typing, Error boundaries)
```

#### 6.3 API 開發時
**使用者**: Backend Developer
**查閱時機**: 實作 RESTful API 時

**使用方式**:
```
開發 API:

查閱: 5-api-design/ (待補充 ISSUE-004)
獲得:
- RESTful 設計原則
- URL 命名規範 (/api/v1/agents, /api/v1/plugins)
- HTTP 方法使用 (GET, POST, PUT, DELETE, PATCH)
- 狀態碼標準 (200, 201, 400, 401, 404, 500)
- Request/Response 格式
- 錯誤處理規範
- 分頁與排序
- API 版本管理

查閱: architecture/openapi-specification.yaml (待補充)
獲得:
- 完整的 OpenAPI Spec
- Request/Response Schema
```

#### 6.4 數據庫操作時
**使用者**: Backend Developer, DBA
**查閱時機**: 編寫數據庫遷移或查詢時

**使用方式**:
```
查閱: 6-database-standards/ (待補充 ISSUE-006)
獲得:
- Table 命名規範 (PascalCase, 複數形式: Agents, Users)
- Column 命名規範 (PascalCase: AgentId, CreatedAt)
- Index 命名規範 (IX_{TableName}_{ColumnName})
- Foreign Key 命名規範 (FK_{TableName}_{ReferencedTable})
- 遷移腳本規範
- 查詢優化指南
```

#### 6.5 測試時
**使用者**: Developer, QA Engineer
**查閱時機**: 編寫測試時

**使用方式**:
```
查閱: 7-testing-strategy/
獲得:
- 測試金字塔 (60% Unit, 30% Integration, 10% E2E)
- 單元測試規範 (xUnit/NUnit for C#, Jest for TypeScript)
- 整合測試規範 (TestContainers)
- E2E 測試規範 (Playwright)
- 測試覆蓋率目標 (≥80% Unit, ≥60% Integration)
- Mock/Stub 使用指南
```

#### 6.6 部署時
**使用者**: DevOps Engineer
**查閱時機**: 部署到各環境時

**使用方式**:
```
查閱: 8-deployment-architecture/
獲得:
- Docker Compose 配置 (開發環境)
- Kubernetes Manifests (生產環境)
- 環境變數管理 (Dev/Staging/Prod)
- CI/CD Pipeline 配置
- 部署檢查清單
```

#### 6.7 安全實施時
**使用者**: Security Engineer, Backend Developer
**查閱時機**: 實作安全功能時

**使用方式**:
```
查閱: 9-security-standards/
獲得:
- 認證授權實施 (OAuth 2.0, JWT)
- 密鑰管理 (Azure Key Vault)
- 輸入驗證與清理
- SQL Injection 防護
- XSS 防護
- CSRF 防護
- Code Interpreter 安全 (4 層防護)
```

#### 6.8 監控時
**使用者**: DevOps Engineer, SRE
**查閱時機**: 設置監控和告警時

**使用方式**:
```
查閱: 10-monitoring-operations/
獲得:
- Prometheus metrics 定義
- Grafana dashboard 配置
- 日誌收集策略 (ELK/Loki)
- 告警規則 (Critical/Warning/Info)
- SLI/SLO 定義
```

#### 6.9 災難恢復時
**使用者**: DevOps Engineer, DBA
**查閱時機**: 災難恢復規劃和演練時

**使用方式**:
```
查閱: 11-disaster-recovery/
獲得:
- RTO/RPO 目標
- 備份策略 (PostgreSQL, Redis)
- 恢復流程 (Step-by-step)
- 災難演練計劃
```

#### 6.10 成本優化時
**使用者**: Product Owner, DevOps Engineer
**查閱時機**: 成本審查和優化時

**使用方式**:
```
查閱: 12-cost-estimation/
獲得:
- Azure 服務定價
- 成本優化策略 (Reserved Instances, Spot Instances)
- 資源擴展經濟學
- 成本監控告警設置
- 財務預測模型
```

---

### 分類 7: /poc-projects (PoC 實作代碼)

**文檔位置**: `C:\AI Semantic Kernel\poc-projects\`

**內容結構**:
```
poc-projects/
├── poc1-sk-agents/                    # PoC 1: Semantic Kernel Agents
├── poc2-persona-builder/              # PoC 2: Persona Builder
├── poc3-code-interpreter/             # PoC 3: Code Interpreter
├── poc4-text-to-sql/                  # PoC 4: Text-to-SQL
├── poc5-knowledge-rag/                # PoC 5: Knowledge RAG
└── poc6-vueflow-crdt/                 # PoC 6: VueFlow CRDT
```

**運用場景**:

#### 7.1 技術參考時 ⭐ 核心用途
**使用者**: Developer
**查閱時機**: 實作相同或類似功能時

**使用方式**:
```
開發 Agent 執行引擎:

查閱: poc-projects/poc1-sk-agents/
獲得:
- 完整可運行的代碼範例
- Semantic Kernel 整合方式
- Plugin 註冊模式
- Agent 執行流程
- 錯誤處理方式

複製可用代碼:
- AgentExecutionService.cs (可直接複製)
- PluginManager.cs (可參考架構)
- 配置文件 (appsettings.json)
```

#### 7.2 問題排查時
**使用者**: Developer, Tech Lead
**查閱時機**: 遇到技術問題時

**使用方式**:
```
問題: Semantic Kernel Plugin 註冊失敗

查閱: poc-projects/poc1-sk-agents/
獲得:
- 可運行的 Plugin 註冊範例
- 除錯日誌
- 常見錯誤處理

對比:
- PoC 代碼 vs 生產代碼
- 找出差異
```

#### 7.3 Code Review 時
**使用者**: Senior Developer, Tech Lead
**查閱時機**: Review 相關功能的 PR 時

**使用方式**:
```
Review Agent 執行邏輯:

查閱: poc-projects/poc1-sk-agents/
對比:
- PR 實作 vs PoC 驗證的方式
- 確保沒有偏離已驗證的模式
- 檢查是否有額外的優化
```

#### 7.4 性能基準時
**使用者**: Performance Engineer
**查閱時機**: 性能測試和優化時

**使用方式**:
```
查閱: poc-validation/POC-VALIDATION-REPORT.md
獲得 PoC 性能基準:
- PoC 3: P95 <8s (Code Interpreter)
- PoC 6: 60 FPS, <200ms latency (VueFlow)

對比生產實作:
- 是否達到或超過 PoC 性能?
- 如果不達標,查閱 PoC 代碼找出原因
```

---

### 分類 8: /poc-validation (PoC 驗證報告)

**文檔位置**: `C:\AI Semantic Kernel\poc-validation\`

**內容**:
- POC-VALIDATION-REPORT.md (完整驗證報告)
- 每個 PoC 的詳細驗證文檔

**運用場景**:

#### 8.1 技術決策時
**使用者**: Tech Lead, System Architect
**查閱時機**: 技術選型或架構決策時

**使用方式**:
```
問題: 是否應該使用 VueFlow 還是 ReactFlow?

查閱: poc-validation/POC-6-vueflow-crdt/
獲得:
- PoC 6 驗證結果: 98.2% 質量
- 性能測試: 60 FPS, <200ms 延遲
- CRDT 整合: Yjs 完美配合
- 決策: VueFlow 已驗證可行,選擇 VueFlow
```

#### 8.2 風險評估時
**使用者**: Product Owner, Tech Lead
**查閱時機**: Sprint Planning, 功能評估時

**使用方式**:
```
問題: Code Interpreter 在 MVP 的風險?

查閱: poc-validation/POC-VALIDATION-REPORT.md → PoC 3
獲得:
- 驗證狀態: 實測完成 ✅
- 安全分數: 90/100 (需強化到 98)
- 風險: 需要 Seccomp + AppArmor 強化
- 結論: 技術可行,但需要安全審計 (ISSUE-011)
```

#### 8.3 估算工作量時
**使用者**: Scrum Master, Developer
**查閱時機**: Story 估算時

**使用方式**:
```
估算 US 5.2 (Knowledge 檢索):

查閱: poc-validation/POC-VALIDATION-REPORT.md → PoC 5
獲得:
- PoC 開發時間: X 小時
- 技術複雜度: 中等
- 已驗證可行性: 是
- 估算: PoC 時間 × 2 = 生產實作時間 (考慮測試和重構)
```

#### 8.4 測試驗收時
**使用者**: QA Engineer
**查閱時機**: 功能驗收測試時

**使用方式**:
```
驗收 Code Interpreter:

查閱: poc-validation/POC-VALIDATION-REPORT.md → PoC 3
獲得驗收基準:
- Python 代碼執行: 成功 ✅
- 安全沙箱: 4 層防護 ✅
- 性能: P95 <8s ✅
- 安全分數: 90/100 ⚠️ (需達到 98)

驗收測試:
- [ ] Python 代碼執行成功
- [ ] 4 層安全防護啟用
- [ ] 性能達標
- [ ] 安全分數 ≥ 98 (需安全審計)
```

---

## 🔄 文件交叉引用流程

### 開發流程中的文件查閱順序

#### 流程 1: 開發單一 User Story

```
Step 1: 理解需求
├─ 查閱: user-stories/epics/epic-XX.md
├─ 理解: User Story 完整描述, 驗收標準, 技術要求
└─ 輸出: 明確的開發目標

Step 2: 查看設計
├─ 查閱: ux-design/wireframes/XX.md
├─ 理解: 頁面佈局, 互動流程, 組件使用
└─ 輸出: UI/UX 實作指南

Step 3: 查看架構
├─ 查閱: architecture/Architecture-Design-Document.md
├─ 查閱: architecture/database-schema.md
├─ 查閱: 相關 ADR
├─ 理解: 系統設計, 數據模型, 技術決策
└─ 輸出: 技術實作方案

Step 4: 查看 PoC 範例
├─ 查閱: poc-projects/poc-X/
├─ 理解: 已驗證的實作方式
└─ 輸出: 可複製的代碼範例

Step 5: 查看技術標準
├─ 查閱: technical-implementation/4-coding-standards/
├─ 查閱: technical-implementation/5-api-design/
├─ 查閱: technical-implementation/6-database-standards/
├─ 理解: 編碼規範, API 設計規範, 數據庫規範
└─ 輸出: 符合標準的實作

Step 6: 開發與測試
├─ 編寫代碼 (遵循 Coding Standards)
├─ 編寫測試 (遵循 Testing Strategy)
└─ 通過 Code Review (遵循 Code Review Checklist)

Step 7: 驗收
├─ 查閱: user-stories/epics/epic-XX.md (驗收標準)
├─ 執行驗收測試
└─ 標記 Story 為 Done
```

---

#### 流程 2: Sprint Planning

```
Step 1: 確定 Sprint 目標
├─ 查閱: user-stories/mvp-planning.md
├─ 理解: 當前 Sprint 應完成哪個 Epic
└─ 輸出: Sprint Goal

Step 2: 選擇 User Stories
├─ 查閱: user-stories/epics/epic-XX.md
├─ 選擇: 該 Epic 下的 Stories
└─ 輸出: Sprint Backlog

Step 3: 估算工作量
├─ 查閱: 每個 Story 的技術要求
├─ 查閱: poc-validation/ (參考 PoC 開發時間)
├─ 估算: Story Points
└─ 輸出: Sprint Capacity

Step 4: 任務分解
├─ 查閱: architecture/ (理解技術依賴)
├─ 查閱: ux-design/ (理解 UI 依賴)
├─ 分解: Story → Tasks
└─ 輸出: Sprint Task Board

Step 5: 團隊承諾
├─ 查閱: project-management/ (團隊 Capacity)
├─ 確認: 團隊可完成 Sprint Backlog
└─ 輸出: Sprint Commitment
```

---

#### 流程 3: Code Review

```
Step 1: 檢查需求符合度
├─ 查閱: user-stories/epics/epic-XX.md (驗收標準)
├─ 檢查: PR 是否滿足所有驗收標準
└─ 輸出: ✅/❌ 需求符合度

Step 2: 檢查設計符合度
├─ 查閱: ux-design/wireframes/XX.md
├─ 檢查: UI 實作是否與設計一致
└─ 輸出: ✅/❌ 設計符合度

Step 3: 檢查架構符合度
├─ 查閱: architecture/ (ADRs, Database Schema)
├─ 檢查: 實作是否遵循架構決策
└─ 輸出: ✅/❌ 架構符合度

Step 4: 檢查代碼質量
├─ 查閱: technical-implementation/4-coding-standards/
├─ 檢查: 命名, 組織, 最佳實踐
└─ 輸出: ✅/❌ 代碼質量

Step 5: 檢查測試覆蓋
├─ 查閱: technical-implementation/7-testing-strategy/
├─ 檢查: 測試覆蓋率 ≥ 目標
└─ 輸出: ✅/❌ 測試覆蓋

Step 6: 最終決策
├─ 所有檢查通過 → Approve
├─ 有問題 → Request Changes
└─ 輸出: PR 狀態
```

---

## 📊 文件優先級矩陣

### 不同角色的核心文件

| 角色 | 核心文件 (必讀) | 參考文件 (需要時查閱) |
|------|----------------|---------------------|
| **Product Owner** | • Brief<br>• User Stories<br>• MVP Planning | • Architecture<br>• UX Design<br>• PoC Validation |
| **Scrum Master** | • User Stories<br>• Project Management<br>• Sprint Planning | • MVP Planning<br>• Technical Implementation |
| **Frontend Developer** | • User Stories (epics)<br>• UX Design (wireframes, design system)<br>• Coding Standards | • Architecture (API design)<br>• PoC Projects (poc6) |
| **Backend Developer** | • User Stories (epics)<br>• Architecture (ADRs, Database Schema)<br>• Coding Standards | • PoC Projects (poc1-5)<br>• API Design<br>• Security Standards |
| **System Architect** | • Architecture<br>• ADRs<br>• Technical Implementation | • PoC Validation<br>• Performance Strategy<br>• Brief |
| **DevOps Engineer** | • Deployment Architecture<br>• Monitoring Operations<br>• Technical Implementation | • Disaster Recovery<br>• Security Standards<br>• Cost Estimation |
| **QA Engineer** | • User Stories (驗收標準)<br>• Testing Strategy<br>• UX Design (測試場景) | • PoC Validation (基準)<br>• Architecture (系統理解) |
| **UI/UX Designer** | • UX Design<br>• User Research<br>• Wireframes | • User Stories<br>• Brief (商業目標) |

---

## 🎯 最佳實踐

### 1. 文件查閱原則

**從大到小**:
Brief → User Stories → Architecture → Technical Implementation → PoC

**從抽象到具體**:
商業目標 → 功能需求 → 系統設計 → 實作細節 → 代碼範例

**從"為什麼"到"如何做"**:
Brief (Why) → User Stories (What) → Architecture (How in principle) → Technical Implementation (How in detail) → PoC (How in code)

### 2. 文件更新原則

**變更時同步更新**:
- User Stories 變更 → 更新相關 Architecture, UX Design
- Architecture 變更 → 更新相關 Technical Implementation
- PoC 發現新模式 → 更新 Architecture, Technical Implementation

**版本控制**:
- 所有文件納入 Git 版本控制
- 重要變更建立 Git Tag
- 變更歷史記錄在文件頂部

### 3. 文件檢索技巧

**全文搜索** (推薦工具):
- VSCode: Ctrl+Shift+F (全局搜索)
- grep: `grep -r "keyword" docs/`
- Ripgrep (更快): `rg "keyword" docs/`

**文件導航**:
- 使用各文件夾的 README.md 作為導航
- 建立 docs/README.md 作為總索引 (推薦)

**交叉引用**:
- 文件間使用相對路徑連結
- 範例: `[Architecture](../architecture/Architecture-Design-Document.md)`

---

## ✅ 總結

### 核心文件運用原則

1. **Brief**: 商業決策和產品方向的北極星
2. **User Stories**: 開發和測試的驗收標準
3. **UX Design**: 前端實作的視覺規範
4. **Architecture**: 系統設計和技術決策的權威
5. **Technical Implementation**: 實作細節和標準的指南
6. **PoC Projects**: 已驗證的代碼範例和參考實作
7. **PoC Validation**: 技術可行性和性能基準的證據
8. **Project Management**: 團隊協作和流程的規範

### 文件查閱快速參考

| 問題 | 查閱文件 |
|------|---------|
| 這個功能為什麼要做? | Brief.md |
| 這個功能要做什麼? | User Stories → Epic |
| 這個畫面長什麼樣? | UX Design → Wireframes |
| 這個組件用什麼顏色? | UX Design → Design System |
| 這個 API 怎麼設計? | Architecture → API Design |
| 這個數據怎麼存? | Architecture → Database Schema |
| 這個技術決策為什麼這樣選? | Architecture → ADR |
| 這段代碼怎麼寫? | PoC Projects → 對應 PoC |
| 這個功能工作量多少? | PoC Validation → 對應 PoC |
| 代碼規範是什麼? | Technical Implementation → Coding Standards |
| API 規範是什麼? | Technical Implementation → API Design |
| 測試要怎麼寫? | Technical Implementation → Testing Strategy |
| 環境要怎麼設置? | Technical Implementation → Dev Environment |
| 部署要怎麼做? | Technical Implementation → Deployment Architecture |

---

**文檔版本**: v1.0.0
**最後更新**: 2025-11-02
**下次更新**: 開始開發後持續更新
