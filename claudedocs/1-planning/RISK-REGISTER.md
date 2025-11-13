# 風險登記冊 (Risk Register)

**Semantic Kernel Agentic Framework - Comprehensive Risk Registry**

**版本**: 2.0
**創建日期**: 2025-12-11
**最後更新**: 2025-12-12
**維護責任**: Project Manager + Scrum Master
**狀態**: 🟢 Active

---

## 📋 文檔目的

本文檔提供專案完整風險登記冊，基於 [docs/brief/Project-Brief.md](../../docs/brief/Project-Brief.md) 的專案範圍和 [docs/architecture/Architecture-Design-Document.md](../../docs/architecture/Architecture-Design-Document.md) 的技術架構，包括：
- 🎯 **風險識別**: 所有已知風險和潛在風險
- 📊 **風險評估**: 影響程度、發生機率、風險等級
- 🛡️ **緩解策略**: 風險應對措施和行動計劃
- 📈 **風險監控**: 風險狀態追蹤和趨勢分析

---

## 🎯 風險評估矩陣

### 影響程度 (Impact)
- **🔥 Critical (5)**: 專案失敗或重大延遲 (>4 週)
- **🔴 High (4)**: 嚴重影響時程或預算 (2-4 週延遲)
- **🟡 Medium (3)**: 中等影響 (1-2 週延遲)
- **🟢 Low (2)**: 輕微影響 (<1 週延遲)
- **⚪ Minimal (1)**: 可忽略影響

### 發生機率 (Probability)
- **🔥 Very High (5)**: >80% 機率發生
- **🔴 High (4)**: 60-80% 機率
- **🟡 Medium (3)**: 40-60% 機率
- **🟢 Low (2)**: 20-40% 機率
- **⚪ Very Low (1)**: <20% 機率

### 風險等級 (Risk Level = Impact × Probability)
- **🔥 Critical**: 16-25 (立即行動)
- **🔴 High**: 10-15 (優先處理)
- **🟡 Medium**: 6-9 (監控並規劃)
- **🟢 Low**: 3-5 (接受並監控)
- **⚪ Minimal**: 1-2 (記錄即可)

---

## 📊 風險總覽

### 當前風險統計 (截至 2025-12-12)

| 風險類別 | Critical | High | Medium | Low | 總計 |
|---------|---------|------|--------|-----|------|
| **技術風險** | 1 | 2 | 3 | 2 | 8 |
| **資源風險** | 0 | 1 | 2 | 1 | 4 |
| **時程風險** | 0 | 2 | 1 | 1 | 4 |
| **範圍風險** | 1 | 1 | 2 | 0 | 4 |
| **質量風險** | 0 | 1 | 2 | 2 | 5 |
| **外部風險** | 0 | 0 | 1 | 1 | 2 |
| **總計** | **2** | **7** | **11** | **7** | **27** |

### 風險狀態分布

| 狀態 | 數量 | 佔比 |
|-----|------|------|
| 🔴 **Active** (需要行動) | 9 | 33% |
| 🟡 **Monitoring** (監控中) | 11 | 41% |
| 🟢 **Mitigated** (已緩解) | 5 | 19% |
| ⚫ **Closed** (已關閉) | 2 | 7% |

---

## 🔥 Critical 級別風險 (2 個)

### RISK-001: 工作流引擎複雜度超出預期 🔥
**類別**: 技術風險
**狀態**: 🔴 Active
**識別日期**: 2025-11-12
**負責人**: Tech Lead
**參考文檔**: [DEVELOPMENT-STRATEGY.md](./DEVELOPMENT-STRATEGY.md), [DEPENDENCY-MATRIX.md](./DEPENDENCY-MATRIX.md)

#### 風險描述
完整工作流引擎開發複雜度遠超原始評估，可能導致專案嚴重延遲或功能縮減。

**詳細說明** (參考 [docs/user-stories/module-04-workflow-orchestration/README.md](../../docs/user-stories/module-04-workflow-orchestration/README.md)):
- 原始計劃: US 4.1 Multi-Agent Orchestration (13 SP, Sprint 12-13)
- 實際需求: 完整工作流引擎需要 34-43 SP (增加 21-30 SP, +161%-231%)
- 功能範圍:
  - Task Generator (LLM 驅動任務規劃) - [US-7.3-Task-Generation-with-LLM.md](../../docs/user-stories/module-04-workflow-orchestration/US-7.3-Task-Generation-with-LLM.md)
  - Multi-Agent Coordination - [US-7.4-Multi-Agent-Coordination.md](../../docs/user-stories/module-04-workflow-orchestration/US-7.4-Multi-Agent-Coordination.md)
  - Agent Messaging (Message Bus) - [docs/architecture/adr/ADR-007-phased-communication-architecture.md](../../docs/architecture/adr/ADR-007-phased-communication-architecture.md)
  - Workflow Execution Engine - [US-7.2-Workflow-Execution-Engine.md](../../docs/user-stories/module-04-workflow-orchestration/US-7.2-Workflow-Execution-Engine.md)
  - State Management (Checkpoint/Resume) - [docs/architecture/adr/ADR-006-hybrid-state-management.md](../../docs/architecture/adr/ADR-006-hybrid-state-management.md)
  - Feedback Loop - [US-7.6-Feedback-Loop.md](../../docs/user-stories/module-04-workflow-orchestration/US-7.6-Feedback-Loop.md)

#### 風險評估
- **影響程度**: 🔥 Critical (5) - 可能導致 MVP 交付失敗
- **發生機率**: 🔴 High (4) - 已在 [TECHNICAL-DECISIONS-LOG.md](./TECHNICAL-DECISIONS-LOG.md) (TD-009) 確認低估
- **風險等級**: 🔥 **Critical (20)** = 5 × 4

#### 潛在影響
```yaml
時程影響:
  原計劃: Sprint 12-13 (6 週)
  調整後: Sprint 7-9 (Phase 1B) + Sprint 15-16 (Phase 1E) (12 週)
  延遲: +6 週 (100% 增加)
  參考: docs/project-management/sprint-planning-template.md

成本影響:
  額外開發: 21-30 SP
  額外資源: 可能需要增加 1 名資深開發者
  參考: docs/brief/Project-Brief.md (Resource Planning)

質量影響 (參考: docs/testing/testing-strategy.md):
  測試複雜度: 大幅增加
  單元測試需求: ≥85% 覆蓋率
  集成測試複雜度: Sequential, Parallel, Conditional, Loop 場景
  Bug 風險: 高複雜度系統的潛在缺陷

範圍影響:
  可能被迫削減:
    - Code Interpreter 部分功能 (docs/user-stories/module-05-code-interpreter/README.md)
    - Text-to-SQL 部分進階功能 (docs/user-stories/module-06-text-to-sql/README.md)
```

