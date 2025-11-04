# Workflow 編輯器 Wireframe - Part 2: 節點配置系統

**版本**: 1.0.0 (Part 2/5)
**日期**: 2025-10-29
**狀態**: 🔄 進行中
**複雜度**: ⭐⭐⭐⭐⭐ (極高)

---

## 📊 數據處理節點（5 種）

### 15. Transform Node（數據轉換節點）

```yaml
類型: transform
圖標: 🔄
顏色: #8B5CF6 (紫色)
用途: 轉換數據格式和結構
輸入: 1 個（原始數據）
輸出: 1 個（轉換後數據）

配置屬性:
  - 轉換類型:
      - JSON to XML（JSON 轉 XML）
      - XML to JSON（XML 轉 JSON）
      - CSV to JSON（CSV 轉 JSON）
      - JSON to CSV（JSON 轉 CSV）
      - Custom Transform（自定義轉換）
  - 轉換腳本:
      - JavaScript/TypeScript 代碼
      - JSONata 表達式
      - XSLT 樣式表
  - 映射規則:
      - 欄位映射定義
      - 預設值設定
      - 空值處理

範例配置:
  轉換類型: JSON to CSV
  映射規則:
    - name → 姓名
    - email → 電子郵件
    - age → 年齡
  空值處理: 使用 "N/A"

連接規則:
  - 輸入必須是有效的數據格式
  - 輸出格式必須匹配下游節點需求

視覺樣式:
  ┌─────────────┐
  │  🔄 Trans   │
  │  JSON→CSV   │
  └──────┬──────┘
         ▼
```

---

### 16. Query Node（數據查詢節點）

```yaml
類型: query
圖標: 🔍
顏色: #06B6D4 (青色)
用途: 查詢和過濾數據
輸入: 1 個（數據源）
輸出: 1 個（查詢結果）

配置屬性:
  - 查詢語言:
      - JSONPath（JSON 查詢）
      - XPath（XML 查詢）
      - SQL-like（類 SQL 語法）
      - GraphQL（圖查詢）
  - 查詢表達式:
      - 可視化查詢構建器
      - 手動輸入表達式
  - 過濾條件:
      - 比較條件（==, !=, >, <）
      - 邏輯組合（AND, OR, NOT）
      - 模糊匹配（contains, startsWith）
  - 結果選項:
      - 返回單一結果
      - 返回結果數組
      - 返回計數

範例配置:
  查詢語言: JSONPath
  查詢表達式: $.users[?(@.age > 18)]
  過濾條件: status == "active"
  結果選項: 返回數組

連接規則:
  - 輸入必須是結構化數據
  - 查詢表達式必須有效

視覺樣式:
  ┌─────────────┐
  │  🔍 Query   │
  │  Filter Users│
  │  age > 18   │
  └──────┬──────┘
         ▼
```

---

### 17. Format Node（格式化節點）

```yaml
類型: format
圖標: 📝
顏色: #10B981 (綠色)
用途: 格式化文本和數據輸出
輸入: 1 個（數據）
輸出: 1 個（格式化結果）

配置屬性:
  - 格式化類型:
      - String Template（字符串模板）
      - Markdown（Markdown 格式）
      - HTML（HTML 格式）
      - JSON Pretty Print（JSON 美化）
      - Custom Formatter（自定義）
  - 模板引擎:
      - Handlebars
      - Mustache
      - EJS
      - Template Literals
  - 模板內容:
      - 使用 {{variable}} 綁定變量
      - 支持條件語句和循環
  - 輸出編碼:
      - UTF-8
      - ASCII
      - Base64

範例配置:
  格式化類型: String Template
  模板引擎: Handlebars
  模板內容: |
    Hello {{name}},
    Your order {{orderId}} has been shipped.
    Tracking: {{trackingNumber}}

連接規則:
  - 輸入數據必須包含模板所需變量
  - 模板語法必須有效

視覺樣式:
  ┌─────────────┐
  │  📝 Format  │
  │  Email Tmpl │
  └──────┬──────┘
         ▼
```

