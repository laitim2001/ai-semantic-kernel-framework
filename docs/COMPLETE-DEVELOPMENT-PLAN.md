# AI Workflow Platform - 完整開發計劃文檔

**專案名稱**: AI Workflow Platform (Semantic Kernel Agentic Framework)
**文檔版本**: v1.0.0
**創建日期**: 2025-11-03
**狀態**: Stage 2 - Development Preparation → Stage 3 - Iterative Development
**預計開發週期**: 5-6 個月 (包含 Gap Closure)

---

## 📋 文檔目的

本文檔提供完整、詳細、有系統和順序的開發規劃，並明確說明每個開發階段需要參考和查閱的文件。

**適用對象**:
- 🎯 Product Owner (PO) - 全局掌控開發進度
- 👔 Scrum Master (SM) - Sprint 規劃與執行
- 💻 開發團隊 (Frontend, Backend, DevOps)
- 🧪 QA 工程師 - 測試規劃與執行
- 🏗️ 架構師 - 技術決策與指導

---

## 📊 目錄

1. [專案當前狀態總覽](#1-專案當前狀態總覽)
2. [關鍵文檔索引](#2-關鍵文檔索引)
3. [Week 0: 前置準備與關鍵決策](#week-0-前置準備與關鍵決策-1-2-週)
4. [Week 1-3: Gap Closure 文檔補充](#week-1-3-gap-closure-文檔補充-3-週)
5. [Week 4: Sprint 0 - 環境與基礎設施](#week-4-sprint-0---環境與基礎設施)
6. [Month 2-4: MVP 開發 (Sprint 1-8)](#month-2-4-mvp-開發-sprint-1-8)
7. [Sprint 規劃詳細內容](#sprint-規劃詳細內容)
8. [開發工作流程指南](#開發工作流程指南)
9. [風險管理](#風險管理)
10. [成功指標與里程碑](#成功指標與里程碑)
11. [附錄: 快速參考表](#附錄-快速參考表)

---

## 1. 專案當前狀態總覽

### 1.1 整體完成度: **75-80%**

根據 PO Validation 結果 (詳見 `claudedocs/PO-VALIDATION-REPORT-FINAL.md`):

| 類別 | 完成度 | 分數 | 狀態 |
|------|--------|------|------|
| Brief | 95% | 9.0/10 | ✅ 優秀 (需切分) |
| User Stories | 95% | 9.5/10 | ✅ 優秀 |
| UX Design | 95% | 9.5/10 | ✅ 優秀 |
| Architecture | 95% | 9.5/10 | ✅ 優秀 |
| PoC Validation | 100% | 10/10 | ✅ 完美 |
| **Technical Implementation** | **20%** | **2.0/10** | ⚠️ **需補充** |
| 跨文檔一致性 | 70% | 7.0/10 | ⚠️ 需改善 |
| 開發就緒度 | 70% | 7.0/10 | ⚠️ 需改善 |

**總分**: **7.5/10** - **PASS with Conditions** ✅

### 1.2 關鍵成就 ✅

1. **完整的規劃文檔**
   - ✅ Brief (206KB, 需切分但內容完整)
   - ✅ 8 個模組的 User Stories (26 個文件, ~135KB)
   - ✅ 12 個核心頁面的 UX 設計 (含 Workflow Editor V2 完整設計)
   - ✅ 完整的系統架構文檔 (含 7 個 ADRs)

2. **技術可行性驗證**
   - ✅ 6 個 PoC 全部完成 (平均代碼質量 **97.7%**)
   - ✅ PoC 3 實測完成 (90/100 安全分數)
   - ✅ 所有核心技術風險已驗證

3. **明確的技術決策**
   - ✅ **Monorepo** 架構 (pnpm + Turborepo)
   - ✅ **React 18 + TypeScript** (主應用)
   - ✅ **Vue 3 + VueFlow** (Workflow Editor, Phase 2)
   - ✅ **ASP.NET Core 8** (後端服務)

4. **清晰的開發路線圖**
   - ✅ Epic 切分完成 (8 個 Epic 文件)
   - ✅ MVP 範圍明確定義
   - ✅ Sprint 優先級排序

### 1.3 待解決問題 (12 個 Issues)

根據 PO Validation 識別的問題，按優先級排序：

#### 🔴 Critical (2 個) - 必須在 Week 0 解決

| Issue | 問題 | 影響 | 解決時間 |
|-------|------|------|---------|
| **ISSUE-008** | **Workflow Editor 範圍衝突**<br>Brief: Phase 2 vs UX/ADR/PoC: 已完成 | 阻塞開發計劃 | Week 0 (決策) |
| **ISSUE-001** | **Brief.md 過大** (206KB)<br>需切分為多個章節 | 影響文檔引用效率 | 1 day |

#### 🟡 Important (8 個) - Week 1-3 補充

| Issue | 問題 | 解決時間 |
|-------|------|---------|
| ISSUE-002 | C# Coding Standards 文檔缺失 | 2-3 days |
| ISSUE-003 | TypeScript/React Coding Standards 文檔缺失 | 2-3 days |
| ISSUE-004 | API Design Guidelines 不完整 | 2-3 days |
| ISSUE-005 | Dev Environment Setup 文檔不完整 | 1 day |
| ISSUE-006 | Database Naming Conventions 未定義 | 0.5 day |
| ISSUE-007 | CI/CD Pipeline 文檔缺失 | 1 day |
| ISSUE-011 | Code Interpreter 安全審計待執行 | 10 days (外包) |
| ISSUE-012 | Development Workflow 定義不完整 | 1 day |

#### 🟢 Minor (2 個) - 可延後

| Issue | 問題 | 備註 |
|-------|------|------|
| ISSUE-009 | Figma 高保真原型未完成 | 可選，低保真已足夠 |
| ISSUE-010 | PoC 5 Azure AI Search 未實測 | Phase 1 使用簡化版 |

**Gap Closure 總時間**: **16-23 天** (3-4.5 週)

### 1.4 開發時間軸總覽

```
Timeline Overview:
│
├─ Week 0 (1-2 週) ⏳ 當前階段
│  └─ 關鍵決策 + 資源準備
│     • ISSUE-008 決策 (Workflow Editor 範圍)
│     • Azure 雲端資源申請
│     • 安全審計服務商洽談
│
├─ Week 1-3 (3 週) ⏳ 平行執行
│  ├─ Track A: Gap Closure (補充文檔)
│  │  • ISSUE-001: Brief 切分
│  │  • ISSUE-002-007: 技術標準文檔
│  │  • ISSUE-012: 開發流程定義
│  │
│  └─ Track B: Sprint 0 準備
│     • Monorepo 架構設置
│     • 開發環境配置
│     • CI/CD Pipeline 建立
│
├─ Week 4 (Sprint 0, 2 週)
│  └─ 環境驗證 + 團隊 Onboarding
│
├─ Month 2-4 (Sprint 1-8, 16 週)
│  └─ MVP 開發 (8 個 Sprints × 2 週)
│     • Sprint 1-2: 核心 Agent 能力
│     • Sprint 3-4: 差異化能力 (Persona, Code Interpreter)
│     • Sprint 5-6: 進階功能 (Multi-Agent, Knowledge RAG)
│     • Sprint 7-8: 企業功能 + 監控
│
└─ Month 5 (Alpha Testing, 3-4 週)
   └─ 內部測試 + Bug 修復
```

**預計正式開發開始**: Week 4 (Sprint 0)
**預計 MVP 完成**: Month 5 End
**總開發週期**: **5-6 個月**

---

## 2. 關鍵文檔索引

### 2.1 文檔分類總覽

本專案擁有 **8 大類** 文檔，涵蓋從商業規劃到技術實施的完整鏈條。

**詳細使用策略**: 請參閱 `docs/DOCUMENT-USAGE-STRATEGY.md` (15,000+ 行完整指南)

#### 📁 1. Brief - 商業規劃
**路徑**: `docs/brief.md`
**用途**: 商業目標、市場定位、產品願景
**使用者**: PO, PM, Sales, Marketing
**開發時參考**: 產品決策、目標對齊、外部溝通

#### 📁 2. User Stories - 需求文檔
**路徑**: `docs/user-stories/`
**核心文件**:
- `README.md` - 總覽
- `implementation-strategy.md` - 實施策略
- `mvp-planning.md` - MVP 範圍定義
- `modules/` - 10 個模組 (module-01.md ~ module-10.md)
- `epics/` - 8 個 Epic 文件 (詳細切分)

**開發時參考** ⭐ **核心**:
- Sprint Planning: 從 Epic 選擇 Stories
- 開發實施: 閱讀驗收標準
- 測試: 根據驗收標準創建測試案例
- Code Review: 驗證 PR 是否符合驗收標準

#### 📁 3. Architecture - 架構文檔
**路徑**: `docs/architecture/`
**核心文件**:
- `Architecture-Design-Document.md` - 主架構文檔
- `ADR-006.md` ~ `ADR-012.md` - 7 個架構決策記錄
- `database-schema.md` - 數據庫設計
- `performance-scalability-strategy.md` - 性能策略
- `components/` - 組件文檔 (待補充)

**開發時參考** ⭐ **核心**:
- 系統設計: 理解組件架構
- API 開發: 查看 API 規格
- 技術決策: 參考 ADRs 的理由
- 數據庫操作: 查詢 Schema 定義

#### 📁 4. Project Management - 專案管理
**路徑**: `docs/project-management/`
**用途**: Sprint 規劃、Code Review Checklist、Release Planning
**使用者**: PO, SM, 開發團隊

#### 📁 5. UX Design - UX 設計
**路徑**: `docs/ux-design/`
**核心文件**:
- `user-research/` - 用戶研究 (3 個 Persona, 5 個 Journey Maps)
- `information-architecture/` - 資訊架構
- `wireframes/low-fidelity/` - 12 個低保真線框圖
- `design-system/` - 設計系統 (完整定義)

**開發時參考** ⭐ **核心**:
- 前端開發: 根據 Wireframes 實作
- UI Review: 對比實作與設計
- Accessibility Testing: 遵循 WCAG 指南

#### 📁 6. Technical Implementation - 技術實施
**路徑**: `docs/technical-implementation/`
**12 個階段** (部分待補充):
1. PoC Validation ✅
2. Dev Environment Setup ⚠️ (待補充)
3. Project Structure ✅
4. Coding Standards ⚠️ (待補充)
5. API Design ⚠️ (待補充)
6. Database Standards ⚠️ (待補充)
7. Testing Strategy ✅
8. Deployment Architecture ✅
9. Security Standards ⚠️ (待補充)
10. Monitoring & Operations ✅
11. Disaster Recovery ✅
12. Cost Estimation ✅

**開發時參考** ⭐ **核心**:
- 環境設置: Dev Environment 文檔
- 編碼: Coding Standards
- API 開發: API Design Guidelines
- 數據庫: Naming Conventions
- 測試: Testing Strategy
- 部署: Deployment Architecture

#### 📁 7. PoC Projects - PoC 實作代碼
**路徑**: `poc-projects/`
**6 個 PoCs**:
- `poc1-sk-agents/` - Semantic Kernel Agents (97.1%)
- `poc2-persona-builder/` - Persona Builder (97.9%)
- `poc3-code-interpreter/` - Code Interpreter (97.9%, 實測 90/100)
- `poc4-text-to-sql/` - Text-to-SQL (97.3%)
- `poc5-knowledge-rag/` - Knowledge RAG (97.8%)
- `poc6-vueflow-crdt/` - VueFlow CRDT (98.2%)

**開發時參考** ⭐ **核心**:
- 技術參考: 複製/調整已驗證的代碼
- Troubleshooting: 對比 PoC vs 生產代碼
- Code Review: 確保與 PoC 模式一致

#### 📁 8. PoC Validation - PoC 驗證報告
**路徑**: `poc-validation/`
**核心文件**: `POC-VALIDATION-REPORT.md`
**用途**: 技術決策、風險評估、工作估算

### 2.2 新增關鍵文檔 (Week 0 創建)

| 文檔 | 路徑 | 用途 |
|------|------|------|
| **BMad 階段分析** | `docs/BMAD-STAGE-ANALYSIS.md` | BMad Method 階段對照與下一步規劃 |
| **PO 驗證報告** | `claudedocs/PO-VALIDATION-REPORT-FINAL.md` | 完整的 PO Master Checklist 驗證結果 |
| **文檔切分報告** | `docs/DOCUMENT-SHARDING-REPORT.md` | Epic 切分執行結果 |
| **專案初始化決策** | `docs/PROJECT-INITIALIZATION-DECISION.md` | Monorepo 架構、技術棧、專案結構決策 |
| **文檔使用策略** | `docs/DOCUMENT-USAGE-STRATEGY.md` | 8 大類文檔的使用指南 (15,000+ 行) |
| **開發計劃** | `docs/COMPLETE-DEVELOPMENT-PLAN.md` | 本文檔 |

---

## Week 0: 前置準備與關鍵決策 (1-2 週)

**目標**: 解決阻塞問題，準備開發資源

### Week 0.1 關鍵決策

#### 🔴 Decision Point 1: ISSUE-008 - Workflow Editor 範圍決策

**問題描述**:
- **Brief.md** 明確指出: "❌ 視覺化工作流程設計器 (Drag & Drop) ⏰ 推遲到: Phase 2"
- **但實際上**:
  - ✅ UX Design 已完成 (7 部分, ~20,000 行, n8n 風格設計)
  - ✅ ADR-012 已批准 (2025-11-01, VueFlow + Yjs 技術選型)
  - ✅ PoC 6 已驗證 (98.2% 質量, 60 FPS, <200ms 延遲)

**決策選項**:

**選項 A: 保持 Phase 2 (推薦 ✅)**
```yaml
決策: Workflow Editor 保持在 Phase 2
理由:
  1. ✅ 商業策略一致性: Brief 是經過市場分析的商業決策
  2. ✅ 風險控制: MVP 保持 8 個月可完成
  3. ✅ 資源優化: 集中在 4 大核心差異化能力
     - Persona Framework
     - Code Interpreter
     - Text-to-SQL
     - Knowledge RAG (90%+ 準確率)
  4. ✅ Phase 2 準備充分: UX/ADR/PoC 都已完成，Phase 2 可以快速啟動

行動:
  - 更新 docs/user-stories/mvp-planning.md (移除 Epic 4)
  - 更新 Sprint 規劃 (移除 Multi-Agent Orchestration)
  - MVP 時間: 8 個月 (不變)
  - Phase 2 時間: 3-4 個月 (加入 Workflow Editor)

影響:
  - MVP 範圍: 8 → 7 Epics
  - MVP 時間: 8 個月 (不變)
  - Phase 2: 增加 Workflow Editor (3-4 個月)
```

**選項 B: 納入 Phase 1**
```yaml
決策: Workflow Editor 納入 Phase 1 MVP
理由:
  - UX/ADR/PoC 都已完成
  - 市場競爭力提升
  - 差異化能力增強

風險:
  - ⚠️ MVP 時間延長: 8 → 10-11 個月 (+2-3 個月)
  - ⚠️ 資源壓力: 需要 Vue 3 + VueFlow 專家
  - ⚠️ 複雜度增加: React + Vue 混合架構
  - ⚠️ 測試範圍擴大: WebSocket + CRDT 測試

影響:
  - MVP 範圍: 8 Epics (保持)
  - MVP 時間: 10-11 個月 (+25%)
  - 需要額外資源: 1 位 Vue 3 專家
```

**選項 C: 最小化版本納入 Phase 1**
```yaml
決策: 僅納入簡化版 Workflow Editor
範圍:
  - ✅ 基礎節點拖拽 (5 種核心節點)
  - ✅ 連接線配置
  - ❌ 實時協作 (延後)
  - ❌ 版本控制 (延後)
  - ❌ 進階節點類型 (延後)

影響:
  - MVP 時間: 8 → 9 個月 (+1 個月)
  - Phase 2: 完整版 Workflow Editor (2 個月)
```

**推薦決策**: **選項 A - 保持 Phase 2** ✅

**決策時間**: **Week 0 (2025-11-03 前完成)**
**決策者**: Product Owner + 關鍵利害關係人
**文檔更新**: PO 決策後立即更新 `docs/user-stories/mvp-planning.md`

---

#### 🔴 Decision Point 2: 雲端資源準備

**需申請資源**:

1. **Azure OpenAI Service** (已有 API Key，需確認額度)
   - GPT-4o (主要模型)
   - GPT-4o-mini (快速任務)
   - text-embedding-ada-002 (嵌入模型)
   - **月度預算**: $500-1000 (開發階段)

2. **Azure AI Search** (Phase 1 可選，Phase 2 必須)
   - Knowledge RAG 向量存儲
   - **月度費用**: ~$250 (Basic Tier)

3. **Azure Container Registry** (生產部署時需要)
   - Docker 映像存儲
   - **月度費用**: ~$5 (Basic)

**行動時間**: Week 0 (開發前完成)
**負責人**: DevOps / IT 管理員

---

#### 🔴 Decision Point 3: 安全審計服務商

**背景**: ISSUE-011 - Code Interpreter 安全審計

**PoC 3 當前狀態**:
- ✅ Docker 隔離
- ✅ 資源限制 (CPU 1 core, Memory 256MB, 30 秒超時)
- ✅ 網絡隔離 (無外網訪問)
- ⚠️ Seccomp + AppArmor 未啟用 (安全分數 90/100)

**需要審計**:
1. Docker 安全配置審查
2. Seccomp profile 設計與測試
3. AppArmor/SELinux 策略設計
4. 滲透測試 (沙盒逃逸測試)
5. 安全加固建議

**預算**: $3,000 - $5,000
**時間**: 10 工作天
**行動時間**: Week 0 (聯繫廠商，Week 1 開始審計)

**推薦廠商**:
- Option A: 本地資安公司 (熟悉在地法規)
- Option B: 國際資安顧問 (Docker 安全專家)

---

### Week 0.2 文檔準備

#### Task 0.1: ISSUE-001 - Brief 切分 (1 day)

**目標**: 將 206KB 的 `brief.md` 切分為多個章節

**切分結構**:
```
docs/brief/
├── README.md                    # 總覽 (2KB)
├── 01-executive-summary.md      # 執行摘要 (10KB)
├── 02-problem-statement.md      # 問題陳述 (20KB)
├── 03-solution-overview.md      # 解決方案總覽 (30KB)
├── 04-market-analysis.md        # 市場分析 (40KB)
├── 05-technical-approach.md     # 技術方法 (30KB)
├── 06-business-model.md         # 商業模式 (20KB)
├── 07-roadmap.md                # 產品路線圖 (15KB)
├── 08-success-metrics.md        # 成功指標 (10KB)
├── 09-risks-mitigation.md       # 風險與緩解 (15KB)
└── 10-appendix.md               # 附錄 (16KB)
```

**執行方式**: 使用 Task agent 進行文檔切分
**驗證**: 確保所有章節總和 = 原 brief.md 內容

---

## Week 1-3: Gap Closure 文檔補充 (3 週)

**目標**: 補充 Technical Implementation 缺失文檔，達到 100% 開發就緒

### Week 1: 核心開發標準 (5 days)

#### Task 1.1: ISSUE-002 - C# Coding Standards (2-3 days)

**目標**: 撰寫完整的 C# 代碼規範文檔

**產出**: `docs/technical-implementation/4-coding-standards/csharp-coding-standards.md`

**內容大綱**:
```markdown
# C# Coding Standards

## 1. 命名約定
- PascalCase: Classes, Methods, Properties, Events
- camelCase: Local variables, method parameters
- _camelCase: Private fields
- UPPER_CASE: Constants

## 2. 文件組織
- Namespace structure
- Using statements order
- Class member order

## 3. 代碼風格
- Indentation: 4 spaces
- Line length: 120 characters
- Brace style: Allman (next line)

## 4. 語言特性
- var usage guidelines
- async/await best practices
- LINQ usage
- Nullable reference types

## 5. Semantic Kernel 特定規範
- Plugin naming conventions
- Function attributes usage
- KernelFunction best practices
- Memory management

## 6. 範例代碼
[完整的良好/不良範例]
```

**參考資料**:
- Microsoft C# Coding Conventions
- PoC 1 實作代碼 (`poc-projects/poc1-sk-agents/`)
- Semantic Kernel 官方文檔

**負責人**: Senior Backend Developer
**審查者**: Architect

---

#### Task 1.2: ISSUE-003 - TypeScript/React Coding Standards (2-3 days)

**目標**: 撰寫完整的 TypeScript + React 代碼規範文檔

**產出**: `docs/technical-implementation/4-coding-standards/typescript-react-coding-standards.md`

**內容大綱**:
```markdown
# TypeScript & React Coding Standards

## 1. TypeScript 配置
- tsconfig.json 標準設置
- Strict mode 啟用
- Path aliases 配置

## 2. 命名約定
- PascalCase: Components, Types, Interfaces, Enums
- camelCase: Functions, variables, hooks
- UPPER_SNAKE_CASE: Constants

## 3. React 組件規範
- Functional components only
- Hooks usage guidelines
- Component composition patterns
- Props interface definition

## 4. 狀態管理 (Zustand)
- Store structure
- Selector patterns
- Async actions

## 5. 樣式規範 (Tailwind CSS)
- Class organization
- Custom utilities
- Responsive design patterns

## 6. API 整合 (TanStack Query)
- Query key structure
- Mutation patterns
- Error handling

## 7. 表單處理 (React Hook Form + Zod)
- Schema definition
- Validation patterns
- Error display

## 8. 範例代碼
[完整的良好/不良範例]
```

**參考資料**:
- TypeScript Official Handbook
- React Official Docs
- shadcn/ui Component Library
- PoC 6 實作代碼 (`poc-projects/poc6-vueflow-crdt/`)

**負責人**: Senior Frontend Developer
**審查者**: Architect

---

#### Task 1.3: ISSUE-012 - Development Workflow Definition (1 day)

**目標**: 定義完整的開發流程和協作規範

**產出**: `docs/technical-implementation/development-workflow.md`

**內容大綱**:
```markdown
# Development Workflow

## 1. Git 工作流程
- Branch strategy: GitFlow
  - main: 生產環境
  - develop: 開發主線
  - feature/*: 功能分支
  - bugfix/*: Bug 修復分支
  - release/*: 發布分支
- Commit message 規範 (Conventional Commits)
- Pull Request 流程

## 2. Code Review Checklist
### Backend (C#)
- [ ] 符合 C# Coding Standards
- [ ] 單元測試覆蓋率 ≥80%
- [ ] API 文檔更新 (OpenAPI)
- [ ] 錯誤處理完整
- [ ] 日誌記錄適當
- [ ] 性能考量 (N+1 query, caching)

### Frontend (TypeScript/React)
- [ ] 符合 TypeScript/React Coding Standards
- [ ] 組件測試覆蓋
- [ ] Accessibility 檢查 (WCAG 2.1 AA)
- [ ] 響應式設計 (Desktop/Tablet/Mobile)
- [ ] 性能優化 (lazy loading, memoization)
- [ ] TypeScript strict mode 無錯誤

## 3. Definition of Done (DoD)
- [ ] 代碼完成並通過 Code Review
- [ ] 單元測試撰寫並通過
- [ ] 整合測試通過 (如適用)
- [ ] 文檔更新 (API, README, 技術文檔)
- [ ] 驗收標準全部滿足 (User Story)
- [ ] UI 與 Wireframe 一致 (Frontend)
- [ ] 無 linting errors/warnings
- [ ] 部署到 dev 環境並驗證

## 4. Sprint 流程
### Sprint Planning (Week 1, Day 1)
1. SM 從 Epic 選擇 User Stories
2. 團隊估算工作量 (Story Points)
3. 分配任務給開發者
4. 定義 Sprint Goal

### Daily Standup (每天 15 分鐘)
- 昨天完成了什麼
- 今天計劃做什麼
- 有什麼阻礙

### Sprint Review (Week 2, Day 5 下午)
- Demo 完成的 User Stories
- 收集 Stakeholder 回饋

### Sprint Retrospective (Week 2, Day 5 下午)
- What went well?
- What could be improved?
- Action items for next sprint

## 5. 環境管理
- **dev**: 開發環境 (feature 分支合併後自動部署)
- **staging**: 預生產環境 (develop 分支)
- **production**: 生產環境 (main 分支)

## 6. CI/CD Pipeline
[詳見 ISSUE-007 CI/CD 文檔]
```

**參考資料**:
- GitFlow Workflow
- Conventional Commits
- docs/project-management/

**負責人**: Scrum Master
**審查者**: PO, Architect

---

### Week 2: API 與數據庫標準 (5 days)

#### Task 2.1: ISSUE-004 - API Design Guidelines (2-3 days)

**目標**: 補充完整的 API 設計規範和 OpenAPI 模板

**產出**: `docs/technical-implementation/5-api-design/api-design-guidelines.md`

**內容大綱**:
```markdown
# API Design Guidelines

## 1. RESTful API 設計原則
- Resource naming conventions
- HTTP methods usage (GET, POST, PUT, PATCH, DELETE)
- HTTP status codes standard
- URL structure best practices

## 2. Request/Response 格式
### Request
```json
{
  "data": { ... },
  "metadata": {
    "requestId": "uuid",
    "timestamp": "ISO8601"
  }
}
```

### Response (Success)
```json
{
  "data": { ... },
  "metadata": {
    "requestId": "uuid",
    "timestamp": "ISO8601"
  }
}
```

### Response (Error)
```json
{
  "error": {
    "code": "ERROR_CODE",
    "message": "Human-readable message",
    "details": [...]
  },
  "metadata": {
    "requestId": "uuid",
    "timestamp": "ISO8601"
  }
}
```

## 3. 錯誤處理標準
- Error code naming convention
- HTTP status code mapping
- Error response structure
- Validation error format

## 4. 認證與授權
- JWT Token structure
- Authorization header format
- Token refresh mechanism
- Permission checking

## 5. 分頁與排序
- Pagination parameters (page, pageSize, limit, offset)
- Sorting parameters (sortBy, sortOrder)
- Response metadata (total, page, pageSize)

## 6. 過濾與搜索
- Filter query parameters
- Search endpoint design
- Advanced query syntax

## 7. 版本控制
- API versioning strategy (URL path: /api/v1/)
- Deprecation policy
- Backward compatibility

## 8. 性能優化
- Rate limiting (per user/API key)
- Response compression (gzip)
- ETags for caching
- Partial response (field filtering)

## 9. OpenAPI 3.0 規範
- Specification template
- Documentation generation
- Client SDK generation

## 10. 範例 API 文檔
[完整的 CRUD API 範例]
```

**OpenAPI 模板**: `docs/technical-implementation/5-api-design/openapi-template.yaml`
```yaml
openapi: 3.0.0
info:
  title: AI Workflow Platform API
  version: 1.0.0
  description: RESTful API for AI Workflow Platform

servers:
  - url: https://api.example.com/v1
    description: Production
  - url: https://dev-api.example.com/v1
    description: Development

paths:
  /agents:
    get:
      summary: List all agents
      parameters:
        - name: page
          in: query
          schema:
            type: integer
            default: 1
        - name: pageSize
          in: query
          schema:
            type: integer
            default: 20
      responses:
        '200':
          description: Successful response
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/AgentListResponse'
    post:
      summary: Create a new agent
      requestBody:
        required: true
        content:
          application/json:
            schema:
              $ref: '#/components/schemas/AgentCreateRequest'
      responses:
        '201':
          description: Agent created
        '400':
          description: Invalid request
        '429':
          description: Rate limit exceeded

components:
  schemas:
    AgentListResponse:
      type: object
      properties:
        data:
          type: array
          items:
            $ref: '#/components/schemas/Agent'
        metadata:
          $ref: '#/components/schemas/PaginationMetadata'

  securitySchemes:
    bearerAuth:
      type: http
      scheme: bearer
      bearerFormat: JWT

security:
  - bearerAuth: []
```

**參考資料**:
- Microsoft REST API Guidelines
- OpenAPI Specification 3.0
- docs/architecture/Architecture-Design-Document.md (API 章節)

**負責人**: Backend Architect
**審查者**: Architect, Senior Backend Developer

---

#### Task 2.2: ISSUE-006 - Database Naming Conventions (0.5 day)

**目標**: 定義統一的數據庫命名規範

**產出**: `docs/technical-implementation/6-database-standards/database-naming-conventions.md`

**內容大綱**:
```markdown
# Database Naming Conventions

## 1. 表名 (Table Names)
- 命名風格: **snake_case** (PostgreSQL convention)
- 單數 vs 複數: **複數** (users, agents, workflows)
- 前綴: 無前綴 (避免 tbl_users)

範例:
```
✅ Good: users, agents, workflows, agent_executions
❌ Bad: User, tblUsers, AgentExecution
```

## 2. 列名 (Column Names)
- 命名風格: **snake_case**
- 主鍵: **id** (bigserial, UUID)
- 外鍵: **{table_singular}_id** (例如: agent_id, user_id)
- 布林值: **is_*, has_*, can_*** (例如: is_active, has_permission)
- 時間戳: **created_at, updated_at, deleted_at**

範例:
```
✅ Good: user_id, is_active, created_at, system_prompt
❌ Bad: userId, active, createdAt, systemPrompt
```

## 3. 索引 (Indexes)
- 命名格式: **idx_{table}_{columns}**
- 唯一索引: **uk_{table}_{columns}**
- 外鍵索引: **fk_{table}_{ref_table}**

範例:
```
CREATE INDEX idx_agents_user_id ON agents(user_id);
CREATE UNIQUE INDEX uk_users_email ON users(email);
CREATE INDEX idx_executions_status_created_at ON agent_executions(status, created_at);
```

## 4. 約束 (Constraints)
- 主鍵: **pk_{table}**
- 外鍵: **fk_{table}_{ref_table}**
- 檢查約束: **chk_{table}_{column}**
- 唯一約束: **uk_{table}_{columns}**

範例:
```
ALTER TABLE agents ADD CONSTRAINT pk_agents PRIMARY KEY (id);
ALTER TABLE agents ADD CONSTRAINT fk_agents_users FOREIGN KEY (user_id) REFERENCES users(id);
ALTER TABLE agents ADD CONSTRAINT chk_agents_status CHECK (status IN ('active', 'inactive', 'archived'));
```

## 5. 數據類型選擇
- ID: **bigserial** (auto-increment) 或 **UUID** (分布式)
- 字符串: **varchar(n)** (有長度限制) 或 **text** (無限制)
- 數字: **integer, bigint, numeric(precision, scale)**
- 布林: **boolean**
- 時間: **timestamp with time zone** (建議使用 UTC)
- JSON: **jsonb** (優於 json，支持索引)

## 6. Schema 組織
- 預設 schema: **public**
- 多租戶: 每個租戶一個 schema (tenant_001, tenant_002) 或 tenant_id 列
- 歷史數據: **history.{table}** (時態表)
- 審計: **audit.{table}_audit**

## 7. 視圖 (Views)
- 命名格式: **vw_{purpose}**
- 物化視圖: **mv_{purpose}**

範例:
```
CREATE VIEW vw_active_agents AS ...;
CREATE MATERIALIZED VIEW mv_agent_statistics AS ...;
```

## 8. 函數和存儲過程 (Functions/Procedures)
- 命名格式: **fn_{purpose}** 或 **sp_{purpose}**
- 使用動詞開頭

範例:
```
CREATE FUNCTION fn_calculate_token_usage(...) RETURNS ...;
CREATE PROCEDURE sp_archive_old_executions(...) ...;
```

## 9. 避免使用的名稱
- SQL 保留字 (user, order, table, ...)
- 過於通用的名稱 (data, info, temp)
- 縮寫 (除非業界標準: url, api, llm)

## 10. 範例 Schema
[完整的 agents 表範例]
```

**參考資料**:
- PostgreSQL Naming Convention Best Practices
- docs/architecture/database-schema.md

**負責人**: Backend Developer
**審查者**: Architect

---

### Week 3: 環境與部署 (5 days)

#### Task 3.1: ISSUE-005 - Dev Environment Setup Guide (1 day)

**目標**: 補充完整的開發環境設置文檔 (Windows + macOS)

**產出**: `docs/technical-implementation/2-dev-environment/complete-setup-guide.md`

**內容大綱**:
```markdown
# Complete Development Environment Setup Guide

## 1. 系統需求
### Windows 11
- RAM: ≥16GB (建議 32GB)
- Disk: ≥50GB 可用空間 (SSD)
- WSL 2 啟用

### macOS (Ventura 或更新)
- RAM: ≥16GB (建議 32GB)
- Disk: ≥50GB 可用空間

## 2. 必裝軟體清單

### 2.1 版本控制
- **Git** (≥2.40)
  - Windows: Git for Windows
  - macOS: `brew install git`
- 配置:
  ```bash
  git config --global user.name "Your Name"
  git config --global user.email "your.email@example.com"
  git config --global core.autocrlf input  # Unix line endings
  ```

### 2.2 IDE
- **Visual Studio Code** (最新版)
- **必裝擴展**:
  ```
  code --install-extension dbaeumer.vscode-eslint
  code --install-extension esbenp.prettier-vscode
  code --install-extension bradlc.vscode-tailwindcss
  code --install-extension Vue.volar
  code --install-extension ms-dotnettools.csharp
  code --install-extension GitHub.copilot
  code --install-extension ms-azuretools.vscode-docker
  code --install-extension redhat.vscode-yaml
  ```
- **推薦擴展**:
  ```
  code --install-extension eamodio.gitlens
  code --install-extension usernamehw.errorlens
  code --install-extension streetsidesoftware.code-spell-checker
  ```

### 2.3 Frontend 開發環境
- **Node.js** (≥20 LTS)
  - Windows: 使用 nvm-windows
    ```powershell
    # Install nvm-windows from GitHub releases
    nvm install 20
    nvm use 20
    ```
  - macOS: 使用 nvm
    ```bash
    brew install nvm
    nvm install 20
    nvm use 20
    ```
- **pnpm** (≥8.0)
  ```bash
  npm install -g pnpm@latest
  ```
- 驗證安裝:
  ```bash
  node --version   # v20.x.x
  pnpm --version   # 8.x.x
  ```

### 2.4 Backend 開發環境
- **.NET 8 SDK**
  - Windows: Download from Microsoft
  - macOS: `brew install dotnet@8`
- 驗證安裝:
  ```bash
  dotnet --version  # 8.x.x
  ```

### 2.5 容器化環境
- **Docker Desktop** (≥4.20)
  - Windows: WSL 2 backend
  - macOS: Apple Silicon 或 Intel 版本
- 配置:
  - RAM: 8GB
  - CPUs: 4 cores
  - Disk: 100GB
- 驗證安裝:
  ```bash
  docker --version         # 24.x.x
  docker compose version   # 2.x.x
  ```

### 2.6 數據庫工具
- **PostgreSQL Client**
  - Windows: pgAdmin 4 或 DBeaver
  - macOS: Postico 或 DBeaver
- **Redis Client**
  - RedisInsight (跨平台)

## 3. 專案 Clone 與設置

### 3.1 Clone 倉庫
```bash
git clone https://github.com/your-org/ai-workflow-platform.git
cd ai-workflow-platform
```

### 3.2 安裝依賴
```bash
# 安裝所有 workspace 依賴
pnpm install

# 驗證 Turborepo
pnpm turbo --version
```

### 3.3 環境變數設置
```bash
# 複製環境變數模板
cp .env.example .env

# 編輯 .env (填入 Azure OpenAI API Key 等)
```

### 3.4 啟動開發環境
```bash
# 啟動所有基礎設施 (PostgreSQL, Redis, RabbitMQ)
docker compose -f infra/docker-compose/dev.yml up -d

# 等待服務啟動 (約 30 秒)
docker compose -f infra/docker-compose/dev.yml ps

# 執行數據庫遷移
cd apps/api-gateway
dotnet ef database update

# 啟動所有應用
pnpm dev
```

### 3.5 驗證安裝
訪問:
- Frontend: http://localhost:3000
- API Gateway: http://localhost:5000/swagger
- PostgreSQL: localhost:5432 (user: postgres, password: dev_password)
- Redis: localhost:6379
- RabbitMQ Management: http://localhost:15672 (user: guest, password: guest)

## 4. 常見問題排查

### 4.1 pnpm 安裝失敗
```bash
# 清除緩存
pnpm store prune

# 重新安裝
rm -rf node_modules pnpm-lock.yaml
pnpm install
```

### 4.2 Docker 無法啟動
- Windows: 確認 WSL 2 啟用
- macOS: 確認 Docker Desktop 有足夠資源分配

### 4.3 .NET 建置失敗
```bash
# 清理並重新建置
dotnet clean
dotnet restore
dotnet build
```

### 4.4 端口衝突
```bash
# 查看端口占用 (Windows)
netstat -ano | findstr :3000

# 查看端口占用 (macOS/Linux)
lsof -i :3000

# 修改 .env 中的端口設置
```

## 5. 開發工具推薦

### 5.1 API 測試
- **Postman** 或 **Insomnia**
- **REST Client** (VSCode 擴展)

### 5.2 Git GUI
- **GitKraken** (跨平台)
- **Sourcetree** (Windows/macOS)
- **GitHub Desktop** (簡單易用)

### 5.3 終端工具
- **Windows Terminal** (Windows)
- **iTerm2** (macOS)
- **Oh My Zsh** (Shell enhancement)

## 6. 團隊協作工具
- **Slack** - 團隊溝通
- **Jira** - 任務追蹤
- **Confluence** - 文檔協作
- **Figma** - 設計協作

## 7. 性能優化建議
- 啟用 pnpm store: `pnpm config set store-dir ~/.pnpm-store`
- 配置 Git 全局 ignore: `git config --global core.excludesfile ~/.gitignore_global`
- WSL 2 性能優化 (Windows):
  ```
  # .wslconfig
  [wsl2]
  memory=8GB
  processors=4
  swap=4GB
  ```

## 8. 下一步
- 閱讀 `docs/DOCUMENT-USAGE-STRATEGY.md` (了解文檔使用方式)
- 閱讀 `docs/technical-implementation/4-coding-standards/` (編碼規範)
- 加入 Slack #dev-general 頻道
- 向 Scrum Master 取得 Sprint 任務
```

**參考資料**:
- docs/PROJECT-INITIALIZATION-DECISION.md
- 現有的 technical-implementation/2-dev-environment/ 文件

**負責人**: DevOps Engineer
**審查者**: Senior Developers (Frontend + Backend)

---

#### Task 3.2: ISSUE-007 - CI/CD Pipeline Documentation (1 day)

**目標**: 撰寫完整的 CI/CD Pipeline 配置文檔

**產出**: `docs/technical-implementation/8-deployment-architecture/cicd-pipeline.md`

**內容大綱**:
```markdown
# CI/CD Pipeline Documentation

## 1. Pipeline 總覽

```mermaid
graph LR
    A[Push to Branch] --> B{Branch Type?}
    B -->|feature/*| C[Feature Pipeline]
    B -->|develop| D[Develop Pipeline]
    B -->|main| E[Production Pipeline]

    C --> C1[Lint]
    C1 --> C2[Unit Tests]
    C2 --> C3[Build]

    D --> D1[Lint]
    D1 --> D2[Unit + Integration Tests]
    D2 --> D3[Build]
    D3 --> D4[Deploy to Dev]
    D4 --> D5[E2E Tests]

    E --> E1[Lint]
    E1 --> E2[All Tests]
    E2 --> E3[Security Scan]
    E3 --> E4[Build Docker Images]
    E4 --> E5[Deploy to Staging]
    E5 --> E6[Smoke Tests]
    E6 --> E7[Manual Approval]
    E7 --> E8[Deploy to Production]
```

## 2. GitHub Actions 工作流程

### 2.1 Feature Branch Pipeline
**文件**: `.github/workflows/feature.yml`
**觸發**: Push to `feature/*`, `bugfix/*`
**目的**: 快速回饋代碼質量

```yaml
name: Feature Branch CI

on:
  push:
    branches:
      - 'feature/**'
      - 'bugfix/**'
  pull_request:
    branches:
      - develop

jobs:
  lint:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: pnpm/action-setup@v2
        with:
          version: 8
      - uses: actions/setup-node@v4
        with:
          node-version: '20'
          cache: 'pnpm'
      - run: pnpm install
      - run: pnpm turbo run lint

  test:
    runs-on: ubuntu-latest
    needs: lint
    steps:
      - uses: actions/checkout@v4
      - uses: pnpm/action-setup@v2
      - run: pnpm install
      - run: pnpm turbo run test --coverage
      - uses: codecov/codecov-action@v3
        with:
          files: ./coverage/coverage-final.json

  build:
    runs-on: ubuntu-latest
    needs: test
    steps:
      - uses: actions/checkout@v4
      - uses: pnpm/action-setup@v2
      - run: pnpm install
      - run: pnpm turbo run build
```

### 2.2 Develop Branch Pipeline
**文件**: `.github/workflows/develop.yml`
**觸發**: Push to `develop`
**目的**: 自動部署到 Dev 環境並執行 E2E 測試

```yaml
name: Develop CI/CD

on:
  push:
    branches:
      - develop

jobs:
  test-and-build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: pnpm/action-setup@v2
      - uses: actions/setup-node@v4
      - uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'

      # Frontend
      - run: pnpm install
      - run: pnpm turbo run lint test build

      # Backend
      - run: dotnet restore
      - run: dotnet test --configuration Release
      - run: dotnet build --configuration Release

  deploy-to-dev:
    needs: test-and-build
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      # Build Docker images
      - uses: docker/setup-buildx-action@v3
      - uses: docker/login-action@v3
        with:
          registry: ${{ secrets.ACR_REGISTRY }}
          username: ${{ secrets.ACR_USERNAME }}
          password: ${{ secrets.ACR_PASSWORD }}

      - name: Build and push images
        run: |
          docker compose -f infra/docker-compose/production.yml build
          docker compose -f infra/docker-compose/production.yml push

      # Deploy to Dev environment (Azure Container Apps or Kubernetes)
      - uses: azure/login@v1
        with:
          creds: ${{ secrets.AZURE_CREDENTIALS }}

      - run: |
          kubectl config use-context dev-cluster
          kubectl apply -f infra/kubernetes/dev/
          kubectl rollout status deployment/api-gateway -n dev

  e2e-tests:
    needs: deploy-to-dev
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: pnpm/action-setup@v2
      - run: pnpm install
      - run: pnpm --filter e2e-tests run test
        env:
          BASE_URL: https://dev-api.example.com
```

### 2.3 Production Pipeline
**文件**: `.github/workflows/production.yml`
**觸發**: Push to `main`
**目的**: 完整測試 + 安全掃描 + Staging 部署 + 手動批准 + 生產部署

```yaml
name: Production CI/CD

on:
  push:
    branches:
      - main

jobs:
  full-test-suite:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: pnpm/action-setup@v2
      - uses: actions/setup-dotnet@v3

      # Run all tests
      - run: pnpm install
      - run: pnpm turbo run lint test:unit test:integration
      - run: dotnet test --configuration Release --collect:"XPlat Code Coverage"

      # Code coverage report
      - uses: codecov/codecov-action@v3

  security-scan:
    runs-on: ubuntu-latest
    needs: full-test-suite
    steps:
      - uses: actions/checkout@v4

      # Dependency vulnerability scan
      - uses: snyk/actions/node@master
        env:
          SNYK_TOKEN: ${{ secrets.SNYK_TOKEN }}

      # Container image scan
      - uses: aquasecurity/trivy-action@master
        with:
          image-ref: '${{ secrets.ACR_REGISTRY }}/api-gateway:${{ github.sha }}'
          format: 'sarif'
          output: 'trivy-results.sarif'

      - uses: github/codeql-action/upload-sarif@v2
        with:
          sarif_file: 'trivy-results.sarif'

  build-and-push:
    runs-on: ubuntu-latest
    needs: security-scan
    steps:
      - uses: actions/checkout@v4
      - uses: docker/setup-buildx-action@v3
      - uses: docker/login-action@v3
        with:
          registry: ${{ secrets.ACR_REGISTRY }}
          username: ${{ secrets.ACR_USERNAME }}
          password: ${{ secrets.ACR_PASSWORD }}

      - name: Build and push with tags
        run: |
          docker compose -f infra/docker-compose/production.yml build

          # Tag with commit SHA and latest
          docker tag api-gateway:latest ${{ secrets.ACR_REGISTRY }}/api-gateway:${{ github.sha }}
          docker tag api-gateway:latest ${{ secrets.ACR_REGISTRY }}/api-gateway:latest

          docker push ${{ secrets.ACR_REGISTRY }}/api-gateway:${{ github.sha }}
          docker push ${{ secrets.ACR_REGISTRY }}/api-gateway:latest

  deploy-to-staging:
    runs-on: ubuntu-latest
    needs: build-and-push
    environment:
      name: staging
      url: https://staging.example.com
    steps:
      - uses: azure/login@v1
        with:
          creds: ${{ secrets.AZURE_CREDENTIALS }}

      - uses: azure/k8s-set-context@v3
        with:
          cluster-name: production-cluster
          namespace: staging

      - run: |
          kubectl set image deployment/api-gateway \
            api-gateway=${{ secrets.ACR_REGISTRY }}/api-gateway:${{ github.sha }} \
            -n staging
          kubectl rollout status deployment/api-gateway -n staging

  smoke-tests:
    runs-on: ubuntu-latest
    needs: deploy-to-staging
    steps:
      - uses: actions/checkout@v4
      - run: |
          # Health check
          curl -f https://staging-api.example.com/health

          # Basic API tests
          pnpm --filter smoke-tests run test
        env:
          BASE_URL: https://staging-api.example.com

  deploy-to-production:
    runs-on: ubuntu-latest
    needs: smoke-tests
    environment:
      name: production
      url: https://app.example.com
    steps:
      - uses: trstringer/manual-approval@v1
        with:
          secret: ${{ github.TOKEN }}
          approvers: user1,user2
          minimum-approvals: 2
          issue-title: "Deploy to Production"

      - uses: azure/login@v1
        with:
          creds: ${{ secrets.AZURE_CREDENTIALS }}

      - uses: azure/k8s-set-context@v3
        with:
          cluster-name: production-cluster
          namespace: production

      - name: Blue-Green Deployment
        run: |
          # Deploy to green environment
          kubectl set image deployment/api-gateway-green \
            api-gateway=${{ secrets.ACR_REGISTRY }}/api-gateway:${{ github.sha }} \
            -n production
          kubectl rollout status deployment/api-gateway-green -n production

          # Health check
          kubectl exec -it deploy/api-gateway-green -n production -- curl http://localhost/health

          # Switch traffic to green
          kubectl patch service api-gateway -n production -p '{"spec":{"selector":{"version":"green"}}}'

          # Wait 5 minutes for monitoring
          sleep 300

          # If no issues, scale down blue
          kubectl scale deployment api-gateway-blue --replicas=0 -n production
```

## 3. 環境配置

### 3.1 Dev Environment
- **URL**: https://dev-api.example.com
- **Database**: dev-postgres (Azure Database for PostgreSQL)
- **Redis**: dev-redis (Azure Cache for Redis)
- **Azure OpenAI**: Shared instance (低配額)
- **Auto-deploy**: Yes (每次 push to develop)

### 3.2 Staging Environment
- **URL**: https://staging-api.example.com
- **Database**: staging-postgres (生產同配置)
- **Redis**: staging-redis (生產同配置)
- **Azure OpenAI**: Dedicated instance (中配額)
- **Auto-deploy**: Yes (每次 push to main)
- **Data**: Production data snapshot (weekly refresh)

### 3.3 Production Environment
- **URL**: https://api.example.com
- **Database**: production-postgres (高可用, 自動備份)
- **Redis**: production-redis (Cluster mode)
- **Azure OpenAI**: Dedicated instance (高配額)
- **Auto-deploy**: No (需要手動批准)
- **Blue-Green Deployment**: Yes

## 4. 監控與告警

### 4.1 Pipeline 監控
- GitHub Actions dashboard
- Slack 通知 (#ci-cd channel)
  - Build failures
  - Deployment status
  - Security scan results

### 4.2 Deployment 監控
- Application Insights (Azure)
- Prometheus + Grafana
- Alert rules:
  - Deployment failure
  - High error rate (>5%)
  - High latency (P95 >5s)

## 5. 回滾策略

### 5.1 自動回滾
- Health check 失敗 → 自動回滾到前一版本
- Error rate >10% for 5 minutes → 自動回滾

### 5.2 手動回滾
```bash
# List recent deployments
kubectl rollout history deployment/api-gateway -n production

# Rollback to previous version
kubectl rollout undo deployment/api-gateway -n production

# Rollback to specific revision
kubectl rollout undo deployment/api-gateway --to-revision=3 -n production
```

## 6. 性能優化

### 6.1 Build Cache
- pnpm store cache (shared across jobs)
- Docker layer cache (BuildKit)
- Turborepo remote cache (Vercel)

### 6.2 Parallel Jobs
- Frontend and Backend tests run in parallel
- Multi-architecture Docker builds (amd64, arm64)

## 7. 安全最佳實踐
- **Secrets Management**: GitHub Secrets / Azure Key Vault
- **Least Privilege**: Service accounts with minimal permissions
- **Audit Logs**: All deployments logged to Azure Monitor
- **Image Signing**: Cosign for Docker image signing
- **SBOM Generation**: Generate Software Bill of Materials

## 8. 故障排查

### 8.1 Build Failures
```bash
# Check GitHub Actions logs
gh run view <run-id>

# Re-run failed job
gh run rerun <run-id>
```

### 8.2 Deployment Failures
```bash
# Check pod status
kubectl get pods -n production

# View pod logs
kubectl logs -f deployment/api-gateway -n production

# Describe pod for events
kubectl describe pod <pod-name> -n production
```

## 9. 開發者指南

### 9.1 本地測試 Pipeline
```bash
# Install act (https://github.com/nektos/act)
brew install act

# Run feature pipeline locally
act -W .github/workflows/feature.yml
```

### 9.2 跳過 CI
```bash
# Skip CI for documentation-only changes
git commit -m "docs: update README [skip ci]"
```

## 10. 未來改進
- [ ] Canary deployments (逐步推出新版本)
- [ ] Feature flags integration (LaunchDarkly)
- [ ] Performance regression testing (Lighthouse CI)
- [ ] Automated rollback based on metrics
```

**參考資料**:
- GitHub Actions Official Docs
- Azure DevOps Best Practices
- docs/technical-implementation/8-deployment-architecture/

**負責人**: DevOps Engineer
**審查者**: Architect

---

#### Task 3.3: ISSUE-011 - 啟動 Code Interpreter 安全審計 (Week 1 聯繫, Week 3 執行)

**目標**: 聘請外部資安顧問進行 Code Interpreter 沙盒安全審計

**執行計劃**:

**Week 1**: 聯繫資安服務商
- 準備審計範圍文檔 (Scope of Work)
- 提供 PoC 3 代碼和架構文檔
- 取得報價和時間表

**Week 2**: 簽約與準備
- 簽署 NDA 和服務合約
- 提供測試環境訪問權限
- 安排 Kick-off Meeting

**Week 3**: 執行審計 (10 工作天)
- Day 1-2: 架構審查和靜態代碼分析
- Day 3-5: 滲透測試 (沙盒逃逸測試)
- Day 6-7: Seccomp profile 設計與測試
- Day 8-9: AppArmor 策略設計與測試
- Day 10: 報告撰寫與建議

**產出**:
- 安全審計報告 (Security Audit Report)
- Seccomp profile 配置文件
- AppArmor policy 文件
- 安全加固實施計劃

**預算**: $3,000 - $5,000
**負責人**: Backend Architect
**審查者**: PO, Architect

---

### Gap Closure 完成檢查清單

**Week 1-3 完成後，確認以下項目**:

- [ ] ISSUE-001: Brief 切分完成 (10 個章節文件)
- [ ] ISSUE-002: C# Coding Standards 完成 (~20 頁)
- [ ] ISSUE-003: TypeScript/React Coding Standards 完成 (~25 頁)
- [ ] ISSUE-004: API Design Guidelines + OpenAPI 模板完成 (~30 頁)
- [ ] ISSUE-005: Dev Environment Setup 完整指南完成 (~20 頁)
- [ ] ISSUE-006: Database Naming Conventions 完成 (~10 頁)
- [ ] ISSUE-007: CI/CD Pipeline 文檔完成 (~30 頁)
- [ ] ISSUE-011: 安全審計執行中 (10 工作天)
- [ ] ISSUE-012: Development Workflow 定義完成 (~15 頁)

**預計文檔增加**: ~160 頁 (約 80,000 行)

**技術實施完整度**: 20% → **95%** ✅

---

## Week 4: Sprint 0 - 環境與基礎設施 (2 週)

**目標**: 設置完整的開發環境和基礎設施，準備開始 Sprint 1

**參考文檔**:
- `docs/PROJECT-INITIALIZATION-DECISION.md` - Monorepo 架構決策
- `docs/technical-implementation/2-dev-environment/` - 環境設置指南
- `docs/technical-implementation/8-deployment-architecture/` - 部署架構

### Sprint 0 時間表

| Day | 活動 | 負責人 | 產出 |
|-----|------|--------|------|
| 1-2 | Monorepo 架構設置 | DevOps + Architect | 完整的 Monorepo 結構 |
| 3-4 | 前端專案初始化 | Frontend Lead | React + Vue 專案設置 |
| 5-6 | 後端專案初始化 | Backend Lead | ASP.NET Core 服務設置 |
| 7-8 | 基礎設施配置 | DevOps | Docker Compose + Kubernetes |
| 9 | CI/CD Pipeline 配置 | DevOps | GitHub Actions 工作流程 |
| 10 | 團隊 Onboarding 與驗證 | SM + 全員 | 所有開發者環境 Ready |

---

### Day 1-2: Monorepo 架構設置

#### Task 0.1: 初始化 Git 倉庫與目錄結構

**執行步驟**:

```bash
# 1. 初始化 Git 倉庫
cd "C:\AI Semantic Kernel"
git init
git branch -M main

# 2. 創建完整目錄結構
mkdir -p apps/{web,workflow-editor,api-gateway,agent-service,code-interpreter,knowledge-service,multi-agent-orchestrator}
mkdir -p packages/{shared-types,ui-components,sdk-dotnet,sdk-typescript}
mkdir -p infra/{docker-compose,kubernetes/{dev,staging,production},terraform}
mkdir -p tools/{scripts,generators}
mkdir -p .github/workflows

# 3. 創建 pnpm workspace 配置
cat > pnpm-workspace.yaml << 'EOF'
packages:
  - 'apps/*'
  - 'packages/*'
  - 'tools/*'
EOF

# 4. 創建根 package.json
cat > package.json << 'EOF'
{
  "name": "ai-workflow-platform",
  "version": "0.1.0",
  "private": true,
  "description": "AI Workflow Platform - Semantic Kernel Agentic Framework",
  "scripts": {
    "dev": "turbo run dev",
    "build": "turbo run build",
    "test": "turbo run test",
    "test:unit": "turbo run test:unit",
    "test:integration": "turbo run test:integration",
    "test:e2e": "turbo run test:e2e",
    "lint": "turbo run lint",
    "lint:fix": "turbo run lint:fix",
    "format": "prettier --write \"**/*.{ts,tsx,md,json}\"",
    "clean": "turbo run clean && rm -rf node_modules .turbo",
    "typecheck": "turbo run typecheck"
  },
  "devDependencies": {
    "turbo": "^1.10.0",
    "typescript": "^5.0.0",
    "prettier": "^3.0.0",
    "@types/node": "^20.0.0"
  },
  "engines": {
    "node": ">=20.0.0",
    "pnpm": ">=8.0.0"
  },
  "packageManager": "pnpm@8.15.0"
}
EOF

# 5. 創建 Turborepo 配置
cat > turbo.json << 'EOF'
{
  "$schema": "https://turbo.build/schema.json",
  "globalDependencies": [".env", "tsconfig.json"],
  "pipeline": {
    "build": {
      "dependsOn": ["^build"],
      "outputs": ["dist/**", ".next/**", "build/**"]
    },
    "dev": {
      "cache": false,
      "persistent": true
    },
    "test": {
      "dependsOn": ["^build"],
      "outputs": ["coverage/**"]
    },
    "test:unit": {
      "outputs": ["coverage/**"]
    },
    "test:integration": {
      "dependsOn": ["^build"],
      "outputs": ["coverage/**"]
    },
    "test:e2e": {
      "dependsOn": ["^build"],
      "outputs": ["coverage/**", "playwright-report/**"]
    },
    "lint": {
      "outputs": []
    },
    "lint:fix": {
      "outputs": []
    },
    "typecheck": {
      "dependsOn": ["^build"],
      "outputs": []
    },
    "clean": {
      "cache": false
    }
  }
}
EOF

# 6. 創建根 TypeScript 配置
cat > tsconfig.json << 'EOF'
{
  "compilerOptions": {
    "target": "ES2020",
    "lib": ["ES2020", "DOM", "DOM.Iterable"],
    "module": "ESNext",
    "moduleResolution": "bundler",
    "jsx": "react-jsx",
    "strict": true,
    "esModuleInterop": true,
    "skipLibCheck": true,
    "forceConsistentCasingInFileNames": true,
    "resolveJsonModule": true,
    "isolatedModules": true,
    "incremental": true,
    "noUnusedLocals": true,
    "noUnusedParameters": true,
    "noFallthroughCasesInSwitch": true,
    "allowSyntheticDefaultImports": true
  }
}
EOF

# 7. 創建 .gitignore
cat > .gitignore << 'EOF'
# Dependencies
node_modules
.pnpm-store

# Build outputs
dist
build
.next
.turbo
*.tsbuildinfo

# Environment variables
.env
.env.local
.env.development.local
.env.test.local
.env.production.local

# Logs
logs
*.log
npm-debug.log*
yarn-debug.log*
yarn-error.log*
pnpm-debug.log*

# OS
.DS_Store
Thumbs.db

# IDE
.vscode/*
!.vscode/settings.json
!.vscode/extensions.json
.idea

# Testing
coverage
.nyc_output
playwright-report

# .NET
bin/
obj/
*.user
*.suo

# Docker
*.dockerignore

# Temporary
tmp/
temp/
EOF

# 8. 創建 .env.example
cat > .env.example << 'EOF'
# Azure OpenAI
AZURE_OPENAI_ENDPOINT=https://your-resource.openai.azure.com/
AZURE_OPENAI_KEY=your-api-key
AZURE_OPENAI_DEPLOYMENT_GPT4O=gpt-4o
AZURE_OPENAI_DEPLOYMENT_GPT4O_MINI=gpt-4o-mini
AZURE_OPENAI_DEPLOYMENT_EMBEDDING=text-embedding-ada-002

# Database
DATABASE_URL=postgresql://postgres:dev_password@localhost:5432/ai_workflow_dev
DATABASE_URL_TEST=postgresql://postgres:test_password@localhost:5433/ai_workflow_test

# Redis
REDIS_URL=redis://localhost:6379

# RabbitMQ
RABBITMQ_URL=amqp://guest:guest@localhost:5672

# JWT
JWT_SECRET=your-secret-key-here-change-in-production
JWT_EXPIRATION=24h

# API
API_PORT=5000
WEB_PORT=3000

# Environment
NODE_ENV=development
ASPNETCORE_ENVIRONMENT=Development
EOF

# 9. 安裝依賴
pnpm install

# 10. 驗證設置
pnpm turbo --version
```

**產出**:
- ✅ 完整的 Monorepo 目錄結構
- ✅ pnpm workspace 配置
- ✅ Turborepo 配置
- ✅ TypeScript 根配置
- ✅ 環境變數模板

**驗證**:
```bash
tree -L 2 -I 'node_modules'
```

---

### Day 3-4: 前端專案初始化

#### Task 0.2: React 主應用設置

**執行步驟**:

```bash
# 1. 創建 React 應用 (使用 Vite)
cd apps
pnpm create vite web --template react-ts

cd web

# 2. 安裝核心依賴
pnpm add react-router-dom @tanstack/react-query zustand
pnpm add react-hook-form zod @hookform/resolvers
pnpm add axios

# 3. 安裝 UI 庫 (Tailwind CSS + shadcn/ui)
pnpm add -D tailwindcss postcss autoprefixer
pnpm dlx tailwindcss init -p

# 安裝 shadcn/ui
pnpm dlx shadcn-ui@latest init

# 4. 安裝開發依賴
pnpm add -D @types/react @types/react-dom
pnpm add -D eslint @typescript-eslint/parser @typescript-eslint/eslint-plugin
pnpm add -D prettier eslint-config-prettier eslint-plugin-prettier
pnpm add -D vitest @testing-library/react @testing-library/jest-dom @testing-library/user-event
pnpm add -D @playwright/test

# 5. 配置 Tailwind CSS
cat > tailwind.config.js << 'EOF'
/** @type {import('tailwindcss').Config} */
export default {
  darkMode: ['class'],
  content: [
    './index.html',
    './src/**/*.{js,ts,jsx,tsx}',
  ],
  theme: {
    extend: {
      colors: {
        // Design System colors (from design-system/color-palette.md)
        primary: {
          50: '#eff6ff',
          100: '#dbeafe',
          500: '#3b82f6',
          600: '#2563eb',
          700: '#1d4ed8',
        },
        // ... (完整的顏色定義)
      },
      fontFamily: {
        sans: ['Inter', 'system-ui', 'sans-serif'],
        mono: ['JetBrains Mono', 'monospace'],
      },
    },
  },
  plugins: [],
}
EOF

# 6. 創建目錄結構
mkdir -p src/{components,pages,hooks,services,stores,types,utils,styles}
mkdir -p src/components/{ui,layout,features}

# 7. 創建基礎組件 (從 Design System)
cat > src/components/layout/AppLayout.tsx << 'EOF'
import { Outlet } from 'react-router-dom';
import { Sidebar } from './Sidebar';
import { Header } from './Header';

export function AppLayout() {
  return (
    <div className="flex h-screen bg-gray-50">
      <Sidebar />
      <div className="flex-1 flex flex-col overflow-hidden">
        <Header />
        <main className="flex-1 overflow-y-auto p-6">
          <Outlet />
        </main>
      </div>
    </div>
  );
}
EOF

# 8. 配置路由
cat > src/routes.tsx << 'EOF'
import { createBrowserRouter } from 'react-router-dom';
import { AppLayout } from './components/layout/AppLayout';
import { Dashboard } from './pages/Dashboard';
import { AgentList } from './pages/agents/AgentList';
import { AgentCreate } from './pages/agents/AgentCreate';
import { AgentDetail } from './pages/agents/AgentDetail';
// ... (其他路由)

export const router = createBrowserRouter([
  {
    path: '/',
    element: <AppLayout />,
    children: [
      { index: true, element: <Dashboard /> },
      { path: 'agents', element: <AgentList /> },
      { path: 'agents/new', element: <AgentCreate /> },
      { path: 'agents/:id', element: <AgentDetail /> },
      // ... (其他路由)
    ],
  },
]);
EOF

# 9. 配置 Zustand Store
cat > src/stores/authStore.ts << 'EOF'
import { create } from 'zustand';
import { persist } from 'zustand/middleware';

interface User {
  id: string;
  email: string;
  name: string;
  role: 'SuperAdmin' | 'TenantAdmin' | 'Developer' | 'Analyst' | 'User';
}

interface AuthState {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
  login: (email: string, password: string) => Promise<void>;
  logout: () => void;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      user: null,
      token: null,
      isAuthenticated: false,
      login: async (email, password) => {
        // API call to login
        const response = await fetch('/api/auth/login', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ email, password }),
        });
        const data = await response.json();
        set({ user: data.user, token: data.token, isAuthenticated: true });
      },
      logout: () => {
        set({ user: null, token: null, isAuthenticated: false });
      },
    }),
    { name: 'auth-storage' }
  )
);
EOF

# 10. 配置 TanStack Query
cat > src/main.tsx << 'EOF'
import React from 'react';
import ReactDOM from 'react-dom/client';
import { RouterProvider } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import { router } from './routes';
import './styles/index.css';

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 1000 * 60 * 5, // 5 minutes
      refetchOnWindowFocus: false,
    },
  },
});

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <QueryClientProvider client={queryClient}>
      <RouterProvider router={router} />
      <ReactQueryDevtools initialIsOpen={false} />
    </QueryClientProvider>
  </React.StrictMode>
);
EOF

