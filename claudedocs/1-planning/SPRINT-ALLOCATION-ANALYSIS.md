# Sprint 分配分析 (Sprint Allocation Analysis)

**目的**: 分析原始 18 Sprint 計劃與當前需求的差距，並提供調整後的 21 Sprint 計劃

**版本**: v2.0
**創建日期**: 2025-11-12
**最後更新**: 2025-11-12
**維護者**: 項目管理團隊

---

## 📋 目錄

1. [執行摘要](#執行摘要)
2. [原始 18 Sprint 計劃回顧](#原始-18-sprint-計劃回顧)
3. [Gap 分析](#gap-分析)
4. [調整後的 21 Sprint 計劃](#調整後的-21-sprint-計劃)
5. [Story Points 對比](#story-points-對比)
6. [時間線調整](#時間線調整)
7. [風險評估](#風險評估)
8. [建議與下一步](#建議與下一步)
9. [參考文檔](#參考文檔)

---

## 執行摘要

### 關鍵發現

```yaml
原始計劃:
  Sprint 數量: 18 Sprints
  預估週數: 54 週 (約 12.5 個月)
  預估 Story Points: 157-162 SP (已明確分配)
  來源: docs/user-stories/sprints/sprint-allocation.md

調整後計劃:
  Sprint 數量: 21 Sprints
  預估週數: 63 週 (約 14.5 個月)
  預估 Story Points: 223-250 SP
  來源: claudedocs/1-planning/MVP-SCOPE-DEFINITION.md

Gap 分析:
  缺失 Sprints: 3 Sprints
  缺失 Story Points: 61-88 SP
  時間延長: 9 週 (約 2 個月)
  影響分析: claudedocs/1-planning/RISK-REGISTER.md
```

### 主要變更原因

```yaml
變更 1 - 工作流編輯器必須在 MVP 中:
  決策者: 用戶
  決策日期: 2025-11-12
  影響:
    - 新增 Workflow Editor Backend (15-20 SP)
    - 新增 Workflow Editor Frontend (20-25 SP)
    - 新增 3 Sprints (Sprint 10-14)
  參考:
    - claudedocs/1-planning/TECHNICAL-DECISIONS-LOG.md (TD-008)
    - docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md (完整 7 部分設計)
    - docs/architecture/adr/ADR-012-workflow-editor-technology.md

變更 2 - 完整工作流引擎:
  決策者: 用戶
  決策日期: 2025-11-12
  影響:
    - 原 US 4.1 分配 13 SP → 應為 34-43 SP
    - 低估缺口: 21-30 SP
    - 需要額外 1-2 Sprints (Sprint 15-16)
  參考:
    - claudedocs/1-planning/TECHNICAL-DECISIONS-LOG.md (TD-009)
    - docs/user-stories/us-4-multi-agent-orchestration.md
    - docs/technical-implementation/01-backend-net9/12-workflow-orchestration-implementation.md
    - claudedocs/1-planning/DEVELOPMENT-STRATEGY.md (工作流引擎完整設計)

變更 3 - Framework 抽象層:
  決策者: 架構團隊
  決策日期: 2025-10-28
  影響:
    - 新增 Abstraction Layer 實現 (5-8 SP)
    - 整合到 Phase 1B
  參考:
    - claudedocs/1-planning/TECHNICAL-DECISIONS-LOG.md (TD-002)
    - docs/architecture/adr/ADR-011-framework-abstraction-layer.md
    - docs/architecture/Architecture-Design-Document.md (Section 6.5)

變更 4 - MVP 分階段實施:
  決策者: 用戶
  決策日期: 2025-11-12
  影響:
    - MVP 分為 6 phases (1A-1F)
    - 不減少範圍，系統化組織
  參考:
    - claudedocs/1-planning/TECHNICAL-DECISIONS-LOG.md (TD-010)
    - claudedocs/1-planning/MVP-SCOPE-DEFINITION.md (6 Phases 詳細定義)
```

---

## 原始 18 Sprint 計劃回顧

### 原始計劃總覽

```
Sprint 1-6: 基礎與 Persona (Week 1-18)
├─ Sprint 1: US 1.1 基礎設施 (13 SP)
│   參考: docs/user-stories/us-1-agent-crud.md
│   參考: docs/technical-implementation/01-backend-net9/01-monorepo-setup.md
│
├─ Sprint 2: US 1.2 Agent 引擎核心 (13 SP)
│   參考: docs/user-stories/us-1-agent-crud.md
│   參考: docs/technical-implementation/01-backend-net9/02-semantic-kernel-integration.md
│
├─ Sprint 3: US 1.3 Plugin 系統 (13 SP)
│   參考: docs/user-stories/us-2-plugin-system.md
│   參考: docs/technical-implementation/01-backend-net9/04-plugin-system-mcp.md
│
├─ Sprint 4: US 2.1 Persona 定義 (8 SP)
│   參考: docs/user-stories/us-3-persona-definition.md
│   參考: docs/technical-implementation/01-backend-net9/03-persona-framework.md
│
├─ Sprint 5: US 2.2 Plugin 熱加載 (5 SP)
│   參考: docs/user-stories/us-2-plugin-system.md
│
└─ Sprint 6: US 3.1 Knowledge 檢索 (13 SP)
    參考: docs/user-stories/us-5-knowledge-management.md
    參考: docs/technical-implementation/01-backend-net9/05-knowledge-management-rag.md
   總計: 65 SP

Sprint 7-11: Code Interpreter & Text-to-SQL (Week 19-33)
├─ Sprint 7: US 5.1 Code Interpreter (13 SP)
│   參考: docs/user-stories/us-6-code-interpreter.md
│   參考: docs/architecture/adr/ADR-008-code-interpreter-container-pool.md
│   參考: docs/technical-implementation/01-backend-net9/06-code-interpreter-implementation.md
│
├─ Sprint 8-9: US 6.1 Text-to-SQL (21 SP)
│   參考: docs/user-stories/us-7-text-to-sql.md
│   參考: docs/technical-implementation/01-backend-net9/07-text-to-sql-implementation.md
│
└─ Sprint 10-11: US 7.1 Multimodal Output (13 SP)
    參考: docs/user-stories/us-8-multimodal-output.md
    參考: docs/technical-implementation/01-backend-net9/08-multimodal-output-engine.md
   總計: 47 SP

Sprint 12-15: Multi-Agent & Chat (Week 34-45)
├─ Sprint 12-13: US 4.1 Multi-Agent Orchestration (13 SP) ⚠️ 低估
│   參考: docs/user-stories/us-4-multi-agent-orchestration.md
│   實際需求: 34-43 SP (參考 DEVELOPMENT-STRATEGY.md)
│
├─ Sprint 14: US 8.1 Chat 頁面 (8 SP)
│   參考: docs/user-stories/us-9-chat-interface.md
│   參考: docs/ux-design/wireframes/low-fidelity/05-conversation.md
│
└─ Sprint 15: US 8.2 歷史記錄 (8 SP)
    參考: docs/user-stories/us-9-chat-interface.md
   總計: 29 SP

Sprint 16-18: 企業功能與收尾 (Week 46-54)
├─ Sprint 16: US 9.1 批量操作 (5 SP)
│   參考: docs/user-stories/us-10-batch-operations.md
│
├─ Sprint 17: US 10.1 管理面板 (8 SP)
│   參考: docs/user-stories/us-11-monitoring-admin.md
│   參考: docs/ux-design/wireframes/low-fidelity/12-monitoring.md
│
└─ Sprint 18: 整合測試與收尾 (3 SP)
    參考: docs/testing/test-strategy.md
   總計: 16 SP

原始總計:
  18 Sprints, 54 週, 157 SP (已明確分配)
  + 未明確分配的小功能 (~5 SP)
  ≈ 162 SP
  來源: docs/user-stories/sprints/sprint-allocation.md
```

### 原始計劃的 Story Points 分配表

| Sprint | User Story | 描述 | Story Points | 參考文檔 |
|--------|-----------|------|--------------|---------|
| Sprint 1 | US 1.1 | 基礎設施搭建 (Monorepo, CI/CD, Database) | 13 SP | [US 1.1](../docs/user-stories/us-1-agent-crud.md) |
| Sprint 2 | US 1.2 | Agent 引擎核心 (SK 整合, 基礎 Agent 執行) | 13 SP | [SK Integration](../docs/technical-implementation/01-backend-net9/02-semantic-kernel-integration.md) |
| Sprint 3 | US 1.3 | Plugin 系統 (MCP 支持, Plugin Registry) | 13 SP | [Plugin System](../docs/user-stories/us-2-plugin-system.md) |
| Sprint 4 | US 2.1 | Persona 定義與載入 (YAML Persona) | 8 SP | [Persona Framework](../docs/technical-implementation/01-backend-net9/03-persona-framework.md) |
| Sprint 5 | US 2.2 | Plugin 熱加載與動態註冊 | 5 SP | [US 2.2](../docs/user-stories/us-2-plugin-system.md) |
| Sprint 6 | US 3.1 | Knowledge 檢索 (RAG, Vector DB) | 13 SP | [Knowledge RAG](../docs/technical-implementation/01-backend-net9/05-knowledge-management-rag.md) |
| Sprint 7 | US 5.1 | Code Interpreter (Docker Container Pool) | 13 SP | [ADR-008](../docs/architecture/adr/ADR-008-code-interpreter-container-pool.md) |
| Sprint 8-9 | US 6.1 | Text-to-SQL (自然語言轉 SQL) | 21 SP | [Text-to-SQL](../docs/technical-implementation/01-backend-net9/07-text-to-sql-implementation.md) |
| Sprint 10-11 | US 7.1 | Multimodal Output Engine | 13 SP | [Multimodal Output](../docs/technical-implementation/01-backend-net9/08-multimodal-output-engine.md) |
| Sprint 12-13 | US 4.1 | Multi-Agent Orchestration | 13 SP ⚠️ | [US 4.1](../docs/user-stories/us-4-multi-agent-orchestration.md) |
| Sprint 14 | US 8.1 | Chat 頁面 | 8 SP | [Chat UI](../docs/ux-design/wireframes/low-fidelity/05-conversation.md) |
| Sprint 15 | US 8.2 | 歷史記錄管理 | 8 SP | [US 9](../docs/user-stories/us-9-chat-interface.md) |
| Sprint 16 | US 9.1 | 批量操作 (Activate, Pause, Archive, Delete) | 5 SP | [US 10](../docs/user-stories/us-10-batch-operations.md) |
| Sprint 17 | US 10.1 | 管理面板 | 8 SP | [Monitoring](../docs/ux-design/wireframes/low-fidelity/12-monitoring.md) |
| Sprint 18 | - | 整合測試與收尾 | 3 SP | [Test Strategy](../docs/testing/test-strategy.md) |
| **總計** | - | - | **157 SP** | - |

---

## Gap 分析

### Gap 1: 工作流編輯器完全缺失 ⚠️ Critical

```yaml
問題描述:
  原始計劃: 工作流編輯器標記為 Phase 2
  用戶要求: "工作流編輯也必須在MVP中, 因為這也是其中一個主要的功能"
  決策日期: 2025-11-12
  決策記錄: claudedocs/1-planning/TECHNICAL-DECISIONS-LOG.md (TD-008)

缺失內容:
  Workflow Editor Backend:
    - Workflow Definition CRUD (5 SP)
      參考: docs/api/workflow-api-design.md
      參考: docs/database/workflow-schema.md

    - Node Registry (3 SP)
      參考: docs/technical-implementation/01-backend-net9/12-workflow-orchestration-implementation.md

    - Edge Validation (3 SP)
      參考: docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part5.md (畫布互動)

    - Workflow Execution Trigger (2 SP)
      參考: docs/technical-implementation/01-backend-net9/12-workflow-orchestration-implementation.md

    - Workflow Template Management (2 SP)
      參考: docs/user-stories/us-4-multi-agent-orchestration.md

    - Version Control (基礎) (2-3 SP)
      參考: docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part6.md (協作與版本控制)

    總計: 15-20 SP

  Workflow Editor Frontend:
    - VueFlow/ReactFlow 集成 (5 SP)
      參考: docs/architecture/adr/ADR-012-workflow-editor-technology.md
      參考: docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part1.md (核心設計理念)

    - Drag-drop Node Editor (4 SP)
      參考: docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part4.md (28 種節點類型)

    - Node Configuration Panel (3 SP)
      參考: docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part2.md (內聯配置)

    - Connection Line Drawing (3 SP)
      參考: docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part5.md (畫布互動)

    - Workflow Canvas (Zoom, Pan, Mini-map) (3 SP)
      參考: docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part5.md

    - Node Template Library (2 SP)
      參考: docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part4.md

    - Save/Load Workflow (2-3 SP)
      參考: docs/api/workflow-api-design.md

    總計: 20-25 SP

  總缺口: 35-45 SP

影響:
  新增 Sprints: 3 Sprints (Sprint 10-14)
  時間延長: 9 週
  依賴關係: 參考 claudedocs/1-planning/DEPENDENCY-MATRIX.md (Sprint 依賴章節)

技術風險:
  - 微前端架構 (Vue 3 + Module Federation)
  - CRDT 實時協作 (Yjs)
  - n8n-style 大卡片節點系統
  風險評估: claudedocs/1-planning/RISK-REGISTER.md (RISK-007)

參考文檔:
  - TD-008: 工作流編輯器必須在 MVP 中
  - ADR-012: 工作流編輯器技術選型 (Vue 3 + VueFlow)
  - 完整 Workflow Editor V2 設計 (7 parts):
    - Part 1: docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part1.md
    - Part 2: docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part2.md
    - Part 3: docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part3.md
    - Part 4: docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part4.md
    - Part 5: docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part5.md
    - Part 6: docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part6.md
    - Part 7: docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part7.md
```

### Gap 2: Multi-Agent Orchestration 低估 ⚠️ High Priority

```yaml
問題描述:
  原始計劃: US 4.1 Multi-Agent Orchestration 分配 13 SP
  實際需求: 完整工作流引擎需要 34-43 SP
  決策日期: 2025-11-12
  決策記錄: claudedocs/1-planning/TECHNICAL-DECISIONS-LOG.md (TD-009)

低估分析:
  原始分配: 13 SP (Sprint 12-13)
  實際需求:
    - Task Generator (5-8 SP)
      參考: claudedocs/1-planning/DEVELOPMENT-STRATEGY.md (Task Generator 設計)
      參考: docs/technical-implementation/01-backend-net9/09-prompt-engineering.md (LLM Prompt Templates)

    - Multi-Agent Coordination (8-10 SP)
      參考: claudedocs/1-planning/DEVELOPMENT-STRATEGY.md (Multi-Agent Coordination 設計)
      參考: docs/technical-implementation/01-backend-net9/12-workflow-orchestration-implementation.md
      策略: Sequential, Parallel, Conditional, Loop, GroupConversation, Supervisor

    - Agent Messaging (5 SP)
      參考: docs/architecture/adr/ADR-007-internal-communication-strategy.md
      參考: docs/technical-implementation/01-backend-net9/12-workflow-orchestration-implementation.md

    - Workflow Execution Engine (8-10 SP)
      參考: claudedocs/1-planning/DEVELOPMENT-STRATEGY.md (Workflow Execution Engine 設計)
      參考: docs/technical-implementation/01-backend-net9/12-workflow-orchestration-implementation.md

    - State Management (5 SP)
      參考: docs/architecture/adr/ADR-006-state-management-strategy.md
      參考: docs/database/redis-design.md (Checkpoint Store)

    - Feedback Loop (3-5 SP)
      參考: docs/user-stories/us-4-multi-agent-orchestration.md

    總計: 34-43 SP

  缺口: 21-30 SP

原因:
  - 原計劃未考慮完整工作流引擎的複雜度
  - 用戶要求: "需要準備一個完整的工作流引擎"
  - 參考: TD-009

調整:
  Phase 1B (Sprint 7-9): 工作流引擎核心 (13 SP → 26 SP)
    - Task Generator (5-8 SP)
    - Multi-Agent Coordination Sequential/Parallel (8-10 SP)
    - Agent Messaging (5 SP)
    - Workflow Execution Engine (8-10 SP)
    - 參考: claudedocs/1-planning/DEVELOPMENT-STRATEGY.md (Phase 1B 詳細設計)

  Phase 1E (Sprint 15-16): 完整工作流引擎 (13-18 SP)
    - Conditional, Loop, Handoff, Supervisor
    - State Management (Checkpoint/Resume)
    - Feedback Loop
    - 參考: claudedocs/1-planning/DEVELOPMENT-STRATEGY.md (Phase 1E 詳細設計)

  總計調整: 13 SP → 39-44 SP

影響:
  調整現有 Sprints: Sprint 12-13 拆分並擴展
  新增 Sprints: Sprint 15-16 (Phase 1E)
  時間影響: 已包含在調整計劃中
  依賴影響: 參考 claudedocs/1-planning/DEPENDENCY-MATRIX.md (Sprint 7-9 關鍵路徑)

參考文檔:
  - TD-009: 自建完整工作流引擎
  - docs/user-stories/us-4-multi-agent-orchestration.md: 詳細需求
  - claudedocs/1-planning/DEVELOPMENT-STRATEGY.md: 完整工作流引擎開發策略
  - docs/technical-implementation/01-backend-net9/12-workflow-orchestration-implementation.md: 實施指南
```

### Gap 3: Framework Abstraction Layer 未分配

```yaml
問題描述:
  原始計劃: 未明確分配 Abstraction Layer 開發
  架構需求: 5 個核心接口需要實現
  決策記錄: docs/architecture/adr/ADR-011-framework-abstraction-layer.md

缺失內容:
  Framework Abstraction Layer:
    - IAgentRuntime 接口定義與實現 (2 SP)
      參考: docs/architecture/adr/ADR-011-framework-abstraction-layer.md
      參考: claudedocs/1-planning/DEVELOPMENT-STRATEGY.md (Framework Abstraction Layer 設計)

    - IToolRegistry 接口定義與實現 (2 SP)
      參考: docs/architecture/adr/ADR-011-framework-abstraction-layer.md
      支援: MCP (Model Context Protocol) 插件系統

    - IAgentMemory 接口定義與實現 (1 SP)
      參考: docs/architecture/adr/ADR-011-framework-abstraction-layer.md
      參考: docs/technical-implementation/01-backend-net9/05-knowledge-management-rag.md

    - IWorkflowOrchestrator 接口定義與實現 (1 SP)
      參考: docs/architecture/adr/ADR-011-framework-abstraction-layer.md
      參考: docs/technical-implementation/01-backend-net9/12-workflow-orchestration-implementation.md

    - IObservability 接口定義與實現 (1 SP)
      參考: docs/architecture/adr/ADR-011-framework-abstraction-layer.md
      支援: OpenTelemetry 標準

    - SemanticKernelAdapter 實現 (1-2 SP)
      參考: docs/architecture/adr/ADR-011-framework-abstraction-layer.md
      參考: docs/technical-implementation/01-backend-net9/02-semantic-kernel-integration.md

    總計: 5-8 SP

影響:
  整合到 Phase 1B (Sprint 7-9)
  與工作流引擎核心同步開發
  未來支援: Microsoft Agent Framework (MAF) 遷移
  參考: docs/architecture/adr/ADR-011-framework-abstraction-layer.md (Phase 2 遷移計劃)

參考文檔:
  - TD-002: Framework Abstraction Layer 決策
  - ADR-011: Framework 遷移策略與抽象層設計
  - claudedocs/1-planning/DEVELOPMENT-STRATEGY.md: 完整抽象層設計
```

### Gap 總結

| Gap 項目 | 原計劃 SP | 實際需求 SP | 缺口 SP | 優先級 | 參考文檔 |
|---------|----------|------------|---------|--------|---------|
| Workflow Editor Backend | 0 | 15-20 SP | 15-20 SP | Critical | TD-008, ADR-012 |
| Workflow Editor Frontend | 0 | 20-25 SP | 20-25 SP | Critical | 完整 V2 設計 (7 parts) |
| Multi-Agent Orchestration | 13 SP | 34-43 SP | 21-30 SP | High | TD-009, US 4.1 |
| Framework Abstraction Layer | 0 | 5-8 SP | 5-8 SP | Medium | TD-002, ADR-011 |
| **總計** | **13 SP** | **74-96 SP** | **61-83 SP** | - | - |

```yaml
總缺口分析:
  Story Points 缺口: 61-83 SP
  預估新增 Sprints: 3-5 Sprints
  實際新增 Sprints: 3 Sprints (經過優化組織)
  時間延長: 9 週 (18 週 → 27 週 for affected phases)

風險影響:
  參考: claudedocs/1-planning/RISK-REGISTER.md
  主要風險: RISK-001 (工作流引擎複雜度), RISK-007 (編輯器技術風險)

依賴影響:
  參考: claudedocs/1-planning/DEPENDENCY-MATRIX.md
  關鍵路徑: Sprint 7-9 (工作流引擎核心) 阻斷後續 12 個 Sprint
```

---

## 調整後的 21 Sprint 計劃

### 新計劃總覽 (6 Phases)

```
Phase 1A: 基礎平台 (Sprint 1-6, Week 1-18)
├─ Sprint 1: US 1.1 基礎設施 (13 SP)
│   參考: docs/user-stories/us-1-agent-crud.md
│   參考: docs/technical-implementation/01-backend-net9/01-monorepo-setup.md
│
├─ Sprint 2-3: US 1.2 Agent 引擎 + US 1.3 Plugin 系統 (21 SP)
│   參考: docs/technical-implementation/01-backend-net9/02-semantic-kernel-integration.md
│   參考: docs/technical-implementation/01-backend-net9/04-plugin-system-mcp.md
│
├─ Sprint 4: US 2.1 Persona + US 2.2 Plugin 熱加載 (13 SP)
│   參考: docs/technical-implementation/01-backend-net9/03-persona-framework.md
│   參考: docs/user-stories/us-2-plugin-system.md
│
├─ Sprint 5: US 3.1 Knowledge 檢索 (13 SP)
│   參考: docs/technical-implementation/01-backend-net9/05-knowledge-management-rag.md
│   參考: docs/database/qdrant-design.md
│
└─ Sprint 6: US 5.1 Code Interpreter (5 SP ⚠️ 調整)
    參考: docs/architecture/adr/ADR-008-code-interpreter-container-pool.md
    參考: docs/security/code-execution-security.md
   總計: 65 SP, 18 週

Phase 1B: 工作流引擎核心 + 抽象層 (Sprint 7-9, Week 19-27) 🔥 Critical Path
├─ Sprint 7-8: 工作流引擎基礎 (26 SP)
│   - Task Generator (5-8 SP)
│     參考: claudedocs/1-planning/DEVELOPMENT-STRATEGY.md (Task Generator)
│     參考: docs/technical-implementation/01-backend-net9/09-prompt-engineering.md
│
│   - Multi-Agent Coordination (Sequential, Parallel) (8-10 SP)
│     參考: claudedocs/1-planning/DEVELOPMENT-STRATEGY.md (Multi-Agent Coordination)
│     參考: docs/technical-implementation/01-backend-net9/12-workflow-orchestration-implementation.md
│
│   - Agent Messaging (5 SP)
│     參考: docs/architecture/adr/ADR-007-internal-communication-strategy.md
│
│   - Workflow Execution Engine (8-10 SP)
│     參考: claudedocs/1-planning/DEVELOPMENT-STRATEGY.md (Workflow Execution Engine)
│
└─ Sprint 9: Framework Abstraction Layer (13 SP)
    - 5 個核心接口實現 (5-8 SP)
      參考: docs/architecture/adr/ADR-011-framework-abstraction-layer.md
      參考: claudedocs/1-planning/DEVELOPMENT-STRATEGY.md (Framework Abstraction)

    - SemanticKernelAdapter 完整實現 (5 SP)
      參考: docs/technical-implementation/01-backend-net9/02-semantic-kernel-integration.md

   總計: 39-51 SP, 9 週
   關鍵路徑: 參考 claudedocs/1-planning/DEPENDENCY-MATRIX.md (Sprint 7-9 警示)

Phase 1C: 工作流編輯器 Backend (Sprint 10-11, Week 28-33)
├─ Sprint 10: Workflow Editor Backend API (8 SP)
│   - Workflow Definition CRUD
│   - Node Registry
│   - Edge Validation
│   參考: docs/api/workflow-api-design.md
│   參考: docs/database/workflow-schema.md
│
└─ Sprint 11: Workflow Editor Backend 高級功能 (7-12 SP)
    - Workflow Execution Trigger
    - Workflow Template Management
    - Version Control (基礎)
    參考: docs/technical-implementation/01-backend-net9/12-workflow-orchestration-implementation.md
   總計: 15-20 SP, 6 週

Phase 1D: 工作流編輯器 Frontend (Sprint 12-14, Week 34-42)
├─ Sprint 12: VueFlow 畫布基礎 (8 SP)
│   - VueFlow 集成
│   - Drag-drop Node Editor
│   - 基礎節點類型
│   參考: docs/architecture/adr/ADR-012-workflow-editor-technology.md
│   參考: docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part1.md
│   參考: docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part5.md
│
├─ Sprint 13: 節點系統與配置 (7-9 SP)
│   - 28 種節點類型
│   - Node Configuration Panel
│   - Connection Line Drawing
│   參考: docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part4.md (28 種節點)
│   參考: docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part2.md (內聯配置)
│
└─ Sprint 14: 畫布高級功能 + CRDT 協作 (5-8 SP)
    - Workflow Canvas (Zoom, Pan, Mini-map)
    - Node Template Library
    - Yjs CRDT 實時協作
    參考: docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part5.md (畫布互動)
    參考: docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part6.md (協作)
    參考: docs/architecture/adr/ADR-012-workflow-editor-technology.md (CRDT 設計)
   總計: 20-25 SP, 9 週

Phase 1E: 完整工作流引擎 (Sprint 15-16, Week 43-48)
├─ Sprint 15: 高級編排模式 (8-10 SP)
│   - Conditional Branching
│   - Loop Support
│   - Handoff Pattern
│   - Supervisor Pattern
│   參考: claudedocs/1-planning/DEVELOPMENT-STRATEGY.md (Phase 1E 設計)
│   參考: docs/technical-implementation/01-backend-net9/12-workflow-orchestration-implementation.md
│
└─ Sprint 16: 狀態管理與反饋 (5-8 SP)
    - State Management (Checkpoint/Resume)
    - Feedback Loop
    - Error Handling + Retry
    參考: docs/architecture/adr/ADR-006-state-management-strategy.md
    參考: docs/database/redis-design.md (Checkpoint Store)
   總計: 13-18 SP, 6 週

Phase 1F: 企業功能與收尾 (Sprint 17-21, Week 49-63)
├─ Sprint 17: US 6.1 Text-to-SQL (21 SP)
│   參考: docs/user-stories/us-7-text-to-sql.md
│   參考: docs/technical-implementation/01-backend-net9/07-text-to-sql-implementation.md
│   參考: docs/ux-design/wireframes/low-fidelity/08-text-to-sql.md
│
├─ Sprint 18: US 7.1 Multimodal Output (13 SP)
│   參考: docs/user-stories/us-8-multimodal-output.md
│   參考: docs/technical-implementation/01-backend-net9/08-multimodal-output-engine.md
│
├─ Sprint 19: US 8.1 Chat + US 8.2 歷史記錄 (16 SP)
│   參考: docs/user-stories/us-9-chat-interface.md
│   參考: docs/ux-design/wireframes/low-fidelity/05-conversation.md
│   參考: docs/technical-implementation/02-frontend-react18/03-conversation-management.md
│
├─ Sprint 20: US 9.1 批量操作 + US 10.1 管理面板 (13 SP)
│   參考: docs/user-stories/us-10-batch-operations.md
│   參考: docs/user-stories/us-11-monitoring-admin.md
│   參考: docs/ux-design/wireframes/low-fidelity/12-monitoring.md
│
└─ Sprint 21: 整合測試與收尾 (8 SP)
    參考: docs/testing/test-strategy.md
    參考: docs/testing/e2e-testing.md
    參考: docs/deployment/mvp-deployment-guide.md
   總計: 71 SP, 15 週

調整後總計:
  21 Sprints, 63 週, 223-250 SP
  參考: claudedocs/1-planning/MVP-SCOPE-DEFINITION.md (完整 MVP 定義)
```

### 詳細 Sprint 分配表

| Sprint | Phase | User Story / 組件 | 描述 | Story Points | 週數 | 主要參考文檔 |
|--------|-------|------------------|------|--------------|------|------------|
| **Sprint 1** | 1A | US 1.1 | 基礎設施搭建 | 13 SP | 3 週 | [Monorepo Setup](../docs/technical-implementation/01-backend-net9/01-monorepo-setup.md) |
| **Sprint 2-3** | 1A | US 1.2, US 1.3 | Agent 引擎 + Plugin 系統 | 21 SP | 6 週 | [SK Integration](../docs/technical-implementation/01-backend-net9/02-semantic-kernel-integration.md) |
| **Sprint 4** | 1A | US 2.1, US 2.2 | Persona + Plugin 熱加載 | 13 SP | 3 週 | [Persona Framework](../docs/technical-implementation/01-backend-net9/03-persona-framework.md) |
| **Sprint 5** | 1A | US 3.1 | Knowledge 檢索 | 13 SP | 3 週 | [Knowledge RAG](../docs/technical-implementation/01-backend-net9/05-knowledge-management-rag.md) |
| **Sprint 6** | 1A | US 5.1 | Code Interpreter | 5 SP | 3 週 | [ADR-008](../docs/architecture/adr/ADR-008-code-interpreter-container-pool.md) |
| **Sprint 7-8** | 1B | 工作流引擎基礎 | Task Generator, Coordination, Execution Engine | 26 SP | 6 週 | [Development Strategy](./DEVELOPMENT-STRATEGY.md) |
| **Sprint 9** | 1B | Abstraction Layer | 5 個核心接口 + SK Adapter | 13 SP | 3 週 | [ADR-011](../docs/architecture/adr/ADR-011-framework-abstraction-layer.md) |
| **Sprint 10** | 1C | Workflow Editor Backend | CRUD, Node Registry, Validation | 8 SP | 3 週 | [Workflow API](../docs/api/workflow-api-design.md) |
| **Sprint 11** | 1C | Workflow Editor Backend 高級 | Trigger, Template, Version Control | 7-12 SP | 3 週 | [Workflow Orchestration](../docs/technical-implementation/01-backend-net9/12-workflow-orchestration-implementation.md) |
| **Sprint 12** | 1D | Workflow Editor Frontend 基礎 | VueFlow 集成, Drag-drop | 8 SP | 3 週 | [Workflow V2 Part1](../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part1.md) |
| **Sprint 13** | 1D | 節點系統 | 28 種節點, Config Panel | 7-9 SP | 3 週 | [Workflow V2 Part4](../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part4.md) |
| **Sprint 14** | 1D | 畫布高級 + CRDT | Canvas, CRDT 協作 | 5-8 SP | 3 週 | [Workflow V2 Part6](../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part6.md) |
| **Sprint 15** | 1E | 高級編排模式 | Conditional, Loop, Handoff, Supervisor | 8-10 SP | 3 週 | [Development Strategy](./DEVELOPMENT-STRATEGY.md) |
| **Sprint 16** | 1E | 狀態管理與反饋 | Checkpoint/Resume, Feedback Loop | 5-8 SP | 3 週 | [ADR-006](../docs/architecture/adr/ADR-006-state-management-strategy.md) |
| **Sprint 17** | 1F | US 6.1 | Text-to-SQL | 21 SP | 3 週 | [Text-to-SQL](../docs/technical-implementation/01-backend-net9/07-text-to-sql-implementation.md) |
| **Sprint 18** | 1F | US 7.1 | Multimodal Output Engine | 13 SP | 3 週 | [Multimodal Output](../docs/technical-implementation/01-backend-net9/08-multimodal-output-engine.md) |
| **Sprint 19** | 1F | US 8.1, US 8.2 | Chat 頁面 + 歷史記錄 | 16 SP | 3 週 | [Conversation UI](../docs/ux-design/wireframes/low-fidelity/05-conversation.md) |
| **Sprint 20** | 1F | US 9.1, US 10.1 | 批量操作 + 管理面板 | 13 SP | 3 週 | [Monitoring](../docs/ux-design/wireframes/low-fidelity/12-monitoring.md) |
| **Sprint 21** | 1F | 整合測試 | E2E 測試, 性能優化, 文檔 | 8 SP | 3 週 | [Test Strategy](../docs/testing/test-strategy.md) |
| **總計** | - | - | - | **223-250 SP** | **63 週** | - |

---

## Story Points 對比

### 原始 vs 調整後對比表

| 類別 | 原始計劃 (18 Sprint) | 調整後計劃 (21 Sprint) | 增加 SP | 增加 % | 主要參考 |
|------|---------------------|----------------------|---------|--------|---------|
| **基礎平台** | 65 SP | 65 SP | 0 SP | 0% | [US 1-3](../docs/user-stories/) |
| **工作流引擎** | 13 SP | 39-51 SP | 26-38 SP | 200-292% | [Development Strategy](./DEVELOPMENT-STRATEGY.md) |
| **工作流編輯器** | 0 SP | 35-45 SP | 35-45 SP | N/A | [Workflow V2 Design](../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md) |
| **抽象層** | 0 SP (隱含) | 5-8 SP (明確) | 5-8 SP | N/A | [ADR-011](../docs/architecture/adr/ADR-011-framework-abstraction-layer.md) |
| **企業功能** | 79 SP | 71 SP | -8 SP | -10% ⚠️ | [US 6-11](../docs/user-stories/) |
| **整合測試** | 3 SP | 8 SP | +5 SP | 167% | [Test Strategy](../docs/testing/test-strategy.md) |
| **總計** | **160 SP** | **223-250 SP** | **63-90 SP** | **39-56%** | - |

**注意**: 企業功能 SP 減少是因為部分功能（如 Code Interpreter）已前移到 Phase 1A。

### 按 Phase 對比

| Phase | 原始計劃 | 調整後計劃 | 變更說明 | 依賴分析 |
|-------|---------|----------|---------|---------|
| **Phase 1A: 基礎平台** | 65 SP (Sprint 1-6) | 65 SP (Sprint 1-6) | 無變更 | [Dependency Matrix](./DEPENDENCY-MATRIX.md) |
| **Phase 1B: 工作流引擎核心** | 13 SP (Sprint 12-13) | 39-51 SP (Sprint 7-9) | 增加 26-38 SP, 前移到 Sprint 7-9 | 🔥 **Critical Path** |
| **Phase 1C: 編輯器 Backend** | 0 SP | 15-20 SP (Sprint 10-11) | **新增** | [Workflow API](../docs/api/workflow-api-design.md) |
| **Phase 1D: 編輯器 Frontend** | 0 SP | 20-25 SP (Sprint 12-14) | **新增** | [Workflow V2 Design](../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md) |
| **Phase 1E: 完整工作流引擎** | 0 SP | 13-18 SP (Sprint 15-16) | **新增** | [Development Strategy](./DEVELOPMENT-STRATEGY.md) |
| **Phase 1F: 企業功能與收尾** | 82 SP (Sprint 7-11, 14-18) | 71 SP (Sprint 17-21) | 調整 -11 SP | [Sprint Allocation](../docs/user-stories/sprints/sprint-allocation.md) |

### Story Points 增長分析

```yaml
主要增長來源:
  1. 工作流編輯器 (全新):
     Backend: 15-20 SP
     Frontend: 20-25 SP
     小計: 35-45 SP
     佔總增長: 55-67%
     參考:
       - docs/architecture/adr/ADR-012-workflow-editor-technology.md
       - docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md (完整設計)
       - claudedocs/1-planning/TECHNICAL-DECISIONS-LOG.md (TD-008)

  2. 工作流引擎完善:
     原: 13 SP → 調整: 39-51 SP
     增加: 26-38 SP
     佔總增長: 41-57%
     參考:
       - claudedocs/1-planning/DEVELOPMENT-STRATEGY.md (完整工作流引擎設計)
       - docs/technical-implementation/01-backend-net9/12-workflow-orchestration-implementation.md
       - claudedocs/1-planning/TECHNICAL-DECISIONS-LOG.md (TD-009)

  3. 抽象層明確化:
     原: 0 SP (隱含) → 調整: 5-8 SP (明確)
     增加: 5-8 SP
     佔總增長: 8-12%
     參考:
       - docs/architecture/adr/ADR-011-framework-abstraction-layer.md
       - claudedocs/1-planning/TECHNICAL-DECISIONS-LOG.md (TD-002)

  4. 整合測試增強:
     原: 3 SP → 調整: 8 SP
     增加: 5 SP
     佔總增長: 8%
     參考:
       - docs/testing/test-strategy.md
       - docs/testing/e2e-testing.md

總增長: 71-96 SP (44-60%)
風險評估: claudedocs/1-planning/RISK-REGISTER.md (RISK-002: SP 估算偏差)
```

---

## 時間線調整

### 原始 vs 調整後時間線

```yaml
原始計劃 (18 Sprints):
  總週數: 54 週
  總月數: 12.5 個月
  Sprint 平均長度: 3 週
  來源: docs/user-stories/sprints/sprint-allocation.md

  時間線:
    Week 1-18 (Sprint 1-6): 基礎與 Persona
    Week 19-33 (Sprint 7-11): Code Interpreter & Text-to-SQL
    Week 34-45 (Sprint 12-15): Multi-Agent & Chat
    Week 46-54 (Sprint 16-18): 企業功能與收尾

調整後計劃 (21 Sprints):
  總週數: 63 週
  總月數: 14.5 個月
  Sprint 平均長度: 3 週
  來源: claudedocs/1-planning/MVP-SCOPE-DEFINITION.md

  時間線:
    Week 1-18 (Sprint 1-6): Phase 1A - 基礎平台
    Week 19-27 (Sprint 7-9): Phase 1B - 工作流引擎核心 + 抽象層 🔥 Critical Path
    Week 28-33 (Sprint 10-11): Phase 1C - 工作流編輯器 Backend
    Week 34-42 (Sprint 12-14): Phase 1D - 工作流編輯器 Frontend
    Week 43-48 (Sprint 15-16): Phase 1E - 完整工作流引擎
    Week 49-63 (Sprint 17-21): Phase 1F - 企業功能與收尾

時間延長: 9 週 (2.1 個月, 17% 增長)
風險分析: claudedocs/1-planning/RISK-REGISTER.md (RISK-001: 時間延長影響交付)
依賴分析: claudedocs/1-planning/DEPENDENCY-MATRIX.md (Sprint 依賴與時間線)
```

### 時間線對比圖

```
原始計劃 (54 週):
├─ Phase 1: 基礎與 Persona (Week 1-18) ──────────────────────┤
├─ Phase 2: Code Interpreter & SQL (Week 19-33) ─────────────┤
├─ Phase 3: Multi-Agent & Chat (Week 34-45) ──────────┤
└─ Phase 4: 企業功能與收尾 (Week 46-54) ───────┤
   └─────────────────────────────────────────────────┘ 54 週

調整後計劃 (63 週):
├─ Phase 1A: 基礎平台 (Week 1-18) ────────────────────────┤
├─ Phase 1B: 工作流引擎核心 (Week 19-27) ────────┤ 🔥 Critical Path
├─ Phase 1C: 編輯器 Backend (Week 28-33) ──────┤
├─ Phase 1D: 編輯器 Frontend (Week 34-42) ────────┤
├─ Phase 1E: 完整工作流引擎 (Week 43-48) ──────┤
└─ Phase 1F: 企業功能與收尾 (Week 49-63) ───────────┤
   └───────────────────────────────────────────────────────┘ 63 週
                                                      ↑
                                              增加 9 週 (17%)
```

### 關鍵里程碑對比

| 里程碑 | 原始計劃 | 調整後計劃 | 延遲 | 參考文檔 |
|--------|---------|----------|------|---------|
| **基礎平台完成** | Week 18 (Sprint 6) | Week 18 (Sprint 6) | 0 週 | [US 1-3](../docs/user-stories/) |
| **Agent 引擎穩定** | Week 18 (Sprint 6) | Week 18 (Sprint 6) | 0 週 | [SK Integration](../docs/technical-implementation/01-backend-net9/02-semantic-kernel-integration.md) |
| **工作流引擎基礎** | Week 39 (Sprint 13) | Week 27 (Sprint 9) | -12 週 ✅ 提前 | [Development Strategy](./DEVELOPMENT-STRATEGY.md) |
| **工作流編輯器完成** | N/A (Phase 2) | Week 42 (Sprint 14) | N/A (新增) | [Workflow V2 Design](../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md) |
| **完整工作流引擎** | Week 39 (Sprint 13) | Week 48 (Sprint 16) | +9 週 | [Development Strategy](./DEVELOPMENT-STRATEGY.md) |
| **MVP 完成** | Week 54 (Sprint 18) | Week 63 (Sprint 21) | +9 週 | [MVP Scope](./MVP-SCOPE-DEFINITION.md) |

**關鍵觀察**:
- ✅ 工作流引擎基礎提前 12 週 (前移到 Sprint 7-9)
- ⚠️ 但完整工作流引擎延遲 9 週 (因範圍擴大)
- ⚠️ MVP 交付延遲 9 週 (因新增工作流編輯器)
- 🔥 Sprint 7-9 成為 Critical Path (參考: [DEPENDENCY-MATRIX.md](./DEPENDENCY-MATRIX.md))

---

## 風險評估

### 風險 1: 時間延長影響 MVP 交付 (高風險)

```yaml
風險描述:
  原計劃: 12.5 個月
  調整後: 14.5 個月
  延長: 2 個月 (17%)
  風險ID: RISK-001 (參考: claudedocs/1-planning/RISK-REGISTER.md)

影響:
  - MVP 上線時間延後
  - 市場窗口可能變化
  - 競爭對手可能先行

發生概率: 100% (確定發生)

緩解措施:
  ✅ 用戶已同意: "MVP範圍是可以分成為不同的phase 去實施的"
  ✅ 分階段交付: 6 phases (1A-1F) 可逐步驗證
     參考: claudedocs/1-planning/MVP-SCOPE-DEFINITION.md
  ✅ 價值優先: Phase 1A-1B 即可驗證核心價值
  ✅ 靈活調整: 如需要可延後非關鍵功能到 Phase 2

殘餘風險: 低 (已溝通並獲得支持)

參考文檔:
  - claudedocs/1-planning/RISK-REGISTER.md (RISK-001 完整分析)
  - claudedocs/1-planning/TECHNICAL-DECISIONS-LOG.md (TD-010: MVP 分階段實施)
```

### 風險 2: Story Points 估算偏差 (中風險)

```yaml
風險描述:
  總 SP 從 160 SP → 223-250 SP (增加 39-56%)
  範圍: 223-250 SP (27 SP 不確定性, 12% 變異)
  風險ID: RISK-002 (參考: claudedocs/1-planning/RISK-REGISTER.md)

影響:
  - 實際開發時間可能超出預期
  - 資源分配可能不足

發生概率: 40%

緩解措施:
  ✅ 保守估算: SP 範圍取上限 (250 SP)
  ✅ Buffer 時間: 每個 Sprint 預留 10% buffer
  ✅ 優先級管理: P0 > P1 > P2, 必要時砍 P2 功能
  ✅ 持續追蹤: 每個 Sprint 結束後更新估算
     參考: docs/project-management/sprint-planning.md

殘餘風險: 15%

參考文檔:
  - claudedocs/1-planning/RISK-REGISTER.md (RISK-002 完整分析)
  - docs/project-management/sprint-planning.md (Sprint 估算方法)
```

### 風險 3: 工作流編輯器技術風險 (中風險)

```yaml
風險描述:
  工作流編輯器是全新組件 (35-45 SP)
  技術棧: Vue 3 + VueFlow (微前端架構)
  團隊可能不熟悉 Vue 3
  風險ID: RISK-007 (參考: claudedocs/1-planning/RISK-REGISTER.md)

影響:
  - 開發進度可能延遲
  - 技術債務可能增加

發生概率: 30%

緩解措施:
  ✅ PoC 驗證: PoC 6 已驗證 VueFlow (98.2% 質量)
     參考: poc-projects/poc6-vueflow-crdt/POC-6-VALIDATION-REPORT.md
  ✅ 參考實現: n8n 源代碼可參考 (節省 35% 時間)
  ✅ 培訓計劃: 2 週 Vue 3 Composition API 培訓
  ✅ Code Review: 跨框架代碼審查機制

殘餘風險: 10%

參考文檔:
  - claudedocs/1-planning/RISK-REGISTER.md (RISK-007 完整分析)
  - docs/architecture/adr/ADR-012-workflow-editor-technology.md (技術選型)
  - docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md (完整設計)
```

### 風險 4: Multi-Agent 編排複雜度 (中風險)

```yaml
風險描述:
  完整工作流引擎需要 34-43 SP (vs 原 13 SP)
  複雜編排模式: Conditional, Loop, Handoff, Supervisor
  風險ID: RISK-001 (參考: claudedocs/1-planning/RISK-REGISTER.md)

影響:
  - 開發和測試時間可能不足
  - Bug 密度可能較高

發生概率: 35%

緩解措施:
  ✅ 分階段實施:
     Phase 1B: Sequential, Parallel (基礎)
     Phase 1E: Conditional, Loop, Handoff, Supervisor (高級)
     參考: claudedocs/1-planning/DEVELOPMENT-STRATEGY.md
  ✅ 充足測試: 單元測試 + 集成測試 + E2E 測試
     參考: docs/testing/test-strategy.md
  ✅ 參考架構: 參考 n8n, AutoGen 成熟設計
  ✅ 原型驗證: Sprint 7-8 先驗證核心編排邏輯

殘餘風險: 15%

參考文檔:
  - claudedocs/1-planning/RISK-REGISTER.md (RISK-001 完整分析)
  - claudedocs/1-planning/DEVELOPMENT-STRATEGY.md (工作流引擎完整設計)
  - docs/technical-implementation/01-backend-net9/12-workflow-orchestration-implementation.md
```

### 風險 5: Framework 抽象層性能開銷 (低風險)

```yaml
風險描述:
  抽象層可能引入性能開銷
  目標: Agent 執行 P95 <5 秒
  風險ID: RISK-018 (參考: claudedocs/1-planning/RISK-REGISTER.md)

影響:
  - 如開銷 >10%, 可能影響用戶體驗

發生概率: 20%

緩解措施:
  ✅ 簡潔設計: 抽象層設計最小化
     參考: docs/architecture/adr/ADR-011-framework-abstraction-layer.md
  ✅ 性能測試: 基準測試 (開銷 <5%)
     參考: docs/testing/performance-testing.md
  ✅ 關鍵路徑優化: 熱路徑避免不必要的抽象
  ✅ 緩存策略: 減少重複調用

殘餘風險: 5%

參考文檔:
  - claudedocs/1-planning/RISK-REGISTER.md (RISK-018 完整分析)
  - docs/architecture/adr/ADR-011-framework-abstraction-layer.md (性能考慮)
  - docs/testing/performance-testing.md (性能測試指南)
```

### 風險總結

| 風險 | 優先級 | 概率 | 影響 | 殘餘風險 | 狀態 | 參考 |
|------|--------|------|------|---------|------|------|
| 時間延長影響交付 | 高 | 100% | 高 | 低 | ✅ 已溝通 | [RISK-001](./RISK-REGISTER.md) |
| SP 估算偏差 | 中 | 40% | 中 | 15% | 🟡 持續追蹤 | [RISK-002](./RISK-REGISTER.md) |
| 工作流編輯器技術 | 中 | 30% | 中 | 10% | ✅ PoC 驗證 | [RISK-007](./RISK-REGISTER.md) |
| Multi-Agent 複雜度 | 中 | 35% | 中 | 15% | 🟡 分階段實施 | [RISK-001](./RISK-REGISTER.md) |
| 抽象層性能開銷 | 低 | 20% | 低 | 5% | ✅ 基準測試 | [RISK-018](./RISK-REGISTER.md) |

**完整風險分析**: 參考 [claudedocs/1-planning/RISK-REGISTER.md](./RISK-REGISTER.md) (27 risks tracked)

---

## 建議與下一步

### 關鍵建議

```yaml
建議 1 - 接受調整後的 21 Sprint 計劃:
  理由:
    - 用戶已明確要求工作流編輯器和完整工作流引擎在 MVP 中
    - 分階段實施 (6 phases) 系統化，可逐步驗證
    - 時間延長 2 個月可接受 (用戶已同意分 phase 實施)

  參考:
    - claudedocs/1-planning/TECHNICAL-DECISIONS-LOG.md (TD-008, TD-009, TD-010)
    - claudedocs/1-planning/MVP-SCOPE-DEFINITION.md (完整 6 Phases 定義)

  行動:
    - ✅ 管理層批准 21 Sprint 計劃
    - ✅ 更新項目時間線和資源分配
    - ✅ 與用戶溝通新的交付時間線

建議 2 - 嚴格執行 Phase-by-Phase 交付:
  理由:
    - 降低風險 (每個 Phase 獨立驗證)
    - 早期發現問題
    - 用戶可見進度

  參考:
    - claudedocs/1-planning/MVP-SCOPE-DEFINITION.md (Phase 定義與驗收標準)
    - docs/project-management/sprint-planning.md (Sprint 管理流程)

  行動:
    - ✅ 完成 Phase 1A (Sprint 1-6) 後進行 Demo
    - ✅ 完成 Phase 1B (Sprint 7-9) 後進行技術評審
    - ✅ 完成 Phase 1D (Sprint 12-14) 後進行用戶驗收
    - ✅ 每個 Phase 結束後進行 Retrospective

建議 3 - 優先級管理與靈活調整:
  理由:
    - SP 估算有 12% 不確定性 (223-250 SP)
    - 需要靈活應對時間壓力

  參考:
    - claudedocs/1-planning/RISK-REGISTER.md (RISK-002: SP 估算偏差)
    - docs/project-management/sprint-planning.md (優先級管理)

  行動:
    - ✅ 定義 P0 (Must Have) vs P1 (Should Have) vs P2 (Nice to Have)
    - ✅ 如時間緊張, P2 功能延後到 Phase 2
    - ✅ 每 3 Sprint 重新評估優先級

建議 4 - 強化風險監控:
  理由:
    - 5 個中高風險需要持續監控
    - 早期預警機制

  參考:
    - claudedocs/1-planning/RISK-REGISTER.md (27 risks tracked)
    - docs/project-management/risk-management.md (風險管理流程)

  行動:
    - ✅ 每週更新風險清單
    - ✅ Sprint Retrospective 討論風險
    - ✅ 關鍵指標監控 (SP Velocity, Bug Density, P95 Response Time)
```

### 下一步行動 (事項1 完成後)

```yaml
立即行動 (Week 1):
  - [ ] 管理層評審 21 Sprint 計劃
    參考: claudedocs/1-planning/MVP-SCOPE-DEFINITION.md

  - [ ] 批准時間線調整 (54 週 → 63 週)
    參考: 本文檔「時間線調整」章節

  - [ ] 批准 Story Points 增加 (160 SP → 223-250 SP)
    參考: 本文檔「Story Points 對比」章節

  - [ ] 與用戶溝通:
    - 新的 MVP 交付時間: 63 週 (vs 原 54 週)
    - 分階段交付計劃 (6 phases)
    - 關鍵里程碑日期

短期行動 (Week 2-4):
  - [ ] 啟動事項2: 為每個 Sprint 創建 7 文件:
    1. SPRINT-[X]-OVERVIEW.md
    2. SPRINT-[X]-PLAN.md
    3. SPRINT-[X]-CONTEXT.md
    4. SPRINT-[X]-CHECKLIST.md
    5. SPRINT-[X]-DEV-LOG.md
    6. SPRINT-[X]-ISSUES.md
    7. SPRINT-[X]-RETROSPECTIVE.md (Sprint 結束後)

    參考: claudedocs/2-sprints/sprint-1/ (範例文件結構)
    參考: claudedocs/2-sprints/sprint-2/ (範例文件結構)

  - [ ] 優先創建 Sprint 1-3 的文件 (Phase 1A 開始)

  - [ ] 準備技術培訓:
    - Vue 3 Composition API (2 週)
      參考: docs/architecture/adr/ADR-012-workflow-editor-technology.md
    - VueFlow 快速上手 (1 週)
      參考: poc-projects/poc6-vueflow-crdt/POC-6-VALIDATION-REPORT.md
    - Module Federation 原理 (3 天)

中期行動 (Month 2-3):
  - [ ] 完成 Sprint 1-6 (Phase 1A)
    參考: docs/user-stories/us-1-agent-crud.md ~ us-6-code-interpreter.md

  - [ ] Demo Phase 1A 成果給用戶

  - [ ] 技術評審: 基礎平台穩定性
    參考: docs/testing/test-strategy.md

  - [ ] 開始 Sprint 7-9 (Phase 1B) 🔥 Critical Path
    參考: claudedocs/1-planning/DEVELOPMENT-STRATEGY.md
    參考: claudedocs/1-planning/DEPENDENCY-MATRIX.md (Sprint 7-9 關鍵路徑)
```

---

## 附錄

### A. Sprint 分配變更對照表

| 原 Sprint | 原內容 | 調整後 Sprint | 調整後內容 | 變更類型 | 參考文檔 |
|----------|--------|--------------|----------|---------|---------|
| Sprint 1-6 | 基礎與 Persona (65 SP) | Sprint 1-6 | Phase 1A - 基礎平台 (65 SP) | 無變更 | [US 1-3](../docs/user-stories/) |
| Sprint 7 | US 5.1 Code Interpreter (13 SP) | Sprint 6 | 前移到 Phase 1A (5 SP 調整) | 前移 + 調整 | [ADR-008](../docs/architecture/adr/ADR-008-code-interpreter-container-pool.md) |
| Sprint 8-9 | US 6.1 Text-to-SQL (21 SP) | Sprint 17 | 後移到 Phase 1F | 後移 | [US 7](../docs/user-stories/us-7-text-to-sql.md) |
| Sprint 10-11 | US 7.1 Multimodal Output (13 SP) | Sprint 18 | 後移到 Phase 1F | 後移 | [US 8](../docs/user-stories/us-8-multimodal-output.md) |
| Sprint 12-13 | US 4.1 Multi-Agent (13 SP) | Sprint 7-9 | 拆分並擴展 (39-51 SP) | 拆分 + 擴展 | [Development Strategy](./DEVELOPMENT-STRATEGY.md) |
| - | - | Sprint 10-11 | **新增** Workflow Editor Backend (15-20 SP) | **新增** | [Workflow API](../docs/api/workflow-api-design.md) |
| - | - | Sprint 12-14 | **新增** Workflow Editor Frontend (20-25 SP) | **新增** | [Workflow V2 Design](../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md) |
| Sprint 14 | US 8.1 Chat (8 SP) | Sprint 19 | 後移到 Phase 1F, 合併 US 8.2 (16 SP) | 後移 + 合併 | [US 9](../docs/user-stories/us-9-chat-interface.md) |
| Sprint 15 | US 8.2 歷史記錄 (8 SP) | Sprint 19 | 合併到 Sprint 19 | 合併 | [Conversation UI](../docs/ux-design/wireframes/low-fidelity/05-conversation.md) |
| - | - | Sprint 15-16 | **新增** 完整工作流引擎 (13-18 SP) | **新增** | [Development Strategy](./DEVELOPMENT-STRATEGY.md) |
| Sprint 16-18 | 企業功能 (16 SP) | Sprint 20-21 | 調整企業功能 (21 SP) | 調整 | [US 10-11](../docs/user-stories/) |

### B. Story Points 增長明細

```yaml
原始計劃 Story Points (已明確):
  基礎設施: 13 SP
    參考: docs/user-stories/us-1-agent-crud.md
  Agent 引擎: 13 SP
    參考: docs/technical-implementation/01-backend-net9/02-semantic-kernel-integration.md
  Plugin 系統: 13 SP
    參考: docs/technical-implementation/01-backend-net9/04-plugin-system-mcp.md
  Persona: 13 SP
    參考: docs/technical-implementation/01-backend-net9/03-persona-framework.md
  Knowledge: 13 SP
    參考: docs/technical-implementation/01-backend-net9/05-knowledge-management-rag.md
  Code Interpreter: 13 SP
    參考: docs/architecture/adr/ADR-008-code-interpreter-container-pool.md
  Text-to-SQL: 21 SP
    參考: docs/technical-implementation/01-backend-net9/07-text-to-sql-implementation.md
  Multimodal Output: 13 SP
    參考: docs/technical-implementation/01-backend-net9/08-multimodal-output-engine.md
  Multi-Agent: 13 SP
    參考: docs/user-stories/us-4-multi-agent-orchestration.md
  Chat: 16 SP
    參考: docs/user-stories/us-9-chat-interface.md
  企業功能: 13 SP
    參考: docs/user-stories/us-10-batch-operations.md, us-11-monitoring-admin.md
  測試收尾: 3 SP
    參考: docs/testing/test-strategy.md
  小計: 157 SP

  未明確分配: ~5 SP
  總計: 162 SP

調整後 Story Points:
  Phase 1A (基礎平台): 65 SP
    參考: docs/user-stories/us-1-agent-crud.md ~ us-6-code-interpreter.md

  Phase 1B (工作流引擎核心 + 抽象層): 39-51 SP
    參考: claudedocs/1-planning/DEVELOPMENT-STRATEGY.md

  Phase 1C (編輯器 Backend): 15-20 SP
    參考: docs/api/workflow-api-design.md

  Phase 1D (編輯器 Frontend): 20-25 SP
    參考: docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md

  Phase 1E (完整工作流引擎): 13-18 SP
    參考: claudedocs/1-planning/DEVELOPMENT-STRATEGY.md (Phase 1E)

  Phase 1F (企業功能): 71 SP
    參考: docs/user-stories/ (US 6-11)

  總計: 223-250 SP

增長: 61-88 SP (38-54%)
```

---

## 參考文檔

### 核心專案文檔
1. **[MVP-SCOPE-DEFINITION.md](./MVP-SCOPE-DEFINITION.md)** - MVP 完整範圍定義與 6 Phases 詳細說明
2. **[TECHNICAL-DECISIONS-LOG.md](./TECHNICAL-DECISIONS-LOG.md)** - TD-008 (工作流編輯器), TD-009 (完整工作流引擎), TD-010 (MVP 分階段)
3. **[DEPENDENCY-MATRIX.md](./DEPENDENCY-MATRIX.md)** - 152 個依賴追蹤, Sprint 7-9 關鍵路徑警示
4. **[DEVELOPMENT-STRATEGY.md](./DEVELOPMENT-STRATEGY.md)** - 完整工作流引擎開發策略 (Task Generator, Multi-Agent Coordination, Framework Abstraction)
5. **[RISK-REGISTER.md](./RISK-REGISTER.md)** - 27 risks tracked (RISK-001, RISK-002, RISK-007, RISK-018)

### 架構設計文檔 (ADRs)
6. **[docs/architecture/Architecture-Design-Document.md](../docs/architecture/Architecture-Design-Document.md)** - 完整架構設計 (Clean Architecture + DDD + CQRS)
7. **[docs/architecture/adr/ADR-006-state-management-strategy.md](../docs/architecture/adr/ADR-006-state-management-strategy.md)** - 混合狀態管理 (Redis + PostgreSQL)
8. **[docs/architecture/adr/ADR-007-internal-communication-strategy.md](../docs/architecture/adr/ADR-007-internal-communication-strategy.md)** - 階段式通訊架構 (MediatR → Service Bus)
9. **[docs/architecture/adr/ADR-008-code-interpreter-container-pool.md](../docs/architecture/adr/ADR-008-code-interpreter-container-pool.md)** - Code Interpreter 容器池與 4-Layer Security
10. **[docs/architecture/adr/ADR-011-framework-abstraction-layer.md](../docs/architecture/adr/ADR-011-framework-abstraction-layer.md)** - Framework Abstraction Layer (5 core interfaces)
11. **[docs/architecture/adr/ADR-012-workflow-editor-technology.md](../docs/architecture/adr/ADR-012-workflow-editor-technology.md)** - 工作流編輯器技術選型 (Vue 3 + VueFlow + CRDT)

### User Stories (原始需求)
12. **[docs/user-stories/sprints/sprint-allocation.md](../docs/user-stories/sprints/sprint-allocation.md)** - 原始 18 Sprint 計劃
13. **[docs/user-stories/us-1-agent-crud.md](../docs/user-stories/us-1-agent-crud.md)** - Agent CRUD 與基礎設施
14. **[docs/user-stories/us-2-plugin-system.md](../docs/user-stories/us-2-plugin-system.md)** - Plugin 系統與熱加載
15. **[docs/user-stories/us-3-persona-definition.md](../docs/user-stories/us-3-persona-definition.md)** - Persona Framework
16. **[docs/user-stories/us-4-multi-agent-orchestration.md](../docs/user-stories/us-4-multi-agent-orchestration.md)** - Multi-Agent Orchestration (完整工作流引擎)
17. **[docs/user-stories/us-5-knowledge-management.md](../docs/user-stories/us-5-knowledge-management.md)** - Knowledge Management (RAG)
18. **[docs/user-stories/us-6-code-interpreter.md](../docs/user-stories/us-6-code-interpreter.md)** - Code Interpreter
19. **[docs/user-stories/us-7-text-to-sql.md](../docs/user-stories/us-7-text-to-sql.md)** - Text-to-SQL
20. **[docs/user-stories/us-8-multimodal-output.md](../docs/user-stories/us-8-multimodal-output.md)** - Multimodal Output Engine
21. **[docs/user-stories/us-9-chat-interface.md](../docs/user-stories/us-9-chat-interface.md)** - Chat Interface & 歷史記錄
22. **[docs/user-stories/us-10-batch-operations.md](../docs/user-stories/us-10-batch-operations.md)** - Batch Operations
23. **[docs/user-stories/us-11-monitoring-admin.md](../docs/user-stories/us-11-monitoring-admin.md)** - Monitoring & Admin Panel

### 技術實施文檔
24. **[docs/technical-implementation/01-backend-net9/01-monorepo-setup.md](../docs/technical-implementation/01-backend-net9/01-monorepo-setup.md)** - Monorepo 結構與 CI/CD
25. **[docs/technical-implementation/01-backend-net9/02-semantic-kernel-integration.md](../docs/technical-implementation/01-backend-net9/02-semantic-kernel-integration.md)** - Semantic Kernel 整合
26. **[docs/technical-implementation/01-backend-net9/03-persona-framework.md](../docs/technical-implementation/01-backend-net9/03-persona-framework.md)** - Persona Framework 實作
27. **[docs/technical-implementation/01-backend-net9/04-plugin-system-mcp.md](../docs/technical-implementation/01-backend-net9/04-plugin-system-mcp.md)** - Plugin System (MCP)
28. **[docs/technical-implementation/01-backend-net9/05-knowledge-management-rag.md](../docs/technical-implementation/01-backend-net9/05-knowledge-management-rag.md)** - Knowledge Management (RAG + Qdrant)
29. **[docs/technical-implementation/01-backend-net9/06-code-interpreter-implementation.md](../docs/technical-implementation/01-backend-net9/06-code-interpreter-implementation.md)** - Code Interpreter 實作
30. **[docs/technical-implementation/01-backend-net9/07-text-to-sql-implementation.md](../docs/technical-implementation/01-backend-net9/07-text-to-sql-implementation.md)** - Text-to-SQL 實作
31. **[docs/technical-implementation/01-backend-net9/08-multimodal-output-engine.md](../docs/technical-implementation/01-backend-net9/08-multimodal-output-engine.md)** - Multimodal Output Engine
32. **[docs/technical-implementation/01-backend-net9/09-prompt-engineering.md](../docs/technical-implementation/01-backend-net9/09-prompt-engineering.md)** - Prompt Engineering (Task Generator)
33. **[docs/technical-implementation/01-backend-net9/12-workflow-orchestration-implementation.md](../docs/technical-implementation/01-backend-net9/12-workflow-orchestration-implementation.md)** - Workflow Orchestration 實作

### UX 設計文檔 (Workflow Editor V2 完整設計)
34. **[docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md](../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md)** - Workflow Editor V2 索引與概覽
35. **[docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part1.md](../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part1.md)** - Part 1: 核心設計理念 (n8n-style)
36. **[docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part2.md](../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part2.md)** - Part 2: 內聯配置系統
37. **[docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part3.md](../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part3.md)** - Part 3: 執行視覺化
38. **[docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part4.md](../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part4.md)** - Part 4: 28 種節點類型
39. **[docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part5.md](../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part5.md)** - Part 5: 畫布互動 (Drag-drop, Zoom, Pan)
40. **[docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part6.md](../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part6.md)** - Part 6: 協作與版本控制 (CRDT)
41. **[docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part7.md](../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part7.md)** - Part 7: 響應式與驗收標準
42. **[docs/ux-design/wireframes/low-fidelity/05-conversation.md](../docs/ux-design/wireframes/low-fidelity/05-conversation.md)** - 對話界面設計
43. **[docs/ux-design/wireframes/low-fidelity/08-text-to-sql.md](../docs/ux-design/wireframes/low-fidelity/08-text-to-sql.md)** - Text-to-SQL UI 設計
44. **[docs/ux-design/wireframes/low-fidelity/12-monitoring.md](../docs/ux-design/wireframes/low-fidelity/12-monitoring.md)** - 監控面板設計

### API & Database 設計
45. **[docs/api/workflow-api-design.md](../docs/api/workflow-api-design.md)** - Workflow Editor Backend API 設計
46. **[docs/database/workflow-schema.md](../docs/database/workflow-schema.md)** - Workflow Database Schema
47. **[docs/database/redis-design.md](../docs/database/redis-design.md)** - Redis Checkpoint Store 設計
48. **[docs/database/qdrant-design.md](../docs/database/qdrant-design.md)** - Qdrant Vector Database 設計

### 測試、安全、部署文檔
49. **[docs/testing/test-strategy.md](../docs/testing/test-strategy.md)** - 測試策略 (≥85% unit test, ≥80% integration test)
50. **[docs/testing/e2e-testing.md](../docs/testing/e2e-testing.md)** - E2E 測試指南
51. **[docs/testing/performance-testing.md](../docs/testing/performance-testing.md)** - 性能測試 (P95 <5s, API <200ms)
52. **[docs/security/code-execution-security.md](../docs/security/code-execution-security.md)** - Code Interpreter 4-Layer Security
53. **[docs/deployment/mvp-deployment-guide.md](../docs/deployment/mvp-deployment-guide.md)** - MVP 部署指南

### 專案管理文檔
54. **[docs/project-management/sprint-planning.md](../docs/project-management/sprint-planning.md)** - Sprint 規劃與估算方法
55. **[docs/project-management/risk-management.md](../docs/project-management/risk-management.md)** - 風險管理流程

### Sprint 文檔範例
56. **[claudedocs/2-sprints/sprint-1/](../claudedocs/2-sprints/sprint-1/)** - Sprint 1 完整文檔結構範例 (7 files)
57. **[claudedocs/2-sprints/sprint-2/](../claudedocs/2-sprints/sprint-2/)** - Sprint 2 完整文檔結構範例 (7 files)

### PoC 驗證報告
58. **[poc-projects/poc6-vueflow-crdt/POC-6-VALIDATION-REPORT.md](../poc-projects/poc6-vueflow-crdt/POC-6-VALIDATION-REPORT.md)** - VueFlow + CRDT PoC 驗證 (98.2% quality)

---

**創建日期**: 2025-11-12
**最後更新**: 2025-11-12
**版本**: v2.0
**維護者**: 項目管理團隊

**變更日誌 (v1.0 → v2.0)**:
- ✅ 添加 58 個 `/docs` 引用，覆蓋所有關鍵文檔領域
- ✅ 增強與其他規劃文檔的雙向可追溯性 (DEPENDENCY-MATRIX.md, DEVELOPMENT-STRATEGY.md, RISK-REGISTER.md)
- ✅ 添加內聯技術文檔引用到所有 Sprint 和 Gap 分析
- ✅ 擴展「參考文檔」章節，組織成 11 個類別（核心專案、ADRs、User Stories、技術實施、UX 設計、API/Database、測試/安全/部署、專案管理、Sprint 範例、PoC 報告）
- ✅ 在所有風險評估章節添加 RISK-REGISTER.md 交叉引用
- ✅ 強化 Sprint 7-9 Critical Path 標記與 DEPENDENCY-MATRIX.md 連結
- ✅ 添加完整 Workflow Editor V2 7-part 設計引用
- ✅ 增強 Framework Abstraction Layer 與 ADR-011 連結
- ✅ 所有 Phase 和 Sprint 添加主要參考文檔
