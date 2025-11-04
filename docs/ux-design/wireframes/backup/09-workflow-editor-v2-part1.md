# Workflow 編輯器 V2 - Part 1: 核心設計理念與大卡片節點系統

**版本**: 2.0.0 (Part 1/7)
**設計參考**: n8n (主要) + Temporal + Prefect
**日期**: 2025-10-29
**狀態**: ✅ 已完成
**複雜度**: ⭐⭐⭐⭐⭐ (極高)

---

## 🎯 設計理念轉變

### 從 Power Automate 到 n8n 的思維轉變

```yaml
❌ 舊設計理念 (Power Automate 風格):
  節點: 小卡片（160-200px），資訊密度低
  配置: 完全依賴側邊欄，頻繁切換
  執行: 狀態簡單，需要查看日誌
  數據流: 不可見，需要點擊查看

✅ 新設計理念 (n8n 風格):
  節點: 大卡片（300-450px），資訊豐富
  配置: 內聯編輯為主，側邊欄為輔
  執行: 狀態內嵌，結果直接可見
  數據流: 連接線上顯示數據摘要
```

---

## 📐 頁面佈局架構（n8n 風格）

### Desktop 版本（≥1280px）

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│ ← [🔀 Workflow Editor] 客戶服務流程 v2.3     [▶️ Execute] [💾 Save]  ⚙️ 🔔 👤 │
├──────────────┬──────────────────────────────────────────┬───────────────────────┤
│              │                                          │                       │
│ 🧩 Nodes     │  🎨 Canvas (無限畫布)                   │  ⚙️ Node Settings     │
│              │                                          │  (僅當選中節點時顯示) │
│ 🔍 Search    │                                          │                       │
│ ┌──────────┐│  ┌─────────────────────────────────────┐│  ┌─────────────────┐ │
│ │Filter...  ││  │                                     ││  │ HTTP Request    │ │
│ └──────────┘│  │  ┌──────────────────────────────┐   ││  │ ─────────────── │ │
│              │  │  │ 🎬 Manual Trigger            │   ││  │                 │ │
│ 📂 Triggers  │  │  │ ──────────────────────────── │   ││  │ 🔗 URL          │ │
│ • Manual     │  │  │ Workflow starts manually     │   ││  │ https://...     │ │
│ • Schedule   │  │  │                              │   ││  │                 │ │
│ • Webhook    │  │  │ ⏱️ Last: 2 min ago           │   ││  │ 📋 Parameters   │ │
│              │  │  └──────────┬───────────────────┘   ││  │ • limit: 10     │ │
│ 🔌 Core      │  │             │ trigger_data          ││  │ • sort: name    │ │
│ • HTTP       │  │             ▼                       ││  │                 │ │
│ • Function   │  │  ┌──────────────────────────────┐   ││  │ [Test Request] │ │
│ • Set        │  │  │ 💬 Get User Input            │   ││  │                 │ │
│ • If         │  │  │ ──────────────────────────── │   ││  │ 📊 Response     │ │
│              │  │  │ Wait for user question       │   ││  │ ✅ 200 OK       │ │
│ 🤖 AI        │  │  │                              │   ││  │ 10 items        │ │
│ • OpenAI     │  │  │ Input: Text field            │   ││  └─────────────────┘ │
│ • Anthropic  │  │  │ Placeholder: "Enter question"│   ││                       │
│ • Agent      │  │  │                              │   ││                       │
│              │  │  │ ✅ Executed in 0.5s          │   ││                       │
│ 📊 Data      │  │  │ 📥 Input: "我想查詢訂單"     │   ││                       │
│ • Filter     │  │  └──────────┬───────────────────┘   ││                       │
│ • Transform  │  │             │ question="我想..."    ││                       │
│ • Aggregate  │  │             ▼                       ││                       │
│              │  │  ┌──────────────────────────────┐   ││                       │
│ 🗄️ Database  │  │  │ 🎯 Classify Question         │   ││                       │
│ • PostgreSQL │  │  │ ──────────────────────────── │   ││                       │
│ • MySQL      │  │  │ Agent: GPT-4 Classification  │   ││                       │
│ • MongoDB    │  │  │ Temperature: 0.3             │   ││                       │
│              │  │  │                              │   ││                       │
│ 📧 Comms     │  │  │ Prompt: "Classify the user..."│  ││                       │
│ • Email      │  │  │ Input: {{$json.question}}    │   ││                       │
│ • Slack      │  │  │                              │   ││                       │
│ • Webhook    │  │  │ 🔄 Running... 2.1s           │   ││                       │
│              │  │  └──────────┬───────────────────┘   ││                       │
│ 💼 Business  │  │             │ category="order"      ││                       │
│ • Salesforce │  │             │ confidence=0.95       ││                       │
│ • HubSpot    │  │             ▼                       ││                       │
│              │  │  ┌──────────────────────────────┐   ││                       │
│ [+ Custom]   │  │  │ 📊 Display Result            │   ││                       │
│              │  │  │ ──────────────────────────── │   ││                       │
│ [280px]      │  │  │ Format: Markdown             │   ││  [380px]              │
│              │  │  │ Template: "Category: {{...}}"│   ││  (隱藏時畫布更寬)     │
│              │  │  └──────────────────────────────┘   ││                       │
│              │  │                                     ││                       │
│              │  └─────────────────────────────────────┘│                       │
│              │                                          │                       │
│              │  執行控制欄（底部）                       │                       │
│              │  ┌─────────────────────────────────────┐│                       │
│              │  │ 📊 Executions | ⏱️ Last: 2m | ✅ Success │                  │
│              │  │ [View History] [🐛 Debug Mode]      ││                       │
│              │  └─────────────────────────────────────┘│                       │
└──────────────┴──────────────────────────────────────────┴───────────────────────┘
```

---

## 🎴 大卡片節點設計（核心創新）

### 節點卡片尺寸標準

```yaml
n8n 風格卡片尺寸:
  Default (大多數節點):
    寬度: 320px
    最小高度: 180px
    最大高度: 600px (可滾動)

  Compact (簡單節點):
    寬度: 280px
    最小高度: 120px

  Expanded (複雜節點):
    寬度: 400px
    最小高度: 240px

  執行後展開:
    自動展開顯示結果
    +60-120px 高度（結果區域）
