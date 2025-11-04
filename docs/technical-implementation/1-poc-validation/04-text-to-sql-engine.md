# PoC 4: Text-to-SQL Engine 驗證

**優先級**: 🔴 P0 (最高優先級)
**預計時間**: 3 days (Day 8-10)
**狀態**: ⏳ 待開始
**負責人**: Backend Tech Lead + Data Engineer

**基於**: Semantic Kernel + PostgreSQL 16 + SQL Parser (TECH-STACK-ANALYSIS v2.0.0)

---

## 🎯 驗證目標

驗證 Text-to-SQL 引擎的準確率與安全性，達到 **85%+ 準確率目標**：

1. ✅ **自然語言轉 SQL**: LLM 生成 SQL 查詢
2. ✅ **SQL Injection 防護**: 0 漏洞容忍
3. ✅ **Schema 語義增強**: 提高 SQL 準確率
4. ✅ **準確率評估**: 達到 85%+ 目標
5. ✅ **性能基準**: SQL 生成時間 <3 秒

---

## 📋 Text-to-SQL 架構

```
┌──────────────────────────────────────────┐
│      Text-to-SQL Engine                  │
│  ┌────────────────────────────────────┐  │
│  │  Schema Parser & Analyzer          │  │
│  │  - Table/Column Metadata           │  │
│  │  - Relationships (FK)              │  │
│  │  - Sample Data                     │  │
│  └────────────────────────────────────┘  │
│  ┌────────────────────────────────────┐  │
│  │  Semantic Enhancement              │  │
│  │  - Column Descriptions             │  │
│  │  - Business Terminology            │  │
│  │  - Few-Shot Examples               │  │
│  └────────────────────────────────────┘  │
│  ┌────────────────────────────────────┐  │
│  │  LLM SQL Generator                 │  │
│  │  - Prompt Engineering              │  │
│  │  - Semantic Kernel                 │  │
│  └────────────────────────────────────┘  │
│  ┌────────────────────────────────────┐  │
│  │  SQL Security Validator            │  │
│  │  - SQL Parser                      │  │
│  │  - Injection Detection             │  │
│  │  - Prepared Statements             │  │
│  └────────────────────────────────────┘  │
└──────────────────────────────────────────┘
```

---

## 🔧 環境準備

### 測試數據庫 Schema

```sql
-- Agents 表
CREATE TABLE agents (
    id UUID PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    user_id UUID NOT NULL,
    created_at TIMESTAMP DEFAULT NOW(),
    updated_at TIMESTAMP DEFAULT NOW()
);

-- Conversations 表
CREATE TABLE conversations (
    id UUID PRIMARY KEY,
    agent_id UUID NOT NULL REFERENCES agents(id),
    user_id UUID NOT NULL,
    title VARCHAR(255),
    created_at TIMESTAMP DEFAULT NOW()
);

-- Messages 表
CREATE TABLE messages (
    id UUID PRIMARY KEY,
    conversation_id UUID NOT NULL REFERENCES conversations(id),
    role VARCHAR(50) NOT NULL,  -- 'user' or 'assistant'
    content TEXT NOT NULL,
    created_at TIMESTAMP DEFAULT NOW()
);

-- 測試數據
INSERT INTO agents VALUES
('a1', 'CustomerBot', 'Customer service agent', 'u1', NOW(), NOW()),
('a2', 'TechBot', 'Technical support agent', 'u1', NOW(), NOW());

INSERT INTO conversations VALUES
('c1', 'a1', 'u1', 'Product Inquiry', NOW()),
('c2', 'a2', 'u1', 'Bug Report', NOW());

INSERT INTO messages VALUES
('m1', 'c1', 'user', 'What is your return policy?', NOW()),
('m2', 'c1', 'assistant', 'Our return policy allows...', NOW()),
('m3', 'c2', 'user', 'The app crashes on startup', NOW());
```

---

## 💻 實現步驟

### 步驟 1: Schema 語義增強