# 11. 配置 ESLint
cat > .eslintrc.json << 'EOF'
{
  "extends": [
    "eslint:recommended",
    "plugin:@typescript-eslint/recommended",
    "plugin:react/recommended",
    "plugin:react-hooks/recommended",
    "prettier"
  ],
  "parser": "@typescript-eslint/parser",
  "plugins": ["@typescript-eslint", "react", "react-hooks"],
  "rules": {
    "react/react-in-jsx-scope": "off",
    "@typescript-eslint/no-unused-vars": ["error", { "argsIgnorePattern": "^_" }]
  },
  "settings": {
    "react": {
      "version": "detect"
    }
  }
}
EOF

# 12. 配置 Vitest
cat > vitest.config.ts << 'EOF'
import { defineConfig } from 'vitest/config';
import react from '@vitejs/plugin-react';

export default defineConfig({
  plugins: [react()],
  test: {
    environment: 'jsdom',
    globals: true,
    setupFiles: './src/test/setup.ts',
    coverage: {
      provider: 'v8',
      reporter: ['text', 'json', 'html'],
      exclude: ['node_modules/', 'src/test/'],
    },
  },
});
EOF

# 13. 創建測試 setup
mkdir -p src/test
cat > src/test/setup.ts << 'EOF'
import { expect, afterEach } from 'vitest';
import { cleanup } from '@testing-library/react';
import * as matchers from '@testing-library/jest-dom/matchers';

