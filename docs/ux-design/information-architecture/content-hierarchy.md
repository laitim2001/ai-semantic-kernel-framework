# Content Hierarchy - Semantic Kernel Agentic Framework

## 概覽

本文件定義了 Semantic Kernel Agentic Framework 的內容層次結構，包括信息優先級、內容組織原則、以及跨模組的內容關聯關係。

**設計目標**:
- **信息優先級**: 最重要的信息最先展示
- **認知負荷**: 避免一次性呈現過多信息
- **任務導向**: 內容組織支持用戶任務完成
- **一致性**: 統一的內容層次和展示模式

---

## 內容層次體系

### 層次結構定義

```
內容層次（由高到低）:

Level 1: 頁面標題與主要操作
  ├─ 功能: 明確當前位置和主要任務
  ├─ 組件: 頁面標題、主要 CTA、面包屑
  └─ 優先級: P0（最高）

Level 2: 關鍵信息與狀態
  ├─ 功能: 核心數據和系統狀態
  ├─ 組件: KPI 卡片、狀態指示器、摘要信息
  └─ 優先級: P0

Level 3: 主要內容區域
  ├─ 功能: 用戶主要工作區域
  ├─ 組件: 列表、表格、表單、編輯器
  └─ 優先級: P0

Level 4: 輔助信息與操作
  ├─ 功能: 支持性信息和次要操作
  ├─ 組件: 側邊欄、工具欄、提示信息
  └─ 優先級: P1

Level 5: 詳細信息與高級功能
  ├─ 功能: 深度信息和專業功能
  ├─ 組件: 折疊面板、彈窗、高級設置
  └─ 優先級: P2
```

---

## 一、頁面級內容層次

### 1.1 Dashboard (首頁)

**內容優先級矩陣**:

```yaml
Level 1: 頁面頂部
  - component: 頁面標題
    content: "首頁" / "歡迎回來，[用戶名]"
    priority: P0
    placement: 左上角

  - component: 快捷操作
    content: [創建 Agent] [新建對話] [數據分析]
    priority: P0
    placement: 右上角

Level 2: 關鍵指標（KPI Cards）
  - component: API 調用量
    metrics:
      - 今日調用: "12,345 次"
      - 本月調用: "567,890 次"
      - 趨勢: +15% ↑
    priority: P0
    layout: 1/4 寬度

  - component: 活躍 Agents
    metrics:
      - 運行中: "8 個"
      - 總數: "23 個"
    priority: P0
    layout: 1/4 寬度

  - component: 系統健康
    metrics:
      - 狀態: ✅ 正常
      - 告警: "0 條"
    priority: P0
    layout: 1/4 寬度

  - component: 成本統計
    metrics:
      - 今日成本: "$12.34"
      - 本月成本: "$567.89"
      - 預算使用率: "65%"
    priority: P0 (David), P1 (Alex/Sarah)
    layout: 1/4 寬度

Level 3: 最近活動
  - component: 最近的 Agents
    content: 最近使用的 5 個 Agents（卡片列表）
    actions: [啟動/停止] [配置] [測試]
    priority: P0
    layout: 1/2 寬度

  - component: 最近的對話
    content: 最近的 5 個對話（列表）
    actions: [繼續對話] [查看詳情]
    priority: P0
    layout: 1/2 寬度

Level 4: 快捷入口
  - component: 功能快捷方式
    content: [數據分析] [SQL 查詢] [工作流執行]
    priority: P1
    layout: 全寬
```

**視覺層次**:
```
┌────────────────────────────────────────────────────────┐
│ Level 1: 首頁                          [創建Agent]     │ ← 頁面標題 + 主 CTA
├────────────────────────────────────────────────────────┤
│ Level 2: [API調用] [活躍Agents] [系統健康] [成本]     │ ← KPI 卡片
├─────────────────────────┬──────────────────────────────┤
│ Level 3: 最近的 Agents  │ 最近的對話                   │ ← 主要內容
│  ┌───────────────────┐  │  ┌────────────────────────┐ │
│  │ Agent 卡片 1      │  │  │ 對話 1                 │ │
│  │ Agent 卡片 2      │  │  │ 對話 2                 │ │
│  └───────────────────┘  │  └────────────────────────┘ │
├─────────────────────────┴──────────────────────────────┤
│ Level 4: [數據分析] [SQL查詢] [工作流執行]             │ ← 快捷入口
└────────────────────────────────────────────────────────┘
```

