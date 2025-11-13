# Claude Docs - AI 開發助手文檔中心

**版本**: 2.0.0
**最後更新**: 2025-12-08
**狀態**: ✅ 全新結構上線

---

## 📚 文檔架構總覽

本文檔中心採用**雙層架構設計**:
- **Reference Layer** (`/docs`): 靜態規劃參考文檔 (BMAD Method, 架構設計, 原始需求)
- **Execution Layer** (`/claudedocs`): 動態開發執行追蹤 (實際進度, 變更記錄, 狀態報告)

---

## 🗂️ 文檔結構 (7 個核心文件夾)

### 1️⃣ `1-planning/` - 總體規劃

**用途**: 統一的開發計劃和 Roadmap

| 文件 | 說明 | 更新頻率 |
|-----|------|---------|
| `MASTER-DEVELOPMENT-SCHEDULE.md` | 18 個 Sprint 統一日程表 (計劃 vs 實際) | 每 Sprint 更新 |
| `SPRINT-ROADMAP.md` | 可視化 Roadmap 和里程碑 | 每月更新 |

**關鍵內容**:
- 所有 18 個 Sprint 計劃日期和實際日期對比
- 10 個主要里程碑 (M0-M10) 追蹤
- 進度統計 (按優先級, 按模組, 按 Sprint)
- 差異分析 (提前/延遲天數統計)

---

### 2️⃣ `2-sprints/` - Sprint 詳細文檔

**用途**: 每個 Sprint 的詳細規劃、執行追蹤和回顧

**結構** (每個 Sprint 包含 7 個標準文件):
```
2-sprints/
├── sprint-1/
│   ├── SPRINT-1-OVERVIEW.md        # Sprint 概覽與高層指標
│   ├── SPRINT-1-PLAN.md            # 詳細執行計劃與技術實施
│   ├── SPRINT-1-CONTEXT.md         # 上下文參考與文檔引用
│   ├── SPRINT-1-CHECKLIST.md       # 任務檢查清單與進度追蹤
│   ├── SPRINT-1-DEV-LOG.md         # 開發日誌與問題記錄
│   ├── SPRINT-1-ISSUES.md          # 問題追蹤與解決方案
│   └── SPRINT-1-RETROSPECTIVE.md   # Sprint 回顧與經驗總結
├── sprint-2/
│   ├── SPRINT-2-1-OVERVIEW.md        # Sprint 2 概覽
│   ├── SPRINT-2-2-PLAN.md            # Sprint 2 執行計劃
│   ├── SPRINT-2-3-CONTEXT.md         # Sprint 2 上下文
│   ├── SPRINT-2-4-CHECKLIST.md       # Sprint 2 任務清單
│   ├── SPRINT-2-5-DEV-LOG.md         # Sprint 2 開發日誌
│   ├── SPRINT-2-6-ISSUES.md          # Sprint 2 問題追蹤
│   └── SPRINT-2-7-RETROSPECTIVE.md   # Sprint 2 回顧 (完成後)
...
├── sprint-21/
│   └── [7 個標準文件]
```

**7 個標準文件說明**:

1. **OVERVIEW.md** - Sprint 概覽
   - Sprint 目標和關鍵交付物
   - User Stories 計劃 (優先級、Story Points、狀態)
   - 成功指標 (Definition of Done)
   - 風險與依賴關係
   - 進度統計與燃盡圖數據

2. **PLAN.md** - 詳細執行計劃
   - 本 Sprint 要建立什麼 (What to Build)
   - 技術實施方案 (How to Build)
   - API 規格、UI 設計、Database Schema
   - 開發順序與階段劃分
   - 編碼規範與參考文檔

3. **CONTEXT.md** - 上下文參考
   - 精簡引用 /docs 中的關鍵內容
   - 避免 AI 助手在 180+ 文件中搜索
   - 包含文檔位置、行號、快速參考

4. **CHECKLIST.md** - 任務檢查清單
   - 可標記進度的詳細任務列表
   - 分類追蹤 (Backend、Frontend、Testing、Documentation)
   - 完成度統計 (百分比)
   - 實時更新狀態

