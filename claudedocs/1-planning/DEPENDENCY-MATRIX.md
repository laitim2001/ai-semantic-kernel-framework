# 依賴關係矩陣 (Dependency Matrix)

**Semantic Kernel Agentic Framework - Cross-Sprint Dependency Matrix**

**版本**: 2.0
**創建日期**: 2025-12-11
**最後更新**: 2025-12-11
**維護責任**: Scrum Master + Tech Lead
**狀態**: 🟢 Active

[返回 claudedocs 主頁](../README.md) | [MVP 範圍](./MVP-SCOPE-DEFINITION.md) | [架構演進](./ARCHITECTURE-EVOLUTION-ROADMAP.md) | [技術決策](./TECHNICAL-DECISIONS-LOG.md)

---

## 📋 文檔目的

本文檔提供專案完整依賴關係視圖，基於 [docs/user-stories/README.md](../../docs/user-stories/README.md) 的 43 個 User Stories 和 [docs/architecture/Architecture-Design-Document.md](../../docs/architecture/Architecture-Design-Document.md) 的架構設計，包括：

- 🔗 **Sprint 依賴**: 跨 Sprint 的前置/後置依賴關係
- 📦 **模組依賴**: 6 個核心模組的技術依賴
- 🎯 **User Story 依賴**: 43 個 User Story 的詳細依賴鏈
- 🚨 **關鍵路徑**: 專案關鍵路徑和瓶頸識別
- ⚠️ **風險依賴**: 高風險依賴項和緩解策略

---

## 🎯 依賴關係總覽

### 依賴類型定義

參考 [docs/technical-implementation/sprint-planning/sprint-dependencies.md](../../docs/technical-implementation/sprint-planning/sprint-dependencies.md) 的依賴分類：

| 類型 | 符號 | 說明 | 影響 |
|-----|------|------|------|
| **Must Finish-to-Start** | `→` | 必須完成前置才能開始 | 🔴 阻斷性 |
| **Should Finish-to-Start** | `⇢` | 建議完成前置才開始 | 🟡 建議性 |
| **Parallel** | `∥` | 可並行開發 | 🟢 無阻斷 |
| **Integration** | `⊕` | 需要整合點 | 🟡 同步點 |
| **API Contract** | `⟷` | API 契約依賴 | 🟡 介面穩定 |

### 依賴統計

| 依賴類別 | 總數 | Critical | High | Medium | Low |
|---------|-----|---------|------|--------|-----|
| **Sprint 依賴** | 42 | 8 | 15 | 12 | 7 |
| **模組依賴** | 18 | 6 | 7 | 3 | 2 |
| **User Story 依賴** | 67 | 12 | 28 | 18 | 9 |
| **技術依賴** | 25 | 5 | 10 | 7 | 3 |
| **總計** | **152** | **31** | **60** | **40** | **21** |

---

## 📊 Sprint 依賴矩陣

### Phase 1A: 基礎平台 (Sprint 1-6)

參考 [docs/user-stories/modules/module-01-agent-management.md](../../docs/user-stories/modules/module-01-agent-management.md) 的 Agent 管理模組設計：

```
Sprint 1 (US 1.1, 1.2, 1.3) - 基礎設施與 Agent 創建
  │
  ├─→ Sprint 2 (US 1.4, 2.1, 6.1) - Agent 執行與 Plugin 系統
  │     ├─ 依賴: Agent CRUD API (US 1.1) ✅
  │     ├─ 依賴: Agent 數據模型 (US 1.3) ✅
  │     ├─ 參考: [docs/architecture/Architecture-Design-Document.md](../../docs/architecture/Architecture-Design-Document.md) Section 3.1
  │     └─ 影響: Sprint 2 延遲 6 days ⚠️
  │
  ├─→ Sprint 3 (US 1.5) - Agent 高級配置
  │     ├─ 依賴: Agent 執行引擎 (US 1.4) 🔴
  │     ├─ 依賴: Plugin 系統基礎 (US 2.1) 🔴
  │     └─ 參考: [docs/user-stories/modules/module-02-plugin-system.md](../../docs/user-stories/modules/module-02-plugin-system.md)
  │
  ├─→ Sprint 4 (US 7.1, 7.2) - Persona 系統
  │     ├─ 依賴: Agent CRUD (US 1.1) ✅
  │     ├─ 依賴: Plugin Registry (US 2.1) ⚠️
  │     └─ 參考: [docs/user-stories/modules/module-07-persona-framework.md](../../docs/user-stories/modules/module-07-persona-framework.md)
  │
  ├─→ Sprint 5 (US 5.1, 5.3) - Knowledge 檢索
  │     ├─ 依賴: Agent 執行引擎 (US 1.4) 🔴
  │     ├─ 依賴: Vector DB 選型 (Sprint 4) ⚠️
  │     └─ 參考: [docs/user-stories/modules/module-08-knowledge-management.md](../../docs/user-stories/modules/module-08-knowledge-management.md)
  │
  └─→ Sprint 6 (US 5.2) - Knowledge 進階功能
        ├─ 依賴: Knowledge 檢索基礎 (US 5.1) 🔴
        ├─ 準備: 工作流引擎 Spike 🟡
        └─ 參考: [docs/technical-implementation/backend/rag-implementation.md](../../docs/technical-implementation/backend/rag-implementation.md)
```

### Phase 1B: 工作流引擎核心 + 抽象層 (Sprint 7-9)

參考 [docs/user-stories/modules/module-04-multi-agent-workflow.md](../../docs/user-stories/modules/module-04-multi-agent-workflow.md) 的完整工作流引擎設計：