---

### 1.2 Agent 列表頁

**內容優先級矩陣**:

```yaml
Level 1: 頁面頂部
  - component: 頁面標題
    content: "Agents 管理"
    priority: P0

  - component: 主要操作
    content: [創建 Agent]（藍色按鈕）
    priority: P0

Level 2: 篩選與搜索工具
  - component: 搜索框
    placeholder: "搜索 Agent 名稱或描述"
    priority: P0

  - component: 快速篩選
    options: [全部] [運行中] [已停止]
    priority: P0

  - component: 高級篩選（可選）
    filters:
      - 標籤篩選
      - 創建時間範圍
      - 模型類型
    priority: P1
    default_state: 折疊

Level 3: Agent 列表
  - component: Agent 卡片 / 列表項
    content:
      - Agent 名稱（粗體）
      - 描述（1-2 行摘要）
      - 狀態指示器（運行中/已停止）
      - 關鍵指標（調用次數、響應時間）
    actions:
      - 主要: [啟動/停止] [測試]
      - 次要: [配置] [刪除]（更多菜單）
    priority: P0

Level 4: 分頁與排序
  - component: 分頁控制
    priority: P0

  - component: 排序選項
    options: [最近使用] [創建時間] [名稱]
    priority: P1
```

**內容密度控制**:
- **卡片視圖**: 最多顯示 12 個卡片/頁（3 列 × 4 行）
- **列表視圖**: 最多顯示 20 個列表項/頁
- **默認視圖**: 卡片視圖（視覺豐富，適合瀏覽）
- **切換選項**: 用戶可切換為列表視圖（信息密集，適合查找）

---

### 1.3 Agent 詳情頁

**內容優先級矩陣**:

```yaml
Level 1: 頁面頂部
  - component: Agent 名稱
    content: "GPT-4 客服助手"（可編輯）
    priority: P0

  - component: 狀態與操作
    content:
      - 狀態徽章: [🟢 運行中] / [🔴 已停止]
      - 主要操作: [啟動/停止] [測試對話]
      - 次要操作: [複製] [導出] [刪除]
    priority: P0

Level 2: 標籤頁導航
  - component: Tab Navigation
    tabs:
      - 📝 基本配置（默認激活）
      - 👤 Persona 配置
      - 🔧 能力配置
      - 🧪 測試與調試
      - 🚀 部署與發布
    priority: P0

Level 3: 標籤頁內容（以「基本配置」為例）
  - component: Agent 信息
    fields:
      - Agent 名稱（必填）
      - 描述（可選）
      - 標籤（可選）
    priority: P0

  - component: LLM 配置
    fields:
      - Provider（必選）
      - 模型（必選）+ 💡 推薦說明
      - Temperature（可選，默認 0.7）
      - Max Tokens（可選，默認 1000）
    priority: P0

  - component: 保存按鈕
    content: [取消] [保存更改]
    priority: P0
    placement: 表單底部 + 固定在頁面底部（滾動時）

Level 4: 輔助信息（側邊欄，可選）
  - component: Agent 統計
    metrics:
      - 總調用次數
      - 平均響應時間
      - 成功率
      - 最後使用時間
    priority: P1

  - component: 快速操作
    actions: [查看日誌] [性能分析] [成本報告]
    priority: P1
```

**標籤頁內容層次（通用模式）**:
```
每個標籤頁內容結構:
├─ Level 3a: 主要表單/配置區域
│  ├─ 必填字段（標*）
│  └─ 可選字段
│
├─ Level 3b: 預覽/測試區域（如適用）
│  └─ 實時預覽、測試結果
│
└─ Level 3c: 操作按鈕
   └─ [取消] [保存] 或 [上一步] [下一步]
```

---

### 1.4 創建 Agent（引導式流程）

**內容優先級矩陣**:

```yaml
Level 1: 流程頂部
  - component: 進度指示器
    content: "步驟 2/5: LLM 配置"
    visual: 進度條（40% 完成）
    priority: P0

  - component: 頁面標題
    content: "創建新 Agent - LLM 配置"
    priority: P0

Level 2: 步驟內容
  - component: 主要配置區域
    content: 當前步驟的表單字段
    priority: P0

  - component: 幫助提示（內聯）
    content: 💡 "推薦使用 GPT-4 以獲得最佳性能"
    priority: P0
    placement: 關鍵決策點附近

Level 3: 導航按鈕
  - component: 底部操作欄
    content:
      - [< 上一步]（次要按鈕，左側）
      - [下一步 >]（主要按鈕，右側）
      - [保存為草稿]（文字鏈接，中間）
    priority: P0
    placement: 固定在頁面底部

Level 4: 側邊欄（可選）
  - component: 配置預覽
    content: 已完成步驟的摘要
    priority: P1
    default_state: 桌面端顯示，移動端折疊
```