expect.extend(matchers);

afterEach(() => {
  cleanup();
});
EOF
```

**產出**:
- ✅ React 18 + TypeScript 專案
- ✅ Tailwind CSS + shadcn/ui 配置
- ✅ React Router 路由設置
- ✅ Zustand 狀態管理
- ✅ TanStack Query 配置
- ✅ Vitest + Testing Library 配置

---

#### Task 0.3: Vue 3 Workflow Editor 設置 (Phase 2, 先建立骨架)

**執行步驟**:

```bash
# 1. 創建 Vue 3 應用
cd apps
pnpm create vite workflow-editor --template vue-ts

cd workflow-editor

# 2. 安裝核心依賴
pnpm add vue-router pinia
pnpm add @vueflow/core @vueflow/background @vueflow/controls @vueflow/minimap
pnpm add yjs y-websocket

# 3. 安裝開發依賴
pnpm add -D @vitejs/plugin-vue
pnpm add -D eslint @typescript-eslint/parser @typescript-eslint/eslint-plugin
pnpm add -D prettier eslint-config-prettier eslint-plugin-vue
pnpm add -D vitest @vue/test-utils

# 4. 配置 Vite
cat > vite.config.ts << 'EOF'
import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';
import path from 'path';

export default defineConfig({
  plugins: [vue()],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
    },
  },
  server: {
    port: 3001,
  },
});
EOF