#### 緩解策略
```yaml
策略 1 - 分階段實施 (參考: DEVELOPMENT-STRATEGY.md):
  Phase 1B (Sprint 7-9, 26 SP):
    - Task Generator (5-8 SP)
    - Multi-Agent Coordination Sequential/Parallel (8-10 SP)
    - Agent Messaging (5 SP)
    - Workflow Execution Engine 基礎 (8-10 SP)
    - 參考: docs/technical-implementation/01-backend-net9/12-workflow-orchestration-implementation.md

  Phase 1E (Sprint 15-16, 13-18 SP):
    - Conditional, Loop, Handoff, Supervisor
    - State Management (Checkpoint/Resume)
    - Feedback Loop
    - 參考: docs/user-stories/module-04-workflow-orchestration/US-7.4-Multi-Agent-Coordination.md

  狀態: ✅ 已採納

策略 2 - 技術預研 (Spike):
  Sprint 6 最後 2 天: 技術 Spike
    - 評估 Semantic Kernel Process Framework 可用性
      (參考: docs/architecture/adr/ADR-001-semantic-kernel-vs-agent-framework.md)
    - 評估自建 vs 採用開源方案 (Temporal, Apache Airflow)
      (參考: docs/architecture/adr/ADR-003-workflow-execution-engine.md)
    - 原型驗證核心功能可行性

  狀態: ⏳ 計劃中

策略 3 - 引入專家:
  時機: Sprint 7 開始
  資源: 增加 1 名資深工作流引擎開發者 (0.5 FTE)
  期間: Sprint 7-16 (10 週)
  技能需求: LLM orchestration, async messaging, state management
    (參考: docs/standards/coding-standards.md)

  狀態: ⏳ 待批准

策略 4 - Framework Abstraction Layer (參考: docs/architecture/adr/ADR-011-framework-abstraction-layer.md):
  實施:
    - 定義 5 個核心接口 (IAgentRuntime, IToolRegistry, IAgentMemory, IWorkflowOrchestrator, IObservability)
    - 實現 SemanticKernelAdapter
    - 保留未來遷移能力

  狀態: ✅ 已採納，Phase 1B 實施

策略 5 - 降低其他模組優先級:
  降級為 P1:
    - US 5.1 Code Interpreter 部分功能 (docs/user-stories/module-05-code-interpreter/US-5.1-Code-Execution-API.md)
    - US 6.1 Text-to-SQL 部分進階功能 (docs/user-stories/module-06-text-to-sql/US-6.1-Text-to-SQL-API.md)
  保留核心: Agent, Plugin, Workflow, Knowledge, Chat

  狀態: ⚠️ 備用方案
```

#### 監控指標
```yaml
Sprint 7-9 進度監控 (參考: docs/testing/testing-strategy.md):
  - Phase 1B 完成度 (目標: 100%)
  - 單元測試覆蓋率 (目標: ≥85%)
  - 工作流執行成功率 (目標: ≥95%)
  - 性能基準: 工作流啟動 <2s, 任務執行 <5s
    (參考: docs/technical-implementation/08-performance-optimization/01-performance-requirements.md)

技術複雜度指標 (參考: docs/standards/coding-standards.md):
  - Code complexity (目標: ≤15 cyclomatic complexity)
  - 集成測試通過率 (目標: 100%)
  - API 響應時間 <200ms (參考: docs/api/workflow-api-specification.md)
```

#### 應急預案
```yaml
如果 Sprint 9 結束仍未完成核心引擎:
  1. 召開緊急 Stakeholder 會議
  2. 評估以下選項:
     - Option A: 延長 Sprint 7-9 到 4-5 週
     - Option B: 降低工作流引擎範圍 (僅 Sequential + Parallel)
     - Option C: 採用 Semantic Kernel Process Framework
       (參考: docs/architecture/adr/ADR-001-semantic-kernel-vs-agent-framework.md)
  3. 更新專案計劃和時程表
```

---

### RISK-002: 範圍蔓延 (Scope Creep) 持續發生 🔥
**類別**: 範圍風險
**狀態**: 🔴 Active
**識別日期**: 2025-12-07
**負責人**: Scrum Master + Product Owner
**參考文檔**: [CHANGE-LOG.md](../4-changes/CHANGE-LOG.md), [docs/standards/development-workflow.md](../../docs/standards/development-workflow.md)

#### 風險描述
專案已發生 2 次嚴重範圍蔓延 (CHANGE-001, CHANGE-002)，如果持續發生將導致專案無法按時交付。

**歷史記錄** (參考 [CHANGE-LOG.md](../4-changes/CHANGE-LOG.md)):
- **CHANGE-001**: US 1.4 從 5 SP 擴展為 13 SP (+8 SP, +160%, +6 days)
  - 參考: [docs/user-stories/module-01-agent-management/US-1.4-Agent-Execution-Engine.md](../../docs/user-stories/module-01-agent-management/US-1.4-Agent-Execution-Engine.md)
- **CHANGE-002**: US 2.1 自然延伸至 US 2.2/2.3 部分功能 (+1 day, 但節省未來 2-3 days)
  - 參考: [docs/user-stories/module-02-plugin-system/US-2.1-Plugin-System-Core.md](../../docs/user-stories/module-02-plugin-system/US-2.1-Plugin-System-Core.md)

#### 風險評估
- **影響程度**: 🔥 Critical (5) - 累積延遲可能超過 4 週
- **發生機率**: 🔴 High (4) - 已發生 2 次，趨勢明顯
- **風險等級**: 🔥 **Critical (20)** = 5 × 4

#### 潛在影響
```yaml
時程累積影響:
  Sprint 2 延遲: +6 days (CHANGE-001)
  如果 Sprint 3-21 各延遲 2 days: +40 days (8 週)
  總延遲: 63 週 → 71+ 週 (增加 2 個月)

成本影響:
  額外開發: 每次變更平均 +4 SP
  如果發生 5 次: +20 SP (+9% 總工作量)
  參考: docs/brief/Project-Brief.md (Budget Allocation)

團隊士氣:
  持續加班: 導致團隊疲勞
  範圍不確定: 降低團隊信心
  參考: docs/project-management/team-health-guidelines.md
```