**引導流程設計原則**:
- **一次一個焦點**: 每步只問 1-3 個相關問題
- **漸進式披露**: 複雜選項默認折疊，提供「高級設置」入口
- **即時反饋**: 表單驗證錯誤即時顯示
- **可回退**: 允許用戶返回修改前面的步驟

---

### 1.5 對話界面（多模態聊天）

**內容優先級矩陣**:

```yaml
Level 1: 頂部工具欄
  - component: Agent 選擇器
    content: "GPT-4 客服助手" ▼
    priority: P0

  - component: 對話標題
    content: "我的對話"（可編輯）
    priority: P1

  - component: 操作按鈕
    content: [設置] [導出]
    priority: P1

Level 2: 聊天窗口（主要內容）
  - component: 消息列表
    content:
      - 用戶消息（右對齊，藍色氣泡）
      - Agent 回覆（左對齊，灰色氣泡）
    priority: P0
    layout: 垂直滾動，最新消息在底部

  - component: 消息內容類型
    types:
      - 文字（支持 Markdown 格式）
      - 代碼塊（語法高亮）
      - 圖表（交互式）
      - 表格（可排序、可篩選）
      - 圖片（可放大）
    priority: P0

  - component: 消息操作
    actions: [複製] [重新生成] [👍/👎 反饋]
    priority: P1
    placement: 懸停顯示

Level 3: 輸入區域
  - component: 文本輸入框
    features:
      - 多行輸入（自動擴展）
      - 快捷命令（/）
      - @ 提及（從知識庫）
    priority: P0

  - component: 附件與功能
    content:
      - 📎 上傳文件
      - 🖼️ 上傳圖片
      - 🔬 啟動 Code Interpreter
      - 📊 啟動 Text-to-SQL
    priority: P0

  - component: 發送按鈕
    content: [發送] (Enter) / [Shift+Enter 換行]
    priority: P0

Level 4: 右側邊欄（可折疊）
  - component: 對話信息
    content:
      - 消息數量
      - Token 使用量
      - 估算成本
    priority: P1

  - component: 上下文管理
    content: 當前上下文的消息列表（可選擇）
    priority: P2

  - component: 相關資源
    content: [使用的知識庫] [執行的代碼] [查詢的 SQL]
    priority: P2
```

**內容密度控制**:
- **消息顯示**: 最多顯示最近 50 條消息，更多需滾動加載
- **代碼塊長度**: 超過 20 行自動折疊，顯示「展開代碼」按鈕
- **圖表尺寸**: 默認適配窗口寬度，支持全屏查看

---

### 1.6 Code Interpreter 工作區

**內容優先級矩陣**:

```yaml
Level 1: 頂部工具欄
  - component: 工作區標題
    content: "數據分析工作區"
    priority: P0

  - component: 主要操作
    content: [執行] [停止] [導出報告]
    priority: P0

Level 2: 輸入區域（中央突出）
  - component: 自然語言輸入框
    placeholder: "請描述您的數據分析需求，例如：分析銷售數據的月度趨勢"
    priority: P0
    size: 大型輸入框（3-5 行）

  - component: 模板快捷方式
    content: [數據清洗] [趨勢分析] [相關性分析] [可視化]
    priority: P1

Level 3: 代碼與結果區域（上下分屏）
  - component: 代碼預覽區（上半部分）
    content:
      - 生成的 Python 代碼（語法高亮）
      - [編輯代碼] [執行] 按鈕
    priority: P0
    size: 30-40% 高度

  - component: 結果展示區（下半部分）
    content:
      - 執行狀態（正在執行... / 完成 / 錯誤）
      - 文本輸出
      - 表格（可交互）
      - 圖表（可縮放、可導出）
    priority: P0
    size: 60-70% 高度

Level 4: 左側邊欄（文件管理）
  - component: 上傳數據
    content: 拖放上傳區域
    priority: P1

  - component: 已上傳文件
    content: 文件列表（名稱、大小、操作）
    priority: P1

Level 5: 右側邊欄（歷史與導出）
  - component: 執行歷史
    content: 歷史記錄列表（最近 10 條）
    priority: P2

  - component: 導出選項
    content: [PDF] [PowerPoint] [Excel] [Jupyter Notebook]
    priority: P1
```

