# 文檔重組與開發管理體系建立計劃

**版本**: 1.0.0
**日期**: 2025-11-07
**狀態**: 🚧 規劃中
**優先級**: 🔴 Critical - 必須在繼續開發前完成

---

## 📌 目的

建立清晰、有系統的文檔組織結構和開發計劃管理體系，確保：
1. 文檔角色清晰分工
2. 開發計劃完整可追蹤
3. 規劃與實際執行對比
4. 變更記錄和決策記錄完整

---

## 🎯 核心問題

### 問題 1: 文檔角色混亂

**當前狀況**:
- `/docs` (86+ 文件): BMAD Method 規劃文檔 + 部分開發計劃
- `/claudedocs` (31 文件): Sprint執行記錄 + AI助手指令 + 狀態報告

**問題**:
- 兩個文件夾職責不清
- 存在重複內容 (如: `COMPLETE-DEVELOPMENT-PLAN.md` 在兩處都有)
- 開發人員不知道該參考哪個

### 問題 2: 開發計劃分散

**當前開發計劃文檔**:
```
/docs:
- user-stories/sprints/sprint-allocation.md (1222 行) - 18 Sprint 詳細分配
- user-stories/sprints/sprint-backlog.md (718 行) - Sprint 總覽
- COMPLETE-DEVELOPMENT-PLAN.md (100 行) - 高層計劃

/claudedocs:
- SPRINT-1-2-ROADMAP.md (415 行) - Sprint 1-2 路線圖
- SPRINT-2-KICKOFF.md (881 行) - Sprint 2 啟動會議
- PROJECT-STATUS-REPORT.md (2314 行) - 項目狀態報告
```

**問題**:
- 信息分散在多個文檔
- 沒有統一的開發日程表 (Development Schedule)
- 沒有進度追蹤機制 (Progress Tracking)
- 沒有變更記錄 (Change Log)

### 問題 3: 規劃與執行脫節

**規劃 vs 實際**:
- 📋 **規劃**: 18 個 Sprint, 43 個 User Stories (在 `/docs`)
- ✅ **實際**: 完成了 Sprint 1 (US 1.1-1.3) + Sprint 2 部分 (US 1.4)
- ❌ **問題**:
  - 沒有文檔記錄實際執行與計劃的差異
  - 沒有文檔說明為什麼實際與計劃不同
  - 沒有更新後的實際開發路線圖

---

## 🏗️ 解決方案：雙層文檔體系

### 方案概述

建立清晰的 **雙層文檔體系**：

```
Layer 1: /docs (Reference Layer - 參考層)
└── BMAD Method 規劃文檔
    └── 靜態、權威、不常變更
    └── 角色：規劃藍圖和設計參考

Layer 2: /claudedocs (Execution Layer - 執行層)
└── 開發執行文檔
    └── 動態、實時、頻繁更新
    └── 角色：執行計劃、進度追蹤、變更記錄
```

---

## 📁 文檔重組方案

### /docs - Reference Layer (參考層)

**角色定位**: **靜態參考文檔 - 項目規劃藍圖**

**職責**:
1. BMAD Method 規劃文檔 (Brief, Architecture, User Stories, UX Design)
2. 技術決策記錄 (ADRs)
3. 系統架構設計
4. PoC 驗證報告
5. **原始 Sprint 分配計劃** (作為基準參考)

**特點**:
- ✅ 文檔穩定，不頻繁變更
- ✅ 作為項目規劃的「藍圖」和「設計文檔」
- ✅ 新成員培訓和項目理解的主要來源
- ❌ **不記錄實際執行狀態和變更**

**目錄結構** (保持現有結構):
```
docs/
├── README.md                          # 文檔總覽
├── brief-*.md                         # Project Brief (已切分)
├── architecture/                      # 系統架構
│   ├── Architecture-Design-Document.md
│   ├── ADR-*.md                       # 架構決策記錄
│   └── ...
├── user-stories/                      # User Stories 規劃
│   ├── modules/                       # 10 個模塊
│   ├── sprints/                       # Sprint 規劃 (基準)
│   │   ├── sprint-backlog.md         # Sprint 總覽
│   │   └── sprint-allocation.md      # 詳細分配 (原始計劃)
│   └── templates/
├── ux-design/                         # UX 設計
├── project-management/                # 項目管理計劃
└── technical-implementation/          # 技術實施文檔
    ├── 1-poc-validation/              # PoC 驗證
    ├── 2-tech-decisions/              # 技術決策
    ├── 3-system-design/               # 系統設計
    ├── 4-coding-standards/            # 編碼標準
    ├── 5-api-design/                  # API 設計
    ├── 6-database-design/             # 資料庫設計
    ├── 7-testing-strategy/            # 測試策略
    └── ...
```