5. **DEV-LOG.md** - 開發日誌
   - 每日開發記錄
   - 遇到的問題與解決方案
   - 學到的知識與經驗
   - 進度追蹤與明日計劃

6. **ISSUES.md** - 問題追蹤
   - Open Issues (進行中的問題)
   - Resolved Issues (已解決的問題)
   - 問題嚴重性、重現步驟、根本原因分析
   - 解決方案與相關 Commit

7. **RETROSPECTIVE.md** - Sprint 回顧
   - Sprint 指標統計
   - 做得好的地方 (Keep)
   - 需要改進的地方 (Improve)
   - 學到的知識 (Insights)
   - 下個 Sprint 的行動計劃

**狀態標記**:
- ✅ 已完成: Sprint 1 (7 個文件完整)
- 🔄 進行中: Sprint 2 (準備創建)
- ⏳ 未開始: Sprint 3-21

**模板位置**: `/claudedocs/templates/` 包含所有 7 個文件的標準模板

---

### 3️⃣ `3-progress/` - 進度追蹤

**用途**: User Story 和功能完成度追蹤

| 文件 | 說明 | 更新頻率 |
|-----|------|---------|
| `USER-STORY-STATUS.md` | 43 個 User Story 詳細狀態 | 實時更新 |
| `FEATURE-COMPLETION-MATRIX.md` | 10 個模組完成度矩陣 | 每週更新 |

**USER-STORY-STATUS.md 內容**:
- 每個 User Story 的計劃 vs 實際 Story Points
- 計劃 vs 實際時間對比
- 範圍變更記錄 (例如 US 1.4: 5 SP → 13 SP)
- 依賴關係追蹤
- 風險和阻塞項

**FEATURE-COMPLETION-MATRIX.md 內容**:
- 10 個核心模組完成度百分比
- 功能點細分和狀態
- 5 個核心差異化功能追蹤 (⭐ 標記)
- 依賴關係圖
- 風險追蹤

---

### 4️⃣ `4-changes/` - 變更記錄

**用途**: 正式的變更控制和決策記錄

| 文件 | 說明 | 更新頻率 |
|-----|------|---------|
| `CHANGE-LOG.md` | 所有正式變更記錄 | 變更發生時 |
| `SCOPE-CHANGE-REQUESTS.md` | 範圍變更請求追蹤 | 變更發生時 |

**CHANGE-LOG.md 內容**:
- 變更編號 (CHANGE-001, CHANGE-002...)
- 變更類型 (範圍/需求/技術/資源)
- 影響等級 (低/中/高/關鍵)
- 詳細影響分析 (時間/Story Points/資源)
- 審批流程 (Tech Lead, PO, Scrum Master)
- 經驗教訓和改進措施

**示例**:
- **CHANGE-001**: US 1.4 範圍擴展 (5 SP → 13 SP, +6 days, 4 個 Phase)

---

### 5️⃣ `5-status/` - 狀態報告

**用途**: 當前項目狀態快照

| 文件 | 說明 | 更新頻率 |
|-----|------|---------|
| `PROJECT-STATUS-REPORT.md` | 項目總體狀態報告 | 每週更新 |

**PROJECT-STATUS-REPORT.md 內容**:
- 當前 Sprint 進度
- 最近完成的 User Story
- 測試結果統計 (單元測試/集成測試)
- 文件變更記錄
- API 統計
- 下一步計劃
- 風險和阻塞項

---

### 6️⃣ `6-ai-assistant/` - AI 助手指引

**用途**: AI 開發助手的維護指引和最佳實踐

| 文件 | 說明 | 更新頻率 |
|-----|------|---------|
| `AI-ASSISTANT-INSTRUCTIONS.md` | AI 助手維護文檔的指引 | 結構變更時 |

**AI-ASSISTANT-INSTRUCTIONS.md 內容**:
- 如何維護和更新各類文檔
- 文檔更新優先級和頻率
- 文檔質量標準
- 變更控制流程
- 跨文檔一致性檢查

