# 對話界面 Wireframe

**版本**: 1.0.0
**日期**: 2025-10-29
**狀態**: ✅ 已完成
**複雜度**: ⭐⭐⭐ (中等)

---

## 📋 頁面概覽

### 目的
提供流暢的 Agent 對話體驗，支援多模態輸入、Function calling 可視化、對話分支管理和歷史紀錄。

### 目標用戶
- **Alex（IT 開發者）**: 測試 Agent 功能、除錯 Function calls、查看執行細節
- **Sarah（業務分析師）**: 透過對話獲取數據分析、產生報告、日常業務查詢
- **David（企業管理員）**: 監控對話品質、管理對話歷史、匯出對話記錄

### 成功指標
- 消息發送延遲 < 100ms
- Agent 回覆首字延遲 < 2s（streaming）
- 檔案上傳成功率 > 99%
- 對話載入時間 < 500ms
- 用戶滿意度評分 > 4.5/5

---

## 🖥️ Desktop 版本佈局（≥1280px）

### 完整 3-Column 佈局

```
┌─────────────────────────────────────────────────────────────────────────────────────┐
│ ← Sidebar   [🤖 Customer Service Bot v1.2.0] [Agent 名稱]          ⚙️ 🔔 👤 Alex   │ Top Bar
├──────────────┬───────────────────────────────────────────────────┬──────────────────┤
│              │                                                   │                  │
│ 📝 對話歷史  │              🗨️ 對話消息流                        │  📊 Agent 資訊   │
│              │                                                   │                  │
│ ┌──────────┐│                                                   │ ┌──────────────┐│
│ │🔍 搜尋... ││  ┌─────────────────────────────────────────────┐ │ │ 🤖 Agent     ││
│ └──────────┘│  │ 👤 Alex Chen            Today 09:15          │ │ │ Customer Ser.││
│              │  │                                             │ │ │              ││
│ + 新對話     │  │ 請幫我查詢最近 7 天的訂單統計                │ │ │ 🟢 運行中    ││
│              │  │                                             │ │ │ GPT-4        ││
│ 📅 今天      │  └─────────────────────────────────────────────┘ │ │ Temp: 0.7    ││
│              │                                                   │ └──────────────┘│
│ • 訂單統計查詢│  ┌─────────────────────────────────────────────┐ │                  │
│   09:15      │  │ 🤖 Customer Service Bot   Today 09:15      │ │ 🧠 上下文       │
│              │  │                                             │ │                  │
│ • 客戶投訴處理│  │ 好的，我來為您查詢最近 7 天的訂單統計...     │ │ Token 使用量    │
│   08:30      │  │                                             │ │ ┌──────────────┐│
│              │  │ 🔧 正在調用函數：get_order_statistics()      │ │ │ 2,456 / 8,000││
│ 📅 昨天      │  │   ├─ start_date: "2025-10-22"              │ │ │ ████████░░░░ ││
│              │  │   ├─ end_date: "2025-10-29"                │ │ │ 30.7%        ││
│ • 產品推薦   │  │   └─ status: "completed"                   │ │ └──────────────┘│
│   Yesterday  │  │                                             │ │                  │
│              │  │ 📊 查詢結果：                                │ │ 📎 附件 (2)     │
│ • 退貨流程   │  │ • 總訂單數：1,245 筆 (+18% vs 上週)          │ │ • sales_data.csv│
│   Yesterday  │  │ • 總金額：$45,678 (+12% vs 上週)            │ │ • chart.png     │
│              │  │ • 平均訂單價值：$36.7                       │ │                  │
│ 📅 本週      │  │                                             │ │ 🌲 對話分支     │
│              │  │                 [📋 複製] [🔄 重試] [✏️ 編輯]  │ │                  │
│ • 庫存盤點   │  └─────────────────────────────────────────────┘ │ ┌──────────────┐│
│   Oct 27     │                                                   │ │ Main (當前)  ││
│              │  ┌─────────────────────────────────────────────┐ │ │ └─ Branch A  ││
│ • 價格調整方案│  │ 👤 Alex Chen            Today 09:16          │ │ │ └─ Branch B  ││
│   Oct 26     │  │                                             │ │ └──────────────┘│
│              │  │ 可以幫我產生圖表嗎？                         │ │                  │
│ 📅 本月      │  │                         📎 chart_config.json │ │ ⚡ 快捷指令    │
│              │  └─────────────────────────────────────────────┘ │                  │
│ • 月度報告   │                                                   │ /help - 幫助    │
│   Oct 15     │  ┌─────────────────────────────────────────────┐ │ /clear - 清空   │
│              │  │ 🤖 Customer Service Bot   Today 09:16      │ │ /export - 匯出  │
│ • Q3 數據分析│  │                                             │ │ /retry - 重試   │
│   Oct 10     │  │ 💬 正在輸入...                               │ │                  │
│              │  │                                             │ │                  │
│ ⋮            │  └─────────────────────────────────────────────┘ │                  │
│              │                                                   │                  │
│ [280px 寬]   │                 [主要內容區域]                    │  [320px 寬]      │
│              │                                                   │                  │
├──────────────┴───────────────────────────────────────────────────┴──────────────────┤
│                          💬 輸入區域                                                 │
│ ┌─────────────────────────────────────────────────────────────────────────────────┐│
│ │ 📎  輸入您的消息... (支援 Markdown)                            🎤 📷 📁  [🔧 /]  ││
│ │                                                                                   ││
│ │                                                                          [⬆️ 發送] ││
│ └─────────────────────────────────────────────────────────────────────────────────┘│
│ Shift+Enter 換行 • /指令 快捷操作 • @ 提及附件                       Token: 2,456   │
└─────────────────────────────────────────────────────────────────────────────────────┘
```

---

## 📊 內容層級（Desktop）

### Level 1: 頁面結構（3-Column Layout）
```
┌─────────────┬──────────────────────┬─────────────┐
│ 左側欄      │ 中央對話區           │ 右側欄      │
│ (280px)     │ (主要內容)           │ (320px)     │
│             │                      │             │
│ • 對話歷史  │ • 消息流             │ • Agent資訊 │
│ • 搜尋      │ • Function calls     │ • 上下文    │
│ • 分類      │ • 輸入區域           │ • 附件      │
│             │                      │ • 分支管理  │
└─────────────┴──────────────────────┴─────────────┘
```

### Level 2: 左側欄 - 對話歷史

#### 搜尋與新建
```
┌─────────────────────────────┐
│ 🔍 搜尋對話...              │ ← 即時搜尋（debounce 300ms）
└─────────────────────────────┘

[ + 新對話 ]                   ← Primary Button
```

#### 時間分組列表
```
📅 今天                         ← 時間分隔符

┌─────────────────────────────┐
│ • 訂單統計查詢               │ ← 對話標題（自動提取）
│   09:15          [⋮]        │   時間戳 + 更多選單
└─────────────────────────────┘
  └─ 點擊：載入對話
  └─ Hover：顯示預覽
  └─ [⋮]：重命名、刪除、匯出

┌─────────────────────────────┐
│ • 客戶投訴處理               │
│   08:30          [⋮]        │
└─────────────────────────────┘

📅 昨天

┌─────────────────────────────┐
│ • 產品推薦                   │
│   Yesterday      [⋮]        │
└─────────────────────────────┘

📅 本週
⋮

📅 本月
⋮

[ 載入更多... ]                ← Infinite scroll
```