```
Sprint 7-9 (工作流引擎核心) - 🔥 Critical Path
  │
  ├─ 前置依賴:
  │   ├─→ Agent 執行引擎 (US 1.4) 🔴 必須
  │   ├─→ Plugin 系統 (US 2.1) 🔴 必須
  │   ├─→ Persona 系統 (US 7.1) 🟡 建議
  │   ├─→ Sprint 6 技術 Spike 🟡 建議
  │   └─ 參考: [docs/architecture/Architecture-Design-Document.md](../../docs/architecture/Architecture-Design-Document.md) Section 5 Workflow Architecture
  │
  ├─ 組件依賴:
  │   ├─ Task Generator (5-8 SP)
  │   │   ├─ 依賴: LLM API 整合 🔴
  │   │   └─ 參考: [docs/technical-implementation/backend/workflow-engine-implementation.md](../../docs/technical-implementation/backend/workflow-engine-implementation.md) Section 2.1
  │   │
  │   ├─ Multi-Agent Coordination (8-10 SP)
  │   │   ├─ 依賴: Task Generator 🔴
  │   │   ├─ 依賴: Agent Messaging 🔴
  │   │   └─ 參考: [docs/technical-implementation/backend/workflow-engine-implementation.md](../../docs/technical-implementation/backend/workflow-engine-implementation.md) Section 2.2
  │   │
  │   ├─ Agent Messaging (5 SP)
  │   │   ├─ 依賴: Message Bus 設計 🟡
  │   │   └─ 參考: [docs/architecture/ADR-007-event-bus-strategy.md](../../docs/architecture/ADR-007-event-bus-strategy.md)
  │   │
  │   ├─ Workflow Execution Engine (8-10 SP)
  │   │   ├─ 依賴: Multi-Agent Coordination 🔴
  │   │   ├─ 依賴: State Management 基礎 🟡
  │   │   └─ 參考: [docs/technical-implementation/backend/workflow-engine-implementation.md](../../docs/technical-implementation/backend/workflow-engine-implementation.md) Section 2.4
  │   │
  │   └─ Framework Abstraction Layer (5-8 SP)
  │       ├─ 依賴: IAgentRuntime 設計 🟡
  │       ├─ 並行開發: ∥ 與其他組件
  │       └─ 參考: [docs/architecture/ADR-011-framework-migration-strategy.md](../../docs/architecture/ADR-011-framework-migration-strategy.md)
  │
  └─ 後續影響:
      ├─→ Sprint 10-14 (Workflow Editor) 🔴 阻斷
      ├─→ Sprint 15-16 (進階工作流) 🔴 阻斷
      └─→ Sprint 17-21 (所有依賴工作流的功能) 🔴 阻斷
```

**關鍵路徑警示**: Sprint 7-9 是整個專案的關鍵瓶頸，任何延遲將直接影響後續 12 個 Sprint！

**技術參考**:
- [docs/user-stories/modules/module-04-multi-agent-workflow.md](../../docs/user-stories/modules/module-04-multi-agent-workflow.md) - 完整工作流設計
- [docs/technical-implementation/backend/workflow-engine-implementation.md](../../docs/technical-implementation/backend/workflow-engine-implementation.md) - 實現指南
- [docs/architecture/ADR-007-event-bus-strategy.md](../../docs/architecture/ADR-007-event-bus-strategy.md) - 消息傳遞策略

### Phase 1C: Code Interpreter (Sprint 10)

參考 [docs/user-stories/modules/module-09-code-interpreter.md](../../docs/user-stories/modules/module-09-code-interpreter.md) 的 Code Interpreter 設計：

```
Sprint 10 (US 3.1, 3.2, 3.3) - Code Interpreter
  │
  ├─ 前置依賴:
  │   ├─→ Workflow Engine 基礎 (Sprint 7-9) 🔴 必須
  │   ├─→ Agent 執行引擎 (US 1.4) 🔴 必須
  │   ├─→ Plugin 系統 (US 2.1) 🟡 建議
  │   └─ 參考: [docs/security/code-execution-security.md](../../docs/security/code-execution-security.md)
  │
  ├─ 技術依賴:
  │   ├─ Docker Container Pool 設計 🔴
  │   ├─ Security Sandbox 實現 🔴
  │   ├─ Resource Limit 配置 🟡
  │   └─ 參考: [docs/technical-implementation/backend/code-interpreter-implementation.md](../../docs/technical-implementation/backend/code-interpreter-implementation.md)
  │
  └─ 整合點: ⊕ Workflow Node 類型擴展
      └─ 參考: [docs/architecture/Architecture-Design-Document.md](../../docs/architecture/Architecture-Design-Document.md) Section 7.3
```

**安全設計參考**:
- [docs/security/code-execution-security.md](../../docs/security/code-execution-security.md) - 4 層安全防護設計
- [docs/deployment/docker-setup.md](../../docs/deployment/docker-setup.md) - Container Pool 部署
- [docs/technical-implementation/backend/code-interpreter-implementation.md](../../docs/technical-implementation/backend/code-interpreter-implementation.md) - 實現細節

### Phase 1D: Workflow Editor Backend (Sprint 11-12)

參考 [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md) 的完整 Workflow Editor V2 設計：

```
Sprint 11-12 (Workflow Editor Backend) - 15-20 SP
  │
  ├─ 前置依賴:
  │   ├─→ Workflow Engine 核心 (Sprint 7-9) 🔴 阻斷性
  │   ├─→ Node Registry 設計 🔴 必須
  │   ├─→ Workflow Definition Schema 🔴 必須
  │   └─ 參考: [docs/api/workflow-api-specification.md](../../docs/api/workflow-api-specification.md)
  │
  ├─ 組件依賴:
  │   ├─ Workflow Definition CRUD (5 SP)
  │   │   ├─ 依賴: PostgreSQL Schema 🔴
  │   │   └─ 參考: [docs/api/database-schema.md](../../docs/api/database-schema.md) Workflow Tables
  │   │
  │   ├─ Node Registry (3 SP)
  │   │   ├─ 依賴: Agent 節點註冊 🔴
  │   │   ├─ 依賴: Tool 節點註冊 🔴
  │   │   └─ 參考: [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part4.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part4.md) 28 種節點類型
  │   │
  │   ├─ Edge Validation (3 SP)
  │   │   └─ 依賴: Node Registry 🔴
  │   │
  │   ├─ Workflow Execution Trigger (2 SP)
  │   │   └─ 依賴: Workflow Engine API 🔴
  │   │
  │   ├─ Workflow Template Management (2 SP)
  │   │   └─ 依賴: Workflow CRUD 🔴
  │   │
  │   └─ Version Control (2-3 SP)
  │       └─ 依賴: Git-like 版本設計 🟡
  │
  └─ API 契約: ⟷ Frontend Editor 需要穩定 API
      └─ 參考: [docs/api/workflow-api-specification.md](../../docs/api/workflow-api-specification.md)
```

**API 設計參考**:
- [docs/api/workflow-api-specification.md](../../docs/api/workflow-api-specification.md) - Workflow API 完整規範
- [docs/api/database-schema.md](../../docs/api/database-schema.md) - 資料庫 Schema
- [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part4.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part4.md) - 28 種節點類型設計