```

---

### 節點卡片結構（通用模板）

```
┌─────────────────────────────────────────────────────┐
│ 🎴 節點頭部區域 (Header)                    [⚙️] [×]│
│ ┌─────────────────────────────────────────────────┐ │
│ │ 🔵 [圖標] 節點類型名稱                          │ │
│ │ 節點自定義標題（可編輯）                        │ │
│ └─────────────────────────────────────────────────┘ │
├─────────────────────────────────────────────────────┤
│ 📝 核心配置區域 (Main Config) - 內聯編輯           │
│ ┌─────────────────────────────────────────────────┐ │
│ │ 參數 1: [值或表達式]                    [🔍]    │ │
│ │ 參數 2: [值或表達式]                    [🔍]    │ │
│ │ 參數 3: [▼ 下拉選單]                           │ │
│ │                                                 │ │
│ │ [+ Add Parameter]                               │ │
│ └─────────────────────────────────────────────────┘ │
├─────────────────────────────────────────────────────┤
│ 🔗 數據引用預覽區域 (Data Preview)                 │
│ ┌─────────────────────────────────────────────────┐ │
│ │ Input from: 📌 Previous Node                    │ │
│ │ {{$json.fieldName}} ← 點擊複製表達式            │ │
│ └─────────────────────────────────────────────────┘ │
├─────────────────────────────────────────────────────┤
│ 📊 執行狀態與結果區域 (Execution Result)           │
│ ┌─────────────────────────────────────────────────┐ │
│ │ ✅ Executed in 1.23s                            │ │
│ │ 📥 Input: 1 item                                │ │
│ │ 📤 Output: 1 item                               │ │
│ │                                                 │ │
│ │ 🔍 Output Data Preview:                         │ │
│ │ {                                               │ │
│ │   "id": 12345,                                  │ │
│ │   "name": "John Doe",                           │ │
│ │   "email": "john@example.com"                   │ │
│ │ }                                               │ │
│ │                                                 │ │
│ │ [👁️ View Full Output] [📋 Copy JSON]           │ │
│ └─────────────────────────────────────────────────┘ │
├─────────────────────────────────────────────────────┤
│ 🔧 快捷操作區域 (Quick Actions)                    │
│ [▶️ Execute] [🔄 Re-run] [📊 View Logs] [🐛 Debug] │
└─────────────────────────────────────────────────────┘
```

---

## 🎨 具體節點卡片範例

### 1. HTTP Request 節點（最常用）

```
┌─────────────────────────────────────────────────────────┐
│ 🌐 HTTP Request                             [⚙️] [×]    │
│ Get User Profile Data                                   │
├─────────────────────────────────────────────────────────┤
│ 🔗 Request Configuration                                │
│ ┌───────────────────────────────────────────────────┐   │
│ │ Method: [GET ▼]                                   │   │
│ │                                                   │   │
│ │ URL: https://api.example.com/users/               │   │
│ │      {{$json.userId}}                      [🔍]  │   │
│ │      └─ 預覽: https://api.example.com/users/123  │   │
│ │                                                   │   │
│ │ 🔐 Authentication: [Bearer Token ▼]              │   │
│ │ Token: {{$env.API_TOKEN}}                  [🔍]  │   │
│ │                                                   │   │
│ │ ☑ Send Query Parameters (2)                      │   │
│ │   • limit: 10                              [🗑️]  │   │
│ │   • sort: created_at                       [🗑️]  │   │
│ │   [+ Add Parameter]                               │   │
│ │                                                   │   │
│ │ ☐ Send Headers                                    │   │
│ │ ☐ Send Body                                       │   │
│ └───────────────────────────────────────────────────┘   │
├─────────────────────────────────────────────────────────┤
│ 🔗 Input Data                                           │
│ From: 💬 Get User Input (node_002)                      │
│ Available fields: userId, userName, timestamp           │
│ [📋 Insert Reference]                                   │
├─────────────────────────────────────────────────────────┤
│ ✅ Executed in 234ms                                    │
│ 📥 Sent: 1 request                                      │
│ 📤 Received: 200 OK | 1 item | 2.3 KB                   │
│                                                         │
│ 🔍 Response Preview:                                    │
│ ┌───────────────────────────────────────────────────┐   │
│ │ {                                                 │   │
│ │   "id": 123,                                      │   │
│ │   "name": "John Doe",                             │   │
│ │   "email": "john@example.com",                    │   │
│ │   "created_at": "2025-10-29T10:30:00Z",           │   │
│ │   "status": "active"                              │   │
│ │   ...                                             │   │
│ │ }                                                 │   │
│ └───────────────────────────────────────────────────┘   │
│                                                         │
│ [👁️ View Full Response] [📋 Copy JSON] [📊 View Headers]│
├─────────────────────────────────────────────────────────┤
│ [▶️ Test Request] [🔄 Re-run] [📝 Add Note]             │
└─────────────────────────────────────────────────────────┘
```

**設計亮點**：
- ✅ URL 支持表達式，實時預覽解析結果
- ✅ 參數可展開/收合，避免卡片過長
- ✅ 執行結果直接顯示在卡片上
- ✅ JSON 預覽可折疊，支持複製
- ✅ 快速測試按鈕，無需執行整個 workflow

---

### 2. AI Agent 節點（核心業務）

```
┌─────────────────────────────────────────────────────────┐
│ 🤖 AI Agent - OpenAI                        [⚙️] [×]    │
│ Classify User Question                                  │
├─────────────────────────────────────────────────────────┤
│ 🧠 Model Configuration                                  │
│ ┌───────────────────────────────────────────────────┐   │
│ │ Model: [gpt-4-turbo ▼]                            │   │
│ │ Temperature: [=======>    ] 0.3                   │   │
│ │ Max Tokens: [2000        ]                        │   │
│ │ ☑ Stream Response                                 │   │
│ └───────────────────────────────────────────────────┘   │
├─────────────────────────────────────────────────────────┤
│ 💬 Prompt Template                                      │
│ ┌───────────────────────────────────────────────────┐   │
│ │ System: You are a customer service classifier.    │   │
│ │                                                   │   │
│ │ User: Classify this question into one of:        │   │
│ │ - order_inquiry                                   │   │
│ │ - technical_support                               │   │
│ │ - billing_question                                │   │
│ │ - general_inquiry                                 │   │
│ │                                                   │   │
│ │ Question: {{$json.question}}            [🔍]     │   │
│ │           └─ 預覽: "我想查詢訂單狀態"             │   │
│ │                                                   │   │
│ │ Response format: JSON with category & confidence  │   │
│ │                                                   │   │
│ │ [✨ AI Assist] [📋 Load Template] [💾 Save]       │   │
│ └───────────────────────────────────────────────────┘   │
├─────────────────────────────────────────────────────────┤
│ 🔗 Input Data                                           │
│ From: 💬 Get User Input (node_002)                      │
│ • question: "我想查詢訂單狀態"                           │
│ • timestamp: "2025-10-29T15:30:00Z"                     │
│ [📋 Insert Field Reference]                             │
├─────────────────────────────────────────────────────────┤
│ ✅ Executed in 2.34s                                    │
│ 💰 Cost: $0.0234 (1,234 input + 89 output tokens)      │
│ 📤 Output: JSON (1 item)                                │
│                                                         │
│ 🔍 AI Response:                                         │
│ ┌───────────────────────────────────────────────────┐   │
│ │ {                                                 │   │
│ │   "category": "order_inquiry",                    │   │
│ │   "confidence": 0.95,                             │   │
│ │   "reasoning": "用戶明確提到'查詢訂單'關鍵詞，  │   │
│ │                且語氣為諮詢性質",                │   │
│ │   "suggested_action": "route_to_order_team"       │   │
│ │ }                                                 │   │
│ └───────────────────────────────────────────────────┘   │
│                                                         │
│ [👁️ View Full Response] [📋 Copy] [📊 Token Usage]     │
├─────────────────────────────────────────────────────────┤
│ [▶️ Test Prompt] [🔄 Regenerate] [💡 Optimize Prompt]   │
└─────────────────────────────────────────────────────────┘
```

**設計亮點**：
- ✅ Prompt 編輯器支持多行輸入
- ✅ 表達式實時預覽（顯示實際數據）
- ✅ Token 使用量和成本顯示
- ✅ AI 輔助優化 Prompt
- ✅ 可以直接在卡片上測試

---

### 3. If/Condition 節點（邏輯控制）

```
┌─────────────────────────────────────────────────────────┐
│ ❓ IF Condition                             [⚙️] [×]    │
│ Check Order Status Priority                             │
├─────────────────────────────────────────────────────────┤
│ 🔀 Condition Rules                                      │
│ ┌───────────────────────────────────────────────────┐   │
│ │ If the following conditions are TRUE:             │   │
│ │                                                   │   │
│ │ ┌─────────────────────────────────────────────┐   │   │
│ │ │ {{$json.category}}            [🔍]          │   │   │
│ │ │ └─ Current: "order_inquiry"                 │   │   │
│ │ │                                             │   │   │
│ │ │ [equals ▼] "order_inquiry"                  │   │   │
│ │ │                                             │   │   │
│ │ │ [AND ▼]                                     │   │   │
│ │ │                                             │   │   │
│ │ │ {{$json.confidence}}          [🔍]          │   │   │
│ │ │ └─ Current: 0.95                            │   │   │
│ │ │                                             │   │   │
│ │ │ [greater than ▼] 0.8                        │   │   │
│ │ │                                             │   │   │
│ │ │ [+ Add Condition]                           │   │   │
│ │ └─────────────────────────────────────────────┘   │   │
│ │                                                   │   │
│ │ Mode: [All conditions must match ▼]               │   │
│ └───────────────────────────────────────────────────┘   │
├─────────────────────────────────────────────────────────┤
│ 🔗 Input Data                                           │
│ From: 🤖 Classify Question (node_003)                   │
│ • category: "order_inquiry"                             │
│ • confidence: 0.95                                      │
├─────────────────────────────────────────────────────────┤
│ ✅ Executed in 0.05s                                    │
│ 📊 Evaluation Result: TRUE ✅                           │
│                                                         │
│ 🔀 Branch Taken: TRUE (Output 1)                        │
│ ├─ ✅ Conditions Met (2/2)                              │
│ │  • category equals "order_inquiry": ✅               │
│ │  • confidence > 0.8: ✅ (0.95)                       │
│ │                                                      │
│ └─ ➡️ Routed to: Process Order Inquiry                 │
│                                                         │
│ FALSE branch (Output 2): Not executed                   │
├─────────────────────────────────────────────────────────┤
│ [▶️ Test Conditions] [🔄 Re-evaluate]                   │
└─────────────────────────────────────────────────────────┘
```

**設計亮點**：
- ✅ 視覺化條件構建器（不需要寫代碼）
- ✅ 實時顯示當前值
- ✅ 清晰顯示哪個分支被執行
- ✅ 顯示每個條件的評估結果

---

### 4. Function/Code 節點（開發者專用）

```
┌─────────────────────────────────────────────────────────┐
│ ⚡ Function                                 [⚙️] [×]    │
│ Transform User Data                                     │
├─────────────────────────────────────────────────────────┤
│ 💻 JavaScript Code Editor                               │
│ ┌───────────────────────────────────────────────────┐   │
│ │ // Input: items[] from previous node              │   │
│ │ // Each item has: {category, confidence, ...}     │   │
│ │                                                   │   │
│ │ for (const item of items) {                       │   │
│ │   // Transform data                               │   │
│ │   item.priority = item.confidence > 0.9           │   │
│ │     ? 'high'                                      │   │
│ │     : 'normal';                                   │   │
│ │                                                   │   │
│ │   item.processedAt = new Date().toISOString();   │   │
│ │                                                   │   │
│ │   // Add custom field                             │   │
│ │   item.department = getDepartment(item.category); │   │
│ │ }                                                 │   │
│ │                                                   │   │
│ │ return items;                                     │   │
│ │                                                   │   │
│ │ [🎨 Format Code] [📖 Docs] [🔍 Autocomplete: ON]  │   │
│ └───────────────────────────────────────────────────┘   │
├─────────────────────────────────────────────────────────┤
│ 🔗 Input Data (items: 1)                                │
│ ┌───────────────────────────────────────────────────┐   │
│ │ {                                                 │   │
│ │   "category": "order_inquiry",                    │   │
│ │   "confidence": 0.95,                             │   │
│ │   "question": "我想查詢訂單"                       │   │
│ │ }                                                 │   │
│ └───────────────────────────────────────────────────┘   │
├─────────────────────────────────────────────────────────┤
│ ✅ Executed in 0.12s                                    │
│ 📤 Output: 1 item (Transformed)                         │
│                                                         │
│ 🔍 Output Preview:                                      │
│ ┌───────────────────────────────────────────────────┐   │
│ │ {                                                 │   │
│ │   "category": "order_inquiry",                    │   │
│ │   "confidence": 0.95,                             │   │
│ │   "question": "我想查詢訂單",                      │   │
│ │   "priority": "high",              ← 新增        │   │
│ │   "processedAt": "2025-10-29...",  ← 新增        │   │
│ │   "department": "order_team"        ← 新增        │   │
│ │ }                                                 │   │
│ └───────────────────────────────────────────────────┘   │
│                                                         │
│ [👁️ View Full Output] [📋 Copy Code] [🐛 Debug]        │
├─────────────────────────────────────────────────────────┤
│ [▶️ Test Code] [🔄 Re-run] [💾 Save as Snippet]         │
└─────────────────────────────────────────────────────────┘
```

**設計亮點**：
- ✅ Monaco Editor 嵌入卡片
- ✅ 語法高亮和自動完成
- ✅ 輸入/輸出預覽並排顯示
- ✅ 可以保存常用代碼片段

---

## 🎭 節點狀態視覺化（n8n 精髓）

### 狀態類型和視覺設計

```yaml
1. 待執行 (Idle):
   ┌─────────────────────────┐
   │ 🔵 HTTP Request         │ ← 藍色標題列
   │ Get User Data           │
   │ ─────────────────────── │
   │ [未執行，等待觸發]       │
   └─────────────────────────┘
   邊框: 淺灰色 #E5E7EB
   背景: 白色 #FFFFFF