#### 對話項目狀態
```
• 正常對話
🔒 已鎖定對話（唯讀）
⭐ 已標記為重要
📌 已固定在頂部
```

### Level 3: 中央對話區 - 消息流

#### 用戶消息氣泡
```
┌─────────────────────────────────────────────────┐
│ 👤 Alex Chen            Today 09:15             │ ← 發送者 + 時間戳
│                                                 │
│ 請幫我查詢最近 7 天的訂單統計                    │ ← 消息內容（支援 Markdown）
│                                                 │
│                 [📋 複製] [✏️ 編輯] [🗑️ 刪除]     │ ← Hover 顯示操作按鈕
└─────────────────────────────────────────────────┘
```

#### Agent 回覆氣泡（基本）
```
┌─────────────────────────────────────────────────┐
│ 🤖 Customer Service Bot   Today 09:15           │
│                                                 │
│ 好的，我來為您查詢最近 7 天的訂單統計...         │
│                                                 │
│                 [📋 複製] [🔄 重試] [👍] [👎]     │ ← 操作 + 評分按鈕
└─────────────────────────────────────────────────┘
```

#### Agent 回覆氣泡（Function Calling）
```
┌─────────────────────────────────────────────────┐
│ 🤖 Customer Service Bot   Today 09:15           │
│                                                 │
│ 好的，我來為您查詢最近 7 天的訂單統計...         │
│                                                 │
│ 🔧 正在調用函數：get_order_statistics()         │ ← Function call 展開區塊
│   ┌─────────────────────────────────────────┐  │
│   │ 📥 輸入參數：                            │  │
│   │   ├─ start_date: "2025-10-22"          │  │
│   │   ├─ end_date: "2025-10-29"            │  │
│   │   └─ status: "completed"               │  │
│   │                                         │  │
│   │ 📤 返回結果：                            │  │
│   │   {                                     │  │
│   │     "total_orders": 1245,              │  │
│   │     "total_amount": 45678,             │  │
│   │     "avg_order_value": 36.7            │  │
│   │   }                                     │  │
│   │                                         │  │
│   │ ⏱️ 執行時間：1.24s                       │  │
│   │ ✅ 狀態：成功                            │  │
│   └─────────────────────────────────────────┘  │
│                                                 │
│ 📊 查詢結果：                                    │
│ • 總訂單數：1,245 筆 (+18% vs 上週)              │
│ • 總金額：$45,678 (+12% vs 上週)                │
│ • 平均訂單價值：$36.7                           │
│                                                 │
│                 [📋 複製] [🔄 重試] [👍] [👎]     │
└─────────────────────────────────────────────────┘
```

#### Agent 回覆氣泡（Streaming 中）
```
┌─────────────────────────────────────────────────┐
│ 🤖 Customer Service Bot   Today 09:16           │
│                                                 │
│ 💬 正在輸入...                                   │ ← 打字動畫
│ ▋                                               │ ← 光標閃爍
│                                                 │
└─────────────────────────────────────────────────┘
```

#### 用戶消息（含附件）
```
┌─────────────────────────────────────────────────┐
│ 👤 Alex Chen            Today 09:16             │
│                                                 │
│ 可以幫我產生圖表嗎？                             │
│                                                 │
│ 📎 附件：                                        │
│ ┌─────────────────────────────────────────────┐│
│ │ 📄 chart_config.json       2.4 KB           ││
│ │ [👁️ 預覽] [⬇️ 下載]                          ││
│ └─────────────────────────────────────────────┘│
│                                                 │
│                 [📋 複製] [✏️ 編輯] [🗑️ 刪除]     │
└─────────────────────────────────────────────────┘
```

#### 系統消息
```
┌─────────────────────────────────────────────────┐
│        ⓘ 對話已於 09:20 自動儲存                 │ ← 居中顯示
└─────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────┐
│        🕐 對話已持續 30 分鐘，已使用 2,456 tokens │
└─────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────┐
│        ⚠️ Agent 回應超時，請重試                 │
└─────────────────────────────────────────────────┘
```

#### 對話分支點
```
                    ╭─────────────╮
                    │  分支 A     │ ← 點擊切換到分支
                    ╰─────────────╯
                          │
┌───────────────────────┐ │
│ 原始消息...           │─┤
└───────────────────────┘ │
                          │
                    ╭─────────────╮
                    │  分支 B     │
                    ╰─────────────╯
```

### Level 4: 輸入區域

#### 輸入框結構
```
┌─────────────────────────────────────────────────────────────────┐
│ 📎  輸入您的消息... (支援 Markdown)              🎤 📷 📁  [🔧 /] │ ← Toolbar
│                                                                 │
│ [多行文本輸入區域，支援自動擴展]                                  │ ← Auto-resize
│                                                                 │
│                                                        [⬆️ 發送] │ ← 發送按鈕
└─────────────────────────────────────────────────────────────────┘
Shift+Enter 換行 • /指令 快捷操作 • @ 提及附件         Token: 2,456
```

#### Toolbar 按鈕
```
📎  ← 附加檔案（點擊打開選擇器）
🎤  ← 語音輸入（點擊開始錄音）
📷  ← 拍照/上傳圖片
📁  ← 選擇檔案
🔧  ← Function 工具選單
/   ← 快捷指令選單
```

#### 快捷指令彈出選單（輸入 / 觸發）
```
┌─────────────────────────────────────┐
│ 🔍 搜尋指令...                      │
├─────────────────────────────────────┤
│ /help      顯示幫助信息             │
│ /clear     清空當前對話             │
│ /export    匯出對話記錄             │
│ /retry     重新生成上一個回覆       │
│ /branch    創建新的對話分支         │
│ /model     切換 LLM 模型            │
│ /temp      調整 Temperature         │
└─────────────────────────────────────┘
```

#### @ 提及附件選單
```
┌─────────────────────────────────────┐
│ 🔍 搜尋附件...                      │
├─────────────────────────────────────┤
│ 📄 sales_data.csv      (2.4 MB)    │
│ 📊 chart.png           (156 KB)    │
│ 📝 report.pdf          (1.2 MB)    │
└─────────────────────────────────────┘
```

#### 輸入狀態指示
```
狀態 1: 空閒
[ 輸入您的消息... ]                    Token: 0

狀態 2: 輸入中
[ 請幫我查詢最近 7 天的訂單統計▋ ]      Token: 45

狀態 3: 發送中
[ 發送中... ] [⏳]                     ← 禁用狀態

狀態 4: 等待回覆
[ Agent 正在回覆... ] [⏳]             ← 禁用狀態，顯示 Stop 按鈕

狀態 5: Token 超限警告
[ 輸入您的消息... ]
⚠️ Token 使用量已達 90% (7,200/8,000)  ← 警告提示
```

### Level 5: 右側欄 - Agent 資訊與上下文