#### 緩解策略
```yaml
策略 1 - 建立正式變更控制流程:
  實施日期: 2025-12-10
  流程文檔: docs/standards/change-management-process.md
  模板: SCOPE-CHANGE-REQUEST.md (claudedocs/4-changes/)

  流程:
    1. 填寫 SCOPE-CHANGE-REQUEST.md
    2. Tech Lead 技術評審
    3. Product Owner 業務評審
    4. Scrum Master 流程評審
    5. 三方簽署批准

  審批標準:
    - 🟢 低影響 (≤1 SP, ≤1 day): Tech Lead
    - 🟡 中影響 (≤3 SP, ≤3 days): Tech Lead + PO
    - 🔴 高影響 (>3 SP, >3 days): Tech Lead + PO + SM + PM

  狀態: ✅ 已實施 (2025-12-11)

策略 2 - 加強 Sprint Planning:
  改進措施 (參考: docs/project-management/sprint-planning-template.md):
    - DoD (Definition of Done) 必須在 Planning 時明確
    - 技術複雜度評估包含依賴分析
      (參考: DEPENDENCY-MATRIX.md)
    - Story Points 評估包含測試和文檔工作量
      (參考: docs/testing/testing-strategy.md)
    - 預留 10% buffer (1.3 SP per Sprint)

  實施: Sprint 3 Planning (2025-12-16)
  狀態: ⏳ 計劃中

策略 3 - 引入 Spike 機制:
  適用場景: 技術複雜度不確定的 User Story
  Spike 時間: 0.5-2 days
  產出: 詳細工作量評估 + 技術方案
  參考: docs/standards/spike-guidelines.md

  首次應用: Sprint 6 工作流引擎 Spike
  狀態: ⏳ 計劃中

策略 4 - 每 Sprint 中期檢查點:
  時機: Sprint Day 7-8 (中期)
  檢查內容:
    - 進度是否符合預期 (±10%)
    - 是否出現範圍蔓延跡象
    - 是否需要調整後續任務

  實施: Sprint 3 開始
  狀態: ⏳ 計劃中
```

#### 監控指標
```yaml
每 Sprint 監控:
  - 實際 SP vs 計劃 SP (目標: ≤110%)
  - 實際天數 vs 計劃天數 (目標: ≤105%)
  - 變更請求數量 (目標: ≤1 per Sprint)
  - 參考: docs/project-management/sprint-metrics-tracking.md

累積趨勢:
  - 平均 SP 增長率 (目標: <5% per Sprint)
  - 累積延遲天數 (目標: <10 days total)
```

---

## 🔴 High 級別風險 (7 個)

### RISK-003: Semantic Kernel 框架限制 🔴
**類別**: 技術風險
**狀態**: 🟡 Monitoring
**識別日期**: 2025-10-28
**負責人**: Tech Lead
**參考文檔**: [docs/architecture/adr/ADR-001-semantic-kernel-vs-agent-framework.md](../../docs/architecture/adr/ADR-001-semantic-kernel-vs-agent-framework.md)

#### 風險描述
Semantic Kernel Process Framework 可能無法滿足複雜工作流需求，導致需要大量自建。

**技術限制** (參考 [docs/architecture/adr/ADR-003-workflow-execution-engine.md](../../docs/architecture/adr/ADR-003-workflow-execution-engine.md)):
- ❌ 缺乏 Conditional, Loop 支援
- ❌ 狀態管理不完整 (無 Checkpoint/Resume)
- ❌ 不支援複雜的 Multi-Agent 協作模式

#### 風險評估
- **影響程度**: 🔴 High (4) - 需要自建完整工作流引擎
- **發生機率**: 🟡 Medium (3) - TD-009 已確認部分限制
- **風險等級**: 🔴 **High (12)** = 4 × 3

#### 緩解策略
```yaml
策略 - Framework Abstraction Layer:
  實施: TD-002, ADR-011
  文檔: docs/architecture/adr/ADR-011-framework-abstraction-layer.md

  設計:
    - 定義 5 個核心接口:
      1. IAgentRuntime - Agent 執行抽象
      2. IToolRegistry - Tool/Plugin 註冊抽象 (MCP 支援)
      3. IAgentMemory - Agent 記憶抽象
      4. IWorkflowOrchestrator - 工作流編排抽象
      5. IObservability - 可觀測性抽象 (OpenTelemetry)

    - 實現 SemanticKernelAdapter
      (參考: docs/technical-implementation/01-backend-net9/05-semantic-kernel-integration.md)

    - 保留未來遷移到其他框架的能力
      (LangChain, Microsoft Agent Framework)

  狀態: ✅ 已採納，Phase 1B 實施
```

---

### RISK-004: 工作流編輯器前端複雜度 🔴
**類別**: 技術風險
**狀態**: 🟡 Monitoring
**識別日期**: 2025-11-12
**負責人**: Frontend Lead
**參考文檔**: [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md)

#### 風險描述
VueFlow 集成和複雜的 Drag-drop 交互開發難度高，可能超出前端團隊能力。

**技術挑戰** (參考完整 7 部分設計):
- n8n 風格大卡片節點系統 (part1: 核心設計理念)
- 內聯配置面板 (part2: 內聯配置)
- 執行視覺化實時更新 (part3: 執行視覺化)
- 28 種節點類型實現 (part4: 節點類型)
- 畫布互動與智能連接線 (part5: 畫布互動)
- 多用戶實時協作 CRDT (part6: 協作與架構)
- 響應式設計與無障礙 (part7: 響應式與驗收)

#### 風險評估
- **影響程度**: 🔴 High (4) - 工作流編輯器是 MVP 核心功能
- **發生機率**: 🟡 Medium (3) - 前端團隊缺乏 VueFlow 經驗
- **風險等級**: 🔴 **High (12)** = 4 × 3

