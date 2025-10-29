# 模組 08: Structured Data + Text-to-SQL

**Semantic Kernel Agentic Framework - User Stories**

**模組**: Module 08
**User Stories**: US 8.1-8.4
**版本**: 2.0.0
**日期**: 2025-10-28

[返回總覽](../user-stories.md)

---

### 📦 模組 8: Structured Data + Text-to-SQL ⭐ 核心差異化能力

**Epic**: 讓 Agent 能夠理解和查詢結構化數據（SQL、Excel、CSV），並自動生成安全的 SQL 查詢

**差異化價值**:
- 超越 Copilot Studio 的數據查詢能力
- 自然語言轉 SQL，無需技術背景
- 企業級安全防護（SQL Injection、權限控制）

**User Stories 數量**: 4 個（US 8.1 - 8.4）

---

#### US 8.1 - Schema 理解與上下文構建

**作為** IT 開發者
**我想要** Agent 能夠理解資料庫 Schema 和表關係
**以便** 準確回答業務查詢

**驗收標準**:

✅ 必須項（Schema 自動發現）:
- [ ] 支援主流資料庫（SQL Server、PostgreSQL、MySQL、SQLite）
- [ ] 自動讀取 Table Schema（表名、欄位名、資料型別）
- [ ] 自動偵測 Primary Key 和 Foreign Key 關係
- [ ] 自動提取 Index 和 Constraint 資訊
- [ ] 支援 Schema 快取（避免每次查詢都重新讀取）

✅ 必須項（Schema 語義增強）:
- [ ] 允許為表和欄位添加業務描述（例如：`customer_id` → "客戶唯一識別碼"）
- [ ] 支援同義詞配置（例如：`訂單` = `order` = `orders`）
- [ ] 支援業務術語對應（例如：`銷售額` = `SUM(amount)`）
- [ ] Schema 版本控制（Schema 變更時自動更新）

✅ 必須項（上下文構建）:
- [ ] 將 Schema 資訊轉換為 LLM 友好的描述
- [ ] 包含表關係說明（"customers 和 orders 通過 customer_id 關聯"）
- [ ] 包含業務邏輯說明（"銷售額 = 單價 × 數量"）
- [ ] 包含範例查詢（Few-shot Learning）

**Schema 描述範例**:
```yaml
# Schema Context for LLM

tables:
  - name: customers
    description: "客戶資料表，儲存所有客戶基本資訊"
    columns:
      - name: customer_id
        type: INT
        description: "客戶唯一識別碼"
        primary_key: true
      - name: name
        type: VARCHAR(100)
        description: "客戶姓名"
      - name: email
        type: VARCHAR(100)
        description: "客戶電子郵件"

  - name: orders
    description: "訂單資料表，儲存所有訂單紀錄"
    columns:
      - name: order_id
        type: INT
        description: "訂單唯一識別碼"
        primary_key: true
      - name: customer_id
        type: INT
        description: "客戶ID（關聯到 customers 表）"
        foreign_key: customers.customer_id
      - name: amount
        type: DECIMAL(10,2)
        description: "訂單金額"
      - name: order_date
        type: DATE
        description: "訂單日期"

relationships:
  - type: "one-to-many"
    from: "customers.customer_id"
    to: "orders.customer_id"
    description: "一個客戶可以有多筆訂單"

business_terms:
  - term: "銷售額"
    definition: "SUM(orders.amount)"
  - term: "客戶總消費"
    definition: "SUM(orders.amount) GROUP BY customer_id"

synonyms:
  客戶: ["customer", "顧客", "用戶"]
  訂單: ["order", "訂購單"]

example_queries:
  - question: "列出所有客戶"
    sql: "SELECT * FROM customers;"
  - question: "查詢最近一個月的訂單"
    sql: "SELECT * FROM orders WHERE order_date >= DATEADD(month, -1, GETDATE());"
```

**技術實現**:
- JDBC/ODBC Schema Metadata API
- Schema Cache（Redis 或 In-Memory）
- Schema 語義增強配置檔（YAML）
- LLM Context Builder

**📊 優先級**: P0 (MVP 必須) - Text-to-SQL 基礎
**🎯 技術目標**: Schema 載入 <500ms（含快取）
**🔗 相關**: US 8.2 (Text-to-SQL 生成), US 5.4 (Excel 資料)

---

#### US 8.2 - 安全的 Text-to-SQL 生成

**作為** 業務用戶
**我想要** 用自然語言查詢資料庫
**以便** 無需學習 SQL 語法即可獲取資料

**驗收標準**:

✅ 必須項（自然語言理解）:
- [ ] 支援中文和英文查詢
- [ ] 理解複雜查詢（JOIN、聚合、子查詢、排序）
- [ ] 理解模糊查詢（"類似 ABC 的客戶" → `LIKE '%ABC%'`）
- [ ] 理解時間範圍（"最近一週" → `WHERE date >= DATEADD(week, -1, GETDATE())`）
- [ ] 理解業務術語（"銷售額" → `SUM(amount)`）

