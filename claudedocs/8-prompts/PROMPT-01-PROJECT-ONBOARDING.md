# PROMPT-01: 項目快速入門

**場景**: 首次接觸項目或久未參與開發
**目標**: 快速了解項目概況、結構、當前狀態
**預估時間**: 2-3 分鐘
**適用對象**: 新成員、回歸開發者、跨 Sprint 重新開始

---

## 🎯 使用方式

```
開發者: @PROMPT-01-PROJECT-ONBOARDING.md
AI: [執行以下指令]
```

---

## 🤖 AI 執行指令

當開發者引用此 Prompt 時,請按以下順序執行:

### 步驟 1: 項目基本信息
**讀取文件**:
- `docs/brief/brief-1-overview.md` (項目概覽)
- `claudedocs/README.md` (文檔結構)

**提取並輸出**:
- 項目名稱和願景 (1-2 句話)
- 核心差異化功能 (列出 5 個)
- 文檔結構概覽 (7 個文件夾用途)

---

### 步驟 2: 當前開發狀態
**讀取文件**:
- `claudedocs/5-status/PROJECT-STATUS-REPORT.md`
- `claudedocs/3-progress/USER-STORY-STATUS.md`

**提取並輸出**:
- 當前 Sprint 編號和進度百分比
- 已完成 User Stories (數量/總數)
- 進行中 User Stories (列出 ID 和標題)
- 最近完成的工作 (3-5 項)
- 已知問題 (如果有)

---

### 步驟 3: 技術架構概覽
**讀取文件**:
- `docs/architecture/architecture-overview.md`
- `claudedocs/5-status/PROJECT-STATUS-REPORT.md` (API 端點清單)

**提取並輸出**:
- 技術棧: Backend, Frontend, Database, Cloud, AI
- 系統架構圖描述 (簡要)
- 現有 API 端點數量和主要分類
- 測試覆蓋率統計

---

### 步驟 4: 項目結構導航
**執行命令**:
```bash
ls -la "C:\AI Semantic Kernel"
```

**輸出**:
- 專案根目錄主要文件夾: src/, tests/, docs/, claudedocs/, poc-projects/
- 各文件夾用途簡述

---

### 步驟 5: 快速導航連結
**輸出格式**:
```markdown
## 📚 關鍵文檔路徑

### 規劃層 (/docs)
- 項目概覽: docs/brief/brief-1-overview.md
- 需求規格: docs/brief/brief-2-requirements.md
- 技術架構: docs/architecture/architecture-overview.md
- User Stories: docs/user-stories/README.md

### 執行層 (/claudedocs)
- 文檔中心: claudedocs/README.md
- 狀態報告: claudedocs/5-status/PROJECT-STATUS-REPORT.md
- 進度追蹤: claudedocs/3-progress/USER-STORY-STATUS.md
- AI 指引: claudedocs/6-ai-assistant/AI-ASSISTANT-INSTRUCTIONS.md

### 代碼結構 (/src)
- 解決方案: src/AIAgentPlatform.sln
- Domain: src/AIAgentPlatform.Domain/
- Application: src/AIAgentPlatform.Application/
- Infrastructure: src/AIAgentPlatform.Infrastructure/
- API: src/AIAgentPlatform.API/
- 測試: tests/AIAgentPlatform.UnitTests/, tests/AIAgentPlatform.IntegrationTests/
```

---

## 🎯 最終輸出摘要

請以清晰的 Markdown 格式總結以上信息,並在最後提供:

**下一步建議**:
- 開發新功能 → `@PROMPT-02-NEW-FEATURE-PREP.md [US-X.X]`
- 修復問題 → `@PROMPT-03-BUG-FIX-PREP.md`
- 架構審查 → `@PROMPT-07-ARCHITECTURE-REVIEW.md`

---

## 📊 預期輸出

- **輸出長度**: ~1000-1500 字
- **文檔讀取數量**: ~6 個文件
- **執行時間**: 2-3 分鐘

---

**版本**: 2.0.0
**最後更新**: 2025-12-08
**維護者**: Development Team