**視覺層次**:
```
┌─────────────────────────────────────────────────┐
│ Level 1: 數據分析工作區              [執行]    │
├─────────────────────────────────────────────────┤
│ Level 2: [輸入您的分析需求...]                  │ ← 大型輸入框
│          [數據清洗] [趨勢分析] [相關性分析]     │ ← 模板快捷方式
├─────────────────────────────────────────────────┤
│ Level 3a: 生成的代碼                            │ ← 代碼預覽（30%）
│   import pandas as pd                           │
│   df = pd.read_csv('sales.csv')                 │
│   ...                                           │
├─────────────────────────────────────────────────┤
│ Level 3b: 執行結果                              │ ← 結果展示（70%）
│   ┌─────────────────────────────────────────┐  │
│   │ [圖表: 銷售趨勢分析]                     │  │
│   │                                         │  │
│   │ [表格: 前 10 名產品]                     │  │
│   └─────────────────────────────────────────┘  │
└─────────────────────────────────────────────────┘
```

---

### 1.7 Multi-Agent Workflow 編輯器

**內容優先級矩陣**:

```yaml
Level 1: 頂部工具欄
  - component: 工作流名稱
    content: "客服處理流程"（可編輯）
    priority: P0

  - component: 主要操作
    content: [保存] [測試運行] [發布]
    priority: P0

Level 2: 畫布（中央主要區域）
  - component: 可視化編輯器
    content:
      - Agent 節點
      - 控制節點（開始、結束、分支）
      - 數據流連線
    priority: P0
    interaction:
      - 拖拽添加節點
      - 連接節點
      - 選中編輯
      - 縮放平移

  - component: 畫布工具欄
    content: [選擇] [拖動] [縮放+] [縮放-] [適應屏幕]
    priority: P0
    placement: 畫布左上角

Level 3: 左側工具面板
  - component: Agent 列表
    content: 可拖拽的 Agent 卡片
    priority: P0

  - component: 控制節點
    content: [開始] [結束] [條件] [並行] [循環]
    priority: P0

  - component: 模板庫
    content: 預定義工作流模板
    priority: P1

Level 4: 右側配置面板
  - component: 節點配置（選中節點時）
    content:
      - 節點名稱
      - Agent 選擇
      - 輸入/輸出映射
      - 錯誤處理
    priority: P0

  - component: 工作流配置（未選中節點時）
    content:
      - 觸發條件
      - 全局設置
    priority: P1

Level 5: 底部狀態欄
  - component: 工作流統計
    content: "5 個節點，3 個 Agents"
    priority: P2

  - component: 驗證狀態
    content: ✅ "工作流配置有效" / ❌ "發現 2 個錯誤"
    priority: P0
```

**視覺層次**:
```
┌──────────────────────────────────────────────────────────┐
│ Level 1: 客服處理流程              [保存] [測試] [發布]  │
├───────┬──────────────────────────────────────┬───────────┤
│Level 3│ Level 2: 可視化編輯器（畫布）          │ Level 4:  │
│       │                                      │ 節點配置  │
│Agents │  [開始] → [Agent 1] → [Agent 2]      │           │
│  📋   │     ↓                    ↓           │ 名稱:     │
│  🤖   │  [分支?] → [Agent 3] → [結束]        │ Agent 1   │
│  🔬   │                                      │           │
│       │                                      │ 輸入:     │
│控制節點│                                      │ {user_q}  │
│  ▶️   │                                      │           │
│  ⏹️   │                                      │ 輸出:     │
│  ◇    │                                      │ {answer}  │
├───────┴──────────────────────────────────────┴───────────┤
│ Level 5: 5 個節點，3 個 Agents | ✅ 工作流配置有效       │
└──────────────────────────────────────────────────────────┘
```

---

## 二、跨頁面內容模式

### 2.1 列表頁通用模式