---

### 18. Calculate Node（計算節點）

```yaml
類型: calculate
圖標: 🧮
顏色: #F59E0B (橙色)
用途: 執行數學計算和統計分析
輸入: 1-N 個（數值數據）
輸出: 1 個（計算結果）

配置屬性:
  - 計算類型:
      - 基本運算: +, -, *, /, %
      - 數學函數: sqrt, pow, log, abs
      - 統計函數: sum, avg, min, max, median
      - 自定義公式: JavaScript 表達式
  - 公式定義:
      - 可視化公式構建器
      - 手動輸入表達式
      - 引用前置節點數據
  - 精度設定:
      - 小數位數
      - 四捨五入規則
  - 單位轉換:
      - 貨幣換算
      - 單位換算（長度、重量等）

範例配置:
  計算類型: 自定義公式
  公式: (price * quantity * (1 - discount)) * (1 + tax)
  精度: 2 位小數
  單位: USD

連接規則:
  - 輸入必須是數值或可轉換為數值
  - 公式中的變量必須存在

視覺樣式:
  ┌─────────────┐
  │  🧮 Calc    │
  │  Total Price│
  │  w/ Tax     │
  └──────┬──────┘
         ▼
```

---

### 19. Storage Node（儲存節點）

```yaml
類型: storage
圖標: 📂
顏色: #6366F1 (靛藍色)
用途: 儲存數據到持久化存儲
輸入: 1 個（要儲存的數據）
輸出: 1 個（儲存確認 + 數據 ID）

配置屬性:
  - 儲存位置:
      - Database（資料庫）
      - File System（文件系統）
      - Object Storage（對象存儲 S3）
      - Cache（快取 Redis）
      - Workflow Memory（流程記憶體）
  - 儲存設定:
      - 資料庫連接字串
      - 表名/集合名
      - 文件路徑/Bucket
      - Key 生成規則
  - 儲存策略:
      - Overwrite（覆蓋）
      - Append（追加）
      - Upsert（存在則更新）
      - Create New（創建新記錄）
  - 過期設定:
      - TTL（生存時間）
      - 自動清理規則

範例配置:
  儲存位置: Database
  連接: workflow_db
  表名: execution_results
  策略: Upsert
  Key: workflow_id + timestamp

連接規則:
  - 必須配置有效的儲存連接
  - 輸入數據必須可序列化

視覺樣式:
  ┌─────────────┐
  │  📂 Store   │
  │  DB: results│
  │  Upsert     │
  └──────┬──────┘
         ▼
```

---

## 🔌 整合節點（5 種）

### 20. Database Node（資料庫節點）

```yaml
類型: database
圖標: 🗄️
顏色: #DC2626 (紅色)
用途: 執行資料庫查詢和操作
輸入: 1 個（查詢參數）
輸出: 1 個（查詢結果）

配置屬性:
  - 資料庫類型:
      - PostgreSQL
      - MySQL / MariaDB
      - MongoDB
      - SQL Server
      - SQLite
  - 連接設定:
      - 連接字串（支持環境變量）
      - 連接池設定
      - 超時時間
  - 操作類型:
      - SELECT（查詢）
      - INSERT（插入）
      - UPDATE（更新）
      - DELETE（刪除）
      - STORED PROCEDURE（存儲過程）
  - SQL 查詢:
      - 手動輸入 SQL
      - 可視化查詢構建器
      - 參數化查詢（防 SQL 注入）
  - 結果處理:
      - 返回格式（JSON/Array）
      - 結果限制（最大行數）
      - 錯誤處理策略

範例配置:
  資料庫: PostgreSQL
  連接: ${DB_CONNECTION_STRING}
  操作: SELECT
  查詢: |
    SELECT * FROM users
    WHERE created_at > $1
    ORDER BY created_at DESC
    LIMIT 100
  參數: [startDate]

連接規則:
  - 必須有有效的資料庫連接
  - SQL 語法必須正確
  - 參數必須匹配查詢

視覺樣式:
  ┌─────────────┐
  │  🗄️ DB      │
  │  PostgreSQL │
  │  SELECT User│
  └──────┬──────┘
         ▼
```