#### Agent 資訊卡片
```
┌──────────────────────────────────┐
│ 🤖 Agent                         │
│ Customer Service Bot             │ ← Agent 名稱
│                                  │
│ 🟢 運行中                        │ ← 狀態指示
│ GPT-4                            │ ← 模型
│ Temp: 0.7                        │ ← Temperature
│                                  │
│ [ 查看詳情 ]                     │ ← 連結到 Agent Detail 頁
└──────────────────────────────────┘
```

#### Token 使用量監控
```
┌──────────────────────────────────┐
│ 🧠 上下文                        │
│                                  │
│ Token 使用量                     │
│ ┌────────────────────────────┐  │
│ │ 2,456 / 8,000              │  │
│ │ ████████░░░░░░░░            │  │ ← 進度條
│ │ 30.7%                      │  │
│ └────────────────────────────┘  │
│                                  │
│ 📊 Token 分布：                  │
│ • 系統提示：  456 (18.6%)       │
│ • 對話歷史：1,234 (50.2%)       │
│ • 當前消息：  766 (31.2%)       │
│                                  │
│ [ 清理歷史 ]                     │ ← 釋放 Token
└──────────────────────────────────┘
```

#### 附件管理
```
┌──────────────────────────────────┐
│ 📎 附件 (2)                      │
│                                  │
│ ┌────────────────────────────┐  │
│ │ 📄 sales_data.csv          │  │
│ │ 2.4 MB                     │  │
│ │ [👁️] [⬇️] [🗑️]              │  │ ← 預覽/下載/刪除
│ └────────────────────────────┘  │
│                                  │
│ ┌────────────────────────────┐  │
│ │ 📊 chart.png               │  │
│ │ 156 KB                     │  │
│ │ [👁️] [⬇️] [🗑️]              │  │
│ └────────────────────────────┘  │
│                                  │
│ [ + 上傳附件 ]                   │
└──────────────────────────────────┘
```

#### 對話分支管理
```
┌──────────────────────────────────┐
│ 🌲 對話分支                      │
│                                  │
│ ┌────────────────────────────┐  │
│ │ Main (當前)                │  │ ← 主分支，高亮顯示
│ │ └─ Branch A                │  │ ← 子分支
│ │     ├─ Branch A-1          │  │
│ │     └─ Branch A-2          │  │
│ │ └─ Branch B                │  │
│ └────────────────────────────┘  │
│                                  │
│ [ + 創建分支 ]                   │
└──────────────────────────────────┘
```

#### 快捷指令列表
```
┌──────────────────────────────────┐
│ ⚡ 快捷指令                      │
│                                  │
│ /help     - 顯示幫助             │
│ /clear    - 清空對話             │
│ /export   - 匯出記錄             │
│ /retry    - 重試回覆             │
│ /branch   - 創建分支             │
│ /model    - 切換模型             │
│                                  │
│ [ 查看全部 ]                     │
└──────────────────────────────────┘
```

---

## 📱 Tablet 版本佈局（768px - 1279px）

### 2-Column Layout（側欄可折疊）

```
┌─────────────────────────────────────────────────────────────────┐
│ ☰  [🤖 Customer Service Bot]              ⚙️ 🔔 👤 Alex        │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│                    🗨️ 對話消息流                                │
│                                                                 │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │ 👤 Alex Chen            Today 09:15                      │  │
│  │ 請幫我查詢最近 7 天的訂單統計                             │  │
│  └──────────────────────────────────────────────────────────┘  │
│                                                                 │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │ 🤖 Customer Service Bot   Today 09:15                    │  │
│  │                                                          │  │
│  │ 好的，我來為您查詢...                                     │  │
│  │                                                          │  │
│  │ 🔧 正在調用函數：get_order_statistics()                  │  │
│  │   [展開查看詳細參數...]                                   │  │
│  │                                                          │  │
│  │ 📊 查詢結果：                                             │  │
│  │ • 總訂單數：1,245 筆 (+18%)                               │  │
│  │ • 總金額：$45,678 (+12%)                                 │  │
│  └──────────────────────────────────────────────────────────┘  │
│                                                                 │
│  ⋮                                                              │
│                                                                 │
├─────────────────────────────────────────────────────────────────┤
│ 💬 輸入區域                                                     │
│ ┌─────────────────────────────────────────────────────────────┐│
│ │ 📎  輸入您的消息...                    🎤 📷 📁        [⬆️]  ││
│ └─────────────────────────────────────────────────────────────┘│
└─────────────────────────────────────────────────────────────────┘
```

### 左側滑出選單（點擊 ☰ 觸發）

```
┌──────────────────────┐
│ 📝 對話歷史          │
│                      │
│ ┌──────────────────┐│
│ │ 🔍 搜尋...       ││
│ └──────────────────┘│
│                      │
│ [ + 新對話 ]         │
│                      │
│ 📅 今天              │
│ • 訂單統計查詢       │
│ • 客戶投訴處理       │
│                      │
│ 📅 昨天              │
│ • 產品推薦           │
│                      │
│ ⋮                    │
│                      │
│ [關閉 ✕]             │
└──────────────────────┘
```

### 右側滑出面板（點擊 Agent 名稱觸發）

```
                      ┌──────────────────────┐
                      │ 📊 Agent 資訊        │
                      │                      │
                      │ 🤖 Customer Service  │
                      │ 🟢 運行中            │
                      │ GPT-4 • Temp: 0.7    │
                      │                      │
                      │ ──────────────────   │
                      │                      │
                      │ 🧠 Token: 2,456/8K   │
                      │ ████████░░░░ 30.7%   │
                      │                      │
                      │ ──────────────────   │
                      │                      │
                      │ 📎 附件 (2)          │
                      │ • sales_data.csv     │
                      │ • chart.png          │
                      │                      │
                      │ [關閉 ✕]             │
                      └──────────────────────┘
```

---

## 📱 Mobile 版本佈局（<768px）

### 全屏對話視圖

```
┌────────────────────────────────┐
│ ← [Customer Service Bot]  ⋮   │ Top Bar (Fixed)
├────────────────────────────────┤
│                                │
│  🗨️ 消息流（全屏）              │
│                                │
│ ┌────────────────────────────┐│
│ │ 👤 Alex        09:15       ││
│ │ 請幫我查詢最近 7 天         ││
│ │ 的訂單統計                  ││
│ └────────────────────────────┘│
│                                │
│ ┌────────────────────────────┐│
│ │ 🤖 Bot         09:15       ││
│ │                            ││
│ │ 好的，我來為您查詢...       ││
│ │                            ││
│ │ 🔧 get_order_statistics()  ││
│ │ [展開]                      ││
│ │                            ││
│ │ 📊 查詢結果：               ││
│ │ • 總訂單數：1,245 筆        ││
│ │ • 總金額：$45,678          ││
│ │                            ││
│ │ [複製] [重試]               ││
│ └────────────────────────────┘│
│                                │
│  ⋮                             │
│                                │
├────────────────────────────────┤
│ 💬 輸入區域 (Fixed Bottom)     │
│ ┌────────────────────────────┐│
│ │ 輸入...       📎 🎤   [⬆️]  ││
│ └────────────────────────────┘│
└────────────────────────────────┘
```

### 底部選單（點擊 ⋮ 觸發）