---

### /claudedocs - Execution Layer (執行層)

**角色定位**: **動態執行文檔 - 開發管理中樞**

**職責**:
1. **開發計劃管理** (Development Planning)
2. **進度追蹤** (Progress Tracking)
3. **Sprint 執行記錄** (Sprint Execution)
4. **變更記錄** (Change Log)
5. **狀態報告** (Status Reports)
6. **決策記錄** (Decision Log)
7. **AI 助手指令** (AI Instructions)

**特點**:
- ✅ 文檔動態更新，反映實時狀態
- ✅ 記錄實際執行與計劃的差異
- ✅ 記錄所有變更和決策
- ✅ 作為開發團隊的「作戰地圖」

**重組後目錄結構**:
```
claudedocs/
├── README.md                          # 🆕 執行層文檔總覽
│
├── 1-planning/                        # 🆕 開發計劃
│   ├── MASTER-DEVELOPMENT-SCHEDULE.md # 🆕 主開發日程表
│   ├── SPRINT-ROADMAP.md              # 🆕 實際 Sprint 路線圖
│   ├── MILESTONE-TRACKER.md           # 🆕 里程碑追蹤
│   └── RISK-REGISTER.md               # 🆕 風險登記冊
│
├── 2-sprints/                         # 🆕 Sprint 執行記錄
│   ├── sprint-0/                      # Sprint 0 (準備階段)
│   │   ├── KICKOFF.md
│   │   ├── PLAN.md
│   │   └── RETROSPECTIVE.md
│   ├── sprint-1/                      # Sprint 1
│   │   ├── KICKOFF.md
│   │   ├── DAILY-STANDUP.md
│   │   ├── PROGRESS.md
│   │   └── RETROSPECTIVE.md
│   ├── sprint-2/                      # Sprint 2 (當前)
│   │   ├── KICKOFF.md                 # 已存在
│   │   ├── PROGRESS.md                # 🆕
│   │   └── sessions/                  # Session 記錄
│   └── ...
│
├── 3-progress/                        # 🆕 進度追蹤
│   ├── USER-STORY-STATUS.md           # 🆕 User Story 狀態追蹤
│   ├── FEATURE-COMPLETION-MATRIX.md   # 🆕 功能完成矩陣
│   ├── TEST-COVERAGE-REPORT.md        # 🆕 測試覆蓋率報告
│   └── VELOCITY-TRACKER.md            # 🆕 速度追蹤
│
├── 4-changes/                         # 🆕 變更管理
│   ├── CHANGE-LOG.md                  # 🆕 變更日誌
│   ├── SCOPE-CHANGE-REQUESTS.md       # 🆕 範圍變更請求
│   └── TECHNICAL-DECISIONS.md         # 🆕 技術決策記錄
│
├── 5-status/                          # 狀態報告 (保留)
│   ├── PROJECT-STATUS-REPORT.md       # 當前主報告
│   ├── WEEKLY-STATUS/                 # 🆕 每週狀態
│   └── MILESTONE-REPORTS/             # 🆕 里程碑報告
│
├── 6-ai-assistant/                    # 🆕 AI 助手專用
│   ├── AI-ASSISTANT-INSTRUCTIONS.md   # AI 指令 (保留)
│   ├── CHANGELOG-AI-INSTRUCTIONS.md   # 變更記錄 (保留)
│   └── AI-TOOLS-AND-TEMPLATES/        # 🆕 AI 工具模板
│
└── 7-archive/                         # 🆕 歸檔文檔
    ├── preparation-phase/             # 準備階段歸檔
    ├── sprint-0/                      # Sprint 0 歸檔
    └── legacy-docs/                   # 舊文檔歸檔
```

---

## 🆕 核心新文檔設計

### 1. MASTER-DEVELOPMENT-SCHEDULE.md

**目的**: 統一的主開發日程表