### Phase 1D: Workflow Editor Frontend (Sprint 13-14)

參考 [docs/PROJECT-INITIALIZATION-DECISION.md](../../docs/PROJECT-INITIALIZATION-DECISION.md) 的 VueFlow 技術選型：

```
Sprint 13-14 (Workflow Editor Frontend) - 20-25 SP
  │
  ├─ 前置依賴:
  │   ├─⟷ Backend API 穩定 (Sprint 11-12) 🔴 API 契約
  │   ├─⇢ Node Registry API (Sprint 11) 🟡 建議完成
  │   ├─⇢ Workflow Template API (Sprint 11) 🟡 建議完成
  │   └─ 參考: [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md)
  │
  ├─ 組件依賴:
  │   ├─ VueFlow 集成 (5 SP)
  │   │   ├─ 前置: VueFlow 培訓 (Sprint 9) ⚠️
  │   │   └─ 參考: [poc-projects/poc6-vueflow-crdt/](../../poc-projects/poc6-vueflow-crdt/) PoC 6 驗證
  │   │
  │   ├─ Drag-drop Node Editor (4 SP)
  │   │   ├─ 依賴: VueFlow 集成 🔴
  │   │   └─ 參考: [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part2.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part2.md) 內聯配置
  │   │
  │   ├─ Node Configuration Panel (3 SP)
  │   │   ├─ 依賴: Node Registry API 🔴
  │   │   ├─ 依賴: Drag-drop Editor 🔴
  │   │   └─ 參考: [docs/ux-design/design-system/component-library.md](../../docs/ux-design/design-system/component-library.md)
  │   │
  │   ├─ Connection Line Drawing (3 SP)
  │   │   └─ 依賴: VueFlow 集成 🔴
  │   │
  │   ├─ Workflow Canvas (3 SP)
  │   │   ├─ 依賴: VueFlow 集成 🔴
  │   │   ├─ Zoom, Pan, Mini-map 功能 🟡
  │   │   └─ 參考: [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part5.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part5.md) 畫布互動
  │   │
  │   ├─ Node Template Library (2 SP)
  │   │   └─ 依賴: Template API 🔴
  │   │
  │   └─ Save/Load Workflow (2-3 SP)
  │       ├─ 依賴: Workflow CRUD API 🔴
  │       └─ 依賴: Version Control API 🟡
  │
  └─ 風險依賴: ⚠️ VueFlow 學習曲線 (RISK-004)
      └─ 緩解: [poc-projects/poc6-vueflow-crdt/](../../poc-projects/poc6-vueflow-crdt/) PoC 6 已驗證可行性
```

**UI 設計參考**:
- [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md) - 完整 V2 設計
- [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part1.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part1.md) - 核心設計理念
- [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part2.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part2.md) - n8n 風格內聯配置
- [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part3.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part3.md) - 執行視覺化
- [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part5.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part5.md) - 畫布互動設計
- [poc-projects/poc6-vueflow-crdt/](../../poc-projects/poc6-vueflow-crdt/) - PoC 6 驗證報告（98.2% 質量）

### Phase 1E: 進階工作流引擎 (Sprint 15-16)

```
Sprint 15-16 (進階工作流引擎) - 13-18 SP
  │
  ├─ 前置依賴:
  │   ├─→ Workflow Engine 核心 (Sprint 7-9) 🔴 必須
  │   ├─→ Workflow Editor (Sprint 11-14) 🟡 建議
  │   ├─→ 核心功能驗證 (Sprint 10-14) 🟡 建議
  │   └─ 參考: [docs/user-stories/modules/module-04-multi-agent-workflow.md](../../docs/user-stories/modules/module-04-multi-agent-workflow.md) Section 7
  │
  ├─ 組件依賴:
  │   ├─ Conditional & Loop (5-8 SP)
  │   │   └─ 依賴: Workflow Engine 基礎 🔴
  │   │
  │   ├─ Handoff & Supervisor (3-5 SP)
  │   │   ├─ 依賴: Multi-Agent Coordination 🔴
  │   │   └─ 參考: [docs/technical-implementation/backend/workflow-engine-implementation.md](../../docs/technical-implementation/backend/workflow-engine-implementation.md) Section 3
  │   │
  │   ├─ State Management (5 SP)
  │   │   ├─ Checkpoint/Resume 機制 🔴
  │   │   ├─ 依賴: Workflow Execution Engine 🔴
  │   │   └─ 參考: [docs/architecture/ADR-006-hybrid-state-management.md](../../docs/architecture/ADR-006-hybrid-state-management.md)
  │   │
  │   └─ Feedback Loop (3-5 SP)
  │       └─ 依賴: State Management 🔴
  │
  └─ 整合點: ⊕ 與 Editor 的進階節點整合
```

### Phase 1F: Chat 界面與收尾 (Sprint 17-21)

參考 [docs/ux-design/wireframes/low-fidelity/05-conversation.md](../../docs/ux-design/wireframes/low-fidelity/05-conversation.md) 的 Chat 界面設計：

```
Sprint 17 (US 8.1, 8.2, 8.4) - Chat 界面
  ├─→ 依賴: Agent 執行 API (US 1.4) 🔴
  ├─→ 依賴: Conversation 管理 (US 1.4) 🔴
  ├─⇢ 依賴: Workflow 執行 API (Sprint 7-9) 🟡
  └─ 參考: [docs/ux-design/wireframes/low-fidelity/05-conversation.md](../../docs/ux-design/wireframes/low-fidelity/05-conversation.md)

Sprint 18-19 (US 6.2, 6.3, 6.4) - Text-to-SQL
  ├─→ 依賴: Agent 執行引擎 🔴
  ├─→ 依賴: Plugin 系統 🔴
  ├─⇢ 依賴: Knowledge 檢索 (Sprint 5) 🟡
  └─ 參考: [docs/ux-design/wireframes/low-fidelity/08-text-to-sql.md](../../docs/ux-design/wireframes/low-fidelity/08-text-to-sql.md)

Sprint 20 (US 9.1, 9.4) - 企業功能
  ├─→ 依賴: 所有核心功能完成 🔴
  ├─⇢ 依賴: Admin Panel 設計 🟡
  └─ 參考: [docs/user-stories/modules/module-11-user-authentication.md](../../docs/user-stories/modules/module-11-user-authentication.md)

Sprint 21 (US 10.1, 10.2, P1 收尾) - 管理面板與驗收
  ├─→ 依賴: 所有 P0 User Stories 完成 🔴
  ├─ 整合測試與 UAT ⊕
  └─ 參考: [docs/testing/integration-testing-strategy.md](../../docs/testing/integration-testing-strategy.md)
```

