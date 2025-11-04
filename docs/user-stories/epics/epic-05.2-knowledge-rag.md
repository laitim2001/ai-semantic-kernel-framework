# Epic 5.2: Knowledge RAG (Retrieval-Augmented Generation)

**Semantic Kernel Agentic Framework - Epic Documentation**

**Epic**: 5.2 - Knowledge RAG (知識檢索與生成)
**Module**: 05 - Agent Memory
**Priority**: P0 (MVP 必須) - **核心競爭力**
**Complexity**: High
**Story Points**: 20

[返回 Module 05](../modules/module-05-agent-memory.md) | [返回 Epic 索引](./README.md)

---

## Epic 描述

實現高準確率(90%+)的知識檢索能力,這是本項目的核心競爭力之一,必須顯著超越 Microsoft Copilot Studio。

**業務價值**:
- **差異化優勢**: 90%+ 檢索準確率 vs Copilot Studio
- 企業知識有效利用
- 減少錯誤回答和幻覺

**技術目標**:
- 多層檢索策略 (Vector + Keyword + Hybrid)
- Re-ranking 機制
- 準確率自動評估和優化

---

## User Stories

### US 5.2 - 精確檢索策略配置（達成 90%+ 準確率）⭐ 關鍵

**作為** IT 開發者
**我想要** 精細調整知識檢索策略
**以便** 達到 90% 以上的檢索準確率

**背景**:
這是本項目的核心競爭力之一，必須顯著超越 Copilot Studio 的檢索準確率。通過多層檢索策略和 Re-ranking，達到 90%+ 的目標。

**驗收標準**:

✅ 必須項（檢索策略）:

**策略 1: 向量搜尋（Semantic Search）**
- [ ] 使用 Embedding 進行語義搜尋
- [ ] 支援混合檢索（向量 + 關鍵字）
- [ ] 可調整向量相似度閾值（0.7-0.9）
- [ ] 支援多語言 Embedding

**策略 2: 關鍵字搜尋（Keyword Search）**
- [ ] BM25 演算法
- [ ] 全文檢索（Full-Text Search）
- [ ] 支援同義詞擴展
- [ ] 支援中文分詞

**策略 3: 混合檢索（Hybrid Search）**
- [ ] 向量搜尋 + 關鍵字搜尋
- [ ] 權重可調（向量 70% + 關鍵字 30%）
- [ ] Reciprocal Rank Fusion（RRF）合併結果

**策略 4: Re-ranking**
- [ ] 使用 Cross-Encoder 重排序
- [ ] 考慮文件新鮮度（最近更新優先）
- [ ] 考慮用戶反饋（點讚/點踩）

✅ 必須項（參數控制）:
- [ ] Top-K 設定（返回前 K 個結果，1-20）
- [ ] 相似度閾值（過濾低相關結果，0-1）
- [ ] Chunk 大小設定（500-2000 tokens）
- [ ] Chunk 重疊設定（0-200 tokens）
- [ ] Re-ranking 模型選擇

✅ 必須項（準確率測試）:

**建立標準測試集**:
- [ ] 準備 100+ 問答對
- [ ] 涵蓋不同問題類型（事實、推理、摘要）
- [ ] 涵蓋不同領域（產品、技術、政策）
- [ ] UAT 用戶標註正確答案和相關文檔

**自動評估**:
- [ ] 自動計算準確率（正確答案/總問題數）
- [ ] 計算 MRR（Mean Reciprocal Rank）
- [ ] 計算 NDCG（Normalized Discounted Cumulative Gain）
- [ ] 準確率 Dashboard 即時顯示

**A/B 測試**:
- [ ] 支援測試不同檢索策略
- [ ] 並排比較準確率
- [ ] 自動選擇最佳策略

**測試集範例**:
```json
{
  "test_cases": [
    {
      "question": "公司的退款政策是什麼？",
      "expected_answer": "30 天內可全額退款",
      "expected_sources": ["退款政策.pdf", "客服手冊.docx"],
      "expected_page": 2,
      "difficulty": "easy"
    },
    {
      "question": "如果產品有質量問題，退款後還能再次購買嗎？",
      "expected_answer": "可以，退款不影響再次購買資格",
      "expected_sources": ["退款政策.pdf", "FAQ.md"],
      "difficulty": "medium"
    }
  ]
}
```

