# Sprint 10 概覽 - Text-to-SQL 基礎 (核心差異化功能)

**Sprint**: Sprint 10 | **週次**: Week 28-30 | **日期**: 2026-05-18 ~ 2026-06-07 (21 days) | **狀態**: ⏳ 未開始

## 🎯 Sprint 目標
實現**核心差異化能力 #4 - Text-to-SQL (第一部分)**,包含數據庫連接、Schema 理解和 SQL 生成。

## 📊 User Stories (13 SP)

| User Story | SP | 模組 | 優先級 |
|-----------|----|----- |-------|
| **US 5.5** - 數據庫連接 (Text-to-SQL) ⭐ | 13 | Module 5, 8 | P0 - 核心差異化 |

**注意**: US 5.5 包含 US 8.1 (Schema 理解與上下文構建) 的工作

## 📋 核心技術任務

### 數據庫連接管理
```yaml
支持數據庫:
  - PostgreSQL
  - SQL Server
  - MySQL

Connection String 管理:
  - 加密存儲
  - 連接池管理
  - 超時處理
```

### Schema 元數據提取 (US 8.1)
```yaml
提取內容:
  - Tables, Columns, Data Types
  - Primary Keys, Foreign Keys
  - Indexes, Constraints

上下文構建:
  - Schema Summary 生成
  - 表關係圖
  - Sample Data (前 5 行)
```

### Text-to-SQL 生成
```yaml
LLM Prompt Engineering:
  - Schema-aware Prompt
  - Few-shot Examples
  - SQL 語法驗證

實現邏輯:
  1. 用戶自然語言查詢
  2. Schema 上下文注入
  3. GPT-4 生成 SQL
  4. SQL 語法驗證
  5. 執行 SQL
  6. 返回結果
```

### API 端點
```yaml
POST /api/v1/databases/connect:
  - 創建數據庫連接

POST /api/v1/text-to-sql/query:
  Request:
    naturalLanguageQuery: string
    databaseId: Guid
  Response:
    generatedSql: string
    results: object[]
    rowCount: number
```

## 🎯 驗收標準

### 功能驗收
- ✅ 支持 3 種數據庫 (PostgreSQL, SQL Server, MySQL)
- ✅ Schema 元數據提取完整
- ✅ SQL 生成準確率 ≥ 85%

### 性能驗收
- ✅ Schema 提取 < 3 seconds
- ✅ SQL 生成 < 5 seconds
- ✅ 查詢執行 < 10 seconds

## 🎓 關鍵風險

### 風險 1: SQL 生成準確率不足
**影響**: 高 🔴
**緩解**: Few-shot Examples 優化, Schema Context 增強, SQL 驗證邏輯

### 風險 2: 多數據庫兼容性問題
**影響**: 中 🟡
**緩解**: 針對每個數據庫單獨測試, SQL 方言處理

## 🔗 依賴
- **依賴**: Sprint 9 (US 5.4 Excel/CSV 上傳)
- **後續**: Sprint 11 (US 8.2 安全 Text-to-SQL)

**狀態**: ⏳ 規劃中