# 5. 創建目錄結構
mkdir -p src/{components,composables,stores,types,utils,styles}

# 6. 創建基礎 VueFlow 組件
cat > src/components/WorkflowCanvas.vue << 'EOF'
<script setup lang="ts">
import { VueFlow, useVueFlow, Panel, Background, Controls, MiniMap } from '@vueflow/core';
import { ref } from 'vue';
import '@vueflow/core/dist/style.css';
import '@vueflow/core/dist/theme-default.css';

const { nodes, edges, addNodes, addEdges, onConnect } = useVueFlow();

// Phase 2 implementation
</script>

<template>
  <div class="workflow-canvas">
    <VueFlow :nodes="nodes" :edges="edges" @connect="onConnect">
      <Background />
      <Controls />
      <MiniMap />
      <Panel position="top-left">
        <h3>Workflow Editor V2</h3>
        <p>Phase 2 Implementation</p>
      </Panel>
    </VueFlow>
  </div>
</template>

<style scoped>
.workflow-canvas {
  width: 100vw;
  height: 100vh;
}
</style>
EOF

# 7. Phase 2 標記文件
cat > PHASE2.md << 'EOF'
# Workflow Editor - Phase 2 Implementation

## 當前狀態
- ✅ 專案骨架已建立
- ✅ VueFlow 依賴已安裝
- ⏸️ 完整實施待 Phase 2

## Phase 2 實施範圍
根據 docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-*.md:

1. 28 種節點類型實現
2. 內聯配置面板
3. 執行視覺化
4. 畫布互動與連接線
5. 多用戶協作 (Yjs CRDT)
6. 版本控制與歷史

## 參考文檔
- docs/ux-design/wireframes/low-fidelity/09-workflow-editor-v2-index.md
- docs/architecture/ADR-012-workflow-editor-technology.md
- poc-projects/poc6-vueflow-crdt/

## 預計實施時間
3-4 個月 (Phase 2)
EOF
```

**產出**:
- ✅ Vue 3 + TypeScript 專案骨架
- ✅ VueFlow 基礎配置
- ⏸️ Phase 2 標記 (完整實施延後)

---

### Day 5-6: 後端專案初始化

#### Task 0.4: ASP.NET Core API Gateway 設置

**執行步驟**:

```bash
# 1. 創建 API Gateway 專案
cd apps
dotnet new webapi -n api-gateway
cd api-gateway

# 2. 安裝 NuGet 套件
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package StackExchange.Redis
dotnet add package Swashbuckle.AspNetCore
dotnet add package Serilog.AspNetCore
dotnet add package FluentValidation.AspNetCore

# 3. 創建目錄結構
mkdir -p {Controllers,Models,Services,Middleware,Data,DTOs,Validators}

# 4. 配置 Program.cs
cat > Program.cs << 'EOF'
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

// Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "AI Workflow Platform API", Version = "v1" });
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://app.example.com")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
EOF

# 5. 創建 DbContext
cat > Data/AppDbContext.cs << 'EOF'
using Microsoft.EntityFrameworkCore;

namespace ApiGateway.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Agent> Agents => Set<Agent>();
    public DbSet<AgentExecution> AgentExecutions => Set<AgentExecution>();
    // ... (其他 DbSets)

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Table names (snake_case)
        modelBuilder.Entity<User>().ToTable("users");
        modelBuilder.Entity<Agent>().ToTable("agents");
        modelBuilder.Entity<AgentExecution>().ToTable("agent_executions");

        // Indexes
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<Agent>().HasIndex(a => a.UserId);

        // Relationships
        modelBuilder.Entity<Agent>()
            .HasOne(a => a.User)
            .WithMany(u => u.Agents)
            .HasForeignKey(a => a.UserId);
    }
}
EOF

# 6. 創建基礎 Models
cat > Models/User.cs << 'EOF'
namespace ApiGateway.Models;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "User"; // SuperAdmin, TenantAdmin, Developer, Analyst, User
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<Agent> Agents { get; set; } = new List<Agent>();
}
EOF

# 7. 創建範例 Controller
cat > Controllers/AgentsController.cs << 'EOF'
using Microsoft.AspNetCore.Mvc;
using ApiGateway.Data;
using ApiGateway.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AgentsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<AgentsController> _logger;

    public AgentsController(AppDbContext context, ILogger<AgentsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/v1/agents
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Agent>>> GetAgents([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var agents = await _context.Agents
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var total = await _context.Agents.CountAsync();

        return Ok(new
        {
            data = agents,
            metadata = new
            {
                total,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling(total / (double)pageSize)
            }
        });
    }

    // GET: api/v1/agents/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Agent>> GetAgent(Guid id)
    {
        var agent = await _context.Agents.FindAsync(id);

        if (agent == null)
        {
            return NotFound(new { error = new { code = "AGENT_NOT_FOUND", message = "Agent not found" } });
        }

        return Ok(new { data = agent });
    }

    // POST: api/v1/agents
    [HttpPost]
    public async Task<ActionResult<Agent>> CreateAgent(AgentCreateDto dto)
    {
        var agent = new Agent
        {
            Name = dto.Name,
            SystemPrompt = dto.SystemPrompt,
            ModelName = dto.ModelName,
            UserId = Guid.Parse(User.FindFirst("sub")?.Value ?? throw new UnauthorizedAccessException())
        };

        _context.Agents.Add(agent);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAgent), new { id = agent.Id }, new { data = agent });
    }

    // ... (其他 CRUD 操作)
}
EOF

# 8. 配置 appsettings
cat > appsettings.Development.json << 'EOF'
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=ai_workflow_dev;Username=postgres;Password=dev_password",
    "Redis": "localhost:6379"
  },
  "Serilog": {
    "Using": ["Serilog.Sinks.Console", "Serilog.Sinks.File"],
    "MinimumLevel": "Debug",
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "File",
        "Args": { "path": "logs/api-.log", "rollingInterval": "Day" }
      }
    ]
  }
}
EOF
```

**產出**:
- ✅ ASP.NET Core 8 API Gateway
- ✅ PostgreSQL + Entity Framework Core 配置
- ✅ Redis 配置
- ✅ Swagger API 文檔
- ✅ Serilog 日誌配置
- ✅ 基礎 CRUD Controllers

---

#### Task 0.5: Semantic Kernel Agent Service 設置

**執行步驟**:

```bash
# 1. 創建 Agent Service 專案
cd apps
dotnet new console -n agent-service
cd agent-service

# 2. 安裝 Semantic Kernel
dotnet add package Microsoft.SemanticKernel --version 1.66.0
dotnet add package Microsoft.Extensions.Configuration
dotnet add package Microsoft.Extensions.Configuration.Json
dotnet add package Microsoft.Extensions.Hosting
dotnet add package Serilog.Extensions.Hosting

# 3. 創建目錄結構
mkdir -p {Agents,Plugins,Services,Models}

# 4. 配置 Program.cs (參考 PoC 1)
# (從 poc-projects/poc1-sk-agents/ 複製核心邏輯)

# 5. 創建基礎 Agent 實現
cat > Agents/BaseAgent.cs << 'EOF'
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace AgentService.Agents;

public class BaseAgent
{
    private readonly Kernel _kernel;
    private readonly ILogger<BaseAgent> _logger;

    public BaseAgent(Kernel kernel, ILogger<BaseAgent> logger)
    {
        _kernel = kernel;
        _logger = logger;
    }

    public async Task<string> ExecuteAsync(string input)
    {
        _logger.LogInformation("Executing agent with input: {Input}", input);

        var response = await _kernel.InvokePromptAsync(input);

        return response.ToString();
    }
}
EOF