---

### 21. API Node（API 調用節點）

```yaml
類型: api
圖標: 🌐
顏色: #0EA5E9 (天藍色)
用途: 調用外部 HTTP/REST API
輸入: 1 個（請求參數）
輸出: 1 個（API 響應）

配置屬性:
  - HTTP 方法:
      - GET
      - POST
      - PUT
      - PATCH
      - DELETE
  - 請求設定:
      - URL（支持變量替換）
      - Headers（請求頭）
      - Query Parameters（查詢參數）
      - Body（請求體）
  - 認證方式:
      - None（無認證）
      - API Key（API 密鑰）
      - Bearer Token（JWT）
      - Basic Auth（基本認證）
      - OAuth 2.0
  - 請求選項:
      - 超時時間（秒）
      - 重試次數
      - 重試延遲策略
  - 響應處理:
      - 響應格式（JSON/XML/Text）
      - 狀態碼處理
      - 錯誤映射

範例配置:
  方法: POST
  URL: https://api.example.com/v1/users
  Headers:
    Content-Type: application/json
    Authorization: Bearer ${API_TOKEN}
  Body: |
    {
      "name": "{{userName}}",
      "email": "{{userEmail}}"
    }
  超時: 30 秒
  重試: 3 次

連接規則:
  - URL 必須有效
  - 認證信息必須正確
  - 請求體必須符合 API 規範

視覺樣式:
  ┌─────────────┐
  │  🌐 API     │
  │  POST User  │
  │  /v1/users  │
  └──────┬──────┘
         ▼
```

---

### 22. Email Node（郵件節點）

```yaml
類型: email
圖標: 📧
顏色: #EF4444 (紅色)
用途: 發送電子郵件通知
輸入: 1 個（郵件內容）
輸出: 1 個（發送狀態）

配置屬性:
  - SMTP 設定:
      - SMTP 伺服器
      - 端口（25/465/587）
      - 使用 SSL/TLS
      - 認證信息
  - 郵件設定:
      - 收件人（To）: 單一或多個
      - 副本（Cc）: 可選
      - 密送（Bcc）: 可選
      - 寄件人（From）
      - 主題（Subject）
      - 內容（Body）
  - 內容類型:
      - Plain Text（純文字）
      - HTML（富文本）
      - Markdown（自動轉 HTML）
  - 附件:
      - 支持多個附件
      - 來源: URL/File Path/Base64
      - 大小限制
  - 模板:
      - 使用預設模板
      - 變量替換

範例配置:
  SMTP: smtp.gmail.com:587
  寄件人: noreply@example.com
  收件人: {{userEmail}}
  主題: Your workflow {{workflowName}} completed
  內容類型: HTML
  內容: |
    <h1>Workflow Completed</h1>
    <p>Status: {{status}}</p>
    <p>Duration: {{duration}}</p>

連接規則:
  - SMTP 連接必須有效
  - 收件人郵箱必須有效
  - 郵件內容不可為空

視覺樣式:
  ┌─────────────┐
  │  📧 Email   │
  │  Send Notif │
  │  to: user   │
  └──────┬──────┘
         ▼
```

---

### 23. File Node（檔案節點）

