# Epic 導航索引

**Semantic Kernel Agentic Framework - Epic Navigation**

本目錄包含從大型模組切分出的 Epic 級別文檔,方便 IDE 開發時精確引用。

---

## 📚 Epic 索引結構

### 為什麼需要 Epic Sharding?

**問題**: 某些模組文件過大 (>600 lines),不利於:
- IDE 精確引用
- 開發時快速定位
- 獨立功能追蹤

**解決方案**: 將大型模組按 Epic 切分為獨立文件,每個 Epic 包含:
- Epic 描述和目標
- 完整的 User Stories
- 技術要求和驗收標準

---

## 📦 Epic 清單

### Phase 1 (MVP) - 8 個月

#### P0 - Core Agent Capabilities

**Module 01: Agent Management** (未切分 - 375 lines)
- [完整模組](../modules/module-01-agent-creation.md) - 6 Stories
  - US 1.1: Agent CRUD
  - US 1.2: Agent Configuration
  - US 1.3: LLM Provider Integration
  - US 1.4: Agent Execution
  - US 1.5: Agent Versioning
  - US 1.6: Agent Testing

**Module 02: Plugin System** (未切分 - 303 lines)
- [完整模組](../modules/module-02-plugin-system.md) - 4 Stories
  - US 2.1: Plugin Interface
  - US 2.2: Plugin Security
  - US 2.3: Plugin Marketplace

**Module 05: Agent Memory** (已切分 - 原 657 lines)
- [Epic 5.1: Conversation Memory](./epic-05.1-conversation-memory.md) - 2 Stories
  - US 5.1: 知識庫文件上傳與處理
  - US 5.3: 知識庫管理與更新

- [Epic 5.2: Knowledge RAG](./epic-05.2-knowledge-rag.md) - 4 Stories
  - US 5.2: 精確檢索策略配置 ⭐ 核心競爭力
  - US 5.4: Excel/CSV 數據源上傳
  - US 5.5: 數據庫連接 (Text-to-SQL)
  - US 5.6: 智能數據可視化
  - US 5.7: 跨數據源關聯查詢 (Phase 2)

**Module 06: Chat Interface** (未切分 - 344 lines)
- [完整模組](../modules/module-06-chat-interface.md) - 4 Stories
  - US 6.1: 基礎對話介面
  - US 6.2: 流式回應
  - US 6.3: 多模態支援
  - US 6.4: 對話歷史管理

---

#### P1 - Differentiated Capabilities

**Module 03: Code Interpreter** (未切分 - 372 lines)
- [完整模組](../modules/module-03-code-interpreter.md) - 5 Stories
  - US 3.1: Python Sandbox
  - US 3.2: 4-Layer Security
  - US 3.3: Result Visualization
  - US 3.4: R Language Support (Phase 2)
  - US 3.5: Package Management (Phase 2)

**Module 07: Persona Framework** (未切分 - 318 lines)
- [完整模組](../modules/module-07-persona-framework.md) - 4 Stories
  - US 7.1: Persona Configuration
  - US 7.2: Prompt Engineering
  - US 7.3: Style Consistency Validation
  - US 7.4: Persona Evolution (Phase 2)

**Module 08: Structured Data / Text-to-SQL** (未切分 - 401 lines)
- [完整模組](../modules/module-08-structured-data.md) - 4 Stories
  - US 8.1: Schema Understanding
  - US 8.2: Text-to-SQL Generation
  - US 8.3: Row-Level Security
  - US 8.4: Result Presentation

---

#### P2 - Advanced Features

**Module 04: Multi-Agent** (未切分 - 311 lines)
- [完整模組](../modules/module-04-multi-agent.md) - 4 Stories
  - US 4.1: Agent Communication
  - US 4.2: Task Orchestration
  - US 4.3: Advanced Routing (Phase 2)

---

#### P3 - Enterprise & Operations

**Module 09: Enterprise Features** (已切分 - 原 721 lines)
- [Epic 9.1: Authentication & RBAC](./epic-09.1-authentication.md) - 1 Story
  - US 9.1: RBAC 角色權限管理

- [Epic 9.2: Multi-Tenant](./epic-09.2-multi-tenant.md) - 1 Story
  - US 9.2: Multi-Tenant 架構

- [Epic 9.3: i18n & API Metering](./epic-09.3-i18n-api-metering.md) - 2 Stories
  - US 9.3: i18n 國際化支援
  - US 9.4: API Metering 與限流

**Module 10: Monitoring** (已切分 - 原 998 lines)
- [Epic 10.1: Monitoring Dashboard](./epic-10.1-monitoring-dashboard.md) - 1 Story
  - US 10.1: 即時監控儀表板

- [Epic 10.2: Quality Monitoring](./epic-10.2-quality-monitoring.md) - 1 Story
  - US 10.2: Agent 品質監控與告警

- [Epic 10.3: User Analytics](./epic-10.3-user-analytics.md) - 1 Story (Phase 2)
  - US 10.3: 用戶行為分析

---

## 📊 統計摘要

### 切分成果

| 模組 | 原始行數 | 切分狀態 | Epic 數量 | Stories 數量 |
|------|----------|----------|-----------|--------------|
| Module 05 | 657 | ✅ 已切分 | 2 | 6 |
| Module 09 | 721 | ✅ 已切分 | 3 | 4 |
| Module 10 | 998 | ✅ 已切分 | 3 | 3 |
| **總計** | **2,376** | **3 模組** | **8 Epics** | **13 Stories** |

### 優先級分佈

- **P0 (MVP 必須)**: 6 Epics
- **P1 (MVP 高優先)**: 1 Epic
- **P2 (Phase 2)**: 1 Epic

---

## 🎯 使用指南

### 開發者使用場景

**場景 1: 實作 Knowledge 檢索功能**
```
需求: 實作 90%+ 準確率的知識檢索
引用文檔: docs/user-stories/epics/epic-05.2-knowledge-rag.md
包含內容:
- 完整的檢索策略 (Vector, Keyword, Hybrid, Re-ranking)
- 準確率測試方法和標準
- 技術實現細節
```

**場景 2: 實作 Multi-Tenant 隔離**
```
需求: 實作多租戶資料隔離
引用文檔: docs/user-stories/epics/epic-09.2-multi-tenant.md
包含內容:
- 完整的隔離機制設計
- Global Query Filter 實現
- 配額管理邏輯
```

**場景 3: 實作監控儀表板**
```
需求: 實作系統監控 Dashboard
引用文檔: docs/user-stories/epics/epic-10.1-monitoring-dashboard.md
包含內容:
- 監控指標定義
- 儀表板設計範例
- 告警規則配置
```

---

## 🔗 相關文檔

- [User Stories 總覽](../README.md)
- [模組列表](../modules/)
- [MVP 規劃](../mvp-planning.md)
- [實施策略](../implementation-strategy.md)
- [Sprint 分配](../sprints/sprint-allocation.md)

---

## 📝 維護說明

### 何時需要新增 Epic?

當模組文件 >600 lines 時,考慮切分為 Epic:
1. 按功能邏輯分組
2. 每個 Epic 獨立完整
3. 保持原模組文件作為總覽

### Epic 文件命名規範

```
epic-{module-number}.{epic-number}-{epic-name}.md

範例:
- epic-05.1-conversation-memory.md
- epic-05.2-knowledge-rag.md
- epic-09.1-authentication.md
```

---

**版本**: 1.0.0
**最後更新**: 2025-11-02
**維護者**: Product Owner
**狀態**: ✅ Epic Sharding 完成