**準確率計算**:
```
準確率 = (返回正確文檔的問題數 / 總問題數) × 100%

評分標準:
- 返回的 Top-1 結果包含正確答案: +1 分
- 返回的 Top-3 結果包含正確答案: +0.5 分
- 未返回正確答案: 0 分

目標: ≥90%（80% 是最低可接受門檻）
```

✅ 必須項（UI 介面）:
- [ ] 檢索策略配置頁面
- [ ] 參數滑桿調整（即時預覽效果）
- [ ] 測試查詢介面（輸入問題，查看檢索結果）
- [ ] 準確率 Dashboard
- [ ] 檢索結果解釋（為什麼返回這些結果）

**技術實現**:
- Azure AI Search Hybrid Search
- Cross-Encoder Re-ranking（HuggingFace）
- 測試框架（pytest + 自動化）

**📊 優先級**: P0 (MVP 必須，核心競爭力)
**🎯 準確率目標**: ≥90%（80% 是最低門檻）
**🔬 測試要求**: 通過 UAT 用戶驗收
**⏱️ 開發工期**: 2-3 週
**🔗 相關**: US 5.1 (文件上傳), ADR (Knowledge 架構)

---

### US 5.4 - Excel/CSV 數據源上傳 ⭐ 結構化數據

**作為** IT 開發者
**我想要** 上傳 Excel/CSV 文件作為 Agent 的知識庫
**以便** Agent 可以查詢和分析結構化數據

**驗收標準**:

✅ 必須項（文件處理）:
- [ ] 支援格式: .xlsx, .xls, .csv
- [ ] 自動檢測表頭和數據類型
- [ ] 處理多 Sheet Excel 文件（選擇要導入的 Sheet）
- [ ] 支援大文件（最多 100MB, 100 萬行）
- [ ] 數據預覽（前 100 行）
- [ ] 編碼自動檢測（UTF-8, Big5, GB2312）

✅ 必須項（Schema 理解）:
- [ ] 自動識別欄位名稱、類型、範例值
- [ ] 數據類型推斷（文字、數字、日期、布林）
- [ ] AI 生成欄位描述和業務含義
- [ ] 支援人工標註和修正
- [ ] 檢測主鍵、外鍵關係（多表場景）
- [ ] 數據質量報告（缺失值比例、異常值檢測）

✅ 必須項（語義標註）:
```
欄位名: revenue
類型: Number
描述: 銷售金額（自動生成或手動標註）
單位: 新台幣
別名: 營收、銷售額、收入
範例值: 50000, 12000, 8500
```

✅ 必須項（數據索引）:
- [ ] 將數據導入到數據庫（PostgreSQL 或專用表）
- [ ] 建立索引加速查詢
- [ ] 支援數據更新（覆蓋或追加）
- [ ] 數據版本管理

**UI 示例**:
```
┌─────────────────────────────────────────┐
│ 步驟 2/3: Schema 理解與標註              │
├─────────────────────────────────────────┤
│ 📊 檢測到的表格結構: sales_data.xlsx    │
│                                          │
│ ┌──────┬──────┬──────────┬──────────┐  │
│ │欄位名│ 類型 │  描述    │  範例值  │  │
│ ├──────┼──────┼──────────┼──────────┤  │
│ │date  │ Date │ 銷售日期 │2024-01-01│  │
│ │product Text │ 產品名稱 │ iPhone   │  │
│ │revenue Number│ 銷售金額 │ 50000    │  │
│ │region│ Text │ 銷售地區 │ 台北     │  │
│ └──────┴──────┴──────────┴──────────┘  │
│                                          │
│ 🏷️ 語義標註:                            │
│ revenue: [銷售金額] 單位: [新台幣]      │
│          別名: [營收, 銷售額]           │
│ region:  [銷售地區] 別名: [區域]        │
│                                          │
│ 💡 AI 建議:                              │
│ 檢測到時間序列數據，建議啟用趨勢分析    │
│                                          │
│     [< 上一步]  [儲存並啟用]             │
└─────────────────────────────────────────┘
```