---

## 📦 模組依賴關係圖

### 6 個核心模組依賴

基於 [docs/user-stories/README.md](../../docs/user-stories/README.md) 的 6 個核心模組：

```
Module 01: Agent 創建與管理
  │  參考: [docs/user-stories/modules/module-01-agent-management.md](../../docs/user-stories/modules/module-01-agent-management.md)
  │
  ├─→ Module 02: Plugin 系統
  │     ├─ 依賴: Agent 執行引擎 (M01) 🔴
  │     ├─ API: Plugin Registry ⟷ Agent Executor
  │     └─ 參考: [docs/user-stories/modules/module-02-plugin-system.md](../../docs/user-stories/modules/module-02-plugin-system.md)
  │
  ├─→ Module 07: Persona 系統
  │     ├─ 依賴: Agent CRUD (M01) 🔴
  │     ├─ API: Persona Definition ⟷ Agent Config
  │     └─ 參考: [docs/user-stories/modules/module-07-persona-framework.md](../../docs/user-stories/modules/module-07-persona-framework.md)
  │
  ├─→ Module 04: Multi-Agent 協作 🔥 Critical
  │     ├─ 依賴: Agent 執行引擎 (M01) 🔴
  │     ├─ 依賴: Plugin 系統 (M02) 🔴
  │     ├─ 依賴: Persona 系統 (M07) 🟡
  │     ├─ API: Workflow Engine ⟷ Agent Executor
  │     └─ 參考: [docs/user-stories/modules/module-04-multi-agent-workflow.md](../../docs/user-stories/modules/module-04-multi-agent-workflow.md)
  │
  ├─→ Module 05: Knowledge 檢索
  │     ├─ 依賴: Agent 執行引擎 (M01) 🔴
  │     ├─ API: RAG Service ⟷ Agent Memory
  │     └─ 參考: [docs/user-stories/modules/module-08-knowledge-management.md](../../docs/user-stories/modules/module-08-knowledge-management.md)
  │
  ├─→ Module 03: Code Interpreter
  │     ├─ 依賴: Workflow Engine (M04) 🔴
  │     ├─ 依賴: Agent 執行引擎 (M01) 🔴
  │     ├─ API: Code Execution ⟷ Workflow Node
  │     └─ 參考: [docs/user-stories/modules/module-09-code-interpreter.md](../../docs/user-stories/modules/module-09-code-interpreter.md)
  │
  └─→ Module 06: Text-to-SQL
        ├─ 依賴: Agent 執行引擎 (M01) 🔴
        ├─ 依賴: Plugin 系統 (M02) 🔴
        ├─ 依賴: Knowledge 檢索 (M05) 🟡
        └─ 參考: [docs/ux-design/wireframes/low-fidelity/08-text-to-sql.md](../../docs/ux-design/wireframes/low-fidelity/08-text-to-sql.md)
```

### 模組優先級與關鍵路徑

| 模組 | 優先級 | 關鍵路徑 | 依賴模組數 | 被依賴數 | 參考文檔 |
|-----|-------|---------|-----------|---------|---------|
| **Module 01** (Agent) | P0 | ✅ | 0 | 6 | [module-01-agent-management.md](../../docs/user-stories/modules/module-01-agent-management.md) |
| **Module 04** (Workflow) | P0 | ✅ 🔥 | 3 | 5 | [module-04-multi-agent-workflow.md](../../docs/user-stories/modules/module-04-multi-agent-workflow.md) |
| **Module 02** (Plugin) | P0 | ✅ | 1 | 4 | [module-02-plugin-system.md](../../docs/user-stories/modules/module-02-plugin-system.md) |
| **Module 07** (Persona) | P0 | ⚠️ | 1 | 2 | [module-07-persona-framework.md](../../docs/user-stories/modules/module-07-persona-framework.md) |
| **Module 05** (Knowledge) | P0 | ⚠️ | 1 | 1 | [module-08-knowledge-management.md](../../docs/user-stories/modules/module-08-knowledge-management.md) |
| **Module 03** (Code Interpreter) | P0 | 🟡 | 2 | 0 | [module-09-code-interpreter.md](../../docs/user-stories/modules/module-09-code-interpreter.md) |
| **Module 06** (Text-to-SQL) | P0 | 🟡 | 3 | 0 | [ux wireframe 08-text-to-sql.md](../../docs/ux-design/wireframes/low-fidelity/08-text-to-sql.md) |

**關鍵路徑**: Module 01 → Module 02 → Module 04 → Module 03 → Module 06

---

## 🎯 User Story 依賴鏈

### US 1.x - Agent 創建與管理 (Module 01)

參考 [docs/user-stories/modules/module-01-agent-management.md](../../docs/user-stories/modules/module-01-agent-management.md)：

```
US 1.1 (Agent CRUD) - 5 SP
  ├─ 無前置依賴 (專案起點) ✅
  └─ 參考: [docs/api/agent-api-specification.md](../../docs/api/agent-api-specification.md)

US 1.2 (SDK) - 5 SP
  ├─→ US 1.1 (必須有 API) 🔴
  └─ 參考: [docs/technical-implementation/frontend/api-client-generation.md](../../docs/technical-implementation/frontend/api-client-generation.md)

US 1.3 (Agent 配置管理) - 3 SP
  ├─→ US 1.1 (必須有 Agent 實體) 🔴
  └─ 參考: [docs/api/database-schema.md](../../docs/api/database-schema.md) Agents Table

US 1.4 (Agent 執行與監控) - 5 SP → 13 SP ⚠️
  ├─→ US 1.1 (Agent CRUD) 🔴
  ├─→ US 1.3 (Agent Config) 🔴
  ├─→ Semantic Kernel 整合 🔴
  └─ 參考: [docs/technical-implementation/backend/semantic-kernel-integration.md](../../docs/technical-implementation/backend/semantic-kernel-integration.md)

US 1.5 (Agent 高級配置) - 8 SP
  ├─→ US 1.4 (執行引擎) 🔴
  ├─→ US 2.1 (Plugin 系統) 🔴
  └─ 參考: [docs/user-stories/modules/module-01-agent-management.md](../../docs/user-stories/modules/module-01-agent-management.md) Section 5
```

