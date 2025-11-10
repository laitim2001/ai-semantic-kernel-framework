# PROMPT-01: 項目快速入門

**場景**: 首次接觸項目或久未參與開發
**目標**: 快速了解項目概況、結構、當前狀態
**預估時間**: 2-3 分鐘

---

## 🎯 AI 執行指令

當開發者使用 `@PROMPT-01-PROJECT-ONBOARDING.md` 時,請按以下順序執行:

### 步驟 1: 項目基本信息
**讀取文件**:
- `docs/brief/brief-1-overview.md`
- `claudedocs/README.md`

**提取並輸出**:
- 項目名稱和願景 (1-2 句話)
- 核心差異化功能 (列出 5 個)
- 文檔結構 (7 個文件夾及用途)

---

### 步驟 2: 當前開發狀態
**讀取文件**:
- `claudedocs/5-status/PROJECT-STATUS-REPORT.md`
- `claudedocs/3-progress/USER-STORY-STATUS.md`

**提取並輸出**:
- 當前 Sprint 編號和完成百分比
- 已完成/進行中的 User Stories
- 最近 3-5 項完成的工作
- 已知問題 (如果有)

---

### 步驟 3: 技術架構概覽
**讀取文件**:
- `docs/architecture/architecture-overview.md`
- `claudedocs/5-status/PROJECT-STATUS-REPORT.md` (API 端點部分)

**提取並輸出**:
- 技術棧: Backend, Frontend, Database, Cloud, AI
- 現有 API 端點數量
- 測試覆蓋情況

---

### 步驟 4: 項目結構導航
**執行命令**:
```bash
ls -la "C:\AI Semantic Kernel"
```

**輸出**:
- 主要文件夾列表: src/, tests/, docs/, claudedocs/, poc-projects/
- 簡要說明各文件夾用途

---

### 步驟 5: 快速導航連結
**輸出格式**:
```markdown
## 📚 關鍵文檔路徑

**規劃層 (/docs)**:
- 項目概覽: docs/brief/brief-1-overview.md
- 技術架構: docs/architecture/architecture-overview.md
- User Stories: docs/user-stories/

**執行層 (/claudedocs)**:
- 狀態報告: claudedocs/5-status/PROJECT-STATUS-REPORT.md
- 進度追蹤: claudedocs/3-progress/USER-STORY-STATUS.md
- AI 指引: claudedocs/6-ai-assistant/AI-ASSISTANT-INSTRUCTIONS.md

**代碼結構 (/src)**:
- 解決方案: src/AIAgentPlatform.sln
- 測試: tests/
```

---

## 🎯 預期輸出長度

- **總字數**: ~800-1200 字
- **輸出時間**: 2-3 分鐘
- **文檔讀取**: 約 6 個文件

---

## 💡 下一步建議

**完成入門後,根據需求選擇**:
- 開發新功能 → 使用 `@PROMPT-02-NEW-FEATURE-PREP.md`
- 修復問題 → 使用 `@PROMPT-03-BUG-FIX-PREP.md`
- 架構審查 → 使用 `@PROMPT-07-ARCHITECTURE-REVIEW.md`

---

**版本**: 2.0.0
**最後更新**: 2025-12-08
**維護者**: Development Team