**Schema Metadata 定義**:

```csharp
public class SchemaMetadata
{
    public List<TableMetadata> Tables { get; set; } = new();
}

public class TableMetadata
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public List<ColumnMetadata> Columns { get; set; } = new();
    public List<string> ForeignKeys { get; set; } = new();
}

public class ColumnMetadata
{
    public string Name { get; set; } = "";
    public string DataType { get; set; } = "";
    public string Description { get; set; } = "";
    public List<string> SampleValues { get; set; } = new();
}
```

**語義增強示例**:

```json
{
  "tables": [
    {
      "name": "agents",
      "description": "AI agents created by users",
      "columns": [
        {
          "name": "name",
          "dataType": "VARCHAR(255)",
          "description": "Agent display name (e.g., 'CustomerBot', 'TechSupport')",
          "sampleValues": ["CustomerBot", "TechBot", "SalesAgent"]
        },
        {
          "name": "created_at",
          "dataType": "TIMESTAMP",
          "description": "When the agent was created"
        }
      ]
    }
  ]
}
```

### 步驟 2: LLM SQL Generator

```csharp
public class TextToSQLEngine
{
    private readonly Kernel _kernel;
    private readonly SchemaMetadata _schema;

    public async Task<string> GenerateSQLAsync(string naturalLanguageQuery)
    {
        // 構建 Prompt
        var prompt = BuildPrompt(naturalLanguageQuery);

        // LLM 生成 SQL
        var result = await _kernel.InvokePromptAsync(prompt);
        var sql = result.GetValue<string>()!;

        // 清理和驗證
        sql = CleanSQL(sql);
        ValidateSQL(sql);

        return sql;
    }

    private string BuildPrompt(string query)
    {
        return $@"
You are a SQL expert. Generate a PostgreSQL query for the following request.

**Database Schema**:
{SerializeSchema(_schema)}

**User Request**: {query}

**Rules**:
1. Use standard PostgreSQL syntax
2. Only query existing tables and columns
3. Use proper JOINs for relationships
4. Return only the SQL query, no explanations
5. Use parameterized queries (use $1, $2 for parameters)

**SQL Query**:
";
    }
}
```

### 步驟 3: SQL Injection 防護

```csharp
public class SQLSecurityValidator
{
    private readonly List<string> _dangerousKeywords = new()
    {
        "DROP", "DELETE", "TRUNCATE", "ALTER", "CREATE",
        "INSERT", "UPDATE", "EXEC", "EXECUTE", "--", "/*", "*/"
    };

    public void ValidateSQL(string sql)
    {
        // 1. 檢查危險關鍵字
        foreach (var keyword in _dangerousKeywords)
        {
            if (sql.ToUpper().Contains(keyword))
            {
                throw new SecurityException(
                    $"SQL contains dangerous keyword: {keyword}");
            }
        }

        // 2. 使用 SQL Parser 驗證語法
        var parser = new TSqlParser();
        var parsed = parser.Parse(sql);
        if (parsed.Errors.Any())
        {
            throw new InvalidOperationException("Invalid SQL syntax");
        }

        // 3. 驗證只包含 SELECT 語句
        if (!sql.Trim().ToUpper().StartsWith("SELECT"))
        {
            throw new SecurityException("Only SELECT queries are allowed");
        }
    }

    public string UseParameterizedQuery(string sql, Dictionary<string, object> parameters)
    {
        // 使用 Npgsql 的 Prepared Statement
        using var cmd = new NpgsqlCommand(sql, connection);
        foreach (var param in parameters)
        {
            cmd.Parameters.AddWithValue(param.Key, param.Value);
        }
        return cmd.CommandText;
    }
}
```

---

## 🧪 測試用例

### 測試套件 1: 簡單查詢