2. 執行中 (Running):
   ┌─────────────────────────┐
   │ 🟡 HTTP Request         │ ← 黃色標題列 + 脈衝動畫
   │ Get User Data           │
   │ ─────────────────────── │
   │ 🔄 Executing... 2.1s    │ ← 進度動畫
   │ [████████░░░░░] 67%     │
   └─────────────────────────┘
   邊框: 黃色 #F59E0B + 陰影
   背景: 淡黃色 #FFFBEB

3. 執行成功 (Success):
   ┌─────────────────────────┐
   │ 🟢 HTTP Request         │ ← 綠色標題列
   │ Get User Data           │
   │ ─────────────────────── │
   │ ✅ Success in 234ms     │
   │ 📤 Output: 1 item       │
   │ {                       │
   │   "id": 123,            │
   │   "name": "John"        │
   │ }                       │
   └─────────────────────────┘
   邊框: 綠色 #10B981
   背景: 淡綠色 #ECFDF5

4. 執行失敗 (Error):
   ┌─────────────────────────┐
   │ 🔴 HTTP Request         │ ← 紅色標題列
   │ Get User Data           │
   │ ─────────────────────── │
   │ ❌ Failed in 30.0s      │
   │ Error: Connection timeout│
   │ [View Details]          │
   │ [Retry]                 │
   └─────────────────────────┘
   邊框: 紅色 #EF4444
   背景: 淡紅色 #FEF2F2