**標準列表頁結構**:
```yaml
列表頁通用層次:
  Level 1: 頁面標題 + 主要操作
    - 頁面標題（左）
    - 創建按鈕（右，主要 CTA）

  Level 2: 篩選與搜索
    - 搜索框（全文搜索）
    - 快速篩選（常用條件）
    - 高級篩選（可折疊，更多條件）
    - 排序選項

  Level 3: 列表內容
    - 卡片視圖 / 列表視圖
    - 每項包含: 標題、摘要、關鍵指標、狀態、操作

  Level 4: 分頁與批量操作
    - 分頁控制
    - 批量選擇
    - 批量操作（刪除、導出等）
```

**適用頁面**:
- Agents 列表
- 對話列表
- 知識庫列表
- 工作流列表
- Persona 列表

---

### 2.2 詳情頁通用模式

**標準詳情頁結構**:
```yaml
詳情頁通用層次:
  Level 1: 實體標題 + 狀態 + 主要操作
    - 實體名稱（可編輯）
    - 狀態徽章
    - 主要操作按鈕
    - 次要操作（更多菜單）

  Level 2: 標籤頁導航
    - 3-7 個標籤頁
    - 圖標 + 文字標籤
    - 默認激活第一個標籤

  Level 3: 標籤頁內容
    - 表單 / 配置 / 展示內容
    - 保存/取消按鈕（表單模式）

  Level 4: 側邊欄（可選）
    - 統計信息
    - 快捷操作
    - 相關資源
```

**適用頁面**:
- Agent 詳情
- 對話詳情
- 知識庫詳情
- 工作流詳情
- Persona 詳情

---

### 2.3 創建/編輯表單通用模式

**標準表單結構**:
```yaml
表單通用層次:
  Level 1: 表單標題
    - 明確說明正在創建/編輯的對象

  Level 2: 表單內容
    - 分組組織（使用分隔線或標題）
    - 必填字段標*
    - 字段標籤 + 輸入框 + 幫助文本

  Level 3: 幫助與提示
    - 內聯提示（💡）
    - 字段級驗證錯誤
    - 推薦值說明

  Level 4: 操作按鈕
    - 主要操作（保存、創建）
    - 次要操作（取消、保存為草稿）
    - 固定在底部（滾動時可見）
```

**表單設計原則**:
- **邏輯分組**: 相關字段分組展示
- **漸進式披露**: 高級選項默認折疊
- **即時反饋**: 驗證錯誤即時顯示
- **清晰標籤**: 避免技術術語，使用用戶語言

---

## 三、內容優先級規則

### 3.1 優先級定義

```yaml
P0 (必須看到):
  - 定義: 用戶完成主要任務必需的信息
  - 特徵: 始終可見，不可折疊或隱藏
  - 示例:
    - 頁面標題
    - 主要操作按鈕
    - 核心內容區域
    - 狀態指示器
    - 錯誤提示

P1 (應該看到):
  - 定義: 重要但非關鍵的支持性信息
  - 特徵: 默認可見，可折疊到次要位置
  - 示例:
    - 次要操作
    - 統計信息
    - 篩選器
    - 輔助說明

P2 (可以看到):
  - 定義: 提供額外價值但非必需的信息
  - 特徵: 默認隱藏，用戶主動展開
  - 示例:
    - 高級設置
    - 詳細日誌
    - 歷史記錄
    - 幫助文檔鏈接
```

### 3.2 角色化優先級

**不同角色的優先級差異**:

```yaml
Alex (Developer):
  P0:
    - Agent 創建與配置
    - 測試與調試工具
    - 工作流編輯器
    - 代碼預覽與編輯
  P1:
    - 性能指標
    - API 文檔
    - 模板庫
  P2:
    - 成本統計
    - 用戶管理

Sarah (Analyst):
  P0:
    - Code Interpreter 輸入框
    - Text-to-SQL 查詢框
    - 結果展示（表格、圖表）
    - 導出功能
  P1:
    - 模板和範例
    - 執行歷史
    - 數據預覽
  P2:
    - 代碼編輯
    - 高級配置

David (Admin):
  P0:
    - 系統健康狀態
    - 成本追蹤
    - 用戶管理
    - 告警通知
  P1:
    - API 調用統計
    - 審計日誌
    - 權限配置
  P2:
    - 具體 Agent 配置
    - 對話詳情
```

---

## 四、信息密度控制

### 4.1 內容密度指南

**密度級別定義**:

```yaml
低密度（Sparse）:
  - 特徵: 大量留白，每屏 1-3 個元素
  - 適用場景:
    - 引導式流程
    - 關鍵決策頁面
    - 錯誤/成功狀態頁
  - 示例: 創建 Agent 步驟 1（基本信息）

中密度（Moderate）:
  - 特徵: 平衡的信息與留白，每屏 4-8 個元素
  - 適用場景:
    - 詳情頁
    - 表單頁
    - Dashboard
  - 示例: Agent 詳情頁（基本配置標籤）

高密度（Dense）:
  - 特徵: 信息密集，每屏 9+ 個元素
  - 適用場景:
    - 列表頁
    - 表格視圖
    - 監控 Dashboard
  - 示例: Agent 列表頁（列表視圖）
```

### 4.2 密度控制技術

**技術 1: 漸進式披露（Progressive Disclosure）**
```yaml
原則: 先展示基本信息，高級選項點擊展開

示例: Agent 創建 - LLM 配置
  默認顯示:
    - Provider 選擇
    - 模型選擇
    - Temperature（默認值 0.7）

  點擊「高級設置」展開:
    - Max Tokens
    - Top P
    - Frequency Penalty
    - Presence Penalty
```

**技術 2: 折疊面板（Accordion / Collapsible Sections）**
```yaml
原則: 分組內容，默認折疊非關鍵組

示例: Agent 詳情 - 能力配置
  默認展開:
    - ✅ Knowledge Base（已啟用）

  默認折疊:
    - ☐ Code Interpreter（未啟用）
    - ☐ Text-to-SQL（未啟用）
    - ☐ Web Search（未啟用）
```

**技術 3: 分頁與虛擬滾動**
```yaml
原則: 大量數據分批加載，避免一次性渲染

列表頁:
  - 分頁: 每頁 12-20 項
  - 無限滾動: 移動端優先

表格:
  - 虛擬滾動: 僅渲染可見行
  - 最大行數: 1000 行（超過建議導出或篩選）
```

**技術 4: 摘要與詳情**
```yaml
原則: 列表顯示摘要，點擊查看詳情

示例: Agent 列表卡片
  摘要視圖:
    - Agent 名稱
    - 描述（1 行，截斷...）
    - 狀態、調用次數

  詳情視圖（點擊卡片）:
    - 完整描述
    - 詳細統計
    - 配置摘要
    - 最近活動
```

---

## 五、內容關聯關係

### 5.1 實體關聯圖

```
實體關聯關係:

Agent
├─ 關聯 Persona (1:1)
├─ 關聯 Knowledge Bases (1:N)
├─ 關聯 Conversations (1:N)
├─ 屬於 Workflows (N:N)
└─ 生成 Logs (1:N)

Conversation
├─ 使用 Agent (N:1)
├─ 包含 Messages (1:N)
├─ 引用 Knowledge Base Items (N:N)
└─ 執行 Code (1:N)

Workflow
├─ 包含 Agents (N:N)
├─ 包含 Control Nodes (1:N)
├─ 生成 Executions (1:N)
└─ 使用 Templates (N:1)

Knowledge Base
├─ 包含 Documents (1:N)
├─ 被 Agents 使用 (N:N)
└─ 關聯 Retrieval Configs (1:1)
```

### 5.2 跨實體導航

**導航路徑設計**:

```yaml
從 Agent 詳情頁導航到:
  - Persona 詳情: 點擊 Persona 配置標籤 > 「查看 Persona」
  - 知識庫詳情: 能力配置 > Knowledge Base > 點擊知識庫名稱
  - 對話列表: 頂部操作欄 > 「查看對話」
  - 工作流列表: 側邊欄 > 「使用此 Agent 的工作流」

從對話詳情頁導航到:
  - Agent 詳情: 頂部 Agent 選擇器 > 點擊 Agent 名稱
  - 知識庫詳情: 右側邊欄 > 「相關資源」> 點擊知識庫
  - 代碼詳情: 右側邊欄 > 「執行的代碼」> 點擊代碼塊

從工作流詳情頁導航到:
  - Agent 詳情: 畫布節點 > 右鍵 > 「查看 Agent 詳情」
  - 執行歷史: 標籤頁 > 「執行歷史」
  - 性能分析: 標籤頁 > 「性能分析」
```

### 5.3 關聯信息展示

**內聯關聯（Inline Associations）**:
```yaml
定義: 在當前頁面直接顯示關聯信息摘要

示例: Agent 詳情頁 - Persona 配置標籤
  顯示內容:
    - Persona 名稱（可點擊跳轉）
    - Persona 風格描述（摘要）
    - 一致性評分
    - [編輯 Persona] 按鈕

優點: 無需離開當前頁面即可獲取關鍵信息
缺點: 信息有限，深度操作需跳轉
```