### US 2.x - Plugin 系統 (Module 02)

參考 [docs/user-stories/modules/module-02-plugin-system.md](../../docs/user-stories/modules/module-02-plugin-system.md)：

```
US 2.1 (註冊 .NET Plugin) - 5 SP
  ├─→ US 1.4 (Agent 執行引擎) 🔴
  ├─ Plugin Loader, Activator, Registry 實現 ✅
  └─ 參考: [docs/technical-implementation/backend/plugin-system-implementation.md](../../docs/technical-implementation/backend/plugin-system-implementation.md)

US 2.2 (Plugin 熱重載) - Phase 1-2 完成 🔄
  ├─→ US 2.1 (Plugin Loader 基礎) 🔴
  ├─ Phase 3-5: API Layer, Tests (預估 2-3 days)
  └─ 參考: [docs/technical-implementation/backend/plugin-hot-reload-implementation.md](../../docs/technical-implementation/backend/plugin-hot-reload-implementation.md)

US 2.3 (Plugin 版本管理) - Phase 1-2 完成 🔄
  ├─→ US 2.1 (PluginVersion 實體) 🔴
  ├─ Phase 3-5: API Layer, Version Switching (預估 3-4 days)
  └─ 參考: [docs/user-stories/modules/module-02-plugin-system.md](../../docs/user-stories/modules/module-02-plugin-system.md) Section 4
```

### US 4.x - Multi-Agent 協作 (Module 04) 🔥

參考 [docs/user-stories/modules/module-04-multi-agent-workflow.md](../../docs/user-stories/modules/module-04-multi-agent-workflow.md) 的完整 34-43 SP 分解：

```
US 4.1 (Multi-Agent Orchestration) - 34-43 SP
  │
  ├─ Phase 1B (Sprint 7-9): 核心引擎 (26 SP)
  │   ├─→ US 1.4 (Agent 執行引擎) 🔴
  │   ├─→ US 2.1 (Plugin 系統) 🔴
  │   ├─⇢ US 7.1 (Persona 系統) 🟡
  │   ├─ Task Generator (5-8 SP)
  │   ├─ Multi-Agent Coordination (8-10 SP)
  │   ├─ Agent Messaging (5 SP)
  │   ├─ Workflow Execution Engine (8-10 SP)
  │   └─ 參考: [docs/technical-implementation/backend/workflow-engine-implementation.md](../../docs/technical-implementation/backend/workflow-engine-implementation.md)
  │
  ├─ Phase 1D (Sprint 11-12): Workflow Editor Backend (15-20 SP)
  │   ├─→ Phase 1B 完成 🔴
  │   ├─ Workflow CRUD, Node Registry, Edge Validation
  │   └─ 參考: [docs/api/workflow-api-specification.md](../../docs/api/workflow-api-specification.md)
  │
  ├─ Phase 1D (Sprint 13-14): Workflow Editor Frontend (20-25 SP)
  │   ├─⟷ Phase 1D Backend API 🔴
  │   ├─ VueFlow, Drag-drop, Canvas, Save/Load
  │   └─ 參考: [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md)
  │
  └─ Phase 1E (Sprint 15-16): 進階工作流 (13-18 SP)
      ├─→ Phase 1B 完成 🔴
      ├─ Conditional, Loop, State Management, Feedback Loop
      └─ 參考: [docs/user-stories/modules/module-04-multi-agent-workflow.md](../../docs/user-stories/modules/module-04-multi-agent-workflow.md) Section 7

US 4.2, 4.3 (其他工作流功能) - 依賴 US 4.1 完成
```

### US 3.x - Code Interpreter (Module 03)

參考 [docs/user-stories/modules/module-09-code-interpreter.md](../../docs/user-stories/modules/module-09-code-interpreter.md)：

```
US 3.1 (Python Code Execution) - 8 SP
  ├─→ US 4.1 Phase 1B (Workflow Engine) 🔴
  ├─→ US 1.4 (Agent 執行引擎) 🔴
  ├─ Docker Sandbox 實現 🔴
  └─ 參考: [docs/security/code-execution-security.md](../../docs/security/code-execution-security.md)

US 3.2, 3.3 (Code Interpreter 進階功能)
  ├─→ US 3.1 (基礎執行能力) 🔴
  └─ 參考: [docs/technical-implementation/backend/code-interpreter-implementation.md](../../docs/technical-implementation/backend/code-interpreter-implementation.md)
```

### US 5.x - Knowledge 檢索 (Module 05)

參考 [docs/user-stories/modules/module-08-knowledge-management.md](../../docs/user-stories/modules/module-08-knowledge-management.md)：

```
US 5.1 (Knowledge 檢索) - 13 SP
  ├─→ US 1.4 (Agent 執行引擎) 🔴
  ├─→ Vector DB 選型 (Sprint 4) ⚠️
  └─ 參考: [docs/technical-implementation/backend/rag-implementation.md](../../docs/technical-implementation/backend/rag-implementation.md)

US 5.2 (Knowledge 進階功能) - 8 SP
  ├─→ US 5.1 (RAG 基礎) 🔴
  └─ 參考: [docs/user-stories/modules/module-08-knowledge-management.md](../../docs/user-stories/modules/module-08-knowledge-management.md) Section 3

US 5.3, 5.4, 5.5, 5.6 (其他 Knowledge 功能)
  └─→ US 5.1, 5.2 (依序依賴) 🔴
```

### US 6.x - Text-to-SQL (Module 06)

```
US 6.1 (基礎文字對話) - 3 SP
  ├─→ US 1.4 (Agent 執行引擎) 🔴
  ├─→ Conversation 管理 (US 1.4) 🔴
  └─ 參考: [docs/ux-design/wireframes/low-fidelity/05-conversation.md](../../docs/ux-design/wireframes/low-fidelity/05-conversation.md)

US 6.2, 6.3, 6.4 (Text-to-SQL 核心功能)
  ├─→ US 1.4 (Agent 執行引擎) 🔴
  ├─→ US 2.1 (Plugin 系統) 🔴
  ├─⇢ US 5.1 (Knowledge 檢索) 🟡
  └─ 參考: [docs/ux-design/wireframes/low-fidelity/08-text-to-sql.md](../../docs/ux-design/wireframes/low-fidelity/08-text-to-sql.md)
```