**內容結構**:
```markdown
# 主開發日程表

## 當前狀態
- 當前 Sprint: Sprint 2
- 當前週次: Week 2
- 整體進度: X%
- 預計完成日期: YYYY-MM-DD

## 完整日程表
| Sprint | Week | Dates | User Stories | Status | Planned | Actual | Variance |
|--------|------|-------|--------------|--------|---------|--------|----------|
| Sprint 0 | - | 2025-11-01~11-10 | Preparation | ✅ 100% | 10d | 10d | 0d |
| Sprint 1 | 1-3 | 2025-11-11~12-01 | US 1.1-1.3 | ✅ 100% | 21d | 18d | -3d |
| Sprint 2 | 4-6 | 2025-12-02~12-22 | US 1.4, 2.1, 6.1 | 🔄 40% | 21d | TBD | TBD |
| ... | ... | ... | ... | ... | ... | ... | ... |

## 關鍵里程碑
| Milestone | Target Date | Actual Date | Status | Dependencies |
|-----------|-------------|-------------|--------|--------------|
| PoC 完成 | 2025-11-05 | 2025-11-05 | ✅ | - |
| Sprint 1 完成 | 2025-12-01 | 2025-11-28 | ✅ | PoC |
| US 1.4 完成 | 2025-12-15 | 2025-12-07 | ✅ | Sprint 1 |
| ... | ... | ... | ... | ... |

## 變更歷史
- 2025-11-07: US 1.4 實際包含 4 個 Phase 而非原計劃的單一功能
```

---

### 2. USER-STORY-STATUS.md

**目的**: User Story 實時狀態追蹤

**內容結構**:
```markdown
# User Story 狀態追蹤

## 總覽
- 總 User Stories: 43
- 已完成: 4 (9%)
- 進行中: 0
- 未開始: 39

## 詳細狀態

### Sprint 1 (已完成)
| US | Title | Planned | Actual | Status | Variance | Notes |
|----|-------|---------|--------|--------|----------|-------|
| US 1.1 | Agent CRUD | 5 SP | 5 SP | ✅ 100% | 0 SP | 按計劃完成 |
| US 1.2 | .NET SDK | 5 SP | 5 SP | ✅ 100% | 0 SP | 按計劃完成 |
| US 1.3 | Agent 配置 | 3 SP | 3 SP | ✅ 100% | 0 SP | 按計劃完成 |

### Sprint 2 (進行中)
| US | Title | Planned | Actual | Status | Variance | Notes |
|----|-------|---------|--------|--------|----------|-------|
| US 1.4 | Agent 執行 | 5 SP | 13 SP | ✅ 100% | +8 SP | 擴展為 4 個 Phase |
| US 2.1 | Plugin 系統 | 5 SP | - | ⏳ 0% | TBD | 下一個開發項 |
| US 6.1 | Chat 基礎 | 3 SP | - | ⏳ 0% | TBD | 待開始 |

## 計劃 vs 實際對比圖表
[圖表: Planned vs Actual Story Points by Sprint]

## 風險與問題
- ⚠️ US 1.4 範圍擴大: 從 5 SP 增加到 13 SP
  - 影響: Sprint 2 其他 US 可能延遲
  - 緩解: 重新評估 US 2.1 和 6.1 的時程
```

---

### 3. CHANGE-LOG.md

**目的**: 記錄所有計劃變更和決策

**內容結構**:
```markdown
# 變更日誌

## 2025-12-07: US 1.4 範圍擴展
**變更類型**: Scope Change
**影響**: Sprint 2

**變更內容**:
- **原計劃**: US 1.4 為單一功能 (Agent 執行與監控, 5 SP)
- **實際執行**: 擴展為 4 個 Phase:
  - Phase 1: 基礎執行引擎
  - Phase 2: 執行歷史追蹤
  - Phase 3: 效能指標追蹤
  - Phase 4: 即時監控與匯出
- **Story Points**: 5 SP → 13 SP (+8 SP)

**原因**:
1. 實際開發發現需要更完整的監控和歷史記錄功能
2. SignalR WebSocket 集成比預期複雜
3. CSV/JSON 匯出功能為 PO 新需求

**影響分析**:
- ✅ 正面: 功能更完整,監控更全面
- ⚠️ 負面: Sprint 2 其他 US (2.1, 6.1) 可能延遲

**決策**:
- 優先完成 US 1.4 全部 4 個 Phase
- 重新評估 US 2.1 (Plugin 系統) 的優先級
- 考慮將 US 6.1 (Chat 基礎) 移至 Sprint 3

**審批**: PO Approved, 2025-12-07

---

## 2025-11-28: Sprint 1 提前完成
**變更類型**: Schedule Change
**影響**: Sprint 1

**變更內容**:
- **計劃**: 21 天 (3 週)
- **實際**: 18 天
- **提前**: 3 天

**原因**:
1. 團隊效率高於預期
2. 集成測試比預期順利
3. 技術決策明確,減少返工

**決策**:
- Sprint 2 提前開始 (2025-11-29 而非 2025-12-02)
```