```yaml
類型: file
圖標: 📁
顏色: #F59E0B (橙色)
用途: 讀取、寫入、處理檔案
輸入: 1 個（檔案路徑或內容）
輸出: 1 個（處理結果）

配置屬性:
  - 操作類型:
      - Read（讀取）
      - Write（寫入）
      - Append（追加）
      - Delete（刪除）
      - Move（移動）
      - Copy（複製）
  - 檔案位置:
      - Local File System（本地）
      - Cloud Storage（雲端 S3/Azure/GCS）
      - FTP/SFTP
      - Network Share
  - 讀取設定:
      - 編碼（UTF-8/ASCII/Binary）
      - 解析格式（JSON/CSV/XML/Text）
      - 行號範圍（可選）
  - 寫入設定:
      - 覆蓋/追加模式
      - 檔案權限
      - 備份舊檔案
  - 處理選項:
      - 壓縮/解壓縮
      - 加密/解密
      - 格式轉換

範例配置:
  操作: Read
  位置: Cloud Storage (S3)
  Bucket: workflow-files
  路徑: /data/{{fileName}}
  編碼: UTF-8
  解析: CSV
  Header: true

連接規則:
  - 檔案路徑必須有效
  - 必須有讀寫權限
  - 檔案格式必須匹配

視覺樣式:
  ┌─────────────┐
  │  📁 File    │
  │  Read CSV   │
  │  S3: /data/ │
  └──────┬──────┘
         ▼
```

---

### 24. Notification Node（通知節點）

```yaml
類型: notification
圖標: 🔔
顏色: #8B5CF6 (紫色)
用途: 發送各種類型的通知
輸入: 1 個（通知內容）
輸出: 1 個（發送狀態）

配置屬性:
  - 通知渠道:
      - Slack（Slack 訊息）
      - Teams（Microsoft Teams）
      - Discord（Discord Webhook）
      - Webhook（自定義 Webhook）
      - SMS（簡訊）
      - Push Notification（推播通知）
  - Slack 設定:
      - Webhook URL
      - Channel（頻道）
      - Username（顯示名稱）
      - Icon Emoji（圖標）
  - 訊息設定:
      - 標題
      - 內容（支持 Markdown）
      - 優先級（Low/Normal/High/Urgent）
      - 附件（圖片/文件鏈接）
  - 目標:
      - 用戶 ID
      - 群組 ID
      - 頻道名稱
  - 模板:
      - 成功通知模板
      - 失敗通知模板
      - 自定義模板

範例配置:
  渠道: Slack
  Webhook: ${SLACK_WEBHOOK_URL}
  Channel: #workflows
  標題: Workflow Alert
  內容: |
    Workflow: {{workflowName}}
    Status: {{status}}
    {{#if error}}
    Error: {{error}}
    {{/if}}
  優先級: High

連接規則:
  - Webhook URL 必須有效
  - 目標必須存在
  - 訊息內容不可為空

視覺樣式:
  ┌─────────────┐
  │  🔔 Notify  │
  │  Slack      │
  │  #workflows │
  └──────┬──────┘
         ▼
```

---

## 💼 進階功能節點（4 種）

### 25. Memory Node（記憶節點）

```yaml
類型: memory
圖標: 🧠
顏色: #EC4899 (粉紅色)
用途: 管理工作流程記憶體和狀態
輸入: 1-2 個（讀取 Key 或寫入數據）
輸出: 1 個（記憶內容或確認）

配置屬性:
  - 操作模式:
      - Get（讀取）
      - Set（寫入）
      - Update（更新）
      - Delete（刪除）
      - List（列出所有 Key）
  - 記憶範圍:
      - Workflow Scope（流程範圍 - 單次執行）
      - Agent Scope（Agent 範圍 - 跨執行）
      - Global Scope（全局範圍 - 跨流程）
  - Key 管理:
      - Key 名稱（唯一識別）
      - 命名空間（可選）
      - 過期時間（TTL）
  - 值類型:
      - String（字符串）
      - Number（數字）
      - Object（對象）
      - Array（數組）
  - 版本控制:
      - 啟用版本歷史
      - 保留版本數量

範例配置:
  操作: Set
  範圍: Agent Scope
  Key: user_preferences
  Value: {{userData}}
  TTL: 30 天
  版本: 保留最近 5 個

連接規則:
  - Get 操作必須確保 Key 存在
  - Set 操作數據必須可序列化

視覺樣式:
  ┌─────────────┐
  │  🧠 Memory  │
  │  Set: prefs │
  │  TTL: 30d   │
  └──────┬──────┘
         ▼
```