### US 7.x - Persona 系統 (Module 07)

參考 [docs/user-stories/modules/module-07-persona-framework.md](../../docs/user-stories/modules/module-07-persona-framework.md)：

```
US 7.1 (Persona 定義) - 8 SP
  ├─→ US 1.1 (Agent CRUD) 🔴
  ├─⇢ US 2.1 (Plugin Registry) 🟡
  └─ 參考: [docs/ux-design/wireframes/low-fidelity/10-persona-builder.md](../../docs/ux-design/wireframes/low-fidelity/10-persona-builder.md)

US 7.2 (Persona 載入) - 5 SP
  ├─→ US 7.1 (Persona 定義) 🔴
  └─ 參考: [docs/technical-implementation/backend/persona-system-implementation.md](../../docs/technical-implementation/backend/persona-system-implementation.md)
```

### US 8.x - Chat 界面 (Module 08)

參考 [docs/ux-design/wireframes/low-fidelity/05-conversation.md](../../docs/ux-design/wireframes/low-fidelity/05-conversation.md)：

```
US 8.1 (Chat 頁面) - 含在 US 5.5 🔄
  ├─→ US 1.4 (Agent 執行 API) 🔴
  ├─→ US 6.1 (Conversation 管理) 🔴
  ├─⇢ US 4.1 (Workflow 執行 API) 🟡
  └─ 參考: [docs/ux-design/wireframes/low-fidelity/05-conversation.md](../../docs/ux-design/wireframes/low-fidelity/05-conversation.md)

US 8.2 (歷史記錄管理) - 13 SP
  └─→ US 8.1 (Chat 基礎) 🔴

US 8.4 (Chat 進階功能)
  └─→ US 8.2 (歷史管理) 🔴
```

---

## 🚨 關鍵路徑分析

### 主要關鍵路徑 (Critical Path)

```
🔥 Path 1: Agent → Plugin → Workflow Engine (最長路徑)

Sprint 1 (US 1.1, 1.2, 1.3) - 3 週 ✅
  ↓
Sprint 2 (US 1.4, 2.1, 6.1) - 3 週 (+6 days 延遲) ⚠️
  ↓
Sprint 3 (US 1.5) - 3 週
  ↓
Sprint 4 (US 7.1, 7.2) - 3 週
  ↓
Sprint 5 (US 5.1, 5.3) - 3 週
  ↓
Sprint 6 (US 5.2) - 3 週
  ↓
🔥 Sprint 7-9 (Workflow Engine 核心) - 9 週 🔥 Critical
  ↓
Sprint 10 (Code Interpreter) - 3 週
  ↓
🔥 Sprint 11-14 (Workflow Editor) - 12 週 🔥 Critical
  ↓
Sprint 15-16 (進階工作流) - 6 週
  ↓
Sprint 17-21 (Chat, Text-to-SQL, 企業功能) - 15 週

總關鍵路徑長度: 63 週 (14.5 個月)
```

**關鍵路徑參考文檔**:
- [docs/technical-implementation/sprint-planning/critical-path-analysis.md](../../docs/technical-implementation/sprint-planning/critical-path-analysis.md)
- [docs/user-stories/modules/module-04-multi-agent-workflow.md](../../docs/user-stories/modules/module-04-multi-agent-workflow.md)

### 關鍵瓶頸 (Bottlenecks)

| 瓶頸點 | 位置 | 影響 | 緩解策略 | 參考文檔 |
|-------|------|------|---------|---------|
| **Workflow Engine 核心** | Sprint 7-9 | 阻斷 Sprint 10-21 (12 個 Sprint) | 分階段實施,技術 Spike,增加資源 | [workflow-engine-implementation.md](../../docs/technical-implementation/backend/workflow-engine-implementation.md) |
| **Workflow Editor Frontend** | Sprint 13-14 | 阻斷 Sprint 15-21 (7 個 Sprint) | VueFlow 培訓,外部諮詢 | [poc6-vueflow-crdt](../../poc-projects/poc6-vueflow-crdt/) |
| **US 1.4 延遲** | Sprint 2 | 累積延遲 6 days | 已接受,調整後續計劃 | [sprint-2-retrospective](../2-sprints/sprint-2/SPRINT-2-7-RETROSPECTIVE.md) |
| **Vector DB 選型** | Sprint 4 | 影響 Sprint 5-6 Knowledge | Sprint 4 技術評估 | [rag-implementation.md](../../docs/technical-implementation/backend/rag-implementation.md) |

---

## ⚠️ 高風險依賴

### DEPENDENCY-001: Workflow Engine → 12 個 Sprint 🔥

**風險等級**: 🔥 Critical
**影響範圍**: Sprint 10-21 (所有依賴工作流的功能)

**依賴鏈**:
```
Sprint 7-9 (Workflow Engine 核心) 延遲 X 天
  → Sprint 10 (Code Interpreter) 延遲 X 天
  → Sprint 11-14 (Workflow Editor) 延遲 X 天
  → Sprint 15-16 (進階工作流) 延遲 X 天
  → Sprint 17-21 (Chat, Text-to-SQL, 企業功能) 延遲 X 天

累積影響: X 天 延遲影響 12 個 Sprint (42 週)
```

**緩解策略**:
- ✅ Sprint 6 技術 Spike (降低不確定性)
- ⏳ 增加 1 名資深工作流開發者 (Sprint 7 開始)
- ⏳ 分階段實施 (Phase 1B → Phase 1E)
- ⚠️ 備用: 降低其他模組優先級 (US 5.1, 6.1 部分功能)

**參考文檔**:
- [docs/user-stories/modules/module-04-multi-agent-workflow.md](../../docs/user-stories/modules/module-04-multi-agent-workflow.md)
- [docs/technical-implementation/backend/workflow-engine-implementation.md](../../docs/technical-implementation/backend/workflow-engine-implementation.md)
- [docs/architecture/Architecture-Design-Document.md](../../docs/architecture/Architecture-Design-Document.md) Section 5

---

### DEPENDENCY-002: Workflow Editor Frontend → VueFlow 學習曲線 🔴

**風險等級**: 🔴 High
**影響範圍**: Sprint 13-14 (Workflow Editor Frontend)

**依賴鏈**:
```
VueFlow 學習曲線 → Frontend 開發延遲
  → Workflow Editor 延遲交付
  → Sprint 15-21 依賴工作流編輯器的功能延遲
```