---

### 4. SPRINT-PROGRESS.md (每個 Sprint 目錄下)

**目的**: 每個 Sprint 的實時進度追蹤

**內容結構**:
```markdown
# Sprint 2 進度追蹤

## Sprint 基本信息
- Sprint: Sprint 2
- 時間: 2025-12-02 ~ 2025-12-22 (3 週)
- User Stories: US 1.4, 2.1, 6.1
- Story Points: 13 (計劃)

## 當前狀態 (2025-12-07)
- 進度: 40% (US 1.4 完成)
- 剩餘天數: 15 天
- 風險: 🟡 中等 (範圍變更)

## User Story 進度
| US | SP | Status | Start | End | Duration | Notes |
|----|-----|--------|-------|-----|----------|-------|
| US 1.4 | 13 | ✅ 100% | 11-29 | 12-07 | 9d | 4 Phase 完成 |
| US 2.1 | 5 | ⏳ 0% | - | - | - | 待開始 |
| US 6.1 | 3 | ⏳ 0% | - | - | - | 待開始 |

## 每日進度 (Burndown)
| Date | Remaining SP | Completed Today | Notes |
|------|--------------|-----------------|-------|
| 12-02 | 13 | 0 | Sprint 開始 |
| 12-03 | 13 | 0 | US 1.4 Phase 1 開始 |
| 12-04 | 10 | 3 | Phase 1-2 完成 |
| 12-05 | 7 | 3 | Phase 3 完成 |
| 12-06 | 3 | 4 | Phase 4 進行中 |
| 12-07 | 0 | 3 | Phase 4 完成, US 1.4 Done |

## 風險與問題
- ⚠️ US 1.4 範圍擴大 (+8 SP)
- ⚠️ US 2.1 和 6.1 可能需要延遲或調整

## 決策記錄
- 2025-12-07: 優先完成 US 1.4,重新評估 2.1/6.1
```

---

## 🔄 文檔遷移計劃

### Phase 1: 歸檔與清理 (1 天)

1. **創建歸檔目錄**:
   ```
   claudedocs/7-archive/
   ├── preparation-phase/
   ├── sprint-0/
   └── legacy-docs/
   ```

2. **歸檔準備階段文檔**:
   - `PREPARATION-PHASE-COMPLETION-SUMMARY.md` → 7-archive/preparation-phase/
   - `GAP-CLOSURE-VALIDATION-REPORT.md` → 7-archive/preparation-phase/
   - `DEVELOPMENT-PLAN-READINESS-VERIFICATION.md` → 7-archive/preparation-phase/
   - `CONSISTENCY-ANALYSIS-REPORT.md` → 7-archive/preparation-phase/
   - `PO-VALIDATION-REPORT*.md` → 7-archive/preparation-phase/

3. **歸檔 Sprint 0 文檔**:
   - `SPRINT-0-*.md` → 7-archive/sprint-0/

4. **歸檔決策文檔**:
   - `ISSUE-*.md` → 7-archive/legacy-docs/
   - `WORKFLOW-EDITOR-*.md` → 7-archive/legacy-docs/

### Phase 2: 創建新文檔結構 (2 天)

1. **創建目錄結構**:
   ```bash
   mkdir 1-planning 2-sprints 3-progress 4-changes 5-status 6-ai-assistant
   ```

2. **創建核心文檔**:
   - `MASTER-DEVELOPMENT-SCHEDULE.md`
   - `SPRINT-ROADMAP.md`
   - `USER-STORY-STATUS.md`
   - `CHANGE-LOG.md`
   - `FEATURE-COMPLETION-MATRIX.md`