---

### 7️⃣ `7-archive/` - 歷史歸檔

**用途**: 歸檔歷史文檔,保持主目錄清潔

**歸檔內容**:
- 舊的 Sprint 文檔 (SPRINT-0, SPRINT-1-2-ROADMAP)
- 完成的 User Story 總結 (US-1.4-Phase3-Summary)
- 歷史驗證報告 (PO-VALIDATION-REPORT)
- 舊的規劃文檔 (DOCUMENTATION-REORGANIZATION-PLAN)
- 決策記錄 (ISSUE-001, ISSUE-008)
- AI 助手指引測試文檔

**歸檔原則**:
- 不再需要頻繁訪問的文檔
- 已被新文檔取代的舊文檔
- 歷史參考價值的文檔
- 保留以供審計和回顧

---

## 📊 文檔使用指南

### 🔍 快速查找

**我想查看...**

| 需求 | 查看文件 |
|-----|---------|
| 整體開發計劃和進度 | `1-planning/MASTER-DEVELOPMENT-SCHEDULE.md` |
| 某個 Sprint 詳細規劃 | `2-sprints/sprint-X/SPRINT-X-OVERVIEW.md` |
| 某個 User Story 狀態 | `3-progress/USER-STORY-STATUS.md` |
| 模組完成度 | `3-progress/FEATURE-COMPLETION-MATRIX.md` |
| 最近的變更記錄 | `4-changes/CHANGE-LOG.md` |
| 當前項目狀態 | `5-status/PROJECT-STATUS-REPORT.md` |
| 歷史文檔 | `7-archive/` |

---

### 📅 更新頻率建議

| 文檔類型 | 更新頻率 | 更新時機 |
|---------|---------|---------|
| MASTER-DEVELOPMENT-SCHEDULE | 每 Sprint | Sprint 開始/結束時 |
| SPRINT-X-OVERVIEW | Sprint 執行中 | 實際進度偏離計劃時 |
| USER-STORY-STATUS | 實時 | User Story 狀態變更時 |
| FEATURE-COMPLETION-MATRIX | 每週 | User Story 完成時 |
| CHANGE-LOG | 變更發生時 | 正式變更批准後 |
| PROJECT-STATUS-REPORT | 每週 | 週五或 Sprint Review |

---

### 🎯 核心差異化功能追蹤

本項目有 **5 個核心差異化功能** (用 ⭐ 標記):

| # | 功能名稱 | User Story | Sprint | 狀態 |
|---|---------|-----------|--------|------|
| 1 | **Persona Builder** | US 1.5 | Sprint 3 | ⏳ 未開始 |
| 2 | **精確檢索 (90%+)** | US 5.2 | Sprint 6 | ⏳ 未開始 |
| 3 | **自動可視化生成** | US 3.2 | Sprint 8 | ⏳ 未開始 |
| 4 | **Text-to-SQL** | US 5.5, 8.2 | Sprint 10-11 | ⏳ 未開始 |
| 5 | **可視化工作流編輯器** | US 4.1 | Sprint 12 | ⏳ 未開始 |

**追蹤位置**:
- `3-progress/FEATURE-COMPLETION-MATRIX.md` - 總體追蹤
- `2-sprints/sprint-X/SPRINT-X-OVERVIEW.md` - 詳細規劃

---

## 🔄 與 /docs 文件夾的關係

### 雙層架構設計

| 層級 | 位置 | 性質 | 用途 | 更新頻率 |
|-----|------|------|------|---------|
| **Reference Layer** | `/docs` | 靜態 | 規劃參考 | 罕見 |
| **Execution Layer** | `/claudedocs` | 動態 | 執行追蹤 | 頻繁 |

### /docs 包含的內容
- **BMAD Method 規劃**: Brief, Management, Architecture, Development
- **原始需求**: User Stories 原始定義
- **Sprint 分配**: 18 個 Sprint 的 User Story 分配矩陣
- **技術架構**: 系統架構設計文檔
- **業務規劃**: 產品願景和市場分析

