# PROMPT-01: PROJECT ONBOARDING
# 專案上手指南

> **用途**: 新專案或新開發者快速上手 IPA 平台
> **變數**: 無
> **預估時間**: 5-10 分鐘
> **版本**: v2.0.0

---

## 📋 執行目標

幫助 AI 助手或新開發者快速理解:
1. 專案背景和目標
2. 技術架構和技術棧
3. 專案結構和關鍵文檔
4. 開發工作流程和規範
5. 可用的 AI 助手指令和 Prompts

---

## 🎯 執行步驟

### Step 1: 讀取專案核心文檔

按順序讀取以下文檔:

```yaml
必讀文檔 (MUST READ):
  1. README.md
     - 專案概述
     - 快速開始指南

  2. docs/bmm-workflow-status.yaml
     - 專案當前階段
     - 工作流程狀態

  3. docs/03-implementation/sprint-status.yaml
     - 當前 Sprint 信息
     - Story 進度狀態

  4. docs/01-planning/prd/prd-main.md
     - 產品需求文檔
     - 核心功能列表

  5. docs/02-architecture/technical-architecture.md
     - 技術架構設計
     - 技術選型決策

選讀文檔 (RECOMMENDED):
  6. docs/00-discovery/product-brief/product-brief.md
     - 產品定位和願景

  7. docs/03-implementation/sprint-planning/sprint-0-mvp-revised.md
     - 當前 Sprint 詳細計劃

  8. claudedocs/AI-ASSISTANT-INSTRUCTIONS.md
     - AI 助手指令手冊
```

### Step 2: 提取關鍵信息

從文檔中提取以下關鍵信息:

```yaml
專案基本信息:
  - 專案名稱
  - 專案定位
  - 核心價值主張
  - 目標用戶

技術架構:
  - 前端技術棧
  - 後端技術棧
  - 數據庫選型
  - 雲服務選型
  - 核心框架

當前狀態:
  - 專案階段 (Discovery/Planning/Implementation)
  - 當前 Sprint
  - Sprint 進度百分比
  - 下一個里程碑

團隊配置:
  - 團隊規模
  - 角色分配
  - Sprint 週期
```

### Step 3: 分析專案結構

掃描專案目錄結構,理解組織方式:

```bash
# 執行命令掃描目錄結構
ls -R docs/
ls -R backend/ (如果存在)
ls -R frontend/ (如果存在)
ls -R claudedocs/
```

記錄關鍵目錄:
- 文檔目錄: `docs/`
- AI 助手文檔: `claudedocs/`
- 後端代碼: `backend/`
- 前端代碼: `frontend/`
- 腳本工具: `scripts/`
- 配置文件位置

### Step 4: 理解開發工作流程

總結開發工作流程:

```yaml
Git 工作流程:
  - 主分支名稱
  - 分支命名規範
  - Commit message 格式
  - PR 流程

Sprint 工作流程:
  - Sprint 週期
  - Story 開發流程
  - 測試流程
  - 部署流程

文檔更新流程:
  - 狀態文件更新時機
  - Session 摘要要求
  - Sprint 報告要求
```

### Step 5: 列出可用工具和指令

總結可用的 AI 助手工具:

```yaml
Instructions (AI-ASSISTANT-INSTRUCTIONS.md):
  - Instruction 1: 更新專案狀態
  - Instruction 2: 生成 Sprint 完成報告
  - Instruction 3: Git 標準工作流程
  - Instruction 4: 創建 Pull Request
  - Instruction 5: 生成 Session 摘要
  - Instruction 6: 文檔一致性檢查
  - Instruction 7: 完整 Sprint 結束流程
  - Instruction 8: 快速進度同步
  - Instruction 9: 架構審查
  - Instruction 10: 代碼審查

Prompts (claudedocs/prompts/):
  - PROMPT-01: 專案上手 (當前)
  - PROMPT-02: 新 Sprint 準備
  - PROMPT-03: Bug 修復準備
  - PROMPT-04: Sprint 開發
  - PROMPT-05: 測試階段
  - PROMPT-06: 進度保存
  - PROMPT-07: 架構審查
  - PROMPT-08: 代碼審查
  - PROMPT-09: Session 結束
```

---

## 📤 輸出格式

生成結構化的專案上手報告:

```markdown
# IPA 平台專案上手報告
生成時間: {TIMESTAMP}

## 📊 專案概覽

**專案名稱**: {PROJECT_NAME}
**專案定位**: {PROJECT_POSITIONING}
**目標用戶**: {TARGET_USERS}

**核心價值**:
- {VALUE_PROP_1}
- {VALUE_PROP_2}
- {VALUE_PROP_3}

---

## 🏗️ 技術架構

### 前端技術棧
- 框架: {FRONTEND_FRAMEWORK}
- UI 庫: {UI_LIBRARY}
- 狀態管理: {STATE_MANAGEMENT}

### 後端技術棧
- 語言/框架: {BACKEND_FRAMEWORK}
- API 風格: {API_STYLE}
- 認證方式: {AUTH_METHOD}

### 數據庫
- 主數據庫: {PRIMARY_DATABASE}
- 緩存: {CACHE_SOLUTION}
- 消息隊列: {MESSAGE_QUEUE}

### 雲服務 (Azure)
- 計算: {COMPUTE_SERVICE}
- 存儲: {STORAGE_SERVICE}
- 監控: {MONITORING_SERVICE}

### 核心框架
- 框架名稱: {CORE_FRAMEWORK}
- 版本: {FRAMEWORK_VERSION}
- 特殊考慮: {FRAMEWORK_NOTES}

---

## 📍 當前狀態

**專案階段**: {CURRENT_PHASE}
(Discovery / Planning / Solutioning / Implementation)

**當前 Sprint**: {CURRENT_SPRINT}
**Sprint 週期**: {SPRINT_DURATION} 週
**Sprint 進度**: {COMPLETED_POINTS}/{TOTAL_POINTS} ({PERCENTAGE}%)

**已完成的 Stories**:
- [ ] {STORY_ID_1}: {STORY_TITLE_1}
- [ ] {STORY_ID_2}: {STORY_TITLE_2}

**進行中的 Stories**:
- [x] {STORY_ID_3}: {STORY_TITLE_3}

**下一個里程碑**: {NEXT_MILESTONE}

---

## 📁 專案結構

### 文檔目錄 (docs/)
```
docs/
├── 00-discovery/        # 探索階段文檔
│   ├── brainstorming/   # 腦力激盪
│   └── product-brief/   # 產品簡介
├── 01-planning/         # 規劃階段文檔
│   ├── prd/            # 產品需求
│   └── ui-ux/          # UI/UX 設計
├── 02-architecture/     # 架構階段文檔
│   └── gate-check/     # 關卡檢查
└── 03-implementation/   # 實施階段文檔
    └── sprint-planning/ # Sprint 計劃
```

### AI 助手文檔 (claudedocs/)
```
claudedocs/
├── AI-ASSISTANT-INSTRUCTIONS.md  # 核心指令
├── prompts/                      # Prompt 庫
│   ├── README.md
│   ├── PROMPT-01-*.md
│   └── ...
├── sprint-reports/               # Sprint 報告
└── session-logs/                 # Session 記錄
```

### 代碼目錄
```
backend/                # 後端代碼
frontend/               # 前端代碼
scripts/                # 工具腳本
```

---

## 🔄 開發工作流程

### Git 工作流程

1. **分支命名規範**:
   - Feature: `feature/sprint-{N}-{story-id}-{description}`
   - Bugfix: `bugfix/{bug-id}-{description}`
   - Hotfix: `hotfix/{issue-id}-{description}`

2. **Commit Message 格式**:
   ```
   <type>(<scope>): <description>

   [optional body]
   ```

   Types: feat, fix, docs, refactor, test, chore

3. **PR 流程**:
   - 創建 feature branch
   - 完成開發和測試
   - 提交 PR
   - Code review
   - 合併到 main

### Sprint 工作流程

```yaml
Sprint 開始:
  1. Sprint Planning 會議
  2. 閱讀 Sprint 計劃文檔
  3. 理解分配的 Stories

Story 開發:
  1. @PROMPT-02 準備 Story
  2. @PROMPT-04 執行開發
  3. @PROMPT-05 執行測試
  4. @PROMPT-06 保存進度

Sprint 結束:
  1. Instruction 6 檢查文檔
  2. Instruction 7 完整結束流程
  3. Sprint Retrospective