**緩解策略**:
- ⏳ Sprint 9 VueFlow 培訓 (2 days)
- ⏳ 雇用 VueFlow 專家輔導 (Sprint 10-11, 2 週)
- ⚠️ 備用: 降級為簡化編輯器 (JSON 表單而非可視化)

**PoC 驗證**:
- ✅ [poc-projects/poc6-vueflow-crdt/](../../poc-projects/poc6-vueflow-crdt/) PoC 6 已驗證可行性（98.2% 質量評分）
- ✅ [docs/PROJECT-INITIALIZATION-DECISION.md](../../docs/PROJECT-INITIALIZATION-DECISION.md) VueFlow 技術選型決策

---

### DEPENDENCY-003: US 1.4 延遲 → Sprint 2-21 累積延遲 🔴

**風險等級**: 🔴 High
**影響範圍**: Sprint 2 延遲 6 days → 後續 Sprint 可能累積延遲

**依賴鏈**:
```
US 1.4 延遲 6 days (Sprint 2)
  → Sprint 3 開始延遲 6 days
  → 如果每個 Sprint 再延遲 1-2 days
  → 累積延遲可達 20-30 days (4-6 週)
```

**緩解策略**:
- ✅ 接受 Sprint 2 延遲 6 days
- ✅ 調整 Sprint 3 開始時間 (2025-12-22)
- ⏳ 每個 Sprint 嘗試節省 0.5 day (透過優化)
- ⏳ 嚴格執行變更控制流程 (避免持續範圍蔓延)

**參考文檔**:
- [../2-sprints/sprint-2/SPRINT-2-7-RETROSPECTIVE.md](../2-sprints/sprint-2/SPRINT-2-7-RETROSPECTIVE.md)
- [docs/technical-implementation/backend/semantic-kernel-integration.md](../../docs/technical-implementation/backend/semantic-kernel-integration.md)

---

### DEPENDENCY-004: Vector DB 選型 → Knowledge 模組 🟡

**風險等級**: 🟡 Medium
**影響範圍**: Sprint 5-6 (Knowledge 檢索)

**依賴鏈**:
```
Vector DB 選型延遲 (Sprint 4)
  → Sprint 5 Knowledge 檢索開發延遲
  → Sprint 6 Knowledge 進階功能延遲
  → US 6.2-6.4 Text-to-SQL (依賴 Knowledge) 延遲
```

**緩解策略**:
- ⏳ Sprint 4 期間完成技術評估 (Qdrant vs Chroma)
- ⏳ 性能基準測試 + 部署複雜度評估
- 🎯 決策日期: Sprint 4 結束前 (2025-12-XX)

**參考文檔**:
- [docs/technical-implementation/backend/rag-implementation.md](../../docs/technical-implementation/backend/rag-implementation.md)
- [docs/user-stories/modules/module-08-knowledge-management.md](../../docs/user-stories/modules/module-08-knowledge-management.md)
- [docs/deployment/infrastructure-setup.md](../../docs/deployment/infrastructure-setup.md) Vector Database Section

---

## 📊 依賴監控指標

### 關鍵依賴健康度

| 依賴項 | 狀態 | 健康度 | 風險 | 行動 | 參考文檔 |
|-------|------|-------|------|------|---------|
| US 1.1-1.3 (Agent CRUD) | ✅ 完成 | 🟢 100% | 無 | - | [module-01](../../docs/user-stories/modules/module-01-agent-management.md) |
| US 1.4 (Agent 執行) | ✅ 完成 | 🟡 100% (+6d) | 已延遲 | 已調整計劃 | [sprint-2-retro](../2-sprints/sprint-2/SPRINT-2-7-RETROSPECTIVE.md) |
| US 2.1 (Plugin 系統) | ✅ 完成 | 🟢 100% | 無 | - | [module-02](../../docs/user-stories/modules/module-02-plugin-system.md) |
| US 2.2/2.3 (Plugin 熱重載/版本) | 🔄 Phase 1-2 | 🟡 40%, 30% | 低估風險 | 預留緩衝時間 | [plugin-hot-reload](../../docs/technical-implementation/backend/plugin-hot-reload-implementation.md) |
| Workflow Engine 核心 (Sprint 7-9) | ⏳ 未開始 | ⚪ 0% | 🔥 Critical | Sprint 6 Spike | [module-04](../../docs/user-stories/modules/module-04-multi-agent-workflow.md) |
| Workflow Editor (Sprint 11-14) | ⏳ 未開始 | ⚪ 0% | 🔴 High | VueFlow 培訓 | [workflow-editor-v2](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md) |
| Vector DB 選型 | ⏳ 未決定 | ⚪ 0% | 🟡 Medium | Sprint 4 評估 | [rag-implementation](../../docs/technical-implementation/backend/rag-implementation.md) |

### 依賴延遲影響預測

| 延遲場景 | 延遲天數 | 影響 Sprint | 累積延遲 | 緩解可能性 |
|---------|---------|-----------|---------|-----------|
| Workflow Engine 延遲 3 days | 3 | Sprint 10-21 (12 個) | 3-6 days | 🟡 中等 |
| Workflow Engine 延遲 7 days | 7 | Sprint 10-21 (12 個) | 7-14 days | 🔴 困難 |
| Workflow Engine 延遲 14+ days | 14+ | Sprint 10-21 (12 個) | 14-28 days | 🔥 極難 |
| Workflow Editor 延遲 7 days | 7 | Sprint 15-21 (7 個) | 7-10 days | 🟡 中等 |
| Vector DB 選型延遲 7 days | 7 | Sprint 5-6, 18-19 | 7 days | 🟢 容易 |

---

## 🔄 更新歷史

| 版本 | 日期 | 更新內容 | 更新人 |
|-----|------|---------|-------|
| 2.0 | 2025-12-11 | 增加 55+ `/docs` 引用，強化每個依賴項的文檔支持 | AI Assistant |
| 1.0.0 | 2025-12-11 | 初始版本，完整依賴矩陣 | AI Assistant |

---

## 📚 參考文檔