#### 緩解策略
```yaml
策略 1 - 技術培訓:
  時機: Sprint 9 期間 (Phase 1D 開始前)
  內容:
    - VueFlow 官方教程 (2 days)
    - ReactFlow 參考案例研究 (1 day)
    - 原型開發練習 (2 days)
  參考: docs/technical-implementation/02-frontend-react/README.md

  狀態: ⏳ 計劃中

策略 2 - 外部諮詢:
  雇用 VueFlow 專家進行 2 週輔導 (Sprint 10-11)
  技能需求: Vue 3 Composition API, VueFlow, Canvas interactions

  狀態: ⏳ 待批准

策略 3 - PoC 驗證 (參考: poc-projects/poc6-vueflow-crdt/):
  已完成 PoC 6: VueFlow + CRDT 可行性驗證
  質量評分: 98.2%
  驗證內容:
    - VueFlow 基礎畫布功能
    - CRDT 多用戶協作
    - 性能基準測試 (支援 100+ 節點)

  狀態: ✅ 已驗證

策略 4 - 降級備用方案:
  如果 VueFlow 太複雜，降級為簡化編輯器:
    - 使用 JSON 表單編輯工作流 (而非可視化)
    - 僅支援 Template 選擇 (而非自由編輯)
    - 參考: docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-part7.md (備用方案)

  狀態: ⚠️ 備用方案
```

---

### RISK-005: 前端開發資源不足 🔴
**類別**: 資源風險
**狀態**: 🔴 Active
**識別日期**: 2025-11-07
**負責人**: Project Manager
**參考文檔**: [docs/brief/Project-Brief.md](../../docs/brief/Project-Brief.md) (Resource Planning)

#### 風險描述
Frontend 工作量 (React 主應用 + Vue Workflow 編輯器) 需要 2 名全職前端，但目前只有 1 名。

**工作量分析**:
- React 18 主應用 (12 核心頁面):
  - Dashboard, Agent List/Create/Detail
  - Chat Interface, Knowledge Base, Code Interpreter
  - Text-to-SQL, Persona Builder, Settings, Monitoring
  - 參考: [docs/ux-design/README.md](../../docs/ux-design/README.md)

- Vue 3 Workflow Editor (完整功能):
  - 28 種節點類型實現
  - 內聯配置面板
  - 執行視覺化
  - 實時協作 (CRDT)
  - 參考: [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md)

#### 風險評估
- **影響程度**: 🔴 High (4) - 前端延遲會影響整體交付
- **發生機率**: 🔴 High (4) - 工作量評估清楚
- **風險等級**: 🔴 **High (16)** = 4 × 4

#### 緩解策略
```yaml
策略 - 招聘第二名前端:
  時機: Sprint 6 之前 (Phase 1D 開始前)
  要求:
    - React 18 熟練
    - Vue 3 經驗
    - Material-UI, VueFlow 經驗優先
    - TypeScript 精通
  技能評估: docs/standards/frontend-coding-standards.md

  狀態: ⏳ 招聘中
```

---

### RISK-006: Sprint 2 延遲影響後續 Sprint 🔴
**類別**: 時程風險
**狀態**: 🔴 Active
**識別日期**: 2025-12-07
**負責人**: Scrum Master
**參考文檔**: [docs/project-management/sprint-planning-template.md](../../docs/project-management/sprint-planning-template.md)

#### 風險描述
Sprint 2 已延遲 6 天，可能導致 Sprint 3-21 全部延遲。

#### 風險評估
- **影響程度**: 🔴 High (4) - 累積延遲可能達 2 個月
- **發生機率**: 🟡 Medium (3) - 部分可透過優化緩解
- **風險等級**: 🔴 **High (12)** = 4 × 3

#### 緩解策略
```yaml
策略 1 - 調整 Sprint 3 計劃:
  接受 Sprint 2 延遲 6 天
  Sprint 3 開始: 2025-12-22 (而非 2025-12-16)
  參考: claudedocs/2-sprints/sprint-3/SPRINT-3-PLAN.md

  狀態: ✅ 已調整

策略 2 - 優化後續 Sprint:
  每個 Sprint 嘗試節省 0.5 day
  通過 12 個 Sprint: 節省 6 days
  抵銷 Sprint 2 延遲
  優化方向:
    - 改進 Code Review 效率 (docs/standards/code-review-checklist.md)
    - 減少會議時間
    - 提升自動化測試覆蓋率 (docs/testing/unit-testing-guidelines.md)

  狀態: ⏳ 執行中
```

---

### RISK-007: Code Interpreter Docker 安全隔離 🔴
**類別**: 質量風險
**狀態**: 🟡 Monitoring
**識別日期**: 2025-11-07
**負責人**: Security Engineer
**參考文檔**: [docs/architecture/adr/ADR-008-code-interpreter-container-pool.md](../../docs/architecture/adr/ADR-008-code-interpreter-container-pool.md), [docs/security/code-execution-security.md](../../docs/security/code-execution-security.md)

#### 風險描述
Code Interpreter 執行用戶代碼，Docker 沙箱可能存在安全漏洞。

**安全威脅** (參考 [docs/security/code-execution-security.md](../../docs/security/code-execution-security.md)):
- Container Escape (容器逃逸)
- Resource Exhaustion (資源耗盡攻擊)
- Malicious Code Injection (惡意代碼注入)
- Network Access Abuse (網路訪問濫用)

#### 風險評估
- **影響程度**: 🔴 High (4) - 安全漏洞可能導致系統被攻擊
- **發生機率**: 🟡 Medium (3) - Docker 隔離通常可靠，但仍有風險
- **風險等級**: 🔴 **High (12)** = 4 × 3

#### 緩解策略
```yaml
策略 1 - 4-Layer Security Architecture (參考: ADR-008):
  Layer 1 - Container 隔離:
    - Docker isolation with restricted capabilities
    - Non-root user execution
    - Read-only filesystem (除 /tmp)

  Layer 2 - Resource Limits:
    - CPU: 0.5 core
    - Memory: 512MB
    - Disk I/O: Limited
    - Execution timeout: 30s (default), max 300s

  Layer 3 - Network Isolation:
    - network_mode: "none" (完全禁用網路)
    - 僅允許與 API Gateway 通訊

  Layer 4 - Seccomp Profile:
    - 禁用危險系統調用
    - 限制檔案系統訪問
    - 禁用 kernel module loading

  實施: docs/technical-implementation/01-backend-net9/08-code-interpreter-implementation.md
  狀態: ⏳ Phase 1C 實施

策略 2 - 安全審計:
  時機: Sprint 10 完成後
  內容:
    - 滲透測試 (Penetration Testing)
    - 程式碼安全審查 (Security Code Review)
    - 容器逃逸測試 (Container Escape Testing)
  參考: docs/security/security-testing-checklist.md

  狀態: ⏳ 計劃中

策略 3 - 容器生命週期管理:
  動態容器池:
    - Pre-warmed containers (5 個待命)
    - 使用後立即銷毀 (用完即棄)
    - 定時清理 (防止資源洩漏)
  參考: docs/architecture/adr/ADR-008-code-interpreter-container-pool.md

  狀態: ⏳ Phase 1C 實施
```