```

---

## 🛠️ 可用的 AI 助手工具

### Instructions 指令

**日常開發**:
- `Instruction 1`: 更新 sprint-status.yaml
- `Instruction 3`: Git 提交
- `Instruction 8`: 快速進度同步

**Sprint 管理**:
- `Instruction 2`: 生成 Sprint 完成報告
- `Instruction 5`: 生成 Session 摘要
- `Instruction 7`: 完整 Sprint 結束流程

**質量保證**:
- `Instruction 6`: 文檔一致性檢查
- `Instruction 9`: 架構審查
- `Instruction 10`: 代碼審查

**發布流程**:
- `Instruction 4`: 創建 Pull Request

### Prompts 提示詞

**準備階段**:
- `@PROMPT-01`: 專案上手 (當前使用中)
- `@PROMPT-02 {Sprint} {Story}`: 準備新 Story
- `@PROMPT-03 {BugID}`: 準備修復 Bug

**開發階段**:
- `@PROMPT-04 {Sprint} {Story}`: 執行 Story 開發
- `@PROMPT-05 {Story}`: 執行測試

**完成階段**:
- `@PROMPT-06 {Sprint} {Story}`: 保存進度
- `@PROMPT-09`: Session 結束

**審查階段**:
- `@PROMPT-07`: 架構審查
- `@PROMPT-08 {Path}`: 代碼審查

---

## 🎯 建議的下一步

基於當前專案狀態,建議:

### 如果是新開發者:
1. ✅ 閱讀完整的技術架構文檔
2. ✅ 設置本地開發環境
3. ✅ 閱讀當前 Sprint 計劃
4. ✅ 與團隊成員溝通了解當前進度

### 如果是 AI 助手:
1. ✅ 已完成專案上手
2. ⏳ 準備好協助開發工作
3. ⏳ 可以執行以下操作:
   - 回答專案相關問題
   - 執行 Instructions 指令
   - 使用其他 Prompts 協助開發
   - 生成文檔和報告

---

## 📚 重要文檔快速鏈接

| 文檔 | 路徑 | 用途 |
|------|------|------|
| 專案 README | `README.md` | 專案概述 |
| 工作流程狀態 | `docs/bmm-workflow-status.yaml` | 階段追蹤 |
| Sprint 狀態 | `docs/03-implementation/sprint-status.yaml` | Sprint 追蹤 |
| PRD | `docs/01-planning/prd/prd-main.md` | 產品需求 |
| 技術架構 | `docs/02-architecture/technical-architecture.md` | 架構設計 |
| Sprint 計劃 | `docs/03-implementation/sprint-planning/` | Sprint 規劃 |
| AI 指令 | `claudedocs/AI-ASSISTANT-INSTRUCTIONS.md` | 助手指令 |
| Prompts 庫 | `claudedocs/prompts/` | Prompt 文件 |

---

## ✅ 上手檢查清單

完成以下檢查項,確認已充分理解專案:

專案理解:
- [ ] 了解專案背景和目標
- [ ] 理解核心功能和價值主張
- [ ] 知道目標用戶是誰

技術架構:
- [ ] 了解前端技術棧
- [ ] 了解後端技術棧
- [ ] 了解數據庫和雲服務選型
- [ ] 理解核心框架 (Agent Framework)

當前狀態:
- [ ] 知道專案當前在哪個階段
- [ ] 了解當前 Sprint 目標
- [ ] 清楚當前進度和下一個里程碑

工作流程:
- [ ] 理解 Git 工作流程
- [ ] 理解 Sprint 工作流程
- [ ] 知道如何使用 AI 助手工具

準備就緒:
- [ ] 可以開始協助開發工作
- [ ] 知道如何獲取幫助
- [ ] 知道下一步要做什麼

---

**完成時間**: {COMPLETION_TIME}
**生成者**: AI Assistant (PROMPT-01)
```

---

## 💡 使用提示

### 何時使用此 Prompt

- ✅ 新專案啟動時
- ✅ 新 AI 助手加入專案時
- ✅ 新開發者加入團隊時
- ✅ 專案發生重大變更後
- ✅ 需要全面回顧專案狀態時

### 預期效果

執行此 Prompt 後,AI 助手應該能夠:
- 回答關於專案的基本問題
- 理解專案當前狀態和進度
- 知道如何使用其他 Prompts 和 Instructions
- 提供專案相關的建議和指導

---

## 🔗 相關文檔

- [AI Assistant Instructions](../AI-ASSISTANT-INSTRUCTIONS.md)
- [Prompts README](./README.md)
- [PROMPT-02: New Sprint Prep](./PROMPT-02-NEW-SPRINT-PREP.md)
- [PROMPT-04: Sprint Development](./PROMPT-04-SPRINT-DEVELOPMENT.md)

---

**版本**: v2.0.0
**更新日期**: 2025-11-20
**維護者**: AI Assistant Team