✅ 必須項（SQL 生成品質）:
- [ ] 生成可執行的標準 SQL
- [ ] 自動選擇最佳 JOIN 策略
- [ ] 避免笛卡爾積和性能問題
- [ ] 添加適當的 WHERE 條件避免全表掃描
- [ ] 限制查詢結果數量（預設 TOP 1000）

✅ 必須項（安全防護 - SQL Injection）:
- [ ] 使用參數化查詢（Prepared Statements）
- [ ] 禁止執行 DDL 語句（CREATE、DROP、ALTER）
- [ ] 禁止執行 DML 語句（INSERT、UPDATE、DELETE）- 僅允許 SELECT
- [ ] 禁止執行系統命令（xp_cmdshell、EXEC 等）
- [ ] SQL 語句黑名單過濾
- [ ] 自動偵測惡意 SQL 模式

✅ 必須項（查詢驗證）:
- [ ] 生成 SQL 後先進行語法驗證
- [ ] 執行前估算查詢成本（避免過於複雜的查詢）
- [ ] 設定執行時間限制（例如 30 秒）
- [ ] 設定記憶體限制（避免 OOM）
- [ ] 失敗後提供友善的錯誤訊息

**Text-to-SQL 範例**:
```python
# 使用者查詢
"顯示最近一個月銷售額最高的前 10 名客戶"

# Agent 內部流程
Step 1: 理解意圖
- 需要表: customers, orders
- 時間範圍: 最近一個月
- 聚合: SUM(amount)
- 排序: DESC
- 限制: TOP 10

Step 2: 生成 SQL（含安全防護）
generated_sql = """
SELECT TOP 10
    c.customer_id,
    c.name,
    SUM(o.amount) AS total_sales
FROM customers c
INNER JOIN orders o ON c.customer_id = o.customer_id
WHERE o.order_date >= DATEADD(month, -1, GETDATE())
GROUP BY c.customer_id, c.name
ORDER BY total_sales DESC
"""

Step 3: 安全檢查
✅ 僅使用 SELECT
✅ 無 DDL/DML
✅ 無系統命令
✅ 使用參數化查詢（時間範圍）
✅ 限制結果數量（TOP 10）

Step 4: 驗證和執行
- 語法驗證: ✅ 通過
- 成本估算: ✅ 可接受（預估 100ms）
- 執行查詢: ✅ 成功
- 結果: 10 筆客戶資料

Step 5: 格式化回應
"以下是最近一個月銷售額最高的前 10 名客戶：

| 客戶ID | 姓名 | 總銷售額 |
|--------|------|----------|
| 1001   | ABC  | $50,000  |
| 1002   | XYZ  | $48,000  |
...
"
```

**技術實現**:
- LLM Text-to-SQL（GPT-4、Claude、或開源模型）
- SQL Parser 和 Validator
- Prepared Statement Engine
- SQL 黑名單和模式檢測
- Query Cost Estimator

**📊 優先級**: P0 (MVP 必須) - 核心功能
**🎯 準確率目標**: >85%（簡單查詢）、>70%（複雜查詢）
**🔗 相關**: US 8.1 (Schema 理解), US 8.3 (權限控制)

---

#### US 8.3 - 細粒度權限控制（Row-Level Security）

**作為** 系統管理員
**我想要** 控制不同用戶能查詢哪些資料
**以便** 確保資料安全和隱私合規

**驗收標準**:

✅ 必須項（表級權限）:
- [ ] 配置每個用戶/角色可存取的表
- [ ] 支援白名單模式（僅允許明確授權的表）
- [ ] 支援黑名單模式（禁止特定敏感表）
- [ ] 動態權限檢查（每次查詢前驗證）

✅ 必須項（欄位級權限）:
- [ ] 配置每個用戶/角色可查詢的欄位
- [ ] 自動過濾敏感欄位（例如：密碼、信用卡號）
- [ ] 支援欄位遮罩（例如：顯示 `***-**-1234` 而非完整身分證）
- [ ] 禁止查詢未授權欄位（自動從 SELECT 中移除）

✅ 必須項（行級權限 - Row-Level Security）:
- [ ] 根據用戶角色自動添加 WHERE 條件
- [ ] 支援多租戶隔離（`WHERE tenant_id = @current_user_tenant`）
- [ ] 支援部門隔離（`WHERE department = @current_user_department`）
- [ ] 支援區域隔離（銷售人員僅能看自己區域的資料）

✅ 必須項（審計日誌）:
- [ ] 記錄所有查詢（誰、何時、查詢什麼、結果多少筆）
- [ ] 記錄權限拒絕事件（嘗試存取未授權資料）
- [ ] 查詢歷史可追溯（至少保留 90 天）
- [ ] 異常查詢告警（例如：大量資料匯出）