---

### RISK-008: US 2.2/2.3 剩餘工作低估 🔴
**類別**: 範圍風險
**狀態**: 🟡 Monitoring
**識別日期**: 2025-12-11
**負責人**: Tech Lead
**參考文檔**: [docs/user-stories/module-02-plugin-system/US-2.2-Plugin-Version-Management.md](../../docs/user-stories/module-02-plugin-system/US-2.2-Plugin-Version-Management.md), [US-2.3-Plugin-Hot-Reload.md](../../docs/user-stories/module-02-plugin-system/US-2.3-Plugin-Hot-Reload.md)

#### 風險描述
US 2.2/2.3 已完成 Phase 1-2 (40%, 30%)，剩餘 Phase 3-5 可能比預估 (2-3 days, 3-4 days) 更久。

#### 風險評估
- **影響程度**: 🟡 Medium (3) - 可能延遲 Sprint 2 完成 1-2 days
- **發生機率**: 🔴 High (4) - API 層和測試通常比預期久
- **風險等級**: 🔴 **High (12)** = 3 × 4

#### 緩解策略
```yaml
策略 - 預留緩衝時間:
  US 2.2 預估: 2-3 days → 調整為 3-4 days
  US 2.3 預估: 3-4 days → 調整為 4-5 days

  Phase 3-5 工作內容 (參考 US-2.2, US-2.3):
    - API Layer (Controller, DTOs)
    - 集成測試 (Version rollback scenarios)
    - 文檔更新 (API specifications)

  狀態: ✅ 已調整計劃
```

---

### RISK-009: 測試覆蓋率下降 🔴
**類別**: 質量風險
**狀態**: 🟡 Monitoring
**識別日期**: 2025-12-07
**負責人**: QA Lead
**參考文檔**: [docs/testing/testing-strategy.md](../../docs/testing/testing-strategy.md), [docs/testing/unit-testing-guidelines.md](../../docs/testing/unit-testing-guidelines.md)

#### 風險描述
Sprint 2 快速開發可能導致測試覆蓋率下降，技術債務累積。

**質量門檻** (參考 [docs/standards/coding-standards.md](../../docs/standards/coding-standards.md)):
- 單元測試覆蓋率: ≥85%
- 集成測試覆蓋率: ≥80%
- Code Complexity: ≤15 (cyclomatic complexity)
- 0 Critical/High severity bugs

#### 風險評估
- **影響程度**: 🟡 Medium (3) - 質量下降影響後續開發
- **發生機率**: 🔴 High (4) - Sprint 2 時間壓力大
- **風險等級**: 🔴 **High (12)** = 3 × 4

#### 緩解策略
```yaml
策略 - 測試門檻 (參考: docs/testing/testing-strategy.md):
  CI/CD Pipeline 強制檢查:
    - 單元測試覆蓋率 ≥80%
    - 集成測試通過率 100%
    - SonarQube Quality Gate Pass
    - 無 Critical/High severity bugs

  每個 PR 必須滿足以上條件才能 merge
  參考: docs/devops/ci-cd-pipeline-configuration.md

  狀態: ✅ 已實施 (CI/CD Pipeline)
```

---

## 🟡 Medium 級別風險 (11 個)

### RISK-010: Vector DB 選型延遲 🟡
**類別**: 技術風險
**狀態**: 🟡 Monitoring
**識別日期**: 2025-11-07
**負責人**: Tech Lead
**參考文檔**: [docs/user-stories/module-03-knowledge-management/US-4.2-RAG-Implementation.md](../../docs/user-stories/module-03-knowledge-management/US-4.2-RAG-Implementation.md)

#### 風險描述
Qdrant vs Chroma 選型尚未決定，可能影響 Sprint 5 Knowledge 檢索開發。

**評估標準** (參考 [docs/technical-implementation/03-data-layer/vector-database-evaluation.md](../../docs/technical-implementation/03-data-layer/vector-database-evaluation.md)):
- 性能基準測試 (Query latency, Indexing speed)
- 部署複雜度 (Docker, Kubernetes)
- 社群支援度 (GitHub stars, documentation)
- 功能完整性 (Filtering, hybrid search)

#### 風險評估
- **影響程度**: 🟡 Medium (3)
- **發生機率**: 🟡 Medium (3)
- **風險等級**: 🟡 **Medium (9)** = 3 × 3

#### 緩解策略
```yaml
策略 - Sprint 4 技術評估:
  時機: Sprint 4 期間
  評估內容:
    - 性能基準測試 (1M vectors, P99 latency)
    - 部署複雜度測試
    - API 易用性評估
  參考: docs/technical-implementation/03-data-layer/vector-database-evaluation.md

  決策日期: Sprint 4 結束前
  狀態: ⏳ 計劃中
```

---

### RISK-011: PostgreSQL 性能瓶頸 🟡
**類別**: 技術風險
**狀態**: 🟢 Mitigated
**識別日期**: 2025-11-05
**負責人**: Backend Lead
**參考文檔**: [docs/database/database-schema.md](../../docs/database/database-schema.md), [docs/technical-implementation/08-performance-optimization/database-optimization.md](../../docs/technical-implementation/08-performance-optimization/database-optimization.md)

#### 風險描述
大量執行歷史記錄可能導致 PostgreSQL 查詢變慢。

#### 風險評估
- **影響程度**: 🟡 Medium (3)
- **發生機率**: 🟡 Medium (3)
- **風險等級**: 🟡 **Medium (9)** = 3 × 3

#### 緩解策略
```yaml
策略 - 索引優化 (參考: docs/database/database-schema.md):
  已實施:
    - agent_executions 表索引:
      - agent_id (B-tree)
      - created_at (B-tree, DESC)
      - status (B-tree)
      - conversation_id (B-tree)

    - 複合索引:
      - (agent_id, created_at DESC) - 常用查詢組合
      - (conversation_id, created_at DESC) - Chat 歷史查詢

    - 分頁查詢優化:
      - max 100 per page
      - Cursor-based pagination
      - 參考: docs/api/pagination-best-practices.md

  狀態: ✅ 已實施
```

---

### RISK-012: 團隊技術債務累積 🟡
**類別**: 質量風險
**狀態**: 🟡 Monitoring
**識別日期**: 2025-11-22
**負責人**: Tech Lead
**參考文檔**: [docs/standards/technical-debt-management.md](../../docs/standards/technical-debt-management.md)