**側邊欄關聯（Sidebar Associations）**:
```yaml
定義: 在側邊欄顯示相關實體列表

示例: 對話詳情頁 - 右側邊欄
  相關資源:
    - 使用的知識庫（3 個）
      - 客服知識庫 →
      - 產品文檔 →
    - 執行的代碼（2 個）
      - 數據分析腳本 →
    - 查詢的 SQL（1 個）
      - 銷售查詢 →

優點: 方便快速訪問相關資源
缺點: 需要額外屏幕空間
```

**彈窗關聯（Modal Associations）**:
```yaml
定義: 點擊觸發彈窗，顯示關聯詳情

示例: Agent 列表頁 - 快速查看
  觸發: 點擊 Agent 卡片上的「👁️ 快速查看」
  顯示內容:
    - Agent 基本信息
    - 關聯 Persona 摘要
    - 最近 5 個對話
    - 關鍵統計指標
    - [查看完整詳情] 按鈕

優點: 快速預覽，無需完整頁面跳轉
缺點: 不適合深度操作
```

---

## 六、內容可訪問性

### 6.1 文字內容可訪問性

**字體大小規範**:
```yaml
字體大小層次:
  H1 (頁面標題): 32px (2rem)
  H2 (區域標題): 24px (1.5rem)
  H3 (組標題): 20px (1.25rem)
  H4 (子標題): 18px (1.125rem)
  Body (正文): 16px (1rem) ← 基準
  Small (輔助文字): 14px (0.875rem)
  Caption (說明文字): 12px (0.75rem)

行高（Line Height）:
  標題: 1.2-1.3
  正文: 1.5-1.6
  代碼: 1.4
```

**顏色對比度規範（WCAG 2.1 AA）**:
```yaml
對比度要求:
  正文文字: ≥4.5:1
  大文字（18px+ 或 14px+ 粗體）: ≥3:1
  圖標和圖形元素: ≥3:1

顏色使用原則:
  - 不僅依賴顏色傳達信息
  - 提供文字標籤或圖標輔助
  - 測試工具: WebAIM Contrast Checker
```

### 6.2 結構化內容

**語義化標記**:
```html
<!-- 使用語義化 HTML5 標籤 -->
<header>頁面頭部</header>
<nav>導航</nav>
<main>主要內容</main>
<aside>側邊欄</aside>
<footer>頁腳</footer>

<!-- 使用標題層次 -->
<h1>Agent 管理</h1>
  <h2>最近的 Agents</h2>
    <h3>GPT-4 客服助手</h3>
```

**ARIA 標籤**:
```html
<!-- 區域標籤 -->
<nav aria-label="主導航">...</nav>
<div role="search" aria-label="搜索">...</div>

<!-- 狀態標籤 -->
<span aria-live="polite">加載中...</span>

<!-- 描述性標籤 -->
<button aria-label="創建新 Agent">+</button>
```

### 6.3 多媒體內容

**圖片與圖標**:
```yaml
圖片替代文本:
  - 裝飾性圖片: alt=""
  - 信息性圖片: alt="描述性文字"
  - 功能性圖片: alt="功能說明"

圖標可訪問性:
  - 提供 aria-label
  - 配合文字標籤使用
  - 懸停顯示 Tooltip
```

**圖表與可視化**:
```yaml
圖表可訪問性:
  - 提供數據表格視圖
  - 提供文字摘要
  - 支持鍵盤導航
  - 高對比度模式

示例: 銷售趨勢圖
  - 默認: 交互式圖表
  - 切換: [表格視圖]
  - 摘要: "銷售額從 1 月的 $10K 增長到 12 月的 $50K，增長率 400%"
```

---

## 七、內容性能優化

### 7.1 內容加載策略

**優先級加載**:
```yaml
加載順序:
  1. 關鍵 CSS（內聯）
  2. 頁面結構 HTML
  3. 關鍵 JavaScript
  4. Above-the-fold 內容
  5. Below-the-fold 內容（懶加載）
  6. 非關鍵資源（圖片、字體等）
```