---

### 26. Permission Node（權限節點）

```yaml
類型: permission
圖標: 🔐
顏色: #DC2626 (紅色)
用途: 檢查和管理執行權限
輸入: 1 個（權限檢查請求）
輸出: 2 個（允許路徑 + 拒絕路徑）

配置屬性:
  - 權限類型:
      - Role-Based（基於角色）
      - User-Based（基於用戶）
      - Resource-Based（基於資源）
      - Attribute-Based（基於屬性 ABAC）
  - 權限規則:
      - 必需角色: [Admin, Editor, Viewer]
      - 必需權限: [read, write, execute]
      - 自定義規則: JavaScript 表達式
  - 檢查模式:
      - All（需要全部權限）
      - Any（需要任一權限）
      - Custom（自定義邏輯）
  - 行為設定:
      - 拒絕時動作:
          - Block（阻止繼續）
          - Log and Continue（記錄並繼續）
          - Request Approval（請求審批）
      - 審批流程: 定義審批者

範例配置:
  權限類型: Role-Based
  必需角色: [Admin, Manager]
  檢查模式: Any
  拒絕動作: Request Approval
  審批者: admin@example.com

連接規則:
  - 必須配置權限規則
  - 允許和拒絕路徑必須都有連接

視覺樣式:
      ┌─────────────┐
      │  🔐 Auth    │
      │  Role Check │
      └──┬────────┬─┘
    Allowed  Denied
        ▼        ▼
```

---

### 27. Analytics Node（分析節點）

```yaml
類型: analytics
圖標: 📊
顏色: #10B981 (綠色)
用途: 記錄和分析執行數據
輸入: 1 個（要分析的數據）
輸出: 1 個（分析結果 + 原始數據）

配置屬性:
  - 分析類型:
      - Event Tracking（事件追蹤）
      - Metrics Collection（指標收集）
      - User Behavior（用戶行為）
      - Performance Monitoring（性能監控）
  - 追蹤事件:
      - 事件名稱
      - 事件屬性
      - 時間戳
      - 關聯 ID
  - 指標定義:
      - 指標名稱
      - 指標類型（Counter/Gauge/Histogram）
      - 單位
      - 標籤（Tags）
  - 數據目標:
      - Google Analytics
      - Mixpanel
      - Amplitude
      - Custom Endpoint
  - 聚合選項:
      - 實時聚合
      - 批次發送
      - 數據採樣率

範例配置:
  分析類型: Event Tracking
  事件: workflow_execution
  屬性:
    workflow_id: {{workflowId}}
    duration: {{duration}}
    status: {{status}}
  目標: Google Analytics
  採樣率: 100%

連接規則:
  - 分析目標必須配置
  - 不影響主流程執行

視覺樣式:
  ┌─────────────┐
  │  📊 Analytics│
  │  Track Event│
  │  GA4        │
  └──────┬──────┘
         ▼
```

---

### 28. Test Node（測試節點）

