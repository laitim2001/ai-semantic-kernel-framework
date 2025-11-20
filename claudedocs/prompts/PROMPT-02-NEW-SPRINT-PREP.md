# PROMPT-02: NEW SPRINT PREPARATION
# 新 Sprint Story 準備

> **用途**: 準備開始新的 Sprint Story,理解需求和技術背景
> **變數**: `{SPRINT_ID}` `{STORY_ID}`
> **預估時間**: 3-5 分鐘
> **版本**: v2.0.0

---

## 🔤 變數定義

```yaml
{SPRINT_ID}:
  描述: Sprint 標識符
  格式: "Sprint-{N}" 或 "sprint_{N}"
  範例: "Sprint-0", "sprint_0"

{STORY_ID}:
  描述: Story 標識符
  格式: "S{Sprint}-{Story}"
  範例: "S0-1", "S0-2", "S1-3"
```

---

## 🎯 執行步驟

### Step 1: 讀取 Sprint Status

```yaml
讀取文件:
  - docs/03-implementation/sprint-status.yaml

查找 Story:
  - 使用 {STORY_ID} 定位到具體 Story
  - 提取 Story 所有信息:
    - id
    - title (標題)
    - description (描述)
    - story_points (故事點)
    - assignee (負責人)
    - status (當前狀態)
    - priority (優先級)
    - dependencies (依賴項)
    - notes (備註)
```

### Step 2: 讀取 Sprint 計劃文檔

```yaml
文件路徑:
  - docs/03-implementation/sprint-planning/sprint-{N}-*.md

提取信息:
  - Sprint 目標
  - Story 詳細描述
  - 驗收標準
  - 技術要求
  - 測試要求
```

### Step 3: 讀取技術架構文檔

```yaml
文件路徑:
  - docs/02-architecture/technical-architecture.md
  - docs/02-architecture/technical-architecture-part2.md
  - docs/02-architecture/technical-architecture-part3.md

關注:
  - 與 Story 相關的架構設計
  - 技術選型決策
  - 接口規範
  - 數據模型
```

### Step 4: 檢查依賴項

```yaml
如果 Story 有 dependencies:
  - 檢查每個依賴 Story 的狀態
  - 確認所有依賴都已完成
  - 如果有未完成的依賴,警告用戶

輸出依賴檢查結果:
  ✅ S0-1: Development Environment Setup (completed)
  ⚠️ S0-2: Azure App Service Setup (in-progress)
  ❌ S0-3: CI/CD Pipeline (not-started)
```

---

## 📤 輸出格式

```markdown
# Sprint Story 準備報告: {STORY_ID}

**生成時間**: {TIMESTAMP}
**生成者**: AI Assistant (PROMPT-02)

---

## 📊 Story 基本信息

| 項目 | 內容 |
|------|------|
| **Story ID** | {STORY_ID} |
| **標題** | {STORY_TITLE} |
| **Sprint** | {SPRINT_ID} |
| **Story Points** | {STORY_POINTS} |
| **負責人** | {ASSIGNEE} |
| **優先級** | {PRIORITY} |
| **當前狀態** | {CURRENT_STATUS} |

---

## 📋 需求摘要

### Story 描述
{STORY_DESCRIPTION}

### 驗收標準
1. {ACCEPTANCE_CRITERIA_1}
2. {ACCEPTANCE_CRITERIA_2}
3. {ACCEPTANCE_CRITERIA_3}

### 功能要求
- {FUNCTIONAL_REQUIREMENT_1}
- {FUNCTIONAL_REQUIREMENT_2}

### 非功能要求
- {NON_FUNCTIONAL_REQUIREMENT_1}
- {NON_FUNCTIONAL_REQUIREMENT_2}

---

## 🔧 技術背景

### 相關架構組件
- **組件**: {COMPONENT_NAME}
- **技術棧**: {TECH_STACK}
- **接口**: {API_INTERFACE}

### 技術參考文檔
- [技術架構](../../docs/02-architecture/technical-architecture.md#{SECTION})
- [Sprint 計劃](../../docs/03-implementation/sprint-planning/sprint-{N}-*.md)
- [PRD 功能規格](../../docs/01-planning/prd/features/feature-{N}*.md)

---

## ⚠️ 依賴項檢查

{DEPENDENCY_CHECK_RESULTS}

---

## ✅ 準備檢查清單

環境準備:
- [ ] 本地開發環境已啟動
- [ ] 相關文檔已閱讀
- [ ] 技術架構已理解

依賴確認:
- [ ] 所有依賴 Story 已完成
- [ ] 相關 API 接口已就緒
- [ ] 測試環境已準備

代碼準備:
- [ ] 創建 feature branch
- [ ] 了解相關代碼位置

---

## 🚀 下一步行動

1. ✅ Story 準備完成,可以開始開發
2. ⏭️ 執行 `@PROMPT-04-SPRINT-DEVELOPMENT.md {SPRINT_ID} {STORY_ID}`
3. 📋 或查看技術文檔進行深入研究

---

## 📚 相關資源

- [Sprint Status](../../docs/03-implementation/sprint-status.yaml)
- [技術架構](../../docs/02-architecture/technical-architecture.md)
- [開發指南](../../docs/03-implementation/local-development-guide.md)

---

**生成工具**: PROMPT-02
**版本**: v2.0.0
```

---

## 💡 使用範例

```bash
# 準備開始 Sprint 0 的 Story S0-2
用戶: "@PROMPT-02-NEW-SPRINT-PREP.md Sprint-0 S0-2"

AI 執行:
1. 讀取 sprint-status.yaml 找到 S0-2
2. 讀取 Sprint 0 計劃文檔
3. 讀取技術架構文檔
4. 檢查依賴項 (S0-1 是否完成)
5. 生成準備報告

輸出:
---
📋 Sprint Story 準備完成

Story: S0-2 - Azure App Service Setup
Sprint: Sprint 0
Points: 5
負責人: DevOps

需求摘要:
- 創建 App Service Plan (Standard S1)
- 配置 staging 和 production 環境
- 設置環境變數
- 配置自動擴展規則

依賴檢查:
✅ 所有依賴已完成

準備就緒: ✅
下一步: @PROMPT-04-SPRINT-DEVELOPMENT.md Sprint-0 S0-2
---
```

---

## 🔗 相關文檔

- [AI Assistant Instructions](../AI-ASSISTANT-INSTRUCTIONS.md)
- [PROMPT-04: Sprint Development](./PROMPT-04-SPRINT-DEVELOPMENT.md)
- [Sprint Status YAML](../../docs/03-implementation/sprint-status.yaml)

---

**版本**: v2.0.0
**更新日期**: 2025-11-20