**權限配置範例**:
```yaml
# 權限配置檔
permissions:
  role: "sales_representative"  # 銷售代表

  table_access:
    allowed_tables:
      - customers
      - orders
      - products
    forbidden_tables:
      - employees
      - salaries
      - internal_memos

  column_access:
    customers:
      allowed: ["customer_id", "name", "email", "phone"]
      forbidden: ["ssn", "credit_card"]  # 敏感資料
      masked: ["phone"]  # 部分遮罩: "0912-***-456"

  row_level_security:
    customers:
      # 銷售人員僅能看自己區域的客戶
      filter: "region = @current_user_region"

    orders:
      # 銷售人員僅能看自己區域的訂單
      filter: "customer_id IN (SELECT customer_id FROM customers WHERE region = @current_user_region)"

  query_limits:
    max_rows: 1000  # 每次查詢最多返回 1000 筆
    max_execution_time: 30  # 查詢最多執行 30 秒
```

**執行範例**:
```sql
-- 用戶查詢（銷售代表 John，區域=北區）
"顯示所有客戶"

-- Agent 自動添加 RLS 條件
SELECT customer_id, name, email, CONCAT(SUBSTRING(phone, 1, 4), '-***-', SUBSTRING(phone, 9, 3)) AS phone
FROM customers
WHERE region = '北區'  -- 自動添加
LIMIT 1000;

-- 審計日誌
{
  "user": "john@company.com",
  "role": "sales_representative",
  "timestamp": "2025-10-28T14:30:00Z",
  "query": "顯示所有客戶",
  "generated_sql": "SELECT ... WHERE region = '北區'",
  "rows_returned": 150,
  "execution_time_ms": 45
}
```

**技術實現**:
- 權限配置管理系統
- SQL Rewriter（自動添加 RLS 條件）
- 欄位遮罩引擎
- 審計日誌系統（寫入資料庫或 Log 系統）

**📊 優先級**: P0 (MVP 必須) - 企業級安全
**🎯 安全目標**: 0 權限繞過漏洞
**🔗 相關**: US 8.2 (Text-to-SQL), US 9.1 (RBAC)

---

#### US 8.4 - 查詢結果展示與互動

**作為** 業務用戶
**我想要** 清晰易讀的查詢結果展示
**以便** 快速理解資料洞察

**驗收標準**:

✅ 必須項（結果展示）:
- [ ] 表格格式展示（支援排序、分頁）
- [ ] 數據視覺化（自動生成圖表，當適合時）
- [ ] 摘要統計（總筆數、平均值、最大/最小值）
- [ ] 資料型別友善顯示（日期格式化、貨幣符號、千分位）

✅ 必須項（互動式探索）:
- [ ] 點擊欄位可進一步查詢（"點擊客戶名稱查看詳細資料"）
- [ ] 結果可匯出（CSV、Excel、JSON）
- [ ] 支援 Follow-up 查詢（"再顯示這些客戶的訂單明細"）
- [ ] 查詢歷史導航（"回到上一個查詢"）

✅ 必須項（智能建議）:
- [ ] 根據查詢結果推薦下一步（"您可能想看：銷售趨勢圖"）
- [ ] 異常值告警（"客戶 ABC 的消費突然增加 300%"）
- [ ] 業務洞察提示（"本月銷售額比上月下降 15%"）

✅ 必須項（錯誤處理）:
- [ ] SQL 語法錯誤時，提供修正建議
- [ ] 查詢超時時，建議簡化查詢
- [ ] 無結果時，提供替代查詢建議
- [ ] 權限錯誤時，說明原因和解決方法

**展示範例**:
```
User: "最近一個月銷售額最高的客戶"

Agent: "✅ 查詢完成（45ms，共 10 筆資料）

📊 最近一個月銷售額最高的前 10 名客戶

| 排名 | 客戶ID | 姓名 | 總銷售額 | 訂單數 | 平均訂單金額 |
|------|--------|------|----------|--------|--------------|
| 1    | 1001   | ABC  | $50,000  | 25     | $2,000       |
| 2    | 1002   | XYZ  | $48,000  | 30     | $1,600       |
| ...

📈 摘要統計
- 總銷售額: $350,000
- 平均銷售額: $35,000
- 最高單筆: $50,000 (ABC)

💡 洞察
- 🔥 客戶 ABC 的銷售額比上月成長 45%
- ⚠️ 客戶 XYZ 的平均訂單金額下降 20%

🔍 您可能還想查詢：
1️⃣ 這些客戶的訂單明細
2️⃣ 銷售額趨勢圖（過去 6 個月）
3️⃣ 按產品類別分析

[匯出 CSV] [匯出 Excel] [查看 SQL]
"
```

**技術實現**:
- 結果格式化引擎
- 圖表自動生成（Chart.js、D3.js）
- 統計分析模組
- 洞察生成（LLM 分析結果）

**📊 優先級**: P1 (MVP 高優先) - UX 增強
**🎯 UX 目標**: 結果渲染 <500ms
**🔗 相關**: US 6.5 (表格展示), US 5.5 (Knowledge 查詢)

---