3. **重組 Sprint 文檔**:
   ```
   2-sprints/
   ├── sprint-1/
   │   ├── RETROSPECTIVE.md (from SPRINT-1-RETROSPECTIVE.md)
   │   └── sessions/ (from SPRINT-1-SESSION-*.md)
   ├── sprint-2/
   │   ├── KICKOFF.md (from SPRINT-2-KICKOFF.md)
   │   ├── PROGRESS.md (new)
   │   └── sessions/
   ```

### Phase 3: 數據遷移與填充 (2 天)

1. **填充 MASTER-DEVELOPMENT-SCHEDULE.md**:
   - 從 `sprint-allocation.md` 提取計劃數據
   - 從 `PROJECT-STATUS-REPORT.md` 提取實際數據
   - 計算差異和變更

2. **填充 USER-STORY-STATUS.md**:
   - 從 `sprint-allocation.md` 提取所有 US
   - 從各 Sprint 完成報告提取實際狀態
   - 生成對比表

3. **填充 CHANGE-LOG.md**:
   - 從 `PROJECT-STATUS-REPORT.md` 提取歷史變更
   - 從 Session 記錄提取決策
   - 按時間順序組織

### Phase 4: 更新引用和索引 (1 天)

1. **更新 README.md**:
   - 說明新文檔結構
   - 提供快速導航
   - 更新文檔使用指南

2. **更新 AI 助手指令**:
   - 更新 `AI-ASSISTANT-INSTRUCTIONS.md`
   - 添加新文檔的更新規則
   - 定義文檔維護流程

3. **驗證引用**:
   - 檢查所有文檔間的引用
   - 更新過期鏈接
   - 確保文檔可導航

---

## 📅 執行時間表

**總時長**: 6 工作天

| 階段 | 任務 | 時間 | 負責人 |
|------|------|------|--------|
| Phase 1 | 歸檔與清理 | 1 天 | AI Assistant |
| Phase 2 | 創建新結構 | 2 天 | AI Assistant |
| Phase 3 | 數據遷移 | 2 天 | AI Assistant + PO Review |
| Phase 4 | 更新引用 | 1 天 | AI Assistant |

**開始日期**: 2025-12-08
**完成日期**: 2025-12-13

---

## ✅ 驗收標準

1. **文檔角色清晰**:
   - [ ] `/docs` 職責明確: 靜態參考文檔
   - [ ] `/claudedocs` 職責明確: 動態執行文檔
   - [ ] 無重複內容

2. **開發計劃完整**:
   - [ ] MASTER-DEVELOPMENT-SCHEDULE.md 包含所有 Sprint
   - [ ] USER-STORY-STATUS.md 包含所有 43 個 US
   - [ ] CHANGE-LOG.md 記錄所有歷史變更

3. **進度可追蹤**:
   - [ ] 每個 Sprint 有 PROGRESS.md
   - [ ] 計劃 vs 實際對比清晰
   - [ ] 變更原因有記錄

4. **文檔可導航**:
   - [ ] README.md 提供完整索引
   - [ ] 所有文檔間引用正確
   - [ ] AI 助手指令已更新

5. **PO 驗收**:
   - [ ] PO 確認文檔結構合理
   - [ ] PO 確認進度追蹤有效
   - [ ] PO 確認可持續維護

---

## 🎯 成功指標

1. **文檔使用效率**: 開發人員可在 5 分鐘內找到所需文檔
2. **進度透明度**: PO 可隨時了解當前進度和變更
3. **決策可追溯**: 所有決策都有記錄和原因
4. **持續維護**: AI 助手可自動更新大部分文檔

---

## 📝 後續維護流程

### 每日更新:
- Sprint PROGRESS.md (Burndown 數據)
- USER-STORY-STATUS.md (如有狀態變更)

### 每週更新:
- MASTER-DEVELOPMENT-SCHEDULE.md (里程碑檢查)
- WEEKLY-STATUS 報告

### Sprint 結束:
- Sprint RETROSPECTIVE.md
- CHANGE-LOG.md (總結變更)
- USER-STORY-STATUS.md (更新完成狀態)

### 里程碑完成:
- MILESTONE-REPORTS/ (生成報告)
- FEATURE-COMPLETION-MATRIX.md (更新矩陣)

---

**批准**: 待 PO 審查
**開始執行**: 待批准後立即開始