**📊 優先級**: P0 (MVP 必須，核心差異化)
**🎯 驗收方式**: 可成功導入 10 萬行 Excel 數據
**🔗 相關**: US 5.5 (Text-to-SQL), US 3.1 (Code Interpreter)

---

### US 5.5 - 數據庫連接（Text-to-SQL）⭐ 核心能力

**作為** IT 開發者
**我想要** 連接數據庫作為 Agent 的知識源
**以便** Agent 可以直接查詢數據庫回答問題

**背景**:
這是選擇 Semantic Kernel 的重要原因之一。通過 Text-to-SQL，Agent 可以自然語言查詢結構化數據。

**驗收標準**:

✅ 必須項（連接管理）:
- [ ] 支援數據庫: SQL Server, PostgreSQL, MySQL
- [ ] 連接字串配置（加密儲存）
- [ ] 連接測試和健康檢查
- [ ] 連接池管理（避免連接耗盡）
- [ ] 連接逾時設定（5 秒）

✅ 必須項（Schema 抓取）:
- [ ] 自動抓取 Database Schema
- [ ] 提取表名、欄位名、資料類型
- [ ] 提取主鍵、外鍵關係
- [ ] 提取索引資訊
- [ ] 生成 ERD（實體關係圖）

✅ 必須項（Text-to-SQL 生成）:
- [ ] 自然語言 → SQL 查詢（使用 GPT-4）
- [ ] 支援基本查詢（SELECT, WHERE, ORDER BY）
- [ ] 支援聚合查詢（COUNT, SUM, AVG, GROUP BY）
- [ ] 支援 JOIN 查詢（多表關聯）
- [ ] 支援子查詢
- [ ] SQL 語法優化（效能考量）

✅ 必須項（SQL 安全性）⭐ 關鍵:

**唯讀權限**:
- [ ] 數據庫連接使用 **READ ONLY** 權限
- [ ] 禁止 DML: INSERT, UPDATE, DELETE
- [ ] 禁止 DDL: DROP, ALTER, CREATE
- [ ] 禁止系統表查詢（information_schema 除外）

**SQL Injection 防護**:
- [ ] 參數化查詢（Prepared Statements）
- [ ] SQL 白名單過濾（僅允許 SELECT）
- [ ] 特殊字元轉義
- [ ] SQL 語法驗證（防止惡意 SQL）

**執行限制**:
- [ ] 查詢超時: 5 秒（防止慢查詢）
- [ ] 結果集大小限制: 1000 行（防止記憶體溢出）
- [ ] 並發查詢限制: 每用戶 5 個/分鐘
- [ ] 查詢日誌審計（記錄所有 SQL）

✅ 必須項（執行流程）:
```
1. 用戶問題 → LLM 理解意圖
2. Schema 查詢 → 找到相關表和欄位
3. SQL 生成 → GPT-4 生成 SQL
4. SQL 驗證 → 安全性檢查、語法檢查
5. SQL 執行 → 數據庫查詢
6. 結果格式化 → 表格 + 可視化
7. 自然語言解釋 → Agent 解釋結果
```

**互動範例**:

**場景 1: 簡單查詢**
```
用戶: "上個月哪個產品賣最好？"

Agent: "我來查詢上個月的銷售數據..."

[生成 SQL]
SELECT product_name, SUM(revenue) as total_revenue
FROM sales
WHERE sale_date >= DATE_SUB(CURDATE(), INTERVAL 1 MONTH)
GROUP BY product_name
ORDER BY total_revenue DESC
LIMIT 1

[執行結果]
product_name | total_revenue
-------------|---------------
iPhone 15    | $1,250,000

Agent: "上個月銷售最好的產品是 **iPhone 15**，
       總銷售額達到 $1,250,000。"
```