```
┌────────────────────────────────┐
│ 📝 對話歷史                    │
│ 📊 Agent 資訊                  │
│ 📎 附件管理                    │
│ 🌲 對話分支                    │
│ ⚙️ 對話設定                    │
│ 📤 匯出對話                    │
│ ─────────────────────          │
│ ✕ 關閉                         │
└────────────────────────────────┘
```

---

## 🎯 互動元素詳細說明

### 1. 消息操作

#### 用戶消息操作
```
Hover 顯示：
┌─────────────────────────────────────────┐
│ 👤 Alex Chen            Today 09:15     │
│                                         │
│ 請幫我查詢最近 7 天的訂單統計            │
│                                         │
│         [📋 複製] [✏️ 編輯] [🗑️ 刪除]     │ ← Hover 顯示
└─────────────────────────────────────────┘

操作說明：
• 📋 複製：複製消息內容到剪貼簿
• ✏️ 編輯：進入編輯模式（編輯後自動創建分支）
• 🗑️ 刪除：刪除消息（顯示確認對話框）
```

#### Agent 回覆操作
```
Hover 顯示：
┌─────────────────────────────────────────┐
│ 🤖 Customer Service Bot   Today 09:15   │
│                                         │
│ 好的，我來為您查詢...                    │
│                                         │
│         [📋 複製] [🔄 重試] [👍] [👎]     │ ← Hover 顯示
└─────────────────────────────────────────┘

操作說明：
• 📋 複製：複製 Agent 回覆內容
• 🔄 重試：使用相同提示詞重新生成回覆
• 👍 好評：標記為有用的回覆（用於改進）
• 👎 差評：標記為無用的回覆（顯示反饋表單）
```

### 2. 編輯消息流程

```
Step 1: 點擊編輯按鈕
┌─────────────────────────────────────────┐
│ 👤 Alex Chen            Today 09:15     │
│                                         │
│ 請幫我查詢最近 7 天的訂單統計▋           │ ← 進入編輯模式
│                                         │
│         [✓ 儲存] [✕ 取消]                │
└─────────────────────────────────────────┘

Step 2: 修改內容後儲存
┌─────────────────────────────────────────┐
│ ⓘ 是否創建新的對話分支？                │
│                                         │
│ [ 創建分支 ]  [ 覆蓋原消息 ]             │
└─────────────────────────────────────────┘

Step 3: 創建分支
                    ╭─────────────╮
                    │ 編輯後分支   │ ← 新分支
                    ╰─────────────╯
                          │
┌───────────────────────┐ │
│ 原始消息...           │─┤
└───────────────────────┘ │
                          │
                    ╭─────────────╮
                    │ 主對話      │ ← 原分支
                    ╰─────────────╯
```

### 3. Function Calling 可視化

#### 折疊/展開控制
```
折疊狀態（預設）：
┌─────────────────────────────────────────┐
│ 🔧 正在調用函數：get_order_statistics()  │
│ [▶️ 展開查看詳細資訊]                     │
└─────────────────────────────────────────┘

展開狀態：
┌─────────────────────────────────────────┐
│ 🔧 正在調用函數：get_order_statistics()  │
│ [▼ 收起詳細資訊]                         │
│   ┌─────────────────────────────────┐   │
│   │ 📥 輸入參數：                    │   │
│   │   {                             │   │
│   │     "start_date": "2025-10-22", │   │
│   │     "end_date": "2025-10-29",   │   │
│   │     "status": "completed"       │   │
│   │   }                             │   │
│   │                                 │   │
│   │ 📤 返回結果：                    │   │
│   │   {                             │   │
│   │     "total_orders": 1245,       │   │
│   │     "total_amount": 45678       │   │
│   │   }                             │   │
│   │                                 │   │
│   │ ⏱️ 執行時間：1.24s               │   │
│   │ ✅ 狀態：成功                    │   │
│   │                                 │   │
│   │ [📋 複製參數] [📋 複製結果]       │   │
│   └─────────────────────────────────┘   │
└─────────────────────────────────────────┘
```

#### Function Call 狀態
```
⏳ 執行中：
🔧 正在調用函數：get_order_statistics()
   ⏳ 執行中... (0.5s)

✅ 成功：
🔧 調用函數：get_order_statistics()
   ✅ 成功 (1.24s)

❌ 失敗：
🔧 調用函數：get_order_statistics()
   ❌ 錯誤：Invalid date format
   [🔄 重試]
```

### 4. 對話分支管理

#### 分支樹視圖
```
┌──────────────────────────────────┐
│ 🌲 對話分支                      │
│                                  │
│ Main (當前) ⭐                   │ ← 主分支，高亮
│ ├─ 訂單查詢優化 💡               │ ← 優化版本
│ │  └─ 加入圖表 📊               │ ← 進一步改進
│ └─ 簡化版查詢 📝                 │ ← 另一種方案
│                                  │
│ [ + 從當前位置創建分支 ]         │
└──────────────────────────────────┘

操作：
• 點擊分支名稱：切換到該分支
• Hover 分支：顯示預覽和操作按鈕
• [⋮]：重命名、刪除、合併分支
```

#### 創建分支流程
```
Step 1: 點擊「創建分支」
┌────────────────────────────────┐
│ 🌲 創建新分支                  │
│                                │
│ 分支名稱：                     │
│ ┌────────────────────────────┐│
│ │ 訂單查詢優化               ││
│ └────────────────────────────┘│
│                                │
│ 從哪個消息開始分支？           │
│ ○ 最新消息                    │
│ ● 選擇的消息 (09:15)          │ ← 可選擇分支點
│                                │
│ [ 創建 ]  [ 取消 ]             │
└────────────────────────────────┘

Step 2: 分支創建成功
┌────────────────────────────────┐
│ ✅ 分支「訂單查詢優化」已創建   │
│                                │
│ [ 切換到新分支 ]  [ 留在主分支 ]│
└────────────────────────────────┘
```

### 5. 檔案上傳與附件管理

#### 上傳流程
```
Step 1: 點擊附件按鈕
┌────────────────────────────────┐
│ 📎 選擇附件類型                │
│                                │
│ 📄 文件 (.pdf, .docx, .txt)    │
│ 📊 數據 (.csv, .xlsx, .json)   │
│ 📷 圖片 (.jpg, .png, .gif)     │
│ 📹 影片 (.mp4, .mov)           │
│ 📁 資料夾 (壓縮上傳)           │
└────────────────────────────────┘

Step 2: 選擇檔案
[系統檔案選擇器開啟]

Step 3: 上傳進度
┌────────────────────────────────┐
│ ⬆️ 上傳中...                   │
│                                │
│ 📄 sales_data.csv              │
│ ████████████░░░░░░ 65%         │ ← 進度條
│ 1.6 MB / 2.4 MB                │
│                                │
│ [ 取消 ]                       │
└────────────────────────────────┘

Step 4: 上傳完成
┌────────────────────────────────┐
│ ✅ 上傳成功                    │
│                                │
│ 📄 sales_data.csv (2.4 MB)     │
│                                │
│ 是否要分析此檔案？             │
│ [ 開始分析 ]  [ 稍後 ]         │
└────────────────────────────────┘
```