| 測試場景 | 自然語言 | 預期 SQL | 成功標準 |
|---------|---------|---------|----------|
| TC-1.1 | "List all agents" | `SELECT * FROM agents` | 準確 |
| TC-1.2 | "Show agents created by user u1" | `SELECT * FROM agents WHERE user_id = 'u1'` | 準確 |
| TC-1.3 | "Count total agents" | `SELECT COUNT(*) FROM agents` | 準確 |
| TC-1.4 | "Find agent named CustomerBot" | `SELECT * FROM agents WHERE name = 'CustomerBot'` | 準確 |

### 測試套件 2: JOIN 查詢

| 測試場景 | 自然語言 | 預期 SQL | 成功標準 |
|---------|---------|---------|----------|
| TC-2.1 | "Show conversations for agent CustomerBot" | `SELECT c.* FROM conversations c JOIN agents a ON c.agent_id = a.id WHERE a.name = 'CustomerBot'` | 準確 |
| TC-2.2 | "List messages in conversation c1" | `SELECT * FROM messages WHERE conversation_id = 'c1'` | 準確 |

### 測試套件 3: 聚合查詢

| 測試場景 | 自然語言 | 預期 SQL | 成功標準 |
|---------|---------|---------|----------|
| TC-3.1 | "How many messages per conversation?" | `SELECT conversation_id, COUNT(*) FROM messages GROUP BY conversation_id` | 準確 |
| TC-3.2 | "Average messages per agent" | Complex JOIN + GROUP BY | 準確 |

### 測試套件 4: SQL Injection 防護

| 測試場景 | 攻擊輸入 | 預期結果 | 成功標準 |
|---------|---------|----------|----------|
| TC-4.1 | "'; DROP TABLE agents; --" | 被阻擋 | 100% 阻擋 |
| TC-4.2 | "1' OR '1'='1" | 被阻擋 | 100% 阻擋 |
| TC-4.3 | "UNION SELECT * FROM users" | 被阻擋 | 100% 阻擋 |

---

## ✅ 成功標準驗證

### 1. 簡單查詢準確率 >80% ✅ / ❌

**測試結果**: 正確數 / 總數 = _____ / 20 = _____%

**結果**: ✅ / ❌

### 2. 0 SQL Injection 漏洞 ✅ / ❌

**測試結果**: 成功阻擋 / 測試數 = _____ / 10 = _____%

**結果**: ✅ / ❌

### 3. SQL 生成時間 <3 秒 ✅ / ❌

**平均時間**: _____ 秒

**結果**: ✅ / ❌

### 4. Schema 語義增強有效 ✅ / ❌

**改進率**: 無語義增強 ___% → 有語義增強 ___% = 提升 ___%

**結果**: ✅ / ❌

---

## 🔍 關鍵發現

### 準確率分析

1. **簡單查詢準確率**: _____%
2. **JOIN 查詢準確率**: _____%
3. **聚合查詢準確率**: _____%
4. **整體準確率**: _____%

### 錯誤分析

| 錯誤類型 | 數量 | 佔比 | 主要原因 |
|---------|------|------|---------|
| 語法錯誤 | ___ | __% | _______________ |
| 邏輯錯誤 | ___ | __% | _______________ |
| Schema 理解錯誤 | ___ | __% | _______________ |
| 其他 | ___ | __% | _______________ |

---

## ⚠️ 風險識別

| 風險 | 嚴重性 | 緩解方案 |
|------|--------|----------|
| 複雜查詢準確率低 | 高 | Few-Shot 範例增強 |
| SQL Injection 漏洞 | 極高 | 多層防護 + 審計 |
| Schema 變更適應性 | 中 | 自動 Schema 同步 |

---

## 💡 建議與下一步

- 建立測試 benchmark
- 實現 Query Cost Estimator
- 設計結果可視化 (圖表)

---

**最後更新**: 2025-10-30
**PoC 負責人**: Backend Tech Lead

[← 上一個 PoC](./03-code-interpreter-sandbox.md) | [下一個 PoC: Knowledge RAG →](./05-knowledge-rag-accuracy.md)