# 6. 創建 appsettings.json
cat > appsettings.json << 'EOF'
{
  "AzureOpenAI": {
    "Endpoint": "https://your-resource.openai.azure.com/",
    "DeploymentName": "gpt-4o",
    "ApiKey": "your-api-key"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
EOF
```

**產出**:
- ✅ Semantic Kernel Agent Service 骨架
- ⏸️ 完整實施待 Sprint 1

---

### Day 7-8: 基礎設施配置

#### Task 0.6: Docker Compose 開發環境設置

**執行步驟**:

```bash
# 1. 創建 Docker Compose 配置
cd infra/docker-compose

cat > dev.yml << 'EOF'
version: '3.8'

services:
  postgres:
    image: postgres:16-alpine
    container_name: ai-workflow-postgres
    environment:
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: dev_password
      POSTGRES_DB: ai_workflow_dev
    ports:
      - "5432:5432"
    volumes:
      - postgres-data:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      interval: 5s
      timeout: 5s
      retries: 5

  redis:
    image: redis:7-alpine
    container_name: ai-workflow-redis
    ports:
      - "6379:6379"
    volumes:
      - redis-data:/data
    healthcheck:
      test: ["CMD", "redis-cli", "ping"]
      interval: 5s
      timeout: 3s
      retries: 5

  rabbitmq:
    image: rabbitmq:3-management-alpine
    container_name: ai-workflow-rabbitmq
    environment:
      RABBITMQ_DEFAULT_USER: guest
      RABBITMQ_DEFAULT_PASS: guest
    ports:
      - "5672:5672"   # AMQP port
      - "15672:15672" # Management UI
    volumes:
      - rabbitmq-data:/var/lib/rabbitmq
    healthcheck:
      test: ["CMD", "rabbitmq-diagnostics", "ping"]
      interval: 10s
      timeout: 10s
      retries: 5

  # Adminer (Database UI)
  adminer:
    image: adminer:latest
    container_name: ai-workflow-adminer
    ports:
      - "8080:8080"
    environment:
      ADMINER_DEFAULT_SERVER: postgres

volumes:
  postgres-data:
  redis-data:
  rabbitmq-data:

networks:
  default:
    name: ai-workflow-network
EOF

# 2. 創建啟動腳本
cat > start-dev.sh << 'EOF'
#!/bin/bash

echo "🚀 Starting AI Workflow Platform Dev Environment..."

# Start infrastructure
docker compose -f dev.yml up -d

# Wait for services to be healthy
echo "⏳ Waiting for services to be ready..."
sleep 10

# Check health
docker compose -f dev.yml ps

echo "✅ Dev environment is ready!"
echo ""
echo "📊 Services:"
echo "  PostgreSQL: localhost:5432 (user: postgres, password: dev_password)"
echo "  Redis: localhost:6379"
echo "  RabbitMQ: localhost:5672"
echo "  RabbitMQ Management: http://localhost:15672 (user: guest, password: guest)"
echo "  Adminer: http://localhost:8080"
echo ""
echo "🛠️  Next steps:"
echo "  1. Run database migrations: cd apps/api-gateway && dotnet ef database update"
echo "  2. Start applications: pnpm dev"
EOF

chmod +x start-dev.sh

# 3. 創建停止腳本
cat > stop-dev.sh << 'EOF'
#!/bin/bash

echo "🛑 Stopping AI Workflow Platform Dev Environment..."

docker compose -f dev.yml down

echo "✅ Dev environment stopped."
EOF

chmod +x stop-dev.sh
```

**產出**:
- ✅ Docker Compose 配置 (PostgreSQL, Redis, RabbitMQ)
- ✅ 啟動/停止腳本
- ✅ Adminer 數據庫 UI

**驗證**:
```bash
cd infra/docker-compose
./start-dev.sh

# 測試連接
psql -h localhost -U postgres -d ai_workflow_dev
redis-cli ping
curl http://localhost:15672
```

---

### Day 9: CI/CD Pipeline 配置

#### Task 0.7: GitHub Actions 工作流程設置

**執行步驟**:

```bash
# 1. 創建 Feature Branch Pipeline
cat > .github/workflows/feature.yml << 'EOF'
name: Feature Branch CI

on:
  push:
    branches:
      - 'feature/**'
      - 'bugfix/**'
  pull_request:
    branches:
      - develop

jobs:
  lint-and-test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - uses: pnpm/action-setup@v2
        with:
          version: 8

      - uses: actions/setup-node@v4
        with:
          node-version: '20'
          cache: 'pnpm'

      - uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'

      - name: Install dependencies
        run: pnpm install

      - name: Lint
        run: pnpm turbo run lint

      - name: TypeScript typecheck
        run: pnpm turbo run typecheck

      - name: Unit tests
        run: pnpm turbo run test:unit

      - name: .NET tests
        run: dotnet test --configuration Release

      - name: Build
        run: pnpm turbo run build
EOF

# 2. 創建 Develop Branch Pipeline (參考 ISSUE-007 完整實現)

# 3. 配置 GitHub Secrets (手動在 GitHub UI 設置)
cat > .github/SECRETS.md << 'EOF'
# GitHub Secrets 配置清單

需要在 GitHub Repository Settings → Secrets and variables → Actions 添加:

## Azure 相關
- `AZURE_CREDENTIALS`: Azure Service Principal JSON
- `ACR_REGISTRY`: Azure Container Registry URL (e.g., myregistry.azurecr.io)
- `ACR_USERNAME`: ACR 用戶名
- `ACR_PASSWORD`: ACR 密碼

## OpenAI 相關
- `AZURE_OPENAI_ENDPOINT`: Azure OpenAI Endpoint
- `AZURE_OPENAI_KEY`: Azure OpenAI API Key

## 數據庫相關
- `DATABASE_URL_DEV`: Dev 環境數據庫連接字串
- `DATABASE_URL_STAGING`: Staging 環境數據庫連接字串
- `DATABASE_URL_PRODUCTION`: Production 環境數據庫連接字串

## 其他
- `SNYK_TOKEN`: Snyk security scan token (可選)
EOF
```

**產出**:
- ✅ Feature Branch CI Pipeline
- 📋 GitHub Secrets 配置清單
- ⏸️ Develop/Production Pipelines (待完整實施)

---

### Day 10: 團隊 Onboarding 與驗證

#### Task 0.8: 團隊環境驗證

**驗證清單**:

```markdown
# Sprint 0 驗證清單

## 每位開發者必須完成:

### 1. 環境設置
- [ ] 已安裝所有必須軟體 (參考 docs/technical-implementation/2-dev-environment/complete-setup-guide.md)
- [ ] VSCode 安裝所有必裝擴展
- [ ] Git 配置完成
- [ ] Docker Desktop 運行正常

### 2. 專案 Clone
- [ ] 專案已 clone 到本地
- [ ] pnpm install 成功
- [ ] 環境變數 (.env) 已配置

### 3. 基礎設施
- [ ] Docker Compose 成功啟動 (PostgreSQL, Redis, RabbitMQ)
- [ ] 可以連接 PostgreSQL (使用 Adminer 或 DBeaver)
- [ ] 可以連接 Redis (使用 RedisInsight)
- [ ] RabbitMQ Management UI 可訪問

### 4. 前端應用
- [ ] `cd apps/web && pnpm dev` 成功啟動
- [ ] 可以訪問 http://localhost:3000
- [ ] Hot reload 正常工作
- [ ] `pnpm test:unit` 測試通過
- [ ] `pnpm lint` 無錯誤

### 5. 後端應用
- [ ] `cd apps/api-gateway && dotnet build` 成功
- [ ] 數據庫遷移成功 `dotnet ef database update`
- [ ] `dotnet run` 成功啟動
- [ ] 可以訪問 http://localhost:5000/swagger
- [ ] `dotnet test` 測試通過

### 6. 文檔閱讀
- [ ] 已閱讀 docs/DOCUMENT-USAGE-STRATEGY.md
- [ ] 已閱讀 docs/technical-implementation/4-coding-standards/
- [ ] 已閱讀 docs/technical-implementation/development-workflow.md
- [ ] 了解 Git 工作流程 (GitFlow)
- [ ] 了解 Code Review Checklist
- [ ] 了解 Definition of Done

### 7. 團隊工具
- [ ] 已加入 Slack #dev-general 頻道
- [ ] 已加入 Jira 專案
- [ ] 已獲得 Azure OpenAI API Key (測試用)
- [ ] 已獲得 GitHub 倉庫訪問權限

## Sprint 0 Demo (Day 10 下午)

每位開發者需要 Demo:
1. 啟動 Docker Compose 基礎設施
2. 啟動前端應用並訪問
3. 啟動後端 API 並訪問 Swagger
4. 執行一個簡單的 API 請求 (例如: GET /api/v1/agents)
5. 運行測試並展示結果

## 問題追蹤
如有任何問題,記錄在:
- Slack #dev-help 頻道
- 或向 Scrum Master / 技術負責人求助
```

**負責人**: Scrum Master
**時間**: Day 10 全天 + Demo

---

## Sprint 0 完成標準

**Sprint 0 完成後,確認以下項目**:

### 基礎設施 ✅
- [ ] Monorepo 架構完整設置
- [ ] 前端應用 (React + Vue) 骨架完成
- [ ] 後端服務 (API Gateway, Agent Service) 骨架完成
- [ ] Docker Compose 開發環境可運行
- [ ] CI/CD Pipeline 配置完成 (至少 Feature Branch Pipeline)

### 團隊就緒 ✅
- [ ] 所有開發者環境驗證通過
- [ ] 所有開發者閱讀核心文檔
- [ ] 所有開發者了解開發流程
- [ ] Slack, Jira, GitHub 權限配置完成

### 文檔完整 ✅
- [ ] Dev Environment Setup Guide 完成
- [ ] Coding Standards (C# + TypeScript) 完成
- [ ] Development Workflow 定義完成
- [ ] API Design Guidelines 完成
- [ ] Database Naming Conventions 完成
- [ ] CI/CD Pipeline 文檔完成

### 技術驗證 ✅
- [ ] 前端可以成功 build 和 dev
- [ ] 後端可以成功 build 和 test
- [ ] 數據庫連接正常
- [ ] Redis 連接正常
- [ ] Azure OpenAI API 可以調用 (測試)

**Sprint 0 Review**: Week 4, Day 10 下午
**Sprint 1 Planning**: Week 5, Day 1

---

## Month 2-4: MVP 開發 (Sprint 1-8)

**目標**: 完成 MVP 核心功能開發 (8 個 Sprints × 2 週)

**Sprint 週期**: 2 週 (10 個工作天)

**參考文檔**:
- `docs/user-stories/epics/` - 所有 Epic 文件
- `docs/user-stories/mvp-planning.md` - MVP 範圍定義
- `docs/architecture/Architecture-Design-Document.md` - 系統架構
- `docs/ux-design/wireframes/` - UI 設計
- `poc-projects/` - PoC 參考代碼

### MVP Scope 總覽

根據 ISSUE-008 決策 (假設選項 A: 保持 Phase 2):

**Phase 1 MVP** - **7 個 Epics**:

| Epic | 名稱 | User Stories | 優先級 | Sprint |
|------|------|-------------|--------|--------|
| Epic 1 | Agent 生命週期管理 | 6 個 Stories | P0 (Core) | Sprint 1 |
| Epic 2 | Plugin 系統 | 4 個 Stories | P0 (Core) | Sprint 1-2 |
| Epic 5.1 | Conversation Memory | 3 個 Stories | P0 (Core) | Sprint 2 |
| Epic 6 | Chat Interface | 4 個 Stories | P0 (Core) | Sprint 2 |
| Epic 3 | Code Interpreter | 5 個 Stories | P1 (Differentiated) | Sprint 3-4 |
| Epic 7 | Persona Framework | 4 個 Stories | P1 (Differentiated) | Sprint 3-4 |
| Epic 8 | Text-to-SQL | 4 個 Stories | P1 (Differentiated) | Sprint 3-4 |
| Epic 5.2 | Knowledge RAG | 3 個 Stories | P2 (Advanced) | Sprint 5-6 |
| Epic 9.1 | Authentication & RBAC | 3 個 Stories | P3 (Enterprise) | Sprint 7 |
| Epic 9.2 | Multi-Tenant Support | 2 個 Stories | P3 (Enterprise) | Sprint 7 |
| Epic 10.1 | Monitoring Dashboard | 2 個 Stories | P3 (Enterprise) | Sprint 8 |
| Epic 10.2 | Quality Monitoring | 2 個 Stories | P3 (Enterprise) | Sprint 8 |

**總計**: **7 Epics**, **42 User Stories**, **8 Sprints** (16 週 = 4 個月)

**Phase 2** (延後):
- ❌ Epic 4: Multi-Agent Orchestration (4 Stories) → Phase 2
- ❌ Workflow Editor V2 (28 種節點類型) → Phase 2

---

### Sprint 規劃原則

#### Sprint 容量估算
- **每個 Sprint**: 2 週 = 10 個工作天
- **每位開發者**: 6 Story Points / Sprint (考慮 Code Review, 會議等)
- **團隊規模**: 假設 4 位開發者 (2 Frontend + 2 Backend)
- **總容量**: 24 Story Points / Sprint

#### Story Points 估算標準
```yaml
1 Point: 1-2 小時 (簡單 CRUD, UI 組件)
2 Points: 0.5 天 (中等複雜度, 單一功能)
3 Points: 1 天 (需要設計, 有複雜邏輯)
5 Points: 2-3 天 (複雜功能, 多模組整合)
8 Points: 4-5 天 (史詩級任務, 可能需切分)
```

#### Sprint 結構
```yaml
Sprint Pattern (2 weeks):
  Week 1:
    Day 1: Sprint Planning (半天) + 開發開始
    Day 2-5: 開發 + Daily Standup
  Week 2:
    Day 6-9: 開發 + Testing + Code Review
    Day 10: Sprint Review (Demo) + Sprint Retrospective
```

---

## Sprint 規劃詳細內容

### Sprint 1: 核心 Agent 能力 (Week 5-6)

**Sprint Goal**: 建立 Agent 的完整生命週期管理能力

**Epic 涵蓋**:
- Epic 1: Agent 生命週期管理 (6 Stories)
- Epic 2: Plugin 系統 (部分, 2 Stories)

#### User Stories 清單

| Story ID | 名稱 | 驗收標準數量 | Story Points | 負責 |
|----------|------|------------|-------------|------|
| US 1.1 | 通過 Web UI 建立 Agent | 6 個驗收標準 | 5 | Frontend + Backend |
| US 1.2 | 配置 System Prompt 和參數 | 5 個驗收標準 | 3 | Frontend + Backend |
| US 1.3 | Agent 列表與搜尋 | 5 個驗收標準 | 3 | Frontend + Backend |
| US 1.4 | Agent 詳情查看 | 4 個驗收標準 | 2 | Frontend + Backend |
| US 1.5 | Agent 編輯與更新 | 4 個驗收標準 | 3 | Frontend + Backend |
| US 1.6 | Agent 刪除與封存 | 3 個驗收標準 | 2 | Backend |
| US 2.1 | Plugin 列表與安裝 | 5 個驗收標準 | 5 | Frontend + Backend |
| US 2.2 | Plugin 配置 | 4 個驗收標準 | 3 | Frontend + Backend |

**總計**: 8 Stories, **26 Story Points**

#### Sprint 1 參考文檔

**開發階段參考**:

1. **需求理解** (Day 1):
   - 📄 `docs/user-stories/epics/epic-01.md` (Agent 生命週期管理)
   - 📄 `docs/user-stories/modules/module-01.md` (完整模組描述)
   - 📄 `docs/user-stories/mvp-planning.md` (MVP 範圍確認)

2. **UI 設計參考** (Day 1-2):
   - 📄 `docs/ux-design/wireframes/low-fidelity/02-agent-list.md` (Agent 列表頁面)
   - 📄 `docs/ux-design/wireframes/low-fidelity/03-agent-create.md` (Agent 創建頁面)
   - 📄 `docs/ux-design/wireframes/low-fidelity/04-agent-detail.md` (Agent 詳情頁面)
   - 📄 `docs/ux-design/design-system/component-library.md` (組件使用規範)
   - 📄 `docs/ux-design/design-system/color-palette.md` (顏色系統)

3. **架構設計參考** (Day 1-2):
   - 📄 `docs/architecture/Architecture-Design-Document.md` (Section 5: 核心服務)
   - 📄 `docs/architecture/database-schema.md` (agents 表設計)
   - 📄 `docs/architecture/ADR-006-agent-state-management.md` (狀態管理決策)
   - 📄 `docs/architecture/performance-scalability-strategy.md` (性能目標)

4. **技術標準參考** (全程):
   - 📄 `docs/technical-implementation/4-coding-standards/csharp-coding-standards.md`
   - 📄 `docs/technical-implementation/4-coding-standards/typescript-react-coding-standards.md`
   - 📄 `docs/technical-implementation/5-api-design/api-design-guidelines.md`
   - 📄 `docs/technical-implementation/6-database-standards/database-naming-conventions.md`

5. **PoC 參考代碼** (實作時):
   - 📁 `poc-projects/poc1-sk-agents/` (Semantic Kernel Agent 實作範例)
     - `Program.cs` - Agent 初始化
     - `Plugins/` - Plugin 系統範例
     - `Models/` - 數據模型

6. **測試策略參考** (Day 6-9):
   - 📄 `docs/technical-implementation/7-testing-strategy/` (測試金字塔)
   - 📄 `docs/poc-validation/POC-VALIDATION-REPORT.md` (PoC 1 驗證基準)

#### Sprint 1 開發流程範例

**US 1.1: 通過 Web UI 建立 Agent**

**Day 1 (Sprint Planning Day)**:
1. SM 從 `docs/user-stories/epics/epic-01.md` 提取 US 1.1
2. 團隊閱讀驗收標準 (6 個):
   ```
   1. ✅ 使用者可以訪問 "建立 Agent" 頁面
   2. ✅ 表單包含必填欄位: Agent 名稱, System Prompt, LLM 模型選擇
   3. ✅ 模型選擇下拉列表包含: GPT-4, GPT-4o, GPT-4o-mini
   4. ✅ 提交表單後,系統在後端建立 Agent 記錄
   5. ✅ 成功建立後,重定向到 Agent 詳情頁面
   6. ✅ 表單驗證: Agent 名稱不可為空, System Prompt 不可為空
   ```
3. Frontend Dev 查閱 `docs/ux-design/wireframes/low-fidelity/03-agent-create.md` 理解 UI
4. Backend Dev 查閱 `docs/architecture/database-schema.md` 理解 agents 表結構
5. 團隊估算: **5 Story Points** (2-3 天)
6. 分配任務:
   - Frontend Dev 1: 創建 AgentCreateForm 組件
   - Backend Dev 1: 創建 POST /api/v1/agents API
   - Backend Dev 2: 整合 Semantic Kernel

**Day 2-3 (前端開發)**:

Frontend Dev 1 執行步驟:

```tsx
// Step 1: 查閱 Wireframe
// docs/ux-design/wireframes/low-fidelity/03-agent-create.md

// Step 2: 創建組件 (apps/web/src/pages/agents/AgentCreate.tsx)
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useMutation } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { Input, Select, Textarea, Button } from '@/components/ui';

// Step 3: 查閱驗收標準,定義 Zod Schema
const agentCreateSchema = z.object({
  name: z.string().min(1, '名稱不可為空'),
  systemPrompt: z.string().min(1, 'System Prompt 不可為空'),
  modelName: z.enum(['GPT-4', 'GPT-4o', 'GPT-4o-mini']),
});

type AgentCreateForm = z.infer<typeof agentCreateSchema>;

export function AgentCreate() {
  const navigate = useNavigate();

  // React Hook Form setup
  const { register, handleSubmit, formState: { errors } } = useForm<AgentCreateForm>({
    resolver: zodResolver(agentCreateSchema),
  });

  // API mutation (TanStack Query)
  const createAgentMutation = useMutation({
    mutationFn: async (data: AgentCreateForm) => {
      // Step 4: 查閱 API Design Guidelines
      // docs/technical-implementation/5-api-design/api-design-guidelines.md
      const response = await fetch('/api/v1/agents', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data),
      });
      if (!response.ok) throw new Error('Failed to create agent');
      return response.json();
    },
    onSuccess: (data) => {
      // 驗收標準 5: 成功建立後,重定向到 Agent 詳情頁面
      navigate(`/agents/${data.data.id}`);
    },
  });

  const onSubmit = (data: AgentCreateForm) => {
    createAgentMutation.mutate(data);
  };

  return (
    <div className="max-w-2xl mx-auto">
      <h1 className="text-2xl font-bold mb-6">建立 Agent</h1>

      {/* Step 5: 參考 Wireframe 實現表單佈局 */}
      <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
        {/* 驗收標準 2: 必填欄位 */}
        <Input
          label="Agent 名稱"
          {...register('name')}
          error={errors.name?.message}
          placeholder="例如: 資料分析助手"
        />

        {/* 驗收標準 3: 模型選擇 */}
        <Select
          label="LLM 模型"
          {...register('modelName')}
          error={errors.modelName?.message}
          options={[
            { value: 'GPT-4', label: 'GPT-4 (最強大,最昂貴)' },
            { value: 'GPT-4o', label: 'GPT-4o (平衡)' },
            { value: 'GPT-4o-mini', label: 'GPT-4o-mini (快速,便宜)' },
          ]}
        />

        <Textarea
          label="System Prompt"
          {...register('systemPrompt')}
          error={errors.systemPrompt?.message}
          rows={6}
          placeholder="你是一位專業的資料分析助手..."
        />

        {/* 驗收標準 6: 表單驗證 */}
        <div className="flex justify-end gap-2">
          <Button
            type="button"
            variant="outline"
            onClick={() => navigate('/agents')}
          >
            取消
          </Button>
          <Button
            type="submit"
            variant="primary"
            loading={createAgentMutation.isPending}
          >
            建立 Agent
          </Button>
        </div>
      </form>
    </div>
  );
}

// Step 6: 撰寫單元測試
// apps/web/src/pages/agents/AgentCreate.test.tsx
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { AgentCreate } from './AgentCreate';

describe('AgentCreate', () => {
  it('should render form fields', () => {
    render(<AgentCreate />);
    expect(screen.getByLabelText('Agent 名稱')).toBeInTheDocument();
    expect(screen.getByLabelText('LLM 模型')).toBeInTheDocument();
    expect(screen.getByLabelText('System Prompt')).toBeInTheDocument();
  });

  it('should validate required fields', async () => {
    const user = userEvent.setup();
    render(<AgentCreate />);

    // Submit without filling fields
    await user.click(screen.getByText('建立 Agent'));

    // Check validation errors
    await waitFor(() => {
      expect(screen.getByText('名稱不可為空')).toBeInTheDocument();
      expect(screen.getByText('System Prompt 不可為空')).toBeInTheDocument();
    });
  });

  // ... (更多測試案例)
});
```

**Day 2-3 (後端開發)**:

Backend Dev 1 執行步驟:

```csharp
// Step 1: 查閱 Database Schema
// docs/architecture/database-schema.md

// Step 2: 創建 Model (apps/api-gateway/Models/Agent.cs)
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiGateway.Models;

[Table("agents")] // 遵循 Database Naming Convention
public class Agent
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string SystemPrompt { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string ModelName { get; set; } = "GPT-4o";

    [Required]
    public Guid UserId { get; set; }

    public string Status { get; set; } = "active"; // active, inactive, archived

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;

    public ICollection<AgentExecution> Executions { get; set; } = new List<AgentExecution>();
}

// Step 3: 創建 DTO (apps/api-gateway/DTOs/AgentCreateDto.cs)
using System.ComponentModel.DataAnnotations;

namespace ApiGateway.DTOs;

public class AgentCreateDto
{
    [Required(ErrorMessage = "名稱不可為空")]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "System Prompt 不可為空")]
    public string SystemPrompt { get; set; } = string.Empty;

    [Required]
    [AllowedValues("GPT-4", "GPT-4o", "GPT-4o-mini")]
    public string ModelName { get; set; } = "GPT-4o";
}

// Step 4: 查閱 API Design Guidelines
// docs/technical-implementation/5-api-design/api-design-guidelines.md