#### 附件預覽
```
圖片預覽：
┌────────────────────────────────┐
│ 👁️ chart.png                   │
│                                │
│ ┌────────────────────────────┐│
│ │                            ││
│ │   [圖片預覽顯示]            ││
│ │                            ││
│ └────────────────────────────┘│
│                                │
│ 156 KB • 1920x1080             │
│                                │
│ [⬇️ 下載] [🔗 複製連結] [✕ 關閉]│
└────────────────────────────────┘

CSV 預覽：
┌────────────────────────────────┐
│ 👁️ sales_data.csv              │
│                                │
│ ┌────────────────────────────┐│
│ │ Date       | Sales | Region ││
│ │ 2025-10-22 | 1,234 | North  ││
│ │ 2025-10-23 | 2,345 | South  ││
│ │ ...                         ││
│ └────────────────────────────┘│
│                                │
│ 2.4 MB • 1,245 rows            │
│                                │
│ [⬇️ 下載] [📊 分析] [✕ 關閉]   │
└────────────────────────────────┘
```

### 6. 語音輸入

#### 語音錄製流程
```
Step 1: 點擊麥克風按鈕
┌────────────────────────────────┐
│ 🎤 語音輸入                    │
│                                │
│ 請允許瀏覽器使用麥克風         │
│                                │
│ [ 允許 ]  [ 拒絕 ]             │
└────────────────────────────────┘

Step 2: 開始錄音
┌────────────────────────────────┐
│ 🎤 正在錄音...                 │
│                                │
│     ⚫️ REC                     │ ← 錄音指示燈
│                                │
│  ▁▂▃▅▇█▇▅▃▂▁                  │ ← 音波動畫
│                                │
│     00:05                      │ ← 錄音時長
│                                │
│ [ ⏸️ 暫停 ]  [ ⏹️ 停止 ]        │
└────────────────────────────────┘

Step 3: 轉錄中
┌────────────────────────────────┐
│ 🎤 語音轉文字中...             │
│                                │
│  ⏳ 處理中... (3s)             │
│                                │
└────────────────────────────────┘

Step 4: 轉錄完成
┌────────────────────────────────┐
│ ✅ 轉錄完成                    │
│                                │
│ "請幫我查詢最近 7 天的訂單統計" │ ← 轉錄文字
│                                │
│ [ ✏️ 編輯 ]  [ ✓ 使用 ]  [ ✕ 取消 ]│
└────────────────────────────────┘
```

### 7. Markdown 支援

#### 支援的 Markdown 語法
```
# 標題 1
## 標題 2
### 標題 3

**粗體文字**
*斜體文字*
~~刪除線~~

- 無序列表 1
- 無序列表 2

1. 有序列表 1
2. 有序列表 2

[連結文字](https://example.com)

`行內代碼`

```python
# 代碼區塊
def hello():
    print("Hello")
```

> 引用文字

| 表頭 1 | 表頭 2 |
|--------|--------|
| 內容 1 | 內容 2 |
```

#### 即時預覽切換
```
輸入區域 Toolbar：
┌─────────────────────────────────────────┐
│ 📝 [編輯] [預覽]  ← Tab 切換             │
│                                         │
│ [編輯模式]                               │
│ **粗體** *斜體* `代碼`▋                  │
│                                         │
└─────────────────────────────────────────┘

┌─────────────────────────────────────────┐
│ 📝 [編輯] [預覽]  ← Tab 切換             │
│                                         │
│ [預覽模式]                               │
│ **粗體** *斜體* `代碼`                   │ ← 渲染後效果
│                                         │
└─────────────────────────────────────────┘
```

---

## ♿ 無障礙設計

### 鍵盤導航

```yaml
全局快捷鍵:
  Ctrl+N: 新建對話
  Ctrl+K: 快捷指令選單
  Ctrl+F: 搜尋對話
  Ctrl+/: 顯示所有快捷鍵
  Esc: 關閉彈出選單/取消操作

輸入區域:
  Enter: 發送消息
  Shift+Enter: 換行
  Ctrl+Enter: 發送消息（備用）
  Ctrl+Z: 復原
  Ctrl+Y: 重做
  /: 觸發快捷指令
  @: 觸發附件選單

消息導航:
  Tab: 下一個可聚焦元素
  Shift+Tab: 上一個可聚焦元素
  Arrow Up/Down: 在消息間移動
  Space: 展開/收起 Function call
  Enter: 啟動當前聚焦的按鈕
```

### ARIA 標籤

```html
<!-- 對話歷史列表 -->
<aside aria-label="對話歷史" role="complementary">
  <input
    type="search"
    aria-label="搜尋對話"
    placeholder="搜尋對話..."
  />

  <nav aria-label="對話列表">
    <ul role="list">
      <li role="listitem">
        <button aria-label="訂單統計查詢，今天 09:15">
          訂單統計查詢
        </button>
      </li>
    </ul>
  </nav>
</aside>

<!-- 消息流 -->
<main aria-label="對話消息" role="main">
  <div
    role="log"
    aria-live="polite"
    aria-atomic="false"
    aria-label="對話消息流"
  >
    <!-- 用戶消息 -->
    <article
      role="article"
      aria-label="Alex Chen 於 09:15 發送的消息"
    >
      <p>請幫我查詢最近 7 天的訂單統計</p>

      <div role="toolbar" aria-label="消息操作">
        <button aria-label="複製消息">📋 複製</button>
        <button aria-label="編輯消息">✏️ 編輯</button>
        <button aria-label="刪除消息">🗑️ 刪除</button>
      </div>
    </article>

    <!-- Agent 回覆 -->
    <article
      role="article"
      aria-label="Customer Service Bot 於 09:15 的回覆"
    >
      <p>好的，我來為您查詢...</p>

      <!-- Function Call -->
      <details aria-label="函數調用詳情">
        <summary>🔧 正在調用函數：get_order_statistics()</summary>
        <div role="region" aria-label="函數執行詳情">
          <!-- Function details -->
        </div>
      </details>

      <div role="toolbar" aria-label="回覆操作">
        <button aria-label="複製回覆">📋 複製</button>
        <button aria-label="重新生成回覆">🔄 重試</button>
        <button aria-label="標記為有用">👍</button>
        <button aria-label="標記為無用">👎</button>
      </div>
    </article>
  </div>
</main>

<!-- 輸入區域 -->
<form
  role="form"
  aria-label="消息輸入表單"
  onsubmit="handleSubmit"
>
  <textarea
    aria-label="輸入您的消息"
    placeholder="輸入您的消息... (支援 Markdown)"
    rows="1"
    aria-multiline="true"
  ></textarea>

  <div role="toolbar" aria-label="輸入工具列">
    <button aria-label="附加檔案">📎</button>
    <button aria-label="語音輸入">🎤</button>
    <button aria-label="上傳圖片">📷</button>
    <button aria-label="快捷指令">🔧</button>
  </div>

  <button
    type="submit"
    aria-label="發送消息"
  >
    ⬆️ 發送
  </button>
</form>

<!-- 右側資訊面板 -->
<aside aria-label="Agent 資訊與上下文" role="complementary">
  <section aria-labelledby="agent-info-heading">
    <h2 id="agent-info-heading">Agent 資訊</h2>
    <!-- Agent info content -->
  </section>

  <section aria-labelledby="token-usage-heading">
    <h2 id="token-usage-heading">Token 使用量</h2>
    <div
      role="progressbar"
      aria-valuenow="30.7"
      aria-valuemin="0"
      aria-valuemax="100"
      aria-label="Token 使用量 30.7%"
    >
      30.7%
    </div>
  </section>
</aside>
```