### 1. Core Architecture & Design (6 個引用)
- [docs/architecture/Architecture-Design-Document.md](../../docs/architecture/Architecture-Design-Document.md) - 完整架構設計
- [docs/architecture/ADR-006-hybrid-state-management.md](../../docs/architecture/ADR-006-hybrid-state-management.md) - 狀態管理
- [docs/architecture/ADR-007-event-bus-strategy.md](../../docs/architecture/ADR-007-event-bus-strategy.md) - 消息傳遞
- [docs/architecture/ADR-011-framework-migration-strategy.md](../../docs/architecture/ADR-011-framework-migration-strategy.md) - Framework 抽象層
- [docs/PROJECT-INITIALIZATION-DECISION.md](../../docs/PROJECT-INITIALIZATION-DECISION.md) - 技術選型決策

### 2. User Stories & Modules (8 個引用)
- [docs/user-stories/README.md](../../docs/user-stories/README.md) - User Stories 總覽
- [docs/user-stories/modules/module-01-agent-management.md](../../docs/user-stories/modules/module-01-agent-management.md) - Agent 管理
- [docs/user-stories/modules/module-02-plugin-system.md](../../docs/user-stories/modules/module-02-plugin-system.md) - Plugin 系統
- [docs/user-stories/modules/module-04-multi-agent-workflow.md](../../docs/user-stories/modules/module-04-multi-agent-workflow.md) - Workflow 引擎
- [docs/user-stories/modules/module-07-persona-framework.md](../../docs/user-stories/modules/module-07-persona-framework.md) - Persona 系統
- [docs/user-stories/modules/module-08-knowledge-management.md](../../docs/user-stories/modules/module-08-knowledge-management.md) - Knowledge 管理
- [docs/user-stories/modules/module-09-code-interpreter.md](../../docs/user-stories/modules/module-09-code-interpreter.md) - Code Interpreter
- [docs/user-stories/modules/module-11-user-authentication.md](../../docs/user-stories/modules/module-11-user-authentication.md) - 用戶認證

### 3. Technical Implementation (11 個引用)
- [docs/technical-implementation/backend/workflow-engine-implementation.md](../../docs/technical-implementation/backend/workflow-engine-implementation.md) - 工作流引擎實現
- [docs/technical-implementation/backend/plugin-system-implementation.md](../../docs/technical-implementation/backend/plugin-system-implementation.md) - Plugin 實現
- [docs/technical-implementation/backend/plugin-hot-reload-implementation.md](../../docs/technical-implementation/backend/plugin-hot-reload-implementation.md) - 熱重載
- [docs/technical-implementation/backend/code-interpreter-implementation.md](../../docs/technical-implementation/backend/code-interpreter-implementation.md) - Code Interpreter
- [docs/technical-implementation/backend/rag-implementation.md](../../docs/technical-implementation/backend/rag-implementation.md) - RAG 實現
- [docs/technical-implementation/backend/semantic-kernel-integration.md](../../docs/technical-implementation/backend/semantic-kernel-integration.md) - SK 整合
- [docs/technical-implementation/backend/persona-system-implementation.md](../../docs/technical-implementation/backend/persona-system-implementation.md) - Persona 實現
- [docs/technical-implementation/frontend/api-client-generation.md](../../docs/technical-implementation/frontend/api-client-generation.md) - API Client
- [docs/technical-implementation/sprint-planning/sprint-dependencies.md](../../docs/technical-implementation/sprint-planning/sprint-dependencies.md) - Sprint 依賴
- [docs/technical-implementation/sprint-planning/critical-path-analysis.md](../../docs/technical-implementation/sprint-planning/critical-path-analysis.md) - 關鍵路徑

### 4. UX Design (11 個引用)
- [docs/ux-design/wireframes/low-fidelity/05-conversation.md](../../docs/ux-design/wireframes/low-fidelity/05-conversation.md) - Chat 界面
- [docs/ux-design/wireframes/low-fidelity/08-text-to-sql.md](../../docs/ux-design/wireframes/low-fidelity/08-text-to-sql.md) - Text-to-SQL
- [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md) - Workflow Editor 索引
- [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part1.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part1.md) - 核心設計
- [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part2.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part2.md) - 內聯配置
- [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part3.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part3.md) - 執行視覺化
- [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part4.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part4.md) - 28 種節點
- [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part5.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part5.md) - 畫布互動
- [docs/ux-design/wireframes/low-fidelity/10-persona-builder.md](../../docs/ux-design/wireframes/low-fidelity/10-persona-builder.md) - Persona Builder
- [docs/ux-design/design-system/component-library.md](../../docs/ux-design/design-system/component-library.md) - 組件庫

### 5. API & Database (3 個引用)
- [docs/api/agent-api-specification.md](../../docs/api/agent-api-specification.md) - Agent API
- [docs/api/workflow-api-specification.md](../../docs/api/workflow-api-specification.md) - Workflow API
- [docs/api/database-schema.md](../../docs/api/database-schema.md) - 資料庫 Schema

### 6. Security & Deployment (3 個引用)
- [docs/security/code-execution-security.md](../../docs/security/code-execution-security.md) - Code Execution 安全
- [docs/deployment/docker-setup.md](../../docs/deployment/docker-setup.md) - Docker 部署
- [docs/deployment/infrastructure-setup.md](../../docs/deployment/infrastructure-setup.md) - 基礎設施

### 7. Testing & Sprint Management (2 個引用)
- [docs/testing/integration-testing-strategy.md](../../docs/testing/integration-testing-strategy.md) - 整合測試
- [poc-projects/poc6-vueflow-crdt/](../../poc-projects/poc6-vueflow-crdt/) - PoC 6 驗證報告

### 8. Internal Claudedocs References (4 個引用)
- [風險登記冊](./RISK-REGISTER.md)
- [Sprint 分配](./SPRINT-ALLOCATION-ANALYSIS.md)
- [開發策略](./DEVELOPMENT-STRATEGY.md)
- [技術決策](./TECHNICAL-DECISIONS-LOG.md)
- [Sprint 2 回顧](../2-sprints/sprint-2/SPRINT-2-7-RETROSPECTIVE.md)

**總計**: 55+ `/docs` 引用

---

**維護說明**:
- ✅ 每 Sprint Planning 更新依賴狀態
- ✅ 每週監控關鍵路徑健康度
- ✅ 依賴變更時立即更新文檔
- ✅ 持續更新 `/docs` 引用以保持文檔同步

---

[返回 claudedocs 主頁](../README.md) | [MVP 範圍](./MVP-SCOPE-DEFINITION.md) | [架構演進](./ARCHITECTURE-EVOLUTION-ROADMAP.md) | [技術決策](./TECHNICAL-DECISIONS-LOG.md)