```yaml
類型: test
圖標: 🧪
顏色: #F59E0B (橙色)
用途: 執行自動化測試和斷言
輸入: 1 個（測試數據）
輸出: 2 個（通過路徑 + 失敗路徑）

配置屬性:
  - 測試類型:
      - Assertion（斷言測試）
      - Schema Validation（Schema 驗證）
      - Performance Test（性能測試）
      - Data Quality（數據質量）
  - 斷言規則:
      - Equals（等於）
      - Not Equals（不等於）
      - Contains（包含）
      - Matches Regex（匹配正則）
      - Greater Than / Less Than
      - Custom JavaScript
  - 驗證 Schema:
      - JSON Schema
      - OpenAPI Schema
      - Custom Validator
  - 測試條件:
      - 多個斷言（ALL/ANY）
      - 自定義錯誤訊息
      - 嚴重程度（Error/Warning）
  - 報告選項:
      - 記錄測試結果
      - 發送失敗通知
      - 生成測試報告

範例配置:
  測試類型: Assertion
  斷言規則:
    - data.status equals "success"
    - data.records.length > 0
    - data.timestamp matches ISO 8601
  條件: ALL
  失敗動作: 發送通知 + 記錄日誌

連接規則:
  - 必須定義測試條件
  - 通過和失敗路徑必須都有連接

視覺樣式:
      ┌─────────────┐
      │  🧪 Test    │
      │  Assert OK  │
      └──┬────────┬─┘
      Pass   Fail
        ▼      ▼
```

---

## ⚙️ 屬性面板完整設計

### 面板結構（詳細版）

```
┌──────────────────────────────────────┐
│ 節點屬性                            │
│ ┌────────────────────────────────┐   │
│ │ 🎯 Agent - 分類專家             │   │
│ │ ID: node_a1b2c3                │   │
│ │ 類型: agent_single             │   │
│ └────────────────────────────────┘   │
├──────────────────────────────────────┤
│ 標籤導航                            │
│ [📝 基本設定] [🔗 連接] [⚙️ 進階]  │
│ [📋 執行日誌] [📊 統計]             │
├──────────────────────────────────────┤
│                                      │
│ 【📝 基本設定 標籤】                 │
│                                      │
│ 節點名稱 *                          │
│ ┌────────────────────────────────┐   │
│ │ 分類專家                        │   │
│ └────────────────────────────────┘   │
│                                      │
│ 描述                                 │
│ ┌────────────────────────────────┐   │
│ │ 負責分類用戶問題到正確的處理    │   │
│ │ 類別                            │   │
│ │                                │   │
│ └────────────────────────────────┘   │
│                                      │
│ Agent 選擇 *                        │
│ ┌────────────────────────────────┐   │
│ │ ▼ 選擇 Agent                   │   │
│ ├────────────────────────────────┤   │
│ │   🎯 分類專家 (GPT-4)   ✓      │   │
│ │   💬 客服助手 (GPT-3.5)        │   │
│ │   📊 數據分析 (Claude)         │   │
│ └────────────────────────────────┘   │
│                                      │
│ 執行設定                            │
│ ┌─────────────┬──────────────────┐   │
│ │ Temperature │ [========>] 0.7  │   │
│ ├─────────────┼──────────────────┤   │
│ │ Max Tokens  │ [2000         ] │   │
│ ├─────────────┼──────────────────┤   │
│ │ Stream      │ ☑ 啟用串流輸出   │   │
│ └─────────────┴──────────────────┘   │
│                                      │
│ 提示詞模板                          │
│ ┌────────────────────────────────┐   │
│ │ 你是專業的問題分類專家。        │   │
│ │ 用戶問題: {{userInput}}        │   │
│ │ 請分類到以下類別之一:           │   │
│ │ - 技術支持                      │   │
│ │ - 帳務問題                      │   │
│ │ - 產品諮詢                      │   │
│ │▋                               │   │
│ └────────────────────────────────┘   │
│ [🔍 測試] [📋 從模板載入]           │
│                                      │
│ 節點圖標與顏色                       │
│ ┌───┬───┬───┬───┬───┬───┬───┬───┐   │
│ │🎯│💬│📊│🔍│⚙️│🎨│📁│🔔│   │
│ └───┴───┴───┴───┴───┴───┴───┴───┘   │
│ ┌───┬───┬───┬───┬───┬───┬───┬───┐   │
│ │🔴│🟠│🟡│🟢│🔵│🟣│⚫│⚪│   │
│ └───┴───┴───┴───┴───┴───┴───┴───┘   │
│                                      │
├──────────────────────────────────────┤
│ [✅ 套用變更] [❌ 取消] [🔄 重置]   │
└──────────────────────────────────────┘
```