### 螢幕閱讀器優化

```yaml
消息更新通知:
  實現: aria-live="polite"
  行為:
    - 新消息到達時自動朗讀
    - 不打斷當前朗讀內容
    - Streaming 時每 3 秒更新一次

焦點管理:
  發送消息後:
    - 焦點移至輸入框
    - 朗讀"消息已發送"

  收到回覆後:
    - 焦點保持在輸入框
    - 朗讀"收到新回覆"

  開啟彈出選單:
    - 焦點移至選單第一項
    - Esc 關閉並返回原位置

錯誤處理:
  發送失敗:
    - 朗讀錯誤訊息
    - 焦點移至重試按鈕

  Token 超限:
    - 朗讀警告訊息
    - 焦點移至清理歷史按鈕
```

---

## ⚡ 性能優化

### 1. 虛擬滾動

```typescript
interface VirtualScrollConfig {
  // 配置
  itemHeight: 100, // 平均消息高度
  bufferSize: 5,   // 緩衝區大小（上下各 5 條）

  // 計算
  totalItems: 1000,           // 總消息數
  visibleItems: 10,           // 可視區域顯示 10 條
  renderedItems: 20,          // 實際渲染 20 條（10 + buffer）

  // 性能
  memoryUsage: '~2MB',        // vs. 100MB 全量渲染
  initialRender: '< 50ms',    // vs. 2000ms
  scrollFPS: '60fps'          // 保持流暢滾動
}
```

### 2. 消息 Streaming

```typescript
interface StreamingConfig {
  // Streaming 設定
  chunkSize: 1,               // 每次接收 1 個 token
  updateInterval: 50,         // 50ms 更新一次 UI

  // 優化策略
  strategy: 'requestAnimationFrame', // 使用 RAF 優化渲染
  debounce: true,             // 防抖處理

  // 性能指標
  firstTokenLatency: '< 200ms',   // 首個 token 延遲
  tokensPerSecond: 20,            // 每秒 20 tokens
  uiUpdateCost: '< 5ms'           // UI 更新成本
}
```

### 3. 圖片與附件優化

```typescript
interface MediaOptimization {
  // 圖片優化
  images: {
    lazyLoad: true,            // 延遲載入
    placeholder: 'blur',       // 模糊佔位符
    formats: ['webp', 'jpg'],  // 優先 WebP
    maxWidth: 1200,            // 最大寬度
    compression: 0.8           // 壓縮質量
  },

  // 附件處理
  attachments: {
    previewGeneration: 'server-side',  // 伺服器端生成預覽
    thumbnailSize: '200x200',          // 縮圖尺寸
    caching: 'aggressive'              // 積極快取
  },

  // 性能收益
  bandwidth: '-70%',           // 減少 70% 流量
  loadTime: '-60%'             // 減少 60% 載入時間
}
```

### 4. 快取策略

```typescript
interface CacheStrategy {
  // 對話列表快取
  conversationList: {
    type: 'IndexedDB',
    ttl: 3600,              // 1 小時
    maxSize: 100            // 最多 100 個對話
  },

  // 消息快取
  messages: {
    type: 'Memory + IndexedDB',
    hot: 50,                // 記憶體中保留 50 條
    cold: 'IndexedDB',      // 其他存 IndexedDB
    ttl: 86400              // 24 小時
  },

  // 附件快取
  attachments: {
    type: 'Cache API',
    strategy: 'stale-while-revalidate',
    maxSize: '100MB'
  },

  // 性能收益
  offlineSupport: true,      // 支援離線訪問
  loadTime: '< 100ms'        // 快取命中時載入時間
}
```

### 5. 輸入優化

```typescript
interface InputOptimization {
  // 防抖
  debounce: {
    typing: 300,            // 打字狀態 300ms
    autoSave: 2000,         // 自動儲存 2s
    search: 500             // 搜尋 500ms
  },

  // 節流
  throttle: {
    tokenCount: 200,        // Token 計數 200ms
    scrollEvent: 16         // 滾動事件 16ms (60fps)
  },

  // 性能收益
  cpuUsage: '-80%',         // 減少 80% CPU 使用
  networkCalls: '-90%'      // 減少 90% 網路請求
}
```

### 6. 代碼分割

```typescript
interface CodeSplitting {
  // 路由級分割
  routes: {
    conversation: () => import('./Conversation'),
    history: () => import('./ConversationHistory'),
    settings: () => import('./ConversationSettings')
  },

  // 功能級分割
  features: {
    voiceInput: () => import('./VoiceInput'),
    fileUpload: () => import('./FileUpload'),
    markdown: () => import('./MarkdownEditor')
  },

  // 性能收益
  initialBundle: '120KB',    // vs. 800KB
  loadTime: '< 500ms',       // vs. 2000ms
  cacheHitRate: '> 95%'      // 快取命中率
}
```

---

## 🔧 技術實現建議

### 1. 狀態管理（Zustand）