**場景 2: 多表 JOIN 查詢**
```
用戶: "比較各地區的平均客單價"

Agent: "我需要關聯銷售表和訂單表..."

[生成 SQL]
SELECT
    r.region_name,
    AVG(o.total_amount) as avg_order_value,
    COUNT(o.order_id) as order_count
FROM orders o
JOIN customers c ON o.customer_id = c.customer_id
JOIN regions r ON c.region_id = r.region_id
WHERE o.order_date >= DATE_SUB(CURDATE(), INTERVAL 3 MONTH)
GROUP BY r.region_name
ORDER BY avg_order_value DESC

[執行結果 + 柱狀圖]

Agent: "各地區平均客單價分析：
🏆 台北: $1,200（訂單數: 1500）
📊 新竹: $1,150（訂單數: 800）
📉 台中: $950（訂單數: 2000）

新竹雖然訂單數較少，但客單價較高，
建議加強新竹地區的高端產品推廣。"
```

✅ 必須項（錯誤處理）:
- [ ] SQL 語法錯誤友好提示
- [ ] 查詢超時提示（建議優化查詢）
- [ ] 表/欄位不存在提示
- [ ] 權限不足提示
- [ ] Agent 自動修正 SQL 並重試（最多 2 次）

**技術實現**:
- ADO.NET / Dapper（數據庫訪問）
- GPT-4 生成 SQL（Few-Shot Prompting）
- SQL Parser（語法驗證）
- SQL Formatter（可讀性優化）

**📊 優先級**: P0 (MVP 必須，核心原因)
**🎯 準確率目標**: SQL 生成成功率 >85%
**🔒 安全要求**: 通過安全審計，零 SQL Injection 風險
**⏱️ 開發工期**: 4-5 週
**🔗 相關**: US 5.4 (Excel/CSV), US 5.6 (可視化)

---

### US 5.6 - 智能數據可視化（自動圖表生成）

**作為** 業務用戶
**我想要** Agent 自動將查詢結果可視化
**以便** 更直觀理解數據

**驗收標準**:

✅ 必須項（自動圖表選擇）:
- [ ] 根據數據類型自動選擇圖表
- [ ] 時間 + 數值 → 折線圖/區域圖
- [ ] 類別 + 數值 → 柱狀圖/條形圖
- [ ] 兩個數值 → 散點圖
- [ ] 比例數據 → 圓餅圖/環形圖
- [ ] 多維度數據 → 熱力圖/堆疊柱狀圖

✅ 必須項（支援圖表類型）:
- [ ] 折線圖（Line Chart）
- [ ] 柱狀圖（Bar Chart）
- [ ] 圓餅圖（Pie Chart）
- [ ] 散點圖（Scatter Plot）
- [ ] 熱力圖（Heatmap）
- [ ] 區域圖（Area Chart）
- [ ] 箱型圖（Box Plot）

✅ 必須項（圖表功能）:
- [ ] 多維度數據自動分組和聚合
- [ ] 圖表標題自動生成（基於查詢問題）
- [ ] 軸標籤自動設定（單位、格式）
- [ ] 圖例自動生成
- [ ] 中文顯示支援
- [ ] 顏色主題配置

✅ 必須項（互動功能）:
- [ ] 圖表縮放（Zoom）
- [ ] 圖表平移（Pan）
- [ ] Tooltip 顯示詳細數據
- [ ] 圖例點擊顯示/隱藏數據系列
- [ ] 圖表可下載（PNG, SVG, PDF）

**智能推薦邏輯**:
```javascript
function selectChartType(data) {
  const columnTypes = analyzeColumnTypes(data);

  // 時間序列數據
  if (hasTimeColumn(columnTypes) && hasNumericColumn(columnTypes)) {
    return 'line_chart'; // 折線圖
  }

  // 類別數據
  if (hasCategoryColumn(columnTypes) && hasNumericColumn(columnTypes)) {
    if (data.length <= 10) {
      return 'bar_chart'; // 柱狀圖
    } else {
      return 'horizontal_bar_chart'; // 條形圖（數據多時橫向更易讀）
    }
  }

  // 兩個數值欄位
  if (numericColumnCount(columnTypes) >= 2) {
    return 'scatter_plot'; // 散點圖
  }

  // 比例數據（加總為 100%）
  if (isTotalPercentage(data)) {
    return 'pie_chart'; // 圓餅圖
  }

  // 預設
  return 'table'; // 表格
}
```

**技術實現**:
- Plotly.js / ECharts（圖表庫）
- D3.js（進階可視化，Phase 2）
- Chart.js（輕量級選項）

