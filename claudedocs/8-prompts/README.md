# AI 助手 Prompt 模板庫

**版本**: 2.0.0
**最後更新**: 2025-12-08
**用途**: 提供標準化的 AI 助手對話啟動模板

---

## 📋 目錄

### 對話開始前 (Session 初始化)
1. [PROMPT-01-PROJECT-ONBOARDING.md](#prompt-01) - 項目快速入門
2. [PROMPT-02-NEW-FEATURE-PREP.md](#prompt-02) - 新功能開發準備
3. [PROMPT-03-BUG-FIX-PREP.md](#prompt-03) - Bug 修復準備

### 對話進行中 (Session 執行)
4. [PROMPT-04-FEATURE-DEVELOPMENT.md](#prompt-04) - 功能開發中
5. [PROMPT-05-TESTING-PHASE.md](#prompt-05) - 測試階段
6. [PROMPT-06-PROGRESS-SAVE.md](#prompt-06) - 進度保存與同步

### 特殊場景
7. [PROMPT-07-ARCHITECTURE-REVIEW.md](#prompt-07) - 架構審查
8. [PROMPT-08-CODE-REVIEW.md](#prompt-08) - 代碼審查
9. [PROMPT-09-SESSION-END.md](#prompt-09) - Session 結束

---

## 🎯 核心設計理念

### 新版本設計 (v2.0.0)
- **AI 自主讀取**: 開發者只需引用檔名,AI 自動讀取並執行
- **精簡高效**: Prompt 長度控制在 80-300 行
- **變數支援**: 支援關鍵場景的上下文變數
- **指令式設計**: 清晰的 AI 執行步驟,而非示範性內容
- **輸出適中**: 提供足夠詳細但不冗長的分析和建議

### 與舊版本 (v1.0.0) 的差異
| 特性 | v1.0.0 | v2.0.0 |
|------|--------|--------|
| **使用方式** | 複製整個內容貼上 | `@PROMPT-XX.md` 引用 |
| **長度** | 200+ 行 | 80-300 行 |
| **變數** | 無 | 支援 (選擇性) |
| **AI 角色** | 被動接收 | 主動讀取執行 |
| **輸出** | 示範性範例 | 實際分析結果 |

---

## 📖 使用指南

### 如何使用這些 Prompt

#### 基本用法 (無變數)
```
開發者: @PROMPT-01-PROJECT-ONBOARDING.md
AI: [自動讀取文件並執行項目快速入門流程]
```

#### 變數用法 (支援變數的 Prompt)
```
開發者: @PROMPT-02-NEW-FEATURE-PREP.md US-1.5
AI: [自動讀取文件並分析 US-1.5 相關內容]

開發者: @PROMPT-04-FEATURE-DEVELOPMENT.md US-1.5 Phase-2
AI: [自動讀取文件並提供 US-1.5 Phase 2 的開發指引]

開發者: @PROMPT-08-CODE-REVIEW.md src/AIAgentPlatform.Application
AI: [自動讀取文件並審查指定模組的代碼]
```

### 範例場景

#### 場景 1: 首次接觸項目
```
開發者: @PROMPT-01-PROJECT-ONBOARDING.md
```
AI 會自動:
- 讀取項目概覽和文檔結構
- 顯示當前 Sprint 狀態
- 分析技術棧和架構
- 提供快速導航連結

#### 場景 2: 準備開發新功能
```
開發者: @PROMPT-02-NEW-FEATURE-PREP.md US-1.6
```
AI 會自動:
- 分析 US-1.6 的 User Story 文檔
- 審查現有架構和相關代碼
- 評估技術可行性
- 規劃實作步驟

#### 場景 3: 開發進行中
```
開發者: @PROMPT-04-FEATURE-DEVELOPMENT.md US-1.6 Phase-2
```
AI 會自動:
- 提供 Phase 2 的實作指引
- 檢查代碼質量和最佳實踐
- 提醒測試要求
- 追蹤進度

#### 場景 4: Session 結束前
```
開發者: @PROMPT-09-SESSION-END.md
```
AI 會自動:
- 檢查狀態報告是否最新
- 檢查 Git 未提交變更
- 評估 Session 摘要需求
- 確認待辦事項記錄

---

## 🗂️ Prompt 詳細說明

### <a id="prompt-01"></a>PROMPT-01: 項目快速入門

**場景**: 首次接觸項目或久未參與開發
**目標**: 快速了解項目概況、結構、當前狀態
**預估時間**: 2-3 分鐘
**變數支援**: ❌ 無變數 (固定流程)

**使用方式**:
```
@PROMPT-01-PROJECT-ONBOARDING.md
```

**AI 執行內容**:
1. 讀取項目概覽和文檔結構
2. 顯示當前 Sprint 狀態和進度
3. 分析技術棧和架構
4. 提供快速導航連結

---

### <a id="prompt-02"></a>PROMPT-02: 新功能開發準備

**場景**: 準備開發新功能前
**目標**: 架構審查、User Story 分析、技術可行性評估
**預估時間**: 5-10 分鐘
**變數支援**: ✅ `[US-X.X]` User Story ID

**使用方式**:
```
@PROMPT-02-NEW-FEATURE-PREP.md US-1.5
```

**AI 執行內容**:
1. User Story 詳細分析
2. 現有架構審查
3. 相關代碼檢視
4. 技術可行性評估
5. 實作步驟規劃

---

### <a id="prompt-03"></a>PROMPT-03: Bug 修復準備

**場景**: 準備修復 Bug 前
**目標**: 問題定位、影響範圍分析、修復策略
**預估時間**: 3-5 分鐘
**變數支援**: ✅ `[BUG-ID]` Bug 編號或問題描述

**使用方式**:
```
@PROMPT-03-BUG-FIX-PREP.md BUG-001
```
或
```
@PROMPT-03-BUG-FIX-PREP.md "登入失敗錯誤"
```

**AI 執行內容**:
1. 問題信息收集
2. 問題定位分析
3. 影響範圍評估
4. 修復策略建議
5. 測試計劃

---

### <a id="prompt-04"></a>PROMPT-04: 功能開發中

**場景**: 功能開發進行中
**目標**: 實作指引、測試提醒、進度追蹤
**預估時間**: 3-5 分鐘
**變數支援**: ✅ `[US-X.X]` + `[Phase-N]` (可選)

**使用方式**:
```
@PROMPT-04-FEATURE-DEVELOPMENT.md US-1.5 Phase-2
```
或
```
@PROMPT-04-FEATURE-DEVELOPMENT.md US-1.5
```

**AI 執行內容**:
1. User Story 上下文回顧
2. Phase 特定實作指引 (如提供 Phase)
3. 代碼質量檢查清單
4. 測試要求提醒
5. 進度建議

---

### <a id="prompt-05"></a>PROMPT-05: 測試階段

**場景**: 功能開發完成,準備執行測試
**目標**: 測試執行、失敗分析、覆蓋率檢查
**預估時間**: 5-10 分鐘 (依測試數量)
**變數支援**: ✅ `[US-X.X]` (可選)

**使用方式**:
```
@PROMPT-05-TESTING-PHASE.md US-1.5
```
或
```
@PROMPT-05-TESTING-PHASE.md (執行全部測試)
```

**AI 執行內容**:
1. 測試前編譯檢查
2. 執行單元測試
3. 執行集成測試
4. 失敗分析 (如有)
5. 測試覆蓋率檢查
6. 測試報告更新

---

### <a id="prompt-06"></a>PROMPT-06: 進度保存與同步

**場景**: 保存當前開發進度,同步文檔,上傳到 GitHub
**目標**: 文檔同步、索引更新、Git 操作、狀態報告更新
**預估時間**: 5-10 分鐘
**變數支援**: ✅ `[US-X.X]` + `[Phase-N]` (可選)

**使用方式**:
```
@PROMPT-06-PROGRESS-SAVE.md US-1.5 Phase-2
```

**AI 執行內容**:
1. 工作信息收集
2. 文檔一致性檢查
3. 更新項目狀態報告
4. 更新 User Story 狀態
5. Git 狀態檢查
6. 執行 Git 工作流程
7. 生成 Session 摘要 (如需要)
8. 索引維護
9. 最終驗證

---

### <a id="prompt-07"></a>PROMPT-07: 架構審查

**場景**: 架構設計驗證、系統擴展性評估、技術債務識別
**目標**: 架構一致性檢查、擴展性評估、重構建議
**預估時間**: 10-15 分鐘
**變數支援**: ❌ 無變數 (完整架構審查)

**使用方式**:
```
@PROMPT-07-ARCHITECTURE-REVIEW.md
```

**AI 執行內容**:
1. 架構文檔審查
2. 代碼架構一致性檢查
3. 依賴關係分析
4. 可擴展性評估
5. 技術債務識別
6. 安全性檢查
7. 性能檢查
8. 改進建議總結

---

### <a id="prompt-08"></a>PROMPT-08: 代碼審查

**場景**: 代碼質量檢查、PR 創建前審查、最佳實踐驗證
**目標**: 代碼質量、安全檢查、最佳實踐驗證
**預估時間**: 5-10 分鐘
**變數支援**: ✅ `[路徑]` 文件或目錄路徑 (可選)

**使用方式**:
```
@PROMPT-08-CODE-REVIEW.md src/AIAgentPlatform.Application/Features/Agents
```
或
```
@PROMPT-08-CODE-REVIEW.md (審查最近變更)
```

**AI 執行內容**:
1. 識別審查範圍
2. 代碼質量檢查
3. 架構一致性檢查
4. 安全性審查
5. 性能檢查
6. 測試覆蓋檢查
7. 最佳實踐檢查
8. 審查總結和建議

---

### <a id="prompt-09"></a>PROMPT-09: Session 結束

**場景**: Session 結束前的強制檢查清單
**目標**: 確保文檔同步、代碼已提交、進度已保存
**預估時間**: 3-5 分鐘
**變數支援**: ❌ 無變數 (標準結束檢查)
**重要性**: 🔴 Critical - 每次 Session 結束前必須執行

**使用方式**:
```
@PROMPT-09-SESSION-END.md
```

**AI 執行內容**:
1. 檢查 PROJECT-STATUS-REPORT.md 是否最新
2. 檢查 Git 未提交變更
3. 評估 Session 摘要需求
4. 檢查 Todo 事項記錄
5. 生成 Session 結束總結

---

## 🎯 快速選擇流程圖

```
開始新的 AI 助手對話
    ↓
判斷場景
    ↓
    ├─ 首次接觸項目? → @PROMPT-01-PROJECT-ONBOARDING.md
    ├─ 準備開發新功能? → @PROMPT-02-NEW-FEATURE-PREP.md US-X.X
    ├─ 準備修復 Bug? → @PROMPT-03-BUG-FIX-PREP.md BUG-ID
    ├─ 功能開發中? → @PROMPT-04-FEATURE-DEVELOPMENT.md US-X.X [Phase-N]
    ├─ 執行測試中? → @PROMPT-05-TESTING-PHASE.md [US-X.X]
    ├─ 需要保存進度? → @PROMPT-06-PROGRESS-SAVE.md US-X.X [Phase-N]
    ├─ 需要架構審查? → @PROMPT-07-ARCHITECTURE-REVIEW.md
    ├─ 需要代碼審查? → @PROMPT-08-CODE-REVIEW.md [路徑]
    └─ Session 結束? → @PROMPT-09-SESSION-END.md (必須執行)
```

---

## 📊 變數支援總覽

| Prompt | 支援變數 | 變數說明 | 範例 |
|--------|---------|---------|------|
| **PROMPT-01** | ❌ | 無變數 | `@PROMPT-01-PROJECT-ONBOARDING.md` |
| **PROMPT-02** | ✅ | `[US-X.X]` User Story ID | `@PROMPT-02-NEW-FEATURE-PREP.md US-1.5` |
| **PROMPT-03** | ✅ | `[BUG-ID]` Bug 編號或描述 | `@PROMPT-03-BUG-FIX-PREP.md BUG-001` |
| **PROMPT-04** | ✅ | `[US-X.X]` + `[Phase-N]` (可選) | `@PROMPT-04-FEATURE-DEVELOPMENT.md US-1.5 Phase-2` |
| **PROMPT-05** | ✅ | `[US-X.X]` (可選) | `@PROMPT-05-TESTING-PHASE.md US-1.5` |
| **PROMPT-06** | ✅ | `[US-X.X]` + `[Phase-N]` (可選) | `@PROMPT-06-PROGRESS-SAVE.md US-1.5 Phase-2` |
| **PROMPT-07** | ❌ | 無變數 | `@PROMPT-07-ARCHITECTURE-REVIEW.md` |
| **PROMPT-08** | ✅ | `[路徑]` 文件/目錄 (可選) | `@PROMPT-08-CODE-REVIEW.md src/Application` |
| **PROMPT-09** | ❌ | 無變數 | `@PROMPT-09-SESSION-END.md` |

---

## 📌 最佳實踐

### 1. Session 開始時
- **新成員或久未接觸**: `@PROMPT-01-PROJECT-ONBOARDING.md`
- **準備新任務**: `@PROMPT-02-NEW-FEATURE-PREP.md US-X.X` 或 `@PROMPT-03-BUG-FIX-PREP.md BUG-ID`
- **提供具體上下文**: 使用變數提供 User Story ID 或 Bug 描述

### 2. Session 進行中
- **Phase 開始**: `@PROMPT-04-FEATURE-DEVELOPMENT.md US-X.X Phase-N`
- **測試執行**: `@PROMPT-05-TESTING-PHASE.md US-X.X`
- **保存進度**: `@PROMPT-06-PROGRESS-SAVE.md US-X.X Phase-N` (每完成重要功能或每日結束)
- **重大變更前**: `@PROMPT-07-ARCHITECTURE-REVIEW.md` (主動檢查架構)
- **PR 創建前**: `@PROMPT-08-CODE-REVIEW.md [路徑]`

### 3. Session 結束時
- **必須執行**: `@PROMPT-09-SESSION-END.md` (確保文檔同步、進度保存)
- **記錄待辦**: 讓 AI 檢查並記錄下次 Session 的待辦事項
- **更新索引**: 重大變更後執行索引維護

---

## 🔧 Prompt 結構說明

每個 Prompt 文件遵循以下標準結構:

```markdown
# PROMPT-XX: [標題]

**場景**: [使用場景描述]
**目標**: [執行目標]
**預估時間**: [執行時間]
**適用對象**: [適用情況]

---

## 🎯 使用方式

```
開發者: @PROMPT-XX.md [變數]
AI: [執行說明]
```

**變數說明** (如適用):
- [變數說明]

---

## 🤖 AI 執行指令

當開發者引用此 Prompt 時,請按以下順序執行:

### 步驟 1: [步驟標題]
**執行命令/讀取文件**:
[具體指令]

**輸出**:
[輸出格式範例]

### 步驟 2: [步驟標題]
...

---

## 📊 預期輸出

- **輸出長度**: [字數範圍]
- **文檔讀取數量**: [文件數量]
- **執行時間**: [時間範圍]

---

**版本**: X.Y.Z
**最後更新**: YYYY-MM-DD
**維護者**: Development Team
```

---

## 🔗 相關文檔

- **AI 助手指引**: [6-ai-assistant/AI-ASSISTANT-INSTRUCTIONS.md](../6-ai-assistant/AI-ASSISTANT-INSTRUCTIONS.md)
- **項目狀態報告**: [5-status/PROJECT-STATUS-REPORT.md](../5-status/PROJECT-STATUS-REPORT.md)
- **文檔中心**: [README.md](../README.md)
- **User Story 狀態**: [3-progress/USER-STORY-STATUS.md](../3-progress/USER-STORY-STATUS.md)

---

## 📝 版本歷史

### v2.0.0 (2025-12-08)
- 🎯 **重大改版**: 從複製貼上模式改為 AI 自主讀取模式
- ✅ **變數支援**: 支援關鍵場景的上下文變數 (US ID, Phase, Bug ID, 路徑)
- 📏 **精簡設計**: Prompt 長度從 200+ 行縮減至 80-300 行
- 🤖 **指令式設計**: 清晰的 AI 執行步驟,而非示範性內容
- 📊 **輸出優化**: 適中詳細度,避免冗長

### v1.0.0 (2025-12-08)
- 初始版本
- 複製貼上使用模式
- 無變數支援

---

## 💡 常見問題

**Q: 什麼時候使用變數?**
A: 當你需要 AI 分析特定 User Story、Bug 或代碼路徑時使用變數。例如 `@PROMPT-02-NEW-FEATURE-PREP.md US-1.5` 會讓 AI 專注分析 US-1.5。

**Q: 變數可以省略嗎?**
A: 大部分支援變數的 Prompt 都可以省略變數。如果省略,AI 會詢問你或分析最近變更的內容。

**Q: 為什麼 PROMPT-09 是必須執行的?**
A: PROMPT-09 實施 Instruction 10 的強制檢查清單,確保文檔同步、代碼已提交、進度已保存,防止進度遺失。

**Q: 可以自定義 Prompt 嗎?**
A: 可以。複製現有 Prompt,根據團隊需求調整,重新命名為 `PROMPT-XX-CUSTOM-[name].md`,並在此 README 添加說明。

**Q: Prompt 之間有依賴關係嗎?**
A: 沒有強制依賴,但建議遵循最佳實踐的順序 (例如: PROMPT-02 → PROMPT-04 → PROMPT-05 → PROMPT-06 → PROMPT-09)。

---

**維護者**: Development Team
**更新頻率**: 根據團隊反饋持續改進
**問題反饋**: 透過 claudedocs/4-changes/CHANGE-LOG.md 記錄改進建議