```typescript
// stores/conversationStore.ts
import create from 'zustand';

interface Message {
  id: string;
  role: 'user' | 'assistant' | 'system';
  content: string;
  timestamp: Date;
  functionCalls?: FunctionCall[];
  attachments?: Attachment[];
  reactions?: Reaction[];
}

interface Conversation {
  id: string;
  title: string;
  agentId: string;
  messages: Message[];
  branches: Branch[];
  currentBranch: string;
  metadata: {
    createdAt: Date;
    updatedAt: Date;
    tokenUsage: number;
    messageCount: number;
  };
}

interface ConversationStore {
  // 狀態
  conversations: Conversation[];
  activeConversationId: string | null;
  isLoading: boolean;
  isStreaming: boolean;

  // 對話操作
  createConversation: (agentId: string) => void;
  loadConversation: (id: string) => Promise<void>;
  deleteConversation: (id: string) => void;

  // 消息操作
  sendMessage: (content: string, attachments?: File[]) => Promise<void>;
  editMessage: (messageId: string, newContent: string) => void;
  deleteMessage: (messageId: string) => void;
  retryMessage: (messageId: string) => Promise<void>;

  // 分支操作
  createBranch: (fromMessageId: string, name: string) => void;
  switchBranch: (branchId: string) => void;
  deleteBranch: (branchId: string) => void;

  // 附件操作
  uploadAttachment: (file: File) => Promise<Attachment>;
  deleteAttachment: (attachmentId: string) => void;

  // 評分操作
  rateMessage: (messageId: string, rating: 'up' | 'down') => void;
}

const useConversationStore = create<ConversationStore>((set, get) => ({
  conversations: [],
  activeConversationId: null,
  isLoading: false,
  isStreaming: false,

  createConversation: (agentId: string) => {
    const newConversation: Conversation = {
      id: generateId(),
      title: '新對話',
      agentId,
      messages: [],
      branches: [{ id: 'main', name: 'Main', parentId: null }],
      currentBranch: 'main',
      metadata: {
        createdAt: new Date(),
        updatedAt: new Date(),
        tokenUsage: 0,
        messageCount: 0
      }
    };

    set((state) => ({
      conversations: [...state.conversations, newConversation],
      activeConversationId: newConversation.id
    }));
  },

  sendMessage: async (content: string, attachments?: File[]) => {
    const { activeConversationId } = get();
    if (!activeConversationId) return;

    set({ isStreaming: true });

    // 添加用戶消息
    const userMessage: Message = {
      id: generateId(),
      role: 'user',
      content,
      timestamp: new Date(),
      attachments: attachments ? await uploadFiles(attachments) : []
    };

    set((state) => ({
      conversations: state.conversations.map(conv =>
        conv.id === activeConversationId
          ? {
              ...conv,
              messages: [...conv.messages, userMessage],
              metadata: {
                ...conv.metadata,
                updatedAt: new Date(),
                messageCount: conv.metadata.messageCount + 1
              }
            }
          : conv
      )
    }));

    // 調用 Agent API（Streaming）
    const assistantMessage: Message = {
      id: generateId(),
      role: 'assistant',
      content: '',
      timestamp: new Date(),
      functionCalls: []
    };

    set((state) => ({
      conversations: state.conversations.map(conv =>
        conv.id === activeConversationId
          ? { ...conv, messages: [...conv.messages, assistantMessage] }
          : conv
      )
    }));

    // Streaming 接收
    const stream = await fetchAgentResponse(userMessage);

    for await (const chunk of stream) {
      if (chunk.type === 'content') {
        assistantMessage.content += chunk.data;
      } else if (chunk.type === 'function_call') {
        assistantMessage.functionCalls?.push(chunk.data);
      }

      // 更新 UI
      set((state) => ({
        conversations: state.conversations.map(conv =>
          conv.id === activeConversationId
            ? {
                ...conv,
                messages: conv.messages.map(msg =>
                  msg.id === assistantMessage.id ? { ...assistantMessage } : msg
                )
              }
            : conv
        )
      }));
    }

    set({ isStreaming: false });
  }
}));
```

### 2. 虛擬滾動實現

```typescript
// components/VirtualMessageList.tsx
import { useVirtualizer } from '@tanstack/react-virtual';
import { useRef } from 'react';

interface VirtualMessageListProps {
  messages: Message[];
  onLoadMore?: () => void;
}

export function VirtualMessageList({ messages, onLoadMore }: VirtualMessageListProps) {
  const parentRef = useRef<HTMLDivElement>(null);

  const virtualizer = useVirtualizer({
    count: messages.length,
    getScrollElement: () => parentRef.current,
    estimateSize: () => 100, // 預估消息高度
    overscan: 5,             // 上下各渲染 5 條額外消息

    // 反向滾動（聊天從下往上）
    initialOffset: 999999,

    // 滾動到底部時載入更多
    onChange: (instance) => {
      const lastItem = instance.getVirtualItems().at(-1);
      if (lastItem && lastItem.index === messages.length - 1) {
        onLoadMore?.();
      }
    }
  });

  return (
    <div
      ref={parentRef}
      className="message-list"
      style={{ height: '100%', overflow: 'auto' }}
    >
      <div
        style={{
          height: `${virtualizer.getTotalSize()}px`,
          position: 'relative'
        }}
      >
        {virtualizer.getVirtualItems().map((virtualRow) => {
          const message = messages[virtualRow.index];

          return (
            <div
              key={virtualRow.index}
              data-index={virtualRow.index}
              ref={virtualizer.measureElement}
              style={{
                position: 'absolute',
                top: 0,
                left: 0,
                width: '100%',
                transform: `translateY(${virtualRow.start}px)`
              }}
            >
              <MessageBubble message={message} />
            </div>
          );
        })}
      </div>
    </div>
  );
}
```

### 3. Streaming 實現

```typescript
// services/agentApi.ts
async function* fetchAgentResponse(userMessage: Message): AsyncGenerator<StreamChunk> {
  const response = await fetch('/api/agents/chat', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Accept': 'text/event-stream'
    },
    body: JSON.stringify({
      message: userMessage.content,
      conversationId: userMessage.conversationId,
      agentId: userMessage.agentId
    })
  });

  if (!response.body) {
    throw new Error('No response body');
  }

  const reader = response.body.getReader();
  const decoder = new TextDecoder();
  let buffer = '';

  while (true) {
    const { done, value } = await reader.read();

    if (done) break;

    buffer += decoder.decode(value, { stream: true });
    const lines = buffer.split('\n');
    buffer = lines.pop() || '';

    for (const line of lines) {
      if (line.startsWith('data: ')) {
        const data = JSON.parse(line.slice(6));

        if (data.type === 'content') {
          yield { type: 'content', data: data.delta };
        } else if (data.type === 'function_call') {
          yield { type: 'function_call', data: data.function };
        } else if (data.type === 'done') {
          return;
        }
      }
    }
  }
}

// components/MessageBubble.tsx
function AssistantMessage({ message }: { message: Message }) {
  const [displayContent, setDisplayContent] = useState('');

  useEffect(() => {
    if (!message.isStreaming) {
      setDisplayContent(message.content);
      return;
    }

    // Streaming 動畫
    let index = 0;
    const interval = setInterval(() => {
      if (index < message.content.length) {
        setDisplayContent(message.content.slice(0, index + 1));
        index++;
      } else {
        clearInterval(interval);
      }
    }, 20); // 每 20ms 顯示一個字符

    return () => clearInterval(interval);
  }, [message.content, message.isStreaming]);

  return (
    <div className="assistant-message">
      <ReactMarkdown>{displayContent}</ReactMarkdown>
      {message.isStreaming && <span className="cursor">▋</span>}
    </div>
  );
}
```

### 4. 語音輸入實現

