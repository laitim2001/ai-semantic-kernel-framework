# 低保真 Wireframes

**版本**: 1.0.0
**日期**: 2025-10-29
**狀態**: 🔄 開發中

---

## 概覽

本目錄包含 Semantic Kernel Agentic Framework 的 10 個核心頁面的低保真 Wireframes。

**設計方法**: 使用 Markdown + ASCII Art 創建結構化的線框圖，重點展示：
- 頁面佈局和結構
- 內容層次（Level 1-5）
- 互動元素和操作流程
- 響應式設計適配
- 角色化視圖差異

---

## Wireframe 清單

| # | 文件 | 頁面名稱 | 優先級 | 狀態 |
|---|------|---------|--------|------|
| 1 | [01-dashboard.md](./01-dashboard.md) | Dashboard (首頁) | P0 | ⏳ 待開始 |
| 2 | [02-agent-list.md](./02-agent-list.md) | Agent 列表 | P0 | ⏳ 待開始 |
| 3 | [03-agent-create.md](./03-agent-create.md) | Agent 創建 | P0 | ⏳ 待開始 |
| 4 | [04-agent-detail.md](./04-agent-detail.md) | Agent 詳情 | P0 | ⏳ 待開始 |
| 5 | [05-conversation.md](./05-conversation.md) | 對話界面 | P0 | ⏳ 待開始 |
| 6 | [06-knowledge-base.md](./06-knowledge-base.md) | Knowledge Base | P0 | ⏳ 待開始 |
| 7 | [07-code-interpreter.md](./07-code-interpreter.md) | Code Interpreter | P0 | ⏳ 待開始 |
| 8 | [08-text-to-sql.md](./08-text-to-sql.md) | Text-to-SQL | P0 | ⏳ 待開始 |
| 9 | [09-workflow-editor.md](./09-workflow-editor.md) | Workflow 編輯器 | P1 | ⏳ 待開始 |
| 10 | [10-persona-designer.md](./10-persona-designer.md) | Persona Designer | P1 | ⏳ 待開始 |

---

## Wireframe 結構模板

每個 Wireframe 文檔遵循以下結構：

### 1. 頁面概覽
- **用途和目標**: 此頁面的主要功能
- **主要用戶**: 哪些 Personas 使用此頁面
- **成功指標**: 如何衡量設計成功

### 2. 佈局結構（ASCII Art）
- **桌面版** (≥1280px): 完整佈局
- **平板版** (768-1279px): 適配佈局
- **移動版** (<768px): 簡化佈局

### 3. 內容層次
- **Level 1**: 頁面標題與主要操作
- **Level 2**: 關鍵信息與狀態
- **Level 3**: 主要內容區域
- **Level 4**: 輔助信息與操作
- **Level 5**: 詳細信息與高級功能

### 4. 互動元素
- 按鈕和 CTA
- 表單和輸入
- 導航元素
- 動態內容

### 5. 角色化視圖
- **Alex (開發者)**: 技術控制和效率
- **Sarah (分析師)**: 簡化和可視化
- **David (管理員)**: 監控和管理

### 6. 設計注釋
- 設計決策說明
- 技術實現考量
- 可訪問性要求

---

## 設計原則

### 佈局原則
- **F 型閱讀模式**: 重要信息放在左上角
- **視覺層次**: 使用大小、顏色、間距區分層次
- **白空間**: 避免過度擁擠，保持呼吸感
- **一致性**: 跨頁面使用一致的佈局模式

### 互動原則
- **即時反饋**: 操作後立即顯示結果或進度
- **可逆操作**: 提供撤銷和確認機制
- **清晰狀態**: 明確顯示當前狀態和可用操作
- **錯誤預防**: 防止無效操作和數據丟失

### 可訪問性原則
- **鍵盤導航**: 所有操作支持鍵盤快捷鍵
- **屏幕閱讀器**: 使用語義化 HTML 和 ARIA
- **顏色獨立**: 不依賴顏色傳遞唯一信息
- **焦點管理**: 清晰的焦點指示和邏輯順序

---

## ASCII Art 符號約定

```
基礎佈局符號:
┌─────┐  方框（區域邊界）
│     │  垂直線（區域分隔）
└─────┘  方框結束
═══════  粗線（主要分隔）
───────  細線（次要分隔）
┊ ┊ ┊ ┊  虛線（可選區域）

互動元素:
[Button]  按鈕
(○) Radio  單選按鈕
[x] Checkbox  複選框
[_____]  文本輸入框
[▼ Dropdown]  下拉選單
<Icon>  圖標

內容元素:
【Title】  頁面標題
■ List Item  列表項
→ Link  鏈接
⚙ Settings  設置
🔍 Search  搜索
📊 Chart  圖表
💬 Message  消息
```

---

## 參考資料

- [Week 2 計劃](../WEEK-2-PLAN.md)
- [Site Map](../../information-architecture/sitemap.md)
- [Navigation Structure](../../information-architecture/navigation-structure.md)
- [Content Hierarchy](../../information-architecture/content-hierarchy.md)
- [Personas](../../user-research/personas.md)

---

**下一步**: 開始創建第一個 Wireframe - Dashboard (首頁)