5. 已停用 (Disabled):
   ┌─────────────────────────┐
   │ ⚫ HTTP Request         │ ← 灰色標題列
   │ Get User Data           │
   │ ─────────────────────── │
   │ [Node Disabled]         │
   │ [Enable]                │
   └─────────────────────────┘
   邊框: 深灰色 #6B7280
   背景: 灰白色 #F9FAFB
   透明度: 0.6

6. 暫停中 (Paused - Debug):
   ┌─────────────────────────┐
   │ 🟣 HTTP Request         │ ← 紫色標題列
   │ Get User Data           │
   │ ─────────────────────── │
   │ ⏸️ Paused at breakpoint │
   │ [Continue] [Step]       │
   └─────────────────────────┘
   邊框: 紫色 #8B5CF6
   背景: 淡紫色 #F5F3FF
```

---

## 🎨 視覺設計規範

### 顏色系統（n8n 風格）

```yaml
節點狀態顏色:
  Idle:
    標題: #3B82F6 (藍色)
    邊框: #E5E7EB (淺灰)
    背景: #FFFFFF (白色)

  Running:
    標題: #F59E0B (橙黃)
    邊框: #F59E0B (橙黃) + 脈衝
    背景: #FFFBEB (淡黃)
    動畫: 脈衝 + 進度條

  Success:
    標題: #10B981 (綠色)
    邊框: #10B981 (綠色)
    背景: #ECFDF5 (淡綠)
    圖標: ✅

  Error:
    標題: #EF4444 (紅色)
    邊框: #EF4444 (紅色)
    背景: #FEF2F2 (淡紅)
    圖標: ❌

  Disabled:
    標題: #6B7280 (灰色)
    邊框: #D1D5DB (中灰)
    背景: #F9FAFB (灰白)
    透明度: 0.6