```typescript
// hooks/useVoiceInput.ts
import { useState, useCallback } from 'react';

interface VoiceInputHook {
  isRecording: boolean;
  transcript: string;
  startRecording: () => Promise<void>;
  stopRecording: () => void;
  error: Error | null;
}

export function useVoiceInput(): VoiceInputHook {
  const [isRecording, setIsRecording] = useState(false);
  const [transcript, setTranscript] = useState('');
  const [error, setError] = useState<Error | null>(null);
  const [recognition, setRecognition] = useState<SpeechRecognition | null>(null);

  const startRecording = useCallback(async () => {
    try {
      // 檢查瀏覽器支援
      const SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;

      if (!SpeechRecognition) {
        throw new Error('瀏覽器不支援語音識別');
      }

      // 請求麥克風權限
      await navigator.mediaDevices.getUserMedia({ audio: true });

      const recognition = new SpeechRecognition();
      recognition.lang = 'zh-TW';
      recognition.continuous = true;
      recognition.interimResults = true;

      recognition.onresult = (event) => {
        const transcript = Array.from(event.results)
          .map(result => result[0].transcript)
          .join('');

        setTranscript(transcript);
      };

      recognition.onerror = (event) => {
        setError(new Error(event.error));
        setIsRecording(false);
      };

      recognition.onend = () => {
        setIsRecording(false);
      };

      recognition.start();
      setRecognition(recognition);
      setIsRecording(true);
      setError(null);

    } catch (err) {
      setError(err as Error);
    }
  }, []);

  const stopRecording = useCallback(() => {
    if (recognition) {
      recognition.stop();
      setRecognition(null);
    }
    setIsRecording(false);
  }, [recognition]);

  return {
    isRecording,
    transcript,
    startRecording,
    stopRecording,
    error
  };
}

// components/VoiceInputButton.tsx
function VoiceInputButton({ onTranscript }: { onTranscript: (text: string) => void }) {
  const { isRecording, transcript, startRecording, stopRecording, error } = useVoiceInput();
  const [showModal, setShowModal] = useState(false);

  const handleStart = () => {
    startRecording();
    setShowModal(true);
  };

  const handleStop = () => {
    stopRecording();
    setShowModal(false);
    if (transcript) {
      onTranscript(transcript);
    }
  };

  return (
    <>
      <button onClick={handleStart} aria-label="語音輸入">
        🎤
      </button>

      {showModal && (
        <Modal>
          <h3>🎤 語音輸入</h3>

          {isRecording && (
            <>
              <div className="recording-indicator">
                <span className="rec-dot">⚫️</span> REC
              </div>

              <div className="waveform">
                {/* 音波動畫 */}
              </div>

              <p className="transcript">{transcript || '請開始說話...'}</p>

              <button onClick={handleStop}>⏹️ 停止</button>
            </>
          )}

          {error && (
            <div className="error">
              ❌ {error.message}
            </div>
          )}
        </Modal>
      )}
    </>
  );
}
```

---

## 📐 設計決策記錄（ADR）

### ADR-001: 3-Column 佈局 vs 2-Column 佈局

**決策**: 使用 3-Column 佈局（對話歷史 + 消息流 + Agent 資訊）

**理由**:
1. **分離關注點**: 歷史導航、當前對話、上下文資訊各司其職
2. **提升效率**: 用戶無需切換視圖即可查看所有資訊
3. **空間利用**: Desktop 版本有足夠空間容納 3 欄
4. **參考標準**: ChatGPT、Claude、Copilot 均採用類似佈局

**權衡**:
- ✅ 優點: 資訊密度高、操作效率高
- ❌ 缺點: 小螢幕需要折疊處理、學習曲線略高

**替代方案**:
- 2-Column: 簡化但需要更多點擊切換
- 1-Column: 最簡單但效率最低

---

### ADR-002: Streaming 回覆 vs 一次性回覆

**決策**: 使用 Streaming 方式顯示 Agent 回覆

**理由**:
1. **降低感知延遲**: 用戶看到首個 token < 2s，比等待完整回覆（10-30s）體驗更好
2. **即時反饋**: 用戶可以提前判斷回覆方向，必要時中斷
3. **行業標準**: OpenAI、Anthropic 等主流 LLM 平台均使用 Streaming
4. **性能優化**: 減少伺服器記憶體佔用和網路頻寬

**權衡**:
- ✅ 優點: 感知速度快、用戶體驗好、可中斷
- ❌ 缺點: 實現複雜度高、需要處理中斷和重試

**實現細節**:
```typescript
// Server-Sent Events (SSE)
const stream = await fetch('/api/chat', {
  method: 'POST',
  headers: { 'Accept': 'text/event-stream' }
});

for await (const chunk of stream) {
  // 每 50ms 更新一次 UI
  updateUI(chunk);
}
```

---

### ADR-003: 對話分支管理方式

**決策**: 使用樹狀結構管理對話分支，支援編輯消息時自動創建分支

**理由**:
1. **非破壞性編輯**: 編輯消息不會丟失原始對話，可隨時切換回去
2. **探索多方案**: 允許用戶嘗試不同提示詞，比較效果
3. **專業工具**: Prompt 工程師需要此功能進行 A/B 測試
4. **技術可行**: Git-like 分支模型成熟且易於理解

**權衡**:
- ✅ 優點: 靈活、非破壞性、支援實驗
- ❌ 缺點: UI 複雜度增加、儲存空間增大

**UI 設計**:
```
Main Branch (主分支)
├─ Branch A (編輯後的版本)
│  └─ Branch A-1 (進一步優化)
└─ Branch B (另一種方案)
```

**替代方案**:
- 線性歷史: 簡單但無法回溯
- 複製對話: 可行但管理混亂

---

### ADR-004: Function Calling 可視化設計

**決策**: 使用可展開/收起的卡片顯示 Function Call 詳情（參數、返回值、執行時間）

**理由**:
1. **透明度**: 開發者需要了解 Agent 調用了哪些函數、參數是什麼
2. **除錯需求**: 當結果不符預期時，需要檢查函數調用過程
3. **學習價值**: 用戶可以學習 Agent 如何使用工具
4. **折疊設計**: 預設折疊避免干擾，需要時可展開

**權衡**:
- ✅ 優點: 資訊完整、支援除錯、教育價值
- ❌ 缺點: 增加視覺複雜度

**UI 範例**:
```
🔧 正在調用函數：get_order_statistics()
[▶️ 展開查看詳細資訊]

↓ 展開後

🔧 正在調用函數：get_order_statistics()
[▼ 收起詳細資訊]
  📥 輸入參數：
    { "start_date": "2025-10-22", ... }

  📤 返回結果：
    { "total_orders": 1245, ... }

  ⏱️ 執行時間：1.24s
  ✅ 狀態：成功
```

**替代方案**:
- 完全隱藏: 簡潔但缺乏透明度
- 總是顯示: 資訊過載

---

## ✅ 驗收標準

### 功能完整性
- [ ] 支援即時聊天與 Streaming 回覆
- [ ] 支援多模態輸入（文字、圖片、檔案、語音）
- [ ] Function Calling 可視化展示
- [ ] 對話分支創建與切換
- [ ] 對話歷史搜尋與管理
- [ ] 消息編輯、刪除、重試
- [ ] 附件上傳與預覽
- [ ] Token 使用量監控
- [ ] 快捷指令支援

### 性能指標
- [ ] 消息發送延遲 < 100ms
- [ ] Agent 回覆首字延遲 < 2s
- [ ] 檔案上傳成功率 > 99%
- [ ] 對話載入時間 < 500ms
- [ ] 虛擬滾動支援 >1000 條消息

### 無障礙合規
- [ ] WCAG 2.1 AA 合規
- [ ] 完整鍵盤導航支援
- [ ] 螢幕閱讀器優化
- [ ] ARIA 標籤完整
- [ ] 高對比度模式支援

### 響應式設計
- [ ] Desktop (≥1280px) 3-column 佈局
- [ ] Tablet (768-1279px) 2-column 可折疊佈局
- [ ] Mobile (<768px) 1-column 全屏佈局
- [ ] 所有斷點下功能完整可用

---

**文檔版本**: 1.0.0
**最後更新**: 2025-10-29
**審核狀態**: ✅ 已完成
**下一步**: 開始創建 Knowledge Base Wireframe (06-knowledge-base.md)