**懶加載技術**:
```yaml
圖片懶加載:
  - 使用 loading="lazy" 屬性
  - 僅加載視口內圖片
  - 使用佔位符（Placeholder）

組件懶加載:
  - React.lazy() 動態導入
  - 路由級別代碼分割
  - 標籤頁內容按需加載

數據懶加載:
  - 列表無限滾動
  - 分頁加載
  - 虛擬滾動（Virtual Scrolling）
```

### 7.2 內容緩存策略

**緩存層級**:
```yaml
瀏覽器緩存:
  - 靜態資源: 長期緩存（1 年）
  - API 響應: 短期緩存（5 分鐘）
  - 用戶偏好: LocalStorage

應用緩存:
  - Redux / Context 狀態管理
  - React Query / SWR 數據緩存
  - Service Worker 離線緩存

CDN 緩存:
  - 靜態文件: Edge 緩存
  - API 響應: 可選緩存（謹慎使用）
```

### 7.3 內容壓縮

**文本壓縮**:
```yaml
技術:
  - Gzip / Brotli 壓縮
  - 代碼混淆與壓縮（Minify）
  - Tree Shaking（移除未使用代碼）

預期效果:
  - HTML/CSS/JS: 壓縮 60-80%
  - API JSON 響應: 壓縮 40-60%
```

**圖片優化**:
```yaml
技術:
  - 使用 WebP / AVIF 格式
  - 響應式圖片（srcset）
  - 圖片尺寸優化
  - 圖片壓縮（TinyPNG, ImageOptim）

預期效果:
  - WebP vs PNG/JPG: 減少 25-35% 文件大小
  - 正確尺寸: 避免加載過大圖片
```

---

## 八、內容國際化 (i18n)

### 8.1 多語言支持

**語言優先級**:
```yaml
第一階段（MVP）:
  - 繁體中文（zh-TW）← 主要語言
  - 簡體中文（zh-CN）

第二階段:
  - 英文（en-US）
  - 日文（ja-JP）

語言切換:
  - 位置: 用戶菜單 > 語言偏好
  - 保存: 用戶設置（持久化）
  - 默認: 瀏覽器語言偵測
```

### 8.2 文本外部化

**i18n 鍵命名規範**:
```yaml
命名模式: {模組}.{頁面}.{組件}.{鍵}

示例:
  agents.list.header.title: "Agents 管理"
  agents.list.header.create_button: "創建 Agent"
  agents.detail.tabs.basic_config: "基本配置"
  agents.detail.tabs.persona: "Persona 配置"
```

### 8.3 內容適配

**文本長度適配**:
```yaml
挑戰: 不同語言文本長度差異大

解決方案:
  - UI 組件支持文本溢出處理
  - 使用 flexbox/grid 彈性佈局
  - 關鍵按鈕預留 150% 空間
  - 長文本截斷 + Tooltip

示例:
  英文: "Create"（6 字符）
  繁中: "創建"（2 字符）
  德文: "Erstellen"（9 字符）
```

**日期時間格式化**:
```yaml
格式:
  繁體中文: 2025 年 10 月 29 日
  簡體中文: 2025 年 10 月 29 日
  英文: October 29, 2025
  日文: 2025年10月29日

實現: 使用 Intl.DateTimeFormat API
```

---

## 總結

### 內容層次設計原則

1. **任務優先**: 內容組織支持用戶任務流程
2. **漸進披露**: 逐步展示信息，避免一次性過載
3. **視覺層次**: 使用大小、顏色、位置建立清晰層次
4. **一致性**: 跨頁面使用統一的內容模式
5. **可訪問性**: 符合 WCAG 2.1 AA 標準
6. **性能優先**: 優化加載速度和渲染性能

### 實施檢查清單

**設計階段**:
- [ ] 定義每個頁面的內容優先級（P0/P1/P2）
- [ ] 設計內容佈局和視覺層次
- [ ] 規劃漸進式披露策略
- [ ] 定義角色化內容差異

**開發階段**:
- [ ] 實現內容層次結構
- [ ] 實現懶加載和分頁
- [ ] 實現可訪問性標記（ARIA）
- [ ] 實現響應式適配

**測試階段**:
- [ ] 可訪問性測試（屏幕閱讀器、鍵盤導航）
- [ ] 性能測試（加載速度、渲染性能）
- [ ] 不同角色用戶測試
- [ ] 不同設備和屏幕尺寸測試

---

**文檔版本**: 1.0
**最後更新**: 2025-10-29
**維護者**: UI/UX Designer
**相關文檔**: [Site Map](sitemap.md), [Navigation Structure](navigation-structure.md)