**📊 優先級**: P1 (MVP 高優先)
**🎯 UX 目標**: 90% 數據自動選擇正確圖表類型
**🔗 相關**: US 5.5 (Text-to-SQL), US 3.2 (Code Interpreter 可視化)

---

### US 5.7 - 跨數據源關聯查詢

**作為** IT 開發者
**我想要** Agent 能夠關聯多個數據源進行查詢
**以便** 回答複雜的跨源問題

**驗收標準**:

✅ 必須項（關聯定義）:
- [ ] 手動定義數據源間的關聯關係
- [ ] 指定關聯鍵（例如: sales.product_id = products.id）
- [ ] 支援一對一、一對多關聯
- [ ] 關聯關係可視化展示（ERD）

✅ 必須項（跨源查詢）:
- [ ] Agent 自動識別需要關聯的數據源
- [ ] 生成跨源查詢計劃
- [ ] 本地 JOIN 處理（當無法在資料庫層 JOIN 時）
- [ ] 查詢結果合併和去重

**範例場景**:
```
數據源 1: sales.xlsx（銷售數據）
- product_id
- quantity
- revenue

數據源 2: SQL DB products 表（產品詳情）
- id
- name
- category
- price

關聯: sales.product_id = products.id

用戶問: "銷售前 10 的產品類別是什麼？"

Agent 查詢計劃:
1. 從 sales.xlsx 查詢銷售 Top 10 產品（按 revenue 排序）
2. 提取 product_id 列表
3. 從 SQL DB 查詢這些產品的類別
4. JOIN 並聚合結果
5. 返回: 類別分佈表 + 圓餅圖
```

**技術實現**:
- 數據整合層（Data Integration Layer）
- 本地 SQL 引擎（SQLite in-memory）
- 查詢優化器

**📊 優先級**: P2 (Phase 2)
**🎯 技術難度**: High
**🔗 相關**: US 5.4 (Excel), US 5.5 (SQL)

---

## 技術實作

### 檢索架構

```yaml
Retrieval_Pipeline:
  Stage_1_Vector_Search:
    - Embedding Query
    - Azure AI Search Vector Search
    - Top-K Candidates (K=20)

  Stage_2_Keyword_Search:
    - BM25 Ranking
    - Full-Text Search
    - Top-K Candidates (K=20)

  Stage_3_Hybrid_Fusion:
    - Reciprocal Rank Fusion (RRF)
    - Merge Results
    - Top-K (K=10)

  Stage_4_Reranking:
    - Cross-Encoder Scoring
    - Freshness Factor
    - User Feedback Factor
    - Final Top-K (K=5)
```

### 準確率評估

```python
# 自動評估框架
class AccuracyEvaluator:
    def evaluate(self, test_set, retrieval_config):
        results = []
        for test_case in test_set:
            retrieved_docs = self.retrieve(test_case.question, retrieval_config)
            score = self.calculate_score(retrieved_docs, test_case.expected_sources)
            results.append(score)

        accuracy = sum(results) / len(results)
        mrr = self.calculate_mrr(results)
        ndcg = self.calculate_ndcg(results)

        return {
            "accuracy": accuracy,
            "mrr": mrr,
            "ndcg": ndcg
        }
```

---

## 驗收標準

### 功能驗收
- [ ] 所有 4 種檢索策略實現
- [ ] 參數可調整並即時生效
- [ ] 測試集完整建立
- [ ] A/B 測試框架可用

### 性能驗收
- [ ] 檢索準確率 ≥90%
- [ ] 檢索延遲 <500ms (P95)
- [ ] Re-ranking <200ms

### 安全驗收
- [ ] SQL 只讀權限驗證
- [ ] SQL Injection 零風險
- [ ] 查詢限流生效

---

## 相關文檔

- [Module 05: Agent Memory](../modules/module-05-agent-memory.md)
- [Epic 5.1: Conversation Memory](./epic-05.1-conversation-memory.md)
- [Architecture: Knowledge Service](../../architecture/components/knowledge-service.md)
- [ADR: Knowledge Retrieval Strategy](../../architecture/ADR-knowledge-retrieval.md)

---

**版本**: 1.0.0
**最後更新**: 2025-11-02
**狀態**: ✅ Epic 定義完成