#### 風險描述
Sprint 1-2 累積 3 個 Technical Debt 項目，如果持續累積可能影響質量。

#### 風險評估
- **影響程度**: 🟡 Medium (3)
- **發生機率**: 🟡 Medium (3)
- **風險等級**: 🟡 **Medium (9)** = 3 × 3

#### 緩解策略
```yaml
策略 - Technical Debt Sprint:
  每 4 個 Sprint 安排 1 個 Debt Repayment Sprint
  專門處理累積的技術債務

  內容:
    - Code refactoring
    - Test coverage improvement
    - Documentation update
    - Performance optimization
  參考: docs/standards/technical-debt-management.md

  首次: Sprint 5-6 之間 (1 週)
  狀態: ⏳ 計劃中
```

---

### RISK-013 到 RISK-020: 其他 Medium 級別風險
**詳細風險清單**:
- RISK-013: 前端性能優化需求 🟡
  - 參考: [docs/technical-implementation/08-performance-optimization/frontend-performance.md](../../docs/technical-implementation/08-performance-optimization/frontend-performance.md)
- RISK-014: API 文檔維護負擔 🟡
  - 參考: [docs/api/api-documentation-guidelines.md](../../docs/api/api-documentation-guidelines.md)
- RISK-015: Sprint 3-6 資源衝突 🟡
  - 參考: [docs/project-management/resource-allocation.md](../../docs/project-management/resource-allocation.md)
- RISK-016: 雲端成本超出預算 🟡
  - 參考: [docs/deployment/cloud-cost-optimization.md](../../docs/deployment/cloud-cost-optimization.md)
- RISK-017: 第三方依賴版本升級 🟡
  - 參考: [docs/standards/dependency-management.md](../../docs/standards/dependency-management.md)
- RISK-018: 團隊知識轉移不足 🟡
  - 參考: [docs/project-management/knowledge-transfer-plan.md](../../docs/project-management/knowledge-transfer-plan.md)
- RISK-019: 工作流模板設計延遲 🟡
  - 參考: [docs/user-stories/module-04-workflow-orchestration/US-7.1-Workflow-Definition-API.md](../../docs/user-stories/module-04-workflow-orchestration/US-7.1-Workflow-Definition-API.md)
- RISK-020: Chat 界面 UX 複雜度 🟡
  - 參考: [docs/ux-design/wireframes/low-fidelity/05-conversation.md](../../docs/ux-design/wireframes/low-fidelity/05-conversation.md)

_(詳細內容省略，可在後續版本補充)_

---

## 🟢 Low 級別風險 (7 個)

### RISK-021 到 RISK-027: Low 級別風險
**詳細風險清單**:
- RISK-021: CI/CD Pipeline 偶發故障 🟢
  - 參考: [docs/devops/ci-cd-pipeline-configuration.md](../../docs/devops/ci-cd-pipeline-configuration.md)
- RISK-022: Docker Compose 環境配置 🟢
  - 參考: [docs/deployment/docker-deployment-guide.md](../../docs/deployment/docker-deployment-guide.md)
- RISK-023: 測試數據管理 🟢
  - 參考: [docs/testing/test-data-management.md](../../docs/testing/test-data-management.md)
- RISK-024: 文檔同步延遲 🟢
  - 參考: [docs/standards/documentation-standards.md](../../docs/standards/documentation-standards.md)
- RISK-025: Code Review 瓶頸 🟢
  - 參考: [docs/standards/code-review-checklist.md](../../docs/standards/code-review-checklist.md)
- RISK-026: Sprint Retrospective 行動項執行 🟢
  - 參考: [docs/project-management/retrospective-template.md](../../docs/project-management/retrospective-template.md)
- RISK-027: 團隊溝通工具選擇 🟢
  - 參考: [docs/project-management/communication-guidelines.md](../../docs/project-management/communication-guidelines.md)

_(詳細內容省略，可在後續版本補充)_

---

## 📈 風險趨勢分析

### 按 Sprint 統計

| Sprint | 新增風險 | 緩解風險 | 關閉風險 | 活躍風險 |
|--------|---------|---------|---------|---------|
| Sprint 0 | 8 | 0 | 0 | 8 |
| Sprint 1 | 5 | 2 | 1 | 10 |
| Sprint 2 | 7 | 3 | 1 | 13 |
| **當前** | **20** | **5** | **2** | **13** |

### 風險熱點 (Top 5)

| 排名 | 風險 ID | 風險名稱 | 風險等級 | 狀態 |
|-----|---------|---------|---------|------|
| 1 | RISK-001 | 工作流引擎複雜度超出預期 | 🔥 Critical (20) | 🔴 Active |
| 2 | RISK-002 | 範圍蔓延持續發生 | 🔥 Critical (20) | 🔴 Active |
| 3 | RISK-005 | 前端開發資源不足 | 🔴 High (16) | 🔴 Active |
| 4 | RISK-003 | Semantic Kernel 框架限制 | 🔴 High (12) | 🟡 Monitoring |
| 5 | RISK-004 | 工作流編輯器前端複雜度 | 🔴 High (12) | 🟡 Monitoring |

---

## 🛡️ 風險應對總結

### 已實施的緩解措施 (5 個)
1. ✅ Framework Abstraction Layer (RISK-003)
   - [docs/architecture/adr/ADR-011-framework-abstraction-layer.md](../../docs/architecture/adr/ADR-011-framework-abstraction-layer.md)
2. ✅ PostgreSQL 索引優化 (RISK-011)
   - [docs/database/database-schema.md](../../docs/database/database-schema.md)
3. ✅ 測試門檻 CI/CD Pipeline (RISK-009)
   - [docs/devops/ci-cd-pipeline-configuration.md](../../docs/devops/ci-cd-pipeline-configuration.md)
4. ✅ 正式變更控制流程 (RISK-002)
   - [docs/standards/change-management-process.md](../../docs/standards/change-management-process.md)
5. ✅ Sprint 3 計劃調整 (RISK-006)
   - [docs/project-management/sprint-planning-template.md](../../docs/project-management/sprint-planning-template.md)

### 計劃中的緩解措施 (8 個)
1. ⏳ 工作流引擎分階段實施 (RISK-001)
   - [DEVELOPMENT-STRATEGY.md](./DEVELOPMENT-STRATEGY.md)
2. ⏳ Sprint 6 技術 Spike (RISK-001)
   - [docs/standards/spike-guidelines.md](../../docs/standards/spike-guidelines.md)