// Step 5: 創建 Controller (apps/api-gateway/Controllers/AgentsController.cs)
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiGateway.Data;
using ApiGateway.Models;
using ApiGateway.DTOs;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AgentsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<AgentsController> _logger;

    public AgentsController(AppDbContext context, ILogger<AgentsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// 建立新的 Agent
    /// </summary>
    /// <remarks>
    /// 驗收標準 4: 提交表單後,系統在後端建立 Agent 記錄
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Agent>> CreateAgent([FromBody] AgentCreateDto dto)
    {
        _logger.LogInformation("Creating agent: {AgentName}", dto.Name);

        // 驗收標準 6: 表單驗證
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                error = new
                {
                    code = "VALIDATION_ERROR",
                    message = "表單驗證失敗",
                    details = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                }
            });
        }

        // Step 6: 查閱 C# Coding Standards (錯誤處理)
        try
        {
            // 取得當前使用者 ID (從 JWT Token)
            var userId = Guid.Parse(User.FindFirst("sub")?.Value
                ?? throw new UnauthorizedAccessException("User not authenticated"));

            // 建立 Agent
            var agent = new Agent
            {
                Name = dto.Name,
                SystemPrompt = dto.SystemPrompt,
                ModelName = dto.ModelName,
                UserId = userId,
                Status = "active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Agents.Add(agent);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Agent created successfully: {AgentId}", agent.Id);

            // 驗收標準 5: 返回新建立的 Agent (用於前端重定向)
            return CreatedAtAction(
                nameof(GetAgent),
                new { id = agent.Id },
                new { data = agent, metadata = new { requestId = HttpContext.TraceIdentifier } }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create agent");
            return StatusCode(500, new
            {
                error = new
                {
                    code = "INTERNAL_ERROR",
                    message = "建立 Agent 失敗,請稍後再試"
                }
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Agent>> GetAgent(Guid id)
    {
        var agent = await _context.Agents.FindAsync(id);
        if (agent == null)
        {
            return NotFound(new { error = new { code = "AGENT_NOT_FOUND", message = "Agent not found" } });
        }
        return Ok(new { data = agent });
    }
}

// Step 7: 撰寫單元測試 (apps/api-gateway.Tests/Controllers/AgentsControllerTests.cs)
using Xunit;
using Microsoft.EntityFrameworkCore;
using ApiGateway.Controllers;
using ApiGateway.Data;
using ApiGateway.DTOs;

public class AgentsControllerTests
{
    private readonly AppDbContext _context;
    private readonly AgentsController _controller;

    public AgentsControllerTests()
    {
        // In-memory database for testing
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;
        _context = new AppDbContext(options);
        _controller = new AgentsController(_context, Mock.Of<ILogger<AgentsController>>());
    }

    [Fact]
    public async Task CreateAgent_ValidDto_ReturnsCreatedAgent()
    {
        // Arrange
        var dto = new AgentCreateDto
        {
            Name = "Test Agent",
            SystemPrompt = "You are a test assistant",
            ModelName = "GPT-4o"
        };

        // Act
        var result = await _controller.CreateAgent(dto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var agent = Assert.IsType<Agent>((createdResult.Value as dynamic).data);
        Assert.Equal("Test Agent", agent.Name);
        Assert.Equal("active", agent.Status);
    }

    [Fact]
    public async Task CreateAgent_EmptyName_ReturnsBadRequest()
    {
        // Arrange
        var dto = new AgentCreateDto
        {
            Name = "",
            SystemPrompt = "Test",
            ModelName = "GPT-4o"
        };

        // Act
        _controller.ModelState.AddModelError("Name", "名稱不可為空");
        var result = await _controller.CreateAgent(dto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }
}
```

**Day 4 (整合 Semantic Kernel)**:

Backend Dev 2 執行步驟:

```csharp
// Step 1: 查閱 PoC 1 參考代碼
// poc-projects/poc1-sk-agents/Program.cs

// Step 2: 查閱 ADR-006 (Agent State Management)
// docs/architecture/ADR-006-agent-state-management.md

// Step 3: 創建 Agent Service (apps/agent-service/Services/AgentExecutionService.cs)
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace AgentService.Services;

public class AgentExecutionService
{
    private readonly IKernelBuilder _kernelBuilder;
    private readonly ILogger<AgentExecutionService> _logger;

    public AgentExecutionService(IConfiguration config, ILogger<AgentExecutionService> logger)
    {
        _logger = logger;

        // 從 PoC 1 複製 Kernel 配置
        _kernelBuilder = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(
                deploymentName: config["AzureOpenAI:DeploymentName"],
                endpoint: config["AzureOpenAI:Endpoint"],
                apiKey: config["AzureOpenAI:ApiKey"]
            );
    }

    public async Task<string> ExecuteAsync(Guid agentId, string systemPrompt, string userInput)
    {
        _logger.LogInformation("Executing agent {AgentId}", agentId);

        // 建立 Kernel 實例
        var kernel = _kernelBuilder.Build();

        // 執行 Prompt (從 PoC 1 複製邏輯)
        var executionSettings = new OpenAIPromptExecutionSettings
        {
            MaxTokens = 1000,
            Temperature = 0.7
        };

        var prompt = $"{systemPrompt}\n\nUser: {userInput}";
        var response = await kernel.InvokePromptAsync(prompt, new(executionSettings));

        return response.ToString();
    }
}
```

**Day 5-9 (測試與 Code Review)**:

1. **Frontend 測試** (Day 5-6):
   - 執行單元測試: `pnpm --filter web run test:unit`
   - 執行 E2E 測試: `pnpm --filter web run test:e2e`
   - Accessibility 測試: WCAG 2.1 AA 檢查 (使用 axe DevTools)
   - 響應式測試: Desktop (1920px) / Tablet (768px) / Mobile (375px)

2. **Backend 測試** (Day 5-6):
   - 執行單元測試: `dotnet test --filter FullyQualifiedName~AgentsController`
   - 整合測試: 使用 WebApplicationFactory 測試完整 API flow
   - 性能測試: 驗證 API P95 latency <300ms (參考 performance-scalability-strategy.md)

3. **Code Review** (Day 7-8):
   - Frontend Lead 審查 AgentCreate.tsx
     - 檢查: Coding Standards, Component 結構, Accessibility
   - Backend Lead 審查 AgentsController.cs
     - 檢查: Coding Standards, API Design, Error Handling, Security
   - Architect 審查整體架構是否符合 Architecture Document

4. **Bug 修復與改進** (Day 8-9):
   - 根據 Code Review 回饋修改
   - 確保所有驗收標準通過

**Day 10 (Sprint Review & Retrospective)**:

**Sprint Review (下午 2:00-3:30)**:
- Demo US 1.1-1.6, US 2.1-2.2 (8 Stories)
- 展示:
  1. 建立 Agent 流程 (從 UI 到數據庫)
  2. Agent 列表與搜尋
  3. Agent 詳情查看
  4. Agent 編輯與刪除
  5. Plugin 列表與安裝
- Stakeholder 回饋收集

**Sprint Retrospective (下午 3:30-4:30)**:
- What went well?
  - Wireframes 非常清晰,前端實作順利
  - PoC 1 代碼參考價值高
  - Code Review 質量好
- What could be improved?
  - Database migration 遇到一些問題 (未遵循 Naming Convention)
  - 測試覆蓋率未達 80% (實際 72%)
- Action items for Sprint 2:
  - 更嚴格遵循 Database Naming Convention
  - 增加測試覆蓋率目標到 80%+

---

### Sprint 2: Chat Interface & Memory (Week 7-8)

**Sprint Goal**: 完成對話介面和記憶管理

**Epic 涵蓋**:
- Epic 5.1: Conversation Memory (3 Stories)
- Epic 6: Chat Interface (4 Stories)
- Epic 2: Plugin 系統 (剩餘 2 Stories)

#### User Stories 清單

| Story ID | 名稱 | Story Points | 負責 |
|----------|------|-------------|------|
| US 5.1 | 短期記憶 (Session Memory) | 5 | Backend |
| US 5.2 | 長期記憶 (Persistent Memory) | 5 | Backend |
| US 5.3 | 記憶檢索與相關性排序 | 5 | Backend |
| US 6.1 | 基礎對話介面 (文字) | 5 | Frontend |
| US 6.2 | 多模態消息展示 | 5 | Frontend |
| US 6.3 | 對話歷史管理 | 3 | Frontend + Backend |
| US 6.4 | 實時回應 (SSE) | 5 | Frontend + Backend |
| US 2.3 | Plugin 測試與驗證 | 3 | Backend |
| US 2.4 | Plugin 錯誤處理 | 2 | Backend |

**總計**: 9 Stories, **38 Story Points** (需適當調整範圍)

**調整建議**: 移除 US 2.3, US 2.4 → **33 Story Points** (更合理)

#### Sprint 2 參考文檔

**開發階段參考**:

1. **需求理解**:
   - 📄 `docs/user-stories/epics/epic-05.1-conversation-memory.md`
   - 📄 `docs/user-stories/epics/epic-06.md` (Chat Interface)
   - 📄 `docs/user-stories/modules/module-05.md`, `module-06.md`

2. **UI 設計參考**:
   - 📄 `docs/ux-design/wireframes/low-fidelity/05-conversation.md` ⭐ **核心**
   - 📄 `docs/ux-design/design-system/component-library.md` (Message 組件)

3. **架構設計參考**:
   - 📄 `docs/architecture/Architecture-Design-Document.md` (Section 5.1: Conversation Service)
   - 📄 `docs/architecture/database-schema.md` (conversations, messages 表)
   - 📄 `docs/architecture/ADR-006-agent-state-management.md` (記憶存儲策略)
   - 📄 `docs/architecture/performance-scalability-strategy.md` (Redis caching for session memory)

4. **PoC 參考代碼**:
   - 📁 `poc-projects/poc1-sk-agents/` (Semantic Kernel Memory)
   - 📁 `poc-projects/poc5-knowledge-rag/` (Memory retrieval patterns)

5. **技術實施**:
   - 📄 `docs/technical-implementation/5-api-design/` (WebSocket/SSE 設計)
   - 📄 `docs/technical-implementation/6-database-standards/` (messages 表設計)

#### Sprint 2 技術亮點

**實時回應 (US 6.4)**:

使用 Server-Sent Events (SSE) 實現流式回應:

```typescript
// Frontend (React)
useEffect(() => {
  const eventSource = new EventSource(`/api/v1/agents/${agentId}/execute/stream`);

  eventSource.onmessage = (event) => {
    const data = JSON.parse(event.data);
    setStreamingMessage((prev) => prev + data.chunk);
  };

  eventSource.onerror = () => {
    eventSource.close();
  };

  return () => eventSource.close();
}, [agentId]);
```

```csharp
// Backend (ASP.NET Core)
[HttpPost("{id}/execute/stream")]
public async Task ExecuteStream(Guid id, [FromBody] ExecuteRequest request)
{
    Response.Headers.Add("Content-Type", "text/event-stream");
    Response.Headers.Add("Cache-Control", "no-cache");

    await foreach (var chunk in _agentService.ExecuteStreamAsync(id, request.Input))
    {
        await Response.WriteAsync($"data: {JsonSerializer.Serialize(new { chunk })}\n\n");
        await Response.Body.FlushAsync();
    }
}
```

---

### Sprint 3-4: 差異化能力 (Week 9-12)

**Sprint Goal**: 實現三大核心差異化能力 (Persona, Code Interpreter, Text-to-SQL)

**Epic 涵蓋**:
- Epic 7: Persona Framework (4 Stories)
- Epic 3: Code Interpreter (5 Stories)
- Epic 8: Text-to-SQL (4 Stories)

**總計**: 13 Stories, 預估 **65 Story Points** (需分為 2 個 Sprints)

#### Sprint 3 (Week 9-10)

| Story ID | 名稱 | Story Points | 負責 |
|----------|------|-------------|------|
| US 7.1 | Persona 模板建立 | 5 | Frontend + Backend |
| US 7.2 | Few-Shot 範例編輯 | 5 | Frontend |
| US 7.3 | Persona 測試與預覽 | 3 | Frontend + Backend |
| US 7.4 | Persona 版本管理 | 3 | Backend |
| US 3.1 | 代碼執行環境 (Docker) | 8 | Backend + DevOps |
| US 3.2 | 安全沙盒配置 | 5 | Backend + DevOps |

**總計**: 6 Stories, **29 Story Points**

#### Sprint 4 (Week 11-12)

| Story ID | 名稱 | Story Points | 負責 |
|----------|------|-------------|------|
| US 3.3 | 代碼輸入與執行 UI | 5 | Frontend |
| US 3.4 | 執行結果展示 (圖表) | 5 | Frontend |
| US 3.5 | 執行歷史與管理 | 3 | Frontend + Backend |
| US 8.1 | Schema 探索與管理 | 5 | Backend |
| US 8.2 | 自然語言轉 SQL | 5 | Backend |
| US 8.3 | SQL 安全驗證 | 5 | Backend |
| US 8.4 | 查詢結果展示 | 3 | Frontend |

**總計**: 7 Stories, **31 Story Points**

#### Sprint 3-4 參考文檔

**Persona Framework (Sprint 3)**:
- 📄 `docs/ux-design/wireframes/low-fidelity/10-persona-builder.md` ⭐ **核心**
- 📁 `poc-projects/poc2-persona-builder/` (完整實作參考)
- 📄 `docs/user-stories/epics/epic-07.md`

**Code Interpreter (Sprint 3-4)**:
- 📄 `docs/ux-design/wireframes/low-fidelity/07-code-interpreter.md` ⭐ **核心**
- 📁 `poc-projects/poc3-code-interpreter/` (Docker 安全配置)
- 📄 `docs/user-stories/epics/epic-03.md`
- 📄 `docs/architecture/ADR-008-code-execution-security.md` (4 層安全架構)
- 📄 `docs/technical-implementation/9-security-standards/` (Seccomp + AppArmor)

**Text-to-SQL (Sprint 4)**:
- 📄 `docs/ux-design/wireframes/low-fidelity/08-text-to-sql.md` ⭐ **核心**
- 📁 `poc-projects/poc4-text-to-sql/` (SQL 生成與驗證邏輯)
- 📄 `docs/user-stories/epics/epic-08.md`
- 📄 `docs/architecture/database-schema.md` (data_sources 表)

#### Sprint 3-4 技術重點

**Code Interpreter 安全 (US 3.1-3.2)**:

4 層安全架構實施:
```yaml
Layer 1: Docker 隔離
  - 使用 Docker SDK for .NET 啟動容器
  - 只讀 filesystem (除 /tmp)
  - 無特權模式 (--cap-drop=ALL)

Layer 2: 資源限制
  - CPU: 1 core
  - Memory: 256MB
  - 超時: 30 秒
  - 磁碟: /tmp 限制 100MB

Layer 3: 網絡隔離
  - --network=none (無外網訪問)
  - 僅允許與 API 通信 (通過 volume mount)

Layer 4: Seccomp + AppArmor (Sprint 4 或 Phase 1 後)
  - Seccomp profile: 限制系統調用
  - AppArmor policy: 限制文件訪問
```

**參考**: `poc-projects/poc3-code-interpreter/` 的 `docker-compose.yml` 和 `seccomp-profile.json`

---

### Sprint 5-6: 進階功能 (Week 13-16)

**Sprint Goal**: Knowledge RAG + Multi-Agent (視 ISSUE-008 決策)

**Epic 涵蓋**:
- Epic 5.2: Knowledge RAG (3 Stories)
- Epic 4: Multi-Agent Orchestration (4 Stories, 如果 Phase 1)

#### Sprint 5 (Week 13-14)

**Knowledge RAG**:

| Story ID | 名稱 | Story Points | 負責 |
|----------|------|-------------|------|
| US 5.4 | 文件上傳與管理 | 5 | Frontend + Backend |
| US 5.5 | 文件解析與 Embedding | 8 | Backend |
| US 5.6 | RAG 檢索與生成 | 8 | Backend |
| US 5.7 | 4 種檢索策略 | 5 | Backend |

**總計**: 4 Stories, **26 Story Points**

#### Sprint 5 參考文檔

- 📄 `docs/ux-design/wireframes/low-fidelity/06-knowledge-base.md` ⭐ **核心**
- 📁 `poc-projects/poc5-knowledge-rag/` (RAG 實作範例)
- 📄 `docs/user-stories/epics/epic-05.2-knowledge-rag.md`
- 📄 `docs/architecture/Architecture-Design-Document.md` (Section 5.3: Knowledge Service)

**RAG 4 種檢索策略** (US 5.7):
1. **Vector Search** (向量檢索): 使用 Embedding 相似度
2. **Keyword Search** (關鍵字檢索): 使用 PostgreSQL Full-Text Search
3. **Hybrid Search** (混合檢索): Vector + Keyword 結合
4. **Re-ranking** (重排序): 使用 Semantic Kernel Reranker

---

### Sprint 6 (Week 15-16)

**根據 ISSUE-008 決策**:

**選項 A (Workflow Editor Phase 2)**: 聚焦 Knowledge RAG 優化 + 文檔完善
- Knowledge RAG 性能優化 (達到 90%+ 準確率目標)
- API 文檔補充 (OpenAPI spec)
- E2E 測試補充
- 技術債務清理

**選項 B (Workflow Editor Phase 1)**: 開始 Multi-Agent Orchestration
- US 4.1-4.4 (Multi-Agent Orchestration 4 Stories)
- 整合 Workflow Editor V2 基礎版本

---

### Sprint 7-8: 企業功能與監控 (Week 17-20)

**Sprint Goal**: 完成企業級功能和監控系統

**Epic 涵蓋**:
- Epic 9.1: Authentication & RBAC (3 Stories)
- Epic 9.2: Multi-Tenant Support (2 Stories)
- Epic 10.1: Monitoring Dashboard (2 Stories)
- Epic 10.2: Quality Monitoring (2 Stories)

#### Sprint 7 (Week 17-18)

**Authentication & Multi-Tenant**:

| Story ID | 名稱 | Story Points | 負責 |
|----------|------|-------------|------|
| US 9.1 | 使用者註冊與登入 (OAuth 2.0 + JWT) | 8 | Backend |
| US 9.2 | RBAC 權限管理 (5 個角色) | 5 | Backend |
| US 9.3 | 權限檢查中介層 | 3 | Backend |
| US 9.4 | 多租戶數據隔離 | 8 | Backend |
| US 9.5 | 租戶管理 UI | 5 | Frontend |

**總計**: 5 Stories, **29 Story Points**

#### Sprint 8 (Week 19-20)

**Monitoring & Quality**:

| Story ID | 名稱 | Story Points | 負責 |
|----------|------|-------------|------|
| US 10.1 | 監控儀表板 (Metrics) | 5 | Frontend + DevOps |
| US 10.2 | 執行歷史與日誌查詢 | 5 | Frontend + Backend |
| US 10.3 | 質量監控 (準確率, 延遲) | 5 | Backend |
| US 10.4 | 告警規則配置 | 5 | Backend + DevOps |
| US 10.5 | 用戶分析 Dashboard | 3 | Frontend |

**總計**: 5 Stories, **23 Story Points**

#### Sprint 7-8 參考文檔

**Authentication & RBAC**:
- 📄 `docs/user-stories/epics/epic-09.1-authentication.md` ⭐ **核心**
- 📄 `docs/architecture/Architecture-Design-Document.md` (Section 6: 安全架構)
- 📄 `docs/technical-implementation/9-security-standards/`

**Monitoring**:
- 📄 `docs/ux-design/wireframes/low-fidelity/12-monitoring.md` ⭐ **核心**
- 📄 `docs/user-stories/epics/epic-10.1-monitoring-dashboard.md`
- 📄 `docs/technical-implementation/10-monitoring-operations/`
- 📄 `docs/architecture/performance-scalability-strategy.md` (Monitoring 章節)

**RBAC 5 個角色權限矩陣** (US 9.2):

| 操作 | SuperAdmin | TenantAdmin | Developer | Analyst | User |
|------|-----------|------------|-----------|---------|------|
| 管理租戶 | ✅ | ❌ | ❌ | ❌ | ❌ |
| 管理用戶 | ✅ | ✅ | ❌ | ❌ | ❌ |
| 建立 Agent | ✅ | ✅ | ✅ | ✅ | ❌ |
| 執行 Agent | ✅ | ✅ | ✅ | ✅ | ✅ |
| 上傳知識庫 | ✅ | ✅ | ✅ | ✅ | ❌ |
| 查看監控 | ✅ | ✅ | ✅ | ❌ | ❌ |
| 配置 Plugin | ✅ | ✅ | ✅ | ❌ | ❌ |

---

## MVP 開發完成檢查清單

**Sprint 8 完成後,確認以下項目**:

### 功能完整性 ✅
- [ ] **Epic 1**: Agent 生命週期管理 (6 Stories) 全部完成
- [ ] **Epic 2**: Plugin 系統 (4 Stories) 全部完成
- [ ] **Epic 3**: Code Interpreter (5 Stories) 全部完成
- [ ] **Epic 5.1**: Conversation Memory (3 Stories) 全部完成
- [ ] **Epic 5.2**: Knowledge RAG (3 Stories) 全部完成
- [ ] **Epic 6**: Chat Interface (4 Stories) 全部完成
- [ ] **Epic 7**: Persona Framework (4 Stories) 全部完成
- [ ] **Epic 8**: Text-to-SQL (4 Stories) 全部完成
- [ ] **Epic 9.1-9.2**: Enterprise Features (5 Stories) 全部完成
- [ ] **Epic 10.1-10.2**: Monitoring (4 Stories) 全部完成

**總計**: **7 Epics**, **42 User Stories** 全部完成 ✅

### 質量標準 ✅
- [ ] **測試覆蓋率**:
  - Frontend Unit Tests: ≥80%
  - Backend Unit Tests: ≥80%
  - Integration Tests: ≥60%
  - E2E Tests: 核心流程 100%
- [ ] **性能目標** (from `performance-scalability-strategy.md`):
  - API Gateway P95 Latency: <300ms
  - Agent Execution P95 Latency: <5s
  - Code Interpreter P95 Latency: <8s
  - Knowledge RAG Accuracy: ≥90%
- [ ] **安全標準**:
  - Code Interpreter 安全分數: ≥90/100
  - OWASP Top 10 檢查通過
  - 所有 API 需要認證
  - RBAC 權限檢查完整
- [ ] **Accessibility**:
  - WCAG 2.1 AA 標準通過
  - 鍵盤導航支援
  - 屏幕閱讀器兼容
- [ ] **Code Quality**:
  - 無 ESLint errors
  - 無 TypeScript errors (strict mode)
  - 所有 Code Review comments resolved
  - 技術債務 <5%

### 文檔完整性 ✅
- [ ] **API 文檔**: OpenAPI 3.0 spec 完整
- [ ] **用戶文檔**: 用戶指南和教程
- [ ] **開發者文檔**: README, 架構圖, 部署指南
- [ ] **Changelog**: 記錄所有功能和變更

### 部署就緒 ✅
- [ ] Docker Compose 生產配置完成
- [ ] Kubernetes manifests 完成
- [ ] CI/CD Pipeline 全部測試通過
- [ ] Dev, Staging 環境部署成功
- [ ] 監控和日誌系統配置完成

---

## Month 5: Alpha Testing (Week 21-24)

**目標**: 內部測試、Bug 修復、性能優化

### Week 21-22: Alpha Testing

#### 測試範圍

1. **功能測試** (Week 21):
   - 所有 42 個 User Stories 的驗收標準測試
   - 跨瀏覽器測試 (Chrome, Firefox, Safari, Edge)
   - 響應式測試 (Desktop, Tablet, Mobile)

2. **整合測試** (Week 21):
   - Agent → Code Interpreter 整合
   - Agent → Knowledge RAG 整合
   - Agent → Text-to-SQL 整合
   - Multi-user concurrent testing

3. **性能測試** (Week 22):
   - Load Testing (k6):
     - 模擬 100 concurrent users
     - 目標: API P95 <300ms, Agent P95 <5s
   - Stress Testing:
     - 找出系統瓶頸
   - Endurance Testing:
     - 24 小時穩定性測試

4. **安全測試** (Week 22):
   - OWASP ZAP 自動掃描
   - Manual penetration testing
   - Code Interpreter 沙盒逃逸測試
   - SQL Injection 測試 (Text-to-SQL)

#### Bug 追蹤與修復

**Bug 優先級**:
- **P0 (Critical)**: 系統無法使用、數據丟失、安全漏洞 → 立即修復
- **P1 (High)**: 核心功能受影響 → 1-2 天內修復
- **P2 (Medium)**: 非核心功能問題 → 1 週內修復
- **P3 (Low)**: UI 小問題、文檔錯誤 → Phase 2 修復

**測試報告範例**:
```markdown
# Alpha Testing Bug Report

## P0 Critical Bugs
1. **Bug-001**: Code Interpreter 沙盒逃逸 (Security)
   - 重現步驟: ...
   - 影響: 可以訪問宿主機文件系統
   - 修復優先級: P0
   - 負責人: Backend Dev 2
   - 狀態: 🔴 Open → 🟢 Fixed (2025-11-15)

## P1 High Bugs
2. **Bug-002**: Knowledge RAG 準確率只有 75% (低於目標 90%)
   - 重現步驟: ...
   - 影響: 核心差異化能力未達標
   - 修復優先級: P1
   - 負責人: Backend Dev 1
   - 狀態: 🟡 In Progress

## P2 Medium Bugs
3. **Bug-003**: Agent 列表頁面 Pagination 異常
   - 重現步驟: ...
   - 影響: 使用體驗受影響
   - 修復優先級: P2
   - 負責人: Frontend Dev 1
   - 狀態: 🟢 Fixed

## Performance Issues
4. **Perf-001**: API Gateway P95 latency 450ms (超過目標 300ms)
   - 原因分析: Database N+1 query
   - 修復方案: 添加 eager loading
   - 修復後: 280ms ✅
```

### Week 23-24: 性能優化與文檔完善

#### 性能優化項目

1. **Frontend 優化** (Week 23):
   - Code splitting (React.lazy)
   - Image optimization (WebP, lazy loading)
   - Bundle size 優化 (<500KB gzipped)
   - Lighthouse score >90

2. **Backend 優化** (Week 23-24):
   - Database query optimization (N+1 問題)
   - Redis caching 策略優化
   - Connection pooling 調優
   - Background job 優化 (Hangfire/Quartz)

3. **監控與告警** (Week 24):
   - Application Insights 完整配置
   - Prometheus + Grafana Dashboards
   - Alert rules 配置
   - 日誌聚合 (ELK/Loki)

#### 文檔完善

1. **用戶文檔**:
   - 用戶指南 (User Guide)
   - 快速開始 (Quickstart)
   - FAQ
   - 教程 (Tutorials)

2. **開發者文檔**:
   - API 參考 (OpenAPI/Swagger)
   - 架構圖更新
   - 部署指南
   - Troubleshooting

3. **管理員文檔**:
   - 安裝指南
   - 配置指南
   - 監控指南
   - 備份與恢復

---

## 開發工作流程指南

本節提供每個開發者在日常開發中需要遵循的完整工作流程。

**參考文檔**: `docs/technical-implementation/development-workflow.md`

### 1. 每日開發流程

#### 早上開始工作 (9:00 AM)

```bash
# Step 1: 同步代碼
cd ai-workflow-platform
git checkout develop
git pull origin develop

# Step 2: 查看今日任務
# 在 Jira 查看 Sprint Board → "In Progress" 任務

# Step 3: 切換到 feature 分支 (或創建新分支)
git checkout -b feature/US-1.1-agent-create

# Step 4: 啟動開發環境
cd infra/docker-compose
./start-dev.sh

# 等待服務啟動 (約 30 秒)

# Step 5: 啟動應用
cd ../..
pnpm dev  # 或 dotnet run (後端)
```

#### Daily Standup (9:30 AM, 15 分鐘)

在 Slack #dev-standup 頻道或視訊會議回答三個問題:
1. **昨天完成了什麼**: "完成了 US 1.1 的前端表單組件"
2. **今天計劃做什麼**: "實作 Agent Create API 並整合前後端"
3. **有什麼阻礙**: "需要確認 Database Schema 的 agents 表設計"

#### 開發時段 (9:30 AM - 12:00 PM, 1:00 PM - 6:00 PM)

**Step 1: 閱讀 User Story 和驗收標準**

```bash
# 查閱 Epic 文件
code docs/user-stories/epics/epic-01.md

# 找到 US 1.1: 通過 Web UI 建立 Agent
# 閱讀 6 個驗收標準:
# 1. ✅ 使用者可以訪問 "建立 Agent" 頁面
# 2. ✅ 表單包含必填欄位: Agent 名稱, System Prompt, LLM 模型選擇
# 3. ✅ 模型選擇下拉列表包含: GPT-4, GPT-4o, GPT-4o-mini
# 4. ✅ 提交表單後,系統在後端建立 Agent 記錄
# 5. ✅ 成功建立後,重定向到 Agent 詳情頁面
# 6. ✅ 表單驗證: Agent 名稱不可為空, System Prompt 不可為空
```

**Step 2: 查閱相關設計文檔**

```bash
# Frontend: 查閱 Wireframe
code docs/ux-design/wireframes/low-fidelity/03-agent-create.md

# Backend: 查閱 Architecture
code docs/architecture/Architecture-Design-Document.md
code docs/architecture/database-schema.md

# 查閱 PoC 參考代碼
code poc-projects/poc1-sk-agents/Program.cs
```

**Step 3: 實作功能**

**Frontend 實作流程**:
```tsx
// 1. 創建組件文件
// apps/web/src/pages/agents/AgentCreate.tsx

// 2. 根據 Wireframe 實現 UI
// 參考: docs/ux-design/wireframes/low-fidelity/03-agent-create.md
// 參考: docs/ux-design/design-system/component-library.md

// 3. 根據驗收標準實現邏輯
// 驗收標準 2: 必填欄位
// 驗收標準 3: 模型選擇
// 驗收標準 6: 表單驗證

// 4. 整合 API
// 參考: docs/technical-implementation/5-api-design/api-design-guidelines.md

// 5. 遵循 Coding Standards
// 參考: docs/technical-implementation/4-coding-standards/typescript-react-coding-standards.md
```

**Backend 實作流程**:
```csharp
// 1. 創建 Model
// 參考: docs/architecture/database-schema.md
// 遵循: docs/technical-implementation/6-database-standards/database-naming-conventions.md

// 2. 創建 DTO
// 參考: docs/technical-implementation/5-api-design/api-design-guidelines.md

// 3. 創建 Controller
// 遵循: docs/technical-implementation/4-coding-standards/csharp-coding-standards.md
// 遵循: docs/technical-implementation/5-api-design/api-design-guidelines.md

// 4. 整合 Semantic Kernel (如適用)
// 參考: poc-projects/poc1-sk-agents/

// 5. 錯誤處理與日誌
// 參考: docs/technical-implementation/4-coding-standards/csharp-coding-standards.md (Error Handling 章節)
```

**Step 4: 撰寫測試**

**Frontend 測試**:
```bash
# 創建測試文件
# apps/web/src/pages/agents/AgentCreate.test.tsx

pnpm --filter web run test:unit AgentCreate.test.tsx
```

**Backend 測試**:
```bash
# 創建測試文件
# apps/api-gateway.Tests/Controllers/AgentsControllerTests.cs

dotnet test --filter FullyQualifiedName~AgentsControllerTests
```

**驗證測試覆蓋率**:
```bash
# Frontend
pnpm --filter web run test:unit --coverage
# 確保覆蓋率 ≥80%

# Backend
dotnet test --collect:"XPlat Code Coverage"
# 確保覆蓋率 ≥80%
```

**Step 5: 本地驗證**

```bash
# 1. Lint 檢查
pnpm turbo run lint

# 2. TypeScript 類型檢查
pnpm turbo run typecheck

# 3. 執行所有測試
pnpm turbo run test

# 4. 本地 build
pnpm turbo run build

# 5. 手動測試
# - 啟動應用
# - 測試所有驗收標準
# - 測試 Edge cases
```

**Step 6: 提交代碼**

```bash
# 1. 查看變更
git status
git diff

# 2. Stage 變更
git add apps/web/src/pages/agents/AgentCreate.tsx
git add apps/web/src/pages/agents/AgentCreate.test.tsx
git add apps/api-gateway/Controllers/AgentsController.cs
# ... (其他文件)

# 3. 提交 (遵循 Conventional Commits)
git commit -m "feat(agent): implement agent creation UI and API

- Add AgentCreate component with form validation
- Add POST /api/v1/agents API endpoint
- Add unit tests for component and controller
- All 6 acceptance criteria met

Refs: US-1.1

🤖 Generated with [Claude Code](https://claude.com/claude-code)

Co-Authored-By: Claude <noreply@anthropic.com>"

# 4. Push 到 remote
git push origin feature/US-1.1-agent-create
```

**Step 7: 創建 Pull Request**

```bash
# 使用 GitHub CLI
gh pr create \
  --title "feat(agent): Implement Agent Creation (US 1.1)" \
  --body "$(cat <<'EOF'
## Summary
實現 US 1.1: 通過 Web UI 建立 Agent

## Changes
- ✅ 前端: AgentCreate 組件 (表單驗證, 模型選擇)
- ✅ 後端: POST /api/v1/agents API endpoint
- ✅ 測試: 單元測試覆蓋率 85%
- ✅ 所有 6 個驗收標準已滿足

## Acceptance Criteria
- [x] 使用者可以訪問 "建立 Agent" 頁面
- [x] 表單包含必填欄位: Agent 名稱, System Prompt, LLM 模型選擇
- [x] 模型選擇下拉列表包含: GPT-4, GPT-4o, GPT-4o-mini
- [x] 提交表單後,系統在後端建立 Agent 記錄
- [x] 成功建立後,重定向到 Agent 詳情頁面
- [x] 表單驗證: Agent 名稱不可為空, System Prompt 不可為空

## Testing
- ✅ Unit tests: 85% coverage
- ✅ Manual testing: All scenarios tested
- ✅ Cross-browser: Chrome, Firefox, Safari
- ✅ Responsive: Desktop, Tablet, Mobile

## Screenshots
[Attach screenshots]

## References
- User Story: docs/user-stories/epics/epic-01.md (US 1.1)
- Wireframe: docs/ux-design/wireframes/low-fidelity/03-agent-create.md
- API Design: docs/technical-implementation/5-api-design/api-design-guidelines.md

🤖 Generated with [Claude Code](https://claude.com/claude-code)
EOF
)" \
  --base develop
```

#### 下午結束 (6:00 PM)

```bash
# 1. 更新 Jira 任務狀態
# US-1.1: In Progress → Code Review

# 2. 通知 Reviewer
# 在 Slack #dev-code-review 頻道:
# "PR ready for review: feat(agent): Implement Agent Creation (US 1.1)
#  Link: https://github.com/org/repo/pull/123
#  Reviewers: @frontend-lead @backend-lead"

# 3. 關閉開發環境 (可選)
cd infra/docker-compose
./stop-dev.sh
```

---

### 2. Code Review 流程

**參考文檔**: `docs/technical-implementation/development-workflow.md` (Code Review Checklist)

#### 作為 Reviewer

**Step 1: 接收 Code Review 請求**

收到 Slack 通知或 GitHub PR 通知

**Step 2: 審查 Pull Request**

```bash
# 1. Checkout PR 分支
gh pr checkout 123

# 2. 啟動開發環境並測試
cd infra/docker-compose
./start-dev.sh

cd ../..
pnpm dev

# 3. 手動測試所有驗收標準
```

**Step 3: 使用 Code Review Checklist**

**Frontend Review Checklist**:
- [ ] **Coding Standards**: 符合 `typescript-react-coding-standards.md`
  - [ ] Component 使用 Functional Components + Hooks
  - [ ] Props interface 有明確定義
  - [ ] 命名遵循 camelCase/PascalCase
- [ ] **Wireframe 一致性**: UI 與 `docs/ux-design/wireframes/` 一致
- [ ] **Accessibility**: WCAG 2.1 AA 標準
  - [ ] 所有表單欄位有 `<label>`
  - [ ] 鍵盤導航支援
  - [ ] 色彩對比度 >4.5:1
- [ ] **Responsive Design**: Desktop/Tablet/Mobile 測試通過
- [ ] **Testing**: 單元測試覆蓋率 ≥80%
- [ ] **Performance**:
  - [ ] 無不必要的 re-renders
  - [ ] 使用 React.memo / useMemo (如適用)
- [ ] **Type Safety**: TypeScript strict mode 無錯誤
- [ ] **Error Handling**: 錯誤處理完整 (API errors, network errors)

**Backend Review Checklist**:
- [ ] **Coding Standards**: 符合 `csharp-coding-standards.md`
  - [ ] 命名遵循 PascalCase/camelCase
  - [ ] async/await 正確使用
  - [ ] LINQ 使用得當
- [ ] **API Design**: 符合 `api-design-guidelines.md`
  - [ ] RESTful conventions
  - [ ] Request/Response 格式標準
  - [ ] HTTP status codes 正確
  - [ ] Error responses 有 code, message, details
- [ ] **Database**: 符合 `database-naming-conventions.md`
  - [ ] 表名、列名使用 snake_case
  - [ ] 索引命名正確
  - [ ] Foreign keys 定義正確
- [ ] **Security**:
  - [ ] 無 SQL injection 風險
  - [ ] 輸入驗證完整
  - [ ] 敏感資訊不出現在日誌
  - [ ] 認證/授權檢查
- [ ] **Testing**: 單元測試覆蓋率 ≥80%
- [ ] **Performance**:
  - [ ] 無 N+1 query 問題
  - [ ] 適當使用 caching
  - [ ] Database 查詢優化

**Step 4: 提供回饋**

在 GitHub PR 中添加 comments:

```markdown
## Frontend Review

### ✅ Good
- Code structure is clean and follows React best practices
- All acceptance criteria are met
- Test coverage is 85%

### 🔧 Needs Changes
1. **Accessibility Issue** (Line 45):
   ```tsx
   // Missing label for input
   <input type="text" placeholder="Name" />

   // Should be:
   <Input label="Agent Name" placeholder="例如: 資料分析助手" />
   ```

2. **Performance Concern** (Line 78):
   ```tsx
   // Unnecessary re-render
   const models = modelOptions.map(...)

   // Should use useMemo:
   const models = useMemo(() => modelOptions.map(...), [modelOptions]);
   ```

### 📝 Suggestions (Optional)
- Consider adding loading skeleton for better UX

## Verdict
**Changes Requested** 🔧

Please address the 2 issues above, then I'll approve.
```

**Step 5: Approve 或 Request Changes**

```bash
# 如果所有檢查通過
gh pr review 123 --approve --body "LGTM! ✅ All checks passed."

# 如果需要修改
gh pr review 123 --request-changes --body "[見上方 comments]"
```

---

#### 作為 PR Author (收到 Review 回饋)

**Step 1: 閱讀回饋**

查看 GitHub PR comments

**Step 2: 修改代碼**

```bash
# 根據回饋修改代碼
code apps/web/src/pages/agents/AgentCreate.tsx

# 修改後提交
git add apps/web/src/pages/agents/AgentCreate.tsx
git commit -m "fix(agent): address code review feedback

- Add label for input (accessibility)
- Use useMemo to optimize performance
- Add loading skeleton

🤖 Generated with [Claude Code](https://claude.com/claude-code)"

git push origin feature/US-1.1-agent-create
```

**Step 3: 回覆 Reviewer**

在 GitHub PR comments 回覆:
```markdown
@reviewer Thanks for the feedback!

✅ Fixed:
1. Added label for input (commit abc123)
2. Used useMemo for optimization (commit abc123)

📸 Added loading skeleton as suggested (commit def456)

Ready for re-review.
```

**Step 4: 等待 Re-review 和 Approve**

---

### 3. Merge 和部署流程

**Step 1: PR Approved 後 Merge**

```bash
# Squash and merge (推薦)
gh pr merge 123 --squash --delete-branch
```

**Step 2: 自動觸發 CI/CD**

Merge 到 `develop` 後:
1. GitHub Actions `develop.yml` 自動觸發
2. 執行: Lint → Test → Build → Deploy to Dev
3. E2E tests 在 Dev 環境執行
4. 成功後在 Slack #deployments 通知

**Step 3: 驗證 Dev 環境**

```bash
# 訪問 Dev 環境
open https://dev-app.example.com

# 測試新功能
# 確認所有驗收標準在 Dev 環境通過
```

**Step 4: 更新 Jira 任務**

- US-1.1: Code Review → **Done** ✅
- 在 Jira 添加 comment:
  ```
  Merged to develop: PR #123
  Deployed to Dev: https://dev-app.example.com
  Verified: All acceptance criteria met ✅
  ```

---

### 4. Sprint Review 準備

**Sprint 最後一天 (Day 10)**

**上午**: 準備 Demo

```bash
# 1. 確保所有 Stories 標記為 Done
# 在 Jira Sprint Board 檢查

# 2. 準備 Demo 環境
# 使用 Staging 環境 (更穩定)
open https://staging-app.example.com

# 3. 準備 Demo 腳本
# 例如 Sprint 1 Demo:
```

**Demo 腳本範例 (Sprint 1)**:

```markdown
# Sprint 1 Demo Script

## Sprint Goal
✅ 建立 Agent 的完整生命週期管理能力

## Completed Stories (8/8)
- US 1.1: 通過 Web UI 建立 Agent ✅
- US 1.2: 配置 System Prompt 和參數 ✅
- US 1.3: Agent 列表與搜尋 ✅
- US 1.4: Agent 詳情查看 ✅
- US 1.5: Agent 編輯與更新 ✅
- US 1.6: Agent 刪除與封存 ✅
- US 2.1: Plugin 列表與安裝 ✅
- US 2.2: Plugin 配置 ✅

## Demo Flow (15 minutes)

### Part 1: Agent Creation (5 min)
1. 導航到 Agent 列表頁面
2. 點擊 "建立 Agent" 按鈕
3. 填寫表單:
   - 名稱: "Customer Support Assistant"
   - System Prompt: "You are a professional customer support assistant..."
   - 模型: GPT-4o
4. 提交表單
5. ✅ 展示: 重定向到 Agent 詳情頁面

### Part 2: Agent Management (5 min)
1. 返回 Agent 列表
2. 展示搜尋功能 (搜尋 "Customer")
3. 展示篩選功能 (Status: Active)
4. 點擊 Agent 查看詳情
5. 編輯 Agent (修改 System Prompt)
6. ✅ 展示: 更新成功

### Part 3: Plugin System (5 min)
1. 導航到 Plugin 管理頁面
2. 展示可用 Plugins 列表
3. 安裝 "Weather Plugin"
4. 配置 Plugin (API Key)
5. ✅ 展示: Plugin 安裝成功並可用

## Metrics
- **Velocity**: 26 Story Points completed
- **Test Coverage**:
  - Frontend: 85%
  - Backend: 83%
- **Performance**:
  - API P95 Latency: 280ms ✅ (目標 <300ms)
  - Agent Creation: <2s ✅

## Next Sprint Preview
- Sprint 2: Chat Interface & Conversation Memory
- Epic 5.1 + Epic 6
```

**下午**: Sprint Review Meeting (2:00-3:30 PM)

**參與者**: PO, SM, 開發團隊, Stakeholders

**流程**:
1. **Sprint Goal Recap** (SM, 2 min)
2. **Demo** (開發團隊, 15 min)
3. **Stakeholder Q&A** (10 min)
4. **Metrics Review** (SM, 3 min)
5. **Feedback Collection** (所有人, 10 min)
6. **Next Sprint Preview** (PO, 5 min)

---

### 5. Sprint Retrospective

**時間**: Sprint Review 後 (3:30-4:30 PM)

**參與者**: 開發團隊 only (不含 Stakeholders)

**流程**:

**Step 1: What Went Well?** (15 min)

團隊成員輪流分享:
```markdown
### What Went Well? ✅

**Frontend Dev 1**:
- Wireframes 非常清晰,前端實作順利
- shadcn/ui 組件庫大大提升開發效率
- Code Review 質量很好

**Backend Dev 1**:
- PoC 1 代碼參考價值高,節省了很多時間
- Database Schema 設計完善
- Team collaboration 很好

**DevOps**:
- Docker Compose 設置順利
- CI/CD Pipeline 運行穩定

**SM**:
- Sprint Planning 準確,沒有大幅度調整範圍
- Daily Standup 有效,阻礙及時解決
```

**Step 2: What Could Be Improved?** (15 min)

團隊成員指出問題:
```markdown
### What Could Be Improved? ⚠️

**Frontend Dev 2**:
- Testing Library 使用不熟悉,花了很多時間學習
- 有時候不確定是否需要等待 Code Review 還是繼續下一個任務

**Backend Dev 2**:
- Database migration 遇到問題 (未遵循 Naming Convention)
- 測試覆蓋率未達 80% (實際 72%)
- API 文檔更新不及時

**Frontend Dev 1**:
- 有些 User Story 的驗收標準不夠明確
- Wireframe 中有些互動細節沒有定義
```

**Step 3: Action Items** (20 min)

團隊討論並決定改進措施:
```markdown
### Action Items for Sprint 2 🎯

1. **Testing Coverage**
   - 目標: ≥80% (所有代碼)
   - 行動: 在 PR checklist 中增加覆蓋率檢查
   - 負責人: SM 更新 Checklist, 所有開發者執行

2. **Database Naming Convention**
   - 目標: 100% 遵循 snake_case 規範
   - 行動: 在 Code Review 時嚴格檢查
   - 負責人: Backend Lead

3. **API 文檔自動化**
   - 目標: API 變更時自動更新 Swagger
   - 行動: 配置 Swashbuckle XML comments
   - 負責人: Backend Dev 1

4. **Testing Library Training**
   - 目標: 全員熟悉 Testing Library 最佳實踐
   - 行動: 下週一舉辦 1 小時 workshop
   - 負責人: Frontend Lead

5. **驗收標準澄清流程**
   - 目標: Sprint Planning 時確保所有驗收標準清晰
   - 行動: Planning 時允許開發者提問和澄清
   - 負責人: PO + SM
```

**Step 4: 檢討 Action Items from Last Sprint** (10 min)

如果有上一個 Sprint 的 Action Items,檢討執行情況。

---

## 風險管理

**參考文檔**: `claudedocs/PO-VALIDATION-REPORT-FINAL.md` (Risk Assessment)

### 識別的風險

#### 🔴 High Risk

**Risk 1: ISSUE-008 - Workflow Editor 範圍決策未定**
- **Impact**: 阻塞開發計劃
- **Probability**: 已發生
- **Mitigation**: Week 0 必須決策 (選項 A 推薦)
- **Contingency**: 如無法決策,延後 1 週並重新評估

**Risk 2: ISSUE-011 - Code Interpreter 安全審計延遲**
- **Impact**: PoC 3 安全分數 90/100 vs 目標 98/100
- **Probability**: Medium (外包廠商延遲)
- **Mitigation**:
  - Week 0 提前聯繫廠商
  - 簽訂有 SLA 的合約
- **Contingency**:
  - 如果 Week 3 未完成,Sprint 3-4 僅使用當前 90 分配置
  - Phase 1 後補強至 98 分

**Risk 3: Azure OpenAI API Quota 不足**
- **Impact**: 開發和測試受阻
- **Probability**: Medium (高併發測試時)
- **Mitigation**:
  - Week 0 申請提高 quota
  - 使用 Rate Limiting 控制請求頻率
- **Contingency**:
  - 降級使用 GPT-4o-mini (開發階段)
  - 申請額外的 Azure OpenAI 資源

#### 🟡 Medium Risk

**Risk 4: Knowledge RAG 準確率未達 90% 目標**
- **Impact**: 核心差異化能力受影響
- **Probability**: Medium (技術挑戰)
- **Mitigation**:
  - Sprint 5 重點投入 RAG 優化
  - 使用 PoC 5 驗證的策略 (4 種檢索策略)
  - 必要時引入 Azure AI Search (更強大的向量搜尋)
- **Contingency**:
  - 如果 Sprint 5 未達標,Sprint 6 繼續優化
  - 降低目標至 85% (仍高於行業平均)

**Risk 5: 團隊成員技能差距**
- **Impact**: 某些任務執行緩慢
- **Probability**: Medium
- **Skill Gaps**:
  - Vue 3 經驗不足 (如果 Workflow Editor Phase 1)
  - Semantic Kernel 初次使用
  - Docker 安全配置經驗不足
- **Mitigation**:
  - Week 0-1: 技術培訓
  - Pair Programming
  - PoC 代碼作為學習材料
- **Contingency**:
  - 調整 Story Points 估算
  - 必要時外部顧問支援

**Risk 6: 第三方服務依賴**
- **Impact**: Azure OpenAI, Azure AI Search 服務中斷
- **Probability**: Low (Azure SLA 99.9%)
- **Mitigation**:
  - 監控 Azure Status Dashboard
  - 實作 Circuit Breaker pattern
  - Graceful degradation
- **Contingency**:
  - 暫時使用 Mock responses (開發階段)
  - 切換到備用 Azure region

#### 🟢 Low Risk

**Risk 7: 測試覆蓋率未達標**
- **Impact**: 代碼質量下降
- **Probability**: Low (有明確檢查流程)
- **Mitigation**:
  - PR Checklist 包含覆蓋率檢查
  - CI/CD Pipeline 自動檢查
- **Contingency**:
  - Sprint Retrospective 時加強要求

**Risk 8: UI/UX 與 Wireframe 不一致**
- **Impact**: 使用體驗受影響
- **Probability**: Low (Wireframe 已完整)
- **Mitigation**:
  - Code Review 檢查 Wireframe 一致性
  - Sprint Review Demo 時檢查
- **Contingency**:
  - 及時修正,不延後到下個 Sprint

---

### 風險監控

**每週風險檢查** (每週五, Sprint Planning 前):

```markdown
# Weekly Risk Review (Week X)

## High Risks Update

### Risk 1: ISSUE-008 決策
- Status: 🔴 Open → 🟢 Resolved (Week 0)
- Decision: 選項 A - 保持 Phase 2 ✅
- Impact: 阻塞已解除

### Risk 2: ISSUE-011 安全審計
- Status: 🟡 In Progress (Week 3)
- Progress: 已完成 Day 5/10
- Expected: 如期完成 ✅

### Risk 3: Azure OpenAI Quota
- Status: 🟢 Resolved (Week 1)
- Action: Quota 提升至 100K TPM
- Impact: 無阻塞

## New Risks Identified

### Risk 9: Database Performance (New)
- Impact: API latency 增加
- Probability: Medium
- Description: Agent 列表查詢在 1000+ Agents 時超過 1s
- Mitigation: 添加索引 (agents.user_id, agents.status)
- Owner: Backend Dev 1
- Deadline: Week X+1

## Risk Trend
🔴 High: 3 → 1 (改善 ✅)
🟡 Medium: 2 → 3 (新增 1)
🟢 Low: 2 → 2 (持平)
```

---

## 成功指標與里程碑

### MVP 成功指標

根據 `docs/architecture/performance-scalability-strategy.md` 和 `docs/brief.md`:

#### 1. 功能完整性 ✅

- [ ] **7 Epics** 全部完成 (42 User Stories)
- [ ] **4 大核心差異化能力** 全部實現:
  - [ ] Persona Framework (Epic 7)
  - [ ] Code Interpreter (Epic 3)
  - [ ] Text-to-SQL (Epic 8)
  - [ ] Knowledge RAG (Epic 5.2) - **準確率 ≥90%**

#### 2. 性能指標 ⚡

| 指標 | 目標 | 測量方式 |
|------|------|---------|
| **API Gateway P95 Latency** | <300ms | Application Insights |
| **Agent Execution P95 Latency** | <5s | Application Insights |
| **Code Interpreter P95 Latency** | <8s | Application Insights |
| **Knowledge RAG Accuracy** | ≥90% | 人工評估 + 自動測試 |
| **Frontend First Paint** | <1s | Lighthouse |
| **Frontend Interactive Time** | <3s | Lighthouse |
| **API Availability** | ≥99.5% | Uptime monitoring |

#### 3. 質量指標 🧪

| 指標 | 目標 | 現狀 |
|------|------|------|
| **Unit Test Coverage** | ≥80% | ⏳ TBD |
| **Integration Test Coverage** | ≥60% | ⏳ TBD |
| **E2E Test Coverage** | 核心流程 100% | ⏳ TBD |
| **Code Review Approval Rate** | 100% | ⏳ TBD |
| **Critical Bugs** | 0 | ⏳ TBD |
| **Security Vulnerabilities** | 0 (High/Critical) | ⏳ TBD |

#### 4. 用戶體驗指標 🎨

| 指標 | 目標 | 測量方式 |
|------|------|---------|
| **WCAG 2.1 AA Compliance** | 100% | axe DevTools |
| **Lighthouse Score** | >90 | Lighthouse CI |
| **Mobile Responsive** | 100% (375px-1920px) | Manual testing |
| **Browser Compatibility** | Chrome, Firefox, Safari, Edge | BrowserStack |

#### 5. 安全指標 🛡️

| 指標 | 目標 | 測量方式 |
|------|------|---------|
| **Code Interpreter Security Score** | ≥90/100 | 安全審計 |
| **OWASP Top 10 Vulnerabilities** | 0 | OWASP ZAP scan |
| **Authentication Coverage** | 100% APIs | Code Review |
| **RBAC Implementation** | 5 roles, 完整權限矩陣 | Manual testing |

---

### 里程碑追蹤

#### Phase 1: Planning & Design (已完成 ✅)

- [x] **M1.1**: Brief 完成 (2025-10-15)
- [x] **M1.2**: User Stories 完成 (2025-10-20)
- [x] **M1.3**: UX Design 完成 (2025-10-25)
- [x] **M1.4**: Architecture Design 完成 (2025-10-28)
- [x] **M1.5**: PoC 驗證完成 (2025-10-30)
- [x] **M1.6**: PO Validation 完成 (2025-11-03) ✅

#### Phase 2: Development Preparation (當前階段 ⏳)

- [ ] **M2.1**: Week 0 關鍵決策完成 (2025-11-08)
  - ISSUE-008 決策
  - Azure 資源申請
  - 安全審計廠商簽約
- [ ] **M2.2**: Gap Closure 完成 (2025-11-22)
  - 9 個 Issues 全部解決
  - 技術實施文檔 95% 完成
- [ ] **M2.3**: Sprint 0 完成 (2025-12-06)
  - Monorepo 架構設置完成
  - 開發環境全員 Ready
  - CI/CD Pipeline 運行

#### Phase 3: MVP Development (未來 4 個月 📅)

- [ ] **M3.1**: Sprint 1-2 完成 (2025-12-20)
  - Epic 1, 2, 5.1, 6 完成
  - Agent + Chat Interface 可用
- [ ] **M3.2**: Sprint 3-4 完成 (2026-01-17)
  - Epic 3, 7, 8 完成
  - 3 大差異化能力可用
- [ ] **M3.3**: Sprint 5-6 完成 (2026-02-14)
  - Epic 5.2 完成
  - Knowledge RAG 準確率 ≥90%
- [ ] **M3.4**: Sprint 7-8 完成 (2026-03-13)
  - Epic 9, 10 完成
  - 企業功能 + 監控完成
- [ ] **M3.5**: MVP 開發完成 (2026-03-13) 🎉

#### Phase 4: Alpha Testing (1 個月 📅)

- [ ] **M4.1**: Alpha Testing 完成 (2026-03-27)
  - 所有功能測試通過
  - 性能測試達標
  - 安全測試通過
- [ ] **M4.2**: Bug 修復完成 (2026-04-03)
  - P0/P1 bugs 全部修復
  - P2 bugs 80% 修復
- [ ] **M4.3**: 文檔完善完成 (2026-04-10)
  - 用戶文檔、開發者文檔、管理員文檔全部完成

#### Phase 5: Production Deployment (1 週 📅)

- [ ] **M5.1**: Staging 部署成功 (2026-04-12)
- [ ] **M5.2**: Production 部署成功 (2026-04-17) 🚀
- [ ] **M5.3**: Post-deployment 監控 (2026-04-24)
  - 7 天穩定運行
  - 無 Critical issues

**預計 MVP 上線日期**: **2026-04-17** (約 5.5 個月後)

---

## 附錄: 快速參考表

### A. 常見問題快速查找

| 問題 | 查閱文檔 | 章節 |
|------|---------|------|
| **這個功能為什麼要做?** | `docs/brief.md` | Problem Statement |
| **這個功能要做什麼?** | `docs/user-stories/epics/epic-XX.md` | User Stories |
| **這個畫面長什麼樣?** | `docs/ux-design/wireframes/low-fidelity/XX-*.md` | Wireframes |
| **這個組件用什麼顏色?** | `docs/ux-design/design-system/color-palette.md` | Color System |
| **這個 API 怎麼設計?** | `docs/technical-implementation/5-api-design/` | API Guidelines |
| **這個數據怎麼存?** | `docs/architecture/database-schema.md` | Database Schema |
| **這段代碼怎麼寫?** | `poc-projects/pocX-*/` | PoC Reference Code |
| **代碼規範是什麼?** | `docs/technical-implementation/4-coding-standards/` | Coding Standards |
| **測試要怎麼寫?** | `docs/technical-implementation/7-testing-strategy/` | Testing Strategy |
| **怎麼部署?** | `docs/technical-implementation/8-deployment-architecture/` | Deployment Guide |
| **CI/CD 怎麼配置?** | `docs/technical-implementation/8-deployment-architecture/cicd-pipeline.md` | CI/CD Pipeline |
| **遇到 Bug 怎麼辦?** | Slack #dev-help 或 Jira | - |

---

### B. 開發工具清單

#### IDE & 擴展

**Visual Studio Code** (必裝擴展):
```bash
# ESLint
code --install-extension dbaeumer.vscode-eslint

# Prettier
code --install-extension esbenp.prettier-vscode

# Tailwind CSS IntelliSense
code --install-extension bradlc.vscode-tailwindcss

# Vue Language Features (Volar)
code --install-extension Vue.volar

# C# Dev Kit
code --install-extension ms-dotnettools.csharp

# Docker
code --install-extension ms-azuretools.vscode-docker

# YAML
code --install-extension redhat.vscode-yaml

# GitLens (推薦)
code --install-extension eamodio.gitlens

# Error Lens (推薦)
code --install-extension usernamehw.errorlens
```

#### 命令行工具

```bash
# Node.js 版本管理
nvm (macOS/Linux) 或 nvm-windows (Windows)

# 套件管理器
pnpm ≥8.0

# .NET SDK
dotnet ≥8.0

# Docker
Docker Desktop ≥4.20

# Git CLI
gh (GitHub CLI)

# 資料庫工具
psql (PostgreSQL client)
redis-cli (Redis client)

# API 測試
curl 或 httpie
```

#### 瀏覽器工具

- **Chrome DevTools** (必須)
- **axe DevTools** (Accessibility testing)
- **React Developer Tools** (React debugging)
- **Vue.js devtools** (Vue debugging)
- **Redux DevTools** (State debugging)

#### 桌面應用

- **Postman** 或 **Insomnia** (API testing)
- **DBeaver** 或 **Postico** (Database GUI)
- **RedisInsight** (Redis GUI)
- **GitKraken** 或 **Sourcetree** (Git GUI, 可選)

---

### C. 文檔版本控制

| 文檔 | 版本 | 最後更新 | 負責人 |
|------|------|---------|--------|
| COMPLETE-DEVELOPMENT-PLAN.md | v1.0.0 | 2025-11-03 | SM + Architect |
| PO-VALIDATION-REPORT-FINAL.md | v1.0.0 | 2025-11-03 | PO |
| PROJECT-INITIALIZATION-DECISION.md | v1.0.0 | 2025-11-03 | Architect |
| DOCUMENT-USAGE-STRATEGY.md | v1.0.0 | 2025-11-03 | SM |
| DOCUMENT-SHARDING-REPORT.md | v1.0.0 | 2025-11-03 | SM |
| C# Coding Standards | v1.0.0 | TBD (Week 1) | Backend Lead |
| TypeScript Coding Standards | v1.0.0 | TBD (Week 1) | Frontend Lead |
| API Design Guidelines | v1.0.0 | TBD (Week 2) | Backend Architect |
| Development Workflow | v1.0.0 | TBD (Week 1) | SM |

**版本更新策略**:
- **Major (X.0.0)**: 重大變更、架構調整
- **Minor (x.Y.0)**: 新增章節、重要補充
- **Patch (x.y.Z)**: 小修正、錯字更正

---

### D. 團隊聯繫方式

**Slack 頻道**:
- `#dev-general` - 一般開發討論
- `#dev-frontend` - 前端專屬
- `#dev-backend` - 後端專屬
- `#dev-code-review` - Code Review 通知
- `#dev-help` - 求助頻道
- `#deployments` - 部署通知
- `#ci-cd` - CI/CD Pipeline 通知
- `#monitoring` - 系統監控告警

**會議時程**:
- **Daily Standup**: 每天 9:30 AM (15 分鐘)
- **Sprint Planning**: Sprint 第 1 天, 9:00 AM (4 小時)
- **Sprint Review**: Sprint 第 10 天, 2:00 PM (1.5 小時)
- **Sprint Retrospective**: Sprint Review 後, 3:30 PM (1 小時)
- **Backlog Refinement**: Sprint 中間 (Week 1, Day 5), 2:00 PM (2 小時)

---

### E. 緊急聯絡

**生產環境事故 (P0)**:
1. 在 Slack `#incidents` 頻道報告
2. 通知 On-call Engineer (輪班表在 Jira)
3. 創建 Jira Incident ticket
4. 啟動 Incident Response Playbook

**安全漏洞發現**:
1. **不要**在公開 Slack 頻道討論
2. 直接聯繫 Security Lead (私訊)
3. 創建 Private Jira Security ticket
4. 遵循 Responsible Disclosure Policy

---

## 總結

本開發計劃文檔提供了從 Week 0 到 MVP 完成的完整路線圖,涵蓋:

✅ **前置準備** (Week 0-3):
- 關鍵決策 (ISSUE-008)
- Gap Closure (9 個 Issues)
- Sprint 0 環境設置

✅ **MVP 開發** (Sprint 1-8, 16 週):
- 7 個 Epics, 42 個 User Stories
- 3 大差異化能力 (Persona, Code Interpreter, Text-to-SQL, Knowledge RAG)
- 企業功能 (RBAC, Multi-Tenant, Monitoring)

✅ **Alpha Testing** (Week 21-24):
- 功能測試、性能測試、安全測試
- Bug 修復與優化
- 文檔完善

✅ **開發工作流程**:
- 每日開發流程
- Code Review 流程
- Sprint Review/Retrospective
- 風險管理

✅ **成功指標**:
- 功能完整性 (7 Epics)
- 性能指標 (P95 latency, 準確率)
- 質量指標 (測試覆蓋率, 安全)
- 里程碑追蹤

**下一步行動**: Week 0 立即執行 (關鍵決策 + Gap Closure)

---

**文檔版本**: v1.0.0
**創建日期**: 2025-11-03
**負責人**: Scrum Master + Architect
**審查者**: Product Owner, 技術團隊

**文檔維護**:
- 每個 Sprint 結束後更新進度
- 重大決策或範圍變更後更新
- 版本號遞增

**相關文檔**:
- `docs/BMAD-STAGE-ANALYSIS.md` - BMad Method 階段分析
- `claudedocs/PO-VALIDATION-REPORT-FINAL.md` - PO 驗證報告
- `docs/PROJECT-INITIALIZATION-DECISION.md` - 專案初始化決策
- `docs/DOCUMENT-USAGE-STRATEGY.md` - 文檔使用策略
- `docs/user-stories/mvp-planning.md` - MVP 範圍定義
- `docs/architecture/Architecture-Design-Document.md` - 系統架構
- `docs/technical-implementation/` - 技術實施文檔 (12 個階段)

---

**🎯 祝開發順利!讓我們一起打造一個優秀的 AI Workflow Platform!** 🚀