### 連接標籤設計

```
【🔗 連接標籤】

輸入連接 (1)
┌──────────────────────────────────┐
│ 📥 Input Port: user_question     │
├──────────────────────────────────┤
│ 數據類型: String                 │
│ 來源節點: Input Node (node_001) │
│ 來源輸出: user_input             │
│ 必填: ☑                          │
│ 預設值: [空]                     │
│                                  │
│ 數據映射:                        │
│ ┌──────────────┬───────────────┐ │
│ │ 來源欄位     │ 目標變量      │ │
│ ├──────────────┼───────────────┤ │
│ │ user_input   │ {{userInput}} │ │
│ │              │ [+ 新增映射]  │ │
│ └──────────────┴───────────────┘ │
└──────────────────────────────────┘

輸出連接 (2)
┌──────────────────────────────────┐
│ 📤 Output Port: classification  │
├──────────────────────────────────┤
│ 數據類型: Object                 │
│ 輸出 Schema:                     │
│ {                                │
│   "category": "string",          │
│   "confidence": "number",        │
│   "reasoning": "string"          │
│ }                                │
│                                  │
│ 連接到:                          │
│ • Router Node (node_003)         │
│ • Output Node (node_005)         │
│                                  │
│ [🔍 測試輸出格式]                │
└──────────────────────────────────┘

連接驗證
┌──────────────────────────────────┐
│ ✅ 輸入數據類型匹配              │
│ ✅ 輸出 Schema 有效              │
│ ⚠️ 警告: 輸出未連接到終點節點    │
└──────────────────────────────────┘
```

### 進階標籤設計

```
【⚙️ 進階標籤】

執行控制
┌──────────────────────────────────┐
│ 超時設定                         │
│ ┌──────┐ 秒                      │
│ │ 60   │ ⏱️ (0 = 無限制)         │
│ └──────┘                         │
│                                  │
│ 重試策略                         │
│ ☑ 啟用自動重試                   │
│ 重試次數: [3  ] 次               │
│ 重試延遲: [2  ] 秒               │
│ 退避策略: ▼ 指數退避             │
│   • 固定延遲                     │
│   • 線性退避                     │
│   • 指數退避                     │
└──────────────────────────────────┘

錯誤處理
┌──────────────────────────────────┐
│ 失敗時動作:                      │
│ ○ 停止整個工作流程               │
│ ● 繼續執行（標記為失敗）         │
│ ○ 執行備用路徑                   │
│                                  │
│ 錯誤輸出:                        │
│ ☑ 記錄詳細錯誤訊息               │
│ ☑ 包含堆疊追蹤                   │
│ ☐ 發送錯誤通知                   │
└──────────────────────────────────┘

日誌與監控
┌──────────────────────────────────┐
│ 日誌級別:                        │
│ ▼ Info                           │
│   • Debug (詳細)                 │
│   • Info (一般)                  │
│   • Warning (警告)               │
│   • Error (僅錯誤)               │
│                                  │
│ 記錄內容:                        │
│ ☑ 輸入數據                       │
│ ☑ 輸出數據                       │
│ ☑ 執行時間                       │
│ ☐ Token 使用量                   │
└──────────────────────────────────┘

性能優化
┌──────────────────────────────────┐
│ ☑ 啟用結果快取                   │
│ 快取 TTL: [3600] 秒             │
│ 快取 Key: auto / 自定義          │
│                                  │
│ ☐ 啟用並行執行 (如適用)         │
│ 最大並行數: [5  ]               │
└──────────────────────────────────┘
```