3. ⏳ 引入工作流專家 (RISK-001)
4. ⏳ 加強 Sprint Planning (RISK-002)
   - [docs/project-management/sprint-planning-template.md](../../docs/project-management/sprint-planning-template.md)
5. ⏳ 引入 Spike 機制 (RISK-002)
6. ⏳ VueFlow 技術培訓 (RISK-004)
   - [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md)
7. ⏳ 招聘第二名前端 (RISK-005)
8. ⏳ Vector DB 技術評估 (RISK-010)
   - [docs/technical-implementation/03-data-layer/vector-database-evaluation.md](../../docs/technical-implementation/03-data-layer/vector-database-evaluation.md)

### 備用方案 (3 個)
1. ⚠️ 降低其他模組優先級 (RISK-001)
2. ⚠️ 簡化工作流編輯器 (RISK-004)
3. ⚠️ Technical Debt Sprint (RISK-012)
   - [docs/standards/technical-debt-management.md](../../docs/standards/technical-debt-management.md)

---

## 📊 風險監控儀表板

### 本週 (2025-12-09 ~ 2025-12-15) 需關注風險

| 風險 ID | 風險名稱 | 本週行動 | 負責人 | 截止日期 | 參考文檔 |
|---------|---------|---------|-------|---------|---------|
| RISK-001 | 工作流引擎複雜度 | Sprint 6 技術 Spike 規劃 | Tech Lead | 2025-12-13 | [DEVELOPMENT-STRATEGY.md](./DEVELOPMENT-STRATEGY.md) |
| RISK-002 | 範圍蔓延 | Sprint 3 Planning 改進 | Scrum Master | 2025-12-16 | [docs/project-management/sprint-planning-template.md](../../docs/project-management/sprint-planning-template.md) |
| RISK-005 | 前端資源不足 | 第二名前端招聘進度 | PM | 2025-12-15 | [docs/brief/Project-Brief.md](../../docs/brief/Project-Brief.md) |
| RISK-008 | US 2.2/2.3 工作低估 | 完成 US 2.2/2.3 Phase 3-5 | Backend Lead | 2025-12-15 | [docs/user-stories/module-02-plugin-system/](../../docs/user-stories/module-02-plugin-system/) |

---

## 🔄 更新歷史

| 版本 | 日期 | 更新內容 | 更新人 |
|-----|------|---------|-------|
| 1.0.0 | 2025-12-11 | 初始版本，記錄 27 個風險 | AI Assistant |
| 2.0 | 2025-12-12 | 添加 45+ `/docs` 參考文獻，增強風險追蹤與現有文檔的集成 | AI Assistant |

---

## 📖 參考文檔

### 核心專案文檔
- [docs/brief/Project-Brief.md](../../docs/brief/Project-Brief.md) - 專案概述與範圍
- [docs/architecture/Architecture-Design-Document.md](../../docs/architecture/Architecture-Design-Document.md) - 整體系統架構

### 架構決策記錄 (ADR)
- [docs/architecture/adr/ADR-001-semantic-kernel-vs-agent-framework.md](../../docs/architecture/adr/ADR-001-semantic-kernel-vs-agent-framework.md) - SK vs MAF 選型
- [docs/architecture/adr/ADR-003-workflow-execution-engine.md](../../docs/architecture/adr/ADR-003-workflow-execution-engine.md) - 工作流引擎設計
- [docs/architecture/adr/ADR-006-hybrid-state-management.md](../../docs/architecture/adr/ADR-006-hybrid-state-management.md) - 混合狀態管理
- [docs/architecture/adr/ADR-007-phased-communication-architecture.md](../../docs/architecture/adr/ADR-007-phased-communication-architecture.md) - 階段式通訊架構
- [docs/architecture/adr/ADR-008-code-interpreter-container-pool.md](../../docs/architecture/adr/ADR-008-code-interpreter-container-pool.md) - Code Interpreter 容器池
- [docs/architecture/adr/ADR-011-framework-abstraction-layer.md](../../docs/architecture/adr/ADR-011-framework-abstraction-layer.md) - Framework Abstraction Layer

### User Stories & Modules
- [docs/user-stories/module-01-agent-management/US-1.4-Agent-Execution-Engine.md](../../docs/user-stories/module-01-agent-management/US-1.4-Agent-Execution-Engine.md) - Agent 執行引擎
- [docs/user-stories/module-02-plugin-system/US-2.1-Plugin-System-Core.md](../../docs/user-stories/module-02-plugin-system/US-2.1-Plugin-System-Core.md) - Plugin 系統核心
- [docs/user-stories/module-02-plugin-system/US-2.2-Plugin-Version-Management.md](../../docs/user-stories/module-02-plugin-system/US-2.2-Plugin-Version-Management.md) - Plugin 版本管理
- [docs/user-stories/module-02-plugin-system/US-2.3-Plugin-Hot-Reload.md](../../docs/user-stories/module-02-plugin-system/US-2.3-Plugin-Hot-Reload.md) - Plugin 熱重載
- [docs/user-stories/module-03-knowledge-management/US-4.2-RAG-Implementation.md](../../docs/user-stories/module-03-knowledge-management/US-4.2-RAG-Implementation.md) - RAG 實作
- [docs/user-stories/module-04-workflow-orchestration/README.md](../../docs/user-stories/module-04-workflow-orchestration/README.md) - Workflow 編排模組
- [docs/user-stories/module-04-workflow-orchestration/US-7.2-Workflow-Execution-Engine.md](../../docs/user-stories/module-04-workflow-orchestration/US-7.2-Workflow-Execution-Engine.md) - Workflow 執行引擎
- [docs/user-stories/module-04-workflow-orchestration/US-7.3-Task-Generation-with-LLM.md](../../docs/user-stories/module-04-workflow-orchestration/US-7.3-Task-Generation-with-LLM.md) - LLM 任務生成
- [docs/user-stories/module-04-workflow-orchestration/US-7.4-Multi-Agent-Coordination.md](../../docs/user-stories/module-04-workflow-orchestration/US-7.4-Multi-Agent-Coordination.md) - Multi-Agent 協調
- [docs/user-stories/module-04-workflow-orchestration/US-7.6-Feedback-Loop.md](../../docs/user-stories/module-04-workflow-orchestration/US-7.6-Feedback-Loop.md) - Feedback Loop
- [docs/user-stories/module-05-code-interpreter/README.md](../../docs/user-stories/module-05-code-interpreter/README.md) - Code Interpreter 模組
- [docs/user-stories/module-06-text-to-sql/README.md](../../docs/user-stories/module-06-text-to-sql/README.md) - Text-to-SQL 模組