節點類型顏色編碼:
  Trigger: #8B5CF6 (紫色)
  Core: #3B82F6 (藍色)
  AI: #10B981 (綠色)
  Data: #F59E0B (橙色)
  Database: #DC2626 (紅色)
  Communication: #EC4899 (粉紅)
  Business: #6366F1 (靛藍)
```

### 字體系統

```yaml
標題:
  節點名稱: 14px, 600, Inter
  節點類型: 12px, 500, Inter

內容:
  標籤: 12px, 500, Inter
  值: 13px, 400, Inter
  代碼: 13px, 400, Fira Code

按鈕:
  主要: 13px, 500, Inter
  次要: 12px, 500, Inter
```

### 間距系統

```yaml
卡片內部:
  標題區: padding: 12px 16px
  內容區: padding: 16px
  區塊間距: margin-bottom: 16px

元素間距:
  小間距: 4px
  中間距: 8px
  大間距: 12px
  區塊間距: 16px
```

---

## 🔗 連接線設計（簡介）

### 基本樣式

```yaml
預設連接線:
  類型: 貝塞爾曲線 (Bezier Curve)
  顏色: #9CA3AF (中灰)
  寬度: 2px
  樣式: 實線

執行中連接線:
  顏色: #3B82F6 (藍色)
  寬度: 3px
  動畫: 流動點粒子效果