---

## 🔗 連接規則驗證系統

### 連接兼容性矩陣

```yaml
連接規則:
  類型匹配:
    String:
      - 可連接: String, Any
      - 自動轉換: Number (嘗試解析), Boolean (truthy/falsy)
    Number:
      - 可連接: Number, Any
      - 自動轉換: String (toString)
    Object:
      - 可連接: Object, Any
      - 自動轉換: JSON String
    Array:
      - 可連接: Array, Any
      - 不可轉換其他類型

  必填驗證:
    - 標記為 Required 的輸入必須連接
    - 可通過預設值繞過連接要求

  循環檢測:
    - 不允許節點連接到自身
    - 不允許形成循環依賴（除非是 Loop 節點）

  分支限制:
    - Start 節點: 只能有輸出
    - End 節點: 只能有輸入
    - Condition/Router 節點: 必須有 2+ 輸出
    - Merge 節點: 必須有 2+ 輸入
```

### 驗證提示系統

```
視覺提示:
┌─────────────┐
│  🎯 Agent   │ ← 紅色邊框 + ❌ 圖標 = 驗證失敗
│  未配置     │
└──────┬──────┘
       ▼ 紅色虛線 = 連接無效

┌─────────────┐
│  💬 Input   │ ← 黃色邊框 + ⚠️ 圖標 = 警告
│  缺少驗證   │
└──────┬──────┘
       ▼ 黃色線 = 有警告

┌─────────────┐
│  📊 Output  │ ← 綠色邊框 + ✓ 圖標 = 正常
│  已就緒     │
└──────┬──────┘
       ▼ 綠色線 = 有效連接

錯誤訊息面板 (畫布底部):
┌──────────────────────────────────────────┐
│ ❌ 驗證錯誤 (2) ⚠️ 警告 (1)               │
├──────────────────────────────────────────┤
│ ❌ Agent Node (node_002): 未選擇 Agent   │
│    → 點擊前往配置                        │
│                                          │
│ ❌ Router Node (node_004): 路由規則未定義 │
│    → 點擊前往配置                        │
│                                          │
│ ⚠️ Output Node (node_006): 未連接到 End  │
│    → 自動連接 / 忽略                     │
└──────────────────────────────────────────┘
```

---

## ✅ Part 2 檢查清單

### 完成項目
- [✅] 數據處理節點定義 (5 種)
  - [✅] Transform, Query, Format, Calculate, Storage
- [✅] 整合節點定義 (5 種)
  - [✅] Database, API, Email, File, Notification
- [✅] 進階功能節點定義 (4 種)
  - [✅] Memory, Permission, Analytics, Test
- [✅] 完整屬性面板設計（3 個標籤）
- [✅] 連接標籤詳細設計
- [✅] 進階標籤詳細設計
- [✅] 連接規則驗證系統
- [✅] 視覺驗證提示系統

### 節點庫完整統計
```
✅ 總計 28 種專業節點類型:
  • 基礎節點: 5 種 (Start, End, Input, Output, Wait)
  • Agent 節點: 4 種 (Single, Multi, Loop, Stream)
  • 邏輯控制: 5 種 (Condition, Loop, Router, Merge, Timer)
  • 數據處理: 5 種 (Transform, Query, Format, Calculate, Storage)
  • 整合節點: 5 種 (Database, API, Email, File, Notification)
  • 進階功能: 4 種 (Memory, Permission, Analytics, Test)
```

### 下一步（Part 3）
- [ ] 執行引擎設計
- [ ] 實時執行追蹤 UI
- [ ] 變量檢查器
- [ ] 斷點調試功能
- [ ] 執行日誌面板
- [ ] 錯誤堆疊追蹤
- [ ] 性能分析工具

---

**文檔狀態**: Part 2/5 完成 ✅
**下一階段**: Part 3 - 執行與調試功能
**預計完成**: 2025-10-29