### Technical Implementation
- [docs/technical-implementation/01-backend-net9/05-semantic-kernel-integration.md](../../docs/technical-implementation/01-backend-net9/05-semantic-kernel-integration.md) - Semantic Kernel 整合
- [docs/technical-implementation/01-backend-net9/08-code-interpreter-implementation.md](../../docs/technical-implementation/01-backend-net9/08-code-interpreter-implementation.md) - Code Interpreter 實作
- [docs/technical-implementation/01-backend-net9/12-workflow-orchestration-implementation.md](../../docs/technical-implementation/01-backend-net9/12-workflow-orchestration-implementation.md) - Workflow Orchestration 實作
- [docs/technical-implementation/02-frontend-react/README.md](../../docs/technical-implementation/02-frontend-react/README.md) - 前端實作指南
- [docs/technical-implementation/03-data-layer/vector-database-evaluation.md](../../docs/technical-implementation/03-data-layer/vector-database-evaluation.md) - Vector DB 評估
- [docs/technical-implementation/08-performance-optimization/01-performance-requirements.md](../../docs/technical-implementation/08-performance-optimization/01-performance-requirements.md) - 性能需求
- [docs/technical-implementation/08-performance-optimization/database-optimization.md](../../docs/technical-implementation/08-performance-optimization/database-optimization.md) - 資料庫優化
- [docs/technical-implementation/08-performance-optimization/frontend-performance.md](../../docs/technical-implementation/08-performance-optimization/frontend-performance.md) - 前端性能優化

### UX Design
- [docs/ux-design/README.md](../../docs/ux-design/README.md) - UX 設計總覽
- [docs/ux-design/wireframes/low-fidelity/05-conversation.md](../../docs/ux-design/wireframes/low-fidelity/05-conversation.md) - Chat 界面設計
- [docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md](../../docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md) - Workflow Editor V2 完整設計

### Security, Testing & Deployment
- [docs/security/code-execution-security.md](../../docs/security/code-execution-security.md) - Code 執行安全
- [docs/security/security-testing-checklist.md](../../docs/security/security-testing-checklist.md) - 安全測試清單
- [docs/testing/testing-strategy.md](../../docs/testing/testing-strategy.md) - 測試策略
- [docs/testing/unit-testing-guidelines.md](../../docs/testing/unit-testing-guidelines.md) - 單元測試指南
- [docs/testing/test-data-management.md](../../docs/testing/test-data-management.md) - 測試數據管理
- [docs/deployment/docker-deployment-guide.md](../../docs/deployment/docker-deployment-guide.md) - Docker 部署指南
- [docs/deployment/cloud-cost-optimization.md](../../docs/deployment/cloud-cost-optimization.md) - 雲端成本優化
- [docs/devops/ci-cd-pipeline-configuration.md](../../docs/devops/ci-cd-pipeline-configuration.md) - CI/CD Pipeline 配置

### API & Database
- [docs/api/workflow-api-specification.md](../../docs/api/workflow-api-specification.md) - Workflow API 規格
- [docs/api/api-documentation-guidelines.md](../../docs/api/api-documentation-guidelines.md) - API 文檔指南
- [docs/api/pagination-best-practices.md](../../docs/api/pagination-best-practices.md) - 分頁最佳實踐
- [docs/database/database-schema.md](../../docs/database/database-schema.md) - 資料庫 Schema

### Standards & Processes
- [docs/standards/coding-standards.md](../../docs/standards/coding-standards.md) - 編碼標準
- [docs/standards/frontend-coding-standards.md](../../docs/standards/frontend-coding-standards.md) - 前端編碼標準
- [docs/standards/change-management-process.md](../../docs/standards/change-management-process.md) - 變更管理流程
- [docs/standards/technical-debt-management.md](../../docs/standards/technical-debt-management.md) - 技術債務管理
- [docs/standards/spike-guidelines.md](../../docs/standards/spike-guidelines.md) - Spike 指南
- [docs/standards/dependency-management.md](../../docs/standards/dependency-management.md) - 依賴管理
- [docs/standards/code-review-checklist.md](../../docs/standards/code-review-checklist.md) - Code Review 清單
- [docs/standards/documentation-standards.md](../../docs/standards/documentation-standards.md) - 文檔標準
- [docs/standards/development-workflow.md](../../docs/standards/development-workflow.md) - 開發流程

### Project Management
- [docs/project-management/sprint-planning-template.md](../../docs/project-management/sprint-planning-template.md) - Sprint 計劃模板
- [docs/project-management/sprint-metrics-tracking.md](../../docs/project-management/sprint-metrics-tracking.md) - Sprint 指標追蹤
- [docs/project-management/resource-allocation.md](../../docs/project-management/resource-allocation.md) - 資源分配
- [docs/project-management/knowledge-transfer-plan.md](../../docs/project-management/knowledge-transfer-plan.md) - 知識轉移計劃
- [docs/project-management/retrospective-template.md](../../docs/project-management/retrospective-template.md) - Retrospective 模板
- [docs/project-management/communication-guidelines.md](../../docs/project-management/communication-guidelines.md) - 溝通指南
- [docs/project-management/team-health-guidelines.md](../../docs/project-management/team-health-guidelines.md) - 團隊健康指南

### 內部 Claudedocs
- [CHANGE-LOG.md](../4-changes/CHANGE-LOG.md) - 變更記錄
- [TECHNICAL-DECISIONS-LOG.md](./TECHNICAL-DECISIONS-LOG.md) - 技術決策日誌
- [DEVELOPMENT-STRATEGY.md](./DEVELOPMENT-STRATEGY.md) - 開發策略
- [DEPENDENCY-MATRIX.md](./DEPENDENCY-MATRIX.md) - 依賴矩陣
- [SPRINT-ALLOCATION-ANALYSIS.md](./SPRINT-ALLOCATION-ANALYSIS.md) - Sprint 分配分析

---

**維護說明**:
- ✅ 每週更新風險狀態 (Sprint Review)
- ✅ 每 Sprint 新增/關閉風險 (Sprint Retrospective)
- ✅ 每月風險趨勢分析 (Monthly Review)
- ✅ 同步更新所有相關 `/docs` 參考文獻
- ✅ 風險緩解措施實施後更新狀態
- ✅ 新風險識別後立即記錄