數據流標籤:
  位置: 連接線中點
  背景: 半透明白色
  內容: 數據摘要或欄位名稱
  字體: 11px, 500

連接點 (Port):
  大小: 8px × 8px
  顏色: #D1D5DB (待連接)
  顏色: #3B82F6 (可連接)
  顏色: #10B981 (已連接)
```

---

## 📊 節點庫面板設計

### 左側節點庫（重新設計）

```
┌────────────────────────┐
│ 🧩 Nodes               │
│                        │
│ 🔍 Search              │
│ ┌────────────────────┐ │
│ │ Filter nodes...    │ │
│ └────────────────────┘ │
│                        │
│ 📂 Triggers (3)        │
│ ┌────────────────────┐ │
│ │ 🎬 Manual          │ │
│ │ Start workflow     │ │
│ ├────────────────────┤ │
│ │ ⏰ Schedule        │ │
│ │ Run on schedule    │ │
│ ├────────────────────┤ │
│ │ 🌐 Webhook         │ │
│ │ HTTP endpoint      │ │
│ └────────────────────┘ │
│                        │
│ 🔌 Core (8) ▼          │
│ ┌────────────────────┐ │
│ │ 🌐 HTTP Request    │ │
│ │ Make API calls     │ │
│ ├────────────────────┤ │
│ │ ⚡ Function        │ │
│ │ Run custom code    │ │
│ ├────────────────────┤ │
│ │ 📝 Set             │ │
│ │ Set values         │ │
│ ├────────────────────┤ │
│ │ ❓ IF              │ │
│ │ Conditional logic  │ │
│ └────────────────────┘ │
│                        │
│ 🤖 AI (6) ▼            │
│ 🗄️ Database (5) ▼      │
│ 📧 Communication (4) ▼ │
│ 💼 Business (8) ▼      │
│                        │
│ [+ Create Custom Node] │
└────────────────────────┘
```

**節點卡片懸浮預覽**：
```
滑鼠懸浮時顯示：
┌──────────────────────────────┐
│ 🌐 HTTP Request              │
│ ────────────────────────────│
│ Make HTTP requests to APIs   │
│                              │
│ Features:                    │
│ • All HTTP methods           │
│ • Authentication support     │
│ • Response parsing           │
│ • Error handling             │
│                              │
│ Common uses:                 │
│ • Fetch data from APIs       │
│ • Submit forms               │
│ • Trigger webhooks           │
│                              │
│ [View Documentation →]       │
└──────────────────────────────┘
```

---

## ✅ Part 1 檢查清單

### 完成項目
- [✅] 設計理念轉變分析（Power Automate → n8n）
- [✅] 新的 3-Panel 佈局設計
- [✅] 大卡片節點系統架構
- [✅] 節點卡片通用模板
- [✅] 4 個具體節點範例
  - [✅] HTTP Request（最常用）
  - [✅] AI Agent（核心業務）
  - [✅] IF Condition（邏輯控制）
  - [✅] Function（開發者專用）
- [✅] 6 種節點狀態視覺化
- [✅] 完整視覺設計規範（顏色、字體、間距）
- [✅] 連接線設計簡介
- [✅] 節點庫面板重新設計

### 核心創新點
✨ **大卡片設計**：320-400px 寬度，資訊豐富
✨ **內聯編輯**：核心配置直接在卡片上修改
✨ **執行結果內嵌**：不需要切換到日誌面板
✨ **表達式預覽**：實時顯示 {{$json.xxx}} 的值
✨ **快速測試**：每個節點可獨立測試

### 下一步（Part 2）
- [ ] 內聯配置詳細設計
- [ ] 表達式系統（{{$json}}, {{$env}}, {{$node}}）
- [ ] 參數編輯器組件
- [ ] 詳細配置側邊欄設計
- [ ] 數據引用和自動完成

---

**文檔狀態**: Part 1/7 完成 ✅
**設計參考**: n8n (主要), Temporal, Prefect
**下一階段**: Part 2 - 內聯配置與表達式系統
**預計完成**: 2025-10-29