### /claudedocs 包含的內容
- **實際執行進度**: 計劃 vs 實際對比
- **變更記錄**: 範圍變更和決策記錄
- **狀態報告**: 當前進度快照
- **Sprint 執行**: 每個 Sprint 的詳細執行過程

---

## 📈 項目進度概覽 (快照)

**當前狀態** (截至 2025-12-08):

### Sprint 進度
- **Sprint 1**: ✅ 完成 (提前 3 天, 2025-11-04 ~ 2025-11-22)
- **Sprint 2**: 🔄 進行中 (40%, Day 14/27, 預計 2025-12-21 完成)
- **Sprint 3-18**: ⏳ 未開始

### User Story 完成度
- **總計**: 43 個 User Stories
- **已完成**: 4 個 (US 1.1, 1.2, 1.3, 1.4)
- **完成率**: 10.8% (4/43)

### 模組完成度
- **Module 1 (Agent 管理)**: 66.7% (4/6 User Stories)
- **其他 9 個模組**: 0%

### 核心差異化功能
- **已完成**: 0/5
- **進行中**: 0/5
- **未開始**: 5/5

---

## 🛠️ 維護指南

### 文檔命名規範

**Sprint 文檔**:
- 概覽: `SPRINT-X-OVERVIEW.md`
- 進度: `SPRINT-X-PROGRESS.md`
- 回顧: `SPRINT-X-RETROSPECTIVE.md`

**變更記錄**:
- 編號: `CHANGE-001`, `CHANGE-002`...
- 格式: `[CHANGE-XXX] 標題`

**狀態標記**:
- ✅ 已完成
- 🔄 進行中
- ⏳ 未開始
- ⚠️ 有風險
- 🔴 阻塞

---

## 📖 相關文檔連結

### 規劃參考 (/docs)
- [User Stories 總覽](../docs/user-stories/README.md)
- [Sprint 分配矩陣](../docs/user-stories/sprints/sprint-allocation.md)
- [架構設計](../docs/architecture/README.md)
- [技術規範](../docs/technical-specs/README.md)

### 核心追蹤文檔 (/claudedocs)
- [開發日程表](./1-planning/MASTER-DEVELOPMENT-SCHEDULE.md)
- [User Story 狀態](./3-progress/USER-STORY-STATUS.md)
- [功能完成矩陣](./3-progress/FEATURE-COMPLETION-MATRIX.md)
- [變更記錄](./4-changes/CHANGE-LOG.md)
- [項目狀態](./5-status/PROJECT-STATUS-REPORT.md)

---

## 💡 最佳實踐

### 1. 開始新 Sprint 時
1. 檢查 `MASTER-DEVELOPMENT-SCHEDULE.md` 確認 Sprint 計劃
2. 閱讀 `SPRINT-X-OVERVIEW.md` 了解詳細任務
3. 開始執行時創建 `SPRINT-X-PROGRESS.md` 追蹤每日進度

### 2. User Story 完成時
1. 更新 `USER-STORY-STATUS.md` 狀態
2. 更新 `FEATURE-COMPLETION-MATRIX.md` 完成度
3. 如有變更,記錄到 `CHANGE-LOG.md`
4. 更新 `PROJECT-STATUS-REPORT.md`

### 3. Sprint 結束時
1. 更新 `MASTER-DEVELOPMENT-SCHEDULE.md` 實際日期
2. 更新 `SPRINT-X-OVERVIEW.md` 實際結果
3. 創建 `SPRINT-X-RETROSPECTIVE.md` 回顧
4. 歸檔臨時文件到 `7-archive/`

---

## 📞 支持

**文檔維護**: AI 開發助手 (Claude)
**文檔問題**: 請在項目 Issue 中提出
**最後更新**: 2025-12-08

---

**版本歷史**:
- **v2.0.0** (2025-12-08): 全新 7 文件夾結構上線
- **v1.x** (2025-11-xx): 舊結構 (已歸檔)
